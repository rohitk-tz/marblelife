using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    public class ManagementAndLocalChartViewModel
    {
        public decimal TotalCallsForDay { get; set; }
        public decimal TotalMissedCalls { get; set; }
        public decimal TotalMissedCallsForTheDay { get; set; }
        public decimal TotalCallsReceivedForDay { get; set; }
        public string DayOfDate { get; set; }
        public int OrderBy { get; set; }
        public int Occurance { get; set; }
        public int TotalDaysInAMonth { get; set; }
        public string ColumnColor { get; set; }
    }
}
