using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IPaymentItemFactory
    {
        PaymentItem CreatePaymentItemDomain(PaymentItemEditModel model);
        PaymentItemEditModel CreatePaymentItemModel(PaymentItem domain);
    }
}
