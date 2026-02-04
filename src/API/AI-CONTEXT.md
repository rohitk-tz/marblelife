<!-- AUTO-GENERATED: Header -->
# API Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:25:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module is the **HTTP Interface** implementation of the application. It exposes RESTful endpoints consumed by the frontend (Mobile/Web). It is responsible for:
1.  **Routing**: Mapping URLs to Controllers.
2.  **Request Handling**: Authentication, Session Setup, and CORS.
3.  **Transport Logic**: Deserializing JSON inputs and serializing responses.
4.  **Orchestration**: Delegating business logic to **Core** services.

### Design Patterns
-   **MVC / Web API**: Uses ASP.NET Web API 2 Controllers.
-   **Areas**: Code partitioning by functional domain (e.g., `Areas/Users`, `Areas/Sales`).
-   **Middleware (Pipeline)**: Uses `Global.asax` events (`BeginRequest`, `EndRequest`) for request lifecycle management.
-   **Token Auth**: Usage of custom token header for session restoration.

### Request Flow
1.  **Global.asax: BeginRequest**:
    -   Adds CORS headers.
    -   Reads `token` header.
    -   Calls `SetupSessionContext` to populate `ISessionContext` for the request scope.
        -   *Crucial*: This makes `ApplicationManager.Session` available in Services.
2.  **Controller Action**:
    -   Receives Request.
    -   Validates Model (FluentValidation).
    -   Calls `IService.Method()`.
3.  **Global.asax: EndRequest**:
    -   Clears `IAppContextStore` (disposes DbContext).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Controller Structure (Typical)
Located in `Areas/{AreaName}/Controllers`.

```csharp
[RoutePrefix("api/{AreaName}/{Controller}")]
public class MyController : ApiController 
{
    private readonly IMyService _service;
    
    // Constructor Injection via Unity
    public MyController(IMyService service) { _service = service; }
    
    [HttpPost]
    [Route("action")]
    public IHttpActionResult Action(ViewModel model) { ... }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### Global Configuration (`Global.asax.cs`)
-   **`Application_Start`**:
    -   `AreaRegistration.RegisterAllAreas()`
    -   `ApiDependencyRegistrar.RegisterDepndencies()`
    -   `ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12` (Force TLS 1.2)
-   **`Application_BeginRequest`**:
    -   Manually handles OPTIONS requests for CORS (Old school approach).
    -   Sets `Access-Control-Allow-Origin: *`.

### Authentication
-   **Mechanism**: Custom Token Header.
-   **Header Key**: Defined in `SessionHelper.TokenKeyName`.
-   **Behavior**: `SessionHelper.SetSessionModel(token)` restores the user session from the DB/Cache.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core](../Core/AI-CONTEXT.md)** - Logic and Models.
-   **[DependencyInjection](../DependencyInjection/AI-CONTEXT.md)** - Container setup.

### External
-   **ASP.NET Web API 2** (`System.Web.Http`)
-   **FluentValidation** (`FluentValidation.WebApi`)

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### CORS Handling
The application manually handles CORS in `Application_BeginRequest` instead of using the standard `Microsoft.AspNet.WebApi.Cors` package.
-   **Implication**: If adding new headers/methods, you must update the hardcoded strings in `Global.asax.cs`.

### Session Context
The `SetupSessionContext` method is critical. If `ApplicationManager.Session` is null in a Service, it's likely because:
1.  The `token` header was missing.
2.  The token was invalid.
3.  The logic is running in a background thread (not an HTTP request) without using `SetupCurrentContextWinJob`.
<!-- END CUSTOM SECTION -->
