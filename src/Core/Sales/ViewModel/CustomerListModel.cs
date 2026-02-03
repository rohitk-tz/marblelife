using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class CustomerListModel
    {
        public IEnumerable<CustomerViewModel> Collection { get; set; }
        public CustomerListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
