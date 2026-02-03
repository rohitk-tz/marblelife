using Core.Application.Attribute;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AccountCreditViewModel
    {
        public long Id { get; set; }

        public DateTime CreditedOn { get; set; }
        public CustomerViewModel Customer { get; set; }
        public string QbInvoiceNumber { get; set; }
        public long FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
    }
}
