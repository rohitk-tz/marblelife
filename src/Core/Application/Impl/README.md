# Core.Application.Impl

> **Production implementations of cross-cutting infrastructure services** for the Marblelife Franchisee Management System.

## Overview

This module provides **battle-tested implementations** of the core application infrastructure contracts. Think of it as the **"Swiss Army Knife"** of the system - it handles all the unglamorous but critical tasks:

- **Time & Timezone Management**: Because franchises operate across multiple US timezones
- **Security**: Password hashing that meets industry standards (well, mostly)
- **File Operations**: Excel import/export for business data, PDF generation for documents
- **Configuration**: Centralized access to 100+ application settings
- **Session Management**: User authentication state and franchisee context switching

### Key Philosophy

These implementations are **deliberately separated** from the Core.Application interfaces to allow:
- **Testability**: Mock implementations for unit tests
- **Flexibility**: Swap implementations (e.g., different PDF engines) without changing contracts
- **Dependency Inversion**: High-level modules depend on abstractions, not concrete implementations

---

## 🚀 Quick Start

### Time Management

```csharp
// Injected via DI
private readonly IClock _clock;

// Get current UTC time
var utcNow = _clock.UtcNow;

// Convert to franchisee's local time (EST default)
var localTime = _clock.ToLocal(utcNow);

// Convert with specific timezone
_clock.SetTimeZoneInfo("Pacific Standard Time");
var pstTime = _clock.ToLocal(utcNow);

// Convert from offset (e.g., from browser)
var offsetMinutes = -300; // EST = UTC-5
var localFromOffset = _clock.ToLocal(utcNow, offsetMinutes);

// Date helpers
var firstDay = _clock.FirstDayOfWeek(DateTime.Today);
var lastDay = _clock.LastDayOfMonth(DateTime.Today);
```

**Common Timezones**:
- `"Eastern Standard Time"` (default)
- `"Central Standard Time"`
- `"Mountain Standard Time"`
- `"Pacific Standard Time"`

---

### Password Security

```csharp
private readonly ICryptographyOneWayHashService _hashService;

// Hash a password (e.g., during registration)
var password = "SecureP@ssw0rd123";
var hash = _hashService.CreateHash(password);

// Store hash.HashedText and hash.Salt in database
person.PasswordHash = hash.HashedText;
person.PasswordSalt = hash.Salt;
_repository.Save(person);

// Validate password (e.g., during login)
var inputPassword = "SecureP@ssw0rd123";
var storedHash = new SecureHash 
{ 
    HashedText = person.PasswordHash, 
    Salt = person.PasswordSalt 
};

var isValid = _hashService.Validate(inputPassword, storedHash);
if (isValid)
{
    // Grant access
}
```

**Security Features**:
- ✅ Random 24-byte salt per password
- ✅ PBKDF2-SHA1 with 1000 iterations
- ✅ Timing-attack resistant comparison
- ⚠️ Iteration count is below modern standards (should be 10,000+)

---

### File Management

#### Saving Files

```csharp
private readonly IFileService _fileService;

var fileModel = new FileModel
{
    Name = "invoice_2024.pdf",
    Caption = "January 2024 Invoice",
    Size = 150000, // bytes
    MimeType = "application/pdf",
    RelativeLocation = "Invoices\\2024\\01"
};

var destination = MediaLocationHelper.GetInvoiceLocation();
var savedFile = _fileService.SaveFile(fileModel, destination, "INV_");

// savedFile.Id is now populated from database
Console.WriteLine($"File saved with ID: {savedFile.Id}");
```

#### Retrieving Files

```csharp
var fileModel = _fileService.Get(fileId: 12345);
Console.WriteLine($"File URL: {fileModel.RelativeLocation}");
```

#### Moving Files

```csharp
var tempLocation = MediaLocationHelper.GetTempMediaLocation();
var finalLocation = MediaLocationHelper.GetJobMediaLocation();

var sourcePath = tempLocation.Path + "\\temp_upload.jpg";
var finalPath = _fileService.MoveFile(
    source: sourcePath,
    destination: finalLocation,
    fileName: "job_photo_123",
    ext: ".jpg"
);
```

---

### Media Locations

```csharp
// Standard folder locations
var tempFolder = MediaLocationHelper.GetTempMediaLocation();          // "Temp"
var invoiceFolder = MediaLocationHelper.GetInvoiceLocation();          // "InvoiceUpdate"
var jobPhotos = MediaLocationHelper.GetJobMediaLocation();             // "JobMedia"
var customerInvoices = MediaLocationHelper.GetACustomerInvoiceLocation(); // "CustomerInvoice"

// Get physical path
Console.WriteLine(tempFolder.Path); 
// Output: "C:\MarbleLife\Media\Temp" (depends on MediaRootPath config)

// Generate temp Excel path
var excelPath = MediaLocationHelper.TempPathForExcel();
// Output: "C:\MarbleLife\Media\Sales\{GUID}.xlsx"
```

---

### PDF Generation

```csharp
private readonly IPdfFileService _pdfService;

// 1. Prepare your model
var invoiceData = new
{
    InvoiceNumber = "INV-2024-001",
    CustomerName = "John Doe",
    TotalAmount = 1500.00m,
    Items = new[] 
    {
        new { Description = "Marble Polishing", Amount = 1000m },
        new { Description = "Sealing", Amount = 500m }
    }
};

// 2. Point to HTML template (supports token replacement like {{CustomerName}})
var templatePath = @"C:\Templates\Invoice.html";

// 3. Generate PDF
var destinationFolder = MediaLocationHelper.GetInvoiceLocation().Path;
var pdfPath = _pdfService.GeneratePdfFromTemplateAndModel(
    model: invoiceData,
    destinationFolder: destinationFolder,
    generatedFileName: "INV-2024-001.pdf",
    templateFilePath: templatePath
);

if (pdfPath != null)
{
    Console.WriteLine($"PDF generated: {pdfPath}");
}
else
{
    Console.WriteLine("PDF generation failed");
}
```

**Requirements**:
- `wkhtmltopdf.exe` must be in `bin` folder
- HTML template uses mustache-style tokens: `{{PropertyName}}`

**Configuration** (optional):
```csharp
_pdfService.Switches = new WkHtmltoPdfSwitches
{
    MarginTop = 10,
    MarginBottom = 10,
    PageSize = "Letter",  // or "A4"
    Orientation = "Portrait"
};
```

---

### Excel Parsing

#### Parsing HomeAdvisor Leads

```csharp
var filePath = @"C:\Uploads\homeadvisor_leads_2024.xlsx";

try
{
    DataTable leadData = HomeAdvisorFileParser.ReadExcel(filePath);
    
    foreach (DataRow row in leadData.Rows)
    {
        var companyName = row["company name"]?.ToString();
        var srId = row["sr id"]?.ToString();
        var city = row["city"]?.ToString();
        var leadCost = decimal.Parse(row["net lead $"]?.ToString() ?? "0");
        
        // Process lead...
    }
}
catch (Exception ex)
{
    // File doesn't contain required headers
    Console.WriteLine($"Invalid file: {ex.Message}");
}
```

**Required Headers**:
- `ha account`, `company name`, `sr id`, `sr submitted date`, `task`, `net lead $`, `city`, `state`, `zip code`, `lead type`

#### Parsing Customer Data

```csharp
var filePath = @"C:\Uploads\customer_import.xlsx";
DataTable customerData = CustomerExcelFileParser.ReadExcel(filePath);

foreach (DataRow row in customerData.Rows)
{
    var customerId = row["customerid"]?.ToString();
    // Additional columns vary by franchisee template
}
```

**Required Header**: `customerid` (other columns dynamic)

#### Parsing Generic Transaction Files

```csharp
var filePath = @"C:\Uploads\transactions.xlsx";
DataTable transactions = ExcelFileParser.ReadExcel(filePath);

// This parser expects: "type", "num", "date"
foreach (DataRow row in transactions.Rows)
{
    var transactionType = row["type"]?.ToString();
    var transactionNum = row["num"]?.ToString();
    var transactionDate = row["date"]?.ToString();
}
```

#### Parsing Multi-Sheet Files (Zip Code Data)

```csharp
var filePath = @"C:\Data\zipcode_data.xlsx";

// Get all sheet IDs
var sheetIds = ZipExcelFileParser.GetSheetIds(filePath);

foreach (var sheetId in sheetIds)
{
    DataTable sheetData = ZipExcelFileParser.ReadExcelZip(filePath, sheetId);
    
    // Process each sheet (expects headers: "id", "isupdated")
    foreach (DataRow row in sheetData.Rows)
    {
        var id = row["id"]?.ToString();
        var isUpdated = row["isupdated"]?.ToString();
    }
}
```

---

### Excel Creation

#### Simple Export

```csharp
private readonly IExcelFileCreator _excelCreator;

var customers = new List<CustomerViewModel>
{
    new CustomerViewModel 
    { 
        Name = "John Doe", 
        Email = "john@example.com", 
        Phone = "555-1234" 
    },
    // ... more customers
};

var outputPath = MediaLocationHelper.TempPathForExcel();
bool success = _excelCreator.CreateExcelDocument(customers, outputPath);

if (success)
{
    // Download or email the file
}
```

**Automatic Behavior**:
- Uses reflection to read public properties
- Column names match property names
- Respects `[DownloadField]` attribute to control visibility
- Respects `[DisplayName]` attribute for custom headers

#### Custom Column Control

```csharp
public class CustomerExportViewModel
{
    [DownloadField]
    [DisplayName("Customer ID")]
    public long Id { get; set; }
    
    [DownloadField]
    [DisplayName("Full Name")]
    public string Name { get; set; }
    
    [DownloadField]
    [DisplayName("Contact Email")]
    public string Email { get; set; }
    
    // This property will NOT be exported (no DownloadField attribute)
    public string InternalNotes { get; set; }
}
```

---

### Session Management

#### Building User Sessions

```csharp
private readonly ISessionFactory _sessionFactory;

// From UserLogin object (after authentication)
var userLogin = new UserLogin { Id = 12345, Username = "john.doe" };
var sessionContext = new SessionContext();

var enrichedSession = _sessionFactory.BuildSession(sessionContext, userLogin);

// Access session data
var session = enrichedSession.UserSession;
Console.WriteLine($"User: {session.Name.FirstName} {session.Name.LastName}");
Console.WriteLine($"Role: {session.RoleName}");
Console.WriteLine($"Franchisee: {session.OrganizationName}");
Console.WriteLine($"Tasks Due Today: {session.TodayToDoCount}");
Console.WriteLine($"Currency: {session.CurrencyCode}"); // e.g., "USD", "CAD"
```

#### Building Customer Sessions (Signature Workflows)

```csharp
// Customer signing an estimate via mobile link
var customerSession = _sessionFactory.GetCustomerSessionModel(
    customerId: 5678,
    estimateCustomerId: null,
    estimateInvoiceId: 9101,
    typeId: 3,  // Signature type
    code: "ABC123XYZ"  // Validation code from link
);

if (customerSession.IsSigned)
{
    Console.WriteLine($"Already signed: {customerSession.Signature}");
}
else
{
    // Show signature capture UI
}
```

#### Retrieving Active Sessions

```csharp
var sessionId = "550e8400-e29b-41d4-a716-446655440000"; // From cookie/token

var activeSession = _sessionFactory.GetActiveSessionModel(sessionId);

if (activeSession == null)
{
    // Session expired or invalid
    RedirectToLogin();
}
else
{
    // Continue with user context
}
```

---

### Configuration Access

```csharp
private readonly ISettings _settings;

// Core Settings
var pageSize = _settings.PageSize;                    // e.g., 10
var mediaPath = _settings.MediaRootPath;              // e.g., "C:\MarbleLife\Media"
var defaultTimezone = _settings.DefaultTimeZone;      // e.g., "Eastern Standard Time"

// SMTP Configuration
var smtpHost = _settings.SmtpHost;
var smtpPort = _settings.SmtpPort;
var smtpUsername = _settings.SmtpUserName;

// Feature Flags
if (_settings.ApplyLateFee)
{
    // Calculate and apply late fees
}

if (_settings.SendFeedbackEnabled)
{
    // Send customer feedback requests
}

// External API Keys
var reviewApiKey = _settings.ReviewApiKey;
var awsAccessKey = _settings.AccessKey;
var awsSecretKey = _settings.SecretKey;

// Cron Expressions (for Quartz.NET jobs)
var invoiceCron = _settings.InvoiceGenerationServiceCronExpression; 
// e.g., "0 0 2 * * ?" (2 AM daily)
```

**Web.config/App.config**:
```xml
<appSettings>
  <add key="PageSize" value="10" />
  <add key="MediaRootPath" value="C:\MarbleLife\Media" />
  <add key="DefaultTimeZone" value="Eastern Standard Time" />
  <add key="ApplyLateFee" value="true" />
  <add key="InvoiceGenerationServiceCronExpression" value="0 0 2 * * ?" />
  <add key="ReviewApiKey" value="your-api-key-here" />
</appSettings>
```

---

### Sorting Helpers

```csharp
private readonly ISortingHelper _sortingHelper;

// Generic sorting for any queryable
IQueryable<Customer> query = _context.Customers.Where(c => c.IsActive);

// Sort by name (Ascending = 1, Descending = 2)
var sorted = _sortingHelper.ApplySorting(
    sortQuery: query,
    sortExpression: c => c.Name,
    sortOrder: 1  // Asc
);

// Sort by multiple properties
var sortedByDate = _sortingHelper.ApplySorting(
    sortQuery: sorted,
    sortExpression: c => c.CreatedDate,
    sortOrder: 2  // Desc
);

var results = sortedByDate.ToList();
```

---

### Date Range Helpers

```csharp
using Core.Application.Impl;

// Get all Sundays between two dates
var sundays = DateRangeHelperService.DayOfWeekCollection(
    startDate: new DateTime(2024, 1, 1),
    endDate: new DateTime(2024, 1, 31)
);

// Get first Monday of current week
var mondayDate = DateRangeHelperService.GetFirstDayOfWeek(DateTime.Today);

// Get all month start dates in range
var monthStarts = DateRangeHelperService.MonthsStartDate(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31)
);

// Count specific day occurrences
var fridaysInJanuary = DateRangeHelperService.GetNumberOfDaysInBetween(
    DayOfWeek.Friday,
    start: new DateTime(2024, 1, 1),
    end: new DateTime(2024, 1, 31)
);

// Get fiscal quarters
var quarters = DateRangeHelperService.GetQuarterBetweenYears(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31)
);
foreach (var quarter in quarters)
{
    Console.WriteLine($"Q: {quarter.Item1:d} to {quarter.Item2:d}");
}
```

---

## 📚 API Summary

### Services (Injected via DI)

| Service | Interface | Description |
|---------|-----------|-------------|
| `Clock` | `IClock` | UTC/Local time conversion, timezone management |
| `CryptographyOneWayHashService` | `ICryptographyOneWayHashService` | Password hashing (PBKDF2) |
| `FileFactory` | `IFileFactory` | File entity ↔ ViewModel mapping |
| `FileService` | `IFileService` | File metadata persistence, physical moves |
| `PdfFileService` | `IPdfFileService` | HTML → PDF conversion via wkhtmltopdf |
| `SessionFactory` | `ISessionFactory` | User/Customer session construction |
| `Settings` | `ISettings` | Configuration access (100+ properties) |
| `SortingHelper` | `ISortingHelper` | Generic LINQ sorting |
| `ExcelFileCreator` | `IExcelFileCreator` | List<T> → Excel export |
| `ExcelFileFormaterCreator` | `IExcelFileFormaterCreator` | Styled Excel reports |
| `ExcelTaxDocumentFileFormaterCreator` | `IExcelTaxDocumentFileFormaterCreator` | Franchisee document matrix |

### Static Utilities (No DI Required)

| Utility | Description | Key Method |
|---------|-------------|------------|
| `ExcelFileParser` | Generic transaction imports | `ReadExcel(path)` → DataTable |
| `HomeAdvisorFileParser` | HomeAdvisor lead imports | `ReadExcel(path)` → DataTable |
| `CustomerExcelFileParser` | Customer data imports | `ReadExcel(path)` → DataTable |
| `PriceEstimateExcelFileParser` | Price estimate imports | `ReadExcelZip(path, sheetId)` |
| `ZipExcelFileParser` | Multi-sheet zip code data | `GetSheetIds(path)`, `ReadExcelZip()` |
| `UpdateInvoiceFileParser` | Invoice update imports | `GetSheetIds(path)`, `ReadExcelZip()` |
| `DateRangeHelperService` | Date/time calculations | `GetFirstDayOfWeek()`, `MonthsStartDate()` |
| `MediaLocationHelper` | Standard folder paths | `GetTempMediaLocation()`, etc. |

---

## 🔧 Configuration Guide

### Essential Web.config Settings

```xml
<configuration>
  <appSettings>
    <!-- Core Settings -->
    <add key="PageSize" value="25" />
    <add key="MediaRootPath" value="C:\MarbleLife\Media" />
    <add key="MediaRootUrl" value="/media" />
    <add key="SiteRootUrl" value="https://marblelife.example.com" />
    <add key="DefaultTimeZone" value="Eastern Standard Time" />
    
    <!-- SMTP -->
    <add key="SmtpHost" value="smtp.gmail.com" />
    <add key="SmtpPort" value="587" />
    <add key="SmtpUserName" value="notifications@marblelife.com" />
    <add key="SmtpPassword" value="encrypted-password-here" />
    <add key="EnableSsl" value="true" />
    
    <!-- Feature Flags -->
    <add key="ApplyLateFee" value="true" />
    <add key="ApplyDateValidation" value="true" />
    <add key="SendFeedbackEnabled" value="true" />
    <add key="CreateEmailRecord" value="true" />
    <add key="GetCurrencyExchangeRate" value="true" />
    
    <!-- Payment Processing -->
    <add key="AuthNetTestMode" value="false" />
    <add key="DefaultRoyaltyAmount" value="0.08" />
    <add key="NationChargePercentage" value="0.02" />
    
    <!-- Cron Expressions (Quartz.NET) -->
    <add key="EmailNotificationServiceCronExpression" value="0 0/5 * * * ?" />
    <add key="InvoiceGenerationServiceCronExpression" value="0 0 2 * * ?" />
    <add key="SalesDataParserServiceCronExpression" value="0 0 3 * * ?" />
    <add key="PaymentReminderCronExpression" value="0 0 9 * * MON" />
    
    <!-- External APIs -->
    <add key="ReviewApiKey" value="your-review-system-key" />
    <add key="AccessKey" value="aws-s3-access-key" />
    <add key="SecretKey" value="aws-s3-secret-key" />
    <add key="CurrencyExchangeRateApi" value="https://api.exchangerate.com" />
    <add key="WebLeadsAPIkey" value="web-leads-api-key" />
  </appSettings>
</configuration>
```

### Dependency Injection Setup

```csharp
// In DependencyInjection/UnityConfig.cs
container.RegisterType<IClock, Clock>();
container.RegisterType<ICryptographyOneWayHashService, CryptographyOneWayHashService>();
container.RegisterType<IFileFactory, FileFactory>();
container.RegisterType<IFileService, FileService>();
container.RegisterType<IPdfFileService, PdfFileService>();
container.RegisterType<ISessionFactory, SessionFactory>();
container.RegisterType<ISettings, Settings>();
container.RegisterType<ISortingHelper, SortingHelper>();
container.RegisterType<IExcelFileCreator, ExcelFileCreator>();
// ... etc
```

**Auto-registration via `[DefaultImplementation]` attribute**:
```csharp
// DependencyInjection scans for this attribute
var typesWithAttribute = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.GetCustomAttribute<DefaultImplementationAttribute>() != null);
```

---

## 🛠 Troubleshooting

### PDF Generation Fails

**Symptom**: `GeneratePdfFromTemplateAndModel()` returns null

**Possible Causes**:
1. **Missing wkhtmltopdf.exe**
   ```
   Solution: Download from https://wkhtmltopdf.org/ and place in bin folder
   ```

2. **Permission Issues**
   ```
   Check IIS Application Pool identity has write access to destination folder
   ```

3. **Invalid HTML Template**
   ```
   Validate HTML renders correctly in browser first
   ```

4. **Timeout**
   ```
   Complex PDFs may exceed 30-second timeout. Consider splitting or optimizing template.
   ```

### Excel Parsing Throws Exception

**Symptom**: "File doesnot contain the sheet with required headers"

**Cause**: Excel file missing expected headers

**Solution**: Check parser-specific header requirements:
```csharp
// HomeAdvisorFileParser expects these headers (case-insensitive):
"ha account", "company name", "sr id", "sr submitted date", "task", 
"net lead $", "city", "state", "zip code", "lead type"
```

### Timezone Conversion Issues

**Symptom**: Times are off by 1 hour

**Cause**: Daylight Saving Time not handled correctly

**Solution**: Avoid `ToLocalWithDayLightSaving()` - use standard `ToLocal()` instead:
```csharp
// ❌ DON'T USE (has hardcoded 2019 DST logic)
var local = _clock.ToLocalWithDayLightSaving(utcDate);

// ✅ USE THIS
_clock.SetTimeZoneInfo("Eastern Standard Time");
var local = _clock.ToLocal(utcDate);
```

### Password Validation Always Fails

**Symptom**: `Validate()` returns false for correct password

**Possible Causes**:
1. **Salt not stored**: Ensure both `HashedText` and `Salt` persisted
2. **Encoding mismatch**: Salt/Hash must be Base64 strings
3. **Hash regeneration**: Never call `CreateHash()` on same password - generates different salt

**Debugging**:
```csharp
var hash = _hashService.CreateHash("password123");
Console.WriteLine($"Hash: {hash.HashedText}");
Console.WriteLine($"Salt: {hash.Salt}");

// Later validation
var isValid = _hashService.Validate("password123", hash); // Should be true
```

### Session Construction Throws NullReferenceException

**Symptom**: `SessionFactory.BuildSession()` crashes

**Cause**: User missing required data (e.g., no default organization role)

**Solution**: Validate user setup:
```sql
-- Ensure user has active, default role assignment
SELECT * FROM OrganizationRoleUser 
WHERE UserId = 12345 AND IsActive = 1 AND IsDefault = 1
```

### Configuration Property Returns Null

**Symptom**: `_settings.PropertyName` is null or empty

**Cause**: Missing or misspelled key in Web.config

**Solution**:
```xml
<!-- Ensure exact match (case-sensitive) -->
<add key="SmtpHost" value="smtp.example.com" />
```

**Debugging**:
```csharp
var allKeys = ConfigurationManager.AppSettings.AllKeys;
foreach (var key in allKeys)
{
    Console.WriteLine($"{key} = {ConfigurationManager.AppSettings[key]}");
}
```

---

## 📖 Additional Resources

- **[Core.Application Interfaces](../)** - Interface definitions
- **[Core.Application.Domain](../Domain/)** - Entity models
- **[Infrastructure](../../../../Infrastructure/)** - EF implementations
- **[DependencyInjection](../../../../DependencyInjection/)** - IoC configuration

---

## 🔒 Security Notes

### Password Storage
- ✅ Uses PBKDF2 (industry standard)
- ⚠️ Only 1000 iterations (NIST recommends 10,000+)
- ⚠️ Uses SHA-1 (SHA-256 preferred for new systems)

**Migration Path**: Increase iterations gradually:
```csharp
// Version hash format: $v2$iterations$hash$salt
// Allows gradual migration without breaking existing passwords
```

### Configuration Secrets
- ⚠️ API keys and passwords stored in plaintext AppSettings
- **Recommendation**: Use encrypted configuration sections:
  ```xml
  <configProtectedData>
    <providers>
      <add name="RsaProtectedConfigurationProvider" ... />
    </providers>
  </configProtectedData>
  ```

### File Operations
- ⚠️ No path traversal validation in PDF/File services
- **Recommendation**: Validate all file paths before operations
  ```csharp
  if (!path.StartsWith(_settings.MediaRootPath))
      throw new SecurityException("Invalid file path");
  ```

---

## 📊 Performance Tips

### Excel Operations
- **Large Files**: Consider streaming parsers (EPPlus.Stream) for 100k+ rows
- **Batch Processing**: Process Excel imports in background jobs (Quartz.NET)

### PDF Generation
- **Async**: Wrap in Task.Run() to avoid blocking web requests
- **Caching**: Cache frequently-generated PDFs (e.g., invoices)
- **Queue**: Use message queue (RabbitMQ, Azure Queue) for batch PDF generation

### Session Construction
- **Caching**: Cache UserSessionModel in distributed cache (Redis)
- **EF Optimization**: Use `.Include()` for eager loading
  ```csharp
  var user = _context.OrganizationRoleUsers
      .Include(x => x.Person)
      .Include(x => x.Organization.Franchisee)
      .FirstOrDefault(x => x.UserId == userId);
  ```

---

## 🎯 Best Practices

1. **Always use IClock, not DateTime.Now**
   ```csharp
   // ❌ DON'T
   var now = DateTime.Now;
   
   // ✅ DO
   var now = _clock.Now;
   ```

2. **Never hash passwords in UI layer**
   ```csharp
   // ❌ DON'T (in controller)
   var hash = _hashService.CreateHash(model.Password);
   
   // ✅ DO (in service layer with proper validation)
   _userService.RegisterUser(model); // Service handles hashing
   ```

3. **Use MediaLocationHelper for all file paths**
   ```csharp
   // ❌ DON'T
   var path = "C:\\Uploads\\file.pdf";
   
   // ✅ DO
   var path = MediaLocationHelper.GetTempMediaLocation().Path + "\\file.pdf";
   ```

4. **Validate Excel headers before processing**
   ```csharp
   try 
   {
       var data = HomeAdvisorFileParser.ReadExcel(path);
   } 
   catch (Exception ex) 
   {
       _logger.Error($"Invalid file format: {ex.Message}");
       return BadRequest("File must contain required headers");
   }
   ```

5. **Don't hardcode timezone names**
   ```csharp
   // ❌ DON'T
   _clock.SetTimeZoneInfo("Pacific Standard Time");
   
   // ✅ DO
   _clock.SetTimeZoneInfo(franchisee.TimeZone);
   ```

---

**Generated**: 2026-02-04  
**Maintained by**: DevMind AI  
**For AI Context**: See [AI-CONTEXT.md](AI-CONTEXT.md)
