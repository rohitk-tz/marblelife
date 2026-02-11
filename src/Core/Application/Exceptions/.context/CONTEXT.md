<!-- AUTO-GENERATED: Header -->
# Exceptions — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Provides domain-specific exception types that represent recoverable business errors and invalid operations. These exceptions enable precise error handling, meaningful error messages, and appropriate HTTP status code mapping in API controllers.

### Design Patterns
- **Exception Hierarchy**: All custom exceptions inherit from `CustomBaseException` for centralized catch handling
- **Specific over Generic**: Each exception type represents a distinct failure scenario (validation, authentication, data integrity)
- **Message-First Design**: Constructors accept custom messages while providing sensible defaults
- **Fail-Fast**: Exceptions are thrown immediately when invalid state is detected, preventing cascading failures

### Data Flow
1. Business logic detects invalid operation (e.g., invalid address format, missing file)
2. Throws specific exception type with descriptive message
3. Exception bubbles up through service layer
4. Controller catches exception and maps to HTTP response:
   - `InvalidDataProvidedException` → 400 Bad Request
   - `UserBlockedException` → 403 Forbidden
   - `InvalidDependencyRegistrationException` → 500 Internal Server Error (startup failure)
5. Error message is returned to client in `ResponseModel.Message`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### CustomBaseException.cs (Abstract Base)
```csharp
// Base class for all application-specific exceptions
public abstract class CustomBaseException : Exception
{
    public CustomBaseException() { }
    
    public CustomBaseException(string message) : base(message) { }
    
    // Inherits from System.Exception:
    // - Message: Error description
    // - StackTrace: Call stack for debugging
    // - InnerException: Wrapped exception if applicable
}
```
**Purpose**: Enables `catch (CustomBaseException)` to handle all domain exceptions uniformly.

### InvalidAddressException.cs
```csharp
// Thrown when address validation fails (missing fields, invalid format, geocoding failure)
public class InvalidAddressException : CustomBaseException
{
    public InvalidAddressException(string message) : base(message) { }
    
    // Example messages:
    // - "Street address is required"
    // - "ZIP code must be 5 digits"
    // - "Address could not be geocoded"
}
```

### InvalidFileUploadException.cs
```csharp
// Thrown when uploaded file format is invalid or doesn't meet requirements
public class InvalidFileUploadException : CustomBaseException
{
    // Default message for GeoCode file validation
    public InvalidFileUploadException() 
        : base("Invalid File Upload. There should be only 2 tabs in GeoCode File") { }
    
    public InvalidFileUploadException(string message) : base(message) { }
    
    // Example custom messages:
    // - "File size exceeds 10MB limit"
    // - "Only PDF and JPEG files are allowed"
    // - "Excel file must contain 'Customers' worksheet"
}
```

### InvalidDependencyRegistrationException.cs
```csharp
// Thrown during application startup when DI container registration fails
public class InvalidDependencyRegistrationException : CustomBaseException
{
    public InvalidDependencyRegistrationException(Type baseType, Type implType)
        : base(string.Format("Type {0} cannot be used to register type {1}", 
                             implType.Name, baseType.Name))
    { }
    
    // Example scenario:
    // - Trying to register ConcreteClass as IUnrelatedInterface
    // - Interface not implemented by the provided type
    // - Generic type constraint violations
}
```

### InvalidDataProvidedException.cs
```csharp
// Thrown when user input or external data fails business rule validation
public class InvalidDataProvidedException : CustomBaseException
{
    public InvalidDataProvidedException() : base() { }
    
    public InvalidDataProvidedException(string message) : base(message) { }
    
    // Example messages:
    // - "Invoice total cannot be negative"
    // - "Start date must be before end date"
    // - "Customer ID does not exist"
}
```

### UserBlockedException.cs
```csharp
// Thrown when user attempts to access the system but their account is disabled
public class UserBlockedException : CustomBaseException
{
    // Default message
    public UserBlockedException() : base("User has been blocked.") { }
    
    public UserBlockedException(string message) : base(message) { }
    
    // Example custom messages:
    // - "Your account has been suspended. Contact support."
    // - "Trial period expired. Please upgrade to continue."
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### CustomBaseException (Base Class)
- **Purpose**: Common ancestor for all domain exceptions
- **Usage**: `catch (CustomBaseException ex)` to handle all app-specific errors
- **Constructors**:
  - Parameterless: Sets default message (if overridden in derived class)
  - With message: Custom error description

### InvalidAddressException
- **When to throw**: Address parsing, validation, or geocoding fails
- **HTTP Status Code**: 400 Bad Request
- **Example Scenarios**:
  - Missing required fields (street, city, state, ZIP)
  - Invalid ZIP code format
  - Address not found in geocoding service
  - Country not supported

### InvalidFileUploadException
- **When to throw**: Uploaded file validation fails
- **HTTP Status Code**: 400 Bad Request
- **Example Scenarios**:
  - File size exceeds limit
  - Unsupported file type (e.g., .exe when expecting .pdf)
  - Corrupted file (can't be parsed)
  - Missing required worksheet in Excel file
  - **Default message**: GeoCode file must have exactly 2 tabs

### InvalidDependencyRegistrationException
- **When to throw**: DI container setup detects type mismatch
- **HTTP Status Code**: 500 Internal Server Error (prevents app startup)
- **Example Scenarios**:
  - Registering implementation that doesn't implement interface
  - Generic type constraint violations
  - Circular dependency detection
- **Constructor Parameters**:
  - `baseType`: Interface or base class being registered
  - `implType`: Concrete implementation type causing the failure

### InvalidDataProvidedException
- **When to throw**: Business rule validation fails on user input or external data
- **HTTP Status Code**: 400 Bad Request
- **Example Scenarios**:
  - Negative amounts in financial calculations
  - Date range violations (end before start)
  - Foreign key constraint violations (referencing non-existent records)
  - Required field missing after deserialization
- **Note**: Used in `PathExtensions.ToRelativePath()` when path doesn't contain MediaRootPath

### UserBlockedException
- **When to throw**: User authentication succeeds but account is disabled
- **HTTP Status Code**: 403 Forbidden
- **Example Scenarios**:
  - User.IsActive = false in database
  - Trial period expired
  - Account suspended by admin
  - Payment past due (for subscription systems)
- **Default Message**: "User has been blocked."
- **Typical Flow**: Check after successful authentication but before granting access
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
None - all exceptions are self-contained

### External Dependencies
- **System.Exception** (BCL) — Base class for all exceptions

### Referenced By
- **Core.Application.Extensions.PathExtensions** — Throws `InvalidDataProvidedException` when path conversion fails
- **Controller Layer** — Catches exceptions and converts to HTTP responses
- **Service Layer** — Throws exceptions when business rules are violated
- **FileService/Parsers** — Throws `InvalidFileUploadException` during file processing
- **DI Container (SessionFactory)** — Throws `InvalidDependencyRegistrationException` during startup
- **Authentication Middleware** — Throws `UserBlockedException` when user is blocked
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Exception vs. Validation Pattern
**When to throw exceptions:**
- Unexpected failures (file I/O errors, database unavailable)
- Programming errors (null reference, type mismatch)
- Security violations (blocked user, unauthorized access)

**When to use validation (ModelState/FluentValidation):**
- Expected user input errors (empty required fields)
- Format violations (invalid email, phone number)
- Constraints known at design time

**Rule of Thumb**: If the error is recoverable and expected during normal operation, use validation. If it indicates a failure in assumptions or system state, throw an exception.

### HTTP Status Code Mapping
```csharp
try
{
    // Business logic
}
catch (UserBlockedException ex)
{
    return StatusCode(403, new ResponseModel { Message = CreateErrorMessage(ex.Message) });
}
catch (InvalidAddressException ex)
catch (InvalidFileUploadException ex)
catch (InvalidDataProvidedException ex)
{
    return BadRequest(new ResponseModel { Message = CreateErrorMessage(ex.Message) });
}
catch (InvalidDependencyRegistrationException ex)
{
    // This should never be caught in controllers (app won't start)
    return StatusCode(500, new ResponseModel { Message = CreateErrorMessage("Internal server error") });
}
catch (CustomBaseException ex)
{
    // Catch-all for unexpected domain exceptions
    return StatusCode(500, new ResponseModel { Message = CreateErrorMessage(ex.Message) });
}
```

### When to Create New Exception Types
**Create a new exception if:**
1. It represents a distinct failure category requiring different handling
2. You need to catch it specifically without catching other exceptions
3. It maps to a different HTTP status code

**Don't create a new exception if:**
1. Existing exception + custom message is sufficient
2. It's a validation error (use FluentValidation instead)
3. It's a framework exception (ArgumentNullException, InvalidOperationException, etc.)

### Common Pitfalls
- **Swallowing exceptions**: Never catch and ignore without logging
- **Generic messages**: Always provide context: "Invoice #12345 not found" not "Not found"
- **Throwing in constructors**: Avoid throwing exceptions from domain entity constructors (breaks ORM hydration)
- **Using for control flow**: Exceptions are expensive; don't use for expected branches
<!-- END CUSTOM SECTION -->
