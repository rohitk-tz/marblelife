using System;

namespace Core.MarketingLead.ViewModel
{
    public class MarketingLeadCallDetailViewModel
    {
        public string SessionId { get; set; }
        public DateTime? DateAdded { get; set; }
        public string DialedNumber { get; set; }
        public string CallerId { get; set; }
        public long CallTypeId { get; set; }
        public string TransferType { get; set; }
        public string PhoneLabel { get; set; }
        public string TransferToNumber { get; set; }
        public string ClickDescription { get; set; }
        public int CallDuration { get; set; }
    }
}
