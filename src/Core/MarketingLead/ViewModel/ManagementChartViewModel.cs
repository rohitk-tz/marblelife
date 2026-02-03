using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class ManagementChartViewModel
    {
        public long? FranchiseeId { get; set; }
        public long? MonthNumber { get; set; }
        public string Month { get; set; }
        public double PerTotalReceivedCalls { get; set; }
    }
}
