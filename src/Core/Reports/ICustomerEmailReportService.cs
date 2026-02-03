using Core.Reports.ViewModel;
using Core.Scheduler.ViewModel;
using System;

namespace Core.Reports
{
    public interface ICustomerEmailReportService
    {
        CustomerEmailReportListModel GetCustomerEmailReportList(CustomerEmailReportFilter filter);
        EmailChartDataListModel GetChartData(long franchiseeId, DateTime startDate, DateTime endDate);
        bool DownloadEmailReport(CustomerEmailReportFilter filter, out string fileName);
        bool IsCustomerSyncedToEmailAPI(long customerId);
        bool IsCustomerEmailSyncedToEmailAPI(long customerId, string email);
        MailListModel GetFranchiseeWiseMail(MailListFilter filter);
        ReviewChartDataListModel GetChartDataForReview(long franchiseeId, DateTime startDate, DateTime endDate);
        ReviewCountModel GetReviewCounts(long franchiseeId);
    }
}
