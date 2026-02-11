# Core.Notification Module - Developer Guide

## What is This Module?

The **Notification module** is the **email command center** for the MarbleLife system. Think of it as a smart post office: services drop off email requests (with templates and recipients), they're stored in a queue, and background workers pick them up and deliver them via SendGrid.

### Why a Queue-Based System?

**Problem:** Sending emails synchronously blocks business logic and fails if SMTP is down.

**Solution:** Queue-based architecture with retry logic:
- ✅ Business logic returns immediately (fast response times)
- ✅ Email failures don't crash transactions  
- ✅ Automatic retries (up to 3 attempts)
- ✅ Audit trail (who, when, status)
- ✅ Scheduled delivery (future dating)

### Real-World Analogy

```
Service Layer = Customer dropping letter at post office
Notification Queue = Mail sorting center
Polling Agent = Mail carrier picking up deliveries
Email Dispatcher = Delivery truck (SendGrid)
```

## Quick Start

### Send a Simple Notification

```csharp
// 1. Inject the service
public class YourService
{
    private readonly INotificationService _notificationService;
    private readonly INotificationModelFactory _modelFactory;
    
    public YourService(INotificationService notificationService, 
                       INotificationModelFactory modelFactory)
    {
        _notificationService = notificationService;
        _modelFactory = modelFactory;
    }
    
    // 2. Create notification
    public void SendWelcomeEmail(Person person, long organizationRoleUserId)
    {
        // Build the base model with company info
        var baseModel = _modelFactory.CreateBaseDefault();
        
        // Create the notification-specific model
        var model = new SendLoginCredentialViewModel(baseModel) 
        {
            FullName = person.FullName,
            UserName = person.Email,
            Password = temporaryPassword,
            Franchisee = franchisee.Name
        };
        
        // Queue the notification (returns immediately)
        _notificationService.QueueUpNotificationEmail(
            NotificationTypes.SendLoginCredential,
            model,
            organizationRoleUserId,
            notificationDateTime: null, // Send ASAP
            resource: null              // No attachments
        );
        
        // That's it! The polling agent will send it within minutes
    }
}
```

### Send Email with Attachment

```csharp
public void SendInvoiceEmail(Invoice invoice, long franchiseeId)
{
    // 1. Prepare attachment
    var pdfFile = GenerateInvoicePdf(invoice);
    var resources = new List<NotificationResource> 
    {
        new NotificationResource 
        { 
            ResourceId = pdfFile.Id,
            NotificationEmailId = 0 // Will be set automatically
        }
    };
    
    // 2. Build model
    var baseModel = _modelFactory.CreateBase(franchiseeId);
    var model = new InvoicePaymentReminderNotificationModel(baseModel)
    {
        FullName = franchisee.OwnerName,
        Franchisee = franchisee.Name,
        InvoiceDetailList = MapInvoiceDetails(invoice)
    };
    
    // 3. Queue with attachment
    _notificationService.QueueUpNotificationEmail(
        NotificationTypes.SendInvoiceDetail,
        model,
        organizationRoleUserId,
        DateTime.UtcNow,
        resources // PDF will be attached
    );
}
```

### Schedule Future Notification

```csharp
public void SchedulePaymentReminder(Invoice invoice, DateTime reminderDate)
{
    var model = CreateReminderModel(invoice);
    
    _notificationService.QueueUpNotificationEmail(
        NotificationTypes.PaymentReminder,
        model,
        organizationRoleUserId,
        notificationDateTime: reminderDate, // Will send on this date
        resource: null
    );
    
    // Polling agent will send when NotificationDate arrives
}
```

## Key Concepts

### 1. Notification Types

Every email has a **NotificationTypes** enum value that determines:
- Which email template to use
- Whether queuing is enabled  
- Whether service is enabled
- Special CC logic

```csharp
// Examples
NotificationTypes.ForgetPassword           // Password reset
NotificationTypes.SendInvoiceDetail        // New invoice
NotificationTypes.PaymentReminder          // Overdue payment
NotificationTypes.WeeklyUnpaidInvoiceNotification // Weekly report
```

**See:** `Enum/NotificationTypes.cs` for all 65+ types.

### 2. Email Templates

Templates are stored in `EmailTemplate` table with Razor syntax:

```html
<h1>Hello @Model.FullName!</h1>
<p>Your password reset link: <a href="@Model.PasswordLink">Reset Now</a></p>
<p>From: @Model.Base.CompanyName</p>
```

**Template Selection:**
1. Lookup by `NotificationTypeId`
2. Match franchisee's `LanguageId` (multi-language support)
3. Fall back to default language

### 3. Service Status Lifecycle

```
┌─────────┐     Send Attempt     ┌─────────┐
│ Pending │ ─────────────────> │ Success │
└─────────┘                     └─────────┘
     │                               
     │ Fail (attempt < 3)            
     └──────────> Retry              
     │                               
     │ Fail (attempt >= 3)           
     └──────────────────────> ┌────────┐
                              │ Failed │
                              └────────┘
```

### 4. Polling Agents

Background workers that process the queue:

| Agent | Purpose | Trigger |
|-------|---------|---------|
| **INotificationPollingAgent** | Send all pending notifications | Every few minutes (scheduled job) |
| **IWeeklyNotificationPollingAgent** | Generate weekly reports | Weekly on specific day |
| **IPaymentReminderPollingAgent** | Send payment reminders | Daily |
| **IDocumentNotificationPollingAgent** | Document alerts | Daily |

**Typical Setup (Hangfire):**
```csharp
RecurringJob.AddOrUpdate<INotificationPollingAgent>(
    "process-notifications",
    agent => agent.PollForNotifications(),
    "*/5 * * * *" // Every 5 minutes
);
```

### 5. Recipient Types

```csharp
RecipientType.TO (127)   // Primary recipients
RecipientType.CC (128)   // Carbon copy
RecipientType.BCC (129)  // Blind carbon copy
```

**Auto-CC Logic:**
- Billing notifications → Admin email
- Late fees → Franchisee owner  
- Special franchisees → Configured email

## API Reference

### Core APIs

| Interface | Key Methods | Purpose |
|-----------|-------------|---------|
| **INotificationService** | `QueueUpNotificationEmail<T>(...)` | Queue notification with template |
|  | `QueueUpNotificationDyamicEmail<T>(...)` | Queue with custom body |
| **INotificationPollingAgent** | `PollForNotifications()` | Process queue |
| **IEmailDispatcher** | `SendEmail(...)` | Low-level SendGrid API |
| **INotificationModelFactory** | `CreateBase(orgId)` | Create base view model |
|  | `CreateBaseDefault()` | Create default base model |
| **IUserNotificationModelFactory** | 65+ methods | Create & queue specific notifications |

### Common Patterns

#### Pattern 1: Simple User Notification
```csharp
IUserNotificationModelFactory.CreateForgetPasswordNotification(passwordLink, person);
// Internally calls QueueUpNotificationEmail with ForgetPassword template
```

#### Pattern 2: Complex Business Notification
```csharp
IUserNotificationModelFactory.CreateInvoiceDetailNotification(
    organizationId, 
    franchiseeInvoiceList
);
// Builds complex model, attaches resources, queues email
```

#### Pattern 3: Custom Email Body
```csharp
notificationService.QueueUpNotificationDyamicEmail(
    NotificationTypes.PersonalMailForMembers,
    model,
    fromName,
    fromEmail,
    toEmail,
    DateTime.UtcNow,
    customHtmlBody, // Your custom HTML
    organizationRoleUserId
);
```

## Troubleshooting

<!-- CUSTOM SECTION: Add project-specific troubleshooting here -->

### Email Not Sending

**Check:**
1. Is template active? `EmailTemplate.isActive = true`
2. Is notification type enabled? `NotificationType.IsQueuingEnabled = true` and `IsServiceEnabled = true`
3. Is polling agent running? Check scheduled job status
4. Check `NotificationQueue` table for status and attempt count
5. Verify SendGrid API key in settings

**Debug:**
```sql
-- Find stuck notifications
SELECT * FROM NotificationQueue 
WHERE ServiceStatusId = 111 -- Pending
AND NotificationDate < GETDATE()
ORDER BY NotificationDate DESC;

-- Check failed notifications
SELECT nq.*, ne.Subject, ne.Body 
FROM NotificationQueue nq
JOIN NotificationEmail ne ON ne.Id = nq.NotificationEmailId
WHERE ServiceStatusId = 113 -- Failed
ORDER BY nq.CreatedDate DESC;
```

### Email Delivered to Wrong Person

**Common Causes:**
1. **Billing emails:** Check franchisee's `AccountPersonEmail` (overrides person email)
2. **CC logic:** Certain notification types add automatic CCs (see INotificationService.QueueUpNotificationEmail)
3. **Comma-separated emails:** Multiple recipients in `toEmail` parameter

**Fix:**
```csharp
// Override email resolution
notificationService.QueueUpNotificationEmail(
    notificationType,
    model,
    fromName: "Company",
    fromEmail: "noreply@company.com",
    toEmail: "specific@email.com", // Explicit email
    notificationDateTime: DateTime.UtcNow,
    organizationRoleUserId: null
);
```

### Attachments Not Appearing

**Check:**
1. File exists on disk at `Resource.RelativeLocation + "/" + Resource.Name`
2. `NotificationResource` properly linked to `NotificationEmail`
3. File permissions readable by application
4. SendGrid attachment size limits (< 30MB total)

**Debug:**
```csharp
// Verify file before queuing
var filePath = resource.Resource.RelativeLocation + "/" + resource.Resource.Name;
if (!File.Exists(filePath))
    throw new FileNotFoundException("Attachment missing", filePath);
```

### Duplicate Emails Being Sent

**Causes:**
1. Polling agent running multiple instances
2. Missing deduplication check
3. Transaction rollback re-queuing

**Prevention:**
- Payment reminders: Checked via `PaymentMailReminder` table
- Weekly reports: Checked via `WeeklyNotification` table  
- Custom: Add similar deduplication table

### Templates Not Rendering

**Check:**
1. Razor syntax errors (RazorEngine will throw)
2. Model properties exist in ViewModel
3. `@Model.Base.PropertyName` for base model properties

**Example Error:**
```
"The name 'FullName' does not exist in the current context"
→ Check ViewModel has FullName property
```

<!-- END CUSTOM SECTION -->

## Testing

### Unit Test Example

```csharp
[Test]
public void QueueUpNotificationEmail_CreatesQueueEntry()
{
    // Arrange
    var mockRepo = new Mock<IRepository<NotificationQueue>>();
    var service = new NotificationService(
        mockUnitOfWork, 
        mockOrgService, 
        mockSettings, 
        mockClock
    );
    
    var model = new SendLoginCredentialViewModel(baseModel) 
    {
        FullName = "John Doe",
        UserName = "john@example.com",
        Password = "temp123"
    };
    
    // Act
    var result = service.QueueUpNotificationEmail(
        NotificationTypes.SendLoginCredential,
        model,
        1L, // organizationRoleUserId
        DateTime.UtcNow
    );
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal((long)ServiceStatus.Pending, result.ServiceStatusId);
    mockRepo.Verify(x => x.Save(It.IsAny<NotificationQueue>()), Times.Once);
}
```

### Integration Test (Polling Agent)

```csharp
[Test]
public void PollingAgent_SendsPendingNotifications()
{
    // Arrange: Queue a test notification
    var notification = QueueTestNotification();
    
    // Act: Run polling agent
    pollingAgent.PollForNotifications();
    
    // Assert: Check status changed
    var updated = _notificationQueueRepository.Get(notification.Id);
    Assert.Equal((long)ServiceStatus.Success, updated.ServiceStatusId);
    Assert.NotNull(updated.ServicedAt);
}
```

## Performance Considerations

### Queueing Performance
- **Fast:** Queueing is synchronous but typically < 50ms
- **Database writes:** Each queue operation = 1 transaction
- **Batch queueing:** For bulk notifications, use single transaction:

```csharp
_unitOfWork.StartTransaction();
try {
    foreach (var recipient in recipients) {
        _notificationService.QueueUpNotificationEmail(...);
    }
    _unitOfWork.SaveChanges();
} catch {
    _unitOfWork.Rollback();
}
```

### Polling Performance
- **Batch size:** Agent processes all pending (no pagination)
- **Consider:** Add TOP/LIMIT if queue grows large
- **Monitoring:** Track `AttemptCount` histogram for health

### SendGrid Limits
- **Rate limit:** Check SendGrid plan (free = 100/day)
- **Attachments:** Total size < 30MB per email
- **Recipients:** SendGrid limits vary by plan

## Related Modules

- **Core.Application** - Repository, UnitOfWork, Settings
- **Core.Organizations** - Franchisee, Organization data  
- **Core.Billing** - Invoice, Payment entities
- **Core.Users** - Person, User authentication
- **Core.Scheduler** - Job scheduling notifications

## Further Reading

- [Domain Entities Documentation](Domain/.context/OVERVIEW.md)
- [Email Template Syntax Guide](../../../docs/email-templates.md)
- [SendGrid Integration Guide](../../../docs/sendgrid-setup.md)
- [Notification Types Reference](Enum/.context/OVERVIEW.md)
