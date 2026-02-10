<!-- AUTO-GENERATED: Header -->
# NotificationService (Jobs) — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
NotificationService (deployed as "Jobs") is a comprehensive task scheduler and notification processing engine for the Marblelife system. It runs as a Windows Service or console application, executing 40+ scheduled jobs including email notifications, data synchronization, file parsing, payment reminders, review collection, and third-party API integrations.

### Design Patterns
- **Windows Service Pattern**: Runs continuously as background service or interactive console
- **Quartz.NET Scheduler**: Cron-based job scheduling with configurable intervals
- **Polling Agent Pattern**: Each job type implements a polling agent interface
- **Dependency Injection**: All jobs resolved via ApplicationManager
- **Job Configuration**: XML-based scheduling (Tasks.xml) with job-specific parameters

### Data Flow
1. Application starts as Windows Service or console mode
2. Scheduler loads job definitions from Tasks.xml
3. Each job triggered by cron schedule (hourly, daily, weekly, monthly)
4. Job resolves dependencies (INotificationPollingAgent, ICustomerFeedbackService, etc.)
5. Job executes business logic (send emails, parse files, update records)
6. Results logged via ILogService
7. Scheduler continues until service stopped

### Job Categories
- **Notifications**: Email/SMS alerts to customers and franchisees
- **File Processing**: Sales data, invoice parsing, customer uploads
- **Reminders**: Payment due dates, data upload deadlines
- **Reviews**: Customer feedback collection and API syncing
- **Financial**: Late fees, currency rates, loan schedules
- **Marketing**: Lead imports, HomeAdvisor integration
- **Reporting**: Weekly summaries, monthly analytics
- **Document Management**: Expiry notifications, file migrations
- **Integration**: ReviewPush, sales tax APIs, S3 bucket sync
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Common Interfaces
```csharp
public interface INotificationPollingAgent
{
    void PollForNotifications();  // Sends pending email/SMS notifications
}

public interface IPaymentReminderPollingAgent
{
    void CreateNotificationReminderForPayment();  // Payment due reminders
}

public interface ISalesDataUploadReminderPollingAgent
{
    void CreateNotificationReminderForSalesDataUpload();  // Upload deadline reminders
}

public interface ICustomerFileUploadPollingAgent
{
    void ParseCustomerFile();  // Processes uploaded customer Excel files
}
```

### Job Configuration (Tasks.xml)
```xml
<job>
  <name>NotificationPollingAgent</name>
  <group>Notifications</group>
  <job-type>Jobs.Impl.NotificationJob, Jobs</job-type>
  <durable>true</durable>
  <recover>false</recover>
</job>
<trigger>
  <cron>
    <name>NotificationTrigger</name>
    <cron-expression>0 0/5 * * * ?</cron-expression>  <!-- Every 5 minutes -->
  </cron>
</trigger>
```

### Scheduler Service
```csharp
public class Scheduler : ServiceBase
{
    private ITaskScheduler _scheduler;
    
    protected override void OnStart(string[] args)
    {
        _scheduler = ApplicationManager.DependencyInjection.Resolve<ITaskScheduler>();
        _scheduler.Start();
    }
    
    protected override void OnStop()
    {
        _scheduler.Stop();
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Primary Jobs (Active)

#### Notification Processing
- **`INotificationPollingAgent.PollForNotifications()`**
  - **Frequency**: Every 5 minutes
  - **Purpose**: Sends queued email/SMS notifications from database
  - **Side-effects**: Updates notification status, marks as sent

#### Payment Reminders
- **`IPaymentReminderPollingAgent.CreateNotificationReminderForPayment()`**
  - **Frequency**: Daily
  - **Purpose**: Creates payment due reminders for invoices
  - **Side-effects**: Inserts notification records

#### Sales Data Upload Reminders
- **`ISalesDataUploadReminderPollingAgent.CreateNotificationReminderForSalesDataUpload()`**
  - **Frequency**: Weekly
  - **Purpose**: Reminds franchisees to upload sales data
  - **Side-effects**: Sends emails to franchisees with pending uploads

#### Weekly Notifications
- **`IWeeklyNotificationPollingAgent.CreateWeeklyNotification()`**
  - **Frequency**: Weekly (Monday mornings)
  - **Purpose**: Sends performance summaries and key metrics
  - **Side-effects**: Generates and emails reports

#### Customer Feedback Collection
- **`ISendFeedBackRequestPollingAgent.SendFeedback()`**
  - **Frequency**: Daily
  - **Purpose**: Sends review requests to recently serviced customers
  - **Side-effects**: Creates feedback requests in ReviewPush

- **`IGetCustomerFeedbackService.GetFeedbackResponse()`**
  - **Frequency**: Hourly
  - **Purpose**: Retrieves customer feedback from ReviewPush API
  - **Side-effects**: Updates customer review records

#### Currency Rate Updates
- **`ICurrencyRateService.AllCurrencyRateByDate()`**
  - **Frequency**: Daily
  - **Purpose**: Fetches latest exchange rates for international franchisees
  - **Side-effects**: Updates CurrencyExchangeRate table

#### Invoice Generation
- **`FranchiseeInvoiceGenerationPollingAgent.ProcessRecords()`**
  - **Frequency**: Monthly (1st of month)
  - **Purpose**: Generates royalty invoices for franchisees
  - **Side-effects**: Creates invoice records, sends emails

#### Late Fee Processing
- **`IInvoiceLateFeePollingAgent.LateFeeGenerator()`**
  - **Frequency**: Daily
  - **Purpose**: Applies late fees to overdue invoices
  - **Side-effects**: Updates invoice amounts, creates fee records

### Additional Jobs (Commented Out)
See Program.cs lines 56-213 for full list of 40+ job types covering marketing leads, document notifications, job reminders, price estimates, payroll reports, and S3 synchronization.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Notification](../../Core/Notification/.context/CONTEXT.md)** — Notification agents and email services
- **[Core.Billing](../../Core/Billing/.context/CONTEXT.md)** — Invoice generation, late fees, currency rates
- **[Core.Sales](../../Core/Sales/.context/CONTEXT.md)** — Sales data parsing, customer file uploads
- **[Core.Review](../../Core/Review/.context/CONTEXT.md)** — Customer feedback collection, ReviewPush integration
- **[Core.Scheduler](../../Core/Scheduler/.context/CONTEXT.md)** — Task scheduling infrastructure
- **[Core.MarketingLead](../../Core/MarketingLead/.context/CONTEXT.md)** — Lead imports and conversions
- **[Core.Reports](../../Core/Reports/.context/CONTEXT.md)** — Report generation services
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration

### External
- **Quartz.NET** — Enterprise job scheduling framework
- **System.ServiceProcess** — Windows Service hosting
- **SMTP/SendGrid** — Email delivery
- **Third-Party APIs**: ReviewPush, HomeAdvisor, Sales Tax APIs

### Configuration Files
- **Tasks.xml** — Job definitions and cron schedules
- **App.config** — Database connection, API keys, SMTP settings
- **job_scheduling_data_2_0.xsd** — Quartz XML schema validation
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Deployment Modes

**Windows Service** (Production):
```bash
# Install service
sc create "MarbleLife Jobs" binPath="C:\path\to\Jobs.exe"
sc start "MarbleLife Jobs"
```

**Console Mode** (Development/Debugging):
```bash
# Run interactively with UserInteractive flag
Jobs.exe
# Uncomment specific job in Program.cs for testing
```

### Job Activation Strategy
All jobs in Program.cs are **commented out by default**. This allows selective activation:

**Enable specific job**:
```csharp
// Uncomment the job you want to run
var notificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<INotificationPollingAgent>();
notificationPollingAgent.PollForNotifications();
```

**Scheduler-based execution** (preferred):
Configure in Tasks.xml instead of Program.cs for automatic cron-based scheduling.

### Job Scheduling Patterns

**High-Frequency** (Every 5 minutes):
- Notification polling
- S3 bucket sync

**Hourly**:
- Customer feedback retrieval
- Marketing lead imports

**Daily**:
- Payment reminders
- Currency rate updates
- Late fee processing
- Review requests

**Weekly**:
- Performance summaries
- Sales data upload reminders

**Monthly**:
- Invoice generation (1st of month)
- Annual reports

### Common Job Failures

1. **Database Connection Timeout**:
   - Cause: Long-running query blocks scheduler
   - Solution: Increase command timeout, optimize queries

2. **Email Send Failures**:
   - Cause: SMTP credentials expired or rate limiting
   - Solution: Check App.config SMTP settings, implement retry logic

3. **API Rate Limiting**:
   - Cause: Too many requests to ReviewPush/HomeAdvisor
   - Solution: Add throttling delays, batch requests

4. **File Parsing Errors**:
   - Cause: Invalid Excel format or corrupted upload
   - Solution: Enhanced validation in parsing agents

5. **Memory Leaks**:
   - Cause: Large dataset processing without disposal
   - Solution: Implement IDisposable, use `using` statements

### TLS 1.2 Requirement
```csharp
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
```
Required for modern API integrations (ReviewPush, payment gateways).

### Manual Job Execution
For testing individual jobs:
1. Uncomment job in `ExecuteServices()` method
2. Set breakpoints in job implementation
3. Run in console mode (F5 in Visual Studio)
4. Press Enter when done to terminate

### Job Dependencies
Some jobs have execution order dependencies:
- Currency rates must update before invoice generation
- Customer data must parse before notification sending
- File uploads must complete before report generation

Consider job chaining or sequential scheduling in Tasks.xml.

### Logging Strategy
All jobs log to ILogService:
- **Info**: Job start/completion, record counts
- **Error**: Exceptions with stack traces
- **Debug**: Detailed execution steps (enable in App.config)

### Performance Optimization
- **Batch processing**: Process 100 records at a time, not 10,000 at once
- **Async operations**: Use async/await for I/O-bound tasks
- **Database indexing**: Ensure notification and sales tables indexed
- **Connection pooling**: Reuse database connections across jobs
<!-- END CUSTOM SECTION -->
