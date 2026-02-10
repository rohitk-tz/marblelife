<!-- AUTO-GENERATED: Header -->
# Impl
> Service implementations for time management, file operations, Excel parsing/generation, PDF creation, cryptography, and settings management
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Impl** module contains 25+ service implementations that provide reusable business logic across the application. These services handle cross-cutting concerns like date/time operations, file storage, Excel import/export, PDF generation, image manipulation, and configuration management.

**Key Service Categories:**
1. **Infrastructure Services**: Clock (time), Settings (config), SessionFactory (DI)
2. **File Services**: FileService, FileFactory, ImageHelper, MediaLocation
3. **Excel Services**: 6 parsers (import) + 5 creators (export)
4. **Security**: CryptographyOneWayHashService (password hashing)
5. **PDF**: PdfFileService (HTML → PDF conversion)
6. **Utilities**: SortingHelper (dynamic LINQ), DateRangeHelperService

**Why this architecture?**
- **Separation of Concerns**: Business logic extracted from controllers
- **Testability**: All services use interfaces and dependency injection
- **Reusability**: Services used by multiple controllers and domains
- **Strategy Pattern**: Multiple parser/creator implementations for different file formats
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Use Clock Service for Testable Time
```csharp
using Core.Application.Impl;

public class InvoiceService
{
    private readonly IClock _clock;
    
    public InvoiceService(IClock clock)
    {
        _clock = clock;
    }
    
    public Invoice CreateInvoice(InvoiceModel model)
    {
        return new Invoice
        {
            CreatedDate = _clock.UtcNow,  // Mockable in tests
            DueDate = _clock.Now.AddDays(30)  // Local timezone aware
        };
    }
}
```

### Example 2: Access Settings
```csharp
using Core.Application.Impl;

public class FileController : Controller
{
    private readonly ISettings _settings;
    
    public FileController(ISettings settings)
    {
        _settings = settings;
    }
    
    public IActionResult Upload(IFormFile file)
    {
        string rootPath = _settings.MediaRootPath;  // From web.config
        int pageSize = _settings.PageSize;  // Default pagination
        
        // Use settings throughout controller
    }
}
```

### Example 3: Parse Excel File
```csharp
using Core.Application.Impl;

var parser = new ExcelFileParser();  // or inject via FileFactory

using (var fileStream = file.OpenReadStream())
{
    try
    {
        List<ParsedFileParentModel> parsedData = parser.Parse(fileStream, file.FileName);
        
        // Process parsed data
        foreach (var item in parsedData)
        {
            var invoice = CreateInvoice(item);
            invoiceRepository.Add(invoice);
        }
    }
    catch (InvalidFileUploadException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

### Example 4: Generate Excel Export
```csharp
using Core.Application.Impl;

var creator = new ExcelFileCreator();

// Data collection (uses DownloadFieldAttribute for columns)
var invoices = invoiceRepository.GetAll().Select(i => new InvoiceExportModel
{
    [DownloadField(displayName: "Invoice #")]
    InvoiceNumber = i.Number,
    
    [DownloadField(displayName: "Total", currencyType: "USD")]
    Total = i.Total,
    
    [DownloadField(displayName: "Date")]
    Date = i.CreatedDate
}).ToList();

byte[] excelBytes = creator.Create(invoices);

return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "invoices.xlsx");
```

### Example 5: Hash Password
```csharp
using Core.Application.Impl;

public class UserService
{
    private readonly ICryptographyService _crypto;
    
    public UserService(ICryptographyService crypto)
    {
        _crypto = crypto;
    }
    
    public User Register(string username, string password)
    {
        SecureHash hash = _crypto.ComputeHash(password);
        
        return new User
        {
            Username = username,
            PasswordHash = hash.HashedText,
            PasswordSalt = hash.Salt
        };
    }
    
    public bool Login(string username, string password)
    {
        var user = userRepository.GetByUsername(username);
        var storedHash = new SecureHash(user.PasswordHash, user.PasswordSalt);
        
        return _crypto.VerifyHash(password, storedHash);
    }
}
```

### Example 6: Convert HTML to PDF
```csharp
using Core.Application.Impl;

public class ReportService
{
    private readonly IPdfFileService _pdfService;
    
    public ReportService(IPdfFileService pdfService)
    {
        _pdfService = pdfService;
    }
    
    public byte[] GenerateInvoicePdf(Invoice invoice)
    {
        string html = RenderInvoiceHtml(invoice);
        
        var options = new WkHtmltoPdfSwitches
        {
            PageSize = "Letter",
            Orientation = "Portrait",
            MarginTop = 10,
            MarginBottom = 10
        };
        
        return _pdfService.GeneratePdf(html, options);
    }
}
```

### Example 7: Use Date Range Helpers
```csharp
using Core.Application.Impl;

public class ReportController : Controller
{
    private readonly IClock _clock;
    
    public IActionResult MonthlyReport(int year, int month)
    {
        DateTime firstDay = _clock.FirstDayOfMonth(new DateTime(year, month, 1));
        DateTime lastDay = _clock.LastDayOfMonth(new DateTime(year, month, 1));
        
        var invoices = invoiceRepository.GetByDateRange(firstDay, lastDay);
        return View(invoices);
    }
}
```

### Example 8: Upload and Save File
```csharp
using Core.Application.Impl;
using Core.Application.Extensions;

public class FileController : Controller
{
    private readonly IFileService _fileService;
    
    public IActionResult Upload(IFormFile uploadedFile)
    {
        // Save to temp location
        string tempPath = Path.Combine(Path.GetTempPath(), uploadedFile.FileName);
        using (var stream = new FileStream(tempPath, FileMode.Create))
        {
            uploadedFile.CopyTo(stream);
        }
        
        // Convert to relative path for database
        string relativePath = tempPath.ToRelativePath();
        
        var fileModel = new FileModel
        {
            Name = uploadedFile.FileName,
            Size = uploadedFile.Length,
            RelativeLocation = relativePath,
            MimeType = uploadedFile.ContentType
        };
        
        File savedFile = _fileService.SaveModel(fileModel);
        
        return Ok(new { fileId = savedFile.Id });
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Infrastructure Services
| Service | Purpose | Key Methods |
|---------|---------|-------------|
| `Clock` | Testable date/time with timezone support | `UtcNow`, `Now`, `ToLocal()`, `ToUtc()`, `FirstDay...()` |
| `Settings` | Configuration access | `PageSize`, `MediaRootPath`, `Dimension...` |
| `SessionFactory` | DI registration | `RegisterServices()` |

### File Services
| Service | Purpose | Key Methods |
|---------|---------|-------------|
| `FileService` | File entity persistence | `SaveFile()`, `SaveModel()` |
| `FileFactory` | Create parsers/creators | `CreateParser()`, `CreateCreator()` |
| `ImageHelper` | Image manipulation | `ResizeImage()`, `CropImage()`, `GetDimensions()` |
| `MediaLocation` | Storage location enum | Enum values for file categories |

### Excel Services
| Parser | Purpose |
|--------|---------|
| `ExcelFileParser` | Standard invoice/sales data import |
| `ZipExcelFileParser` | Batch import from ZIP files |
| `HomeAdvisorFileParser` | HomeAdvisor lead import |
| `CustomerExcelFileParser` | Customer data import |
| `UpdateInvoiceFileParser` | Invoice update import |
| `PriceEstimateExcelFileParser` | Price estimate worksheet import |

| Creator | Purpose |
|---------|---------|
| `ExcelFileCreator` | Generic data export using attributes |
| `ExcelFileFormaterCreator` | Styled report export |
| `ExcelFranchiseeFileFormaterCreator` | Franchisee reports |
| `ExcelTaxDocumentFileFormaterCreator` | Tax documents |
| `ExcelFileCreatorMarketingLead` | Marketing lead exports |

### Other Services
| Service | Purpose | Key Methods |
|---------|---------|-------------|
| `CryptographyOneWayHashService` | Password hashing | `ComputeHash()`, `VerifyHash()` |
| `PdfFileService` | HTML to PDF conversion | `GeneratePdf()` |
| `DateRangeHelperService` | Date range calculations | `GetQuarter()`, `GetMonth()` |
| `SortingHelper` | Dynamic LINQ sorting | `ApplySort()` |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Clock.UtcNow returns wrong timezone
**Cause**: Default timezone is EST; may need configuration for other regions.
**Solution**: Call `SetTimeZoneInfo(timeZoneId)` after DI resolution or use `ToLocal(utcDate, offset)`.

### Issue: Excel parser throws InvalidFileUploadException
**Cause**: File format doesn't match expected structure (worksheet count, column order).
**Solution**: 
- Verify Excel file has correct worksheets (check parser expectations)
- Check column headers match expected names
- Add logging to see exact error

### Issue: PDF generation fails with "wkhtmltopdf not found"
**Cause**: wkhtmltopdf executable not installed or not in PATH.
**Solution**:
- Install wkhtmltopdf: https://wkhtmltopdf.org/downloads.html
- Add to system PATH or configure explicit path in Settings
- Verify executable permissions on Linux

### Issue: Settings returns null
**Cause**: Missing app settings in web.config/appsettings.json.
**Solution**: Add required keys:
```xml
<appSettings>
  <add key="PageSize" value="25" />
  <add key="MediaRootPath" value="C:\inetpub\media" />
  <add key="MediaRootUrl" value="/media" />
</appSettings>
```

### Issue: FileService path resolution fails
**Cause**: MediaRootPath not configured or file not in media directory.
**Solution**: 
- Verify `Settings.MediaRootPath` is correct
- Use `PathExtensions.ToFullPath()` only for files in media directory
- Check file actually exists at resolved path

### Issue: Password verification always fails
**Cause**: Salt not stored or hash algorithm changed.
**Solution**: 
- Verify both PasswordHash and PasswordSalt are stored in database
- Don't change hash algorithm after passwords are stored
- Re-hash all passwords if algorithm must change

### Issue: Excel export has no columns
**Cause**: Properties missing `DownloadFieldAttribute`.
**Solution**: Add attribute to each exported property:
```csharp
public class InvoiceExportModel
{
    [DownloadField(displayName: "Invoice Number")]
    public string Number { get; set; }
    
    [DownloadField(displayName: "Total", currencyType: "USD")]
    public decimal Total { get; set; }
}
```
<!-- END CUSTOM SECTION -->
