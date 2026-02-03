using Core.MarketingLead.ViewModel;
using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations
{
  public  interface ILeadPerformanceFranchiseeDetailsService
    {
        void Save(LeadPerformanceEditModel franchiseeViewModel,long franchiseeId);
        LeadPerformanceListModel GetFranchiseePerformance(LeadPerformanceFranchiseeFilter filter);
        LeadPerformanceNationalListModel GetLeadPerformanceNationalList(LeadPerformanceFranchiseeFilter filter);
        LeadFranchiseeNationallListModel GetSeoAndPpcNational(LeadPerformanceFranchiseeFilter filter);
        LeadFranchiseeLocalListModel GetSeoAndPpcLocal(long franchiseeId,List<long> list);
    }
}
