using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class WebLeadReportListModel
    {
        public IEnumerable<WebLeadReportViewModel> Collection { get; set; }
        public MarketingLeadReportFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
