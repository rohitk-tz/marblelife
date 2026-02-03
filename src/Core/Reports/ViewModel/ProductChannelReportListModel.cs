using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class ProductChannelReportListModel
    {
        public IEnumerable<ProductChannelReportViewModel> Collection { get; set; }
        public ProductReportListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
