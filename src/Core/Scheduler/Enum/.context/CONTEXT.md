<!-- AUTO-GENERATED: Header -->
# Enum — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2026-02-10T15:01:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Enum folder defines **strongly-typed enumerations** for workflow states, calendar types, image categories, and system behaviors throughout the scheduler module. These enums enforce type safety at compile time and provide semantic meaning to integer fields stored in the database. Critical for preventing magic numbers in code and ensuring valid state transitions.

### Design Patterns
- **Explicit Value Assignment**: All enums use explicit integer values matching database Lookup table IDs or foreign key references
- **Semantic Naming**: Enum names describe business states (e.g., `InProgress`) not technical states (e.g., `State3`)
- **Workflow Progression**: Some enums represent sequential states (Created → Assigned → InProgress → Completed)
- **Multi-Purpose Enums**: `ConfirmationEnum` handles both success states and error states with descriptive names

### Usage Flow
1. **Job Lifecycle**: `JobStatusType` guides job from creation through completion
2. **Calendar Differentiation**: `ScheduleType` determines whether `JobScheduler` entry represents Job, Estimate, Meeting, or Vacation
3. **Customer Interaction**: `ConfirmationEnum` tracks customer confirmation responses
4. **Image Classification**: `BeforeAfterImagesType` categorizes photos (Before, After, During, Exterior, Invoice)
5. **Materials Tracking**: `TechnicianWorkOrderInvoiceType` defines materials/supplies used on jobs

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Job Workflow Enumerations

#### **JobStatusType** - Job lifecycle states
```csharp
public enum JobStatusType
{
    Created = 1,       // Job record created, not yet assigned to technician
    Assigned = 2,      // Technician(s) assigned, not yet started
    InProgress = 3,    // Work actively being performed
    Completed = 4,     // Job finished, ready for invoicing
    Canceled = 5,      // Job canceled by customer or franchisee
    Tentative = 6      // Preliminary scheduling, not confirmed
}
```

**Usage**: References `Job.StatusId` (foreign key to `JobStatus` lookup table)

**Workflow Rules**:
- Must progress sequentially (can't skip from Created → Completed)
- Canceled jobs cannot transition to other states
- Tentative jobs require customer confirmation before becoming Assigned

---

#### **ScheduleType** - Distinguishes calendar entry types
```csharp
public enum ScheduleType
{
    Job = 145,         // Actual work order
    Estimate = 146,    // Estimate appointment with customer
    Vacation = 148,    // Technician unavailability
    Meeting = 149      // Team meeting or equipment reservation
}
```

**Usage**: References `JobScheduler.SchedulerStatus` (foreign key to Lookup table)

**Critical Distinction**:
- `Job` = `JobScheduler.JobId` populated, billable work
- `Estimate` = `JobScheduler.EstimateId` populated, sales appointment
- `Vacation` = Blocks technician availability, no Job/Estimate link, `IsVacation = true`
- `Meeting` = `JobScheduler.MeetingID` populated, internal coordination

---

### Calendar & Scheduling Enumerations

#### **CalendarStatusType** - Event confirmation status
```csharp
public enum CalendarStatusType
{
    Confirmed = 1,     // Customer/technician confirmed attendance
    Tentative = 2      // Provisional scheduling, pending confirmation
}
```

---

#### **RepeatFrequency** - Recurring schedule patterns
```csharp
public enum RepeatFrequency
{
    Daily = 183,       // Every day (e.g., maintenance contracts)
    Weekly = 181,      // Every 7 days (e.g., weekly cleaning)
    Monthly = 182,     // Every month (e.g., monthly inspections)
    Custom = 184       // Custom recurrence rule (stored separately)
}
```

**Usage**: Determines repeat logic for recurring jobs, vacations, and meetings

---

### Customer Interaction Enumerations

#### **ConfirmationEnum** - Customer confirmation responses
```csharp
public enum ConfirmationEnum
{
    // Success states
    Confirmed = 218,           // Customer confirmed appointment
    AlreadyConfirmed = 2,      // Previously confirmed, duplicate request
    
    // Negative responses
    NotResponded = 216,        // No response yet from customer
    NotConfirmed = 217,        // Customer declined/canceled
    
    // Error states
    InvalidId = 4,             // Confirmation token/ID invalid
    ErrorInConfirming = 3,     // System error during confirmation
    PastScheduler = 5          // Attempted to confirm past appointment
}
```

**Usage**: Tracks customer email/SMS confirmation link responses

**Workflow**:
1. System sends confirmation request → `NotResponded`
2. Customer clicks link → `Confirmed` or `NotConfirmed`
3. Customer clicks again → `AlreadyConfirmed`
4. Link expired/invalid → `InvalidId` or `PastScheduler`

---

### Image Documentation Enumerations

#### **BeforeAfterImagesType** - Photo categorization
```csharp
public enum BeforeAfterImagesType
{
    Before = 203,              // Pre-work condition
    After = 204,               // Post-work condition
    During = 205,              // In-progress documentation
    ExteriorBuilding = 206,    // External building shots (context)
    Invoice = 207              // Scanned/photographed invoice documents
}
```

**Usage**: References `BeforeAfterImages.TypeId` (FK to Lookup table)

**Note**: `BeforeAfterImages.IsBeforeImage` boolean provides primary before/after distinction. This enum adds granularity.

---

#### **BeforeAfterPairType** - Image pairing status
```csharp
public enum BeforeAfterPairType
{
    // Values determined by business logic analysis
    Paired,         // Before and after images linked via PairId
    UnpairedBefore, // Before image without corresponding after
    UnpairedAfter,  // After image without corresponding before
    Standalone      // Image not part of before/after workflow
}
```

---

#### **BeforeAfterBestPairType** - Marketing gallery selection
```csharp
public enum BeforeAfterBestPairType
{
    // Values determined by business logic analysis
    BestPair,       // Promoted to marketing gallery (IsBestImage = true)
    LocalGallery,   // Franchisee-specific gallery (IsAddToLocalGallery = true)
    NotSelected     // Standard documentation only
}
```

---

### Materials & Invoice Enumerations

#### **TechnicianWorkOrderInvoiceType** - Materials and services
```csharp
public enum TechnicianWorkOrderInvoiceType
{
    FloorDiamond = 269,        // Floor diamond grinding pads
    HandDiamond = 270,         // Hand-held diamond tools
    Pads = 271,                // Buffing/polishing pads
    Brushes = 272,             // Scrub brushes
    Polish = 273,              // Polishing compounds
    Grout = 274,               // Grout/caulk materials
    Sealer = 275,              // Stone sealers
    Coating = 276,             // Surface coatings
    Chips = 277,               // Stone repair chips/fill
    Stripping = 278,           // Chemical strippers
    Kits = 279,                // Tool/material kits
    Cleaner = 280,             // Cleaning solutions
    CareProducts = 281         // Customer care products
}
```

**Usage**: References `TechnicianWorkOrder.WorkOrderId` (FK to Lookup table)

**Purpose**: Defines billable materials/supplies used during job execution for invoice line items

---

### Supporting Enumerations

#### **SignatureType** - Digital signature capture points
```csharp
public enum SignatureType
{
    PreCompletion = 289,       // Customer signature before work begins
    PostCompletion = 290       // Customer signature after work completed
}
```

---

#### **DragDropSchedulerEnum** - Drag-and-drop operation results
```csharp
public enum DragDropSchedulerEnum
{
    Changed = 1,               // Successfully rescheduled
    Error = 2,                 // Technician unavailable (conflict detected)
    AlreadyAssigned = 3        // Technician already assigned to same job
}
```

---

#### **EmailEnum** - Notification types (partial list)
```csharp
public enum EmailEnum
{
    WeeklyLateFees,            // Automated late fee notices
    UnpaidInvoices,            // Overdue payment reminders
    CustomerFeedback,          // Post-job satisfaction surveys
    MonthlyReports,            // Franchisee monthly summaries
    ARReports,                 // Accounts receivable reports
    // ... additional notification types
}
```

---

#### **DebuggerLogType** - System logging categories
```csharp
public enum DebuggerLogType
{
    Info,
    Warning,
    Error,
    Critical
}
```

---

#### **ZipParsingType** - Calendar import file processing
```csharp
public enum ZipParsingType
{
    Pending,       // File uploaded, awaiting processing
    Processing,    // Currently being parsed
    Completed,     // Successfully imported
    Failed         // Parsing error occurred
}
```

---

#### **MeasurementEnum**, **LanguageEnum**, **NonResidentalClassEnum** - Utility enums
- **MeasurementEnum**: Square feet, linear feet, per unit, etc.
- **LanguageEnum**: English, Spanish, etc. for multi-language support
- **NonResidentalClassEnum**: Commercial property classifications

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

No public interfaces - enums are **value types** consumed throughout the codebase.

### Common Usage Patterns

#### Status Transitions (Service Layer)
```csharp
public async Task StartJobAsync(long jobId)
{
    var job = await _jobRepository.GetById(jobId);
    
    if (job.StatusId != (long)JobStatusType.Assigned)
    {
        throw new InvalidOperationException("Job must be Assigned before starting");
    }
    
    job.StatusId = (long)JobStatusType.InProgress;
    await _unitOfWork.SaveChangesAsync();
}
```

#### Filtering Queries
```csharp
// Get all in-progress jobs
var activeJobs = await _jobRepository.GetAll()
    .Where(j => j.StatusId == (long)JobStatusType.InProgress)
    .ToListAsync();

// Get technician vacations
var vacations = await _jobSchedulerRepository.GetAll()
    .Where(js => js.SchedulerStatus == (long)ScheduleType.Vacation)
    .Where(js => js.IsVacation == true)
    .ToListAsync();
```

#### Confirmation Processing
```csharp
public ConfirmationEnum ProcessCustomerConfirmation(string token, long schedulerId)
{
    var scheduler = _jobSchedulerRepository.GetById(schedulerId);
    
    if (scheduler == null)
        return ConfirmationEnum.InvalidId;
    
    if (scheduler.StartDate < DateTime.UtcNow)
        return ConfirmationEnum.PastScheduler;
    
    if (scheduler.IsCustomerConfirmed)
        return ConfirmationEnum.AlreadyConfirmed;
    
    try
    {
        scheduler.IsCustomerConfirmed = true;
        _unitOfWork.SaveChanges();
        return ConfirmationEnum.Confirmed;
    }
    catch
    {
        return ConfirmationEnum.ErrorInConfirming;
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Scheduler.Domain** — Enums consumed by entity properties (Job.StatusId, JobScheduler.SchedulerStatus, etc.)
- **Core.Scheduler.ViewModel** — ViewModel validation and business logic
- **Core.Scheduler.Impl** — Service layer state transitions and filtering

### External Dependencies
- **Core.Lookup.Domain** — Many enum values reference Lookup table primary keys (explicit value assignment pattern)
- **System** — Base Enum type

### Database Mapping
Most enums map to database Lookup table entries. The enum integer values **must match** the Lookup.Id primary keys. Mismatch causes foreign key constraint violations.

```sql
-- Example Lookup table entries
INSERT INTO Lookup (Id, Type, Name) VALUES
(1, 'JobStatus', 'Created'),
(2, 'JobStatus', 'Assigned'),
(3, 'JobStatus', 'InProgress'),
(4, 'JobStatus', 'Completed'),
(5, 'JobStatus', 'Canceled'),
(6, 'JobStatus', 'Tentative');

INSERT INTO Lookup (Id, Type, Name) VALUES
(145, 'ScheduleType', 'Job'),
(146, 'ScheduleType', 'Estimate'),
(148, 'ScheduleType', 'Vacation'),
(149, 'ScheduleType', 'Meeting');
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Critical Rules for Enum Values

1. **Never change enum integer values after deployment**: These values are persisted in database. Changing `Completed = 4` to `Completed = 10` breaks all existing jobs.

2. **Adding new enum values**: Append new values, don't insert in middle. If you add `Rescheduled = 7` to `JobStatusType`, use next available ID.

3. **Lookup table synchronization**: When adding enum values, also update Lookup table:
   ```sql
   INSERT INTO Lookup (Id, Type, Name, IsActive)
   VALUES (7, 'JobStatus', 'Rescheduled', 1);
   ```

4. **Explicit casting required**: Database stores `long`, enum is `int`. Always cast:
   ```csharp
   job.StatusId = (long)JobStatusType.Completed;  // CORRECT
   job.StatusId = JobStatusType.Completed;        // WRONG (won't compile)
   ```

### Enum Value Gaps Explained

Notice `ScheduleType` values are not sequential (145, 146, 148, 149 - skips 147). This is because:
- Values reference pre-existing Lookup table IDs
- Lookup table shared across multiple modules
- ID 147 may be used by different module's enum

**Don't "fix" gaps** - they're intentional to maintain database referential integrity.

### ConfirmationEnum Design Rationale

Why mix success/error states in same enum instead of separate enums or throwing exceptions?

**Answer**: Customer-facing API must return all possible outcomes without exceptions:
```csharp
// Customer clicks email link
public IHttpActionResult ConfirmSchedule(string token, long id)
{
    var result = _jobService.ConfirmSchedule(token, id);
    
    switch (result)
    {
        case ConfirmationEnum.Confirmed:
            return View("Success");
        case ConfirmationEnum.AlreadyConfirmed:
            return View("AlreadyConfirmed");  // Friendly message, not error page
        case ConfirmationEnum.PastScheduler:
            return View("Expired");
        // ... etc
    }
}
```

No try/catch needed, all outcomes handled gracefully.

### Enum vs Lookup Table Trade-offs

**Use Enum when**:
- Values rarely change (workflow states)
- Need compile-time type safety
- Values referenced frequently in code (performance)

**Use Lookup table only when**:
- Franchisees customize values
- Values change frequently
- Values added by end users via UI

Most enums in this folder are **hybrid**: Enum for type safety, Lookup table for database storage/referential integrity.

<!-- END CUSTOM SECTION -->
