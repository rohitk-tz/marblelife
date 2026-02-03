using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    [XmlRoot("data")]
    public class RoutingNumberList
    {
        [XmlElement("record")]
        public List<RoutingNumberRecord> record { get; set; }
    }
}
