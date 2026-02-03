using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeAccountCreditFactory : IFranchiseeAccountCreditFactory
    {
        private readonly IClock _clock;
        public FranchiseeAccountCreditFactory(IClock clock)
        {
            _clock = clock;
        }
        public FranchiseeAccountCredit CreateDomain(FranchiseeInvoice franchiseeInvoice)
        {
            var invoice = franchiseeInvoice.Invoice;
            var creditAmount = invoice.InvoicePayments.Sum(x => x.Payment.Amount);
            var curencyExchangeRateId = invoice.InvoiceItems.Select(x => x.CurrencyExchangeRateId).First();
            var isAdFundInvoice = invoice.InvoiceItems.Any(x => x.AdFundInvoiceItem != null);

            var model = new FranchiseeAccountCredit
            {
                InvoiceId = franchiseeInvoice.InvoiceId,
                FranchiseeId = franchiseeInvoice.FranchiseeId,
                Amount = creditAmount,
                Description = null,
                CreditedOn = _clock.UtcNow,
                RemainingAmount = creditAmount,
                CurrencyExchangeRateId = curencyExchangeRateId,
                CreditTypeId = isAdFundInvoice ? (long)AccountCreditType.AdFund : (long)AccountCreditType.Royalty,
                IsNew = true,
            };
            return model;
        }

        public FranchiseeAccountCredit CreateDomain(FranchiseeAccountCreditEditModel model, long franchiseeId, long personId)
        {
            var domain = new FranchiseeAccountCredit
            {
                InvoiceId = model.InvoiceId,
                FranchiseeId = franchiseeId,
                Amount = model.Amount,
                Description = model.Description,
                CreditedOn = _clock.UtcNow,
                RemainingAmount = model.Amount,
                CurrencyExchangeRateId = model.CurrencyExchangeRateId,
                CreditTypeId = model.TypeId,
                PersonId = personId,
                IsNew = true,
            };
            return domain;
        }
    }
}
