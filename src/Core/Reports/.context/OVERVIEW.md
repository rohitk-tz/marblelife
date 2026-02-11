<!-- AUTO-GENERATED: Header -->
# Reports Module
> Comprehensive financial analytics, sales reporting, and operational intelligence for the MarbleLife franchise management system
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Reports module is the **analytical brain** of the MarbleLife franchise system, transforming raw transactional data into actionable business intelligence. Think of it as a **multi-dimensional reporting engine** that serves three primary stakeholders:

1. **Corporate Leadership**: Executive dashboards showing franchise network health, growth trends, compliance metrics
2. **Franchisee Owners**: Operational reports for managing their local business (sales, customer engagement, pricing)
3. **Accounting/Finance**: Audit trails, batch upload tracking, accounts receivable aging, late fee calculations

### Why This Module Exists

Franchise systems generate massive amounts of transactional data (invoices, payments, customer interactions, service deliveries). Without structured reporting, this data is noise. The Reports module:

- **Aggregates** sales data across time periods, service types, and franchisee locations
- **Compares** current performance against historical baselines (YoY growth, MoM trends)
- **Monitors** operational compliance (batch upload deadlines, email opt-in rates)
- **Exports** financial data to Excel for external accounting systems (QuickBooks)
- **Automates** recurring notification workflows (monthly reviews, payroll reports, photo deliverables)

### Key Capabilities

- **Financial Reports**: Sales by service type, late fee aging, accounts receivable schedules
- **Growth Analytics**: Year-over-year comparisons, average monthly growth, top performer rankings
- **Price Management**: Corporate pricing + franchisee overrides, bulk pricing tiers, audit trails
- **Customer Analytics**: Email opt-in coverage, engagement trends, API synchronization status
- **Audit & Compliance**: Batch upload tracking, file import validation, change history
- **Email Automation**: Scheduled reports, photo notifications, API integration alerts

### Architectural Philosophy

The module follows a **layered architecture**:

```
Controllers (Web API)
        ↓
Service Interfaces (Business Logic)
        ↓
Repository Pattern (Data Access)
        ↓
Domain Entities (Database)
        ↓
View Models (DTOs for UI/Excel)
```

Factories (`IReportFactory`) sit between services and view models, ensuring consistent transformation logic. Excel generation is abstracted through `IExcelFileCreator`, allowing swap-out implementations (EPPlus, ClosedXML, etc.) without changing report services.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup

The Reports module depends on several other Core modules:

```bash
# Required module references
- Core.Application (IRepository, IUnitOfWork, IExcelFileCreator)
- Core.Billing (Invoice, Payment entities)
- Core.Organizations (Franchisee, ServiceType entities)
- Core.Sales (FranchiseeSales, Customer entities)
- Core.Notification (NotificationQueue for emails)
```

No external NuGet packages need explicit installation — they're managed at the solution level.

### Example 1: Generating a Sales Report

```csharp
using Core.Reports;
using Core.Reports.ViewModel;

public class SalesReportController : ControllerBase
{
    private readonly IReportService _reportService;
    
    public SalesReportController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    [HttpGet("sales-report")]
    public IActionResult GetSalesReport(
        long franchiseeId, 
        DateTime startDate, 
        DateTime endDate, 
        int pageNumber = 1, 
        int pageSize = 50)
    {
        var filter = new ServiceReportListFilter
        {
            FranchiseeId = franchiseeId,  // 0 = all franchisees
            PaymentDateStart = startDate,
            PaymentDateEnd = endDate,
            ClassTypeId = 0,  // 0 = all classes
            ServiceTypeId = 0,  // 0 = all services
            SortColumn = "TotalSales",
            SortDirection = "desc"
        };
        
        var result = _reportService.GetReportsForService(filter, pageNumber, pageSize);
        
        return Ok(new
        {
            Data = result.Collection,
            Pagination = result.PagingModel,
            TotalSales = result.Collection.Sum(x => x.TotalSales)
        });
    }
}
```

### Example 2: Exporting Growth Report to Excel

```csharp
using Core.Reports;
using Core.Reports.ViewModel;

public class GrowthReportController : ControllerBase
{
    private readonly IGrowthReportService _growthReportService;
    
    [HttpGet("growth-report/export")]
    public IActionResult ExportGrowthReport(int year, long franchiseeId = 0)
    {
        var filter = new GrowthReportFilter
        {
            Year = year,
            FranchiseeId = franchiseeId,
            ClassTypeId = 0,
            ServiceTypeId = 0
        };
        
        string fileName;
        bool success = _growthReportService.DownloadGrowthReport(filter, out fileName);
        
        if (!success)
            return BadRequest("Failed to generate report");
        
        var fileBytes = System.IO.File.ReadAllBytes(fileName);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"GrowthReport_{year}.xlsx");
    }
}
```

### Example 3: Updating Price Estimates (Franchisee Override)

```csharp
using Core.Reports;
using Core.Reports.ViewModel;

public class PriceEstimateController : ControllerBase
{
    private readonly IReportService _reportService;
    
    [HttpPost("price-estimate/save")]
    public IActionResult SaveFranchiseePrice(PriceEstimateSaveModel model, long userId, long roleUserId)
    {
        // Validate user has permission for this franchisee
        if (!UserCanEditFranchisee(userId, model.FranchiseeId))
            return Forbid();
        
        bool success = _reportService.SavePriceEstimateFranchiseeWise(model, roleUserId);
        
        if (!success)
            return BadRequest("Price validation failed - cannot exceed corporate maximum");
        
        return Ok(new { Message = "Prices updated successfully" });
    }
}
```

### Example 4: Bulk Updating Corporate Prices (Admin Only)

```csharp
using Core.Reports;
using Core.Reports.ViewModel;

public class CorporatePricingController : ControllerBase
{
    private readonly IReportService _reportService;
    
    [HttpPost("corporate-pricing/bulk-update")]
    [Authorize(Roles = "Corporate")]
    public IActionResult BulkUpdateCorporatePrices(PriceEstimateBulkUpdateModel model, long roleUserId)
    {
        // model contains:
        // - ServiceTagId
        // - NewCorporatePrice
        // - NewAdditionalPrice
        // - ApplyToAllFranchisees flag
        
        bool success = _reportService.BulkUpdateCorporatePrice(model, roleUserId);
        
        if (!success)
            return StatusCode(500, "Bulk update failed");
        
        return Ok(new { Message = $"Corporate prices updated for service tag {model.ServiceTagId}" });
    }
}
```

### Example 5: Tracking Batch Upload Compliance

```csharp
using Core.Reports;
using Core.Reports.ViewModel;

public class BatchUploadController : ControllerBase
{
    private readonly IReportService _reportService;
    
    [HttpGet("batch-uploads/compliance")]
    public IActionResult GetBatchUploadCompliance(long franchiseeId, int pageNumber = 1, int pageSize = 50)
    {
        var filter = new UploadReportFilter
        {
            FranchiseeId = franchiseeId,
            StartDate = DateTime.UtcNow.AddMonths(-6),  // Last 6 months
            EndDate = DateTime.UtcNow
        };
        
        var result = _reportService.GetBatchReport(filter, pageNumber, pageSize);
        
        // Find overdue uploads
        var overdueUploads = result.Collection
            .Where(x => x.IsOverdue)
            .OrderByDescending(x => x.DaysLate)
            .ToList();
        
        return Ok(new
        {
            AllUploads = result.Collection,
            OverdueCount = overdueUploads.Count,
            OverdueUploads = overdueUploads,
            ComplianceRate = CalculateComplianceRate(result.Collection)
        });
    }
    
    private decimal CalculateComplianceRate(IEnumerable<UploadBatchCollectionViewModel> uploads)
    {
        var total = uploads.Count();
        if (total == 0) return 100m;
        
        var onTime = uploads.Count(x => !x.IsOverdue && x.IsCorrectUploaded);
        return Math.Round((decimal)onTime / total * 100, 2);
    }
}
```

### Example 6: Customer Email Coverage Analytics

```csharp
using Core.Reports;
using Core.Reports.ViewModel;

public class CustomerEmailAnalyticsController : ControllerBase
{
    private readonly ICustomerEmailReportService _emailReportService;
    
    [HttpGet("customer-email/coverage")]
    public IActionResult GetEmailCoverage(long franchiseeId, int? month, int year)
    {
        var filter = new CustomerEmailReportFilter
        {
            FranchiseeId = franchiseeId,
            Month = month,  // Null = entire year
            Year = year
        };
        
        var result = _emailReportService.GetCustomerEmailReportList(filter);
        
        return Ok(new
        {
            BestFranchisee = result.BestFranchisee,
            SystemWideTotals = result.Total,
            FranchiseeBreakdown = result.Collection,
            Insights = new
            {
                SystemCoveragePercent = result.Total.PercentageCurrent,
                ImprovementFromLastPeriod = result.Total.PercentageCurrent - result.Total.PercentagePrevious,
                TopPerformer = result.BestFranchisee?.Franchisee,
                TopPerformerRate = result.BestFranchisee?.PercentageCurrent
            }
        });
    }
}
```

### Example 7: Scheduling Monthly Review Emails

```csharp
using Core.Reports;

public class ScheduledReportJob
{
    private readonly IMonthlyReviewNotificationService _notificationService;
    
    // Called by scheduler on 1st of each month
    public void ExecuteMonthlyReview()
    {
        var previousMonth = DateTime.UtcNow.AddMonths(-1);
        var month = previousMonth.Month;
        var year = previousMonth.Year;
        
        // Generates and emails comprehensive monthly review to all stakeholders
        _notificationService.SendMonthlyReviewNotification(month, year);
        
        // This internally:
        // 1. Generates sales report Excel
        // 2. Generates growth report Excel
        // 3. Generates late fee report Excel
        // 4. Generates AR report Excel
        // 5. Creates executive summary in email body
        // 6. Queues email with all attachments
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Core Report Services

| Service | Method | Description |
|---------|--------|-------------|
| **IReportService** | `GetReportsForService(filter, page, size)` | Paginated sales report by franchisee/service |
| | `DownloadSalesReport(filter, out fileName)` | Excel export of sales report |
| | `GetLateFeeReportList(filter, page, size)` | Late fee aging report |
| | `GetARReportList(filter)` | Accounts receivable aging schedule |
| | `GetPriceEstimateList(model, userId, roleUserId)` | Price estimate matrix (corporate + franchisee) |
| | `SavePriceEstimateFranchiseeWise(model, roleUserId)` | Update franchisee price overrides |
| | `BulkUpdateCorporatePrice(model, roleUserId)` | Bulk update corporate base prices |
| | `SaveFile(model)` | Import price estimates from Excel |
| | `GetBatchReport(filter, page, size)` | Batch upload compliance tracking |
| **IGrowthReportService** | `GetGrowthReport(filter, page, size)` | YoY growth analytics with rankings |
| | `DownloadGrowthReport(filter, out fileName)` | Excel export of growth report |
| **IProductReportService** | `GetReport(filter, page, size)` | Product channel sales analytics |
| **ICustomerEmailReportService** | `GetCustomerEmailReportList(filter)` | Email opt-in coverage analytics |
| | `GetChartData(franchiseeId, start, end)` | Time-series email trend data |
| | `DownloadCustomerEmailReport(filter, out fileName)` | Excel export of email coverage |

### Notification Services

| Service | Method | Description |
|---------|--------|-------------|
| **IEmailNotificationForPayrollReport** | `SendPayRollReportByEmail(franchiseeIds, start, end)` | Email payroll report to franchisees + accounting |
| **IEmailNotificationForPhotoReport** | `SendEmailForPhotoReportNotification(franchiseeId, jobIds)` | Email before/after photos from S3 |
| **IMonthlyReviewNotificationService** | `SendMonthlyReviewNotification(month, year)` | Monthly digest email with multiple attachments |
| **IEmailAPIIntegrationNotificationService** | `SendEmailAPIIntegrationNotification()` | Alert on email API sync failures |
| **ISalesDataUploadReportNotificationService** | Monitors and alerts on late/failed batch uploads |

### Factory Services

| Service | Method | Description |
|---------|--------|-------------|
| **IReportFactory** | `CreateViewModel(FranchiseeServiceClassCollection)` | Map to ServiceReportViewModel |
| | `CreateViewModel(FranchiseeInvoice)` | Map to LateFeeReportViewModel |
| | `CreateViewModel(EmailReportViewModel)` | Map to CustomerEmailReportViewModel |
| | `CreateDomain(...)` | Build BatchUploadRecord entity |
| **IFranchiseeSalesInfoReportFactory** | `CreateViewModel(FranchiseeSalesInfoList, filter)` | Map to GrowthReportViewModel |
| **ICustomerEmailAPIRecordFactory** | `CreateCustomerEmailAPIRecord(model)` | Map to CustomerEmailAPIRecord entity |
| **IMlfsReportFactory** | `CreateReport(...)` | Build multi-location franchise system report |

### Utility Services

| Service | Method | Description |
|---------|--------|-------------|
| **IPriceEstimateFileParser** | `ParsePriceEstimateFile(filePath)` | Parse Excel file for price import |
| **IS3BucketSync** | `SyncBeforeAfterImages(jobId)` | Download job photos from AWS S3 |
| **IUpdateBatchUploadRecordService** | `UpdateBatchUploadRecord(franchiseeId, uploadDate)` | Mark batch upload complete |
| **IUpdateSalesAmountService** | Updates aggregated sales amounts in FranchiseeSalesInfo |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Excel Export Fails with "File in Use" Error
**Symptom**: `DownloadSalesReport` returns false, temp file path locked

**Cause**: Previous export not cleaned up, file handle still open

**Solution**:
- Ensure `IExcelFileCreator` implementation calls `Dispose()` on Excel package
- Check temp file cleanup job is running (should run hourly)
- Verify temp directory has write permissions

### Growth Report Shows 0% Growth for All Franchisees
**Symptom**: All `AverageGrowth` values = 0

**Cause**: `FranchiseeSalesInfo` table not populated for previous year

**Solution**:
- Run sales aggregation job: `IUpdateSalesAmountService.UpdateSalesAmounts(year - 1)`
- Verify `FranchiseeSalesInfo` has records for both `Year` and `Year - 1`
- Check franchisee has invoices in both years

### Price Estimate Save Fails Silently
**Symptom**: `SavePriceEstimateFranchiseeWise` returns false, no error message

**Cause**: Price validation rule violation (franchisee price > corporate max)

**Solution**:
- Check if corporate has configured max price limits
- Query `PriceEstimateServices` for `CorporateMaxPrice` column
- If limit exists, franchisee price must be ≤ max
- Consider returning validation error message instead of bool

### Batch Upload Marked as Late but Was On Time
**Symptom**: `IsOverdue = true`, but franchisee uploaded before deadline

**Cause**: `ExpectedUploadDate` calculation doesn't account for timezone

**Solution**:
- Ensure `BatchUploadRecord.EndDate` is stored in UTC
- `ExpectedUploadDate = EndDate.AddDays(WaitPeriod)` must use UTC
- Compare `UploadedOn` (UTC) against `ExpectedUploadDate` (UTC)

### Email Notifications Not Sending
**Symptom**: Reports generate, but emails never arrive

**Cause**: `NotificationQueue` entries stuck in "Pending" status

**Solution**:
- Check notification processor background job is running
- Query `NotificationQueue` for `ErrorMessage` column
- Common errors:
  - SMTP connection timeout (firewall issue)
  - Attachment size exceeds limit (compress Excel files)
  - Invalid recipient email (validate franchisee email on save)

### Customer Email Coverage Shows 100% but Customers Have No Emails
**Symptom**: `CustomerEmailReportViewModel.PercentageCurrent = 100`, but `CustomerEmail` table mostly empty

**Cause**: Query filtering customers without invoices, skewing percentage

**Solution**:
- Verify query includes **all customers** in denominator, not just customers with invoices in period
- Consider separate metrics: "Customers with Email" vs "Invoiced Customers with Email"

### S3 Photo Download Fails in Photo Report
**Symptom**: `SendEmailForPhotoReportNotification` throws S3 exception

**Cause**: AWS credentials expired or bucket permissions changed

**Solution**:
- Verify `IS3BucketSync` configuration has valid IAM credentials
- Check S3 bucket policy allows `GetObject` for application role
- Ensure photo file paths in database match actual S3 keys (case-sensitive)

### Bulk Price Update Overwrites Franchisee Overrides
**Symptom**: After `BulkUpdateCorporatePrice`, franchisee custom prices are lost

**Cause**: Bulk update should only affect records where `FranchiseePrice IS NULL`

**Solution**:
- Ensure bulk update SQL: `UPDATE PriceEstimateServices SET CorporatePrice = @newPrice WHERE FranchiseePrice IS NULL`
- Do not use: `UPDATE ... SET CorporatePrice = @newPrice, FranchiseePrice = @newPrice`

### Excel Export Columns Out of Order
**Symptom**: Excel file has columns in wrong sequence, QuickBooks import fails

**Cause**: `[DownloadField]` attributes don't control order, property declaration order does

**Solution**:
- Reorder properties in view model class to match desired Excel column order
- Or implement custom `IExcelFileCreator` that reads `Order` attribute parameter

<!-- END CUSTOM SECTION -->
