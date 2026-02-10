<!-- AUTO-GENERATED: Header -->
# ORM — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2026-02-10T10:57:49Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Serves as the Object-Relational Mapping bridge between domain entities and MySQL database using Entity Framework 6. Manages database context, entity mappings, automatic audit tracking (CreatedBy/DateCreated/ModifiedBy/DateModified), soft delete pattern (IsDeleted flag), many-to-many relationships via join tables, and metadata reflection utilities for dynamic SQL operations.

### Design Patterns
- **DbContext Pattern**: `MakaluDbContext` as central database gateway managing 100+ entity sets
- **Soft Delete Pattern**: Override `SaveChanges()` to convert `EntityState.Deleted` to `IsDeleted=true` updates
- **Audit Trail Pattern**: Automatic population of `DataRecorderMetaData` on insert/update via change tracker interception
- **Repository Pattern Integration**: Consumed by Infrastructure layer's `Repository<T>` and `UnitOfWork`
- **Metadata Caching**: Dictionary-based cache for `EntitySetBase` lookups to avoid repeated metadata queries

### Data Flow
1. API/Service layer requests `IUnitOfWork` from DI container
2. UnitOfWork creates/reuses `MakaluDbContext` instance
3. Repository<T> wraps `DbSet<T>` from context, applies `.Where(e => !e.IsDeleted)` filter
4. LINQ query executes against MySQL via EF6 query translation
5. On SaveChanges: Audit metadata applied → Deletes converted to soft deletes → Base SaveChanges called
6. Transaction committed/rolled back via UnitOfWork
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### MakaluDbContext
```csharp
[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
public class MakaluDbContext : DbContext
{
    // Connection: Named "ConnectionString" in app.config
    public MakaluDbContext() : base(DbConnection.DbContextConnectionAttribute) { }
    
    // 100+ DbSets including:
    public virtual DbSet<Person> Person { get; set; }
    public virtual DbSet<Organization> Organization { get; set; }
    public virtual DbSet<Franchisee> Franchisee { get; set; }
    public virtual DbSet<Invoice> Invoice { get; set; }
    public virtual DbSet<Customer> Customer { get; set; }
    public virtual DbSet<Job> Job { get; set; }
    // ... see full list in MakaluDbContext.cs lines 234-414
    
    // Overrides SaveChanges to apply audit metadata and soft deletes
    public override int SaveChanges();
    
    // Configures many-to-many relationships via Fluent API
    protected override void OnModelCreating(DbModelBuilder modelBuilder);
    
    // Metadata utilities for dynamic operations
    private string GetTableName(Type type);
    private ReadOnlyMetadataCollection<EdmMember> GetPrimaryKeyName(Type type);
}
```

### DomainBase (from Core module)
```csharp
// All entities inherit from this
public abstract class DomainBase
{
    public long Id { get; set; }
    
    [NotMapped]
    public bool IsDeleted { get; set; } // Soft delete flag
    
    [NotMapped]
    public bool IsNew { get; set; } // Used to determine Created vs Modified audit
    
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
}

public class DataRecorderMetaData
{
    public long Id { get; set; }
    public bool IsNew { get; set; }
    public DateTime DateCreated { get; set; }
    public long? CreatedBy { get; set; } // OrganizationRoleUserId from session
    public DateTime? DateModified { get; set; }
    public long? ModifiedBy { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `MakaluDbContext.SaveChanges()`
- **Input**: None (operates on tracked entities via ChangeTracker)
- **Output**: `int` (number of state entries written to database)
- **Behavior**: 
  - Retrieves `IClock` and `ISessionContext` from DI container
  - For Added entities: Sets `DataRecorderMetaData.DateCreated` and `CreatedBy`
  - For Modified entities: Sets `DataRecorderMetaData.DateModified` and `ModifiedBy`
  - For Deleted entities: Calls `SoftDelete()` to execute `UPDATE {table} SET IsDeleted=1` SQL directly, then detaches entity to prevent hard delete
  - Throws `DbUpdateException` on constraint violations or concurrency conflicts

### `MakaluDbContext.OnModelCreating(DbModelBuilder modelBuilder)`
- **Input**: `DbModelBuilder` from EF6
- **Output**: void (side-effects on modelBuilder)
- **Behavior**:
  - Configures 5 many-to-many relationships: City<->Zip, Organization<->Address, Organization<->Phone, Person<->Address, Person<->Phone
  - Configures NotificationEmail 1:1 with NotificationQueue (shared primary key)
  - Applies soft delete filter to all entities inheriting from DomainBase via `HandleDelete<T>` using `.Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted)`
  - Removes `PluralizingTableNameConvention` to keep table names singular

### `MakaluDbContext.SoftDelete(DbEntityEntry entry)`
- **Input**: `DbEntityEntry` in Deleted state
- **Output**: void
- **Behavior**: Executes raw SQL `UPDATE {TableName} SET IsDeleted = 1 WHERE {PrimaryKeys}={Values}`, then sets entry state to Detached to cancel EF's hard delete operation

### `GetTableName(Type type)`, `GetPrimaryKeyName(Type type)`
- **Input**: Entity CLR type
- **Output**: Table name string or primary key column name(s)
- **Behavior**: Uses `IObjectContextAdapter` to access EF metadata workspace, caches result in static dictionary to avoid repeated metadata queries, throws `ArgumentException` if entity type not found in model
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

- **Internal**: 
  - [Core](../../Core/.context/CONTEXT.md) — Domain entities (DomainBase, all 100+ entity classes across Application/Billing/Organizations/Geo/Sales/Users/Notification/Reports/Review/MarketingLead/Scheduler/ToDo subfolders)
  - Core.Application — `IClock`, `ISessionContext`, `ApplicationManager.DependencyInjection`
- **External**: 
  - `EntityFramework 6.1.3` — ORM framework with LINQ query provider and change tracking
  - `MySql.Data.Entity.EF6 6.9.9` — MySQL provider for EF6 with MySqlEFConfiguration
  - `MySql.Data 6.9.9` — ADO.NET driver for MySQL connectivity
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Key Implementation Details
1. **Soft Delete Mechanism**: Uses direct SQL UPDATE instead of entity state manipulation because EF6's `.Map(m => m.Requires("IsDeleted").HasValue(false))` in OnModelCreating creates a discriminator column that filters queries but doesn't prevent hard deletes during SaveChanges.

2. **Audit Tracking Gotcha**: The `IsNew` flag on DomainBase must be set to true BEFORE adding entity to DbSet, otherwise DateCreated won't populate. Repository<T>.Insert() handles this automatically.

3. **Many-to-Many Relationships**: EF6 requires explicit join table configuration via Fluent API. The tables (CityZip, OrganizationAddress, etc.) exist in database but have no C# class representation.

4. **Metadata Cache**: The `_mappingCache` dictionary is static shared across all DbContext instances. In multi-tenant scenarios, ensure this doesn't leak metadata between databases.

5. **Session Context Resolution**: Uses service locator pattern (`ApplicationManager.DependencyInjection.Resolve<>`) instead of constructor injection because EF6 DbContext instantiation doesn't support DI out-of-box without custom DbConfiguration.

### Common Pitfalls
- **N+1 Queries**: Always use `.Include()` or explicit loading for navigation properties in loops. Repository pattern doesn't automatically eager load.
- **Detached Entities**: Calling `SaveChanges()` on entity fetched from different DbContext instance will fail. Always use UnitOfWork to ensure single context per request.
- **Cascade Delete**: EF6 cascade delete is disabled by soft delete pattern. Must manually mark children as deleted if business logic requires it.
<!-- END CUSTOM SECTION -->
