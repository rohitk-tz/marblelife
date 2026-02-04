<!-- AUTO-GENERATED: Header -->
# Jobs Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:30:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is a **Windows Service** handling background tasks, recurring jobs, and long-running processes. It decouples expensive operations (like generating invoices or parsing uploads) from the web request lifecycle.

### Design Patterns
-   **Scheduler Pattern**: Uses **Quartz.NET** to manage job execution schedules.
-   **Windows Service**: Runs as a background system service (`ServiceBase`).
-   **Polymorphic Jobs**: Each job implements the Quartz `IJob` interface (in `Jobs.Impl`).
-   **Configuration-Driven**: Schedule timings (CRON expressions) are injected from `ISettings`, allowing dynamic updates without recompilation.

### Execution Flow
1.  **Startup (`Program.Main`)**:
    -   If `UserInteractive`: Runs `ExecuteServices` (Dev Mode).
    -   If Service Mode: Runs `Scheduler : ServiceBase`.
2.  **Scheduler Initialization (`TaskScheduler.Run`)**:
    -   Creates Quartz `StdSchedulerFactory`.
    -   Calls `ScheduleJobs()` to register all 50+ jobs.
3.  **Job Triggering**:
    -   Quartz triggers a job based on valid CRON expression.
    -   Job executes logic (usually delegates to a Core Service).
    -   LogService records start/failure.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Key Jobs (Examples)

| Job Name | Responsibility | Cron Source |
| :--- | :--- | :--- |
| `EmailNotification` | Sends queued emails from `NotificationQueue`. | `EmailNotificationServiceCronExpression` |
| `InvoiceGenerator` | Generates monthly/recurring invoices. | `InvoiceGenerationServiceCronExpression` |
| `SalesDataParser` | Parses uploaded sales Excel/CSV files. | `SalesDataParserServiceCronExpression` |
| `PaymentReminder` | Sends reminders for overdue payments. | `PaymentReminderCronExpression` |
| `SyncS3bucket` | Syncs images with AWS S3. | `S3BucketSyncIn2Min` |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core](../Core/AI-CONTEXT.md)** - Logic is typically delegated to Core services (e.g., `IEmailNotificationService`).
-   **[DependencyInjection](../DependencyInjection/AI-CONTEXT.md)** - Used to resolve services within the Job context (`WinJobSessionContext`).

### External
-   **Quartz.NET** - Scheduling engine.
-   **Topshelf** (Optional - often used with Quartz but here looks like raw ServiceBase).

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Debugging
-   **User Interactive Mode**: You can run this executable from the command line. It will detect `Environment.UserInteractive` and run `ExecuteServices(logger)`.
-   **Caution**: The `ExecuteServices` method in `Program.cs` currently has **all jobs commented out**. To debug a specific job, you must uncomment it locally.

### Timezones
-   All triggers are configured with `x.InTimeZone(dateTimeService.CurrentTimeZone)`. This ensures jobs run at the correct local time regardless of the server's UTC setting (assuming `dateTimeService` is configured correctly).

### Deployment
-   This is deployed as a Windows Service. Ensure the service account has permission to access network resources (like the DB or S3 keys) if not using Integrated Security.
<!-- END CUSTOM SECTION -->
