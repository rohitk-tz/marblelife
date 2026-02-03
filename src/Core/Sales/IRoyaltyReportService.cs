using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface IRoyaltyReportService
    {
        RoyaltyReportListModel GetRoyaltyReport(long franchiseeId);
    }
}
