# Core.Notification.Enum - Developer Guide

## What's in This Folder?

Four enumeration types that define **type-safe constants** for the notification system:

| Enum | Values | Purpose |
|------|--------|---------|
| **NotificationTypes** | 65 types | All notification categories (invoices, passwords, jobs, etc.) |
| **ServiceStatus** | 3 states | Processing status (Pending → Success/Failed) |
| **RecipientType** | 3 types | Email recipient types (TO/CC/BCC) |
| **NotificationResourceType** | 2 types | Attachment vs embedded resources |

## Quick Reference

### NotificationTypes (Most Used)

**Common Types:**
```csharp
// Authentication
NotificationTypes.ForgetPassword = 1
NotificationTypes.SendLoginCredential = 2

// Billing
NotificationTypes.SendInvoiceDetail = 3
NotificationTypes.PaymentReminder = 4
NotificationTypes.PaymentConfirmation = 7
NotificationTypes.LateFeeReminderForPayment = 6

// Reports
NotificationTypes.WeeklyLateFeeNotification = 9
NotificationTypes.WeeklyUnpaidInvoiceNotification = 10

// Jobs/Scheduling
NotificationTypes.NewJobNotificationToUser = 25
NotificationTypes.NewJobNotificationToTech = 26

// Customer Engagement
NotificationTypes.CustomerFeedbackRequest = 11
NotificationTypes.BeforeAfterImages = 34
```

**By Category:**

*Authentication (1-2):*
- ForgetPassword
- SendLoginCredential
- SendLoginCredentialWithSetupGuide (21)

*Billing (3-8, 50-52):*
- SendInvoiceDetail, PaymentReminder, PaymentConfirmation
- LateFeeReminderForPayment, LateFeeReminderForSalesData
- MailToCustomerForInvoice, MailToCustomerForSignedInvoice

*Sales Data (5, 15-20):*
- SalesDataUploadReminder
- AnnualUpload* (16-20)

*Reports (9-14, 33):*
- WeeklyLateFeeNotification, WeeklyUnpaidInvoiceNotification
- MonthlyReports, ArReportNotification

*Job Scheduling (25-32, 48):*
- NewJobNotificationToUser/Tech
- CancleJobNotificationToTech, UpdateJobNotificationToTech
- UrgentJobNotificationToUser

### ServiceStatus (3 values)

```csharp
ServiceStatus.Pending = 111   // Waiting to be sent
ServiceStatus.Success = 112   // Sent successfully
ServiceStatus.Failed = 113    // Failed after 3 attempts
```

**Status Flow:**
```
[New] → Pending → [Attempt 1] → Success ✓
                      ↓ fail
                  → [Attempt 2] → Success ✓
                      ↓ fail
                  → [Attempt 3] → Success ✓
                      ↓ fail
                  → Failed ✗
```

### RecipientType (3 values)

```csharp
RecipientType.TO = 127    // Primary recipient
RecipientType.CC = 128    // Carbon copy
RecipientType.BCC = 129   // Blind carbon copy (not implemented)
```

### NotificationResourceType (2 values)

```csharp
NotificationResourceType.Attachment = 121         // File attachment
NotificationResourceType.EmbeddedResource = 122   // Inline image (not fully implemented)
```

## Usage Examples

### Example 1: Queue Notification by Type

```csharp
public void SendPasswordReset(string email, string resetLink) {
    var model = new UserForgetPasswordNotificationViewModel(baseModel) {
        FullName = user.FullName,
        PasswordLink = resetLink
    };
    
    // Use enum for type safety
    notificationService.QueueUpNotificationEmail(
        NotificationTypes.ForgetPassword,  // ← Type-safe enum
        model,
        organizationRoleUserId
    );
}
```

### Example 2: Check Status

```csharp
public string GetNotificationStatus(long notificationId) {
    var notification = _repo.Get(notificationId);
    
    switch (notification.ServiceStatusId) {
        case (long)ServiceStatus.Pending:
            return $"Scheduled for {notification.NotificationDate}";
        case (long)ServiceStatus.Success:
            return $"Sent successfully at {notification.ServicedAt}";
        case (long)ServiceStatus.Failed:
            return $"Failed after {notification.AttemptCount} attempts";
        default:
            return "Unknown status";
    }
}
```

### Example 3: Add Multiple Recipients

```csharp
public void SendInvoiceWithCCs(Invoice invoice) {
    var notification = CreateNotificationQueue(invoice);
    
    // Primary recipient (TO)
    notification.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
        RecipientEmail = customer.Email,
        RecipientTypeId = (long)RecipientType.TO
    });
    
    // CC to admin
    notification.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
        RecipientEmail = "admin@company.com",
        RecipientTypeId = (long)RecipientType.CC
    });
    
    // CC to franchisee owner
    notification.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
        RecipientEmail = franchisee.OwnerEmail,
        RecipientTypeId = (long)RecipientType.CC
    });
    
    _repo.Save(notification);
}
```

### Example 4: Filter by Notification Type

```csharp
// Get all billing notifications
var billingTypes = new[] {
    (long)NotificationTypes.SendInvoiceDetail,
    (long)NotificationTypes.PaymentReminder,
    (long)NotificationTypes.PaymentConfirmation
};

var billingNotifications = _repo.Fetch(
    x => billingTypes.Contains(x.NotificationTypeId)
      && x.ServiceStatusId == (long)ServiceStatus.Success
      && x.ServicedAt >= DateTime.Today
);

Console.WriteLine($"Sent {billingNotifications.Count()} billing emails today");
```

### Example 5: Retry Failed Notifications

```csharp
public void RetryFailedNotifications() {
    var failed = _notificationQueueRepo.Fetch(
        x => x.ServiceStatusId == (long)ServiceStatus.Failed
          && x.ServicedAt > DateTime.UtcNow.AddDays(-7) // Last 7 days
    );
    
    foreach (var notification in failed) {
        // Reset to retry
        notification.ServiceStatusId = (long)ServiceStatus.Pending;
        notification.AttemptCount = 0;
        notification.NotificationDate = DateTime.UtcNow; // Send ASAP
        
        _notificationQueueRepo.Save(notification);
    }
    
    _unitOfWork.SaveChanges();
    Console.WriteLine($"Re-queued {failed.Count()} failed notifications");
}
```

## Enum Value Lookup

### Find Notification Type by Category

**Authentication:**
```csharp
var authTypes = new[] {
    NotificationTypes.ForgetPassword,
    NotificationTypes.SendLoginCredential,
    NotificationTypes.SendLoginCredentialWithSetupGuide
};
```

**Billing & Payments:**
```csharp
var billingTypes = new[] {
    NotificationTypes.SendInvoiceDetail,
    NotificationTypes.PaymentReminder,
    NotificationTypes.PaymentConfirmation,
    NotificationTypes.LateFeeReminderForPayment,
    NotificationTypes.LateFeeReminderForSalesData
};
```

**Job Scheduling:**
```csharp
var jobTypes = new[] {
    NotificationTypes.NewJobNotificationToUser,
    NotificationTypes.NewJobNotificationToTech,
    NotificationTypes.CancleJobNotificationToTech,
    NotificationTypes.UpdateJobNotificationToTech,
    NotificationTypes.UrgentJobNotificationToUser
};
```

**Weekly/Monthly Reports:**
```csharp
var reportTypes = new[] {
    NotificationTypes.WeeklyLateFeeNotification,
    NotificationTypes.WeeklyUnpaidInvoiceNotification,
    NotificationTypes.MonthlyReviewNotification,
    NotificationTypes.ArReportNotification
};
```

## Tips & Gotchas

### Tip 1: Always Cast to Long

Enums must be cast to `long` for database operations:
```csharp
// ✅ Correct
var pending = (long)ServiceStatus.Pending;
var query = _repo.Fetch(x => x.ServiceStatusId == pending);

// ❌ Won't compile (can't compare enum to long directly)
var query = _repo.Fetch(x => x.ServiceStatusId == ServiceStatus.Pending);
```

### Tip 2: Use Enum Names in Logging

```csharp
// ✅ Good: Readable log
_logger.Info($"Queuing {NotificationTypes.PaymentReminder} for user {userId}");
// Output: "Queuing PaymentReminder for user 123"

// ❌ Bad: Magic number
_logger.Info($"Queuing notification type {4} for user {userId}");
// Output: "Queuing notification type 4 for user 123" (unclear)
```

### Tip 3: Check Enum Exists in Database

Not all enum values may be configured in the database:
```csharp
public void QueueNotification(NotificationTypes type) {
    // Verify configuration exists
    var config = _notificationTypeRepo.Get(x => x.Id == (long)type);
    
    if (config == null) {
        throw new InvalidOperationException($"NotificationType {type} not configured");
    }
    
    if (!config.IsQueuingEnabled) {
        _logger.Warn($"Queuing disabled for {type}");
        return; // Skip
    }
    
    // Proceed with queuing...
}
```

### Gotcha 1: Typo in Enum Name

```csharp
// Note the typo: "Cancle" instead of "Cancel"
NotificationTypes.CancleJobNotificationToTech = 27
```
This is a legacy typo. Use the enum as-is to maintain compatibility with existing data.

### Gotcha 2: Non-Contiguous Values

Enum values are not sequential (gaps exist):
```csharp
// Values jump from 20 to 21 to 22, then later to 25, etc.
AnnualUploadRejected = 20,
SendLoginCredentialWithSetupGuide = 21,
DocumentUploadNotification = 22,
// ... gap ...
NewJobNotificationToUser = 25,
```

Don't assume `NotificationTypes.SomeType + 1` is the next type!

### Gotcha 3: BCC Not Implemented

`RecipientType.BCC (129)` exists in the enum but EmailDispatcher doesn't support BCC sending yet.

## Troubleshooting

### Issue: "No Email template found"

**Cause:** NotificationType exists but EmailTemplate is missing or inactive.

**Fix:**
```sql
-- Check template exists
SELECT * FROM EmailTemplates 
WHERE NotificationTypeId = 4 -- PaymentReminder
  AND isActive = 1;

-- If missing, insert template
INSERT INTO EmailTemplates (NotificationTypeId, Subject, Body, isActive, LanguageId)
VALUES (4, 'Payment Reminder', '<html>...</html>', 1, 1);
```

### Issue: Wrong Status After Sending

**Cause:** Polling agent may have failed to update status.

**Fix:**
```csharp
// Manual status update
notification.ServiceStatusId = (long)ServiceStatus.Success;
notification.ServicedAt = DateTime.UtcNow;
_repo.Save(notification);
_unitOfWork.SaveChanges();
```

## Related Documentation

- [Parent Module](../.context/OVERVIEW.md)
- [Domain Entities](../Domain/.context/OVERVIEW.md)
- [Service Implementations](../Impl/.context/OVERVIEW.md)
