<!-- AUTO-GENERATED: Header -->
# Core.Scheduler Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:15:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Field Service Management** domain. It manages the operational workflow: creating Estimates, converting them to Jobs, scheduling these Jobs on a Calendar, and assigning Technicians.

### Key Entities
-   `Job`: The central work unit. Represents a scheduled visit to a customer.
-   `JobEstimate`: A proposal sent to a customer. Can be converted to a Job.
-   `JobScheduler`: Represents the time slot/appointment on the calendar.
-   `JobDetails`: Specifics about what needs to be done.
-   `TechnicianWorkOrder`: The instruction set for the field service agent.

### Logic Flow
1.  **Estimation**: Sales Rep creates `JobEstimate`.
2.  **Scheduling**: Estimator creates a `Job` and assigns it a timestamp in `JobScheduler`.
3.  **Assignment**: Technicians are assigned.
4.  **Execution**: Work is performed, `TechnicianWorkOrder` is updated.
5.  **Completion**: Job marked complete, likely triggers Invoice generation (in Billing).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `job` (legacy casing): The main entity. Links to `Customer`, `Franchisee`.
-   `JobEstimateServices`: The specific services included in the estimate.
-   `BeforeAfterImages`: Photos taken by technicians (Proof of Work).

### Services
-   `IJobService`: Workflow management (Create, Update Status).
-   `IEstimateService`: Quote capability.
-   `IJobSchedulerService`: Calendar management logic.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Users](../Users/AI-CONTEXT.md)**: Technicians and Sales Reps are Users/Persons.
-   **[Core.Sales](../Sales/AI-CONTEXT.md)**: Jobs are performed for Customers.
-   **[Core.Billing](../Billing/AI-CONTEXT.md)**: Completed jobs become Invoices.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Naming Inconsistency
Note that `job.cs` is lowercase while `JobScheduler.cs` is PascalCase. This suggests different eras of development or a migration from another system.

### Complex Data Model
The relationship between `Job`, `JobScheduler`, and `JobEstimate` is complex. A Job might have multiple Schedule slots (multi-day job?). `JobEstimate` is often the precursor but looks like it can exist independently.
<!-- END CUSTOM SECTION -->
