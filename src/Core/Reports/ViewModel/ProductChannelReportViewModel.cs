using Core.Application.Attribute;

namespace Core.Reports.ViewModel 
{
    [NoValidatorRequired]
    public class ProductChannelReportViewModel
    {
        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string Service { get; set; }

        [DownloadField(Required = false)]
        public long? ServiceTypeId { get; set; }

        [DownloadField(CurrencyType = "$")]
        public decimal TotalSales { get; set; }
    }
}
