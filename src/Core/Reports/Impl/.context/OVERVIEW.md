<!-- AUTO-GENERATED: Header -->
# Reports/Impl
> Concrete implementations of report services, factories, and notification automation
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Impl subfolder is the **execution engine** of the Reports module. While interfaces define contracts (what methods exist), implementations define behavior (how methods work). This folder contains 26 classes spanning four categories:

1. **Report Services** — Business logic for generating, filtering, and exporting reports
2. **Factories** — Data transformation between domain entities and view models
3. **Notification Services** — Email automation for scheduled reports and alerts
4. **Data Processing** — ETL operations, file parsing, S3 integration, batch tracking

### Implementation Highlights

| Category | Key Classes | Responsibility |
|----------|-------------|----------------|
| Core Reports | ReportService, GrowthReportService, ProductReportService, CustomerEmailReportService | Query data, apply filters, paginate, export to Excel |
| Factories | ReportFactory, MlfsReportFactory, FranchiseeSalesInfoReportFactory | Transform domain → view model, view model → domain |
| Notifications | EmailNotificationForPayrollReport, MonthlyReviewNotificationService | Generate reports, attach to emails, queue for delivery |
| Data Processing | PriceEstimateFileParser, S3BucketSync, UpdateBatchUploadRecordService | Parse Excel, sync S3, track uploads |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Implementing a New Report Service

```csharp
using Core.Application;
using Core.Application.Attribute;
using Core.Reports;
using Core.Reports.ViewModel;

namespace Core.Reports.Impl
{
    [DefaultImplementation]  // Auto-wire this as IMyReportService implementation
    public class MyReportService : IMyReportService
    {
        private readonly IRepository<MyEntity> _myRepo;
        private readonly IReportFactory _factory;
        private readonly IExcelFileCreator _excelCreator;
        private readonly IClock _clock;
        
        // Inject dependencies via constructor
        public MyReportService(
            IUnitOfWork unitOfWork, 
            IReportFactory factory,
            IExcelFileCreator excelCreator,
            IClock clock)
        {
            _myRepo = unitOfWork.Repository<MyEntity>();
            _factory = factory;
            _excelCreator = excelCreator;
            _clock = clock;
        }
        
        public MyReportListModel GetReport(MyReportFilter filter, int page, int pageSize)
        {
            // 1. Query repository with filters
            var query = _myRepo.Table
                .Where(x => filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                .Where(x => filter.StartDate == null || x.Date >= filter.StartDate);
            
            // 2. Apply sorting
            query = filter.SortBy == "Amount" 
                ? query.OrderByDescending(x => x.Amount) 
                : query.OrderBy(x => x.Date);
            
            // 3. Paginate
            var totalCount = query.Count();
            var pagedData = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // 4. Transform to view models
            var viewModels = pagedData.Select(_factory.CreateViewModel).ToList();
            
            // 5. Return result with paging info
            return new MyReportListModel
            {
                Collection = viewModels,
                Filter = filter,
                PagingModel = new PagingModel(page, pageSize, totalCount)
            };
        }
        
        public bool DownloadReport(MyReportFilter filter, out string fileName)
        {
            // Get unpaginated data
            var data = _myRepo.Table
                .Where(x => filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                .ToList();
            
            var viewModels = data.Select(_factory.CreateViewModel).ToList();
            
            // Generate Excel file
            fileName = $"{MediaLocationHelper.GetTempMediaLocation().Path}/myReport-{_clock.UtcNow:yyyy-MM-dd_HH-mm-ss}.xlsx";
            return _excelCreator.CreateExcelDocument(viewModels, fileName);
        }
    }
}
```

### Example 2: Implementing a Factory

```csharp
using Core.Application.Attribute;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Reports.Domain;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class MyReportFactory : IMyReportFactory
    {
        // Factories typically have no dependencies (pure transformation)
        
        public MyReportViewModel CreateViewModel(MyEntity entity)
        {
            return new MyReportViewModel
            {
                Id = entity.Id,
                Franchisee = entity.Franchisee.Organization.Name,
                Amount = entity.Amount,
                Date = entity.Date,
                FormattedAmount = $"${entity.Amount:N2}",
                Status = entity.IsComplete ? "Complete" : "Pending"
            };
        }
        
        public MyEntity CreateDomain(MyReportSaveModel model)
        {
            return new MyEntity
            {
                FranchiseeId = model.FranchiseeId,
                Amount = model.Amount,
                Date = model.Date,
                IsComplete = false
            };
        }
    }
}
```

### Example 3: Implementing a Notification Service

```csharp
using Core.Application;
using Core.Application.Attribute;
using Core.Reports;
using Core.Notification.Domain;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class MyNotificationService : IMyNotificationService
    {
        private readonly IRepository<MyEntity> _myRepo;
        private readonly IRepository<NotificationQueue> _notificationRepo;
        private readonly IExcelFileCreator _excelCreator;
        private readonly IClock _clock;
        
        public MyNotificationService(
            IUnitOfWork unitOfWork,
            IExcelFileCreator excelCreator,
            IClock clock)
        {
            _myRepo = unitOfWork.Repository<MyEntity>();
            _notificationRepo = unitOfWork.Repository<NotificationQueue>();
            _excelCreator = excelCreator;
            _clock = clock;
        }
        
        public void SendMyReport(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            // 1. Query report data
            var data = _myRepo.Table
                .Where(x => x.FranchiseeId == franchiseeId
                         && x.Date >= startDate
                         && x.Date <= endDate)
                .ToList();
            
            // 2. Generate Excel attachment
            var fileName = $"{MediaLocationHelper.GetTempMediaLocation().Path}/myReport-{franchiseeId}-{_clock.UtcNow:yyyyMMdd}.xlsx";
            var viewModels = data.Select(x => new MyReportViewModel { /* map */ }).ToList();
            _excelCreator.CreateExcelDocument(viewModels, fileName);
            
            // 3. Create notification queue entry
            var notification = new NotificationQueue
            {
                NotificationTypeId = (long)NotificationType.MyReport,
                Subject = $"My Report for {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}",
                Body = $"<p>Please find attached your report for the period.</p>",
                RecipientEmail = GetFranchiseeEmail(franchiseeId),
                AttachmentPath = fileName,
                Status = "Pending",
                CreatedDate = _clock.UtcNow
            };
            
            _notificationRepo.Insert(notification);
            
            // Background processor will send email asynchronously
        }
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Implementation Not Auto-Wired by DI Container
**Symptom**: `InvalidOperationException: Unable to resolve service for type 'IMyService'`

**Solution**: Missing `[DefaultImplementation]` attribute
```csharp
[DefaultImplementation]  // Add this
public class MyService : IMyService { }
```

### Excel Export Throws OutOfMemoryException
**Symptom**: Large reports (50,000+ rows) fail with memory error

**Solution**: Stream Excel generation instead of loading all data into memory
- Use `IExcelFileCreator` implementation with streaming support
- Or batch export into multiple files (e.g., 10,000 rows per file)

### Notification Emails Not Sending
**Symptom**: `NotificationQueue` entries stuck in "Pending" status

**Solution**: Background processor not running
- Verify `NotificationProcessorJob` is scheduled and enabled
- Check processor logs for errors (SMTP connection, invalid recipient)
- Manually trigger: `NotificationProcessor.ProcessQueue()`

### S3 Download Fails with Access Denied
**Symptom**: `AmazonS3Exception: Access Denied`

**Solution**: IAM credentials lack GetObject permission
- Verify IAM role has `s3:GetObject` permission for bucket
- Check bucket policy allows application's IAM role
- Ensure S3 bucket name matches configuration

### ReportService Constructor Injection Failure
**Symptom**: Too many constructor parameters (20+), DI container throws

**Solution**: This indicates God Object anti-pattern
- Refactor into smaller services (see Developer Insights section)
- Or use Service Locator pattern (not recommended)

<!-- END CUSTOM SECTION -->
