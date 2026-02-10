<!-- AUTO-GENERATED: Header -->
# Impl — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2026-02-10T15:02:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Impl (Implementation) folder contains the **concrete service layer classes** that implement the interface contracts defined in the parent Scheduler folder. This is where business logic lives - complex workflows, validation rules, database orchestration, notification dispatching, image processing, and integration with external systems. These are heavyweight services with many dependencies, encapsulating multi-step operations.

### Design Patterns
- **Service Layer Pattern**: Each implementation class marked with `[DefaultImplementation]` attribute for dependency injection
- **Repository Pattern**: Services consume repositories (never direct DbContext access)
- **Unit of Work Pattern**: `IUnitOfWork` manages transactional boundaries
- **Factory Pattern**: Services use factories (`IJobFactory`, `IEstimateInvoiceFactory`) for domain object creation
- **Notification Observer**: Multiple notification services react to scheduling events
- **Polling Agent Pattern**: `CalendarParsePollingAgent` runs as scheduled background task
- **Template Method**: Notification services share common structure (fetch data → format message → dispatch)

### Service Categories

#### **Core Job Management Services** (Heavy Business Logic)
- **JobService** (372 KB): Master service for job CRUD, scheduling, image upload, availability checks, drag-drop operations
- **EstimateService** (126 KB): Estimate management, vacation/meeting scheduling, recurring schedules, email notifications
- **JobSchedulerService**: DateTime management for schedulers with timezone handling and cascade updates

#### **Image Processing Services** (File & Graphics Operations)
- **BeforeAfterImageService**: Complex filtering and retrieval of before/after photos for admin review
- **BeforeAfterThumbNailService**: Thumbnail generation (500x500 max, aspect-ratio preserving, high-quality resize)
- **Imager**: Static utility class for JPEG encoding, canvas operations, codec resolution

#### **Calendar & Import Services** (External Integration)
- **CalendarImportService**: Calendar file upload handling (file storage, database record creation)
- **CalendarParsePollingAgent**: Background iCalendar (.ics) parsing via Ical.Net library
- **HomeAdvisorParser**: Third-party lead import parsing
- **ZipFileParser**: ZIP archive extraction for bulk imports

#### **Notification Services** (Multi-Channel Messaging)
- **SendNewJobNotificationtoTechService**: Technician assignment/cancellation push notifications
- **JobReminderNotificationtoUsersService**: Automated reminder notifications (±2 days)
- **AutoGenereatedMailForBestFitNotification**: Saturday best-fit image notifications
- **BeforeAfterImagesNotificationServices**: Image upload confirmation emails
- **CancellationMailForTechSales**: Cancellation alert emails
- **ZipParserNotificationService**: Import status notifications
- **MailForNonResidentalBuildingTypeNotification**: Commercial job-specific alerts

#### **Financial & Integration Services**
- **EstimateInvoiceServices**: Invoice line item management (dimensions, pricing, tax calculation)
- **SalesTaxAPIServices**: Sales tax lookup by ZIP code and county
- **AttachingInvoicesServices**: PDF invoice attachment to jobs/estimates

#### **Factory Services** (Domain Object Creation)
- **JobFactory**, **JobDetailsFactory**: Create properly initialized Job entities
- **EstimateInvoiceFactory**: Create estimate invoice line items
- **GeoCodeFactory**: Create geocode records
- **CalendarFactory**: Create calendar import records

#### **Utility Services**
- **GeoCodeService**: Address geocoding (lat/lng conversion)
- **WorkOrderTechnicianService**: Work order type configuration management
- **DebuggerLog**: System logging service
- **EncryptionHelper**: Data encryption/decryption utilities

### Data Flow

#### **Job Save Flow (JobService.Save)**
```
1. Validate JobEditModel (QBInvoiceNumber uniqueness, customer exists)
2. Create/Update Job entity via IJobFactory
3. Calculate delta for JobScheduler collection (adds, updates, deletes)
4. Update technician assignments (OrganizationRoleUser links)
5. Persist changes via repositories + IUnitOfWork
6. Dispatch notifications via ISendNewJobNotificationtoTechService
7. Update related JobEstimate if converted from estimate
```

#### **Image Upload Flow (JobService.SaveJobEstimateMediaFiles)**
```
1. Receive FileUploadModel with uploaded files + metadata
2. Save files to media folder via IFileService
3. Create BeforeAfterImages entities with surface/location metadata
4. Generate thumbnails via IBeforeAfterThumbNailService (async)
5. Link images to JobScheduler and JobEstimateServices (PairId)
6. Return BeforeAfterImageModel[] with image IDs and URLs
```

#### **Calendar Import Flow (CalendarParsePollingAgent.ParseCalendarFile)**
```
1. Query CalendarFileUpload for unprocessed files (state = 'uploaded')
2. Load .ics file from disk
3. Parse using Ical.Net library (extract events, attendees, dates)
4. Determine type (Job vs Estimate) based on SalesRepId presence
5. Create Job/JobEstimate entities via IJobService/IEstimateService
6. Apply timezone offsets from ITimeZoneInformationRepository
7. Update file state to 'processed' or 'error'
```

#### **Drag-Drop Reschedule Flow (JobService.SaveDragDropEvent)**
```
1. Validate new technician not already assigned to same job
2. Check availability via CheckAvailability() (conflict detection)
3. Update JobScheduler.AssigneeId and dates via IJobSchedulerService
4. Send cancellation email to old technician
5. Send assignment email to new technician
6. Return DragDropSchedulerEnum result (Changed | Error | AlreadyAssigned)
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Key Implementation Classes

#### **JobService** - Implements `IJobService`
```csharp
[DefaultImplementation(LifeCyclePolicy.PerHttpRequest)]
public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobSchedulerRepository _schedulerRepository;
    private readonly IJobFactory _jobFactory;
    private readonly IBeforeAfterImageService _imageService;
    private readonly IFileService _fileService;
    private readonly ISendNewJobNotificationtoTechService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    // ... 20+ dependencies
    
    public void Save(JobEditModel model) { /* 300+ lines */ }
    public bool CheckAvailability(long jobId, long techId, DateTime start, DateTime end, bool isVacation) { /* ... */ }
    public DragDropSchedulerEnum SaveDragDropEvent(DragDropSchedulerModel model) { /* ... */ }
    // ... 50+ methods
}
```

**Critical Dependencies**:
- Repositories: Job, JobScheduler, JobCustomer, JobStatus, BeforeAfterImages
- Factories: JobFactory, JobDetailsFactory, JobInfoFactory
- Services: FileService, BeforeAfterImageService, NotificationService, GeoCodeService
- Infrastructure: IUnitOfWork, ISettings, IClock, IUserNotificationModelFactory

---

#### **EstimateService** - Implements `IEstimateService`
```csharp
[DefaultImplementation(LifeCyclePolicy.PerHttpRequest)]
public class EstimateService : IEstimateService
{
    private readonly IJobEstimateRepository _estimateRepository;
    private readonly IJobSchedulerRepository _schedulerRepository;
    private readonly IEstimateInvoiceServices _invoiceServices;
    private readonly IOrganizationRoleUserInfoService _userService;
    private readonly IUnitOfWork _unitOfWork;
    // ... 15+ dependencies
    
    public void Save(JobEstimateEditModel model) { /* ... */ }
    public void SaveVacation(JobEstimateEditModel model) { /* ... */ }
    public void RepeatVacation(VacationRepeatEditModel model) { /* recurring logic */ }
    public long SaveMeeting(JobEstimateEditModel model) { /* ... */ }
    // ... 40+ methods
}
```

---

#### **BeforeAfterThumbNailService** - Implements `IBeforeAfterThumbNailService`
```csharp
[DefaultImplementation]
public class BeforeAfterThumbNailService : IBeforeAfterThumbNailService
{
    public FileBase CreateImageThumb(FileBase filebase, string mediaFolder)
    {
        using (var image = Image.FromFile(originalPath))
        {
            // Calculate aspect-ratio preserving dimensions (500x500 max)
            int width = image.Width > 500 ? 500 : image.Width;
            int height = (int)(width / (double)image.Width * image.Height);
            
            if (height > 500)
            {
                height = 500;
                width = (int)(height / (double)image.Height * image.Width);
            }
            
            using (var thumbnail = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(thumbnail))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, width, height);
                
                // Save as JPEG with 85% quality
                thumbnail.Save(thumbnailPath, GetJpegCodec(), GetEncoderParams(85));
            }
        }
        
        // Create FileBase entity for thumbnail
        return new FileBase { /* ... */ };
    }
}
```

---

#### **CalendarParsePollingAgent** - Implements `ICalendarParsePollingAgent`
```csharp
[DefaultImplementation]
public class CalendarParsePollingAgent : ICalendarParsePollingAgent
{
    public void ParseCalendarFile()
    {
        // Get unprocessed files
        var files = _calendarUploadRepository.GetAll()
            .Where(f => f.State == "uploaded")
            .ToList();
        
        foreach (var file in files)
        {
            try
            {
                // Load and parse .ics file
                using (var stream = File.OpenRead(file.FilePath))
                {
                    var calendar = Calendar.Load(stream);  // Ical.Net
                    
                    foreach (var evt in calendar.Events)
                    {
                        // Extract event details
                        var startDate = evt.Start.AsSystemLocal;
                        var endDate = evt.End.AsSystemLocal;
                        var attendees = evt.Attendees.Select(a => a.Value).ToList();
                        
                        // Determine type (Job vs Estimate)
                        bool isJob = file.SalesRepId.HasValue;
                        
                        if (isJob)
                        {
                            // Create Job via JobService
                            var jobModel = MapToJobEditModel(evt, file);
                            _jobService.Save(jobModel);
                        }
                        else
                        {
                            // Create Estimate via EstimateService
                            var estimateModel = MapToJobEstimateEditModel(evt, file);
                            _estimateService.Save(estimateModel);
                        }
                    }
                }
                
                // Update state
                file.State = "processed";
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                file.State = "error";
                file.ErrorMessage = ex.Message;
                _unitOfWork.SaveChanges();
            }
        }
    }
}
```

---

### Notification Service Pattern

All notification services follow similar structure:

```csharp
[DefaultImplementation]
public class SendNewJobNotificationtoTechService : ISendNewJobNotificationtoTechService
{
    public void SendJobNotificationtoTech(DateTime date)
    {
        // 1. Query scheduled jobs for target date
        var schedulers = _jobSchedulerRepository.GetAll()
            .Where(js => DbFunctions.TruncateTime(js.StartDate) == date.Date)
            .Where(js => js.AssigneeId.HasValue)
            .ToList();
        
        // 2. Filter by franchisee feature flag
        var enabledFranchisees = GetFranchiseesWithFeatureEnabled("NewJobNotificationToTechAndSales");
        schedulers = schedulers.Where(js => enabledFranchisees.Contains(js.FranchiseeId)).ToList();
        
        // 3. Resolve technician contact info
        foreach (var scheduler in schedulers)
        {
            var tech = _userRepository.GetById(scheduler.AssigneeId.Value);
            var phoneNumber = tech.Person.PhoneNumber;
            var techName = tech.Person.FullName;
            
            // 4. Generate notification message
            var notification = _notificationFactory.Create(new
            {
                Type = "NewJobAssignment",
                TechName = techName,
                JobAddress = scheduler.Job?.JobCustomer?.Address,
                StartTime = scheduler.ActualStartDate,
                JobDescription = scheduler.Job?.Description ?? scheduler.Title
            });
            
            // 5. Dispatch notification (SMS/push)
            _notificationService.Send(phoneNumber, notification);
        }
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

See parent folder [CONTEXT.md](../. context/CONTEXT.md) for interface contracts. Impl classes implement those interfaces.

### Key Service Methods (Implementation Details)

#### **JobService.CheckAvailability()**
**Algorithm**:
```csharp
public bool CheckAvailability(long jobId, long techId, DateTime start, DateTime end, bool isVacation)
{
    // Query overlapping schedules
    var conflicts = _schedulerRepository.GetAll()
        .Where(js => js.AssigneeId == techId)
        .Where(js => js.Id != jobId)  // Exclude current job
        .Where(js => !js.IsDeleted && js.IsActive)
        .Where(js => js.StartDate < end && js.EndDate > start)  // Overlap: (start1 < end2 AND end1 > start2)
        .ToList();
    
    // If checking job availability, also exclude vacations
    if (!isVacation)
    {
        // No filtering needed - vacations already included
    }
    
    return !conflicts.Any();
}
```

**Time Complexity**: O(n) where n = number of schedulers for technician (typically < 100)

---

#### **EstimateService.RepeatVacation()**
**Algorithm** (Recurring Vacation Creation):
```csharp
public void RepeatVacation(VacationRepeatEditModel model)
{
    var baseVacation = _schedulerRepository.GetById(model.VacationId);
    DateTime currentDate = baseVacation.StartDate;
    
    for (int i = 0; i < model.OccurrenceCount; i++)
    {
        // Calculate next occurrence date
        switch (model.Frequency)
        {
            case RepeatFrequency.Daily:
                currentDate = currentDate.AddDays(1);
                break;
            case RepeatFrequency.Weekly:
                currentDate = currentDate.AddDays(7);
                break;
            case RepeatFrequency.Monthly:
                currentDate = currentDate.AddMonths(1);
                break;
            case RepeatFrequency.Custom:
                currentDate = currentDate.AddDays(model.CustomDayInterval);
                break;
        }
        
        // Create new vacation instance
        var vacation = new JobScheduler
        {
            AssigneeId = baseVacation.AssigneeId,
            StartDate = currentDate,
            EndDate = currentDate.Add(baseVacation.EndDate - baseVacation.StartDate),  // Same duration
            IsVacation = true,
            IsRepeat = true,
            ParentJobId = baseVacation.Id,  // Link to original
            FranchiseeId = baseVacation.FranchiseeId,
            SchedulerStatus = (long)ScheduleType.Vacation,
            Title = baseVacation.Title
        };
        
        _schedulerRepository.Add(vacation);
    }
    
    _unitOfWork.SaveChanges();
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Scheduler.Domain** — Entity models (Job, JobEstimate, JobScheduler, BeforeAfterImages, etc.)
- **Core.Scheduler.Enum** — Enumerations for state management
- **Core.Scheduler.ViewModel** — DTOs for service operations
- **Core.Organizations** — OrganizationRoleUser, Franchisee repositories and services
- **Core.Users** — Person, Role repositories
- **Core.Notification** — Email/SMS dispatch services, IUserNotificationModelFactory
- **Core.FileManagement** — IFileService, FileBase entity
- **Core.Sales** — MarketingClass, ServiceType repositories
- **Core.Billing** — Invoice entities and repositories
- **Core.Lookup** — Lookup table repositories

### External Dependencies
- **Ical.Net** — iCalendar parsing library for .ics file imports
- **System.Drawing** — Image manipulation (thumbnail generation, resizing, encoding)
- **System.Drawing.Imaging** — JPEG codec, encoder parameters
- **Entity Framework** — Database queries, Include/ThenInclude, DbFunctions
- **Dependency Injection Framework** — `[DefaultImplementation]` attribute for auto-registration

### Infrastructure Dependencies
- **IUnitOfWork** — Transaction management, SaveChanges orchestration
- **ISettings** — Application configuration (media folder paths, feature flags, SMTP settings)
- **IClock** — Testable DateTime.Now abstraction
- **IHttpContextAccessor** — Current user resolution

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Performance Bottlenecks

1. **JobService.Save()**: Heavy method with 20+ repository calls, N+1 queries for JobScheduler collection. Use profiler before optimizing.

2. **BeforeAfterImageService.GetBeforeAfterImagesForFranchiseeAdmin()**: Loads images with Base64 encoding - extremely memory-intensive for large result sets. Always use pagination and date filters.

3. **CalendarParsePollingAgent**: Runs as scheduled task (typically every 5 minutes). Multiple files processed sequentially - no parallelization. Consider async/await refactor for high-volume imports.

### Common Service Layer Mistakes

1. **Forgetting IUnitOfWork.SaveChanges()**: Changes not persisted until explicit call. Missing SaveChanges = lost data.

2. **Calling services from services without transaction scope**: Service A calls Service B, both call SaveChanges independently = partial commits on failure. Use transactional service methods.

3. **Lazy loading inside loops**: 
   ```csharp
   // BAD: N+1 queries
   foreach (var job in jobs)
   {
       var customer = job.JobCustomer;  // Lazy load
   }
   
   // GOOD: Eager load
   var jobs = _jobRepository.GetAll()
       .Include(j => j.JobCustomer)
       .ToList();
   ```

4. **Not disposing image resources**: System.Drawing.Image must be disposed or memory leaks occur. Always use `using` statements.

### Notification Service Configuration

All notification services depend on franchisee feature flags. If notifications not sending:
1. Check `Franchisee.FeatureFlags` table for enabled features
2. Verify notification scheduled task running (Windows Task Scheduler or background service)
3. Check notification audit logs for delivery failures
4. Validate technician phone numbers in `Person.PhoneNumber` field

### Calendar Import Troubleshooting

**Polling agent not processing files:**
- Verify `CalendarFileUpload.State = 'uploaded'` (case-sensitive)
- Check polling interval in app config (default 5 minutes)
- Review error logs for parsing exceptions

**Events not creating jobs/estimates:**
- Ensure event dates not in past (threshold typically 30 days)
- Verify franchisee ID valid
- Check technician IDs exist in OrganizationRoleUser table
- Validate calendar format is standard iCalendar (.ics)

### Thumbnail Generation Edge Cases

- **Large images (> 10 MB)**: OutOfMemoryException possible. Consider max file size validation before upload.
- **Unsupported formats**: Code only handles PNG, JPG, JPEG. Other formats (BMP, GIF) may fail.
- **Aspect ratio edge cases**: Very wide panoramic images (ratio > 5:1) may produce tiny thumbnails. Consider minimum dimension constraints.

<!-- END CUSTOM SECTION -->
