using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class ProductReportListFilter
    {
        public long FranchiseeId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public DateTime? PaymentDateStart { get; set; }
        public DateTime? PaymentDateEnd { get; set; }
        public string TypeIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
    }
}
