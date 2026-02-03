using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class TaxRates : DomainBase
    {   
        public long? FranchiseeId { get; set; }
        public decimal? TaxForServices { get; set; }
        public decimal? TaxForProducts { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
