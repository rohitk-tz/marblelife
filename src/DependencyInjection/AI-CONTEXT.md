# DependencyInjection Module - AI Context

## Purpose

The **DependencyInjection** module provides the Inversion of Control (IoC) container infrastructure for the MarbleLife application. It uses **Microsoft Unity Container 4.0.1** to manage service lifetimes, resolve dependencies, and enable constructor injection across all application layers (API, Jobs, Core, Infrastructure). The module supports both attribute-based auto-registration and explicit registration patterns with multiple lifetime management strategies.

## Architecture Overview

The DI module implements the **Dependency Injection** and **Service Locator** patterns, providing a centralized mechanism for registering and resolving application dependencies. It supports auto-discovery of implementations via attributes, custom lifetime managers for different execution contexts (web requests, background jobs, threads), and integration with validation frameworks.

### Key Architectural Patterns

1. **Dependency Injection**: Constructor injection for all service dependencies
2. **Service Locator**: Static `IoC` class for resolving dependencies
3. **Factory Pattern**: Validation factory for FluentValidation integration
4. **Lifetime Management**: Multiple strategies (Singleton, PerRequest, PerThread, Transient)
5. **Convention-Based Registration**: Auto-discovery via `[DefaultImplementation]` attribute
6. **Module-Based Registration**: Domain-specific registration methods

## Project Structure

```
DependencyInjection/
├── IoC.cs                          # Unity container wrapper (service locator)
├── DependencyInjection.cs          # IDependencyInjection implementation
├── DependencyRegistrar.cs          # Core module registration
├── ApiDependencyRegistrar.cs       # API-specific registration
├── DefaultImplementationAttribute.cs # Auto-discovery marker attribute
├── AppContextLifetimeManager.cs    # Web API request-scoped lifetime
├── WinJobContextLifetimeManager.cs # Windows Service job-scoped lifetime
├── IAppContextStore.cs             # Context storage interface
├── AppContextStore.cs              # Thread-safe context storage
├── Properties/                     # Assembly metadata
├── packages.config                 # NuGet dependencies
└── DependencyInjection.csproj      # Project configuration
```

## IoC - Unity Container Wrapper

### Purpose

Static service locator providing type-safe access to the Unity container for dependency resolution.

### Core Functionality

```csharp
public static class IoC
{
    // Single static Unity container instance
    private static readonly IUnityContainer Container = new UnityContainer();
    
    // Generic registration
    public static void Register<TBase, TConcrete>() where TConcrete : TBase
    {
        Container.RegisterType<TBase, TConcrete>();
    }
    
    // Registration with lifetime manager
    public static void Register<TBase, TConcrete>(LifetimeManager lifetimeManager) 
        where TConcrete : TBase
    {
        Container.RegisterType<TBase, TConcrete>(lifetimeManager);
    }
    
    // Named registration (multiple implementations)
    public static void Register<TBase>(string name, Type concrete)
    {
        Container.RegisterType(typeof(TBase), concrete, name);
    }
    
    // Instance registration (singleton pattern)
    public static void RegisterInstance<T>(T instance)
    {
        Container.RegisterInstance(typeof(T), instance);
    }
    
    // Generic resolution
    public static T Resolve<T>()
    {
        return Container.Resolve<T>();
    }
    
    // Named resolution
    public static T Resolve<T>(string name)
    {
        return Container.Resolve<T>(name);
    }
    
    // Type-based resolution (for dynamic scenarios)
    public static object Resolve(Type type)
    {
        return Container.Resolve(type);
    }
}
```

### Usage Examples

```csharp
// Register service
IoC.Register<ICustomerService, CustomerService>();

// Register with custom lifetime
IoC.Register<ISessionContext, SessionContext>(new AppContextLifetimeManager());

// Register singleton instance
IoC.RegisterInstance<ISettings>(new Settings());

// Resolve service
var customerService = IoC.Resolve<ICustomerService>();

// Resolve named implementation
var specialLogger = IoC.Resolve<ILogService>("FileLogger");
```

## DependencyInjection - IDependencyInjection Implementation

### Purpose

Provides instance-based dependency resolution (alternative to static IoC).

```csharp
public interface IDependencyInjection
{
    T Resolve<T>();
    T Resolve<T>(string name);
    object Resolve(Type type);
}

public class DependencyInjection : IDependencyInjection
{
    public T Resolve<T>()
    {
        return IoC.Resolve<T>();
    }
    
    public T Resolve<T>(string name)
    {
        return IoC.Resolve<T>(name);
    }
    
    public object Resolve(Type type)
    {
        return IoC.Resolve(type);
    }
}
```

**Use Case**: Injecting DI container itself for dynamic resolution scenarios (factory patterns).

```csharp
public class JobExecutor
{
    private readonly IDependencyInjection _di;
    
    public JobExecutor(IDependencyInjection di)
    {
        _di = di;
    }
    
    public void ExecuteJob(string jobTypeName)
    {
        var jobType = Type.GetType(jobTypeName);
        var job = _di.Resolve(jobType) as IJob;
        job?.Execute();
    }
}
```

## DefaultImplementationAttribute

### Purpose

Marks classes for automatic discovery and registration during initialization.

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class DefaultImplementationAttribute : Attribute
{
    public Type InterfaceType { get; set; }
    public LifetimeManagerType Lifetime { get; set; } = LifetimeManagerType.Singleton;
}
```

### Auto-Discovery Mechanism

```csharp
public static void RegisterDefaultImplementations()
{
    // Scan all loaded assemblies
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    
    foreach (var assembly in assemblies)
    {
        var typesWithAttribute = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<DefaultImplementationAttribute>() != null);
        
        foreach (var type in typesWithAttribute)
        {
            var attribute = type.GetCustomAttribute<DefaultImplementationAttribute>();
            
            // Auto-discover interface (e.g., "ICustomerService" for "CustomerService")
            var interfaceType = attribute.InterfaceType 
                ?? type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
            
            if (interfaceType != null)
            {
                IoC.Register(interfaceType, type, GetLifetimeManager(attribute.Lifetime));
            }
        }
    }
}
```

### Usage Example

```csharp
// Automatically registers ICustomerService -> CustomerService
[DefaultImplementation]
public class CustomerService : ICustomerService
{
    // Implementation
}

// Explicitly specify interface
[DefaultImplementation(InterfaceType = typeof(ISpecialService))]
public class CustomNamedService : ISpecialService
{
    // Implementation
}

// Specify lifetime
[DefaultImplementation(Lifetime = LifetimeManagerType.PerRequest)]
public class RequestScopedService : IRequestScopedService
{
    // Implementation
}
```

## DependencyRegistrar - Core Module Registration

### Purpose

Registers core application services, utilities, and infrastructure components.

### Registration Methods

#### RegisterDependencies()

**Purpose**: One-time registration of core services at application startup.

```csharp
public static void RegisterDependencies()
{
    // Auto-discover implementations with [DefaultImplementation]
    RegisterDefaultImplementations();
    
    // Register validation factory (FluentValidation integration)
    IoC.RegisterInstance<IValidatorFactory>(new ValidatorFactory());
    
    // Register utilities
    IoC.Register<ITransactionHelper, TransactionHelper>();
    IoC.Register<ISaveCascadeHelper, SaveCascadeHelper>();
    
    // Register singleton instances
    IoC.RegisterInstance<Random>(new Random());
    
    // Register factories
    IoC.Register<ICustomerFactory, CustomerFactory>();
    IoC.Register<IInvoiceFactory, InvoiceFactory>();
    
    // Register domain services
    IoC.Register<ICustomerService, CustomerService>();
    IoC.Register<IInvoiceService, InvoiceService>();
}
```

#### SetupCurrentContext(IAppContextStore contextStore)

**Purpose**: Configures Web API request-scoped dependencies.

```csharp
public static void SetupCurrentContext(IAppContextStore contextStore)
{
    // Register context store for request scoping
    IoC.RegisterInstance<IAppContextStore>(contextStore);
    
    // Register request-scoped services with AppContextLifetimeManager
    IoC.Register<ISessionContext, SessionContext>(new AppContextLifetimeManager());
    IoC.Register<IUnitOfWork, UnitOfWork>(new AppContextLifetimeManager());
    
    // Services that need current user context
    IoC.Register<IDropDownHelperService, DropDownHelperService>(new AppContextLifetimeManager());
}
```

**Request Lifecycle**:
1. HTTP request begins
2. `AppContextStore` initialized with request-scoped GUID
3. Services resolved with `AppContextLifetimeManager` share instance per request
4. Request ends, context cleared, services disposed

#### SetupCurrentContextWinJob()

**Purpose**: Configures Windows Service/Job-scoped dependencies.

```csharp
public static void SetupCurrentContextWinJob()
{
    // Register job-scoped context
    IoC.Register<ISessionContext, WinJobSessionContext>(new PerThreadLifetimeManager());
    IoC.Register<IUnitOfWork, UnitOfWork>(new PerThreadLifetimeManager());
    
    // Each job execution thread gets own instances
    IoC.Register<IPollingAgentService, PollingAgentService>(new PerThreadLifetimeManager());
}
```

**Job Lifecycle**:
1. Job execution starts on scheduler thread
2. `PerThreadLifetimeManager` ensures thread-local instances
3. Job completes, thread-local instances disposed
4. Next job execution gets fresh instances

#### SetLocalizationBasedRegistration(string franchiseeTimeZone)

**Purpose**: Dynamically registers timezone-aware clock service.

```csharp
public static void SetLocalizationBasedRegistration(string franchiseeTimeZone)
{
    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(franchiseeTimeZone);
    var clock = new Clock(timeZone);
    
    IoC.RegisterInstance<IClock>(clock);
}
```

**Use Case**: Multi-tenant applications where each franchisee operates in different timezone.

## ApiDependencyRegistrar - API-Specific Registration

### Purpose

Extends core registration with API-specific services (session management, context storage).

```csharp
public static class ApiDependencyRegistrar
{
    public static void RegisterDependencies()
    {
        // Register core dependencies first
        DependencyRegistrar.RegisterDependencies();
        
        // Register API-specific context store
        var contextStore = new AppContextStore();
        IoC.RegisterInstance<IAppContextStore>(contextStore);
        
        // Register request-scoped services
        DependencyRegistrar.SetupCurrentContext(contextStore);
        
        // API-specific services
        IoC.Register<ISessionHelper, SessionHelper>(new AppContextLifetimeManager());
        IoC.Register<IResponseHelper, ResponseHelper>(new AppContextLifetimeManager());
        
        // Register API utilities
        IoC.Register<IDropDownHelperService, DropDownHelperService>();
    }
}
```

**Integration**: Called from `Global.asax.cs` in API project during `Application_Start`.

## Lifetime Managers

### AppContextLifetimeManager - Request-Scoped

**Purpose**: Ensures one instance per HTTP request in Web API.

```csharp
public class AppContextLifetimeManager : LifetimeManager
{
    private readonly Guid _key = Guid.NewGuid();
    
    public override object GetValue()
    {
        var contextStore = IoC.Resolve<IAppContextStore>();
        return contextStore.GetValue(_key);
    }
    
    public override void SetValue(object newValue)
    {
        var contextStore = IoC.Resolve<IAppContextStore>();
        contextStore.SetValue(_key, newValue);
    }
    
    public override void RemoveValue()
    {
        var contextStore = IoC.Resolve<IAppContextStore>();
        contextStore.RemoveValue(_key);
    }
}
```

**Storage**: Uses `IAppContextStore` (thread-safe dictionary) to store instances per request context.

**Cleanup**: `Global.asax.cs` calls `contextStore.Clear()` in `EndRequest` event.

### WinJobContextLifetimeManager - Job-Scoped

**Purpose**: Ensures immutable job instances for Windows Service execution.

```csharp
public class WinJobContextLifetimeManager : LifetimeManager
{
    private readonly object _value;
    
    public WinJobContextLifetimeManager(object value)
    {
        _value = value;
    }
    
    public override object GetValue()
    {
        return _value;
    }
    
    public override void SetValue(object newValue)
    {
        // Immutable - ignore set
    }
    
    public override void RemoveValue()
    {
        // Immutable - ignore remove
    }
}
```

**Use Case**: Pre-created job instances that should never be recreated or shared.

### Built-In Unity Lifetime Managers

**ContainerControlledLifetimeManager (Singleton)**:
- Single instance for application lifetime
- Default for most services

**HierarchicalLifetimeManager**:
- Singleton per container hierarchy
- Used for child containers

**PerThreadLifetimeManager**:
- One instance per thread
- Used for background job services

**TransientLifetimeManager**:
- New instance every resolution
- Used for stateless, lightweight services

## AppContextStore - Request Context Storage

### Purpose

Thread-safe storage for request-scoped service instances.

```csharp
public interface IAppContextStore
{
    object GetValue(Guid key);
    void SetValue(Guid key, object value);
    void RemoveValue(Guid key);
    void Clear();
}

public class AppContextStore : IAppContextStore
{
    private readonly ConcurrentDictionary<Guid, object> _store = new ConcurrentDictionary<Guid, object>();
    
    public object GetValue(Guid key)
    {
        return _store.TryGetValue(key, out var value) ? value : null;
    }
    
    public void SetValue(Guid key, object value)
    {
        _store[key] = value;
    }
    
    public void RemoveValue(Guid key)
    {
        _store.TryRemove(key, out _);
    }
    
    public void Clear()
    {
        _store.Clear();
    }
}
```

**Usage in Global.asax.cs**:
```csharp
protected void Application_BeginRequest(object sender, EventArgs e)
{
    // Context store automatically used by AppContextLifetimeManager
}

protected void Application_EndRequest(object sender, EventArgs e)
{
    var contextStore = IoC.Resolve<IAppContextStore>();
    contextStore.Clear(); // Prevent memory leaks
}
```

## Validation Factory Integration

### Purpose

Integrates FluentValidation with DI container for automatic validator resolution.

```csharp
public class ValidatorFactory : IValidatorFactory
{
    public IValidator<T> GetValidator<T>()
    {
        return IoC.Resolve<IValidator<T>>();
    }
    
    public IValidator GetValidator(Type type)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(type);
        return IoC.Resolve(validatorType) as IValidator;
    }
}
```

**Usage**:
```csharp
// Register validator
[DefaultImplementation]
public class CustomerValidator : AbstractValidator<Customer>, IValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Email).EmailAddress();
    }
}

// Resolve and use
var validator = validatorFactory.GetValidator<Customer>();
var result = validator.Validate(customer);
```

## Integration Across Modules

### API Module Integration

```csharp
// Global.asax.cs
protected void Application_Start()
{
    // Register all dependencies
    ApiDependencyRegistrar.RegisterDependencies();
    
    // Set up Web API dependency resolver
    GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(IoC.Container);
}
```

### Jobs Module Integration

```csharp
// ApplicationManager.cs in Jobs project
public static class ApplicationManager
{
    public static IDependencyInjection DependencyInjection { get; private set; }
    
    public static void Init()
    {
        DependencyRegistrar.RegisterDependencies();
        DependencyRegistrar.SetupCurrentContextWinJob();
        DependencyInjection = new DependencyInjection();
    }
}

// Scheduler.cs
protected override void OnStart(string[] args)
{
    ApplicationManager.Init();
    // Jobs now use DI for all dependencies
}
```

### Console Application Integration

```csharp
// Program.cs
static void Main(string[] args)
{
    DependencyRegistrar.RegisterDependencies();
    DependencyRegistrar.SetupCurrentContextWinJob(); // Or SetupCurrentContext for web-like
    
    var service = IoC.Resolve<IMyService>();
    service.DoWork();
}
```

## For AI Agents

### Adding New Service Registration

1. **Define Interface and Implementation**:
```csharp
public interface INewService
{
    void DoWork();
}

[DefaultImplementation] // Auto-discovery
public class NewService : INewService
{
    private readonly IDependency _dependency;
    
    public NewService(IDependency dependency)
    {
        _dependency = dependency;
    }
    
    public void DoWork()
    {
        _dependency.Execute();
    }
}
```

2. **Explicit Registration** (if not using attribute):
```csharp
// In DependencyRegistrar.RegisterDependencies()
IoC.Register<INewService, NewService>();

// Or with custom lifetime
IoC.Register<INewService, NewService>(new AppContextLifetimeManager());
```

3. **Use in Dependent Classes**:
```csharp
public class MyController : ApiController
{
    private readonly INewService _newService;
    
    public MyController(INewService newService)
    {
        _newService = newService;
    }
    
    public IHttpActionResult Get()
    {
        _newService.DoWork();
        return Ok();
    }
}
```

### Choosing Lifetime Manager

**Singleton (Default)**:
- Stateless services
- Configuration services
- Thread-safe shared resources

**AppContextLifetimeManager (Request-Scoped)**:
- Session context
- UnitOfWork/database context
- Services that need current user information

**PerThreadLifetimeManager (Thread-Scoped)**:
- Background job services
- Thread-local resources

**TransientLifetimeManager (New Instance)**:
- Factories
- Lightweight stateless operations
- Services with mutable state

### Best Practices

- **Prefer constructor injection** over service locator pattern
- **Mark implementations** with `[DefaultImplementation]` for consistency
- **Use interfaces** for all service contracts
- **Avoid circular dependencies** (A → B → A)
- **Dispose resources** properly (implement IDisposable for services managing resources)
- **Test with mocks** by registering test doubles in DI container

## For Human Developers

### Debugging DI Issues

#### Service Not Registered
```csharp
// Error: "Resolution of the dependency failed"
// Solution: Add registration
IoC.Register<IMissingService, MissingServiceImpl>();
```

#### Circular Dependency
```csharp
// Error: Stack overflow during resolution
// A depends on B, B depends on A
// Solution: Refactor to break cycle (extract interface, use mediator)
```

#### Wrong Lifetime
```csharp
// Problem: Singleton service holds reference to request-scoped service
// Solution: Inject IUnitOfWork factory instead of instance
public class MySingletonService
{
    private readonly Func<IUnitOfWork> _unitOfWorkFactory;
    
    public MySingletonService(Func<IUnitOfWork> unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }
    
    public void DoWork()
    {
        using (var unitOfWork = _unitOfWorkFactory())
        {
            // Use unitOfWork
        }
    }
}
```

### Testing with DI

#### Unit Tests
```csharp
[TestFixture]
public class CustomerServiceTests
{
    [SetUp]
    public void SetUp()
    {
        // Register test doubles
        IoC.Register<IRepository<Customer>, MockCustomerRepository>();
        IoC.Register<ILogService, NullLogService>();
    }
    
    [Test]
    public void TestGetCustomer()
    {
        var service = IoC.Resolve<ICustomerService>();
        var customer = service.Get(123);
        Assert.IsNotNull(customer);
    }
}
```

#### Integration Tests
```csharp
[TestFixture]
public class ApiIntegrationTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Use real registrations
        ApiDependencyRegistrar.RegisterDependencies();
    }
    
    [Test]
    public void TestEndToEnd()
    {
        var controller = IoC.Resolve<CustomerController>();
        var result = controller.Get(123);
        Assert.IsNotNull(result);
    }
}
```

### Performance Considerations

- **Singleton is fastest**: No lookup overhead after first resolution
- **Request-scoped adds overhead**: Dictionary lookup per resolution
- **Avoid resolving in tight loops**: Cache resolved services
- **Dispose properly**: AppContextStore.Clear() in EndRequest prevents leaks

## Dependencies

- **Unity Container 4.0.1** - IoC container implementation
- **FluentValidation 6.2.1** - Validation framework integration
- **CommonServiceLocator** - Service locator abstraction (Unity adapter)

## Troubleshooting

### Common Issues

**Service Resolution Fails**:
- Verify service is registered: Check `RegisterDependencies()` methods
- Check for typos in interface/class names
- Ensure attribute-based discovery is working: `RegisterDefaultImplementations()`

**Memory Leaks**:
- Ensure `AppContextStore.Clear()` called in `EndRequest`
- Dispose services implementing `IDisposable`
- Avoid capturing request-scoped services in singletons

**Thread Safety Issues**:
- Use `PerThreadLifetimeManager` for background jobs
- Avoid shared mutable state in singleton services
- Use `ConcurrentDictionary` for thread-safe collections

**Slow Application Startup**:
- Auto-discovery scans all assemblies - optimize by filtering namespaces
- Cache resolved services in static fields where appropriate
- Use lazy initialization for expensive services
