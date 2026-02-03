using Core.Application.Attribute;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Core.Organizations.Domain;
using Core.Application;
using Core.Organizations.ViewModel;
using Core.Billing.Domain;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class AccountCreditFactory : IAccountCreditFactory
    {
        private readonly IAccountCreditItemFactory _creditMemoItemFactory;
        private readonly ICustomerFactory _customerFactory;
        private readonly IRepository<FranchiseeSales> _franchiseeSales;
        private readonly IRepository<AccountCredit> _accountCreditRepository;
        public AccountCreditFactory(IUnitOfWork unitOfWork, IAccountCreditItemFactory creditMemoItemFactory, ICustomerFactory customerFactory)
        {
            _creditMemoItemFactory = creditMemoItemFactory;
            _customerFactory = customerFactory;
            _franchiseeSales = unitOfWork.Repository<FranchiseeSales>();
            _accountCreditRepository = unitOfWork.Repository<AccountCredit>();
        }

        public AccountCredit CreateDomain(AccountCreditEditModel model, long currencyExchangeRateId)
        {
            return new AccountCredit
            {
                CreditedOn = model.CreditedOn,
                CustomerId = model.CustomerId,
                QbInvoiceNumber = model.QbInvoiceNumber,
                IsNew = model.Id < 1,
                CreditMemoItems = model.AccountCreditItems != null ?
                                  new List<AccountCreditItem>(model.AccountCreditItems.Select(x => _creditMemoItemFactory.CreateDomain(x, currencyExchangeRateId)))
                                    : null
            };
        }

        public AccountCreditViewModel CreateViewModel(FranchiseeSales domain)
        {
            if (domain.AccountCreditId < 0)
                domain.AccountCreditId = 0;
            var creditDetails = domain.AccountCredit.CreditMemoItems.Select(x => _creditMemoItemFactory.CreateViewModel(x));

            var viewModel = new AccountCreditViewModel();
            viewModel.Id = domain.AccountCreditId.Value;
            viewModel.CreditedOn = domain.AccountCredit.CreditedOn;
            viewModel.Customer = _customerFactory.CreateViewModel(domain.Customer);
            viewModel.QbInvoiceNumber = domain.QbInvoiceNumber;
            viewModel.FranchiseeName = domain.Franchisee.Organization.Name;
            viewModel.FranchiseeId = domain.FranchiseeId;
            viewModel.Amount = creditDetails != null ? creditDetails.Sum(x => x.Amount) : 0;
            viewModel.Description = creditDetails != null ? string.Join(",", creditDetails.Select(x => x.Description)) : null;
            return viewModel;
        }

        public FranchiseeAccountCreditViewModel CreateModel(Franchisee franchisee, AccountCreditListFilter filter)
        {
            var model = new FranchiseeAccountCreditViewModel();
            var franchiseeSales = _franchiseeSales.Table.Where(x => x.FranchiseeId == franchisee.Id && x.AccountCredit != null && x.AccountCredit.CreditMemoItems.Any()
                                     && (filter.Month == x.AccountCredit.CreditedOn.Month)
            && (filter.Year == x.AccountCredit.CreditedOn.Year)
            ).ToArray();
            model = CreateViewModel(franchiseeSales, franchisee);
            return model;
        }

        private FranchiseeAccountCreditViewModel CreateViewModel(IEnumerable<FranchiseeSales> franchiseesales, Franchisee franchisee)
        {
            var totalSales = franchisee.FranchiseeSales.Sum(x => x.Amount);

            var totalCreditAmount = franchiseesales.Select(x => x.AccountCredit.CreditMemoItems.Sum(z => z.Amount)).Sum();
            var currencyRate = franchisee.SalesDataUploads.Select(x => x.CurrencyExchangeRate).FirstOrDefault();

            decimal? percentageCredit = 0;
            string credit = null;
            if (totalCreditAmount > 0)
            {
                percentageCredit = totalCreditAmount / totalSales * 100;
                credit = string.Format("{0:0.00}", percentageCredit);
            }
            var model = new FranchiseeAccountCreditViewModel();
            model.CreditAmount = totalCreditAmount;
            model.CreditMemoPercentage = credit;
            model.FranchiseeId = franchisee.Id;
            model.FranchiseeName = franchisee.Organization.Name;
            model.TotalSales = totalSales;
            model.CurrencyCode = franchisee.Currency;
            model.CurrencyRate = currencyRate != null ? currencyRate.Rate : 1;
            return model;
        }

        public FranchiseeAccountCreditModel CreateViewModel(FranchiseeAccountCredit domain, IEnumerable<AccountCreditPayment> creditHistory)
        {
            var currencyExchangerate = domain.CurrencyExchangeRate != null ? domain.CurrencyExchangeRate.Rate : 1;
            var accountCreditHistory = creditHistory.Where(x => x.FranchiseeAccountCreditId == domain.Id);

            var clearAmount = domain.RemainingAmount == 0 ? ((domain.Amount > accountCreditHistory.Sum(x => x.Amount)) ? domain.Amount - accountCreditHistory.Sum(x => x.Amount) : 0) : 0;
            return new FranchiseeAccountCreditModel
            {
                InitialAmount = domain.Amount,
                RemainingAmount = domain.RemainingAmount,
                AccountCreditId = domain.Id,
                InvoiceId = domain.InvoiceId < 1 ? null : domain.InvoiceId,
                CreditedOn = domain.CreditedOn,
                CurrencyRate = currencyExchangerate,
                Description = domain.Description,
                ClearedAmount = clearAmount,
                CreditType = domain.CreditType != null ? domain.CreditType.Name : null,
                CreditHistory = accountCreditHistory.Any() ? accountCreditHistory.Select(x => CreateViewModel(x)) : null,
                CreditedBy = domain != null && domain.Person != null ? string.Join(" ", new[] { domain.Person.FirstName, domain.Person.MiddleName, domain.Person.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))) : null
            };
        }

        public AccountCreditPayment CreateDomain(Payment payment, FranchiseeAccountCredit accountCredit, decimal amount)
        {
            var accountCreditPayment = new AccountCreditPayment
            {
                PaymentId = payment.Id,
                FranchiseeAccountCreditId = accountCredit.Id,
                Amount = amount,
                IsNew = true
            };
            return accountCreditPayment;
        }

        private FranchiseeAccountCreditPaymentViewModel CreateViewModel(AccountCreditPayment domain)
        {
            var currencyExchangerate = (domain.Payment.CurrencyExchangeRate != null && domain.Payment.CurrencyExchangeRate.Rate >= 0)
                                         ? domain.Payment.CurrencyExchangeRate.Rate : 1;

            var model = new FranchiseeAccountCreditPaymentViewModel
            {
                AccountCreditId = domain.FranchiseeAccountCreditId,
                AccountCreditPaymentId = domain.Id,
                PaymentId = domain.PaymentId,
                PaymentDate = domain.Payment.Date,
                CurrencyRate = currencyExchangerate,
                Amount = domain.Amount
            };
            var invoiceIds = domain.Payment.InvoicePayments.Select(x => x.InvoiceId).ToList();
            if (invoiceIds.Any())
            {
                model.InvoiceId = string.Join(", ", invoiceIds.Select(x => x.ToString()));
            }
            return model;
        }
    }
}
