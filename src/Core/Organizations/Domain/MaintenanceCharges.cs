using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class MaintenanceCharges : DomainBase
    {
        public long? FranchiseeId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        public string Material { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public string UOM { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public string Notes { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
