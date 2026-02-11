<!-- AUTO-GENERATED: Header -->
# API Module — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2026-02-10T11:11:13Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The API module serves as the primary Web API layer for the MarbleLife application, implementing ASP.NET MVC 5 + Web API 2 with Areas-based organization. It orchestrates HTTP request/response handling, token-based authentication, session management, automatic transaction management, and provides RESTful endpoints across 11 functional areas (Dashboard, Organizations, Payment, Reports, Sales, Scheduler, Users, Application, Geo, MarketingLead, Review).

### Design Patterns

- **Areas Pattern**: Organizes controllers into logical business domains (Sales, Scheduler, Payment, etc.) with dedicated route registrations
- **Action Filter Pipeline**: Global filters execute in a specific order for every request: Authentication → Data Binding → Transaction Management → Exception Handling
- **Dependency Injection**: Custom DependencyResolver integrates with ApplicationManager for per-request controller resolution
- **Request-Scoped Storage**: AppContextStore uses HttpContext.Items to maintain per-request state (session, timezone, user context)
- **Token-Based Authentication**: Stateless authentication via custom Token header, validated against active sessions in database
- **Unit of Work**: DbTransactionFilterAttribute automatically commits database changes after successful action execution
- **Exception Translation**: Converts domain exceptions (NotAuthenticatedException, ValidationFailureException) into structured JSON responses with error codes

### Data Flow

1. **Request Entry**: `Application_BeginRequest()` in Global.asax.cs
2. **CORS Headers**: Added for cross-origin support (supports GET, POST, DELETE with custom headers)
3. **Session Setup**: Token header extracted → `SessionHelper.SetSessionModel()` → populates `ISessionContext`
4. **Timezone Localization**: Client timezone offset captured from headers → stored in `IClock`
5. **Routing**: Areas route configuration determines controller/action based on URL pattern (`{area}/{controller}/{action}`)
6. **Filter Pipeline Execution**:
   - **BasicAuthenticationAttribute**: Validates token presence and active session (throws NotAuthenticatedException if invalid, allows [AllowAnonymous] bypass)
   - **CustomDataBinderAttribute**: Sets HTTP method type, validates model state, auto-populates DataRecorderMetaData with CreatedBy/ModifiedBy from session
   - **Action Execution**: Controller method runs
   - **DbTransactionFilterAttribute**: Commits UnitOfWork if no exceptions
   - **BasicExceptionFilterAttribute**: Catches exceptions, logs them, translates to ResponseModel with error codes
7. **Response Wrapping**: All responses wrapped in ResponseModel with success/error messages
8. **Cleanup**: `Application_EndRequest()` clears AppContextStore

### Key Components

#### Global Filters (Applied to All Requests)
```csharp
// Execution order in FilterConfig:
1. BasicAuthenticationAttribute    // Token validation
2. CustomDataBinderAttribute        // Model binding & validation
3. DbTransactionFilterAttribute     // Transaction commit
4. BasicExceptionFilterAttribute    // Exception handling
```

#### Session Management
- **Token Storage**: Headers["Token"] → validated against UserLog table
- **Session Context**: UserSessionModel contains OrganizationRoleUserId, permissions, franchisee info
- **Lifecycle**: Created on login → validated per-request → destroyed on logout
- **Timezone Handling**: Client offset stored per-request for date/time conversions

#### Route Configuration
```csharp
// WebApiConfig.cs routes (priority order):
1. Attribute routes (highest priority)
2. Area-specific custom routes (e.g., dashboard/revenue/{franchiseeId})
3. Convention routes:
   - {area}/{controller}/{id} with numeric ID constraint
   - {area}/{controller}/{action}
   - RESTful routes by HTTP method (GET, POST, PUT, DELETE)
```

#### Dependency Injection
- **Registration**: ApiDependencyRegistrar.RegisterDepndencies() in Application_Start
- **Resolution**: Custom DependencyResolver checks if type inherits from BaseController/ApiController
- **Scoping**: Per-request via AppContextStore (cleared in Application_EndRequest)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Session Types
```csharp
// ISessionContext - Per-request user context
public interface ISessionContext
{
    string Token { get; set; }                    // Session token from header
    UserSessionModel UserSession { get; set; }    // Active user session data
}

// UserSessionModel - Contains authenticated user information
// - OrganizationRoleUserId: Current user's role ID
// - Permissions: User authorization claims
// - FranchiseeId: Associated franchisee (if applicable)
```

### Request/Response Models
```csharp
// ResponseModel - Base response wrapper for all API calls
public class ResponseModel
{
    public object Data { get; set; }                      // Response payload
    public string Message { get; set; }                   // Success/error message
    public ResponseErrorCodeType ErrorCode { get; set; }  // Structured error code
    public bool IsFeedbackSet { get; }                    // Has feedback been set
}

// PostResponseModel - Response for POST/PUT operations
public class PostResponseModel : ResponseModel
{
    public ModelValidationOutput ModelValidation { get; set; }  // Validation results
}

// ModelValidationOutput - Validation error details
public class ModelValidationOutput
{
    public bool IsValid { get; set; }
    public List<ModelValidationItem> Errors { get; set; }  // Field-level errors
}
```

### Exception Types
```csharp
// Custom exceptions mapped to error codes
NotAuthenticatedException        → ResponseErrorCodeType.UserNotAuthenticated
ValidationFailureException       → ResponseErrorCodeType.ValidationFailure
UserBlockedException            → ResponseErrorCodeType.UserBlocked
InvalidDataProvidedException    → ResponseErrorCodeType.InvalidData
```

### Enumerations
```csharp
// HttpMethodType - Request method classification
public enum HttpMethodType
{
    Get,
    Post,
    Put,
    Delete
}

// ResponseErrorCodeType - Structured error codes
public enum ResponseErrorCodeType
{
    RandomException,
    ValidationFailure,
    UserNotAuthenticated,
    UserBlocked,
    InvalidData
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Custom Attributes (Global Filters)

#### `BasicAuthenticationAttribute : ActionFilterAttribute`
- **Input**: HttpActionContext with request headers
- **Behavior**: 
  - Validates Token header presence and format
  - Checks ISessionContext.UserSession is populated
  - Allows bypass with [AllowAnonymous] attribute
  - Throws NotAuthenticatedException if validation fails
- **Side-effects**: None (read-only validation)

#### `DbTransactionFilterAttribute : ActionFilterAttribute`
- **Input**: HttpActionExecutedContext after action completes
- **Output**: None
- **Behavior**: 
  - Calls IUnitOfWork.SaveChanges() if no exception occurred
  - Commits all pending database changes atomically
  - Skipped if action threw exception
- **Side-effects**: Persists all entity changes to database

#### `CustomDataBinderAttribute : ActionFilterAttribute`
- **Input**: HttpActionContext with action arguments
- **Behavior**: 
  - Sets HTTP method type on controller (Get/Post/Put/Delete)
  - Validates ModelState and populates ModelValidationOutput
  - Auto-fills DataRecorderMetaData (CreatedBy, DateCreated, ModifiedBy, DateModified)
  - Throws ValidationFailureException if ModelState.IsValid = false
  - Wraps successful responses in ResponseModel
- **Side-effects**: Modifies action argument objects (sets audit fields)

#### `BasicExceptionFilterAttribute : ExceptionFilterAttribute`
- **Input**: HttpActionExecutedContext with exception
- **Output**: ResponseModel with error details
- **Behavior**: 
  - Logs exception to ILogService
  - Maps exception type to ResponseErrorCodeType
  - Wraps error in ResponseModel with message and code
  - Sets HTTP 500 status if response null
- **Side-effects**: Writes to application logs

### Core Services

#### `SessionHelper` (Static Utility)
**CreateNewSession(HttpRequestBase, UserLogin)**
- **Input**: HTTP request, user login entity
- **Output**: Session token (GUID string)
- **Behavior**: Generates new session ID, logs session to UserLog table with browser/OS metadata, builds SessionContext
- **Side-effects**: Inserts UserLog record, populates ISessionContext

**SetSessionModel(string token)**
- **Input**: Session token from header
- **Output**: UserSessionModel or null
- **Behavior**: Retrieves active session from database, populates ISessionContext
- **Side-effects**: Modifies ISessionContext (Token, UserSession)

**DestroySession(string token)**
- **Input**: Session token
- **Output**: None
- **Behavior**: Marks session as ended in UserLog table
- **Side-effects**: Updates UserLog.EndTime

**SetClientTimeZone()**
- **Input**: Reads TimeZoneOffset/TimeZoneName from request headers
- **Output**: None
- **Behavior**: Configures IClock.BrowserTimeZone for date/time localization
- **Side-effects**: Modifies IClock instance

#### `AppContextStore : IAppContextStore`
**AddItem(string key, object item)**
- **Input**: Key-value pair
- **Output**: None
- **Behavior**: Stores object in HttpContext.Items dictionary for request lifetime
- **Side-effects**: Throws if key exists or item is null

**Get(string key)**
- **Input**: Key
- **Output**: Stored object or null
- **Behavior**: Retrieves object from request-scoped dictionary

**ClearStorage()**
- **Input**: None
- **Output**: None
- **Behavior**: Disposes IDisposable objects, clears dictionary, removes from HttpContext.Items
- **Side-effects**: Called in Application_EndRequest, releases resources

#### `BaseController : ApiController`
**SetPostResponseModel(HttpMethodType)**
- **Input**: HTTP method type
- **Output**: None
- **Behavior**: Initializes PostResponseModel with empty validation
- **Side-effects**: Sets ResponseModel property

**SetData(object data)**
- **Input**: Response payload
- **Output**: ResponseModel
- **Behavior**: Wraps data in ResponseModel, sets default success message if not already set
- **Side-effects**: Modifies ResponseModel.Data

**SetValidationResult(ModelStateDictionary)**
- **Input**: Model state from action context
- **Output**: None
- **Behavior**: Extracts field-level validation errors, populates ModelValidationOutput
- **Side-effects**: Sets PostResponseModel.ModelValidation

### Area Structure

| Area | Controllers | Purpose |
|------|-------------|---------|
| **Application** | BaseController, FileController, DropdownController | Base controller for all APIs, file upload/download, shared dropdown data |
| **Dashboard** | DashboardController | Revenue charts, leaderboards, sales summaries, recent invoices, customer counts, document summaries |
| **Organizations** | FranchiseeController, FranchiseeDocumentController | Franchisee CRUD, document management |
| **Payment** | PaymentController, FranchiseeAccountCreditController | Payment processing, account credit management |
| **Reports** | MLFSReportController, PriceEstimateController, ARReportController, CustomerEmailReportController, GrowthReportController | Various business reports |
| **Sales** | SalesFunnelController, InvoiceController, CustomerController, SalesInvoiceController, BatchController, SalesController | Sales pipeline, invoice management, customer CRUD |
| **Scheduler** | CalendarController, EstimateController, EstimateInvoiceController, GeoCodeController | Appointment scheduling, estimate management, geocoding |
| **Users** | (User management controllers) | User authentication, authorization |
| **Geo** | (Geographic controllers) | Geographic data management |
| **MarketingLead** | WebLeadController, MarketingLeadController, MarketingLeadGraphController, RoutingNumberController | Lead capture, lead management, analytics |
| **Review** | (Review controllers) | Customer review management |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **[Core.Application](../../Core/.context/CONTEXT.md)** — Application infrastructure (DI container, ILogService, IClock, IUnitOfWork)
- **[Core.Users](../../Core/.context/CONTEXT.md)** — User domain models (UserLogin, UserSessionModel, ISessionFactory)
- **[Core.Sales](../../Core/.context/CONTEXT.md)** — Sales domain models and services

### External Dependencies
- **ASP.NET Web API 2** — HTTP request pipeline, routing, controllers
- **System.Web.Http** — Web API framework
- **System.Web.Mvc** — MVC framework for Areas
- **FluentValidation.WebApi** — Model validation integration
- **Newtonsoft.Json** — JSON serialization (camelCase contract resolver)
- **Ionic.Zip** — File compression utilities

### Configuration Dependencies
- **Web.config** — Connection strings, app settings
- **Global.asax** — Application lifecycle events
- **WebApiConfig** — Route configuration
- **FilterConfig** — Global filter registration

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Authentication Flow
1. Client sends request with `Token` header
2. `Application_BeginRequest` extracts token → calls `SessionHelper.SetSessionModel(token)`
3. `BasicAuthenticationAttribute` validates `ISessionContext.UserSession != null`
4. If valid, request proceeds; if invalid, throws NotAuthenticatedException (mapped to 401-like response)

### Transaction Management
- All database changes are deferred until `DbTransactionFilterAttribute.OnActionExecuted()`
- Single commit point ensures atomic operations
- Exceptions prevent commit (automatic rollback)
- No need for explicit SaveChanges() in controller actions

### Model Validation Strategy
- FluentValidation registered as global validator provider
- CustomDataBinderAttribute checks ModelState before action execution
- Validation errors returned in structured format with field names
- Throws ValidationFailureException to abort request

### CORS Configuration
- Wildcard "*" origin (permissive for all clients)
- Supports GET, POST, DELETE methods
- Custom headers: token, timezoneoffset
- OPTIONS preflight handled in Application_BeginRequest

### Timezone Handling
- Client sends offset in minutes via `Timezoneoffset` header
- Stored in `IClock.BrowserTimeZone` per-request
- Used throughout application for date/time conversions
- Ensures UI displays dates in user's local timezone

### Error Handling Strategy
- Domain exceptions carry business logic errors (e.g., NotAuthenticatedException)
- BasicExceptionFilterAttribute catches all exceptions
- CustomBaseException logged with URL and token context
- All errors wrapped in ResponseModel with error codes for client parsing

### Security Considerations
- Token validation on every request (except [AllowAnonymous])
- Session stored in database (can be revoked server-side)
- User agent and IP tracked for audit trail
- TLS 1.2 enforced via ServicePointManager.SecurityProtocol

### Performance Notes
- AppContextStore uses HttpContext.Items (in-memory, per-request)
- Session loaded once per request (cached in AppContextStore)
- Dependency injection resolves controllers per-request (not singleton)
- Database changes batched in single SaveChanges() call

<!-- END CUSTOM SECTION -->
