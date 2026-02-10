<!-- AUTO-GENERATED: Header -->
# Scheduler Module
> Comprehensive job scheduling and management system for MarbleLife franchise operations
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Scheduler module is the heart of MarbleLife's field operations, managing the entire lifecycle of customer jobs from initial estimate through completion and invoicing. Think of it as the air traffic control system for a multi-franchise service business - it coordinates when and where technicians work, ensures no double-bookings, tracks job progress through photos, manages customer communication, and handles financial workflows.

**Why This Module Exists:**
- **Multi-Tenant Scheduling**: Each franchisee operates independently but shares the same system infrastructure
- **Complex Availability Management**: Technicians have jobs, estimates, meetings, and vacations - all must be coordinated
- **Job Lifecycle Tracking**: From quote → scheduled estimate → approved job → technician assignment → work completion → invoicing
- **Visual Documentation**: Before/after photos are critical for quality control, marketing, and customer satisfaction
- **Integration Hub**: Connects to QuickBooks for invoicing, geocoding services for territory management, calendar imports from external systems, and notification services for technicians

**Key Design Decisions:**
1. **JobScheduler as Central Entity**: Rather than duplicating scheduling logic, `JobScheduler` handles jobs, estimates, meetings, and vacations uniformly
2. **Estimate-Centric Workflow**: Jobs are often created from estimates, maintaining the `EstimateId` link for audit trail and customer history
3. **Factory Pattern for Complexity**: Creating a properly initialized Job or Estimate requires validation, defaults, and relationship setup - factories encapsulate this complexity
4. **Async Calendar Processing**: Imported calendar files are parsed in background via polling agent to avoid blocking user operations
5. **Image Metadata Richness**: Before/after photos store extensive metadata (surface type, material, finish, location) to enable powerful filtering for franchisee admins and marketing teams

**Real-World Analogy:**
Imagine a hospital's surgery scheduling system. It must:
- Prevent surgeon double-booking (technician availability)
- Track OR preparation (estimate appointments)
- Coordinate surgical teams (multi-tech job assignments)
- Document procedures with photos (before/after images)
- Handle billing (QuickBooks integration)
- Notify staff of changes (technician notifications)

The Scheduler module does all of this for marble restoration and maintenance services.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating an Estimate
```csharp
// Inject service
private readonly IEstimateService _estimateService;

// Create estimate DTO
var estimateModel = new JobEstimateEditModel
{
    CustomerId = 12345,
    TypeId = 100,  // MarketingClass ID for service type
    Amount = 1250.00m,
    EstimateHour = 4,
    StartDate = DateTime.UtcNow.AddDays(7),
    EndDate = DateTime.UtcNow.AddDays(7).AddHours(4),
    FranchiseeId = 42,
    JobCustomer = new JobCustomerEditModel
    {
        FirstName = "John",
        LastName = "Smith",
        Email = "john@example.com",
        Phone = "555-1234",
        Address = new AddressEditModel
        {
            Address1 = "123 Main St",
            City = "Atlanta",
            StateId = 11,  // Georgia
            ZipCode = "30301"
        }
    },
    EstimateSchedulerList = new List<JobSchedulerEditModel>
    {
        new JobSchedulerEditModel
        {
            AssigneeId = 789,  // Sales rep user ID
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(7).AddHours(1),
            ScheduleType = (long)ScheduleType.Estimate
        }
    }
};

// Save estimate (creates JobEstimate + JobScheduler entries)
_estimateService.Save(estimateModel);
```

### Converting Estimate to Job
```csharp
// Inject service
private readonly IJobService _jobService;

// Load approved estimate
var estimate = _estimateService.Get(estimateId: 5678);

// Create job from estimate
var jobModel = new JobEditModel
{
    EstimateId = estimate.Id,  // Critical: links job to original estimate
    JobTypeId = estimate.TypeId,
    StatusId = (long)JobStatusType.Created,
    CustomerId = estimate.CustomerId,
    StartDate = DateTime.UtcNow.AddDays(14),
    EndDate = DateTime.UtcNow.AddDays(14).AddHours(6),
    Description = "Marble floor restoration - 2000 sqft",
    QBInvoiceNumber = "INV-2024-1234",
    FranchiseeId = estimate.FranchiseeId,
    TechIds = new List<long> { 101, 102 },  // Two technicians assigned
    JobSchedulerList = new List<JobSchedulerEditModel>
    {
        new JobSchedulerEditModel
        {
            AssigneeId = 101,  // First technician
            StartDate = DateTime.UtcNow.AddDays(14),
            EndDate = DateTime.UtcNow.AddDays(14).AddHours(6),
            ScheduleType = (long)ScheduleType.Job
        },
        new JobSchedulerEditModel
        {
            AssigneeId = 102,  // Second technician
            StartDate = DateTime.UtcNow.AddDays(14),
            EndDate = DateTime.UtcNow.AddDays(14).AddHours(6),
            ScheduleType = (long)ScheduleType.Job
        }
    }
};

// Check technician availability before saving
bool tech1Available = _jobService.CheckAvailability(
    jobId: 0,  // New job
    techId: 101,
    startDate: jobModel.StartDate.Value,
    endDate: jobModel.EndDate.Value,
    isVacation: false
);

bool tech2Available = _jobService.CheckAvailability(
    jobId: 0,
    techId: 102,
    startDate: jobModel.StartDate.Value,
    endDate: jobModel.EndDate.Value,
    isVacation: false
);

if (tech1Available && tech2Available)
{
    _jobService.Save(jobModel);
    // Notification service automatically sends assignment notifications to both techs
}
```

### Capturing Before/After Images
```csharp
// Inject services
private readonly IJobService _jobService;
private readonly IBeforeAfterThumbNailService _thumbnailService;

// Upload before images (technician arrives on site)
var beforeUpload = new FileUploadModel
{
    JobId = 9876,
    EstimateId = null,
    SchedulerId = 11111,
    UserId = 101,  // Technician ID
    IsBeforeImage = true,
    ServiceTypeId = 25,  // Marble floor restoration
    Files = new List<HttpPostedFileBase> { /* uploaded files */ },
    SurfaceType = "Floor",
    SurfaceMaterial = "Marble",
    FinishMaterial = "Polished",
    SurfaceColor = "White Carrara",
    BuildingLocation = "Main lobby",
    FloorNumber = "1st floor"
};

var beforeImages = _jobService.SaveJobEstimateMediaFiles(beforeUpload);
// Returns BeforeAfterImageModel[] with image IDs

// Later: upload after images (job complete)
var afterUpload = new FileUploadModel
{
    JobId = 9876,
    SchedulerId = 11111,
    UserId = 101,
    IsBeforeImage = false,
    ServiceTypeId = 25,
    PairId = beforeImages.First().ServiceId,  // Links to before image's service line
    Files = new List<HttpPostedFileBase> { /* uploaded files */ },
    // Same metadata as before
    SurfaceType = "Floor",
    SurfaceMaterial = "Marble",
    FinishMaterial = "Polished"
};

var afterImages = _jobService.SaveJobEstimateMediaFiles(afterUpload);
// System automatically generates thumbnails via IBeforeAfterThumbNailService
```

### Drag-and-Drop Rescheduling
```csharp
// Inject service
private readonly IJobService _jobService;

// User drags job from Tech A to Tech B on calendar UI
var dragDropModel = new DragDropSchedulerModel
{
    SchedulerId = 22222,
    NewAssigneeId = 103,  // Tech B
    NewStartDate = DateTime.UtcNow.AddDays(15),
    NewEndDate = DateTime.UtcNow.AddDays(15).AddHours(6)
};

var result = _jobService.SaveDragDropEvent(dragDropModel);

switch (result)
{
    case DragDropSchedulerEnum.Changed:
        Console.WriteLine("Job successfully reassigned");
        // System sends cancellation email to old tech, assignment email to new tech
        break;
    case DragDropSchedulerEnum.AlreadyAssigned:
        Console.WriteLine("Tech B is already assigned to this job");
        break;
    case DragDropSchedulerEnum.Error:
        Console.WriteLine("Tech B is not available (conflict detected)");
        break;
}
```

### Blocking Technician Vacation
```csharp
// Inject service
private readonly IEstimateService _estimateService;

// Create vacation block
var vacationModel = new JobEstimateEditModel
{
    FranchiseeId = 42,
    StartDate = DateTime.Parse("2024-12-20"),
    EndDate = DateTime.Parse("2025-01-03"),
    EstimateSchedulerList = new List<JobSchedulerEditModel>
    {
        new JobSchedulerEditModel
        {
            AssigneeId = 101,  // Technician on vacation
            StartDate = DateTime.Parse("2024-12-20"),
            EndDate = DateTime.Parse("2025-01-03"),
            IsVacation = true,
            ScheduleType = (long)ScheduleType.Vacation,
            Title = "Holiday Vacation"
        }
    }
};

_estimateService.SaveVacation(vacationModel);

// Now CheckAvailability() will return false for Tech 101 during this period
bool available = _jobService.CheckAvailability(
    jobId: 0,
    techId: 101,
    startDate: DateTime.Parse("2024-12-25"),
    endDate: DateTime.Parse("2024-12-25").AddHours(4),
    isVacation: false
);
// Returns: false (technician on vacation)
```

### Importing Calendar Files
```csharp
// Inject service
private readonly ICalendarImportService _calendarService;

// User uploads .ics file from Outlook/Google Calendar
var importModel = new CalendarImportModel
{
    FranchiseeId = 42,
    FileType = "Job",  // "Job" or "Estimate"
    UploadedFile = /* HttpPostedFileBase from form */
};

_calendarService.Save(importModel);
// File stored, queued for background processing by ICalendarParsePollingAgent
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Service Interface | Primary Methods | Purpose |
|-------------------|-----------------|---------|
| **IJobService** | `Save()`, `Get()`, `CheckAvailability()`, `SaveDragDropEvent()`, `ChangeStatus()`, `SaveJobEstimateMediaFiles()` | Core job CRUD, scheduling validation, image upload, status workflow |
| **IEstimateService** | `Save()`, `Get()`, `SaveVacation()`, `SaveMeeting()`, `RepeatVacation()`, `SendingUpdationMails()` | Estimate management, vacation/meeting scheduling, notifications |
| **IJobSchedulerService** | `ChangeSchedulerDateTime()` | Reschedule operations with timezone handling |
| **IBeforeAfterImageService** | `GetBeforeAfterImagesForFranchiseeAdmin()` | Filtered image retrieval for admin review |
| **IBeforeAfterThumbNailService** | `CreateImageThumb()` | Thumbnail generation (500x500 max, aspect-preserving) |
| **ICalendarImportService** | `Save()` | Upload calendar files for import |
| **ICalendarParsePollingAgent** | `ParseCalendarFile()` | Background .ics parsing (scheduled task) |
| **ISendNewJobNotificationtoTechService** | `SendJobNotificationtoTech()`, `SendJobNotificationtoTechForCancelled()` | Technician push notifications |
| **IEstimateInvoiceServices** | `Save()`, `Get()`, `Delete()` | Invoice line item management |
| **ISalesTaxAPIServices** | `GetSalesTax()` | Sales tax calculation by ZIP/county |
| **IAttachingInvoicesServices** | `Save()` | Attach PDF invoices to jobs |
| **IGeoCodeService** | `GeocodeAddress()` | Address to lat/lng conversion |
| **IWorkOrderTechnicianService** | `GetAll()`, `Save()` | Work order type configuration |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Images Not Showing in Admin Review
**Problem**: Before/after images uploaded but not appearing in franchisee admin gallery.

**Checklist**:
1. Verify `IsBeforeImage` flag correctly set (true for before, false for after)
2. Check `PairId` links to valid `JobEstimateServices.Id`
3. Confirm `ServiceTypeId` and `CategoryId` match filter criteria
4. Ensure thumbnails generated (check `ThumbFileId` not null)
5. Validate `S3BucketURL` accessible (CORS, permissions)
6. Check image not soft-deleted (`IsDeleted = false`)

### Technician Not Receiving Job Notifications
**Problem**: Job assigned but technician never gets notified.

**Checklist**:
1. Verify franchisee feature flag: `NewJobNotificationToTechAndSales` enabled
2. Confirm technician has valid `OrganizationRoleUser.Id` linked to `JobScheduler.AssigneeId`
3. Check notification service running (scheduled task active)
4. Validate technician phone number format in user profile
5. Ensure job `StartDate` within notification window (±2 days typically)
6. Check notification audit logs for delivery failures

### Calendar Import Failed
**Problem**: Uploaded .ics file not creating jobs/estimates.

**Checklist**:
1. Verify file format is valid iCalendar (.ics)
2. Check `CalendarFileUpload` state field (should progress: uploaded → processing → processed)
3. Review error logs from `ICalendarParsePollingAgent`
4. Confirm event dates are valid (not in past beyond threshold)
5. Validate technician IDs exist if specified in calendar attendees
6. Ensure polling agent scheduled task running

### Double-Booking Despite Availability Check
**Problem**: `CheckAvailability()` returned true but technician now has conflicting jobs.

**Checklist**:
1. Verify `CheckAvailability()` called immediately before `Save()` (race condition)
2. Confirm checking correct technician ID (AssigneeId vs SalesRepId confusion)
3. Check timezone offsets properly applied (UTC vs local time mismatch)
4. Validate job not excluded from own availability check (pass correct jobId)
5. Ensure vacation schedules included in check (`isVacation` parameter)
6. Review transaction isolation level (read committed may miss concurrent inserts)

### Estimate Not Converting to Job
**Problem**: Attempting to create job from estimate but fails validation.

**Checklist**:
1. Verify `EstimateId` provided in `JobEditModel`
2. Confirm estimate status allows conversion (not cancelled/rejected)
3. Check customer ID matches between estimate and job
4. Validate franchisee ID consistent
5. Ensure at least one technician assigned (`TechIds` not empty)
6. Confirm `StartDate` not in past

<!-- END CUSTOM SECTION -->
