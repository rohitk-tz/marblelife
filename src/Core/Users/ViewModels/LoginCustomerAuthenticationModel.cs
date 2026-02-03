using Core.Application.ViewModel;

namespace Core.Users.ViewModels
{
    [Core.Application.Attribute.NoValidatorRequired]
    public class LoginCustomerAuthenticationModel : EditModelBase
    {
        public FeedbackMessageModel Message { get; set; }
        public string Code { get; set; }
    }
}
