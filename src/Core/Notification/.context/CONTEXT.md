# Core.Notification Module - AI/Agent Context

## Architectural Overview

The Notification module is a **comprehensive email notification system** implementing a **queue-based architecture with polling agents**. It manages all email communications in the MarbleLife system including transactional emails, reminders, reports, and document notifications.

### Design Pattern: Producer-Consumer with Queue Polling

```
┌─────────────────┐         ┌──────────────────┐         ┌─────────────────┐
│  Service Layer  │ ──────> │ Notification     │ ──────> │  Polling Agent  │
│  (Producers)    │ Queues  │ Queue (Storage)  │ Polls   │  (Consumer)     │
└─────────────────┘         └──────────────────┘         └─────────────────┘
                                     │                            │
                                     ▼                            ▼
                            ┌──────────────────┐         ┌─────────────────┐
                            │ NotificationEmail│         │ Email Dispatcher│
                            │ + Recipients     │         │  (SendGrid)     │
                            │ + Resources      │         └─────────────────┘
                            └──────────────────┘
```

**Core Components:**
1. **Service Layer** - Queues notifications with templates and recipients
2. **Notification Queue** - Persistent storage with scheduling and retry logic  
3. **Polling Agents** - Background workers that process queued notifications
4. **Email Dispatcher** - SMTP integration via SendGrid API
5. **Template System** - Dynamic email templates with Razor engine

## Data Flow

### Queueing Flow (Write Path)
```
1. Business Logic → INotificationService.QueueUpNotificationEmail()
2. Load EmailTemplate by NotificationTypes enum
3. Apply Razor templating with ViewModel
4. Create NotificationQueue + NotificationEmail + Recipients
5. Attach resources (files) if provided
6. Persist to database with ServiceStatus.Pending
```

### Processing Flow (Read Path)
```
1. Polling Agent runs on schedule (INotificationPollingAgent)
2. Query pending notifications (NotificationDate < Now, Status = Pending)
3. For each notification:
   - Extract recipients (TO/CC/BCC)
   - Call IEmailDispatcher.SendEmail() with SendGrid
   - Update ServiceStatus (Success/Failed)
   - Increment AttemptCount (max 3 retries)
4. Mark as serviced with ServicedAt timestamp
```

## Public Interfaces

### Core Service Interface

#### INotificationService
Primary interface for queueing notifications.

**Methods:**
- `QueueUpNotificationEmail<T>(NotificationTypes, T model, long orgRoleUserId, DateTime?, List<NotificationResource>)`
  - **Purpose:** Queue notification to specific user with template model
  - **Returns:** NotificationQueue entity
  - **Side Effects:** Persists to database immediately via IRepository
  - **Email Resolution:** Looks up user email, falls back to franchisee AccountPersonEmail/ContactEmail
  
- `QueueUpNotificationEmail<T>(NotificationTypes, T model, string fromName, string fromEmail, string toEmail, DateTime, long?, List<NotificationResource>, string recipientEmail)`
  - **Purpose:** Queue notification with explicit sender/recipient
  - **Returns:** NotificationQueue entity  
  - **Side Effects:** Handles CC logic based on notification type, supports multi-language templates
  - **Special Logic:** 
    - Injects email signatures for specific notification types
    - Adds admin CC for payment/invoice notifications
    - Supports comma-separated email lists
  
- `QueueUpNotificationDyamicEmail<T>(NotificationTypes, T model, string fromName, string fromEmail, string toEmail, DateTime, string body, long?, List<NotificationResource>, string recipientEmail)`
  - **Purpose:** Queue notification with custom body (bypasses template)
  - **Returns:** NotificationQueue entity
  - **Side Effects:** Marks email as IsDynamicEmail = true

**Dependencies:**
- `IRepository<NotificationQueue>`
- `IRepository<EmailTemplate>`
- `IOrganizationRoleUserInfoService`
- `ISettings` (SMTP config, company info)

### Polling Agents

#### INotificationPollingAgent
Processes queued notifications on schedule.

**Methods:**
- `PollForNotifications()`
  - **Purpose:** Main polling loop that sends pending notifications
  - **Algorithm:**
    1. Fetch notifications where (NotificationDate < Now AND Status = Pending AND IsServiceEnabled)
    2. For each: extract TO recipients, call EmailDispatcher, update status
    3. Retry up to 3 times on failure
  - **Side Effects:** Updates ServiceStatus, ServicedAt, AttemptCount

#### IWeeklyNotificationPollingAgent
Generates weekly summary reports.

**Methods:**
- `CreateWeeklyNotification()`
  - **Purpose:** Send weekly late fee and unpaid invoice reports
  - **Output:** Excel attachments with franchisee-wise summaries
  - **Runs:** Based on Settings.WeeklyReminderDay (day of week)

#### IPaymentReminderPollingAgent
Payment-related notifications.

**Methods:**
- `CreateNotificationReminderForPayment()` - Daily payment reminders for overdue invoices
- `CreatePaymentConfirmationNotification(Invoice, Payment, long)` - Confirmation on payment receipt
- `CreateLoanCompletionNotification(FranchiseeLoan)` - Loan payoff notification

#### IDocumentNotificationPollingAgent
Document upload and expiry notifications.

**Methods:**
- `CreateDocumentUploadNotification(string fileName, ICollection<long> franchiseeIds, long? createdBy)` - Notify franchisees of new documents
- `SendExpiryNotification()` - Alert for documents expiring within 3 days

### Email Dispatcher

#### IEmailDispatcher
Low-level email sending via SendGrid.

**Methods:**
- `SendEmail(string body, string subject, string fromName, string fromEmail, string[] ccEmails, IEnumerable<NotificationResource> resources, params string[] toEmail)`
  - **Purpose:** Send email via SendGrid API with attachments
  - **Implementation:** Uses SendGridClient with async/await
  - **Attachments:** Converts files to Base64, supports inline disposition
  - **Priority:** Sets "Urgent" header
  - **Error Handling:** Logs failures but doesn't throw

### Notification Factories

#### INotificationModelFactory
Creates base email view models.

**Methods:**
- `CreateBase(long organizationId)` - Base model with franchisee-specific data
- `CreateBaseDefault()` - Base model with system defaults

#### IUserNotificationModelFactory
Creates and queues all notification types (65+ notification types).

**Key Methods (Sample):**
- `CreateForgetPasswordNotification(string passwordLink, Person)`
- `CreateInvoiceDetailNotification(long orgId, IList<FranchiseeInvoice>)`
- `CreatePaymentReminderNotification(IList<FranchiseeInvoice>, Franchisee)`
- `CreateWeeklyNotification(File, IEnumerable<FranchiseeInvoice>, DateTime, DateTime, NotificationTypes)`
- `SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee)`
- `BeforeAfterImageNotificationtoCustomer(BeforeAfterImageMailViewModel, File, NotificationTypes)`

**Pattern:** All methods internally call `INotificationService.QueueUpNotificationEmail()` with typed ViewModels

### Domain-Specific Services

#### IAnnualAuditNotificationService
Annual sales data upload notifications.

**Methods:**
- `CreateAnnualUploadNotification(AnnualSalesDataUpload)` - Notify on upload completion
- `CreateReviewActionNotification(AnnualSalesDataUpload, bool isAccept)` - Approval/rejection notifications

#### IInvoiceNotificationService
Invoice generation notifications.

**Methods:**
- `CreateInvoiceDetailNotification(IList<FranchiseeInvoice>, long franchiseeId)` - Send invoice details

#### ILateFeeNotificationService
Late fee notifications for payments and sales data.

**Methods:**
- `CreateLateFeeNotification(InvoiceItem, long organizationId, DateTime)` - Notify on late fee generation

#### ICustomerFeedbackAPIRecordService
Customer feedback request integration.

**Methods:**
- `SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee)` - Send feedback survey

## Critical Types

### NotificationTypes (Enum)
Defines 65+ notification types (see Enum/NotificationTypes.cs).

**Categories:**
- **Authentication:** ForgetPassword (1), SendLoginCredential (2)
- **Billing:** SendInvoiceDetail (3), PaymentReminder (4), PaymentConfirmation (7), LateFeeReminder (6, 8)
- **Reporting:** WeeklyLateFeeNotification (9), WeeklyUnpaidInvoiceNotification (10), ArReportNotification (33)
- **Sales:** SalesDataUploadReminder (5), AnnualUpload* (16-20)
- **Documents:** DocumentUploadNotification (22), DocumentExpiryNotification (23)
- **Scheduling:** NewJobNotificationToUser (25), NewJobNotificationToTech (26), CancelJob (27), UpdateJob (28)
- **Customer:** CustomerFeedbackRequest (11), BeforeAfterImages (34), InvoiceImages (35)

### ServiceStatus (Enum)
- `Pending (111)` - Awaiting processing
- `Success (112)` - Successfully sent
- `Failed (113)` - Failed after max retries

### RecipientType (Enum)
- `TO (127)` - Primary recipient
- `CC (128)` - Carbon copy
- `BCC (129)` - Blind carbon copy

## Integration Points

### External Dependencies
- **SendGrid API** - Email delivery (API key in Settings.SmtpEmailApiKey)
- **RazorEngine** - Email template parsing (Razor.Parse)
- **File System** - Attachment storage at MediaLocationHelper paths

### Internal Dependencies
```
Core.Notification
├─> Core.Application (IRepository, IUnitOfWork, IClock, ISettings)
├─> Core.Organizations (Franchisee, OrganizationRoleUser)
├─> Core.Billing (Invoice, Payment, InvoiceItem)
├─> Core.Users (Person, User)
├─> Core.Scheduler (JobScheduler, JobEstimateEditModel)
├─> Core.Sales (SalesDataUpload, AnnualSalesDataUpload)
├─> Core.Review (ReviewAPIResponseModel)
└─> Core.Geo (AddressViewModel)
```

### Database Tables
- `NotificationQueue` - Queue storage with scheduling
- `NotificationEmail` - Email content and metadata
- `NotificationEmailRecipient` - TO/CC/BCC recipients
- `NotificationResource` - File attachments
- `EmailTemplate` - Template definitions
- `NotificationType` - Notification configurations
- `EmailSignatures` - User-specific signatures
- `PaymentMailReminder` - Deduplication for payment reminders
- `WeeklyNotification` - Deduplication for weekly reports

## Configuration & Settings

### Required Settings (ISettings)
- `SmtpEmailApiKey` - SendGrid API key
- `FromEmail` - Default sender email
- `CompanyName` - Default sender name
- `CCToAdmin` - Admin CC email
- `RecipientEmail` - Special recipient email
- `SendWeeklyReminder` - Enable weekly notifications
- `SendWeekyLateFeeNotification` - Enable late fee reports
- `SendWeekyUnpaidInvoicesNotification` - Enable unpaid invoice reports
- `WeeklyReminderDay` - Day of week for weekly notifications (0-6)
- `SendExpiryNotification` - Enable document expiry alerts

## Edge Cases & Gotchas

### Email Resolution Logic
For billing notifications (invoices, payments, late fees), email is resolved in this order:
1. `AccountPersonEmail` from franchisee
2. `ContactEmail` from franchisee
3. Person's primary email (fallback)

### Multi-Language Support
Templates are selected based on franchisee's `LanguageId`. Falls back to default language if not found.

### Signature Injection
Custom email signatures are injected for specific notification types:
- Invoice notifications
- Payment notifications  
- Job scheduling notifications
- Customer communications

Signature lookup: `EmailSignatures` table where `IsDefault = true` and `IsActive = true` and `Person.Email = fromEmail`

### CC Logic by Notification Type
- **Billing notifications:** Add admin CC (`Settings.CCToAdmin`)
- **Late fees:** Add franchisee owner email as CC
- **Specific franchisees (Detroit, Philadelphia, Pittsburgh, Grand Rapids):** Add recipient email as CC for job notifications
- **Before/After images:** Add custom CC from model

### Retry Logic
- Max 3 attempts per notification
- After 3 failures, status set to `Failed`
- No automatic retry scheduling (manual intervention required)

### Deduplication
- **Payment reminders:** `PaymentMailReminder` table prevents duplicate reminders for same invoice on same date
- **Weekly notifications:** `WeeklyNotification` table prevents duplicate reports for same week

### Dynamic Email Bodies
When using `QueueUpNotificationDyamicEmail()`, the body is provided directly (not from template). Still supports Razor templating and signature injection.

## File Structure

```
Notification/
├── .context/                   # This documentation
├── I*.cs (12 files)            # Public interfaces
├── Domain/                     # Domain entities (7 files)
│   ├── EmailTemplate.cs
│   ├── NotificationEmail.cs
│   ├── NotificationEmailRecipient.cs
│   ├── NotificationQueue.cs
│   ├── NotificationResource.cs
│   └── NotificationType.cs
├── Enum/                       # Enumerations (4 files)
│   ├── NotificationTypes.cs
│   ├── ServiceStatus.cs
│   ├── RecipientType.cs
│   └── NotificationResourceType.cs
├── Impl/                       # Service implementations (13 files)
│   ├── NotificationService.cs
│   ├── NotificationPollingAgent.cs
│   ├── EmailDispatcher.cs
│   ├── UserNotificationModelFactory.cs (2144 lines)
│   ├── NotificationModelFactory.cs
│   ├── WeeklyNotificationPollingAgent.cs
│   ├── PaymentReminderPollingAgent.cs
│   ├── DocumentNotificationPollingAgent.cs
│   ├── AnnualAuditNotificationService.cs
│   ├── InvoiceNotificationService.cs
│   ├── LateFeeNotificationService.cs
│   ├── CustomerFeedbackAPIRecordService.cs
│   └── NotificationServiceHelper.cs
└── ViewModel/                  # DTOs (33 files)
    ├── EmailNotificationModelBase.cs
    ├── *NotificationModel.cs   # Various notification models
    └── *ViewModel.cs            # Supporting view models
```

## Usage Examples

### Queue a Password Reset Email
```csharp
var model = new UserForgetPasswordNotificationViewModel(baseModel) {
    FullName = person.FullName,
    PasswordLink = resetLink,
    UserName = person.Email,
    Franchisee = franchisee.Name
};

notificationService.QueueUpNotificationEmail(
    NotificationTypes.ForgetPassword,
    model,
    organizationRoleUserId,
    notificationDateTime: DateTime.UtcNow,
    resource: null
);
```

### Queue Invoice with Attachments
```csharp
var resources = new List<NotificationResource> {
    new NotificationResource { ResourceId = invoicePdf.Id }
};

notificationService.QueueUpNotificationEmail(
    NotificationTypes.SendInvoiceDetail,
    invoiceModel,
    "Company Name",
    "noreply@company.com",
    franchisee.Email,
    DateTime.UtcNow,
    organizationRoleUserId,
    resources
);
```

### Run Polling Agent
```csharp
// Typically called by scheduled job (Hangfire, Windows Service, etc.)
pollingAgent.PollForNotifications();
```

## Related Documentation
- [Domain Entities](Domain/.context/CONTEXT.md)
- [Enumerations](Enum/.context/CONTEXT.md)
- [Implementations](Impl/.context/CONTEXT.md)
- [View Models](ViewModel/.context/CONTEXT.md)
