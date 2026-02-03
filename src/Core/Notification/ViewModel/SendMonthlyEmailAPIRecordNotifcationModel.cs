using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    public class SendMonthlyEmailAPIRecordNotifcationModel
    {
        public string FullName { get; set; }
        public string Startdate { get; set; }
        public string EndDate { get; set; }
        public IList<EmailAPINotificationModel> List { get; set; }
        public EmailNotificationModelBase Base { get; private set; }

        public SendMonthlyEmailAPIRecordNotifcationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
