using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class InterestRateInvoiceItem : DomainBase
    {
        [ForeignKey("InvoiceItem")]
        public override long Id { get; set; }

        public decimal Amount { get; set; }
        public int WaitPeriod { get; set; }
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
