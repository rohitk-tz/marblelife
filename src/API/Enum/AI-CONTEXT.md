# API/Enum - AI Context

## Purpose

The **Enum** folder contains API-specific enumeration types that are used exclusively by the API layer for HTTP-related concerns, request processing, and API-specific business logic that doesn't belong in the Core module.

## Key File: HttpMethodType.cs

### Purpose
Defines enumeration for HTTP methods used in API operations, providing a strongly-typed way to specify HTTP verbs in code.

### Definition
```csharp
public enum HttpMethodType
{
    GET = 1,
    POST = 2,
    PUT = 3,
    DELETE = 4,
    PATCH = 5,
    OPTIONS = 6,
    HEAD = 7
}
```

### Usage Scenarios

#### 1. Custom Routing Logic
```csharp
public class CustomRouteHandler
{
    public bool CanHandle(HttpMethodType method)
    {
        return method == HttpMethodType.GET || method == HttpMethodType.POST;
    }
}
```

#### 2. API Documentation Generation
```csharp
public class ApiDocumentationGenerator
{
    public string GetMethodDescription(HttpMethodType method)
    {
        return method switch
        {
            HttpMethodType.GET => "Retrieves resources",
            HttpMethodType.POST => "Creates new resources",
            HttpMethodType.PUT => "Updates existing resources",
            HttpMethodType.DELETE => "Removes resources",
            _ => "Unknown operation"
        };
    }
}
```

#### 3. Logging and Audit Trail
```csharp
public class RequestLogger
{
    public void LogRequest(HttpMethodType method, string url, string userId)
    {
        _logService.Info($"User {userId} executed {method} on {url}");
    }
}
```

#### 4. Permission Validation
```csharp
public class PermissionValidator
{
    public bool HasPermission(RoleType role, HttpMethodType method)
    {
        // ReadOnly roles can only use GET
        if (role == RoleType.ReadOnly && method != HttpMethodType.GET)
            return false;
            
        return true;
    }
}
```

## HTTP Method Semantics

### GET (Read)
- **Purpose**: Retrieve resource(s)
- **Idempotent**: Yes (multiple calls have same effect)
- **Safe**: Yes (doesn't modify data)
- **Cacheable**: Yes
- **Request Body**: No
- **Examples**:
  - `GET /Organizations/Franchisee/123` - Get single franchisee
  - `GET /Sales/Customer?status=active` - Get filtered customers

### POST (Create/Complex Operations)
- **Purpose**: Create new resource or perform complex operations
- **Idempotent**: No (multiple calls create multiple resources)
- **Safe**: No (modifies data)
- **Cacheable**: No
- **Request Body**: Yes
- **Examples**:
  - `POST /Organizations/Franchisee` - Create new franchisee
  - `POST /Reports/Generate` - Generate complex report

### PUT (Update)
- **Purpose**: Update existing resource (full replacement)
- **Idempotent**: Yes (same update repeated has same result)
- **Safe**: No (modifies data)
- **Cacheable**: No
- **Request Body**: Yes
- **Examples**:
  - `PUT /Organizations/Franchisee/123` - Update franchisee
  - `PUT /Sales/Customer/456` - Update customer information

### DELETE (Remove)
- **Purpose**: Remove resource
- **Idempotent**: Yes (deleting same resource twice has same result)
- **Safe**: No (modifies data)
- **Cacheable**: No
- **Request Body**: No (typically)
- **Examples**:
  - `DELETE /Organizations/Franchisee/123` - Delete franchisee
  - `DELETE /Sales/Invoice/789` - Delete invoice

### PATCH (Partial Update)
- **Purpose**: Partially update resource (specific fields)
- **Idempotent**: Depends on implementation
- **Safe**: No (modifies data)
- **Cacheable**: No
- **Request Body**: Yes (only fields to update)
- **Examples**:
  - `PATCH /Organizations/Franchisee/123` - Update only status field
  - `PATCH /Sales/Customer/456` - Update only email

### OPTIONS (Metadata)
- **Purpose**: Get supported HTTP methods for resource
- **Idempotent**: Yes
- **Safe**: Yes
- **Cacheable**: Yes
- **Request Body**: No
- **Examples**:
  - `OPTIONS /Organizations/Franchisee` - Returns: GET, POST, PUT, DELETE
  - Used for CORS preflight requests

### HEAD (Headers Only)
- **Purpose**: Same as GET but returns only headers (no body)
- **Idempotent**: Yes
- **Safe**: Yes
- **Cacheable**: Yes
- **Request Body**: No
- **Examples**:
  - `HEAD /Reports/Large` - Check if report exists/size without downloading

## REST Conventions in MarbleLife API

### Resource Naming
- Use **plural nouns** for collections: `/Franchisees`, `/Customers`
- Use **singular noun** for single resource: `/Franchisee/123`, `/Customer/456`
- Area acts as namespace: `/Organizations/Franchisee`, `/Sales/Customer`

### Standard CRUD Mapping
```
GET    /Organizations/Franchisee        → List franchisees
GET    /Organizations/Franchisee/123    → Get franchisee by ID
POST   /Organizations/Franchisee        → Create new franchisee
PUT    /Organizations/Franchisee/123    → Update franchisee
DELETE /Organizations/Franchisee/123    → Delete franchisee
```

### Non-CRUD Operations
For operations that don't fit CRUD:
```
POST /Sales/Invoice/123/Send           → Send invoice (action)
POST /Reports/Generate                 → Generate report (complex operation)
GET  /Dashboard/Metrics                → Get aggregated data
POST /Users/Login                      → Authentication (operation)
POST /Scheduler/Job/123/Reschedule    → Reschedule job (action)
```

## For AI Agents

### Adding New HTTP Method Support

1. **Add to enum** (if standard HTTP method not already present):
```csharp
public enum HttpMethodType
{
    // ... existing values
    CUSTOM = 8  // Only for non-standard methods
}
```

2. **Update routing** in `WebApiConfig.cs`:
```csharp
config.Routes.MapHttpRoute(
    name: "DefaultApiCustomMethod",
    routeTemplate: "{area}/{controller}/{id}",
    defaults: new { action = "CustomAction", id = RouteParameter.Optional },
    constraints: new { httpMethod = new HttpMethodConstraint(new HttpMethod("CUSTOM")) }
);
```

3. **Implement controller action**:
```csharp
[HttpCustomMethod]
public ActionResult CustomAction(int id)
{
    // Implementation
}
```

### Using Enum in Code

**Type-safe method checking**:
```csharp
public void ProcessRequest(HttpMethodType method)
{
    switch (method)
    {
        case HttpMethodType.GET:
            return HandleGet();
        case HttpMethodType.POST:
            return HandlePost();
        default:
            throw new NotSupportedException($"Method {method} not supported");
    }
}
```

**Conversion from HttpRequest**:
```csharp
public HttpMethodType GetMethodType(HttpRequestMessage request)
{
    return request.Method.Method switch
    {
        "GET" => HttpMethodType.GET,
        "POST" => HttpMethodType.POST,
        "PUT" => HttpMethodType.PUT,
        "DELETE" => HttpMethodType.DELETE,
        _ => throw new NotSupportedException()
    };
}
```

## For Human Developers

### Best Practices

#### Method Selection
- **GET** for reading data (safe, idempotent)
- **POST** for creating resources or complex operations
- **PUT** for full updates (idempotent)
- **PATCH** for partial updates (rare in this codebase)
- **DELETE** for removing resources (idempotent)

#### Idempotency
Ensure idempotent methods (GET, PUT, DELETE) can be safely retried:
```csharp
[HttpPut]
public bool UpdateFranchisee(long id, FranchiseeViewModel model)
{
    // Check if already in desired state
    var existing = _service.Get(id);
    if (existing.LastModified >= model.LastModified)
        return true; // Already updated
    
    _service.Update(id, model);
    return true;
}
```

#### Security by Method
```csharp
public class MethodBasedAuthorization : ActionFilterAttribute
{
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        var method = GetHttpMethodType(actionContext.Request.Method);
        var role = GetUserRole();
        
        // Read-only users can only use GET
        if (role == RoleType.ReadOnly && method != HttpMethodType.GET)
        {
            throw new UnauthorizedAccessException("Insufficient permissions");
        }
    }
}
```

#### Error Responses by Method
```csharp
private HttpStatusCode GetAppropriateErrorCode(HttpMethodType method, Exception ex)
{
    return (method, ex) switch
    {
        (HttpMethodType.GET, NotFoundException) => HttpStatusCode.NotFound,
        (HttpMethodType.POST, ValidationException) => HttpStatusCode.BadRequest,
        (HttpMethodType.POST, DuplicateException) => HttpStatusCode.Conflict,
        (HttpMethodType.PUT, NotFoundException) => HttpStatusCode.NotFound,
        (HttpMethodType.PUT, ValidationException) => HttpStatusCode.BadRequest,
        (HttpMethodType.DELETE, NotFoundException) => HttpStatusCode.NotFound,
        (HttpMethodType.DELETE, ForeignKeyException) => HttpStatusCode.Conflict,
        _ => HttpStatusCode.InternalServerError
    };
}
```

### Common Patterns

#### Safe Read Operations
```csharp
[HttpGet]
public IHttpActionResult GetFranchisee(long id)
{
    var franchisee = _service.Get(id);
    if (franchisee == null)
        return NotFound();
    
    return Ok(franchisee);
}
```

#### Creation with Location Header
```csharp
[HttpPost]
public IHttpActionResult CreateFranchisee(FranchiseeViewModel model)
{
    var id = _service.Create(model);
    
    return Created(
        new Uri($"/Organizations/Franchisee/{id}", UriKind.Relative),
        new { id, message = "Franchisee created successfully" }
    );
}
```

#### Idempotent Updates
```csharp
[HttpPut]
public IHttpActionResult UpdateFranchisee(long id, FranchiseeViewModel model)
{
    if (!_service.Exists(id))
        return NotFound();
    
    _service.Update(id, model);
    return Ok(new { message = "Franchisee updated successfully" });
}
```

#### Safe Deletes
```csharp
[HttpDelete]
public IHttpActionResult DeleteFranchisee(long id)
{
    if (!_service.Exists(id))
        return NotFound(); // Idempotent - already deleted
    
    if (_service.HasDependencies(id))
        return Conflict(new { message = "Cannot delete franchisee with active jobs" });
    
    _service.Delete(id);
    return Ok(new { message = "Franchisee deleted successfully" });
}
```

## Comparison with Core Enums

### API/Enum vs. Core/[Domain]/Enum
- **API/Enum**: HTTP/API-specific enumerations (HttpMethodType, etc.)
- **Core/Enum**: Business domain enumerations (FranchiseeStatus, RoleType, etc.)

**Rule of Thumb**:
- If enum relates to HTTP, routing, or API infrastructure → API/Enum
- If enum represents business concept or domain state → Core/[Domain]/Enum

## Related Files

- **App_Start/WebApiConfig.cs**: Uses HTTP methods in route constraints
- **Attribute/BasicAuthenticationAttribute.cs**: May check method type for auth rules
- **Controllers/**: Controller actions decorated with `[Http{Method}]` attributes
- **Core/Application/Enum/**: Domain-specific enumerations

## Testing HTTP Methods

```csharp
[Test]
public void GetFranchisee_ReturnsData()
{
    var response = client.GetAsync("/Organizations/Franchisee/123").Result;
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
}

[Test]
public void PostFranchisee_CreatesResource()
{
    var model = new FranchiseeViewModel { Name = "Test" };
    var response = client.PostAsJsonAsync("/Organizations/Franchisee", model).Result;
    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
}

[Test]
public void DeleteFranchisee_IsIdempotent()
{
    // First delete
    var response1 = client.DeleteAsync("/Organizations/Franchisee/123").Result;
    Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
    
    // Second delete - should still return success (idempotent)
    var response2 = client.DeleteAsync("/Organizations/Franchisee/123").Result;
    Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
}
```

## Additional Enums

As the API evolves, additional enums may be added to this folder:
- **ResponseFormatType**: JSON, XML, CSV (if multiple formats supported)
- **ApiVersionType**: V1, V2, V3 (if versioning implemented)
- **RateLimitPolicyType**: Free, Premium, Enterprise (if rate limiting added)
- **CacheStrategyType**: NoCache, ShortTerm, LongTerm (if caching policies added)

Keep API-specific enums here, move domain enums to appropriate Core folder.
