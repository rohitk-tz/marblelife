# Core/Application - AI Agent Context

## Module Purpose

The **Core.Application** namespace serves as the foundational infrastructure layer of the MarbleLife application. It provides essential cross-cutting concerns, base abstractions, and reusable services that are consumed by all domain modules. This layer implements the Application Service pattern and houses core interfaces that define the contract for infrastructure implementations.

## Architecture Overview

### Design Patterns
1. **Repository Pattern** - Generic data access abstraction (`IRepository<T>`)
2. **Unit of Work Pattern** - Transaction management (`IUnitOfWork`)
3. **Factory Pattern** - Object creation and transformation (`IFileFactory`, `ISessionFactory`)
4. **Strategy Pattern** - Multiple implementations for file generation (Excel, PDF)
5. **Dependency Injection** - Inversion of control via `IDependencyInjectionHelper`
6. **Value Object Pattern** - Immutable domain primitives (`SecureHash`, `Name`, `Dimension`)
7. **Service Locator** - Central application manager (`ApplicationManager`)

### Layer Responsibilities
- **Interfaces** (Root level): Define contracts for infrastructure services
- **Implementations** (`Impl/`): Concrete service implementations
- **Domain Models** (`Domain/`): Shared base entities and metadata structures
- **View Models** (`ViewModel/`): Data transfer objects for API/UI layer
- **Value Objects** (`ValueType/`): Immutable domain primitives
- **Exceptions** (`Exceptions/`): Custom exception hierarchy
- **Attributes** (`Attribute/`): Metadata annotations for validation and behavior
- **Enums** (`Enum/`): Shared enumeration types
- **Extensions** (`Extensions/`): C# extension methods

---

## Root-Level File Inventory (23 Files)

### 1. ApplicationManager.cs
**Type**: Static Service Locator  
**Purpose**: Central registry for application-wide dependencies  
**Key Members**:
- `IDependencyInjectionHelper DependencyInjection` - DI container access
- `ISettings Settings` - Application configuration access

**Usage**: Acts as a global access point for core services when constructor injection is unavailable (e.g., static contexts, attributes).

**Dependencies**: None (top-level static class)

---

### 2. ExcelFileFormaterForDirectoryCreator.cs
**Type**: Implementation Class  
**Purpose**: Creates formatted Excel documents for franchisee directory exports  
**Key Members**:
- `bool CreateExcelDocument(List<FranchiseeModel>, string xlsxFilePath)`

**Status**: Currently throws `NotImplementedException` - likely pending implementation or deprecated

**Dependencies**: `Core.Organizations.ViewModel.FranchiseeModel`

---

### 3. IAppContextStore.cs
**Type**: Interface  
**Purpose**: Application-wide context storage abstraction (similar to HTTP session or cache)  
**Key Methods**:
- `void AddItem(string key, object item)` - Store object with key
- `void UpdateItem(string key, object item)` - Update existing entry
- `object Get(string key)` - Retrieve stored object
- `void Remove(string key)` - Delete entry
- `void ClearStorage()` - Clear all stored data

**Use Cases**: Temporary storage for request-scoped or application-scoped data, caching user preferences, storing workflow state.

**Dependencies**: None

---

### 4. IClock.cs
**Type**: Interface  
**Purpose**: Time abstraction for testability and timezone handling  
**Key Members**:
- `DateTime UtcNow { get; }` - Current UTC time
- `DateTime Now { get; }` - Current local time
- `TimeZoneInfo CurrentTimeZone { get; }` - Active timezone
- `string BrowserTimeZone { get; set; }` - Client timezone identifier
- `DateTime ToLocal(DateTime utcDate, ...)` - Multiple overloads for UTC → Local conversion
- `DateTime ToUtc(DateTime localDate, ...)` - Local → UTC conversion
- `void SetTimeZoneInfo(string/int)` - Configure active timezone
- `DateTime ToLocalWithDayLightSaving(DateTime)` - DST-aware conversion

**Design Rationale**: Abstraction allows mocking time in tests and centralizes timezone logic for a global franchise system.

**Dependencies**: `System.TimeZoneInfo`

---

### 5. ICryptographyOneWayHashService.cs
**Type**: Interface  
**Purpose**: Password hashing and validation service  
**Key Methods**:
- `SecureHash CreateHash(string text)` - Hash password with salt
- `bool Validate(string text, SecureHash goodHash)` - Verify password against stored hash

**Security**: Implements one-way hashing (likely BCrypt or PBKDF2) with salting to prevent rainbow table attacks.

**Dependencies**: `Core.Application.ValueType.SecureHash`

---

### 6. IDependencyInjectionHelper.cs
**Type**: Interface  
**Purpose**: Abstraction over IoC container operations  
**Key Methods**:
- `T Resolve<T>()` - Resolve dependency by type
- `object Resolve(Type obj)` - Resolve by runtime type
- `T Resolve<T>(string name)` - Resolve named registration
- `void Register<TAbstract, TConcrete>()` - Register type mapping

**Use Cases**: Service resolution, dynamic object creation, plugin architecture support.

**Dependencies**: None (abstraction over Castle Windsor, Autofac, or similar)

---

### 7. IExcelFileCreator.cs
**Type**: Interface  
**Purpose**: Generic Excel file generation service  
**Key Methods**:
- `bool CreateExcelDocument<T>(List<T>, string xlsxFilePath)` - Generate Excel from generic list
- `DataTable ListToDataTable<T>(List<T>, string tableName)` - Convert list to DataTable
- `bool CreateExcelDocument(DataSet, string excelFilename)` - Generate from DataSet
- `void ToCSV(DataTable, string filePath)` - Export DataTable to CSV
- `void ToCSVWriter<T>(List<T>, string filePath)` - Export generic list to CSV

**Use Cases**: Report generation, data export, bulk data templates.

**Dependencies**: `System.Data.DataTable`, `System.Data.DataSet`

---

### 8. IExcelFileCreatorMarketingLead.cs
**Type**: Interface  
**Purpose**: Specialized Excel export for marketing lead call details  
**Key Methods**:
- `bool CreateExcelDocument(List<CallDetailViewModel>, string xlsxFilePath, List<string> columnsName)` - Export with custom columns
- `DataTable ListToDataTable(List<string> columnList, List<CallDetailViewModel>, string tableName)` - Convert with column filtering
- `bool CreateExcelDocument(DataSet, string excelFilename)` - Standard DataSet export

**Design Note**: Separate interface suggests marketing lead exports require special formatting or column selection logic.

**Dependencies**: `Core.MarketingLead.ViewModel.CallDetailViewModel`

---

### 9. IExcelFileFormaterCreator.cs
**Type**: Interface  
**Purpose**: Formatted Excel export for annual sales reports  
**Key Methods**:
- `bool CreateExcelDocument(List<AnnualGroupedReport>, string xlsxFilePath)` - Generate formatted annual report

**Dependencies**: `Core.Sales.ViewModel.AnnualGroupedReport`

---

### 10. IExcelFileFormaterForDirectoryCreator.cs
**Type**: Interface  
**Purpose**: Franchisee directory Excel export  
**Key Methods**:
- `bool CreateExcelDocument(List<FranchiseeModel>, string xlsxFilePath)` - Generate franchisee directory

**Dependencies**: `Core.Organizations.ViewModel.FranchiseeModel`

---

### 11. IExcelFranchiseeFileFormaterCreator.cs
**Type**: Interface  
**Purpose**: Alternative franchisee Excel export (possibly different format)  
**Key Methods**:
- `bool CreateExcelDocument(List<FranchiseeModel>, string xlsxFilePath)`

**Note**: Similar to #10 - may represent different report formats or deprecated version.

**Dependencies**: `Core.Organizations.ViewModel.FranchiseeModel`

---

### 12. IExcelTaxDocumentFileFormaterCreator.cs
**Type**: Interface  
**Purpose**: Tax document Excel export with custom column selection  
**Key Methods**:
- `bool CreateExcelDocument(List<FranchiseeDocumentViewModel>, string xlsxFilePath, List<string> columnList)` - Generate with selected columns

**Dependencies**: `Core.Organizations.ViewModel.FranchiseeDocumentViewModel`

---

### 13. IFileFactory.cs
**Type**: Interface (Factory Pattern)  
**Purpose**: Transforms between domain entities and view models for file operations  
**Key Methods**:
- `File CreateDomain(FileModel model, File inPersistence)` - Convert view model to domain entity, merging with existing state
- `FileModel CreateModel(File file)` - Convert domain entity to view model

**Pattern**: Data Mapper / Factory - separates persistence concerns from API contracts.

**Dependencies**: `Core.Application.Domain.File`, `Core.Application.ViewModel.FileModel`

---

### 14. IFileService.cs
**Type**: Interface  
**Purpose**: High-level file management operations  
**Key Methods**:
- `File SaveModel(FileModel)` - Persist file metadata
- `FileModel Get(long fileId)` - Retrieve by ID
- `FileModel Get(File domain)` - Convert domain to view model
- `File SaveFile(FileModel, MediaLocation, string fileNamePrefix)` - Save file to storage location
- `string MoveFile(string source, MediaLocation destination, string fileNamePrefix, string ext)` - Move file between locations

**Use Cases**: File upload handling, file organization, media management.

**Dependencies**: `Core.Application.Domain.File`, `Core.Application.ViewModel.FileModel`, `Core.Application.Impl.MediaLocation`

---

### 15. IImageHelper.cs
**Type**: Interface  
**Purpose**: Image processing utility  
**Key Methods**:
- `string ImageConvertToByteAarry(string Url)` - Download image from URL and convert to byte array (base64 string)

**Use Cases**: Image downloading, format conversion, embedding images in documents.

**Dependencies**: None

---

### 16. ILogService.cs
**Type**: Interface  
**Purpose**: Logging abstraction supporting multiple levels  
**Key Methods**:
- `void Trace(string message)` - Verbose diagnostic logging
- `void Debug(string message)` - Debug-level logging
- `void Info(string message)` - Informational logging
- `void Error(string message)` - Error logging (3 overloads)
- `void Error(string message, Exception exception)` - Error with exception
- `void Error(Exception exception)` - Exception-only logging

**Design**: Follows standard logging levels (Trace < Debug < Info < Error).

**Dependencies**: `System.Exception`

---

### 17. IPdfFileService.cs
**Type**: Interface  
**Purpose**: PDF generation from templates  
**Key Methods**:
- `string GeneratePdfFromTemplateAndModel(object model, string destinationFolder, string generatedFileName, string templateFilePath)` - Merge template with model data

**Use Cases**: Invoice generation, report generation, document generation from HTML/Razor templates.

**Returns**: Path to generated PDF file

**Dependencies**: None (template engine agnostic)

---

### 18. IPdfGenerator.cs
**Type**: Interface  
**Purpose**: Low-level PDF generation from HTML  
**Key Methods**:
- `void SetDefaultSwitch(WkHtmltoPdfSwitches)` - Configure PDF generator options
- `string Generate(string sourcePath, ...)` - Generate from HTML file
- `string Generate(StringBuilder htmlStream, ...)` - Generate from HTML string

**Implementation**: Wraps wkhtmltopdf command-line tool

**Dependencies**: `Core.Application.Impl.WkHtmltoPdfSwitches`, `System.Text.StringBuilder`

---

### 19. IRepository<T>.cs
**Type**: Generic Interface (Repository Pattern)  
**Purpose**: Generic data access layer abstraction  
**Constraint**: `where T : DomainBase`  
**Key Methods**:
- `T Get(long id)` - Retrieve by primary key
- `T Get(Expression<Func<T, bool>>)` - Retrieve by predicate
- `IEnumerable<T> Fetch(Expression<Func<T, bool>>)` - Query multiple records
- `IEnumerable<T> Fetch(..., int pageSize, int pageNumber)` - Paginated query
- `long Count(Expression<Func<T, bool>>)` - Count matching records
- `void Save(T entity)` - Insert or update
- `void Delete(T entity)` - Delete by entity
- `void Delete(Expression<Func<T, bool>>)` - Delete by predicate
- `void Delete(long id)` - Delete by ID
- `IQueryable<T> Table { get; }` - Full table access (with tracking)
- `IQueryable<T> TableNoTracking { get; }` - Read-only query
- `Task<List<T>> TableAsync(Expression<Func<T, bool>>)` - Async query
- `IQueryable<T> IncludeMultiple(params Expression<Func<T, object>>[])` - Eager loading

**Pattern**: Combines Repository + Specification patterns with async support.

**Dependencies**: `System.Linq`, `System.Linq.Expressions`, `System.Threading.Tasks`

---

### 20. ISessionContext.cs
**Type**: Interface  
**Purpose**: Current user session state container  
**Key Members**:
- `string Token { get; set; }` - Authentication token
- `UserSessionModel UserSession { get; set; }` - User details and permissions

**Use Cases**: Authorization checks, user identification, audit logging.

**Dependencies**: `Core.Users.ViewModels.UserSessionModel`

---

### 21. ISessionFactory.cs
**Type**: Interface (Factory Pattern)  
**Purpose**: Session creation and hydration  
**Key Methods**:
- `ISessionContext BuildSession(ISessionContext, UserLogin)` - Create session from login
- `ISessionContext BuildSession(ISessionContext, long userId)` - Create session from user ID
- `UserSessionModel GetUserSessionModel(long userId)` - Load user session data
- `UserSessionModel GetActiveSessionModel(string sessionId)` - Retrieve active session
- `UserSessionModel GetCustomerSessionModel(long? customerId, long? estimateCustomerId, long? estimateInvoiceId, long? typeId, string code)` - Create customer session

**Use Cases**: Authentication flow, session restoration, impersonation, customer portal access.

**Dependencies**: `Core.Users.Domain.UserLogin`, `Core.Users.ViewModels.UserSessionModel`

---

### 22. ISettings.cs
**Type**: Interface  
**Purpose**: Application configuration repository (267 properties)  
**Key Categories**:

#### Core Settings
- `int PageSize` - Default pagination size
- `string MediaRootPath` / `MediaRootUrl` - File storage configuration
- `Dimension DimensionNormal/Small/Large` - Image size presets
- `string SiteRootUrl` - Application base URL
- `string LogoImage` - Company logo path
- `string DefaultTimeZone` - Default timezone

#### Email Configuration
- `string SmtpHost` / `SmtpPort` / `SmtpUserName` / `SmtpPassword` - SMTP settings
- `bool EnableSsl` - TLS/SSL toggle
- `string FromEmail` / `ToSuperAdmin` - Default email addresses
- `string CompanyName` / `ApplicationName` - Email branding

#### AWS Configuration
- `string AWSSecreatKey` / `AWSAccessKey` - AWS credentials
- `string AWSBucketName` / `AWSBucketURL` / `AWSBucketThumbURL` - S3 storage

#### Cron Expressions (25+ scheduled jobs)
- `EmailNotificationServiceCronExpression`
- `SalesDataUploadReminderCronExpression`
- `InvoiceGenerationServiceCronExpression`
- `MarketingLeadDataParserServiceCronExpression`
- `PaymentReminderCronExpression`
- `SendCustomerFeedbackRequestCronExpression`
- `DocumentExpiryNotificationCronExpression`
- ... (20+ more scheduled tasks)

#### Business Rules
- `decimal NationChargePercentage` - National fee percentage
- `bool ApplyLateFee` - Late fee enforcement toggle
- `int DefaultRoyaltyLateFeeWaitPeriod` - Grace period in days
- `int PaymentReminderDayCount` - Reminder schedule
- `DateTime LateFeeStartDate` - Fee enforcement start date

#### Feature Flags
- `bool SendNotificationToFranchiser`
- `bool ApplyDateValidation`
- `bool GetMergeField`
- `bool SendWeeklyReminder`
- `bool SendFeedbackEnabled`
- `bool AutoGeneratedMailForBestFitEnabled`
- ... (30+ feature toggles)

#### Marketing Lead Integration
- `string AccessKey` / `SecretKey` - API credentials (v1)
- `string AccessKeyV2` / `SecretKeyV2` - API credentials (v2)
- `bool GetCallDetails` / `GetWebLeads` - Feature toggles
- `string WebLeadsAPIkey` - External API key

#### Review System
- `string ClientId` / `ReviewApiKey` - Review platform integration
- `string KioskLink` - Customer feedback kiosk URL

#### Notification Recipients (10+ recipient configurations)
- `string RecipientEmail` / `CCToAdmin` / `CCToMarketing`
- `string MailChimpReportRecipients`
- `string CCToReviewReportRecipients`
- `string UnpainInvoiceRecipients`
- `string AuditRecipients`

**Design Note**: Massive configuration interface suggests complex multi-tenant franchise system with extensive automation.

**Dependencies**: `Core.Application.ValueType.Dimension`

---

### 23. IUnitOfWork.cs
**Type**: Interface (Unit of Work Pattern)  
**Purpose**: Transaction boundary and repository factory  
**Key Methods**:
- `IRepository<T> Repository<T>()` - Get repository for entity type
- `void SaveChanges()` - Commit pending changes
- `void StartTransaction()` - Begin transaction
- `void Setup()` - Initialize context
- `void Cleanup()` - Release resources
- `void Rollback()` - Abort transaction
- `void ResetContext()` - Clear change tracking

**Pattern**: Implements Unit of Work + Repository Factory patterns.

**Dependencies**: `System.IDisposable`, `IRepository<T>`

---

## Subfolder Overview

### Attribute/ (6 files)
Custom C# attributes for metadata and behavior modification:
- **CascadeEntityAttribute** - Marks relationships for cascading operations
- **DefaultImplementationAttribute** - Specifies default implementation for interface
- **DownloadFieldAttribute** - Marks properties for export in downloads
- **NoValidationResolveAtStartAttribute** - Skips startup validation
- **NoValidatorRequiredAttribute** - Disables validation for view model

**Purpose**: Declarative configuration, reducing boilerplate code.

### Domain/ (7 files)
Shared domain entities and base classes:
- **DomainBase** - Base class for all entities (likely contains Id, timestamps)
- **File** - File metadata entity (name, size, location, MIME type)
- **Folder** - Folder/directory entity
- **ContentType** - Document content type classification
- **DataRecorderMetaData** - Audit metadata (created by, date, IP)
- **Lookup** - Key-value lookup data
- **LookupType** - Lookup category

**Purpose**: Shared persistence models used across multiple bounded contexts.

### Enum/ (4 files)
Application-wide enumerations:
- **FileTypes** - File category enumeration
- **LookupTypes** - Predefined lookup categories
- **MessageType** - Notification/message classification

**Purpose**: Type-safe constants, preventing magic strings.

### Exceptions/ (7 files)
Custom exception hierarchy:
- **CustomBaseException** - Base exception class
- **InvalidAddressException** - Address validation failure
- **InvalidDataProvidedException** - Input data validation error
- **InvalidDependencyRegistrationException** - DI configuration error
- **InvalidFileUploadException** - File upload validation failure
- **UserBlockedException** - Account blocked error

**Purpose**: Structured error handling with specific catch blocks.

### Extensions/ (3 files)
C# extension methods:
- **CurrencyExtension** - Currency formatting and conversion
- **PathExtensions** - File path manipulation

**Purpose**: Fluent APIs, reducing utility class proliferation.

### Impl/ (26 files)
Concrete implementations of application interfaces:
- **Clock** - `IClock` implementation
- **CryptographyOneWayHashService** - BCrypt/PBKDF2 hashing
- **ExcelFileCreator** - Generic Excel generation
- **ExcelFileCreatorMarketingLead** - Marketing lead Excel
- **ExcelFileFormaterCreator** - Formatted sales reports
- **ExcelFileParser** - Parse uploaded Excel files
- **ZipExcelFileParser** - Parse Excel from ZIP archives
- **CustomerExcelFileParser** - Customer data import
- **HomeAdvisorFileParser** - HomeAdvisor lead import
- **UpdateInvoiceFileParser** - Invoice update import
- **PriceEstimateExcelFileParser** - Price estimate import
- **ExcelFranchiseeFileFormaterCreator** - Franchisee reports
- **ExcelTaxDocumentFileFormaterCreator** - Tax document reports
- **FileFactory** - File entity/model transformation
- **FileService** - File operations orchestration
- **ImageHelper** - Image downloading/conversion
- **PdfFileService** - PDF generation from templates
- **SessionFactory** - Session creation and hydration
- **Settings** - Configuration implementation
- **SortingHelper** / **ISortingHelper** - Generic sorting utilities
- **DateRangeHelperService** - Date range calculations
- **MediaLocation** - File storage path abstraction
- **MediaLocationHelper** - Storage location utilities
- **WkHtmltoPdfSwitches** - PDF generator configuration

**Purpose**: Separation of interface from implementation, enabling testing and swapping implementations.

### ValueType/ (4 files)
Immutable value objects:
- **SecureHash** - Password hash + salt
- **Name** - Validated name value object
- **Dimension** - Image dimension (width × height)

**Purpose**: Domain-Driven Design value objects, encapsulating validation.

### ViewModel/ (13 files)
Data transfer objects (DTOs):
- **EditModelBase** - Base class for editable view models
- **FileModel** - File upload/download DTO
- **DropDownListViewModel** - Select list item
- **PagingModel** - Pagination metadata
- **PostResponseModel** - Generic POST response
- **ResponseModel** - Generic API response wrapper
- **DeleteInvoiceResponseModel** - Invoice deletion response
- **FeedbackMessageModel** - User feedback DTO
- **ModelValidationItem** / **ModelValidationOutput** - Validation error structure
- **HomeAdvisorParentModel** - HomeAdvisor import DTO
- **ParsedFileParentModel** - File parsing result

**Purpose**: API contracts, decoupling domain models from external interfaces.

---

## Data Flow

### File Upload Flow
1. Client POSTs `FileModel` to API endpoint
2. Controller calls `IFileService.SaveFile(FileModel, MediaLocation, fileNamePrefix)`
3. FileService uses `IFileFactory.CreateDomain(FileModel, null)` to create `File` entity
4. FileService saves physical file to `MediaLocation.Path`
5. `IRepository<File>.Save(file)` persists metadata
6. `IUnitOfWork.SaveChanges()` commits transaction
7. FileService returns `FileModel` with generated ID and URL

### Authentication Flow
1. User submits credentials via API
2. Service calls `ICryptographyOneWayHashService.Validate(password, storedHash)`
3. On success, `ISessionFactory.BuildSession(sessionContext, userLogin)` creates session
4. `UserSessionModel` populated with user details and permissions
5. `ISessionContext.Token` generated and returned to client
6. Subsequent requests include token in header
7. Middleware validates token and hydrates `ISessionContext.UserSession`

### Report Generation Flow
1. Service queries data via `IRepository<T>.Fetch(predicate)`
2. Data transformed to report view models
3. Service calls appropriate Excel interface (e.g., `IExcelFileFormaterCreator.CreateExcelDocument(data, filePath)`)
4. Implementation generates Excel using EPPlus/NPOI
5. File saved to `MediaLocation`
6. File metadata persisted via `IFileService.SaveModel(fileModel)`
7. Download link returned to client

---

## Critical Dependencies

### Internal Dependencies
- `Core.Users.Domain` - User entities and authentication
- `Core.Users.ViewModels` - User DTOs
- `Core.Organizations.ViewModel` - Franchisee/organization DTOs
- `Core.Sales.ViewModel` - Sales report DTOs
- `Core.MarketingLead.ViewModel` - Marketing lead DTOs

### External Dependencies
- **Entity Framework** - ORM for `IRepository<T>` implementation
- **EPPlus** or **NPOI** - Excel file generation
- **wkhtmltopdf** - HTML to PDF conversion (via `IPdfGenerator`)
- **BCrypt.NET** or **PBKDF2** - Password hashing
- **Castle Windsor** / **Autofac** - Dependency injection container
- **Log4Net** / **NLog** - Logging provider
- **AWS SDK** - S3 storage integration

---

## Usage Guidelines for AI Agents

### Adding New Infrastructure Service
1. Define interface in root `Application/` folder
2. Implement in `Impl/` subfolder
3. Register in DI container configuration
4. Use constructor injection in consuming classes

```csharp
// Step 1: Define interface
namespace Core.Application
{
    public interface IEmailService
    {
        void Send(string to, string subject, string body);
    }
}

// Step 2: Implement
namespace Core.Application.Impl
{
    public class EmailService : IEmailService
    {
        private readonly ISettings _settings;
        private readonly ILogService _logger;
        
        public EmailService(ISettings settings, ILogService logger)
        {
            _settings = settings;
            _logger = logger;
        }
        
        public void Send(string to, string subject, string body)
        {
            // Implementation
        }
    }
}

// Step 3: Register (in DI configuration)
container.Register<IEmailService, EmailService>();

// Step 4: Use
public class UserService
{
    private readonly IEmailService _emailService;
    
    public UserService(IEmailService emailService)
    {
        _emailService = emailService;
    }
}
```

### Creating Value Objects
```csharp
// In ValueType/
namespace Core.Application.ValueType
{
    public class EmailAddress
    {
        public string Value { get; private set; }
        
        private EmailAddress(string email)
        {
            if (!IsValid(email))
                throw new ArgumentException("Invalid email address");
            Value = email.ToLowerInvariant();
        }
        
        public static EmailAddress Create(string email) => new EmailAddress(email);
        
        private static bool IsValid(string email)
        {
            // Validation logic
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }
        
        public override string ToString() => Value;
    }
}
```

### Using Repository Pattern
```csharp
public class FranchiseeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Franchisee> _franchiseeRepo;
    
    public FranchiseeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _franchiseeRepo = _unitOfWork.Repository<Franchisee>();
    }
    
    public void UpdateFranchisee(Franchisee franchisee)
    {
        _unitOfWork.StartTransaction();
        try
        {
            _franchiseeRepo.Save(franchisee);
            _unitOfWork.SaveChanges();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}
```

### Timezone Handling
```csharp
public class ReportService
{
    private readonly IClock _clock;
    
    public ReportService(IClock clock)
    {
        _clock = clock;
    }
    
    public Report GenerateReport()
    {
        // Always work in UTC internally
        var utcNow = _clock.UtcNow;
        
        // Convert to local for display
        var localTime = _clock.ToLocal(utcNow);
        
        // Generate report...
    }
}
```

---

## Testing Considerations

### Mockable Abstractions
All interfaces are designed for easy mocking:
- `IClock` - Control time in tests
- `ISettings` - Override configuration
- `IRepository<T>` - Mock data access
- `ILogService` - Verify logging calls
- `IFileService` - Simulate file operations

### Example Test
```csharp
[Test]
public void UserService_CreateUser_HashesPassword()
{
    // Arrange
    var mockCrypto = new Mock<ICryptographyOneWayHashService>();
    mockCrypto.Setup(x => x.CreateHash(It.IsAny<string>()))
              .Returns(new SecureHash("hashed", "salt"));
    
    var service = new UserService(mockCrypto.Object);
    
    // Act
    service.CreateUser("user@test.com", "password123");
    
    // Assert
    mockCrypto.Verify(x => x.CreateHash("password123"), Times.Once);
}
```

---

## Common Pitfalls

1. **Avoid `ApplicationManager` in new code** - Use constructor injection instead of service locator
2. **Don't bypass `IUnitOfWork`** - Always use transactions for data modifications
3. **Use `IClock` not `DateTime.Now`** - Ensures timezone handling and testability
4. **Don't return domain entities from API** - Always transform to view models via factories
5. **Validate early** - Use value objects and custom exceptions at boundaries
6. **Log errors before rethrowing** - `ILogService.Error(ex)` before `throw`

---

## Related Documentation
- `../Domain/` - Domain entity definitions
- `../Users/` - User management and authentication
- `../Organizations/` - Franchisee/organization management
- `../Sales/` - Sales and invoice domain
- `../MarketingLead/` - Marketing lead management
