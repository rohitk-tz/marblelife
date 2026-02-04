# API/Impl/Exceptions - AI Context

## Purpose

This folder contains API-specific exception handling classes and custom exceptions for HTTP-related error scenarios.

## Contents

Custom exception classes for API layer:
- **HttpResponseException**: Base class for HTTP exceptions
- **BadRequestException**: 400 Bad Request scenarios
- **UnauthorizedException**: 401 Unauthorized scenarios
- **ForbiddenException**: 403 Forbidden scenarios
- **NotFoundException**: 404 Not Found scenarios
- **InternalServerErrorException**: 500 Internal Server Error scenarios

## For AI Agents

**API Exception Pattern**:
```csharp
public class NotFoundException : HttpResponseException
{
    public NotFoundException(string message) 
        : base(HttpStatusCode.NotFound)
    {
        Content = new StringContent(JsonConvert.SerializeObject(new
        {
            error = "Not Found",
            message = message
        }));
    }
}
```

**Usage in Controllers**:
```csharp
public IHttpActionResult Get(int id)
{
    var entity = _service.Get(id);
    if (entity == null)
    {
        throw new NotFoundException($"Entity with ID {id} not found");
    }
    return Ok(entity);
}
```

## For Human Developers

API exceptions map to HTTP status codes and provide consistent error responses to clients.

### Best Practices:
- Throw appropriate HTTP exceptions in controllers
- Use exception filters for global error handling
- Include detailed error messages for debugging
- Log exceptions before throwing
- Return consistent error response format
- Hide sensitive information in production
