using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class PriceEstimateServices : DomainBase
    {   
        public long? FranchiseeId { get; set; }
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? CorporateAdditionalPrice { get; set; }
        public decimal? FranchiseePrice { get; set; }
        public decimal? FranchiseeAdditionalPrice { get; set; }
        public string AlternativeSolution { get; set; }

        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsPriceChangedByAdmin { get; set; }
        public long? ServiceTagId { get; set; }
        [ForeignKey("ServiceTagId")]
        public virtual ServicesTag ServicesTag { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public bool IsFranchiseePriceExceed { get; set; }
        public bool IsFranchiseePriceExceedForEmail { get; set; }
    }
}
