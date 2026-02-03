using Core.Application.Attribute;
using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class OrganizationViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public long TypeId { get; set; }

        public AddressEditModel Address { get; set; }
        public IEnumerable<object> PhoneNumbers { get; set; }
        public bool IsActive { get; set; }

        public OrganizationViewModel()
        {
            Address = new AddressEditModel();
        }

    }
}
