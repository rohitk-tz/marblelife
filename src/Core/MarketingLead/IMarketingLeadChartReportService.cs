using Core.MarketingLead.ViewModel;
using System.Collections.Generic;

namespace Core.MarketingLead
{
    public interface IMarketingLeadChartReportService
    {
        MarketingLeadChartListModel GetPhoneVsWebReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetBusVsPhoneReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetLocalVsNationalReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetSpamVsPhoneReport(MarketingLeadReportFilter filter);
        CallDetailReportListModel GetSummaryReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetLocalVsNationalPhoneReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetDailyPhoneReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetSeasonalLeadReport(MarketingLeadReportFilter filter);
        CallDetailReportListModel GetAdjustedSummaryReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetCallDetailsReport(MarketingLeadReportFilter filter);
        MarketingLeadChartListModel GetLocalSitePerformanceReport(MarketingLeadReportFilter filter);
        ManagementVsLocalChartListModel GetManagementVsLocalReport(ManagementChartReportFilter filter);
        ManagementCharViewModel GetManagementReport(ManagementChartReportFilter filter);
    }
}
