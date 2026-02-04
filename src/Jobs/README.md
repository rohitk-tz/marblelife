<!-- AUTO-GENERATED: Header -->
# Jobs Layer
> Background Worker and Scheduler
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Jobs console application is responsible for executing background tasks. It uses **Quartz.NET** to schedule jobs based on CRON expressions defined in the database settings.

Examples of Background Tasks:
-   Sending Emails
-   Generating Invoices
-   Parsing Excel Uploads
-   Syncing Data with 3rd Party APIs (ReviewPush, Authorize.Net)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Development (Running Locally)

1.  Open `Program.cs`.
2.  Uncomment the specific job you want to test in `ExecuteServices()`.
    ```csharp
    // var notificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<INotificationPollingAgent>();
    // notificationPollingAgent.PollForNotifications(); <-- Uncomment this
    ```
3.  Run the project as a **Console Application**.
4.  The job will execute immediately once.

### Production (Windows Service)

1.  Build the project (`Release` mode).
2.  Install using `installutil.exe` or `sc create`.
3.  The service runs `Scheduler.OnStart`, which initializes the Quartz Scheduler.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Troubleshooting -->
## 🔧 Troubleshooting

### "Job Not Running"
1.  Check `SystemAuditRecord` or Logs.
2.  Verify the **CRON Expression** in the Settings table.
3.  Ensure the `Jobs` service is actually `Running` in Windows Services.
4.  Check if the server time matches the expected TimeZone logic.

### "Job Crashing"
-   Exceptions in `Program.Main` are caught and logged. Check the `LogService` output (file or DB).
-   If a job fails, Quartz typically retries or logs the error depending on the configuration.
<!-- END AUTO-GENERATED -->
