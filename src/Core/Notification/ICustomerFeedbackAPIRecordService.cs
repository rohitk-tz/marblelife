using Core.Organizations.Domain;
using Core.Review.ViewModel;

namespace Core.Notification
{
    public interface ICustomerFeedbackAPIRecordService
    {
        ReviewAPIResponseModel SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee franchisee);
    }
}
