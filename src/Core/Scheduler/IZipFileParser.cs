using Core.Scheduler.ViewModel;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
    public interface IZipFileParser
    {
        IList<CountyCreateEditModel> PrepareDomainFromDataTableForCounty(DataTable dt);
        IList<ZipCreateEditModel> PrepareDomainFromDataTableForZip(DataTable dt);
        bool CheckForValidCountyHeader(DataTable dt, out string message);
        bool CheckForValidZipHeader(DataTable dt, out string message);
    }
}
