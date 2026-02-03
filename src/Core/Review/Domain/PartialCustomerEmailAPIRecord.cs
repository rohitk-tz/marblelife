using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Review.Domain
{
  public  class PartialPaymentEmailApiRecord : DomainBase
    {
        public long CustomerId { get; set; }
        public long FranchiseeId { get; set; }
        public string CustomerEmail { get; set; }
        public string ApiCustomerId { get; set; }
        public string APIListId { get; set; }
        public string APIEmailId { get; set; }
        public string ErrorResponse { get; set; }
        public string Status { get; set; }
        public bool IsSynced { get; set; }
        public bool IsFailed { get; set; }
        public long InvoiceId { get; set; }

        public long statusId { get; set; }
        public DateTime? DateCreated { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
        [ForeignKey("statusId")]
        public virtual Lookup Lookup { get; set; }
    }
}
