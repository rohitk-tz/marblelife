using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class PaymentItem : DomainBase
    {
        public long PaymentId { get; set; }
        public long ItemId { get; set; }
        public long ItemTypeId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        [ForeignKey("ItemId")]
        public virtual ServiceType ServiceType { get; set; }
    }
}
