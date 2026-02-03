using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    public class FranchiseeAccountCreditModel
    {
        public long AccountCreditId { get; set; }
        public long? InvoiceId { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal RemainingAmount { get; set; } 
        public DateTime CreditedOn { get; set; }
        public decimal CurrencyRate { get; set; }
        public string Description { get; set; }
        public decimal ClearedAmount { get; set; }
        public string CreditType { get; set; } 
        public IEnumerable<FranchiseeAccountCreditPaymentViewModel> CreditHistory { get; set; } 
        public string CreditedBy { get; set; }
    }
}
