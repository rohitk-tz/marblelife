using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    public class RoutingNumberRecord
    {
        [XmlElement("phone_label")]
        public string PhoneLabel { get; set; }
        [XmlElement("phone_number")]
        public string PhoneNumber { get; set; }
    }
}
