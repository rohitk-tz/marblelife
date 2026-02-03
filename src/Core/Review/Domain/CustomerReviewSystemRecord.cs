using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Review.Domain
{
    public class CustomerReviewSystemRecord : DomainBase
    {
        public long CustomerId { get; set; }
        public long FranchiseeId { get; set; }
        public long? BusinessId { get; set; }
        public long ReviewSystemCustomerId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

    }
}
