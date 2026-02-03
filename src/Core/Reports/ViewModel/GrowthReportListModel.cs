using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class GrowthReportListModel
    {
        public IEnumerable<GrowthReportViewModel> Collection { get; set; }
        public GrowthReportFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
