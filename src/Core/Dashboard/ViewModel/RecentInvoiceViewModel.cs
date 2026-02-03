using Core.Application.Attribute;
using System;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class RecentInvoiceViewModel
    {
        public long FranchiseeId { get; set; }
        public string FranchiseName { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? PayableAmount { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime GeneratedOn { get; set; }
        public long InvoiceId { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsLateFeeCharged { get; set; }
        public long AccountTypeId { get; set; }
    }
}
