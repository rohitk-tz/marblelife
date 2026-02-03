using Core.Application.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Core.Sales.ViewModel
{
    public class AnnualAuditSalesListModel
    {
        public IEnumerable<Auditaddress> Collection { get; set; }
        public SalesDataListFilter Filter { get; set; }
        public List<AnnualGroupedReport> GroupCollection { get; set; }

    }
}
