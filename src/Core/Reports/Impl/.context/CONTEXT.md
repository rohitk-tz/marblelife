<!-- AUTO-GENERATED: Header -->
# Reports/Impl — Module Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-11T06:35:45Z
**File Count**: 26 implementation files
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Impl subfolder contains **concrete implementations** of report service interfaces, factory interfaces, and notification service interfaces. This is where business logic executes: querying repositories, aggregating data, applying transformations, generating Excel files, and orchestrating email notifications. Implementations follow a consistent pattern: inject repositories via constructor, implement interface methods with business logic, delegate view model creation to factories.

### Design Patterns
- **Dependency Injection**: All implementations declare dependencies via constructor (repositories, factories, helpers)
- **[DefaultImplementation] Attribute**: Marks default implementation for interface, enabling auto-wiring in DI container
- **Repository Pattern**: All database access through `IRepository<T>`, never direct DbContext
- **Factory Delegation**: View model creation delegated to `IReportFactory` implementations for separation of concerns
- **Service Orchestration**: Complex operations (monthly review email) orchestrate multiple services/repositories

### Implementation Categories

#### 1. Core Report Services (Business Logic)
- **ReportService** (147KB) — Massive service handling sales reports, price estimates, late fees, AR, batch uploads
- **GrowthReportService** — Year-over-year growth analytics with Excel export
- **ProductReportService** — Product channel sales analytics
- **CustomerEmailReportService** — Email opt-in coverage analytics with charts

#### 2. Factory Implementations (Data Transformation)
- **ReportFactory** — Primary factory creating view models from domain aggregates
- **MlfsReportFactory** — Multi-location franchise system report factory
- **FranchiseeSalesInfoReportFactory** — Growth report view model factory
- **CustomerEmailAPIRecordFactory** — Email API record factory

#### 3. Notification Services (Email Automation)
- **EmailNotificationForPayrollReport** — Payroll report automation
- **EmailNotificationForPhotoReport** — Before/after photo email notifications
- **MonthlyReviewNotificationService** — Monthly digest email orchestration
- **EmailAPIIntegrationNotificationService** — API sync failure alerts
- **PriceEstimateParserNotificationService** — File parser error notifications
- **SalesDataUploadReportNotificationService** — Batch upload compliance alerts
- **SendCustomerListNotificationService** — Customer list export notifications

#### 4. Data Processing Services (ETL & Integration)
- **UpdateBatchUploadRecordService** — Batch upload status tracking
- **UpdateSalesAmountService** — Sales data aggregation updates
- **CreateEmailRecordForApiService** — Email API record creation
- **CreateMergeRecordForApiService** — Email merge field record creation
- **PriceEstimateFileParser** — Excel file parsing for price imports
- **CalendarImagesMigration** — Image migration utility (54KB)
- **S3BucketSync** — AWS S3 photo synchronization

#### 5. Specialized Report Generators
- **MlfsReport** — Multi-location franchise system complex report (43KB)
- **MLFSReportGrouping** — MLFS grouping logic

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Key Implementations -->
## Key Implementation Details

### ReportService.cs (147KB — Core Report Engine)
**Purpose**: Monolithic service implementing `IReportService` with 45+ methods for all report types

**Key Methods**:
- `GetReportsForService()` — Sales reports with franchisee/service filtering
- `GetLateFeeReportList()` — Late fee aging report
- `GetARReportList()` — Accounts receivable aging
- `GetPriceEstimateList()` — Price estimate matrix retrieval
- `SavePriceEstimateFranchiseeWise()` — Franchisee price override with validation
- `BulkUpdateCorporatePrice()` — Corporate bulk price updates
- `SaveFile()` — Excel price import with validation
- `GetBatchReport()` — Batch upload compliance tracking
- `DownloadSalesReport()`, `DownloadLateFeeReport()`, `DownloadPriceEstimateDataFile()` — Excel exports

**Dependencies**: 20+ repositories injected via constructor (FranchiseeService, FranchiseeInvoice, Payment, PriceEstimateServices, etc.)

**Anti-Pattern Alert**: This is a **God Object** — too many responsibilities in one class. Refactoring recommendation: split into SalesReportService, PriceEstimateService, BatchUploadService.

### GrowthReportService.cs
**Purpose**: Year-over-year growth analytics

**Key Logic**:
```csharp
// Query FranchiseeSalesInfo for current year and previous year
var salesInfo = _franchiseeSalesInfoRepository.Table
    .Where(x => x.Year == filter.Year || x.Year == (filter.Year - 1));

// Group by franchisee and calculate:
// - YTD sales current year
// - YTD sales last year
// - Average monthly sales: YTD / monthsElapsed
// - Growth %: (currentAvg - lastAvg) / lastAvg * 100
```

### CustomerEmailReportService.cs
**Purpose**: Email opt-in coverage analytics

**Key Logic**:
- Query customers with invoices in specified month/year
- Join `CustomerEmail` to identify opt-ins
- Calculate coverage: `(customersWithEmail / totalCustomers) * 100`
- Compare current month vs previous month
- Rank franchisees by coverage %

### ReportFactory.cs
**Purpose**: Primary factory for view model creation

**Key Methods**:
- `CreateViewModel(FranchiseeServiceClassCollection)` → `ServiceReportViewModel`
- `CreateViewModel(FranchiseeInvoice)` → `LateFeeReportViewModel`
- `CreateViewModel(EmailReportViewModel)` → `CustomerEmailReportViewModel`
- `CreateDomain(...)` → `BatchUploadRecord` entity

### EmailNotificationForPayrollReport.cs
**Purpose**: Automated payroll report generation and email delivery

**Flow**:
1. Query payroll data for specified franchisees and date range
2. Generate Excel via `IExcelFileCreator`
3. Create `NotificationQueue` entry with attachment
4. Set recipients: franchisee admin + corporate accounting
5. Background job processes queue and sends email

### MonthlyReviewNotificationService.cs
**Purpose**: Monthly digest email with multiple report attachments

**Flow**:
1. Generate sales report Excel
2. Generate growth report Excel
3. Generate late fee report Excel
4. Generate AR report Excel
5. Create HTML executive summary
6. Queue email with 4 attachments to all stakeholders

### PriceEstimateFileParser.cs
**Purpose**: Parse Excel files for bulk price imports

**Validation**:
- Required columns: ServiceTagId, CorporatePrice, FranchiseeId (optional)
- Price range validation: 0 < price < 999999
- Service tag ID existence check
- Duplicate row detection

**Error Handling**:
- Creates `PriceEstimateFileUpload` record with success/failure counts
- Logs errors to JSON file referenced by `ParsedLogFileId`
- Returns validation errors to UI for user correction

### S3BucketSync.cs
**Purpose**: AWS S3 integration for job photo management

**Key Operations**:
- `SyncBeforeAfterImages(jobId)` — Download job photos from S3
- Store local file paths in database for email attachments
- Used by `EmailNotificationForPhotoReport`

**Configuration**:
- S3 bucket name from `ISettings`
- IAM credentials from environment variables or AWS profile
- Supports multiple regions

### UpdateBatchUploadRecordService.cs
**Purpose**: Track batch upload compliance

**Key Logic**:
```csharp
// Mark upload complete
record.UploadedOn = DateTime.UtcNow;

// Validate uploaded data
if (ValidateSalesData(record))
    record.IsCorrectUploaded = true;
else
    record.IsCorrectUploaded = false;

// Send alert if late
if (record.UploadedOn > record.ExpectedUploadDate)
    SendLateUploadNotification(record);
```

### CalendarImagesMigration.cs (54KB)
**Purpose**: One-time data migration utility for calendar images

**Note**: Large implementation file for a migration task. Should be moved to separate migration project once complete.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **[../Domain/](../Domain/.context/CONTEXT.md)** — Domain entities (FranchiseeSalesInfo, BatchUploadRecord, CustomerEmailAPIRecord)
- **[../ViewModel/](../ViewModel/.context/CONTEXT.md)** — DTO models for API responses
- **[../../Billing/](../../../Billing/.context/CONTEXT.md)** — Invoice, Payment, PaymentItem entities
- **[../../Organizations/](../../../Organizations/.context/CONTEXT.md)** — Franchisee, ServiceType, MarketingClass entities
- **[../../Sales/](../../../Sales/.context/CONTEXT.md)** — FranchiseeSales, Customer, CustomerEmail entities
- **[../../Notification/](../../../Notification/.context/CONTEXT.md)** — NotificationQueue, NotificationEmailRecipient entities
- **[../../Application/](../../../Application/.context/CONTEXT.md)** — IRepository, IUnitOfWork, IExcelFileCreator, ISortingHelper, IClock, IFileService

### External Package Dependencies
- **EPPlus** — Excel file generation (`IExcelFileCreator` likely wraps this)
- **AWS SDK for .NET** — S3 bucket integration (`Amazon.S3`)
- **Entity Framework Core** — ORM for repository pattern
- **System.Linq** — LINQ queries and projections

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### ReportService Refactoring Opportunity
`ReportService.cs` is 147KB with 20+ repository dependencies. Recommended split:

```
ReportService.cs (147KB)
  ↓ Refactor into:
- SalesReportService.cs (GetReportsForService, DownloadSalesReport)
- LateFeeReportService.cs (GetLateFeeReportList, DownloadLateFeeReport)
- ARReportService.cs (GetARReportList, GetArReportModel)
- PriceEstimateService.cs (GetPriceEstimateList, SavePriceEstimate*, BulkUpdate*)
- BatchUploadService.cs (GetBatchReport, DownloadUploadReport)
```

This would improve maintainability and testability.

### Excel Export Performance
All `Download*Report` methods use synchronous Excel generation:
```csharp
bool success = _excelFileCreator.CreateExcelDocument(collection, fileName);
```

For large datasets (10,000+ rows), consider:
- Async Excel generation with progress tracking
- Streaming Excel writer to avoid memory spikes
- Background job queue for large exports (send email when complete)

### Notification Service Pattern
All notification services follow consistent pattern:
1. Generate report data
2. Create Excel attachment (if applicable)
3. Create `NotificationQueue` entry
4. Background processor sends email

**Key**: Notification creation is **synchronous**, email sending is **asynchronous** (decoupled via queue).

### Factory vs Service Boundary
- **Factories**: Data transformation only (domain → view model, view model → domain)
- **Services**: Business logic (queries, validation, calculations, persistence)

**Violation Example**: If factory has `IRepository` dependency, it's doing too much (should be in service).

### S3 Integration Error Handling
`S3BucketSync` should implement retry logic with exponential backoff:
```csharp
var retryPolicy = Policy
    .Handle<AmazonS3Exception>()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

await retryPolicy.ExecuteAsync(() => _s3Client.GetObjectAsync(request));
```

### Batch Upload Validation Tolerance
`UpdateBatchUploadRecordService` validates uploaded sales against invoice totals:
- Tolerance: ±2% (accounts for rounding differences)
- If delta > 2%, `IsCorrectUploaded = false` and alert sent
- Tolerance configurable via `ISettings.BatchUploadValidationTolerance`

### Price Estimate Import Column Mapping
Excel import expects specific column order:
1. Service Tag ID (required)
2. Service Name (display only, not used)
3. Corporate Price (required)
4. Corporate Additional Price (optional)
5. Franchisee ID (optional — if blank, applies to all)
6. Franchisee Price (optional override)

**Template Available**: Download template from UI to ensure correct format.

<!-- END CUSTOM SECTION -->
