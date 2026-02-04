# Core/Dashboard - AI Context

## Purpose

The **Dashboard** module provides aggregated real-time metrics, KPIs, and visualizations for executive and operational dashboards across the MarbleLife platform.

## Key Entities (Domain/)

### Dashboard Components
- **DashboardWidget**: Individual dashboard components
- **DashboardLayout**: User-specific dashboard configurations
- **DashboardMetric**: Calculated KPI values
- **DashboardFilter**: Dashboard-level filters
- **WidgetData**: Cached widget data

### Metrics
- **SalesMetric**: Revenue, pipeline, conversion metrics
- **OperationalMetric**: Job completion, efficiency metrics
- **FinancialMetric**: Profitability, cash flow indicators
- **CustomerMetric**: Satisfaction, retention metrics
- **TechnicianMetric**: Productivity, utilization metrics

## Service Interfaces

### Dashboard Services
- **IDashboardService**: Main dashboard orchestration
- **IDashboardWidgetService**: Widget management
- **IDashboardLayoutService**: Layout customization
- **IDashboardMetricService**: Metric calculation and caching
- **IDashboardRefreshService**: Real-time data updates

### Widget Services
- **ISalesWidgetService**: Sales-related widgets
- **IOperationalWidgetService**: Operational metrics
- **IFinancialWidgetService**: Financial dashboards
- **IMarketingWidgetService**: Marketing analytics
- **ICustomWidgetService**: Custom widget creation

## Implementations (Impl/)

Business logic for:
- Real-time metric calculation
- Dashboard data aggregation
- Widget caching strategies
- Auto-refresh mechanisms
- Multi-user dashboard configurations

## Enumerations (Enum/)

- **WidgetType**: SalesChart, RevenueGauge, LeadFunnel, JobMap, TopCustomers, etc.
- **MetricType**: Count, Sum, Average, Percentage, Ratio
- **DashboardType**: Executive, Operational, Franchisee, Technician
- **RefreshRate**: RealTime, Minute, FiveMinutes, Hour, Daily
- **ChartType**: Line, Bar, Pie, Donut, Gauge, Map, Table

## ViewModels (ViewModel/)

- **DashboardViewModel**: Complete dashboard configuration
- **WidgetViewModel**: Widget configuration and data
- **MetricViewModel**: KPI values with trends
- **DashboardFilterViewModel**: Active filters
- **WidgetDataViewModel**: Widget rendering data

## Business Rules

1. **Performance**: Dashboards cache data based on refresh rate
2. **Access Control**: Users see only authorized data
3. **Real-time Updates**: WebSocket connections for live dashboards
4. **Customization**: Users can customize widget layout
5. **Responsive**: Dashboards adapt to screen size

## Dependencies

- **Core/Reports**: Data aggregation and reporting
- **Core/Sales**: Sales metrics
- **Core/Scheduler**: Operational metrics
- **Core/Billing**: Financial metrics
- **Core/Organizations**: Franchisee filtering

## For AI Agents

### Creating Dashboard
```csharp
// Build executive dashboard
var dashboard = _dashboardService.Build(new DashboardViewModel
{
    Type = DashboardType.Executive,
    Widgets = new[]
    {
        new WidgetViewModel
        {
            Type = WidgetType.RevenueGauge,
            Size = WidgetSize.Medium,
            RefreshRate = RefreshRate.RealTime,
            Position = new Position(0, 0)
        },
        new WidgetViewModel
        {
            Type = WidgetType.SalesTrend,
            Size = WidgetSize.Large,
            Period = Period.Last30Days,
            Position = new Position(1, 0)
        }
    }
});
```

### Real-time Metrics
```csharp
// Subscribe to real-time updates
_dashboardRefreshService.Subscribe(dashboardId, (updatedMetrics) =>
{
    // Push updates to client via SignalR
    Clients.User(userId).Send("DashboardUpdate", updatedMetrics);
});
```

## For Human Developers

- Implement efficient caching for dashboard metrics
- Use WebSockets for real-time updates
- Optimize queries for large datasets
- Support drill-down from dashboard widgets
- Implement role-based widget visibility
