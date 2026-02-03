using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class MarketingLeadChartListModel
    {
        public string Franchisee { get; set; }
        public IEnumerable<MarketingLeadChartViewModel> ChartData { get; set; }
    }
}
