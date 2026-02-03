using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Users.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeAccountCredit : DomainBase
    {
        public DateTime CreditedOn { get; set; }
        public long FranchiseeId { get; set; }
        public long? InvoiceId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal RemainingAmount { get; set; }
        public long CurrencyExchangeRateId { get; set; }

        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }

        public long? CreditTypeId { get; set; }

        [ForeignKey("CreditTypeId")]
        public virtual Lookup CreditType { get; set; }
        public long? PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        
    }
}
