<!-- AUTO-GENERATED: Header -->
# Core — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2026-02-10T10:57:49Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Domain models, business logic, and service contracts for the MarbleLife franchise management system. Implements clean architecture with domain-driven design patterns: domain entities (1146 C# files across 15 business domains), service interfaces (150+), factories for object creation, FluentValidation for input validation, and ViewModels for API contracts. Each subfolder represents a bounded context (Billing, Organizations, Sales, Users, Scheduler, etc.).

### Design Patterns
- **Domain-Driven Design**: Bounded contexts per business domain (Billing, Organizations, Sales, Users, Notification, Reports, Review, Scheduler, Geo, MarketingLead, Dashboard, ToDo, ReviewApi, AWS, Localization)
- **Factory Pattern**: Every domain entity has `I*Factory` interface for creation/mapping (e.g., `IInvoiceFactory`, `IPersonFactory`, `IJobFactory`)
- **Service Pattern**: Business logic encapsulated in `I*Service` interfaces (e.g., `IInvoiceService`, `IPaymentService`, `IUserService`)
- **Repository Pattern**: Data access via `IRepository<T>` generic interface (implemented in Infrastructure layer)
- **Unit of Work**: Transaction management via `IUnitOfWork` (implemented in Infrastructure layer)
- **Fluent Validation**: `AbstractValidator<T>` for all ViewModels/DTOs with composable validation rules
- **Dependency Injection**: `[DefaultImplementation]` attribute auto-wires interfaces to implementations

### Data Flow
1. API controller receives ViewModel from HTTP request
2. `IValidator<ViewModel>` validates input via FluentValidation rules
3. `I*Factory.CreateDomain(ViewModel)` converts ViewModel → Domain entity
4. `I*Service` executes business logic using `IRepository<T>` and `IUnitOfWork`
5. On save: ORM layer applies audit metadata (CreatedBy/DateCreated/ModifiedBy/DateModified)
6. `I*Factory.CreateViewModel(DomainEntity)` converts Domain → ViewModel for response
7. Side-effects: `INotificationService` queues emails, `I*PollingAgent` schedules async jobs
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### DomainBase (Foundation for All Entities)
```csharp
// All domain entities inherit from this base class
public abstract class DomainBase
{
    public virtual long Id { get; set; } // Primary key
    
    [NotMapped]
    public bool IsNew { get; set; } // Indicates entity is being created (for audit tracking)
    
    [NotMapped]
    public bool IsDeleted { get; set; } // Soft delete flag (actual DB column managed by ORM)
}
```

### Module Structure (15 Bounded Contexts)
Each subfolder contains:
- **Domain/** - Entity classes inheriting from DomainBase (15-40 per module)
- **Enum/** - Enumerations for status, types, states
- **Impl/** - Concrete service implementations
- **ViewModel/** - DTOs for API/UI contracts
- **I*Service.cs** - Service interfaces
- **I*Factory.cs** - Factory interfaces
- **I*Validator.cs** - Validation interfaces

### Cross-Cutting Interfaces (Application/)
```csharp
public interface IRepository<T> where T : DomainBase
{
    IQueryable<T> Table { get; } // Queryable with soft-delete filter applied
    T Get(long id);
    void Insert(T entity);
    void Update(T entity);
    void Delete(long id); // Soft delete
}

public interface IUnitOfWork : IDisposable
{
    void Setup(); // Opens connection and begins transaction
    IRepository<T> Repository<T>() where T : DomainBase;
    void SaveChanges(); // Commits transaction
    void Rollback();
}

public interface ISessionContext
{
    UserSession UserSession { get; } // Current logged-in user info
}

public interface IClock
{
    DateTime UtcNow { get; } // Testable time abstraction
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Submodule Documentation
Each subfolder has detailed `.context/CONTEXT.md` and `.context/OVERVIEW.md` files:

| Module | Purpose | Key Entities | Documentation |
|--------|---------|--------------|---------------|
| **Application** | Infrastructure abstractions, base classes, DI helpers | `DomainBase`, `File`, `Folder`, `Lookup` | [Application CONTEXT](./Application/.context/CONTEXT.md) |
| **Billing** | Payment processing, invoicing, financial transactions | `Invoice`, `Payment`, `ChargeCard`, `ECheck`, `PaymentItem` | [Billing CONTEXT](./Billing/.context/CONTEXT.md) |
| **Organizations** | Franchisee management, fee structures, organizational hierarchy | `Franchisee`, `Organization`, `FeeProfile`, `RoyaltyFeeSlabs` | [Organizations CONTEXT](./Organizations/.context/CONTEXT.md) |
| **Geo** | Geographic data (addresses, cities, states, zips, countries) | `Address`, `City`, `State`, `Zip`, `Country` | [Geo CONTEXT](./Geo/.context/CONTEXT.md) |
| **Sales** | Sales data, customer management, marketing classes | `Customer`, `AccountCredit`, `MarketingClass`, `EstimateInvoice` | [Sales CONTEXT](./Sales/.context/CONTEXT.md) |
| **Users** | User authentication, roles, person/contact management | `Person`, `User`, `UserLogin`, `Role`, `EmailSignatures` | [Users CONTEXT](./Users/.context/CONTEXT.md) |
| **Notification** | Email/notification queue, templates, distribution | `NotificationQueue`, `EmailTemplate`, `NotificationEmail` | [Notification CONTEXT](./Notification/.context/CONTEXT.md) |
| **Reports** | Financial reports, sales analytics, audit trails | Report factories, email record creation | [Reports CONTEXT](./Reports/.context/CONTEXT.md) |
| **Review** | Customer feedback & review management | `CustomerFeedback`, `CustomerFeedbackRequest`, `CustomerFeedbackResponse` | [Review CONTEXT](./Review/.context/CONTEXT.md) |
| **MarketingLead** | Lead management, HomeAdvisor integration | `MarketingLeads`, lead conversion tracking | [MarketingLead CONTEXT](./MarketingLead/.context/CONTEXT.md) |
| **Scheduler** | Job scheduling, appointments, estimates | `Job`, `JobScheduler`, `EstimateInvoice`, `BeforeAfterImages` | [Scheduler CONTEXT](./Scheduler/.context/CONTEXT.md) |
| **ToDo** | Task/todo items management | `ToDo` entity and factory | [ToDo CONTEXT](./ToDo/.context/CONTEXT.md) |
| **ReviewApi** | External review API integration | ReviewApi factory and service | [ReviewApi CONTEXT](./ReviewApi/.context/CONTEXT.md) |
| **Dashboard** | Dashboard data aggregation & widgets | Dashboard factory and service | [Dashboard CONTEXT](./Dashboard/.context/CONTEXT.md) |
| **AWS** | AWS cloud services integration (S3, etc.) | AWS service abstractions | [AWS CONTEXT](./AWS/.context/CONTEXT.md) |
| **Localization** | Multi-language support, translations | Validation translations | [Localization CONTEXT](./Localization/.context/CONTEXT.md) |

### Common Service Patterns

#### Factory Interface Pattern
```csharp
public interface IInvoiceFactory
{
    Invoice CreateDomain(InvoiceViewModel viewModel); // ViewModel → Domain
    InvoiceViewModel CreateViewModel(Invoice domain); // Domain → ViewModel
    Invoice CreateNew(long franchiseeId, DateTime invoiceDate); // Factory method
}
```

#### Service Interface Pattern
```csharp
public interface IInvoiceService
{
    Invoice Get(long id);
    IEnumerable<Invoice> GetByFranchisee(long franchiseeId);
    Invoice Create(InvoiceViewModel viewModel);
    void Update(Invoice invoice);
    void Delete(long id); // Soft delete
    void ProcessLateFees(long franchiseeId); // Business logic
}
```

#### Validator Pattern
```csharp
public class InvoiceViewModelValidator : AbstractValidator<InvoiceViewModel>
{
    public InvoiceViewModelValidator()
    {
        RuleFor(x => x.FranchiseeId).GreaterThan(0);
        RuleFor(x => x.InvoiceDate).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Invoice must have at least one item");
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

- **Internal**: None (Core is the foundation layer - other modules depend on it)
- **External**: 
  - `FluentValidation 6.2.1` — Input validation framework
  - `System.ComponentModel.DataAnnotations` — Attribute-based validation
  - `EntityFramework 6.1.3` — Referenced for IQueryable extensions (not direct EF usage)
  - `Authorize.Net 1.9.0` — Payment gateway integration (Billing module)
  - `DocumentFormat.OpenXml` — Excel file generation
  - `Ionic.Zip` — ZIP archive operations
  - `Newtonsoft.Json 13.0.3` — JSON serialization

**Consumed By**: 
- [ORM](../../ORM/.context/CONTEXT.md) — References Core domain entities
- [Infrastructure](../../Infrastructure/.context/CONTEXT.md) — Implements Core service interfaces
- [DependencyInjection](../../DependencyInjection/.context/CONTEXT.md) — Registers Core services
- [API](../../API/.context/CONTEXT.md) — Uses ViewModels and service interfaces
- [Jobs](../../Jobs/.context/CONTEXT.md) — Uses polling agent interfaces
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Key Design Decisions

1. **Why Factories Instead of Constructors?**
   - Constructors in domain entities are simple (basic property assignment)
   - Factories handle complex creation logic (validation, defaults, computed properties, cross-entity relationships)
   - Factories centralize mapping logic (ViewModel ↔ Domain), making refactoring easier
   - Example: `IInvoiceFactory.CreateDomain()` maps 15 ViewModel properties → Invoice + InvoiceItems + PaymentItems

2. **Why Separate Validators from Services?**
   - Validators are reusable across multiple services/controllers
   - Validation happens early in request pipeline (before service layer)
   - FluentValidation composition: complex ViewModels compose simpler validators
   - Example: `UserEditModelValidator` composes `UserLoginEditModelValidator` + `PersonEditModelValidator`

3. **Repository Pattern Without Stored Procedures**
   - All database operations via LINQ (no raw SQL or SPs in Core layer)
   - Complex queries built using `IQueryable<T>` composition
   - Business logic in C# services, not database procedures
   - Trade-off: More flexible but potentially less performant for complex aggregations

4. **Soft Delete Everywhere**
   - All entities support `IsDeleted` flag (enforced by DomainBase inheritance)
   - Hard deletes NEVER occur (audit trail requirement)
   - Repository<T>.Table auto-filters `WHERE IsDeleted = false`
   - Reports can opt-in to include deleted records via `TableIncludingDeleted`

5. **Factory Method Naming Convention**
   - `CreateDomain(ViewModel)` — ViewModel → Domain entity
   - `CreateViewModel(Domain)` — Domain entity → ViewModel
   - `CreateNew(...)` — Factory method for new entities with defaults
   - `CreateFrom(...)` — Factory method for entity transformation

### Common Gotchas

1. **IsNew Flag Must Be Set**: When creating new entities, set `IsNew = true` BEFORE saving, otherwise audit fields (DateCreated/CreatedBy) won't populate.

2. **ViewModels != Domain Entities**: Never return domain entities directly from controllers. Always map through factory's `CreateViewModel()` to prevent over-posting and EF tracking issues.

3. **Service Composition Complexity**: Some services inject 10+ dependencies (factories, validators, other services). Consider facade pattern if service becomes unwieldy.

4. **Validation Happens in Controller Layer**: FluentValidation integrated with MVC ModelState. Service layer assumes input is already validated.

5. **Circular Dependencies Possible**: With 15 modules cross-referencing each other, circular dependencies can occur. Use interfaces and delayed resolution via DI container.
<!-- END CUSTOM SECTION -->
