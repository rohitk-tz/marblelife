<!-- AUTO-GENERATED: Header -->
# ORM (Object-Relational Mapping)
> Entity Framework 6 database context for MySQL with soft deletes and automatic audit tracking
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The ORM module provides the data access layer for the entire MarbleLife/Makalu application. It uses **Entity Framework 6** with **MySQL** to map C# domain entities to database tables. The central `MakaluDbContext` class manages 100+ entity types across billing, sales, customer management, and more.

**Key Features:**
- 🔄 **Soft Deletes**: Records are never physically deleted - they're marked with `IsDeleted = 1`
- 📝 **Automatic Audit Tracking**: Every create/modify operation automatically logs user and timestamp
- 🔗 **Complex Relationships**: Manages many-to-many relationships with explicit junction tables
- 🛡️ **Type Safety**: All entities inherit from `DomainBase` for consistent behavior
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Setup
The DbContext requires a connection string in your configuration file:

```xml
<connectionStrings>
    <add name="ConnectionString" 
         connectionString="Server=localhost;Database=makalu;Uid=root;Pwd=yourpassword;" 
         providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

### Basic CRUD Operations

```csharp
using ORM;
using Core.Users.Domain;

// Create a new context instance (typically per request)
using (var context = new MakaluDbContext())
{
    // CREATE - Add a new person
    var person = new Person
    {
        FirstName = "John",
        LastName = "Doe",
        IsNew = true  // Mark as new for audit metadata
    };
    
    context.Person.Add(person);
    context.SaveChanges();  // Auto-populates CreatedBy, DateCreated
    
    // READ - Query entities
    var activePeople = context.Person
        .Where(p => !p.IsDeleted)  // Soft-deleted records are auto-filtered
        .ToList();
    
    // UPDATE - Modify entity
    person.FirstName = "Jane";
    context.SaveChanges();  // Auto-populates ModifiedBy, DateModified
    
    // DELETE - Soft delete (record remains in DB)
    context.Person.Remove(person);
    context.SaveChanges();  // Executes: UPDATE Person SET IsDeleted = 1
}
```

### Working with Related Entities

```csharp
using (var context = new MakaluDbContext())
{
    // Many-to-many: Person with multiple addresses
    var person = context.Person.Include(p => p.Addresses)
        .FirstOrDefault(p => p.Id == 123);
    
    var newAddress = new Address
    {
        Street = "123 Main St",
        City = context.City.Find(1),
        IsNew = true
    };
    
    person.Addresses.Add(newAddress);
    context.SaveChanges();
    // Junction table PersonAddress is auto-managed by EF
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## 📚 Key DbSets

| DbSet | Description |
|-------|-------------|
| `Person` | User profiles and contact information |
| `Organization` | Companies, franchises, and business entities |
| `Franchisee` | Franchise-specific data and relationships |
| `Customer` | Customer profiles and segmentation |
| `Invoice` | Billing invoices and line items |
| `Payment` | Payment records (credit card, eCheck, check) |
| `Job` | Work orders, estimates, and job tracking |
| `NotificationQueue` | Email notification queue and templates |
| `CustomerFeedbackResponse` | Customer reviews and ratings |
| `FranchiseeSales` | Sales data uploads and reporting |

**See [AI-CONTEXT.md](./AI-CONTEXT.md) for the complete list of 100+ entities.**
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Configuration -->
## ⚙️ Configuration

### Required Dependencies
```xml
<!-- packages.config -->
<package id="EntityFramework" version="6.x" />
<package id="MySql.Data.Entity" version="6.x" />
```

### Dependency Injection Requirements
The DbContext depends on:
- **`IClock`**: For consistent UTC timestamps (injectable for testing)
- **`ISessionContext`**: For current user identification in audit logs

These must be registered in your IoC container at application startup.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## 🔧 Troubleshooting

### Common Issues

**1. "Object reference not set" on SaveChanges()**
- **Cause**: `SessionContext` or `IClock` not registered in DI container
- **Fix**: Ensure `ApplicationManager.DependencyInjection` is initialized before creating DbContext

**2. Soft-deleted records still appearing in queries**
- **Cause**: Querying with `DbSet.Find()` bypasses EF filters
- **Fix**: Use `.Where(x => !x.IsDeleted)` or LINQ queries instead of `Find()`

**3. "The entity type X is not part of the model"**
- **Cause**: Missing `DbSet<X>` property in MakaluDbContext
- **Fix**: Add `public virtual DbSet<YourEntity> YourEntities { get; set; }`

**4. Many-to-many relationships not saving**
- **Cause**: Junction table not properly configured in `OnModelCreating`
- **Fix**: Check that both entity navigation properties are populated before SaveChanges

### Performance Tips
- **Dispose contexts promptly**: Use `using` statements to avoid connection leaks
- **Batch operations**: SaveChanges() once after multiple adds/updates instead of per operation
- **Eager vs Lazy Loading**: Use `.Include()` for related entities to avoid N+1 queries
- **Soft delete archival**: Consider periodically moving old soft-deleted records to an archive table
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Related Documentation -->
## 📖 Related Documentation

- **[DatabaseDeploy](../DatabaseDeploy/README.md)** - Database migration scripts
- **[Core](../Core/README.md)** - Domain entity definitions
- **[DependencyInjection](../DependencyInjection/README.md)** - IoC container setup
<!-- END CUSTOM SECTION -->
