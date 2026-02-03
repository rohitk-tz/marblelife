using Core.Application.Attribute;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class LateFeeReminderNotificationModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public long InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public decimal? LateFee { get; set; }
        public string DueDate { get; set; }
        public string LateFeeType { get; set; }
        public string Description { get; set; }
        public string ExpectedDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public LateFeeReminderNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
