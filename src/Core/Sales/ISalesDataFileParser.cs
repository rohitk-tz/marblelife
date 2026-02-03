using Core.Application.ViewModel;
using Core.Billing.Domain;
using System.Collections.Generic;
using System.Data;

namespace Core.Sales
{
    public interface ISalesDataFileParser
    {
        IList<ParsedFileParentModel> PrepareDomainFromDataTable(DataTable dt);
        bool CheckForValidHeader(DataTable dt, out string message);
        void PrepareHeaderIndex(DataTable dt);
        bool CheckForValidClassName(DataTable dt, out string result);
    }
}