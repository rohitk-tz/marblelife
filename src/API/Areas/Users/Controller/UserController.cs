using System.Web.Http;
using Core.Application;
using Core.Users;
using Api.Areas.Application.Controller;
using Core.Users.ViewModels;
using Core.Application.ViewModel;
using Core.Users.Enum;
using System.Linq;

namespace Api.Areas.Users.Controller
{
    public class UserController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IUserService _userService;
        private readonly IUserLoginService _userLoginService;
        private readonly ISendLoginCredentialsService _sendLoginCredentialService;

        public UserController(ISessionContext sessionContext, IUserService userService, ISettings settings, IUserLoginService userLoginService, ISendLoginCredentialsService sendLoginCredentialService)
        {
            _sessionContext = sessionContext;
            _userService = userService;
            _userLoginService = userLoginService;
            _sendLoginCredentialService = sendLoginCredentialService;
        }

        [HttpGet]
        public UserEditModel Get(long id,long franchiseeId=default(long))
        {
            return _userService.Get(id,franchiseeId);
        }

        [HttpPost]
        public bool Post([FromBody]UserEditModel model)
        {
            var isUnique = IsUniqueUserName(0,model.UserLoginEditModel.UserName);//dervice call

            if(isUnique == false)
            {
                ResponseModel.SetErrorMessage("UserName not Unique");
                ResponseModel.ErrorCode = 0;
                return false;
            }
            if (model.RoleId == (long)RoleType.SuperAdmin || model.IsExecutive)
            {
                model.OrganizationId = 1; //To do replace with default franchisor id
            }
            if (model.OrganizationId < 1)
                model.OrganizationId = _sessionContext.UserSession.OrganizationId;

            if (model.IsExecutive)
            {
                model.RoleId = (long)RoleType.FrontOfficeExecutive;
            }

            if (!model.IsExecutive && model.RoleIds.Any(x => x == (long)RoleType.OperationsManager))
            {
                var result = _userService.VerifyOpsMgrRole(model);
                if (!result)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Can't create user, please Select Tech Role too for creating Operations Manager Role!");
                    return false;
                }
            }

            model.CreateLogin = true;
            _userService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User created successfully.");
            return true;
        }

        [HttpPost]
        public bool Lock(long id, [FromBody]bool isLocked)
        {
            bool lockResult;
            bool isEquipment;
            var result = _userLoginService.Lock(id, isLocked, out lockResult,out isEquipment);
            if (result)
            {
                if (lockResult)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User account is temporarily locked.");
                    return false;
                }
                else
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User account has been Unlocked.");
                    return true;
                }
            }
            else
            {
                 if (isEquipment)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Cannot Lock account for Equipment.");
                    return false;
                }
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Can't Lock User Account as user is scheduled for future jobs.");
                return false;
            }
        }

        [HttpPut]
        public bool Put(long id, [FromBody]UserEditModel model)
        {
            var isUnique = IsUniqueUserName(0, model.UserLoginEditModel.UserName);//dervice call

            if (isUnique == false && model.IsChanged)
            {
                ResponseModel.SetErrorMessage("UserName not Unique");
                ResponseModel.ErrorCode = 0;
                return false;
            }
            if (model.OrganizationId < 1)
                model.OrganizationId = _sessionContext.UserSession.OrganizationId;

            if (model.IsExecutive)
            {
                model.RoleId = (long)RoleType.FrontOfficeExecutive;
            }

            if (!model.IsExecutive && model.RoleIds.Any(x => x == (long)RoleType.OperationsManager))
            {
                var response = _userService.VerifyOpsMgrRole(model);
                if (!response)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Can't Update user, please Select Tech Role too for creating Operations Manager Role!");
                    return false;
                }
            }

            model.CreateLogin = true;
            var result = _userService.CheckSchedulerAssignment(model);
            if (result)
            {
                _userService.Save(model);
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User Updated successfully.");
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to Remove User Role, as Job(s)/estimate(s) are associated with it!");
            return result;
        }

        [HttpDelete]
        public bool Delete(long userId = 0)
        {
            _userService.Delete(userId);
            return true;
        }

        [HttpGet]
        public UserListModel Get([FromUri]UserListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive && filter.FranchiseeId <= 0)
                filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;

            filter.IsFrontOfficeExecutive = (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive);
            return _userService.GetUsers(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public bool IsUniqueEmail(int id, string email)
        {
            return _userLoginService.IsUniqueEmailAddress(email, id);
        }

        [HttpGet]
        public bool IsUniqueUserName(int id, string userName)
        {
            return _userLoginService.IsUniqueUserName(userName, id);
        }

        [HttpPost]
        public bool ManageAccount(long userId, [FromBody]long[] franchiseeIds)
        {
            _userService.ManageAccount(userId, franchiseeIds);
            return true;
        }
        [HttpGet]
        public string GetImageUrl()
        {
            return _userService.GetImageUrl();
        }

        [HttpPost]
        public bool PostForEquipment([FromBody] UserEquipmentEditModel model)
        {
            model.CreateLogin = false;
            _userService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User created successfully.");
            return true;
        }

        [HttpPost]
        public bool UserEdit(long id, [FromBody] UserEquipmentEditModel model)
        {

            if (model.OrganizationId < 1)
                model.OrganizationId = _sessionContext.UserSession.OrganizationId;

            model.CreateLogin = false;
                _userService.Save(model);
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User Updated successfully.");
            return true;
        }

        [HttpPost]
        public bool SchedulerDefaultView([FromUri] string deafaultView)
        {

              var orgId= _sessionContext.UserSession.OrganizationRoleUserId;

            _userService.SchedulerDefaultView(orgId, deafaultView);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("User Updated successfully.");
            return true;
        }
        

        [HttpPost]
        public string GetDefaultView()
        {
            var orgId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _userService.GetDefaultView(orgId); ;
        }
        
        [HttpPost]
        public UserSignatureListEditModel GetUserSignature()
        {
            var orgId = _sessionContext.UserSession.OrganizationRoleUserId;
            var userId = _sessionContext.UserSession.UserId;
            return _userService.GetUserSignature(orgId, userId); ;
        }
        [HttpPost]
        public bool SaveUserSignature(UserSignatureListSaveModel model)
        {
            var orgId = _sessionContext.UserSession.OrganizationRoleUserId;
            var userId = _sessionContext.UserSession.UserId;
            return _userService.SaveUserSignature(model, orgId, userId); ;
        }

    }
}
