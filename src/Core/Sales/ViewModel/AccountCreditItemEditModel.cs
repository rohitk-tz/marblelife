using Core.Application.Attribute;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AccountCreditItemEditModel 
    {
        public long Id { get; set; }
        public long CreditMemoId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public long CurrencyExchangeRateId { get; set; }
    }
}
