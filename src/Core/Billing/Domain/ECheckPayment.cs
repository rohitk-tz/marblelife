using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class ECheckPayment : DomainBase
    {
        [ForeignKey("Payment")]
        public override long Id { get; set; }
        public long ECheckId { get; set; }
        public string TransactionId { get; set; }
        public virtual Payment Payment { get; set; }

        [ForeignKey("ECheckId")]
        public virtual ECheck ECheck { get; set; }
    }
}
