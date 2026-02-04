<!-- AUTO-GENERATED: Header -->
# Core.Application Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:35:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Shared Kernel / Base Class Library**. It defines the fundamental building blocks used by every other module in the system.
It contains the **Interfaces** for Infrastructure concerns (Logging, Config, Data Access) so that the Domain layer remains decoupled from implementation details.

### Key Components
-   **Interfaces**: `IUnitOfWork`, `IRepository<T>`, `ISettings`, `ILogService`.
-   **Base Classes**: `DomainBase` (Id, IsNew, IsDeleted).
-   **Utilities**: `IClock` (Testable time), `ICryptographyOneWayHashService` (Passwords).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Data Access Contracts
-   `IRepository<T>`: Generic CRUD contract.
-   `IUnitOfWork`: Transaction management contract.

### Value Types
-   `Name`: Value object for FirstName/LastName.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Usage
-   **EVERYTHING**: Every other module in `Core.*` depends on `Core.Application`.
-   **Implementation**: The actual implementation of these interfaces resides in `src/Infrastructure`.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Dependency Injection
This module defines the *Service Contracts*. The `DepedencyInjection` module binds these to `Infrastructure` implementations.
**Rule**: Always code against `IClock`, not `DateTime.Now`.

### Settings
`ISettings` is a massive interface linked to `Web.config` / `App.config`. If you need to add a new config key, add it here first.
<!-- END CUSTOM SECTION -->
