using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class ManagemenChartViewModel
    {
        public string FranchiseeName { get; set; }
        public IList<ManagementChartViewModel> ManagementVsLocalAverageData { get; set; }
    }
}
