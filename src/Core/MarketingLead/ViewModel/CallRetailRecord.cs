using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    public class CallRetailRecord
    {
        [XmlElement("date_added")]
        public string DateAdded { get; set; }
        [XmlElement("sid")]
        public string SessionId { get; set; }
        [XmlElement("call_type")]
        public string CallType { get; set; }
        [XmlElement("click_description")]
        public string ClickDescription { get; set; }
        [XmlElement("dnis")]
        public string DialedNumber { get; set; }
        //[XmlElement("ani")]
        //public string CallerId { get; set; }
        [XmlElement("call_duration")]
        public int CallDuration { get; set; }
        [XmlElement("transfer_type")]
        public string TransferType { get; set; }
        [XmlElement("transfer_to_number")]
        public string TransferToNumber { get; set; }
        [XmlElement("phone_label")]
        public string PhoneLabel { get; set; }
        [XmlElement("caller_id")]
        public string CallerId { get; set; }
    }
}
