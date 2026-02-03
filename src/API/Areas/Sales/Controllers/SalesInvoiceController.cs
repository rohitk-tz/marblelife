using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Organizations;
using Core.Sales;
using Core.Sales.ViewModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class SalesInvoiceController : BaseController
    {
        private ISalesInvoiceService _salesInvoiceService;
        private ISessionContext _sessionContext;


        public SalesInvoiceController(ISalesInvoiceService salesInvoiceService, ISessionContext sessionContext)
        {
            _salesInvoiceService = salesInvoiceService;
            _sessionContext = sessionContext;
        }

        [HttpPost]
        public HttpResponseMessage Download([FromBody]SalesDataListFilter filter) 
        {
            string fileName;
            var result = _salesInvoiceService.DownloadInvoiceFile(filter, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;
                const string name = "Sales_InvoiceData.xlsx";

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name,
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool Upload([FromBody]CustomerFileUploadCreateModel model)
        {
            _salesInvoiceService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("The list has been uploaded successfully, data will be updated shortly.");
            return true;
        }
    }
}