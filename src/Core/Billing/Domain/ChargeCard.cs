using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class ChargeCard : DomainBase
    {
        public string NameOnCard { get; set; }
        public long TypeId { get; set; }
        public string Number { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }

        [ForeignKey("TypeId")]
        public virtual Lookup CardType { get; set; }
    }
}
