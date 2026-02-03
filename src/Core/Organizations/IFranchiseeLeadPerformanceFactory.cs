using Core.MarketingLead.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations
{
   public interface IFranchiseeLeadPerformanceFactory
    {
        LeadPerformanceEditModel CreateEditModel(LeadPerformanceFranchiseeDetails franchiseeTechMailService);

        LeadPerformanceFranchiseeDetails CreateDomain( double amount, LeadPerformanceEnum leadPerformanceEnum,long? franchiseeId);
        LeadPerformanceFranchiseeViewModel CreateViewModel(LeadPerformanceFranchiseeDetails franchiseeTechMailService, OrganizationRoleUser organizationRoleUser);
    }
}
