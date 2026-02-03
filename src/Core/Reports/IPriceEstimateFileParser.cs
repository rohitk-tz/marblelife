using Core.Scheduler.ViewModel;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports
{
    public interface IPriceEstimateFileParser
    {
        IList<PriceEstimateUploadEditModel> PrepareDomainFromDataTable(DataTable dt, bool isSuperAdmin);
        bool CheckForValidHeader(DataTable dt, out string message);
        bool CheckForValidHeaderForSA(DataTable dt, out string message);
    }
}
