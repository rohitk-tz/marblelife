using Core.Application.Attribute;
using Core.Geo.ViewModel;
using Core.Localization.Validations;
using FluentValidation;

namespace Core.Geo.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<AddressEditModel>))]
    public class AddressEditModelValidator : AbstractValidator<AddressEditModel>
    {
        public AddressEditModelValidator()
        {
            RuleFor(x => x.AddressLine1).NotNull().WithMessage(Shared.Required).NotEmpty().WithMessage(Shared.Required).Length(1, 128).WithMessage(string.Format(Shared.MaxCharLength, 128)).When(x => !AddressEditModel.IsNullOrEmpty(x));
            RuleFor(x => x.AddressLine2).Length(0, 128).WithMessage(string.Format(Shared.MaxCharLength, 128)).When(x => !AddressEditModel.IsNullOrEmpty(x));

            RuleFor(x => x.CountryId).GreaterThan(0).WithMessage(Shared.Required).When(x => !AddressEditModel.IsNullOrEmpty(x));
            //RuleFor(x => x.StateId).GreaterThan(0).WithMessage(Shared.Required).When(x => !AddressEditModel.IsNullOrEmpty(x));
            RuleFor(x => x.State)
               .NotNull()
               .WithMessage(Shared.Required)
               .NotEmpty()
               .WithMessage(Shared.Required)
               .When(x => !AddressEditModel.IsNullOrEmpty(x));
            RuleFor(x => x.City)
                .NotNull()
                .WithMessage(Shared.Required)
                .NotEmpty()
                .WithMessage(Shared.Required)
                .When(x => !AddressEditModel.IsNullOrEmpty(x));
            RuleFor(x => x.ZipCode).NotNull().WithMessage(Shared.Required).NotEmpty().WithMessage(Shared.Required).When(x => !AddressEditModel.IsNullOrEmpty(x));
        }
    }
}
