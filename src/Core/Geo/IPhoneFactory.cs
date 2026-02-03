using Core.Geo.ViewModel;
using Core.Users.Domain;

namespace Core.Geo
{
   public interface IPhoneFactory
    {
        PhoneEditModel CreateEditModel(Phone domain, string phone = null);

        Phone CreateDomain(PhoneEditModel model);
    }
}
