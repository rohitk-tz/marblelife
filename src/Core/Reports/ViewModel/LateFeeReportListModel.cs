using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class LateFeeReportListModel
    {
        public IEnumerable<LateFeeReportViewModel> Collection { get; set; }
        public LateFeeReportFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
