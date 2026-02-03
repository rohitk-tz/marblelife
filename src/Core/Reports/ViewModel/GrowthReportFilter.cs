using Core.Application.Attribute;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class GrowthReportFilter
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long? FranchiseeId { get; set; }
        public long ClassTypeId { get; set; }
        public long ServiceTypeId { get; set; } 
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
    }
}
