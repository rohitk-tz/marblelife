using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
  public  class ManagementCharViewModel
    {
        public IList<ManagementChartListModel> ManagementVsLocalAverageData { get; set; }
        public IList<string> MonthCollection { get; set; }
        public IList<ManagementChartListModelForChart> ManagementChartListModelForChart { get; set; }
    }
}
