using Core.Application.Attribute;
using Core.Application.Exceptions;
using Core.Geo.ViewModel;
using Core.Users.Domain;
using FluentValidation.Validators;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    class PhoneNumberTextValidator : PropertyValidator, IPhoneNumberTextValidator
    {
        public PhoneNumberTextValidator()
            : base("")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {

            if (context.Instance == null || context.PropertyValue == null || string.IsNullOrWhiteSpace(context.PropertyValue.ToString())) return false;

            try
            {
                var phoneObj = Phone.Create(((PhoneEditModel)context.PropertyValue).PhoneNumber, 0, 0);

                if (phoneObj == null) return false;
            }
            catch (InvalidDataProvidedException)
            {
                return false;
            }

            return true;
        }
    }
}
