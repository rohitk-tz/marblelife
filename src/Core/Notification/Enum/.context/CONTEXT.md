# Core.Notification.Enum - AI/Agent Context

## Overview

The Enum folder contains **4 enumeration types** that define the notification system's type-safe constants. These enums prevent magic numbers and provide compile-time type safety for notification operations.

## Enumerations

### 1. NotificationTypes.cs (65 values)

**Purpose:** Defines all supported notification types in the system. Each value maps to a NotificationType.Id in the database.

**Full Enumeration:**
```csharp
public enum NotificationTypes
{
    // Authentication & User Management (1-2)
    ForgetPassword = 1,
    SendLoginCredential = 2,
    SendLoginCredentialWithSetupGuide = 21,
    
    // Billing & Invoicing (3-8, 50-52)
    SendInvoiceDetail = 3,
    PaymentReminder = 4,
    PaymentConfirmation = 7,
    LateFeeReminderForPayment = 6,
    LateFeeReminderForSalesData = 8,
    MailToCustomerForInvoice = 50,
    MailToCustomerForSignedInvoice = 51,
    MailToSalesRepForSignedInvoice = 52,
    
    // Sales Data Management (5, 15-20)
    SalesDataUploadReminder = 5,
    MonthlySalesUploadNotification = 15,
    AnnualUploadFailNotification = 16,
    AnnualUploadParsedNotification = 17,
    AnnualUploadNotificationToAdmin = 18,
    AnnualUploadApproved = 19,
    AnnualUploadRejected = 20,
    
    // Weekly/Monthly Reports (9-14, 33, 63)
    WeeklyLateFeeNotification = 9,
    WeeklyUnpaidInvoiceNotification = 10,
    ListCustomerMonthlyNotification = 12,
    MonthlyReviewNotification = 13,
    MonthlyMailChimpReport = 14,
    ArReportNotification = 33,
    WeeklyNotificationOfPhotoManagement = 63,
    
    // Customer Feedback & Reviews (11, 54-56)
    CustomerFeedbackRequest = 11,
    PostJobFeedbackToCustomer = 54,
    PostJobFeedbackToSalesRep = 55,
    PostJobFeedbackToAdmin = 56,
    
    // Document Management (22-24)
    DocumentUploadNotification = 22,
    DocumentExpiryNotification = 23,
    DocumentUploadNotificationToFranchisee = 24,
    
    // Job/Scheduling Notifications (25-32, 48)
    NewJobNotificationToUser = 25,
    NewJobNotificationToTech = 26,
    CancleJobNotificationToTech = 27,
    UpdateJobNotificationToTech = 28,
    NewJobNotificationToUserOnDay = 30,
    NewJobNotificationToUserReassigned = 31,
    UrgentJobNotificationToUser = 32,
    DeletionJobNotificationToTech = 48,
    PostEstimatetoCustomer = 29,
    
    // Before/After Images (34, 39-40, 42)
    BeforeAfterImages = 34,
    BeforeAfterBestPairMail = 39,
    LOCALSITEIMAGEGALLERY = 40,
    BeforeAfterImageForFA = 42,
    
    // Customer Communication (35, 37-38, 53)
    InvoiceImages = 35,
    NewCustomerMail = 37,
    UpdateCustomerMail = 38,
    MailToCustomerForPostJobCompletion = 53,
    
    // Financial & Loans (36)
    FranchiseeLoanCompletion = 36,
    
    // Administrative (41, 43-44, 46-47, 49, 57-58)
    NonResidentalBuildingMail = 41,
    NotificationToFA = 43,
    RenewableMailForFranchiseeBefore9Month = 44,
    RenewableMailForFranchiseeBefore8Month = 45,
    MeetingMailForMemebers = 46,
    PersonalMailForMemebers = 47,
    MailToFranchiseeAdminForRPID = 49,
    WebLeadsMail = 57,
    NotificationToSuperAdminIfFranchiseePriceExceedsTheBulkCorporatePrice = 58,
    
    // Payroll Reports (59-62)
    SendLinkToFranchiseeOwnerAndSchedulerForPayrollReport = 59,
    SendLinkToSalesRepAndTechnicianForPayrollReport = 60,
    PhotoReportEmailToFranchiseeOwnerAndScheduler = 61,
    PhotoReportEmailToSalesRepandTechnician = 62
}
```

**Categories:**
- **Authentication:** User login, password reset
- **Billing:** Invoices, payments, late fees, confirmations
- **Sales Data:** Upload reminders, annual audits, approvals
- **Reports:** Weekly/monthly summaries for admins
- **Customer Engagement:** Feedback requests, job completion
- **Scheduling:** Job notifications to technicians and customers
- **Documents:** Upload notifications, expiry alerts
- **Images:** Before/after photo notifications
- **Administrative:** Franchise management, web leads, payroll

**Usage Pattern:**
```csharp
// Queue notification by type
notificationService.QueueUpNotificationEmail(
    NotificationTypes.SendInvoiceDetail,  // Type-safe enum
    invoiceModel,
    organizationRoleUserId
);

// Switch logic based on type
switch (notificationType) {
    case NotificationTypes.PaymentReminder:
        // Add admin CC
        break;
    case NotificationTypes.LateFeeReminderForPayment:
        // Add franchisee owner CC
        break;
}
```

**Database Mapping:**
Each enum value maps to `NotificationType.Id` in the database. The NotificationType table contains:
- Configuration flags (IsServiceEnabled, IsQueuingEnabled)
- Human-readable Title and Description
- Associated EmailTemplate records

---

### 2. ServiceStatus.cs (3 values)

**Purpose:** Tracks the processing status of queued notifications.

**Values:**
```csharp
public enum ServiceStatus
{
    Pending = 111,   // Awaiting processing by polling agent
    Success = 112,   // Successfully sent via SendGrid
    Failed = 113     // Failed after max retry attempts (3)
}
```

**Lifecycle:**
```
Pending (111) → [Send Attempt] → Success (112)
     ↓
     └─[Fail] → Retry (still Pending) → [Max attempts] → Failed (113)
```

**Usage Pattern:**
```csharp
// Query pending notifications
var pending = _repo.Fetch(
    x => x.ServiceStatusId == (long)ServiceStatus.Pending
      && x.NotificationDate < DateTime.UtcNow
);

// Update status after sending
notification.ServiceStatusId = (long)ServiceStatus.Success;
notification.ServicedAt = DateTime.UtcNow;
_repo.Save(notification);
```

**Business Rules:**
- **Pending:** Initial state when queued
- **Success:** Marked after SendGrid returns HTTP 200
- **Failed:** Marked after 3 failed attempts (see NotificationQueue.AttemptCount)

---

### 3. RecipientType.cs (3 values)

**Purpose:** Defines the type of email recipient (TO/CC/BCC).

**Values:**
```csharp
public enum RecipientType
{
    TO = 127,    // Primary recipient
    CC = 128,    // Carbon copy (visible to all)
    BCC = 129    // Blind carbon copy (hidden from others)
}
```

**Usage Pattern:**
```csharp
// Add TO recipient
notification.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = customer.Email,
    RecipientTypeId = (long)RecipientType.TO
});

// Add CC recipient (e.g., admin)
notification.Recipients.Add(new NotificationEmailRecipient {
    RecipientEmail = "admin@company.com",
    RecipientTypeId = (long)RecipientType.CC
});

// Query TO recipients only
var toEmails = notification.Recipients
    .Where(x => x.RecipientTypeId == (long)RecipientType.TO)
    .Select(x => x.RecipientEmail)
    .ToArray();
```

**Auto-CC Logic (INotificationService):**
Certain notification types automatically add CC recipients:
- **Billing notifications:** Admin email added as CC
- **Late fees:** Franchisee owner added as CC
- **Before/After images:** Custom CC from model
- **Specific franchisees:** Special recipient email as CC

**SendGrid Implementation:**
- TO recipients → `emailMessage.AddTo()`
- CC recipients → `emailMessage.AddCc()`
- BCC recipients → Not currently implemented (future enhancement)

---

### 4. NotificationResourceType.cs (2 values)

**Purpose:** Defines how file attachments are included in emails.

**Values:**
```csharp
public enum NotificationResourceType
{
    Attachment = 121,        // Standard email attachment
    EmbeddedResource = 122   // Inline embedded image (Content-ID)
}
```

**Usage Pattern:**
```csharp
// Standard attachment (PDF, Excel, etc.)
var resource = new NotificationResource {
    ResourceId = pdfFile.Id,
    // ResourceType assumed as Attachment (default)
};

// Embedded image (e.g., company logo)
var logoResource = new NotificationResource {
    ResourceId = logoFile.Id,
    // Would use EmbeddedResource type for inline images
};
```

**Implementation Note:**
Currently, all resources are treated as **Attachment (121)** in EmailDispatcher. The EmbeddedResource type is defined but not fully implemented. Future enhancement would use Content-ID for inline images:

```html
<!-- Future embedded resource usage -->
<img src="cid:logo.png" alt="Company Logo" />
```

**SendGrid API:**
- Attachment: `attachment.Disposition = "attachment"`
- EmbeddedResource: `attachment.Disposition = "inline"` + `Content-Id` header

---

## Enum Usage Best Practices

### 1. Type Safety
```csharp
// ✅ Good: Type-safe enum
notificationService.QueueUpNotificationEmail(
    NotificationTypes.PaymentReminder,
    model,
    userId
);

// ❌ Bad: Magic number
notificationService.QueueUpNotificationEmail(
    (NotificationTypes)4,  // What is 4?
    model,
    userId
);
```

### 2. Casting to Long
All enums cast to `long` for database storage:
```csharp
// Cast to long for database queries
var templateId = (long)NotificationTypes.ForgetPassword;
var template = _repo.Get(x => x.NotificationTypeId == templateId);

// Cast to long for database FK
notification.ServiceStatusId = (long)ServiceStatus.Pending;
```

### 3. Switch Statements
```csharp
// Exhaustive switch with all cases
switch (notificationType) {
    case NotificationTypes.ForgetPassword:
        // Handle password reset
        break;
    case NotificationTypes.SendInvoiceDetail:
        // Handle invoice
        break;
    default:
        throw new NotImplementedException($"Unhandled: {notificationType}");
}
```

### 4. Enum Extension Methods
```csharp
// Example extension method
public static class NotificationTypesExtensions {
    public static bool IsBillingNotification(this NotificationTypes type) {
        return type == NotificationTypes.SendInvoiceDetail
            || type == NotificationTypes.PaymentReminder
            || type == NotificationTypes.PaymentConfirmation
            || type == NotificationTypes.LateFeeReminderForPayment;
    }
}

// Usage
if (notificationType.IsBillingNotification()) {
    // Add admin CC
}
```

## Database-Enum Synchronization

### Lookup Tables
Three enums map to **Lookup** table (common enum storage):
- ServiceStatus (111-113)
- RecipientType (127-129)
- NotificationResourceType (121-122)

### NotificationType Table
NotificationTypes enum maps to dedicated **NotificationType** table with additional metadata:
```sql
CREATE TABLE NotificationTypes (
    Id BIGINT PRIMARY KEY,
    Title NVARCHAR(255),
    Description NVARCHAR(MAX),
    IsServiceEnabled BIT,
    IsQueuingEnabled BIT
);

-- Seed data
INSERT INTO NotificationTypes (Id, Title, IsServiceEnabled, IsQueuingEnabled)
VALUES (1, 'Forget Password', 1, 1),
       (2, 'Send Login Credential', 1, 1),
       (3, 'Send Invoice Detail', 1, 1);
```

### Maintaining Sync
When adding new notification types:
1. Add enum value to `NotificationTypes.cs`
2. Add corresponding row to `NotificationTypes` table
3. Create `EmailTemplate` records for each language
4. Test with both queuing and sending

## Integration Points

### Used By
- **NotificationService:** Validates NotificationTypes against database configuration
- **NotificationPollingAgent:** Filters by ServiceStatus.Pending
- **EmailDispatcher:** Groups recipients by RecipientType
- **UserNotificationModelFactory:** All 65+ methods use NotificationTypes

### Dependencies
- **Core.Application.Domain.Lookup:** RecipientType, ServiceStatus, NotificationResourceType stored in Lookup table
- **Core.Notification.Domain.NotificationType:** NotificationTypes enum maps to this table

## Edge Cases & Validation

### NotificationTypes
- **Range:** 1-63 (non-contiguous, some numbers skipped)
- **Typos:** Note "CancleJobNotificationToTech" (typo in enum name)
- **Validation:** Ensure NotificationType.Id exists in database before queuing

### ServiceStatus
- **State Machine:** Only transitions: Pending → Success, Pending → Failed
- **No Rollback:** Cannot transition Failed → Pending (requires manual reset)

### RecipientType
- **BCC Not Implemented:** Enum exists but EmailDispatcher doesn't handle BCC
- **Multiple TO Recipients:** SendGrid limitation - only first TO used, rest as CC

### NotificationResourceType
- **EmbeddedResource Not Implemented:** Enum exists but feature incomplete

## Related Documentation
- [Parent Module](../.context/CONTEXT.md)
- [Domain Entities](../Domain/.context/CONTEXT.md)
- [Service Implementations](../Impl/.context/CONTEXT.md)
