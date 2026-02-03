using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.MarketingLead.Domain
{
    public class FranchsieeGoogleReviewUrlAPI:DomainBase
    {
        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Francbhisee { get; set; }
        public string BusinessName { get; set; }
        public string BrightLocalLink { get; set; }
    }
}

