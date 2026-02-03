using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IInvoiceItemService
    {
        InvoiceItem Save(InvoiceItemEditModel model);
        decimal GetRoyaltyGeneratedForGivenMonthYear(long franchiseeId, int month, int year);
    }
}
