using Core.Reports.ViewModel;

namespace Core.Reports
{
    public interface IGrowthReportService
    {
        GrowthReportListModel GetGrowthReport(GrowthReportFilter filter, int pageNumber, int pageSize);
        bool DownloadGrowthReport(GrowthReportFilter filter, out string fileName);
    }
}
