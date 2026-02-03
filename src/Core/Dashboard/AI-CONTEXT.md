<!-- AUTO-GENERATED: Header -->
# Core.Dashboard Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:45:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is a **Presentation Support** domain.
It aggregates data specifically for the **Web Dashboard** tiles and widgets. It wraps data from other domains into UI-friendly ViewModels.

### Key Logic
-   **DashboardService**: The aggregator. It calls `Sales`, `Billing`, `Jobs` to get counts (e.g., "5 New Leads", "3 Overdue Invoices").

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Sales](../Sales/AI-CONTEXT.md)**
-   **[Core.Scheduler](../Scheduler/AI-CONTEXT.md)**
-   **[Core.Billing](../Billing/AI-CONTEXT.md)**

<!-- END AUTO-GENERATED -->
