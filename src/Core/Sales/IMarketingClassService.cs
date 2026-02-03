namespace Core.Sales
{
    public interface IMarketingClassService
    {
        string GetMarketingClassByInvoiceId(long invoiceId);
        string GetMarketingClassByPaymentId(long paymentId);
        string GetSubMarketingClassByInvoiceId(long invoiceId);
    }
}
