# Core.Notification.ViewModel - Developer Guide

## What Are ViewModels?

ViewModels are **Data Transfer Objects (DTOs)** used for email template rendering. Think of them as the data contracts between C# code and HTML email templates.

**Analogy:** If email templates are forms with blank fields, ViewModels are the data that fills in those blanks.

## Quick Reference

### Common ViewModels

| ViewModel | Notification Type | Purpose |
|-----------|-------------------|---------|
| **UserForgetPasswordNotificationViewModel** | ForgetPassword | Password reset emails |
| **SendLoginCredentialViewModel** | SendLoginCredential | Welcome emails with credentials |
| **InvoicePaymentReminderNotificationModel** | PaymentReminder | Overdue payment reminders |
| **PaymentConfirmationNotificationModel** | PaymentConfirmation | Payment received confirmations |
| **WeeklyNotificationListModel** | WeeklyLateFee/UnpaidInvoice | Weekly admin reports |
| **NewJobOrEstimateReminderNotificationModel** | NewJobNotificationTo* | Job scheduling alerts |
| **SendCustomerFeedbackNotificationModel** | CustomerFeedbackRequest | Survey invitations |

### Base Model

**EmailNotificationModelBase** - Common properties for ALL notifications:
- Company logo, name, phone
- Site root URL (for building links)
- Owner information
- Helper methods

## Usage Examples

### Example 1: Password Reset Email

**ViewModel:**
```csharp
var baseModel = _notificationModelFactory.CreateBaseDefault();

var model = new UserForgetPasswordNotificationViewModel(baseModel) 
{
    FullName = "John Doe",
    PasswordLink = "https://app.company.com/reset?token=abc123",
    UserName = "john@example.com",
    Franchisee = "Detroit Office"
};
```

**Razor Template:**
```html
<html>
<body>
    <img src="@Model.Base.CompanyLogoImageUrl" />
    <h1>Hello @Model.FullName!</h1>
    <p>Someone requested a password reset for your account at @Model.Franchisee.</p>
    <p><a href="@Model.PasswordLink">Reset Your Password</a></p>
    <p>If you didn't request this, please ignore this email.</p>
    <p>Thanks,<br/>@Model.Base.CompanyName</p>
</body>
</html>
```

**Output:**
```html
<html>
<body>
    <img src="/images/logo.png" />
    <h1>Hello John Doe!</h1>
    <p>Someone requested a password reset for your account at Detroit Office.</p>
    <p><a href="https://app.company.com/reset?token=abc123">Reset Your Password</a></p>
    <p>If you didn't request this, please ignore this email.</p>
    <p>Thanks,<br/>MarbleLife</p>
</body>
</html>
```

### Example 2: Invoice Reminder with List

**ViewModel:**
```csharp
var baseModel = _notificationModelFactory.CreateBase(franchiseeId);

var model = new InvoicePaymentReminderNotificationModel(baseModel)
{
    FullName = "Jane Smith",
    Franchisee = "Philadelphia Office",
    InvoiceDetailList = new List<PaymentViewModelForInvoice> 
    {
        new PaymentViewModelForInvoice 
        {
            InvoiceId = 12345,
            Amount = "$1,250.00",
            DueDate = "2026-02-15",
            Status = "Overdue"
        },
        new PaymentViewModelForInvoice 
        {
            InvoiceId = 12350,
            Amount = "$875.50",
            DueDate = "2026-02-20",
            Status = "Due Soon"
        }
    },
    HasCustomSignature = "block",
    Signature = "<p>Best regards,<br/><b>John Manager</b></p>"
};
```

**Razor Template:**
```html
<h1>Payment Reminder - @Model.Franchisee</h1>
<p>Dear @Model.FullName,</p>
<p>The following invoices require your attention:</p>
<table border="1">
    <tr>
        <th>Invoice #</th>
        <th>Amount</th>
        <th>Due Date</th>
        <th>Status</th>
    </tr>
    @foreach(var invoice in Model.InvoiceDetailList) {
    <tr>
        <td>@invoice.InvoiceId</td>
        <td>@invoice.Amount</td>
        <td>@invoice.DueDate</td>
        <td>@invoice.Status</td>
    </tr>
    }
</table>
<div style="display:@Model.HasCustomSignature">
    @Model.Signature
</div>
```

### Example 3: Job Notification to Technician

**ViewModel:**
```csharp
var model = new NewJobOrEstimateReminderNotificationModel(baseModel)
{
    CustomerName = "Bob Customer",
    Franchisee = "Grand Rapids",
    JobDate = "February 12, 2026",
    JobTime = "2:00 PM - 4:00 PM",
    JobAddress = "123 Main St, Grand Rapids, MI 49503",
    TechnicianName = "Mike Technician",
    JobType = "Marble Restoration",
    SpecialInstructions = "Customer has a large dog - please call before arriving",
    JobId = "JOB-2026-0042",
    TechList = new List<TechListViewModel> 
    {
        new TechListViewModel 
        {
            TechName = "Mike Technician",
            TechPhone = "(616) 555-0100",
            TechEmail = "mike@company.com"
        },
        new TechListViewModel 
        {
            TechName = "Sarah Assistant",
            TechPhone = "(616) 555-0101",
            TechEmail = "sarah@company.com"
        }
    }
};
```

**Razor Template:**
```html
<h1>New Job Assignment - @Model.Franchisee</h1>
<p>Hi @Model.TechnicianName,</p>
<p>You have been assigned to the following job:</p>
<table>
    <tr><td><b>Job ID:</b></td><td>@Model.JobId</td></tr>
    <tr><td><b>Customer:</b></td><td>@Model.CustomerName</td></tr>
    <tr><td><b>Date:</b></td><td>@Model.JobDate</td></tr>
    <tr><td><b>Time:</b></td><td>@Model.JobTime</td></tr>
    <tr><td><b>Address:</b></td><td>@Model.JobAddress</td></tr>
    <tr><td><b>Service:</b></td><td>@Model.JobType</td></tr>
</table>
<p><b>Special Instructions:</b> @Model.SpecialInstructions</p>
<h3>Assigned Team:</h3>
<ul>
@foreach(var tech in Model.TechList) {
    <li>@tech.TechName - @tech.TechPhone</li>
}
</ul>
```

## ViewModel Patterns

### Pattern 1: Base Model Injection

**All ViewModels use constructor injection for Base:**
```csharp
public class MyNotificationModel
{
    public string MyProperty { get; set; }
    public EmailNotificationModelBase Base { get; private set; }
    
    // Constructor
    public MyNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
    {
        Base = emailNotificationModelBase;
    }
}
```

**Why:** Ensures Base is never null, provides type safety.

### Pattern 2: Signature Support

**Many ViewModels support custom signatures:**
```csharp
public string HasCustomSignature { get; set; }      // "block" or "none"
public string NotHasCustomSignature { get; set; }   // "block" or "none"
public string Signature { get; set; }               // HTML signature
```

**Template Usage:**
```html
<!-- Show custom signature if available -->
<div style="display:@Model.HasCustomSignature">
    @Model.Signature
</div>

<!-- Show default signature otherwise -->
<div style="display:@Model.NotHasCustomSignature">
    <p>Best regards,<br/>@Model.Base.CompanyName Team</p>
</div>
```

**Set Automatically:** NotificationService injects signatures from EmailSignatures table for applicable notification types.

### Pattern 3: Collection Properties

**Use IList for collections:**
```csharp
public IList<InvoiceDetailViewModel> InvoiceList { get; set; }
public List<TechListViewModel> TechList { get; set; }
```

**Template Iteration:**
```html
@foreach(var item in Model.InvoiceList) {
    <!-- Render each item -->
}
```

### Pattern 4: Base Property Access

**Access company info via Base:**
```html
<!-- Logo -->
<img src="@Model.Base.CompanyLogoImageUrl" alt="@Model.Base.CompanyName" />

<!-- Company Name -->
<h1>@Model.Base.CompanyName</h1>

<!-- Build Links -->
<a href="@Model.Base.GetUrl("/dashboard")">Go to Dashboard</a>

<!-- Contact Info -->
<p>Phone: @Model.Base.Phone</p>
<p>@Model.Base.OwnerName, @Model.Base.Designation</p>
```

## Creating Custom ViewModels

### Step 1: Define ViewModel Class

```csharp
using Core.Notification.ViewModel;

namespace Core.Notification.ViewModel
{
    public class MyCustomNotificationModel
    {
        // Custom properties
        public string RecipientName { get; set; }
        public string CustomMessage { get; set; }
        public DateTime EventDate { get; set; }
        
        // Required Base property
        public EmailNotificationModelBase Base { get; private set; }
        
        // Required constructor
        public MyCustomNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
```

### Step 2: Create Email Template

```html
<!-- Subject Template -->
@Model.RecipientName - Event Reminder

<!-- Body Template -->
<html>
<body>
    <img src="@Model.Base.CompanyLogoImageUrl" />
    <h1>Hello @Model.RecipientName!</h1>
    <p>@Model.CustomMessage</p>
    <p>Event Date: @Model.EventDate.ToString("MMMM dd, yyyy")</p>
    <p>Best regards,<br/>@Model.Base.CompanyName</p>
</body>
</html>
```

### Step 3: Add to UserNotificationModelFactory

```csharp
public void CreateMyCustomNotification(string recipientName, string message, DateTime eventDate)
{
    var baseModel = _notificationModelFactory.CreateBaseDefault();
    
    var model = new MyCustomNotificationModel(baseModel)
    {
        RecipientName = recipientName,
        CustomMessage = message,
        EventDate = eventDate
    };
    
    _notificationService.QueueUpNotificationEmail(
        NotificationTypes.MyCustomType,  // Add to enum
        model,
        fromName: _settings.CompanyName,
        fromEmail: _settings.FromEmail,
        toEmail: recipientEmail,
        notificationDateTime: DateTime.UtcNow,
        organizationRoleUserId: null
    );
}
```

## Troubleshooting

### Issue: "Property does not exist in current context"

**Error:**
```
RazorEngine.Templating.TemplateCompilationException:
The name 'CustomerName' does not exist in the current context
```

**Cause:** Template references `@Model.CustomerName` but ViewModel doesn't have that property.

**Fix:**
```csharp
// Add missing property to ViewModel
public string CustomerName { get; set; }
```

### Issue: Null Reference in Template

**Error:**
```
NullReferenceException: Object reference not set to an instance of an object
```

**Cause:** Accessing property that wasn't set (e.g., `@Model.InvoiceList` when list is null).

**Fix:**
```html
<!-- Add null check in template -->
@if (Model.InvoiceList != null && Model.InvoiceList.Any()) {
    @foreach(var invoice in Model.InvoiceList) {
        <!-- ... -->
    }
}
```

Or ensure property is initialized:
```csharp
public IList<InvoiceViewModel> InvoiceList { get; set; } = new List<InvoiceViewModel>();
```

### Issue: Base Properties Not Rendering

**Symptoms:** `@Model.Base.CompanyName` shows blank in email.

**Cause:** Base model not populated by factory.

**Fix:**
```csharp
// Verify factory call
var baseModel = _notificationModelFactory.CreateBaseDefault();

// Check Settings are configured
Console.WriteLine($"CompanyName: {_settings.CompanyName}");
Console.WriteLine($"LogoImage: {_settings.LogoImage}");
```

## ViewModel Categories Summary

| Category | ViewModels | Notification Types |
|----------|------------|-------------------|
| **Authentication** | UserForgetPassword, SendLoginCredential | 1-2, 21 |
| **Billing** | InvoicePaymentReminder, PaymentConfirmation, LateFeeReminder | 3-8, 50-52 |
| **Reports** | WeeklyNotificationList, MonthlyNotification | 9-14, 33 |
| **Sales Data** | SalesdataUploadReminder, AnnualAuditNotification | 5, 15-20 |
| **Documents** | DocumentUploadNotification | 22-24 |
| **Jobs** | NewJobOrEstimateReminderNotification | 25-32, 48 |
| **Customer** | SendCustomerFeedback, BeforeAfterImageMail | 11, 34-39, 53-56 |
| **Loans** | FranchiseeLoanCompletion | 36 |
| **Misc** | WebLeadsNotification, CustomerMail, NotificationToFA | 37-38, 41-47, 49, 57-63 |

## Best Practices

1. **Always inject Base:** All ViewModels must have Base property
2. **Initialize collections:** Set lists to empty list if not used
3. **Null-safe templates:** Use `@if` checks before iterating
4. **Descriptive properties:** Use clear property names (avoid abbreviations)
5. **Consistent naming:** Follow existing naming conventions
6. **Add [NoValidatorRequired]:** For notification-only models (no user input)

## Related Documentation

- [Parent Module](../.context/OVERVIEW.md)
- [Domain Entities](../Domain/.context/OVERVIEW.md)
- [Implementations](../Impl/.context/OVERVIEW.md)
