using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Users.Domain;
using Core.Users.ViewModels;

namespace Core.Organizations
{
    public interface IOrganizationFactory
    {
        OrganizationEditModel CreateEditModel(Organization domain);

        Organization CreateDomain(OrganizationEditModel model);

        OrganizationRoleUser CreateDomain(UserEditModel userEditModel, Person person, OrganizationRoleUser domain);

        OrganizationViewModel CreateViewModel(Organization domain);

        OrganizationRoleUser CreateDomain(Person person, Organization organization, long ownerId);
        OrganizationRoleUser CreateDomain(OrganizationRoleUser orgRoleUser, Franchisee franchisee);
        FranchiseeInfoViewModel CreateViewModel(OrganizationRoleUser orgRoleUser);
        OrganizationRoleUserFranchisee CreateDomain(long orgId, OrganizationRoleUser orgRoleUser, OrganizationRoleUserFranchisee inDb);
        FranchiseeInfoViewModel CreateViewModel(OrganizationRoleUserFranchisee orgRoleUser);

        OrganizationRoleUser CreateDomain(UserEquipmentEditModel userEditModel, Person person, OrganizationRoleUser domain);
    }
}
