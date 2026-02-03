using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
  public  class LeadFranchiseeNationallListModel
    {
        public LeadFranchiseeNationallListModel()
        {
            TotalCount = new List<LeadWebSiteFranchiseeLocalViewModel>();
        }
       
        public List<LeadWebSiteFranchiseeLocalViewModel> TotalCount { get; set; }
    }
}
