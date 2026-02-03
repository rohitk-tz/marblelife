using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
   public class InvoicePayment : DomainBase
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
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }
    }
}
