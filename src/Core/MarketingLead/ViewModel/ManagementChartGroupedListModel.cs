using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class ManagementChartGroupedListModel
    {
        public string WorkingHourCategory { get; set; }
        public List<ManagementAndLocalChartGraphViewModel> ManagementVsLocalTableData { get; set; }
    }
}
