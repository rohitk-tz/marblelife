using Core.Application.Attribute;
using Core.Localization.Validations;
using Core.Organizations.ViewModels;
using FluentValidation;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<FeeProfileEditModel>))]
    public class FeeProfileEditModelValidator : AbstractValidator<FeeProfileEditModel>
    {
        public FeeProfileEditModelValidator()
        {
            RuleFor(x => x.Slabs).NotNull().WithMessage(Shared.Required).NotEmpty().WithMessage(Shared.Required).When(x => x.SalesBasedRoyalty == true);
            RuleFor(x => x.MinimumRoyaltyPerMonth).NotNull().WithMessage(Shared.Required).GreaterThan(0).WithMessage(Shared.Required).When(x => x.SalesBasedRoyalty == true);
            RuleFor(x => x.AdFundPercentage).NotNull().WithMessage(Shared.Required).GreaterThan(0).WithMessage(Shared.Required).When(x => x.SalesBasedRoyalty == true);

            RuleFor(x => x.Slabs).Must((m, x) =>
            {
                if (m.SalesBasedRoyalty == false) return true;
                if (x == null || x.Count() < 1) return false;

                if (!x.All(y => y.MinValue < y.MaxValue && y.MinValue >= 0))
                    return false;

                decimal? prevMinValue = -1, prevMaxValue = -1;

                foreach (var item in x.OrderBy(y => y.MinValue).ThenBy(y => y.MaxValue).ToList())
                {
                    if (item.MinValue <= prevMinValue) return false;
                    if (item.MaxValue <= prevMaxValue) return false;
                    if (item.MinValue != prevMaxValue + 1) return false;

                    prevMinValue = item.MinValue;
                    prevMaxValue = item.MaxValue;
                }

                return true;
            });
        }
    }
}
