using FluentValidation.Validators;

namespace Core.Users
{
    public interface IUniqueEmailValidator : IPropertyValidator
    {
        bool IsValid(long personId, string email);
    }
}
