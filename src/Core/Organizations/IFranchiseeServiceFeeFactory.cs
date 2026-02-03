using Core.MarketingLead.Domain;
using Core.MarketingLead.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IFranchiseeServiceFeeFactory
    {
        ICollection<FranchiseeServiceFeeEditModel> CreateEditModel(IEnumerable<FranchiseeServiceFee> serviceFee);
        ICollection<FranchiseeServiceFee> CreateDomain(ICollection<FranchiseeServiceFeeEditModel> serviceFeeList, Franchisee franchisee);
        ICollection<FranchiseeServiceFeeEditModel> CreateViewModel(IEnumerable<OneTimeProjectFee> projectFeeList);
        ICollection<FranchiseeServiceFeeEditModel> CreateViewModel(IEnumerable<FranchiseeLoan> loanList);
        OneTimeProjectFee CreateOneTimeProject(FranchiseeServiceFeeEditModel serviceFee); 
        FranchiseeLoan CreateFranchiseeLoan(FranchiseeServiceFeeEditModel serviceFee);
        FranchiseeServiceFee CreateDomainForServiceFee(FranchiseeServiceFeeEditModel model);
        FranchiseeLoanSchedule CreateDomain(AmortPaymentSchedule schedule);
        LoanAdjustmentAudit CreateDomain(FranchiseeChangeServiceFee franchiseeChange, FranchiseeLoan franchiseeLoan);
        FranchiseeLoanViewModel CreateViewModel(FranchiseeLoanSchedule loanSchedule);
        FranchiseeServiceFee CreateDomainForServiceFeeForCalls(Phonechargesfee model);

        AutomationClassMarketingLead CreateViewModel(MarketingLeadCallDetailV3 domain,MarketingLeadCallDetail domain2);

    }
}
