using Core.Application.Attribute;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class AnnualAuditNotificationModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public int Year { get; set; }
        public string AdminName { get; set; }
        public EmailNotificationModelBase Base { get; private set; }

        public AnnualAuditNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
