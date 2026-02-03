using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.Scheduler.ViewModel
{
    [XmlRoot("GeocodeResponse")]
    public class GeoCodeResponse 
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("result")]
        public AddressGeoCodeModel AddressGeoCodeModel { get; set; }
       
    }
}
