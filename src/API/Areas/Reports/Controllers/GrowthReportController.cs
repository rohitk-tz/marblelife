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
    public class GrowthReportController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IGrowthReportService _growthReportService;

        public GrowthReportController(ISessionContext sessionContext, IGrowthReportService growthReportService)
        {
            _sessionContext = sessionContext;
            _growthReportService = growthReportService;
        }

        [HttpGet]
        public GrowthReportListModel Get([FromUri]GrowthReportFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _growthReportService.GetGrowthReport(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public HttpResponseMessage DownloadGrowthReport([FromBody]GrowthReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }

            string fileName;
            var result = _growthReportService.DownloadGrowthReport(filter, out fileName);
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
                    FileName = "GrowthReport.xlsx",
                };
                return response;
            }
            else return null;
        }

    }
}