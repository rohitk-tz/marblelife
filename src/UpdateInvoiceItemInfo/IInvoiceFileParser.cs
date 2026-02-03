using System.Collections.Generic;
using System.Data;

namespace UpdateInvoiceItemInfo
{
    public interface IInvoiceFileParser
    {
        IList<InvoiceInfoEditModel> PrepareDomainFromDataTable(DataTable dt);
    }
}
