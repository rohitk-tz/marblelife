using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Organizations.ViewModel;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesDataListViewModel
    {
        public IEnumerable<FranchiseeSalesViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public SalesDataListFilter Filter { get; set; }
    }
}
