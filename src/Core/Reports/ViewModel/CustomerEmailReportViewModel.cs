using Core.Application.Attribute;

namespace Core.Reports.ViewModel
{
    public class CustomerEmailReportViewModel
    {
        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public double TotalCustomer { get; set; }

        [DownloadField(Required = false)]
        public double PreviousCustomers { get; set; }
        public double CustomerWithEmail { get; set; }

        [DownloadField(Required = false)]
        public double PreviousCustomerWithEmail { get; set; }
        [DownloadField(Required = false)]
        public decimal PercentagePrevious { get; set; }
        [DownloadField(Required = false)]
        public decimal PercentageCurrent { get; set; }
        public string CurrentPercentage { get; set; }
        public string PreviousPercentage { get; set; }
        public string Trend { get; set; }
    }
}
