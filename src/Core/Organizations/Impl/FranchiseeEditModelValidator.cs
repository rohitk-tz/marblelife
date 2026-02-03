using Core.Application.Attribute;
using Core.Localization.Validations;
using Core.Organizations.ViewModels;
using FluentValidation;

namespace Core.Organizations.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<FranchiseeEditModel>))]

    public class FranchiseeEditModelValidator : AbstractValidator<FranchiseeEditModel>
    {
        public FranchiseeEditModelValidator()
        {
            RuleFor(x => x.FranchiseeServices).NotNull().WithMessage(Shared.Required).NotEmpty().WithMessage(Franchisee.AtLeastOneServiceRequired);
        }
    }
}
