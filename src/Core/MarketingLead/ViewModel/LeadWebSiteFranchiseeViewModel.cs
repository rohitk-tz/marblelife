using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class LeadWebSiteFranchiseeViewModel
    {
        public LeadWebSiteFranchiseeViewModel()
        {
            WebSiteSeoCost = new List<double>();
            WebSitePpcCost = new List<double>();
            Total = new List<double>();
            Average = new List<double>();
        }
        public string FranchiseeName { get; set; }
        public bool IsExpand { get; set; }
        public bool IsZero { get; set; }
        
        public List<double> WebSiteSeoCost { get; set; }
        public List<double> WebSitePpcCost { get; set; }
        public List<double> Total { get; set; }
        public List<double> Average { get; set; }

    }
}
