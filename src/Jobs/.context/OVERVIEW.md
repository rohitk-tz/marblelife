<!-- AUTO-GENERATED: Header -->
# Jobs Module
> Windows Service for background task scheduling and execution using Quartz.NET
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Jobs module is the **automation engine** of the MarbleLife application - think of it as the "cron daemon" or "task scheduler" that runs in the background orchestrating 50+ recurring jobs without human intervention. It operates as a Windows Service (similar to systemd services on Linux) that wakes up at scheduled intervals to perform critical business operations like sending email notifications, processing uploaded files, generating invoices, syncing data with external APIs, and calculating financial reports.

**Why does this exist?**
Modern business applications need to perform tasks on a schedule rather than in response to user actions. Examples include:
- Sending daily email notifications to thousands of customers
- Processing sales data uploads every Monday morning
- Generating late fee invoices on the 1st of each month
- Polling external APIs for new lead data every 15 minutes
- Cleaning up expired documents once per week

Rather than requiring developers to build custom scheduling logic for each feature, this module provides a **centralized job scheduling infrastructure** powered by Quartz.NET, a mature enterprise scheduling framework. Each business domain (billing, notifications, sales, reporting) registers jobs with the scheduler, which then manages execution, logging, error handling, and retry logic automatically.

**Key Design Philosophy:**
- **Separation of Concerns**: Jobs (when to run) are separate from Polling Agents (what to run)
- **Fail-Safe Execution**: Database sessions are automatically managed; exceptions in one job don't crash the scheduler
- **Environment Flexibility**: Same codebase runs as Windows Service in production or console app for debugging
- **Configuration-Driven**: Changing schedules requires only config updates, no code deployment
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Setup -->
## Setup

### Prerequisites
- .NET Framework 4.5.2 or higher
- Windows operating system (for Windows Service features)
- Quartz.NET 2.4.1
- Access to Core, Infrastructure, and DependencyInjection projects

### Installation as Windows Service

```bash
# Build the project in Release mode
msbuild Jobs.csproj /p:Configuration=Release

# Install using .NET Framework InstallUtil
cd bin\Release
installutil Jobs.exe

# Start the service
sc start Jobs

# Verify service is running
sc query Jobs
```

### Running in Interactive/Debug Mode

Run from Visual Studio or command line - the application detects interactive mode automatically:

```bash
# From bin\Debug directory
Jobs.exe

# Service will start in console mode with verbose logging
# Press Enter to stop
```

### Configuration

Cron expressions are defined in the settings configuration (injected via `ISettings`). Example cron expression formats:

```
"0 0/2 * 1/1 * ? *"     # Every 2 minutes
"0 0 1 * * ?"           # Daily at 1:00 AM
"0 0 8 ? * MON"         # Every Monday at 8:00 AM
"0 15 10 ? * 6#3"       # Third Friday of each month at 10:15 AM
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating a New Scheduled Job

**Step 1**: Create job class inheriting from `BaseJob`

```csharp
using Core.Application;
using Jobs.Impl;
using System;

namespace Jobs.Impl
{
    public class MyNewJob : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        
        public MyNewJob() 
            : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("MyNewJob Constructor - " + _clock.UtcNow);
        }
        
        public override void Execute()
        {
            _logService.Info("MyNewJob started at " + _clock.UtcNow);
            
            try
            {
                // Resolve polling agent from DI container
                var pollingAgent = ApplicationManager.DependencyInjection
                    .Resolve<IMyPollingAgent>();
                
                // Delegate business logic to polling agent
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - MyNewJob. ", e);
            }
            
            _logService.Info("MyNewJob end at " + _clock.UtcNow);
        }
    }
}
```

**Step 2**: Register job in `TaskScheduler.ScheduleJobs()`

```csharp
private void ScheduleJobs()
{
    var dateTimeService = ApplicationManager.DependencyInjection.Resolve<IClock>();
    
    // ... existing job registrations ...
    
    MyNewJobScheduler(dateTimeService);  // Add new job
}

private void MyNewJobScheduler(IClock dateTimeService)
{
    var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
    
    IJobDetail job = JobBuilder.Create<MyNewJob>()
        .WithIdentity("MyNewJob")
        .Build();
    
    ITrigger trigger = TriggerBuilder.Create()
        .ForJob(job)
        .WithCronSchedule(
            settings.MyNewJobCronExpression,  // Add this to ISettings
            x => x.InTimeZone(dateTimeService.CurrentTimeZone))
        .Build();
    
    _logService.Info("Running MyNewJob");
    _scheduler.ScheduleJob(job, trigger);
}
```

**Step 3**: Add cron expression to settings configuration

```csharp
// In your ISettings implementation or configuration file
public string MyNewJobCronExpression => "0 0 2 * * ?";  // Daily at 2:00 AM
```

### Testing a Job in Interactive Mode

Uncomment the job test code in `Program.ExecuteServices()`:

```csharp
private static void ExecuteServices(ILogService logger)
{
    logger.Info("Starting services");
    try
    {
        // Uncomment your job to test it manually
        var myAgent = ApplicationManager.DependencyInjection.Resolve<IMyPollingAgent>();
        myAgent.ProcessRecords();
    }
    catch (Exception exception)
    {
        logger.Error("Starting services", exception);
    }
    
    logger.Info("Press enter key to terminate");
    Console.ReadLine();
}
```

Run the project - it will execute the job once and wait for input.

### Preventing Concurrent Executions

Add `[DisallowConcurrentExecution]` attribute for jobs that must not overlap:

```csharp
using Quartz;

[DisallowConcurrentExecution]
public class MyLongRunningJob : BaseJob
{
    // Quartz.NET ensures this job completes before next trigger fires
}
```

### Using Thread-Local Storage

Store job-specific state using `IAppContextStore`:

```csharp
public override void Execute()
{
    var contextStore = ApplicationManager.DependencyInjection.Resolve<IAppContextStore>();
    
    // Store data
    contextStore.AddItem("ProcessingStartTime", _clock.UtcNow);
    contextStore.AddItem("RecordCount", 0);
    
    // Process records...
    
    // Retrieve data
    var startTime = (DateTime)contextStore.Get("ProcessingStartTime");
    var count = (int)contextStore.Get("RecordCount");
    
    // Clean up
    contextStore.ClearStorage();
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Core Classes

| Class | Description |
|-------|-------------|
| `Program` | Service entry point - handles interactive vs service mode, DI setup, security protocols |
| `Scheduler` | Windows Service wrapper - implements OnStart/OnStop lifecycle events |
| `TaskScheduler` | Quartz.NET orchestrator - registers and manages all scheduled jobs |
| `BaseJob` | Abstract job base class - provides session management and exception handling |
| `AppContextStore` | Thread-local storage for job state - thread-safe dictionary with disposal support |
| `WinJobSessionContext` | Job session context - implements ISessionContext for authentication/user context |

### Key Jobs (by Domain)

#### Notification Jobs
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `EmailNotification` | Every 2 minutes | Polls and sends pending email notifications |
| `WeeklyNotification` | Weekly | Sends weekly summary notifications |
| `NewJobNotification` | Daily | Sends job reminders to clients |
| `DocumentExpiryNotification` | Daily | Alerts about expiring documents |

#### Billing Jobs
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `InvoiceGenerator` | Monthly | Generates franchisee invoices |
| `InvoiceLateFeeGenerator` | Daily | Applies late fees to overdue invoices |
| `PaymentReminder` | Configurable | Sends payment reminder notifications |
| `AttachingInvoices` | Configurable | Attaches invoice documents |

#### Sales & Data Processing
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `SalesDataParser` | Weekly | Parses uploaded sales data files |
| `SalesDataUploadReminder` | Weekly | Reminds franchisees to upload sales data |
| `UpdateGrowthReport` | Daily | Updates growth metrics dashboard |
| `BatchUploadRecord` | Configurable | Processes batch upload records |

#### File Processing
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `CustomerFileParser` | Configurable | Parses customer data files |
| `CalendarFileParser` | Configurable | Processes calendar files |
| `AnnualFileParser` | Annually | Parses annual audit files |
| `ZipParserNotification` | Configurable | Processes ZIP file uploads |

#### Marketing & Leads
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `MarketingLead` | Every 15 min | Fetches marketing lead data |
| `UpdateConvertedLead` | Daily | Updates converted lead status |
| `HomeAdvisorParser` | Configurable | Parses HomeAdvisor lead data |

#### Review & Feedback
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `SendCustomerFeedbackRequest` | Post-service | Sends feedback request emails |
| `GetCustomerFeedbackResponse` | Hourly | Retrieves feedback responses |
| `ReviewPushLocationAPI` | Daily | Syncs reviews to location API |

#### Image & Media
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `BeforeAfterImagesParser` | Configurable | Processes before/after images |
| `BeforeAfterImagesUploadwithS3Bucket` | Daily | Syncs images to S3 |
| `SyncS3BucketInEvery2Min` | Every 2 min | Syncs S3 bucket continuously |

#### Financial
| Job Class | Trigger | Purpose |
|-----------|---------|---------|
| `CurrencyExchangeRateGenerator` | Daily | Updates currency exchange rates |
| `CalculateLoanSchedule` | Monthly | Calculates loan schedules |
| `GetSalesTaxData` | Configurable | Retrieves sales tax data |

### Public Methods

| Method | Description |
|--------|-------------|
| `ITaskScheduler.Run()` | Initializes Quartz.NET scheduler and starts job execution |
| `ITaskScheduler.Stop()` | Gracefully shuts down all scheduled jobs |
| `BaseJob.Execute()` | Abstract method - derived classes implement business logic |
| `IAppContextStore.AddItem(key, item)` | Adds item to thread-local storage |
| `IAppContextStore.Get(key)` | Retrieves item from thread-local storage |
| `IAppContextStore.ClearStorage()` | Clears all stored items, disposing IDisposables |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture Diagram -->
## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Program.Main()                        │
│  - DI Registration (IAppContextStore, ISessionContext)      │
│  - Security Protocol Setup (TLS 1.2)                        │
│  - Mode Detection (Interactive vs Service)                  │
└────────────────────┬────────────────────────────────────────┘
                     │
        ┌────────────┴────────────┐
        │                         │
        ▼                         ▼
┌───────────────┐         ┌──────────────────┐
│  Interactive  │         │  Service Mode    │
│     Mode      │         │  (Production)    │
│               │         │                  │
│ ExecuteServices│        │ ServiceBase.Run()│
│  (Manual Test)│         │      ↓           │
└───────────────┘         │  Scheduler       │
                          │   OnStart()      │
                          └────────┬─────────┘
                                   │
                          ┌────────▼─────────┐
                          │  TaskScheduler   │
                          │     Run()        │
                          └────────┬─────────┘
                                   │
                          ┌────────▼─────────┐
                          │  ScheduleJobs()  │
                          │                  │
                          │  Registers 50+   │
                          │  jobs with       │
                          │  Quartz.NET      │
                          └────────┬─────────┘
                                   │
         ┌─────────────────────────┼─────────────────────────┐
         │                         │                         │
         ▼                         ▼                         ▼
    ┌─────────┐             ┌─────────┐             ┌─────────┐
    │  Job 1  │             │  Job 2  │             │  Job N  │
    │ BaseJob │             │ BaseJob │      ...    │ BaseJob │
    └────┬────┘             └────┬────┘             └────┬────┘
         │                       │                       │
         ▼                       ▼                       ▼
    Session Setup           Session Setup           Session Setup
         │                       │                       │
         ▼                       ▼                       ▼
    Execute()                Execute()                Execute()
    ├─ Resolve Agent        ├─ Resolve Agent        ├─ Resolve Agent
    ├─ Log Start            ├─ Log Start            ├─ Log Start
    ├─ Call Agent           ├─ Call Agent           ├─ Call Agent
    ├─ Catch Errors         ├─ Catch Errors         ├─ Catch Errors
    └─ Log End              └─ Log End              └─ Log End
         │                       │                       │
         ▼                       ▼                       ▼
    Session Dispose         Session Dispose         Session Dispose
```

### Job Execution Flow

```
Quartz Scheduler Trigger Fires
        │
        ▼
┌──────────────────────────────┐
│ BaseJob.Execute(context)     │
│                              │
│ 1. Resolve IUnitOfWork       │
│ 2. Call Setup()              │ ◄── Opens DB session
│                              │
│ 3. try {                     │
│      Execute()               │ ◄── Derived class logic
│    } catch { }               │ ◄── Silent catch (job logs before throwing)
│                              │
│ 4. finally {                 │
│      DisposeSession()        │ ◄── Always closes DB session
│    }                         │
└──────────────────────────────┘
        │
        ▼
┌──────────────────────────────┐
│ Derived Job.Execute()        │
│                              │
│ Log("Job started")           │
│                              │
│ try {                        │
│   agent = Resolve<IAgent>()  │
│   agent.DoWork()             │
│ } catch (Exception e) {      │
│   Log("Error", e)            │ ◄── Must log before returning
│ }                            │
│                              │
│ Log("Job end")               │
└──────────────────────────────┘
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem: Service won't start**
- **Check**: Windows Event Viewer for service startup errors
- **Solution**: Verify all DLL dependencies are in bin directory, ensure config file is valid
- **Solution**: Run `installutil /u Jobs.exe` then reinstall to clear stale registrations

**Problem: Jobs not executing at expected times**
- **Check**: Log files for "Running {JobName}" entries during `ScheduleJobs()`
- **Solution**: Verify cron expression syntax - test at https://www.freeformatter.com/cron-expression-generator-quartz.html
- **Solution**: Check timezone configuration in `IClock.CurrentTimeZone`

**Problem: Job executes but no work happens**
- **Check**: Log for "Exception - {JobName}" error messages
- **Check**: Polling agent is registered in DI container
- **Solution**: Test polling agent directly in `Program.ExecuteServices()` interactive mode
- **Solution**: Verify database connection string and permissions

**Problem: Database connection leaks**
- **Check**: Monitor database connections during job execution
- **Solution**: Ensure polling agents don't manually create `IUnitOfWork` - let `BaseJob` manage sessions
- **Solution**: Verify polling agents don't hold long-lived database connections

**Problem: Jobs running concurrently when they shouldn't**
- **Solution**: Add `[DisallowConcurrentExecution]` attribute to job class
- **Check**: Job execution time - if it exceeds trigger interval, overlaps will occur

**Problem: Service crashes after exception in job**
- **Check**: Derived job has try-catch around polling agent calls
- **Check**: `BaseJob` is not being bypassed (always inherit from `BaseJob`, never implement `IJob` directly)

### Debugging Tips

1. **Enable verbose logging**: Set log level to Debug in configuration to see Quartz.NET internals
2. **Test in interactive mode**: Always test new jobs in console mode before deploying as service
3. **Use breakpoints**: Attach Visual Studio debugger to running service process (`Debug → Attach to Process → Jobs.exe`)
4. **Check database sessions**: Query `sys.dm_exec_sessions` (SQL Server) to verify sessions are closing
5. **Monitor thread pool**: Quartz uses .NET ThreadPool - check thread exhaustion with performance counters
6. **Validate cron expressions**: Small syntax errors (e.g., missing `?` for day-of-week) cause silent failures

### Performance Tuning

- **Reduce job count**: Combine similar jobs with infrequent schedules
- **Increase trigger intervals**: Long-running jobs should have intervals > execution time
- **Optimize polling agents**: Batch operations instead of per-record processing
- **Use async polling agents**: If agents do I/O, make them async to reduce thread blocking
- **Monitor memory**: Each job execution creates garbage - frequent executions can cause GC pressure

### Migration/Upgrade Notes

- **Quartz 3.x Migration**: Upgrading Quartz.NET requires namespace changes (`Quartz.Impl.StdSchedulerFactory` → `Quartz.StdSchedulerFactory`)
- **.NET Core Migration**: Windows Service becomes `BackgroundService`, `ServiceBase` is replaced
- **Dependency Injection Migration**: Consider replacing `ApplicationManager.DependencyInjection` with Microsoft.Extensions.DependencyInjection for .NET Core
<!-- END CUSTOM SECTION -->
