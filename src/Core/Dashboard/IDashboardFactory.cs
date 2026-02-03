using Core.Billing.Domain;
using Core.Dashboard.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;

namespace Core.Dashboard
{
    public interface IDashboardFactory
    {
        FranchiseeDirectoryViewModel CreateViewModel(Organization domain);
        SalesSummaryViewModel CreateViewModel(SalesDataUpload domain);
        RecentInvoiceViewModel CreateViewModel(FranchiseeInvoice domain);
        DocumentSummaryViewModel CreateViewModel(FranchiseDocument domain, long franchiseeId);
        DocumentType CreateViewModel(DocumentType domain, long franchiseeId);
    }
}
