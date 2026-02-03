using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Review.Domain
{
    public class CustomerFeedbackRequest : DomainBase
    {
        public long FranchiseeSalesId { get; set; }
        public DateTime DateSend { get; set; }
        public string DataPacket { get; set; }

        [ForeignKey("FranchiseeSalesId")]
        public virtual FranchiseeSales FranchiseeSales { get; set; }
        public bool IsQueued { get; set; }

        public string CustomerEmail { get; set; }

        public long CustomerReviewSystemRecordId { get; set; }

        public long? ResponseId { get; set; }

        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }

        public string QBInvoiceId { get; set; }

        [ForeignKey("CustomerReviewSystemRecordId")]
        public virtual CustomerReviewSystemRecord CustomerReviewSystemRecord { get; set; }

        [ForeignKey("ResponseId")]
        public virtual CustomerFeedbackResponse CustomerFeedbackResponse { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public bool IsSystemGenerated { get; set; }

        public long AuditActionId { get; set; }
        [ForeignKey("AuditActionId")]
        public virtual Lookup AuditAction { get; set; }

        public long? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Lookup Status { get; set; }
    }
}