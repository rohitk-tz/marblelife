using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class ECheckFactory : IECheckFactory
    {
        public ECheck CreateDomain(ECheckEditModel model)
        {
            var eCheck = new ECheck
            {
                RoutingNumber = model.Number,
                AccountTypeId = model.AccountTypeId,
                Name = model.Name,
                AccountNumber = model.AccountNumber,
                BankName = model.BankName,
                IsNew = true
            };
            return eCheck;
        }

        public ECheckPayment CreateECheckPayment(ECheckEditModel model, Payment payment)
        {
            var eCheckPayment = new ECheckPayment
            {
                Id = payment.Id,
                ECheckId = model.InstrumentId,
                TransactionId = model.ProcessorResponse.RawResponse,
                IsNew = true
            };
            return eCheckPayment;
        }
    }
}
