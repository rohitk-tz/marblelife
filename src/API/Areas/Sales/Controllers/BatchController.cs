using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class BatchController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly ISalesDataUploadService _salesDataUploadService;
        private readonly ISalesDataUploadCreateModelValidator _salesDataUploadCreateModelValidator;
        public BatchController(ISessionContext sessionContext, ISalesDataUploadService salesDataUploadService
            , ISalesDataUploadCreateModelValidator salesDataUploadCreateModelValidator)
        {
            _salesDataUploadService = salesDataUploadService;
            _sessionContext = sessionContext;
            _salesDataUploadCreateModelValidator = salesDataUploadCreateModelValidator;
        }

        [HttpPost]
        public bool Upload([FromBody]SalesDataUploadCreateModel model)
        {
            var isValid = _salesDataUploadCreateModelValidator.ValidateDates(model);
            bool isAnnualValid = true;
            if (!isValid)
            {
                PostResponseModel.Message = model.Message;
                return false;
            }
            //if (model.IsAnnualUpload && model.AnnualFile != null)
            //{
            //    isAnnualValid = _salesDataUploadService.isValidUpload(model);
            //}
            if (!isAnnualValid)
            {
                PostResponseModel.Message = model.Message;
                return false;
            }
            var isValidRange = _salesDataUploadService.CheckValidRangeForSalesUpload(model);
            if (!isValidRange)
            {
                PostResponseModel.Message = model.Message;
                return false;
            }
            var isValidDocument = _salesDataUploadService.CheckForExpiringDocument(model);
            if (!isValidDocument)
            {
                PostResponseModel.Message = model.Message;
                return false;
            }
            

            _salesDataUploadService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("File saved successfully.");
            return true;
        }

        [HttpPost]
        public bool UploadCustomerList([FromBody]SalesDataUploadCreateModel model)
        {
            _salesDataUploadService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("File saved successfully.");
            return true;
        }

        [HttpPost]
        public bool Update([FromBody]SalesDataUploadCreateModel model)
        {
            if (_sessionContext.UserSession.RoleId == (long)RoleType.SuperAdmin)
            {
                _salesDataUploadService.Update(model);
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("File saved successfully.");
            }
            return true;
        }

        [HttpGet]
        public SalesDataUploadListModel Get([FromUri]SalesDataListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _salesDataUploadService.GetBatchList(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public DateTime? GetLastUploadedBatch([FromUri] long franchiseeId)
        {
            return _salesDataUploadService.GetLastUploadedBatch(franchiseeId);
        }

        [HttpDelete]
        public bool Delete(long id)
        {
            var res = _salesDataUploadService.Delete(id);
            if (!res)
                ResponseModel.SetErrorMessage("The record can not be deleted.");

            return res;
        }

        [HttpGet]
        public bool Reparse(long id)
        {
            var res = _salesDataUploadService.Reparse(id);
            if (!res)
                ResponseModel.SetErrorMessage("The record can not be re-parsed.");
            else ResponseModel.SetSuccessMessage("The Record will be re-parsed");

            return res;
        }

        [HttpPost]
        public AnnualUploadValidationModel GetAnnualUplodInfo([FromBody]AnnualUploadValidationModel model)
        {
            return _salesDataUploadService.GetAnnualUploadInfo(model);
        }
    }
}