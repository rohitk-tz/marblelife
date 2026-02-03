using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    public class SalesFunnelLocalGraphListModel
    {
        public string Franchisee { get; set; }
        public IEnumerable<SalesFunnelLocalGraphViewModel> ChartData { get; set; }
    }
}
