using Core.Application.ViewModel;

namespace Core.Billing.ViewModel
{
    public abstract class PaymentEditModel : EditModelBase
    {
        public decimal Amount { get; set; }
        public long InstrumentTypeId { get; set; }
        public long InvoiceId { get; set; }
        public bool IsLoanOverPayment { get; set; }
        public decimal OverPaymentAmount { get; set; }
        public long LoanScheduleId { get; set; } 
    }
}
