using System;
using System.Collections.Generic;

namespace Core.Billing.ViewModel
{
    public class FranchiseeSalesPaymentEditModel
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public decimal Amount { get; set; }
        public long InstrumentTypeId { get; set; }
        public long CurrencyExchangeRateId { get; set; }
        public IList<PaymentItemEditModel> PaymentItems { get; set; }
        public string InstrumentType { get; set; }
        public string Description { get; set; }
        public List<AccountCreditPaymentEditModel> AccountCreditPaymentEditModel { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? DebitAmount { get; set; }
        public FranchiseeSalesPaymentEditModel()
        {
            PaymentItems = new List<PaymentItemEditModel>();
        }

    }
}
