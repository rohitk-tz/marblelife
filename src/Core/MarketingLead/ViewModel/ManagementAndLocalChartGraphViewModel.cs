using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
  public  class ManagementAndLocalChartGraphViewModel
    {
        public string BusinessHour { get; set; }
        public decimal Hour { get; set; }
        public IList<ManagementAndLocalChartViewModel> ChartData { get; set; }
        public string Rowcolor { get; set; }
        public string TimeStatus { get; set; }
        public decimal TotalCallsParticularHour { get; set; }
        public decimal TotalCallsReceivedAParticularHour { get; set; }
        public decimal TotalMissedCallsParticularHour { get; set; }
         // Adding New Value
        public decimal TotalMissedCallsForMonday { get; set; }
        public decimal TotalMissedCallsForTuesday { get; set; }
        public decimal TotalMissedCallsForWednesday { get; set; }
        public decimal TotalMissedCallsForThursday { get; set; }
        public decimal TotalMissedCallsForFriday { get; set; }
        public decimal TotalMissedCallsForSaturday { get; set; }
        public decimal TotalMissedCallsForSunday { get; set; }
        public string DayOfDate { get; set; }
        public int OrderBy { get; set; }
        public int Occurance { get; set; }
        public int TotalDaysInAMonth { get; set; }
        public string ColumnColor { get; set; }
    }
}
