# NotificationService - Job Notification Service

## Overview
The NotificationService is a background Windows service (Console Application) designed to send automated notifications for scheduled jobs, reminders, and system alerts. It runs as a scheduled task or service to process notification queues.

## Purpose
- Send email notifications for upcoming jobs
- SMS notifications to technicians
- Customer appointment reminders
- System alert notifications
- Scheduled report delivery

## Technology Stack
- **.NET Framework**: C# Console Application
- **Job Scheduling**: Quartz.NET or Windows Task Scheduler
- **Email**: SMTP integration
- **SMS**: Third-party SMS gateway integration
- **Database**: Entity Framework for data access

## Project Structure
```
/NotificationService
├── Jobs.csproj                    # Project file
└── [Service implementation files]
```

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <add key="SmtpServer" value="smtp.server.com" />
    <add key="SmtpPort" value="587" />
    <add key="SmtpUsername" value="notifications@marblelife.com" />
    <add key="SmtpPassword" value="encrypted_password" />
    <add key="SmsApiKey" value="sms_api_key" />
    <add key="SmsApiUrl" value="https://sms-gateway.com/api" />
    <add key="NotificationIntervalMinutes" value="15" />
    <add key="ReminderHoursBefore" value="24" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Service Architecture

### Main Service Loop
```csharp
public class NotificationService
{
    private readonly ILogService _logService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private Timer _timer;
    
    public void Start()
    {
        _logService.Info("NotificationService starting...");
        
        var interval = TimeSpan.FromMinutes(
            Convert.ToInt32(ConfigurationManager.AppSettings["NotificationIntervalMinutes"])
        );
        
        _timer = new Timer(ProcessNotifications, null, TimeSpan.Zero, interval);
    }
    
    private void ProcessNotifications(object state)
    {
        try
        {
            _logService.Debug("Processing notifications...");
            
            ProcessJobReminders();
            ProcessSystemAlerts();
            ProcessScheduledReports();
            
            _logService.Debug("Notification processing complete");
        }
        catch (Exception ex)
        {
            _logService.Error("Error processing notifications", ex);
        }
    }
    
    private void ProcessJobReminders()
    {
        var reminderHours = Convert.ToInt32(
            ConfigurationManager.AppSettings["ReminderHoursBefore"]
        );
        
        var upcomingJobs = GetJobsNeedingReminders(reminderHours);
        
        foreach (var job in upcomingJobs)
        {
            SendJobReminder(job);
        }
    }
    
    private void SendJobReminder(Job job)
    {
        // Send email to customer
        _emailService.SendJobReminder(job.Customer.Email, job);
        
        // Send SMS to technician
        if (!string.IsNullOrEmpty(job.Technician.Phone))
        {
            _smsService.SendJobReminder(job.Technician.Phone, job);
        }
        
        // Mark notification as sent
        MarkNotificationSent(job.Id);
    }
}
```

## Notification Types

### 1. Job Reminders
- **24-hour advance notice** to customers
- **2-hour advance notice** to technicians
- **Job completion follow-up** (24 hours after)

### 2. Estimate Reminders
- Follow-up on pending estimates
- Estimate expiration warnings
- Price change notifications

### 3. System Alerts
- Failed payment notifications
- System errors
- Data import completion
- Report generation completion

### 4. Marketing Communications
- Service reminders (annual maintenance)
- Promotional offers
- Customer satisfaction surveys
- Newsletter distribution

## Email Templates

### Job Reminder Template
```csharp
public class JobReminderEmailTemplate
{
    public string Subject => "Reminder: Upcoming Marble Life Service";
    
    public string Body(Job job)
    {
        return $@"
            <html>
            <body>
                <h2>Service Reminder</h2>
                <p>Dear {job.Customer.FirstName},</p>
                <p>This is a reminder that your Marble Life service is scheduled for:</p>
                <ul>
                    <li><strong>Date:</strong> {job.ScheduledDate:MMMM dd, yyyy}</li>
                    <li><strong>Time:</strong> {job.ScheduledDate:h:mm tt}</li>
                    <li><strong>Service:</strong> {job.ServiceType.Name}</li>
                    <li><strong>Technician:</strong> {job.Technician.FullName}</li>
                </ul>
                <p>If you need to reschedule, please contact us at {job.Franchisee.Phone}</p>
                <p>Thank you for choosing Marble Life!</p>
            </body>
            </html>
        ";
    }
}
```

## SMS Integration

```csharp
public interface ISmsService
{
    Task<bool> SendSms(string phoneNumber, string message);
}

public class TwilioSmsService : ISmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;
    
    public TwilioSmsService(ISettings settings)
    {
        _accountSid = settings.TwilioAccountSid;
        _authToken = settings.TwilioAuthToken;
        _fromNumber = settings.TwilioFromNumber;
    }
    
    public async Task<bool> SendSms(string phoneNumber, string message)
    {
        try
        {
            var client = new TwilioRestClient(_accountSid, _authToken);
            
            var result = await client.SendMessage(
                _fromNumber,
                phoneNumber,
                message
            );
            
            return result.Status == "sent";
        }
        catch (Exception ex)
        {
            _logService.Error($"Failed to send SMS to {phoneNumber}", ex);
            return false;
        }
    }
}
```

## Database Integration

### Notification Queue Table
```sql
CREATE TABLE NotificationQueue (
    Id BIGINT PRIMARY KEY IDENTITY,
    Type INT NOT NULL, -- Email, SMS, Push
    Status INT NOT NULL, -- Pending, Sent, Failed
    RecipientType INT NOT NULL, -- Customer, Technician, Admin
    RecipientId BIGINT NOT NULL,
    Subject NVARCHAR(200),
    Body NVARCHAR(MAX),
    ScheduledDate DATETIME NOT NULL,
    SentDate DATETIME,
    FailureReason NVARCHAR(500),
    RetryCount INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);
```

### Domain Model
```csharp
public class NotificationQueue
{
    public long Id { get; set; }
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public RecipientType RecipientType { get; set; }
    public long RecipientId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? SentDate { get; set; }
    public string FailureReason { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public enum NotificationType
{
    Email = 1,
    SMS = 2,
    Push = 3
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}
```

## Error Handling

```csharp
private void ProcessNotificationWithRetry(NotificationQueue notification)
{
    const int MaxRetries = 3;
    
    try
    {
        SendNotification(notification);
        
        notification.Status = NotificationStatus.Sent;
        notification.SentDate = DateTime.UtcNow;
    }
    catch (Exception ex)
    {
        notification.RetryCount++;
        
        if (notification.RetryCount >= MaxRetries)
        {
            notification.Status = NotificationStatus.Failed;
            notification.FailureReason = ex.Message;
            _logService.Error($"Notification {notification.Id} failed after {MaxRetries} retries", ex);
        }
        else
        {
            _logService.Warning($"Notification {notification.Id} failed, retry {notification.RetryCount}/{MaxRetries}");
        }
    }
    finally
    {
        _unitOfWork.Repository<NotificationQueue>().Save(notification);
        _unitOfWork.SaveChanges();
    }
}
```

## Logging

```csharp
public class NotificationLogger
{
    private readonly ILogService _logService;
    
    public void LogNotificationSent(NotificationQueue notification)
    {
        _logService.Info($"Notification sent: Type={notification.Type}, Recipient={notification.RecipientId}, Subject={notification.Subject}");
    }
    
    public void LogNotificationFailed(NotificationQueue notification, Exception ex)
    {
        _logService.Error($"Notification failed: Type={notification.Type}, Recipient={notification.RecipientId}, Subject={notification.Subject}", ex);
    }
    
    public void LogServiceStatus(int pending, int sent, int failed)
    {
        _logService.Info($"Notification status: Pending={pending}, Sent={sent}, Failed={failed}");
    }
}
```

## Deployment

### Windows Service Installation
```cmd
# Install as Windows Service
sc create "MarbleLife.NotificationService" binPath="C:\Services\NotificationService\NotificationService.exe"

# Start service
sc start "MarbleLife.NotificationService"

# Configure service to start automatically
sc config "MarbleLife.NotificationService" start=auto
```

### Task Scheduler Alternative
```xml
<?xml version="1.0" encoding="UTF-16"?>
<Task version="1.2">
  <Triggers>
    <TimeTrigger>
      <Repetition>
        <Interval>PT15M</Interval>
        <StopAtDurationEnd>false</StopAtDurationEnd>
      </Repetition>
      <StartBoundary>2024-01-01T00:00:00</StartBoundary>
      <Enabled>true</Enabled>
    </TimeTrigger>
  </Triggers>
  <Actions>
    <Exec>
      <Command>C:\Services\NotificationService\NotificationService.exe</Command>
    </Exec>
  </Actions>
</Task>
```

## Performance Optimization

1. **Batch Processing**: Process notifications in batches to reduce database roundtrips
2. **Async Operations**: Use async/await for I/O operations
3. **Connection Pooling**: Reuse database connections
4. **Rate Limiting**: Throttle external API calls
5. **Caching**: Cache frequently accessed configuration

## Monitoring

### Health Checks
- Service running status
- Last successful run time
- Pending notification count
- Failed notification rate
- Average processing time

### Alerts
- Service stopped unexpectedly
- High failure rate (>10%)
- Processing taking too long
- Queue backing up

## Best Practices

1. **Idempotency**: Ensure notifications can be safely retried
2. **Transaction Management**: Use transactions for database updates
3. **Graceful Shutdown**: Handle service stop requests cleanly
4. **Configuration Management**: Externalize all configuration
5. **Logging**: Comprehensive logging for troubleshooting
6. **Error Handling**: Robust error handling and retry logic
7. **Testing**: Unit tests for business logic, integration tests for external services

## Related Services
- See `/CalendarImportService/AI-CONTEXT.md` for calendar integration
- See `/CustomerDataUpload/AI-CONTEXT.md` for data processing
- See Core.Application documentation for shared infrastructure
