using Core.Application.Attribute;
using Core.Users.ViewModels;
using FluentValidation;

namespace Core.Users.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<UserEditModel>))]
    public class UserEditModelValidator: AbstractValidator<UserEditModel>
    {
        public UserEditModelValidator(IValidator<UserLoginEditModel> userLoginModel, IValidator<PersonEditModel> personEditModel)
        {
            RuleFor(x => x.UserLoginEditModel).SetValidator(userLoginModel);
            RuleFor(x => x.PersonEditModel).SetValidator(personEditModel);
            //RuleFor(x => x.RoleId).GreaterThan(0).WithMessage(Shared.Required);
        }
    }
}
