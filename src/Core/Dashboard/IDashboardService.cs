using Core.Billing.ViewModel;
using Core.Dashboard.Impl;
using Core.Dashboard.ViewModel;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;

namespace Core.Dashboard
{
    public interface IDashboardService
    {
        FranchiseeDirectoryListModel GetFranchiseeDirectoryList();
        SalesSummaryListModel GetSalesSummary(long franchiseeId);
        RecentInvoiceListModel GetRecentInvoices(long franchiseeId);
        IList<SalesRepLeaderboardViewModel> GetSalesRepLeaderboard(long franchiseeId, DateTime startDate, DateTime endDate);
        IList<FranchiseeLeaderboardViewModel> GetFranchiseeLeaderboard(long franchiseeId, DateTime startDate, DateTime endDate);
        List<ChartDataModel> GetChartData(long franchiseeId, DateTime startDate, DateTime endDate);
        List<ChartDataModel> GetChartDataForService(long franchiseeId, DateTime startDate, DateTime endDate);
        CustomerCountViewModel GetCustomerCount(long franchiseeId, DateTime startDate, DateTime endDate);
        IEnumerable<StartEndDateViewModel> GetPendingUploadRange(long franchiseeId);
        RecentInvoiceListModel GetUnpaidInvoices(long franchiseeId);
        AnnualUploadResponseModel GetAnnualUploadResponse(long franchiseeId);
        DocumentSummaryListModel GetDocumentsSummary(long franchiseeId);
        DocumentPendingListModel GetDocumentsPending(long franchiseeId);
        FranchiseeDirectoryListModel GetFranchiseeDirectoryListForSuperAdmin(string name);
        string Redirection(string token);
    }
}
