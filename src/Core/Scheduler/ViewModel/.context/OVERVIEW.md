<!-- AUTO-GENERATED: Header -->
# ViewModels / DTOs
> Data transfer objects for API operations and UI binding
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

ViewModels are the "JSON contracts" of the scheduling system. They define what data flows in and out of the API, decoupling external interfaces from internal domain models.

**Why Separate ViewModels from Domain Entities?**
- **Versioning**: Change domain model without breaking API contracts
- **Optimization**: Include only fields needed for specific operations (no unnecessary data transfer)
- **Security**: Exclude sensitive fields (e.g., internal IDs, audit timestamps)
- **UI Convenience**: Add computed fields (colors, formatted strings) not stored in database
- **Validation**: Attach data annotations without polluting domain entities

**Key ViewModels:**
- `JobEditModel` & `JobEstimateEditModel`: Complete forms for job/estimate CRUD
- `JobSchedulerEditModel`: Calendar entry data
- `ReviewMarketingBeforeAfterImageViewModel`: Rich image data with metadata
- `FileUploadModel`: Multi-file upload with surface/location metadata
- Various filter/list models for queries

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Basic CRUD Example
```csharp
// CREATE: Client sends JobEditModel JSON
var jobModel = new JobEditModel
{
    JobTypeId = 100,
    StatusId = (long)JobStatusType.Created,
    StartDate = DateTime.UtcNow.AddDays(7),
    EndDate = DateTime.UtcNow.AddDays(7).AddHours(4),
    JobCustomer = new JobCustomerEditModel
    {
        FirstName = "John",
        LastName = "Smith",
        Email = "john@example.com"
    },
    TechIds = new List<long> { 101, 102 }
};

// Service layer converts DTO → Domain entity
_jobService.Save(jobModel);

// READ: Service converts Domain entity → DTO
var model = _jobService.Get(jobId);  // Returns JobEditModel
```

### Image Upload with Metadata
```csharp
var uploadModel = new FileUploadModel
{
    JobId = 9876,
    SchedulerId = 11111,
    IsBeforeImage = true,
    Files = Request.Files.Cast<HttpPostedFileBase>().ToList(),
    SurfaceType = "Floor",
    SurfaceMaterial = "Marble",
    BuildingLocation = "Main lobby"
};

var images = _jobService.SaveJobEstimateMediaFiles(uploadModel);
// Returns List<BeforeAfterImageModel> with image IDs
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## ViewModel Summary

| Category | Key ViewModels | Purpose |
|----------|----------------|---------|
| **Job** | JobEditModel, JobViewModel, JobListModel | Job CRUD operations |
| **Estimate** | JobEstimateEditModel, ShiftChargesViewModel | Estimate management |
| **Scheduler** | JobSchedulerEditModel, DragDropSchedulerModel | Calendar operations |
| **Customer** | JobCustomerEditModel, CustomerInfoModel | Customer data |
| **Images** | ReviewMarketingBeforeAfterImageViewModel, BeforeAfterImageFilter, FileUploadModel | Photo management |
| **Notes** | SchedulerNoteModel, JobNoteEditModel | Comments/notes |
| **Signatures** | JobEstimateSignatureEditModel | Digital signatures |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Model Binding Failures
**Problem**: Controller receives `null` model or missing properties.

**Common Causes**:
1. Property name mismatch (case-sensitive in some contexts)
2. Missing `[FromBody]` attribute for complex types
3. Incorrect Content-Type header (should be `application/json`)

**Solution**: Enable model binding logging, verify JSON structure matches ViewModel properties.

### Validation Errors Not Showing
**Problem**: `ModelState.IsValid` always true despite invalid data.

**Solution**: Check for `[NoValidatorRequired]` attribute - disables automatic validation. Add explicit validation attributes or implement `IValidatableObject`.

<!-- END CUSTOM SECTION -->
