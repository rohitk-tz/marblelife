<!-- AUTO-GENERATED: Header -->
# Core.Reports Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:25:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Business Intelligence / Reporting** engine. It aggregates data from Sales, Jobs, and Billing to produce actionable insights and PDF reports for Franchisees and HQ.

### Key Entities
-   `MlfsReport`: Base entity for generated reports.
-   `GrowthReport`: Analysis of franchisee growth over time.
-   `ProductReport`: Sales breakdown by product/service type.
-   `CustomerEmailAPIRecord`: Audit log of emails sent as part of reporting? (Seems like a mix of reporting and notification logging).

### Logic Flow
1.  **Aggregation**: Services query `FranchiseeSales` (Organizations) and `Jobs` (Scheduler).
2.  **Calculation**: Compute KPIs (MoM Growth, Revenue per Service).
3.  **Presentation**: Generates ViewModels for the UI Dashboard (`GrowthReportViewModel`).
4.  **Distribution**: Likely used by `Jobs` to email PDF versions of these reports.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `FranchiseeSalesInfo`: Aggregated sales used for reporting (possibly a snapshot table).
-   `BatchUploadRecord`: Tracks the processing of bulk data uploads for reporting purposes.

### Services
-   `IGrowthReportService`: Growth KPI logic.
-   `IProductReportService`: Product mix analysis.
-   `ICustomerEmailReportService`: Reporting on email campaign effectiveness?

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Sales](../Sales/AI-CONTEXT.md)**: Primary data source (Customers, Sales Data Uploads).
-   **[Core.Organizations](../Organizations/AI-CONTEXT.md)**: Reports are grouped by Franchisee.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Snapshotting
Check if `FranchiseeSalesInfo` is a real-time view or a nightly snapshot.
Financial reporting often requires immutable snapshots to ensure reports don't change if historical data is modified later ("As-Of" reporting).

### Performance
Reporting queries can be heavy. Look for raw SQL or stored procedures in the implementation of these services (e.g., in `Infrastructure`) rather than pure LINQ if performance is an issue.
<!-- END CUSTOM SECTION -->
