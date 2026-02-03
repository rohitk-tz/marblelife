using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IChargeCardPaymentService
    {
        bool RollbackPayment(long accountTypeId,string transactionId);
        ProcessorResponse ChargeCardPayment(ChargeCardPaymentEditModel model, long franchiseeId, out decimal creditCardCharge, out decimal chargedAmount);
        ProcessorResponse ChargeCardOnFile(InstrumentOnFileEditModel model, ProcessorResponse paymentResponse, long franchiseeId);
        void Save(ProcessorResponse response, Payment payment);
    }
}
