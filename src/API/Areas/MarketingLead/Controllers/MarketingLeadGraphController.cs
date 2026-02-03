using Api.Areas.Application.Controller;
using Core.Application;
using Core.MarketingLead;
using Core.MarketingLead.ViewModel;
using Core.Users.Enum;
using System.Collections.Generic;
using System.Web.Http;

namespace API.Areas.MarketingLead.Controllers
{
    [AllowAnonymous]
    public class MarketingLeadGraphController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IMarketingLeadChartReportService _marketingLeadChartReportService;

        public MarketingLeadGraphController(ISessionContext sessionContext, IMarketingLeadChartReportService marketingLeadChartReportService)
        {
            _sessionContext = sessionContext;
            _marketingLeadChartReportService = marketingLeadChartReportService;
        }

        [HttpPost]
        public MarketingLeadChartListModel GetPhoneVsWebReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetPhoneVsWebReport(filter);
        }

        [HttpPost]
        public MarketingLeadChartListModel GetBusVsPhoneReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetBusVsPhoneReport(filter);
        }

        [HttpPost]
        public MarketingLeadChartListModel GetLocalVsNationalReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetLocalVsNationalReport(filter);
        }

        [HttpPost]
        public MarketingLeadChartListModel GetSpamVsPhoneReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetSpamVsPhoneReport(filter); 
        }

        [HttpPost]
        public CallDetailReportListModel GetSummaryReport([FromBody]MarketingLeadReportFilter filter) 
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetSummaryReport(filter); 
        }

        [HttpPost]
        public MarketingLeadChartListModel GetLocalVsNationalPhoneReport([FromBody]MarketingLeadReportFilter filter) 
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetLocalVsNationalPhoneReport(filter); 
        }

        [HttpPost]
        public MarketingLeadChartListModel GetDailyPhoneReport([FromBody]MarketingLeadReportFilter filter) 
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetDailyPhoneReport(filter); 
        }

        [HttpPost]
        public MarketingLeadChartListModel GetSeasonalLeadReport([FromBody]MarketingLeadReportFilter filter) 
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetSeasonalLeadReport(filter);  
        }
        [HttpPost]
        public CallDetailReportListModel GetAdjustedSummaryReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetAdjustedSummaryReport(filter);
        }
        [HttpPost]
        public MarketingLeadChartListModel GetCallDataReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetCallDetailsReport(filter);
        }

        [HttpPost]
        public MarketingLeadChartListModel GetLocalSitePerformanceReport([FromBody]MarketingLeadReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadChartReportService.GetLocalSitePerformanceReport(filter);
        }

        [HttpPost]
        public ManagementVsLocalChartListModel GetManagementVsLocalReport([FromBody]ManagementChartReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _marketingLeadChartReportService.GetManagementVsLocalReport(filter);
        }
        [HttpPost]
        public ManagementCharViewModel GetManagementReport([FromBody]ManagementChartReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _marketingLeadChartReportService.GetManagementReport(filter);
        }

    }
}