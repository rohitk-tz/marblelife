using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Sales.ViewModel;

namespace Core.Billing
{
    public interface IInvoiceService
    {
        Invoice Save(InvoiceEditModel model);
        void SavePaymentItem(Invoice inDb, FranchiseeSalesPaymentEditModel model);
        InvoiceDetailsViewModel InvoiceDetails(long invoiceId);
        InvoiceListModel GetInvoiceList(InvoiceListFilter filter, int pageNumber, int pageSize);
        InvoiceDetailsViewModel FranchiseeInvoiceDetails(long invoiceId);
        bool DownloadInvoiceListFile(long[] invoiceIds, out string fileName);
        DeleteInvoiceResponseModel DeleteInvoiceItem(long invoiceItemId);
        bool DownloadInvoiceListAllFile(InvoiceListFilter filter, out string fileName);
        bool CreateExcelAdfund(long[] invoiceIds, out string fileName);
        bool CreateExcelRoyality(long[] invoiceIds, out string fileName);
        bool CreateExcelForAllFiles(InvoiceListFilter filter, out string fileName);
        InvoiceListModel GetDownloadedInvoiceList(long[] invoiceIds);
        bool MarkInvoicesAsDownloaded(long[] invoiceIds);
        InvoiceDetailsViewModel InvoicePaymentDetails(long invoiceid);
        bool SaveInvoiceReconciliationNotes(InvoiceReconciliationNotesModel model);
    }
}
