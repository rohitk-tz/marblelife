using Core.Users.Domain;
using Core.Users.ViewModels;

namespace Core.Users
{
    public interface IUserLoginFactory
    {
        UserLoginEditModel CreateEditModel(UserLogin userLogin);
        UserLogin CreateDomain(UserLoginEditModel model, Person person, UserLogin userLogin);
        UserLogin CreateResetPasswordDomain(ChangePasswordEditModel model, UserLogin inPersistence);
        UserLogin CreateDomain(OrganizationOwnerEditModel organizationOwner, string email);
    }
}
