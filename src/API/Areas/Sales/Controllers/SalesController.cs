using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.ViewModel;
using Core.Organizations;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Scheduler.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class SalesController : BaseController
    {
        private IFranchiseeSalesService _franchiseeSalesService;
        private IInvoiceService _invoiceService;
        private ISessionContext _sessionContext;
        private IRoyaltyReportService _royaltyReportService;

        public SalesController(IFranchiseeSalesService franchiseeSalesService, ISessionContext sessionContext, IInvoiceService invoiceService,
            IRoyaltyReportService royaltyReportService)
        {
            _franchiseeSalesService = franchiseeSalesService;
            _sessionContext = sessionContext;
            _invoiceService = invoiceService;
            _royaltyReportService = royaltyReportService;
        }

        [HttpGet]
        public SalesDataListViewModel Get([FromUri]SalesDataListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _franchiseeSalesService.GetSalesData(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public InvoiceDetailsViewModel Get([FromUri]long invoiceId)
        {
            return _invoiceService.InvoiceDetails(invoiceId);
        }

        [HttpGet]
        public RoyaltyReportListModel GetRoyaltyReport([FromUri]long salesDataUploadId)
        {
            return _royaltyReportService.GetRoyaltyReport(salesDataUploadId);
        }

        [HttpPost]
        public HttpResponseMessage DownloadSalesDataFile([FromBody]SalesDataListFilter filter)
        {
            string fileName;
            var result = _franchiseeSalesService.DownloadSalesDataFile(filter, out fileName);
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
                    FileName = "SalesData.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public UpdateMarketingClassListModel UpdateSalesData([FromBody] UpdateMarketingClassInfoListFilter filter)
        {
            filter.IsFromDownload = false;
            return _franchiseeSalesService.UpdateSalesData(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadInvoiceFile([FromBody] UpdateMarketingClassInfoListFilter filter)
        {
            string fileName;
            filter.IsFromDownload = true;
            var result = _franchiseeSalesService.DownloadInvoiceAllList(filter, out fileName);
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
                    FileName = "QB_Invoice_Payment.xlsx",
                };

                return response;
            }
            else return null;

        }


        [HttpPost]
        public bool Upload([FromBody] CustomerFileUploadCreateModel filter)
        {
            _franchiseeSalesService.SaveFile(filter);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("File has been uploaded successfully, data will be updated shortly.");
            return true;
        }

        [HttpGet]
        public ZipDataUploadListModel GetInvoiceParseList([FromUri] ZipDataListFilter filter, [FromUri] int pageNumber, [FromUri] int pageSize)
        {
          var list=  _franchiseeSalesService.GetUpdateSalesList(filter,pageNumber,pageSize);
            return list;
        }
    }
}