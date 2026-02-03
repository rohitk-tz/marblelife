using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IPaymentService
    {
        ProcessorResponse MakePaymentByNewChargeCard(ChargeCardPaymentEditModel model, long franchiseeId, long invoiceId);
        ProcessorResponse MakePaymentOnFileChargeCard(InstrumentOnFileEditModel model, long franchiseeId, long invoiceId);
        ProcessorResponse MakePaymentByECheck(ECheckEditModel model, long franchiseeId, long invoiceId);
        decimal AccountCreditPayment(decimal amount, Invoice invoice, long franchiseeId);
        ProcessorResponse AdjustAccountCredit(long franchiseeId, long invoiceId);
        void CreateOverPaymentInvoiceItem(decimal amount, long franchiseeId, long invoiceId);
    }
}
