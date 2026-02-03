using Core.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.ViewModel
{
   public class AnnualSalesDataCustonerListModel
    {
        public IEnumerable<AnnualSalesDataCustomerViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public AnnualSalesDataListFiltercs Filter { get; set; }
    }
}
