using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class EmailChartDataListModel
    {
        public string Franchisee { get; set; }
        public IEnumerable<ChartViewModel> ChartData { get; set; } 
    }
}
