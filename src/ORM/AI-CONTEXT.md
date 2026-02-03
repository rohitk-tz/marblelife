<!-- AUTO-GENERATED: Header -->
# ORM Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:07:35+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module provides the **Object-Relational Mapping (ORM) layer** for the entire application using Entity Framework 6 with MySQL. The `MakaluDbContext` class serves as the central database context, managing entity mappings, soft deletes, audit metadata, and complex many-to-many relationships across all business domains (Billing, Organizations, Sales, Users, Notifications, Jobs, Reviews, etc.).

### Design Patterns
- **Unit of Work**: `MakaluDbContext` implements the Unit of Work pattern through Entity Framework's `DbContext`
- **Repository Pattern**: Consumers access entities through `DbSet<T>` properties
- **Soft Delete Pattern**: Overrides `SaveChanges()` to intercept deletions and mark records with `IsDeleted = 1` instead of physical deletion
- **Audit Metadata Injection**: Automatically populates `DataRecorderMetaData` (Created/Modified dates and users) on all entities
- **Table Per Type (TPT)**: Uses discriminator mapping with `IsDeleted` filtering for inheritance hierarchies
- **Static Caching**: Uses `_mappingCache` to cache entity metadata for performance

### Data Flow
1. **Entity Changes** → `ChangeTracker.Entries()` detects modifications
2. **Audit Injection** → `SetDataRecorderMetaData()` automatically sets CreatedBy/ModifiedBy/Timestamps
3. **Soft Delete Interception** → Entities marked for deletion are converted to UPDATE statements
4. **Persistence** → `base.SaveChanges()` commits to MySQL database
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

> **Note**: This module exposes 100+ entity DbSets. Below are the critical infrastructure types.

### Core Infrastructure Types

```csharp
// Base class for all domain entities - provides soft delete and new entity tracking
public abstract class DomainBase
{
    public virtual long Id { get; set; }
    
    [NotMapped]
    public bool IsNew { get; set; }
    
    [NotMapped]
    public bool IsDeleted { get; set; }  // Soft delete flag - NOT persisted to DB
}

// Audit metadata automatically tracked on all entities
public class DataRecorderMetaData
{
    public long Id { get; set; }
    public DateTime DateCreated { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? DateModified { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsNew { get; set; }
}
```

### Entity Mapping Cache
```csharp
private static Dictionary<Type, EntitySetBase> _mappingCache;
// Purpose: Caches EF metadata lookups for GetTableName() performance
// Thread Safety: ⚠️ NOT thread-safe - relies on single-threaded DbContext usage
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### `MakaluDbContext() : base(DbConnection.DbContextConnectionAttribute)`
- **Input**: None (uses connection string from config key "ConnectionString")
- **Output**: Initialized DbContext instance
- **Behavior**: Configures MySQL-specific Entity Framework provider

### `SaveChanges() : int`
- **Input**: None
- **Output**: Number of affected records
- **Behavior**: 
  - Resolves current user from `ISessionContext` (defaults to userId=1 if none)
  - Injects audit metadata (CreatedBy/ModifiedBy/Timestamps) via `SetDataRecorderMetaData()`
  - Intercepts DELETE operations and executes soft delete via `SoftDelete()`
  - **Side Effects**: 
    - Sets `IsDeleted = 1` on deleted records (UPDATE instead of DELETE)
    - Auto-populates `DataRecorderMetaData` on new/modified entities
  - **Threading**: ⚠️ Not thread-safe - DbContext is designed for single-threaded use

### `OnModelCreating(DbModelBuilder modelBuilder) : void` (Protected)
- **Behavior**:
  - Configures many-to-many relationships with explicit junction tables:
    - `CityZip` (City ↔ Zip)
    - `OrganizationAddress`, `OrganizationPhone` (Organization ↔ Address/Phone)
    - `PersonAddress`, `PersonPhone` (Person ↔ Address/Phone)
  - Configures required one-to-one relationships (NotificationEmail ↔ NotificationQueue)
  - **Critical**: Dynamically applies soft delete filter to ALL entities inheriting from `DomainBase` using reflection
  - Removes pluralizing table name convention (table names match entity names exactly)

### `SoftDelete(DbEntityEntry entry) : void` (Private)
- **Input**: Entity marked for deletion
- **Output**: Void
- **Behavior**:
  - Constructs raw SQL: `UPDATE {TableName} SET IsDeleted = 1 WHERE {PrimaryKeys}`
  - Executes via `Database.ExecuteSqlCommand()`
  - Sets entry state to `Detached` to prevent actual EF deletion
  - **Security Note**: ⚠️ Uses string interpolation for SQL - validated through EF metadata (not user input)

### Helper Methods
- `GetTableName(Type) : string` - Retrieves physical table name from EF metadata
- `GetPrimaryKeyName(Type) : ReadOnlyMetadataCollection<EdmMember>` - Gets primary key columns
- `GetEntitySet(Type) : EntitySetBase` - Cached metadata lookup
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Dependencies
- **[Core](../Core/AI-CONTEXT.md)** - All domain entities (Person, Organization, Franchisee, Invoice, etc.)
  - `Core.Application` - `IClock`, `ISessionContext` for audit metadata
  - `Core.Application.Domain` - Base domain types
  - `Core.Billing.Domain` - Payment and invoicing entities
  - `Core.Organizations.Domain` - Organization hierarchy
  - `Core.Sales.Domain` - Sales tracking entities
  - `Core.Users.Domain` - User authentication entities
  - `Core.Notification.Domain` - Email notification queue
  - `Core.Review.Domain` - Customer review system
  - `Core.Scheduler.Domain` - Job scheduling entities
  - `Core.ToDo.Domain` - Task management

### External Dependencies
- **Entity Framework 6** (`System.Data.Entity`) - ORM framework
- **MySQL.Data.Entity** - MySQL provider for EF6
- **System.ComponentModel.DataAnnotations** - Schema annotations

### Configuration Dependencies
- **Connection String**: Expects `ConnectionString` key in configuration
- **Dependency Injection**: Requires `ApplicationManager.DependencyInjection` to resolve `IClock` and `ISessionContext`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Database Schema -->
## 🗄️ Database Schema Strategy

### Entity Organization
The DbContext exposes **100+ DbSets** organized by domain:

#### Core Entities
- **Users**: `Person`, `UserLogin`, `UserLog`, `OrganizationRoleUser`, `Role`
- **Organizations**: `Organization`, `Franchisee`, `SalesRep`
- **Geography**: `Country`, `State`, `City`, `Zip`, `County`, `Address`

#### Billing & Payments
- **Invoicing**: `Invoice`, `InvoiceItem`, `FranchiseeInvoice`, `LateFeeInvoiceItem`, `InterestRateInvoiceItem`
- **Payments**: `Payment`, `PaymentItem`, `InvoicePayment`
- **Payment Methods**: `ChargeCard`, `ECheck`, `Check`, `PaymentInstrument`
- **Fee Management**: `FeeProfile`, `LateFee`, `RoyaltyFeeSlabs`

#### Sales & Customers
- **Customers**: `Customer`, `CustomerEmail`, `CustomerClass` (Marketing segmentation)
- **Sales Data**: `FranchiseeSales`, `SalesDataUpload`, `AnnualSalesDataUpload`
- **Marketing**: `MarketingClass`, `MasterMarketingClass`, `SubClassMarketingClass`
- **Marketing Leads**: `MarketingLeadCallDetail`, `MarketingLeadCallDetailV2-V5` (versioned)

#### Jobs & Estimates
- **Jobs**: `Job`, `JobStatus`, `JobEstimate`, `JobCustomer`, `JobResource`, `JobNote`
- **Estimates**: `EstimateInvoice`, `EstimateInvoiceCustomer`, `EstimateInvoiceService`
- **Pricing**: `PriceEstimateServices`, `TaxRates`, `ShiftCharges`, `ReplacementCharges`

#### Notifications & Communications
- **Email Queue**: `NotificationQueue`, `NotificationEmail`, `NotificationResource`, `NotificationEmailRecipient`
- **Templates**: `EmailTemplate`, `NotificationType`
- **Reminders**: `PaymentMailReminder`, `SalesDataMailReminder`, `WeeklyNotification`

#### Reviews & Feedback
- **Customer Reviews**: `CustomerFeedbackResponse`, `CustomerFeedbackRequest`, `CustomerReviewSystemRecord`
- **Review Push**: `ReviewPushAPILocaltion`, `ReviewPushCustomerFeedback`

#### Audit & Tracking
- **Entity Audits**: `AuditInvoice`, `AuditPayment`, `AuditCustomer`, `AuditAddress`
- **System Logs**: `SystemAuditRecord`, `CustomerLog`, `UserLog`, `DebuggerLogs`
- **History Tracking**: `AddressHistryLog`, `FranchiseeRegistrationHistry`, `Perpetuitydatehistry`

### Soft Delete Implementation
- **Mechanism**: `IsDeleted` column added to base table via Table-Per-Type mapping
- **Filter**: Automatically applied through `HandleDelete<T>()` method
  - Uses `.Map(m => m.Requires("IsDeleted").HasValue(false))`
  - Makes deleted records invisible to EF queries automatically
- **Scope**: Applied to ALL entities inheriting from `DomainBase` via reflection
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Critical Behaviors -->
## ⚠️ Critical Behaviors & Edge Cases

### Soft Delete Caveat
- **SQL Injection Safety**: The `SoftDelete()` method uses string.Format() but values come from EF metadata, NOT user input
- **Multi-Column Primary Keys**: Correctly handles composite keys via `GetPrimaryKeyName()`
- **Detached Entities**: After soft delete, entity is detached - further modifications won't be tracked

### Audit Metadata Edge Cases
- **New Entity Detection**: Relies on `DomainBase.IsNew` flag (NOT EF's EntityState)
- **Current User Fallback**: Uses userId=1 if no session context available (system operations)
- **Clock Dependency**: Timestamps use `IClock.UtcNow` (testable abstraction, not `DateTime.UtcNow`)

### Performance Considerations
- **Mapping Cache**: `_mappingCache` dictionary avoids repeated metadata reflection
- **Change Tracker Enumeration**: Called twice in `SaveChanges()` - once before metadata injection, once after
- **Dynamic Method Invocation**: `HandleDelete()` uses reflection to apply soft delete pattern - one-time cost during context initialization

### Thread Safety
⚠️ **DbContext is NOT thread-safe** - designed for single-threaded, short-lived usage per request
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Historical Context
- Multiple `MarketingLeadCallDetailV2-V5` versions suggest iterative schema evolution without breaking changes
- `UpgradeLog*.htm` files in parent directory indicate EF migrations history

### Testing Considerations
- Mock `IClock` for deterministic timestamp testing
- Mock `ISessionContext` to control audit user assignment
- Use in-memory database or test doubles for `MakaluDbContext` in unit tests

### Migration Strategy
- Soft deletes enable data recovery and audit compliance
- Consider periodic archival of soft-deleted records for performance
- Review `DatabaseDeploy` folder for migration scripts
<!-- END CUSTOM SECTION -->
