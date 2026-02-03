using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Review.Domain
{
    public class CustomerFeedbackResponse : DomainBase
    {
        public string ResponseContent { get; set; }
        public DateTime DateOfReview { get; set; }
        public long? CustomerId { get; set; }
        public long? FranchiseeId { get; set; }
        public decimal Rating { get; set; }
        public double Recommend { get; set; }
        public bool ShowReview { get; set; }
        public long? FeedbackId { get; set; }
        public DateTime? DateOfDataInDataBase { get; set; }
        public string Url { get; set; }
        public long? ReviewId { get; set; }
        public bool IsFromNewReviewSystem { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public string CustomerName { get; set; }


        public long AuditActionId { get; set; }
        [ForeignKey("AuditActionId")]
        public virtual Lookup AuditAction { get; set; }


        public bool IsFromGoogleAPI { get; set; }

        public bool IsFromSystemReviewSystem { get; set; }
        public bool IsSentToMarketingWebsite { get; set; }
    }
}
