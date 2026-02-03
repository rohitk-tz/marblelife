using Core.Application.Attribute;
using System;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceListFilter
    {
        public string Text { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public long FranchiseeId { get; set; }
        public long? StatusId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? DueDateStart { get; set; }
        public DateTime? DueDateEnd { get; set; }
        public long? SalesDataUploadId { get; set; }
        public long LateFeeTypeId { get; set; }
        public bool IsDownloaded { get; set; }
        public DateTime? PaymentDateStart { get; set; }
        public DateTime? PaymentDateEnd { get; set; }
        public string UndownloadedInvoice { get; set; }
        public decimal LoanAndLoanInt { get; set; }
        public decimal ISQFT { get; set; }
        public decimal WebSEO { get; set; }
        public decimal BackUpCharges { get; set; }

        public bool? IsAdfund { get; set; }
        public bool? IsRoyality { get; set; }

        public Sort Sort { get; set; }
    }


    public class Sort
    {
        public string PropName { get; set; }
        public long? Order { get; set; }
    }


}
