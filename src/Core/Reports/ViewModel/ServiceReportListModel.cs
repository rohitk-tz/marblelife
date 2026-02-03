using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class ServiceReportListModel
    {
        public IEnumerable<ServiceReportViewModel> Collection { get; set; }
        public ServiceReportListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
