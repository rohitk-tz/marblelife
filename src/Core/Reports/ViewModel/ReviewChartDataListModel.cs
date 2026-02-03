using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.ViewModel
{
    public class ReviewChartDataListModel
    {
        public string Franchisee { get; set; }
        public IEnumerable<ChartViewModel> ChartData { get; set; }
    }
}
