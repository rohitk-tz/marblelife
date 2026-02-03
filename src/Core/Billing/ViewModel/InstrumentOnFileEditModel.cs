using Core.Application.Attribute;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class InstrumentOnFileEditModel : EPaymentEditModel
    {
        public string CustomerProfileId { get; set; }
        public string PaymentProfileId { get; set; }
        public long InstrumentId { get; set; }
        public long ProfileTypeId { get; set; }
    }
}
