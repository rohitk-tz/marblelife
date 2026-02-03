using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class CustomerEmailReportListModel
    {
        public long FranchiseeId { get; set; }
        public IEnumerable<CustomerEmailReportViewModel> Collection { get; set; }
        public CustomerEmailReportViewModel BestFranchisee { get; set; }
        public CustomerEmailReportViewModel Total { get; set; }
        public CustomerEmailReportFilter Filter { get; set; }
    }
}
