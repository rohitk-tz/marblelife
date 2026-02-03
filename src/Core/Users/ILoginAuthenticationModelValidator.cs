using Core.Scheduler.Domain;
using Core.Users.ViewModels;

namespace Core.Users
{
    public interface ILoginAuthenticationModelValidator
    {
        bool IsValid(LoginAuthenticationModel model);
        bool IsValidForReviewAPI(LoginAuthenticationModel model);
        bool IsValidForCustomer(LoginCustomerAuthenticationModel model);
        CustomerSignatureInfo GetCustomerSignatureInfo(LoginCustomerAuthenticationModel model);
    }
}
