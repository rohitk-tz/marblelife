using Core.Application.Attribute;
using Core.Localization.Validations;
using Core.Users.ViewModels;
using FluentValidation;

namespace Core.Users.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<PersonEditModel>))]
    public class PersonEditModelValidator : AbstractValidator<PersonEditModel>
    {
        public PersonEditModelValidator(IUniqueEmailValidator emailValidator, IPhoneNumberTextValidator phoneValidator)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(Shared.Required).NotNull().WithMessage(Shared.Required).When(x => string.IsNullOrEmpty(x.Name.LastName)).WithMessage(Shared.Required);

            RuleFor(x => x.Email).SetValidator(emailValidator).When(x => x.PersonId < 1 && !string.IsNullOrEmpty(x.Email))
                .Must((x, email) => emailValidator.IsValid(x.PersonId, x.Email)).WithMessage(Shared.UniqueEmail);


            RuleForEach(x => x.PhoneNumbers).SetValidator(phoneValidator);

        }
    }
}
