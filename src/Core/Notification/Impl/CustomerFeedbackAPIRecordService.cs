using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Review;
using Core.Review.ViewModel;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class CustomerFeedbackAPIRecordService : ICustomerFeedbackAPIRecordService
    {
        private IUserNotificationModelFactory _userNotificationModelFactory;
        private IClock _clock;
        public CustomerFeedbackAPIRecordService(IUserNotificationModelFactory userNotificationModelFactory, IClock clock)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
            _clock = clock;
        }
        public ReviewAPIResponseModel SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee franchisee)
        {
            var currentDate = _clock.UtcNow;
            var response = _userNotificationModelFactory.SendEmailFeedbackRequest(customerEmail, customerName, franchisee);
            return response;
        }
    }
}
