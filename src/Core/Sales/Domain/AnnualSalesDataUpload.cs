using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class AnnualSalesDataUpload : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long FileId { get; set; }

        public long? SalesDataUploadId { get; set; }
        [ForeignKey("SalesDataUploadId")]
        public virtual SalesDataUpload SalesDataUpload { get; set; }

        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }

        public long StatusId { get; set; }

        public long? ParsedLogFileId { get; set; }

        public int? NoOfFailedRecords { get; set; }
        public int? NoOfParsedRecords { get; set; }
        public int? NoOfMismatchedRecords { get; set; }

        public decimal? WeeklyRoyality { get; set; }
        public decimal? AnnualRoyality { get; set; }

        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }


        public bool? IsAuditAddressParsing { get; set; }

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

        public long AuditActionId { get; set; }
        [ForeignKey("AuditActionId")]
        public virtual Lookup AuditAction { get; set; }
    }
}
