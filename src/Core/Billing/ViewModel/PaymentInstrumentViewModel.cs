using Core.Application.Attribute;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class PaymentInstrumentViewModel
    {
        public long InstrumentId { get; set; }
        public long TypeId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string ExpirationDate { get; set; }
        public string CardType { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public long AccountTypeId { get; set; }
        public long PaymentInstrumentId { get; set; }
    }
}
