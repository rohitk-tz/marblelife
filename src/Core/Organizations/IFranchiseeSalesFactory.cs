using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.ViewModel;

namespace Core.Organizations
{
    public interface IFranchiseeSalesFactory
    {
        FranchiseeSalesViewModel CreateViewModel(FranchiseeSales domain);
        FranchiseeSales CreateDomain(FranchiseeSalesEditModel model);
        FranchiseeSalesEditModel CreateModel(long franchiseeId, long invoiceId, long customerId, string qbInvoiceNumber, long salesDataUploadId, decimal amount);

        UpdateMarketingClassViewModel CreateDomainModel(FranchiseeSales domain);
        UpdateMarketingClassViewModel CreateDomainInvoiceModel(InvoiceItem invoiceItem, FranchiseeSales domain);
        DownloadAllInvoiceModel CreateDomainDownloadInvoiceModel(UpdateMarketingClassViewModel model);
        UpdateMarketingClassfileupload CreateViewModel(CustomerFileUploadCreateModel model);
        ZipDataUploadViewModel CreateViewModel(UpdateMarketingClassfileupload model);
    }
}
