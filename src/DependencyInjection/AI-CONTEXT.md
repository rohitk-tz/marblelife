<!-- AUTO-GENERATED: Header -->
# DependencyInjection Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:18:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module serves as the **Composition Root** for the application using the **Unity Container**. It centralizes the configuration of the Inversion of Control (IoC) container, determining how interfaces resolve to concrete implementations across the system.

### Design Patterns
-   **Service Locator (Anti-Pattern utilized safely)**: Uses a static `IoC` wrapper to expose the container, which populates the static `ApplicationManager`.
-   **Convention over Configuration**: Automatically registers classes marked with `[DefaultImplementation]` to their matching `I{ClassName}` interface.
-   **Lifetime Management**: Custom `AppContextLifetimeManager` handles scoping (e.g., Per-Request vs Per-Thread).

### Data Flow (Registration)
1.  **Startup**: `DependencyRegistrar.RegisterDependencies()` is called.
2.  **Auto-Scan**: `RegisterDefaultImplementations()` scans referenced assemblies for `[DefaultImplementation]`.
3.  **Context Setup**: `SetupCurrentContext()` is called (e.g., from `Global.asax`) to bind request-scoped services like `IUnitOfWork`.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### API Wrapper

```csharp
internal static class IoC 
{
    // Wraps UnityContainer methods to prevent direct dependency on Unity throughout the app
    static void Register<Abstract, Concrete>();
    static T Resolve<T>();
}
```

### Attributes

```csharp
// Used in other modules to mark auto-registrable classes
[AttributeUsage(AttributeTargets.Class)]
public class DefaultImplementationAttribute : Attribute 
{
    public Type Interface { get; set; } // Optional: Force specific interface
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### `DependencyRegistrar`
-   **`RegisterDependencies()`**: Main entry point. Registers global singletons and performs the assembly scan.
-   **`SetupCurrentContext(IAppContextStore)`**: Registers request-scoped dependencies (DbConnection, UoW, Clock). The `IAppContextStore` abstraction allows different storage mechanisms (HTTP Context vs Thread Local).
-   **`SetupCurrentContextWinJob()`**: Registers thread-scoped dependencies for background jobs (Console Apps).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core](../Core/AI-CONTEXT.md)** - Registers Core interfaces (`IClock`, `ISettings`).
-   **[Infrastructure](../Infrastructure/AI-CONTEXT.md)** - Registers Infrastructure implementations (`UnitOfWork`, `Repository`).

### External
-   **Microsoft.Practices.Unity** - The underlying DI Container.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Lifecycle Management
-   **Web Apps**: Use `SetupCurrentContext` with `AppContextLifetimeManager`. This ensures one `MakaluDbContext` per HTTP request.
-   **Background Jobs**: Use `SetupCurrentContextWinJob`. This ensures one `MakaluDbContext` per thread.
-   **Critical**: Mixing these up often leads to "DbContext has been disposed" errors or memory leaks.

### Debugging Resolution Failures
-   If `IoC.Resolve<T>` fails, check:
    1.  Is the implementation marked with `[DefaultImplementation]`?
    2.  Is `RegisterDependencies()` called in the app startup?
    3.  Is the referenced assembly actually loaded?
<!-- END CUSTOM SECTION -->
