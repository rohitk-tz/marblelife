namespace Core.Notification.ViewModel
{
    public class WeeklyUnpaidInvoiceNotificationReportModel
    {
        public long InvoiceId { get; set; }
        public string Franchisee { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string LateFeeApplicable { get; set; }
        public decimal PayableAmount { get; set; }
    }
}
