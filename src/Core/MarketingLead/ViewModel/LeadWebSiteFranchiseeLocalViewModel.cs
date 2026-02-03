using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
  public  class LeadWebSiteFranchiseeLocalViewModel
    {
        public LeadWebSiteFranchiseeLocalViewModel()
        {
            CountLocal = new List<double>();
        }
        public bool IsZero { get; set; }
        public string FranchiseeName { get; set; }
        public List<double> CountLocal { get; set; }
        
    }
}
