using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class FranchiseeSalesInfo : DomainBase
    {
        public long FranchiseeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal AmountInLocalCurrency { get; set; }
        public DateTime UpdatedDate { get; set; }

        public long ClassTypeId { get; set; }
        public long ServiceTypeId { get; set; }

        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType Service { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
