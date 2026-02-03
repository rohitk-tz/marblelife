using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface IAccountCreditItemFactory
    {
        AccountCreditItem CreateDomain(AccountCreditItemEditModel model, long currencyExchangeRateId);

        AccountCreditItemViewModel CreateViewModel(AccountCreditItem domain);
    }
}
