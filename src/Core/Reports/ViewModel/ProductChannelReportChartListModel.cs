using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.ViewModel
{
    public class ProductChannelReportChartListModel
    {
        public IEnumerable<ProductChartViewModel> ChartData { get; set; }
        public IEnumerable<SalesFunnelReportChartViewModel> Graphs { get; set; }
    }
}
