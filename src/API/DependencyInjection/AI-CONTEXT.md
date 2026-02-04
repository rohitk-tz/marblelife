# API/DependencyInjection - AI Context

## Purpose

The **DependencyInjection** folder contains the Inversion of Control (IoC) container configuration for the API layer. It registers API-specific services and delegates to lower layers (Core, Infrastructure) for their service registrations, creating a complete dependency graph for the application.

## Key File: ApiDependencyRegistrar.cs

This is the **single entry point** for all dependency registration in the API project. It's called from `Global.asax.cs` during `Application_Start`.

### Implementation

```csharp
public class ApiDependencyRegistrar
{
    public static void RegisterDepndencies()
    {
        // 1. Register all Core and Infrastructure dependencies
        DependencyRegistrar.RegisterDependencies();

        // 2. Register API-specific services
        ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
        ApplicationManager.DependencyInjection.Register<ISessionContext, SessionContext>();
        ApplicationManager.DependencyInjection.Register<IDropDownHelperService, DropDownHelperService>();

        // 3. Setup context for request-scoped storage
        DependencyRegistrar.SetupCurrentContext(new AppContextStore());
    }
}
```

## Architecture Pattern

The MarbleLife application uses a **layered dependency registration** approach:

```
API Layer (ApiDependencyRegistrar)
    ↓ calls
Infrastructure Layer (DependencyRegistrar)
    ↓ registers
    - Repository implementations
    - External service integrations
    - Database context
    ↓ calls
Core Layer
    ↓ registers
    - Business services (I*Service implementations)
    - Domain factories (I*Factory implementations)
    - Utility services
```

## Registered Services

### IAppContextStore
**Purpose**: Thread-safe storage for request-scoped data.

**Implementation**: `AppContextStore.cs` in `API/Impl/`

**Usage**: Stores per-request data that needs to be accessible across different layers during request processing.

**Lifecycle**: Instance per request (cleared in `Application_EndRequest`)

### ISessionContext
**Purpose**: Provides access to current user session information.

**Implementation**: `SessionContext.cs` in `API/Impl/`

**Properties**:
- `UserSession`: Current authenticated user details
  - `UserId`: Unique user identifier
  - `RoleId`: User's role for authorization
  - `OrganizationId`: Franchisee/organization the user belongs to
  - `Email`: User's email address
  - `FullName`: User's display name

**Usage in Controllers**:
```csharp
public class FranchiseeController : BaseController
{
    private readonly ISessionContext _sessionContext;

    public FranchiseeController(ISessionContext sessionContext)
    {
        _sessionContext = sessionContext;
    }

    [HttpPost]
    public bool Post([FromBody] FranchiseeEditModel model)
    {
        // Access current user information
        model.CreatedBy = _sessionContext.UserSession.UserId;
        model.CreatedByRole = _sessionContext.UserSession.RoleId;
        
        // Authorization check
        if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
        {
            model.OrganizationId = _sessionContext.UserSession.OrganizationId;
        }
        
        _franchiseeService.Save(model);
        return true;
    }
}
```

**Lifecycle**: Instance per request (populated in `Application_BeginRequest`)

### IDropDownHelperService
**Purpose**: Provides lookup data for dropdowns and selection lists.

**Implementation**: `DropDownHelperService.cs` in `API/Areas/Application/Impl/`

**Common Methods**:
- `GetFranchisees()`: List of franchisees for selection
- `GetServiceTypes()`: Available service types
- `GetStatuses()`: Status enumerations
- `GetRoles()`: User roles
- `GetStates()`: US states/provinces

**Usage**:
```csharp
[HttpGet]
public DropDownResponse GetFranchiseeDropdown()
{
    return _dropDownHelper.GetFranchisees(_sessionContext.UserSession.RoleId);
}
```

**Lifecycle**: Singleton (shared across all requests)

## Dependency Resolution

### Controller Resolution

The API uses a custom `DependencyResolver` (in `App_Start/WebApiConfig.cs`) that integrates with Web API:

```csharp
public class DependencyResolver : IDependencyResolver
{
    public object GetService(Type serviceType)
    {
        // Only resolve controllers from DI
        return serviceType.BaseType != typeof(BaseController) && 
               serviceType.BaseType != typeof(ApiController) 
            ? null 
            : ApplicationManager.DependencyInjection.Resolve(serviceType);
    }
}
```

### Service Resolution in Code

Resolve services anywhere in the application:
```csharp
var service = ApplicationManager.DependencyInjection.Resolve<IMyService>();
```

**Best Practice**: Prefer constructor injection over manual resolution:
```csharp
// Good: Constructor injection
public class MyController : BaseController
{
    private readonly IMyService _service;
    
    public MyController(IMyService service)
    {
        _service = service;
    }
}

// Avoid: Manual resolution (use only when necessary)
public class MyController : BaseController
{
    public void MyAction()
    {
        var service = ApplicationManager.DependencyInjection.Resolve<IMyService>();
    }
}
```

## Registration Lifecycle

### Application Startup Sequence

1. **IIS/Application Pool starts**
2. **Global.asax.cs: Application_Start()**
   ```csharp
   ApiDependencyRegistrar.RegisterDepndencies();
   ```
3. **ApiDependencyRegistrar.RegisterDepndencies()**
   - Calls `DependencyRegistrar.RegisterDependencies()` (Infrastructure/Core)
   - Registers API-specific services
   - Sets up context storage
4. **Container Ready**: All services registered and available
5. **First Request**: Services resolved via dependency injection

### Request Lifecycle

1. **Application_BeginRequest**: Session context populated from token
2. **Controller Creation**: Dependencies injected via constructor
3. **Action Execution**: Services used in business logic
4. **Application_EndRequest**: Context storage cleared

## Service Lifetimes

### Singleton
Services that are created once and shared across all requests:
- Stateless services
- Cache services
- Configuration services
- Examples: `ILogService`, `ISettings`, `IDropDownHelperService`

### Per-Request (Scoped)
Services that are created once per HTTP request:
- Session-related services
- Context services
- Unit of work
- Examples: `ISessionContext`, `IAppContextStore`, `IUnitOfWork`

### Transient
Services that are created every time they're requested:
- Factories
- Short-lived operations
- Stateful services
- Examples: `I*Factory` implementations

## For AI Agents

### Adding New API Service

1. **Create service interface** (if doesn't exist in Core):
```csharp
// In API/Areas/MyArea/
public interface IMyCustomService
{
    void DoSomething();
}
```

2. **Create implementation**:
```csharp
// In API/Areas/MyArea/Impl/
public class MyCustomService : IMyCustomService
{
    private readonly IDependency _dependency;
    
    public MyCustomService(IDependency dependency)
    {
        _dependency = dependency;
    }
    
    public void DoSomething() { /* implementation */ }
}
```

3. **Register in ApiDependencyRegistrar**:
```csharp
public static void RegisterDepndencies()
{
    DependencyRegistrar.RegisterDependencies();
    
    ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
    ApplicationManager.DependencyInjection.Register<ISessionContext, SessionContext>();
    ApplicationManager.DependencyInjection.Register<IDropDownHelperService, DropDownHelperService>();
    
    // Add your service
    ApplicationManager.DependencyInjection.Register<IMyCustomService, MyCustomService>();
    
    DependencyRegistrar.SetupCurrentContext(new AppContextStore());
}
```

4. **Use in controllers via constructor injection**:
```csharp
public class MyController : BaseController
{
    private readonly IMyCustomService _myService;
    
    public MyController(IMyCustomService myService)
    {
        _myService = myService;
    }
}
```

### Modifying Session Context

To add new properties to session:

1. **Update Core interface** `ISessionContext`:
```csharp
public interface ISessionContext
{
    UserSession UserSession { get; set; }
    string NewProperty { get; set; } // Add here
}
```

2. **Update implementation** `API/Impl/SessionContext.cs`:
```csharp
public class SessionContext : ISessionContext
{
    public UserSession UserSession { get; set; }
    public string NewProperty { get; set; } // Add here
}
```

3. **Populate in** `SessionHelper.SetSessionModel`:
```csharp
var sessionContext = ApplicationManager.DependencyInjection.Resolve<ISessionContext>();
sessionContext.UserSession = LoadUserSession(token);
sessionContext.NewProperty = "value"; // Set here
```

## For Human Developers

### Best Practices

#### Registration
- Register API-specific services only (business logic in Core)
- Use interfaces for all registrations (enables mocking/testing)
- Register in logical order (dependencies first)
- Document service lifetimes in comments

#### Service Design
- Keep services stateless when possible
- Inject dependencies via constructor
- Avoid service locator pattern (manual `Resolve` calls)
- Use appropriate lifetime (singleton vs. scoped vs. transient)

#### Testing
- Register mock implementations for testing
- Use separate test registrar for unit tests
- Test service resolution at application startup
- Verify all dependencies can be resolved

### Common Issues

#### Circular Dependencies
**Symptom**: Stack overflow or "circular dependency" exception during startup

**Solution**:
- Refactor services to remove circular references
- Use lazy initialization
- Extract interface and inject that instead

#### Missing Registration
**Symptom**: `NullReferenceException` when accessing service

**Solution**:
- Verify service is registered in `ApiDependencyRegistrar`
- Check registration is in correct layer (API vs. Core vs. Infrastructure)
- Ensure registration happens before first request

#### Wrong Lifetime
**Symptom**: Stale data, unexpected state, or memory leaks

**Solution**:
- Use per-request lifetime for stateful services
- Use singleton for truly stateless services
- Verify context clearing in `Application_EndRequest`

### Debugging Tips

**Verify Registration**:
```csharp
// In Application_Start (temporary debug code)
var service = ApplicationManager.DependencyInjection.Resolve<IMyService>();
if (service == null)
{
    throw new Exception("IMyService not registered!");
}
```

**Check Dependency Chain**:
```csharp
// Trace dependency resolution
var logger = ApplicationManager.DependencyInjection.Resolve<ILogService>();
logger.Info($"Resolved: {typeof(IMyService).Name}");
```

**Test Container Configuration**:
```csharp
[Test]
public void AllControllersCanBeResolved()
{
    ApiDependencyRegistrar.RegisterDepndencies();
    
    var controllerTypes = typeof(BaseController).Assembly
        .GetTypes()
        .Where(t => t.IsSubclassOf(typeof(BaseController)));
    
    foreach (var type in controllerTypes)
    {
        var instance = ApplicationManager.DependencyInjection.Resolve(type);
        Assert.IsNotNull(instance, $"{type.Name} could not be resolved");
    }
}
```

## Container Implementation

The application uses **Autofac** (or similar) as the IoC container. The container is abstracted behind `ApplicationManager.DependencyInjection` to allow switching containers if needed.

### Container Interface
```csharp
public interface IDependencyInjection
{
    void Register<TInterface, TImplementation>() where TImplementation : TInterface;
    void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface;
    T Resolve<T>();
    object Resolve(Type type);
}
```

## Related Files

- **Global.asax.cs**: Calls `ApiDependencyRegistrar.RegisterDepndencies()`
- **Infrastructure/DependencyInjection/DependencyRegistrar.cs**: Registers Core/Infrastructure services
- **API/Impl/SessionContext.cs**: Session context implementation
- **API/Impl/AppContextStore.cs**: Request-scoped storage implementation
- **App_Start/WebApiConfig.cs**: Custom `DependencyResolver` for Web API integration

## Migration Notes

When upgrading to ASP.NET Core:
- Use built-in DI container (`IServiceCollection`)
- Register services in `Startup.cs` or `Program.cs`
- Use `AddScoped`, `AddSingleton`, `AddTransient` methods
- Constructor injection remains the same
- No need for custom `DependencyResolver`
