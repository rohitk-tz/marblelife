using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class ManagementChartListModel
    {
        public string CategoryName { get; set; }
        public IList<ManagementChartDayListModel> ManagementVsLocalAverageData { get; set; }

        
    }
}
