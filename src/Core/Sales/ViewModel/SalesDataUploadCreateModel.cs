using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Sales.ViewModel
{
    public class SalesDataUploadCreateModel : EditModelBase
    {
        public long Id { get; set; }
        public long FranchiseeId { get; set; }
        public long FileId { get; set; }

        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }

        public long StatusId { get; set; }

        public decimal? AccruedAmount { get; set; }

        public FileModel File { get; set; }
        public FileModel AnnualFile { get; set; }
        public FeedbackMessageModel Message { get; set; }
        public bool IsUpdate { get; set; }
        public long AnnualUploadId { get; set; }
        public DateTime AnnualUploadStartDate { get; set; }
        public DateTime AnnualUploadEndDate { get; set; }
        public bool IsAnnualUpload { get; set; }
        public long AuditActionId { get; set; }
        public long CurrencyExchareRateId { get; set; }
        public bool IsInvoiceGenerated { get; set; }
        public string Year { get; set; }
    }
}
