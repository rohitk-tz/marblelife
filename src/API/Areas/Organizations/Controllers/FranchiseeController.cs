using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Organizations;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using Core.Users.Enum;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using static Core.Organizations.Impl.FranchiseeInfoService;

namespace API.Areas.Organizations.Controllers
{
    [AllowAnonymous]
    public class FranchiseeController : BaseController
    {
        private readonly IFranchiseeInfoService _franchiseeService;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoServiceService;
        private readonly IFranchiseeServiceFeeService _franchiseeServiceFeeService;
        private ISessionContext _sessionContext;

        public FranchiseeController(IFranchiseeInfoService franchiseeService, ISessionContext sessionContext,
            IOrganizationRoleUserInfoService organizationRoleUserInfoServiceService, IFranchiseeServiceFeeService franchiseeServiceFeeService)
        {
            _franchiseeService = franchiseeService;
            _sessionContext = sessionContext;
            _organizationRoleUserInfoServiceService = organizationRoleUserInfoServiceService;
            _franchiseeServiceFeeService = franchiseeServiceFeeService;
        }


        [HttpGet]
        public FranchiseeEditModel Get(long id)
        {
            var model = _franchiseeService.Get(id);
            return model;
        }

        [HttpPost]
        public bool Post([FromBody] FranchiseeEditModel model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            _franchiseeService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Organization saved successfully.");
            return true;
        }

        [HttpGet]
        public bool DeleteFranchisee([FromUri] long id)
        {
            var result = _franchiseeService.Delete(id);
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Franchisee deleted successfully.");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Franchisee, as SalesData is uploaded");
            return false;
        }

        [HttpGet]
        public FranchiseeListModel Get([FromUri] FranchiseeListFilter filter, [FromUri] int pageNumber, [FromUri] int pageSize)
        {
            if (filter.FranchiseeStatus != null)
            {
                filter.status = filter.FranchiseeStatus == 1 ? true : false;
            }
            filter.LoggedInRoleId = _sessionContext.UserSession.RoleId;
            filter.LoggedInUserId = _sessionContext.UserSession.UserId;
            return _franchiseeService.GetFranchiseeCollection(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public FeeProfileViewModel GetFeeProfile([FromUri] long id)
        {
            return _franchiseeService.GetFranchiseeFeeProfile(id);
        }
        [HttpGet]
        public FranchiseeInfoListModel GetFranchiseeListForLogin([FromUri] long userId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                userId = _sessionContext.UserSession.UserId;
            }
            return _organizationRoleUserInfoServiceService.GetFranchiseeListForLogin(userId, _sessionContext.UserSession.RoleId);
        }
        [HttpPost]
        public bool ManageFranchisee([FromBody] ManageFranchiseeAccountModel model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            bool result = false;

            if (_sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin)
            {
                model.CurrentFranchiseeId = model.CurrentFranchiseeId <= 1 ? _sessionContext.UserSession.OrganizationId : model.CurrentFranchiseeId;
                result = _organizationRoleUserInfoServiceService.ManageFranchisee(model);
            }
            else if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                model.CurrentFranchiseeId = model.CurrentFranchiseeId <= 1 ? _sessionContext.UserSession.LoggedInOrganizationId.Value : model.CurrentFranchiseeId;
                result = _organizationRoleUserInfoServiceService.ManageFrontOfficeFranchisee(model);
            }
            else if (_sessionContext.UserSession.RoleId == (long)RoleType.SuperAdmin)
            {
                model.CurrentFranchiseeId = model.CurrentFranchiseeId <= 1 ? _sessionContext.UserSession.LoggedInOrganizationId.Value : model.CurrentFranchiseeId;
                result = _organizationRoleUserInfoServiceService.ManageFranchisee(model);
            }
            return result;
        }

        [HttpGet]
        public FranchiseeInfoListModel GetFranchiseeInfoCollection(long userId)
        {
            return _organizationRoleUserInfoServiceService.GetFranchiseeInfoCollection(userId);
        }

        [HttpGet]
        public IEnumerable<FranchiseeNotesViewModel> GetFranchiseeNotes(long franchiseeId)
        {
            return _franchiseeService.GetFranchiseeNotes(franchiseeId);
        }
        [HttpPost]
        public bool DeactivateFranchisee([FromBody] DeactivateFranchisee model)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                return false;
            }
            var result = _franchiseeService.DeactivateFranchisee(model.FranchiseeId, model.DeactivateNote);
            return result;
        }
        [HttpGet]
        public bool ActivateFranchisee([FromUri] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                return false;
            }
            var result = _franchiseeService.ActivateFranchisee(franchiseeId);
            return result;
        }

        [HttpGet]
        public bool IsUniqueBusinessId(int id, long? businessId)
        {
            return _franchiseeService.IsUniqueBusinessId(businessId, id);
        }

        [HttpGet]
        public bool GetGeoCode([FromUri] long franchiseeId)
        {
            return _franchiseeService.GetGeoCode(franchiseeId);
        }

        [HttpGet]
        public DeleteInvoiceResponseModel DeleteFee([FromUri] long id, [FromUri] long typeId)
        {
            var result = _franchiseeServiceFeeService.Delete(id, typeId);
            return result;
        }

        [HttpGet]
        public FranchiseeServiceFeeListsModel GetOneTimeProjectFee([FromUri] long id)
        {
            var result = _franchiseeServiceFeeService.GetOneTimeFeeList(id);
            return result;
        }

        [HttpPost]
        public bool SaveServiceFee([FromBody] FranchiseeServiceFeeEditModel model)
        {
            _franchiseeServiceFeeService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("fee saved successfully.");
            return true;
        }

        public FranchiseeServiceFeeListsModel GetLoanList([FromUri] long id)
        {
            var result = _franchiseeServiceFeeService.GetLoanList(id);
            return result;
        }
        [HttpPost]
        public bool SaveChangeServiceFee([FromBody] FranchiseeChangeServiceFee model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            _franchiseeServiceFeeService.SaveLoanAdjustmentType(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("fee saved successfully.");
            return true;
        }

        [HttpPost]
        public bool GetFranchiseeRoyality([FromUri] long franchiseeId)
        {
            return _franchiseeServiceFeeService.GetFranchiseeRoyality(franchiseeId);
        }
        [HttpGet]
        public FranchiseeTeamImageViewModel GetFranchiseeTeamImage([FromUri] long franchiseeId)
        {
            return _franchiseeServiceFeeService.GetFranchiseeTeamImage(franchiseeId);
        }

        [HttpPost]
        public bool SaveFranchiseeTeamImage([FromBody] FranchiseeTeamImageEditModel franchiseeTeamImage)
        {
            return _franchiseeServiceFeeService.SaveFranchiseeTeamImage(franchiseeTeamImage);
        }

        [HttpPost]
        public HttpResponseMessage DownloadFranchiseeLoan([FromBody] long? franchiseeLoanId)
        {
            string fileName;
            var result = _franchiseeServiceFeeService.DownloadFranchiseeLoan(franchiseeLoanId, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "FranchiseeLoan.xlsx",
                };
                return response;
            }
            else return null;
        }
        [HttpPost]
        public HttpResponseMessage DownloadFranchisee([FromUri] FranchiseeListFilter filter)
        {
            string fileName;
            var result = _franchiseeService.DownloadFranchisee(filter, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "FranchiseeLoan.xlsx",
                };
                return response;
            }
            else return null;
        }


        [HttpGet]
        public HttpResponseMessage DownloadFranchiseeDirectory([FromUri] FranchiseeListFilter filter)
        {
            string fileName;
            var result = _franchiseeService.DownloadFranchiseeDirectory(filter, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "FranchiseeLoan.xlsx",
                };
                return response;
            }
            else return null;
        }


        [HttpGet]
        public FranchiseeResignListModel GetFranchiseeList([FromUri] FranchiseeListFilter filter)
        {
            if (filter.FranchiseeStatus != null)
            {
                filter.status = filter.FranchiseeStatus == 1 ? true : false;
            }
            filter.RoleId = _sessionContext.UserSession.RoleId;
            filter.LoggedInRoleId = _sessionContext.UserSession.RoleId;
            filter.LoggedInUserId = _sessionContext.UserSession.UserId;
            return _franchiseeService.GetFranchiseeResignList(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadFileFranchiseeDirectory([FromUri] FranchiseeListFilter filter)
        {
            string fileName;
            if (filter.FranchiseeStatus != null)
            {
                filter.status = filter.FranchiseeStatus == 1 ? true : false;
            }
            var result = _franchiseeService.DownloadFileFranchiseeDirectoryRedesign(filter, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "FranchiseeLoan.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpGet]
        public string GetFranchiseeDeactivationNote(long franchiseeId)
        {
            return _franchiseeService.GetFranchiseeDeactivationNote(franchiseeId);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetFranchiseeRPID(long? franchiseeId)
        {
            return _franchiseeService.GetFranchiseeRPID(franchiseeId);
        }

        [HttpPost]
        public bool OneTimeProjectChangeStatus([FromBody] OneTimeProjectFilter filter)
        {
            return _franchiseeService.OneTimeProjectChangeStatus(filter);
        }

        [HttpPost]
        public FranchiseeDocumentListModel GetFranchiseeDocumentReport([FromBody] FranchiseeDocumentFilter filter)
        {
            return _franchiseeService.GetFranchiseeDocumentReport(filter);
        }

        [HttpPost]
        public IEnumerable<DropdownListItem> GetFranchiseeDocumentList()
        {
            return _franchiseeService.GetFranchiseeDocumentList();
        }

        [HttpPost]
        public bool SavePrePayLoan([FromBody] FranchiseePrePayLoanFeeEditModel model)
        {
            _franchiseeServiceFeeService.SavePrePayLoan(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Loan PrePaid successfully. Changes will Reflect after sometime");
            return true;
        }
        [HttpPost]
        public bool SaveFranchisseeNotes([FromBody] FranchiseeNotesDurationViewModel model)
        {
            //model.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.GetValueOrDefault();
            model.UserId = _sessionContext.UserSession.UserId;
            model.RoleId = _sessionContext.UserSession.RoleId;
            var isSaved = _franchiseeServiceFeeService.SaveFranchisseeNotes(model);

            if (isSaved)
            {
                if (model.RoleId == ((long?)RoleType.SuperAdmin) || model.RoleId == ((long?)RoleType.FrontOfficeExecutive))
                {
                    if (model.Duration != default(long))
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Duration Added Successfully.");
                    }
                    else
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Notes Added Successfully.");
                    }
                }
                else
                {
                    if (model.Duration != default(long))
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Duration Will be Approved by Super Admin / Front Office.");
                    }
                    else
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Notes Added Successfully.");
                    }
                }
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error in Saving Notes.");
            }
            return true;
        }

        public DurationApprovalListModel GetDurationApprovalList(long franchiseeId)
        {
            var roleId = _sessionContext.UserSession.RoleId;
            return _franchiseeServiceFeeService.GetDurationApprovalList(franchiseeId, roleId);
        }
        public bool ChangeDurationStatus(DurationApprovalFilterModel model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            var result = _franchiseeServiceFeeService.ChangeDurationStatus(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Status Changed Successfully.");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error in Changing Status.");
                return false;
            }
        }
        public HttpResponseMessage DownloadTaxReport([FromBody] FranchiseeDocumentFilter filter)
        {
            string fileName;
            var result = _franchiseeService.DownloadTaxReport(filter, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
               response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "FranchiseeLoan.xlsx",
                };
                return response;
            }
            else return null;
        }
        [HttpPost]
        public bool SEOChargesChangeStatus([FromBody] OneTimeProjectFilter filter)
        {
            return _franchiseeService.SEOChargesChangeStatus(filter);
        }
    }



}
