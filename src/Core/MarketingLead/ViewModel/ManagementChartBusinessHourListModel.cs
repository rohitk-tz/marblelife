using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    public class ManagementChartBusinessHourListModel
    {
        public string BusinessHour { get; set; }
        public List<ManagementAndLocalChartGraphViewModel> ManagementVsLocalTableData { get; set; }
    }
}
