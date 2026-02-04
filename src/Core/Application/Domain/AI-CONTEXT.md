<!-- AUTO-GENERATED: Header -->
# Application.Domain Module Context
**Version**: f145a16b867a9004888c3b91d5633737a64584f9  
**Generated**: 2025-02-04T06:10:00Z  
**Path**: `src/Core/Application/Domain`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
The **Application.Domain** namespace contains foundational domain entities that represent cross-cutting concerns used throughout the Marblelife franchise management system. These entities provide:

1. **Audit Trail Infrastructure** (`DataRecorderMetaData`) - Timestamp and user tracking for all business entities
2. **File Management Domain** (`File`, `Folder`, `ContentType`) - Document/image storage metadata
3. **Lookup Data Pattern** (`Lookup`, `LookupType`) - Enumeration-style reference data with database persistence

This is the **Shared Kernel** of the Application bounded context - these entities are not business-specific (not Billing, not Scheduler, not Sales) but serve as infrastructure primitives that other Core modules depend upon.

### Design Patterns

- **Base Entity Pattern**: All entities inherit from `DomainBase` (located at `src/Core/DomainBase.cs`), providing:
  - `Id` (long) - Primary key
  - `IsNew` (bool, NotMapped) - Transient flag for insert vs. update logic
  - `IsDeleted` (bool, NotMapped) - Soft-delete marker

- **Audit Trail Mixin**: `DataRecorderMetaData` is used as a **navigation property** in 50+ entities across the system (Organizations, Sales, MarketingLead modules). It provides:
  - Creation timestamp/user tracking
  - Modification timestamp/user tracking
  - Cloneable metadata for audit history

- **Type-Safe Enumerations**: The `Lookup`/`LookupType` pair implements the **Type-Safe Enum Pattern** (database-backed enumerations):
  - `LookupType` defines categories (e.g., "Phone", "InvoiceStatus")
  - `Lookup` defines values within those categories (e.g., "Mobile", "Paid")
  - `LookupTypes` enum (at `Core.Application.Enum.LookupTypes`) provides compile-time constants

- **Dependency Injection for Time**: `DataRecorderMetaData` uses `IClock` interface via DI to avoid `DateTime.Now` - enables testability and timezone handling

### Data Flow

#### DataRecorderMetaData Usage Flow
```
1. Entity creation → Constructor calls IClock.UtcNow via DI
2. Business logic modifies entity → Calls SetModifiedBy(userId)
3. SetModifiedBy updates DateModified + ModifiedBy
4. Persistence layer saves both entity + audit metadata
```

#### File Storage Flow
```
1. Client uploads file → IFileService.SaveFile()
2. FileModel (ViewModel) → FileFactory transforms to File (Domain)
3. File entity persists with:
   - Physical location (RelativeLocation)
   - Metadata (Name, Size, MimeType, css class)
   - Audit trail (DataRecorderMetaData foreign key)
4. DataRecorderMetaData tracks upload user/time
```

#### Lookup Resolution Flow
```
1. API receives LookupType enum value (e.g., LookupTypes.InvoiceStatus)
2. Repository queries Lookup table WHERE LookupTypeId = 9
3. Returns active Lookup records ordered by RelativeOrder
4. Frontend displays as dropdown/radio buttons
```

### Thread Safety
- **DataRecorderMetaData**: Not thread-safe. Assumes single-threaded Entity Framework context per HTTP request.
- **Lookup Data**: Read-only after seeding. Safe for concurrent reads.

### Performance Considerations
- `DataRecorderMetaData` uses **virtual properties** → Entity Framework lazy loading enabled
- `File.DataRecorderMetaData` navigation has explicit `[ForeignKey]` to avoid EF convention mismatches
- Lookup tables are heavily read, rarely written → ideal for **caching layer** (not implemented in Domain)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### DataRecorderMetaData
**Purpose**: Audit trail for entity creation and modification tracking.

```csharp
namespace Core.Application.Domain
{
    /// <summary>
    /// Audit metadata tracking WHO and WHEN for entity lifecycle.
    /// Used as navigation property in 50+ domain entities.
    /// </summary>
    public class DataRecorderMetaData : DomainBase
    {
        // WHO created this record (User.Id or null if system-generated)
        public virtual long? CreatedBy { get; set; }
        
        // WHEN was this record created (always UTC)
        public virtual DateTime DateCreated { get; set; }
        
        // WHO last modified this record (null if never modified)
        public virtual long? ModifiedBy { get; set; }
        
        // WHEN was this record last modified (null if never modified)
        public virtual DateTime? DateModified { get; set; }
        
        // Clone for audit history purposes (preserves last known state)
        public virtual DataRecorderMetaData GetClone();
        
        // Update modification tracking (resolves Clock from DI internally)
        public virtual void SetModifiedBy(long modifiedBy);
        
        // Update modification tracking with explicit timestamp
        public virtual void SetModifiedBy(long modifiedBy, DateTime dateModified);
    }
}
```

**Key Behaviors**:
- Constructors resolve `IClock` via `ApplicationManager.DependencyInjection.Resolve<IClock>()`
- `SetModifiedBy(long)` overload internally calls `Clock.UtcNow` (concrete class, not interface - **potential DI inconsistency**)
- `CreatedBy = 0` is treated as "system user" and nullified
- All timestamps are **UTC** - timezone conversions handled by `IClock.ToLocal()`

**Used By**: FranchiseeNotes, LeadPerformanceDetails, Organization, FranchiseeRegistrationHistory, WebLeadData, etc.

---

### File
**Purpose**: Metadata for uploaded documents/images (invoices, job photos, franchise documents).

```csharp
namespace Core.Application.Domain
{
    /// <summary>
    /// Represents a file uploaded to the system (filesystem or S3).
    /// Does NOT store binary content - only metadata + location.
    /// </summary>
    public class File : DomainBase
    {
        // Original filename (e.g., "invoice.pdf")
        public string Name { get; set; }
        
        // User-friendly description (e.g., "Q1 2023 Invoice")
        public string Caption { get; set; }
        
        // File size in bytes
        public decimal Size { get; set; }
        
        // Path relative to storage root (e.g., "/uploads/2023/01/abc123.pdf")
        public string RelativeLocation { get; set; }
        
        // MIME type (e.g., "application/pdf", "image/jpeg")
        public string MimeType { get; set; }
        
        // CSS class for icon rendering (e.g., "fa-file-pdf-o")
        public string css { get; set; }
        
        // Foreign key to audit trail
        public long DataRecorderMetaDataId { get; set; }
        
        [ForeignKey("DataRecorderMetaDataId")]
        public DataRecorderMetaData DataRecorderMetaData { get; set; }
        
        // Soft delete flag for deferred cleanup (nullable for legacy compatibility)
        public bool? IsFileToBeDeleted { get; set; }
    }
}
```

**Key Design Decisions**:
- **No binary storage** - this is a metadata-only entity. Actual bytes stored via `IFileService.SaveFile()` to filesystem/S3.
- `DataRecorderMetaDataId` uses explicit `[ForeignKey]` attribute → EF6 relationship mapping.
- `IsFileToBeDeleted` supports **deferred deletion** pattern (mark for async cleanup job).
- Commented-out `FileReferenceId` suggests abandoned polymorphic file relationship feature.

**Related Components**:
- `IFileService` - Business logic for file upload/download
- `IFileFactory` - Converts between `File` (Domain) ↔ `FileModel` (ViewModel)
- `FileModel` - ViewModel with 40+ properties (URLs, thumbnail IDs, cropping data)

---

### Folder
**Purpose**: Hierarchical directory structure for organizing files.

```csharp
namespace Core.Application.Domain
{
    /// <summary>
    /// Virtual folder for file organization (not OS filesystem folders).
    /// </summary>
    public class Folder : DomainBase
    {
        // Logical path (e.g., "/franchisee/123/invoices")
        public virtual string RelativePath { get; set; }
    }
}
```

**Key Characteristics**:
- **Virtual property** → EF6 lazy loading or override capability in proxies
- Simple container - no FK to files (likely queried via string prefix matching on `File.RelativeLocation`)
- No audit trail (DataRecorderMetaData) - folders are structural, not auditable

**Usage Pattern**:
```csharp
// Likely usage (not confirmed in codebase scan):
var invoiceFolder = new Folder { RelativePath = "/franchisee/123/invoices" };
var files = context.Files.Where(f => f.RelativeLocation.StartsWith(folder.RelativePath));
```

---

### ContentType
**Purpose**: Maps file extensions to MIME types for content negotiation.

```csharp
namespace Core.Application.Domain
{
    /// <summary>
    /// MIME type registry for file upload validation and HTTP response headers.
    /// </summary>
    public class ContentType : DomainBase
    {
        // Display name (e.g., "PDF Document")
        public virtual string Name { get; set; }
        
        // Standard MIME type (e.g., "application/pdf")
        public virtual string MimeType { get; set; }
    }
}
```

**Key Characteristics**:
- **Virtual properties** → EF6 overrideable in derived proxies
- No FK to File entity → likely used for **validation** during upload, not storage
- Seeded via database migration (not managed via UI)

**Expected Seed Data**:
```
Name              | MimeType
------------------|-----------------------
PDF Document      | application/pdf
JPEG Image        | image/jpeg
PNG Image         | image/png
Excel Spreadsheet | application/vnd.ms-excel
```

---

### Lookup
**Purpose**: Database-backed enumeration values for dropdown lists and reference data.

```csharp
namespace Core.Application.Domain
{
    /// <summary>
    /// Generic lookup value (e.g., "Mobile" for Phone type, "Paid" for Invoice Status).
    /// </summary>
    public class Lookup : DomainBase
    {
        // Foreign key to LookupType (category)
        public long LookupTypeId { get; set; }
        
        // Display text (e.g., "Credit Card", "Partial Payment")
        public string Name { get; set; }
        
        // API-friendly code (e.g., "CC", "PARTIAL_PAY")
        public string Alias { get; set; }
        
        // UI sort order (nullable for legacy data)
        public byte? RelativeOrder { get; set; }
        
        // Soft delete (hide from dropdowns without removing data)
        public bool IsActive { get; set; }
    }
}
```

**Key Design**:
- `LookupTypeId` corresponds to `LookupTypes` enum values
- `Alias` enables **API versioning** (change display Name, keep Alias stable)
- `RelativeOrder` supports **custom sorting** (not alphabetical)
- `IsActive` implements **soft delete** (preserves FK integrity for historical records)

**Usage Example**:
```csharp
// Get all active phone types ordered by preference
var phoneTypes = context.Lookups
    .Where(l => l.LookupTypeId == (long)LookupTypes.Phone && l.IsActive)
    .OrderBy(l => l.RelativeOrder)
    .ToList();
// Returns: [Mobile, Home, Work, Fax]
```

---

### LookupType
**Purpose**: Category definition for Lookup values (metadata for Type-Safe Enum pattern).

```csharp
namespace Core.Application.Domain
{
    /// <summary>
    /// Category for grouping Lookup values (e.g., "Phone", "InvoiceStatus").
    /// Maps to LookupTypes enum in Core.Application.Enum.
    /// </summary>
    public class LookupType : DomainBase
    {
        // Category name (e.g., "Phone", "Address", "InvoiceStatus")
        public string Name { get; set; }
    }
}
```

**Key Characteristics**:
- Extremely simple entity - just a name wrapper around `Id`
- `Id` matches enum values in `LookupTypes`:
  ```csharp
  public enum LookupTypes
  {
      Phone = 1,
      Address = 2,
      InstrumentType = 5,
      InvoiceStatus = 9,
      // ... 30+ more
  }
  ```
- Seeded via database migration - **NEVER modified via UI**

**Design Rationale**:
- Enables dynamic dropdown lists without code deployment
- Supports internationalization (translate `Lookup.Name` without changing IDs)
- Franchisees can add custom lookup values (within predefined types)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### DataRecorderMetaData

#### Constructors
```csharp
// Default constructor - uses IClock.UtcNow from DI
DataRecorderMetaData()

// Specify creation date explicitly (for imports/migrations)
DataRecorderMetaData(DateTime dateCreated)

// Specify creation date + user
DataRecorderMetaData(DateTime dateCreated, long createdBy)

// Specify creating user (date from IClock.UtcNow)
DataRecorderMetaData(long createdBy)
```

**Behavior**:
- All constructors resolve `IClock` via `ApplicationManager.DependencyInjection`
- `createdBy = 0` is normalized to `null` (system user convention)
- Comments show `// IsNew = true;` (disabled) - suggests abandoned change tracking

#### Methods

##### `GetClone() → DataRecorderMetaData`
**Purpose**: Create shallow copy for audit history.

**Behavior**:
```csharp
return new DataRecorderMetaData(ModifiedBy ?? CreatedBy ?? 0);
```
- Clones with "last known modifier" as creator of clone
- Does NOT preserve `DateCreated` or `DateModified` → clone gets fresh timestamps
- **Use Case**: Snapshot before entity modification for audit log

##### `SetModifiedBy(long modifiedBy) → void`
**Purpose**: Update modification tracking with current UTC time.

**Side Effects**:
- Resolves `Clock` (concrete class) from DI → calls `UtcNow`
- Updates `ModifiedBy` and `DateModified`
- **WARNING**: Uses `Clock` (concrete) not `IClock` (interface) → DI inconsistency

##### `SetModifiedBy(long modifiedBy, DateTime dateModified) → void`
**Purpose**: Update modification tracking with explicit timestamp.

**Behavior**:
- `modifiedBy = 0` → sets `ModifiedBy = null`
- `modifiedBy > 0` → sets `ModifiedBy = modifiedBy`
- Always sets `DateModified = dateModified`

**Use Case**: Backdated modifications (data imports, audit corrections)

---

### File
**No business methods** - pure data container. All logic in `IFileService` and `IFileFactory`.

**Key Properties**:
- `IsFileToBeDeleted` (bool?) → Flags for async cleanup job (see `src/Jobs`)
- `DataRecorderMetaData` (navigation) → Audit trail for upload user/time

---

### Folder, ContentType, Lookup, LookupType
**No business methods** - pure data containers managed via standard Repository pattern.

**Access Pattern**:
```csharp
IRepository<Lookup> lookupRepo = unitOfWork.GetRepository<Lookup>();
var activeStatuses = lookupRepo.Query()
    .Where(l => l.LookupTypeId == (long)LookupTypes.InvoiceStatus)
    .Where(l => l.IsActive)
    .OrderBy(l => l.RelativeOrder)
    .ToList();
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Dependencies (Core Modules)

#### Direct Dependencies
- **[Core/DomainBase.cs](../../DomainBase.cs)** - Base entity with `Id`, `IsNew`, `IsDeleted`
- **[Application/IClock](../IClock.cs)** - Time abstraction for UTC/local conversion
- **[Application/Impl/Clock](../Impl/Clock.cs)** - Concrete implementation (⚠️ directly referenced in DataRecorderMetaData)
- **[Application/ApplicationManager](../ApplicationManager.cs)** - Static DI resolver
- **[Application/Enum/LookupTypes](../Enum/LookupTypes.cs)** - Enum constants for LookupTypeId

#### Consumed By (50+ Entities)
- **[Organizations Module](../../Organizations/AI-CONTEXT.md)** - 14 entities use `DataRecorderMetaData`:
  - `FranchiseeNotes`, `Organization`, `FranchiseeRegistrationHistory`, `ReplacementCharges`, etc.
- **[Sales Module](../../Sales/AI-CONTEXT.md)** - Customer import tracking
- **[MarketingLead Module](../../MarketingLead/AI-CONTEXT.md)** - `WebLeadData`, `CallDetailsReportNotes`
- **[Billing Module](../../Billing/AI-CONTEXT.md)** - Invoice/payment audit trail
- **[Scheduler Module](../../Scheduler/AI-CONTEXT.md)** - Job modification tracking

### External Dependencies

#### NuGet Packages
- **EntityFramework 6.x** - All entities participate in EF6 Code First
  - `[ForeignKey]` attribute from `System.ComponentModel.DataAnnotations.Schema`
  - `[NotMapped]` attribute in DomainBase
  - Virtual properties for lazy loading

#### .NET Framework
- `System.ComponentModel.DataAnnotations` - Data annotations
- `System` - DateTime handling

### Cross-Module Integration

#### File Storage Workflow
```
API Layer → IFileService → IFileFactory → File (Domain) → IRepository → EF6 → SQL Server
                                            ↓
                                    DataRecorderMetaData (audit)
```

#### Lookup Resolution Workflow
```
API receives LookupTypes enum → Repository query → Lookup (Domain) → JSON response
```

#### Audit Trail Pattern
```
Business Logic → Entity.Save() → Populate DataRecorderMetaDataId → EF6 SaveChanges()
                                                                        ↓
                                                        Tracks WHO/WHEN in separate table
```

### Dependency Injection Notes
- `DataRecorderMetaData` uses **Service Locator anti-pattern**:
  ```csharp
  ApplicationManager.DependencyInjection.Resolve<IClock>()
  ```
  Should ideally receive `IClock` via constructor injection.
- Unity Container configured in `src/DependencyInjection` module.

### Database Schema
Entity Framework mappings located in:
- `src/ORM/Mapping/ApplicationMapping/` (expected location)
- Tables: `DataRecorderMetaData`, `File`, `Folder`, `ContentType`, `Lookup`, `LookupType`
- Relationships:
  - `File.DataRecorderMetaDataId` → `DataRecorderMetaData.Id` (1:1)
  - `Lookup.LookupTypeId` → `LookupType.Id` (N:1)
  - **50+ entities** → `DataRecorderMetaData.Id` (1:1 or N:1)
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### 🚨 Known Issues & Gotchas

1. **DI Inconsistency in DataRecorderMetaData**:
   - Constructors resolve `IClock` (interface)
   - `SetModifiedBy()` resolves `Clock` (concrete class)
   - **Impact**: Mocking for unit tests fails on overload without explicit DateTime

2. **Virtual Properties Without Reason**:
   - `Folder.RelativePath`, `ContentType.Name/MimeType` are virtual
   - No derived classes found in codebase scan
   - **Likely**: Legacy from EF6 dynamic proxy generation (no longer needed with EF Core)

3. **Soft Delete Confusion**:
   - `DomainBase.IsDeleted` (NotMapped transient flag)
   - `File.IsFileToBeDeleted` (nullable persisted flag)
   - `Lookup.IsActive` (active/inactive toggle)
   - **Three different deletion semantics** - no unified pattern

4. **Commented-Out Code in File Entity**:
   ```csharp
   //public long? FileReferenceId { get; set; }
   ```
   Suggests abandoned feature for file versioning or polymorphic relationships.

### 🔧 Maintenance Recommendations

1. **Refactor DataRecorderMetaData DI**:
   - Accept `IClock` via constructor parameter
   - Remove static `ApplicationManager.DependencyInjection` calls
   - Enables true unit testing without integration test infrastructure

2. **Standardize Soft Delete**:
   - Create `ISoftDeletable` interface
   - Unify on single pattern (e.g., `IsDeleted` + `DeletedAt` timestamp)
   - Remove `File.IsFileToBeDeleted` in favor of `DomainBase.IsDeleted`

3. **Add Lookup Caching**:
   - Lookups are read 1000x more than written
   - Add `ILookupCache` service with distributed cache (Redis)
   - Invalidate on Lookup Insert/Update (rare event)

4. **Document Foreign Key Conventions**:
   - Mix of explicit `[ForeignKey]` (File) and EF conventions (elsewhere)
   - Create architectural decision record (ADR) for when to use annotations

### 📊 Usage Statistics (from codebase scan)

- **DataRecorderMetaData**: Used in 50+ domain entities across 6 modules
- **File**: Referenced by 3 services (FileService, FileFactory, ImageHelper)
- **Lookup**: 37 distinct LookupTypes defined in enum
- **Folder**: Low usage (1-2 references found) - possibly deprecated feature

### 🧪 Testing Strategies

#### Unit Testing DataRecorderMetaData
```csharp
// Mock IClock to control time
var mockClock = new Mock<IClock>();
mockClock.Setup(c => c.UtcNow).Returns(new DateTime(2023, 1, 1));

// PROBLEM: Cannot inject into DataRecorderMetaData() constructor
// WORKAROUND: Use explicit DateTime overload
var metadata = new DataRecorderMetaData(mockClock.Object.UtcNow, userId);
```

#### Integration Testing File Entity
```csharp
// Requires EF6 + SQL Server test database
using (var context = new ApplicationDbContext(testConnectionString))
{
    var file = new File {
        Name = "test.pdf",
        DataRecorderMetaData = new DataRecorderMetaData(userId)
    };
    context.Files.Add(file);
    context.SaveChanges();
    
    // Verify audit trail persisted
    Assert.IsNotNull(file.DataRecorderMetaData.CreatedBy);
}
```

### 🔮 Future Evolution

1. **Migrate to EF Core**:
   - Remove virtual properties (EF Core uses change tracking snapshots)
   - Replace `[ForeignKey]` with Fluent API in `OnModelCreating()`
   - Async repository methods

2. **Event Sourcing for Audit Trail**:
   - Replace `DataRecorderMetaData` with event stream
   - Every modification emits `EntityModifiedEvent`
   - Enables full history reconstruction, not just "last modified"

3. **Cloud-Native File Storage**:
   - Add `StorageProvider` enum (FileSystem, S3, Azure Blob)
   - Abstract `IFileStorageService` with provider-specific implementations
   - `File.RelativeLocation` becomes opaque blob key
<!-- END CUSTOM SECTION -->
