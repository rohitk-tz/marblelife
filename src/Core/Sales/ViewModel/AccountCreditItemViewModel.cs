using Core.Application.Attribute;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AccountCreditItemViewModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyRate { get; set; }
    }
}
