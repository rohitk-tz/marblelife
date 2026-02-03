using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FloorGrindingAdjustment : DomainBase
    {
        public long? FranchiseeId { get; set; }
        public string DiameterOfGrindingPlate { get; set; }
        public decimal? Area { get; set; }
        public decimal? AdjustmentFactor { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
