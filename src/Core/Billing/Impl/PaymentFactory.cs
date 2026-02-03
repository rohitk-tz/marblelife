using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Application.Attribute;
using Core.Billing.Enum;
using System.Collections.Generic;
using System.Linq;
using Core.Application;
using Core.Organizations.Domain;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class PaymentFactory : IPaymentFactory
    {
        private readonly IPaymentItemFactory _paymentItemFactory;
        private readonly IClock _clock;

        public PaymentFactory(IPaymentItemFactory paymentItemFactory, IClock clock)
        {
            _clock = clock;
            _paymentItemFactory = paymentItemFactory;
        }
        public Payment CreateDomain(FranchiseeSalesPaymentEditModel model)
        {
            return new Payment()
            {
                Id = model.Id,
                Date = model.Date,
                Amount = model.Amount,
                InstrumentTypeId = (long)InstrumentType.Cash,
                CurrencyExchangeRateId = model.CurrencyExchangeRateId,
                PaymentItems = (model.PaymentItems != null) ? new List<PaymentItem>(model.PaymentItems.Select(x => _paymentItemFactory.CreatePaymentItemDomain(x))) : null,
                IsNew = model.Id <= 0,
            };
        }
        public FranchiseeSalesPaymentEditModel CreateViewModel(Payment domain, string instrumentType)
        {
            return new FranchiseeSalesPaymentEditModel()
            {
                Id = domain.Id,
                Date = domain.Date,
                DateString = domain.Date != null ? domain.Date.ToShortDateString() : "",
                Amount = domain.Amount,
                InstrumentTypeId = (domain.InstrumentTypeId <= 0) ? (long)InstrumentType.Cash : domain.InstrumentTypeId,
                InstrumentType = instrumentType != null ? instrumentType : null,
                PaymentItems = domain.PaymentItems != null ? domain.PaymentItems.Select(_paymentItemFactory.CreatePaymentItemModel).ToList() : new List<PaymentItemEditModel>()
            };
        }
        public Payment CreatePaymentDomain(decimal amount, long instrumentTypeId, long currencyExchangeRateId)
        {
            return new Payment
            {
                Amount = amount,
                IsNew = true,
                Date = _clock.UtcNow,
                InstrumentTypeId = instrumentTypeId,
                CurrencyExchangeRateId = currencyExchangeRateId
            };
        }
        public DownloadPaymentModel CreateModel(long invoiceId, string franchiseeName, decimal amount, string transactionDate, string paymentMode,
            string memo, string cardDetail)
        {
            return new DownloadPaymentModel
            {
                Amount = amount,
                TransactionDate = transactionDate,
                ApplyToInvoice = invoiceId,
                Customer = franchiseeName,
                PaymentMethod = paymentMode,
                CheckNumber = cardDetail,
                Memo = memo
            };
        }

        public FranchiseeSalesPaymentEditModel CreateViewModel(Payment domain, string instrumentType
            , List<AccountCreditPaymentEditModel> franchiseeAccountCredit)
        {
            return new FranchiseeSalesPaymentEditModel()
            {
                Id = domain.Id,
                Date = domain.Date,
                DateString = domain.Date != null ? domain.Date.ToShortDateString() : "",
                Amount = domain.Amount,
                InstrumentTypeId = (domain.InstrumentTypeId <= 0) ? (long)InstrumentType.Cash : domain.InstrumentTypeId,
                InstrumentType = instrumentType != null ? instrumentType : null,
                PaymentItems = domain.PaymentItems != null ? domain.PaymentItems.Select(_paymentItemFactory.CreatePaymentItemModel).ToList() : new List<PaymentItemEditModel>(),
                 AccountCreditPaymentEditModel= franchiseeAccountCredit

            };
        }

        public AccountCreditPaymentEditModel CreateViewModel(AccountCreditPayment accountCreditPayment)
        {
            return new AccountCreditPaymentEditModel()
            {
                Amount = accountCreditPayment.Amount,
                Description = ("(Credit #" + accountCreditPayment.FranchiseeAccountCreditId + ") ") + (accountCreditPayment.FranchiseeAccountCredit != null ? accountCreditPayment.FranchiseeAccountCredit.Description : ""),
                AccountCreditId = accountCreditPayment.FranchiseeAccountCredit != null ?
                accountCreditPayment.FranchiseeAccountCredit.Id.ToString():""
            };
        }
    }
}
