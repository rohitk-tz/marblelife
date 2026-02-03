using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class ProductReportChartListModel
    {
        public IEnumerable<ProductChartViewModel> ChartData { get; set; }
        public IEnumerable<ProductGraphViewModel> Graphs { get; set; }
    }
}
