using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    public class ManagementChartDayViewModel
    {
        public string DayOfWeek { get; set; }
        public string OrderBy { get; set; }
        public decimal Value { get; set; }
    }
}
