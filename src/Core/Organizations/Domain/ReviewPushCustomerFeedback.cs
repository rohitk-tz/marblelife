using Core.Application.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.Domain
{
    public class ReviewPushCustomerFeedback : DomainBase
    {
        public string Name { get; set; }
        public long? Review_Id { get; set; }
        public long? Location_Id { get; set; }
        public long? Rp_ID { get; set; }

        public long? FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public long? Rating { get; set; }
        public DateTime? Rp_date { get; set; }
        public DateTime? Db_date { get; set; }
        public string Url { get; set; }
        public string Review { get; set; }
        public string Email { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long AuditActionId { get; set; }
        [ForeignKey("AuditActionId")]
        public virtual Lookup AuditAction { get; set; }
    }
}
