# API/Areas/Dashboard - AI Context

## Purpose

The **Dashboard** area provides aggregated metrics, KPIs, and visualization data for business intelligence dashboards. It exposes endpoints that return summary statistics, trends, and performance indicators for franchisees and corporate management.

## Key Functionality

### Business Metrics
- Revenue trends and forecasts
- Job completion rates
- Customer acquisition metrics
- Technician utilization rates
- Invoice payment status
- Lead conversion rates

### Visualizations
- Charts and graphs data
- Trend analysis
- Comparative metrics (YoY, MoM)
- Geographic performance maps
- Pipeline visualizations

### Real-Time Data
- Active jobs count
- Today's revenue
- Pending invoices
- Available technicians
- Recent customer reviews

## Key Controller

### DashboardController.cs
Primary dashboard data provider.

**Endpoints**:
- `GET /Dashboard/Dashboard/GetSummary` - Get dashboard summary with all KPIs
- `GET /Dashboard/Dashboard/GetRevenueData` - Get revenue trends
- `GET /Dashboard/Dashboard/GetJobStats` - Get job statistics
- `GET /Dashboard/Dashboard/GetCustomerMetrics` - Get customer acquisition data
- `GET /Dashboard/Dashboard/GetPerformanceByFranchisee` - Get franchisee comparisons

## Key ViewModels

```csharp
public class DashboardSummaryViewModel
{
    // Today's Metrics
    public decimal TodayRevenue { get; set; }
    public int TodayJobs { get; set; }
    public int TodayNewCustomers { get; set; }
    
    // Month to Date
    public decimal MonthRevenue { get; set; }
    public int MonthJobs { get; set; }
    public decimal MonthGrowthPercentage { get; set; }
    
    // Year to Date
    public decimal YearRevenue { get; set; }
    public int YearJobs { get; set; }
    
    // Pipeline
    public decimal PipelineValue { get; set; }
    public int ActiveOpportunities { get; set; }
    
    // Outstanding
    public decimal OutstandingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
}

public class RevenueTrendViewModel
{
    public List<DataPoint> MonthlyRevenue { get; set; }
    public List<DataPoint> Forecast { get; set; }
    public decimal GrowthRate { get; set; }
}

public class DataPoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Label { get; set; }
}
```

## Authorization

- **Super Admin**: See all franchisee aggregated data
- **Franchisee Admin**: See only their franchisee's data
- **Technicians**: Limited view of their performance metrics
- **Read-Only**: Full dashboard access without modification rights

## Business Rules

- Data aggregated by franchisee, date range, service type
- Real-time data refreshed every 5 minutes
- Historical data cached for performance
- Comparisons always use same date ranges (YoY, MoM)
- Currency formatted based on franchisee settings

## Performance Considerations

- Heavy use of caching for historical data
- Aggregations pre-calculated in background jobs
- Minimal joins in dashboard queries
- Use of materialized views for complex metrics
- Real-time data from in-memory cache

## Related Areas

- **Sales**: Revenue and customer metrics
- **Scheduler**: Job statistics
- **Organizations**: Franchisee comparisons
- **Reports**: Detailed report generation
