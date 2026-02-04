<!-- AUTO-GENERATED: Header -->
# Application Domain Entities
> Shared kernel domain models for file management, audit tracking, and reference data
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The `Core.Application.Domain` namespace contains the foundational domain entities that power cross-cutting concerns throughout the Marblelife franchise management system. Think of these entities as the **"DNA"** of the application - they're present in nearly every business operation but don't belong to any specific business module (not Billing, not Scheduler, not Sales).

### What's Inside?

- **📝 Audit Trail** (`DataRecorderMetaData`) - Every business record knows WHO created it and WHEN, WHO modified it last and WHEN. This entity is the system's memory.

- **📁 File Management** (`File`, `Folder`, `ContentType`) - Franchisees upload thousands of documents: invoices, job photos, compliance documents. These entities track metadata (not the bytes themselves).

- **🏷️ Reference Data** (`Lookup`, `LookupType`) - Dropdown lists in the UI (phone types, invoice statuses, service categories) are stored as data, not hard-coded. This enables franchisees to customize their system without code changes.

### Why These Entities Matter

**Traditional Approach** (Hard-coded Enums):
```csharp
public enum InvoiceStatus { Draft, Sent, Paid, Overdue }
```
❌ Adding "Partial Payment" requires code deployment  
❌ Can't translate for international franchises  
❌ Can't disable obsolete values without breaking old invoices

**Marblelife Approach** (Lookup Pattern):
```csharp
var statuses = lookupRepo.Query()
    .Where(l => l.LookupTypeId == LookupTypes.InvoiceStatus)
    .Where(l => l.IsActive)
    .OrderBy(l => l.RelativeOrder);
```
✅ Add "Partial Payment" via SQL insert (zero downtime)  
✅ Translate `Lookup.Name` per franchisee locale  
✅ Set `IsActive = false` to hide from UI, preserve historical FK integrity
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Installation / Setup

These entities are part of the Core library - no separate installation needed. They participate in Entity Framework 6 Code First:

```bash
# Database creation/migration handled by DatabaseDeploy project
# See: src/DatabaseDeploy/AI-CONTEXT.md
```

### Basic Example: Audit Trail

Every time you save a business entity, attach audit metadata:

```csharp
// Creating a new franchisee
var franchisee = new Franchisee {
    Name = "Marblelife of Seattle",
    DataRecorderMetaData = new DataRecorderMetaData(currentUserId)
    // DataRecorderMetaData captures:
    // - CreatedBy = currentUserId
    // - DateCreated = IClock.UtcNow (injected)
};
unitOfWork.GetRepository<Franchisee>().Add(franchisee);
unitOfWork.SaveChanges();

// Later, when updating...
franchisee.Name = "Marblelife of Greater Seattle";
franchisee.DataRecorderMetaData.SetModifiedBy(currentUserId);
// Now also captures:
// - ModifiedBy = currentUserId  
// - DateModified = IClock.UtcNow
unitOfWork.SaveChanges();
```

### Basic Example: File Upload

Uploading an invoice PDF to the system:

```csharp
// Step 1: Client uploads file to API
// Step 2: Service saves physical bytes to filesystem/S3
string physicalPath = fileService.SaveFile(fileModel, MediaLocation.Invoices, "INV_");

// Step 3: Create domain entity to track metadata
var file = new File {
    Name = "Invoice-2023-Q1.pdf",
    Caption = "Q1 2023 Royalty Invoice",
    Size = 245678,
    RelativeLocation = physicalPath,
    MimeType = "application/pdf",
    css = "fa-file-pdf-o", // FontAwesome icon class
    DataRecorderMetaData = new DataRecorderMetaData(uploadUserId)
};

// Step 4: Persist metadata
unitOfWork.GetRepository<File>().Add(file);
unitOfWork.SaveChanges();

// Later retrieval:
var invoice = repository.Get(fileId);
Console.WriteLine($"Uploaded by User #{invoice.DataRecorderMetaData.CreatedBy} " +
                  $"on {invoice.DataRecorderMetaData.DateCreated}");
```

### Basic Example: Lookup Values

Populating a dropdown for phone types:

```csharp
// Backend: Load active phone types
var phoneTypes = unitOfWork.GetRepository<Lookup>()
    .Query()
    .Where(l => l.LookupTypeId == (long)LookupTypes.Phone)
    .Where(l => l.IsActive)
    .OrderBy(l => l.RelativeOrder)
    .Select(l => new { l.Id, l.Name, l.Alias })
    .ToList();

// Returns:
// [
//   { Id: 1, Name: "Mobile", Alias: "MOBILE" },
//   { Id: 2, Name: "Home", Alias: "HOME" },
//   { Id: 3, Name: "Work", Alias: "WORK" },
//   { Id: 4, Name: "Fax", Alias: "FAX" }
// ]

// Frontend: Render as <select>
// <option value="1">Mobile</option>
// <option value="2">Home</option>
// ...
```

Adding a new lookup value (no code deployment needed):

```sql
-- Database admin adds new phone type
INSERT INTO Lookup (LookupTypeId, Name, Alias, RelativeOrder, IsActive)
VALUES (1, 'VoIP', 'VOIP', 5, 1);

-- Immediately available in all UI dropdowns on next page load
```

### Advanced Example: Folder Organization

Creating a logical folder structure for a franchisee:

```csharp
// Create folder hierarchy for Marblelife of Seattle
var rootFolder = new Folder { RelativePath = "/franchisee/123" };
var invoiceFolder = new Folder { RelativePath = "/franchisee/123/invoices" };
var jobPhotosFolder = new Folder { RelativePath = "/franchisee/123/jobs/photos" };

repository.Add(rootFolder);
repository.Add(invoiceFolder);
repository.Add(jobPhotosFolder);

// Files stored under these logical paths
var invoice = new File {
    Name = "INV-2023-001.pdf",
    RelativeLocation = "/franchisee/123/invoices/INV-2023-001.pdf"
};

// Query all files in a folder
var invoiceFiles = repository.Query<File>()
    .Where(f => f.RelativeLocation.StartsWith("/franchisee/123/invoices/"))
    .ToList();
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## 📚 API Summary

### DataRecorderMetaData

| Member | Description |
|--------|-------------|
| `CreatedBy` | User ID who created the record (nullable for system-generated) |
| `DateCreated` | UTC timestamp of record creation |
| `ModifiedBy` | User ID who last modified the record |
| `DateModified` | UTC timestamp of last modification |
| `GetClone()` | Creates shallow copy for audit history |
| `SetModifiedBy(userId)` | Updates modification tracking with current time |
| `SetModifiedBy(userId, date)` | Updates modification tracking with explicit timestamp |

**Constructors**:
- `DataRecorderMetaData()` - Uses current UTC time
- `DataRecorderMetaData(DateTime dateCreated)` - Explicit creation date
- `DataRecorderMetaData(long createdBy)` - Current time + user
- `DataRecorderMetaData(DateTime dateCreated, long createdBy)` - Full control

---

### File

| Property | Type | Description |
|----------|------|-------------|
| `Name` | string | Original filename (e.g., "invoice.pdf") |
| `Caption` | string | User-friendly description |
| `Size` | decimal | File size in bytes |
| `RelativeLocation` | string | Path relative to storage root |
| `MimeType` | string | Content type (e.g., "application/pdf") |
| `css` | string | CSS class for icon rendering |
| `DataRecorderMetaDataId` | long | Foreign key to audit trail |
| `DataRecorderMetaData` | DataRecorderMetaData | Navigation property for upload tracking |
| `IsFileToBeDeleted` | bool? | Flag for deferred deletion |

**Related Services**:
- `IFileService` - Upload/download business logic
- `IFileFactory` - Transforms between File (domain) and FileModel (viewmodel)

---

### Folder

| Property | Type | Description |
|----------|------|-------------|
| `RelativePath` | string | Logical folder path (e.g., "/franchisee/123/invoices") |

**Usage**: Organizational structure for files. Query files by path prefix.

---

### ContentType

| Property | Type | Description |
|----------|------|-------------|
| `Name` | string | Display name (e.g., "PDF Document") |
| `MimeType` | string | Standard MIME type (e.g., "application/pdf") |

**Usage**: Validation during file upload, content negotiation for downloads.

---

### Lookup

| Property | Type | Description |
|----------|------|-------------|
| `LookupTypeId` | long | Category ID (maps to LookupTypes enum) |
| `Name` | string | Display text for UI |
| `Alias` | string | API-friendly code (stable identifier) |
| `RelativeOrder` | byte? | Custom sort order |
| `IsActive` | bool | Visibility toggle (soft delete) |

**Related Enum**: `Core.Application.Enum.LookupTypes` (37 predefined categories)

---

### LookupType

| Property | Type | Description |
|----------|------|-------------|
| `Name` | string | Category name (e.g., "Phone", "InvoiceStatus") |

**Note**: ID values match `LookupTypes` enum. Seeded via database migration, never modified via UI.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Common Patterns -->
## 🎯 Common Patterns

### Pattern 1: Audit Trail on Entity Creation

```csharp
public class MyBusinessEntity : DomainBase
{
    public string Name { get; set; }
    public long DataRecorderMetaDataId { get; set; }
    
    [ForeignKey("DataRecorderMetaDataId")]
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
}

// Usage:
var entity = new MyBusinessEntity {
    Name = "Test",
    DataRecorderMetaData = new DataRecorderMetaData(currentUserId)
};
```

**Why**: Separates audit concerns from business logic. Single `DataRecorderMetaData` table serves 50+ entities.

### Pattern 2: Lookup as Typed Enum

```csharp
// Define enum constant
public enum LookupTypes
{
    InvoiceStatus = 9
}

// Query active lookups
var statuses = repository.Query<Lookup>()
    .Where(l => l.LookupTypeId == (long)LookupTypes.InvoiceStatus)
    .Where(l => l.IsActive)
    .OrderBy(l => l.RelativeOrder);

// Use Alias for API stability
var paidStatus = statuses.FirstOrDefault(s => s.Alias == "PAID");
```

**Why**: Combines compile-time safety (enum) with runtime flexibility (database).

### Pattern 3: File Soft Delete

```csharp
// Mark file for deletion (async cleanup job processes later)
file.IsFileToBeDeleted = true;
repository.Update(file);

// Background job queries and processes
var filesToDelete = repository.Query<File>()
    .Where(f => f.IsFileToBeDeleted == true)
    .ToList();

foreach (var file in filesToDelete)
{
    fileStorage.Delete(file.RelativeLocation);
    repository.Delete(file); // Hard delete from DB
}
```

**Why**: Defers expensive I/O (file deletion) to background process. Improves user experience.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## 🔧 Troubleshooting

### Issue: "IClock not registered in DI container"

**Symptom**: Exception when creating `DataRecorderMetaData()` without parameters.

**Solution**:
```csharp
// Ensure Clock is registered in Unity container (src/DependencyInjection)
container.RegisterType<IClock, Clock>(new ContainerControlledLifetimeManager());
```

**Workaround**: Use explicit constructor overload:
```csharp
var metadata = new DataRecorderMetaData(DateTime.UtcNow, userId);
```

---

### Issue: "Lookup dropdown showing deleted values"

**Symptom**: Old lookup values appear in UI despite being obsolete.

**Root Cause**: Query not filtering by `IsActive`.

**Solution**:
```csharp
// ❌ Wrong
var lookups = repository.Query<Lookup>()
    .Where(l => l.LookupTypeId == typeId);

// ✅ Correct
var lookups = repository.Query<Lookup>()
    .Where(l => l.LookupTypeId == typeId && l.IsActive);
```

---

### Issue: "File uploads failing with FK constraint violation"

**Symptom**: SQL error when saving File entity.

**Root Cause**: `DataRecorderMetaData` not persisted before File.

**Solution** (use proper transaction):
```csharp
using (var transaction = unitOfWork.BeginTransaction())
{
    try {
        var metadata = new DataRecorderMetaData(userId);
        repository.Add(metadata);
        unitOfWork.SaveChanges(); // Generates metadata.Id
        
        var file = new File {
            DataRecorderMetaDataId = metadata.Id,
            // ... other properties
        };
        repository.Add(file);
        unitOfWork.SaveChanges();
        
        transaction.Commit();
    } catch {
        transaction.Rollback();
        throw;
    }
}
```

**Better Solution** (let EF handle cascade):
```csharp
var file = new File {
    DataRecorderMetaData = new DataRecorderMetaData(userId), // Navigation property
    // ... other properties
};
repository.Add(file);
unitOfWork.SaveChanges(); // EF inserts metadata first, then file
```

---

### Issue: "DateTime values off by hours"

**Symptom**: Audit timestamps showing wrong times in UI.

**Root Cause**: `DataRecorderMetaData` stores UTC, UI displays without conversion.

**Solution**: Always convert to franchisee's local timezone:
```csharp
var clock = DependencyResolver.Resolve<IClock>();
var localCreatedDate = clock.ToLocal(entity.DataRecorderMetaData.DateCreated);

// Or with explicit timezone
var seattleTime = clock.ToLocal(
    entity.DataRecorderMetaData.DateCreated,
    TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")
);
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Migration Guide -->
## 🔄 Migration Guide

### Adding a New Lookup Type

**Step 1**: Add enum value to `LookupTypes` enum:
```csharp
// src/Core/Application/Enum/LookupTypes.cs
public enum LookupTypes
{
    // ... existing values
    MyNewType = 999 // Choose unused ID
}
```

**Step 2**: Seed `LookupType` table:
```sql
-- In DatabaseDeploy migration script
INSERT INTO LookupType (Id, Name) VALUES (999, 'MyNewType');
```

**Step 3**: Seed `Lookup` values:
```sql
INSERT INTO Lookup (LookupTypeId, Name, Alias, RelativeOrder, IsActive)
VALUES 
    (999, 'Option A', 'OPTION_A', 1, 1),
    (999, 'Option B', 'OPTION_B', 2, 1),
    (999, 'Option C', 'OPTION_C', 3, 1);
```

**Step 4**: Use in code:
```csharp
var options = repository.Query<Lookup>()
    .Where(l => l.LookupTypeId == (long)LookupTypes.MyNewType)
    .Where(l => l.IsActive)
    .OrderBy(l => l.RelativeOrder);
```

### Attaching Audit Trail to Existing Entity

**Step 1**: Add properties to entity:
```csharp
public class MyExistingEntity : DomainBase
{
    // ... existing properties
    
    public long DataRecorderMetaDataId { get; set; }
    
    [ForeignKey("DataRecorderMetaDataId")]
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

**Step 2**: Create migration to add FK column:
```sql
ALTER TABLE MyExistingEntity
ADD DataRecorderMetaDataId BIGINT NULL;

ALTER TABLE MyExistingEntity
ADD CONSTRAINT FK_MyExistingEntity_DataRecorderMetaData
FOREIGN KEY (DataRecorderMetaDataId) REFERENCES DataRecorderMetaData(Id);
```

**Step 3**: Backfill existing records:
```sql
-- Create metadata for orphaned records
INSERT INTO DataRecorderMetaData (DateCreated, CreatedBy)
SELECT GETUTCDATE(), NULL
FROM MyExistingEntity
WHERE DataRecorderMetaDataId IS NULL;

-- Link records to metadata
UPDATE e
SET e.DataRecorderMetaDataId = m.Id
FROM MyExistingEntity e
CROSS APPLY (
    SELECT TOP 1 Id FROM DataRecorderMetaData
    WHERE CreatedBy IS NULL
    ORDER BY Id DESC
) m
WHERE e.DataRecorderMetaDataId IS NULL;
```

**Step 4**: Make FK required (optional):
```sql
ALTER TABLE MyExistingEntity
ALTER COLUMN DataRecorderMetaDataId BIGINT NOT NULL;
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Related Documentation -->
## 📖 Related Documentation

- **[Core Module Overview](../../AI-CONTEXT.md)** - Business logic architecture
- **[Application Module](../AI-CONTEXT.md)** - Shared kernel overview
- **[ORM Module](../../../ORM/AI-CONTEXT.md)** - Entity Framework 6 mappings
- **[Organizations Module](../../Organizations/AI-CONTEXT.md)** - Heavy user of DataRecorderMetaData
- **[API Layer](../../../API/AI-CONTEXT.md)** - REST endpoints for file upload/download

### External Resources

- [Entity Framework 6 Documentation](https://docs.microsoft.com/ef/ef6/)
- [Type-Safe Enum Pattern](https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
<!-- END CUSTOM SECTION -->
