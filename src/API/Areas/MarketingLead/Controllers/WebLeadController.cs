using Api.Areas.Application.Controller;
using Core.Application;
using Core.MarketingLead;
using Core.MarketingLead.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.MarketingLead.Controllers
{
    [AllowAnonymous]
    public class WebLeadController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IMarketingLeadsReportService _marketingLeadService;

        public WebLeadController(ISessionContext sessionContext, IMarketingLeadsReportService marketingLeadService)
        {
            _sessionContext = sessionContext;
            _marketingLeadService = marketingLeadService;
        }

        [HttpGet]
        public WebLeadListViewModel Get([FromUri]WebLeadFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetWebLeadList(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public HttpResponseMessage DownloadWebLeads([FromBody]WebLeadFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }

            string fileName;
            var result = _marketingLeadService.DownloadWebLeads(filter, out fileName);
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
                    FileName = "webLeads.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public WebLeadReportListModel GetWebLeadReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetWebLeadReport(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadWebLeadReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }

            string fileName;
            var result = _marketingLeadService.DownloadWebLeadReport(filter, out fileName);
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
                    FileName = "webLeadReport.xlsx",
                };
                return response;
            }
            else return null;
        }
    }
}