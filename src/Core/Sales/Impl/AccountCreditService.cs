using Core.Application.Attribute;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Application;
using System.Linq;
using Core.Organizations.Domain;
using Core.Application.ViewModel;
using Core.Application.Impl;
using Core.Users.Enum;
using Core.Organizations.ViewModel;
using Core.Organizations;
using System;
using Core.Billing.Domain;
using Core.Billing.Enum;
using System.Collections.Generic;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class AccountCreditService : IAccountCreditService
    {
        private readonly IAccountCreditFactory _accountCreditFactory;
        private readonly IAccountCreditItemFactory _accountCredititemFactory;
        private readonly IRepository<AccountCredit> _accountCreditRepository;
        private readonly IRepository<AccountCreditPayment> _accountCreditPaymentRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        private readonly IClock _clock;
        private readonly IFranchiseeAccountCreditFactory _franchiseeAccountCreditFactory;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;

        public AccountCreditService(IUnitOfWork unitOfWork, IAccountCreditFactory accountCreditFactory, ISortingHelper sortingHelper,
            IAccountCreditItemFactory accountCredititemFactory, IClock clock, IFranchiseeAccountCreditFactory franchiseeAccountCreditFactory)
        {
            _accountCreditFactory = accountCreditFactory;
            _accountCredititemFactory = accountCredititemFactory;
            _sortingHelper = sortingHelper;
            _accountCreditRepository = unitOfWork.Repository<AccountCredit>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
            _clock = clock;
            _franchiseeAccountCreditFactory = franchiseeAccountCreditFactory;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _accountCreditPaymentRepository = unitOfWork.Repository<AccountCreditPayment>();
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
        }

        public AccountCreditListModel Get(AccountCreditListFilter filter, int pageNumber, int pageSize)
        {
            var salesData = _franchiseeSalesRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId) &&
            (x.AccountCreditId != null) && (string.IsNullOrEmpty(filter.CustomerName) ||
            (x.AccountCredit.Customer.Name.Contains(filter.CustomerName))) &&
            (string.IsNullOrEmpty(filter.QbInvoiceNumber) ||
            (x.AccountCredit.QbInvoiceNumber.Contains(filter.QbInvoiceNumber))) &&
            (string.IsNullOrEmpty(filter.Text) || (x.Franchisee.Organization.Name.Contains(filter.Text))
               || (x.AccountCredit.Customer.CustomerEmails.Any(m => m.Email.Contains(filter.Text)))
               || (x.AccountCredit.Customer.Address.AddressLine1.Contains(filter.Text)) || (x.AccountCredit.Customer.Address.AddressLine2.Contains(filter.Text))
               || (x.AccountCredit.Customer.Address.City.Name.Contains(filter.Text)) || (x.AccountCredit.Customer.Address.State.Name.Contains(filter.Text))
               || (x.AccountCredit.Customer.Address.Zip.Code.Contains(filter.Text)) || (x.AccountCredit.Customer.Phone.Contains(filter.Text))) &&
            ((filter.from == null || x.AccountCredit.CreditedOn >= filter.from) && (filter.to == null || x.AccountCredit.CreditedOn <= filter.to)));


            salesData = _sortingHelper.ApplySorting(salesData, x => x.AccountCredit.CreditedOn, (long)SortingOrder.Desc);
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn.ToUpper())
                {
                    case "ID":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Id, filter.SortingOrder);
                        break;
                    case "FRANCHISEENAME":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "QBINVOICENUMBER":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.AccountCredit.QbInvoiceNumber, filter.SortingOrder);
                        break;
                    case "CREDITEDON":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.AccountCredit.CreditedOn, filter.SortingOrder);
                        break;
                    case "CUSTOMERNAME":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.Name, filter.SortingOrder);
                        break;
                    case "EMAIL":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.CustomerEmails.FirstOrDefault().Email, filter.SortingOrder);
                        break;
                    case "PHONE":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.Phone, filter.SortingOrder);
                        break;
                    case "STREETADDRESS":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.Address.AddressLine1, filter.SortingOrder);
                        break;
                    case "CITY":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.Address.City.Name, filter.SortingOrder);
                        break;
                    case "STATE":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.Address.State.Name, filter.SortingOrder);
                        break;
                    case "ZIPCODE":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Customer.Address.ZipCode, filter.SortingOrder);
                        break;
                }
            }
            var result = salesData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new AccountCreditListModel
            {
                Collection = result.Select(x => _accountCreditFactory.CreateViewModel(x)),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, salesData.Count())
            };
        }
        public FranchiseeAccountCreditListModel GetAccountCreditList(AccountCreditListFilter filter, int pageNumber, int pageSize)
        {
            var franchiseeList = _franchiseeRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.Id == filter.FranchiseeId));

            franchiseeList = _sortingHelper.ApplySorting(franchiseeList, x => x.Organization.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn.ToUpper())
                {
                    case "FRANCHISEENAME":
                        franchiseeList = _sortingHelper.ApplySorting(franchiseeList, x => x.Organization.Name, filter.SortingOrder);
                        break;
                }
            }

            var result = franchiseeList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new FranchiseeAccountCreditListModel
            {
                Collection = result.Select(x => _accountCreditFactory.CreateModel(x, filter)),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, franchiseeList.Count())
            };
        }
        public AccountCredit Save(AccountCreditEditModel model, long currencyExchangerateId)
        {
            var creditMemo = _accountCreditFactory.CreateDomain(model, currencyExchangerateId);
            _accountCreditRepository.Save(creditMemo);
            return creditMemo;
        }

        public FranchiseeAccountCreditList GetFranchiseeAccountCredit(long franchiseeId, int pageNumber, int pageSize)
        {
            var accountCreditList = _franchiseeAccountCreditRepository.Table.Where(x => x.FranchiseeId == franchiseeId).OrderByDescending(x => x.CreditedOn).ToList();
            var franchisee = _franchiseeRepository.Get(franchiseeId);

            var creditHistory = _accountCreditPaymentRepository.Table.Where(x => x.FranchiseeAccountCredit.FranchiseeId == franchiseeId
                                && x.Payment != null).ToList();
            var sumByCategory = GetSumByCategory(accountCreditList);
            var result = accountCreditList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new FranchiseeAccountCreditList
            {
                Collection = result.Select(x => _accountCreditFactory.CreateViewModel(x, creditHistory)),
                Franchisee = franchisee.Organization.Name,
                Total = accountCreditList.Sum(x => x.RemainingAmount),
                CurrencyCode = franchisee.Currency,
                PagingModel = new PagingModel(pageNumber, pageSize, accountCreditList.Count()),
                SumByCategory = sumByCategory
            };
        }

        private SumByCategory GetSumByCategory(List<FranchiseeAccountCredit> accountCreditList)
        {
            var adFundAmount = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.AdFund).Select(x => x);
            return new SumByCategory()
            {
                TotalByAdFund = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.AdFund).Sum(x => x.RemainingAmount),
                TotalByTotalSales = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.AllSalesCredit).Sum(x => x.RemainingAmount),
                TotalByRoyality = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.Royalty).Sum(x => x.RemainingAmount),
            };
        }
        public void SaveAccountCredit(FranchiseeAccountCreditEditModel accountCredit, long franchiseeId, long organizationRoleUserId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var personId = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == organizationRoleUserId).UserId;
            DateTime date = _clock.UtcNow;
            var currencyExchangerate = GetCurrencyExchangeRate(franchisee, date);
            if (currencyExchangerate != null)
                accountCredit.CurrencyExchangeRateId = currencyExchangerate.Id;
            else
                accountCredit.CurrencyExchangeRateId = 1;

            var domain = _franchiseeAccountCreditFactory.CreateDomain(accountCredit, franchiseeId, personId);
            _franchiseeAccountCreditRepository.Save(domain);
        }

        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime date)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;

            var currencyExchangeRate = new CurrencyExchangeRate();
            if (countryId > 0)
            {
                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == date.Year && x.DateTime.Month == date.Month
                                        && x.DateTime.Day == date.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

                if (currencyExchangeRate == null)
                    currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First();
                return currencyExchangeRate;
            }
            else
            {
                return currencyExchangeRate;
            }
        }

        public bool DeleteAccountCredit(long accountCreditId)
        {
            var accountCredit = _franchiseeAccountCreditRepository.Get(accountCreditId);

            if (accountCredit != null)
            {
                _franchiseeAccountCreditRepository.Delete(accountCredit);
                return true;
            }
            return false;
        }

        public bool RemoveAccountCredit(long accountCreditId)
        {
            var accountCredit = _franchiseeAccountCreditRepository.Get(accountCreditId);

            if (accountCredit != null)
            {
                accountCredit.RemainingAmount = 0;
                _franchiseeAccountCreditRepository.Save(accountCredit);
                return true;
            }
            return false;
        }
        public FranchiseeAccountCreditViewModel GetCreditForInvoice(long franchiseeId, long invoiceId)
        {
            var invoice = _invoiceRepository.Get(invoiceId);
            var typeId = invoice.InvoiceItems.Any(i => i.AdFundInvoiceItem != null) ? (long)AccountCreditType.AdFund : (long)AccountCreditType.Royalty;

            var accountCreditList = _franchiseeAccountCreditRepository.Table.Where(x => x.FranchiseeId == franchiseeId
                                        ).ToList();
            var accountCreditListForCrediAmount = accountCreditList.Where(x => (x.CreditTypeId == typeId || x.CreditTypeId == (long)AccountCreditType.AllSalesCredit)).ToList();
            var franchisee = _franchiseeRepository.Get(franchiseeId);

            return new FranchiseeAccountCreditViewModel
            {
                CreditAmount = accountCreditListForCrediAmount.Sum(x => x.RemainingAmount),
                CurrencyCode = franchisee.Currency,
                 SumByCategory=GetSumByCategory(accountCreditList)
                
            };
        }
    }
}

