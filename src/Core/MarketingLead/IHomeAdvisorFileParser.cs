using Core.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead
{
  public interface IHomeAdvisorFileParser
    {
        IList<HomeAdvisorParentModel> PrepareDomainFromDataTable(DataTable dt);
        bool CheckForValidHeader(DataTable dt, out string message);
        void PrepareHeaderIndex(DataTable dt);
    }
}
