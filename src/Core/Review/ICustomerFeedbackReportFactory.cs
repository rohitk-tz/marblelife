using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;

namespace Core.Review
{
    public interface ICustomerFeedbackReportFactory
    {
        CustomerFeedbackReportViewModel CreateViewModel(CustomerFeedbackRequest request);
        ReviewSystemRecordViewModel CreateModel(CustomerFeedbackRequest request);
        CustomerFeedbackReportViewModel CreateViewModel(ReviewPushCustomerFeedback request);
        CustomerFeedbackReportViewModel CreateViewModel(CustomerFeedbackResponse response);
    }
}
