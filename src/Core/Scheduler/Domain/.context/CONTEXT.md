<!-- AUTO-GENERATED: Header -->
# Domain — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2026-02-10T15:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Domain folder contains the **persistent entity models** representing the core business concepts of the MarbleLife scheduling system. These are not simple DTOs - they are rich domain entities with navigation properties, foreign key relationships, computed properties, and Entity Framework mappings. This is the heart of the domain model that enforces data integrity, relationship constraints, and business rules at the database schema level.

### Design Patterns
- **Entity Framework Code-First**: All classes inherit from `DomainBase` providing Id, audit timestamps
- **Navigation Properties**: Virtual properties for lazy loading related entities (e.g., `Job.JobScheduler` collection)
- **Foreign Key Attributes**: Explicit FK mapping via `[ForeignKey]` attributes for clarity
- **Computed Properties**: `[NotMapped]` properties like `ActualStartDate` calculate values from raw data + timezone offset
- **Collection Initialization**: Constructors initialize collections to prevent null reference exceptions
- **Cascade Delete Control**: `[CascadeEntity]` attribute on critical relationships like `DataRecorderMetaData`
- **Audit Trail**: `DataRecorderMetaData` tracks who created/modified records and when

### Entity Relationships

```
JobEstimate (1) ─────> (N) Job ─────> (N) JobScheduler ─────> (1) OrganizationRoleUser (Technician)
     │                    │                   │
     ├──> (N) JobNote     ├──> (N) JobNote    ├──> (N) JobNote (Vacation notes)
     │                    │                   │
     └──> (N) JobScheduler└──> (1) JobStatus  ├──> (N) BeforeAfterImages
                │                             │
                └──> (1) JobCustomer          └──> (1) Meeting
                                              │
                                              └──> (1) Invoice

BeforeAfterImages ─────> JobEstimateServices (PairId)
                 ├─────> JobScheduler (SchedulerId)
                 ├─────> File (FileId, ThumbFileId)
                 └─────> Franchisee (FranchiseeId)

TechnicianWorkOrder ───> Lookup (WorkOrderId)

Meeting (self-referencing) ─> Parent Meeting (ParentId)
```

### Data Flow
1. **Estimate Creation**: `JobEstimate` entity created with customer reference
2. **Scheduler Assignment**: `JobScheduler` entities link estimate to technicians/sales reps
3. **Job Conversion**: `Job` entity created with `EstimateId` foreign key preserving audit trail
4. **Image Documentation**: `BeforeAfterImages` linked to `JobScheduler` during work execution
5. **Status Tracking**: `Job.StatusId` references `JobStatus` lookup table
6. **Invoice Generation**: `JobScheduler.InvoiceId` links to billing system

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Entities

#### **Job** - Actual work order
```csharp
public class Job : DomainBase
{
    // Service classification
    public long JobTypeId { get; set; }
    [ForeignKey("JobTypeId")]
    public virtual MarketingClass JobType { get; set; }

    // Scheduling
    public DateTime StartDate { get; set; }  // UTC
    public DateTime EndDate { get; set; }    // UTC
    public double? Offset { get; set; }      // Timezone offset in minutes
    public DateTime StartDateTimeString { get; set; }  // UI display
    public DateTime EndDateTimeString { get; set; }

    // Workflow
    public long StatusId { get; set; }
    [ForeignKey("StatusId")]
    public virtual JobStatus JobStatus { get; set; }  // Created, Assigned, InProgress, Completed, Canceled

    // Customer relationship
    public long CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual JobCustomer JobCustomer { get; set; }

    // Financial integration
    public string QBInvoiceNumber { get; set; }  // UNIQUE - QuickBooks sync

    // Traceability
    public long? EstimateId { get; set; }  // NULL if job not from estimate
    [ForeignKey("EstimateId")]
    public virtual JobEstimate JobEstimate { get; set; }

    // Location
    public string GeoCode { get; set; }  // Lat,Long format

    // Details
    public string Description { get; set; }

    // Navigation properties (1:N)
    public virtual ICollection<JobScheduler> JobScheduler { get; set; }  // Multiple scheduler entries (multi-day, multi-tech)
    public virtual ICollection<JobNote> JobNote { get; set; }

    // Constructor
    public Job()
    {
        JobNote = new Collection<JobNote>();
    }
}
```

**Critical Constraints:**
- `QBInvoiceNumber` must be unique across ALL jobs (enforced by database index)
- `StartDate` and `EndDate` stored in UTC, display conversion via `Offset`
- Soft delete via `DomainBase.IsDeleted` flag (never hard delete jobs)

---

#### **JobEstimate** - Quote/preliminary estimate
```csharp
public class JobEstimate : DomainBase
{
    // Service details
    public int EstimateHour { get; set; }  // Estimated time to complete
    public decimal Amount { get; set; }    // Quoted price
    public string Description { get; set; }

    // Service classification
    public long? TypeId { get; set; }
    [ForeignKey("TypeId")]
    public virtual MarketingClass MarketingClass { get; set; }

    // Customer relationship
    public long CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual JobCustomer JobCustomer { get; set; }

    // Scheduling
    public DateTime? StartDate { get; set; }  // Nullable - estimate may not be scheduled yet
    public DateTime? EndDate { get; set; }
    public double? Offset { get; set; }
    public DateTime StartDateTimeString { get; set; }
    public DateTime EndDateTimeString { get; set; }

    // Location
    public string GeoCode { get; set; }

    // Hierarchical estimates
    public long? ParentEstimateId { get; set; }
    [ForeignKey("ParentEstimateId")]
    public virtual JobEstimate JobEstimates { get; set; }  // Self-referencing FK

    // Navigation properties
    public virtual ICollection<Job> Jobs { get; set; }  // Jobs created from this estimate
    public virtual ICollection<JobNote> JobNote { get; set; }

    // Constructor
    public JobEstimate()
    {
        JobNote = new Collection<JobNote>();
        Jobs = new Collection<Job>();
    }
}
```

**Critical Constraints:**
- `StartDate/EndDate` nullable to support "not yet scheduled" estimates
- `ParentEstimateId` enables complex job breakdowns (e.g., multi-phase projects)
- One estimate can spawn multiple jobs (e.g., phases, recurring service)

---

#### **JobScheduler** - Calendar entry linking jobs/estimates to resources
```csharp
public class JobScheduler : DomainBase
{
    // Links to work item (mutual exclusivity: Job XOR Estimate XOR Meeting)
    public long? JobId { get; set; }
    [ForeignKey("JobId")]
    public virtual Job Job { get; set; }

    public long? EstimateId { get; set; }
    [ForeignKey("EstimateId")]
    public virtual JobEstimate Estimate { get; set; }

    public long? MeetingID { get; set; }
    [ForeignKey("MeetingID")]
    public virtual Meeting Meeting { get; set; }

    // Title (for meetings/vacations)
    public string Title { get; set; }

    // Resource assignment
    public long? AssigneeId { get; set; }  // Technician
    [ForeignKey("AssigneeId")]
    public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }

    public long? SalesRepId { get; set; }
    [ForeignKey("SalesRepId")]
    public virtual OrganizationRoleUser SalesRep { get; set; }

    // Franchisee ownership
    public long FranchiseeId { get; set; }
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }

    // Scheduling
    public DateTime StartDate { get; set; }  // UTC
    public DateTime EndDate { get; set; }
    public double? Offset { get; set; }  // Timezone offset in minutes
    public DateTime StartDateTimeString { get; set; }
    public DateTime EndDateTimeString { get; set; }

    // Computed property for UI display
    [NotMapped]
    public DateTime ActualStartDate
    {
        get { return StartDate.AddMinutes(Offset.GetValueOrDefault()); }
    }

    [NotMapped]
    public DateTime ActualEndDate
    {
        get { return EndDate.AddMinutes(Offset.GetValueOrDefault()); }
    }

    // Scheduler status
    public long SchedulerStatus { get; set; }  // FK to Lookup (Confirmed, Tentative, etc.)
    [ForeignKey("SchedulerStatus")]
    public virtual Lookup Lookup { get; set; }

    // Service type (optional)
    public long? ServiceTypeId { get; set; }
    [ForeignKey("ServiceTypeId")]
    public virtual ServiceType ServiceType { get; set; }

    // Special flags
    public bool IsVacation { get; set; }  // Blocks technician availability
    public bool IsRepeat { get; set; }    // Recurring schedule
    public bool IsImported { get; set; }  // Created from calendar import
    public bool IsActive { get; set; }    // Soft delete flag (in addition to DomainBase.IsDeleted)

    // Invoice integration
    public bool IsInvoiceRequired { get; set; }
    public string InvoiceReason { get; set; }
    public long? InvoiceId { get; set; }
    [ForeignKey("InvoiceId")]
    public virtual Invoice Invoice { get; set; }
    public string QBInvoiceNumber { get; set; }

    // Workflow flags
    public bool IsJobConverted { get; set; }  // Estimate converted to job
    public bool IsCancellationMailSend { get; set; }
    public bool IsCustomerMailSend { get; set; }
    public bool IsCustomerAvailable { get; set; }
    public decimal? EstimateWorth { get; set; }  // Estimate amount at time of scheduling

    // Recurring job support
    public long? ParentJobId { get; set; }
    [ForeignKey("ParentJobId")]
    public virtual JobScheduler JobScheduler1 { get; set; }  // Self-referencing FK

    // Person override (for meetings)
    public long? PersonId { get; set; }
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; }

    // Audit trail
    public long DataRecorderMetaDataId { get; set; }
    [CascadeEntity]
    [ForeignKey("DataRecorderMetaDataId")]
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

    // Navigation
    public virtual ICollection<JobNote> VacationNote { get; set; }
}
```

**Critical Constraints:**
- Exactly one of `JobId`, `EstimateId`, or `MeetingID` must be populated (business rule, not DB constraint)
- `IsVacation = true` means `AssigneeId` is blocked from other assignments during this period
- `ParentJobId` links recurring job instances to original scheduler
- `ActualStartDate/ActualEndDate` MUST be used for UI display, raw dates for queries

---

#### **BeforeAfterImages** - Photo documentation with rich metadata
```csharp
public class BeforeAfterImages : DomainBase
{
    // Image type
    public bool? IsBeforeImage { get; set; }  // true = before, false = after, null = other

    // Service classification
    public long? ServiceTypeId { get; set; }
    [ForeignKey("ServiceTypeId")]
    public virtual ServiceType ServiceType { get; set; }

    public long? TypeId { get; set; }  // Lookup FK
    [ForeignKey("TypeId")]
    public virtual Lookup Lookup { get; set; }

    public long? CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public virtual JobEstimateImageCategory JobEstimateImageCategory { get; set; }

    public long? MarkertingClassId { get; set; }
    [ForeignKey("MarkertingClassId")]
    public virtual MarketingClass MarketingClass { get; set; }

    // Pairing mechanism
    public long? PairId { get; set; }  // Links before/after images
    [ForeignKey("PairId")]
    public virtual JobEstimateServices JobEstimateImagePairing { get; set; }

    public long? ServiceId { get; set; }
    [ForeignKey("ServiceId")]
    public virtual JobEstimateServices JobEstimateServices { get; set; }

    // Surface metadata
    public string SurfaceType { get; set; }       // "Floor", "Countertop", "Shower", etc.
    public string SurfaceMaterial { get; set; }   // "Marble", "Granite", "Travertine", etc.
    public string FinishMaterial { get; set; }    // "Polished", "Honed", "Leathered", etc.
    public string SurfaceColor { get; set; }      // "White Carrara", "Black Galaxy", etc.

    // Location metadata
    public string BuildingLocation { get; set; }  // "Main lobby", "Master bathroom", etc.
    public int FloorNumber { get; set; }
    public string CompanyName { get; set; }
    public string PropertyManager { get; set; }
    public string MaidService { get; set; }
    public string MAIDJANITORIAL { get; set; }

    // File references
    public long? FileId { get; set; }
    [ForeignKey("FileId")]
    public virtual File File { get; set; }  // Full-resolution image

    public long? ThumbFileId { get; set; }
    [ForeignKey("ThumbFileId")]
    public virtual File ThumbFile { get; set; }  // Thumbnail (500x500 max)

    public string ImageUrl { get; set; }      // Relative path
    public string S3BucketURL { get; set; }   // Cloud storage URL

    // Scheduler linkage
    public long? SchedulerId { get; set; }
    [ForeignKey("SchedulerId")]
    public virtual JobScheduler JobScheduler { get; set; }

    public long? JobId { get; set; }
    [ForeignKey("JobId")]
    public virtual JobScheduler JobSchedulerJobId { get; set; }

    public long? EstimateId { get; set; }
    [ForeignKey("EstimateId")]
    public virtual JobScheduler JobSchedulerEstimateId { get; set; }

    // Franchisee ownership
    public long? FranchiseeId { get; set; }
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }

    // Uploader tracking
    public long? UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual Person Person { get; set; }

    public string PersonName { get; set; }  // Snapshot of name at upload time

    public long? RoleId { get; set; }
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; }

    // Gallery/marketing flags
    public bool IsBestImage { get; set; }  // Promoted to marketing gallery
    public DateTime? BestFitMarkDateTime { get; set; }

    public bool IsAddToLocalGallery { get; set; }  // Franchisee local portfolio
    public DateTime? AddToGalleryDateTime { get; set; }

    public bool IsImageCropped { get; set; }
    public bool IsImagePairReviewed { get; set; }

    // Audit trail
    public long? DataRecorderMetaDataId { get; set; }
    [ForeignKey("DataRecorderMetaDataId")]
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

**Critical Constraints:**
- Before/after pairs linked via `PairId` (references `JobEstimateServices.Id`)
- `IsBeforeImage` NULL indicates non-standard image type (exterior, invoice scan, etc.)
- Thumbnails generated asynchronously - `ThumbFileId` may be NULL immediately after upload
- `IsBestImage` flag requires admin review approval before marketing use
- Multiple foreign keys to `JobScheduler` (SchedulerId, JobId, EstimateId) - only one populated per image

---

#### **Meeting** - Calendar meetings/events
```csharp
public class Meeting : DomainBase
{
    public DateTime StartDate { get; set; }  // UTC
    public DateTime EndDate { get; set; }
    public double? Offset { get; set; }  // Timezone offset
    public DateTime StartDateTimeString { get; set; }
    public DateTime EndDateTimeString { get; set; }

    public string Title { get; set; }

    // Hierarchical meetings
    public long? ParentId { get; set; }
    [ForeignKey("ParentId")]
    public Meeting Parent { get; set; }  // Self-referencing FK for recurring meetings

    // Special flags
    public bool IsEquipment { get; set; }  // Equipment reservation vs person meeting
}
```

**Critical Constraints:**
- Linked to `JobScheduler` via `JobScheduler.MeetingID`
- `ParentId` enables recurring meeting series (all instances reference original)
- `IsEquipment = true` indicates equipment/resource reservation, not person attendance

---

#### **TechnicianWorkOrder** - Work order type configuration
```csharp
public class TechnicianWorkOrder : DomainBase
{
    public string Name { get; set; }  // Display name
    public bool IsActive { get; set; }  // Soft delete flag

    public long? WorkOrderId { get; set; }
    [ForeignKey("WorkOrderId")]
    public virtual Lookup WorkOrder { get; set; }  // References Lookup table
}
```

**Purpose**: Defines available work order types for invoice line items (FloorDiamond, HandDiamond, Pads, Brushes, Polish, etc.). Simple configuration entity.

---

### Supporting Entities (Brief Overview)

#### **JobCustomer** - Customer information
- Name, email, phone, address
- Links to `Customer` entity (separate module)

#### **JobStatus** - Job workflow states
- Lookup entity: Created, Assigned, InProgress, Completed, Canceled, Tentative

#### **JobNote** - Comments on jobs/estimates/vacations
- Text field, user attribution, timestamp
- Foreign keys to `Job`, `JobEstimate`, or `JobScheduler` (vacation notes)

#### **JobEstimateServices** - Service line items
- Service description, quantity, unit price
- Dimensions (length, width, area)
- Links to `JobEstimate` or `Job`
- Used as `PairId` for before/after image pairing

#### **JobResource** - File attachments
- PDF invoices, documents, additional photos
- Links to `Job` or `JobEstimate`

#### **EstimateInvoiceAssignee** - Invoice assignments
- Many-to-many: Estimate/Invoice ↔ Technicians

#### **CustomerSignature** - Digital signatures
- Signature image, timestamp, IP address
- Pre-completion vs post-completion signatures

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

This is a domain model folder - **no public interfaces** defined here. Entities are consumed by service layer (`IJobService`, `IEstimateService`) and repository interfaces.

### Entity Access Patterns

```csharp
// Via repositories (injected into services)
public interface IJobRepository
{
    Job GetById(long id);
    IQueryable<Job> GetAll();
    void Add(Job job);
    void Update(Job job);
    void Delete(long id);  // Soft delete (sets IsDeleted = true)
}
```

**Critical Query Patterns:**

1. **Job with all schedulers and technicians:**
```csharp
var job = _jobRepository.GetAll()
    .Include(j => j.JobScheduler)
        .ThenInclude(js => js.OrganizationRoleUser)
    .Include(j => j.JobCustomer)
    .Include(j => j.JobStatus)
    .FirstOrDefault(j => j.Id == jobId);
```

2. **Technician availability check:**
```csharp
var conflicts = _jobSchedulerRepository.GetAll()
    .Where(js => js.AssigneeId == technicianId)
    .Where(js => js.StartDate < endDate && js.EndDate > startDate)
    .Where(js => js.IsDeleted == false)
    .ToList();
```

3. **Before/after image pairs for job:**
```csharp
var imagePairs = _beforeAfterImageRepository.GetAll()
    .Where(img => img.SchedulerId == schedulerId)
    .Where(img => img.IsDeleted == false)
    .GroupBy(img => img.PairId)
    .Select(g => new
    {
        PairId = g.Key,
        BeforeImage = g.FirstOrDefault(img => img.IsBeforeImage == true),
        AfterImage = g.FirstOrDefault(img => img.IsBeforeImage == false)
    })
    .ToList();
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Application.Domain** — `DomainBase` (Id, IsDeleted, timestamps)
- **Core.Application.Attribute** — `[CascadeEntity]` for delete behavior
- **Core.Organizations.Domain** — `OrganizationRoleUser` (technicians/sales reps), `Franchisee`
- **Core.Users.Domain** — `Person`, `Role`
- **Core.Sales.Domain** — `MarketingClass`, `ServiceType`, `JobEstimateServices`
- **Core.Billing.Domain** — `Invoice`, `Franchisee`
- **Core.FileManagement.Domain** — `File` entity for image storage
- **Core.Lookup.Domain** — `Lookup` table for extensible enumerations

### External Dependencies
- **Entity Framework Core** — ORM mapping, navigation properties, lazy loading
- **System.ComponentModel.DataAnnotations** — `[ForeignKey]`, `[NotMapped]` attributes
- **System.Collections.ObjectModel** — `Collection<T>` for navigation property initialization

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Critical Relationships to Understand

1. **JobScheduler is the scheduling hub**: Don't think of it as "just a calendar entry". It's the many-to-many resolution table connecting Jobs/Estimates to Technicians, with rich scheduling metadata.

2. **Estimate → Job is one-to-many, not one-to-one**: A single estimate can spawn multiple jobs (e.g., "Phase 1: Floors", "Phase 2: Countertops"). Always preserve `EstimateId` on Job for audit trail.

3. **Before/After image pairing is indirect**: Images don't directly reference each other. They share the same `PairId` (which is a `JobEstimateServices.Id`). Query pattern:
   ```csharp
   var beforeImages = images.Where(i => i.PairId == X && i.IsBeforeImage == true);
   var afterImages = images.Where(i => i.PairId == X && i.IsBeforeImage == false);
   ```

4. **Timezone handling is manual**: Database stores UTC (`StartDate`, `EndDate`). Display layer MUST use computed properties (`ActualStartDate`, `ActualEndDate`) or apply `Offset` manually. **Never** use raw dates in UI.

5. **JobScheduler.JobId vs BeforeAfterImages.JobId**: These reference DIFFERENT entities! `JobScheduler.JobId` → `Job` entity. `BeforeAfterImages.JobId` → `JobScheduler` entity (bad naming, historical artifact).

### Performance Traps

1. **Lazy loading hell**: `Job` → `JobScheduler` → `OrganizationRoleUser` → `Person` → `Address`. Always use `.Include()` for known navigation paths.

2. **BeforeAfterImages queries are expensive**: Images have 10+ foreign keys. Filter by `SchedulerId` or `FranchiseeId` FIRST, then join other tables.

3. **JobScheduler availability queries**: Index on `(AssigneeId, StartDate, EndDate, IsDeleted)` is critical. Missing this index causes table scans.

### Common Mistakes

1. **Forgetting soft deletes**: Always filter `IsDeleted == false` in queries. EF global query filters help but don't cover all scenarios.

2. **QBInvoiceNumber collisions**: Database has unique constraint. Always call `IJobService.IsValidQbNumber()` before setting this field.

3. **Orphaned JobScheduler records**: Deleting a Job doesn't cascade to JobScheduler. Service layer MUST explicitly delete related schedulers.

4. **Image thumbnail race condition**: `ThumbFileId` NULL immediately after upload. Background service generates thumbnails asynchronously. UI must handle missing thumbnails gracefully.

5. **Meeting vs Vacation confusion**: Both use `JobScheduler.IsVacation`. Differentiate via `MeetingID` (Meeting populated = meeting, NULL + IsVacation = true = vacation).

### Schema Evolution Gotchas

- `StartDateTimeString` / `EndDateTimeString` appear redundant but are used for specific UI frameworks that require DateTime objects (not computed properties).
- Multiple `JobScheduler` foreign keys on `BeforeAfterImages` exist because the entity evolved: originally tied to `SchedulerId`, then added `JobId`/`EstimateId` shortcuts. Only use `SchedulerId` in new code.
- `PersonName` on `BeforeAfterImages` is a denormalized snapshot - don't update if Person.Name changes (historical record).

<!-- END CUSTOM SECTION -->
