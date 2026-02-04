# API/Areas/Reports - AI Context

## Purpose

The **Reports** area provides business intelligence reports, data exports, and analytics across all business domains. It generates formatted reports in multiple formats (PDF, Excel, CSV) for analysis and decision-making.

## Key Functionality

### Report Generation
- On-demand report generation
- Scheduled report delivery
- Multiple output formats (PDF, Excel, CSV)
- Customizable parameters and filters
- Template-based formatting

### Report Categories
- Financial reports (revenue, expenses, profitability)
- Operational reports (jobs, schedules, utilization)
- Sales reports (pipeline, conversion, customer acquisition)
- Performance reports (franchisee comparisons, KPIs)
- Compliance reports (audit trails, regulatory)

### Data Exports
- Bulk data exports
- Custom date ranges
- Filtered exports
- Scheduled exports

### Analytics
- Trend analysis
- Comparative analytics
- Forecasting
- Anomaly detection

## Key Controllers

### ReportController.cs
Primary report generation operations.

**Endpoints**:
- `POST /Reports/Report/Generate` - Generate report
- `GET /Reports/Report/GetAvailable` - Get available reports
- `GET /Reports/Report/Download/{id}` - Download generated report
- `GET /Reports/Report/GetHistory` - Get report generation history
- `POST /Reports/Report/Schedule` - Schedule recurring report

### ExportController.cs
Data export operations.

**Endpoints**:
- `POST /Reports/Export/Customers` - Export customer data
- `POST /Reports/Export/Invoices` - Export invoice data
- `POST /Reports/Export/Jobs` - Export job data
- `POST /Reports/Export/Custom` - Custom data export

## Common Reports

### Financial Reports

#### Revenue Report
- Total revenue by period
- Revenue by franchisee
- Revenue by service type
- Payment method breakdown
- Outstanding receivables

#### Profitability Report
- Revenue vs. expenses
- Profit margins by franchisee
- Profit margins by service
- Cost analysis

### Operational Reports

#### Job Completion Report
- Jobs completed vs. scheduled
- Average completion time
- Job cancellation rate
- Technician productivity

#### Schedule Utilization Report
- Technician utilization rates
- Available capacity
- Peak scheduling times
- Geographic distribution

### Sales Reports

#### Pipeline Report
- Opportunities by stage
- Conversion rates
- Average deal size
- Win/loss analysis

#### Customer Acquisition Report
- New customers by period
- Acquisition cost
- Customer lifetime value
- Retention rates

### Performance Reports

#### Franchisee Performance Report
- Revenue by franchisee
- Job completion metrics
- Customer satisfaction scores
- Growth rates

#### KPI Dashboard Report
- Summary of all key metrics
- Trend indicators
- Targets vs. actuals
- Alert conditions

## Key ViewModels

```csharp
public class ReportRequest
{
    public string ReportType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReportFormat Format { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<long> FranchiseeIds { get; set; }  // Filter by franchisees
}

public class ReportResponse
{
    public long ReportId { get; set; }
    public string ReportName { get; set; }
    public DateTime GeneratedDate { get; set; }
    public string DownloadUrl { get; set; }
    public long FileSizeBytes { get; set; }
}

public enum ReportFormat
{
    PDF = 1,
    Excel = 2,
    CSV = 3,
    JSON = 4
}

public class ScheduledReportConfig
{
    public string ReportType { get; set; }
    public ReportFormat Format { get; set; }
    public RecurrenceType Recurrence { get; set; }
    public List<string> EmailRecipients { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

public enum RecurrenceType
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    Yearly = 5
}
```

## Report Generation Process

### 1. Request Validation
```csharp
private void ValidateReportRequest(ReportRequest request)
{
    if (request.EndDate < request.StartDate)
        throw new ValidationException("End date must be after start date");
    
    if ((request.EndDate - request.StartDate).TotalDays > 365)
        throw new ValidationException("Date range cannot exceed 1 year");
    
    if (!IsAuthorized(request.FranchiseeIds))
        throw new UnauthorizedException("Access denied to selected franchisees");
}
```

### 2. Data Collection
```csharp
private async Task<ReportData> CollectReportData(ReportRequest request)
{
    // Query relevant data based on report type
    var data = await _reportDataService.GetData(
        request.ReportType,
        request.StartDate,
        request.EndDate,
        request.FranchiseeIds,
        request.Parameters
    );
    
    return data;
}
```

### 3. Report Formatting
```csharp
private byte[] FormatReport(ReportData data, ReportFormat format)
{
    switch (format)
    {
        case ReportFormat.PDF:
            return _pdfGenerator.Generate(data);
        
        case ReportFormat.Excel:
            return _excelGenerator.Generate(data);
        
        case ReportFormat.CSV:
            return _csvGenerator.Generate(data);
        
        default:
            throw new NotSupportedException($"Format {format} not supported");
    }
}
```

### 4. Storage and Delivery
```csharp
private async Task<string> StoreAndDeliver(byte[] reportBytes, ReportRequest request)
{
    // Store report file
    var fileId = await _fileService.SaveAsync(reportBytes, $"Report_{DateTime.Now:yyyyMMdd}.{request.Format.ToString().ToLower()}");
    
    // Generate download URL
    var url = $"/Reports/Report/Download/{fileId}";
    
    // Send email if scheduled
    if (request.IsScheduled && request.EmailRecipients.Any())
    {
        await _emailService.SendReportEmail(request.EmailRecipients, url);
    }
    
    return url;
}
```

## Authorization

- **Super Admin**: Generate reports for all franchisees
- **Franchisee Admin**: Generate reports for their franchisee only
- **Read-Only**: Access to all reports
- **Technicians**: Limited access to their performance reports

## Business Rules

- Date ranges limited to prevent performance issues
- Large reports generated asynchronously
- Report files expire after 30 days
- Scheduled reports run during off-peak hours
- Failed report generation retried automatically
- Report access logged for compliance
- PII in reports protected per GDPR/privacy regulations

## Performance Optimization

### Caching
- Cache frequently requested reports
- Pre-aggregate data for common report types
- Use materialized views for complex queries

### Async Generation
```csharp
[HttpPost]
public async Task<IHttpActionResult> GenerateLargeReport([FromBody] ReportRequest request)
{
    // Estimate report size
    var estimatedSize = await EstimateReportSize(request);
    
    if (estimatedSize > LargeSizeThreshold)
    {
        // Queue for async generation
        var jobId = await _reportQueue.Enqueue(request);
        
        return Ok(new 
        { 
            jobId, 
            status = "queued",
            message = "Report generation in progress. You will receive an email when complete."
        });
    }
    
    // Generate synchronously for small reports
    var reportBytes = await GenerateReport(request);
    return Ok(new { data = reportBytes });
}
```

### Database Optimization
- Use read replicas for report queries
- Optimize indexes for common report queries
- Partition large tables by date
- Archive old data

## Export Formats

### PDF
- Professional formatting
- Page headers/footers
- Charts and graphs
- Print-optimized

### Excel
- Multiple worksheets
- Formulas and calculations
- Charts
- Pivot tables
- Conditional formatting

### CSV
- Simple flat file
- Easy import to other systems
- Minimal formatting
- Fast generation

## Testing

```csharp
[Test]
public void GenerateReport_ValidRequest_ReturnsReport()
{
    var request = new ReportRequest
    {
        ReportType = "RevenueReport",
        StartDate = DateTime.Now.AddMonths(-1),
        EndDate = DateTime.Now,
        Format = ReportFormat.PDF,
        FranchiseeIds = new List<long> { 1 }
    };
    
    var response = _reportController.Generate(request).Result;
    
    Assert.IsNotNull(response.DownloadUrl);
    Assert.IsTrue(response.FileSizeBytes > 0);
}

[Test]
public void GenerateReport_UnauthorizedFranchisee_ThrowsException()
{
    SetupSession(RoleType.FranchiseeAdmin, organizationId: 1);
    
    var request = new ReportRequest
    {
        ReportType = "RevenueReport",
        FranchiseeIds = new List<long> { 999 }  // Different franchisee
    };
    
    Assert.Throws<UnauthorizedException>(() => 
        _reportController.Generate(request)
    );
}
```

## Integration Points

- **All Areas**: Data sources for reports
- **Notification**: Email report delivery
- **Application/File**: File storage and downloads
- **Dashboard**: Real-time metrics for reports

## Future Enhancements

- Interactive reports (drill-down capability)
- Report designer for custom reports
- Data visualization library
- Report sharing and collaboration
- Mobile-optimized report viewing
- API for third-party BI tools
