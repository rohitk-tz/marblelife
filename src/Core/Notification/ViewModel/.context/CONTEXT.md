# Core.Notification.ViewModel - AI/Agent Context

## Architectural Overview

The ViewModel folder contains **34 Data Transfer Objects (DTOs)** used for email template rendering. These ViewModels are designed for Razor template binding and follow a consistent pattern with a `Base` property for common data.

### ViewModel Pattern

All notification ViewModels follow this structure:

```
┌────────────────────────────────────┐
│ ConcreteNotificationModel          │
│ ─────────────────────────────────  │
│ + FullName: string                 │  ← Notification-specific data
│ + Franchisee: string               │
│ + [Other Properties]               │
│ + Base: EmailNotificationModelBase │  ← Common data (logo, URLs, etc.)
│ + Constructor(base)                │  ← Inject base in constructor
└────────────────────────────────────┘
           │
           ▼
┌────────────────────────────────────┐
│ EmailNotificationModelBase         │  ← Shared across all notifications
│ ─────────────────────────────────  │
│ + CompanyLogoImageUrl              │
│ + CompanyName                      │
│ + SiteRootUrl                      │
│ + Address: AddressViewModel        │
│ + GetUrl(relativeUrl): string      │
└────────────────────────────────────┘
```

**Design Benefits:**
- DRY principle: Common properties (logo, company name) inherited via Base
- Razor templates access: `@Model.FullName` for specific data, `@Model.Base.CompanyName` for common data
- Type safety: Strongly-typed models prevent template errors

## ViewModels by Category

### 1. Base Models

#### EmailNotificationModelBase.cs
**Purpose:** Base class for all notification ViewModels. Contains company branding and common properties.

**Properties:**
```csharp
public string CompanyLogoImageUrl { get; set; }     // Logo URL for email header
public string CompanyName { get; set; }             // Company name
public string ApplicationName { get; set; }         // Application title
public AddressViewModel Address { get; set; }       // Company address
public string SiteRootUrl { get; set; }             // Base URL for links
public string Designation { get; set; }             // Owner designation
public string Phone { get; set; }                   // Company phone
public string OwnerName { get; set; }               // Owner name
public string SchedulingAppliation { get; set; }    // Scheduling app name
```

**Helper Method:**
```csharp
public string GetUrl(string relativeUrl)
{
    return string.Format("{0}{1}", SiteRootUrl, relativeUrl);
}
```

**Usage in Templates:**
```html
<img src="@Model.Base.CompanyLogoImageUrl" alt="@Model.Base.CompanyName" />
<p>Contact us: @Model.Base.Phone</p>
<a href="@Model.Base.GetUrl("/invoices/123")">View Invoice</a>
```

---

### 2. Authentication & User Management

#### UserForgetPasswordNotificationViewModel.cs
**Notification Type:** ForgetPassword (1)

**Properties:**
```csharp
public string FullName { get; set; }            // User's full name
public string PasswordLink { get; set; }        // Password reset URL
public string UserName { get; set; }            // User's email/username
public string Franchisee { get; set; }          // Franchisee name
public EmailNotificationModelBase Base { get; private set; }
```

**Constructor:** `UserForgetPasswordNotificationViewModel(EmailNotificationModelBase base)`

**Template Usage:**
```html
<h1>Hello @Model.FullName,</h1>
<p>Click here to reset your password for @Model.Franchisee:</p>
<a href="@Model.PasswordLink">Reset Password</a>
```

---

#### SendLoginCredentialViewModel.cs
**Notification Types:** SendLoginCredential (2), SendLoginCredentialWithSetupGuide (21)

**Properties:**
```csharp
public string FullName { get; set; }
public string Password { get; set; }            // Temporary password
public string UserName { get; set; }            // Login username
public string Franchisee { get; set; }
public EmailNotificationModelBase Base { get; private set; }
```

**Template Usage:**
```html
<h1>Welcome @Model.FullName!</h1>
<p>Your login credentials for @Model.Franchisee:</p>
<p>Username: @Model.UserName</p>
<p>Password: @Model.Password</p>
```

---

### 3. Billing & Invoicing

#### InvoicePaymentReminderNotificationModel.cs
**Notification Types:** PaymentReminder (4)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public IList<PaymentViewModelForInvoice> InvoiceDetailList { get; set; }    // List of unpaid invoices
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }      // "block" or "none" (CSS display)
public string NotHasCustomSignature { get; set; }   // "block" or "none"
public string Signature { get; set; }               // HTML signature
```

**Signature Properties:** Used for conditional rendering of custom vs default signatures in templates.

**Template Usage:**
```html
<h1>Payment Reminder for @Model.Franchisee</h1>
<table>
@foreach(var invoice in Model.InvoiceDetailList) {
    <tr>
        <td>Invoice #@invoice.InvoiceId</td>
        <td>@invoice.Amount</td>
        <td>@invoice.DueDate</td>
    </tr>
}
</table>
<div style="display:@Model.HasCustomSignature">@Model.Signature</div>
<div style="display:@Model.NotHasCustomSignature">Default Signature</div>
```

---

#### PaymentConfirmationNotificationModel.cs
**Notification Type:** PaymentConfirmation (7)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public long InvoiceId { get; set; }
public string Amount { get; set; }                  // Payment amount
public string PaymentDate { get; set; }
public string GeneratedOn { get; set; }             // Invoice generation date
public string DueDate { get; set; }
public string AdFund { get; set; }                  // Ad fund amount
public string Royalty { get; set; }                 // Royalty amount
public ICollection<FranchiseeSalesPaymentEditModel> Payments { get; set; }  // Payment details
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
public EmailNotificationModelBase Base { get; private set; }
```

**Attribute:** `[NoValidatorRequired]` - Skips model validation

---

#### LateFeeReminderNotificationModel.cs
**Notification Types:** LateFeeReminderForPayment (6), LateFeeReminderForSalesData (8)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public string LateFeeAmount { get; set; }
public string OriginalAmount { get; set; }          // Invoice amount before late fee
public string TotalAmount { get; set; }             // Invoice + late fee
public string DueDate { get; set; }
public string LateFeeDate { get; set; }             // When late fee applied
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

---

### 4. Weekly/Monthly Reports

#### WeeklyNotificationListModel.cs
**Notification Types:** WeeklyLateFeeNotification (9), WeeklyUnpaidInvoiceNotification (10)

**Properties:**
```csharp
public IEnumerable<WeeklyNotificationReportViewModel> WeeklyCollection { get; set; }       // Current week data
public IEnumerable<WeeklyNotificationReportViewModel> PreviousCollection { get; set; }     // Previous week data
public IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> WeeklyCollectionFranchiseeWise { get; set; }  // Grouped by franchisee
public EmailNotificationModelBase Base { get; private set; }
public string FullName { get; set; }
public string StartDate { get; set; }
public string EndDate { get; set; }
public string TotalAmount { get; set; }             // Sum of all amounts
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

**Usage:** Weekly reports sent to admins with Excel attachments.

---

#### WeeklyNotificationReportViewModel.cs
**Purpose:** Individual invoice in weekly report.

**Properties:**
```csharp
public string Franchisee { get; set; }
public long InvoiceId { get; set; }
public DateTime StartDate { get; set; }
public DateTime EndDate { get; set; }
public string InvoiceAmount { get; set; }
public string PayableAmount { get; set; }
public string DueDate { get; set; }
public string LateFeeApplicable { get; set; }       // "Yes" or "No"
```

---

#### WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel.cs
**Purpose:** Franchisee-wise AR aging report.

**Properties:**
```csharp
public string Franchisee { get; set; }
public string Thirty { get; set; }                  // $ 0-30 days
public string Sixty { get; set; }                   // $ 30-60 days
public string Ninety { get; set; }                  // $ 60-90 days
public string moreThanNinety { get; set; }          // $ 90+ days
public string Total { get; set; }
public decimal TotalInt { get; set; }               // For sorting/calculations
```

**Attribute:** `[Description("Franchisee Wise")]` - Excel sheet name

---

#### MonthlyNotificationModel.cs
**Notification Types:** MonthlyReviewNotification (13), MonthlyMailChimpReport (14)

**Properties:**
```csharp
public string FullName { get; set; }
public string StartDate { get; set; }
public string EndDate { get; set; }
public EmailNotificationModelBase Base { get; private set; }
```

**Simple model for monthly summary emails with Excel attachments.**

---

### 5. Sales Data & Annual Audits

#### SalesdataUploadReminderNotificationModel.cs
**Notification Type:** SalesDataUploadReminder (5)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public string StartDate { get; set; }
public string EndDate { get; set; }
public string PaymentFrequency { get; set; }        // Monthly, Quarterly, etc.
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

---

#### AnnualAuditNotificationModel.cs
**Notification Types:** AnnualUpload* (16-20)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public string UploadDate { get; set; }
public string FileName { get; set; }
public string Status { get; set; }                  // Parsed, Failed, Approved, Rejected
public string Comments { get; set; }                // Admin comments on review
public EmailNotificationModelBase Base { get; private set; }
```

**Used for annual sales data upload notifications (parsing results, admin review actions).**

---

### 6. Document Management

#### DocumentUploadNotificationModel.cs
**Notification Types:** DocumentUploadNotification (22), DocumentUploadNotificationToFranchisee (24)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public string FileName { get; set; }
public string UploadedBy { get; set; }              // Uploader name
public string UploadDate { get; set; }
public EmailNotificationModelBase Base { get; private set; }
```

---

### 7. Job Scheduling

#### NewJobOrEstimateReminderNotificationModel.cs
**Notification Types:** NewJobNotificationToUser (25), NewJobNotificationToTech (26), UpdateJobNotificationToTech (28), etc. (15+ job-related types)

**Properties:**
```csharp
public string CustomerName { get; set; }
public string Franchisee { get; set; }
public string JobDate { get; set; }
public string JobTime { get; set; }
public string JobAddress { get; set; }
public string TechnicianName { get; set; }          // Assigned tech
public string JobType { get; set; }                 // Service type
public string SpecialInstructions { get; set; }
public string JobId { get; set; }
public List<TechListViewModel> TechList { get; set; }   // Multiple techs
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

**TechListViewModel** (nested):
```csharp
public string TechName { get; set; }
public string TechPhone { get; set; }
public string TechEmail { get; set; }
```

---

### 8. Customer Engagement

#### SendCustomerFeedbackNotificationModel.cs
**Notification Type:** CustomerFeedbackRequest (11)

**Properties:**
```csharp
public string CustomerName { get; set; }
public string CustomerEmail { get; set; }
public string Franchisee { get; set; }
public string FeedbackLink { get; set; }            // Survey URL
public EmailNotificationModelBase Base { get; private set; }
```

---

#### BeforeAfterImageMailNotificationModel.cs
**Notification Types:** BeforeAfterImages (34), InvoiceImages (35)

**Properties:**
```csharp
public string CustomerName { get; set; }
public string CustomerEmail { get; set; }
public string Franchisee { get; set; }
public string JobDate { get; set; }
public List<string> BeforeImageUrls { get; set; }   // Before photos
public List<string> AfterImageUrls { get; set; }    // After photos
public string CCMail { get; set; }                  // Additional CCs
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

---

#### BeforeAfterBestPairViewModel.cs
**Notification Type:** BeforeAfterBestPairMail (39)

**Properties:**
```csharp
public string CustomerName { get; set; }
public string BeforeImageUrl { get; set; }          // Best before image
public string AfterImageUrl { get; set; }           // Best after image
public string JobDescription { get; set; }
public string Testimonial { get; set; }             // Customer testimonial
public EmailNotificationModelBase Base { get; private set; }
```

---

### 9. Loan & Financial

#### FranchiseeLoanCompletionNotificationModel.cs
**Notification Type:** FranchiseeLoanCompletion (36)

**Properties:**
```csharp
public string FullName { get; set; }
public string Franchisee { get; set; }
public string LoanAmount { get; set; }
public string CompletionDate { get; set; }
public string TotalPaid { get; set; }
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

---

### 10. Miscellaneous

#### WebLeadsNotificationModel.cs
**Notification Type:** WebLeadsMail (57)

**Properties:**
```csharp
public string LeadName { get; set; }
public string LeadEmail { get; set; }
public string LeadPhone { get; set; }
public string LeadMessage { get; set; }
public string LeadSource { get; set; }              // Website, form, etc.
public string CCMail { get; set; }
public EmailNotificationModelBase Base { get; private set; }
```

---

#### NotificationToFAModel.cs
**Notification Type:** NotificationToFA (43)

**Properties:**
```csharp
public string Message { get; set; }
public string Subject { get; set; }
public EmailNotificationModelBase Base { get; private set; }
```

---

#### CustomerMailViewModel.cs
**Notification Types:** NewCustomerMail (37), UpdateCustomerMail (38)

**Properties:**
```csharp
public string CustomerName { get; set; }
public string CustomerEmail { get; set; }
public string Franchisee { get; set; }
public string Action { get; set; }                  // "New" or "Update"
public EmailNotificationModelBase Base { get; private set; }
public string HasCustomSignature { get; set; }
public string NotHasCustomSignature { get; set; }
public string Signature { get; set; }
```

---

#### DateRangeViewModel.cs
**Purpose:** Helper model for date range display.

**Properties:**
```csharp
public DateTime StartDate { get; set; }
public DateTime EndDate { get; set; }
```

---

### 11. Special Attributes

#### NotmappedAttribute.cs
**Purpose:** Custom attribute (not a ViewModel).

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class NotmappedAttribute : Attribute { }
```

**Usage:** Mark properties that should not be mapped to database (similar to EF's [NotMapped]).

---

## Common Patterns

### 1. Constructor Injection of Base
All ViewModels inject `EmailNotificationModelBase` via constructor:

```csharp
public ConcreteNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
{
    Base = emailNotificationModelBase;
}
```

**Why:** Ensures Base is always set, prevents null reference errors in templates.

### 2. Signature Properties
Many ViewModels include signature-related properties:

```csharp
public string HasCustomSignature { get; set; }      // CSS: "block" or "none"
public string NotHasCustomSignature { get; set; }   // CSS: "block" or "none"
public string Signature { get; set; }               // HTML signature content
```

**Usage in Template:**
```html
<div style="display:@Model.HasCustomSignature">
    @Model.Signature
</div>
<div style="display:@Model.NotHasCustomSignature">
    <p>Best Regards,<br/>@Model.Base.CompanyName</p>
</div>
```

**Set By:** NotificationService injects signatures for specific notification types from EmailSignatures table.

### 3. Collection Properties
ViewModels for lists/reports use `IList`, `ICollection`, or `IEnumerable`:

```csharp
public IList<PaymentViewModelForInvoice> InvoiceDetailList { get; set; }
public List<TechListViewModel> TechList { get; set; }
```

**Razor Loops:**
```html
@foreach(var item in Model.InvoiceDetailList) {
    <tr><td>@item.InvoiceId</td><td>@item.Amount</td></tr>
}
```

### 4. Attribute Usage
Some ViewModels use `[NoValidatorRequired]`:

```csharp
[NoValidatorRequired]
public class PaymentConfirmationNotificationModel { ... }
```

**Purpose:** Skip MVC model validation (used for notification models that don't require user input validation).

## Template Access Patterns

### Access Base Properties
```html
<!-- Company Logo -->
<img src="@Model.Base.CompanyLogoImageUrl" />

<!-- Company Name -->
<h1>@Model.Base.CompanyName</h1>

<!-- Build URLs -->
<a href="@Model.Base.GetUrl("/invoices/123")">View Invoice</a>

<!-- Address -->
<p>@Model.Base.Address.Street</p>
<p>@Model.Base.Address.City, @Model.Base.Address.State</p>
```

### Access Model Properties
```html
<!-- User Info -->
<p>Dear @Model.FullName,</p>
<p>Franchisee: @Model.Franchisee</p>

<!-- Lists -->
@foreach(var invoice in Model.InvoiceDetailList) {
    <p>Invoice #@invoice.InvoiceId: @invoice.Amount</p>
}

<!-- Conditional -->
@if (Model.HasSpecialInstructions) {
    <p>@Model.SpecialInstructions</p>
}
```

## ViewModel Lifecycle

```
1. UserNotificationModelFactory creates ViewModel
   ↓
2. Populates properties from domain entities
   ↓
3. Injects Base model (from NotificationModelFactory)
   ↓
4. Passes to NotificationService.QueueUpNotificationEmail()
   ↓
5. NotificationService loads EmailTemplate
   ↓
6. RazorEngine parses template with ViewModel
   ↓
7. Parsed HTML stored in NotificationEmail.Body
   ↓
8. Polling agent sends parsed email
```

## Design Considerations

### Why Constructor Injection for Base?
- **Immutability:** Base is set once, cannot be changed
- **Non-null Guarantee:** Templates can safely access `@Model.Base` without null checks
- **Consistency:** All ViewModels follow same pattern

### Why Separate ViewModels per Notification Type?
- **Type Safety:** Compiler catches missing properties
- **Template Clarity:** Clear which properties are available
- **Maintainability:** Changes to one notification don't affect others

### Why Not Use Inheritance?
- **Flexibility:** Some ViewModels have collections, some don't
- **Simplicity:** Composition (Base property) simpler than inheritance hierarchy
- **Razor Compatibility:** Razor handles composition better than deep inheritance

## Related Documentation
- [Parent Module](../.context/CONTEXT.md)
- [Domain Entities](../Domain/.context/CONTEXT.md)
- [Implementations](../Impl/.context/CONTEXT.md)
- [Enumerations](../Enum/.context/CONTEXT.md)
