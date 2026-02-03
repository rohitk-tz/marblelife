using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class OrganizationEditModel : EditModelBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public string DeactivationNote { get; set; }
        public long TypeId { get; set; }
        public IEnumerable<AddressEditModel> Address { get; set; }
        public IEnumerable<PhoneEditModel> PhoneNumbers { get; set; }
        public bool IsActive { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        public OrganizationEditModel()
        {
            Address = new List<AddressEditModel>();
            PhoneNumbers = new List<PhoneEditModel>();
        }
    }
}
