using Core.Application.Attribute;
using Core.Organizations.ViewModels;
using FluentValidation;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<RoyaltyFeeSlabsEditModel>))]
    public class RoyaltyFeeSlabsEditModelValidator : AbstractValidator<RoyaltyFeeSlabsEditModel>
    {
        public RoyaltyFeeSlabsEditModelValidator()
        {
            RuleFor(x => x.MinValue).GreaterThanOrEqualTo(0).WithMessage("Greater than equal to 0").LessThan(x => x.MaxValue).WithMessage("Should be less than max value");
            RuleFor(x => x.MaxValue).GreaterThanOrEqualTo(0).WithMessage("Greater than 'Min Value'");
            RuleFor(x => x.ChargePercentage).GreaterThan(0).WithMessage("Greater than 0");
        }
    }   


}
