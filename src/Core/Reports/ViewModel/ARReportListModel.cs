using Core.Notification.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.ViewModel
{
   public class ARReportListModel
    {
        public IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> Collection { get; set; }
        public ArReportFilter Filter { get; set; }
    }
}
