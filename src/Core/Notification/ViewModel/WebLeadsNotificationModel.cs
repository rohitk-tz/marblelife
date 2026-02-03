using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    public class WebLeadsNotificationModel
    {
        public string Date { get; set; }
        public string FromMail { get; set; }
        public string ToMail { get; set; }
        public string CCMail { get; set; }
        public EmailNotificationModelBase Base { get; private set; }

        public WebLeadsNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
