using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class AnnualBatchController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IAnnualSalesDataUploadService _annualSalesDataUploadService;
        public AnnualBatchController(ISessionContext sessionContext, IAnnualSalesDataUploadService annualSalesDataUploadService)
        {
            _annualSalesDataUploadService = annualSalesDataUploadService;
            _sessionContext = sessionContext;
        }

        public SalesDataUploadListModel Get([FromUri]SalesDataListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _annualSalesDataUploadService.GetBatchList(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public AnnualAuditSalesListModel GetAnnualSales([FromBody]SalesDataListFilter query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _annualSalesDataUploadService.GetAnnualSalesData(query);
        }

        [HttpPost]
        public AuditInvoiceViewModel GetInvoiceDetails(long invoiceId, long auditInvoiceId)
        {
            return _annualSalesDataUploadService.InvoiceDetails(invoiceId, auditInvoiceId);
        }

        [HttpDelete]
        public bool Delete(long id)
        {
            var res = _annualSalesDataUploadService.Delete(id);
            if (!res)
                ResponseModel.SetErrorMessage("The record can not be deleted.");
            return res;
        }

        [HttpPost]
        public bool ManageBatch(bool isAccept, long batchId)
        {
            var res = _annualSalesDataUploadService.ManageBatch(isAccept, batchId);
            return true;
        }

        [HttpPost]
        public AnnualAuditSalesListModel GetAnnualAuditRecord(SalesDataListFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _annualSalesDataUploadService.GetAnnualAuditRecord(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadAnnualDataFile([FromBody]SalesDataListFilter filter)
        {
            string fileName;
            var result = _annualSalesDataUploadService.DownloadAnnualDataFileFormatted(filter, out fileName);
            //var result = _annualSalesDataUploadService.DownloadAnnualDataFile(filter, out fileName);
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
                    FileName = "AnnualData.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool UploadAnnualFile([FromBody]AnnualDataUploadCreateModel model)
        {

            var isValid = _annualSalesDataUploadService.isValidUpload(model);
            if (isValid)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Annual Data Already Uploaded!");
                return false;
            }
            var result = _annualSalesDataUploadService.SaveUpload(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("File saved successfully!");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to sale Annual Upload!");
                return false;
            }
        }
        [HttpPost]
        public AnnualAuditSalesListModel GetAnnualSalesAddress([FromBody]SalesDataListFilter query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _annualSalesDataUploadService.GetAnnualSalesDataAddress(query);
        }
        [HttpPost]
        public AnnualSalesDataCustonerListModel GetAnnualCustomerAddress([FromBody]AnnualSalesDataListFiltercs query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _annualSalesDataUploadService.GetAnnualSalesCustomerAddress(query, query.PageNumber, query.PageSize);
        }
        [HttpPost]
        public bool UpdateCustomerAddress([FromBody]AnnualSalesDataCustomerViewModel query)
        {
            var isUpdated = _annualSalesDataUploadService.UpdateCustomerAddress(query);
            if (isUpdated)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Value updated successfully!");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to Update Customer Address!");
                return false;
            }
        }

        [HttpPost]
        public bool ReparseAnnualAudit(long? id)
        {
            var isUpdated = _annualSalesDataUploadService.ReparseAnnualReport(id);
            if (isUpdated)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Value updated successfully!");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to Update Customer Address!");
                return false;
            }
            return false;
        }
    }
}