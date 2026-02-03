using Core.Organizations.ViewModel;
using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface IAccountCreditService
    {
        AccountCreditListModel Get(AccountCreditListFilter filter, int pageNumber, int pageSize);
        AccountCredit Save(AccountCreditEditModel model, long currencyExchangeRateId);
        FranchiseeAccountCreditListModel GetAccountCreditList(AccountCreditListFilter filter, int pageNumber, int pageSize);
        FranchiseeAccountCreditList GetFranchiseeAccountCredit(long franchiseeId, int pageNumber, int pageSize);
        void SaveAccountCredit(FranchiseeAccountCreditEditModel accountCredit, long franchiseeId, long organizationRoleUserId);
        bool DeleteAccountCredit(long accountCreditId);
        bool RemoveAccountCredit(long accountCreditId);
        FranchiseeAccountCreditViewModel GetCreditForInvoice(long franchiseeId, long invoiceId);
    }
}
