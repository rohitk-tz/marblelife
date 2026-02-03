<!-- AUTO-GENERATED: Header -->
# Core.Application
> Shared Kernel & Base Classes
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This is the foundation of the `src/Core` library.
It defines:
-   **DomainBase**: The parent class for all Entities.
-   **Repository Pattern**: Interfaces for data access.
-   **Cross-Cutting Concerns**: Logging, Configuration, Exception Handling.

### Key Features
-   **IUnitOfWork**: Manages database transactions.
-   **ISettings**: Strongly-typed access to `web.config` appSettings.
-   **IClock**: Testable date/time provider.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Creating an Entity
Inherit from `DomainBase`:
```csharp
public class MyEntity : DomainBase {
    public string Name { get; set; }
}
```

### Accessing Data
Inject `IUnitOfWork` into your service:
```csharp
public MyService(IUnitOfWork uow) {
    _repo = uow.Repository<MyEntity>();
}
```

<!-- END AUTO-GENERATED -->
