# DependencyInjection Module - Developer Overview

## What is This Module?

The DependencyInjection module is the **heart of service management** in the MarbleLife application. It provides a centralized way to register, configure, and retrieve services throughout the application using Microsoft Unity as the underlying IoC (Inversion of Control) container.

Think of it as a **smart factory** that:
- Knows how to create every service in your application
- Manages their lifecycles (when to create, reuse, or dispose)
- Automatically wires up dependencies between services
- Discovers services by convention using attributes

## Quick Start

### Setting Up Dependency Injection

**For Web Applications:**
```csharp
// In Global.asax.cs
protected void Application_Start()
{
    // Step 1: Register all services
    DependencyRegistrar.RegisterDependencies();
    
    // Step 2: Setup request-scoped lifetime
    var contextStore = new HttpContextStore();
    DependencyRegistrar.SetupCurrentContext(contextStore);
    
    // Optional: Setup timezone-aware services
    DependencyRegistrar.SetLocalizationBasedRegistration("Pacific Standard Time");
}
```

**For Windows Services/Background Jobs:**
```csharp
// In service startup
protected override void OnStart(string[] args)
{
    DependencyRegistrar.RegisterDependencies();
    DependencyRegistrar.SetupCurrentContextWinJob(); // Per-thread lifetime
}
```

### Resolving Services

**Recommended Approach (via ApplicationManager):**
```csharp
public class MyController : Controller
{
    private readonly IUserService _userService;
    
    public MyController()
    {
        // Resolve from the global DI helper
        _userService = ApplicationManager.DependencyInjection.Resolve<IUserService>();
    }
    
    public ActionResult GetUser(int id)
    {
        var user = _userService.GetById(id);
        return View(user);
    }
}
```

**Alternative (Manual Resolution):**
```csharp
var helper = new DependencyInjectionHelper();
var service = helper.Resolve<IMyService>();
```

## How to Register Services

### Method 1: Auto-Registration with Attributes (Recommended)

The easiest way to register a service is to decorate your implementation class with `[DefaultImplementation]`:

```csharp
// In your Core or Infrastructure project
using Core.Application.Attribute;

// Simple auto-registration (finds interface by convention: IUserRepository)
[DefaultImplementation]
public class UserRepository : IUserRepository
{
    public User GetById(int id)
    {
        // Implementation
    }
}

// Explicit interface specification
[DefaultImplementation(Interface = typeof(ICustomService))]
public class SpecialService : ICustomService, IOtherInterface
{
    // Implementation
}
```

**How it works:**
- The module scans all referenced assemblies for `[DefaultImplementation]` attributes
- If no interface is specified, it looks for an interface matching `I{ClassName}`
- Automatically registers the mapping

### Method 2: Manual Registration

For more control, add registrations in `DependencyRegistrar.RegisterDependencies()`:

```csharp
public static void RegisterDependencies()
{
    RegisterDefaultImplementations();
    
    // Add your manual registrations here:
    IoC.Register<IMyService, MyService>();
    IoC.Register<IEmailService, EmailService>();
    IoC.Register<IDependencyInjectionHelper, DependencyInjectionHelper>();
    
    // Singleton instance
    IoC.RegisterInstance(new Random());
    
    SetApplicationManager();
}
```

## Service Lifetimes Explained

### Transient (New Instance Every Time)
**When to use:** Stateless services, lightweight objects, validators

```csharp
IoC.Register<IEmailService, EmailService>();

// Each resolution creates a new instance
var email1 = IoC.Resolve<IEmailService>(); // New instance
var email2 = IoC.Resolve<IEmailService>(); // Another new instance
```

### Per-Request (One Instance Per HTTP Request)
**When to use:** Database contexts, request-specific data, unit of work pattern

```csharp
// In SetupCurrentContext()
IoC.Register<IUnitOfWork, UnitOfWork>(
    new AppContextLifetimeManager(IoC.Resolve<IAppContextStore>())
);

// Same instance throughout the request
var uow1 = IoC.Resolve<IUnitOfWork>(); // Creates instance
var uow2 = IoC.Resolve<IUnitOfWork>(); // Returns same instance (within same request)
```

### Per-Thread (One Instance Per Thread)
**When to use:** Background jobs, Windows services, thread-local storage

```csharp
// In SetupCurrentContextWinJob()
IoC.Register<IUnitOfWork, UnitOfWork>(new PerThreadLifetimeManager());

// Same instance within a thread, different across threads
var uow1 = IoC.Resolve<IUnitOfWork>(); // Creates instance for this thread
var uow2 = IoC.Resolve<IUnitOfWork>(); // Same instance (same thread)
```

### Singleton (One Instance for Application)
**When to use:** Configuration objects, caches, stateless utilities

```csharp
IoC.RegisterInstance(new ConfigurationManager());

// Always returns the same instance
var config1 = IoC.Resolve<ConfigurationManager>(); // The singleton
var config2 = IoC.Resolve<ConfigurationManager>(); // Same instance
```

## Real-World Usage Examples

### Example 1: Repository Pattern with Unit of Work

```csharp
// 1. Define interfaces (in Core project)
public interface IUserRepository
{
    User GetById(int id);
    void Save(User user);
}

public interface IUnitOfWork
{
    void Commit();
    void Rollback();
}

// 2. Implement with auto-registration (in Infrastructure project)
[DefaultImplementation]
public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;
    
    public UserRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork; // Injected automatically
    }
    
    public User GetById(int id)
    {
        // Implementation
    }
    
    public void Save(User user)
    {
        // Save logic
        _unitOfWork.Commit();
    }
}

// 3. Use in your application
public class UserController : Controller
{
    private readonly IUserRepository _userRepo;
    
    public UserController()
    {
        _userRepo = ApplicationManager.DependencyInjection.Resolve<IUserRepository>();
    }
    
    public ActionResult Save(UserViewModel model)
    {
        var user = new User { Name = model.Name };
        _userRepo.Save(user);
        return RedirectToAction("Index");
    }
}
```

### Example 2: Validator Factory Integration

```csharp
// 1. Create a validator (FluentValidation)
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name).NotEmpty();
        RuleFor(u => u.Email).EmailAddress();
    }
}

// 2. Register validator
IoC.Register<IValidator<User>, UserValidator>();

// 3. Configure FluentValidation to use IoC
ValidatorOptions.ValidatorFactory = new ValidatorFactory();

// 4. Validate automatically
var user = new User { Name = "", Email = "invalid" };
var validator = IoC.Resolve<IValidator<User>>();
var result = validator.Validate(user);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error.ErrorMessage);
    }
}
```

### Example 3: Named Registrations

```csharp
// Register multiple implementations with names
IoC.Register<IMessageSender>("Email", typeof(EmailSender));
IoC.Register<IMessageSender>("SMS", typeof(SmsSender));

// Resolve by name
var emailSender = IoC.Resolve<IMessageSender>("Email");
var smsSender = IoC.Resolve<IMessageSender>("SMS");

emailSender.Send("Hello via Email!");
smsSender.Send("Hello via SMS!");
```

## Common Patterns

### Pattern 1: Service Layer with Dependencies

```csharp
[DefaultImplementation]
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    
    public OrderService(
        IOrderRepository orderRepo,
        IEmailService emailService,
        ILogger logger)
    {
        _orderRepo = orderRepo;
        _emailService = emailService;
        _logger = logger;
    }
    
    public void PlaceOrder(Order order)
    {
        try
        {
            _orderRepo.Save(order);
            _emailService.SendOrderConfirmation(order);
            _logger.Info($"Order {order.Id} placed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error("Order placement failed", ex);
            throw;
        }
    }
}
```

### Pattern 2: Factory Pattern with IoC

```csharp
public interface IReportFactory
{
    IReport CreateReport(string reportType);
}

[DefaultImplementation]
public class ReportFactory : IReportFactory
{
    private readonly IDependencyInjectionHelper _di;
    
    public ReportFactory(IDependencyInjectionHelper di)
    {
        _di = di;
    }
    
    public IReport CreateReport(string reportType)
    {
        switch (reportType)
        {
            case "Sales":
                return _di.Resolve<ISalesReport>();
            case "Inventory":
                return _di.Resolve<IInventoryReport>();
            default:
                throw new ArgumentException("Unknown report type");
        }
    }
}
```

## Troubleshooting

### Problem: "Type is not registered"

**Error:**
```
Resolution of the dependency failed, type = "IMyService", name = "(none)".
Exception occurred while: while resolving.
```

**Solutions:**
1. Add `[DefaultImplementation]` to your implementation class
2. Manually register in `DependencyRegistrar.RegisterDependencies()`:
   ```csharp
   IoC.Register<IMyService, MyService>();
   ```
3. Ensure the assembly containing your service is referenced
4. Verify the interface name matches convention (`IClassName` for `ClassName`)

### Problem: Circular Dependencies

**Error:**
```
Resolution of the dependency failed...
Exception occurred while: while resolving...
[Stack shows circular reference: A -> B -> A]
```

**Solutions:**
1. Use property injection instead of constructor injection:
   ```csharp
   public IServiceB ServiceB { get; set; } // Will be injected after construction
   ```
2. Introduce an intermediate interface to break the cycle
3. Use lazy initialization:
   ```csharp
   private Lazy<IServiceB> _serviceB;
   public ServiceA(Func<IServiceB> serviceFactory)
   {
       _serviceB = new Lazy<IServiceB>(serviceFactory);
   }
   ```

### Problem: Lifetime Scope Mismatch

**Symptom:** Stale data, disposed objects, or "ObjectDisposedException"

**Common mistakes:**
- Singleton service depending on per-request service
- Per-request service stored in static variable

**Solution:**
Always ensure parent lifetime >= child lifetime:
```
Singleton >= Per-Thread >= Per-Request >= Transient
```

**Example fix:**
```csharp
// BAD: Singleton depending on request-scoped
public class CacheSingleton
{
    private readonly IUnitOfWork _uow; // Request-scoped!
}

// GOOD: Resolve per-request dependency each time
public class CacheSingleton
{
    private readonly IDependencyInjectionHelper _di;
    
    public void DoWork()
    {
        var uow = _di.Resolve<IUnitOfWork>(); // Fresh instance
    }
}
```

### Problem: "InvalidDependencyRegistrationException"

**Error:**
```
Type 'ConcreteType' cannot be assigned to type 'IInterface'
```

**Causes:**
- Implementation doesn't inherit/implement the interface
- Type mismatch (wrong generic arguments)

**Solution:**
Verify your class signature:
```csharp
// Ensure this is correct
public class UserRepository : IUserRepository // Must implement!
{
    // ...
}
```

### Problem: Validators Not Resolving

**Symptom:** Validation doesn't run or throws exceptions

**Checklist:**
1. Register `ValidatorFactory`:
   ```csharp
   ValidatorOptions.ValidatorFactory = new ValidatorFactory();
   ```
2. Register your validator:
   ```csharp
   IoC.Register<IValidator<User>, UserValidator>();
   ```
3. Check for exclusion attributes:
   - `[NoValidationResolveAtStart]`
   - `[NoValidatorRequired]`

## Best Practices

### ✅ DO

1. **Use constructor injection for required dependencies**
   ```csharp
   public UserService(IUserRepository repo, ILogger logger)
   {
       _repo = repo ?? throw new ArgumentNullException(nameof(repo));
       _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }
   ```

2. **Depend on abstractions (interfaces), not concrete types**
   ```csharp
   private readonly IEmailService _emailService; // Good
   // NOT: private readonly SmtpEmailService _emailService; // Bad
   ```

3. **Use appropriate lifetime scopes**
   - Transient: Stateless services
   - Per-Request: Database contexts, UnitOfWork
   - Singleton: Configuration, caches

4. **Mark auto-discovered services with `[DefaultImplementation]`**
   ```csharp
   [DefaultImplementation]
   public class MyService : IMyService { }
   ```

5. **Resolve services at the composition root (startup)**
   ```csharp
   protected void Application_Start()
   {
       DependencyRegistrar.RegisterDependencies();
   }
   ```

### ❌ DON'T

1. **Don't use Service Locator in business logic**
   ```csharp
   // Bad: Service locator anti-pattern
   public void ProcessOrder()
   {
       var repo = IoC.Resolve<IOrderRepository>();
   }
   
   // Good: Constructor injection
   private readonly IOrderRepository _repo;
   public OrderService(IOrderRepository repo) { _repo = repo; }
   ```

2. **Don't register concrete classes**
   ```csharp
   IoC.Register<UserService, UserService>(); // Bad
   IoC.Register<IUserService, UserService>(); // Good
   ```

3. **Don't resolve in constructors (use lazy)**
   ```csharp
   // Bad: Creates dependencies immediately
   public MyService()
   {
       _dependency = IoC.Resolve<IDependency>();
   }
   
   // Good: Inject dependency
   public MyService(IDependency dependency)
   {
       _dependency = dependency;
   }
   ```

4. **Don't mix lifetime scopes incorrectly**
   ```csharp
   // Bad: Singleton with per-request dependency
   IoC.RegisterInstance(new CacheService(IoC.Resolve<IUnitOfWork>()));
   ```

## Performance Tips

1. **Minimize resolution calls**: Resolve once, reuse the instance
2. **Use lazy initialization** for expensive services:
   ```csharp
   private Lazy<IExpensiveService> _service;
   public MyClass(Func<IExpensiveService> factory)
   {
       _service = new Lazy<IExpensiveService>(factory);
   }
   ```
3. **Prefer constructor injection** over property injection (Unity caches constructors)
4. **Avoid resolving in loops**: Resolve once before the loop

## Migration Guide

### From Manual Instantiation to DI

**Before:**
```csharp
public class OrderController
{
    public ActionResult Create()
    {
        var context = new ApplicationDbContext();
        var repo = new OrderRepository(context);
        var service = new OrderService(repo, new EmailService());
        
        // Use service
    }
}
```

**After:**
```csharp
// 1. Define interfaces
public interface IOrderService { }
public interface IOrderRepository { }

// 2. Mark implementations
[DefaultImplementation]
public class OrderService : IOrderService { }

[DefaultImplementation]
public class OrderRepository : IOrderRepository { }

// 3. Use DI
public class OrderController
{
    private readonly IOrderService _orderService;
    
    public OrderController()
    {
        _orderService = ApplicationManager.DependencyInjection.Resolve<IOrderService>();
    }
}
```

## Additional Resources

- **Unity Documentation**: https://github.com/unitycontainer/unity
- **FluentValidation**: https://fluentvalidation.net/
- **Dependency Injection Principles**: Martin Fowler's IoC article
- **Lifetime Management**: Unity Lifetime Manager documentation

## Getting Help

**Common issues solved:**
- Service not found → Check registration and naming conventions
- Circular dependencies → Use property injection or lazy loading  
- Performance issues → Review lifetime scopes and resolution patterns
- Validation not working → Check `ValidatorFactory` setup

**For support:**
- Check `CONTEXT.md` for technical architecture details
- Review `DependencyRegistrar.cs` for current registrations
- Consult team wiki for project-specific patterns

---

**Module Version**: 1.0  
**Last Updated**: 2025-01-XX  
**Maintained By**: Infrastructure Team
