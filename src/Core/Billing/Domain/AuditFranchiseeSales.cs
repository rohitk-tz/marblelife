using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Billing.Domain
{
    public class AuditFranchiseeSales : DomainBase
    {
        public long? FranchiseeId { get; set; }
        public long? CustomerId { get; set; }
        public long? AuditInvoiceId { get; set; }
        public long? AccountCreditId { get; set; }
        public long ClassTypeId { get; set; }
        public string SalesRep { get; set; }
        public decimal? Amount { get; set; }
        public long? annualSalesDataUploadId { get; set; }
        public string QbInvoiceNumber { get; set; }
        //public long DataRecorderMetaDataId { get; set; }

        public long? AuditCustomerId { get; set; }
        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("AuditInvoiceId")]
        public virtual AuditInvoice AuditInvoice { get; set; }

        [ForeignKey("annualSalesDataUploadId")]
        public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; }

        //[ForeignKey("DataRecorderMetaDataId")]
        //public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long CurrencyExchangeRateId { get; set; }
        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }


        [ForeignKey("AccountCreditId")]
        public virtual AccountCredit AccountCredit { get; set; }

        [ForeignKey("AuditCustomerId")]
        public virtual AuditCustomer AuditCustomer { get; set; }

        public long CustomerInvoiceId { get; set; }
        public string CustomerInvoiceIdString { get; set; }

        public string CustomerQbInvoiceId { get; set; }
    }


}
