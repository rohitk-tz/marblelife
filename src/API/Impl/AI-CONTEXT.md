# API/Impl - AI Context

## Purpose

The **Impl** folder contains implementation classes for API-specific helper services, utilities, and infrastructure components that support the API layer. These implementations handle cross-cutting concerns like session management, file operations, context storage, and exception handling.

## Key Files

### AppContextStore.cs
**Purpose**: Thread-safe storage for request-scoped data using `AsyncLocal<T>`.

**Interface**: `IAppContextStore` (defined in Core/Application)

**Implementation**:
```csharp
public class AppContextStore : IAppContextStore
{
    private readonly AsyncLocal<Dictionary<string, object>> _storage = 
        new AsyncLocal<Dictionary<string, object>>();

    public void SetValue(string key, object value)
    {
        if (_storage.Value == null)
            _storage.Value = new Dictionary<string, object>();
        
        _storage.Value[key] = value;
    }

    public T GetValue<T>(string key)
    {
        if (_storage.Value == null || !_storage.Value.ContainsKey(key))
            return default(T);
        
        return (T)_storage.Value[key];
    }

    public void ClearStorage()
    {
        _storage.Value?.Clear();
    }
}
```

**Usage**:
- Stores per-request context data (user session, correlation IDs, etc.)
- Automatically cleared in `Application_EndRequest` to prevent memory leaks
- Thread-safe for async/await operations

**Use Cases**:
```csharp
// Store correlation ID for request tracking
_appContextStore.SetValue("CorrelationId", Guid.NewGuid().ToString());

// Store user session
_appContextStore.SetValue("UserSession", currentUser);

// Retrieve later in different service/layer
var correlationId = _appContextStore.GetValue<string>("CorrelationId");
```

### SessionContext.cs
**Purpose**: Provides access to the current user's session information.

**Interface**: `ISessionContext` (defined in Core/Application)

**Implementation**:
```csharp
public class SessionContext : ISessionContext
{
    public UserSession UserSession { get; set; }
}
```

**UserSession Properties**:
- `UserId`: Unique identifier for the user
- `RoleId`: User's role (SuperAdmin, FranchiseeAdmin, Technician, etc.)
- `OrganizationId`: Franchisee/organization the user belongs to
- `Email`: User's email address
- `FullName`: User's display name
- `Token`: Authentication token
- `ExpiresAt`: Token expiration timestamp

**Population Flow**:
1. User logs in → Server generates token
2. Client includes token in request header
3. `Global.asax.cs: Application_BeginRequest` calls `SessionHelper.SetSessionModel(token)`
4. Session loaded from database/cache and stored in `ISessionContext`
5. Controllers access via dependency injection

**Usage in Controllers**:
```csharp
public class FranchiseeController : BaseController
{
    private readonly ISessionContext _sessionContext;

    [HttpGet]
    public FranchiseeListModel Get([FromUri] FranchiseeListFilter filter)
    {
        // Authorization check
        if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
        {
            // Limit to user's franchisee only
            filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
        }
        
        // Audit trail
        _logService.Info($"User {_sessionContext.UserSession.UserId} accessed franchisee list");
        
        return _franchiseeService.GetFranchisees(filter);
    }
}
```

### SessionHelper.cs
**Purpose**: Utility class for session management, token parsing, and timezone handling.

**Key Methods**:

#### SetSessionModel(string token)
```csharp
public static void SetSessionModel(string token)
{
    var sessionContext = ApplicationManager.DependencyInjection.Resolve<ISessionContext>();
    var userSession = LoadUserSessionFromToken(token);
    
    if (userSession != null && !IsExpired(userSession))
    {
        sessionContext.UserSession = userSession;
    }
}
```
Loads user session from token and populates `ISessionContext`.

#### SetClientTimeZone()
```csharp
public static void SetClientTimeZone()
{
    var request = HttpContext.Current.Request;
    var timezoneOffset = request.Headers.Get("timezoneoffset");
    
    if (!string.IsNullOrEmpty(timezoneOffset))
    {
        var appContextStore = ApplicationManager.DependencyInjection.Resolve<IAppContextStore>();
        appContextStore.SetValue("ClientTimezoneOffset", int.Parse(timezoneOffset));
    }
}
```
Stores client timezone offset for date/time conversions.

#### TokenKeyName
```csharp
public const string TokenKeyName = "token";
```
Standard header name for authentication tokens.

**Usage Flow**:
```
Client Request
    ↓
    Headers: { "token": "abc123", "timezoneoffset": "-300" }
    ↓
Global.asax: Application_BeginRequest
    ↓
SessionHelper.SetSessionModel(token)
    ↓ Loads user session
SessionHelper.SetClientTimeZone()
    ↓ Stores timezone offset
    ↓
Controller Action
    ↓ Access via ISessionContext
```

### FileUploadHelper.cs
**Purpose**: Handles multipart file uploads from HTTP requests.

**Key Methods**:

#### GetUploadedFiles()
```csharp
public static List<UploadedFile> GetUploadedFiles(HttpRequestMessage request)
{
    var files = new List<UploadedFile>();
    var provider = new MultipartMemoryStreamProvider();
    
    request.Content.ReadAsMultipartAsync(provider).Wait();
    
    foreach (var file in provider.Contents)
    {
        var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
        var stream = file.ReadAsStreamAsync().Result;
        
        files.Add(new UploadedFile
        {
            Filename = filename,
            ContentType = file.Headers.ContentType.MediaType,
            Stream = stream,
            Length = stream.Length
        });
    }
    
    return files;
}
```

**Usage in Controllers**:
```csharp
[HttpPost]
public async Task<IHttpActionResult> UploadDocument()
{
    if (!Request.Content.IsMimeMultipartContent())
        return BadRequest("Multipart content expected");

    var files = FileUploadHelper.GetUploadedFiles(Request);
    
    foreach (var file in files)
    {
        // Validate file
        if (file.Length > MaxFileSize)
            return BadRequest($"File {file.Filename} exceeds maximum size");
        
        // Save to storage
        await _fileService.SaveAsync(file.Stream, file.Filename);
    }
    
    return Ok(new { message = $"{files.Count} files uploaded successfully" });
}
```

**Supported Scenarios**:
- Single file upload
- Multiple file upload
- Mixed form data (files + JSON/form fields)
- Streaming large files
- File type validation
- Size validation

### FileDownloadHelper.cs
**Purpose**: Creates HTTP responses for file downloads with proper headers.

**Key Methods**:

#### CreateFileResponse()
```csharp
public static HttpResponseMessage CreateFileResponse(
    byte[] fileBytes, 
    string filename, 
    string contentType = "application/octet-stream")
{
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent(fileBytes)
    };
    
    response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
    {
        FileName = filename
    };
    
    return response;
}
```

#### CreateStreamResponse()
```csharp
public static HttpResponseMessage CreateStreamResponse(
    Stream stream, 
    string filename, 
    string contentType = "application/octet-stream")
{
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StreamContent(stream)
    };
    
    response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
    {
        FileName = filename
    };
    
    return response;
}
```

**Usage in Controllers**:
```csharp
[HttpGet]
public HttpResponseMessage DownloadInvoice(long invoiceId)
{
    var invoice = _invoiceService.Get(invoiceId);
    var pdfBytes = _pdfGenerator.GenerateInvoicePdf(invoice);
    
    return FileDownloadHelper.CreateFileResponse(
        pdfBytes, 
        $"Invoice-{invoice.InvoiceNumber}.pdf", 
        "application/pdf"
    );
}

[HttpGet]
public HttpResponseMessage DownloadReport(long reportId)
{
    var reportStream = _reportService.GenerateExcel(reportId);
    
    return FileDownloadHelper.CreateStreamResponse(
        reportStream, 
        $"Report-{reportId}.xlsx", 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    );
}
```

**Content Types**:
- `application/pdf` - PDF documents
- `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` - Excel (.xlsx)
- `application/vnd.ms-excel` - Excel (.xls)
- `text/csv` - CSV files
- `image/jpeg`, `image/png` - Images
- `application/zip` - ZIP archives
- `application/octet-stream` - Generic binary

## Exceptions Folder (Impl/Exceptions/)

### NotAuthenticatedException.cs
**Purpose**: Thrown when user is not authenticated (no valid token).

**Usage**:
```csharp
if (string.IsNullOrEmpty(token))
    throw new NotAuthenticatedException();
```

**HTTP Status**: 401 Unauthorized

### ValidationFailureException.cs
**Purpose**: Thrown when request validation fails.

**Usage**:
```csharp
if (string.IsNullOrEmpty(model.Email))
    throw new ValidationFailureException("Email is required");

if (!IsValidEmail(model.Email))
    throw new ValidationFailureException("Invalid email format");
```

**HTTP Status**: 400 Bad Request

**With Validation Details**:
```csharp
var errors = new Dictionary<string, string>
{
    { "Email", "Invalid email format" },
    { "Phone", "Phone number required" }
};

throw new ValidationFailureException("Validation failed", errors);
```

## For AI Agents

### Adding New Helper Service

1. **Create implementation**:
```csharp
// In API/Impl/
public class MyCustomHelper
{
    public static void DoSomething(HttpRequestMessage request)
    {
        // Implementation
    }
}
```

2. **Use in controllers**:
```csharp
[HttpPost]
public IHttpActionResult MyAction()
{
    MyCustomHelper.DoSomething(Request);
    return Ok();
}
```

### Extending Session Context

1. **Add property to ISessionContext**:
```csharp
public interface ISessionContext
{
    UserSession UserSession { get; set; }
    string NewProperty { get; set; } // Add this
}
```

2. **Update SessionContext.cs**:
```csharp
public class SessionContext : ISessionContext
{
    public UserSession UserSession { get; set; }
    public string NewProperty { get; set; } // Add this
}
```

3. **Populate in SessionHelper**:
```csharp
public static void SetSessionModel(string token)
{
    var sessionContext = Resolve<ISessionContext>();
    sessionContext.UserSession = LoadUserSession(token);
    sessionContext.NewProperty = "value"; // Set this
}
```

### Adding New Exception Type

1. **Create exception class**:
```csharp
// In API/Impl/Exceptions/
public class CustomException : Exception
{
    public CustomException(string message) : base(message) { }
}
```

2. **Handle in BasicExceptionFilterAttribute**:
```csharp
if (exception is CustomException)
{
    return new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
        Content = new StringContent(exception.Message)
    };
}
```

## For Human Developers

### Best Practices

#### Session Management
- Always check `_sessionContext.UserSession != null` before accessing properties
- Log session-related operations for audit trails
- Implement token expiration and refresh logic
- Clear sensitive session data after logout

#### File Operations
- Validate file types and sizes
- Scan uploaded files for viruses in production
- Use streaming for large files (avoid loading entire file in memory)
- Set appropriate content types for downloads
- Use descriptive filenames with proper extensions

#### Context Storage
- Clear context at end of request (`Application_EndRequest`)
- Use meaningful keys for stored values
- Don't store large objects in context
- Consider using correlation IDs for request tracking

#### Error Handling
- Use specific exception types (ValidationFailureException, NotAuthenticatedException)
- Include helpful error messages
- Log exceptions before throwing
- Don't expose sensitive information in error messages

### Common Patterns

#### Authorization Check
```csharp
private void EnsureFranchiseeAccess(long franchiseeId)
{
    if (_sessionContext.UserSession.RoleId == (long)RoleType.SuperAdmin)
        return; // Super admin can access all franchisees
    
    if (_sessionContext.UserSession.OrganizationId != franchiseeId)
        throw new UnauthorizedAccessException("Access denied to this franchisee");
}
```

#### Timezone Conversion
```csharp
private DateTime ConvertToClientTime(DateTime serverTime)
{
    var offset = _appContextStore.GetValue<int>("ClientTimezoneOffset");
    return serverTime.AddMinutes(-offset);
}
```

#### File Upload with Validation
```csharp
[HttpPost]
public IHttpActionResult UploadLogo()
{
    var files = FileUploadHelper.GetUploadedFiles(Request);
    
    if (files.Count == 0)
        throw new ValidationFailureException("No file uploaded");
    
    var file = files.First();
    
    // Validate type
    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
    if (!allowedTypes.Contains(file.ContentType))
        throw new ValidationFailureException("Only JPEG, PNG, and GIF images allowed");
    
    // Validate size
    if (file.Length > 5 * 1024 * 1024) // 5 MB
        throw new ValidationFailureException("File size must be less than 5 MB");
    
    _fileService.Save(file.Stream, file.Filename);
    return Ok(new { filename = file.Filename });
}
```

### Testing Helpers

```csharp
[Test]
public void SessionHelper_ValidToken_LoadsSession()
{
    var token = "valid-token";
    SessionHelper.SetSessionModel(token);
    
    var sessionContext = Resolve<ISessionContext>();
    Assert.IsNotNull(sessionContext.UserSession);
    Assert.AreEqual(123, sessionContext.UserSession.UserId);
}

[Test]
public void FileUploadHelper_ParsesMultipleFiles()
{
    var request = CreateMultipartRequest(
        new[] { "file1.txt", "file2.txt" },
        new[] { "content1", "content2" }
    );
    
    var files = FileUploadHelper.GetUploadedFiles(request);
    
    Assert.AreEqual(2, files.Count);
    Assert.AreEqual("file1.txt", files[0].Filename);
    Assert.AreEqual("file2.txt", files[1].Filename);
}
```

## Security Considerations

- **Token Storage**: Never log tokens in plaintext
- **Session Validation**: Validate session on every request
- **File Uploads**: Validate file types, scan for malware, limit sizes
- **File Downloads**: Authorize access before serving files
- **Context Data**: Don't store sensitive data in AppContextStore

## Performance Considerations

- **File Streaming**: Use streams for large files, not byte arrays
- **Session Caching**: Cache session data to avoid database hits
- **Context Cleanup**: Always clear context to prevent memory leaks
- **Async Operations**: Use async file I/O for better scalability

## Related Files

- **Global.asax.cs**: Uses SessionHelper, clears AppContextStore
- **Attribute/BasicAuthenticationAttribute.cs**: Uses SessionContext
- **DependencyInjection/ApiDependencyRegistrar.cs**: Registers implementations
- **Controllers/**: Use all helper services

## Migration to ASP.NET Core

When migrating to ASP.NET Core:
- **AppContextStore** → Use `IHttpContextAccessor` and `HttpContext.Items`
- **SessionContext** → Use built-in session or custom middleware
- **SessionHelper** → Middleware for token parsing
- **FileUploadHelper** → Use `IFormFile` for file uploads
- **FileDownloadHelper** → Use `File()` or `PhysicalFile()` in controllers
