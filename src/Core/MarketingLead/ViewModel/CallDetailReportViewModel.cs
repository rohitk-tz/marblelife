using System;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class CallDetailReportViewModel
    {
        public string PhoneLabel { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<HeaderDataCollection> lstHeader { get; set; }
        public int PhoneLeadTotal { get; set; }
        public int WebLeadTotal { get; set; }
        public int GrandTotal { get; set; }
        public int CCTotal { get; set; }
        public int CorpsTotal { get; set; }
        public int BusneissTotal { get; set; }
        public int DirectResponseTotal{ get; set; }
        public int TotalCallsDetailsTotal { get; set; }
        public int DirectResponseDetailsTotal { get; set; }
        public int AutoDailerTotal { get; set; }
    }

    public class HeaderDataCollection
    {
        public int HeaderMonth { get; set; }
        public int HeaderYear { get; set; }
        public int Day { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Count { get; set; }
        public int WebLead { get; set; }
        public int adjustedData { get; set; }
        public int rawData { get; set; }
        public int CallLead { get; set; }
        public int Total { get; set; }
        public int CcCount { get; set; }
        public int CorpsCount { get; set; }
        public int BusinessCount { get; set; }
        public int DirectResponseCount { get; set; }
        public int TotalCallsDetailsCount { get; set; }
        public int DirectResponseDetailsCount { get; set; }
        public int AutoDailerCount { get; set; }
    }
}