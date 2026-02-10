<!-- AUTO-GENERATED: Header -->
# Scheduler — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2026-02-10T14:59:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Scheduler module is the comprehensive job management system for MarbleLife, orchestrating the complete lifecycle of customer jobs from initial estimate through completion. It manages technician assignments, calendar scheduling, work order generation, customer notifications, before/after photo documentation, invoice attachments, and QuickBooks integration. This is not a simple calendar - it's a multi-tenant franchise scheduling engine with complex business rules for recurring jobs, vacation tracking, meeting coordination, and sales tax calculation.

### Design Patterns
- **Service Layer Pattern**: 29 interface contracts define clear boundaries between business logic and implementation
- **Factory Pattern**: Multiple factories (`IJobFactory`, `IJobDetailsFactory`, `IEstimateInvoiceFactory`, `IGeoCodeFactory`) for complex domain object creation with proper initialization
- **Repository Pattern**: Domain entities accessed through repositories, enforcing separation from data layer
- **Notification Observer Pattern**: Multiple notification services (`ISendNewJobNotificationtoTechService`, `IJobReminderNotificationtoUsersService`) react to scheduling events
- **Polling Agent Pattern**: `ICalendarParsePollingAgent` periodically processes calendar file imports asynchronously
- **DTO/ViewModel Pattern**: Clean separation between domain entities (Domain/) and data transfer objects (ViewModel/)

### Data Flow

#### **Estimate → Job Conversion Flow**
1. Customer request enters via `IEstimateService.Save(JobEstimateEditModel)` creating a `JobEstimate` entity
2. Estimate validated for franchisee territory, service types, and customer address via `IGeoCodeService`
3. Sales rep schedules estimate appointment using `IEstimateService.SaveSchedule()` creating `JobScheduler` entries
4. When customer approves, estimate converts to `Job` via `IJobService.Save(JobEditModel)` with `EstimateId` reference
5. Job assigned to technicians through `JobScheduler` entities with `AssigneeId` (OrganizationRoleUser)

#### **Job Scheduling & Assignment Flow**
1. `IJobService.CheckAvailability()` validates technician availability against existing schedules and vacations
2. `IJobSchedulerService.ChangeSchedulerDateTime()` handles drag-drop reschedule operations with conflict detection
3. `ISendNewJobNotificationtoTechService` sends push notifications to assigned technicians
4. `IJobReminderNotificationtoUsersService` creates automated reminder notifications (±2 days from job start)
5. Customer receives confirmation request via `IJobService.ConfirmSchedule()` with unique token

#### **Job Execution & Documentation Flow**
1. Technician arrives, system tracks via `IJobService.UpdateInfo()` updating QuickBooks invoice number
2. Before photos captured via `IJobService.SaveJobEstimateMediaFiles()` creating `BeforeAfterImages` entities
3. Work performed, after photos saved with surface type, material, and location metadata
4. `IBeforeAfterThumbNailService.CreateImageThumb()` generates optimized thumbnails
5. Best image pairs flagged for marketing via `IBeforeAfterImageService.GetBeforeAfterImagesForFranchiseeAdmin()`
6. Status updated to Completed via `IJobService.ChangeStatus(jobId, JobStatusType.Completed)`

#### **Invoice & Financial Flow**
1. `IEstimateInvoiceServices` manages line items, dimensions, and material charges
2. `ISalesTaxAPIServices` calculates sales tax based on county and service type
3. `IAttachingInvoicesServices` links PDF invoices to jobs/estimates
4. `IEstimateService.SendMailToMember()` emails invoice to customer with signature capability
5. QuickBooks integration via QBInvoiceNumber tracking on `Job` and `JobScheduler` entities

#### **Calendar Import Flow**
1. External calendar files uploaded via `ICalendarImportService.Save(CalendarImportModel)`
2. Files stored with timestamped filenames in media location
3. `ICalendarParsePollingAgent.ParseCalendarFile()` polls for unprocessed files
4. iCalendar (.ics) format parsed using Ical.Net library
5. Events converted to `Job` or `JobEstimate` records based on type detection
6. Timezone offsets applied via `ITimeZoneInformationRepository`

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Domain Entities
Located in [Domain/.context/CONTEXT.md](Domain/.context/CONTEXT.md)

```csharp
// Job - Actual work order
public class Job : DataRecorderMetaData
{
    public long JobTypeId { get; set; }  // FK to MarketingClass
    public long StatusId { get; set; }    // FK to JobStatus
    public long? EstimateId { get; set; } // Optional parent estimate
    public long CustomerId { get; set; }  // FK to JobCustomer
    public string QBInvoiceNumber { get; set; }  // QuickBooks integration
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string GeoCode { get; set; }
    public string Description { get; set; }
    
    // Navigation
    public virtual ICollection<JobScheduler> JobSchedulers { get; set; }
    public virtual ICollection<JobNote> JobNotes { get; set; }
}

// JobEstimate - Quote/preliminary estimate
public class JobEstimate : DataRecorderMetaData
{
    public decimal? Amount { get; set; }
    public int? EstimateHour { get; set; }
    public long CustomerId { get; set; }
    public long TypeId { get; set; }  // FK to MarketingClass
    public long? ParentEstimateId { get; set; }  // Hierarchical estimates
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string GeoCode { get; set; }
    
    // Navigation
    public virtual ICollection<Job> Jobs { get; set; }
    public virtual ICollection<JobNote> JobNotes { get; set; }
}

// JobScheduler - Calendar entry linking jobs/estimates to technicians
public class JobScheduler : DataRecorderMetaData
{
    public long? JobId { get; set; }
    public long? EstimateId { get; set; }
    public long? AssigneeId { get; set; }  // FK to OrganizationRoleUser (technician)
    public long? SalesRepId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long? SchedulerStatus { get; set; }  // FK to Lookup
    public long FranchiseeId { get; set; }
    public bool IsVacation { get; set; }
    public bool IsRepeat { get; set; }
    public bool IsInvoiceRequired { get; set; }
    public bool IsJobConverted { get; set; }
    public decimal? EstimateWorth { get; set; }
    public int? Offset { get; set; }  // Timezone offset
    public string Title { get; set; }
}

// BeforeAfterImages - Photo documentation
public class BeforeAfterImages : DataRecorderMetaData
{
    public bool IsBeforeImage { get; set; }
    public long? ServiceTypeId { get; set; }
    public long? PairId { get; set; }  // FK to JobEstimateServices
    public string SurfaceColor { get; set; }
    public string FinishMaterial { get; set; }
    public string SurfaceMaterial { get; set; }
    public string SurfaceType { get; set; }
    public string FloorNumber { get; set; }
    public string BuildingLocation { get; set; }
    public bool IsBestImage { get; set; }  // Marketing gallery flag
    public bool IsAddToLocalGallery { get; set; }
    public long? FileId { get; set; }
    public long? ThumbFileId { get; set; }
    public string S3BucketURL { get; set; }
    public bool IsImageCropped { get; set; }
    public bool IsImagePairReviewed { get; set; }
    public long? SchedulerId { get; set; }  // FK to JobScheduler
}

// TechnicianWorkOrder - Work order type configuration
public class TechnicianWorkOrder : DataRecorderMetaData
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public long WorkOrderId { get; set; }  // FK to Lookup
}
```

### Key Enumerations
Located in [Enum/.context/CONTEXT.md](Enum/.context/CONTEXT.md)

```csharp
// Job lifecycle states
public enum JobStatusType
{
    Created = 1,
    Assigned = 2,
    InProgress = 3,
    Completed = 4,
    Canceled = 5,
    Tentative = 6
}

// Schedulable item types
public enum ScheduleType
{
    Job = 145,
    Estimate = 146,
    Vacation = 148,
    Meeting = 149
}

// Customer confirmation states
public enum ConfirmationEnum
{
    NotResponded = 216,
    NotConfirmed = 217,
    Confirmed = 218,
    AlreadyConfirmed = 2,
    ErrorInConfirming = 3,
    InvalidId = 4,
    PastScheduler = 5
}

// Image types
public enum BeforeAfterImagesType
{
    Before = 203,
    After = 204,
    During = 205,
    ExteriorBuilding = 206,
    Invoice = 207
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `IJobService` - Core Job Management
Primary service for job CRUD operations, scheduling validation, and image management.

#### `void Save(JobEditModel model)`
- **Input**: `JobEditModel` with job details, tech assignments, customer info, scheduler list, invoice data
- **Output**: None (throws exceptions on failure)
- **Behavior**: 
  - Validates QuickBooks invoice number uniqueness
  - Creates/updates `Job` entity via `IJobFactory`
  - Manages `JobScheduler` collection (creates, updates, deletes based on diff)
  - Handles technician assignment changes
  - Triggers notification services for assigned technicians
  - Validates franchisee territory boundaries
- **Side Effects**: 
  - Database persist via repositories
  - Email/push notifications to technicians
  - Updates related `JobEstimate` if converted from estimate

#### `bool CheckAvailability(long jobId, long techId, DateTime startDate, DateTime endDate, bool isVacation)`
- **Input**: Job ID, technician ID, date range, vacation flag
- **Output**: `true` if technician available, `false` if conflict exists
- **Behavior**: 
  - Queries existing `JobScheduler` records for technician in date range
  - Excludes current job from conflict detection
  - Checks vacation schedules if `isVacation = false`
- **Side Effects**: None (read-only)

#### `DragDropSchedulerEnum SaveDragDropEvent(DragDropSchedulerModel model)`
- **Input**: `DragDropSchedulerModel` with scheduler ID, new tech ID, new dates
- **Output**: Enum result (Changed | Error | AlreadyAssigned)
- **Behavior**:
  - Validates new technician not already assigned to same job
  - Checks availability with `CheckAvailability()`
  - Updates `JobScheduler.AssigneeId` and dates via `IJobSchedulerService`
  - Sends cancellation email to old technician
  - Sends new assignment email to new technician
- **Side Effects**: Database update, email notifications

#### `IEnumerable<BeforeAfterImageModel> SaveJobEstimateMediaFiles(FileUploadModel model)`
- **Input**: `FileUploadModel` with uploaded files, job/estimate ID, user ID, service type metadata
- **Output**: Collection of `BeforeAfterImageModel` with image IDs and URLs
- **Behavior**:
  - Saves files via `IFileService` to media location
  - Creates `BeforeAfterImages` entities with metadata (surface type, material, location)
  - Generates thumbnails via `IBeforeAfterThumbNailService`
  - Links images to `JobScheduler` and `JobEstimateServices`
  - Determines before/after type based on `IsBeforeImage` flag
- **Side Effects**: File system writes, database inserts, thumbnail generation

#### `bool ChangeStatus(long jobId, long statusId)`
- **Input**: Job ID, new status ID (from `JobStatusType` enum)
- **Output**: `true` if successful
- **Behavior**:
  - Validates job exists
  - Updates `Job.StatusId`
  - Triggers workflow actions based on status (e.g., completion emails)
- **Side Effects**: Database update, potential notifications

---

### `IEstimateService` - Estimate & Calendar Management
Manages estimates, meetings, vacations, and recurring schedules.

#### `void Save(JobEstimateEditModel model)`
- **Input**: `JobEstimateEditModel` with estimate details, customer info, scheduler list, signatures
- **Output**: None (throws exceptions on failure)
- **Behavior**:
  - Creates/updates `JobEstimate` entity
  - Manages `JobScheduler` entries for estimate appointments
  - Processes digital signatures via `CustomerSignature` entities
  - Handles invoice line items through `JobEstimateServices`
  - Calculates sales tax via `ISalesTaxAPIServices`
  - Updates estimate status based on approval/rejection
- **Side Effects**: Database persist, email notifications via `SendingUpdationMails()`

#### `void SaveVacation(JobEstimateEditModel model)`
- **Input**: `JobEstimateEditModel` with vacation details (start/end date, assigned tech)
- **Output**: None
- **Behavior**:
  - Creates `JobScheduler` with `IsVacation = true`, `ScheduleType = Vacation`
  - Blocks technician availability for date range
  - Supports recurring vacations via `IsRepeat` flag
- **Side Effects**: Database insert, affects availability checks

#### `void RepeatVacation(VacationRepeatEditModel model)`
- **Input**: `VacationRepeatEditModel` with repeat frequency (daily/weekly/monthly/custom), occurrence count
- **Output**: None
- **Behavior**:
  - Creates multiple `JobScheduler` entries based on frequency
  - Links all occurrences to parent vacation via `ParentId`
  - Validates no conflicts with existing vacations
- **Side Effects**: Batch database inserts

#### `long SaveMeeting(JobEstimateEditModel model)`
- **Input**: `JobEstimateEditModel` with meeting details (attendees, date/time, title)
- **Output**: Meeting ID (primary key)
- **Behavior**:
  - Creates `Meeting` entity
  - Creates `JobScheduler` entries for each attendee with `ScheduleType = Meeting`
  - Supports equipment meetings via `IsEquipment` flag
- **Side Effects**: Database inserts, calendar entries for all attendees

---

### `IJobSchedulerService` - Scheduler Date Management
Handles datetime updates for scheduled items with timezone awareness.

#### `void ChangeSchedulerDateTime(JobEditModel model, DatetimeModel dateTimeModel, List<OldSchedulerModel> oldSchedulerList)`
- **Input**: Job model, new datetime, list of old scheduler records for comparison
- **Output**: None
- **Behavior**:
  - Calculates date/time differences from old to new
  - Updates all related `JobScheduler` entries (maintains relative timing for multi-tech jobs)
  - Handles parent-child scheduler relationships for recurring jobs
  - Applies timezone offsets
- **Side Effects**: Bulk database updates

---

### `IBeforeAfterImageService` - Image Retrieval & Filtering
Retrieves before/after images with complex filtering for franchisee admin review.

#### `BeforeAfterForImageViewModel GetBeforeAfterImagesForFranchiseeAdmin(BeforeAfterImageFilter filter)`
- **Input**: `BeforeAfterImageFilter` with franchisee ID, date range, service type, building type filters
- **Output**: `BeforeAfterForImageViewModel` hierarchical structure (Franchisee → Person → Scheduler → Image Pairs)
- **Behavior**:
  - Filters images by marketing class, service type (residential/commercial)
  - Groups images into before/after pairs based on `PairId` and `IsBeforeImage` flag
  - Includes unpaired images separately
  - Generates Base64 encoded images for inline display
  - Calculates S3 bucket URLs for full-res downloads
  - Filters out deleted images
- **Side Effects**: None (read-only, but intensive database/file operations)

---

### `IBeforeAfterThumbNailService` - Thumbnail Generation

#### `FileBase CreateImageThumb(FileBase filebase, string mediaFolder)`
- **Input**: Original image file metadata, media folder path
- **Output**: Thumbnail `FileBase` entity with new file ID
- **Behavior**:
  - Loads image from file system
  - Calculates aspect-ratio-preserving dimensions (500x500 max)
  - Resizes using high-quality bicubic interpolation
  - Saves optimized thumbnail with 85% JPEG quality
  - Creates `FileBase` entity pointing to thumbnail location
- **Side Effects**: File system write (new thumbnail file), database insert

---

### `ICalendarImportService` - Calendar File Upload

#### `void Save(CalendarImportModel model)`
- **Input**: `CalendarImportModel` with uploaded file, franchisee ID, type (Job/Estimate)
- **Output**: None
- **Behavior**:
  - Validates file extension (.ics supported)
  - Generates timestamped filename (yyyyMMdd_HHmmss_original.ics)
  - Moves file to media location
  - Creates `CalendarFileUpload` database record
  - Sets initial state for polling agent to process
- **Side Effects**: File move, database insert

---

### `ICalendarParsePollingAgent` - Calendar File Processor

#### `void ParseCalendarFile()`
- **Input**: None (polls database for unprocessed files)
- **Output**: None
- **Behavior**:
  - Queries `CalendarFileUpload` for files in 'uploaded' state
  - Parses .ics format using Ical.Net library
  - Extracts events with start/end dates, attendees, descriptions
  - Determines type (Job/Estimate) based on sales rep assignment
  - Creates `Job` or `JobEstimate` entities via appropriate services
  - Updates file state to 'processed' on success, 'error' on failure
  - Applies timezone offsets from `ITimeZoneInformationRepository`
- **Side Effects**: Database writes (jobs/estimates), file state updates

---

### `ISendNewJobNotificationtoTechService` - Technician Notifications

#### `void SendJobNotificationtoTech(DateTime date)`
- **Input**: Target date for notification search
- **Output**: None
- **Behavior**:
  - Queries `JobScheduler` for jobs/estimates on specified date
  - Filters by franchisee feature flag: `NewJobNotificationToTechAndSales`
  - Retrieves technician phone numbers and names from `OrganizationRoleUser`
  - Generates notification messages via `IUserNotificationModelFactory`
  - Sends push notifications/SMS to technician devices
- **Side Effects**: External notification API calls

---

### `IEstimateInvoiceServices` - Invoice Line Item Management

#### Methods (partial list):
- `void Save(EstimateInvoiceEditModel model)` - Save invoice line items with dimensions
- `EstimateInvoiceEditModel Get(long invoiceId)` - Retrieve invoice details
- `bool Delete(long invoiceId)` - Soft delete invoice line
- Invoice includes: service description, quantity, unit price, dimensions, images, notes

---

### `ISalesTaxAPIServices` - Sales Tax Calculation

#### `SalesTaxRatesEditModel GetSalesTax(string zipCode, string county)`
- **Input**: ZIP code, county name
- **Output**: `SalesTaxRatesEditModel` with tax rate percentage
- **Behavior**:
  - Queries `SalesTaxRates` table by ZIP + county
  - Falls back to county-only match if ZIP not found
  - Returns 0% if no match (non-taxable region)
- **Side Effects**: None (read-only)

---

### `IAttachingInvoicesServices` - Invoice PDF Attachment

#### `bool Save(AttachInvoiceWithJobViewModel model)`
- **Input**: `AttachInvoiceWithJobViewModel` with job ID, PDF file, invoice metadata
- **Output**: `true` if successful
- **Behavior**:
  - Saves PDF via `IFileService`
  - Links file to `Job` via `JobResource` entity
  - Updates `Job.QBInvoiceNumber` if provided
- **Side Effects**: File system write, database inserts

---

### `IGeoCodeService` - Address Geocoding

#### Methods:
- `GeoCodeResultModel GeocodeAddress(string address)` - Convert address to lat/lng
- Uses external geocoding API (Google Maps or similar)
- Stores result in `GeoCode` field on Job/Estimate entities
- Used for territory validation and map visualization

---

### `IJobFactory`, `IJobDetailsFactory`, `IEstimateInvoiceFactory` - Domain Object Factories
Factory interfaces following Factory Pattern for complex entity initialization with proper defaults, validation, and relationship setup.

---

### `IWorkOrderTechnicianService` - Work Order Type Management

#### Methods:
- `List<TechnicianWorkOrder> GetAll()` - Get active work order types
- `void Save(TechnicianWorkOrderEditModel model)` - Create/update work order type
- Used for configuring available work order categories (floor diamond, hand diamond, pads, brushes, etc.)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **[Domain](Domain/.context/CONTEXT.md)** — Core domain entities (Job, JobEstimate, JobScheduler, BeforeAfterImages, TechnicianWorkOrder, Meeting)
- **[Enum](Enum/.context/CONTEXT.md)** — Type enumerations (JobStatusType, ScheduleType, ConfirmationEnum, BeforeAfterImagesType)
- **[ViewModel](ViewModel/.context/CONTEXT.md)** — Data transfer objects for API operations (JobEditModel, JobEstimateEditModel, BeforeAfterImageViewModel)
- **[Impl](Impl/.context/CONTEXT.md)** — Service implementations with business logic
- **Core.Organizations** — `OrganizationRoleUser` for technician/sales rep management
- **Core.Notification** — Email/push notification infrastructure
- **Core.FileManagement** — File storage and retrieval (`IFileService`, `FileBase`)
- **Core.Lookup** — Lookup tables for job status, scheduler status, marketing classes

### External Dependencies
- **Ical.Net** — iCalendar (.ics) file parsing for calendar imports
- **System.Drawing** — Image manipulation for thumbnail generation
- **QuickBooks Integration** — QBInvoiceNumber field for accounting synchronization
- **AWS S3** — Cloud storage for before/after images (S3BucketURL field)
- **SMS/Push Notification Service** — Technician notification delivery
- **Geocoding API** — Address to lat/lng conversion for territory validation

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Critical Business Rules
1. **Technician Availability**: Always call `CheckAvailability()` before assigning jobs to prevent double-booking
2. **Estimate-to-Job Conversion**: When converting estimate to job, `EstimateId` must be preserved for audit trail
3. **QuickBooks Sync**: QBInvoiceNumber must be unique across all jobs (validated by `IsValidQbNumber()`)
4. **Timezone Handling**: All dates stored in UTC, display offset calculated via `Offset` field on `JobScheduler`
5. **Before/After Pairing**: Images paired via `PairId` referencing `JobEstimateServices.Id`, not direct before/after linkage
6. **Vacation Blocking**: Vacations are `JobScheduler` entries with `IsVacation = true`, counted in availability checks
7. **Multi-Tenant Isolation**: All queries must filter by `FranchiseeId` to prevent cross-franchise data leakage

### Performance Considerations
- **Image Loading**: `GetBeforeAfterImagesForFranchiseeAdmin()` is expensive - use pagination and date filters
- **Calendar Parsing**: Polling agent should run max once per minute to avoid file lock contention
- **Notification Batching**: `SendJobNotificationtoTech()` queries all jobs for a date - consider batching for high-volume days

### Common Pitfalls
1. **Forgetting Timezone Offsets**: Always use `ActualStartDate`/`ActualEndDate` for display, raw dates for database queries
2. **Scheduler vs Job Confusion**: A single `Job` can have multiple `JobScheduler` entries (multi-day jobs, recurring jobs)
3. **Image Thumbnail Lag**: Thumbnails generated asynchronously - UI must handle missing `ThumbFileId`
4. **Vacation vs Meeting Types**: Both use `JobScheduler`, differentiate via `ScheduleType` enum and `IsVacation` flag

<!-- END CUSTOM SECTION -->
