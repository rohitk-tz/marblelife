using Core.Application.Attribute;
using System;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class ChargeCardPaymentEditModel: EPaymentEditModel
    {
        public ChargeCardEditModel ChargeCardEditModel { get; set; }        

        public long ProfileTypeId { get; set; }

    }
}
