using Core.Application.Attribute;
using Core.Notification;
using Core.Users.Domain;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class SendLoginCredentialsService : ISendLoginCredentialsService
    {
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        public SendLoginCredentialsService(IUserNotificationModelFactory userNotificationModelFactory)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
        }

        public bool SendLoginCredentials(Person person, string password, bool includeSetupGuide)
        {
            _userNotificationModelFactory.CreateLoginCredentialNotification(person, password, includeSetupGuide);
            return true;
        }      
    }
}
