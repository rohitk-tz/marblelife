using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class InvoiceItem : DomainBase
    {
        public long InvoiceId { get; set; }
        public long? ItemId { get; set; }

        public long ItemTypeId { get; set; }

        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }
        public decimal Amount { get; set; }

        public string ItemOriginal { get; set; }
        public long CurrencyExchangeRateId { get; set; }

        [ForeignKey("ItemTypeId")]
        public virtual Lookup Lookup { get; set; }

        [ForeignKey("ItemId")]
        public virtual ServiceType ServiceType { get; set; }

        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }

        [CascadeEntity]
        public virtual RoyaltyInvoiceItem RoyaltyInvoiceItem { get; set; }

        [CascadeEntity]
        public virtual AdFundInvoiceItem AdFundInvoiceItem { get; set; }

        [CascadeEntity]
        public virtual LateFeeInvoiceItem LateFeeInvoiceItem { get; set; }
        [CascadeEntity]
        public virtual InterestRateInvoiceItem InterestRateInvoiceItem { get; set; }
        [CascadeEntity]
        public virtual ServiceFeeInvoiceItem ServiceFeeInvoiceItem { get; set; }
        public virtual Invoice Invoice { get; set; }

        [CascadeEntity]
        public virtual FranchiseeFeeEmailInvoiceItem FranchiseeFeeEmailInvoiceItem { get; set; }

    }
}
