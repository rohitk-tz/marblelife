
using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeEmailEditModel
    {
        public long Id { get; set; }
        public int TechnianCount { get; set; }
        public bool IsGeneric { get; set; }
        public double Amount { get; set; }
        public bool isTechMailFees { get; set; }

        public double MultiplacationFactor { get; set; }

        public double? ChargesForPhone { get; set; }

        public long ChardsForPhoneId { get; set; }
    }
}
