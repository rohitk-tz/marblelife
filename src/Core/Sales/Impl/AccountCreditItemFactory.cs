using System;
using Core.Application.Attribute;
using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class AccountCreditItemFactory : IAccountCreditItemFactory
    {
        public AccountCreditItem CreateDomain(AccountCreditItemEditModel model, long currencyExchangeRateId)
        {
            var domain = new AccountCreditItem();
            domain.Id = model.Id;
            domain.IsNew = model.Id < 1;
            domain.Description = model.Description;
            domain.Amount = model.Amount;
            domain.AccountCreditId = model.CreditMemoId;
            domain.CurrencyExchangeRateId = currencyExchangeRateId;
            return domain;
        }

        public AccountCreditItemViewModel CreateViewModel(AccountCreditItem domain)
        {
            var model = new AccountCreditItemViewModel();
            model.Id = domain.Id;
            model.Description = domain.Description;
            model.Amount = domain.Amount;
            model.CurrencyRate = domain.CurrencyExchangeRate != null ? domain.CurrencyExchangeRate.Rate : 1;
            return model;
        }
    }
}
