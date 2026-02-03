using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class ChargeCardFactory : IChargeCardFactory
    {
        public ChargeCard CreateChargeCard(ChargeCardEditModel model)
        {
            var chargecard = new ChargeCard
            {
                Number = model.Number.Substring(model.Number.Length - 4),
                ExpiryMonth = int.Parse(model.ExpireMonth),
                ExpiryYear = int.Parse(model.ExpireYear),
                NameOnCard = model.Name,
                TypeId = model.TypeId,
                IsNew = true
            };
            return chargecard;
        }
        public ChargeCardPayment CreateChargeCardPayment(ProcessorResponse response, Payment payment)
        {
            var chargeCardPayment = new ChargeCardPayment
            {
                Id = payment.Id,
                ChargeCardId = response.InstrumentId,
                Amount = payment.Amount,
                TransactionId = response.RawResponse,
                IsNew = true
            };
            return chargeCardPayment;
        }
    }
}
