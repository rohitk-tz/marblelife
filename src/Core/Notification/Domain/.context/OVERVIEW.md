# Core.Notification.Domain - Developer Guide

## What Are These Entities?

The Domain folder contains **6 entity classes** that form the database schema for the notification system. Think of them as the blueprint for storing and managing email notifications.

### The Big Picture

```
NotificationQueue = "Mail Delivery Order"
  └─> NotificationEmail = "The Actual Letter"
       ├─> Recipients = "TO/CC/BCC addresses"
       └─> Resources = "Attachments (PDFs, images)"

EmailTemplate = "Letter Templates" (Razor syntax)
NotificationType = "Types of Letters" (Invoice, Password Reset, etc.)
```

## Entity Quick Reference

| Entity | Purpose | Key Properties |
|--------|---------|----------------|
| **NotificationQueue** | Queue entry with status/scheduling | NotificationDate, ServiceStatusId, AttemptCount |
| **NotificationEmail** | Email content and metadata | FromEmail, Subject, Body, Recipients, Resources |
| **NotificationEmailRecipient** | Single TO/CC/BCC recipient | RecipientEmail, RecipientTypeId (TO/CC/BCC) |
| **NotificationResource** | File attachment link | ResourceId (File FK), NotificationEmailId |
| **EmailTemplate** | Razor email template | Subject, Body (Razor), LanguageId, isActive |
| **NotificationType** | Notification type configuration | IsServiceEnabled, IsQueuingEnabled |

## Common Scenarios

### Scenario 1: Query Pending Notifications

```csharp
// Polling agent query
var pending = _notificationQueueRepository.Fetch(
    x => x.NotificationDate < DateTime.UtcNow
      && x.ServiceStatusId == (long)ServiceStatus.Pending
      && x.NotificationType.IsServiceEnabled,
    includeProperties: "NotificationEmail.Recipients,NotificationEmail.Resources"
);

foreach (var notification in pending) {
    var toEmails = notification.NotificationEmail.Recipients
        .Where(r => r.RecipientTypeId == (long)RecipientType.TO)
        .Select(r => r.RecipientEmail)
        .ToArray();
        
    // Send email...
}
```

### Scenario 2: Create Notification with Attachments

```csharp
// Create the queue entry
var queue = new NotificationQueue {
    NotificationTypeId = (long)NotificationTypes.SendInvoiceDetail,
    NotificationDate = DateTime.UtcNow,
    ServiceStatusId = (long)ServiceStatus.Pending,
    FranchiseeId = franchiseeId,
    IsNew = true,
    DataRecorderMetaData = new DataRecorderMetaData()
};

// Add email content
queue.NotificationEmail = new NotificationEmail {
    EmailTemplateId = template.Id,
    FromEmail = "noreply@company.com",
    FromName = "MarbleLife",
    Subject = "Your Invoice is Ready",
    Body = parsedHtmlBody
};

// Add recipient
queue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = customer.Email,
    RecipientTypeId = (long)RecipientType.TO,
    IsNew = true
});

// Add CC (admin)
queue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = "admin@company.com",
    RecipientTypeId = (long)RecipientType.CC,
    IsNew = true
});

// Add attachment
queue.NotificationEmail.Resources = new List<NotificationResource> {
    new NotificationResource {
        ResourceId = invoicePdf.Id,
        IsNew = true
    }
};

// Save (cascades to children)
_notificationQueueRepository.Save(queue);
_unitOfWork.SaveChanges();
```

### Scenario 3: Multi-Language Template Selection

```csharp
// Get franchisee's language
var franchisee = _franchiseeRepository.Get(franchiseeId);
var languageId = franchisee.LanguageId; // e.g., Spanish = 2

// Find template for notification type and language
var template = _emailTemplateRepository.Get(
    x => x.NotificationTypeId == (long)NotificationTypes.PaymentReminder
      && x.LanguageId == languageId
      && x.isActive
);

// Fallback to default language if not found
if (template == null) {
    template = _emailTemplateRepository.Get(
        x => x.NotificationTypeId == (long)NotificationTypes.PaymentReminder
          && x.LanguageId == 1 // English (default)
          && x.isActive
    );
}
```

### Scenario 4: Check Notification Status

```csharp
// Find notification by ID
var notification = _notificationQueueRepository.Get(notificationId);

switch (notification.ServiceStatusId) {
    case (long)ServiceStatus.Pending:
        Console.WriteLine($"Scheduled for: {notification.NotificationDate}");
        Console.WriteLine($"Attempts: {notification.AttemptCount ?? 0}");
        break;
        
    case (long)ServiceStatus.Success:
        Console.WriteLine($"Sent successfully at: {notification.ServicedAt}");
        break;
        
    case (long)ServiceStatus.Failed:
        Console.WriteLine($"Failed after {notification.AttemptCount} attempts");
        Console.WriteLine($"Last attempt: {notification.ServicedAt}");
        break;
}
```

## Entity Details

### NotificationQueue

**When to Use:** Every notification that needs to be sent.

**Key Fields:**
- `NotificationDate` - When to send (can be future)
- `ServiceStatusId` - Pending (111), Success (112), or Failed (113)
- `AttemptCount` - Number of send attempts (max 3)
- `ServicedAt` - When it was processed
- `FranchiseeId` - For reporting/filtering

**Common Queries:**
```csharp
// Get failed notifications
var failed = _repo.Fetch(x => x.ServiceStatusId == (long)ServiceStatus.Failed);

// Get pending for specific franchisee
var pending = _repo.Fetch(x => 
    x.FranchiseeId == franchiseeId 
    && x.ServiceStatusId == (long)ServiceStatus.Pending
);

// Get today's sent notifications
var sent = _repo.Fetch(x => 
    x.ServicedAt >= DateTime.Today 
    && x.ServiceStatusId == (long)ServiceStatus.Success
);
```

### NotificationEmail

**When to Use:** Created automatically with NotificationQueue (1:1 relationship).

**Key Fields:**
- `FromEmail` / `FromName` - Sender info
- `Subject` / `Body` - Email content (parsed from template)
- `IsDynamicEmail` - True if custom body (not from template)

**Navigation Properties:**
- `Recipients` - List of TO/CC/BCC recipients
- `Resources` - List of file attachments
- `EmailTemplate` - Original template used

### NotificationEmailRecipient

**When to Use:** For each recipient (TO/CC/BCC) of an email.

**RecipientType Values:**
- `TO (127)` - Primary recipient
- `CC (128)` - Carbon copy
- `BCC (129)` - Blind carbon copy

**Example:**
```csharp
// Multiple recipients with different types
notification.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = "customer@example.com",
    RecipientTypeId = (long)RecipientType.TO
});

notification.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = "manager@example.com",
    RecipientTypeId = (long)RecipientType.CC
});

notification.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = "audit@example.com",
    RecipientTypeId = (long)RecipientType.BCC
});
```

### NotificationResource

**When to Use:** When attaching files to email (invoices, reports, images).

**Important:** The `Resource` (File entity) must exist before creating NotificationResource.

**Example:**
```csharp
// 1. Save file first
var pdfFile = new File {
    Name = "Invoice_12345.pdf",
    RelativeLocation = "/uploads/invoices",
    MimeType = "application/pdf",
    Size = 245678
};
_fileRepository.Save(pdfFile);
_unitOfWork.SaveChanges();

// 2. Attach to notification
var resource = new NotificationResource {
    ResourceId = pdfFile.Id,
    NotificationEmailId = notificationEmail.Id
};
```

### EmailTemplate

**When to Use:** Configure email templates (typically done via admin UI or seed data).

**Razor Syntax:**
```html
<!-- Subject Template -->
Invoice #@Model.InvoiceId - Due @Model.DueDate

<!-- Body Template -->
<html>
<body>
    <h1>Hello @Model.FullName!</h1>
    <p>Your invoice for @Model.Franchisee is ready.</p>
    <table>
        @foreach(var item in Model.InvoiceDetailList) {
            <tr>
                <td>@item.Description</td>
                <td>@item.Amount</td>
            </tr>
        }
    </table>
    <p><strong>Total: @Model.TotalAmount</strong></p>
</body>
</html>
```

**Activation:**
```csharp
// Activate template
template.isActive = true;
_emailTemplateRepository.Save(template);

// Deactivate old version
oldTemplate.isActive = false;
_emailTemplateRepository.Save(oldTemplate);

_unitOfWork.SaveChanges();
```

### NotificationType

**When to Use:** Control notification behavior without code changes.

**Example:**
```csharp
// Disable payment reminders temporarily
var paymentReminderType = _notificationTypeRepository.Get(
    x => x.Id == (long)NotificationTypes.PaymentReminder
);
paymentReminderType.IsQueuingEnabled = false; // Stop new queues
paymentReminderType.IsServiceEnabled = false; // Stop sending existing
_notificationTypeRepository.Save(paymentReminderType);
_unitOfWork.SaveChanges();

// Re-enable later
paymentReminderType.IsQueuingEnabled = true;
paymentReminderType.IsServiceEnabled = true;
```

## Database Relationships

### One-to-One
- NotificationQueue ↔ NotificationEmail

### One-to-Many
- NotificationEmail → NotificationEmailRecipient
- NotificationEmail → NotificationResource
- NotificationType → EmailTemplate
- NotificationType → NotificationQueue

### Many-to-One
- NotificationQueue → NotificationType
- NotificationQueue → Organization (Franchisee)
- NotificationEmailRecipient → OrganizationRoleUser
- NotificationResource → File

## Troubleshooting

<!-- CUSTOM SECTION -->

### Issue: Notification Not Sending

**Check:**
1. `NotificationQueue.ServiceStatusId` - Should be Pending (111)
2. `NotificationQueue.NotificationDate` - Should be <= Now
3. `NotificationType.IsServiceEnabled` - Should be true
4. `EmailTemplate.isActive` - Should be true
5. `NotificationEmail.Recipients` - Should have at least one TO recipient

**Query:**
```sql
SELECT nq.Id, nq.NotificationDate, nq.ServiceStatusId, nq.AttemptCount,
       nt.Title, nt.IsServiceEnabled,
       ne.Subject,
       (SELECT COUNT(*) FROM NotificationEmailRecipients WHERE NotificationId = ne.Id AND RecipientTypeId = 127) AS ToCount
FROM NotificationQueues nq
JOIN NotificationTypes nt ON nq.NotificationTypeId = nt.Id
LEFT JOIN NotificationEmails ne ON ne.Id = (SELECT TOP 1 Id FROM NotificationEmails WHERE NotificationQueueId = nq.Id)
WHERE nq.ServiceStatusId = 111 -- Pending
  AND nq.NotificationDate < GETDATE()
ORDER BY nq.NotificationDate;
```

### Issue: Attachments Not Showing

**Check:**
```csharp
// Verify resource link
var resources = _notificationResourceRepository.Fetch(
    x => x.NotificationEmailId == notificationEmail.Id
);

foreach (var resource in resources) {
    var filePath = Path.Combine(
        resource.Resource.RelativeLocation,
        resource.Resource.Name
    );
    
    if (!File.Exists(filePath)) {
        Console.WriteLine($"Missing file: {filePath}");
    } else {
        Console.WriteLine($"Found: {filePath} ({resource.Resource.Size} bytes)");
    }
}
```

### Issue: Wrong Template Language

**Debug:**
```csharp
// Check franchisee language
var franchisee = _franchiseeRepository.Get(franchiseeId);
Console.WriteLine($"Franchisee Language: {franchisee.LanguageId}");

// Check available templates
var templates = _emailTemplateRepository.Fetch(
    x => x.NotificationTypeId == notificationTypeId && x.isActive
);

foreach (var t in templates) {
    Console.WriteLine($"Template {t.Id}: LanguageId={t.LanguageId}, Title={t.Title}");
}
```

<!-- END CUSTOM SECTION -->

## Best Practices

1. **Always Use Transactions:** When creating NotificationQueue with children
2. **Eager Load:** Include navigation properties to avoid N+1 queries
3. **Check Active Flags:** Verify `isActive` and `IsServiceEnabled` before queuing
4. **Handle Nulls:** `AttemptCount`, `ServicedAt`, and `FranchiseeId` can be null
5. **Validate Recipients:** Ensure at least one TO recipient exists
6. **File Existence:** Check file exists before attaching

## Related Documentation

- [Parent Module](../.context/OVERVIEW.md)
- [Enumerations](../Enum/.context/OVERVIEW.md)
- [Service Implementations](../Impl/.context/OVERVIEW.md)
