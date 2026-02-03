using Core.Organizations.Domain;
using Core.Users.Domain;
using Core.Users.Enum;

namespace Core.Organizations
{

    public interface IOrganizationRoleUserService
    {
        OrganizationRoleUser Save(Organization organization, Person person, RoleType role);
    }

}