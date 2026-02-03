using System;

namespace Core.MarketingLead.ViewModel
{
    public class MarketingLeadChartViewModel
    {
        public MarketingLeadChartViewModel()
        {
        }
        public DateTime Date { get; set; }
        public DateTime DateForGraph { get; set; }
        public decimal TotalCount { get; set; }
        public decimal LocalCount { get; set; }
        public decimal Total { get; set; }

        public string StartEndWeekRange { get; set; }
        public string Color { get; set; }
        public decimal Local { get; set; }
        public decimal National { get; set; }
        public string DayOfWeek { get; set; }
        public string DateString { get; set; }

        public string LastYearDateString { get; set; } 
        public int WebLeadCount { get; set; }
        public int AutoDialerCount { get; set; }
        public int PrintMediaCount { get; set; }
        public int VanCount { get; set; }
        public int WebLocalCount { get; set; }
        public int WebNationalCount { get; set; } 
        public decimal BusinessDirectoriesCount { get; set; }
        public decimal CallOver2min { get; set; }
        public decimal CallUnder2min { get; set; }
        public decimal PhoneWebLocalCount { get; set; }
        public decimal GooglePPCCount { get; set; }
        public decimal LocalCountForPhonePerDay { get; set; }
    }
}
