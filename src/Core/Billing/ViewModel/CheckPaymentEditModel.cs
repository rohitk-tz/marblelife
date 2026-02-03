using Core.Application.Attribute;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class CheckPaymentEditModel: PaymentEditModel
    {
        public string CheckNumber { get; set; }
        public long AccountTypeId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public long CheckId { get; set; }
        public long ProfileTypeId { get; set; }
    }
}
