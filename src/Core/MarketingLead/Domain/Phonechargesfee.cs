using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class Phonechargesfee : DomainBase
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

        public long? CurrencyExchangeRateId { get; set; }
        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }

        public long? FranchiseetechmailserviceId { get; set; }
        [ForeignKey("FranchiseetechmailserviceId")]
        public virtual FranchiseeTechMailEmail Franchiseetechmailservice { get; set; }

        public bool IsInvoiceGenerated { get; set; }

        public bool IsInvoiceInQueue { get; set; }

        public long? FranchiseeservicefeeId { get; set; }
        [ForeignKey("FranchiseeservicefeeId")]
        public virtual FranchiseeServiceFee FranchiseeServiceFee { get; set; }
    }
}
