using Api.Areas.Application.Controller;
using Core.Application;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    public class UploadBatchReportController : BaseController
    {
        private ISessionContext _sessionContext;
        private IReportService _reportService;

        public UploadBatchReportController(ISessionContext sessionContext,IReportService reportService)
        {
            _sessionContext = sessionContext;
            _reportService = reportService;
        }

        [HttpGet]
        public UploadBatchReportListModel Get([FromUri]UploadReportFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _reportService.GetBatchReport(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public HttpResponseMessage DownloadUploadReport([FromBody]UploadReportFilter filter)
        {
            string fileName;
            var result = _reportService.DownloadUploadReport(filter, out fileName); 
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
                    FileName = "MissingUploadReport.xlsx",
                };
                return response;
            }
            else return null;
        }
    }
}