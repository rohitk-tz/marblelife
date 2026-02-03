using System.Collections.Generic;
using Core.Geo.ViewModel;
using Core.Application.Attribute;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class PhoneService : IPhoneService
    {
        public List<PhoneEditModel> GetDefaultPhoneModel()
        {
            return new List<PhoneEditModel> { new PhoneEditModel()};
        }
    }
}
