using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IChargeCardFactory
    {
        ChargeCard CreateChargeCard(ChargeCardEditModel model);
        ChargeCardPayment CreateChargeCardPayment(ProcessorResponse response, Payment payment);
    }
}
