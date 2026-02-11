<!-- AUTO-GENERATED: Header -->
# Domain Entities
> Persistent entity models for MarbleLife job scheduling, estimates, and image documentation
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Domain folder contains the database-backed entity models that form the foundation of the scheduling system. These are **not** simple data containers - they are rich domain objects with relationships, computed properties, and business constraints enforced at the schema level.

**Think of this as your data dictionary**: If you want to understand what information the system tracks about jobs, estimates, technicians, or before/after photos, start here.

**Key Entities:**
- **Job**: An actual work order assigned to technicians
- **JobEstimate**: A quote/estimate that may convert to a job
- **JobScheduler**: The "calendar entry" linking jobs/estimates to technicians with specific dates/times
- **BeforeAfterImages**: Photo documentation with rich metadata (surface type, material, location)
- **Meeting**: Calendar meetings/events (team meetings, equipment reservations)
- **TechnicianWorkOrder**: Configuration for available work order types

**Why So Many Foreign Keys?**
The scheduling system is a hub connecting multiple business domains:
- **Organizations**: Franchisees, technicians, sales reps
- **Sales**: Service types, marketing classes
- **Customers**: Job customers, addresses
- **Files**: Image files, PDFs, thumbnails
- **Billing**: Invoices, QuickBooks integration
- **Notifications**: Who to notify when things change

This isn't "over-engineering" - it's the reality of a multi-tenant, multi-franchise service business.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Reading Entities with Relationships

```csharp
// Inject repository
private readonly IJobRepository _jobRepository;

// Load job with all related data
var job = await _jobRepository.GetAll()
    .Include(j => j.JobCustomer)
        .ThenInclude(c => c.Address)
    .Include(j => j.JobScheduler)
        .ThenInclude(js => js.OrganizationRoleUser)  // Technicians
            .ThenInclude(oru => oru.Person)
    .Include(j => j.JobStatus)
    .Include(j => j.JobType)  // MarketingClass
    .Include(j => j.JobEstimate)  // Original estimate (if converted)
    .Include(j => j.JobNote)
    .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDeleted);

// Access related data
Console.WriteLine($"Job for {job.JobCustomer.FirstName} {job.JobCustomer.LastName}");
Console.WriteLine($"Status: {job.JobStatus.Name}");
Console.WriteLine($"Technicians: {string.Join(", ", job.JobScheduler.Select(js => js.OrganizationRoleUser.Person.FullName))}");
```

### Creating Entities with Proper Initialization

```csharp
// Use factories from service layer (recommended)
var job = _jobFactory.Create(new JobEditModel { /* ... */ });

// OR manual creation (ensure proper initialization)
var job = new Job
{
    JobTypeId = 100,
    StatusId = (long)JobStatusType.Created,
    CustomerId = 12345,
    StartDate = DateTime.UtcNow.AddDays(7),
    EndDate = DateTime.UtcNow.AddDays(7).AddHours(4),
    Offset = -300,  // EST (UTC-5)
    Description = "Marble floor restoration",
    GeoCode = "33.7490,-84.3880",
    QBInvoiceNumber = "INV-2024-1234",
    
    // Initialize collections to prevent NullReferenceException
    JobNote = new Collection<JobNote>()
};

// Create scheduler entries
job.JobScheduler = new Collection<JobScheduler>
{
    new JobScheduler
    {
        JobId = job.Id,  // Set after job persisted
        AssigneeId = 101,  // Technician
        FranchiseeId = 42,
        StartDate = job.StartDate,
        EndDate = job.EndDate,
        Offset = job.Offset,
        SchedulerStatus = (long)ScheduleType.Job,
        IsVacation = false,
        IsRepeat = false,
        DataRecorderMetaDataId = /* ... */
    }
};

_jobRepository.Add(job);
await _unitOfWork.SaveChangesAsync();
```

### Querying with Timezone Awareness

```csharp
// WRONG: Using raw dates for display
var jobs = _jobRepository.GetAll()
    .Select(j => new
    {
        j.Id,
        StartTime = j.StartDate,  // Returns UTC!
        EndTime = j.EndDate
    });

// CORRECT: Using computed properties
var jobSchedulers = _jobSchedulerRepository.GetAll()
    .ToList()  // Execute query first
    .Select(js => new
    {
        js.Id,
        StartTime = js.ActualStartDate,  // Applies offset
        EndTime = js.ActualEndDate
    });

// OR: Apply offset manually in projection
var jobs = _jobRepository.GetAll()
    .Select(j => new
    {
        j.Id,
        StartTime = DbFunctions.AddMinutes(j.StartDate, j.Offset ?? 0),
        EndTime = DbFunctions.AddMinutes(j.EndDate, j.Offset ?? 0)
    });
```

### Pairing Before/After Images

```csharp
// Query images for a scheduler
var images = await _beforeAfterImageRepository.GetAll()
    .Include(img => img.File)
    .Include(img => img.ThumbFile)
    .Include(img => img.JobEstimateServices)
    .Where(img => img.SchedulerId == schedulerId)
    .Where(img => !img.IsDeleted)
    .ToListAsync();

// Group into pairs
var imagePairs = images
    .Where(img => img.PairId.HasValue && img.IsBeforeImage.HasValue)
    .GroupBy(img => img.PairId.Value)
    .Select(g => new
    {
        ServiceId = g.Key,
        ServiceName = g.First().JobEstimateServices?.ServiceName,
        BeforeImage = g.FirstOrDefault(img => img.IsBeforeImage == true),
        AfterImage = g.FirstOrDefault(img => img.IsBeforeImage == false)
    })
    .Where(pair => pair.BeforeImage != null && pair.AfterImage != null)  // Complete pairs only
    .ToList();

// Unpaired images (incomplete documentation)
var unpairedImages = images
    .Where(img => !img.PairId.HasValue || !img.IsBeforeImage.HasValue)
    .ToList();
```

### Checking Technician Availability

```csharp
// Get all conflicting schedules
var conflicts = await _jobSchedulerRepository.GetAll()
    .Where(js => js.AssigneeId == technicianId)
    .Where(js => !js.IsDeleted && js.IsActive)
    .Where(js => js.StartDate < proposedEndDate && js.EndDate > proposedStartDate)  // Overlap detection
    .Include(js => js.Job)
    .Include(js => js.Estimate)
    .ToListAsync();

bool isAvailable = !conflicts.Any();

// If conflicts exist, show details
if (!isAvailable)
{
    foreach (var conflict in conflicts)
    {
        var description = conflict.IsVacation ? "Vacation" :
                          conflict.JobId.HasValue ? $"Job #{conflict.JobId}" :
                          conflict.EstimateId.HasValue ? $"Estimate #{conflict.EstimateId}" :
                          conflict.MeetingID.HasValue ? $"Meeting: {conflict.Title}" :
                          "Unknown";
        Console.WriteLine($"Conflict: {description} from {conflict.ActualStartDate} to {conflict.ActualEndDate}");
    }
}
```

### Soft Delete Pattern

```csharp
// Soft delete job (preserve audit trail)
var job = await _jobRepository.GetById(jobId);
job.IsDeleted = true;
await _unitOfWork.SaveChangesAsync();

// Also soft delete related JobScheduler entries
var schedulers = await _jobSchedulerRepository.GetAll()
    .Where(js => js.JobId == jobId)
    .ToListAsync();

foreach (var scheduler in schedulers)
{
    scheduler.IsDeleted = true;
}
await _unitOfWork.SaveChangesAsync();

// Queries automatically exclude soft-deleted records (if global filter configured)
var activeJobs = _jobRepository.GetAll()  // IsDeleted = false implicit
    .Where(j => j.StatusId == (long)JobStatusType.InProgress);
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Entity Summary

| Entity | Primary Key | Key Relationships | Purpose |
|--------|-------------|-------------------|---------|
| **Job** | Id | JobCustomer, JobStatus, JobType (MarketingClass), JobEstimate, JobScheduler (1:N) | Actual work order |
| **JobEstimate** | Id | JobCustomer, MarketingClass, Jobs (1:N), JobEstimate (self-referencing) | Quote/estimate |
| **JobScheduler** | Id | Job, JobEstimate, Meeting, OrganizationRoleUser (Assignee), Franchisee, Invoice | Calendar entry linking work to resources |
| **BeforeAfterImages** | Id | JobScheduler, File, ThumbFile, JobEstimateServices (PairId), Franchisee, Person | Photo documentation |
| **Meeting** | Id | Meeting (self-referencing) | Calendar meetings/events |
| **TechnicianWorkOrder** | Id | Lookup (WorkOrderId) | Work order type configuration |
| **JobCustomer** | Id | Customer, Address | Customer details |
| **JobStatus** | Id | None | Workflow state lookup |
| **JobNote** | Id | Job, JobEstimate, JobScheduler | Comments/notes |
| **JobEstimateServices** | Id | JobEstimate/Job | Service line items |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### N+1 Query Performance Issues
**Symptom**: Page loads slowly, database logs show hundreds of individual SELECT queries.

**Solution**: Use `.Include()` to eager-load navigation properties:
```csharp
// Bad: N+1 queries
var jobs = _jobRepository.GetAll().ToList();
foreach (var job in jobs)
{
    Console.WriteLine(job.JobCustomer.Name);  // Lazy load for EACH job
}

// Good: Single query with join
var jobs = _jobRepository.GetAll()
    .Include(j => j.JobCustomer)
    .ToList();
foreach (var job in jobs)
{
    Console.WriteLine(job.JobCustomer.Name);  // Already loaded
}
```

### Timezone Display Bugs
**Symptom**: Job shows at wrong time on calendar, off by several hours.

**Solution**: Always use `ActualStartDate`/`ActualEndDate` for display:
```csharp
// Bad: UTC time displayed to user
var displayTime = jobScheduler.StartDate;  // 2024-01-15 18:00:00 (UTC)

// Good: Local time with offset applied
var displayTime = jobScheduler.ActualStartDate;  // 2024-01-15 13:00:00 (EST)
```

### Image Pairs Not Matching
**Symptom**: Before and after images don't pair correctly in admin gallery.

**Solution**: Verify `PairId` links to same `JobEstimateServices.Id`:
```csharp
// Check pairing consistency
var beforeImage = images.First(i => i.IsBeforeImage == true);
var afterImage = images.First(i => i.IsBeforeImage == false);

if (beforeImage.PairId != afterImage.PairId)
{
    Console.WriteLine("ERROR: Before and after have different PairIds");
    Console.WriteLine($"Before PairId: {beforeImage.PairId}");
    Console.WriteLine($"After PairId: {afterImage.PairId}");
}
```

### Orphaned JobScheduler Records
**Symptom**: JobScheduler entries exist but related Job/Estimate is soft-deleted.

**Solution**: Service layer must cascade soft deletes:
```csharp
public async Task DeleteJobAsync(long jobId)
{
    // Soft delete job
    var job = await _jobRepository.GetById(jobId);
    job.IsDeleted = true;
    
    // CASCADE: Soft delete all schedulers
    var schedulers = await _jobSchedulerRepository.GetAll()
        .Where(js => js.JobId == jobId)
        .ToListAsync();
    foreach (var scheduler in schedulers)
    {
        scheduler.IsDeleted = true;
    }
    
    await _unitOfWork.SaveChangesAsync();
}
```

### QuickBooks Invoice Number Collision
**Symptom**: Database constraint violation when saving Job with `QBInvoiceNumber`.

**Solution**: Always validate uniqueness before setting:
```csharp
public async Task<bool> IsValidQbNumberAsync(string qbNumber)
{
    return !await _jobRepository.GetAll()
        .Where(j => !j.IsDeleted)
        .AnyAsync(j => j.QBInvoiceNumber == qbNumber);
}

// Usage
if (!await _jobService.IsValidQbNumberAsync(model.QBInvoiceNumber))
{
    throw new InvalidOperationException("QBInvoiceNumber already exists");
}
```

<!-- END CUSTOM SECTION -->
