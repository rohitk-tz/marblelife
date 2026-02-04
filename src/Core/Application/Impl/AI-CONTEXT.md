<!-- AUTO-GENERATED: Header -->
# Core.Application.Impl Module Context
**Version**: ee0f7c852dcf4d5a6ef7369c99414ea8e48d8b9e  
**Generated**: 2026-02-04T06:12:51Z  
**Module Path**: `src/Core/Application/Impl`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
**Concrete implementations of the Application module's infrastructure contracts**. This folder provides production-ready implementations for cross-cutting concerns including:
- **Time Management**: Timezone-aware date/time conversions with multi-timezone support
- **Security**: PBKDF2-based password hashing with salting
- **File Operations**: Excel/PDF generation, parsing, and file persistence
- **Configuration**: Application settings wrapper over .NET ConfigurationManager
- **Session Management**: User/Customer session construction and authentication state
- **Data Import/Export**: Specialized Excel parsers for business data (HomeAdvisor, Customers, Invoice Updates)

### Design Patterns

#### 1. **Service Layer Pattern**
All implementations marked with `[DefaultImplementation]` attribute for Unity DI registration. Each service encapsulates specific cross-cutting functionality.

#### 2. **Static Utility Pattern**
Excel parsers (`ExcelFileParser`, `HomeAdvisorFileParser`, `CustomerExcelFileParser`, etc.) implemented as static classes with shared cell-reading logic.

#### 3. **Strategy Pattern (Implicit)**
- Multiple file parsers (`HomeAdvisorFileParser`, `CustomerExcelFileParser`, `PriceEstimateExcelFileParser`, `UpdateInvoiceFileParser`, `ZipExcelFileParser`) share common parsing structure but differ in:
  - Required header validation logic
  - Sheet detection criteria
  - Cell mapping strategies

#### 4. **Builder Pattern**
`SessionFactory.BuildSession()` constructs complex session objects with aggregated data from multiple repositories.

#### 5. **Value Object Pattern**
- `MediaLocation`: Encapsulates file path resolution
- `WkHtmltoPdfSwitches`: Immutable configuration for PDF generation

#### 6. **Facade Pattern**
`Settings` class facades ConfigurationManager, providing typed, domain-specific property access.

### Data Flow

#### Time Management Flow
```
Client Request → IClock.UtcNow
                ↓
Clock.ToLocal(utc, timeZoneInfo) → Apply TimeZone conversion
                ↓
Return Local DateTime
```

#### Session Creation Flow
```
UserLogin/UserId → SessionFactory.BuildSession()
                ↓
Query Person → Query OrganizationRoleUser
                ↓
Query Organization → Build UserSessionModel
                ↓
Enrich with ToDo counts, Currency, Images
                ↓
Return ISessionContext
```

#### Excel Parsing Flow (Generic)
```
File Path → Open SpreadsheetDocument
                ↓
DetermineSheets() → Find sheet with required headers
                ↓
ReadExcel() → Extract cells into DataTable
                ↓
Map headers → Populate rows (skip empty)
                ↓
Return DataTable
```

#### PDF Generation Flow
```
Model + Template → NotificationServiceHelper.FormatContent()
                ↓
Generate HTML → PdfFileService.Generate()
                ↓
wkhtmltopdf.exe process → Write HTML to stdin
                ↓
Read PDF from stdout → Save to destination
                ↓
Return file path
```

### Key Implementation Strategies

#### 1. **Timezone Handling (Clock)**
- Defaults to "Eastern Standard Time" but configurable
- `ToLocal()`: UTC → Local with TimeZone awareness
- `ToLocalWithDayLightSaving()`: Handles historical DST transitions (hardcoded boundary dates)
- `SetTimeZoneInfo()`: Supports offset-based custom timezones
- **Gotcha**: Contains hardcoded DST boundary logic (2019-03-10) which may need updates

#### 2. **Password Security (CryptographyOneWayHashService)**
- **Algorithm**: PBKDF2-SHA1 with 1000 iterations
- **Salt**: 24-byte random salt per password
- **Hash**: 24-byte output
- **Validation**: Timing-attack-resistant comparison using `SlowEquals()`
- **Not Thread-Safe**: Uses `RNGCryptoServiceProvider` which is thread-safe, but individual operations are stateless

#### 3. **Excel Parsing (Shared Logic)**
All parsers follow this pattern:
```csharp
public static DataTable ReadExcel(string path)
{
    // 1. Open SpreadsheetDocument
    // 2. DetermineSheets() - Find correct sheet by headers
    // 3. Build header dictionary (Cell reference → Column index)
    // 4. Iterate rows, map cells to columns
    // 5. Skip rows with no valid data
    // 6. Return DataTable
}
```

**Header Detection Variations**:
- `ExcelFileParser`: Requires `type`, `num`, `date`
- `HomeAdvisorFileParser`: Requires `ha account`, `company name`, `sr id`, `sr submitted date`, `task`, etc.
- `CustomerExcelFileParser`: Requires `customerid`
- `PriceEstimateExcelFileParser`/`ZipExcelFileParser`: Requires `id`, `isupdated`
- `UpdateInvoiceFileParser`: Requires `id`, `isupdated` (filters sheets starting with "COUNTY" or "ZIP")

**Common Helper Methods**:
- `GetCellValue()`: Extracts string value, handles SharedString vs. direct value
- `CellReference()`: Converts cell address (e.g., "A1") to column letters only ("A")

#### 4. **File Persistence (FileService)**
- Uses `MediaLocation` to resolve physical paths
- `SaveFile()`: Persists FileModel to database via Repository
- `MoveFile()`: Physical file relocation
- **Extension Strategy**: Filename includes extension in destination path construction

#### 5. **Configuration Management (Settings)**
- **Pattern**: Property-per-setting accessing `ConfigurationManager.AppSettings`
- **Type Conversion**: Manual conversion with helpers (e.g., `ConvertInt32()`)
- **Cron Expressions**: All background job schedules exposed as properties
- **No Caching**: Every property access reads from ConfigurationManager (already cached internally)

#### 6. **Session Construction (SessionFactory)**
Complex aggregation logic:
1. **User Sessions**:
   - Queries `OrganizationRoleUser` for default active role
   - Joins `Organization`, `Role`, `Person` for user details
   - Enriches with:
     - ToDo count (today's open/in-progress tasks)
     - Profile images (franchisee + user)
     - Currency from franchisee organization
   - **Special Handling**:
     - FranchiseeAdmin: Aggregates ToDo counts across all managed franchises
     - FrontOfficeExecutive: Can switch context to different franchisee via `OrganizationRoleUserFranchisee`
     - SuperAdmin: Sees all ToDos system-wide

2. **Customer Sessions** (for signature workflows):
   - Constructs temporary session for customers signing estimates
   - No authentication, identified by `TypeId + Code`
   - Tracks signature status and scheduler associations

#### 7. **PDF Generation (PdfFileService)**
- **External Dependency**: `wkhtmltopdf.exe` binary
- **Process Management**:
  - Spawns process with HTML on stdin
  - Captures stdout (PDF) and stderr (logs)
  - 30-second timeout (`WaitForExit(30000)`)
- **Error Handling**: Lenient - returns file path even if exit code != 0 if file exists
- **Template Processing**: Uses `NotificationServiceHelper.FormatContent()` for token replacement

#### 8. **Excel Creation (ExcelFileCreator)**
- **Reflection-based**: Uses `PropertyInfo` to introspect `List<T>`
- **Attribute-driven**: Respects `DownloadFieldAttribute` and `DisplayNameAttribute`
- **Type-specific logic**: Hardcoded branches for specific ViewModels (e.g., `SalesFunnelLocalExcelViewModel`)
- **CSV Support**: CsvHelper integration for CSV exports

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Value Objects

#### SecureHash (Used by CryptographyOneWayHashService)
```csharp
namespace Core.Application.ValueType
{
    public class SecureHash
    {
        public string HashedText { get; set; }
        public string Salt { get; set; }
    }
}
```

#### WkHtmltoPdfSwitches
```csharp
public class WkHtmltoPdfSwitches
{
    public decimal MarginTop { get; set; }       // Default: 10mm
    public decimal MarginBottom { get; set; }    // Default: 10mm
    public decimal MarginLeft { get; set; }      // Default: 10mm
    public decimal MarginRight { get; set; }     // Default: 10mm
    public string PageSize { get; set; }         // Default: "A4"
    public long RedirectDelay { get; set; }      // Default: 200ms
    public string Orientation { get; set; }      // Default: "Portrait"
    public int PageWidth { get; set; }
    public int PageHeight { get; set; }
    public string FooterUrl { get; set; }
    
    // Converts to wkhtmltopdf command-line arguments
    public override string ToString() { ... }
}
```

#### MediaLocation
```csharp
public class MediaLocation
{
    private readonly string _relativeFolderPath;
    public string Path { get; private set; }     // Absolute path: MediaRootPath + relativePath
    
    public MediaLocation(string relativeFolderPath) { ... }
    public string GetRelativeFolderPath() { ... }
}
```

### Helper Classes

#### MediaLocationHelper (Static)
Factory methods for standard media folders:
```csharp
public static class MediaLocationHelper
{
    public static MediaLocation GetTempMediaLocation()           // "Temp"
    public static MediaLocation GetAttachmentMediaLocation()     // "Attachment"
    public static MediaLocation GetJobMediaLocation()            // "JobMedia"
    public static MediaLocation GetFranchiseeDocumentLocation()  // "Document"
    public static MediaLocation GetCalendarMediaLocation()       // "CalendarMedia"
    public static MediaLocation GetZipMediaLocation()            // "Zip"
    public static MediaLocation GetSalesMediaLocation()          // "Sales"
    public static MediaLocation GetTempImageLocation()           // "Images"
    public static MediaLocation GetDocumentImageLocation()       // "DocumentImages"
    public static MediaLocation GetInvoiceLocation()             // "InvoiceUpdate"
    public static string TempPathForExcel()                      // Guid-based temp path
}
```

#### DateRangeHelperService (Static)
Date manipulation utilities:
```csharp
public static class DateRangeHelperService
{
    public static IEnumerable<DateTime> DayOfWeekCollection(DateTime start, DateTime end);
    public static DateTime GetFirstDayOfWeek(DateTime date);  // Returns Monday
    public static IEnumerable<Tuple<DateTime>> MonthsStartDate(DateTime start, DateTime end);
    public static IEnumerable<DateTime> GetDaysCollection(DateTime start, DateTime end);
    public static IEnumerable<DateTime> GetHoursCollection(DateTime start, DateTime end);
    public static IEnumerable<int> GetYearsBetween(DateTime start, DateTime end);
    public static int GetNumberOfDaysInBetween(DayOfWeek day, DateTime start, DateTime end);
    public static IEnumerable<Tuple<DateTime,DateTime>> GetQuarterBetweenYears(DateTime start, DateTime end);
}
```

### Parser Return Types
All Excel parsers return `System.Data.DataTable` with dynamic columns based on Excel headers.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### IClock Implementation

#### Clock : IClock
**Location**: `Clock.cs`

```csharp
public class Clock : IClock
{
    // Properties
    public DateTime UtcNow { get; }                    // Returns DateTime.UtcNow
    public DateTime Now { get; }                       // Returns local time (EST default)
    public TimeZoneInfo CurrentTimeZone { get; }       // Current configured timezone
    public string BrowserTimeZone { get; set; }        // Client timezone hint (unused)
    
    // Conversion Methods
    public DateTime ToLocal(DateTime utcDate);
    public DateTime ToLocal(DateTime utcDate, TimeZoneInfo timeZoneInfo);
    public DateTime ToLocal(DateTime utcDate, double offsetMinutes);
    public DateTime ToLocal(DateTime utcDate, double offsetMinutes, int? ignored);
    
    public DateTime ToUtc(DateTime localDate);
    public DateTime ToUtc(DateTime localDate, TimeZoneInfo timeZoneInfo);
    
    public DateTime ToLocalWithDayLightSaving(DateTime utcDate);  // DEPRECATED: Hardcoded 2019 DST rules
    
    // Configuration
    public void SetTimeZoneInfo(double offsetMinutes);  // Creates custom timezone from offset
    public void SetTimeZoneInfo(int offsetMinutes);
    public void SetTimeZoneInfo(string timeZoneId);     // Maps abbreviations (EST, PST) to full IDs
    public TimeZoneInfo GetTimeZoneInfo();
    
    // Date Helpers
    public DateTime FirstDayOfWeek(DateTime dt);
    public DateTime LastDayOfWeek(DateTime dt);
    public DateTime FirstDayOfMonth(DateTime dt);
    public DateTime LastDayOfMonth(DateTime dt);
}
```

**Behavior Notes**:
- **Default Timezone**: Eastern Standard Time (hardcoded in constructor)
- **Abbreviation Mapping**: Extensive mapping logic in `GetTimeZoneId()` (EST, CST, MST, PST, AKST, HST, etc.)
- **Edge Case**: Returns `DateTime.MinValue` unchanged to avoid conversion errors
- **DST Handling**: `ToLocalWithDayLightSaving()` contains hardcoded 2019 boundary logic (should be avoided)

---

### ICryptographyOneWayHashService Implementation

#### CryptographyOneWayHashService : ICryptographyOneWayHashService
**Location**: `CryptographyOneWayHashService.cs`

```csharp
[DefaultImplementation]
public class CryptographyOneWayHashService : ICryptographyOneWayHashService
{
    public const int SALT_BYTES = 24;
    public const int HASH_BYTES = 24;
    public const int PBKDF2_ITERATIONS = 1000;
    
    // Creates salted hash
    public SecureHash CreateHash(string text);
    
    // Validates text against hash (timing-attack resistant)
    public bool Validate(string text, SecureHash goodHash);
}
```

**Security Characteristics**:
- **Algorithm**: PBKDF2 with SHA-1 (via `Rfc2898DeriveBytes`)
- **Iterations**: 1000 (considered low by modern standards; NIST recommends 10,000+)
- **Timing Attack Protection**: `SlowEquals()` ensures constant-time comparison
- **Salt**: Random 24-byte salt generated via `RNGCryptoServiceProvider`

---

### IFileFactory Implementation

#### FileFactory : IFileFactory
**Location**: `FileFactory.cs`

```csharp
[DefaultImplementation]
public class FileFactory : IFileFactory
{
    public File CreateDomain(FileModel model, File inPersistence);
    public FileModel CreateModel(File file);
}
```

**Pattern**: Simple DTO ↔ Entity mapper. Sets `IsNew` flag based on `Id <= 0`.

---

### IFileService Implementation

#### FileService : IFileService
**Location**: `FileService.cs`

```csharp
[DefaultImplementation]
public class FileService : IFileService
{
    public const long ContentTypeOther = 6;
    
    public File SaveFile(FileModel file, MediaLocation destination, string fileNamePrefix);
    public File SaveModel(FileModel fileModel);
    public FileModel Get(long fileId);
    public FileModel Get(File domain);
    public string MoveFile(string source, MediaLocation destination, string fileName, string ext);
}
```

**Dependencies**:
- `IUnitOfWork` → `Repository<File>`
- `IClock` → For `DataRecorderMetaData` timestamps

**Behavior**:
- `SaveFile()`: Persists metadata, actual file operations assumed elsewhere
- `MoveFile()`: Uses `System.IO.File.Move()` for physical relocation

---

### IPdfFileService Implementation

#### PdfFileService : IPdfFileService
**Location**: `PdfFileService.cs`

```csharp
[DefaultImplementation]
public class PdfFileService : IPdfFileService
{
    public WkHtmltoPdfSwitches Switches { get; set; }
    
    public string GeneratePdfFromTemplateAndModel(
        object model, 
        string destinationFolder, 
        string generatedFileName, 
        string templateFilePath);
}
```

**Dependencies**:
- `ISettings` → For template root path resolution
- External: `wkhtmltopdf.exe` binary (must be in bin folder)

**Process Flow**:
1. Read HTML template from `templateFilePath`
2. Format content using `NotificationServiceHelper.FormatContent(template, model)`
3. Spawn `wkhtmltopdf.exe` process with switches
4. Write formatted HTML to process stdin
5. Wait 30 seconds for completion
6. Return PDF file path or null on failure

**Error Handling**: Lenient - checks file existence even if exit code != 0

---

### ISessionFactory Implementation

#### SessionFactory : ISessionFactory
**Location**: `SessionFactory.cs`

```csharp
[DefaultImplementation]
public class SessionFactory : ISessionFactory
{
    // User Sessions
    public ISessionContext BuildSession(ISessionContext sessionContext, UserLogin userLogin);
    public ISessionContext BuildSession(ISessionContext sessionContext, long userId);
    public UserSessionModel GetUserSessionModel(long userId);
    
    // Customer Sessions (Signature workflows)
    public UserSessionModel GetCustomerSessionModel(
        long? customerId, 
        long? estimateCustomerId, 
        long? estimateInvoiceId, 
        long? typeId, 
        string code);
    
    // Active Session Lookup
    public UserSessionModel GetActiveSessionModel(string sessionId);
}
```

**Dependencies**: Requires 11+ repositories for data aggregation.

**User Session Construction**:
```csharp
UserSessionModel {
    UserId, Name, OrganizationId, RoleId, 
    TimeZoneId, CurrencyCode, 
    FileName,            // User profile image
    TeamFileName,        // Franchisee logo
    TodayToDoCount,      // Role-dependent aggregation
    LoggedInOrganizationId  // For FrontOfficeExecutive context switching
}
```

**Customer Session Construction**:
```csharp
UserSessionModel {
    CustomerId, CustomerName, 
    EstimateInvoiceId, EstimateCustomerId,
    IsSigned, Signature,
    SchedulerId, JobOrginialSchedulerId,
    TypeId
}
```

---

### IExcelFileCreator Implementation

#### ExcelFileCreator : IExcelFileCreator
**Location**: `ExcelFileCreator.cs` (33.8 KB - large file)

```csharp
[DefaultImplementation]
public class ExcelFileCreator : IExcelFileCreator
{
    public bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath);
    public DataTable ListToDataTable<T>(List<T> list, string tableName = "");
    // ... many other overloads for specialized formats ...
}
```

**Key Features**:
- Reflection-based column extraction
- Attribute-driven (`DownloadFieldAttribute`, `DisplayNameAttribute`)
- Specialized logic for business ViewModels (SalesFunnel, FranchiseeLoan, etc.)
- CSV export support via CsvHelper

---

### IExcelFileFormaterCreator Implementation

#### ExcelFileFormaterCreator : IExcelFileFormaterCreator
**Location**: `ExcelFileFormaterCreator.cs` (55.7 KB - large file)

Specialized Excel creator with custom formatting, styling, and multi-sheet support. Used for complex business reports.

---

### IExcelTaxDocumentFileFormaterCreator Implementation

#### ExcelTaxDocumentFileFormaterCreator : IExcelTaxDocumentFileFormaterCreator
**Location**: `ExcelTaxDocumentFileFormaterCreator.cs`

```csharp
[DefaultImplementation]
public class ExcelTaxDocumentFileFormaterCreator : IExcelTaxDocumentFileFormaterCreator
{
    public bool CreateExcelDocument(
        List<FranchiseeDocumentViewModel> list, 
        string xlsxFilePath, 
        List<string> columnList);
}
```

**Purpose**: Exports franchisee document expiry matrix (perpetuity vs. dated documents).

---

### ISettings Implementation

#### Settings : ISettings
**Location**: `Settings.cs` (400+ lines)

```csharp
[DefaultImplementation]
public class Settings : ISettings
{
    // Core Settings
    public int PageSize { get; }
    public string MediaRootPath { get; }
    public string MediaRootUrl { get; }
    public string SiteRootUrl { get; }
    public string DefaultTimeZone { get; }
    
    // SMTP Configuration
    public string SmtpHost { get; }
    public int SmtpPort { get; }
    public string SmtpUserName { get; }
    public string SmtpPassword { get; }
    public bool EnableSsl { get; }
    
    // Payment Gateway
    public bool AuthNetTestMode { get; }
    
    // Feature Flags
    public bool SendNotificationToFranchiser { get; }
    public bool ApplyLateFee { get; }
    public bool GetCurrencyExchangeRate { get; }
    public bool SendFeedbackEnabled { get; }
    public bool CreateEmailRecord { get; }
    // ... 50+ more feature flags ...
    
    // Cron Expressions (25+ background job schedules)
    public string EmailNotificationServiceCronExpression { get; }
    public string InvoiceGenerationServiceCronExpression { get; }
    // ... etc ...
    
    // External API Configuration
    public string ReviewApiKey { get; }
    public string AccessKey { get; }  // AWS S3
    public string WebLeadsAPIkey { get; }
    public string CurrencyExchangeRateApi { get; }
}
```

**Pattern**: One property per `ConfigurationManager.AppSettings` key, with type conversion.

---

### ISortingHelper Implementation

#### SortingHelper : ISortingHelper
**Location**: `SortingHelper.cs`

```csharp
[DefaultImplementation]
public class SortingHelper : ISortingHelper
{
    public IOrderedQueryable<M> ApplySorting<M, T>(
        IQueryable<M> sortQuery, 
        Expression<Func<M, T>> sortExpression, 
        long? sortOrder);
}
```

**Behavior**: Applies `OrderBy` vs. `OrderByDescending` based on `sortOrder` enum value (Asc=1).

---

### IImageHelper Implementation

#### ImageHelper : IImageHelper
**Location**: `ImageHelper.cs`

```csharp
public class ImageHelper : IImageHelper
{
    public string ImageConvertToByteAarry(string Url);  // STUB - returns null
}
```

**Status**: Incomplete implementation.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Dependencies

#### Depends On
- **[Core.Application.Domain](../Domain/AI-CONTEXT.md)** - Entity models: `File`, `DataRecorderMetaData`
- **[Core.Application.Interfaces](../)** - All `I*` contracts implemented here
- **[Core.Application.ValueType](../)** - `SecureHash`, `Name`, `Dimension`
- **[Core.Application.Attribute](../)** - `[DefaultImplementation]` for DI registration
- **[Core.Application.Extensions](../)** - String/Path extension methods
- **[Core.Users.Domain](../../Users/Domain/)** - `Person`, `UserLogin`, `OrganizationRoleUser`
- **[Core.Organizations.Domain](../../Organizations/Domain/)** - `Organization`, `Franchisee`, `Role`
- **[Core.Sales.Domain](../../Sales/Domain/)** - `Customer`, `CustomerLog`
- **[Core.Billing.Domain](../../Billing/Domain/)** - `CurrencyExchangeRate`, `EstimateInvoice`
- **[Core.Scheduler.Domain](../../Scheduler/Domain/)** - `EstimateInvoiceCustomer`, `CustomerSignature`
- **[Core.ToDo.Domain](../../ToDo/Domain/)** - `ToDoFollowUpList`
- **[Core.Notification.Impl](../../Notification/Impl/)** - `NotificationServiceHelper` for template formatting

#### Used By
- **[Infrastructure](../../../../Infrastructure/)** - Dependency injection registration
- **[API](../../../../API/)** - Session management, file operations
- **[Jobs](../../../../Jobs/)** - Background processing, Excel parsing
- **All Service Layers** - Time, configuration, security services

### External Dependencies

#### NuGet Packages
- **DocumentFormat.OpenXml** (EPPlus alternative) - Excel file manipulation
- **CsvHelper** - CSV export functionality
- **System.Security.Cryptography** - PBKDF2 hashing

#### External Binaries
- **wkhtmltopdf.exe** - HTML to PDF conversion (must be in bin folder)

#### .NET Framework
- **System.Configuration** - `ConfigurationManager` for Settings
- **System.Data** - `DataTable` for Excel parsing results
- **System.Reflection** - Dynamic property introspection for Excel export
- **System.Linq.Expressions** - Generic sorting expressions

### Configuration Dependencies

#### Required AppSettings (Settings.cs)
```xml
<appSettings>
  <!-- Core -->
  <add key="PageSize" value="10" />
  <add key="MediaRootPath" value="C:\MarbleLife\Media" />
  <add key="DefaultTimeZone" value="Eastern Standard Time" />
  
  <!-- SMTP -->
  <add key="SmtpHost" value="smtp.example.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUserName" value="..." />
  <add key="SmtpPassword" value="..." />
  
  <!-- Feature Flags -->
  <add key="ApplyLateFee" value="true" />
  <add key="SendFeedbackEnabled" value="true" />
  
  <!-- Cron Expressions -->
  <add key="EmailNotificationServiceCronExpression" value="0 0/5 * * * ?" />
  <add key="InvoiceGenerationServiceCronExpression" value="0 0 2 * * ?" />
  
  <!-- External APIs -->
  <add key="ReviewApiKey" value="..." />
  <add key="AccessKey" value="..." />
  <add key="SecretKey" value="..." />
</appSettings>
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Deep Insights -->
## 💡 Deep Insights

### Critical Implementation Details

#### 1. Clock Timezone Gotchas
**Problem**: Hardcoded DST boundary dates in `ToLocalWithDayLightSaving()`
```csharp
var boundaryDate = new DateTime(2019, 3, 10, 2, 0, 0);
if (DateTime.Now < boundaryDate) { ... }
```
**Impact**: This method will always execute the `return ToLocal(utcDate)` branch after March 2019, making the DST adjustment logic dead code.

**Recommendation**: Either remove this method or implement proper DST detection using `TimeZoneInfo.IsDaylightSavingTime()`.

---

#### 2. Password Hashing Strength
**Current**: PBKDF2-SHA1 with 1000 iterations
**Industry Standard** (NIST SP 800-63B): 10,000+ iterations for PBKDF2

**Security Assessment**:
- ✅ Salt is cryptographically random
- ✅ Timing-attack resistant comparison
- ⚠️ Iteration count is 10x below modern recommendations
- ⚠️ SHA-1 as underlying hash (SHA-256 preferred)

**Migration Path**: Cannot change without invalidating existing passwords unless implementing versioned hash storage.

---

#### 3. Excel Parser Pattern Duplication
**Observation**: 5+ parser classes share 90% identical code:
- `ExcelFileParser`
- `HomeAdvisorFileParser`
- `CustomerExcelFileParser`
- `PriceEstimateExcelFileParser`
- `ZipExcelFileParser`
- `UpdateInvoiceFileParser`

**Difference**: Only header validation logic varies.

**Refactoring Opportunity**:
```csharp
public static class GenericExcelParser
{
    public static DataTable ReadExcel(string path, Func<List<string>, bool> headerValidator)
    {
        // Shared logic
    }
}

// Usage
var dt = GenericExcelParser.ReadExcel(path, headers => 
    headers.Contains("customerid"));
```

---

#### 4. PDF Generation Fragility
**Risk Areas**:
1. **External Process Dependency**: If `wkhtmltopdf.exe` missing, fails silently
2. **Timeout**: 30-second hardcoded timeout may be insufficient for complex documents
3. **Error Handling**: Returns file path even if process fails (checks file existence as fallback)

**Improvement**:
```csharp
if (!File.Exists(wkhtmltopdfPath))
    throw new FileNotFoundException("wkhtmltopdf.exe not found", wkhtmltopdfPath);

_process.WaitForExit(configuredTimeout); // Make timeout configurable
```

---

#### 5. SessionFactory Complexity
**Metrics**:
- 11+ repository dependencies
- 170+ lines in `BuildUserSessionModel()`
- Nested conditional logic for role-based ToDo aggregation

**Complexity Indicators**:
- FranchiseeAdmin: Aggregates todos across all managed franchises
- FrontOfficeExecutive: Requires context-switching via `OrganizationRoleUserFranchisee`
- SuperAdmin: System-wide aggregation

**Testing Challenge**: Requires extensive mock setup. Consider extracting role-specific logic to Strategy pattern.

---

#### 6. Settings Class Maintenance
**Problem**: 100+ properties with repetitive pattern:
```csharp
public string PropertyName 
{ 
    get { return ConfigurationManager.AppSettings["PropertyName"]; } 
}
```

**Risk**: No compile-time validation of configuration keys. Typos discovered at runtime.

**Alternative**: Strongly-typed configuration via code generation or `IOptions<T>` pattern (if migrating to .NET Core).

---

#### 7. MediaLocationHelper Responsibilities
**Pattern**: Static factory methods for 15+ folder types.

**Behavior**: Creates directory if missing:
```csharp
if (!Directory.Exists(location.Path)) 
    Directory.CreateDirectory(location.Path);
```

**Side Effect**: I/O operation in getter methods. May cause permission issues if running under restricted account.

---

#### 8. ExcelFileCreator Type-Specific Branching
**Anti-Pattern**: Hardcoded `typeof(T)` checks:
```csharp
if (typeof(T) == typeof(SalesFunnelLocalExcelViewModel))
{
    var displayAttr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true);
    // Special handling...
}
```

**Problem**: Adding new types requires modifying this class (violates Open/Closed Principle).

**Refactoring**: Use attribute-based configuration exclusively:
```csharp
[ExcelColumn(Order = 1, Header = "Custom Name")]
public string PropertyName { get; set; }
```

---

#### 9. File Parser Error Messages
**Observation**: Typo in exception messages:
```csharp
throw new Exception("File doesnot contain the sheet with required headers");
```

**Impact**: User-facing error (should be "does not").

---

#### 10. WkHtmltoPdfSwitches Configuration
**Design**: Immutable-like value object with default constructor setting defaults.

**Command Generation**:
```csharp
public override string ToString() =>
    "--print-media-type --orientation Portrait --margin-top 10mm ...";
```

**Gotcha**: `FooterUrl` property exists but not used in `ToString()` (dead code).

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Performance -->
## ⚡ Performance Considerations

### Excel Parsing
- **Memory**: `DataTable` loads entire sheet into memory. Large files (>10k rows) may cause GC pressure.
- **Optimization**: Use `SpreadsheetDocument.Open(path, false)` (read-only mode) to prevent locking.

### PDF Generation
- **Blocking**: `_process.WaitForExit(30000)` blocks calling thread.
- **Scalability**: Each PDF generation spawns separate process (resource-intensive).
- **Recommendation**: Queue PDF generation jobs in background service.

### Session Construction
- **N+1 Query Risk**: `SessionFactory.BuildUserSessionModel()` performs multiple separate queries.
- **Optimization**: Use EF Include() to eager-load related entities in single query.

### Settings Access
- **Cached**: `ConfigurationManager.AppSettings` is cached internally by .NET.
- **Thread-Safe**: Safe for concurrent access.

<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Security -->
## 🔒 Security Considerations

### Password Storage
- ✅ Uses salted PBKDF2 (not plaintext)
- ⚠️ Iteration count (1000) below modern standards (10,000+)
- ⚠️ SHA-1 as underlying hash (consider SHA-256 migration)

### File Operations
- ⚠️ `PdfFileService` reads template files without path validation (potential path traversal)
- ⚠️ `FileService.MoveFile()` no validation of destination path

### Configuration
- ⚠️ `Settings` exposes sensitive keys (API keys, passwords) as plain properties
- **Recommendation**: Use encrypted configuration sections or Azure Key Vault

### Session Management
- ✅ Customer sessions use code-based validation
- ⚠️ No explicit session timeout management in factory

<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Migration Notes -->
## 🔄 Migration & Evolution

### Breaking Changes to Avoid
1. **CryptographyOneWayHashService**: Changing iteration count or algorithm invalidates existing passwords
2. **Clock.SetTimeZoneInfo()**: Changing timezone abbreviation mappings breaks existing data
3. **MediaLocationHelper**: Changing folder names breaks existing file references

### .NET Core Migration Path
1. Replace `ConfigurationManager` with `IConfiguration`
2. Replace `System.Configuration` with `Microsoft.Extensions.Configuration`
3. Replace `wkhtmltopdf.exe` process with library-based solution (e.g., IronPDF, Puppeteer)
4. Extract static parsers to injectable services

### Refactoring Priorities
1. **High**: Extract shared Excel parsing logic to base class/helper
2. **High**: Make PDF timeout configurable
3. **Medium**: Replace type-specific branching in ExcelFileCreator with attribute-driven approach
4. **Medium**: Validate file paths in FileService operations
5. **Low**: Fix typos in exception messages

<!-- END CUSTOM SECTION -->
