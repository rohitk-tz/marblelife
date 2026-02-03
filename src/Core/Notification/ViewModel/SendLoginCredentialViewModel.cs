namespace Core.Notification.ViewModel
{
    public class SendLoginCredentialViewModel
    {
        public string FullName { get; set; }
        public string Password { get; set; }

        public string UserName { get; set; }
        public string Franchisee { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public SendLoginCredentialViewModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
