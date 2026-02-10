<!-- AUTO-GENERATED: Header -->
# NotificationService (Jobs)
> Comprehensive task scheduler and notification engine for automated system operations
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
NotificationService is the automation backbone of the Marblelife system, running 40+ scheduled jobs that keep the business operating smoothly. It handles everything from sending payment reminders to collecting customer reviews, generating financial reports, and synchronizing data with third-party services.

Think of it as the system's autonomous nervous system — constantly monitoring, processing, and communicating without human intervention. Franchisees get payment reminders automatically, customers receive review requests after service completion, late fees apply on schedule, and management receives weekly performance reports — all orchestrated by this service.

**Deployment**: Runs as a Windows Service in production (24/7 background operation) or console application in development (manual testing with debugging).

**Key Capabilities**:
- **Email/SMS Notifications**: Automated customer and franchisee communication
- **Financial Automation**: Invoice generation, late fees, payment reminders
- **Review Collection**: Automated feedback requests and ReviewPush integration
- **Data Processing**: Sales uploads, customer file parsing, report generation
- **Third-Party Integration**: HomeAdvisor leads, sales tax APIs, S3 storage
- **Reporting**: Weekly summaries, monthly analytics, performance dashboards
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Prerequisites
1. .NET Framework 4.5.2+
2. Windows Server (for service deployment)
3. Quartz.NET scheduler
4. Configured SMTP/email service

### Installation as Windows Service

**Install**:
```bash
# Using sc.exe (Windows Service Control)
sc create "MarbleLife Notification Service" binPath="C:\MarbleLife\Jobs.exe" start=auto
sc description "MarbleLife Notification Service" "Handles scheduled jobs and notifications"
sc start "MarbleLife Notification Service"
```

**Verify Running**:
```bash
sc query "MarbleLife Notification Service"
# Should show STATE: RUNNING
```

**Stop/Uninstall**:
```bash
sc stop "MarbleLife Notification Service"
sc delete "MarbleLife Notification Service"
```

### Running in Console Mode (Development)

**Manual Execution**:
```bash
Jobs.exe
# Press Enter to terminate when done
```

**Enable Specific Job for Testing**:
Edit `Program.cs`:
```csharp
private static void ExecuteServices(ILogService logger)
{
    // Uncomment the job you want to test
    var notificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<INotificationPollingAgent>();
    notificationPollingAgent.PollForNotifications();
}
```

**Expected Console Output**:
```
Starting services
Notification polling started
Processed 15 notifications, 2 failed
Press enter key to terminate
```

### Configuration

**Tasks.xml** (Job Schedules):
```xml
<job-scheduling-data>
  <schedule>
    <job>
      <name>NotificationJob</name>
      <group>MarbleLife</group>
      <job-type>Jobs.Impl.NotificationJob, Jobs</job-type>
    </job>
    <trigger>
      <cron>
        <name>NotificationTrigger</name>
        <cron-expression>0 0/5 * * * ?</cron-expression>  <!-- Every 5 min -->
      </cron>
    </trigger>
  </schedule>
</job-scheduling-data>
```

**Cron Expression Examples**:
```
0 0/5 * * * ?     # Every 5 minutes
0 0 8 * * ?       # Daily at 8 AM
0 0 8 ? * MON     # Every Monday at 8 AM
0 0 1 1 * ?       # First day of month at midnight
```

### Common Job Types

**Notifications** (Every 5 minutes):
- Poll notification queue
- Send email/SMS
- Update delivery status

**Payment Reminders** (Daily at 8 AM):
- Find overdue invoices
- Generate reminder notifications
- Send to franchisees

**Weekly Reports** (Monday at 6 AM):
- Aggregate weekly metrics
- Generate performance summaries
- Email to management

**Monthly Invoice Generation** (1st at midnight):
- Calculate royalties
- Generate invoices
- Send to franchisees

**Customer Reviews** (Daily at 10 AM):
- Find completed jobs from previous day
- Send review request emails
- Track response rates
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Job Type | Frequency | Description |
|----------|-----------|-------------|
| `INotificationPollingAgent` | 5 min | Sends queued email/SMS notifications |
| `IPaymentReminderPollingAgent` | Daily | Payment due date reminders |
| `ISalesDataUploadReminderPollingAgent` | Weekly | Franchisee upload deadline reminders |
| `IWeeklyNotificationPollingAgent` | Weekly | Performance summary emails |
| `ISendFeedBackRequestPollingAgent` | Daily | Customer review requests |
| `IGetCustomerFeedbackService` | Hourly | Retrieves ReviewPush responses |
| `ICurrencyRateService` | Daily | Updates exchange rates |
| `FranchiseeInvoiceGenerationPollingAgent` | Monthly | Generates royalty invoices |
| `IInvoiceLateFeePollingAgent` | Daily | Applies late payment fees |
| `ICustomerFileUploadPollingAgent` | Continuous | Parses uploaded customer Excel files |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Service Won't Start
**Cause**: Missing dependencies or configuration errors.  
**Solution**:
- Check Windows Event Viewer → Application logs
- Verify App.config database connection string
- Ensure Tasks.xml is valid XML
- Run in console mode to see detailed errors

### Jobs Not Executing
**Cause**: Scheduler not loading Tasks.xml properly.  
**Solution**:
- Verify Tasks.xml path is accessible
- Check cron expressions are valid (use cron validator online)
- Enable debug logging in App.config
- Check service account has file system permissions

### Email Notifications Not Sending
**Cause**: SMTP configuration or credentials issue.  
**Solution**:
- Verify SMTP settings in App.config
- Test SMTP credentials manually (telnet test)
- Check for rate limiting (SendGrid, Mailgun)
- Verify firewall allows SMTP port (25, 587, 465)

### Database Connection Timeouts
**Cause**: Long-running jobs blocking connection pool.  
**Solution**:
- Increase command timeout in App.config
- Optimize slow queries (add indexes)
- Implement job timeout limits
- Use connection pooling settings

### Memory Usage Growing
**Cause**: Jobs not disposing resources properly.  
**Solution**:
- Check for IDisposable implementation
- Use `using` statements for database connections
- Process large datasets in batches
- Restart service nightly during off-hours

### Duplicate Notifications Sent
**Cause**: Job executed multiple times due to crash recovery.  
**Solution**:
- Set `<recover>false</recover>` in job definition
- Implement idempotency checks in job logic
- Add "sent" status checks before sending

### Third-Party API Failures
**Cause**: ReviewPush, HomeAdvisor, or sales tax API unavailable.  
**Solution**:
- Implement retry logic with exponential backoff
- Add circuit breaker pattern
- Log failures for manual review
- Set up API health monitoring

### Testing Individual Jobs
**Development Workflow**:
```bash
# 1. Uncomment job in Program.cs
# 2. Set breakpoints in job implementation
# 3. Run in console mode (F5 in Visual Studio)
# 4. Inspect logs and database changes
# 5. Comment out job when done
```

### Monitoring Job Execution
**Check Logs**:
```
# View service logs
Get-EventLog -LogName Application -Source "MarbleLife Notification Service" -Newest 50

# Or check file logs (if configured)
tail -f C:\MarbleLife\Logs\Jobs.log
```

**Database Queries**:
```sql
-- Check notification queue
SELECT COUNT(*) FROM Notification WHERE StatusId = 1;  -- Pending

-- Check recent job execution
SELECT * FROM JobExecution ORDER BY ExecutedAt DESC LIMIT 10;

-- Check failed notifications
SELECT * FROM Notification WHERE StatusId = 3 AND CreatedAt > DATE_SUB(NOW(), INTERVAL 24 HOUR);
```

### Performance Optimization

**Batch Processing**:
```csharp
// Good: Process 100 at a time
var batch = notifications.Take(100);
foreach (var notification in batch)
{
    SendNotification(notification);
}

// Bad: Load all 10,000 notifications
var all = notifications.ToList();  // Memory explosion!
```

**Async Operations**:
```csharp
// Good: Non-blocking I/O
await SendEmailAsync(notification);

// Bad: Blocking thread
SendEmail(notification);  // Blocks thread pool
```

**Connection Management**:
```csharp
// Good: Dispose connection
using (var connection = CreateConnection())
{
    // Execute queries
}

// Bad: Connection leak
var connection = CreateConnection();
ExecuteQuery(connection);  // Never disposed!
```

### Production Deployment Checklist
- [ ] Service installed with auto-start
- [ ] Service account has necessary permissions
- [ ] SMTP credentials configured and tested
- [ ] All third-party API keys set in App.config
- [ ] Database connection string points to production
- [ ] Logging configured (file + event log)
- [ ] Tasks.xml has production schedules (not test intervals)
- [ ] Monitor service health (alert on stop/crash)
- [ ] Set up log rotation (avoid disk full)
- [ ] Document job dependencies and order
- [ ] Test disaster recovery (service restart, database failover)
<!-- END CUSTOM SECTION -->
