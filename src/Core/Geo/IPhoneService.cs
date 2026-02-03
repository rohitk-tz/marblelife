using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Geo
{
    public interface IPhoneService
    {
        List<PhoneEditModel> GetDefaultPhoneModel();
    }
}
