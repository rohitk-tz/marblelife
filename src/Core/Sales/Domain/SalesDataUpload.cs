using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class SalesDataUpload : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long FileId { get; set; }

        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }

        public long StatusId { get; set; }

        public long? ParsedLogFileId { get; set; }
        public int? NumberOfCustomers { get; set; }

        public int? NumberOfInvoices { get; set; }

        public int? NumberOfFailedRecords { get; set; }
        public int? NumberOfParsedRecords { get; set; }

        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }

        public decimal? AccruedAmount { get; set; }

        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }


        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public long CurrencyExchangeRateId { get; set; }
        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }

        public bool IsActive { get; set; }
        public bool IsInvoiceGenerated { get; set; } 
    }
}
