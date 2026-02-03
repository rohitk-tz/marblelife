using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification.ViewModel
{

    public class WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel
    {
        [DisplayName("Franchisee")]
        public string Franchisee { get; set; }
        [DisplayName("1-30 days")]
        public string Thirty { get; set; }
        [DisplayName("31 to 60 days")]
        public string Sixty { get; set; }
        [DisplayName("61 to 90 days")]
        public string Ninety { get; set; }
        [DisplayName("90+ days")]
        public string moreThanNinety { get; set; }
        [DisplayName("Total")]
        public string Total { get; set; }
        [DisplayName("TotalInt")]
        public decimal TotalInt { get; set; }
    }
}
