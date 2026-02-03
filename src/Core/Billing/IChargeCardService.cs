using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IChargeCardService
    {
        ChargeCard Save(ChargeCardEditModel model);
    }
}
