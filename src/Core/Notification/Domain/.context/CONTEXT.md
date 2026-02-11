# Core.Notification.Domain - AI/Agent Context

## Architectural Overview

The Domain folder contains **6 core entity classes** that represent the notification system's data model. These entities implement a **normalized relational database schema** with proper foreign key relationships and navigation properties.

### Entity Relationship Diagram

```
┌──────────────────┐
│ NotificationType │ (Configuration master data)
│ ──────────────── │
│ Id               │
│ Title            │
│ IsServiceEnabled │
│ IsQueuingEnabled │
└────────┬─────────┘
         │ 1:N
         ▼
┌──────────────────┐       1:1        ┌──────────────────┐
│ NotificationQueue│◄───────────────── │ NotificationEmail│
│ ──────────────── │                   │ ──────────────── │
│ Id               │                   │ Id               │
│ NotificationDate │                   │ FromEmail        │
│ ServiceStatusId  │                   │ FromName         │
│ ServicedAt       │                   │ Subject          │
│ AttemptCount     │                   │ Body             │
│ FranchiseeId     │                   │ IsDynamicEmail   │
└────────┬─────────┘                   └────────┬─────────┘
         │                                      │
         │                                      │ 1:N
         │ 1:N                                  ▼
         │                      ┌────────────────────────────┐
         │                      │ NotificationEmailRecipient │
         │                      │ ─────────────────────────  │
         │                      │ NotificationId             │
         │                      │ RecipientEmail             │
         │                      │ OrganizationRoleUserId     │
         │                      │ RecipientTypeId (TO/CC/BCC)│
         │                      └────────────────────────────┘
         │
         │ 1:1                  ┌────────────────────┐
         └─────────────────────>│ EmailTemplate      │
                                │ ─────────────────  │
                                │ NotificationTypeId │
                                │ Title              │
                                │ Subject            │
                                │ Body (Razor)       │
                                │ isActive           │
                                │ LanguageId         │
                                └────────────────────┘

           ┌──────────────────┐
           │NotificationResource│ (File attachments)
           │ ─────────────────  │
           │ ResourceId (FileId)│
           │ NotificationEmailId│
           └───────────────────┘
```

## Domain Entities

### 1. NotificationQueue.cs

**Purpose:** Primary queue storage entity. Represents a single queued notification with scheduling, status tracking, and retry logic.

**Properties:**
```csharp
public long NotificationTypeId { get; set; }        // FK to NotificationType
public DateTime NotificationDate { get; set; }      // When to send (scheduling)
public long ServiceStatusId { get; set; }           // FK to Lookup (Pending/Success/Failed)
public DateTime? ServicedAt { get; set; }           // When processed (audit)
public int? AttemptCount { get; set; }              // Retry counter (max 3)
public string Source { get; set; }                  // Origin context (optional)
public long? FranchiseeId { get; set; }             // FK to Organization (for reporting)
public long DataRecorderMetadataId { get; set; }    // Audit metadata FK
```

**Navigation Properties:**
```csharp
public virtual NotificationType NotificationType { get; set; }
public virtual NotificationEmail NotificationEmail { get; set; }  // 1:1
public virtual Lookup ServiceStatus { get; set; }
public virtual Organization Organization { get; set; }
public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
```

**Key Behaviors:**
- **Scheduling:** `NotificationDate` controls when polling agent sends it
- **Retry Logic:** `AttemptCount` incremented on each failure, max 3
- **Status Lifecycle:** Pending (111) → Success (112) or Failed (113)
- **Audit Trail:** `ServicedAt` records completion time

**Database Query Pattern:**
```csharp
// Polling agent query
var pending = _repository.Fetch(
    x => x.NotificationDate < _clock.UtcNow 
      && x.ServiceStatus.Id == (long)ServiceStatus.Pending
      && x.NotificationType.IsServiceEnabled
);
```

---

### 2. NotificationEmail.cs

**Purpose:** Stores email content, metadata, and relationships to recipients and attachments. One-to-one with NotificationQueue.

**Properties:**
```csharp
public long EmailTemplateId { get; set; }      // FK to EmailTemplate
public string FromEmail { get; set; }          // Sender email
public string FromName { get; set; }           // Sender display name
public string Subject { get; set; }            // Parsed subject (from template)
public string Body { get; set; }               // Parsed body HTML (from template)
public bool? IsDynamicEmail { get; set; }      // True if custom body (not from template)
```

**Navigation Properties:**
```csharp
public virtual EmailTemplate EmailTemplate { get; set; }
public virtual NotificationQueue NotificationQueue { get; set; }      // 1:1 back-reference
public virtual IList<NotificationEmailRecipient> Recipients { get; set; }  // 1:N
public virtual IList<NotificationResource> Resources { get; set; }    // 1:N (attachments)
```

**Key Behaviors:**
- **Constructor:** Initializes `Recipients = new List<NotificationEmailRecipient>()`
- **Template Parsing:** Body/Subject are Razor templates parsed with ViewModel
- **Dynamic Emails:** When `IsDynamicEmail = true`, body is custom HTML (not from template)

**Design Pattern:** Aggregate root for email composition (recipients + resources)

---

### 3. NotificationEmailRecipient.cs

**Purpose:** Represents a single recipient (TO/CC/BCC) for an email. Supports multiple recipients per notification.

**Properties:**
```csharp
public long NotificationId { get; set; }             // FK to NotificationEmail
public long? OrganizationRoleUserId { get; set; }    // FK to user (optional)
public string RecipientEmail { get; set; }           // Actual email address
public long RecipientTypeId { get; set; }            // FK to Lookup (TO=127, CC=128, BCC=129)
```

**Navigation Properties:**
```csharp
public virtual NotificationEmail NotificationEmail { get; set; }
public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }
public virtual Lookup RecipientType { get; set; }
```

**Key Behaviors:**
- **Recipient Types:** 
  - TO (127) - Primary recipients
  - CC (128) - Carbon copy
  - BCC (129) - Blind carbon copy
- **User Linkage:** `OrganizationRoleUserId` links to user record (optional, for reporting)
- **Email Override:** `RecipientEmail` is the actual email sent to (can differ from user's email)

**Querying Pattern:**
```csharp
// Get TO recipients
var toEmails = notification.Recipients
    .Where(x => x.RecipientTypeId == (long)RecipientType.TO)
    .Select(x => x.RecipientEmail)
    .ToArray();

// Get CC recipients
var ccEmails = notification.Recipients
    .Where(x => x.RecipientTypeId == (long)RecipientType.CC)
    .Select(x => x.RecipientEmail)
    .ToArray();
```

---

### 4. NotificationResource.cs

**Purpose:** Links file attachments to email notifications. Represents the many-to-many relationship between NotificationEmail and File.

**Properties:**
```csharp
public long ResourceId { get; set; }             // FK to File
public long NotificationEmailId { get; set; }    // FK to NotificationEmail
```

**Navigation Properties:**
```csharp
public virtual NotificationEmail NotificationEmail { get; set; }
public virtual File Resource { get; set; }       // File entity with path, mime type, name
```

**Key Behaviors:**
- **File Storage:** File entity contains `RelativeLocation` and `Name` for file system access
- **Attachment Rendering:** EmailDispatcher reads `Resource.RelativeLocation + "/" + Resource.Name` and converts to Base64
- **Multiple Attachments:** One NotificationEmail can have multiple NotificationResource entries

**Usage Pattern:**
```csharp
// Attach invoice PDF
var resources = new List<NotificationResource> {
    new NotificationResource {
        ResourceId = invoicePdfFile.Id,
        NotificationEmailId = 0  // Set by framework
    }
};

// Pass to queue method
notificationService.QueueUpNotificationEmail(
    NotificationTypes.SendInvoiceDetail,
    model,
    orgRoleUserId,
    DateTime.UtcNow,
    resources
);
```

---

### 5. EmailTemplate.cs

**Purpose:** Master data entity storing email templates with Razor syntax. Supports multi-language templates.

**Properties:**
```csharp
public long NotificationTypeId { get; set; }    // FK to NotificationType
public string Title { get; set; }               // Template name
public string Description { get; set; }         // Template description
public string Subject { get; set; }             // Razor template for subject
public string Body { get; set; }                // Razor template for body
public bool isActive { get; set; }              // Enable/disable template
public bool IsRequired { get; set; }            // System-required template
public long LanguageId { get; set; }            // FK to Lookup (language)
```

**Navigation Properties:**
```csharp
public virtual NotificationType NotificationType { get; set; }
public virtual Lookup LookUp { get; set; }      // Language lookup
```

**Key Behaviors:**
- **Razor Templating:** `Subject` and `Body` contain Razor syntax (e.g., `@Model.FullName`)
- **Multi-Language:** Multiple templates for same NotificationType with different LanguageId
- **Template Selection Logic:**
  1. Match by NotificationTypeId AND franchisee's LanguageId
  2. Fall back to default language if not found
- **Active Flag:** Only active templates are used (`isActive = true`)

**Razor Example:**
```html
<!-- EmailTemplate.Body -->
<html>
<body>
    <h1>Hello @Model.FullName!</h1>
    <p>Your invoice for @Model.Franchisee is ready.</p>
    <p>Amount: @Model.InvoiceAmount</p>
    <p>Due Date: @Model.DueDate</p>
    <a href="@Model.Base.GetUrl("/invoices/" + @Model.InvoiceId)">View Invoice</a>
</body>
</html>
```

**Parsing Engine:** RazorEngine library (`Razor.Parse(template, model)`)

---

### 6. NotificationType.cs

**Purpose:** Configuration master data defining notification types and their behavior.

**Properties:**
```csharp
public string Title { get; set; }               // Human-readable name
public string Description { get; set; }         // Purpose description
public bool IsServiceEnabled { get; set; }      // Enable processing by polling agent
public bool IsQueuingEnabled { get; set; }      // Enable queueing by services
```

**Key Behaviors:**
- **Service Control:**
  - `IsQueuingEnabled = false` → QueueUpNotificationEmail returns null (no-op)
  - `IsServiceEnabled = false` → Polling agent skips this type
- **Master Data:** Seeded during database initialization
- **Referenced By:** NotificationTypes enum values map to NotificationType.Id

**Enum Mapping:**
```csharp
// NotificationTypes enum (Enum folder)
public enum NotificationTypes {
    ForgetPassword = 1,           // Maps to NotificationType.Id = 1
    SendLoginCredential = 2,      // Maps to NotificationType.Id = 2
    SendInvoiceDetail = 3,        // Maps to NotificationType.Id = 3
    // ... 60+ more types
}
```

**Configuration Query:**
```csharp
// Check if notification type is enabled
var notificationType = _repository.Get(
    x => x.Id == (long)NotificationTypes.PaymentReminder
);

if (!notificationType.IsQueuingEnabled) {
    // Don't queue
    return null;
}
```

---

## Data Flow Through Entities

### Write Path (Queueing)

```
1. INotificationService receives request
   ↓
2. Load EmailTemplate by NotificationTypeId
   ↓
3. Parse template with Razor (Subject/Body)
   ↓
4. Create NotificationQueue entity
   ├─ NotificationDate = scheduled time
   ├─ ServiceStatusId = Pending (111)
   ├─ NotificationEmail = new NotificationEmail {
   │    FromEmail, FromName, Subject, Body,
   │    Recipients = [ TO, CC, BCC recipients ],
   │    Resources = [ attached files ]
   │  }
   └─ FranchiseeId = for reporting
   ↓
5. Save NotificationQueue (cascade saves children)
   ↓
6. Commit transaction
```

### Read Path (Processing)

```
1. NotificationPollingAgent.PollForNotifications()
   ↓
2. Query NotificationQueue:
   WHERE NotificationDate < Now
     AND ServiceStatusId = Pending
     AND NotificationType.IsServiceEnabled = true
   ↓
3. For each NotificationQueue:
   ├─ Load NotificationEmail (navigation property)
   ├─ Load Recipients (TO, CC, BCC)
   ├─ Load Resources (attachments)
   ├─ Call EmailDispatcher.SendEmail()
   ├─ Update ServiceStatusId = Success/Failed
   ├─ Update ServicedAt = Now
   ├─ Increment AttemptCount
   └─ Save changes
   ↓
4. Commit transaction
```

## Database Schema

### Table Names (EF Conventions)
- `NotificationQueues`
- `NotificationEmails`
- `NotificationEmailRecipients`
- `NotificationResources`
- `EmailTemplates`
- `NotificationTypes`

### Key Indexes (Recommended)
```sql
-- Polling agent performance
CREATE INDEX IX_NotificationQueue_Processing 
ON NotificationQueues (NotificationDate, ServiceStatusId, NotificationTypeId)
WHERE ServiceStatusId = 111; -- Pending

-- Recipient lookup
CREATE INDEX IX_NotificationEmailRecipient_Email
ON NotificationEmailRecipients (RecipientEmail, RecipientTypeId);

-- Template lookup
CREATE INDEX IX_EmailTemplate_Type_Language
ON EmailTemplates (NotificationTypeId, LanguageId, isActive);
```

### Foreign Key Relationships
- `NotificationQueue.NotificationTypeId` → `NotificationTypes.Id`
- `NotificationQueue.ServiceStatusId` → `Lookups.Id`
- `NotificationQueue.FranchiseeId` → `Organizations.Id`
- `NotificationEmail.EmailTemplateId` → `EmailTemplates.Id`
- `NotificationEmailRecipient.NotificationId` → `NotificationEmails.Id`
- `NotificationEmailRecipient.OrganizationRoleUserId` → `OrganizationRoleUsers.Id`
- `NotificationEmailRecipient.RecipientTypeId` → `Lookups.Id`
- `NotificationResource.ResourceId` → `Files.Id`
- `NotificationResource.NotificationEmailId` → `NotificationEmails.Id`
- `EmailTemplate.NotificationTypeId` → `NotificationTypes.Id`
- `EmailTemplate.LanguageId` → `Lookups.Id`

## Design Patterns

### 1. Aggregate Root Pattern
- **NotificationQueue** is the aggregate root
- **NotificationEmail** is owned entity (cannot exist without queue)
- **NotificationEmailRecipient** and **NotificationResource** are children of NotificationEmail
- Saves/deletes cascade through aggregate

### 2. Value Object Pattern
- **EmailTemplate.Body** and **EmailTemplate.Subject** are immutable Razor templates
- Parsed into **NotificationEmail.Body** and **NotificationEmail.Subject** (computed values)

### 3. Master Data Pattern
- **NotificationType** and **EmailTemplate** are master data (seeded, rarely changed)
- **NotificationQueue** is transactional data (high volume, frequently written)

### 4. Soft Configuration Pattern
- **NotificationType.IsServiceEnabled** / **IsQueuingEnabled** allow runtime control without code changes
- **EmailTemplate.isActive** allows template versioning

## Entity Lifecycle

### NotificationQueue Lifecycle
```
[New] → [Pending] → [Success/Failed]
         ↑     ↓
         └─────┘ (Retry up to 3 times)
```

### NotificationEmail Lifecycle
```
[Created with Queue] → [Immutable] → [Deleted with Queue*]
* Typically retained for audit trail
```

### EmailTemplate Lifecycle
```
[Seeded] → [Active] → [Inactive] (never deleted)
```

## Integration Points

### Dependencies
- **Core.Application.Domain:** Inherits from `DomainBase` (Id, IsNew, DataRecorderMetaData)
- **Core.Application.Domain.File:** File entity for attachments
- **Core.Application.Domain.Lookup:** RecipientType, ServiceStatus, Language lookups
- **Core.Organizations.Domain:** Organization, OrganizationRoleUser, Franchisee
- **Core.Organizations.Domain.DataRecorderMetaData:** Audit metadata

### Used By
- **NotificationService (Impl):** Creates and saves NotificationQueue
- **NotificationPollingAgent (Impl):** Reads and processes NotificationQueue
- **EmailDispatcher (Impl):** Reads NotificationEmail for sending
- **UserNotificationModelFactory (Impl):** Creates notification models and queues them

## Edge Cases & Constraints

### NotificationQueue
- **AttemptCount:** Can be null initially, incremented to max 3
- **ServicedAt:** Null until processed
- **FranchiseeId:** Nullable, used for reporting dashboards
- **Source:** Optional context string, rarely used

### NotificationEmail
- **IsDynamicEmail:** When true, body is custom (not from template)
- **Recipients:** Must have at least one TO recipient to send
- **Resources:** Can be empty list (no attachments)

### NotificationEmailRecipient
- **OrganizationRoleUserId:** Can be null (e.g., external emails)
- **RecipientEmail:** Required, validated as email format
- **Multiple TO recipients:** Polling agent sends to first TO only, rest as CC (SendGrid limitation workaround)

### EmailTemplate
- **Razor Errors:** Invalid Razor syntax throws exception during parse
- **Model Mismatch:** If template references `@Model.PropertyName` that doesn't exist, RazorEngine throws
- **Language Fallback:** If franchisee language not found, uses default language template

### NotificationResource
- **File Existence:** EmailDispatcher throws FileNotFoundException if file missing on disk
- **File Size:** SendGrid limit of ~30MB total attachments
- **Base64 Encoding:** Files converted to Base64 (increases size by ~33%)

## Performance Considerations

### Polling Agent Queries
- **N+1 Problem:** Use `.Fetch()` with eager loading for navigation properties
- **Batch Size:** No limit on pending notifications (consider adding TOP/LIMIT if queue grows)

### Template Parsing
- **RazorEngine Caching:** Templates are parsed on each queue operation (no caching)
- **Consider:** Cache parsed templates by NotificationTypeId + LanguageId

### Attachment Handling
- **File I/O:** Reading files from disk for each email (bottleneck for large files)
- **Consider:** Pre-cache attachments in memory for bulk operations

## Related Documentation
- [Parent Module](../.context/CONTEXT.md)
- [Enumerations](../Enum/.context/CONTEXT.md)
- [Implementations](../Impl/.context/CONTEXT.md)
- [View Models](../ViewModel/.context/CONTEXT.md)
