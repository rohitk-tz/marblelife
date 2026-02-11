<!-- AUTO-GENERATED: Header -->
# Reports/Domain
> Persistent domain entities for report analytics, audit trails, and integration tracking
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Domain subfolder contains the **database schema** for the Reports module. Think of these entities as the **permanent record** of:

1. **Aggregated Analytics** — Pre-calculated monthly sales summaries for fast report generation
2. **Integration State** — Synchronization status with external email APIs
3. **Audit Trails** — Who uploaded what file, when, and what happened
4. **Compliance Tracking** — Which franchisees are late on required data uploads
5. **Notification Deduplication** — Ensuring weekly emails don't send twice

Unlike transactional entities (invoices, payments, customers), these domain models are **analytics-optimized**. They store summaries, metadata, and status flags rather than raw business transactions.

### Key Entities

| Entity | Purpose | Typical Query Pattern |
|--------|---------|----------------------|
| `FranchiseeSalesInfo` | Monthly sales aggregations by franchisee/service/class | Filter by Year + FranchiseeId for growth reports |
| `BatchUploadRecord` | Audit trail for sales data file uploads | Filter by ExpectedUploadDate < UploadedOn for compliance |
| `CustomerEmailAPIRecord` | Email API sync status per customer | Filter by IsSynced = false for retry jobs |
| `PriceEstimateFileUpload` | Price import file audit trail | Filter by StatusId for failed imports |
| `WeeklyNotification` | Deduplication tracker for scheduled emails | Check exists by NotificationDate + Type before sending |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Querying Growth Report Data

```csharp
using Core.Reports.Domain;

public class GrowthReportService
{
    private readonly IRepository<FranchiseeSalesInfo> _salesInfoRepo;
    
    public GrowthReportViewModel GetGrowthMetrics(long franchiseeId, int year)
    {
        // Get current year aggregations
        var currentYearSales = _salesInfoRepo.Table
            .Where(x => x.FranchiseeId == franchiseeId && x.Year == year)
            .GroupBy(x => x.FranchiseeId)
            .Select(g => new
            {
                YTDSales = g.Sum(s => s.SalesAmount),
                AvgMonthlySales = g.Average(s => s.SalesAmount)
            })
            .FirstOrDefault();
        
        // Get previous year for comparison
        var lastYearSales = _salesInfoRepo.Table
            .Where(x => x.FranchiseeId == franchiseeId && x.Year == year - 1)
            .GroupBy(x => x.FranchiseeId)
            .Select(g => new
            {
                YTDSales = g.Sum(s => s.SalesAmount),
                AvgMonthlySales = g.Average(s => s.SalesAmount)
            })
            .FirstOrDefault();
        
        // Calculate growth percentage
        var growth = lastYearSales.YTDSales > 0 
            ? ((currentYearSales.YTDSales - lastYearSales.YTDSales) / lastYearSales.YTDSales) * 100
            : 0;
        
        return new GrowthReportViewModel
        {
            YTDCurrentYear = currentYearSales.YTDSales,
            YTDLastYear = lastYearSales.YTDSales,
            GrowthPercent = growth
        };
    }
}
```

### Example 2: Tracking Batch Upload Compliance

```csharp
using Core.Reports.Domain;

public class ComplianceService
{
    private readonly IRepository<BatchUploadRecord> _batchUploadRepo;
    
    public List<BatchUploadRecord> GetOverdueUploads()
    {
        var now = DateTime.UtcNow;
        
        // Find uploads where expected date has passed but not yet uploaded
        var overdueUploads = _batchUploadRepo.Table
            .Where(x => x.ExpectedUploadDate < now && x.UploadedOn == null)
            .OrderBy(x => x.ExpectedUploadDate)
            .ToList();
        
        return overdueUploads;
    }
    
    public void MarkUploadComplete(long batchRecordId, bool validationPassed)
    {
        var record = _batchUploadRepo.GetById(batchRecordId);
        record.UploadedOn = DateTime.UtcNow;
        record.IsCorrectUploaded = validationPassed;
        
        _batchUploadRepo.Update(record);
    }
}
```

### Example 3: Managing Email API Sync Status

```csharp
using Core.Reports.Domain;

public class EmailSyncService
{
    private readonly IRepository<CustomerEmailAPIRecord> _emailApiRepo;
    
    public void CreatePendingSyncRecord(long customerId, long franchiseeId, string email)
    {
        var record = new CustomerEmailAPIRecord
        {
            CustomerId = customerId,
            FranchiseeId = franchiseeId,
            CustomerEmail = email,
            Status = "Pending",
            IsSynced = false,
            IsFailed = false,
            DateCreated = DateTime.UtcNow
        };
        
        _emailApiRepo.Insert(record);
    }
    
    public void MarkSyncSuccess(long recordId, string apiCustomerId, string apiListId)
    {
        var record = _emailApiRepo.GetById(recordId);
        record.ApiCustomerId = apiCustomerId;
        record.APIListId = apiListId;
        record.Status = "Synced";
        record.IsSynced = true;
        
        _emailApiRepo.Update(record);
    }
    
    public void MarkSyncFailed(long recordId, string errorResponse)
    {
        var record = _emailApiRepo.GetById(recordId);
        record.ErrorResponse = errorResponse;
        record.Status = "Failed";
        record.IsFailed = true;
        
        _emailApiRepo.Update(record);
    }
    
    public List<CustomerEmailAPIRecord> GetFailedSyncs(DateTime since)
    {
        return _emailApiRepo.Table
            .Where(x => x.IsFailed && x.DateCreated >= since)
            .ToList();
    }
}
```

### Example 4: Preventing Duplicate Notifications

```csharp
using Core.Reports.Domain;

public class NotificationService
{
    private readonly IRepository<WeeklyNotification> _weeklyNotificationRepo;
    
    public bool ShouldSendWeeklyReport(DateTime weekEndingDate, long notificationTypeId)
    {
        // Check if notification already sent for this week + type
        var alreadySent = _weeklyNotificationRepo.Table
            .Any(x => x.NotificationDate == weekEndingDate 
                   && x.NotificationTypeId == notificationTypeId);
        
        return !alreadySent;
    }
    
    public void MarkNotificationSent(DateTime weekEndingDate, long notificationTypeId)
    {
        var record = new WeeklyNotification
        {
            NotificationDate = weekEndingDate,
            NotificationTypeId = notificationTypeId
        };
        
        _weeklyNotificationRepo.Insert(record);
    }
    
    // Usage in scheduled job
    public void SendWeeklyARReport()
    {
        var weekEnding = GetLastSaturday();
        var notificationTypeId = (long)NotificationType.WeeklyARReport;
        
        if (ShouldSendWeeklyReport(weekEnding, notificationTypeId))
        {
            // Generate and send report
            GenerateARReport(weekEnding);
            
            // Mark as sent
            MarkNotificationSent(weekEnding, notificationTypeId);
        }
    }
}
```

### Example 5: Aggregating Sales Data (Nightly Job)

```csharp
using Core.Reports.Domain;
using Core.Sales.Domain;

public class SalesAggregationJob
{
    private readonly IRepository<FranchiseeSales> _franchiseeSalesRepo;
    private readonly IRepository<FranchiseeSalesInfo> _salesInfoRepo;
    
    public void AggregateSalesForMonth(int month, int year)
    {
        // Get all sales for the month
        var sales = _franchiseeSalesRepo.Table
            .Where(x => x.Invoice.GeneratedOn.Month == month 
                     && x.Invoice.GeneratedOn.Year == year)
            .ToList();
        
        // Group by franchisee + service + class
        var aggregations = sales
            .GroupBy(x => new 
            { 
                x.FranchiseeId, 
                x.ServiceTypeId, 
                x.ClassTypeId,
                x.Franchisee.Currency
            })
            .Select(g => new FranchiseeSalesInfo
            {
                FranchiseeId = g.Key.FranchiseeId,
                ServiceTypeId = g.Key.ServiceTypeId,
                ClassTypeId = g.Key.ClassTypeId,
                Month = month,
                Year = year,
                SalesAmount = g.Sum(s => s.Amount),  // USD
                AmountInLocalCurrency = g.Sum(s => s.AmountInLocalCurrency),
                UpdatedDate = DateTime.UtcNow
            });
        
        // Upsert aggregations (update if exists, insert if new)
        foreach (var agg in aggregations)
        {
            var existing = _salesInfoRepo.Table
                .FirstOrDefault(x => x.FranchiseeId == agg.FranchiseeId
                                  && x.Month == agg.Month
                                  && x.Year == agg.Year
                                  && x.ServiceTypeId == agg.ServiceTypeId
                                  && x.ClassTypeId == agg.ClassTypeId);
            
            if (existing != null)
            {
                existing.SalesAmount = agg.SalesAmount;
                existing.AmountInLocalCurrency = agg.AmountInLocalCurrency;
                existing.UpdatedDate = DateTime.UtcNow;
                _salesInfoRepo.Update(existing);
            }
            else
            {
                _salesInfoRepo.Insert(agg);
            }
        }
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### FranchiseeSalesInfo Shows Outdated Data
**Symptom**: Growth reports display stale sales figures

**Solution**: Check `UpdatedDate` column — if older than 24 hours, aggregation job failed
- Verify scheduled job is running: `SELECT * FROM ScheduledTasks WHERE Name = 'SalesAggregation'`
- Check job logs for errors (database timeout, memory issues for large datasets)
- Manually trigger aggregation: `AggregateSalesForMonth(currentMonth, currentYear)`

### BatchUploadRecord Shows False Positive Overdue
**Symptom**: Compliance report flags upload as late, but it was uploaded on time

**Solution**: Timezone mismatch between `ExpectedUploadDate` (UTC) and `UploadedOn` (local time)
- Ensure all date columns stored in UTC
- Convert local time to UTC on insert: `UploadedOn = DateTime.UtcNow`
- Add migration to standardize existing data: `UPDATE BatchUploadRecord SET UploadedOn = CONVERT_TZ(UploadedOn, 'Local', 'UTC')`

### CustomerEmailAPIRecord Stuck in "Pending" Status
**Symptom**: Email sync records never transition to "Synced" or "Failed"

**Solution**: Sync job not processing records
- Check if sync job is enabled: `SELECT * FROM ScheduledTasks WHERE Name = 'EmailAPISync'`
- Verify API credentials haven't expired
- Query for error patterns: `SELECT ErrorResponse, COUNT(*) FROM CustomerEmailAPIRecord WHERE Status = 'Failed' GROUP BY ErrorResponse`

### WeeklyNotification Duplicate Constraint Violation
**Symptom**: `INSERT INTO WeeklyNotification` throws unique constraint error

**Solution**: This is expected behavior (prevents duplicate sends)
- Check if notification already sent: `SELECT * FROM WeeklyNotification WHERE NotificationDate = @date AND NotificationTypeId = @type`
- If constraint error occurs, it means notification was already sent (safe to ignore)

<!-- END CUSTOM SECTION -->
