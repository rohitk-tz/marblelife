# Core/Application/Exceptions - AI Context

## Purpose

This folder contains custom exception types for the MarbleLife application, providing meaningful error handling and consistent exception management across all layers.

## Contents

Custom exception classes:
- **BusinessRuleViolationException**: Business logic errors
- **EntityNotFoundException**: Resource not found (404)
- **ValidationException**: Input validation failures
- **UnauthorizedException**: Access denied (401)
- **ForbiddenException**: Insufficient permissions (403)
- **DuplicateEntityException**: Unique constraint violations
- **ExternalServiceException**: Third-party API failures
- **ConcurrencyException**: Optimistic concurrency conflicts

## For AI Agents

**Creating Custom Exception**:
```csharp
public class CustomBusinessException : Exception
{
    public ErrorCode Code { get; set; }
    public Dictionary<string, object> Details { get; set; }
    
    public CustomBusinessException(string message, ErrorCode code = ErrorCode.Unknown) 
        : base(message)
    {
        Code = code;
        Details = new Dictionary<string, object>();
    }
    
    public CustomBusinessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
```

**Throwing Exceptions**:
```csharp
// Business rule violation
if (!businessRule.IsValid())
{
    throw new BusinessRuleViolationException("Customer cannot have duplicate email addresses");
}

// Entity not found
var entity = _repository.Get(id);
if (entity == null)
{
    throw new EntityNotFoundException($"Entity with ID {id} not found");
}

// Validation error
if (string.IsNullOrEmpty(model.Email))
{
    throw new ValidationException("Email is required");
}
```

**Catching and Handling**:
```csharp
try
{
    _service.ProcessOperation();
}
catch (BusinessRuleViolationException ex)
{
    _logService.LogWarning("Business rule violated", ex);
    return BadRequest(ex.Message);
}
catch (EntityNotFoundException ex)
{
    _logService.LogInfo("Entity not found", ex);
    return NotFound(ex.Message);
}
catch (Exception ex)
{
    _logService.LogError("Unexpected error", ex);
    return InternalServerError("An unexpected error occurred");
}
```

## For Human Developers

Exception handling best practices:
- Use specific exception types for different error scenarios
- Include detailed error messages with context
- Log exceptions appropriately (Warning, Error, Critical)
- Don't swallow exceptions without logging
- Use inner exceptions to preserve stack traces
- Map exceptions to appropriate HTTP status codes in API layer
- Provide user-friendly error messages
- Include error codes for client-side handling
- Use exception filters in API for centralized handling
- Never expose sensitive information in exception messages
- Include correlation IDs for distributed tracing
