using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    //[XmlRoot("response")]
   public class CallDetailListV3
    {
        [XmlElement("response")]
        public List<CallRetailRecordV2> response { get; set; }
    }
}
