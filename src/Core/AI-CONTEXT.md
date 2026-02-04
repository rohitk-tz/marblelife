<!-- AUTO-GENERATED: Header -->
# Core Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:20:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Application Core** and **Business Layer**. It contains:
1.  **Domain Entities**: POCO classes mapped to the database (in `{Module}/Domain`).
2.  **DTOs**: View Models used for API requests/responses (in `{Module}/ViewModels`).
3.  **Business Logic**: Service implementations that orchestrate validation, calculations, and data persistence (in `{Module}/Impl`).
4.  **Interfaces**: The public contracts for all services.

### Application Logic Flow
1.  **Input**: Controller (API layer) calls `IService.Method(viewModel)`.
2.  **Validation**: Service uses validators (e.g., `LoginAuthenticationModelValidator`) to check inputs.
3.  **Processing**: Service performs business logic (e.g., "Check login attempts", "Calculate Tax").
4.  **Persistence**: Service uses `IRepository<T>` to fetch/save data via the UnitOfWork.
5.  **Output**: Returns a Domain Entity or View Model.

### Design Patterns
-   **Domain-Driven Design (Lite)**: organized by Bounded Contexts (Users, Billing, Sales).
-   **Service Layer**: Facade over the domain model and repositories.
-   **Anemic Domain Model**: Entities are mostly data buckets; logic resides in Services. (Typical in EF applications).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Standard Module Structure
Each sub-folder (e.g., `Users`, `Sales`) follows this structure:

-   `Domain/`: Database Entities (e.g., `Person`, `Invoice`).
-   `ViewModels/`: Data Transfer Objects (e.g., `LoginModel`, `SalesReportRequest`).
-   `Impl/`: Service Implementations (e.g., `UserLoginService`).
-   `I{Service}.cs`: Service Interfaces (e.g., `IUserLoginService`).

### Key Domain Modules

| Module | Responsibility |
| :--- | :--- |
| **App** | `User`, `Person`, `Role`, `UserLogin` (Auth & Identity). |
| **Billing** | `Invoice`, `Payment`, `AuthorizeNet` integration. |
| **Sales** | `MarketingLead`, `FranchiseeSales`, External Data Uploads. |
| **Scheduler** | `Job`, `Service`, `Technician` scheduling. |
| **Organizations** | `Franchisee`, `Organization` hierarchy. |
| **Notification** | Email queuing and template management. |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### Service Implementation Pattern
Services are marked with `[DefaultImplementation]` for auto-dependency injection.

```csharp
[DefaultImplementation]
public class UserLoginService : IUserLoginService 
{
    public UserLoginService(IUnitOfWork uow, IClock clock, ...) { ... }
    
    public UserLogin GetbyUserName(string userName) { ... }
    public bool Lock(long userId, bool isLocked, ...) { ... }
}
```

### Validator Pattern
Validators are often separated into their own interfaces/classes.

```csharp
public interface ILoginAuthenticationModelValidator {
    ValidationResult Validate(LoginModel model);
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Infrastructure](../Infrastructure/AI-CONTEXT.md)** - Provides `IRepository<T>`, `IUnitOfWork`, and Payment Service implementations.

### External
-   **Newtonsoft.Json** - Serialization.
-   **AutoMapper** (Likely used in localized places or manually mapped).

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Timezone Handling
-   The `UserLoginService` shows custom logic for EST/EDT handling (`IsEST()`). This suggests the application has specific logic around US Eastern Time that overrides standard UTC handling in places.
-   **Watch out**: `DateTime.Now` is used in `IsEST()`, which relies on the server's local time. Ideally, this should use `IClock`.

### Legacy Artifacts
-   Files like `UpgradeLog.htm` (seen in root) suggest this project has been upgraded from older .NET versions or Visual Studio versions.
-   Some "God Classes" (e.g., huge controllers or services) might exist given the folder sizes.
<!-- END CUSTOM SECTION -->
