<!-- AUTO-GENERATED: Header -->
# ViewModel — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2026-02-10T15:03:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The ViewModel folder contains **Data Transfer Objects (DTOs)** that define the contract between the presentation layer (API controllers, UI) and the service layer. ViewModels flatten complex domain entity graphs, add UI-specific fields (colors, formatted strings), support validation, and optimize data transfer by including only necessary fields. This is the boundary between "how data is stored" (Domain entities) and "how data is transmitted" (ViewModels).

### Design Patterns
- **DTO Pattern**: Separate models for data transfer vs persistence
- **Edit vs View Separation**: `EditModel` for writes (validation attributes), `ViewModel` for reads (computed fields)
- **Flattening Pattern**: Nested domain entities flattened to single DTO (e.g., Job → JobScheduler → Technician → Person becomes flat properties)
- **List Wrappers**: `ListModel` classes contain collections + pagination metadata
- **Filter Objects**: Dedicated filter models for complex queries (e.g., `BeforeAfterImageFilter`)
- **Validation Attributes**: `[Required]`, `[Range]`, `[RegularExpression]` for input validation

### ViewModel Categories

#### **Job Management ViewModels**
- **JobEditModel**: Complete job edit form (80+ properties, includes schedulers, customer, signatures, notes)
- **JobViewModel**: Simplified job display (calendar rendering, status colors)
- **JobListModel**: Job collection with pagination (TotalCount, PageNumber, PageSize)
- **JobSchedulerEditModel**: Scheduler entry edit (dates, tech assignment, invoice tracking)
- **JobOccurenceListModel**: Recurring job instances (repeat frequency, parent job linking)

#### **Estimate ViewModels**
- **JobEstimateEditModel**: Estimate edit form (similar structure to JobEditModel but estimate-specific fields)
- **JobEstimateCategoryViewModel**: Estimate categorization for reporting
- **ShiftChargesViewModel**: Day/night shift pricing, commercial restoration rates
- **JobEstimateSignatureEditModel**: Digital signature capture (pre/post completion)

#### **Customer ViewModels**
- **JobCustomerEditModel**: Customer information (name, email, phone, address)
- **CustomerInfoModel**: Read-only customer display
- **CustomerMailForInvoiceViewModel**: Invoice email parameters

#### **Image ViewModels**
- **BeforeAfterImageViewModel**: Simple before/after pair with CSS styling
- **ReviewMarketingImageViewModel**: Hierarchical structure (Franchisee → Person → Scheduler → Image pairs) for admin gallery
- **ReviewMarketingBeforeAfterImageViewModel**: Rich image metadata (surface type, material, location, Base64 encoding)
- **BeforeAfterImageModel**: Lightweight list wrapper with date filtering
- **BeforeAfterImageFilter**: Complex filtering (franchisee, date range, service type, building type)
- **BeforeAfterFileModel**: File upload metadata
- **BeforeAfterImageSendMailViewModel**: Email notification parameters for image sharing

#### **Scheduler ViewModels**
- **JobSchedulerEditModel**: Core scheduler properties (JobId, EstimateId, AssigneeId, dates, invoice tracking)
- **DatetimeModel**: DateTime change parameters for rescheduling
- **OldSchedulerModel**: Previous scheduler state for diff calculation
- **DragDropSchedulerModel**: Drag-drop operation parameters (new tech, new dates)
- **ConfirmationModel**: Customer confirmation request (token, scheduler ID)
- **ConfirmationResponseModel**: Confirmation result (success, error message)
- **ScheduleAvailabilityFilterList**: Batch availability check for multiple technicians

#### **Note & Communication ViewModels**
- **SchedulerNoteModel**: Comments on jobs/estimates/vacations
- **JobNoteEditModel**: Note edit form (text, user attribution)
- **MailListModel**: Email template list for admin
- **JobEstimateServiceViewModel**: Service line item with invoice email options

#### **File & Media ViewModels**
- **FileUploadModel**: Multi-file upload (files, metadata, service type, surface details)
- **JobResourceEditModel**: File attachment metadata (file ID, type, relative location)
- **InvoiceLineImageModel**: Invoice line item image

#### **Specialty ViewModels**
- **GeoCodeResultModel**: Geocoding API response (lat, lng, formatted address)
- **AddressComponentModel**: Parsed address parts (street, city, state, ZIP)
- **CountyCreateEditModel**: County configuration
- **SalesTaxRatesEditModel**: Sales tax rate by ZIP/county
- **TimeZoneEditModel**: Timezone configuration
- **FranchiseInfoModel**: Franchisee display info
- **MeetingEditModel**: Meeting details
- **VacationRepeatEditModel**: Recurring vacation parameters

### Data Flow Pattern

#### **API → Service → Repository (Write)**
```
1. Controller receives JobEditModel from client
2. ASP.NET model binding + validation
3. Controller passes JobEditModel to IJobService.Save()
4. Service uses IJobFactory to map DTO → Job entity
5. Service persists entity via IJobRepository
6. Service returns void (or ID for creates)
```

#### **Repository → Service → API (Read)**
```
1. Controller calls IJobService.Get(jobId)
2. Service queries IJobRepository for Job entity (with .Include() for relations)
3. Service maps Job entity → JobEditModel
4. Service returns JobEditModel to controller
5. Controller serializes to JSON for client
```

### Validation Strategy

EditModels inherit from `EditModelBase` which provides:
- `IsValid()` method checking data annotation attributes
- `ValidationErrors` collection for error messages
- Integration with ASP.NET ModelState

Example:
```csharp
[Required(ErrorMessage = "Job type is required")]
public long JobTypeId { get; set; }

[Range(1, int.MaxValue, ErrorMessage = "At least one technician must be assigned")]
public int TechnicianCount => TechIds?.Count ?? 0;
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core ViewModels (Most Frequently Used)

#### **JobEditModel** - Complete job edit form
```csharp
public class JobEditModel : EditModelBase
{
    // Identity
    public long? Id { get; set; }
    public long JobId { get; set; }
    public long? EstimateId { get; set; }  // Links to parent estimate

    // Classification
    public long JobTypeId { get; set; }
    public long StatusId { get; set; }
    public long FranchiseeId { get; set; }

    // Scheduling
    public DateTime StartDate { get; set; }  // UTC
    public DateTime EndDate { get; set; }
    public DateTime ActualStartDateString { get; set; }  // With offset
    public DateTime ActualEndDateString { get; set; }

    // Financial
    public string QBInvoiceNumber { get; set; }  // UNIQUE constraint
    public decimal? Worth { get; set; }
    public decimal? LessDeposit { get; set; }
    public long? InvoiceId { get; set; }
    public bool IsInvoiceRequired { get; set; }

    // Customer
    public JobCustomerEditModel JobCustomer { get; set; }

    // Technician assignments
    public ICollection<long> TechIds { get; set; }  // Assigned technicians
    public ICollection<long> JobAssigneeIds { get; set; }
    public long? SalesRepId { get; set; }

    // Schedulers (job can have multiple scheduler entries)
    public IEnumerable<JobSchedulerEditModel> JobSchedulerList { get; set; }

    // Notes & communication
    public IEnumerable<SchedulerNoteModel> Notes { get; set; }
    public string Description { get; set; }
    public string MailBody { get; set; }

    // Signatures (pre/post completion)
    public List<JobEstimateSignatureEditModel> JobSignaturePre { get; set; }
    public List<JobEstimateSignatureEditModel> JobSignaturePost { get; set; }

    // Location
    public string GeoCode { get; set; }
    public string Location { get; set; }
    public string FullAddress { get; set; }

    // UI display fields
    public string Status { get; set; }
    public string StatusColor { get; set; }
    public string SchedulerStatusName { get; set; }
    public string SchedulerStatusColor { get; set; }
    public string Franchisee { get; set; }
    public string jobType { get; set; }
    public string AssigneeName { get; set; }
    public string AssigneePhone { get; set; }

    // Workflow flags
    public bool IsRepeat { get; set; }
    public bool IsActive { get; set; }
    public bool IsImported { get; set; }
    public bool? IsCustomerAvailable { get; set; }
    public bool? IsCustomerMailSend { get; set; }
    public bool IsDataToBeUpdateForAllJobs { get; set; }  // Cascading updates

    // Miscellaneous
    public long? LoggedInUserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
}
```

---

#### **JobSchedulerEditModel** - Scheduler entry
```csharp
public class JobSchedulerEditModel : EditModelBase
{
    public long? Id { get; set; }
    public long? JobId { get; set; }  // Mutually exclusive with EstimateId/MeetingID
    public long? EstimateId { get; set; }
    public long? MeetingID { get; set; }

    public long? AssigneeId { get; set; }  // Technician
    public long? SalesRepId { get; set; }
    public long FranchiseeId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ActualStartDate { get; set; }  // Computed with offset
    public DateTime ActualEndDate { get; set; }

    public long ScheduleType { get; set; }  // Job|Estimate|Vacation|Meeting
    public bool IsVacation { get; set; }
    public bool IsRepeat { get; set; }

    public string Title { get; set; }  // For meetings/vacations
    public string QBInvoiceNumber { get; set; }

    public decimal? EstimateWorth { get; set; }
    public bool IsInvoiceRequired { get; set; }
    public List<long> InvoiceNumbers { get; set; }  // Multiple invoices per scheduler

    // Workflow
    public bool IsJobConverted { get; set; }  // Estimate → Job conversion flag
    public bool IsCustomerAvailable { get; set; }
}
```

---

#### **ReviewMarketingBeforeAfterImageViewModel** - Rich image data for admin
```csharp
public class ReviewMarketingBeforeAfterImageViewModel
{
    // Identity
    public long? BeforeImageId { get; set; }
    public long? AfterImageId { get; set; }
    public long? PairId { get; set; }

    // Image data
    public string Base64Before { get; set; }  // Base64 encoded for inline display
    public string Base64After { get; set; }
    public string BeforeImageURL { get; set; }  // S3 bucket URL
    public string AfterImageURL { get; set; }

    // Surface metadata
    public string SurfaceType { get; set; }  // Floor, Countertop, Shower, etc.
    public string SurfaceMaterial { get; set; }  // Marble, Granite, Travertine, etc.
    public string FinishMaterial { get; set; }  // Polished, Honed, Leathered, etc.
    public string SurfaceColor { get; set; }  // White Carrara, Black Galaxy, etc.

    // Location
    public string BuildingLocation { get; set; }
    public string FloorNumber { get; set; }
    public string CompanyName { get; set; }

    // Classification
    public long? ServiceTypeId { get; set; }
    public string ServiceTypeName { get; set; }
    public long? CategoryId { get; set; }
    public string CategoryName { get; set; }
    public long? MarketingClassId { get; set; }
    public string MarketingClassName { get; set; }

    // Gallery flags
    public bool IsBestImage { get; set; }
    public bool IsAddToLocalGallery { get; set; }
    public bool IsImagePairReviewed { get; set; }

    // Attribution
    public string UploaderName { get; set; }
    public DateTime? UploadDate { get; set; }

    // Scheduler context
    public long? SchedulerId { get; set; }
    public DateTime? JobDate { get; set; }
}
```

---

#### **FileUploadModel** - Multi-file upload with metadata
```csharp
public class FileUploadModel
{
    // Target
    public long? JobId { get; set; }
    public long? EstimateId { get; set; }
    public long? SchedulerId { get; set; }

    // Files
    public List<HttpPostedFileBase> Files { get; set; }
    public long? FileId { get; set; }

    // Image classification
    public bool IsBeforeImage { get; set; }
    public long? ServiceTypeId { get; set; }
    public long? CategoryId { get; set; }
    public long? PairId { get; set; }  // Links before/after images

    // Surface metadata
    public string SurfaceType { get; set; }
    public string SurfaceMaterial { get; set; }
    public string FinishMaterial { get; set; }
    public string SurfaceColor { get; set; }

    // Location metadata
    public string BuildingLocation { get; set; }
    public string FloorNumber { get; set; }
    public string CompanyName { get; set; }

    // Uploader
    public long? UserId { get; set; }
    public string PersonName { get; set; }

    // Franchisee
    public long? FranchiseeId { get; set; }
}
```

---

#### **BeforeAfterImageFilter** - Complex filtering for admin review
```csharp
public class BeforeAfterImageFilter
{
    public long FranchiseeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public long? ServiceTypeId { get; set; }
    public long? MarketingClassId { get; set; }
    public long? BuildingTypeId { get; set; }  // Residential vs Commercial

    public bool? IsReviewed { get; set; }
    public bool? IsBestImage { get; set; }
    public bool? IsAddToLocalGallery { get; set; }

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

ViewModels are **data structures** - no methods/interfaces. Consumed by service layer and API controllers.

### Common Usage Patterns

#### **Controller → Service (Write)**
```csharp
[HttpPost]
[Route("api/job/save")]
public IHttpActionResult SaveJob(JobEditModel model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

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
```

#### **Service → Controller (Read)**
```csharp
[HttpGet]
[Route("api/job/{id}")]
public IHttpActionResult GetJob(long id)
{
    var model = _jobService.Get(id);
    if (model == null)
    {
        return NotFound();
    }
    return Ok(model);
}
```

#### **Image Upload**
```csharp
[HttpPost]
[Route("api/job/upload-images")]
public IHttpActionResult UploadImages()
{
    var model = new FileUploadModel
    {
        JobId = long.Parse(Request.Form["JobId"]),
        SchedulerId = long.Parse(Request.Form["SchedulerId"]),
        IsBeforeImage = bool.Parse(Request.Form["IsBeforeImage"]),
        ServiceTypeId = long.Parse(Request.Form["ServiceTypeId"]),
        Files = Request.Files.Cast<HttpPostedFileBase>().ToList(),
        SurfaceType = Request.Form["SurfaceType"],
        SurfaceMaterial = Request.Form["SurfaceMaterial"],
        BuildingLocation = Request.Form["BuildingLocation"]
    };

    var images = _jobService.SaveJobEstimateMediaFiles(model);
    return Ok(images);
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Application.ViewModel** — `EditModelBase` base class with validation support
- **Core.Application.Attribute** — `[NoValidatorRequired]` attribute
- **Core.Scheduler.Enum** — Enumerations referenced in ViewModels

### External Dependencies
- **System.ComponentModel.DataAnnotations** — Validation attributes (`[Required]`, `[Range]`, `[EmailAddress]`)
- **System.Web** — `HttpPostedFileBase` for file uploads
- **Newtonsoft.Json** — JSON serialization attributes

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Why So Many Properties on JobEditModel?

JobEditModel has 80+ properties because it represents:
- **Job entity** fields (status, dates, description)
- **Customer** nested object (name, address, phone)
- **Schedulers** collection (multi-tech assignments)
- **Notes** collection (comments)
- **Signatures** collections (pre/post)
- **UI display** fields (colors, formatted strings)
- **Workflow flags** (IsRepeat, IsActive, IsCustomerMailSend)

**Don't split into smaller models** - service layer expects complete graph for atomic save operations.

### EditModel vs ViewModel Distinction

- **EditModel**: For writes (POST/PUT) - includes validation attributes, supports partial updates
- **ViewModel**: For reads (GET) - optimized for display, includes computed fields, no validation

Example:
```csharp
// EditModel (write)
public class JobEditModel : EditModelBase
{
    [Required]
    public long JobTypeId { get; set; }
}

// ViewModel (read)
public class JobViewModel
{
    public long JobTypeId { get; set; }
    public string JobTypeName { get; set; }  // Computed from JobType.Name
    public string StatusColor { get; set; }  // UI-specific
}
```

### Base64 Image Encoding Trade-offs

`ReviewMarketingBeforeAfterImageViewModel` includes Base64 encoded images for inline display in admin gallery. 

**Pros:**
- Single API call loads all data (no follow-up image requests)
- Works with JSON API (no multipart/form-data needed)

**Cons:**
- Increases payload size by ~33% (Base64 overhead)
- Memory-intensive for large result sets

**Best Practice**: Always paginate, max 20 images per page.

### FileUploadModel Design Rationale

Why not just accept `List<HttpPostedFileBase>` in controller?

**Answer**: Need metadata for each file (surface type, location, before/after flag). Rather than parallel arrays, FileUploadModel bundles files + metadata.

For multiple files with different metadata, upload in separate requests or extend model to support per-file metadata.

### Validation Gotchas

1. **[NoValidatorRequired] attribute**: Disables automatic validation. Use when validation logic too complex for attributes (e.g., conditional required fields).

2. **Client-side vs server-side validation**: DataAnnotations only enforce server-side. Always check `ModelState.IsValid` in controller even if client validates.

3. **Custom validation**: Implement `IValidatableObject` for cross-property validation:
   ```csharp
   public class JobEditModel : EditModelBase, IValidatableObject
   {
       public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
       {
           if (StartDate >= EndDate)
           {
               yield return new ValidationResult("End date must be after start date", 
                   new[] { nameof(EndDate) });
           }
       }
   }
   ```

<!-- END CUSTOM SECTION -->
