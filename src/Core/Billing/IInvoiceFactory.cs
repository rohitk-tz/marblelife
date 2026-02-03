using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using System;

namespace Core.Billing
{
    public interface IInvoiceFactory
    {
        Invoice CreateDomain(InvoiceEditModel model);
        InvoiceViewModel CreateViewModel(FranchiseeInvoice domain);
        DownloadInvoiceModel CreateModel(long invoiceId, DateTime startdate, DateTime enddate, DateTime date, Franchisee franchisee, InvoiceItem domain, string paymentMode);
        decimal GetSumInvoiceItembasedonItemType(Invoice invoice, InvoiceItemType type);
        decimal GetSumLateFeeBasedonItemType(Invoice invoice, LateFeeType type);
    }
}
