using System;

namespace Core.Organizations.ViewModel
{
    public class LoanScheduleViewModel
    {
        public long Id { get; set; }
        public int LoanTerm { get; set; }
        public long LoanId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }
        public decimal Principal { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal TotalPrincipal { get; set; }
        public string InvoiceItemId { get; set; }
        public decimal OverPayment { get; set; }
        public long CurrencyExchangeRateId { get; set; }
        public decimal CurrencyRate { get; set; }
        public string LoanAdjustment { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
