using System;

namespace Core.Billing.ViewModel
{
    public class DownloadPaymentModel
    {
        public long ApplyToInvoice { get; set; }
        public string TransactionDate { get; set; }
        public string Customer { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string CheckNumber { get; set; }
        public string Memo { get; set; }
    }
}
