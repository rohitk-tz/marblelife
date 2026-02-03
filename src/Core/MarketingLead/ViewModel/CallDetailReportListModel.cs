using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class CallDetailReportListModel
    {
        public IEnumerable<CallDetailReportViewModel> Collection { get; set; }
        public MarketingLeadReportFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
        public CallDetailReportViewModel Summary { get; set; }
        public  CallDetailReportViewModel AdjuctedTotal { get; set; }
    }
}
