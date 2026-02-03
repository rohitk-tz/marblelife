namespace Core.Notification.ViewModel
{
    public class MonthlyNotificationModel
    {
        public string FullName { get; set; } 
        public string StartDate { get; set; }
        public string EndDate { get; set; }  

        public EmailNotificationModelBase Base { get; private set; }
        public MonthlyNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
