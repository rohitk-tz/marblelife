using Core.Application.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class RoutingNumber : DomainBase
    {
        public string PhoneNumber { get; set; }
        public string PhoneLabel { get; set; }
        public long? FranchiseeId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? TagId { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public long? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Lookup Category { get; set; } 

    }
}
