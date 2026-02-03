using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class LateFeeReportViewModel
    {
        public long InvoiceId { get; set; }
        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public string LateFeeType { get; set; }
        [DownloadField(Required = false)]
        public long LateFeeTypeId { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal LateFeeAmount { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal InterestRate { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal PayableAmount { get; set; }
        [DownloadField(Required = false)]
        public string CurrencyCode { get; set; }
        [DownloadField(Required = false)]
        public decimal CurrencyRate { get; set; }
        public string Status { get; set; }
    }
}
