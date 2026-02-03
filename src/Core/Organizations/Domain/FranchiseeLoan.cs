using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Billing.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeLoan : DomainBase
    {
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public decimal InterestratePerAnum { get; set; }
        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        public bool? IsRoyality { get; set; }

        public bool? IsCompleted { get; set; }


        public DateTime? StartDate { get; set; }
        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<FranchiseeLoanSchedule> FranchiseeLoanSchedule { get; set; }

        public long CurrencyExchangeRateId { get; set; }
        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }


        public long? LoanTypeId { get; set; }
        [ForeignKey("LoanTypeId")]
        public virtual Lookup LoanType { get; set; }
    }
}
