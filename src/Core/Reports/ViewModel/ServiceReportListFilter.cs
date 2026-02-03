using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class ServiceReportListFilter
    {
        public long FranchiseeId { get; set; }
        public long ServiceTypeId { get; set; }
        public long ClassTypeId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public DateTime? PaymentDateStart { get; set; }
        public DateTime? PaymentDateEnd { get; set; }
        public string TypeIds { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
