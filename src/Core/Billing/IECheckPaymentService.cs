using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IECheckPaymentService
    {
        ProcessorResponse MakeECheckPayment(ECheckEditModel model, long franchiseeId);
        void Save(ECheckEditModel model, Payment payment);
    }
}
