using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    [XmlRoot("data")]
    public class CallDetailList  
    {


        [XmlElement("record")]
        public List<CallRetailRecord> record { get; set; }  
    }
}
