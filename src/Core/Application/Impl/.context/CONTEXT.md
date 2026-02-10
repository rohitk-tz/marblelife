<!-- AUTO-GENERATED: Header -->
# Impl — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Provides service implementations for cross-cutting concerns (time management, settings, file operations, cryptography, Excel parsing/generation, PDF creation, and data transformation). These classes implement business logic that is reused across multiple domains and act as the "plumbing" layer between controllers and domain entities.

### Design Patterns
- **Service Layer**: Encapsulates business logic separate from controllers and repositories
- **Dependency Injection**: All services use constructor injection for testability
- **Strategy Pattern**: Multiple file parser implementations (ExcelFileParser, ZipExcelFileParser, HomeAdvisorFileParser, etc.)
- **Factory Pattern**: FileFactory creates appropriate file types, SessionFactory manages DI registration
- **Singleton Services**: Settings, Clock accessed via ApplicationManager for consistent state

### Data Flow
1. **Time Management**: Clock → UTC operations → timezone conversion → local datetime
2. **File Operations**: FileService → path resolution → file I/O → database persistence
3. **Excel Parsing**: FileParser → read Excel → validate → map to domain models → return DTOs
4. **Excel Generation**: FileCreator → load data → format → write Excel → return byte array
5. **PDF Generation**: PdfFileService → HTML → wkhtmltopdf → PDF byte array
6. **Settings**: Configuration → Settings service → cached application properties
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Clock.cs & IClock Interface
```csharp
// Provides testable date/time operations with timezone support
public interface IClock
{
    DateTime UtcNow { get; }  // Current UTC time
    DateTime Now { get; }  // Current time in configured timezone (default: EST)
    DateTime ToLocal(DateTime utcDate);  // Convert UTC to local timezone
    DateTime ToUtc(DateTime localDate);  // Convert local to UTC
    TimeZoneInfo CurrentTimeZone { get; }
    // Date range helpers
    DateTime FirstDayOfWeek(DateTime dt);
    DateTime LastDayOfWeek(DateTime dt);
    DateTime FirstDayOfMonth(DateTime dt);
    DateTime LastDayOfMonth(DateTime dt);
}
```
**Key Features**:
- Default timezone: Eastern Standard Time (EST)
- Supports US timezones (EST, CST, MST, PST, AKST, HAST, AST)
- Daylight saving time handling
- Custom timezone via offset (minutes)

### Settings.cs & ISettings Interface
```csharp
// Application configuration wrapper for web.config/app settings
public interface ISettings
{
    int PageSize { get; }  // Default pagination size
    string MediaRootPath { get; }  // Root path for uploaded files
    string MediaRootUrl { get; }  // Base URL for file access
    string DefaultRoyaltyAmount { get; }  // Franchise royalty default
    Dimension DimensionNormal { get; }  // Standard thumbnail (configurable)
    Dimension DimensionSmall { get; }  // Small thumbnail
    Dimension DimensionLarge { get; }  // Large thumbnail
}
```

### FileService.cs & IFileService Interface
```csharp
// Manages file entity persistence and file system operations
public interface IFileService
{
    File SaveFile(FileModel file, MediaLocation fileDestination, string fileNamePrefix);
    File SaveModel(FileModel fileModel);
    // Converts FileModel DTO → File entity → persists to database
}
```

### Excel File Parsers (Multiple Implementations)
```csharp
// Base interface for Excel file parsing strategies
public interface IFileParser
{
    List<ParsedFileParentModel> Parse(Stream fileStream, string filename);
}

// Implementations:
// - ExcelFileParser: Standard invoice/sales data
// - ZipExcelFileParser: Batch processing of zipped Excel files
// - HomeAdvisorFileParser: HomeAdvisor lead import format
// - CustomerExcelFileParser: Customer data import
// - UpdateInvoiceFileParser: Invoice update operations
// - PriceEstimateExcelFileParser: Price estimation worksheets
```

### Excel File Creators
```csharp
// Generates Excel files from data collections
// - ExcelFileCreator: Generic data export using DownloadFieldAttribute reflection
// - ExcelFileFormaterCreator: Formatted reports with styling
// - ExcelFranchiseeFileFormaterCreator: Franchisee-specific reports
// - ExcelTaxDocumentFileFormaterCreator: Tax document generation
// - ExcelFileCreatorMarketingLead: Marketing lead exports
```

### CryptographyOneWayHashService.cs
```csharp
// Provides password hashing with salt
public interface ICryptographyService
{
    SecureHash ComputeHash(string text);  // Generate salted hash
    bool VerifyHash(string text, SecureHash secureHash);  // Verify password
}
```

### FileFactory.cs
```csharp
// Factory for creating file-related objects
public class FileFactory
{
    public static IFileParser CreateParser(FileType type);
    public static IFileCreator CreateCreator(FileType type);
}
```

### ImageHelper.cs
```csharp
// Image manipulation utilities
public static class ImageHelper
{
    public static byte[] ResizeImage(byte[] imageBytes, Dimension targetSize);
    public static byte[] CropImage(byte[] imageBytes, Rectangle cropArea);
    public static Dimension GetDimensions(byte[] imageBytes);
}
```

### PdfFileService.cs
```csharp
// Converts HTML to PDF using wkhtmltopdf
public interface IPdfFileService
{
    byte[] GeneratePdf(string htmlContent, WkHtmltoPdfSwitches options);
}
```

### WkHtmltoPdfSwitches.cs
```csharp
// Configuration for PDF generation
public class WkHtmltoPdfSwitches
{
    public string PageSize { get; set; }  // Letter, A4, etc.
    public string Orientation { get; set; }  // Portrait, Landscape
    public int? MarginTop { get; set; }  // mm
    public int? MarginBottom { get; set; }
    public int? MarginLeft { get; set; }
    public int? MarginRight { get; set; }
    // ... other wkhtmltopdf command-line options
}
```

### DateRangeHelperService.cs
```csharp
// Date range calculations and formatting
public interface IDateRangeHelper
{
    DateRange GetQuarterRange(int year, int quarter);
    DateRange GetYearRange(int year);
    DateRange GetMonthRange(int year, int month);
    string FormatDateRange(DateTime start, DateTime end);
}
```

### SortingHelper.cs & ISortingHelper
```csharp
// Dynamic LINQ sorting for grids/tables
public interface ISortingHelper
{
    IQueryable<T> ApplySort<T>(IQueryable<T> query, string sortExpression);
    // sortExpression format: "PropertyName ASC" or "PropertyName DESC"
}
```

### MediaLocation.cs & MediaLocationHelper.cs
```csharp
// Enum and helper for file storage locations
public enum MediaLocation
{
    Invoices,
    Customers,
    ProfilePhotos,
    BeforeAfterPhotos,
    Documents,
    Temp
}

public static class MediaLocationHelper
{
    public static string GetPath(MediaLocation location);
    public static string GetUrl(MediaLocation location);
}
```

### SessionFactory.cs
```csharp
// Dependency injection container registration
public class SessionFactory
{
    public static void RegisterServices(IServiceCollection services);
    // Scans assemblies for DefaultImplementationAttribute
    // Registers services, repositories, validators
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Clock Service
- **Purpose**: Testable date/time operations with timezone awareness
- **Key Methods**:
  - `UtcNow`: Returns `DateTime.UtcNow` (mockable for testing)
  - `Now`: Current time in configured timezone (default EST)
  - `ToLocal(utcDate)`: Converts UTC to local timezone
  - `ToUtc(localDate)`: Converts local to UTC
  - `FirstDayOfWeek/Month/LastDay...`: Date range helpers for reporting
- **Configuration**: Defaults to EST, supports timezone override via `SetTimeZoneInfo()`

### Settings Service
- **Purpose**: Centralized access to app configuration
- **Key Properties**:
  - `PageSize`: Default pagination (e.g., 25 records)
  - `MediaRootPath`: Filesystem root for uploads (e.g., "C:\inetpub\media")
  - `MediaRootUrl`: Base URL for file downloads (e.g., "/media")
  - `DefaultRoyaltyAmount`: Franchise royalty percentage
  - `DimensionNormal/Small/Large`: Thumbnail sizes from config
- **Usage**: Injected into services needing configuration

### FileService
- **Purpose**: Persist file metadata and coordinate file I/O
- **Key Methods**:
  - `SaveFile`: Handles file upload, path resolution, database save
  - `SaveModel`: Converts FileModel → File entity → saves
- **Side Effects**: Creates File entity, saves to database via repository

### Excel Parsers
- **Purpose**: Extract data from Excel files into domain models
- **Common Pattern**:
  1. Accept Stream and filename
  2. Read Excel using ClosedXML or EPPlus
  3. Validate structure (worksheet names, column count)
  4. Map rows to DTOs (ParsedFileParentModel, HomeAdvisorParentModel)
  5. Return list of parsed objects
- **Error Handling**: Throws `InvalidFileUploadException` on format errors

### Excel Creators
- **Purpose**: Generate Excel files from data collections
- **Key Feature**: Uses `DownloadFieldAttribute` reflection to determine columns
- **Output**: Byte array (Excel file) ready for HTTP download

### CryptographyService
- **Purpose**: Secure password hashing
- **Methods**:
  - `ComputeHash(plainText)`: Returns SecureHash (HashedText + Salt)
  - `VerifyHash(plainText, secureHash)`: Compares hashed values
- **Algorithm**: SHA256 with random salt (configurable)

### PdfFileService
- **Purpose**: Convert HTML to PDF using wkhtmltopdf command-line tool
- **Input**: HTML string + WkHtmltoPdfSwitches configuration
- **Output**: Byte array (PDF file)
- **Dependency**: Requires wkhtmltopdf.exe installed on server

### ImageHelper
- **Purpose**: Image manipulation (resize, crop, get dimensions)
- **Methods**: All static, operate on byte arrays
- **Library**: Uses System.Drawing or ImageSharp
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Application.Domain** — File, DataRecorderMetaData entities
- **Core.Application.ViewModel** — FileModel, ParsedFileParentModel DTOs
- **Core.Application.Extensions** — Path/currency extensions
- **Core.Application.Attribute** — DefaultImplementationAttribute for DI
- **Core.Application.ValueType** — SecureHash, Dimension
- **Core.Application.Exceptions** — InvalidFileUploadException

### External Dependencies
- **ClosedXML** or **EPPlus** — Excel file parsing/generation
- **System.Drawing** or **ImageSharp** — Image manipulation
- **wkhtmltopdf** — PDF generation (external executable)
- **System.Configuration** — App settings access

### Referenced By
- **Controllers** — Inject services for business logic
- **Other Services** — Compose services (e.g., InvoiceService uses FileService)
- **ApplicationManager** — Provides static access to Clock, Settings
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Clock Usage Patterns
Always use `Clock.UtcNow` instead of `DateTime.UtcNow` for testability:
```csharp
// ❌ BAD - Not mockable
var date = DateTime.UtcNow;

// ✅ GOOD - Inject IClock and mock in tests
var date = _clock.UtcNow;
```

### Excel Parser Strategy
Each parser handles specific file formats. Add new parsers by:
1. Implement IFileParser interface
2. Add to FileFactory switch statement
3. Register in DI container

### File Upload Flow
1. Controller receives IFormFile
2. Save to temp location
3. Call FileService.SaveFile(fileModel, location, prefix)
4. FileService resolves paths using PathExtensions
5. Persists File entity to database
6. Returns File with generated ID

### PDF Generation
wkhtmltopdf must be installed and accessible in system PATH or configured path.

### Common Pitfalls
- **Timezone confusion**: Always store UTC in database, convert to local only for display
- **Settings caching**: Settings are read from config on each access (no caching by default)
- **Excel format changes**: Parsers are brittle to worksheet structure changes
- **Path handling**: Always use PathExtensions for cross-platform compatibility
<!-- END CUSTOM SECTION -->
