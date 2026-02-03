using Core.Reports.Domain;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class FranchiseeSalesInfoList
    {
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string CurrencyCode { get; set; } 
        public IEnumerable<FranchiseeSalesInfo> FranchiseeSalesInfo { get; set; }
    }
}
