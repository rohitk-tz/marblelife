namespace Core.Notification.ViewModel
{
    public class UserForgetPasswordNotificationViewModel
    {
        public string FullName { get; set; }
        public string PasswordLink { get; set; }
        public string UserName { get; set; }
        public string Franchisee { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public UserForgetPasswordNotificationViewModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
