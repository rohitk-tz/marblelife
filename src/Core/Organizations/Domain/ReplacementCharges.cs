using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class ReplacementCharges : DomainBase
    {   
        public long? FranchiseeId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        public string Material { get; set; }
        public decimal? CostOfRemovingTile { get; set; }
        public decimal? CostOfInstallingTile { get; set; }
        public decimal? CostOfTileMaterial { get; set; }
        public decimal? TotalReplacementCost { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
