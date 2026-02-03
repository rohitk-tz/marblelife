using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    public class FranchiseeAccountCreditList
    {
        public IEnumerable<FranchiseeAccountCreditModel> Collection { get; set; }
        public string Franchisee { get; set; }
        public decimal Total { get; set; }
        public string CurrencyCode { get; set; }
        public PagingModel PagingModel { get; set; }
        public SumByCategory SumByCategory { get; set; }
    }

    public class SumByCategory
    {
        public decimal TotalByRoyality { get; set; }
        public decimal TotalByAdFund { get; set; }
        public decimal TotalByTotalSales { get; set; }
    }
}
