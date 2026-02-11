# Core.Notification.Impl - Developer Guide

## What's in This Folder?

**13 implementation classes** that power the notification system:

| Category | Classes | Purpose |
|----------|---------|---------|
| **Core Services** | NotificationService, EmailDispatcher, NotificationServiceHelper | Queue notifications, send emails |
| **Factories** | NotificationModelFactory, UserNotificationModelFactory | Create notification models |
| **Polling Agents** | 4 agents | Background workers that process queues |
| **Domain Services** | 4 services | Thin wrappers for specific domains |

## Quick Reference

### Core Services

**NotificationService** - Queue notifications
```csharp
// Queue to user
notificationService.QueueUpNotificationEmail(
    NotificationTypes.ForgetPassword,
    passwordResetModel,
    organizationRoleUserId
);

// Queue with explicit email
notificationService.QueueUpNotificationEmail(
    NotificationTypes.SendInvoiceDetail,
    invoiceModel,
    "Company Name",
    "noreply@company.com",
    "customer@example.com",
    DateTime.UtcNow,
    orgRoleUserId,
    attachments
);
```

**EmailDispatcher** - Send via SendGrid
```csharp
// Called by polling agent
emailDispatcher.SendEmail(
    htmlBody,
    subject,
    fromName,
    fromEmail,
    ccEmails,
    attachments,
    toEmails
);
```

### Factories

**NotificationModelFactory** - Create base models
```csharp
var baseModel = _modelFactory.CreateBaseDefault();
// Contains: SiteRootUrl, LogoImage, CompanyName, etc.
```

**UserNotificationModelFactory** - 65+ notification builders
```csharp
// Example: Invoice notification
_userNotificationModelFactory.CreateInvoiceDetailNotification(
    organizationId,
    franchiseeInvoiceList
);

// Example: Password reset
_userNotificationModelFactory.CreateForgetPasswordNotification(
    passwordLink,
    person
);
```

### Polling Agents

**Setup (Hangfire example):**
```csharp
// General queue processor (every 5 minutes)
RecurringJob.AddOrUpdate<INotificationPollingAgent>(
    "process-notifications",
    agent => agent.PollForNotifications(),
    "*/5 * * * *"
);

// Payment reminders (daily at 8 AM)
RecurringJob.AddOrUpdate<IPaymentReminderPollingAgent>(
    "payment-reminders",
    agent => agent.CreateNotificationReminderForPayment(),
    "0 8 * * *"
);

// Weekly reports (Mondays at 9 AM)
RecurringJob.AddOrUpdate<IWeeklyNotificationPollingAgent>(
    "weekly-reports",
    agent => agent.CreateWeeklyNotification(),
    "0 9 * * 1"
);

// Document expiry (daily at 7 AM)
RecurringJob.AddOrUpdate<IDocumentNotificationPollingAgent>(
    "document-expiry",
    agent => agent.SendExpiryNotification(),
    "0 7 * * *"
);
```

## Common Scenarios

### Scenario 1: Queue Invoice Notification

```csharp
public class BillingService
{
    private readonly IInvoiceNotificationService _invoiceNotificationService;
    
    public void GenerateAndSendInvoice(long franchiseeId)
    {
        // 1. Generate invoice
        var invoices = GenerateInvoices(franchiseeId);
        
        // 2. Queue notification (delegates to UserNotificationModelFactory)
        _invoiceNotificationService.CreateInvoiceDetailNotification(
            invoices,
            franchiseeId
        );
        
        // 3. Polling agent will send within minutes
    }
}
```

### Scenario 2: Send Weekly Report

```csharp
// Scheduled job calls this weekly
public void GenerateWeeklyReports()
{
    // Agent checks settings
    if (!Settings.SendWeeklyReminder) return;
    
    // Generates 3 reports:
    // 1. Late fee report (if enabled)
    // 2. Unpaid invoice report (if enabled)
    // 3. AR aging report (if enabled)
    
    _weeklyNotificationPollingAgent.CreateWeeklyNotification();
    
    // Result: Excel attachments emailed to admins
}
```

### Scenario 3: Custom Notification

```csharp
public void SendCustomAnnouncement(List<string> recipients, string message)
{
    var baseModel = _notificationModelFactory.CreateBaseDefault();
    
    var model = new PersonalMailForMembersModel(baseModel)
    {
        FullName = "Team",
        CustomMessage = message
    };
    
    // Queue with custom body
    _notificationService.QueueUpNotificationDyamicEmail(
        NotificationTypes.PersonalMailForMemebers,
        model,
        "Management",
        "noreply@company.com",
        string.Join(",", recipients),
        DateTime.UtcNow,
        customHtmlBody: message,
        organizationRoleUserId: null
    );
}
```

### Scenario 4: Monitor Queue Health

```csharp
public class NotificationHealthCheck
{
    public HealthStatus CheckQueue()
    {
        // Check pending notifications
        var pending = _notificationQueueRepo.Fetch(
            x => x.ServiceStatusId == (long)ServiceStatus.Pending
              && x.NotificationDate < DateTime.UtcNow.AddHours(-1) // Older than 1 hour
        );
        
        if (pending.Count() > 100)
            return HealthStatus.Unhealthy("Too many pending notifications");
        
        // Check failed notifications
        var failed = _notificationQueueRepo.Fetch(
            x => x.ServiceStatusId == (long)ServiceStatus.Failed
              && x.ServicedAt > DateTime.UtcNow.AddDays(-1) // Last 24 hours
        );
        
        if (failed.Count() > 10)
            return HealthStatus.Degraded($"{failed.Count()} failed notifications");
        
        return HealthStatus.Healthy();
    }
}
```

## Implementation Details

### NotificationService: Email Resolution Logic

When queuing to a user, email is resolved in this order:

```csharp
// 1. For billing notifications: Use accounting email
if (IsBillingNotification(notificationType))
{
    email = franchisee.AccountPersonEmail 
         ?? franchisee.ContactEmail
         ?? person.Email;
}
else
{
    email = person.Email;
}
```

### NotificationService: Auto-CC Logic

Certain notification types get automatic CCs:

```csharp
// Admin CC for billing
if (notificationType == SendInvoiceDetail || 
    notificationType == PaymentConfirmation)
{
    recipients.Add(new NotificationEmailRecipient {
        RecipientEmail = Settings.CCToAdmin,
        RecipientTypeId = (long)RecipientType.CC
    });
}

// Owner CC for late fees
if (notificationType == LateFeeReminderForPayment)
{
    recipients.Add(new NotificationEmailRecipient {
        RecipientEmail = franchisee.OwnerEmail,
        RecipientTypeId = (long)RecipientType.CC
    });
}
```

### EmailDispatcher: SendGrid Integration

```csharp
// Initialize client
var client = new SendGridClient(apiKey);

// Build message
var message = new SendGridMessage {
    From = new EmailAddress(fromEmail, fromName),
    Subject = subject,
    HtmlContent = body
};

// Add recipient (primary TO only)
message.AddTo(new EmailAddress(toEmail[0]));

// Add CCs
foreach (var cc in ccEmails)
    message.AddCc(new EmailAddress(cc));

// Attach files (Base64)
foreach (var resource in resources)
{
    var bytes = File.ReadAllBytes(filePath);
    var base64 = Convert.ToBase64String(bytes);
    message.AddAttachment(fileName, base64);
}

// Send
var response = await client.SendEmailAsync(message);
```

### Polling Agent: Processing Loop

```csharp
public void PollForNotifications()
{
    // 1. Get pending notifications
    var pending = _repo.Fetch(
        x => x.NotificationDate < DateTime.UtcNow
          && x.ServiceStatusId == (long)ServiceStatus.Pending
          && x.NotificationType.IsServiceEnabled
    );
    
    // 2. Process each
    foreach (var notification in pending)
    {
        if (notification.AttemptCount > 3)
        {
            // Max retries exceeded
            notification.ServiceStatusId = (long)ServiceStatus.Failed;
        }
        else
        {
            try
            {
                // Send email
                var toEmails = notification.NotificationEmail.Recipients
                    .Where(r => r.RecipientTypeId == (long)RecipientType.TO)
                    .Select(r => r.RecipientEmail)
                    .ToArray();
                
                _emailDispatcher.SendEmail(...);
                
                notification.ServiceStatusId = (long)ServiceStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to send notification {notification.Id}", ex);
                notification.AttemptCount = (notification.AttemptCount ?? 0) + 1;
            }
        }
        
        // 3. Update status
        notification.ServicedAt = DateTime.UtcNow;
        _repo.Save(notification);
        _unitOfWork.SaveChanges();
    }
}
```

## Troubleshooting

### Issue: Emails Not Sending

**Check polling agent is running:**
```csharp
// Verify last run time (if using Hangfire)
var jobHistory = JobStorage.Current
    .GetConnection()
    .GetRecurringJobs()
    .FirstOrDefault(x => x.Id == "process-notifications");

if (jobHistory?.LastExecution == null)
    Console.WriteLine("Polling agent has never run!");
else
    Console.WriteLine($"Last run: {jobHistory.LastExecution}");
```

**Check pending notifications:**
```sql
-- Should be processed within minutes
SELECT COUNT(*) AS PendingCount
FROM NotificationQueues
WHERE ServiceStatusId = 111 -- Pending
  AND NotificationDate < GETDATE();
```

### Issue: SendGrid Errors

**Check API key:**
```csharp
// Test SendGrid connection
var client = new SendGridClient(Settings.SmtpEmailApiKey);
var testMessage = MailHelper.CreateSingleEmail(
    new EmailAddress("test@test.com"),
    new EmailAddress("test@test.com"),
    "Test",
    "",
    "Test"
);

var response = await client.SendEmailAsync(testMessage);
Console.WriteLine($"Status: {response.StatusCode}");
// Should be 202 (Accepted)
```

**Check SendGrid dashboard** for delivery status, bounces, spam reports.

### Issue: Attachments Missing

**Verify file exists:**
```csharp
foreach (var resource in notification.NotificationEmail.Resources)
{
    var filePath = Path.Combine(
        resource.Resource.RelativeLocation,
        resource.Resource.Name
    );
    
    if (!File.Exists(filePath))
        Console.WriteLine($"ERROR: File not found: {filePath}");
    else
        Console.WriteLine($"OK: {filePath} ({resource.Resource.Size} bytes)");
}
```

### Issue: Duplicate Emails

**Check deduplication tables:**
```sql
-- Payment reminders
SELECT * FROM PaymentMailReminders
WHERE Date = CAST(GETDATE() AS DATE);

-- Weekly reports
SELECT * FROM WeeklyNotifications
WHERE NotificationDate >= DATEADD(day, -7, GETDATE());
```

**Verify single polling agent instance:**
```bash
# Check for multiple processes
ps aux | grep "NotificationPollingAgent"

# Should be only one instance running
```

## Best Practices

1. **Always use factories:** Don't create NotificationQueue manually
2. **Monitor failed notifications:** Set up alerts for Failed status
3. **Test templates:** Use test email before deploying new templates
4. **Limit concurrent agents:** Run only one instance of each polling agent
5. **Log SendGrid responses:** Track API status codes for debugging
6. **Handle file I/O errors:** Check file exists before queuing attachments
7. **Use transactions:** Wrap queue operations in transactions for consistency

## Configuration

### Required Settings

```csharp
// SendGrid
Settings.SmtpEmailApiKey = "SG.xxxxx"

// Company Info
Settings.CompanyName = "MarbleLife"
Settings.FromEmail = "noreply@company.com"
Settings.SiteRootUrl = "https://app.company.com"
Settings.LogoImage = "/images/logo.png"

// CC Emails
Settings.CCToAdmin = "admin@company.com"
Settings.RecipientEmail = "special@company.com"

// Weekly Reports
Settings.SendWeeklyReminder = true
Settings.SendWeekyLateFeeNotification = true
Settings.SendWeekyUnpaidInvoicesNotification = true
Settings.WeeklyReminderDay = 1 // Monday

// Document Expiry
Settings.SendExpiryNotification = true
```

## Related Documentation

- [Parent Module](../.context/OVERVIEW.md)
- [Domain Entities](../Domain/.context/OVERVIEW.md)
- [View Models](../ViewModel/.context/OVERVIEW.md)
