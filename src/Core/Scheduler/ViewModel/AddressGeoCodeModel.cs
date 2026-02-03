using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.Scheduler.ViewModel
{
    public class AddressGeoCodeModel
    {
        [XmlElement("address_component")]
        public List<AddressComponentModel> AddressComponents { get; set; }
    }
}
