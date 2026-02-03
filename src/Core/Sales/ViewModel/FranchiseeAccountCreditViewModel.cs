using Core.Application.Attribute;
using Core.Organizations.ViewModel;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeAccountCreditViewModel
    {
        public long FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public decimal? TotalSales { get; set; }
        public string CreditMemoPercentage { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public SumByCategory SumByCategory { get; set; }
    }
}
