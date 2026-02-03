using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FloorGrindingAdjustmentNotes : DomainBase
    {
        public long? FranchiseeId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        public string Note { get; set; }
        public bool IsChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
