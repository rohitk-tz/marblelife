using Core.Sales.Impl;
using System.Collections.Generic;
using System.Data;

namespace Core.Sales
{
    public interface IInvoiceFileParserService
    {
        IList<InvoiceInfoEditModel> PrepareDomainFromDataTable(DataTable dt);
    }
}
