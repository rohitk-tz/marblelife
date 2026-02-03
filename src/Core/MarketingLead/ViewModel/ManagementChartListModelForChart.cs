using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class ManagementChartListModelForChart
    {
        public DateTime Date { get; set; }

        public double FrontOfficeCount { get; set; }
        public double OfficePersonCount { get; set; }
        public double NationalCount { get; set; }
        public double LocalCount { get; set; }
        public double ResponseNextDayCount { get; set; }
        public double ResponseWhenAvailableCount { get; set; }
        public double Month { get; set; }
    }
}
