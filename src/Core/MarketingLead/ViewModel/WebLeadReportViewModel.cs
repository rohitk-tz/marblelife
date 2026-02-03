using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class WebLeadReportViewModel
    {
        [DownloadField(Required = false)]
        public long? FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string URL { get; set; }
        public ICollection<HeaderDataWebLeadCollection> lstHeader { get; set; }
        public int GrandTotal { get; set; }
    }
    public class HeaderDataWebLeadCollection
    {
        public int HeaderMonth { get; set; }
        public int HeaderYear { get; set; }
        public int Day { get; set; }
        public DateTime StartOfWeek { get; set; }
        public DateTime EndOfWeek { get; set; }
        public int Count { get; set; }
        public int TotalSum { get; set; }
    }
    public class monthyearCollection
    {
        public int MonthVal { get; set; }
        public int YearVal { get; set; }
    }
}