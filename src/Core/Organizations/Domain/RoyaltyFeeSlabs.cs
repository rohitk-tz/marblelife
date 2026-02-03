using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class RoyaltyFeeSlabs : DomainBase
    {
        public long RoyaltyFeeProfileId { get; set; }
        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public decimal ChargePercentage { get; set; }

        [ForeignKey("RoyaltyFeeProfileId")]
        public virtual FeeProfile RoyaltyFeeProfile { get; set; }
    }
}
