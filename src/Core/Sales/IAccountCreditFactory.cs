using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;

namespace Core.Sales
{
    public interface IAccountCreditFactory
    {
        AccountCredit CreateDomain(AccountCreditEditModel model, long currencyExchangeRateId);

        AccountCreditViewModel CreateViewModel(FranchiseeSales domain);
        FranchiseeAccountCreditViewModel CreateModel(Franchisee franchisee, AccountCreditListFilter filter);
        FranchiseeAccountCreditModel CreateViewModel(FranchiseeAccountCredit domain, IEnumerable<AccountCreditPayment> creditHistory);
        AccountCreditPayment CreateDomain(Payment payment, FranchiseeAccountCredit accountCredit, decimal amount);
    }
}
