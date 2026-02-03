using Core.Application.Attribute;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class ECheckEditModel : EPaymentEditModel
    {
        public long InstrumentId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public long AccountTypeId { get; set; }
        public long ProfileTypeId { get; set; }
    }
}
