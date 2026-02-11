<!-- AUTO-GENERATED: Header -->
# ORM
> Entity Framework 6 database context for MarbleLife application with MySQL backend
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The ORM module is your gateway to the database. Think of it as a translator that converts your C# objects into MySQL database operations. Instead of writing SQL by hand, you work with strongly-typed C# classes (like `Customer`, `Invoice`, `Franchisee`) and Entity Framework translates your LINQ queries into SQL.

**Why this module exists**: It centralizes all database configuration in one place (`MakaluDbContext`), ensures every entity follows the same audit and soft-delete rules, and provides a type-safe way to query data without raw SQL strings.

**Key Features**:
- **Automatic Audit Tracking**: Every create/update automatically records who did it and when
- **Soft Deletes**: Deleted records stay in database with `IsDeleted=true` flag for audit trail
- **100+ Entity Mappings**: All business entities (customers, invoices, jobs, etc.) registered as DbSets
- **Many-to-Many Relationships**: Complex relationships like Organization<->Address handled via join tables
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
The ORM module is consumed through the Infrastructure layer's Repository and UnitOfWork patterns. You typically don't instantiate `MakaluDbContext` directly.

```csharp
// In your service/controller (via dependency injection)
private readonly IUnitOfWork _unitOfWork;

public MyService(IUnitOfWork unitOfWork)
{
    _unitOfWork = unitOfWork;
}
```

### Example: Create a new customer
```csharp
public void CreateCustomer(string name, string email, long franchiseeId)
{
    var customer = new Customer
    {
        FirstName = name,
        Email = email,
        FranchiseeId = franchiseeId,
        IsNew = true // Important: Tells audit system this is a new entity
    };
    
    var customerRepo = _unitOfWork.Repository<Customer>();
    customerRepo.Insert(customer);
    _unitOfWork.SaveChanges(); // Audit fields auto-populated here
}
```

### Example: Query with filtering
```csharp
public List<Customer> GetActiveCustomers(long franchiseeId)
{
    var customerRepo = _unitOfWork.Repository<Customer>();
    
    // Repository.Table automatically filters out soft-deleted records
    return customerRepo.Table
        .Where(c => c.FranchiseeId == franchiseeId && c.IsActive)
        .OrderBy(c => c.LastName)
        .ToList();
}
```

### Example: Update with related entities
```csharp
public void UpdateOrganizationWithAddress(long orgId, Address newAddress)
{
    var orgRepo = _unitOfWork.Repository<Organization>();
    
    var organization = orgRepo.Table
        .Include(o => o.Address) // Eager load addresses
        .FirstOrDefault(o => o.Id == orgId);
    
    if (organization != null)
    {
        organization.Address.Add(newAddress);
        orgRepo.Update(organization);
        _unitOfWork.SaveChanges(); // ModifiedBy/DateModified auto-set
    }
}
```

### Example: Soft delete
```csharp
public void DeleteInvoice(long invoiceId)
{
    var invoiceRepo = _unitOfWork.Repository<Invoice>();
    invoiceRepo.Delete(invoiceId); // Marks as deleted
    _unitOfWork.SaveChanges(); // Executes: UPDATE Invoice SET IsDeleted=1 WHERE Id=123
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `MakaluDbContext()` | Constructor - initializes DbContext with "ConnectionString" from config |
| `SaveChanges()` | Persists changes to database with automatic audit metadata and soft delete conversion |
| `OnModelCreating()` | Configures entity relationships via Fluent API (called by EF during first use) |
| `DbSet<Person>`, `DbSet<Organization>`, etc. | 100+ entity set properties for querying and modifying entities |
| `GetTableName(Type)` | Returns database table name for given entity type (used for dynamic SQL) |
| `GetPrimaryKeyName(Type)` | Returns primary key column name(s) for given entity type |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Problem: "Object reference not set to an instance" when accessing navigation property
**Solution**: Navigation properties are lazy-loaded. Either use `.Include(x => x.NavigationProperty)` in your query, or ensure the DbContext is still active when accessing the property.

```csharp
// Bad - navigation property not loaded
var customer = repo.Get(customerId);
var addressCount = customer.Addresses.Count; // NullReferenceException

// Good - explicitly include
var customer = repo.IncludeMultiple(c => c.Addresses, c => c.Phones)
    .FirstOrDefault(c => c.Id == customerId);
var addressCount = customer.Addresses.Count; // Works!
```

### Problem: Audit fields (DateCreated, CreatedBy) are NULL after insert
**Solution**: Set `entity.IsNew = true` before calling `Insert()`. The Repository<T>.Insert() method does this automatically, but manual additions to DbSet don't.

```csharp
// Wrong
var customer = new Customer { Name = "John" };
dbContext.Customer.Add(customer); // IsNew still false
dbContext.SaveChanges(); // DateCreated will be NULL

// Right
var customer = new Customer { Name = "John", IsNew = true };
customerRepo.Insert(customer); // Repository sets IsNew = true
unitOfWork.SaveChanges(); // DateCreated populated correctly
```

### Problem: Soft-deleted records appearing in queries
**Solution**: Use `Repository<T>.Table` instead of `DbContext.DbSet<T>` directly. Repository applies the `!IsDeleted` filter automatically.

```csharp
// Wrong - includes deleted records
var allCustomers = dbContext.Customer.ToList();

// Right - excludes deleted records
var activeCustomers = customerRepo.Table.ToList();
```

### Problem: "A connection was not closed" or connection pool exhausted
**Solution**: Ensure UnitOfWork is disposed properly. Use `using` statement or let ASP.NET MVC's request lifecycle dispose it via DI container.

```csharp
// Wrong - connection leak
var uow = DependencyInjection.Resolve<IUnitOfWork>();
uow.Setup();
uow.SaveChanges();
// Forgot to dispose!

// Right - automatic cleanup
using (var uow = DependencyInjection.Resolve<IUnitOfWork>())
{
    uow.Setup();
    uow.SaveChanges();
} // Disposes DbContext and closes connection
```
<!-- END CUSTOM SECTION -->
