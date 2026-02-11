<!-- AUTO-GENERATED: Header -->
# ViewModel — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Defines Data Transfer Objects (DTOs) that represent the contract between API controllers and clients (frontend, mobile apps, external APIs). ViewModels decouple domain entities from API representation, enabling versioned APIs, validation, and tailored responses without modifying domain logic.

### Design Patterns
- **DTO Pattern**: Separate data representation from domain entities
- **Response Envelope**: `ResponseModel` wraps all API responses with status and error handling
- **Validation Models**: Integrate with FluentValidation for declarative validation rules
- **Inheritance Hierarchy**: `EditModelBase` provides audit metadata for all edit forms
- **Factory Methods**: `FeedbackMessageModel` uses static factory methods for type-safe message creation

### Data Flow
1. **Request**: JSON → Model binding → ViewModel (with validation)
2. **Service**: ViewModel → Domain entity transformation
3. **Response**: Domain entity → ViewModel mapping → JSON
4. **Error Handling**: Exception → ResponseModel with MessageType.Error
5. **Success**: Data → ResponseModel.Data with MessageType.Success
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### ResponseModel.cs
```csharp
// Standard API response wrapper
public class ResponseModel
{
    public FeedbackMessageModel Message { get; set; }  // User feedback (success/warning/error)
    public int ErrorCode { get; set; }  // Error category (0 = success)
    public object Data { get; set; }  // Response payload (can be any type)
    
    public bool IsFeedbackSet { get; }  // True if Message is populated
    
    // Helper methods
    public void SetErrorMessage(string message, ResponseErrorCodeType errorCode = RandomException);
    public void SetSuccessMessage(string message);
}

// Error categories
public enum ResponseErrorCodeType
{
    UserNotAuthenticated = 1,
    UserBlocked = 2,
    ValidationFailure = 3,
    RandomException = 4,
    TrailExpired = 5,
    InvalidData = 6
}
```

### PostResponseModel.cs
```csharp
// Response for POST/PUT operations with validation feedback
public class PostResponseModel : ResponseModel
{
    public ModelValidationOutput ModelValidation { get; set; }  // Validation errors by field
}
```

### FeedbackMessageModel.cs
```csharp
// User-facing feedback message with severity level
public class FeedbackMessageModel
{
    public string Message { get; private set; }  // Feedback text
    public MessageType MessageType { get; private set; }  // Success/Warning/Error
    
    // Factory methods (preferred over direct instantiation)
    public static FeedbackMessageModel CreateSuccessMessage(string message);
    public static FeedbackMessageModel CreateWarningMessage(string message);
    public static FeedbackMessageModel CreateErrorMessage(string message);
}
```

### ModelValidationOutput.cs & ModelValidationItem.cs
```csharp
// Validation results container
public class ModelValidationOutput
{
    public bool IsValid { get; set; }  // True if no errors
    public List<ModelValidationItem> Errors { get; set; }  // Field-specific errors
}

// Individual field validation error
public class ModelValidationItem
{
    public string Name { get; set; }  // Property/field name
    public string Error { get; set; }  // Error message
    
    public ModelValidationItem(string name, string error) { ... }
}
```

### EditModelBase.cs
```csharp
// Base class for all edit forms - provides audit metadata
public class EditModelBase
{
    public DataRecorderMetaData DataRecorderMetaData { get; set; }  // CreatedBy, ModifiedBy, timestamps
    
    public EditModelBase()
    {
        DataRecorderMetaData = new DataRecorderMetaData();
    }
}
```

### FileModel.cs
```csharp
// DTO for file upload/download operations
[NoValidatorRequired]  // Pre-validated on client
public class FileModel : EditModelBase
{
    public long Id { get; set; }
    public string Name { get; set; }  // Original filename
    public string Caption { get; set; }  // Display name
    public decimal Size { get; set; }  // Bytes
    public string RelativeLocation { get; set; }  // Path relative to MediaRootPath
    public string MimeType { get; set; }
    public string Extension { get; set; }
    public string css { get; set; }  // Icon CSS class
    public string ImageUrl { get; set; }  // Public URL
    public string S3BucketImageUrl { get; set; }  // S3 URL (if using cloud storage)
    public string Url { get; set; }  // Download URL
    
    // Thumbnail properties
    public string ThumbImageUrl { get; set; }
    public long? ThumbFileId { get; set; }
    
    // Cropped image properties
    public bool? IsImageCropped { get; set; }
    public string CroppedImage { get; set; }
    public string CroppedImageUrl { get; set; }
    public long? CroppedImageFileId { get; set; }
    
    // Audit properties
    public long UserId { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public long? UploadByRoleId { get; set; }
}
```

### PagingModel.cs
```csharp
// Pagination metadata for list endpoints
public class PagingModel
{
    public int PageSize { get; private set; }  // Records per page
    public int TotalRecords { get; set; }  // Total count across all pages
    public int CurrentPage { get; set; }  // 1-based page index
    
    public PagingModel();  // Uses Settings.PageSize
    public PagingModel(int pageSize);
    public PagingModel(int currentPage, int pageSize, int totalRecords);
}
```

### SelectListModel.cs (DropDownListViewModel.cs)
```csharp
// Dropdown option representation
public class SelectListModel
{
    public long Value { get; set; }  // Option ID
    public string Display { get; set; }  // Display text
}
```

### HomeAdvisorParentModel.cs
```csharp
// DTO for HomeAdvisor lead imports
public class HomeAdvisorParentModel
{
    public long? Id { get; set; }
    public string HAAccount { get; set; }  // HomeAdvisor account ID
    public string FranchiseeName { get; set; }
    public string CompanyName { get; set; }
    public string SRID { get; set; }  // Service request ID
    public DateTime SRSubmittedDate { get; set; }
    public string Task { get; set; }  // Service type
    public decimal? NetLeadDollar { get; set; }  // Lead cost
    public long? CityId { get; set; }
    public long? StateId { get; set; }
    public string ZipCode { get; set; }
    public string LeadType { get; set; }
    public string CityName { get; set; }
    public string StateName { get; set; }
}
```

### ParsedFileParentModel.cs
```csharp
// DTO for Excel invoice/sales data imports
public class ParsedFileParentModel
{
    public string QbIdentifier { get; set; }  // QuickBooks ID
    public DateTime Date { get; set; }
    public long MarketingClassId { get; set; }
    public long? SubMarketingClassId { get; set; }
    public string SalesRep { get; set; }
    public long ServiceTypeId { get; set; }
    public CustomerCreateEditModel Customer { get; set; }  // Nested customer data
    public InvoiceEditModel Invoice { get; set; }  // Nested invoice data
    public AccountCreditEditModel AccountCredit { get; set; }  // Optional credit
    public long CustomerInvoiceId { get; set; }
    public string CustomerInvoiceIdString { get; set; }
}
```

### DeleteInvoiceResponseModel.cs
```csharp
// Response for delete operations with additional context
public class DeleteInvoiceResponseModel
{
    public string Response { get; set; }  // Success/error message
    public bool IsLastItem { get; set; }  // True if deleting last item in list
    public bool IsSuccess { get; set; }  // Operation result
    public bool IsStatusChanged { get; set; }  // True if status changed (soft delete)
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### ResponseModel (API Response Envelope)
- **Purpose**: Standardize all API responses with consistent error handling
- **Properties**:
  - `Message`: User-facing feedback (null for silent success)
  - `ErrorCode`: Machine-readable error category (0 = success)
  - `Data`: Response payload (can be any type - object, list, primitive)
- **Usage**: All controller actions return `ResponseModel` or `PostResponseModel`

### PostResponseModel (Form Submission Response)
- **Purpose**: Extends ResponseModel with field-level validation errors
- **Properties**:
  - `ModelValidation`: Contains `IsValid` flag and list of field errors
- **Usage**: POST/PUT endpoints return this for validation feedback

### FeedbackMessageModel (User Notifications)
- **Purpose**: Type-safe message creation with severity levels
- **Factory Methods**:
  - `CreateSuccessMessage(text)`: Green notification
  - `CreateWarningMessage(text)`: Yellow/orange alert
  - `CreateErrorMessage(text)`: Red error notification
- **Side Effects**: None - pure data object

### ModelValidationOutput & ModelValidationItem
- **Purpose**: Transport validation errors from server to client
- **Structure**: `{ IsValid: false, Errors: [{ Name: "Email", Error: "Required" }] }`
- **Integration**: Works with FluentValidation output

### EditModelBase
- **Purpose**: Provide audit trail for all edit forms
- **Inheritance**: All edit DTOs inherit from this
- **Usage**: Automatically includes CreatedBy, ModifiedBy, timestamps in all edit operations

### FileModel
- **Purpose**: File upload/download DTO with metadata
- **Key Features**:
  - Supports local filesystem and S3 storage
  - Thumbnail and cropped image tracking
  - Audit trail (CreatedBy, UploadByRoleId)
- **Validation**: Uses `[NoValidatorRequired]` - validation done on client

### PagingModel
- **Purpose**: Pagination metadata for list endpoints
- **Usage**:
  ```csharp
  var paging = new PagingModel(currentPage, pageSize, totalRecords);
  return new ResponseModel { Data = new { items, paging } };
  ```

### SelectListModel
- **Purpose**: Dropdown option representation
- **Usage**: Populate dropdowns from lookup tables:
  ```csharp
  var options = lookups.Select(l => new SelectListModel 
  { 
      Value = l.Id, 
      Display = l.Name 
  }).ToList();
  ```

### HomeAdvisorParentModel & ParsedFileParentModel
- **Purpose**: Excel import DTOs for specific data formats
- **Usage**: Returned by file parsers after Excel processing
- **Validation**: Parsers throw `InvalidFileUploadException` on format errors
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Application.Domain.DataRecorderMetaData** — Audit metadata in EditModelBase
- **Core.Application.Enum.MessageType** — Feedback message severity
- **Core.Application.Attribute.NoValidatorRequiredAttribute** — Skip validation for some models
- **Core.Billing.ViewModel.InvoiceEditModel** — Nested in ParsedFileParentModel
- **Core.Sales.ViewModel.CustomerCreateEditModel** — Nested in ParsedFileParentModel

### External Dependencies
- **System.Runtime.Serialization** — [DataContract] attributes for WCF compatibility
- **System.Xml.Serialization** — [XmlIgnore] for XML serialization

### Referenced By
- **Controllers** — All API endpoints use these ViewModels
- **Services** — Map between ViewModels and domain entities
- **Frontend** — JavaScript/TypeScript consume these DTOs via JSON
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Response Model Best Practices

**Always use ResponseModel for consistency:**
```csharp
// ✅ GOOD - Consistent response structure
public IActionResult GetInvoice(long id)
{
    var invoice = invoiceService.Get(id);
    return Ok(new ResponseModel { Data = invoice });
}

// ❌ BAD - Inconsistent response
public IActionResult GetInvoice(long id)
{
    var invoice = invoiceService.Get(id);
    return Ok(invoice);  // No error handling wrapper
}
```

**Set messages for user feedback:**
```csharp
var response = new ResponseModel 
{ 
    Data = savedInvoice,
    Message = FeedbackMessageModel.CreateSuccessMessage("Invoice saved successfully")
};
```

### Validation Flow
1. **Request**: Client sends JSON → Model binding → ViewModel
2. **FluentValidation**: Validator runs automatically (if registered)
3. **Controller**: Check `ModelState.IsValid`
4. **Response**: Return `PostResponseModel` with `ModelValidation` errors

### EditModelBase Usage
All edit forms automatically get audit metadata:
```csharp
public class InvoiceEditModel : EditModelBase
{
    public string Number { get; set; }
    public decimal Total { get; set; }
    // DataRecorderMetaData inherited from base class
}

// In service layer
invoice.DataRecorderMetaData = model.DataRecorderMetaData;
invoice.DataRecorderMetaData.SetModifiedBy(currentUserId);
```

### FileModel vs File Entity
- **FileModel**: API layer (includes URLs, S3 paths, client-facing data)
- **File Entity**: Database layer (only core metadata, relative paths)
- Always map between them - don't expose entities directly to API

### Common Pitfalls
- **Forgetting ErrorCode**: Set `ErrorCode = 0` for success, non-zero for errors
- **Null Data**: Check `response.Data != null` on frontend before accessing
- **Validation bypass**: `NoValidatorRequiredAttribute` should only be used for pre-validated DTOs
- **Message type mismatch**: Don't set `MessageType.Success` with `ErrorCode != 0`
<!-- END CUSTOM SECTION -->
