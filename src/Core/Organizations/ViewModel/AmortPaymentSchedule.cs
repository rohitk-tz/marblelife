using System;

namespace Core.Organizations.ViewModel
{
    public class AmortPaymentSchedule
    {
        public long Id { get; set; }
        public long LoanId { get; set; } 
        public int TermNumber { get; set; }
        public DateTime Date { get; set; }
        public double ScheduledPayment { get; set; }
        public double Interest { get; set; }
        public double TotalInterest { get; set; }
        public double Balance { get; set; }
        public double Principal { get; set; }
        public double Totalprincipal { get; set; }
    }
}
