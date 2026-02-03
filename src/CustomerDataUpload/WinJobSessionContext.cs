using Core.Application;
using Core.Users.ViewModels;

namespace CustomerDataUpload
{
    public class WinJobSessionContext : ISessionContext
    {
        public WinJobSessionContext()
        {

        }
        public string Token { get; set; }
        public UserSessionModel UserSession { get; set; }
    }
}
