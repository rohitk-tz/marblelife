
using Core.Application.Attribute;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class HomeAdvisorReportFilter
    {
        public long? FranchiseeId { get; set; }
        public string Text { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public int PageNumber { get; set; }
        public int WebPageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
