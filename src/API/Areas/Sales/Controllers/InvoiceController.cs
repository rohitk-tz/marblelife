using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.ViewModel;
using Core.Users.Enum;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class InvoiceController : BaseController
    {
        private readonly IInvoiceService _invoiceService;
        private ISessionContext _sessionContext;

        public InvoiceController(IInvoiceService invoiceService, ISessionContext sessionContext)
        {
            _invoiceService = invoiceService;
            _sessionContext = sessionContext;
        }

        [HttpGet]
        public InvoiceListModel Get([FromUri] InvoiceListFilter filter, [FromUri] int pageNumber, [FromUri] int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _invoiceService.GetInvoiceList(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public InvoiceDetailsViewModel Get([FromUri] long invoiceId)
        {
            return _invoiceService.FranchiseeInvoiceDetails(invoiceId);
        }

        [HttpPost]
        public HttpResponseMessage DownloadAdfundInvoiceFile([FromBody] long[] invoiceIds)
        {
            string fileName;
            var resultAdfund = _invoiceService.CreateExcelAdfund(invoiceIds, out fileName);

            if (resultAdfund)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();
                responseStream.Position = 0;
                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("Text/vnd.ms-excel");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Adfund.csv",
                };
                return response;
            }
            else
                return null;

        }
        [HttpPost]
        public HttpResponseMessage DownloadRoyalityInvoiceFile([FromBody] long[] invoiceIds)
        {
            string fileName;
            var resultRoyality = _invoiceService.CreateExcelRoyality(invoiceIds, out fileName);
            if (resultRoyality)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();
                responseStream.Position = 0;
                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                            new MediaTypeHeaderValue("Text/vnd.ms-excel");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Royalty.csv",
                };
                return response;
            }
            else
                return null;

        }

        [HttpPost]
        public HttpResponseMessage DownloadInvoiceListFile([FromBody] long[] invoiceIds)
        {
            string fileName;
            var result = _invoiceService.DownloadInvoiceListFile(invoiceIds, out fileName);
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
                    FileName = "Invoice.xlsx",
                };

                return response;
            }
            else return null;

        }

        [HttpPost]
        public HttpResponseMessage DownloadAllInvoiceFile([FromBody] InvoiceListFilter filter)
        {
            string fileName;
            var result = _invoiceService.CreateExcelForAllFiles(filter, out fileName);
            if (result)
            {
                //_invoiceService.MarkInvoicesAsDownloaded(invoiceIds);
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
        public HttpResponseMessage DownloadInvoiceListAllFile([FromBody] InvoiceListFilter filter)
        {
            string fileName;
            if (filter.Sort.Order != null)
            {
                filter.SortingOrder = filter.Sort.Order;
            }
            if (filter.Sort.PropName != "" || filter.Sort.PropName != null)
            {
                filter.SortingColumn = filter.Sort.PropName;
            }
            var result = _invoiceService.DownloadInvoiceListAllFile(filter, out fileName);
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
                    FileName = "Invoice.xlsx",
                };

                return response;
            }
            else return null;

        }

        [HttpGet]
        public DeleteInvoiceResponseModel DeleteInvoiceItem([FromUri] long invoiceItemId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                return new DeleteInvoiceResponseModel
                {
                    IsLastItem = false,
                    IsSuccess = false,
                    Response = "Can't delete Invoice item."
                };
            }
            var result = _invoiceService.DeleteInvoiceItem(invoiceItemId);
            return result;
        }

        [HttpPost]
        public InvoiceListModel GetDownloadedInvoiceList([FromBody] long[] invoiceIds)
        {
            return _invoiceService.GetDownloadedInvoiceList(invoiceIds);
        }

        [HttpPost]
        public bool MarkAsDownloaded([FromBody] long[] invoiceIds)
        {
            return _invoiceService.MarkInvoicesAsDownloaded(invoiceIds);
        }

        [HttpPost]
        public bool SaveReconciliationNotes([FromBody] InvoiceReconciliationNotesModel model)
        {
            return _invoiceService.SaveInvoiceReconciliationNotes(model);
        }
    }
}