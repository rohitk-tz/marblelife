# Jobs Module - AI Context

## Purpose

The **Jobs** module is the background processing and scheduled task execution layer of the MarbleLife application. It provides a Windows Service-based infrastructure for running automated, time-triggered business operations including notifications, data parsing, report generation, invoice processing, and external system integrations. Built on **Quartz.NET** scheduler, it ensures reliable, configurable, and monitored execution of 47+ scheduled jobs.

## Architecture Overview

The Jobs module implements a **Windows Service** hosting **Quartz.NET** scheduler with a **plugin-based job architecture**. Each job is an independent, self-contained unit that can be scheduled with cron expressions and executed with full dependency injection, logging, and error handling support.

### Key Architectural Patterns

1. **Windows Service Pattern**: Long-running background process (ServiceBase)
2. **Scheduler Pattern**: Quartz.NET job scheduling with cron triggers
3. **Template Method Pattern**: BaseJob abstract class with hook methods
4. **Dependency Injection**: Constructor injection for all job dependencies
5. **Unit of Work Pattern**: Database session management per job execution
6. **Service Locator Pattern**: ApplicationManager for DI container access

## Project Structure

```
Jobs/
├── Impl/                   # Job implementations (47+ scheduled jobs)
│   ├── Notification/       # Email & notification jobs
│   ├── DataParsing/        # File parsing & import jobs
│   ├── Financial/          # Invoice, billing, payment jobs
│   ├── Reports/            # Report generation jobs
│   └── Integration/        # External API integration jobs
├── Templates/              # HTML templates for email/PDF generation
│   ├── before-after-best-pair.cshtml
│   ├── before-after-pair.cshtml
│   ├── customer_invoice.cshtml
│   ├── invoice-job-attachment.cshtml
│   ├── local-office-graph.cshtml
│   └── photo-report.cshtml
├── ApplicationManager.cs   # DI container initialization
├── BaseJob.cs             # Abstract base class for all jobs
├── ITaskScheduler.cs      # Scheduler interface
├── TaskScheduler.cs       # Quartz.NET scheduler implementation
├── Scheduler.cs           # Windows Service entry point
├── Program.cs             # Console/Service launcher
└── Properties/            # Assembly metadata
```

## Windows Service Architecture

### Scheduler.cs - Main Service Class

**Purpose**: Entry point for Windows Service hosting the job scheduler.

**Key Components**:
- Inherits from `ServiceBase` (System.ServiceProcess)
- Marked with `[RunInstaller(true)]` for installutil.exe support
- Dual-mode execution: Service mode (production) or Console mode (debugging)

**Service Lifecycle**:
```csharp
OnStart() → Initializes ApplicationManager (DI)
          → Creates TaskScheduler instance
          → Calls taskScheduler.Run()
          → Logs "Service started successfully"

OnStop()  → Calls taskScheduler.Stop()
          → Logs "Service stopped successfully"
```

**Execution Modes**:
```csharp
// Console Mode (for debugging)
if (Environment.UserInteractive)
{
    scheduler.OnStartPublic(args);
    Console.ReadLine();
    scheduler.OnStopPublic();
}

// Service Mode (production)
else
{
    ServiceBase.Run(new Scheduler());
}
```

### TaskScheduler.cs - Quartz.NET Integration

**Purpose**: Manages job scheduling, registration, and execution using Quartz.NET.

**Initialization**:
```csharp
public void Run()
{
    // Create scheduler factory
    ISchedulerFactory schedFact = new StdSchedulerFactory();
    _scheduler = schedFact.GetScheduler();
    
    // Start scheduler
    _scheduler.Start();
    
    // Register all jobs
    ScheduleJobs();
}
```

**Job Registration Pattern**:
```csharp
private void ScheduleJobs()
{
    // Example: Email Notification Job
    IJobDetail emailJob = JobBuilder
        .Create<EmailNotification>()
        .WithIdentity("EmailNotification")
        .Build();
        
    ITrigger emailTrigger = TriggerBuilder.Create()
        .ForJob(emailJob)
        .WithCronSchedule(
            _settings.EmailNotificationServiceCronExpression,
            x => x.InTimeZone(_clock.CurrentTimeZone)
        )
        .Build();
        
    _scheduler.ScheduleJob(emailJob, emailTrigger);
    
    // Repeat for all 47+ jobs...
}
```

**Graceful Shutdown**:
```csharp
public void Stop()
{
    if (_scheduler != null && !_scheduler.IsShutdown)
    {
        _scheduler.Shutdown(waitForJobsToComplete: true);
    }
}
```

### BaseJob.cs - Abstract Job Template

**Purpose**: Provides common infrastructure for all job implementations (error handling, logging, session management).

**Key Features**:
- Implements `Quartz.IJob` interface
- Manages `IUnitOfWork` lifecycle (automatic disposal)
- Wraps execution in try-catch-finally
- Logs start/end timestamps
- Provides abstract `Execute()` method for subclasses

**Template Method**:
```csharp
public abstract class BaseJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        IUnitOfWork unitOfWork = null;
        
        try
        {
            _log.Info($"{GetType().Name} started at {_clock.Now}");
            
            unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            unitOfWork.Setup();
            
            // Call subclass implementation
            Execute();
            
            _log.Info($"{GetType().Name} completed at {_clock.Now}");
        }
        catch (Exception ex)
        {
            _log.Error($"{GetType().Name} failed", ex);
        }
        finally
        {
            unitOfWork?.Cleanup();
            unitOfWork?.Dispose();
        }
        
        return Task.CompletedTask;
    }
    
    protected abstract void Execute();
}
```

## Job Categories & Implementations

### 1. Notification & Communication Jobs

**EmailNotification**
- **Purpose**: Sends pending email notifications from queue
- **Schedule**: Every 5 minutes (configurable)
- **Operations**: Processes EmailQueue table, sends via SMTP, marks as sent

**WeeklyNotification**
- **Purpose**: Sends weekly digest emails to franchisees
- **Schedule**: Weekly (Sunday 6 AM)
- **Operations**: Aggregates weekly metrics, generates HTML email, sends to franchisees

**CustomerFeedbackNotificationService**
- **Purpose**: Sends review request emails to customers after job completion
- **Schedule**: Daily at 9 AM
- **Operations**: Finds completed jobs without feedback requests, sends email, logs request

**PaymentReminderService**
- **Purpose**: Sends overdue invoice reminders
- **Schedule**: Daily at 8 AM
- **Operations**: Finds overdue invoices, sends reminder emails with payment links

**DocumentExpiryNotificationService**
- **Purpose**: Alerts franchisees about expiring documents (insurance, licenses)
- **Schedule**: Daily at 7 AM
- **Operations**: Checks document expiration dates, sends alerts 30/15/7 days before expiry

### 2. Data Processing & Parsing Jobs

**SalesDataParserService**
- **Purpose**: Parses uploaded sales data files (CSV/Excel)
- **Schedule**: Every 15 minutes
- **Operations**: Reads upload folder, parses files, creates customer/job records, archives files

**InvoiceFileParserService**
- **Purpose**: Imports invoice data from external accounting systems
- **Schedule**: Hourly
- **Operations**: Parses invoice files, creates/updates invoices, reconciles payments

**CalendarFileParserService**
- **Purpose**: Imports calendar events from ICS files
- **Schedule**: Every 30 minutes
- **Operations**: Parses ICS files, creates job/meeting records, assigns to technicians

**MarketingLeadDataUpdateService**
- **Purpose**: Syncs marketing lead data from external CRM systems
- **Schedule**: Every 15 minutes
- **Operations**: Calls external API, updates MarketingLead table, tracks conversion status

**ZipFileParserService**
- **Purpose**: Extracts and processes compressed archive files
- **Schedule**: Hourly
- **Operations**: Unzips uploaded files, routes contents to appropriate parsers

### 3. Financial & Billing Jobs

**InvoiceGeneratorService**
- **Purpose**: Generates invoices for completed jobs
- **Schedule**: Daily at midnight
- **Operations**: Finds uninvoiced jobs, calculates totals, creates Invoice records, sends email

**InvoiceLateFeeGeneratorService**
- **Purpose**: Applies late fees to overdue invoices
- **Schedule**: Daily at 1 AM
- **Operations**: Finds invoices past due date, calculates late fees, creates LateFee records

**LoanScheduleCalculationService**
- **Purpose**: Calculates monthly loan payment schedules
- **Schedule**: Weekly (Monday 3 AM)
- **Operations**: Processes loan records, generates payment schedule, updates balances

**CurrencyExchangeRateGeneratorService**
- **Purpose**: Fetches current currency exchange rates
- **Schedule**: Daily at 2 AM
- **Operations**: Calls currency API, stores rates in CurrencyRate table, archives historical data

**SalesTaxAPIService**
- **Purpose**: Updates sales tax rates from external API
- **Schedule**: Weekly (Monday 5 AM)
- **Operations**: Calls TaxJar/Avalara API, updates tax rates by jurisdiction

**PriceEstimateDataUploadService**
- **Purpose**: Uploads pricing data to external estimating system
- **Schedule**: Daily at 3 AM
- **Operations**: Exports pricing data, uploads via FTP/API, logs results

### 4. Report Generation Jobs

**GrowthReportUpdateService**
- **Purpose**: Generates franchise growth metrics
- **Schedule**: Daily at 4 AM
- **Operations**: Calculates revenue growth, customer acquisition, job completion rates

**PhotoReportService**
- **Purpose**: Generates before/after photo reports for jobs
- **Schedule**: Daily at 5 AM
- **Operations**: Finds jobs with photos, generates PDF reports using Razor templates, emails to customers

**PayrollReportService**
- **Purpose**: Generates technician payroll reports
- **Schedule**: Weekly (Friday 6 AM)
- **Operations**: Calculates hours worked, job commissions, generates CSV for payroll system

**BatchUploadRecordsService**
- **Purpose**: Processes batch import records and generates summary reports
- **Schedule**: Hourly
- **Operations**: Validates import data, creates records, generates success/error reports

**UploadReportNotificationService**
- **Purpose**: Sends upload status notifications to users
- **Schedule**: Every 30 minutes
- **Operations**: Checks upload status, sends email with results, attaches error logs

### 5. External Integration Jobs

**ReviewPushAPIService**
- **Purpose**: Pushes customer reviews to external platforms (Google, Yelp)
- **Schedule**: Every 2 hours
- **Operations**: Finds pending reviews, calls review platform APIs, updates status

**HomeAdvisorParserService**
- **Purpose**: Imports leads from HomeAdvisor
- **Schedule**: Every 15 minutes
- **Operations**: Calls HomeAdvisor API, creates MarketingLead records, assigns to franchisees

**RoutingNumberRetrievalService**
- **Purpose**: Validates bank routing numbers via external API
- **Schedule**: On-demand (triggered by payment setup)
- **Operations**: Calls Federal Reserve routing API, validates bank information

**EmailRecordCreationForAPIsService**
- **Purpose**: Creates email records from API submissions
- **Schedule**: Every 10 minutes
- **Operations**: Processes API email queue, creates EmailQueue records, logs results

**CustomerFeedbackService**
- **Purpose**: Syncs customer feedback from survey platforms
- **Schedule**: Hourly
- **Operations**: Calls survey API (SurveyMonkey, etc.), imports responses, updates customer records

### 6. Maintenance & Cleanup Jobs

**BeforeAfterImageS3BucketSyncService**
- **Purpose**: Synchronizes before/after photos to AWS S3
- **Schedule**: Daily at 2 AM
- **Operations**: Uploads local photos to S3, updates image URLs, archives old files

**AnnualFileCleanupService**
- **Purpose**: Archives old files and purges temporary data
- **Schedule**: Yearly (January 1st)
- **Operations**: Moves old records to archive tables, deletes temp files, vacuums database

## HTML Templates (Templates/)

### Purpose
Razor templates for generating dynamic HTML content used in emails, PDFs, and reports.

### Available Templates

**customer_invoice.cshtml**
- **Purpose**: Customer invoice PDF generation
- **Model**: InvoiceViewModel with customer, job, line items, payment details
- **Output**: Professional invoice with logo, itemized charges, payment instructions

**invoice-job-attachment.cshtml**
- **Purpose**: Email attachment for invoice notifications
- **Model**: Invoice + Job details
- **Output**: HTML email body with invoice summary and payment link

**photo-report.cshtml**
- **Purpose**: Before/after photo comparison report
- **Model**: Job with photo URLs
- **Output**: Side-by-side before/after images with job details

**before-after-best-pair.cshtml**
- **Purpose**: Best quality before/after image selection
- **Model**: Multiple photo pairs with quality scores
- **Output**: Highlights best photo pair for marketing use

**before-after-pair.cshtml**
- **Purpose**: Standard before/after photo comparison
- **Model**: PhotoPairViewModel
- **Output**: Simple side-by-side image comparison

**local-office-graph.cshtml**
- **Purpose**: Franchise performance dashboard
- **Model**: FranchiseeMetrics with charts data
- **Output**: HTML charts showing revenue, jobs, customer satisfaction

### Template Usage Pattern
```csharp
var templateService = new RazorTemplateService();
var model = new InvoiceViewModel { /* populate */ };
string html = templateService.Render("customer_invoice.cshtml", model);
byte[] pdf = pdfGenerator.Generate(html);
```

## Configuration

### App.config Settings

```xml
<appSettings>
  <!-- Cron Expressions (Quartz.NET format) -->
  <add key="EmailNotificationServiceCronExpression" value="0 0/5 * * * ?" /> <!-- Every 5 min -->
  <add key="InvoiceGeneratorServiceCronExpression" value="0 0 0 * * ?" />    <!-- Daily midnight -->
  <add key="WeeklyNotificationCronExpression" value="0 0 6 ? * SUN" />        <!-- Sunday 6 AM -->
  
  <!-- Job-Specific Settings -->
  <add key="EmailBatchSize" value="50" />
  <add key="InvoiceReminderDays" value="7,14,30" />
  <add key="PhotoReportBucketName" value="marblelife-photos" />
  
  <!-- External API Credentials -->
  <add key="HomeAdvisorApiKey" value="..." />
  <add key="TaxJarApiToken" value="..." />
</appSettings>
```

### Cron Expression Reference
- `0 0/5 * * * ?` - Every 5 minutes
- `0 0 * * * ?` - Every hour
- `0 0 0 * * ?` - Daily at midnight
- `0 0 6 ? * MON-FRI` - Weekdays at 6 AM
- `0 0 6 ? * SUN` - Sundays at 6 AM

## Dependency Injection Setup

### ApplicationManager.cs

**Purpose**: Initializes DI container for Windows Service context.

**Initialization**:
```csharp
public static class ApplicationManager
{
    public static IDependencyInjection DependencyInjection { get; private set; }
    
    public static void Init()
    {
        DependencyRegistrar.RegisterDependencies();
        DependencyRegistrar.SetupCurrentContextWinJob();
        DependencyInjection = new DependencyInjection();
    }
}
```

**Key Registrations**:
- `ILogService` - NLog logger
- `IClock` - System time provider
- `ISettings` - Configuration accessor
- `IUnitOfWork` - Database context
- All job implementations (auto-discovered via `[DefaultImplementation]`)
- Polling agents for data parsing

## For AI Agents

### Adding New Scheduled Job

1. **Create Job Class**:
```csharp
[DisallowConcurrentExecution] // Prevents overlapping executions
[DefaultImplementation]
public class NewCustomJob : BaseJob
{
    private readonly ICustomService _service;
    
    public NewCustomJob(ICustomService service)
    {
        _service = service;
    }
    
    protected override void Execute()
    {
        // Job logic here
        _log.Info("Processing custom task");
        _service.DoWork();
    }
}
```

2. **Register in TaskScheduler.ScheduleJobs()**:
```csharp
IJobDetail newJob = JobBuilder
    .Create<NewCustomJob>()
    .WithIdentity("NewCustomJob")
    .Build();
    
ITrigger newTrigger = TriggerBuilder.Create()
    .ForJob(newJob)
    .WithCronSchedule(
        _settings.NewCustomJobCronExpression,
        x => x.InTimeZone(_clock.CurrentTimeZone)
    )
    .Build();
    
_scheduler.ScheduleJob(newJob, newTrigger);
```

3. **Add Configuration**:
```xml
<add key="NewCustomJobCronExpression" value="0 0 * * * ?" />
```

4. **Test in Console Mode**:
```bash
Jobs.exe
# Runs in interactive mode for debugging
```

### Modifying Existing Job

1. Locate job class in `Impl/` folder
2. Update `Execute()` method
3. Add dependencies via constructor injection
4. Test changes in Console mode
5. Update cron expression if schedule changes

### Debugging Jobs

**Console Mode**:
```csharp
// In Program.cs, service runs in console when interactive
if (Environment.UserInteractive)
{
    // Jobs execute normally, output visible in console
    // Press Enter to stop
}
```

**Logging**:
- All jobs log to NLog (console + file)
- Check `Logs/` folder for execution history
- Each job logs start/end times and errors

## For Human Developers

### Job Development Best Practices

#### Error Handling
```csharp
protected override void Execute()
{
    try
    {
        // Job logic
        var data = _service.GetData();
        ProcessData(data);
    }
    catch (Exception ex)
    {
        // BaseJob already logs, but you can add specific handling
        _log.Error($"Failed to process data: {ex.Message}", ex);
        
        // Send alert notification
        _notificationService.SendAlert("Job failed", ex.Message);
        
        // Don't re-throw - BaseJob handles it
    }
}
```

#### Database Operations
```csharp
protected override void Execute()
{
    // UnitOfWork already set up by BaseJob
    var repo = _unitOfWork.Repository<Customer>();
    
    var customers = repo.Table
        .Where(c => c.LastContactDate < DateTime.Now.AddDays(-30))
        .ToList();
    
    foreach (var customer in customers)
    {
        SendFollowUpEmail(customer);
        customer.LastContactDate = DateTime.Now;
        repo.Update(customer);
    }
    
    _unitOfWork.SaveChanges(); // Commit all changes
}
```

#### External API Calls
```csharp
protected override void Execute()
{
    try
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://api.example.com/data");
        
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            ProcessApiData(data);
        }
        else
        {
            _log.Warn($"API returned {response.StatusCode}");
        }
    }
    catch (HttpRequestException ex)
    {
        _log.Error("API call failed", ex);
        // Job will retry on next scheduled execution
    }
}
```

#### Performance Optimization
```csharp
protected override void Execute()
{
    // Batch processing for large datasets
    const int batchSize = 100;
    var totalRecords = _repo.Count(x => x.NeedsProcessing);
    var totalBatches = (totalRecords + batchSize - 1) / batchSize;
    
    for (int batch = 0; batch < totalBatches; batch++)
    {
        var records = _repo.Table
            .Where(x => x.NeedsProcessing)
            .OrderBy(x => x.Id)
            .Skip(batch * batchSize)
            .Take(batchSize)
            .ToList();
        
        ProcessBatch(records);
        
        _unitOfWork.SaveChanges(); // Commit each batch
        _log.Info($"Processed batch {batch + 1}/{totalBatches}");
    }
}
```

### Windows Service Installation

```bash
# Install service
installutil Jobs.exe

# Start service
net start MarbleLifeJobsService

# Stop service
net stop MarbleLifeJobsService

# Uninstall service
installutil /u Jobs.exe
```

### Monitoring Jobs

- **Event Viewer**: Windows Application logs contain service start/stop events
- **NLog Files**: Check `Logs/` folder for detailed execution logs
- **Database Monitoring**: Query `JobExecutionLog` table for job history
- **Performance Counters**: Monitor memory/CPU usage via Task Manager

## Troubleshooting

### Common Issues

**Job Not Executing**
- Check cron expression syntax in App.config
- Verify job is registered in `TaskScheduler.ScheduleJobs()`
- Check Windows Service status (running?)
- Review NLog files for exceptions

**Service Won't Start**
- Check database connection string
- Verify all dependencies are registered in DI
- Review Windows Event Viewer for startup errors
- Ensure service account has proper permissions

**Memory Leaks**
- Verify `IUnitOfWork` is disposed (BaseJob handles this)
- Check for event handler subscriptions (must unsubscribe)
- Review job for large object allocations (use batching)

**Concurrent Execution Issues**
- Add `[DisallowConcurrentExecution]` attribute to job class
- Increase cron interval if job takes longer than schedule
- Consider implementing job locking with database flag

## Architecture Relationship

```
Windows Service (Scheduler.cs)
    ↓ Initializes
ApplicationManager (DI Setup)
    ↓ Creates
TaskScheduler (Quartz.NET)
    ↓ Schedules & Executes
Individual Jobs (BaseJob descendants)
    ↓ Use
Core Services (Business Logic)
    ↓ Use
Infrastructure (Repositories)
    ↓ Use
ORM (Entity Framework)
    ↓ Connect to
Database (MySQL)
```

## Future Enhancements

- **Job Dashboard**: Web UI for monitoring job execution status
- **Job History**: Store execution history in database with duration, status, error messages
- **Dynamic Scheduling**: Allow admins to change cron expressions without code deployment
- **Job Prioritization**: Implement priority queue for critical jobs
- **Distributed Execution**: Scale to multiple servers with job distribution
- **Health Checks**: API endpoint for monitoring service health
