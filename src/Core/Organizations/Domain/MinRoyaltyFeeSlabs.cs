using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class MinRoyaltyFeeSlabs : DomainBase
    {
        
        public long FranchiseeId { get; set; }
        public decimal? StartValue { get; set; }

        public decimal? EndValue { get; set; }

        public decimal MinRoyality { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

    }
}
