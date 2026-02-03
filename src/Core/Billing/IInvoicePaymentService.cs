namespace Core.Billing
{
    public interface IInvoicePaymentService
    {
        void Save(long invoiceId, long paymentId);
    }
}
