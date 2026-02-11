# DependencyInjection Module - Technical Context

## Module Identity
- **Name**: DependencyInjection
- **Type**: Infrastructure / IoC Container
- **Language**: C# 4.5.2
- **Framework**: .NET Framework 4.5.2

## Architecture Overview

### Core Responsibilities
This module provides centralized dependency injection and Inversion of Control (IoC) container management for the MarbleLife application using Microsoft Unity Container. It handles service registration, resolution, lifetime management, and auto-discovery of implementations.

### Design Patterns
- **Service Locator Pattern**: `IoC` class acts as a static service locator facade
- **Factory Pattern**: `ValidatorFactory` creates validator instances dynamically
- **Dependency Injection**: Constructor and property injection via Unity
- **Convention-based Registration**: Auto-registration via `[DefaultImplementation]` attribute
- **Lifetime Management**: Custom `AppContextLifetimeManager` and `WinJobContextLifetimeManager`

### Module Structure
```
DependencyInjection/
├── IoC.cs                          # Internal Unity container facade
├── DependencyRegistrar.cs          # Central registration orchestrator
├── DependencyInjectionHelper.cs    # Public helper interface implementation
├── AppContextLifeTimeManager.cs    # Custom lifetime managers
├── ValidatorFactory.cs             # FluentValidation factory
└── .context/
    ├── CONTEXT.md                  # This file
    ├── OVERVIEW.md                 # Human-readable guide
    └── metadata.json               # Module metadata
```

## Type System

### Public Types

#### `DependencyRegistrar` (Public)
**Purpose**: Centralized service registration orchestrator

**Key Methods**:
```csharp
public static void RegisterDependencies()
```
- Master registration method called at application startup
- Orchestrates all dependency registration including auto-discovery
- Configures `ApplicationManager` with resolved services

```csharp
public static void SetupCurrentContext(IAppContextStore appContextStore)
```
- Configures per-request lifetime management for web applications
- Registers `IUnitOfWork` and `IClock` with `AppContextLifetimeManager`

```csharp
public static void SetupCurrentContextWinJob()
```
- Configures per-thread lifetime management for Windows services/jobs
- Uses `PerThreadLifetimeManager` for thread-isolated instances

```csharp
public static void SetLocalizationBasedRegistration(string systemTimeZoneId)
```
- Registers timezone-aware `IClock` implementation
- Supports localization requirements

```csharp
private static void RegisterDefaultImplementations()
```
- Auto-discovers classes marked with `[DefaultImplementation]` attribute
- Automatically maps implementations to interfaces by convention (Interface = I + ClassName)
- Scans all referenced assemblies for annotated types

#### `DependencyInjectionHelper` (Public)
**Purpose**: Public facade implementing `IDependencyInjectionHelper` interface

**Methods**:
```csharp
T Resolve<T>()                    // Resolve by type
object Resolve(Type obj)          // Resolve by runtime type
T Resolve<T>(string name)         // Resolve named registration
void Register<TAbstract, TConcrete>() // Register type mapping
```

#### `AppContextLifetimeManager` (Public)
**Purpose**: Custom Unity lifetime manager for per-request scope

**Behavior**:
- Stores instances in `IAppContextStore` (typically HttpContext or custom store)
- Each registration gets unique GUID key
- Supports web request-scoped dependencies

**Constructors**:
```csharp
AppContextLifetimeManager(IAppContextStore appContextStore)
AppContextLifetimeManager(IAppContextStore appContextStore, string key)
```

#### `WinJobContextLifetimeManager` (Public)
**Purpose**: Custom lifetime manager for Windows services/jobs

**Behavior**:
- Holds pre-created instance with specific key
- `GetValue()` returns fixed instance
- `SetValue()` and `RemoveValue()` throw `NotImplementedException` (read-only)

#### `ValidatorFactory` (Public)
**Purpose**: Custom FluentValidation factory integrated with IoC

**Behavior**:
- Resolves validators from IoC container
- Skips validation for primitives, value types, strings, and types marked with:
  - `[NoValidationResolveAtStartAttribute]`
  - `[NoValidatorRequiredAttribute]`
- Returns `null` for excluded types (no validation)

### Internal Types

#### `IoC` (Internal Static)
**Purpose**: Unity container wrapper and internal facade

**Key Capabilities**:
- Type-safe and runtime type resolution
- Named registrations
- Instance registrations
- Lifetime manager support
- Generic type registrations
- Injection member support
- Thread-local registrations

**Validation**:
- Validates type compatibility before registration
- Throws `InvalidDependencyRegistrationException` for invalid mappings
- Allows generic type registrations (bypasses validation)

**Methods**:
```csharp
// Resolution
AbstractType Resolve<AbstractType>()
TBase Resolve<TBase>(string name)
object Resolve(Type type)

// Registration (typed)
void Register<AbstractType, ConcreteType>() where ConcreteType : AbstractType
void Register<AbstractType, ConcreteType>(LifetimeManager manager)
void Register<AbstractType, ConcreteType>(string name, params InjectionMember[])
void Register<AbstractType, ConcreteType>(params InjectionMember[])
void Register<TBase>(string name, Type type)

// Registration (dynamic)
void Register(Type base, Type concreteType)
void Register(Type base, Type concreteType, LifetimeManager lifetimeManager)

// Instance registration
void RegisterInstance<AbstractType>(AbstractType instance)
void RegisterInstance<AbstractType>(AbstractType instance, string name, LifetimeManager manager)

// Special lifetime
void RegisterPerThread<AbstractType, ConcreteType>()
```

## Dependencies

### External Dependencies
- **Microsoft.Practices.Unity** (v4.0.1): Core IoC container
- **Unity.AutoRegistration** (v1.0.0.2): Convention-based registration support
- **FluentValidation** (v6.2.1.0): Validator infrastructure
- **System.Web**: For web context access

### Internal Dependencies
- **Core**: Application interfaces, attributes, exceptions
  - `Core.Application.IDependencyInjectionHelper`
  - `Core.Application.IAppContextStore`
  - `Core.Application.ISettings`
  - `Core.Application.IClock`
  - `Core.Application.IUnitOfWork`
  - `Core.Application.Attribute.DefaultImplementationAttribute`
  - `Core.Application.Exceptions.InvalidDependencyRegistrationException`
  - Validators: `ILoginAuthenticationModelValidator`, `ISalesDataUploadCreateModelValidator`
- **Infrastructure**: Concrete implementations
  - `Infrastructure.Application.Impl.UnitOfWork`
  - `Infrastructure.Application.Impl.Clock`

### Dependents
All application modules depend on this for service resolution:
- Web layer (MVC controllers)
- Business logic layer
- Data access layer
- Background services

## Service Lifetime Management

### Lifetime Strategies

#### 1. Transient (Default)
- **Behavior**: New instance per resolution
- **Usage**: Stateless services, DTOs, validators
- **Registration**: `IoC.Register<IService, Service>()`

#### 2. Per-Thread (`PerThreadLifetimeManager`)
- **Behavior**: One instance per thread
- **Usage**: Windows services, background jobs
- **Registration**: `IoC.RegisterPerThread<IService, Service>()`
- **Example**: `IUnitOfWork` in `SetupCurrentContextWinJob()`

#### 3. Per-Request (`AppContextLifetimeManager`)
- **Behavior**: One instance per HTTP request or custom context
- **Usage**: Web applications, request-scoped data
- **Registration**: `new AppContextLifetimeManager(IAppContextStore)`
- **Example**: `IUnitOfWork` and `IClock` in web context

#### 4. Singleton
- **Behavior**: Single instance for application lifetime
- **Usage**: Configuration, caches, stateless utilities
- **Registration**: `IoC.RegisterInstance<T>(instance)`
- **Example**: `Random` instance (note: not thread-safe per TODO)

## Auto-Registration Mechanism

### Convention-Based Discovery
The `RegisterDefaultImplementations()` method uses reflection to:

1. **Scan Assemblies**: Gets all referenced assemblies via `GetReferencedAssemblies()`
2. **Find Candidates**: Locates classes with `[DefaultImplementation]` attribute
3. **Map Interfaces**: 
   - Uses `attribute.Interface` if explicitly set
   - Otherwise, finds interface matching naming convention: `I{ClassName}`
4. **Register**: Calls `IoC.Register(interface, implementation)`

### Example Usage
```csharp
// In Core or Infrastructure assembly
[DefaultImplementation]
public class UserRepository : IUserRepository
{
    // Implementation
}

// Auto-registered as: IUserRepository -> UserRepository

// With explicit interface
[DefaultImplementation(Interface = typeof(ICustomService))]
public class MyService : ICustomService
{
    // Implementation
}
```

### Benefits
- Reduces boilerplate registration code
- Convention over configuration
- Centralized attribute-based metadata
- Automatic interface mapping

## Initialization Flow

### Application Startup Sequence
```
1. DependencyRegistrar.RegisterDependencies()
   ├─→ RegisterDefaultImplementations()      [Auto-discovery]
   ├─→ Manual registrations (helpers, validators)
   ├─→ RegisterInstance(new Random())
   └─→ SetApplicationManager()               [Configure static manager]

2. Context Setup (choose one):
   ├─→ SetupCurrentContext(store)           [Web/Request-scoped]
   └─→ SetupCurrentContextWinJob()          [Windows Service/Thread-scoped]

3. Optional: SetLocalizationBasedRegistration(timezone)
```

## Known Issues & Technical Debt

### Thread Safety
```csharp
// TODO: Random is not thread safe
IoC.RegisterInstance(new Random());
```
**Impact**: Potential contention in multi-threaded scenarios
**Recommendation**: Use `ThreadLocal<Random>` or per-request instances

### Commented Code
Several registrations are commented out:
- Generic repository pattern: `IRepository<>` / `Repository<>`
- Session locator configurations
- Transaction helper registrations

**Implication**: May indicate incomplete feature migration or deprecated patterns

### Lifetime Manager Limitations
`WinJobContextLifetimeManager.SetValue()` and `RemoveValue()` throw `NotImplementedException`
**Impact**: Read-only lifetime manager, cannot update/remove instances

## Integration Points

### Startup Integration
```csharp
// Global.asax.cs or Startup.cs
protected void Application_Start()
{
    DependencyRegistrar.RegisterDependencies();
    var store = new HttpContextStore(); // or custom IAppContextStore
    DependencyRegistrar.SetupCurrentContext(store);
}
```

### Service Resolution
```csharp
// Via helper (recommended)
var helper = ApplicationManager.DependencyInjection;
var service = helper.Resolve<IMyService>();

// Direct (internal only)
var service = IoC.Resolve<IMyService>();
```

### Validator Factory Integration
```csharp
// FluentValidation configuration
ValidatorOptions.ValidatorFactory = new ValidatorFactory();
```

## Configuration

### Unity Container Features Used
- Type mapping with generic constraints
- Named registrations
- Lifetime managers (built-in and custom)
- Injection members (constructor injection)
- Instance registration
- Per-thread scope

### Extensibility Points
1. **Custom Lifetime Managers**: Extend `LifetimeManager` base class
2. **Custom Factories**: Implement Unity factory patterns
3. **Attribute-Based Registration**: Add `[DefaultImplementation]` to classes
4. **Manual Registration**: Add to `DependencyRegistrar.RegisterDependencies()`

## Best Practices

### DO
- Use constructor injection for required dependencies
- Register interfaces, resolve through abstractions
- Use appropriate lifetime scopes
- Mark auto-discovered implementations with `[DefaultImplementation]`
- Keep registration logic in `DependencyRegistrar`

### DON'T
- Don't use service locator pattern outside infrastructure
- Don't register concrete classes directly
- Don't resolve dependencies in constructors (use lazy initialization)
- Don't mix lifetime scopes incorrectly (e.g., singleton depending on transient)
- Don't bypass `DependencyInjectionHelper` in business logic

## Performance Considerations

### Resolution Cost
- First resolution: Reflection + instantiation
- Subsequent resolutions: Cached by Unity (transient still creates new instances)

### Startup Impact
- Auto-discovery scans all referenced assemblies via reflection
- Overhead is one-time at application startup
- Consider pre-registration for performance-critical scenarios

## Security Considerations

### Validation Factory Security
The `ValidatorFactory` intentionally skips validation for certain types to prevent injection attacks:
- Primitives and value types (no complex validation needed)
- Types marked `[NoValidationResolveAtStartAttribute]` (explicit opt-out)
- Generic arrays (e.g., `Int64[]`)

This prevents malicious payloads from triggering unnecessary validator resolution.

## Testing Considerations

### Mock Registration
```csharp
[TestInitialize]
public void Setup()
{
    DependencyRegistrar.RegisterDependencies();
    // Override with mocks
    IoC.Register<IService, MockService>();
}
```

### Isolated Tests
Per-thread lifetime managers help isolate test execution in parallel test runners.

---

**Last Updated**: 2025-01-XX  
**Commit Hash**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366  
**Maintainer**: Infrastructure Team
