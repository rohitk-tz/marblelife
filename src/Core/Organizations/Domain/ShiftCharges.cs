using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class ShiftCharges : DomainBase
    {   
        public long? FranchiseeId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        public decimal? TechDayShiftPrice { get; set; }
        public decimal? CommercialRestorationShiftPrice { get; set; }
        public decimal? MaintenanceTechNightShiftPrice { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
