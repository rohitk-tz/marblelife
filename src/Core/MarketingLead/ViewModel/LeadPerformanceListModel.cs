using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{

    public class LeadPerformanceListModel
    {
        public int TotalCount { get; set; }
        public IEnumerable<LeadPerformanceFranchiseeViewModel> LeadPerformanceFranchiseeViewData { get; set; }
    }
}
