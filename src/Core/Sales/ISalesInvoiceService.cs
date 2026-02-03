using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface ISalesInvoiceService
    {
        bool DownloadInvoiceFile(SalesDataListFilter filter, out string fileName);
        void Save(CustomerFileUploadCreateModel model);
    }
}
