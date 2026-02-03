using Core.Reports.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Reports
{
    public interface IProductReportService
    {
        ProductChannelReportListModel GetReport(ProductReportListFilter filter, int pageNumber, int pageSize);
        bool DownloadReport(ProductReportListFilter filter, out string fileName);
        ProductReportChartListModel GenerateChartData(ProductReportListFilter filter);
        IEnumerable<Tuple<int, int>> MonthsBetween(DateTime startDate, DateTime endDate);

    }
}