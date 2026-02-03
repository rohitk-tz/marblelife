namespace Core.Reports.ViewModel
{
    public class TopLeadersInfoModel
    {
        public int Rank { get; set; }
        public string Franchisee { get; set; }
        public long FranchiseeId { get; set; }
        public decimal TotalSales { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal SalesInLocalCurrency { get; set; }
        public decimal AvgSales { get; set; }
        public decimal Percentage { get; set; }
    }
}
