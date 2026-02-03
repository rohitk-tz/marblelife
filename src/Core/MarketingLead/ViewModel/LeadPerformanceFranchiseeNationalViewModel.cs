using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class LeadPerformanceFranchiseeNationalViewModel
    {
        public LeadPerformanceFranchiseeNationalViewModel()
        {
            CountAdjustedData = new List<long>();
            LeadWebSiteFranchiseeViewModel = new List<LeadWebSiteFranchiseeViewModel>();
            WebCount = new List<long>();
            AdjustedDataLocal = new List<decimal>();
            AdjustedDataNational = new List<decimal>();
            Total = new List<long>();
            Average = new List<double>();
            AverageForSeoPpc = new List<double>();
        }
        public bool IsExpand { get; set; }
        public bool IsZero { get; set; }
        public string FranchiseeName { get; set; }
        public List<long> CountAdjustedData { get; set; }
       
        public List<LeadWebSiteFranchiseeViewModel> LeadWebSiteFranchiseeViewModel { get; set; }
        public List<long> WebCount { get; set; }
        public List<decimal> AdjustedDataLocal { get; set; }
        public List<decimal> AdjustedDataPPC { get; set; }
        public List<decimal> AdjustedDataNational { get; set; }
        public List<long> Total { get; set; }
        public List<double> Average { get; set; }
        
        public List<double> AverageForSeoPpc { get; set; }
    }
}
