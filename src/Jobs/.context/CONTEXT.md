<!-- AUTO-GENERATED: Header -->
# Jobs Module — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2026-02-10T11:16:26Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Jobs module is a Windows Service that orchestrates background task scheduling and execution using Quartz.NET. It manages 50+ scheduled jobs including email notifications, invoice processing, file parsing, data synchronization, and automated reminders. The service operates as a standalone executable that can run both interactively (console mode for debugging) and as a Windows Service in production.

### Design Patterns
- **Service Pattern**: Windows Service architecture with OnStart/OnStop lifecycle management for daemon-style background processing
- **Job Pattern (Quartz.NET)**: Each job implements `IJob` interface through `BaseJob` abstract class, providing standardized execution with automatic session management and exception handling
- **Dependency Injection**: Uses ApplicationManager.DependencyInjection for service resolution, enabling loose coupling and testability
- **Template Method**: `BaseJob` provides session setup/teardown template, concrete jobs implement only `Execute()` business logic
- **Polling Agent Pattern**: Jobs delegate actual work to polling agents (e.g., `INotificationPollingAgent`, `IInvoiceLateFeePollingAgent`) resolved via DI
- **Cron Scheduling**: Time-based execution controlled by cron expressions from `ISettings`, supporting timezone-aware scheduling via `IClock.CurrentTimeZone`

### Data Flow
1. **Service Initialization** (Program.cs) → Sets up dependency injection, security protocol (TLS 1.2), logging, and determines execution mode (interactive vs service)
2. **Scheduler Start** (Scheduler.cs OnStart) → Instantiates `TaskScheduler` and calls `Run()` to begin scheduling
3. **Job Registration** (TaskScheduler.ScheduleJobs) → Each job is registered with Quartz.NET using JobBuilder and TriggerBuilder, bound to cron expressions from configuration
4. **Job Execution Cycle**:
   - Quartz.NET triggers job based on cron schedule
   - `BaseJob.Execute(IJobExecutionContext)` wraps execution: `IUnitOfWork.Setup()` → derived `Execute()` → `IUnitOfWork.Dispose()`
   - Job resolves polling agent from DI container and invokes domain logic
   - Logs success/failure via `ILogService`
5. **Service Shutdown** (Scheduler.cs OnStop) → Calls `TaskScheduler.Stop()` which invokes `_scheduler.Shutdown()`

### Execution Modes
- **Interactive Mode** (`Environment.UserInteractive = true`): Console application for debugging, allows manual job execution via Program.ExecuteServices()
- **Service Mode** (`Environment.UserInteractive = false`): Runs as Windows Service with ServiceBase infrastructure
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Abstractions

```csharp
// ITaskScheduler.cs - Main scheduler contract
public interface ITaskScheduler
{
    string Name { get; }      // Scheduler identifier (typically "TaskScheduler")
    void Run();               // Initialize and start Quartz.NET scheduler
    void Stop();              // Gracefully shutdown all scheduled jobs
}

// BaseJob.cs - Abstract base for all scheduled jobs
public abstract class BaseJob : IJob
{
    // Quartz.NET entry point - handles session lifecycle
    public void Execute(IJobExecutionContext context)
    {
        // 1. Setup database session via IUnitOfWork.Setup()
        // 2. Call derived Execute() with exception handling
        // 3. Dispose session via IUnitOfWork.Dispose() in finally block
    }
    
    // Derived classes implement this with business logic
    public abstract void Execute();
}

// WinJobSessionContext.cs - Job-specific session context
public class WinJobSessionContext : ISessionContext
{
    public string Token { get; set; }                    // Authentication token (if needed)
    public UserSessionModel UserSession { get; set; }    // User context for job execution
}

// AppContextStore.cs - Thread-local storage for job state
public class AppContextStore : IAppContextStore
{
    [ThreadStatic]
    private static Dictionary<string, object> Dictionary;  // Thread-safe per-job storage
    
    void AddItem(string key, object item);      // Add keyed item, throws if duplicate
    void UpdateItem(string key, object item);   // Add or replace item
    object Get(string key);                     // Retrieve item, returns null if missing
    void Remove(string key);                    // Delete item
    void ClearStorage();                        // Clear all items, disposing IDisposable
}
```

### Job Implementation Pattern

```csharp
// Example: EmailNotification.cs
[DisallowConcurrentExecution]  // Prevents overlapping executions (optional)
public class EmailNotification : BaseJob
{
    private readonly ILogService _logService;
    private readonly IClock _clock;

    public EmailNotification() 
        : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
    {
        _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
        _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
        _logService.Info("EmailNotification Constructor- " + _clock.UtcNow);
    }

    public override void Execute()
    {
        _logService.Info("Email notification service started at " + _clock.UtcNow);
        try
        {
            var agent = ApplicationManager.DependencyInjection.Resolve<INotificationPollingAgent>();
            agent.PollForNotifications();
        }
        catch (Exception e)
        {
            _logService.Error("Email notification. " + e);
        }
        _logService.Info("Email notification service end at " + _clock.UtcNow);
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Service Entry Points

#### `Program.Main(string[] args)`
- **Input**: Command-line arguments (unused)
- **Output**: void
- **Behavior**: 
  - Initializes garbage collection, dependency injection, security protocols
  - Detects execution mode via `Environment.UserInteractive`
  - Interactive: calls `ExecuteServices()` for manual testing
  - Service: instantiates `Scheduler` and calls `ServiceBase.Run()`
- **Side Effects**: Registers `IAppContextStore`, `ISessionContext`, sets up logging

#### `Scheduler.OnStart(string[] args)`
- **Input**: Service start arguments
- **Output**: void
- **Behavior**: 
  - Logs start event with timestamp
  - Creates `TaskScheduler` instance and invokes `Run()`
- **Side Effects**: Starts all scheduled jobs

#### `Scheduler.OnStop()`
- **Input**: None
- **Output**: void
- **Behavior**: 
  - Logs stop event with timestamp
  - Calls `TaskScheduler.Stop()` to shutdown scheduler
- **Side Effects**: Gracefully terminates all running jobs

### TaskScheduler Internal Methods

#### `TaskScheduler.Run()`
- **Input**: None
- **Output**: void
- **Behavior**: 
  - Creates `StdSchedulerFactory` to get Quartz.NET scheduler
  - Calls `ScheduleJobs()` to register all jobs
  - Calls `_scheduler.Start()` to begin execution
- **Side Effects**: Initializes Quartz.NET infrastructure

#### `TaskScheduler.ScheduleJobs()`
- **Input**: None
- **Output**: void
- **Behavior**: 
  - Resolves `IClock` for timezone information
  - Calls 50+ private scheduling methods (e.g., `EmailNotification()`, `InvoiceLateFeeGenerator()`)
  - Each method creates `IJobDetail` via `JobBuilder.Create<T>()` and `ITrigger` with cron expression from `ISettings`
  - Registers job+trigger pair via `_scheduler.ScheduleJob(job, trigger)`
- **Side Effects**: Registers all jobs with Quartz.NET scheduler

### Scheduled Job Registration Pattern

Each job follows this pattern (example: `InvoiceLateFeeGenerator`):

```csharp
private void InvoiceLateFeeGenerator(IClock dateTimeService)
{
    var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
    
    // Create job definition
    IJobDetail job = JobBuilder.Create<InvoiceLateFeeGenerator>()
        .WithIdentity("InvoiceLateFeeGenerator")
        .Build();
    
    // Create cron trigger with timezone
    ITrigger trigger = TriggerBuilder.Create()
        .ForJob(job)
        .WithCronSchedule(
            settings.InvoiceLateFeeGeneratorCronExpression,
            x => x.InTimeZone(dateTimeService.CurrentTimeZone))
        .Build();
    
    _logService.Info("Running Invoice LateFee Generator");
    _scheduler.ScheduleJob(job, trigger);
}
```

### Job Lifecycle (BaseJob Pattern)

#### `BaseJob.Execute(IJobExecutionContext context)`
- **Input**: Quartz.NET execution context (contains job details, trigger info)
- **Output**: void
- **Behavior**:
  - Resolves `IUnitOfWork` from DI container
  - Calls `sessionLocator.Setup()` to initialize database session
  - Invokes derived class `Execute()` method
  - Catches all exceptions (silently - logged by derived class)
  - Ensures `DisposeSession()` is called in finally block
- **Side Effects**: Database session lifecycle management

#### `BaseJob.Execute()` (Abstract)
- **Input**: None
- **Output**: void
- **Behavior**: Implemented by each job - typically resolves polling agent and invokes domain logic
- **Side Effects**: Job-specific (email sending, file parsing, invoice generation, etc.)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Scheduled Jobs -->
## Scheduled Jobs Inventory

The module schedules 50+ background jobs organized by domain:

### Notification Jobs
- **EmailNotification**: Polls `INotificationPollingAgent` to send pending email notifications every 2 minutes
- **WeeklyNotification**: Sends weekly summary notifications via `IWeeklyNotificationPollingAgent`
- **DocumentExpiryNotification**: Alerts users about expiring documents via `IDocumentNotificationPollingAgent`
- **NewJobNotification**: Sends new job alerts to clients one day before job time
- **NewJobNotificationOnDay**: Sends job reminders to techs/sales on job day

### Billing/Invoice Jobs
- **InvoiceGenerator**: Generates franchisee invoices via `FranchiseeInvoiceGenerationPollingAgent`
- **InvoiceLateFeeGenerator**: Applies late fees to overdue invoices via `IInvoiceLateFeePollingAgent`
- **PaymentReminder**: Sends payment reminder notifications via `IPaymentReminderPollingAgent`
- **AttachingInvoices**: Attaches invoice documents via `IAttachingInvoicesServices`
- **UpdatingInvoiceIdsNotification**: Updates invoice ID records via `IUpdatingInvoiceIdsNotificationServices`
- **UpdatingInvoiceNotification**: Updates invoice sales data via `IUpdatingInvoiceNotificationServices`
- **EmailNotificationOnFranchiseePriceExceed**: Alerts when franchisee price exceeds threshold by 50%

### Sales & Data Upload Jobs
- **SalesDataParser**: Parses weekly/monthly sales data files via `SalesDataParsePollingAgent`
- **SalesDataUploadReminder**: Reminds franchisees to upload sales data via `ISalesDataUploadReminderPollingAgent`
- **UploadReportNotification**: Sends upload completion notifications via `ISalesDataUploadReportNotificationService`
- **BatchUploadRecord**: Processes batch upload records via `IUpdateBatchUploadRecordService`
- **UpdateGrowthReport**: Updates growth metrics via `IUpdateSalesAmountService`

### File Processing Jobs
- **CustomerFileParser**: Parses customer data files via `ICustomerFileUploadPollingAgent`
- **CalendarFileParser**: Processes calendar files via `CalendarParsePollingAgent`
- **AnnualFileParser**: Parses annual audit files via `AnnualSalesDataParsePollingAgent`
- **InvoiceFileParser**: Updates invoice information via `InvoiceItemUpdateInfoService`
- **ZipParserNotification**: Processes ZIP file uploads via `IZipParserNotificationService`
- **HomeAdvisorParser**: Parses HomeAdvisor lead data via `IHomeAdvisorParser`

### Marketing & Lead Jobs
- **MarketingLead**: Fetches marketing lead data via `IMarketingLeadsService`
- **UpdateConvertedLead**: Updates converted lead status via `IUpdateConvertedLeadsService`
- **UpdateMarketingLeadData**: Updates lead report data via `IUpdateMarketingLeadReportDataService`
- **GetRoutingNumber**: Retrieves routing numbers via `IGetRoutingNumberService`

### Review & Feedback Jobs
- **SendCustomerFeedbackRequest**: Sends feedback request emails via `ISendFeedBackRequestPollingAgent`
- **GetCustomerFeedbackResponse**: Retrieves feedback responses via `IGetCustomerFeedbackService`
- **CustomerFeedbackNotification**: Sends monthly review notifications via `IMonthlyReviewNotificationService`
- **ReviewPushLocationAPI**: Syncs reviews to location API via `IReviewPushLocationAPI`
- **ReviewPushTaazaaFranchiseeMappingParser**: Maps franchisee reviews via Taazaa integration

### Email Integration Jobs
- **CreateEmailRecordOnAPI**: Creates email records for API via `ICreateEmailRecordForApiService`
- **MergeFieldNotification**: Adds merge fields to emails via `ICreateMergeRecordForApiService`
- **SyncedEmailNotification**: Syncs email API integration notifications via `IEmailAPIIntegrationNotificationService`

### Reporting Jobs
- **EmailNotificationForPayrollReport**: Sends payroll report emails
- **EmailNotificationForPhotoReport**: Sends photo report emails
- **ServicedCustomerNotification**: Sends serviced customer list notifications via `ISendCustomerListNotificationService`

### Image & Media Jobs
- **BeforeAfterImagesParser**: Processes before/after images via `IBeforeAfterImagesNotificationServices`
- **BeforeAfterImagesUploadwithS3Bucket**: Syncs images to S3 via `IBeforeAfterImagesUploadwithS3Bucket`
- **SyncS3BucketInEvery2Min**: Syncs S3 bucket every 2 minutes via `IS3BucketSync`
- **CalendarImagesMigrationToNewApplication**: Migrates calendar images via `ICalendarImagesMigration`

### Financial Jobs
- **CurrencyExchangeRateGenerator**: Updates currency exchange rates via `ICurrencyRateService`
- **CalculateLoanSchedule**: Calculates loan schedules via `ICalculateLoanScheduleService`
- **GetSalesTaxData**: Retrieves sales tax data via `ISalesTaxAPIServices`

### Notification Alert Jobs
- **AutoGenereatedMailForBestFit**: Generates best-fit local marketing emails via `IAutoGenereatedMailForBestFitNotification`
- **MailForNonResidentalBuildingType**: Sends emails for non-residential building types via `IMailForNonResidentalBuildingTypeNotification`
- **CancellationMailForTechSalesNotification**: Sends job cancellation emails via `ICancellationMailForTechSalesNotification`
- **NotificationToFA**: Sends notifications to franchise advisors via `INotificationToFA`

### Price Estimation Jobs
- **UploadPriceEstimateData**: Processes price estimate file uploads via `IPriceEstimateParserNotificationService`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **[Core](../../Core/.context/CONTEXT.md)** — Domain models and service interfaces:
  - `Core.Application`: `ILogService`, `IClock`, `ISettings`, `IUnitOfWork`, `ISessionContext`, `IAppContextStore`
  - `Core.Billing`: `IInvoiceLateFeePollingAgent`, `FranchiseeInvoiceGenerationPollingAgent`
  - `Core.Notification`: `INotificationPollingAgent`, `IWeeklyNotificationPollingAgent`, `IPaymentReminderPollingAgent`
  - `Core.Sales`: `ISalesDataUploadReminderPollingAgent`, `SalesDataParsePollingAgent`
  - `Core.Scheduler`: Job scheduling service contracts
  - `Core.Reports`: Report generation services
  - `Core.Review`: Review management services
  - `Core.Organizations`: Organization/franchisee services
  - `Core.MarketingLead`: Marketing lead services
- **[Infrastructure](../../Infrastructure/.context/CONTEXT.md)** — Data access and external integrations
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — IoC container configuration via `DependencyRegistrar`

### External Package Dependencies
- **Quartz.NET 2.4.1** (`Quartz`) — Job scheduling framework providing cron-based triggers, job execution context, and scheduler lifecycle
- **Common.Logging 3.3.1** — Logging abstraction used by Quartz.NET
- **Newtonsoft.Json 12.0.3** — JSON serialization for job configuration and data exchange
- **RestSharp 106.12.0** — HTTP client for external API integrations within polling agents
- **System.ServiceProcess** — Windows Service infrastructure for daemon mode
- **System.Configuration** — Application settings and configuration management

### Configuration Dependencies
- **ISettings Interface**: Provides cron expressions for all jobs (e.g., `EmailNotificationServiceCronExpression`, `InvoiceLateFeeGeneratorCronExpression`)
- **App.config Transforms**: Environment-specific configuration files (Debug, Release, taazaa_qa, new_qa, production) using SlowCheetah transformations
- **Tasks.xml**: Legacy Quartz.NET XML job configuration (currently defines sample `TaskEmailNotification` job)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Configuration -->
## Configuration System

### Cron Expression Configuration
Each job's schedule is controlled by a cron expression retrieved from `ISettings`:

```csharp
// Example cron expressions
settings.EmailNotificationServiceCronExpression      // e.g., "0 0/2 * 1/1 * ? *" (every 2 minutes)
settings.InvoiceLateFeeGeneratorCronExpression      // e.g., "0 0 1 * * ?" (daily at 1 AM)
settings.WeeklyNotificationCronExpression           // e.g., "0 0 8 ? * MON" (Mondays at 8 AM)
```

### Environment-Specific Configuration
The project uses SlowCheetah for XML config transformation:
- **App.Debug.config**: Development settings
- **App.taazaa_qa.config**: QA environment
- **App.new_qa.config**: New QA environment
- **App.production.config**: Production settings
- **App.Release.config**: Release build settings

Configuration transforms override base `App.config` during build based on active configuration.

### Timezone Handling
All cron triggers use `IClock.CurrentTimeZone` to ensure timezone-aware scheduling:

```csharp
.WithCronSchedule(settings.CronExpression, 
    x => x.InTimeZone(dateTimeService.CurrentTimeZone))
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Error Handling -->
## Error Handling Strategy

### Multi-Layer Exception Handling

1. **BaseJob Layer**: Silently catches all exceptions after calling derived `Execute()`, ensuring job crashes don't destabilize scheduler
2. **Derived Job Layer**: Each job has try-catch around polling agent calls, logs exceptions via `ILogService.Error()`, then returns gracefully
3. **Session Disposal**: `finally` block in `BaseJob` ensures `IUnitOfWork.Dispose()` is always called, preventing database connection leaks

### Logging Pattern
All jobs follow this logging pattern:

```csharp
_logService.Info("JobName started at " + _clock.UtcNow);
try {
    // Execute work
} catch (Exception e) {
    _logService.Error("Exception - JobName. ", e);
}
_logService.Info("JobName end at " + _clock.UtcNow);
```

### Concurrency Control
Jobs can use `[DisallowConcurrentExecution]` attribute (Quartz.NET) to prevent overlapping executions:

```csharp
[DisallowConcurrentExecution]  // Email job must complete before next trigger fires
public class EmailNotification : BaseJob { }
```

### Database Session Management
`BaseJob` ensures database sessions are always properly disposed:
- `Setup()` is called before job execution
- `Dispose()` is called in finally block
- Disposal exceptions are caught and logged to prevent cascading failures
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Debugging -->
## Debugging & Testing

### Interactive Mode
Run the service as a console application for debugging:

1. Set `Environment.UserInteractive = true` (automatic when running from IDE)
2. Uncomment desired job in `Program.ExecuteServices()` method
3. Run project - job executes once immediately
4. Press Enter to terminate

### Service Installation
Install as Windows Service:

```bash
installutil Jobs.exe
sc start Jobs
```

### Log Analysis
All jobs log with UTC timestamps via `ILogService`:
- Constructor logs: "JobName Constructor- {timestamp}"
- Execution start: "JobName started at {timestamp}"
- Errors: "Exception - JobName. {exception details}"
- Execution end: "JobName end at {timestamp}"

### Quartz.NET Diagnostics
- `TaskScheduler.ScheduleJobs()` logs: "Running {JobName}" for each registered job
- Service lifecycle logs: "Running TaskScheduler", "Starting job as service", "Stopping task scheduler"
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Key Gotchas
1. **Session Management**: Always let `BaseJob` handle session lifecycle - don't manually create/dispose `IUnitOfWork` in derived jobs
2. **Exception Swallowing**: `BaseJob` silently catches exceptions, so derived jobs MUST log errors before throwing/returning
3. **Cron Timezone**: Forgetting `.InTimeZone()` causes jobs to execute in server local time instead of configured timezone
4. **Concurrent Execution**: Without `[DisallowConcurrentExecution]`, long-running jobs can overlap and cause resource contention
5. **Interactive Mode Testing**: Uncommenting multiple jobs in `ExecuteServices()` runs them sequentially, not concurrently like scheduler

### Extension Points
- Add new job: Create class inheriting `BaseJob`, implement `Execute()`, add scheduling method to `TaskScheduler.ScheduleJobs()`
- Change schedule: Update cron expression in `ISettings` implementation (no code changes needed)
- Add job-specific state: Use `IAppContextStore` for thread-safe storage
- Custom session context: Replace `WinJobSessionContext` registration in `Program.Main()`

### Performance Considerations
- 50+ jobs are registered at startup - startup time is proportional to job count
- Each job execution creates new `IUnitOfWork` session - ensure polling agents are stateless
- Quartz.NET uses thread pool - job count should not exceed available threads to avoid starvation
- `[DisallowConcurrentExecution]` jobs can block future executions if they run longer than trigger interval

### Migration Notes
- `Tasks.xml` is legacy Quartz.NET XML config - new jobs are registered in C# code via `TaskScheduler.ScheduleJobs()`
- Service uses .NET Framework 4.5.2 - upgrade to .NET Core would require refactoring Windows Service infrastructure to use `BackgroundService`
<!-- END CUSTOM SECTION -->
