using Core.Application.Attribute;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class MonthlySalesUploadNotificationModel
    {
        public string FullName { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public MonthlySalesUploadNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
