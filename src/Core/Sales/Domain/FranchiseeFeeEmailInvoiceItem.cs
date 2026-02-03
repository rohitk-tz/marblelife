using Core.Billing.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Sales.Domain
{
   public class FranchiseeFeeEmailInvoiceItem : DomainBase
    {
        [ForeignKey("InvoiceItem")]
        public override long Id { get; set; }

        public decimal? Percentage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Amount { get; set; }
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
