# API/Attribute - AI Context

## Purpose

The **Attribute** folder contains custom action filters and attributes that provide cross-cutting concerns for Web API controllers. These attributes handle authentication, authorization, exception handling, transactions, and custom model binding using the ASP.NET Web API filter pipeline.

## Architecture

Web API filters execute in a specific order during request processing:
1. **Authorization Filters**: First to execute (authentication/authorization)
2. **Action Filters**: Before and after action execution
3. **Exception Filters**: Catch exceptions from actions
4. **Result Filters**: Process action results before returning to client

## Key Files

### BasicAuthenticationAttribute.cs
**Purpose**: Token-based authentication filter that validates user sessions.

**Functionality**:
- Checks for `token` header in HTTP request
- Validates token against stored sessions
- Populates `ISessionContext` with user information
- Allows anonymous access via `[AllowAnonymous]` attribute
- Throws `NotAuthenticatedException` if validation fails

**Usage**:
```csharp
[BasicAuthentication]
public class FranchiseeController : BaseController
{
    // All actions require authentication
}

public class LoginController : BaseController
{
    [AllowAnonymous]
    public TokenResponse Login(LoginModel model)
    {
        // Public endpoint, no authentication required
    }
}
```

**Implementation Details**:
```csharp
public override void OnActionExecuting(HttpActionContext actionContext)
{
    if (IsAuthenticated(actionContext)) return;
    throw new NotAuthenticatedException();
}

protected bool IsAuthenticated(HttpActionContext actionContext)
{
    // 1. Check if [AllowAnonymous] is present
    if (IsAnonymousRoleAllowed(actionContext))
        return true;

    // 2. Check for token header
    if (!actionContext.Request.Headers.Contains(SessionHelper.TokenKeyName))
        return false;

    // 3. Validate token and load session
    var token = actionContext.Request.Headers.GetValues(SessionHelper.TokenKeyName).First();
    if (string.IsNullOrEmpty(token)) return false;
    if (ApplicationManager.DependencyInjection.Resolve<ISessionContext>().UserSession == null) 
        return false;

    return true;
}
```

**Session Flow**:
1. Client includes token in request header: `token: xyz123`
2. `SessionHelper.SetSessionModel(token)` loads user session (executed in Global.asax BeginRequest)
3. `BasicAuthenticationAttribute` verifies session exists
4. Controller actions access user info via `ISessionContext.UserSession`

### BasicExceptionFilterAttribute.cs
**Purpose**: Global exception handling filter that catches, logs, and formats exceptions.

**Functionality**:
- Catches all unhandled exceptions from controller actions
- Logs exceptions via `ILogService`
- Returns user-friendly error messages to clients
- Handles different exception types (validation, authorization, system errors)
- Prevents stack traces from leaking in production

**Common Exception Types**:
```csharp
// Validation errors
throw new ValidationFailureException("Invalid input data");

// Authentication errors
throw new NotAuthenticatedException();

// Authorization errors  
throw new UnauthorizedAccessException("Insufficient permissions");

// Business rule violations
throw new InvalidOperationException("Cannot delete franchisee with active jobs");

// System errors
throw new Exception("Unexpected error occurred");
```

**Response Format**:
```json
{
  "success": false,
  "message": "User-friendly error message",
  "errorCode": "VALIDATION_ERROR",
  "details": null  // Populated only in debug mode
}
```

**Implementation Pattern**:
```csharp
public override void OnException(HttpActionExecutedContext actionExecutedContext)
{
    var exception = actionExecutedContext.Exception;
    
    // Log the exception
    ApplicationManager.DependencyInjection.Resolve<ILogService>()
        .Error("API Exception", exception);
    
    // Create user-friendly response
    var response = new ErrorResponse
    {
        Success = false,
        Message = GetUserFriendlyMessage(exception)
    };
    
    actionExecutedContext.Response = actionExecutedContext.Request
        .CreateResponse(GetHttpStatusCode(exception), response);
}
```

### DbTransactionFilterAttribute.cs
**Purpose**: Automatic database transaction management for controller actions.

**Functionality**:
- Begins database transaction before action execution
- Commits transaction if action succeeds
- Rolls back transaction if action throws exception
- Ensures data consistency for multi-step operations

**Usage**:
```csharp
[BasicAuthentication]
[DbTransactionFilter]
public class FranchiseeController : BaseController
{
    [HttpPost]
    public bool Post([FromBody] FranchiseeEditModel model)
    {
        // Multiple database operations wrapped in transaction
        _franchiseeService.Save(model);
        _auditService.LogCreation(model);
        _notificationService.SendWelcomeEmail(model);
        
        // All succeed or all rollback
        return true;
    }
}
```

**Transaction Scope**:
- Applies to single action method
- Uses `IUnitOfWork` pattern from Core
- Automatic cleanup on completion or exception

**Implementation**:
```csharp
public override void OnActionExecuting(HttpActionContext actionContext)
{
    var unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
    unitOfWork.BeginTransaction();
}

public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
{
    var unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
    
    if (actionExecutedContext.Exception == null)
    {
        unitOfWork.Commit();
    }
    else
    {
        unitOfWork.Rollback();
    }
}
```

### CustomDataBinderAttribute.cs
**Purpose**: Custom model binding for complex request scenarios.

**Functionality**:
- Binds data from multiple sources (query string, body, headers)
- Handles special data types (dates with timezones, custom formats)
- Transforms request data before controller action execution
- Validates and normalizes input data

**Usage Scenarios**:
- Binding date/time with client timezone offset
- Combining query parameters with request body
- Converting between data formats
- Applying business-specific transformations

**Example**:
```csharp
[CustomDataBinder]
public class ReportController : BaseController
{
    [HttpGet]
    public ReportData GetReport([FromUri] ReportFilter filter)
    {
        // CustomDataBinder converts timezone-aware dates from client
        // to server timezone before binding to filter object
        return _reportService.GenerateReport(filter);
    }
}
```

## Filter Execution Order

For a typical request, filters execute in this sequence:

```
1. Global.asax.cs: Application_BeginRequest
   └─ SessionHelper.SetSessionModel(token)

2. BasicAuthenticationAttribute: OnActionExecuting
   └─ Validates token, ensures session exists

3. DbTransactionFilterAttribute: OnActionExecuting
   └─ Begins database transaction

4. CustomDataBinderAttribute: OnActionExecuting
   └─ Custom model binding

5. Controller Action Execution
   └─ Your business logic

6. DbTransactionFilterAttribute: OnActionExecuted
   └─ Commits or rolls back transaction

7. BasicExceptionFilterAttribute: OnException (if exception thrown)
   └─ Logs error, returns formatted response

8. Global.asax.cs: Application_EndRequest
   └─ Clears context storage
```

## For AI Agents

### Creating New Filters

1. **Create filter class**:
```csharp
using System.Web.Http.Filters;

namespace API.Attribute
{
    public class CustomFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // Before action logic
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // After action logic
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
```

2. **Apply filter**:
```csharp
// Global (all controllers)
FilterConfig.RegisterGlobalFilters(filters);
filters.Add(new CustomFilterAttribute());

// Controller-level
[CustomFilter]
public class MyController : BaseController { }

// Action-level
public class MyController : BaseController
{
    [CustomFilter]
    public ActionResult MyAction() { }
}
```

### Modifying Authentication

To change authentication logic:
1. Open `BasicAuthenticationAttribute.cs`
2. Modify `IsAuthenticated` method
3. Test with both authenticated and anonymous requests
4. Ensure `[AllowAnonymous]` still works

### Adding Custom Exception Handling

To handle new exception types:
1. Open `BasicExceptionFilterAttribute.cs`
2. Add exception type mapping:
```csharp
private string GetUserFriendlyMessage(Exception exception)
{
    if (exception is CustomBusinessException)
        return exception.Message;
    
    if (exception is ValidationFailureException)
        return "Validation failed. Please check your input.";
    
    // ... existing mappings
    
    return "An unexpected error occurred.";
}
```

## For Human Developers

### Best Practices

#### Filter Design
- **Single Responsibility**: Each filter should have one clear purpose
- **Stateless**: Filters should not maintain state between requests
- **Performance**: Minimize overhead, filters execute on every request
- **Error Handling**: Catch and log exceptions within filters
- **Dependencies**: Use DI for filter dependencies (resolve from container)

#### Authentication
- Always check `[AllowAnonymous]` first to skip validation
- Log authentication failures for security monitoring
- Don't expose sensitive error details (avoid username enumeration)
- Implement rate limiting for failed attempts
- Use HTTPS in production to protect tokens

#### Exception Handling
- Log full exception details (stack trace, inner exceptions)
- Return sanitized messages to clients (no stack traces)
- Use appropriate HTTP status codes:
  - 400: Bad Request (validation errors)
  - 401: Unauthorized (authentication failed)
  - 403: Forbidden (authorization failed)
  - 404: Not Found (resource doesn't exist)
  - 500: Internal Server Error (unexpected errors)
- Include correlation IDs for tracking errors across systems

#### Transactions
- Only use `[DbTransactionFilter]` for operations that modify data
- Don't use for read-only operations (unnecessary overhead)
- Keep transactions short to avoid locking
- Consider idempotency for POST/PUT operations
- Test rollback scenarios thoroughly

#### Model Binding
- Validate data after binding
- Handle null/missing values gracefully
- Document custom binding behavior
- Avoid complex transformations (do in services instead)

### Common Scenarios

#### Authorize by Role
```csharp
public class RoleAuthorizationAttribute : ActionFilterAttribute
{
    private readonly RoleType[] _allowedRoles;

    public RoleAuthorizationAttribute(params RoleType[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        var session = ApplicationManager.DependencyInjection
            .Resolve<ISessionContext>().UserSession;

        if (!_allowedRoles.Contains((RoleType)session.RoleId))
        {
            throw new UnauthorizedAccessException("Insufficient permissions");
        }
    }
}

// Usage
[RoleAuthorization(RoleType.SuperAdmin, RoleType.FranchiseeAdmin)]
public bool DeleteFranchisee(long id) { }
```

#### Log All Requests
```csharp
public class RequestLoggingAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        var logger = ApplicationManager.DependencyInjection.Resolve<ILogService>();
        var request = actionContext.Request;
        
        logger.Info($"Request: {request.Method} {request.RequestUri}");
    }
}
```

#### Validate Franchisee Access
```csharp
public class FranchiseeAccessAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        var session = ApplicationManager.DependencyInjection
            .Resolve<ISessionContext>().UserSession;
        
        // Super admins can access all franchisees
        if (session.RoleId == (long)RoleType.SuperAdmin)
            return;

        // Get franchisee ID from route or query string
        var franchiseeId = GetFranchiseeIdFromRequest(actionContext);
        
        // Validate user belongs to franchisee
        if (session.OrganizationId != franchiseeId)
        {
            throw new UnauthorizedAccessException("Access denied to this franchisee");
        }
    }
}
```

### Debugging Filters

**Filter Not Executing:**
- Verify filter is registered (global, controller, or action level)
- Check filter inheritance (must inherit from appropriate base class)
- Ensure filter doesn't throw exception in constructor

**Authentication Issues:**
- Check token header is present and correct
- Verify session is loaded in Global.asax BeginRequest
- Confirm `ISessionContext` is properly registered in DI
- Test with and without `[AllowAnonymous]`

**Transaction Rollback:**
- Check exception is thrown before action completes
- Verify `IUnitOfWork` is properly configured
- Ensure database supports transactions
- Check transaction isolation level

### Testing Filters

```csharp
// Unit test example
[Test]
public void BasicAuthentication_NoToken_ThrowsNotAuthenticated()
{
    var filter = new BasicAuthenticationAttribute();
    var context = CreateMockActionContext(); // No token header
    
    Assert.Throws<NotAuthenticatedException>(() => 
        filter.OnActionExecuting(context));
}

[Test]
public void BasicAuthentication_ValidToken_AllowsExecution()
{
    var filter = new BasicAuthenticationAttribute();
    var context = CreateMockActionContext("valid-token");
    
    Assert.DoesNotThrow(() => filter.OnActionExecuting(context));
}
```

## Security Considerations

- **Token Storage**: Never log tokens in plaintext
- **Session Validation**: Validate session on every request
- **Rate Limiting**: Implement rate limiting for authentication attempts
- **Token Expiration**: Enforce token timeout
- **HTTPS Only**: Require HTTPS for all authenticated requests
- **Input Validation**: Always validate input in addition to binding

## Performance Considerations

- **Minimal Logic**: Keep filter logic lightweight
- **Caching**: Cache validation results where appropriate
- **Async Support**: Use async methods for I/O operations
- **Early Exit**: Return early for non-applicable requests
- **Lazy Loading**: Only resolve dependencies when needed

## Related Components

- **Impl/SessionHelper.cs**: Token parsing and session loading
- **Impl/SessionContext.cs**: Current user session storage
- **Impl/Exceptions/**: Custom exception types
- **App_Start/FilterConfig.cs**: Global filter registration
- **Core/Application/ISessionContext.cs**: Session interface

## Migration Notes

When upgrading to newer .NET versions:
- Web API filters → ASP.NET Core middleware/filters
- `ActionFilterAttribute` → `IActionFilter` or `IAsyncActionFilter`
- DI integration differs in ASP.NET Core
- Consider using middleware for cross-cutting concerns
