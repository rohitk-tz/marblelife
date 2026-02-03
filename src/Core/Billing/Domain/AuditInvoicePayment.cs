using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class AuditInvoicePayment : DomainBase
    {
        [NotMapped]
        public override long Id { get; set; }

        [Key]
        [Column(Order = 0)]
        public long InvoiceId { get; set; }

        [Key]
        [Column(Order = 1)]
        public long PaymentId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual AuditInvoice AuditInvoice { get; set; }

        [ForeignKey("PaymentId")]
        public virtual AuditPayment AuditPayment { get; set; }
    }
}
