# API Module - AI Context

## Purpose

The **API** module is the RESTful API layer for the MarbleLife application, built on ASP.NET Web API 2. It exposes HTTP endpoints that enable client applications (web, mobile) to interact with the business logic defined in the Core module. This layer handles HTTP concerns, authentication, authorization, routing, and data serialization.

## Architecture Overview

This is a **Web API 2** project organized using **Areas** to group related endpoints by business domain. Each area contains controllers that expose RESTful endpoints following REST conventions.

### Key Architectural Patterns
1. **Area-Based Organization**: Business domains organized as MVC Areas
2. **Controller-Based Routing**: REST endpoints defined in ApiController classes
3. **Attribute-Based Security**: Authentication/authorization via custom action filters
4. **Dependency Injection**: Constructor injection for all dependencies
5. **Global Exception Handling**: Centralized error handling and logging
6. **CORS Support**: Cross-origin requests enabled for web clients
7. **JSON Serialization**: Camel-case JSON responses

## Project Structure

```
API/
├── App_Start/              # Application startup configuration
├── Areas/                  # Business domain areas (controllers grouped by domain)
│   ├── Application/        # Core application utilities and dropdowns
│   ├── Dashboard/          # Dashboard metrics and KPIs
│   ├── Geo/               # Geographic and location services
│   ├── MarketingLead/     # Lead generation and tracking
│   ├── Organizations/      # Franchisee management
│   ├── Payment/           # Payment processing
│   ├── Reports/           # Business reporting
│   ├── Review/            # Customer review management
│   ├── Sales/             # Sales and customer operations
│   ├── Scheduler/         # Job scheduling
│   └── Users/             # User authentication and management
├── Attribute/             # Custom action filters (auth, validation, transactions)
├── DependencyInjection/   # IoC container setup
├── Enum/                  # API-specific enumerations
├── Impl/                  # API helper implementations
├── Templates/             # Razor templates for PDF/Excel generation
├── Global.asax.cs         # Application lifecycle events
└── Properties/            # Assembly metadata
```

## Global Configuration (Global.asax.cs)

The application entry point configures:
- **Area Registration**: Loads all Area routes
- **Dependency Injection**: Registers all services via `ApiDependencyRegistrar`
- **Web API Configuration**: JSON formatting, routing rules
- **FluentValidation**: Model validation framework
- **CORS Headers**: Allows cross-origin requests
- **Session Management**: Token-based authentication setup
- **TLS 1.2**: Secure communication protocol
- **Global Error Handling**: Logs all unhandled exceptions

### Request Lifecycle
1. **BeginRequest**: Sets up session context from token header, configures localization
2. **Controller Action**: Executes with authentication/authorization filters
3. **EndRequest**: Clears context storage to prevent memory leaks
4. **Error**: Logs exceptions to `ILogService`

## Areas Structure

Each **Area** represents a business domain and contains:
- **AreaRegistration.cs**: Route configuration for the area
- **Controllers/**: API controllers exposing endpoints
- **ViewModel/**: Request/response models (optional, often uses Core ViewModels)
- **Enum/**: Area-specific enumerations (optional)
- **Impl/**: Area-specific helper services (optional)

### Available Areas
| Area | Purpose |
|------|---------|
| **Application** | Dropdown helpers, lookup data, common utilities |
| **Dashboard** | KPIs, metrics, aggregated business data |
| **Geo** | Geographic services, territory management |
| **MarketingLead** | Lead capture, tracking, conversion |
| **Organizations** | Franchisee management, fee structures |
| **Payment** | Payment processing, billing operations |
| **Reports** | Business intelligence, data exports |
| **Review** | Customer reviews, ratings, feedback |
| **Sales** | Customer management, sales pipeline, invoicing |
| **Scheduler** | Job scheduling, appointments, technician dispatch |
| **Users** | Authentication, user management, roles |

## API Conventions

### Routing
Routes are configured in `App_Start/WebApiConfig.cs`:
- **GET**: `/{area}/{controller}` or `/{area}/{controller}/{id}`
- **POST**: `/{area}/{controller}` (body contains data)
- **PUT**: `/{area}/{controller}/{id}` (body contains updated data)
- **DELETE**: `/{area}/{controller}/{id}`
- **Custom Actions**: `/{area}/{controller}/{action}`

### HTTP Methods
- **GET**: Retrieve data (idempotent, cacheable)
- **POST**: Create new resources or complex queries
- **PUT**: Update existing resources
- **DELETE**: Remove resources

### Request/Response Format
- **Content-Type**: `application/json`
- **Request Body**: JSON with camelCase properties
- **Response**: JSON with camelCase properties
- **Success Response**: Data object or boolean
- **Error Response**: Handled by `BasicExceptionFilterAttribute`

### Headers
- **token**: Authentication token (JWT or session ID)
- **timezoneoffset**: Client timezone offset for date conversions
- **Access-Control-Allow-Origin**: CORS header (set to `*`)

## Authentication & Authorization

### Authentication Flow
1. User logs in via `Users` area (typically `LoginController`)
2. Server generates token and returns to client
3. Client includes token in `token` header for all subsequent requests
4. `BasicAuthenticationAttribute` validates token on each request
5. `SessionHelper` loads user session from token
6. `ISessionContext` provides current user information to controllers

### Authorization Levels
- **[AllowAnonymous]**: No authentication required (login, public endpoints)
- **[BasicAuthentication]**: Requires valid token (default for most endpoints)
- **Role-Based**: Controllers check `ISessionContext.UserSession.RoleId` for authorization

### Security Attributes
Located in `Attribute/` folder:
- **BasicAuthenticationAttribute**: Token validation
- **BasicExceptionFilterAttribute**: Exception handling
- **DbTransactionFilterAttribute**: Database transaction management
- **CustomDataBinderAttribute**: Custom model binding

## Dependency Injection

Configured in `DependencyInjection/ApiDependencyRegistrar.cs`:
- Registers API-specific services (`ISessionContext`, `IAppContextStore`)
- Delegates to Core/Infrastructure for domain service registration
- Uses custom `DependencyResolver` for Web API integration

Controllers receive dependencies via constructor injection:
```csharp
public FranchiseeController(
    IFranchiseeInfoService franchiseeService,
    ISessionContext sessionContext)
{
    _franchiseeService = franchiseeService;
    _sessionContext = sessionContext;
}
```

## Common API Patterns

### Standard CRUD Controller
```csharp
[BasicAuthentication]
public class EntityController : BaseController
{
    private readonly IEntityService _service;
    private readonly ISessionContext _session;

    public EntityController(IEntityService service, ISessionContext session)
    {
        _service = service;
        _session = session;
    }

    [HttpGet]
    public EntityViewModel Get(long id)
    {
        return _service.Get(id);
    }

    [HttpGet]
    public PagedResult<EntityViewModel> Get([FromUri] FilterModel filter, int pageNumber, int pageSize)
    {
        return _service.GetPaged(filter, pageNumber, pageSize);
    }

    [HttpPost]
    public bool Post([FromBody] EntityViewModel model)
    {
        model.CreatedBy = _session.UserSession.UserId;
        _service.Save(model);
        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Saved successfully.");
        return true;
    }

    [HttpPut]
    public bool Put(long id, [FromBody] EntityViewModel model)
    {
        model.Id = id;
        _service.Update(model);
        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Updated successfully.");
        return true;
    }

    [HttpDelete]
    public bool Delete(long id)
    {
        _service.Delete(id);
        ResponseModel.SetSuccessMessage("Deleted successfully.");
        return true;
    }
}
```

### Response Models
Controllers use `BaseController` properties:
- **ResponseModel**: For setting success/error messages
- **PostResponseModel**: For POST operation responses with messages

### Error Handling
- `BasicExceptionFilterAttribute` catches all exceptions
- Exceptions logged via `ILogService`
- Friendly error messages returned to client
- Stack traces excluded from production responses

## For AI Agents

### Adding New API Endpoints

1. **Identify the Business Domain**: Determine which Area the endpoint belongs to
2. **Create or Update Controller**:
   ```csharp
   // In Areas/{Domain}/Controllers/
   [BasicAuthentication]
   public class NewEntityController : BaseController
   {
       private readonly INewEntityService _service;
       
       public NewEntityController(INewEntityService service)
       {
           _service = service;
       }
       
       [HttpGet]
       public NewEntityViewModel Get(long id)
       {
           return _service.Get(id);
       }
   }
   ```
3. **Register Area** (if new): Create `{Area}AreaRegistration.cs`
4. **Add Authorization**: Apply `[BasicAuthentication]` or `[AllowAnonymous]`
5. **Test Endpoint**: Use Postman or similar tool
6. **Update Documentation**: Document in area AI-CONTEXT.md

### Modifying Existing Endpoints

1. **Locate Controller**: Find controller in appropriate Area
2. **Update Action Method**: Modify method signature or logic
3. **Update ViewModel**: If request/response structure changes
4. **Test Changes**: Verify existing clients still work
5. **Update Core Services**: If business logic changes

### Testing Endpoints

Use `curl` or HTTP client:
```bash
# GET request
curl -H "token: YOUR_TOKEN" http://localhost/Organizations/Franchisee/123

# POST request
curl -X POST -H "token: YOUR_TOKEN" -H "Content-Type: application/json" \
  -d '{"name":"Test","email":"test@example.com"}' \
  http://localhost/Organizations/Franchisee

# DELETE request
curl -X DELETE -H "token: YOUR_TOKEN" http://localhost/Organizations/Franchisee/123
```

## For Human Developers

### Adding a New Area

1. **Create Area Folder**: `Areas/{NewArea}/`
2. **Create AreaRegistration**:
   ```csharp
   public class NewAreaAreaRegistration : AreaRegistration
   {
       public override string AreaName => "NewArea";
       
       public override void RegisterArea(AreaRegistrationContext context)
       {
           // Custom routes if needed
       }
   }
   ```
3. **Create Controllers Folder**: `Areas/{NewArea}/Controllers/`
4. **Add First Controller**: Inherit from `BaseController`
5. **Test**: Restart application, verify routes registered

### Best Practices

#### Controller Design
- Keep controllers thin (business logic in Core services)
- Use async/await for I/O operations
- Validate inputs (FluentValidation in Core)
- Return appropriate HTTP status codes
- Log important operations

#### Security
- Always use `[BasicAuthentication]` unless truly public
- Check `_sessionContext.UserSession.RoleId` for authorization
- Validate franchisee access (users can only see their data)
- Sanitize inputs to prevent injection attacks
- Never expose sensitive data (passwords, tokens, etc.)

#### Performance
- Use pagination for large datasets (`pageNumber`, `pageSize`)
- Cache frequently accessed data (in Core/Infrastructure)
- Use `[FromUri]` for query parameters, `[FromBody]` for complex objects
- Avoid N+1 queries (optimize Core services)

#### Error Handling
- Let exceptions bubble up to `BasicExceptionFilterAttribute`
- Log errors before throwing
- Return user-friendly messages
- Include correlation IDs for debugging

#### API Versioning
- Currently no versioning strategy implemented
- Consider URL-based versioning for breaking changes: `/v2/{area}/{controller}`

### Common Issues

#### 404 Not Found
- Check Area registration in Global.asax.cs
- Verify route in WebApiConfig.cs
- Ensure controller inherits from BaseController
- Check controller namespace

#### 401 Unauthorized
- Missing or invalid token header
- Session expired
- User not authenticated
- Check `BasicAuthenticationAttribute` is applied

#### 500 Internal Server Error
- Check logs via `ILogService`
- Verify service dependencies registered
- Check for null references
- Validate database connection

### Templates Folder

Contains **Razor templates** for generating:
- PDF invoices (`cutomer_invoice.cshtml`, `invoice-job-attacktment.cshtml`)
- Excel reports (`addin-alarm-history-list.excel.cshtml`)
- HTML for emails or printing

Templates use Razor syntax and are rendered by controllers returning `HttpResponseMessage` with appropriate content type.

## Dependencies

- **Core**: Business logic, domain models, service interfaces
- **Infrastructure**: Database access, external service integrations
- **System.Web.Http**: Web API framework
- **Newtonsoft.Json**: JSON serialization
- **FluentValidation**: Model validation
- **Autofac** (or similar): Dependency injection container

## Configuration Files

- **Web.config**: Application settings, connection strings, bindings
- **packages.config**: NuGet package dependencies
- **Global.asax.cs**: Application lifecycle hooks

## Testing

- No automated tests currently in this project
- Manual testing via HTTP clients (Postman, curl)
- Consider adding:
  - Unit tests for controller logic
  - Integration tests for end-to-end scenarios
  - Load tests for performance validation

## Next Steps for Enhancement

1. **API Documentation**: Add Swagger/OpenAPI for interactive documentation
2. **API Versioning**: Implement versioning strategy for breaking changes
3. **Rate Limiting**: Prevent abuse with rate limiting middleware
4. **Response Caching**: Add caching headers for GET requests
5. **Request Validation**: Enhance input validation with FluentValidation
6. **Health Checks**: Add `/health` endpoint for monitoring
7. **Metrics**: Instrument API with telemetry (Application Insights, etc.)
