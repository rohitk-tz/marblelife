# Core.Notification.Impl - AI/Agent Context

## Architectural Overview

The Impl folder contains **13 service implementation classes** that form the core notification engine. These implementations follow the **Dependency Injection** pattern with `[DefaultImplementation]` attributes for automatic DI registration.

### Component Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Service Layer (Public)                    │
├─────────────────────────────────────────────────────────────┤
│ NotificationService        - Core queuing service           │
│ UserNotificationModelFactory - 65+ notification builders    │
│ NotificationModelFactory   - Base model factory              │
├─────────────────────────────────────────────────────────────┤
│                  Polling Agents (Schedulers)                 │
├─────────────────────────────────────────────────────────────┤
│ NotificationPollingAgent        - General queue processor   │
│ WeeklyNotificationPollingAgent  - Weekly report generator   │
│ PaymentReminderPollingAgent     - Payment reminders         │
│ DocumentNotificationPollingAgent - Document alerts          │
├─────────────────────────────────────────────────────────────┤
│              Domain-Specific Services                        │
├─────────────────────────────────────────────────────────────┤
│ InvoiceNotificationService      - Invoice notifications     │
│ LateFeeNotificationService      - Late fee alerts           │
│ AnnualAuditNotificationService  - Annual sales audit        │
│ CustomerFeedbackAPIRecordService - Feedback requests        │
├─────────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                         │
├─────────────────────────────────────────────────────────────┤
│ EmailDispatcher          - SendGrid API integration         │
│ NotificationServiceHelper - Utility functions               │
└─────────────────────────────────────────────────────────────┘
```

## Implementation Files

### 1. NotificationService.cs (566 lines)

**Purpose:** Core implementation of INotificationService. Handles queuing notifications with templates, recipients, and attachments.

**Key Responsibilities:**
- Load and parse EmailTemplate with Razor engine
- Create NotificationQueue with child entities (NotificationEmail, Recipients, Resources)
- Apply email resolution logic (AccountPersonEmail → ContactEmail → User Email)
- Handle multi-language template selection based on franchisee LanguageId
- Inject email signatures for specific notification types
- Apply auto-CC logic based on notification type
- Support dynamic email bodies (custom HTML)

**Key Methods:**

**`QueueUpNotificationEmail<T>(NotificationTypes, T model, long organizationRoleUserId, DateTime?, List<NotificationResource>)`**
- **Lines:** 43-77
- **Purpose:** Queue notification to specific user with automatic email resolution
- **Email Resolution Order:**
  1. For billing notifications → AccountPersonEmail
  2. Fallback → ContactEmail
  3. Fallback → Person.Email
- **Returns:** NotificationQueue or null if email not found

**`QueueUpNotificationEmail<T>(NotificationTypes, T model, string fromName, string fromEmail, string toEmail, DateTime, long?, List<NotificationResource>, string recipientEmail)`**
- **Lines:** 78-342
- **Purpose:** Primary queuing method with explicit sender/recipient
- **Complex Logic:**
  - **Template Selection (Lines 84-128):** Loads template by NotificationType, matches franchisee language, falls back to default
  - **Validation (Lines 129-142):** Checks template exists, active, queuing enabled
  - **Signature Injection (Lines 143-202):** For specific notification types, injects custom signatures from EmailSignatures table
  - **Template Parsing (Lines 203-204):** Uses RazorEngine to parse Subject and Body with model
  - **CC Logic (Lines 210-252):** Auto-adds CC based on notification type:
    - Late fees → Franchisee owner
    - Billing → Admin email
    - Before/After images → Custom CC from model
    - Specific franchisees (Detroit, Philadelphia, Pittsburgh, Grand Rapids) → Special recipient
  - **Recipient Building (Lines 253-308):** Handles comma-separated emails, assigns TO/CC types
  - **Persistence (Lines 339-341):** Saves NotificationQueue with cascade

**`QueueUpNotificationDyamicEmail<T>(...)`**
- **Lines:** 344-563
- **Purpose:** Queue with custom HTML body (bypasses template body, still uses template subject)
- **Difference:** Sets `IsDynamicEmail = true`, body parameter is custom HTML

**Dependencies:**
```csharp
IRepository<NotificationQueue>
IRepository<EmailTemplate>
IRepository<EmailSignatures>
IOrganizationRoleUserInfoService
IRepository<Organization>
IRepository<Franchisee>
ISettings (SMTP config, CC emails)
IClock (current time)
```

**Design Patterns:**
- **Factory Method:** NotificationServiceHelper.CreateDomain() creates entities
- **Template Method:** FormatContent() handles Razor parsing
- **Strategy Pattern:** Different CC logic per notification type

---

### 2. NotificationPollingAgent.cs (134 lines)

**Purpose:** Background worker that processes queued notifications. Runs on schedule (typically every 5 minutes).

**Key Responsibilities:**
- Query pending notifications (NotificationDate < Now, Status = Pending, IsServiceEnabled = true)
- Send emails via EmailDispatcher
- Update status (Success/Failed) and retry counter
- Handle retry logic (max 3 attempts)

**Key Methods:**

**`PollForNotifications()`**
- **Lines:** 33-84
- **Algorithm:**
  1. Get pending notifications from database
  2. For each notification:
     - Check attempt count (skip if > 3)
     - Extract TO recipients (RecipientType = 127)
     - Call ServiceEmailNotification()
     - Update status and ServicedAt
     - Increment AttemptCount
     - Save changes
  3. Commit transaction per notification (isolation)

**`GetNotificationsToService()`**
- **Lines:** 85-89
- **Query:** Returns notifications where:
  - NotificationDate < Current UTC time
  - ServiceStatusId = Pending (111)
  - NotificationType.IsServiceEnabled = true

**`ServiceEmailNotification(NotificationEmail, string[] recipients)`**
- **Lines:** 91-105
- **Purpose:** Calls EmailDispatcher with TO and CC recipients
- **Error Handling:** Throws ApplicationException on failure (triggers retry)

**Dependencies:**
```csharp
IRepository<NotificationQueue>
IEmailDispatcher
ILogService
IClock
ISettings
```

**Performance:**
- **N+1 Risk:** Uses .Fetch() which loads navigation properties
- **Transaction Per Notification:** Commits after each notification (safe but slower)

**Note:** Contains legacy code (unused SendEmail method with hardcoded Gmail credentials, lines 106-133)

---

### 3. EmailDispatcher.cs (156 lines)

**Purpose:** Low-level email sending via SendGrid API. Handles SMTP integration with attachment support.

**Key Responsibilities:**
- Send emails via SendGrid API with SendGrid.Helpers.Mail
- Convert file attachments to Base64
- Handle TO/CC recipients
- Set email priority headers
- Log send status

**Key Methods:**

**`SendEmail(string body, string subject, string fromName, string fromEmail, string[] ccEmails, IEnumerable<NotificationResource> resources, params string[] toEmail)`**
- **Lines:** 34-91
- **SendGrid Configuration:**
  - API Key from Settings.SmtpEmailApiKey
  - Security: TLS 1.2
  - Priority header: "Urgent"
- **Recipient Logic:**
  - Primary TO: First email only (SendGrid limitation workaround)
  - CC: All ccEmails array
- **Attachments:**
  - Reads from `resource.Resource.RelativeLocation + "/" + resource.Resource.Name`
  - Converts to Base64 via AttachResource()
  - Sets disposition: "inline"
- **Error Handling:** Catches exceptions, logs via ILogService, doesn't rethrow

**`AttachResource(SendGridMessage, string filePath, string contentType, string resourceName)`**
- **Lines:** 100-113
- **Purpose:** Attach file to SendGrid message
- **Throws:** FileNotFoundException if file missing on disk
- **Base64 Encoding:** File.ReadAllBytes() → Convert.ToBase64String()

**`ProcessResponse(Response response)`**
- **Lines:** 94-97
- **Purpose:** Log SendGrid response status code

**SendNormalEmail()**
- **Lines:** 115-153
- **Note:** Legacy code with hardcoded Yahoo SMTP credentials (unused, should be removed)

**Dependencies:**
```csharp
ISettings (SmtpEmailApiKey)
ILogService
SendGrid NuGet package
```

**Configuration:**
```csharp
// SendGrid API Key from settings
var _client = new SendGridClient(_settings.SmtpEmailApiKey);
```

---

### 4. UserNotificationModelFactory.cs (2144 lines)

**Purpose:** Massive factory implementing IUserNotificationModelFactory with 65+ methods. Each method creates a notification model and queues it via INotificationService.

**Pattern:** All methods follow this template:
1. Build base model via INotificationModelFactory
2. Create typed ViewModel (e.g., InvoicePaymentReminderNotificationModel)
3. Populate properties from domain entities
4. Call INotificationService.QueueUpNotificationEmail()
5. Return result (typically void, some return long?)

**Key Method Categories:**

**Authentication & User Management:**
- `CreateForgetPasswordNotification(string passwordLink, Person)`
- `CreateLoginCredentialNotification(Person, string password, bool includeSetupGuide)`

**Billing & Invoicing:**
- `CreateInvoiceDetailNotification(long organizationId, IList<FranchiseeInvoice>)`
- `CreatePaymentReminderNotification(IList<FranchiseeInvoice>, Franchisee)`
- `CreatePaymentConfirmationNotification(Invoice, Payment, long organizationId)`
- `CreateLateFeeReminderNotification(InvoiceItem, long organizationId, long lateFeeTypeId, DateTime)`

**Weekly/Monthly Reports:**
- `CreateWeeklyNotification(File, IEnumerable<FranchiseeInvoice>, DateTime, DateTime, NotificationTypes)`
- `CreateMonthlyNotificationModel(File, DateTime, DateTime, NotificationTypes, File)`
- `CreateWeeklyNotificationForArReport(File, IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel>, DateTime, DateTime, NotificationTypes, decimal)`

**Sales Data:**
- `CreateSalesDataReminderNotification(SalesDataUpload, DateTime, DateTime, long?)`
- `CreateAnnualUploadNotification(AnnualSalesDataUpload)`
- `CreateReviewActionNotification(AnnualSalesDataUpload, bool isAccept)`
- `CreateSalesUploadNotification(File, DateTime, DateTime)`

**Document Management:**
- `CreateDocumentUploadNotification(string fileName, OrganizationRoleUser uploadedBy, Franchisee)`
- `CreateDocumentExpiryNotification(FranchiseDocument doc)`

**Job Scheduling (15+ methods):**
- `ScheduleReminderNotification(JobScheduler, DateTime, DateTime, string encryptedData, NotificationTypes)`
- `NewJobOrEstimateReminderNotificationtoTech(JobEditModel, OrganizationRoleUser)`
- `CancelJobOrEstimateReminderNotificationtoTech(JobEditModel, OrganizationRoleUser)`
- `UpdateJobOrEstimateReminderNotificationtoTech(JobEditModel, OrganizationRoleUser)`
- `UrgentJobOrEstimateReminderNotificationtoTech(JobEditModel, OrganizationRoleUser)`
- Similar methods for estimates (*ForEstimate suffix)
- `ScheduleReminderNotificationToUser(JobScheduler, DateTime, DateTime)`
- `ScheduleReminderNotificationToUserOnDay(JobScheduler, DateTime, DateTime)`

**Customer Engagement:**
- `SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee)` → Returns ReviewAPIResponseModel
- `BeforeAfterImageNotificationtoCustomer(BeforeAfterImageMailViewModel, File, NotificationTypes)` → Returns long? (notification ID)
- `InvoiceCustomerNotificationtoCustomer(BeforeAfterImageMailViewModel, List<File>, NotificationTypes)` → Returns long?
- `InvoiceCustomerNotificationtoCustomerForSignedInvoices(BeforeAfterImageMailViewModel, List<File>, NotificationTypes)` → Returns long?

**Loan & Financial:**
- `CreateLoanCompletionNotification(FranchiseeLoan)`

**Image Management:**
- `CreateBeforeAfterBestPairModel(JobEstimateImage, JobEstimateImage, JobScheduler, MarkbeforeAfterImagesHistry)` → Returns BeforeAfterBestPairViewModel
- `CreateBeforeAfterPairModel(JobEstimateImage, JobEstimateImage, JobScheduler, List<OrganizationRoleUser>)` → Returns BeforeAfterBestPairViewModel

**Web Leads:**
- `SendWebLeadsNotification(NotificationTypes, DateTime)`

**Review System:**
- `CreateReviewSystemRecordNotification(File, DateTime, DateTime, long organizationId)`

**Dependencies:**
All business domain repositories, INotificationService, INotificationModelFactory, plus specific services for each domain area.

**Size Note:** This file is a **God Object anti-pattern** - all notification logic in one 2144-line class. Consider refactoring into category-specific factories (BillingNotificationFactory, JobNotificationFactory, etc.)

---

### 5. NotificationModelFactory.cs (51 lines)

**Purpose:** Simple factory creating base EmailNotificationModelBase instances with company settings.

**Key Methods:**

**`CreateBase(long organizationId)`**
- **Lines:** 20-32
- **Purpose:** Create base model with organization-specific data (currently unused)
- **Returns:** EmailNotificationModelBase with SiteRootUrl, LogoImage, empty Address

**`CreateBaseDefault()`**
- **Lines:** 33-49
- **Purpose:** Create base model with system defaults
- **Populated Fields:**
  - SiteRootUrl, CompanyLogoImageUrl (from Settings)
  - CompanyName, OwnerName, Designation, Phone (from Settings)
  - ApplicationName, SchedulingApplication (from Settings)
  - Empty AddressViewModel

**Usage:** Called by UserNotificationModelFactory to create base models for all notification ViewModels.

**Dependencies:**
```csharp
ISettings (all company configuration)
IAddressFactory (unused currently)
```

---

### 6. WeeklyNotificationPollingAgent.cs (408 lines)

**Purpose:** Generates weekly summary reports (late fees, unpaid invoices, AR report) on scheduled day of week.

**Key Responsibilities:**
- Generate Excel attachments with invoice/payment data
- Send to admins with summary emails
- Prevent duplicate sends via WeeklyNotification deduplication table
- Calculate week boundaries based on Settings.WeeklyReminderDay

**Key Methods:**

**`CreateWeeklyNotification()`**
- **Lines:** 53-82
- **Orchestrator:** Calls SalesDataLateFeeList(), UnpaidInvoiceList(), ArReportList() based on settings

**`SalesDataLateFeeList()`**
- **Lines:** 189-241
- **Purpose:** Weekly late fee report
- **Flow:**
  1. Calculate current week date (align to Settings.WeeklyReminderDay)
  2. Check deduplication (WeeklyNotification table)
  3. Query franchisees with late fees
  4. Create Excel attachment via CreateAttachmentForLateFee()
  5. Queue notification via UserNotificationModelFactory
  6. Save deduplication record
- **Output:** Excel file with late fee details per franchisee

**`UnpaidInvoiceList()`**
- **Lines:** 323-376
- **Purpose:** Weekly unpaid invoice report
- **Query:** Invoices with StatusId = Unpaid and GeneratedOn <= currentWeekDate
- **Output:** Excel with unpaid invoice details

**`ArReportList()`**
- **Lines:** 83-139
- **Purpose:** Accounts Receivable aging report
- **Complex Logic:**
  - Groups unpaid invoices by franchisee
  - Calculates aging buckets: 0-30, 30-60, 60-90, 90+ days
  - Creates franchisee-wise summary via GetArReportModel()
  - Totals all amounts
- **Output:** Excel with AR aging by franchisee

**`CreateAttachmentForLateFee()`**
- **Lines:** 275-298
- **Purpose:** Generate Excel file from WeeklyNotificationReportViewModel collection
- **File Location:** MediaLocationHelper.GetAttachmentMediaLocation().Path
- **Returns:** File domain entity (saved to database)

**`CreateAttachmentForUnpaidInvoice()`**
- **Lines:** 243-273
- **Purpose:** Similar to CreateAttachmentForLateFee, different format (WeeklyUnpaidInvoiceNotificationReportModel)

**`CreateAttachmentForArReport()`**
- **Lines:** 141-186
- **Purpose:** Generate Excel with franchisee-wise AR aging
- **Calculates:** Sum of aging buckets across all franchisees
- **Adds:** Total row at bottom

**`GetWeekDay(DateTime dt, int dayOfWeek)`**
- **Lines:** 312-320
- **Utility:** Calculate most recent occurrence of specific day of week

**Dependencies:**
```csharp
IRepository<FranchiseeInvoice>
IRepository<WeeklyNotification> (deduplication)
IUserNotificationModelFactory
IExcelFileCreator (creates .xlsx files)
IFileService (saves File entities)
IReportFactory
IReportService (GetArReportModel)
IClock, ISettings, ILogService
```

**Configuration:**
```csharp
Settings.SendWeeklyReminder = true/false
Settings.SendWeekyLateFeeNotification = true/false
Settings.SendWeekyUnpaidInvoicesNotification = true/false
Settings.WeeklyReminderDay = 0-6 (Sunday = 0, Monday = 1, etc.)
```

---

### 7. PaymentReminderPollingAgent.cs (134 lines)

**Purpose:** Send daily payment reminders for overdue invoices. Also handles payment confirmation and loan completion notifications.

**Key Methods:**

**`CreateNotificationReminderForPayment()`**
- **Lines:** 37-97
- **Purpose:** Daily reminder for unpaid invoices past due date
- **Algorithm:**
  1. Check queuing enabled via IsNotificationQueuingEnabled()
  2. Query franchisees with unpaid invoices
  3. For each franchisee:
     - Get unpaid invoices
     - Check deduplication (PaymentMailReminder table)
     - Filter by DueDate (must be <= current date)
     - Call UserNotificationModelFactory.CreatePaymentReminderNotification()
     - Save deduplication record (InvoiceId + Date)
  4. Commit per franchisee

**`CreatePaymentConfirmationNotification(Invoice, Payment, long organizationId)`**
- **Lines:** 119-127
- **Purpose:** Confirmation email when payment received
- **Delegates to:** UserNotificationModelFactory

**`CreateLoanCompletionNotification(FranchiseeLoan)`**
- **Lines:** 129-132
- **Purpose:** Notify franchisee when loan fully paid
- **Delegates to:** UserNotificationModelFactory

**`IsNotificationQueuingEnabled()`**
- **Lines:** 109-117
- **Purpose:** Check NotificationType.PaymentReminder is enabled (both IsQueuingEnabled and IsServiceEnabled)

**Dependencies:**
```csharp
IRepository<PaymentMailReminder> (deduplication)
IRepository<NotificationType>
IRepository<Franchisee>
IUserNotificationModelFactory
IClock, ISettings, ILogService
```

**Deduplication:** PaymentMailReminder table prevents sending duplicate reminders for same invoice on same date.

---

### 8. DocumentNotificationPollingAgent.cs (97 lines)

**Purpose:** Handle document upload notifications and document expiry alerts.

**Key Methods:**

**`CreateDocumentUploadNotification(string fileName, ICollection<long> franchiseeIds, long? createdBy)`**
- **Lines:** 35-61
- **Purpose:** Notify franchisees when SuperAdmin uploads document
- **Logic:**
  - If uploader is SuperAdmin → send to all specified franchisees
  - If uploader is not SuperAdmin → send generic notification (no franchisee filter)
- **Loops:** Each franchisee gets separate notification

**`SendExpiryNotification()`**
- **Lines:** 63-95
- **Purpose:** Alert for documents expiring within 3 days
- **Algorithm:**
  1. Check Settings.SendExpiryNotification enabled
  2. Query FranchiseDocument where:
     - IsImportant = true
     - ExpiryDate between Today and Today + 3 days
  3. For each expiring document:
     - Call UserNotificationModelFactory.CreateDocumentExpiryNotification()
     - Commit per document (transaction isolation)
     - ResetContext() to avoid EF tracking issues
- **Error Handling:** Continues on exception (logs and skips failing documents)

**Dependencies:**
```csharp
IRepository<Franchisee>
IRepository<FranchiseDocument>
IRepository<OrganizationRoleUser>
IUserNotificationModelFactory
IClock, ISettings, ILogService
```

**Configuration:**
```csharp
Settings.SendExpiryNotification = true/false
```

---

### 9-11. Domain-Specific Service Wrappers

These are thin wrappers that delegate to UserNotificationModelFactory:

**9. AnnualAuditNotificationService.cs (24 lines)**
```csharp
[DefaultImplementation]
public class AnnualAuditNotificationService : IAnnualAuditNotificationService
{
    private IUserNotificationModelFactory _userNotificationModelFactory;
    
    public void CreateAnnualUploadNotification(AnnualSalesDataUpload annualFileUpload)
        => _userNotificationModelFactory.CreateAnnualUploadNotification(annualFileUpload);
    
    public void CreateReviewActionNotification(AnnualSalesDataUpload annualFileUpload, bool isAccept)
        => _userNotificationModelFactory.CreateReviewActionNotification(annualFileUpload, isAccept);
}
```

**10. InvoiceNotificationService.cs (21 lines)**
```csharp
[DefaultImplementation]
public class InvoiceNotificationService : IInvoiceNotificationService
{
    private IUserNotificationModelFactory _userNotificationModelFactory;
    
    public void CreateInvoiceDetailNotification(IList<FranchiseeInvoice> franchiseeInvoiceList, long franchiseeId)
        => _userNotificationModelFactory.CreateInvoiceDetailNotification(franchiseeId, franchiseeInvoiceList);
}
```

**11. LateFeeNotificationService.cs (32 lines)**
```csharp
[DefaultImplementation]
public class LateFeeNotificationService : ILateFeeNotificationService
{
    private IUserNotificationModelFactory _userNotificationModelFactory;
    
    public void CreateLateFeeNotification(InvoiceItem invoiceItem, long organizationId, DateTime currentDate)
    {
        if (invoiceItem.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.Royalty)
            _userNotificationModelFactory.CreateLateFeeReminderNotification(invoiceItem, organizationId, (long)LateFeeType.Royalty, currentDate);
        
        if (invoiceItem.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.SalesData)
            _userNotificationModelFactory.CreateLateFeeReminderNotification(invoiceItem, organizationId, (long)LateFeeType.SalesData, currentDate);
    }
}
```

**12. CustomerFeedbackAPIRecordService.cs (26 lines)**
```csharp
[DefaultImplementation]
public class CustomerFeedbackAPIRecordService : ICustomerFeedbackAPIRecordService
{
    private IUserNotificationModelFactory _userNotificationModelFactory;
    
    public ReviewAPIResponseModel SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee franchisee)
        => _userNotificationModelFactory.SendEmailFeedbackRequest(customerEmail, customerName, franchisee);
}
```

**Purpose of Wrappers:** Provide focused interfaces for specific domains while centralizing logic in UserNotificationModelFactory.

---

### 13. NotificationServiceHelper.cs (43 lines)

**Purpose:** Static utility class with helper methods for creating domain entities and parsing templates.

**Key Methods:**

**`CreateDomain(NotificationType notificationtype, DateTime notificationdate)`**
- **Lines:** 12-23
- **Purpose:** Factory method to create NotificationQueue entity
- **Returns:** NotificationQueue with:
  - NotificationDate = parameter
  - NotificationType = parameter
  - ServiceStatusId = Pending (111)
  - IsNew = true
  - DataRecorderMetaData = new instance
  - Source = empty string

**`CreateDomain(long id, string fromEmail, string fromName, string subject, string body, List<NotificationResource> resource)`**
- **Lines:** 25-36
- **Purpose:** Factory method to create NotificationEmail entity
- **Returns:** NotificationEmail with all parameters mapped

**`FormatContent<T>(string text, T model)`**
- **Lines:** 38-41
- **Purpose:** Parse Razor template with model
- **Uses:** RazorEngine library (`Razor.Parse(text, model)`)
- **Example:**
  ```csharp
  var subject = "Invoice #@Model.InvoiceId";
  var parsed = FormatContent(subject, invoiceModel);
  // Result: "Invoice #12345"
  ```

**Dependencies:**
```csharp
RazorEngine NuGet package
```

---

## Design Patterns

### 1. Dependency Injection with [DefaultImplementation]
All implementation classes use `[DefaultImplementation]` attribute for automatic DI container registration.

### 2. Factory Pattern
- **NotificationModelFactory:** Creates base models
- **UserNotificationModelFactory:** Creates typed models and queues
- **NotificationServiceHelper:** Static factories for domain entities

### 3. Polling Agent Pattern
Background workers poll database on schedule and process pending items.

### 4. Template Method Pattern
All UserNotificationModelFactory methods follow the same template:
1. Create base model
2. Create typed ViewModel
3. Populate from domain entities
4. Queue via INotificationService

### 5. Facade Pattern
Domain-specific services (InvoiceNotificationService, etc.) provide simple facades over UserNotificationModelFactory.

### 6. Strategy Pattern
NotificationService applies different CC logic based on notification type.

## Integration Points

### External Dependencies
- **SendGrid API:** EmailDispatcher → SendGridClient
- **RazorEngine:** NotificationServiceHelper → Razor.Parse()
- **File System:** EmailDispatcher reads files for attachments

### Internal Dependencies
```
Impl/
├─> Core.Application (IRepository, IUnitOfWork, IClock, ISettings, ILogService)
├─> Core.Notification.Domain (all entities)
├─> Core.Notification.Enum (all enums)
├─> Core.Notification.ViewModel (all ViewModels)
├─> Core.Organizations (Franchisee, OrganizationRoleUser)
├─> Core.Billing (Invoice, Payment, InvoiceItem, FranchiseeInvoice)
├─> Core.Users (Person)
├─> Core.Scheduler (JobScheduler, JobEstimateEditModel, JobEditModel)
├─> Core.Sales (SalesDataUpload, AnnualSalesDataUpload)
├─> Core.Review (ReviewAPIResponseModel)
├─> Core.Reports (IReportFactory, IReportService)
└─> Core.Geo (AddressViewModel)
```

## Performance Considerations

### Polling Agents
- **Batch Size:** No limit on pending notifications (consider TOP/LIMIT for large queues)
- **Transaction Granularity:** WeeklyNotificationPollingAgent commits per notification (safe but slower)
- **N+1 Queries:** Use Fetch() with includeProperties for navigation properties

### Template Parsing
- **RazorEngine:** No caching of parsed templates (parsed on every queue operation)
- **Consider:** Implement template caching by NotificationTypeId + LanguageId

### File I/O
- **Attachment Reading:** EmailDispatcher reads files synchronously from disk
- **Bottleneck:** Large files (PDFs, images) block I/O thread
- **Consider:** Async file reading, pre-caching for bulk operations

### UserNotificationModelFactory
- **God Object:** 2144 lines in single class (hard to maintain, test, extend)
- **Memory:** Multiple repository dependencies loaded in DI (heavy object graph)
- **Refactor:** Split into category-specific factories

## Testing Strategy

### Unit Testing
- **NotificationService:** Mock IRepository, verify entity creation
- **EmailDispatcher:** Mock SendGridClient, verify API calls
- **NotificationServiceHelper:** Test Razor parsing with sample templates

### Integration Testing
- **Polling Agents:** Seed database, run agent, verify status updates
- **End-to-End:** Queue notification → run polling agent → verify email sent (use test SMTP)

### Performance Testing
- **Load Test:** Queue 10,000 notifications, measure polling agent throughput
- **Stress Test:** Concurrent polling agent instances, verify no race conditions

## Edge Cases & Known Issues

### NotificationService
- **Race Condition:** Multiple threads queuing with same franchisee could cause deadlocks on franchisee lookup
- **Signature Injection:** Only works for specific notification types (hardcoded list)
- **Multi-Language:** Falls back to default if franchisee language not found (no error logged)

### EmailDispatcher
- **SendGrid Failures:** Catches exceptions, logs, doesn't rethrow (notification marked Success even if SendGrid failed)
- **Attachment Size:** No size validation before Base64 encoding (SendGrid has ~30MB limit)
- **Multiple TO Recipients:** Only first TO used, rest become CC (workaround for SendGrid limitation)

### Polling Agents
- **Duplicate Processing:** No distributed lock (if multiple agent instances run simultaneously, same notification could be processed twice)
- **Long-Running:** If polling takes > 5 minutes, next schedule could overlap
- **Failed Notification Cleanup:** Failed notifications never auto-deleted (manual cleanup required)

### UserNotificationModelFactory
- **Error Propagation:** Many methods return void (errors only logged, not bubbled up)
- **Null Safety:** Assumes all domain entities non-null (NullReferenceException risk)

## Related Documentation
- [Parent Module](../.context/CONTEXT.md)
- [Domain Entities](../Domain/.context/CONTEXT.md)
- [Enumerations](../Enum/.context/CONTEXT.md)
- [View Models](../ViewModel/.context/CONTEXT.md)
