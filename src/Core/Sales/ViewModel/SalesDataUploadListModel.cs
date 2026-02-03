using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesDataUploadListModel
    {
        public IEnumerable<SalesDataUploadViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public SalesDataListFilter Filter { get; set; }
    }
}
