using Core.Application.Attribute;
using Core.Organizations.ViewModel;
using System.Collections.Generic;


namespace Core.MarketingLead.ViewModel
{
    public class LeadPerformanceNationalListModel
    {
        public LeadPerformanceNationalListModel()
        {
            LeadPerformanceFranchiseeNationalViewModel = new List<LeadPerformanceFranchiseeNationalViewModel>();
            Months = new List<string>();
        }
        public List<LeadPerformanceFranchiseeNationalViewModel> LeadPerformanceFranchiseeNationalViewModel { get; set; }
        public List<string> Months { get; set; }

        public List<double> TotalOfLeadCount { get; set; }

    }
}
