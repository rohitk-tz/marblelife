# Core/Localization - AI Context

## Purpose

The **Localization** module provides internationalization (i18n) support, multi-language capabilities, and validation message localization for the MarbleLife platform.

## Key Components

### Localization Services
- Resource file management
- Culture-specific formatting
- Validation message localization
- Date/time/currency formatting

## Sub-folders

### **Validations/**
Contains localized validation messages for different cultures:
- Error messages
- Field validation text
- Custom validators with localized messages

## Service Interfaces

- **ILocalizationService**: Text translation and resource lookup
- **ICultureService**: Culture management
- **IValidationMessageService**: Localized validation messages
- **IDateTimeFormatter**: Culture-specific date/time formatting
- **ICurrencyFormatter**: Currency formatting by locale

## Implementations (Impl/)

Business logic for:
- Resource file loading
- Culture detection from user preferences
- Fallback to default culture
- Dynamic text translation

## Enumerations (Enum/)

- **SupportedCulture**: en-US, es-ES, fr-FR, etc.
- **DateFormat**: Short, Long, Medium
- **NumberFormat**: Currency, Decimal, Percentage

## ViewModels (ViewModel/)

- **LocalizationViewModel**: Translated text data
- **CultureViewModel**: Culture configuration

## Business Rules

1. **Default Culture**: en-US (English - United States)
2. **Fallback**: If translation missing, use default culture
3. **User Preference**: Use user's preferred language
4. **Validation**: All validation messages localized
5. **Formats**: Date/time/currency formatted per culture

## Dependencies

- **Core/Users**: User language preferences
- **System.Globalization**: .NET globalization support

## For AI Agents

### Getting Localized Text
```csharp
// Get translated text
var text = _localizationService.GetText("WelcomeMessage", culture: "es-ES");
// Returns: "Bienvenido" instead of "Welcome"

// With parameters
var message = _localizationService.GetText(
    "AppointmentConfirmation",
    culture: userCulture,
    parameters: new { Date = appointmentDate, Time = appointmentTime }
);
```

### Validation Messages
```csharp
// Localized validation
public class CustomerValidator : AbstractValidator<CustomerViewModel>
{
    public CustomerValidator(ILocalizationService localization)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(localization.GetText("EmailRequired"))
            .EmailAddress()
            .WithMessage(localization.GetText("EmailInvalid"));
    }
}
```

### Formatting
```csharp
// Format date per culture
var formattedDate = _dateTimeFormatter.Format(
    DateTime.Now,
    culture: "fr-FR",
    format: DateFormat.Long
);
// Returns: "15 mars 2024" for French

// Format currency
var formattedPrice = _currencyFormatter.Format(
    1234.56m,
    culture: "de-DE",
    currencyCode: "EUR"
);
// Returns: "1.234,56 €" for German
```

## For Human Developers

### Resource Files
```
Localization/
├── Resources/
│   ├── Messages.en-US.resx (default)
│   ├── Messages.es-ES.resx (Spanish)
│   ├── Messages.fr-FR.resx (French)
│   └── Validations/
│       ├── Validation.en-US.resx
│       └── Validation.es-ES.resx
```

### Best Practices
- Never hardcode user-facing text
- Use resource keys that describe the context
- Provide context comments in resource files for translators
- Test with different cultures during development
- Use culture-neutral resource files as fallback
- Consider text expansion in other languages (German is 30% longer than English)
- Format numbers, dates, and currency appropriately
- Support right-to-left (RTL) languages if needed
- Externalize all validation messages
- Use plural forms correctly for different languages
