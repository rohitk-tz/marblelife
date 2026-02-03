using Core.Billing.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class OneTimeProjectFee : DomainBase
    {
        public decimal Amount { get; set; }
        public long FranchiseeId { get; set; } 
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? InvoiceItemId { get; set; }
        [ForeignKey("InvoiceItemId")]
        public virtual InvoiceItem InvoiceItem { get; set; }

        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        public long CurrencyExchangeRateId { get; set; }
        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    }
}
