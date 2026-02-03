using Core.Billing.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeLoanSchedule : DomainBase
    {
        public int LoanTerm { get; set; }
        public long LoanId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }
        public decimal Principal { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal OverPaidAmount { get; set; }
        public decimal TotalPrincipal { get; set; }
        public bool IsPrePaid { get; set; }
        public bool IsOverPaid { get; set; }
        public bool CalculateReschedule { get; set; }

        [ForeignKey("LoanId")]
        public virtual FranchiseeLoan FranchiseeLoan { get; set; }

        public long? InvoiceItemId { get; set; }
        [ForeignKey("InvoiceItemId")]
        public virtual InvoiceItem PrincipalInvoiceItem { get; set; }


        public long? InterestAmountInvoiceItemId { get; set; }
        [ForeignKey("InterestAmountInvoiceItemId")]
        public virtual InvoiceItem InterestRateInvoiceItem { get; set; }

        public bool IsRoyality { get; set; }
    }
}
