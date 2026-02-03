using Core.Application.Attribute;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class CustomerEmailReportFilter
    {
        public long FranchiseeId { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; }
    }
}
