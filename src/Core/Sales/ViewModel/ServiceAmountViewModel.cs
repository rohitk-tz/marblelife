using Core.Application.Attribute;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class ServiceAmountViewModel
    {
        public long ServiceId { get; set; }
        public decimal Amount { get; set; }
    }
}
