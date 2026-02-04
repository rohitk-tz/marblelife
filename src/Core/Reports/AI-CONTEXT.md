# Core/Reports - AI Context

## Purpose

The **Reports** module provides business intelligence, analytics, and reporting capabilities across all business domains. It generates operational reports, financial summaries, performance metrics, and executive dashboards.

## Key Entities (Domain/)

### Report Definitions
- **Report**: Report metadata and configuration
- **ReportTemplate**: Reusable report templates
- **ReportSchedule**: Automated report generation schedules
- **ReportParameter**: Dynamic report parameters
- **ReportSnapshot**: Saved report instances

### Report Types
- **SalesReport**: Sales performance and pipeline metrics
- **FinancialReport**: Revenue, expenses, and profitability
- **OperationalReport**: Job completion, technician productivity
- **MarketingReport**: Lead sources, conversion rates, ROI
- **CustomerReport**: Customer analytics and retention
- **FranchiseeReport**: Franchisee performance comparisons

### Report Delivery
- **ReportSubscription**: User report subscriptions
- **ReportDistribution**: Email/export delivery settings
- **ReportHistory**: Report generation audit trail
- **ExportFormat**: PDF, Excel, CSV export configurations

## Service Interfaces

### Report Generation
- **IReportFactory**: Report instance creation
- **IReportService**: Report execution and rendering
- **IReportTemplateService**: Template management
- **IReportScheduleService**: Automated report scheduling
- **IReportExportService**: Multi-format export (PDF, Excel, CSV)

### Report Types
- **ISalesReportService**: Sales analytics and forecasting
- **IFinancialReportService**: P&L, revenue, expenses
- **IOperationalReportService**: Efficiency and productivity
- **IMarketingReportService**: Marketing attribution and ROI
- **ICustomerReportService**: Customer lifetime value, retention
- **IFranchiseeReportService**: Multi-franchisee comparisons

### Data Services
- **IReportDataService**: Query optimization and caching
- **IReportAggregationService**: Data aggregation and summarization
- **IReportChartService**: Chart and graph generation
- **IDashboardService**: Real-time dashboard data

## Implementations (Impl/)

Business logic including:
- SQL query optimization for large datasets
- Report caching strategies
- Data aggregation algorithms
- Export format rendering
- Scheduled report automation
- Real-time dashboard updates

## Enumerations (Enum/)

- **ReportType**: Sales, Financial, Operational, Marketing, Customer, Franchisee, Custom
- **ReportFormat**: HTML, PDF, Excel, CSV, JSON
- **ReportPeriod**: Daily, Weekly, Monthly, Quarterly, Yearly, Custom
- **AggregationType**: Sum, Average, Count, Min, Max, Median
- **ChartType**: Line, Bar, Pie, Area, Scatter, Gauge
- **ReportStatus**: Pending, Generating, Completed, Failed, Cancelled

## ViewModels (ViewModel/)

- **ReportViewModel**: Report configuration and data
- **ReportResultViewModel**: Report execution results
- **SalesReportViewModel**: Sales metrics and trends
- **FinancialReportViewModel**: Financial data
- **DashboardViewModel**: Dashboard widget data
- **ChartDataViewModel**: Chart rendering data
- **ReportFilterViewModel**: Dynamic filters

## Business Rules

### Report Access
1. Role-based report access (admin, franchisee, technician)
2. Franchisees see only their data unless corporate admin
3. Sensitive financial reports require elevated permissions
4. Report subscriptions limited by user tier

### Report Generation
1. Large reports run asynchronously
2. Results cached for performance (1 hour default)
3. Real-time reports bypass cache
4. Maximum date range limits to prevent performance issues
5. Automatic data aggregation for large datasets

### Report Scheduling
1. Scheduled reports run during off-peak hours
2. Email delivery with configurable recipients
3. Failed reports retry 3 times before alerting
4. Automatic cleanup of old report snapshots (90 days)

### Data Accuracy
1. All reports use transaction-consistent data
2. Financial reports reconcile with billing system
3. Historical data snapshots for trend analysis
4. Audit trail for all report generations

## Dependencies

- **Core/Sales**: Sales data and pipeline metrics
- **Core/Billing**: Financial and payment data
- **Core/Scheduler**: Job and technician productivity
- **Core/MarketingLead**: Lead and conversion metrics
- **Core/Organizations**: Franchisee data and comparisons
- **Infrastructure**: Database query optimization

## For AI Agents

### Generating Sales Report
```csharp
// Generate monthly sales report
var report = await _salesReportService.Generate(new SalesReportViewModel
{
    FranchiseeId = franchiseeId,
    StartDate = new DateTime(2024, 1, 1),
    EndDate = new DateTime(2024, 1, 31),
    GroupBy = ReportGrouping.Week,
    Metrics = new[]
    {
        ReportMetric.Revenue,
        ReportMetric.LeadCount,
        ReportMetric.ConversionRate,
        ReportMetric.AverageOrderValue
    }
});

// Export to Excel
var excelFile = _reportExportService.ExportToExcel(report);

// Or send via email
_reportDistributionService.SendEmail(report, recipientEmails);
```

### Creating Dashboard
```csharp
// Build real-time dashboard
var dashboard = _dashboardService.BuildDashboard(new DashboardViewModel
{
    FranchiseeId = franchiseeId,
    Widgets = new[]
    {
        new DashboardWidget
        {
            Type = WidgetType.SalesChart,
            Period = ReportPeriod.Monthly,
            ChartType = ChartType.Line
        },
        new DashboardWidget
        {
            Type = WidgetType.LeadFunnel,
            Period = ReportPeriod.Daily
        },
        new DashboardWidget
        {
            Type = WidgetType.RevenueGauge,
            CompareToTarget = true
        },
        new DashboardWidget
        {
            Type = WidgetType.TopCustomers,
            Limit = 10
        }
    }
});

// Dashboard auto-refreshes every 5 minutes
```

### Scheduling Recurring Report
```csharp
// Schedule weekly executive summary
var schedule = _reportScheduleService.Create(new ReportScheduleViewModel
{
    ReportType = ReportType.ExecutiveSummary,
    Frequency = ReportFrequency.Weekly,
    DayOfWeek = DayOfWeek.Monday,
    Time = new TimeSpan(8, 0, 0), // 8 AM
    Recipients = new[] { "ceo@company.com", "operations@company.com" },
    Format = ReportFormat.PDF,
    IncludeCharts = true,
    Parameters = new Dictionary<string, object>
    {
        { "Period", ReportPeriod.Weekly },
        { "IncludeFranchiseeComparison", true }
    }
});
```

### Custom Report with Filters
```csharp
// Generate custom report with dynamic filters
var report = await _reportService.GenerateCustom(new CustomReportViewModel
{
    Name = "High-Value Customer Analysis",
    DataSource = "Customers",
    Filters = new[]
    {
        new ReportFilter
        {
            Field = "TotalRevenue",
            Operator = FilterOperator.GreaterThan,
            Value = 10000
        },
        new ReportFilter
        {
            Field = "LastServiceDate",
            Operator = FilterOperator.Within,
            Value = "90days"
        }
    },
    Columns = new[]
    {
        "CustomerName",
        "TotalRevenue",
        "ServiceCount",
        "AverageOrderValue",
        "LastServiceDate"
    },
    SortBy = "TotalRevenue",
    SortDirection = SortDirection.Descending
});
```

## For Human Developers

### Common Report Operations

#### 1. Financial Reports
```csharp
// P&L Statement
var profitLoss = _financialReportService.GenerateProfitLoss(
    franchiseeId, 
    startDate, 
    endDate
);

// Revenue by service type
var revenueBreakdown = _financialReportService.GetRevenueByServiceType(
    franchiseeId, 
    period
);

// Accounts receivable aging
var arAging = _financialReportService.GetAccountsReceivableAging();
```

#### 2. Operational Reports
```csharp
// Technician productivity
var productivity = _operationalReportService.GetTechnicianProductivity(
    technicianId, 
    startDate, 
    endDate
);

// Job completion rates
var completionRates = _operationalReportService.GetJobCompletionRates(
    franchiseeId, 
    period
);

// Service quality metrics
var qualityMetrics = _operationalReportService.GetServiceQualityMetrics();
```

#### 3. Marketing Analytics
```csharp
// Lead source ROI
var roi = _marketingReportService.GetLeadSourceROI(
    startDate, 
    endDate, 
    includeAllSources: true
);

// Conversion funnel analysis
var funnel = _marketingReportService.GetConversionFunnel(
    franchiseeId, 
    period
);

// Campaign performance
var campaigns = _marketingReportService.GetCampaignPerformance();
```

### Performance Optimization

#### Caching Strategy
```csharp
// Cache frequently accessed reports
[CacheResult(Duration = 3600)] // 1 hour
public ReportResult GetDailySummary(int franchiseeId, DateTime date)
{
    // Expensive query
}

// Invalidate cache on data changes
_cacheService.InvalidateReportsFor(franchiseeId);
```

#### Query Optimization
```csharp
// Use indexed views for common aggregations
// Pre-aggregate data in nightly batch jobs
// Partition large tables by date
// Use materialized views for complex reports
```

### Best Practices
- Cache report results to reduce database load
- Use async/await for long-running report generation
- Implement pagination for large result sets
- Provide export options (PDF, Excel, CSV)
- Include data refresh timestamps
- Implement report permissions at service layer
- Log all report executions for audit
- Optimize queries with proper indexes
- Use stored procedures for complex reports
- Implement background queue for scheduled reports
- Provide drill-down capabilities in dashboards
- Include data quality indicators
- Support multi-currency for financial reports
- Implement report templates for consistency
