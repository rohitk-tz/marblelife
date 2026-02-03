namespace Core.Notification.ViewModel
{
    public class DocumentUploadNotificationModel
    {
        public string FullName { get; set; }
        public string UploadedBy { get; set; }
        public string Email { get; set; }
        public string Franchisee { get; set; }
        public string DocName { get; set; }
        public string Role { get; set; }
        public string ExpiryDate { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public DocumentUploadNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
