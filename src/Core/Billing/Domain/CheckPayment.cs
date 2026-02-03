using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class CheckPayment : DomainBase
    {
        [ForeignKey("Payment")]
        public override long Id { get; set; }
        public decimal Amount { get; set; }
        public long CheckId { get; set; }
        public virtual Payment Payment { get; set; }

        [ForeignKey("CheckId")]
        public virtual Check Check { get; set; }
    }
}
