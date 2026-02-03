using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
  public  class LeadFranchiseeLocalListModel
    {
        public LeadFranchiseeLocalListModel()
        {
            AdjustedDataLocal = new List<double>();
            AdjustedDataNational = new List<double>();
            WebCount = new List<double>();
            LeadWebSiteFranchiseeViewModel = new List<LeadWebSiteFranchiseeViewModel>();
            Average = new List<double>();
        }
        public List<LeadWebSiteFranchiseeViewModel> LeadWebSiteFranchiseeViewModel { get; set; }
        public List<double> WebCount { get; set; }
        public List<double> AdjustedDataLocal { get; set; }
        public List<double> AdjustedDataNational { get; set; }
        public List<double> Average { get; set; }
    }
}
