using Core.Application;
using Core.Application.Attribute;
using Core.Localization.Validations;
using Core.Users.ViewModels;
using FluentValidation;

namespace Core.Users.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<UserLoginEditModel>))]
    public class UserLoginEditModelValidator : AbstractValidator<UserLoginEditModel>
    {
        public UserLoginEditModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(Shared.Required)
                .NotNull().WithMessage(Shared.Required);
                //.Must((x, y) =>
                //{
                //    var userLoginService = ApplicationManager.DependencyInjection.Resolve<IUserLoginService>();
                //    return !string.IsNullOrEmpty(x.UserName) && userLoginService.IsUniqueUserName(x.UserName, x.Id);
                //})
                //.WithMessage(Shared.UniqueUserNameValidation);

            RuleFor(x => x.Password).NotEmpty().WithMessage(Shared.Required)
                                  .NotNull().WithMessage(Shared.Required)
                                  .When(x => x.ChangePassword);

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(Shared.Required)
                                        .NotNull().WithMessage(Shared.Required)
                                        .Must((x, y) => x.Password == x.ConfirmPassword).WithMessage(Shared.ConfirmPasswordDoesNotMatch).When(x => x.ChangePassword);
        }
    }
}
