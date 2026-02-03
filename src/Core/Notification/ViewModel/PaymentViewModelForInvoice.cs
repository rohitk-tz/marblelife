namespace Core.Notification.ViewModel
{
    public class PaymentViewModelForInvoice
    {
        public long InvoiceId { get; set; }
        public string GeneratedOn { get; set; }
        public string DueDate { get; set; }
        public string Amount { get; set; }
        public string  AdFund { get; set; }
        public string Royalty { get; set; }
    }
}
