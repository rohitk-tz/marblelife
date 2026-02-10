<!-- AUTO-GENERATED: Header -->
# ViewModel
> Data Transfer Objects (DTOs) for API communication, validation, and response formatting
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **ViewModel** module contains 13 DTO classes that define the contract between API controllers and clients. These classes separate API representation from domain entities, enabling versioned APIs, client-specific views, and consistent error handling.

**ViewModel Categories:**
1. **Response Wrappers**: ResponseModel, PostResponseModel, DeleteInvoiceResponseModel
2. **Validation**: ModelValidationOutput, ModelValidationItem
3. **Feedback**: FeedbackMessageModel (with MessageType enum)
4. **Base Classes**: EditModelBase (audit metadata for all edit forms)
5. **Specific DTOs**: FileModel, PagingModel, SelectListModel
6. **Import DTOs**: HomeAdvisorParentModel, ParsedFileParentModel

**Why separate ViewModels from Entities?**
- **API Versioning**: Change ViewModel without breaking domain logic
- **Security**: Control which fields are exposed to clients
- **Validation**: Add validation attributes/rules without polluting domain
- **Computed Properties**: Include calculated fields (URLs, formatted dates)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Return Success Response
```csharp
using Core.Application.ViewModel;

[HttpGet("{id}")]
public IActionResult GetInvoice(long id)
{
    var invoice = invoiceService.GetById(id);
    
    if (invoice == null)
    {
        var errorResponse = new ResponseModel();
        errorResponse.SetErrorMessage("Invoice not found", ResponseErrorCodeType.InvalidData);
        return NotFound(errorResponse);
    }
    
    var response = new ResponseModel
    {
        Data = invoice,
        Message = FeedbackMessageModel.CreateSuccessMessage("Invoice retrieved")
    };
    
    return Ok(response);
}
```

### Example 2: Return Validation Errors
```csharp
using Core.Application.ViewModel;

[HttpPost]
public IActionResult CreateInvoice(InvoiceEditModel model)
{
    if (!ModelState.IsValid)
    {
        var response = new PostResponseModel
        {
            ModelValidation = new ModelValidationOutput
            {
                IsValid = false,
                Errors = ModelState.Select(kvp => new ModelValidationItem(
                    kvp.Key, 
                    kvp.Value.Errors.First().ErrorMessage
                )).ToList()
            }
        };
        response.SetErrorMessage("Validation failed", ResponseErrorCodeType.ValidationFailure);
        return BadRequest(response);
    }
    
    var invoice = invoiceService.Create(model);
    
    var successResponse = new PostResponseModel
    {
        Data = invoice,
        ModelValidation = new ModelValidationOutput { IsValid = true }
    };
    successResponse.SetSuccessMessage("Invoice created successfully");
    
    return Ok(successResponse);
}
```

### Example 3: Create Feedback Messages
```csharp
using Core.Application.ViewModel;

// Success notification (green toast)
var successMsg = FeedbackMessageModel.CreateSuccessMessage("File uploaded successfully");

// Warning notification (yellow/orange alert)
var warningMsg = FeedbackMessageModel.CreateWarningMessage("File is large, upload may take time");

// Error notification (red toast)
var errorMsg = FeedbackMessageModel.CreateErrorMessage("Upload failed: invalid file format");

// Use in response
var response = new ResponseModel
{
    Message = successMsg,
    ErrorCode = 0,
    Data = uploadedFile
};
```

### Example 4: Handle Pagination
```csharp
using Core.Application.ViewModel;

[HttpGet]
public IActionResult GetInvoices(int page = 1, int pageSize = 25)
{
    int totalRecords = invoiceRepository.Count();
    var invoices = invoiceRepository
        .GetAll()
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();
    
    var paging = new PagingModel(page, pageSize, totalRecords);
    
    return Ok(new ResponseModel
    {
        Data = new
        {
            invoices,
            paging = new
            {
                paging.PageSize,
                paging.CurrentPage,
                paging.TotalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            }
        }
    });
}
```

### Example 5: Populate Dropdown
```csharp
using Core.Application.ViewModel;

[HttpGet("invoice-statuses")]
public IActionResult GetInvoiceStatuses()
{
    var statuses = lookupRepository
        .GetAll()
        .Where(l => l.LookupTypeId == (long)LookupTypes.InvoiceStatus && l.IsActive)
        .OrderBy(l => l.RelativeOrder)
        .Select(l => new SelectListModel
        {
            Value = l.Id,
            Display = l.Name
        })
        .ToList();
    
    return Ok(new ResponseModel { Data = statuses });
}
```

### Example 6: Upload File DTO
```csharp
using Core.Application.ViewModel;

[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
    // Save file
    var savedFile = await fileService.SaveFileAsync(file);
    
    // Map to FileModel DTO
    var fileModel = new FileModel
    {
        Id = savedFile.Id,
        Name = savedFile.Name,
        Caption = savedFile.Caption,
        Size = savedFile.Size,
        MimeType = savedFile.MimeType,
        Url = $"/api/files/{savedFile.Id}/download",
        ImageUrl = savedFile.RelativeLocation.ToUrl(),
        ThumbImageUrl = $"/api/files/{savedFile.Id}/thumbnail",
        CreatedOn = savedFile.DataRecorderMetaData.DateCreated,
        CreatedBy = userService.GetUsername(savedFile.DataRecorderMetaData.CreatedBy)
    };
    
    return Ok(new ResponseModel 
    { 
        Data = fileModel,
        Message = FeedbackMessageModel.CreateSuccessMessage("File uploaded")
    });
}
```

### Example 7: Use EditModelBase for Audit
```csharp
using Core.Application.ViewModel;

public class CustomerEditModel : EditModelBase
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // DataRecorderMetaData inherited from EditModelBase
}

[HttpPut("{id}")]
public IActionResult UpdateCustomer(long id, CustomerEditModel model)
{
    var customer = customerRepository.GetById(id);
    
    // Update properties
    customer.Name = model.Name;
    customer.Email = model.Email;
    
    // Update audit metadata
    customer.DataRecorderMetaData = model.DataRecorderMetaData;
    customer.DataRecorderMetaData.SetModifiedBy(currentUserId);
    
    customerRepository.Update(customer);
    unitOfWork.Commit();
    
    return Ok(new ResponseModel 
    { 
        Data = customer,
        Message = FeedbackMessageModel.CreateSuccessMessage("Customer updated")
    });
}
```

### Example 8: Handle Delete Response
```csharp
using Core.Application.ViewModel;

[HttpDelete("{id}")]
public IActionResult DeleteInvoice(long id)
{
    var invoice = invoiceRepository.GetById(id);
    bool isLastItem = invoiceRepository.Count() == 1;
    
    invoiceRepository.Delete(invoice);
    unitOfWork.Commit();
    
    var response = new DeleteInvoiceResponseModel
    {
        Response = "Invoice deleted successfully",
        IsSuccess = true,
        IsLastItem = isLastItem,
        IsStatusChanged = false  // Hard delete, not status change
    };
    
    return Ok(response);
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Response Models
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `ResponseModel` | Standard API response wrapper | `Message`, `ErrorCode`, `Data` |
| `PostResponseModel` | Form submission response | Inherits ResponseModel + `ModelValidation` |
| `DeleteInvoiceResponseModel` | Delete operation response | `Response`, `IsSuccess`, `IsLastItem`, `IsStatusChanged` |

### Validation Models
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `ModelValidationOutput` | Validation result container | `IsValid`, `Errors` (list) |
| `ModelValidationItem` | Single field error | `Name` (field name), `Error` (message) |

### Feedback Models
| Model | Purpose | Key Methods/Properties |
|-------|---------|------------------------|
| `FeedbackMessageModel` | User notification with severity | `CreateSuccessMessage()`, `CreateWarningMessage()`, `CreateErrorMessage()` |

### Base Classes
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `EditModelBase` | Audit metadata for edit forms | `DataRecorderMetaData` (CreatedBy, ModifiedBy, timestamps) |

### Specific DTOs
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `FileModel` | File upload/download DTO | `Name`, `Size`, `RelativeLocation`, `Url`, `ImageUrl`, `ThumbImageUrl` |
| `PagingModel` | Pagination metadata | `PageSize`, `TotalRecords`, `CurrentPage` |
| `SelectListModel` | Dropdown option | `Value` (ID), `Display` (text) |

### Import DTOs
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `HomeAdvisorParentModel` | HomeAdvisor lead import | `HAAccount`, `SRID`, `FranchiseeName`, `NetLeadDollar` |
| `ParsedFileParentModel` | Excel invoice import | `Customer`, `Invoice`, `AccountCredit`, `QbIdentifier` |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Frontend receives null for Data property
**Cause**: Data not set in ResponseModel before returning.
**Solution**: Always populate Data property:
```csharp
// ❌ BAD - Data is null
return Ok(new ResponseModel { Message = successMsg });

// ✅ GOOD
return Ok(new ResponseModel { Message = successMsg, Data = invoice });
```

### Issue: Validation errors not displaying on frontend
**Cause**: ModelValidation not populated in PostResponseModel.
**Solution**: Map ModelState errors to ModelValidationOutput:
```csharp
var response = new PostResponseModel
{
    ModelValidation = new ModelValidationOutput
    {
        IsValid = false,
        Errors = ModelState
            .Where(kvp => kvp.Value.Errors.Any())
            .Select(kvp => new ModelValidationItem(kvp.Key, kvp.Value.Errors.First().ErrorMessage))
            .ToList()
    }
};
```

### Issue: ErrorCode is always 0 even for errors
**Cause**: Forgot to set ErrorCode when calling SetErrorMessage.
**Solution**: Use ResponseErrorCodeType enum:
```csharp
response.SetErrorMessage("Invalid data", ResponseErrorCodeType.ValidationFailure);
// Sets ErrorCode = 3
```

### Issue: Message color is wrong on frontend
**Cause**: Frontend checking wrong property or using wrong MessageType value.
**Solution**: Frontend should check `response.Message.MessageType`:
```javascript
if (response.Message.MessageType === 1) {
    // Success - green
} else if (response.Message.MessageType === 2) {
    // Warning - yellow
} else if (response.Message.MessageType === 3) {
    // Error - red
}
```

### Issue: EditModelBase.DataRecorderMetaData is null
**Cause**: Constructor not called during deserialization.
**Solution**: Initialize in controller if null:
```csharp
[HttpPost]
public IActionResult Create(CustomerEditModel model)
{
    if (model.DataRecorderMetaData == null)
    {
        model.DataRecorderMetaData = new DataRecorderMetaData(currentUserId);
    }
    // ...
}
```

### Issue: FileModel has no Url after upload
**Cause**: Url not set in mapping logic.
**Solution**: Generate URLs when mapping File → FileModel:
```csharp
fileModel.Url = $"/api/files/{file.Id}/download";
fileModel.ImageUrl = file.RelativeLocation.ToUrl();
fileModel.ThumbImageUrl = $"/api/files/{file.Id}/thumbnail";
```

### Issue: Pagination shows wrong total pages
**Cause**: TotalRecords not set or calculated incorrectly.
**Solution**: Always query total count before pagination:
```csharp
// ✅ CORRECT order
int totalRecords = query.Count();  // Count before pagination
var items = query.Skip(...).Take(...).ToList();
var paging = new PagingModel(page, pageSize, totalRecords);

// ❌ WRONG - Count after pagination
var items = query.Skip(...).Take(...).ToList();
int totalRecords = items.Count();  // Always returns pageSize or less
```

### Issue: SelectListModel missing after serialization
**Cause**: Check class name - file is DropDownListViewModel.cs but class is SelectListModel.
**Solution**: Use correct class name `SelectListModel` when deserializing.

### Issue: Nested objects in ParsedFileParentModel are null
**Cause**: Constructor doesn't initialize nested objects, or JSON missing nested data.
**Solution**: Initialize in constructor:
```csharp
public ParsedFileParentModel()
{
    Customer = new CustomerCreateEditModel();
    Invoice = new InvoiceEditModel();
}
```
<!-- END CUSTOM SECTION -->
