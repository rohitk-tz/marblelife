using Core.Application.Attribute;
using Core.Application.ViewModel;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class ChargeCardEditModel : EditModelBase
    {
        public long TypeId { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string CVV { get; set; }

        public string ExpireMonth { get; set; }

        public string ExpireYear { get; set; }
    }
}
