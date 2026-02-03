using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;


namespace Core.Sales
{
   public interface IUpdateInvoiceFileParser
    {
        IList<UpdateInvoiceEditModel> PrepareDomainFromDataTableForUpdateInvoice(DataTable dt);
        bool CheckForValidHeader(DataTable dt, out string message);
    }
}
