using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using System.Collections.Generic;

namespace Core.Billing
{
    public interface IPaymentFactory
    {
        Payment CreateDomain(FranchiseeSalesPaymentEditModel model);
        FranchiseeSalesPaymentEditModel CreateViewModel(Payment domain, string instrumentType);
        Payment CreatePaymentDomain(decimal amount, long instrumentTypeId, long currencyExchangeRateId);
        DownloadPaymentModel  CreateModel(long invoiceId, string franchiseeName, decimal amount, string transactionDate, string paymentMode,
            string memo, string cardDetail);
        FranchiseeSalesPaymentEditModel CreateViewModel(Payment domain, string instrumentType, List<AccountCreditPaymentEditModel> franchiseeAccountCredit);
        AccountCreditPaymentEditModel CreateViewModel(AccountCreditPayment accountCreditPayment);
    }
}
