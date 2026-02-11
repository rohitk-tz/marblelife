<!-- AUTO-GENERATED: Header -->
# Exceptions
> Domain-specific exception types for business rule violations, validation failures, and system errors
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Exceptions** module defines six custom exception types that represent specific failure scenarios in the Marble Life application. These exceptions inherit from `CustomBaseException` and provide meaningful error messages that can be displayed to users or logged for debugging.

**Exception Hierarchy:**
```
System.Exception
└── CustomBaseException (abstract)
    ├── InvalidAddressException
    ├── InvalidFileUploadException
    ├── InvalidDependencyRegistrationException
    ├── InvalidDataProvidedException
    └── UserBlockedException
```

**Why custom exceptions?**
- **Specific Error Handling**: Catch `InvalidFileUploadException` to show file upload errors differently than `InvalidAddressException`
- **Meaningful Messages**: Default messages provide context (e.g., "User has been blocked") instead of generic errors
- **HTTP Mapping**: Controllers map exception types to appropriate HTTP status codes (400, 403, 500)
- **Debugging**: Custom types make log filtering and error tracking easier
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Throw InvalidFileUploadException
```csharp
using Core.Application.Exceptions;

public void ValidateUploadedFile(Stream fileStream, string filename)
{
    if (fileStream.Length > 10 * 1024 * 1024)  // 10MB limit
    {
        throw new InvalidFileUploadException("File size exceeds 10MB limit");
    }
    
    if (!filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidFileUploadException("Only PDF files are allowed");
    }
    
    // GeoCode-specific validation uses default message
    var workbook = new XLWorkbook(fileStream);
    if (workbook.Worksheets.Count != 2)
    {
        throw new InvalidFileUploadException();  
        // Default: "Invalid File Upload. There should be only 2 tabs in GeoCode File"
    }
}
```

### Example 2: Throw InvalidDataProvidedException
```csharp
using Core.Application.Exceptions;

public Invoice CreateInvoice(InvoiceEditModel model)
{
    if (model.Total < 0)
    {
        throw new InvalidDataProvidedException("Invoice total cannot be negative");
    }
    
    if (model.DueDate < DateTime.Today)
    {
        throw new InvalidDataProvidedException("Due date cannot be in the past");
    }
    
    var customer = customerRepository.GetById(model.CustomerId);
    if (customer == null)
    {
        throw new InvalidDataProvidedException($"Customer ID {model.CustomerId} does not exist");
    }
    
    return new Invoice { /* ... */ };
}
```

### Example 3: Throw UserBlockedException After Authentication
```csharp
using Core.Application.Exceptions;

public class AuthenticationService
{
    public User Authenticate(string username, string password)
    {
        var user = userRepository.GetByUsername(username);
        
        if (user == null || !VerifyPassword(user, password))
        {
            return null;  // Invalid credentials
        }
        
        if (!user.IsActive)
        {
            throw new UserBlockedException();  // Default: "User has been blocked."
        }
        
        if (user.TrialExpired)
        {
            throw new UserBlockedException("Trial period has expired. Please upgrade to continue.");
        }
        
        return user;
    }
}
```

### Example 4: Throw InvalidAddressException
```csharp
using Core.Application.Exceptions;

public Address ValidateAndGeocodeAddress(AddressModel model)
{
    if (string.IsNullOrWhiteSpace(model.Street))
    {
        throw new InvalidAddressException("Street address is required");
    }
    
    if (!Regex.IsMatch(model.ZipCode, @"^\d{5}(-\d{4})?$"))
    {
        throw new InvalidAddressException("ZIP code must be in format 12345 or 12345-6789");
    }
    
    var geocodeResult = geocodingService.Geocode(model);
    if (geocodeResult == null)
    {
        throw new InvalidAddressException("Address could not be verified. Please check and try again.");
    }
    
    return new Address { /* ... */ };
}
```

### Example 5: Catch and Map to HTTP Response
```csharp
using Core.Application.Exceptions;
using Core.Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

[HttpPost("upload")]
public IActionResult UploadFile(IFormFile file)
{
    try
    {
        fileService.ProcessUpload(file);
        return Ok(new ResponseModel { Message = CreateSuccessMessage("File uploaded") });
    }
    catch (InvalidFileUploadException ex)
    {
        return BadRequest(new ResponseModel { Message = CreateErrorMessage(ex.Message) });
    }
    catch (InvalidDataProvidedException ex)
    {
        return BadRequest(new ResponseModel { Message = CreateErrorMessage(ex.Message) });
    }
    catch (UserBlockedException ex)
    {
        return StatusCode(403, new ResponseModel { Message = CreateErrorMessage(ex.Message) });
    }
    catch (Exception ex)
    {
        // Log unexpected errors
        logger.LogError(ex, "Unexpected error during file upload");
        return StatusCode(500, new ResponseModel { Message = CreateErrorMessage("An error occurred") });
    }
}
```

### Example 6: Throw InvalidDependencyRegistrationException (DI Setup)
```csharp
using Core.Application.Exceptions;

public void RegisterType<TInterface, TImplementation>()
{
    var interfaceType = typeof(TInterface);
    var implementationType = typeof(TImplementation);
    
    if (!interfaceType.IsAssignableFrom(implementationType))
    {
        throw new InvalidDependencyRegistrationException(interfaceType, implementationType);
        // Message: "Type TImplementation cannot be used to register type TInterface"
    }
    
    services.AddScoped(interfaceType, implementationType);
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Exception Type | Default Message | HTTP Status | When to Use |
|----------------|-----------------|-------------|-------------|
| `CustomBaseException` | (abstract) | — | Base class for all domain exceptions |
| `InvalidAddressException` | (custom required) | 400 | Address validation/geocoding fails |
| `InvalidFileUploadException` | "Invalid File Upload. There should be only 2 tabs in GeoCode File" | 400 | File format/size/type validation fails |
| `InvalidDependencyRegistrationException` | "Type {impl} cannot be used to register type {base}" | 500 | DI registration type mismatch |
| `InvalidDataProvidedException` | (none) | 400 | Business rule validation fails |
| `UserBlockedException` | "User has been blocked." | 403 | User account is disabled |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Exception message is too generic
**Cause**: Using default constructor when custom message is needed.
**Solution**: Always pass descriptive message to constructor:
```csharp
// ❌ Generic
throw new InvalidDataProvidedException();

// ✅ Specific
throw new InvalidDataProvidedException($"Invoice ID {invoiceId} not found");
```

### Issue: Exceptions not caught by error handling middleware
**Cause**: Throwing framework exceptions instead of `CustomBaseException` subclasses.
**Solution**: Wrap framework exceptions or throw custom types:
```csharp
// ❌ Framework exception bypasses custom handler
throw new ArgumentException("Invalid input");

// ✅ Custom exception caught by middleware
throw new InvalidDataProvidedException("Invalid input");
```

### Issue: InvalidDependencyRegistrationException crashes application on startup
**Cause**: This is **expected behavior** - DI configuration errors should prevent app startup.
**Solution**: 
- Fix the type registration issue (ensure implementation actually implements interface)
- Check generic type constraints are satisfied
- Review stack trace to identify which registration failed

### Issue: Should I throw exception or return error result?
**Decision Matrix:**
| Scenario | Approach |
|----------|----------|
| User input validation | Use FluentValidation / ModelState |
| Expected business rule failure | Throw custom exception |
| Unexpected system failure | Throw custom exception |
| Optional operation failure | Return `Result<T>` or nullable |

**Example:**
```csharp
// ✅ Use validation for form input
public class CreateInvoiceValidator : AbstractValidator<InvoiceModel>
{
    RuleFor(x => x.Total).GreaterThan(0);
}

// ✅ Use exception for business logic
public Invoice ProcessPayment(Invoice invoice, decimal amount)
{
    if (invoice.Status == InvoiceStatus.Paid)
        throw new InvalidDataProvidedException("Invoice is already paid");
}
```

### Issue: Exception message contains sensitive data
**Cause**: Including database IDs, user info, or internal paths in exception messages.
**Solution**: Use generic messages for client-facing errors, detailed messages for logging:
```csharp
// ❌ Exposes internal structure
throw new InvalidDataProvidedException($"FK constraint violation on Users.Id = {userId}");

// ✅ Generic message, log details separately
logger.LogError("FK constraint violation: Users.Id = {UserId}", userId);
throw new InvalidDataProvidedException("The requested resource does not exist");
```

### Issue: Multiple exceptions for similar scenarios
**Cause**: Over-engineering exception hierarchy.
**Solution**: Reuse existing exceptions with different messages:
```csharp
// ❌ Don't create separate exceptions
public class InvalidInvoiceException : CustomBaseException { }
public class InvalidCustomerException : CustomBaseException { }

// ✅ Reuse InvalidDataProvidedException
throw new InvalidDataProvidedException("Invalid invoice data");
throw new InvalidDataProvidedException("Invalid customer data");
```
<!-- END CUSTOM SECTION -->
