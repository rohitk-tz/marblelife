using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface ICheckFactory
    {
        Check CreateDomain(CheckPaymentEditModel model);
        CheckPayment CreateCheckPayment(Check check, Payment payment);
    }
}
