using Api.Areas.Application.Controller;
using Api.Areas.Application.ViewModel;
using Core.Application;
using Core.Geo.Domain;
using Core.MarketingLead;
using Core.MarketingLead.ViewModel;
using Core.Scheduler.Domain;
using Core.Users.Enum;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.MarketingLead.Controllers
{
    [AllowAnonymous]
    public class MarketingLeadController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IMarketingLeadsReportService _marketingLeadService;
        private readonly IMarketingLeadChartReportService _marketingLeadChartReportService;

        public MarketingLeadController(ISessionContext sessionContext, IMarketingLeadsReportService marketingLeadService,
            IMarketingLeadChartReportService marketingLeadChartReportService)
        {
            _sessionContext = sessionContext;
            _marketingLeadService = marketingLeadService;
            _marketingLeadChartReportService = marketingLeadChartReportService;
        }

        [HttpGet]
        public CallDetailListModel Get([FromUri] CallDetailFilter filter, [FromUri] int pageNumber, [FromUri] int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if(_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
            }
            return _marketingLeadService.GetCallDetailList(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public HttpResponseMessage DownloadMarketingLead([FromBody] CallDetailFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            string fileName;
            if (filter.Sort.Order != null)
            {
                filter.SortingOrder = filter.Sort.Order;
            }
            if (filter.Sort.PropName != "" || filter.Sort.PropName != null)
            {
                filter.SortingColumn = filter.Sort.PropName;
            }
            var result = _marketingLeadService.DownloadMarketingLeadsWithNewRows(filter, out fileName);
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
                    FileName = "marketingLeads.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public CallDetailReportListModel GetCallDetailReport([FromBody] MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetCallDetailReport(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadCallDetailReport([FromBody] MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }

            string fileName;
            var result = _marketingLeadService.DownloadCallDetailReport(filter, out fileName);
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

        [HttpPost]
        public CallDetailReportListModel GetCallDetailReportRawData([FromBody] MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetCallDetailReportAdjustedData(filter);
        }

        [HttpPost]
        public HomeAdvisorReportListModel GetHomeAdvisorReport([FromBody] HomeAdvisorReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetHomeAdvisorReport(filter);
        }

        [HttpPost]
        public CallDetailListModelV2 GetForV2([FromBody] CallDetailFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetCallDetailListV2(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadLeadFlow([FromBody] CallDetailFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            string fileName;
            var result = _marketingLeadService.DownloadLeadFlow(filter, out fileName);
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
                    FileName = "leadFlow.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public FranchiseePhoneCallListModel GetFranchiseePhoneCalls([FromBody] PhoneCallFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;

            return _marketingLeadService.GetFranchiseePhoneCalls(filter);
        }

        [HttpPost]
        public bool SaveFranchiseePhoneCalls([FromBody] PhoneCallFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _marketingLeadService.SaveFranchiseePhoneCalls(filter);
        }
        [HttpPost]
        public bool EditFranchiseePhoneCalls([FromBody] PhoneCallEditModel filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _marketingLeadService.EditFranchiseePhoneCalls(filter);
        }
        [HttpPost]
        public bool GeneratePhoneCallInvoice([FromBody] PhoneCallInvoiceEditModel filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _marketingLeadService.GeneratePhoneCallInvoice(filter);
        }

        [HttpPost]
        public bool EditFranchiseePhoneCallsByBulk([FromBody] PhoneCallEditByBulkModel filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _marketingLeadService.EditFranchiseePhoneCallsByBulk(filter);
        }

        [HttpPost]
        public FranchiseePhoneCallBulkListModel GetFranchiseePhoneCallsBulkList([FromBody] PhoneCallFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;

            return _marketingLeadService.GetFranchiseePhoneCallsBulkList(filter);
        }

        [HttpPost]
        public bool SaveFranchiseePhoneCallsByBulk([FromBody] PhoneCallEditByBulkList filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _marketingLeadService.SaveFranchiseePhoneCallsByBulk(filter);
        }

        [HttpPost]
        public FranchiseePhoneCallBulkListModel GetFranchiseePhoneCallList([FromBody] PhoneCallFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;

            return _marketingLeadService.GetFranchiseePhoneCallList(filter);
        }


        [HttpPost]
        public AutomationBackUpCallListModel GetAutomationBackUpReport([FromBody] AutomationBackUpFilter filter)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _marketingLeadService.GetAutomationBackUpReport(filter, userId, roleUserId);
        }

        [HttpPost]
        public AutomationBackUpCallFranchiseeModel GetFranchiseePhoneCallListForFranchisee([FromBody] PhoneCallFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _marketingLeadService.GetFranchiseePhoneCallListForFranchisee(filter);
        }
        
        [HttpPost]
        public bool SaveCallDetailsReportNotes(CallDetailsReportNotesViewModel filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            filter.UserRole = _sessionContext.UserSession.RoleName;
            return _marketingLeadService.SaveCallDetailsReportNotes(filter);
        }
        [HttpPost]
        public CallDetailsReportNotesListViewModel GetCallDetailsReportNotes(CallDetailNotesFilter filter)
        {
            return _marketingLeadService.GetCallDetailsReportNotes(filter);
        }

        [HttpGet]
        public IEnumerable<FranchiseeDropdownListItem> GetOfficeCollection()
        {
            return _marketingLeadService.GetOfficeCollection();
        }

        [HttpGet]
        public IEnumerable<FranchiseeDropdownListItem> GetFranchiseeNameValuePairCollection()
        {
            return _marketingLeadService.GetFranchiseeNameValuePair();
        }

        [HttpPost]
        public HttpResponseMessage DownloadCallNotesHistoryDetails([FromBody] CallDetailNotesFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            string fileName;
            var result = _marketingLeadService.DownloadCallNotesHistory(filter, out fileName);
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
                    FileName = "CallNotesHostory.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool EditCallDetailsReportNotes(EditCallDetailsReportNotesViewModel filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            filter.UserRole = _sessionContext.UserSession.RoleName;
            return _marketingLeadService.EditCallDetailsReportNotes(filter);
        }
    }
}