using Core.Users.Domain;
using Core.Users.ViewModels;

namespace Core.Users
{
    public interface IPersonFactory
    {
        PersonEditModel CreateModel(Person domain);
        Person CreateDomain(PersonEditModel model);
        Person CreateDomain(OrganizationOwnerEditModel organizationOwner, UserLogin userLogin, string email);
        Person CreateDomain(PersonEquipmentEditModel model);
    }
}
