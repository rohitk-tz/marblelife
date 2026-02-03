using Core.Application.Attribute;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class SalesRepLeaderboardViewModel
    {
        public string SalesRep { get; set; }

        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }

        public int NoOfJobs { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal AvgSales { get; set; }
    }

}
