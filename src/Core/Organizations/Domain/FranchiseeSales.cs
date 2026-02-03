using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Sales.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeSales : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }
        public long? InvoiceId { get; set; }
        public long? AccountCreditId { get; set; }
        public long ClassTypeId { get; set; }
        public long? SubClassTypeId { get; set; }
        public string  SalesRep { get; set; }
        public decimal Amount { get; set; }
        public long? SalesDataUploadId { get; set; }
        public string QbInvoiceNumber { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("SubClassTypeId")]
        public virtual SubClassMarketingClass SubClassMarketingClass { get; set; }

        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("SalesDataUploadId")]
        public virtual SalesDataUpload SalesDataUpload { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long CurrencyExchangeRateId { get; set; }
        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }


        [ForeignKey("AccountCreditId")]
        public virtual AccountCredit AccountCredit { get; set; }

        public long CustomerInvoiceId { get; set; }
        public string CustomerInvoiceIdString { get; set; }

        public long CustomerQbInvoiceId { get; set; }
        public string CustomerQbInvoiceIdString { get; set; }

    }
}
