# ORM Module - AI Context

## Purpose

The **ORM** module is the Object-Relational Mapping layer for the MarbleLife application, built on **Entity Framework 6** with **MySQL**. It defines the database context (`MakaluDbContext`), entity mappings, database operations, and metadata utilities. This module serves as the bridge between domain entities (defined in Core) and the physical MySQL database, providing LINQ-based query capabilities and change tracking.

## Architecture Overview

The ORM module uses **Entity Framework 6 Code-First** approach with a MySQL database provider. It implements soft delete patterns, automatic audit tracking, cascade relationship management, and runtime metadata inspection for dynamic operations.

### Key Architectural Patterns

1. **DbContext Pattern**: Central database context managing entity sets
2. **Code-First Approach**: Domain entities define database schema
3. **Soft Delete Pattern**: Logical deletes via `IsDeleted` flag
4. **Audit Trail Pattern**: Automatic tracking of creation/modification metadata
5. **Convention-Based Mapping**: Entities inherit from `DomainBase`, conventions applied via fluent API
6. **Cascade Attribute Pattern**: Custom `[CascadeEntity]` attribute for relationship management

## Project Structure

```
ORM/
├── MakaluDbContext.cs       # Main Entity Framework DbContext
├── DomainBase.cs            # Base class for all entities
├── CascadeEntityAttribute.cs # Marks properties for cascade operations
├── Migrations/              # EF migration files (if any)
├── Properties/              # Assembly metadata
├── packages.config          # NuGet dependencies
└── ORM.csproj              # Project configuration
```

## MakaluDbContext - Entity Framework DbContext

### Configuration

**Database Provider**: MySQL 6.9.9  
**Entity Framework**: Version 6.1.3  
**Configuration Class**: `MySqlEFConfiguration`  
**Connection String**: Defined in `DbConnection.DbContextConnectionAttribute`

### DbSets (60+ Entity Sets)

#### Organizations & Users
```csharp
public DbSet<Franchisee> Franchisee { get; set; }
public DbSet<Organization> Organization { get; set; }
public DbSet<OrganizationRoleUser> OrganizationRoleUser { get; set; }
public DbSet<Role> Role { get; set; }
public DbSet<Person> Person { get; set; }
public DbSet<UserLogin> UserLogin { get; set; }
public DbSet<UserLog> UserLog { get; set; }
public DbSet<SalesRep> SalesRep { get; set; }
```

#### Billing & Financial
```csharp
public DbSet<Invoice> Invoice { get; set; }
public DbSet<InvoiceItem> InvoiceItem { get; set; }
public DbSet<Payment> Payment { get; set; }
public DbSet<ChargeCard> ChargeCard { get; set; }
public DbSet<ECheck> ECheck { get; set; }
public DbSet<LateFee> LateFee { get; set; }
public DbSet<CreditCardLog> CreditCardLog { get; set; }
public DbSet<CurrencyRate> CurrencyRate { get; set; }
```

#### Geographic Data
```csharp
public DbSet<Address> Address { get; set; }
public DbSet<City> City { get; set; }
public DbSet<State> State { get; set; }
public DbSet<Zip> Zip { get; set; }
public DbSet<County> County { get; set; }
public DbSet<Country> Country { get; set; }
public DbSet<Phone> Phone { get; set; }
```

#### Sales & Customer Management
```csharp
public DbSet<Customer> Customer { get; set; }
public DbSet<CustomerEmail> CustomerEmail { get; set; }
public DbSet<MarketingClass> MarketingClass { get; set; }
public DbSet<MarketingLead> MarketingLead { get; set; }
public DbSet<LeadSource> LeadSource { get; set; }
```

#### Scheduler & Jobs
```csharp
public DbSet<Job> Job { get; set; }
public DbSet<JobEstimate> JobEstimate { get; set; }
public DbSet<JobNote> JobNote { get; set; }
public DbSet<Meeting> Meeting { get; set; }
public DbSet<JobService> JobService { get; set; }
public DbSet<JobServiceItem> JobServiceItem { get; set; }
```

#### Review System
```csharp
public DbSet<CustomerFeedbackRequest> CustomerFeedbackRequest { get; set; }
public DbSet<CustomerFeedbackResponse> CustomerFeedbackResponse { get; set; }
public DbSet<CustomerReviewTracking> CustomerReviewTracking { get; set; }
public DbSet<ReviewSource> ReviewSource { get; set; }
```

#### Audit & Logging
```csharp
public DbSet<AuditInvoice> AuditInvoice { get; set; }
public DbSet<AuditPayment> AuditPayment { get; set; }
public DbSet<AuditCustomer> AuditCustomer { get; set; }
public DbSet<SystemAuditRecord> SystemAuditRecord { get; set; }
public DbSet<EmailQueue> EmailQueue { get; set; }
```

### OnModelCreating - Fluent API Configuration

**Purpose**: Configures entity relationships, conventions, and soft delete filters.

#### Many-to-Many Relationships
```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    // City <-> Zip many-to-many
    modelBuilder.Entity<City>()
        .HasMany(c => c.Zip)
        .WithMany(z => z.City)
        .Map(m => {
            m.ToTable("CityZip");
            m.MapLeftKey("CityId");
            m.MapRightKey("ZipId");
        });
    
    // Organization <-> Address many-to-many
    modelBuilder.Entity<Organization>()
        .HasMany(o => o.Address)
        .WithMany(a => a.Organization)
        .Map(m => {
            m.ToTable("OrganizationAddress");
            m.MapLeftKey("OrganizationId");
            m.MapRightKey("AddressId");
        });
    
    // Organization <-> Phone many-to-many
    modelBuilder.Entity<Organization>()
        .HasMany(o => o.Phone)
        .WithMany(p => p.Organization)
        .Map(m => {
            m.ToTable("OrganizationPhone");
            m.MapLeftKey("OrganizationId");
            m.MapRightKey("PhoneId");
        });
    
    // Person <-> Address many-to-many
    modelBuilder.Entity<Person>()
        .HasMany(p => p.Address)
        .WithMany(a => a.Person)
        .Map(m => {
            m.ToTable("PersonAddress");
            m.MapLeftKey("PersonId");
            m.MapRightKey("AddressId");
        });
    
    // Person <-> Phone many-to-many
    modelBuilder.Entity<Person>()
        .HasMany(p => p.Phone)
        .WithMany(ph => ph.Person)
        .Map(m => {
            m.ToTable("PersonPhone");
            m.MapLeftKey("PersonId");
            m.MapRightKey("PhoneId");
        });
}
```

#### Convention Removal
```csharp
// Disable automatic table name pluralization
modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
```

#### Soft Delete Filter
```csharp
// Automatically filter out soft-deleted entities in all queries
// (Implementation via SaveChanges override - see below)
```

### SaveChanges Override - Audit & Soft Delete

**Purpose**: Intercepts save operations to apply audit metadata and convert hard deletes to soft deletes.

```csharp
public override int SaveChanges()
{
    var addedEntities = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added)
        .ToList();
    
    var modifiedEntities = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Modified)
        .ToList();
    
    var deletedEntities = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Deleted)
        .ToList();
    
    // Set audit metadata
    foreach (var entry in addedEntities)
    {
        if (entry.Entity is DomainBase entity && entity.DataRecorderMetaData != null)
        {
            entity.DataRecorderMetaData.CreatedBy = _sessionContext.UserSession?.UserId ?? 0;
            entity.DataRecorderMetaData.DateCreated = _clock.UtcNow;
        }
    }
    
    foreach (var entry in modifiedEntities)
    {
        if (entry.Entity is DomainBase entity && entity.DataRecorderMetaData != null)
        {
            entity.DataRecorderMetaData.ModifiedBy = _sessionContext.UserSession?.UserId ?? 0;
            entity.DataRecorderMetaData.DateModified = _clock.UtcNow;
        }
    }
    
    // Convert hard deletes to soft deletes
    foreach (var entry in deletedEntities)
    {
        if (entry.Entity is DomainBase entity)
        {
            entry.State = EntityState.Detached; // Cancel hard delete
            entity.IsDeleted = true;
            entity.DataRecorderMetaData.ModifiedBy = _sessionContext.UserSession?.UserId ?? 0;
            entity.DataRecorderMetaData.DateModified = _clock.UtcNow;
            entry.State = EntityState.Modified; // Mark as update instead
        }
    }
    
    return base.SaveChanges();
}
```

**Key Features**:
- **Automatic Audit Fields**: `CreatedBy`, `DateCreated`, `ModifiedBy`, `DateModified`
- **Soft Delete Implementation**: Deleted entities marked `IsDeleted = true` instead of physical removal
- **User Tracking**: Current user from `ISessionContext` recorded in audit fields
- **UTC Timestamps**: All dates stored in UTC via `IClock.UtcNow`

### Metadata Utilities

**Purpose**: Runtime reflection of EF metadata for dynamic table/column operations.

```csharp
private static readonly Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

public string GetTableName(Type type)
{
    var entitySet = GetEntitySet(type);
    return entitySet.Table ?? entitySet.Name;
}

public string GetPrimaryKeyName(Type type)
{
    var entitySet = GetEntitySet(type);
    return entitySet.ElementType.KeyMembers[0].Name;
}

private EntitySetBase GetEntitySet(Type type)
{
    if (!_mappingCache.ContainsKey(type))
    {
        var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;
        var entityType = metadata.GetItems<EntityType>(DataSpace.OSpace)
            .FirstOrDefault(e => e.Name == type.Name);
        
        if (entityType != null)
        {
            var entitySet = metadata.GetItems<EntityContainer>(DataSpace.CSpace)
                .SelectMany(c => c.EntitySets)
                .FirstOrDefault(s => s.ElementType.Name == entityType.Name);
            
            _mappingCache[type] = entitySet;
        }
    }
    
    return _mappingCache[type];
}
```

**Use Cases**:
- Dynamic query generation
- Generic repository operations requiring table names
- Metadata-driven CRUD operations
- Database schema introspection

## DomainBase - Entity Base Class

### Purpose

All entity classes inherit from `DomainBase`, providing common infrastructure for identity, soft deletes, and tracking.

### Properties

```csharp
public abstract class DomainBase
{
    // Primary Key
    public long Id { get; set; }
    
    // Soft Delete Flag (not mapped to DB directly - handled in SaveChanges)
    [NotMapped]
    public bool IsDeleted { get; set; }
    
    // New Entity Tracking (not persisted)
    [NotMapped]
    public bool IsNew { get; set; }
    
    // Audit Metadata
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

### DataRecorderMetaData

**Purpose**: Tracks who created/modified entity and when.

```csharp
public class DataRecorderMetaData
{
    public long CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? DateModified { get; set; }
}
```

**Automatic Population**: Set by `MakaluDbContext.SaveChanges()` override.

## CascadeEntityAttribute

### Purpose

Marks properties that should be cascade-saved or cascade-deleted when parent entity is saved/deleted.

### Attribute Properties

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class CascadeEntityAttribute : Attribute
{
    // Indicates if property is a collection (ICollection<T>)
    public bool IsCollection { get; set; }
    
    // Additional configuration options
    public CascadeDeleteBehavior DeleteBehavior { get; set; } = CascadeDeleteBehavior.SetNull;
}
```

### Usage Example

```csharp
public class Organization : DomainBase
{
    public string Name { get; set; }
    
    // Cascade save/delete for address collection
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<Address> Addresses { get; set; }
    
    // Cascade save for single entity
    [CascadeEntity]
    public virtual DataRecorderMetaData MetaData { get; set; }
    
    // NOT cascaded - just foreign key reference
    public long? ParentOrganizationId { get; set; }
    public virtual Organization ParentOrganization { get; set; }
}
```

### Cascade Logic (Infrastructure/SaveCascadeHelper)

When `Repository<T>.Update()` is called:
1. Inspect entity properties for `[CascadeEntity]` attribute
2. For collections: Compare existing vs. new items
   - Mark removed items as Deleted
   - Mark new items as Added
   - Mark changed items as Modified
3. For single entities: Update entity state
4. Recursively process child entities

## Database Operations & Query Patterns

### Repository Pattern (from Infrastructure)

The ORM module is consumed by `Repository<T>` in Infrastructure layer:

```csharp
public class Repository<TEntity> : IRepository<TEntity> where TEntity : DomainBase
{
    private readonly MakaluDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    
    public IQueryable<TEntity> Table => _dbSet.Where(e => !e.IsDeleted);
    public IQueryable<TEntity> TableNoTracking => _dbSet.AsNoTracking().Where(e => !e.IsDeleted);
    
    public TEntity Get(long id)
    {
        return _dbSet.Find(id);
    }
    
    public IEnumerable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, int pageSize, int pageNumber)
    {
        return _dbSet
            .Where(e => !e.IsDeleted)
            .Where(predicate)
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
    
    public void Insert(TEntity entity)
    {
        entity.IsNew = true;
        _dbContext.Entry(entity).State = EntityState.Added;
    }
    
    public void Update(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        SaveCascadeHelper.ProcessCascade(_dbContext, entity);
    }
    
    public void Delete(long id)
    {
        var entity = Get(id);
        if (entity != null)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            // SaveChanges will convert to soft delete
        }
    }
}
```

### Common Query Patterns

#### Basic CRUD
```csharp
// Create
var customer = new Customer { Name = "John Doe", Email = "john@example.com" };
repo.Insert(customer);
unitOfWork.SaveChanges();

// Read
var customer = repo.Get(customerId);

// Update
customer.Email = "newemail@example.com";
repo.Update(customer);
unitOfWork.SaveChanges();

// Delete (soft delete)
repo.Delete(customerId);
unitOfWork.SaveChanges();
```

#### LINQ Queries
```csharp
// Filtering
var activeCustomers = repo.Table
    .Where(c => c.IsActive && c.FranchiseeId == franchiseeId)
    .ToList();

// Sorting
var sortedCustomers = repo.Table
    .OrderBy(c => c.Name)
    .ThenByDescending(c => c.DateCreated)
    .ToList();

// Projection
var customerNames = repo.Table
    .Select(c => new { c.Id, c.Name })
    .ToList();

// Aggregation
var totalRevenue = repo.Table
    .Where(c => c.FranchiseeId == franchiseeId)
    .Sum(c => c.TotalSpent);
```

#### Eager Loading
```csharp
// Include related entities to prevent N+1 queries
var customersWithDetails = repo.IncludeMultiple(
    c => c.Address,
    c => c.Phone,
    c => c.Invoices
).Where(c => c.Id == customerId).FirstOrDefault();
```

#### Pagination
```csharp
var pagedCustomers = repo.Fetch(
    c => c.IsActive && c.FranchiseeId == franchiseeId,
    pageSize: 20,
    pageNumber: 2
);
```

#### Async Queries
```csharp
var customers = await repo.TableAsync(c => c.FranchiseeId == franchiseeId)
    .OrderBy(c => c.Name)
    .ToListAsync();
```

## Migration Strategy

### SQL-Based Migrations (Not EF Code-First Migrations)

The project uses **manual SQL scripts** instead of EF Code-First Migrations.

**Migration Management**: Handled by `DatabaseDeploy` project (separate module).

**Why Manual SQL?**
- Direct control over DDL statements
- Better for complex schema changes
- Team familiarity with SQL
- Cross-environment deployment tracking
- No EF migration history table conflicts

### Schema Source

**MySQL Workbench File**: `Schema.mwb` - Visual database design  
**Generated SQL**: `DatabaseDeploy/Schema/003.Tables.sql` - Table definitions  
**Modifications**: `DatabaseDeploy/Modifications/*.sql` - Sequential changes

## Integration with Infrastructure

### UnitOfWork Pattern

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly MakaluDbContext _dbContext;
    private DbTransaction _transaction;
    private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
    
    public void Setup()
    {
        if (_dbContext.Database.Connection.State != ConnectionState.Open)
        {
            _dbContext.Database.Connection.Open();
            _transaction = _dbContext.Database.Connection.BeginTransaction(IsolationLevel.ReadCommitted);
        }
    }
    
    public IRepository<T> Repository<T>() where T : DomainBase
    {
        if (!_repositories.ContainsKey(typeof(T)))
        {
            _repositories[typeof(T)] = new Repository<T>(_dbContext);
        }
        return (IRepository<T>)_repositories[typeof(T)];
    }
    
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
        _transaction?.Commit();
    }
    
    public void Rollback()
    {
        _transaction?.Rollback();
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext?.Dispose();
    }
}
```

## For AI Agents

### Adding New Entity

1. **Create Entity Class**:
```csharp
public class NewEntity : DomainBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public long FranchiseeId { get; set; }
    
    // Navigation properties
    public virtual Franchisee Franchisee { get; set; }
    
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<RelatedEntity> RelatedItems { get; set; }
}
```

2. **Add DbSet to MakaluDbContext**:
```csharp
public DbSet<NewEntity> NewEntity { get; set; }
```

3. **Create Database Table** (via DatabaseDeploy):
```sql
-- DatabaseDeploy/Modifications/[next-number].create_NewEntity.sql
CREATE TABLE NewEntity (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    FranchiseeId BIGINT NOT NULL,
    IsDeleted BIT(1) DEFAULT 0,
    CreatedBy BIGINT NOT NULL,
    DateCreated DATETIME NOT NULL,
    ModifiedBy BIGINT,
    DateModified DATETIME,
    FOREIGN KEY (FranchiseeId) REFERENCES Franchisee(Id)
);
```

4. **Use in Repository**:
```csharp
var repo = unitOfWork.Repository<NewEntity>();
var entity = repo.Get(id);
```

### Modifying Entity Relationships

**Adding Many-to-Many**:
```csharp
// In MakaluDbContext.OnModelCreating
modelBuilder.Entity<EntityA>()
    .HasMany(a => a.EntityBCollection)
    .WithMany(b => b.EntityACollection)
    .Map(m => {
        m.ToTable("EntityAEntityB");
        m.MapLeftKey("EntityAId");
        m.MapRightKey("EntityBId");
    });
```

**Adding One-to-Many**:
```csharp
// Navigation properties already sufficient
// EF conventions handle foreign key relationships
public class Parent : DomainBase
{
    public virtual ICollection<Child> Children { get; set; }
}

public class Child : DomainBase
{
    public long ParentId { get; set; }
    public virtual Parent Parent { get; set; }
}
```

### Best Practices

- **Always inherit from DomainBase**: Ensures soft delete and audit support
- **Use virtual for navigation properties**: Enables lazy loading
- **Mark cascade relationships**: Use `[CascadeEntity]` for dependent entities
- **Use NoTracking for read-only**: Improves performance for queries
- **Eager load related entities**: Prevent N+1 query problems
- **Handle concurrency**: Use row versioning for optimistic concurrency control

## For Human Developers

### Performance Optimization

#### Use NoTracking for Reports
```csharp
var report = dbContext.Invoice
    .AsNoTracking()
    .Where(i => i.FranchiseeId == franchiseeId)
    .Select(i => new ReportDto
    {
        InvoiceNumber = i.InvoiceNumber,
        Total = i.Total
    })
    .ToList();
```

#### Projection vs. Full Entity
```csharp
// Bad - loads entire entity
var customers = dbContext.Customer.ToList();
var names = customers.Select(c => c.Name).ToList();

// Good - projects only needed columns
var names = dbContext.Customer
    .Select(c => c.Name)
    .ToList();
```

#### Batch Operations
```csharp
// Batch insert
foreach (var item in items)
{
    dbContext.Item.Add(item);
}
dbContext.SaveChanges(); // Single round-trip

// Avoid: SaveChanges() in loop
```

### Connection String Configuration

```xml
<connectionStrings>
  <add name="MakaluConnection"
       connectionString="Server=localhost;Database=Makalu;Uid=root;Pwd=password;CharSet=utf8;Allow User Variables=True;Convert Zero Datetime=True;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

**Key Parameters**:
- `CharSet=utf8` - UTF-8 encoding support
- `Allow User Variables=True` - Enables user-defined variables in SQL
- `Convert Zero Datetime=True` - Handles MySQL zero dates

## Dependencies

- **Entity Framework 6.1.3** - ORM framework
- **MySQL.Data.EntityFramework 6.9.9** - MySQL provider for EF6
- **MySql.Data 6.9.9** - MySQL ADO.NET driver

## Troubleshooting

### Common Issues

**Connection Timeout**:
- Add `Connection Timeout=300;` to connection string
- Check MySQL server status
- Verify network connectivity

**Lazy Loading Errors**:
- Mark navigation properties as `virtual`
- Enable lazy loading in DbContext constructor: `Configuration.LazyLoadingEnabled = true`

**Soft Delete Not Working**:
- Ensure entity inherits from `DomainBase`
- Verify `SaveChanges()` override is executing
- Check `IsDeleted` filter in repository queries

**Migration Conflicts**:
- Use DatabaseDeploy project for schema changes
- Avoid EF Code-First migrations (`Update-Database` commands)
- Coordinate with DBA for production schema changes
