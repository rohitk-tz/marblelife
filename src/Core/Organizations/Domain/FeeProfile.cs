using Core.Application.Attribute;
using Core.Application.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FeeProfile : DomainBase
    {
        [ForeignKey("Franchisee")]
        public override long Id { get; set; }
        public long? PaymentFrequencyId { get; set; }
        public decimal MinimumRoyaltyPerMonth { get; set; }
        public bool SalesBasedRoyalty { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal AdFundPercentage { get; set; }      
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("PaymentFrequencyId")]
        public virtual Lookup Lookup { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<RoyaltyFeeSlabs> RoyaltyFeeSlabs { get; set; }
    }
}
