using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;

namespace Core.Organizations
{
    public interface IFranchiseeAccountCreditFactory
    {
        FranchiseeAccountCredit CreateDomain(FranchiseeInvoice franchiseeInvoice);
        FranchiseeAccountCredit CreateDomain(FranchiseeAccountCreditEditModel model, long franchiseeId, long personId);
    }
}
