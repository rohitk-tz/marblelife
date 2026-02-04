<!-- AUTO-GENERATED: Header -->
# Dependency Injection
> IoC Container Configuration using Unity
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module manages the application's dependencies using the **Unity** container. It acts as the "glue" that connects interfaces in **Core** to implementations in **Infrastructure** or **Services**.

Key Features:
-   **Automatic Registration**: Scans for the `[DefaultImplementation]` attribute.
-   **Context Scoping**: Handles dependency lifecycles for both Web (Per Request) and Jobs (Per Thread).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Registering a New Service

**Option 1: Attribute (Recommended)**
Add the `[DefaultImplementation]` attribute to your concrete class in the *Impl* namespace.

```csharp
[DefaultImplementation]
public class MyService : IMyService 
{
    // ...
}
// Automatically registered as: IoC.Register<IMyService, MyService>();
```

**Option 2: Explicit Registration**
Modify `DependencyRegistrar.RegisterDependencies()`:

```csharp
IoC.Register<IMyService, MyService>();
// OR Singleton
IoC.RegisterInstance<IMyService>(new MyService());
```

### Resolving Dependencies

**Constructor Injection (Preferred)**
```csharp
public class MyController 
{
    private readonly IMyService _service;
    
    public MyController(IMyService service) // Unity injects this automatically
    {
        _service = service;
    }
}
```

**Service Locator (Use Sparingly)**
```csharp
var service = IoC.Resolve<IMyService>();
```
<!-- END AUTO-GENERATED -->
