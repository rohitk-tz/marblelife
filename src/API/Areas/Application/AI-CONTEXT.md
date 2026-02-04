# API/Areas/Application - AI Context

## Purpose

The **Application** area contains core utility controllers and services that provide common functionality used across the entire API. This includes the base controller class, dropdown data providers, file operations, and shared helper services.

## Structure

```
Application/
├── ApplicationAreaRegistration.cs    # Area route registration
├── Controller/
│   ├── BaseController.cs            # Base class for all API controllers
│   ├── DropdownController.cs        # Lookup data for dropdowns
│   └── FileController.cs            # File upload/download operations
├── Enum/                            # Application-specific enumerations
├── Impl/
│   └── DropDownHelperService.cs     # Implementation of dropdown service
├── ViewModel/                       # Request/response models
└── IDropDownHelperService.cs        # Dropdown service interface
```

## Key Components

### BaseController.cs
**Purpose**: Abstract base class that all API controllers inherit from, providing common functionality and infrastructure.

**Features**:
- **ResponseModel Management**: Standardized response format
- **PostResponseModel**: Extended response for POST operations with validation
- **Model Validation**: Automatic validation error handling
- **Logging**: Built-in logger access
- **Action Arguments**: Captures and stores request parameters

**Properties**:
```csharp
protected ResponseModel ResponseModel { get; }          // Standard response
protected PostResponseModel PostResponseModel { get; }  // POST response with validation
protected ILogService Logger { get; }                   // Logging service
internal HttpMethodType MethodType { get; set; }        // Current HTTP method
internal string Token { get; set; }                     // Authentication token
```

**Key Methods**:
```csharp
void SetPostResponseModel(HttpMethodType type)          // Initialize POST response
void SetResponseModel(HttpMethodType type)              // Initialize standard response
void SetValidationResult(ModelStateDictionary state)    // Process validation errors
ResponseModel SetData(object data)                      // Set response data
```

**Usage in Derived Controllers**:
```csharp
[BasicAuthentication]
public class FranchiseeController : BaseController
{
    [HttpPost]
    public bool Post([FromBody] FranchiseeEditModel model)
    {
        // Business logic
        _franchiseeService.Save(model);
        
        // Use inherited PostResponseModel to set success message
        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage(
            "Franchisee saved successfully."
        );
        
        return true;
    }
    
    [HttpGet]
    public FranchiseeViewModel Get(long id)
    {
        // Use inherited Logger
        Logger.Info($"Retrieving franchisee {id}");
        
        return _franchiseeService.Get(id);
    }
}
```

**Response Standardization**:
All responses follow a consistent structure:
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* response data */ },
  "modelValidation": {
    "isValid": true,
    "errors": []
  }
}
```

### DropdownController.cs
**Purpose**: Provides lookup data for dropdown lists, selection boxes, and autocomplete fields across the application.

**Common Endpoints**:
```
GET /Application/Dropdown/GetFranchisees        # List of franchisees
GET /Application/Dropdown/GetServiceTypes       # Available services
GET /Application/Dropdown/GetStatuses          # Status enumerations
GET /Application/Dropdown/GetRoles             # User roles
GET /Application/Dropdown/GetStates            # US states/provinces
GET /Application/Dropdown/GetCustomers         # Customer list
GET /Application/Dropdown/GetTechnicians       # Technician list
```

**Response Format**:
```json
{
  "data": [
    { "id": 1, "name": "Option 1", "value": "value1" },
    { "id": 2, "name": "Option 2", "value": "value2" }
  ]
}
```

**Typical Usage**:
```csharp
[HttpGet]
public List<DropdownItem> GetFranchisees()
{
    var role = _sessionContext.UserSession.RoleId;
    var organizationId = _sessionContext.UserSession.OrganizationId;
    
    // Super admin sees all franchisees
    if (role == (long)RoleType.SuperAdmin)
    {
        return _dropDownHelper.GetAllFranchisees();
    }
    
    // Franchisee users see only their franchisee
    return _dropDownHelper.GetFranchisee(organizationId);
}
```

**Client-Side Usage**:
```javascript
// Fetch dropdown data
fetch('/Application/Dropdown/GetFranchisees', {
    headers: { 'token': userToken }
})
.then(response => response.json())
.then(data => {
    // Populate dropdown
    data.data.forEach(item => {
        dropdown.add(new Option(item.name, item.id));
    });
});
```

### FileController.cs
**Purpose**: Handles file upload and download operations for documents, images, and attachments.

**Key Endpoints**:
```
POST /Application/File/Upload          # Upload files
GET  /Application/File/Download/{id}   # Download file by ID
GET  /Application/File/Preview/{id}    # Preview file (image/PDF)
DELETE /Application/File/Delete/{id}   # Delete uploaded file
```

**Upload Implementation**:
```csharp
[HttpPost]
public async Task<IHttpActionResult> Upload()
{
    if (!Request.Content.IsMimeMultipartContent())
        return BadRequest("Multipart content expected");

    var files = FileUploadHelper.GetUploadedFiles(Request);
    var uploadedFiles = new List<FileUploadResult>();
    
    foreach (var file in files)
    {
        // Validate
        if (!IsValidFileType(file.ContentType))
            return BadRequest($"File type {file.ContentType} not allowed");
        
        if (file.Length > MaxFileSize)
            return BadRequest($"File exceeds maximum size");
        
        // Save to storage
        var fileId = await _fileService.SaveAsync(
            file.Stream, 
            file.Filename,
            _sessionContext.UserSession.UserId
        );
        
        uploadedFiles.Add(new FileUploadResult
        {
            FileId = fileId,
            Filename = file.Filename,
            Size = file.Length
        });
    }
    
    return Ok(uploadedFiles);
}
```

**Download Implementation**:
```csharp
[HttpGet]
public HttpResponseMessage Download(long id)
{
    var file = _fileService.Get(id);
    
    if (file == null)
        return Request.CreateResponse(HttpStatusCode.NotFound);
    
    // Authorization check
    if (!CanAccessFile(file, _sessionContext.UserSession))
        return Request.CreateResponse(HttpStatusCode.Forbidden);
    
    var bytes = _fileService.GetFileBytes(id);
    
    return FileDownloadHelper.CreateFileResponse(
        bytes, 
        file.Filename, 
        file.ContentType
    );
}
```

**Supported File Types**:
- **Images**: JPEG, PNG, GIF (for photos, logos)
- **Documents**: PDF, DOCX, XLSX (for invoices, reports)
- **Archives**: ZIP (for bulk downloads)

### IDropDownHelperService.cs & DropDownHelperService.cs
**Purpose**: Service interface and implementation for retrieving lookup data efficiently.

**Key Methods**:
```csharp
public interface IDropDownHelperService
{
    List<DropdownItem> GetFranchisees(long? roleId = null);
    List<DropdownItem> GetServiceTypes();
    List<DropdownItem> GetCustomerStatuses();
    List<DropdownItem> GetJobStatuses();
    List<DropdownItem> GetRoles();
    List<DropdownItem> GetStates();
    List<DropdownItem> GetCountries();
    List<DropdownItem> GetPaymentMethods();
}
```

**Caching Strategy**:
```csharp
public class DropDownHelperService : IDropDownHelperService
{
    private readonly IRepository<Franchisee> _franchiseeRepo;
    private static readonly MemoryCache _cache = MemoryCache.Default;
    
    public List<DropdownItem> GetServiceTypes()
    {
        var cacheKey = "ServiceTypes";
        
        if (_cache.Contains(cacheKey))
            return _cache.Get(cacheKey) as List<DropdownItem>;
        
        var items = _repository.GetAll()
            .Select(s => new DropdownItem 
            { 
                Id = s.Id, 
                Name = s.Name 
            })
            .ToList();
        
        _cache.Add(cacheKey, items, DateTimeOffset.Now.AddHours(24));
        
        return items;
    }
}
```

## Common ViewModels

### DropdownItem
```csharp
public class DropdownItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; }
}
```

### ResponseModel
```csharp
public class ResponseModel
{
    public bool Success { get; set; } = true;
    public string Message { get; set; }
    public object Data { get; set; }
}
```

### PostResponseModel
```csharp
public class PostResponseModel : ResponseModel
{
    public ModelValidationOutput ModelValidation { get; set; }
}

public class ModelValidationOutput
{
    public bool IsValid { get; set; }
    public List<ModelValidationItem> Errors { get; set; }
}

public class ModelValidationItem
{
    public string Field { get; set; }
    public string Message { get; set; }
}
```

## For AI Agents

### Creating New Controller

1. **Inherit from BaseController**:
```csharp
[BasicAuthentication]
public class MyNewController : BaseController
{
    private readonly IMyService _service;
    
    public MyNewController(IMyService service)
    {
        _service = service;
    }
}
```

2. **Use inherited features**:
```csharp
[HttpGet]
public MyViewModel Get(long id)
{
    Logger.Info($"Getting record {id}");
    return _service.Get(id);
}

[HttpPost]
public bool Post([FromBody] MyViewModel model)
{
    _service.Save(model);
    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Saved!");
    return true;
}
```

### Adding Dropdown Endpoint

1. **Add method to IDropDownHelperService**:
```csharp
List<DropdownItem> GetMyNewLookup();
```

2. **Implement in DropDownHelperService**:
```csharp
public List<DropdownItem> GetMyNewLookup()
{
    return _repository.GetAll()
        .Select(x => new DropdownItem { Id = x.Id, Name = x.Name })
        .ToList();
}
```

3. **Add controller endpoint**:
```csharp
[HttpGet]
public List<DropdownItem> GetMyNewLookup()
{
    return _dropDownHelper.GetMyNewLookup();
}
```

## For Human Developers

### Best Practices

#### Controller Design
- Always inherit from `BaseController`
- Use dependency injection for services
- Apply `[BasicAuthentication]` unless truly public
- Use `Logger` for important operations
- Set success messages in `PostResponseModel`

#### Response Handling
- Return consistent response structures
- Include meaningful error messages
- Set appropriate HTTP status codes
- Use `PostResponseModel` for validation feedback

#### File Operations
- Validate file types and sizes
- Check authorization before download
- Use streaming for large files
- Clean up temporary files
- Log file operations for audit

#### Dropdown Data
- Cache static/slow-changing data
- Filter by user permissions
- Return minimal data (ID, Name, Value)
- Consider pagination for large lists
- Invalidate cache when data changes

### Common Patterns

#### Authorization in Controllers
```csharp
private void EnsureAccess(long resourceId)
{
    var role = _sessionContext.UserSession.RoleId;
    var orgId = _sessionContext.UserSession.OrganizationId;
    
    if (role != (long)RoleType.SuperAdmin)
    {
        var resource = _service.Get(resourceId);
        if (resource.OrganizationId != orgId)
            throw new UnauthorizedAccessException();
    }
}
```

#### Validation Error Handling
```csharp
[HttpPost]
public IHttpActionResult Post([FromBody] MyViewModel model)
{
    if (!ModelState.IsValid)
    {
        SetValidationResult(ModelState);
        return BadRequest(PostResponseModel);
    }
    
    _service.Save(model);
    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Saved!");
    return Ok(true);
}
```

#### File Upload with Validation
```csharp
[HttpPost]
public async Task<IHttpActionResult> UploadPhoto()
{
    var files = FileUploadHelper.GetUploadedFiles(Request);
    
    if (files.Count == 0)
        return BadRequest("No file uploaded");
    
    var file = files.First();
    
    // Validate type
    if (!new[] { "image/jpeg", "image/png" }.Contains(file.ContentType))
        return BadRequest("Only JPEG and PNG images allowed");
    
    // Validate size (5 MB)
    if (file.Length > 5 * 1024 * 1024)
        return BadRequest("File must be less than 5 MB");
    
    var fileId = await _fileService.SaveAsync(file.Stream, file.Filename);
    return Ok(new { fileId, filename = file.Filename });
}
```

## Security Considerations

- **Authentication**: Apply `[BasicAuthentication]` to all endpoints except login
- **Authorization**: Check user permissions before data access
- **File Validation**: Validate file types, sizes, and content
- **Input Sanitization**: Validate and sanitize all inputs
- **CORS**: Handled in Global.asax.cs

## Performance Considerations

- **Caching**: Cache dropdown data that changes infrequently
- **Pagination**: Implement pagination for large datasets
- **Async I/O**: Use async/await for file operations
- **Streaming**: Stream large files instead of loading in memory

## Related Files

- **Global.asax.cs**: Application startup, calls area registration
- **App_Start/WebApiConfig.cs**: Routing configuration
- **Attribute/**: Authentication and exception filters
- **Core/Application/**: Interfaces and base services

## Testing

```csharp
[Test]
public void DropdownController_GetFranchisees_ReturnsData()
{
    var controller = new DropdownController(_dropDownHelper, _sessionContext);
    var result = controller.GetFranchisees();
    
    Assert.IsNotNull(result);
    Assert.IsTrue(result.Count > 0);
}

[Test]
public void FileController_Upload_ValidatesFileType()
{
    var controller = new FileController(_fileService, _sessionContext);
    var request = CreateMultipartRequest("virus.exe", "application/x-msdownload");
    
    var result = controller.Upload().Result;
    
    Assert.IsInstanceOf<BadRequestResult>(result);
}
```
