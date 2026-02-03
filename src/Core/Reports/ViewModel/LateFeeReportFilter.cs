using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class LateFeeReportFilter
    {
        public long FranchiseeId { get; set; }
        public long LateFeeTypeId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDateStart { get; set; }
        public DateTime? DueDateEnd { get; set; }
        public long StatusId { get; set; }
        public string Text { get; set; }
    }
}
