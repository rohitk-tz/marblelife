<!-- AUTO-GENERATED: Header -->
# Domain — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Defines the core domain entities that represent the business data model for file management, folder organization, lookup tables, metadata tracking, and content type classification. These entities map directly to database tables via Entity Framework and encapsulate business rules for auditing and data lifecycle management.

### Design Patterns
- **Domain-Driven Design**: Entities represent core business concepts (File, Folder, Lookup) with rich behavior
- **Active Record Pattern**: `DataRecorderMetaData` includes methods for timestamp and user tracking (`SetModifiedBy`)
- **Audit Tracking**: All entities inherit from `DomainBase` which provides `Id`, `IsNew`, and `IsDeleted` for change tracking
- **Dependency Injection for Cross-Cutting Concerns**: `DataRecorderMetaData` uses `ApplicationManager.DependencyInjection.Resolve<Clock>()` for testable time management

### Data Flow
1. Domain entities are instantiated by repositories or ORM hydration
2. Business logic manipulates entity properties and calls methods (e.g., `SetModifiedBy`)
3. Entities are persisted back to database via UnitOfWork/Repository pattern
4. `DataRecorderMetaData` automatically tracks creation/modification timestamps
5. Lookup tables (`Lookup`, `LookupType`) provide referential integrity for dropdown/enum values
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### File.cs
```csharp
// Represents an uploaded file with metadata and S3/filesystem location
public class File : DomainBase
{
    public string Name { get; set; }                // Original filename
    public string Caption { get; set; }             // User-friendly display name
    public decimal Size { get; set; }               // File size in bytes
    public string RelativeLocation { get; set; }    // Path relative to MediaRootPath
    public string MimeType { get; set; }            // Content type (e.g., "image/jpeg")
    public string css { get; set; }                 // CSS class for icon display
    
    public long DataRecorderMetaDataId { get; set; }
    [ForeignKey("DataRecorderMetaDataId")]
    public DataRecorderMetaData DataRecorderMetaData { get; set; }  // Audit trail
    
    public bool? IsFileToBeDeleted { get; set; }    // Soft delete flag for batch operations
}
```

### Folder.cs
```csharp
// Represents a virtual folder for organizing files in a hierarchy
public class Folder : DomainBase
{
    public virtual string RelativePath { get; set; }  // Folder path relative to root (e.g., "/invoices/2023")
}
```

### Lookup.cs
```csharp
// Represents a single value in a lookup table (e.g., "Credit Card" in PaymentTypes)
public class Lookup : DomainBase
{
    public long LookupTypeId { get; set; }      // Foreign key to LookupType table
    public string Name { get; set; }             // Display name (e.g., "Credit Card")
    public string Alias { get; set; }            // Alternative name or code
    public byte? RelativeOrder { get; set; }     // Sort order for dropdown display
    public bool IsActive { get; set; }           // Soft delete / enable-disable flag
}
```

### LookupType.cs
```csharp
// Represents a category of lookup values (e.g., "PaymentTypes", "InvoiceStatuses")
public class LookupType : DomainBase
{
    public string Name { get; set; }  // Category name (e.g., "AccountType")
}
```

### ContentType.cs
```csharp
// Maps file extensions to MIME types for content negotiation
public class ContentType : DomainBase
{
    public virtual string Name { get; set; }      // File extension or type name (e.g., "PDF")
    public virtual string MimeType { get; set; }  // IANA MIME type (e.g., "application/pdf")
}
```

### DataRecorderMetaData.cs
```csharp
// Audit metadata for all entities - tracks who created/modified and when
public class DataRecorderMetaData : DomainBase
{
    public virtual DateTime DateCreated { get; set; }
    public virtual DateTime? DateModified { get; set; }
    public virtual long? CreatedBy { get; set; }      // User ID of creator
    public virtual long? ModifiedBy { get; set; }     // User ID of last modifier
    
    // Creates a shallow clone with the modifier set as creator
    public virtual DataRecorderMetaData GetClone()
    {
        return new DataRecorderMetaData(ModifiedBy ?? CreatedBy ?? 0);
    }
    
    // Updates modification timestamp using current time from Clock service
    public virtual void SetModifiedBy(long modifiedBy)
    {
        SetModifiedBy(modifiedBy, ApplicationManager.DependencyInjection.Resolve<Clock>().UtcNow);
    }
    
    // Updates modification metadata with explicit timestamp (for testing or backdating)
    public virtual void SetModifiedBy(long modifiedBy, DateTime dateModified)
    {
        ModifiedBy = modifiedBy > 0 ? (long?)modifiedBy : null;  // 0 = anonymous/system
        DateModified = dateModified;
    }
    
    // Constructors support various initialization scenarios:
    // - Default: Uses Clock.UtcNow and anonymous user
    // - With dateCreated: Explicit timestamp
    // - With createdBy: Current time + user ID
    // - With both: Full control
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### File Entity
- **Purpose**: Stores uploaded file metadata and location
- **Key Properties**:
  - `RelativeLocation`: Path relative to `ApplicationManager.Settings.MediaRootPath`
  - `MimeType`: Used for HTTP Content-Type headers
  - `Size`: Stored as decimal to support very large files (>2GB)
  - `IsFileToBeDeleted`: Marks files for batch deletion (e.g., cleanup jobs)
- **Relationships**:
  - Many-to-One with `DataRecorderMetaData` for audit trail
- **Usage**: Created by `FileService`, queried by repositories

### Folder Entity
- **Purpose**: Organizes files into logical hierarchies
- **Key Properties**:
  - `RelativePath`: Must start with "/" and use forward slashes (Unix-style)
- **Usage**: Created manually or by folder management services

### Lookup Entity
- **Purpose**: Provides dropdown values and reference data
- **Key Properties**:
  - `LookupTypeId`: Groups lookups by category (see `LookupTypes` enum)
  - `RelativeOrder`: Controls display order in UI dropdowns
  - `IsActive`: Allows soft-deletion of lookup values without breaking foreign keys
- **Usage**: Seeded during application setup, queried for dropdowns/validation

### LookupType Entity
- **Purpose**: Categorizes lookup values (e.g., "Phone", "Address", "InvoiceStatus")
- **Usage**: Typically seeded once, rarely modified after deployment

### ContentType Entity
- **Purpose**: Maps file extensions to MIME types
- **Key Properties**:
  - `Name`: File extension (e.g., "PDF", "JPEG")
  - `MimeType`: IANA standard (e.g., "application/pdf", "image/jpeg")
- **Usage**: Used by FileService to set correct Content-Type headers for downloads

### DataRecorderMetaData Entity
- **Purpose**: Provides audit trail for all domain entities
- **Methods**:
  - `GetClone()`: Creates new metadata preserving user context (for child entities)
  - `SetModifiedBy(long modifiedBy)`: Updates modification timestamp using Clock service (testable)
  - `SetModifiedBy(long modifiedBy, DateTime dateModified)`: Explicit timestamp override (for imports)
- **Key Properties**:
  - `DateCreated`: Set on entity creation, never changed
  - `DateModified`: Updated on every save operation
  - `CreatedBy`/`ModifiedBy`: User IDs; null or 0 indicates system/anonymous
- **Dependency Injection**:
  - Uses `ApplicationManager.DependencyInjection.Resolve<Clock>()` for current time
  - Allows time mocking in unit tests
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **[Core.DomainBase](../../.context/CONTEXT.md)** — Base class providing `Id`, `IsNew`, `IsDeleted` properties
- **[Core.Application.Impl.Clock](../Impl/.context/CONTEXT.md)** — Provides testable `UtcNow` for timestamp generation
- **[Core.Application.Impl.IClock](../Impl/.context/CONTEXT.md)** — Interface for Clock abstraction
- **Core.ApplicationManager** — Dependency injection container access

### External Dependencies
- **System.ComponentModel.DataAnnotations** — Provides `[ForeignKey]` attribute
- **Entity Framework Core** (implied) — ORM for database persistence

### Referenced By
- **Core.Application.ViewModel.FileModel** — DTO representation of File entity
- **Core.Application.ViewModel.EditModelBase** — Embeds `DataRecorderMetaData` for all edit forms
- **Core.Application.Impl.FileService** — Manages File entity lifecycle
- **Repository Layer** — All repositories work with these domain entities
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### DataRecorderMetaData Usage Patterns
1. **Entity Inheritance**: All business entities should have a `DataRecorderMetaData` property or inherit from a base that includes it
2. **Unit of Work Integration**: Repositories should call `SetModifiedBy` before saving changes
3. **Cloning for Child Entities**: Use `GetClone()` when creating child entities to preserve user context:
   ```csharp
   var lineItem = new InvoiceLineItem();
   lineItem.DataRecorderMetaData = invoice.DataRecorderMetaData.GetClone();
   ```

### Lookup Table Strategy
- **Enum vs. Database**: `LookupTypes` enum mirrors database `LookupType` table IDs
- **Seeding**: Lookup values should be seeded during database migration
- **Soft Deletes**: Use `IsActive = false` instead of deleting lookups to preserve historical data integrity

### File Storage
- **Relative vs. Absolute Paths**: Always store `RelativeLocation` in database; use `PathExtensions.ToFullPath()` to resolve absolute path at runtime
- **S3 Integration**: `RelativeLocation` can be S3 key; check `ApplicationManager.Settings.MediaRootPath` to determine storage type

### Common Pitfalls
- **Forgetting to set CreatedBy**: Always pass user ID to constructors or set via `SetModifiedBy` before first save
- **Using DateTime.Now**: Always use `Clock.UtcNow` via DI for testability and timezone consistency
- **Null DateModified**: Check for null before displaying; null means entity was never modified after creation
<!-- END CUSTOM SECTION -->
