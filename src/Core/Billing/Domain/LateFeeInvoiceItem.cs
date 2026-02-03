using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class LateFeeInvoiceItem : DomainBase
    {
        [ForeignKey("InvoiceItem")]
        public override long Id { get; set; }


        public long? LateFeeTypeId { get; set; }

        [ForeignKey("LateFeeTypeId")]
        public virtual Lookup Lookup { get; set; }
        public decimal Amount { get; set; }
        public int WaitPeriod { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime GeneratedOn { get; set; }

        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
