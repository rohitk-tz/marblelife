using Core.Geo.Domain;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.Domain
{
    public class HomeAdvisor : DomainBase
    {
        public string HAAccount { get; set; }

        public string CompanyName { get; set; }
        public string SRID { get; set; }
        public DateTime SRSubmittedDate { get; set; }

        public string Task { get; set; }
        public decimal? NetLeadDollar { get; set; }
        public long? CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public long? StateId { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }
        public string ZipCode { get; set; }
        public string LeadType { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }

        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
