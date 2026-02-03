using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    public class WeeklyNotificationListModel
    {
        public IEnumerable<WeeklyNotificationReportViewModel> WeeklyCollection { get; set; }
        public IEnumerable<WeeklyNotificationReportViewModel> PreviousCollection { get; set; }
        public IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> WeeklyCollectionFranchiseeWise { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public WeeklyNotificationListModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
        public string FullName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TotalAmount { get; set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
    }
}
