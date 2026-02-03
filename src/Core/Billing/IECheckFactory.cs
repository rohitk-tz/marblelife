using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IECheckFactory
    {
        ECheck CreateDomain(ECheckEditModel model);
        ECheckPayment CreateECheckPayment(ECheckEditModel model, Payment payment);
    }
}
