using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class TopLeadersViewModel
    {
        public IEnumerable<TopLeadersInfoModel> Collection { get; set; }
        public TopLeadersInfoModel FranchiseeInfo { get; set; }
        public TopLeadersInfoModel TotalSales { get; set; }
        public TopLeadersInfoModel TotalTopSales { get; set; }
        public string Type { get; set; }
        public long TypeId { get; set; }
    }
}
