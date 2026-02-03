using Core.Application.Attribute;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class SendCustomerFeedbackNotificationModel
    {
        public string Link { get; set; }
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public string Owner { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public SendCustomerFeedbackNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
