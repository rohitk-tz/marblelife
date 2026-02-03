using Core.Application.ViewModel;

namespace Core.Users.ViewModels
{
    public class LoginAuthenticationModel : EditModelBase
    {
        public FeedbackMessageModel Message { get; set; }
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceKey { get; set; }
        public string Code { get; set; }
    }
}
