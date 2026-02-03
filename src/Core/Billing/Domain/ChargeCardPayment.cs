using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class ChargeCardPayment : DomainBase
    {
        [ForeignKey("Payment")]
        public override long Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public long ChargeCardId { get; set; }
        public virtual Payment Payment { get; set; }

        [ForeignKey("ChargeCardId")]
        public virtual ChargeCard ChargeCard { get; set; }

    }
}
