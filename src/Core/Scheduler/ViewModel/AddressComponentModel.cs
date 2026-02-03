using System.Xml.Serialization;

namespace Core.Scheduler.ViewModel
{
    public class AddressComponentModel
    {
        [XmlElement("long_name")]
        public string LongName { get; set; }
        [XmlElement("short_name")]
        public string ShortName { get; set; }
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
