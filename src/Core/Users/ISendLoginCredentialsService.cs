using Core.Users.Domain;

namespace Core.Users
{
    public interface ISendLoginCredentialsService
    {
        bool SendLoginCredentials(Person person, string password, bool includeSetupGuide);
    }
}
