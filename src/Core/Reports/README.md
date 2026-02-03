<!-- AUTO-GENERATED: Header -->
# Core.Reports
> Business Intelligence & Analytics
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module generates the analytic views for the application.
It provides data for the **Dashboard** charts (Growth, Revenue) and generates periodic **PDF Reports**.

### Key Features
-   **Growth Reports**: Month-over-Month revenue tracking.
-   **Product Mix**: Analysis of which services (StoneLife, TileLok) are selling best.
-   **Payroll Reports**: (implied by `IEmailNotificationForPayrollReport`) likely technician commission calculations.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Generating a Report
These are typically read-only operations invoked by the Dashboard controllers.
```csharp
var reportService = IoC.Resolve<IGrowthReportService>();
var data = reportService.GetGrowthData(franchiseeId, year);
```

<!-- END AUTO-GENERATED -->
