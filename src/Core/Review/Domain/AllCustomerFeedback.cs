using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Review.Domain
{
    public class AllCustomerFeedback : DomainBase
    {
        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime? ResponseReceivedDate { get; set; }
        public DateTime? ResponseSyncingDate { get; set; }
        public string ResponseContent { get; set; }
        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public string FranchiseeName { get; set; }
        public decimal? Rating { get; set; }
        public long? Recommend { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerNameFromAPI { get; set; }
        public long AuditStatusId { get; set; } 
        [ForeignKey("AuditStatusId")]
        public virtual Lookup AuditAction { get; set; }
        public string From { get; set; }
        public string FromTable { get; set; }
        public long? ReviewPushCustomerFeedbackId { get; set; }
        [ForeignKey("ReviewPushCustomerFeedbackId")]
        public virtual ReviewPushCustomerFeedback ReviewPushCustomerFeedback { get; set; }
        public long? CustomerFeedbackRequestId { get; set; }
        [ForeignKey("CustomerFeedbackRequestId")]
        public virtual CustomerFeedbackRequest CustomerFeedbackRequest { get; set; }
        public long? CustomerFeedbackResponseId { get; set; }
        [ForeignKey("CustomerFeedbackResponseId")]
        public virtual CustomerFeedbackResponse CustomerFeedbackResponse { get; set; }
        public bool IsOldReview { get; set; }
        public bool IsSentToMarketingWebsite { get; set; }
        public bool IsEmailSent { get; set; }
        public bool IsActive { get; set; }
    }
}
