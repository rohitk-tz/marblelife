using Core.Application.Attribute;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class EPaymentEditModel : PaymentEditModel
    {
        public ProcessorResponse ProcessorResponse { get; set; }
        public bool SaveOnFile { get; set; }

    }
}
