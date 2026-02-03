using Core.Organizations.Domain;
using Core.Organizations.ViewModels;
using Core.Sales.ViewModel;
using Core.Scheduler.ViewModel;
using System.Linq;

namespace Core.Organizations
{
    public interface IFranchiseeSalesService
    {
        FranchiseeSales Save(FranchiseeSalesEditModel model);
        FranchiseeSales Get(string qbInvoiceNumber, long franchiseeId, string customerName);

        SalesDataListViewModel GetSalesData(SalesDataListFilter filter, int pageNumber, int pageSize);
        FranchiseeSales GetLastInvoiceDetails(long customerId);
        FranchiseeSales GetFranchiseeSalesByInvoiceId(long invoiceId);  
        bool DownloadSalesDataFile(SalesDataListFilter filter, out string fileName);
        FranchiseeSales GetCustomerLastFranchisee(long customerId);
        decimal GetSalesOfCustomer(long customerId);
        IQueryable<FranchiseeSales> GetSaleFilterData(SalesDataListFilter filter);

        UpdateMarketingClassListModel UpdateSalesData(UpdateMarketingClassInfoListFilter filter);
        bool DownloadInvoiceAllList(UpdateMarketingClassInfoListFilter filter, out string fileName);
        void SaveFile(CustomerFileUploadCreateModel model);
        ZipDataUploadListModel GetUpdateSalesList(ZipDataListFilter filter, int pageNumber, int pageSize);
        long GetTotalNumberOfInvoices(long customerId);
    }
}
