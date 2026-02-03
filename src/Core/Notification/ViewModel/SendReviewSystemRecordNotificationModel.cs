using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    public class SendReviewSystemRecordNotificationModel
    {
        public string FullName { get; set; }
        public string StartDate { get; set; }
        public string Franchisee { get; set; }
        public string EndDate { get; set; }
        public EmailNotificationModelBase Base { get; private set; }

        public SendReviewSystemRecordNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
