using Core.Review.ViewModel;

namespace Core.Review
{
    public interface ICustomerFeedbackReportService
    {
        CustomerFeedbackReportListModel GetCustomerFeedbackList(CustomerFeedbackReportFilter filter, int pageNumber, int pageSize);
        CustomerFeedbackReportViewModel GetCustomerFeedbackDetail(long responseId, bool isFromNewReviewSystem, bool isFromCustomerReviewTable);
        bool DownloadFeedbackReport(CustomerFeedbackReportFilter filter, out string fileName);
        bool ManageCustomerFeedbackStatus(bool isAccept, long customerId, long id, string fromTable);
    }
}
