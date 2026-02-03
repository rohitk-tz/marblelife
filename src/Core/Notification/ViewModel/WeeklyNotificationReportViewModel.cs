using System;

namespace Core.Notification.ViewModel
{
    public class WeeklyNotificationReportViewModel
    {
        public long FranchiseeId { get; set; }

        public string Franchisee { get; set; }
        public long InvoiceId { get; set; }
        //public string StartDate1 { get; set; }
        //public string EndDate1 { get; set; }
        public string DueDate { get; set; }
        public string GeneratedOn { get; set; }
        public string LateFeeType { get; set; }
        public decimal LateFeeAmount { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PayableAmount { get; set; }
        public string Status { get; set; }
        public string LateFeeApplicable { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
