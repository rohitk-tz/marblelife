using Core.Organizations.Domain;
using Core.Users.Domain;
using Core.Users.ViewModels;

namespace Core.Users
{
    public interface IUserFactory
    {
        UserViewModel CreateViewModel(OrganizationRoleUser organizationRoleUser);
        SalesRep CreateDomain(OrganizationRoleUser organizationRoleUser, UserEditModel userEditModel);
        UserSignatureEditModel CreateSignatureViewModel(EmailSignatures signature);
        EmailSignatures CreateSignatureSaveModel(UserSignatureSaveModel signature, long? userId, long? orgId);
    }
}
