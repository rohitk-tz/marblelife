using Core.Application;
using Core.Application.Attribute;
using Core.Application.Exceptions;
using FluentValidation.Validators;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class UniqueEmailValidator : EmailValidator, IUniqueEmailValidator
    {

        public UniqueEmailValidator()
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.Instance == null || context.PropertyValue == null || string.IsNullOrWhiteSpace(context.PropertyValue.ToString())) return false;

            var result = base.IsValid(context);

            if (result == false)
            {
                throw new InvalidDataProvidedException("Email-id is invalid.");
            }
            var userLoginService = ApplicationManager.DependencyInjection.Resolve<IUserLoginService>();

            if (userLoginService.IsUniqueEmailAddress(context.PropertyValue.ToString()))
                return true;

            throw new InvalidDataProvidedException("Email-id already exists.");
        }

        public bool IsValid(long personId, string email)
        {
            var userLoginService = ApplicationManager.DependencyInjection.Resolve<IUserLoginService>();

            return userLoginService.IsUniqueEmailAddress(email, personId);
        }
    }
}
