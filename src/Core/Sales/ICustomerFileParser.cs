using Core.Sales.ViewModel;
using System.Collections.Generic;
using System.Data;

namespace Core.Sales
{
    public interface ICustomerFileParser
    {
        IList<CustomerCreateEditModel> PrepareDomainFromDataTable(DataTable dt);
    }
}
