using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Organizations;
using Core.Organizations.ViewModel;
using Core.Users.Enum;
using System.Linq;
using System.Web.Http;
using Core.Organizations.Enum;
namespace API.Areas.Organizations.Controllers
{
    public class FranchiseeDocumentController : BaseController
    {
        private ISessionContext _sessionContext;
        private readonly IFranchiseeDocumentService _franchiseeDocumentSevice;

        public FranchiseeDocumentController(ISessionContext sessionContext, IFranchiseeDocumentService franchiseeDocumentSevice)
        {
            _sessionContext = sessionContext;
            _franchiseeDocumentSevice = franchiseeDocumentSevice;
        }

        [HttpPost]
        public bool UploadDocuments([FromBody]FranchiseeDocumentEditModel model)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeIds.Add(_sessionContext.UserSession.OrganizationId);
            }
            if (model.FileModel == null && model.Id == 0 && !model.IsRejected)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Please attach a file to upload!");
                return false;
            }
            if (model.FranchiseeIds == null || model.FranchiseeIds.Count() <= 0)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Please select a Franchisee First.");
                return false;
            }
            //if (model.Id == 0 && model.DocumentTypeId!=(long)DocumentType.NCA && model.DocumentTypeId != (long)DocumentType.UPLOADTAXES && model.DocumentTypeId != (long)DocumentType.AnnualTaxFilling)
            //{
            //    string isValid = _franchiseeDocumentSevice.IsExpiryValid(model); 
            //    if (isValid != "")
            //    {
            //        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Please Choose Expiry Date after " + isValid);
            //        return false;
            //    }
            //}
            var result = _franchiseeDocumentSevice.SaveDocuments(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Document uploaded successfully!");
            }
            return result;
        }

        [HttpGet]
        public DocumentListModel Get([FromUri]DocumentListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            filter.isSaleTech = false;
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FranchiseeAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {

                filter.ShowToUser = true;
                filter.isSaleTech = true;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.GetValueOrDefault();
            }
            filter.loggedinUser = _sessionContext.UserSession.UserId;
            return _franchiseeDocumentSevice.GetFranchiseeDocument(filter, pageNumber, pageSize);
        }

        [HttpDelete]
        public bool Delete([FromUri]long id)
        {
            var result = _franchiseeDocumentSevice.Delete(id);
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Document deleted successfully!");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Document!");
            return false;
        }

        [HttpGet]
        public DocumentViewModel GetById([FromUri]long? id)
        {
            return _franchiseeDocumentSevice.GetFranchiseeInfoById(id);
        }
    }
}