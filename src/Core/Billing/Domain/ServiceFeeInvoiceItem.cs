using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class ServiceFeeInvoiceItem : DomainBase
    {
        [ForeignKey("InvoiceItem")]
        public override long Id { get; set; }

        public long ServiceFeeTypeId { get; set; }
        [ForeignKey("ServiceFeeTypeId")]
        public virtual Lookup ServiceFee { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Percentage { get; set; } 

        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
