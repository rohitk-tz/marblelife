using AuthorizeNet.Api.Contracts.V1;
using Core.Application;
using Core.Billing.ViewModel;

namespace Infrastructure.Billing
{
    public interface IAuthorizeNetCustomerProfileService
    {
        createCustomerProfileResponse CreateNewProfile(long accountTypeId, string cardNumber, string cvv, string expiryDate, long payeeId, string email);
        createCustomerPaymentProfileResponse CreateAdditionalPaymentProfile(long accountTypeId, string customerProfileId, string cardNumber, string cvv, string expiryDate);
        deleteCustomerPaymentProfileResponse DeleteCustomerProfile(long accountTypeId, string profileId, string paymentProfileId);
        ANetApiResponse ChargeNewCard(long accountTypeId, string cardNumber, string cvv, string expiryDate, long invoiceId, decimal amount, long payeeId, string name, string franchiseeName);
        ANetApiResponse ChargeCustomerProfile(long accountTypeId, string customerProfileId, string customerPaymentProfileId, long invoiceId, decimal amount,string franchiseeName);
        ANetApiResponse VoidPayment(long accountTypeId, string transactionId);
        createTransactionResponse DebitBankAccount(long accountTypeId, decimal amount, string accountNumber, string routingNumber, string name,
            string bankName, long invoiceId,string franchiseebame);
        createCustomerProfileResponse CreateECheckProfile(long accountTypeId, string accountNumber, string nameOnAccount, string routingNumber, string bankName, long payeeId, string email);
        createCustomerPaymentProfileResponse CreateAdditionalEChaekPaymentProfile(long accountTypeId, string customerProfileId, string accountNumber, string nameOnAccount, string routingNumber, string bankName);
        bool IfErrorHandleErrorResponse(ANetApiResponse chargeCardResponse, ProcessorResponse processorResponse, long franchiseeId, ILogService _logService);
    }
}
