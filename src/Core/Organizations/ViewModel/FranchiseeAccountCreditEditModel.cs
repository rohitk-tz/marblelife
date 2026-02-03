using Core.Application.Attribute;
using System;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeAccountCreditEditModel
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreditedOn { get; set; }
        public long CurrencyExchangeRateId { get; set; }
        public string Description { get; set; }
        public long TypeId { get; set; } 
    }
}
