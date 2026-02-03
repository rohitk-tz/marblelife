using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IInvoiceItemFactory
    {
        InvoiceItem CreateDomain(InvoiceItemEditModel model);
        InvoiceItemEditModel CreateViewModel(InvoiceItem domain);
    }
}
