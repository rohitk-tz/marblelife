<!-- AUTO-GENERATED: Header -->
# Infrastructure Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:15:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module implements the **infrastructure plumbing** and **external system integrations** for the application. It hides the complexity of:
1.  **Data Access**: Generic Repository and Unit of Work patterns.
2.  **Payment Processing**: Authorize.Net integration for Credit Cards and eChecks.
3.  **Cross-Cutting Concerns**: Logging, PDF generation.

### Design Patterns
-   **Repository Pattern**: `Repository<T>` abstracts Entity Framework logic, providing a clean API (`Fetch`, `Insert`, `Delete`) to the domain layer.
-   **Unit of Work**: `UnitOfWork` manages the database transaction scope, ensuring atomic operations across multiple repositories.
-   **Adapter Pattern**: `AuthorizeNetCustomerProfileService` adapts the Authorize.Net SDK to the application's specific billing needs (Accounts, Profiles, Transactions).
-   **Dependency Injection**: Heavily relies on DI to inject `DbContext`, `ISettings`, and `ILogService`.

### Data Flow (Payment Processing)
1.  **Billing Request** (e.g., "Charge Card") enters `AuthorizeNetCustomerProfileService`.
2.  **Credential Resolution**: Looks up API keys based on `AccountTypeId`.
3.  **SDK Interaction**: Constructs `ANetApiRequest` objects (Environment, Auth, Transaction).
4.  **Execution**: Calls Authorize.Net Controllers (`createTransactionController`).
5.  **Response Handling**: Parses `ANetApiResponse`, handling success/error codes and mapping them to domain responses.

### Data Flow (Data Access)
1.  **Service Layer** requests `IUnitOfWork.Repository<User>()`.
2.  **UnitOfWork** checks internal cache; if missing, creates `Repository<User>` with the current `DbContext`.
3.  **Repository** executes LINQ query on `DbSet<User>`.
4.  **UnitOfWork.SaveChanges()** commits the underlying EF transaction.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Data Access Contracts

```csharp
public interface IRepository<T> where T : DomainBase
{
    // Core CRUD
    T Get(long id);
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
    
    // Querying
    IQueryable<T> Table { get; } // With tracking
    IQueryable<T> TableNoTracking { get; } // Read-only helper
    IEnumerable<T> Fetch(Expression<Func<T, bool>> expression);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : DomainBase;
    void StartTransaction();
    void SaveChanges(); // Commits transaction
    void Rollback();
}
```

### Billing Contracts

```csharp
public interface IAuthorizeNetCustomerProfileService
{
    // Profile Management
    createCustomerProfileResponse CreateNewProfile(...);
    createCustomerPaymentProfileResponse CreateAdditionalPaymentProfile(...);
    deleteCustomerPaymentProfileResponse DeleteCustomerProfile(...);
    
    // Transaction Processing
    ANetApiResponse ChargeNewCard(...);
    ANetApiResponse ChargeCustomerProfile(...);
    ANetApiResponse VoidPayment(...);
    createTransactionResponse DebitBankAccount(...); // eCheck
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### `UnitOfWork`
-   **Scope**: Per-Request (typically).
-   **Behavior**:
    -   Lazy-loads repositories.
    -   Manages a single `MakaluDbContext` instance.
    -   Wraps operations in a database transaction (`IsolationLevel.ReadCommitted`).
    -   `SaveChanges` commits the transaction; `Dispose` rolls back if not committed.

### `Repository<T>`
-   **Behavior**:
    -   Wraps EF `DbSet`.
    -   `Update` handles `SaveCascadeHelper` to manage complex object graphs.
    -   `Fetch` returns `IEnumerable` (materialized), while `Table` returns `IQueryable` (deferred).
    -   Explicit `Insert`/`Update` separation vs EF's `Add`/`Attach`.

### `AuthorizeNetCustomerProfileService`
-   **Inputs**: Raw card details or Profile IDs (`customerProfileId`, `paymentProfileId`).
-   **Behavior**:
    -   Dynamically switches environments (`SANDBOX` vs `PRODUCTION`) based on `ISettings`.
    -   Handles both specific Card transactions and "Customer Profile" (saved card) transactions.
    -   Supports eChecks (Bank Account debits).
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[ORM](../ORM/AI-CONTEXT.md)** - Uses `MakaluDbContext` for data persistence.
-   **[Core](../Core/AI-CONTEXT.md)** - Uses `DomainBase` and domain entities (`AuthorizeNetApiMaster`).

### External
-   **AuthorizeNet.Api** - SDK for payment gateway interaction.
-   **Entity Framework** - Underlying ORM.

### Configuration
-   **Authorize.Net Keys**: Stored in `AuthorizeNetApiMaster` table (Database-driven config, not just app.config).
-   **Environment**: Controlled by `ISettings.AuthNetTestMode`.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Gotchas
-   **UnitOfWork Transaction Logic**: The transaction is started explicitly in `StartTransaction()` (often triggered lazily). Ensure `SaveChanges()` is called or the transaction rolls back on dispose.
-   **Repository Update**: The `Update` method performs a `SetValues` copy from the input entity to the attached entity. This allows working with detached objects but requires careful handling of relationships (handled by `SaveCascadeHelper`).
-   **Authorize.Net Error Handling**: The wrapper methods often return the raw `ANetApiResponse`. Consumers must check `messages.resultCode` manually.

### Architecture Note
This layer effectively decouples the *Core* domain from the *ORM* mechanics and *External APIs*, complying with Clean Architecture principles (mostly).
<!-- END CUSTOM SECTION -->
