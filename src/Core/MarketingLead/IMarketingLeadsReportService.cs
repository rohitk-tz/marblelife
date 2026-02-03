using Core.Geo.Domain;
using Core.MarketingLead.ViewModel;
using Core.Scheduler.Domain;
using System.Collections.Generic;
using System.Linq;
using static Core.Organizations.Impl.FranchiseeInfoService;

namespace Core.MarketingLead
{
    public interface IMarketingLeadsReportService
    {
        CallDetailListModel GetCallDetailList(CallDetailFilter filter, int pageNumber, int pageSize);
        RoutingNumberListModel GetRoutingNumberList(CallDetailFilter filter, int pageNumber, int pageSize);
        bool UpdateFranchisee(long id, long? franchiseeId);
        WebLeadListViewModel GetWebLeadList(WebLeadFilter filter, int pageNumber, int pageSize);
        bool DownloadWebLeads(WebLeadFilter filter, out string fileName);
        bool DownloadMarketingLeads(CallDetailFilter filter, out string fileName);
        bool DownloadRoutingNumber(CallDetailFilter filter, out string fileName);
        bool UpdateTag(long id, long? tagId);
        WebLeadReportListModel GetWebLeadReport(MarketingLeadReportFilter filter);
        bool DownloadWebLeadReport(MarketingLeadReportFilter filter, out string fileName);
        CallDetailReportListModel GetCallDetailReport(MarketingLeadReportFilter filter);
        bool DownloadCallDetailReport(MarketingLeadReportFilter filter, out string fileName);
        CallDetailReportListModel GetCallDetailReportAdjustedData(MarketingLeadReportFilter filter);
        IQueryable<CallDetailReportViewModel> GetCallDetailReportListAdjustedData(MarketingLeadReportFilter filter);
        HomeAdvisorReportListModel GetHomeAdvisorReport(HomeAdvisorReportFilter filter);
        CallDetailListModelV2 GetCallDetailListV2(CallDetailFilter filter);
        bool DownloadLeadFlow(CallDetailFilter filter, out string fileName);
        FranchiseePhoneCallListModel GetFranchiseePhoneCalls(PhoneCallFilter filter);
        bool SaveFranchiseePhoneCalls(PhoneCallFilter filter);
        bool EditFranchiseePhoneCalls(PhoneCallEditModel filter);
        bool GeneratePhoneCallInvoice(PhoneCallInvoiceEditModel filter);
        bool EditFranchiseePhoneCallsByBulk(PhoneCallEditByBulkModel filter);
        FranchiseePhoneCallBulkListModel GetFranchiseePhoneCallsBulkList(PhoneCallFilter filter);

        bool SaveFranchiseePhoneCallsByBulk(PhoneCallEditByBulkList filter);
        FranchiseePhoneCallBulkListModel GetFranchiseePhoneCallList(PhoneCallFilter filter);
        AutomationBackUpCallListModel GetAutomationBackUpReport(AutomationBackUpFilter filter, long userId, long roleUserId);
        bool DownloadMarketingLeadsWithNewRows(CallDetailFilter filter, out string fileName);

        AutomationBackUpCallFranchiseeModel GetFranchiseePhoneCallListForFranchisee(PhoneCallFilter filter);
        bool SaveCallDetailsReportNotes(CallDetailsReportNotesViewModel filter);
        CallDetailsReportNotesListViewModel GetCallDetailsReportNotes(CallDetailNotesFilter filter);
        IEnumerable<FranchiseeDropdownListItem> GetOfficeCollection();
        IEnumerable<FranchiseeDropdownListItem> GetFranchiseeNameValuePair();
        bool DownloadCallNotesHistory(CallDetailNotesFilter filter, out string fileName);
        bool EditCallDetailsReportNotes(EditCallDetailsReportNotesViewModel filter);
    }
}
