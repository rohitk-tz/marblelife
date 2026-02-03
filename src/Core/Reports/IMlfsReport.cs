using Core.Reports.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports
{
    public interface IMlfsReport
    {
        MLFSReportListModel GetReportForPurchase(MLFSReportListFilter filter);
        MLFSReportListModel GetReportForSale(MLFSReportListFilter filter);
        MLFSReportConfigurationListModel GetMLFSConfiguration(MLFSConfigurationFilter filter);
        bool SaveMLFSConfiguration(MLFSEditModel editModel);
    }
}
