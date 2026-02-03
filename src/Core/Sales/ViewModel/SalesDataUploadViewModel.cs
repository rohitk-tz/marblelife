using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesDataUploadViewModel
    {
        public long Id { get; set; }
        public long FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public string Frequency { get; set; }
        public long FileId { get; set; }
        public long? LogFileId { get; set; }

        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public DateTime UploadedOn { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? AccruedAmount { get; set; }
        public int? NumberOfCustomers { get; set; }
        public int? NumberOfInvoices { get; set; }
        public string Status { get; set; }
        public long StatusId { get; set; }

        public int? FailedRecords { get; set; }
        public int? MismatchedRecords { get; set; } 
        public decimal CurrencyRate { get; set; }
        public string UploadedBy { get; set; }
        public string Currency { get; set; }
        public long AuditStatusId { get; set; }
        public string AuditStatus { get; set; }
        public decimal? WeeklyRoyality { get; set; }
        public decimal? AnnualRoyality { get; set; }
        public decimal? TotalCredit { get; set; }
        public decimal? TotalDebit { get; set; }
    }
}
