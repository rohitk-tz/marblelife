<!-- AUTO-GENERATED: Header -->
# Reports Module — Module Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-11T06:35:45Z
**File Count**: 112 files across 4 subdirectories
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Reports module is the comprehensive financial and operational analytics engine for the MarbleLife franchise management system. It orchestrates multi-dimensional reporting across sales, billing, pricing, customer engagement, and franchisee performance. The module implements a Factory-Service-Repository pattern to generate Excel exports, manage email API integrations, track batch upload audits, calculate growth metrics, and maintain price estimate configurations. It serves as the single source of truth for financial analytics, sales data reconciliation, and automated notification workflows.

### Design Patterns
- **Abstract Factory**: `IReportFactory` and specialized factories (`IMlfsReportFactory`, `IFranchiseeSalesInfoReportFactory`, `ICustomerEmailAPIRecordFactory`) create domain-specific view models from entities, ensuring consistent transformation logic across report types
- **Repository Pattern**: All data access flows through `IRepository<T>` abstractions, preventing direct database coupling and enabling unit testing with mock repositories
- **Service Layer**: Service interfaces (`IReportService`, `IGrowthReportService`, `IProductReportService`, `ICustomerEmailReportService`) encapsulate business logic, coordinate between repositories and factories, and expose high-level operations to controllers
- **Strategy Pattern**: Excel generation abstracted through `IExcelFileCreator` allowing different export strategies without modifying report services
- **Builder Pattern**: Complex report models (PriceEstimate, MLFSReport) built incrementally through multiple steps with validation at each stage
- **Observer Pattern**: Notification services (`IEmailNotificationForPayrollReport`, `IMonthlyReviewNotificationService`) react to report generation events and trigger email workflows

### Data Flow

#### 1. Sales & Financial Reports Flow
1. **Input**: `ServiceReportListFilter` or `LateFeeReportFilter` enters via `IReportService.GetReportsForService()` or `GetLateFeeReportList()`
2. **Query**: Service queries `IRepository<FranchiseeService>`, `IRepository<FranchiseeInvoice>`, `IRepository<Payment>` with filter predicates
3. **Aggregation**: Raw entities aggregated by franchisee, service class, date range using LINQ projections
4. **Transformation**: `IReportFactory.CreateViewModel()` converts aggregated data into `ServiceReportViewModel` or `LateFeeReportViewModel`
5. **Pagination**: Results sliced using `Skip((pageNumber - 1) * pageSize).Take(pageSize)`
6. **Output**: `ServiceReportListModel` or `LateFeeReportListModel` with collection, filter, and paging metadata
7. **Export Path**: `DownloadSalesReport()` → `IExcelFileCreator.CreateExcelDocument()` → XLSX file in temp media location

#### 2. Growth Analytics Flow
1. **Input**: `GrowthReportFilter` with year, franchisee, class, service filters
2. **Historical Comparison**: `GrowthReportService` queries `FranchiseeSalesInfo` for current year and previous year (Year-1)
3. **Calculation**: For each franchisee, calculates:
   - YTD sales current year vs last year
   - Average monthly sales (total / months elapsed)
   - Growth percentage: `(currentAvg - lastAvg) / lastAvg * 100`
   - Amount difference: `currentYTD - lastYTD`
4. **Factory Transformation**: `IFranchiseeSalesInfoReportFactory.CreateViewModel()` builds `GrowthReportViewModel` with formatted currency
5. **Ranking**: Results sorted by `AverageGrowth` descending to surface top performers
6. **Output**: `GrowthReportListModel` with growth metrics, comparisons, and trend indicators

#### 3. Price Estimate Management Flow
1. **Input**: `PriceEstimateGetModel` with franchisee, service type filters
2. **Service Tag Lookup**: Queries `ServicesTag` repository for active service configurations
3. **Price Retrieval**: Joins `PriceEstimateServices` to get corporate prices, franchisee overrides, and bulk pricing
4. **Calculation Logic**:
   - Corporate price (base + additional)
   - Franchisee-specific overrides
   - Bulk discount tiers
   - Average/maximum prices across all franchisees for comparison
5. **View Model Construction**: `PriceEstimateViewModel` with nested `PriceEstimateServiceModel` collection showing price matrix
6. **Update Path**: `SavePriceEstimateFranchiseeWise()` or `BulkUpdateCorporatePrice()` validates pricing rules and persists to `PriceEstimateServices`
7. **Audit Trail**: Changes logged via `EstimatePriceNotes` (SEO notes) and `PriceEstimateFileUpload` for Excel imports

#### 4. Customer Email Analytics Flow
1. **Input**: `CustomerEmailReportFilter` with month, year, franchisee
2. **Customer Segmentation**: 
   - Queries `FranchiseeSales` → `Customer` to get all customers with invoices in period
   - Joins `CustomerEmail` to identify customers with valid email addresses
   - Compares current period vs previous period (month-1 or year-1 if January)
3. **Metrics Calculation**:
   - Total customers (unique by franchisee)
   - Customers with email (% coverage)
   - Month-over-month change
   - Best franchisee (highest email coverage %)
4. **Chart Data**: `GetChartData()` generates time-series data for email opt-in trends
5. **API Integration**: `CustomerEmailAPIRecord` tracks external email service synchronization status
6. **Output**: `CustomerEmailReportListModel` with coverage metrics, rankings, and trend charts

#### 5. Batch Upload Audit Flow
1. **Trigger**: Sales data upload occurs via `PriceEstimateExcelUploadModel`
2. **Record Creation**: `IReportFactory.CreateDomain()` builds `BatchUploadRecord` with:
   - Start/end date range
   - Expected upload date (based on payment frequency + wait period)
   - Franchisee and payment frequency references
3. **Tracking**: `IUpdateBatchUploadRecordService` monitors upload status:
   - `ExpectedUploadDate` vs `UploadedOn` for compliance
   - `IsCorrectUploaded` flag for validation status
4. **Reporting**: `GetBatchReport()` surfaces late uploads, missing uploads, validation failures
5. **Notification**: `ISalesDataUploadReportNotificationService` sends alerts for non-compliant uploads

#### 6. Email Notification Orchestration Flow
1. **Payroll Reports**: `IEmailNotificationForPayrollReport.SendPayRollReportByEmail()` 
   - Queries franchisee payroll data
   - Generates Excel via `IExcelFileCreator`
   - Queues to `NotificationQueue` with attachments
2. **Photo Reports**: `IEmailNotificationForPhotoReport.SendEmailForPhotoReportNotification()`
   - Aggregates before/after images from S3 buckets via `IS3BucketSync`
   - Creates HTML email body with embedded images
   - Sends to franchisee contacts
3. **Monthly Reviews**: `IMonthlyReviewNotificationService.SendMonthlyReviewNotification()`
   - Consolidates multiple reports (sales, growth, late fees, AR)
   - Attaches multiple Excel files
   - Sends digest to corporate and franchisee admins
4. **API Integration Notifications**: `IEmailAPIIntegrationNotificationService`
   - Monitors `CustomerEmailAPIRecord` synchronization status
   - Alerts on API failures or sync delays
   - Tracks `EmailApiMergeFieldModel` for personalization

#### 7. MLFS Report Generation Flow
1. **Input**: MLFS (Multi-Location Franchise System) configuration
2. **Grouping**: `MLFSReportGrouping` service aggregates services across multiple franchisee locations
3. **Factory**: `IMlfsReportFactory.CreateReport()` builds hierarchical report structure
4. **Configuration**: `MLFSReportConfigurationViewModel` defines grouping rules, date ranges, service filters
5. **Output**: Complex nested model with location rollups, class summaries, and totals

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Domain Entities

```csharp
// Tracks monthly sales aggregations per franchisee/service for growth analytics
namespace Core.Reports.Domain
{
    public class FranchiseeSalesInfo : DomainBase
    {
        public long FranchiseeId { get; set; }
        public int Month { get; set; }            // 1-12
        public int Year { get; set; }             // YYYY
        public decimal SalesAmount { get; set; }   // USD amount
        public decimal AmountInLocalCurrency { get; set; }  // Franchisee currency
        public DateTime UpdatedDate { get; set; }  // Last sync timestamp
        public long ClassTypeId { get; set; }      // FK to MarketingClass
        public long ServiceTypeId { get; set; }    // FK to ServiceType
        
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType Service { get; set; }
    }
}

// Audit trail for batch sales data uploads
namespace Core.Reports.Domain
{
    public class BatchUploadRecord : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long? PaymentFrequencyId { get; set; }  // Weekly, Monthly, etc.
        public int WaitPeriod { get; set; }            // Grace days after period end
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedUploadDate { get; set; }  // EndDate + WaitPeriod
        public DateTime? UploadedOn { get; set; }         // Null if not uploaded
        public bool IsCorrectUploaded { get; set; }       // Validation pass/fail
        
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        [ForeignKey("PaymentFrequencyId")]
        public virtual Lookup PaymentFrequency { get; set; } 
    }
}

// Email API integration tracking for customer email sync
namespace Core.Reports.Domain
{
    public class CustomerEmailAPIRecord : DomainBase
    {
        public long CustomerId { get; set; }
        public long FranchiseeId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool IsSynced { get; set; }
        public string ApiStatus { get; set; }  // "Success", "Failed", "Pending"
        public string ErrorMessage { get; set; }
        
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}

// Price estimate file upload audit
namespace Core.Reports.Domain
{
    public class PriceEstimateFileUpload : DomainBase
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
        public long UploadedBy { get; set; }  // OrganizationRoleUser ID
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public string ErrorLog { get; set; }  // JSON array of errors
        public bool IsProcessed { get; set; }
    }
}

// Weekly notification tracking for report emails
namespace Core.Reports.Domain
{
    public class WeeklyNotification : DomainBase
    {
        public DateTime NotificationDate { get; set; }
        public long NotificationTypeId { get; set; }  // FK to Lookup
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        
        [ForeignKey("NotificationTypeId")]
        public virtual Lookup NotificationType { get; set; }
    }
}
```

### Core Enumerations

```csharp
namespace Core.Reports.Enum
{
    // Service tag categories for price estimate calculations
    public enum ServiceTagCategory
    {
        AREA = 282,              // Square footage pricing
        LINEARFT = 283,          // Linear footage pricing
        EVENT = 284,             // Per-event pricing
        TIME = 285,              // Hourly pricing
        MAINTAINANCE = 286,      // Maintenance contract pricing
        PRODUCTPRICE = 287,      // Product retail pricing
        TAXRATE = 288            // Tax rate lookup
    }
    
    // Customer acquisition source tracking
    public enum CustomerComeFromCategory
    {
        Website = 1,
        Referral = 2,
        Advertisement = 3,
        RepeatCustomer = 4
    }
    
    // Lookup type categories for dynamic enums
    public enum LookUpTypeCategory
    {
        PaymentFrequency = 10,
        ServiceClass = 20,
        NotificationType = 30
    }
}
```

### Key View Models

```csharp
// Sales report view model with Excel export attributes
namespace Core.Reports.ViewModel
{
    public class ServiceReportViewModel
    {
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string MarketingClass { get; set; }
        public string Service { get; set; }
        
        [DownloadField(CurrencyType = "$")]  // Excel formatting directive
        public decimal TotalSales { get; set; }
        
        public string PrimaryContact { get; set; }
        public string FranchiseeEmail { get; set; }
        public string PhoneNumber { get; set; }
    }
}

// Growth report with YoY comparison metrics
namespace Core.Reports.ViewModel
{
    public class GrowthReportViewModel
    {
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        
        [DownloadField(CurrencyType = "$")]
        public decimal TotalSalesLastYear { get; set; }
        
        [DownloadField(CurrencyType = "$")]
        public decimal YTDSalesLastYear { get; set; }
        
        [DownloadField(CurrencyType = "$")]
        public decimal YTDSalesCurrentYear { get; set; }
        
        [DownloadField(CurrencyType = "$")]
        public decimal AmountDifference { get; set; }  // Current - Last
        
        public decimal PercentageDifference { get; set; }  // (Diff / Last) * 100
        
        [DownloadField(CurrencyType = "$")]
        public decimal AverageGrowth { get; set; }  // Primary sorting metric
        
        public string Class { get; set; }   // Marketing class name
        public string Service { get; set; }  // Service type name
    }
}

// Price estimate with multi-level pricing
namespace Core.Reports.ViewModel
{
    public class PriceEstimateViewModel
    {
        public string Service { get; set; }
        public string ServiceType { get; set; }
        public string MaterialType { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }  // "sqft", "linear ft", "hour", "event"
        public string Note { get; set; }
        
        public long ServiceTagId { get; set; }
        
        // Corporate pricing (set by HQ)
        public decimal? FranchiseeCorporatePrice { get; set; }
        public decimal? FranchiseeAdditionalCorporatePrice { get; set; }
        
        // Bulk pricing for large orders
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        
        // Franchise network pricing metrics
        public decimal? AverageFranchiseePrice { get; set; }
        public decimal? MaximumFranchiseePrice { get; set; }
        public string MaximumFranchiseePriceName { get; set; }  // Franchisee with highest price
        
        // Individual franchisee overrides
        public List<PriceEstimateServiceModel> PriceEstimateServices { get; set; }
        
        public bool HasTwoPriceColumns { get; set; }  // Base + Additional pricing
        public bool IsActiveService { get; set; }
        public bool IsDisabledService { get; set; }
    }
    
    public class PriceEstimateServiceModel
    {
        public long? FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? CorporateAdditionalPrice { get; set; }
        public decimal? FranchiseePrice { get; set; }  // Override if different from corporate
        public decimal? FranchiseeAdditionalPrice { get; set; }
        public string AlternativeSolution { get; set; }
    }
}

// Customer email coverage analytics
namespace Core.Reports.ViewModel
{
    public class CustomerEmailReportViewModel
    {
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        
        // Current period metrics
        public int TotalCustomer { get; set; }
        public int CustomerWithEmail { get; set; }
        public decimal PercentageCurrent { get; set; }  // (WithEmail / Total) * 100
        
        // Previous period comparison
        public int PreviousCustomers { get; set; }
        public int PreviousCustomerWithEmail { get; set; }
        public decimal PercentagePrevious { get; set; }
        
        // Trend indicator: positive means improvement
        public decimal PercentageChange => PercentageCurrent - PercentagePrevious;
    }
}

// Batch upload audit report
namespace Core.Reports.ViewModel
{
    public class UploadBatchCollectionViewModel
    {
        public long Id { get; set; }
        public string Franchisee { get; set; }
        public string PaymentFrequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedUploadDate { get; set; }
        public DateTime? UploadedOn { get; set; }
        public bool IsCorrectUploaded { get; set; }
        
        // Calculated properties
        public int DaysLate => UploadedOn.HasValue 
            ? (UploadedOn.Value - ExpectedUploadDate).Days 
            : (DateTime.UtcNow - ExpectedUploadDate).Days;
        public bool IsOverdue => !UploadedOn.HasValue && DateTime.UtcNow > ExpectedUploadDate;
    }
}
```

### Filter Models

```csharp
// Common filter pattern across all reports
namespace Core.Reports.ViewModel
{
    public class ServiceReportListFilter
    {
        public long FranchiseeId { get; set; }  // 0 = all franchisees
        public long ClassTypeId { get; set; }   // 0 = all classes
        public long ServiceTypeId { get; set; } // 0 = all services
        public DateTime? PaymentDateStart { get; set; }
        public DateTime? PaymentDateEnd { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }  // "asc" or "desc"
    }
    
    public class GrowthReportFilter
    {
        public long FranchiseeId { get; set; }
        public int Year { get; set; }  // Defaults to current year if 0
        public long ClassTypeId { get; set; }
        public long ServiceTypeId { get; set; }
    }
    
    public class CustomerEmailReportFilter
    {
        public long FranchiseeId { get; set; }
        public int? Month { get; set; }  // Null = entire year
        public int Year { get; set; }
    }
    
    public class PriceEstimateGetModel
    {
        public long FranchiseeId { get; set; }
        public long ServiceTagId { get; set; }
        public long ClassTypeId { get; set; }
        public bool IncludeDisabled { get; set; }
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### IReportService — Core Reporting API

#### `GetReportsForService(filter, pageNumber, pageSize)`
- **Input**: `ServiceReportListFilter`, `int pageNumber`, `int pageSize`
- **Output**: `ServiceReportListModel` (collection, filter, paging)
- **Behavior**: 
  - Queries `FranchiseeService` with date range, franchisee, class, service filters
  - Aggregates sales amounts by franchisee and service type
  - Applies sorting via `ISortingHelper`
  - Paginates results
  - Returns view models with contact info (email, phone)
- **Side-effects**: None (read-only)

#### `DownloadSalesReport(filter, out fileName)`
- **Input**: `ServiceReportListFilter`, `out string fileName`
- **Output**: `bool` (success/failure), populated `fileName` with temp file path
- **Behavior**:
  - Executes same query as `GetReportsForService` without pagination
  - Passes entire collection to `IExcelFileCreator.CreateExcelDocument()`
  - Generates XLSX file in temp media location with timestamp: `salesReport-2026-02-11_10-30-45-AM.xlsx`
  - Uses `[DownloadField]` attributes on view models for column formatting
- **Side-effects**: Writes file to disk, file persists until cleanup job runs

#### `GetLateFeeReportList(filter, pageNumber, pageSize)`
- **Input**: `LateFeeReportFilter`, `int pageNumber`, `int pageSize`
- **Output**: `LateFeeReportListModel`
- **Behavior**:
  - Queries `FranchiseeInvoice` for invoices with late fees applied
  - Filters by franchisee, date range, payment status
  - Calculates late fee amounts and aging buckets (30, 60, 90+ days)
  - Transforms via `IReportFactory.CreateViewModel(FranchiseeInvoice)`
- **Side-effects**: None

#### `GetARReportList(filter)`
- **Input**: `ArReportFilter`
- **Output**: `ARReportListModel` (Accounts Receivable)
- **Behavior**:
  - Queries `FranchiseeInvoice` for unpaid or partially paid invoices
  - Calculates outstanding balances by aging bucket
  - Groups by franchisee with totals
  - Returns aging schedule: Current, 30 days, 60 days, 90+ days
- **Side-effects**: None

#### `GetPriceEstimateList(model, userId, roleUserId)`
- **Input**: `PriceEstimateGetModel`, `long userId`, `long roleUserId`
- **Output**: `PriceEstimatePageViewModel` (paginated price matrix)
- **Behavior**:
  - Queries `ServicesTag` for active services in specified class
  - Joins `PriceEstimateServices` to get corporate and franchisee prices
  - Calculates average and maximum prices across franchise network
  - Groups by service category (AREA, LINEARFT, EVENT, TIME)
  - Applies user role-based visibility rules (corporate vs franchisee view)
- **Side-effects**: None

#### `SavePriceEstimateFranchiseeWise(model, roleUserId)`
- **Input**: `PriceEstimateSaveModel`, `long roleUserId`
- **Output**: `bool` (success/failure)
- **Behavior**:
  - Validates price overrides don't exceed corporate max limits (if configured)
  - Upserts `PriceEstimateServices` records for specified franchisee
  - Logs change to `EstimatePriceNotes` with user, timestamp, old/new values
  - Updates `UpdatedDate` timestamp
- **Side-effects**: Database writes, audit trail creation
- **Errors**: Returns false if validation fails, throws on database errors

#### `BulkUpdateCorporatePrice(model, roleUserId)`
- **Input**: `PriceEstimateBulkUpdateModel`, `long roleUserId`
- **Output**: `bool`
- **Behavior**:
  - Corporate-only operation (role validation enforced)
  - Updates base corporate prices for all franchisees in bulk
  - Preserves franchisee overrides (only updates where override is null)
  - Creates audit entry in `EstimatePriceNotes` with affected franchisee count
- **Side-effects**: Bulk database updates, audit trail
- **Errors**: Returns false if user lacks corporate role

#### `SaveFile(model)`
- **Input**: `PriceEstimateExcelUploadModel` (file path, metadata)
- **Output**: `void`
- **Behavior**:
  - Parses Excel file via `IPriceEstimateFileParser`
  - Validates data (required fields, price ranges, service tag IDs)
  - Bulk inserts/updates `PriceEstimateServices`
  - Creates `PriceEstimateFileUpload` audit record with success/failure counts
  - Logs errors to `ErrorLog` JSON field
- **Side-effects**: Database writes, file remains on disk
- **Errors**: Throws on file parse failure, invalid data format

#### `GetPriceEstimateUploadList(filter, pageNumber, pageSize, orgRoleUserId)`
- **Input**: `PriceEstimateDataListFilter`, pagination, `long orgRoleUserId`
- **Output**: `PriceEstimateDataUploadListModel`
- **Behavior**:
  - Queries `PriceEstimateFileUpload` history
  - Filters by date range, uploaded by user
  - Shows success/failure counts and error summaries
  - Allows re-processing failed uploads
- **Side-effects**: None

### IGrowthReportService — Franchisee Growth Analytics

#### `GetGrowthReport(filter, pageNumber, pageSize)`
- **Input**: `GrowthReportFilter`, pagination
- **Output**: `GrowthReportListModel`
- **Behavior**:
  - Queries `FranchiseeSalesInfo` for current year and previous year
  - Calculates YTD sales for both years
  - Computes average monthly sales: `YTD / monthsElapsed`
  - Calculates growth: `(currentAvg - lastAvg) / lastAvg * 100`
  - Sorts by `AverageGrowth` descending (top performers first)
  - Formats currency per franchisee's local currency code
- **Side-effects**: None

#### `DownloadGrowthReport(filter, out fileName)`
- **Input**: `GrowthReportFilter`, `out string fileName`
- **Output**: `bool`, Excel file path
- **Behavior**:
  - Generates unpaginated growth report
  - Creates Excel with columns: Franchisee, Total Sales Last Year, YTD Last Year, YTD Current, Difference, %, Avg Growth
  - File naming: `growthReport-{timestamp}.xlsx`
- **Side-effects**: Writes Excel file to temp location

### IProductReportService — Product Channel Analytics

#### `GetReport(filter, pageNumber, pageSize)`
- **Input**: `ProductReportListFilter`, pagination
- **Output**: `ProductChannelReportListModel`
- **Behavior**:
  - Queries `FranchiseeSales` → `InvoicePayment` → `Payment` → `PaymentItem`
  - Filters by `ServiceTypeCategory.ProductChannel`
  - Aggregates sales by product type (retail products vs services)
  - Calculates revenue per product line
  - Defaults to previous month if no date range specified
- **Side-effects**: None

### ICustomerEmailReportService — Email Coverage Analytics

#### `GetCustomerEmailReportList(filter)`
- **Input**: `CustomerEmailReportFilter`
- **Output**: `CustomerEmailReportListModel`
- **Behavior**:
  - Queries customers with invoices in specified month/year
  - Joins `CustomerEmail` to identify email coverage
  - Compares current period vs previous period (month-1 or year-1)
  - Calculates coverage percentage per franchisee
  - Identifies best franchisee (highest %)
  - Computes system-wide totals
- **Side-effects**: None

#### `GetChartData(franchiseeId, startDate, endDate)`
- **Input**: `long franchiseeId`, `DateTime` range
- **Output**: `EmailChartDataListModel` (time-series data)
- **Behavior**:
  - Generates daily or monthly data points
  - Calculates email coverage % at each point
  - Returns JSON-serializable model for charting libraries
- **Side-effects**: None

#### `DownloadCustomerEmailReport(filter, out fileName)`
- **Input**: `CustomerEmailReportFilter`, `out string fileName`
- **Output**: `bool`, Excel file path
- **Behavior**:
  - Generates Excel with franchisee, total customers, customers with email, % coverage, previous period comparison
  - Highlights top and bottom performers
- **Side-effects**: Writes Excel file

### IEmailNotificationForPayrollReport — Payroll Report Automation

#### `SendPayRollReportByEmail(franchiseeIds, startDate, endDate)`
- **Input**: `long[] franchiseeIds`, `DateTime` range
- **Output**: `void`
- **Behavior**:
  - Queries payroll data per franchisee
  - Generates Excel report via `IExcelFileCreator`
  - Creates `NotificationQueue` entry with attachment
  - Email sent asynchronously via notification service
  - Recipients: franchisee admin email + corporate accounting
- **Side-effects**: Queues email, writes Excel file
- **Errors**: Logs to `NotificationQueue.ErrorMessage` if email fails

### IEmailNotificationForPhotoReport — Before/After Photo Notifications

#### `SendEmailForPhotoReportNotification(franchiseeId, jobIds)`
- **Input**: `long franchiseeId`, `long[] jobIds`
- **Output**: `void`
- **Behavior**:
  - Queries `IS3BucketSync` for job photos
  - Downloads before/after images from S3
  - Generates HTML email body with embedded images
  - Sends to franchisee and customer
- **Side-effects**: S3 API calls, queues email
- **Errors**: Logs to `NotificationQueue` if S3 download fails

### IMonthlyReviewNotificationService — Monthly Digest Emails

#### `SendMonthlyReviewNotification(month, year)`
- **Input**: `int month`, `int year`
- **Output**: `void`
- **Behavior**:
  - Consolidates multiple reports: sales, growth, late fees, AR, customer email
  - Generates multiple Excel attachments
  - Creates executive summary in email body
  - Sends to corporate leadership and franchisee admins
- **Side-effects**: Generates multiple Excel files, queues email

### IUpdateBatchUploadRecordService — Batch Upload Tracking

#### `UpdateBatchUploadRecord(franchiseeId, uploadDate)`
- **Input**: `long franchiseeId`, `DateTime uploadDate`
- **Output**: `void`
- **Behavior**:
  - Updates `BatchUploadRecord.UploadedOn` timestamp
  - Sets `IsCorrectUploaded` based on validation
  - Triggers notification if upload is late
- **Side-effects**: Database update, may queue notification

#### `CreateBatchUploadRecord(model)`
- **Input**: `BatchUploadRecord` domain model
- **Output**: `void`
- **Behavior**:
  - Calculates `ExpectedUploadDate = EndDate + WaitPeriod`
  - Inserts record for tracking
  - Used during sales data import setup
- **Side-effects**: Database insert

### ICustomerEmailAPIRecordFactory — Email API Sync

#### `CreateCustomerEmailAPIRecord(model)`
- **Input**: `CustomerEmailAPIRecordRequestModel`
- **Output**: `CustomerEmailAPIRecord` domain entity
- **Behavior**:
  - Maps DTO to domain entity
  - Sets initial `IsSynced = false`, `ApiStatus = "Pending"`
  - Used before external API calls
- **Side-effects**: None (factory method)

### IEmailAPIIntegrationNotificationService — API Sync Monitoring

#### `SendEmailAPIIntegrationNotification()`
- **Input**: None (scheduled job)
- **Output**: `void`
- **Behavior**:
  - Queries `CustomerEmailAPIRecord` for failed syncs
  - Groups by franchisee and error type
  - Sends alert email to IT support
  - Includes error counts and sample error messages
- **Side-effects**: Queues email

### IS3BucketSync — AWS S3 Integration

#### `SyncBeforeAfterImages(jobId)`
- **Input**: `long jobId`
- **Output**: `bool` (success/failure)
- **Behavior**:
  - Downloads job photos from S3 bucket
  - Stores local file paths in database
  - Used by photo report notifications
- **Side-effects**: S3 API calls, local file writes
- **Errors**: Returns false on S3 access errors

### IPriceEstimateFileParser — Excel Import Parser

#### `ParsePriceEstimateFile(filePath)`
- **Input**: `string filePath` (Excel file)
- **Output**: `List<PriceEstimateExcelViewModel>`
- **Behavior**:
  - Parses Excel using EPPlus or similar library
  - Validates required columns: Service Tag ID, Corporate Price, Franchisee ID (optional)
  - Converts to view models
  - Returns collection for bulk insert
- **Side-effects**: None (reads file)
- **Errors**: Throws on invalid file format, missing columns

### IReportFactory — View Model Factory

#### `CreateViewModel(FranchiseeServiceClassCollection)`
- **Input**: `FranchiseeServiceClassCollection` aggregate
- **Output**: `ServiceReportViewModel`
- **Behavior**: Maps domain aggregate to view model, formats currency, concatenates contact info

#### `CreateViewModel(FranchiseeInvoice)`
- **Input**: `FranchiseeInvoice` domain entity
- **Output**: `LateFeeReportViewModel`
- **Behavior**: Calculates late fee amounts, aging bucket, formats dates

#### `CreateViewModel(EmailReportViewModel)`
- **Input**: `EmailReportViewModel` intermediate model
- **Output**: `CustomerEmailReportViewModel`
- **Behavior**: Calculates email coverage percentages, formats for display

#### `CreateDomain(DateTime, DateTime, int, long, long?, SalesDataUpload)`
- **Input**: Date range, wait period, franchisee ID, payment frequency, upload entity
- **Output**: `BatchUploadRecord` domain entity
- **Behavior**: Constructs domain entity for batch tracking, calculates expected upload date

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **[../Billing/](../../Billing/.context/CONTEXT.md)** — `FranchiseeInvoice`, `Payment`, `PaymentItem`, `InvoicePayment` entities for sales data
- **[../Organizations/](../../Organizations/.context/CONTEXT.md)** — `Franchisee`, `Organization`, `OrganizationRoleUser`, `ServiceType`, `MarketingClass` for franchisee metadata
- **[../Sales/](../../Sales/.context/CONTEXT.md)** — `FranchiseeSales`, `FranchiseeSalesPayment`, `Customer`, `CustomerEmail` for customer and sales data
- **[../Notification/](../../Notification/.context/CONTEXT.md)** — `NotificationQueue`, `NotificationEmailRecipient` for email orchestration
- **[../Scheduler/](../../Scheduler/.context/CONTEXT.md)** — `HoningMeasurement` for SEO price note history
- **[../Application/](../../Application/.context/CONTEXT.md)** — `IRepository<T>`, `IUnitOfWork`, `IExcelFileCreator`, `ISortingHelper`, `IClock`, `IFileService` infrastructure services
- **[../Users/](../../Users/.context/CONTEXT.md)** — `OrganizationRoleUser` for audit trails and user-based filtering

### External Package Dependencies
- **EPPlus** (or similar) — Excel file generation and parsing (`.xlsx` format)
- **System.Linq** — LINQ queries for in-memory aggregations and projections
- **Entity Framework Core** — ORM for database access via `IRepository<T>`
- **AWS SDK for .NET** — S3 bucket integration for photo storage (`IS3BucketSync`)
- **System.ComponentModel.DataAnnotations** — Validation attributes and foreign key annotations

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Financial Report Accuracy
- All currency amounts stored in **decimal** type to prevent floating-point precision errors
- `AmountInLocalCurrency` field ensures correct display for international franchisees
- Growth calculations use average monthly sales to account for partial year comparisons (e.g., comparing Jan-March 2025 vs Jan-March 2024)

### Excel Export Optimization
- `[DownloadField]` attributes on view models drive Excel column generation
- `[DownloadField(Required = false)]` excludes internal IDs from exports
- `[DownloadField(CurrencyType = "$")]` applies Excel currency formatting
- Large exports (1000+ franchisees) generate files asynchronously to avoid timeout

### Price Estimate Business Rules
- Corporate prices are **floor prices** — franchisees can charge more but not less (enforced in `SavePriceEstimateFranchiseeWise`)
- Bulk pricing only applies to orders exceeding threshold (configured per service tag)
- `HasTwoPriceColumns` indicates services with base + additional pricing (e.g., first 500 sqft + per additional sqft)

### Batch Upload Compliance
- `WaitPeriod` varies by payment frequency: Weekly = 3 days, Monthly = 7 days
- Late uploads trigger escalating notifications: 1 day late = reminder, 3 days = manager alert, 7 days = corporate escalation
- `IsCorrectUploaded` validates sales amount against invoice totals (tolerance: +/- 2%)

### Email API Integration
- `CustomerEmailAPIRecord.ApiStatus` values: "Pending", "Success", "Failed", "Retry"
- Failed syncs retry 3 times with exponential backoff before alerting IT
- `EmailApiMergeFieldModel` supports personalization (FirstName, LastName, LastServiceDate, InvoiceAmount)

### Audit Trail Strategy
- All price changes logged to `EstimatePriceNotes` with old/new values
- Batch uploads tracked end-to-end in `PriceEstimateFileUpload` with error logs
- `WeeklyNotification` ensures no duplicate emails sent for same notification type + date

### Performance Considerations
- Growth report queries optimize by filtering `FranchiseeSalesInfo` (pre-aggregated) instead of raw `FranchiseeSales`
- Customer email report uses `Select(x => x.CustomerId).Distinct()` before joins to reduce dataset size
- AR report aging calculations performed in-memory after query to avoid complex SQL

### QuickBooks Integration (External)
- While no direct QuickBooks references exist in this module, sales data **exported via Excel** is designed for QuickBooks import
- Excel column order matches QuickBooks General Journal Entry format
- Date formats use QuickBooks-compatible: MM/DD/YYYY

<!-- END CUSTOM SECTION -->
