using Core.Application.Attribute;

namespace Core.Reports.ViewModel
{
    public class GrowthReportViewModel
    {
        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal TotalSalesLastYear { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal YTDSalesLastYear { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal YTDSalesCurrentYear { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal AmountDifference { get; set; }

        public decimal PercentageDifference { get; set; }
        [DownloadField(Required = false)]

        public decimal AmountToDisplay { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal LastYearAveragePerMonth { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal CurrentYearAveragePerMonth { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal AverageGrowth { get; set; }
        [DownloadField(Required = false)]
        public decimal AverageGrowthToDisplay { get; set; }
        public string Class { get; set; } 
        public string Service { get; set; }
    }
}
