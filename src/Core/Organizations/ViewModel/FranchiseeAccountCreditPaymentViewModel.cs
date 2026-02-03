using System;

namespace Core.Organizations.ViewModel
{
    public class FranchiseeAccountCreditPaymentViewModel
    {
        public long AccountCreditId { get; set; }
        public long AccountCreditPaymentId { get; set; } 
        public long PaymentId { get; set; }
        public string InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal CurrencyRate { get; set; }
    }
}
