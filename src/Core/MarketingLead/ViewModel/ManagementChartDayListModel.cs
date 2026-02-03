using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class ManagementChartDayListModel
    {
        public string FranchiseeName { get; set; }
        public IList<ManagementChartViewModel> ManagementChartViewModel { get; set; }
    }
}
