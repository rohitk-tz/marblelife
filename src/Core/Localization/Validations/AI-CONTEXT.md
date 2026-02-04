# Core/Localization/Validations - AI Context

## Purpose

This folder contains localized validation messages for different cultures and languages, ensuring that validation errors are displayed in the user's preferred language.

## Contents

Resource files for validation messages:
- **Validation.en-US.resx**: English (US) validation messages (default)
- **Validation.es-ES.resx**: Spanish validation messages
- **Validation.fr-FR.resx**: French validation messages
- Other culture-specific validation resources

## For AI Agents

**Resource File Structure**:
```
Name                              | Value
----------------------------------|----------------------------------------
EmailRequired                     | Email address is required
EmailInvalid                      | Please enter a valid email address
PasswordTooShort                  | Password must be at least 8 characters
PasswordRequiresUppercase         | Password must contain an uppercase letter
PasswordRequiresNumber            | Password must contain a number
PhoneInvalid                      | Please enter a valid phone number
DateInPast                        | Date cannot be in the past
AmountMustBePositive              | Amount must be greater than zero
StringLengthExceeded              | {0} cannot exceed {1} characters
RangeValidation                   | {0} must be between {1} and {2}
```

**Using Localized Validation**:
```csharp
public class CustomerValidator : AbstractValidator<CustomerViewModel>
{
    private readonly ILocalizationService _localization;
    
    public CustomerValidator(ILocalizationService localization)
    {
        _localization = localization;
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(_localization.GetValidationMessage("EmailRequired"))
            .EmailAddress()
            .WithMessage(_localization.GetValidationMessage("EmailInvalid"));
            
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage(_localization.GetValidationMessage("PasswordTooShort"));
    }
}
```

**DataAnnotations with Localized Messages**:
```csharp
public class CustomerViewModel
{
    [Required(ErrorMessageResourceType = typeof(ValidationResources), 
              ErrorMessageResourceName = "EmailRequired")]
    [EmailAddress(ErrorMessageResourceType = typeof(ValidationResources),
                  ErrorMessageResourceName = "EmailInvalid")]
    public string Email { get; set; }
    
    [Range(0, 100, ErrorMessageResourceType = typeof(ValidationResources),
                   ErrorMessageResourceName = "RangeValidation")]
    public int Age { get; set; }
}
```

## For Human Developers

Best practices:
- Keep validation messages consistent across the application
- Use resource keys that describe the validation rule
- Provide context in error messages
- Support parameter substitution for dynamic values
- Test validation messages in all supported languages
- Keep messages concise but informative
- Use friendly, non-technical language
- Maintain parity across all language files
- Document any culture-specific validation rules
- Review translations with native speakers
