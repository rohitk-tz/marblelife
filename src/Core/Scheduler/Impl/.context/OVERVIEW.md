<!-- AUTO-GENERATED: Header -->
# Service Implementations
> Concrete business logic implementations for job management, image processing, and notifications
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This folder contains the "doing" layer - where interface contracts become executable code. If the parent folder defines "what" services do, this folder defines "how" they do it.

**Key Services:**
- **JobService** (372 KB): The workhorse - handles job CRUD, scheduling validation, image uploads, drag-drop, status changes
- **EstimateService** (126 KB): Estimate lifecycle, vacation/meeting management, recurring schedules
- **BeforeAfterImageService & BeforeAfterThumbNailService**: Image retrieval and thumbnail generation
- **CalendarImportService & CalendarParsePollingAgent**: External calendar file imports
- **Notification Services** (8+ classes): Multi-channel technician/customer notifications

**Why So Many Dependencies?**
Services orchestrate complex operations spanning multiple domains (customers, users, files, notifications, billing). A single `JobService.Save()` call touches 10+ repositories, 5+ factories, and 3+ notification services.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

Services are resolved via dependency injection:

```csharp
// In controller/application layer
public class JobController : ApiController
{
    private readonly IJobService _jobService;
    
    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }
    
    [HttpPost]
    public IHttpActionResult CreateJob(JobEditModel model)
    {
        try
        {
            _jobService.Save(model);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

**Never instantiate services directly** - always inject interfaces.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Service Implementation Summary

| Service | Methods | Key Responsibilities |
|---------|---------|---------------------|
| **JobService** | 50+ | Job CRUD, availability checks, image upload, drag-drop, status workflow |
| **EstimateService** | 40+ | Estimate management, vacation/meeting scheduling, recurring logic, notifications |
| **JobSchedulerService** | 3 | DateTime management with timezone handling |
| **BeforeAfterImageService** | 5 | Image filtering/retrieval for admin review |
| **BeforeAfterThumbNailService** | 1 | Thumbnail generation (500x500 max) |
| **CalendarImportService** | 1 | Calendar file upload |
| **CalendarParsePollingAgent** | 1 | Background .ics parsing (scheduled task) |
| **SendNewJobNotificationtoTechService** | 3 | Technician assignment/cancellation notifications |
| **EstimateInvoiceServices** | 10+ | Invoice line item management |
| **SalesTaxAPIServices** | 2 | Sales tax calculation by ZIP/county |
| **AttachingInvoicesServices** | 3 | PDF invoice attachment |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Service Not Resolving (DI Error)
**Problem**: "No matching bindings are available" exception.

**Solution**: Ensure class decorated with `[DefaultImplementation]` attribute and interface registered in DI container.

### Performance Issues
**Problem**: Service method takes 10+ seconds.

**Common Causes**:
1. N+1 queries (missing `.Include()` statements)
2. Large image loading without pagination
3. Synchronous I/O operations (file system, external APIs)

**Solution**: Use SQL profiler to identify query patterns, add eager loading, implement async/await for I/O.

<!-- END CUSTOM SECTION -->
