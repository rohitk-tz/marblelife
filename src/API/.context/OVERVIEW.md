<!-- AUTO-GENERATED: Header -->
# API Module
> ASP.NET Web API layer with Areas-based organization, token authentication, and automatic transaction management
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The API module is the HTTP gateway for the MarbleLife application. Think of it as a well-orchestrated airport security checkpoint: every request passes through multiple layers (authentication, validation, transaction management, exception handling) before reaching its destination controller. 

**Why this architecture?** The Areas pattern organizes dozens of controllers into business-focused modules (Sales, Scheduler, Payment, etc.), making the codebase navigable as it scales. The global filter pipeline ensures consistent security, validation, and error handling across all 80+ controllers without boilerplate code in each action.

**Key Innovation**: The `DbTransactionFilterAttribute` implements Unit of Work at the HTTP request level—controllers write business logic without worrying about transaction management. If an action succeeds, changes are committed; if it throws, everything rolls back automatically.

**Authentication Model**: Token-based sessions stored in the database (not JWT) allow server-side revocation and audit trails. The session context is rehydrated on every request from the token header, providing controller actions with user identity and permissions.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup

This is a runtime module, not a library. Configuration happens in `Global.asax.cs` and `App_Start/`:

```bash
# No installation steps—module is part of the main application
# Configuration files:
# - Global.asax.cs: Application lifecycle events
# - App_Start/WebApiConfig.cs: Route registration
# - App_Start/FilterConfig.cs: Global filters
# - Web.config: Connection strings, app settings
```

### Example: Creating a New API Controller

```csharp
using Api.Areas.Application.Controller;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService; // Injected via DependencyResolver
        }
        
        // GET /sales/order/123
        public IHttpActionResult Get(long id)
        {
            var order = _orderService.GetById(id);
            return Ok(order); // Auto-wrapped in ResponseModel by CustomDataBinderAttribute
        }
        
        // POST /sales/order
        public IHttpActionResult Post(CreateOrderModel model)
        {
            // ModelState already validated by CustomDataBinderAttribute
            // DataRecorderMetaData already populated with CreatedBy/DateCreated
            
            var orderId = _orderService.CreateOrder(model);
            
            // DbTransactionFilterAttribute commits changes here
            return Ok(new { OrderId = orderId });
        }
        
        // Protected endpoint - requires Token header
        public IHttpActionResult GetSecretData()
        {
            // BasicAuthenticationAttribute validated token
            // Session available via DI:
            var sessionContext = ApplicationManager.DependencyInjection
                .Resolve<ISessionContext>();
            var userId = sessionContext.UserSession.OrganizationRoleUserId;
            
            return Ok(new { UserId = userId });
        }
        
        // Public endpoint - no authentication required
        [AllowAnonymous]
        public IHttpActionResult GetPublicData()
        {
            return Ok(new { Message = "No token needed" });
        }
    }
}
```

### Example: Custom Area Registration

```csharp
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace API.Areas.Sales
{
    public class SalesAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Sales";
        
        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Custom route for sales funnel
            context.Routes.MapHttpRoute(
                "sales_funnel",
                "sales/funnel/{franchiseeId}",
                new { Controller = "SalesFunnel", Action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            
            // Convention routes handled by WebApiConfig
        }
    }
}
```

### Example: Client-Side API Call

```javascript
// JavaScript client making authenticated API call
async function fetchDashboard(franchiseeId) {
    const response = await fetch(`/dashboard/revenue/${franchiseeId}`, {
        method: 'GET',
        headers: {
            'Token': localStorage.getItem('sessionToken'),
            'Timezoneoffset': new Date().getTimezoneOffset().toString()
        }
    });
    
    const result = await response.json();
    
    if (result.errorCode === 0) {
        console.log('Success:', result.data);
    } else {
        console.error('Error:', result.message, result.errorCode);
        // errorCode mapping:
        // 1 = ValidationFailure
        // 2 = UserNotAuthenticated
        // 3 = UserBlocked
        // 4 = InvalidData
    }
}
```

### Example: Handling Validation Errors

```csharp
// Client receives validation errors in structured format
// POST /sales/customer with invalid data returns:
{
    "data": null,
    "message": "Validation failed",
    "errorCode": 1, // ValidationFailure
    "modelValidation": {
        "isValid": false,
        "errors": [
            { "field": "Email", "message": "Invalid email format" },
            { "field": "PhoneNumber", "message": "Required field" }
        ]
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Global Filters (Applied to All Requests)

| Filter | Execution Order | Purpose |
|--------|-----------------|---------|
| `BasicAuthenticationAttribute` | 1 | Validates token header, checks session, allows [AllowAnonymous] bypass |
| `CustomDataBinderAttribute` | 2 | Binds models, validates ModelState, populates audit fields, wraps responses |
| `DbTransactionFilterAttribute` | 3 | Commits database transaction after successful action execution |
| `BasicExceptionFilterAttribute` | 4 | Catches exceptions, logs them, maps to error codes, wraps in ResponseModel |

### Session Management

| Method | Purpose |
|--------|---------|
| `SessionHelper.CreateNewSession(request, userLogin)` | Generates session token, logs to UserLog table, populates SessionContext |
| `SessionHelper.SetSessionModel(token)` | Retrieves session from database, populates ISessionContext for current request |
| `SessionHelper.DestroySession(token)` | Marks session as ended in database |
| `SessionHelper.SetClientTimeZone()` | Captures client timezone offset from headers |

### Request-Scoped Storage

| Method | Purpose |
|--------|---------|
| `AppContextStore.AddItem(key, item)` | Stores object in HttpContext.Items for request lifetime |
| `AppContextStore.Get(key)` | Retrieves object from request-scoped storage |
| `AppContextStore.ClearStorage()` | Disposes resources, called in Application_EndRequest |

### Base Controller Methods

| Method | Purpose |
|--------|---------|
| `SetPostResponseModel(HttpMethodType)` | Initializes PostResponseModel for POST/PUT operations |
| `SetResponseModel(HttpMethodType)` | Initializes ResponseModel for GET/DELETE operations |
| `SetData(object)` | Wraps response data in ResponseModel |
| `SetValidationResult(ModelStateDictionary)` | Populates validation errors from ModelState |

### Areas and Route Patterns

| Area | Route Pattern Example | Controller Example |
|------|----------------------|-------------------|
| Application | `/application/{controller}/{action}` | BaseController, FileController, DropdownController |
| Dashboard | `/dashboard/revenue/{franchiseeId}` | DashboardController |
| Organizations | `/organizations/franchisee/{id}` | FranchiseeController |
| Payment | `/payment/{controller}/{action}` | PaymentController |
| Reports | `/reports/{controller}/{action}` | MLFSReportController, ARReportController |
| Sales | `/sales/{controller}/{id}` | SalesFunnelController, InvoiceController, CustomerController |
| Scheduler | `/scheduler/{controller}/{action}` | CalendarController, EstimateController |
| Users | `/users/{controller}/{action}` | (User management) |
| Geo | `/geo/{controller}/{action}` | (Geographic services) |
| MarketingLead | `/marketinglead/{controller}/{action}` | WebLeadController, MarketingLeadController |
| Review | `/review/{controller}/{action}` | (Review management) |

### HTTP Status and Error Codes

| HTTP Method | Conventional Route | Action Name |
|-------------|-------------------|-------------|
| GET | `{area}/{controller}/{id?}` | Get |
| POST | `{area}/{controller}` | Post |
| PUT | `{area}/{controller}/{id?}` | Put |
| DELETE | `{area}/{controller}/{id?}` | Delete |

| ResponseErrorCodeType | Value | Meaning |
|----------------------|-------|---------|
| RandomException | 0 | Unhandled exception |
| ValidationFailure | 1 | Model validation failed |
| UserNotAuthenticated | 2 | Invalid or missing token |
| UserBlocked | 3 | User account blocked |
| InvalidData | 4 | Business logic validation failed |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "NotAuthenticatedException" on every request
**Symptom**: All API calls fail with `errorCode: 2` (UserNotAuthenticated)  
**Causes**:
1. Token header missing from request—verify client sends `Token: {guid}` header
2. Session expired in database—check UserLog table for active session
3. ISessionContext not populated—verify Application_BeginRequest calls SetSessionModel

**Fix**:
```javascript
// Ensure client includes token header
fetch('/api/endpoint', {
    headers: { 'Token': sessionToken }
})
```

### "ValidationFailureException" with no error details
**Symptom**: API returns `errorCode: 1` but `modelValidation.errors` is empty  
**Cause**: FluentValidation validators not registered or model validation bypassed  
**Fix**: Verify Global.asax.cs has:
```csharp
GlobalConfiguration.Configuration.Services.Add(
    typeof(System.Web.Http.Validation.ModelValidatorProvider),
    new FluentValidationModelValidatorProvider(new ValidatorFactory())
);
```

### Database changes not persisting
**Symptom**: Controller calls service methods but changes don't save  
**Causes**:
1. DbTransactionFilterAttribute not registered—check FilterConfig.cs
2. Exception thrown after changes (automatic rollback)
3. UnitOfWork scope issue

**Fix**: Verify filters registered in order:
```csharp
filters.Add(new BasicAuthenticationAttribute());
filters.Add(new CustomDataBinderAttribute());
filters.Add(new DbTransactionFilterAttribute()); // Must be here
filters.Add(new BasicExceptionFilterAttribute());
```

### CORS errors in browser console
**Symptom**: `Access-Control-Allow-Origin` error  
**Cause**: Application_BeginRequest not adding CORS headers  
**Fix**: Verify Global.asax.cs has:
```csharp
protected void Application_BeginRequest()
{
    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
    // Handle OPTIONS preflight...
}
```

### Dependency injection returning null
**Symptom**: Constructor parameters are null in controller  
**Cause**: DependencyResolver not configured or dependency not registered  
**Fix**:
1. Verify WebApiConfig.cs sets custom resolver: `config.DependencyResolver = new DependencyResolver();`
2. Check ApiDependencyRegistrar.RegisterDepndencies() in Application_Start
3. Ensure dependency registered in DI container

### Timezone issues with dates
**Symptom**: Dates displayed in wrong timezone  
**Cause**: Client not sending Timezoneoffset header  
**Fix**:
```javascript
// Send timezone offset with every request
fetch('/api/endpoint', {
    headers: {
        'Token': sessionToken,
        'Timezoneoffset': new Date().getTimezoneOffset().toString()
    }
})
```

### DataRecorderMetaData not populated
**Symptom**: CreatedBy/ModifiedBy fields are null  
**Cause**: Model doesn't inherit from EditModelBase  
**Fix**: Ensure view models inherit from EditModelBase:
```csharp
public class CreateOrderModel : EditModelBase
{
    // CustomDataBinderAttribute auto-fills DataRecorderMetaData
}
```

### Custom route not matching
**Symptom**: 404 error even though controller/action exists  
**Cause**: Route priority—attribute routes override area routes  
**Fix**: Check route order in Area*AreaRegistration.cs—custom routes should be registered before convention routes

### Session lost between requests
**Symptom**: Token valid but ISessionContext.UserSession is null  
**Cause**: AppContextStore cleared or session not rehydrated  
**Fix**: Verify Application_BeginRequest calls `SessionHelper.SetSessionModel(token)` before routing

<!-- END CUSTOM SECTION -->
