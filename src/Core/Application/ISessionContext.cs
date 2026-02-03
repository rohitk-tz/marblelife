using Core.Users.ViewModels;

namespace Core.Application
{
    public interface ISessionContext
    {
        string Token { get; set; }
        UserSessionModel UserSession { get; set; }
    }
}
