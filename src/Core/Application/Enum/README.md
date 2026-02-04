<!-- AUTO-GENERATED: Header -->
# Application Enums
> Standardized enumeration types for categorical data across the Marblelife system
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module contains three critical enums that power dropdown lists, file categorization, and user feedback throughout the application:

- **`LookupTypes`**: Category IDs for the dynamic lookup system (credit cards, invoice statuses, photo types)
- **`FileTypes`**: Document and image classification for storage routing
- **`MessageType`**: Success/warning/error semantics for user notifications

These enums provide **type safety** at compile time while enabling **business flexibility** through database-driven values.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Quick Start -->
## 🚀 Quick Start

### Adding the Namespace
```csharp
using Core.Application.Enum;
```

### Example 1: Get Dropdown Data
```csharp
// Get credit card types from the Lookup table
var lookupRepo = _unitOfWork.Repository<Lookup>();
var cardTypes = lookupRepo
    .Fetch(x => x.LookupTypeId == (long)LookupTypes.ChargeCardType && x.IsActive)
    .OrderBy(x => x.RelativeOrder)
    .Select(x => new DropdownListItem { 
        Display = x.Name, 
        Value = x.Id.ToString() 
    });
```

### Example 2: Categorize Uploaded Files
```csharp
// Determine file type from MIME type
FileTypes fileType;
if (contentType.StartsWith("image/"))
    fileType = FileTypes.Image;
else if (contentType == "application/pdf" || contentType.Contains("document"))
    fileType = FileTypes.Document;
else
    fileType = FileTypes.Other;

// Store in database
var file = new File { 
    FileName = uploadedFile.Name, 
    ContentType = contentType,
    FileTypeId = (int)fileType 
};
```

### Example 3: Return Feedback Messages
```csharp
// In your API controller
try
{
    _invoiceService.CreateInvoice(model);
    return FeedbackMessageModel.CreateSuccessMessage("Invoice created successfully");
}
catch (ValidationException ex)
{
    return FeedbackMessageModel.CreateWarningMessage($"Validation issue: {ex.Message}");
}
catch (Exception ex)
{
    _logger.Error(ex);
    return FeedbackMessageModel.CreateErrorMessage("Failed to create invoice");
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Enum Reference -->
## 📚 Enum Reference

### FileTypes

| Value | Enum Name | Description | Common File Extensions |
|-------|-----------|-------------|----------------------|
| `19` | `Document` | Non-image files | `.pdf`, `.doc`, `.docx`, `.xls`, `.xlsx`, `.txt` |
| `20` | `Image` | Photos and graphics | `.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp` |
| `21` | `Other` | Unclassified files | `.zip`, `.csv`, any unknown type |

**Usage Context**: File upload controllers, storage services, file retrieval APIs

---

### LookupTypes (Complete Reference)

#### Contact & Address
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `1` | `Phone` | Phone number types | Home, Work, Mobile, Fax |
| `2` | `Address` | Address categories | Home, Work, Billing, Shipping |

#### Financial Instruments
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `5` | `InstrumentType` | Payment methods | Check, ACH, Credit Card, Cash |
| `6` | `ChargeCardType` | Credit card brands | Visa, Mastercard, Amex, Discover |
| `7` | `AccountType` | Bank account types | Checking, Savings |
| `23` | `RoutingNumberCategory` | Routing number validation | Federal Reserve, Thrift, Credit Union |

#### Sales & CRM
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `8` | `SalesDataUploadStatus` | Import progress tracking | Pending, Processing, Completed, Failed |
| `17` | `CallType` | Call tracking categories | Inbound, Outbound, Follow-up, Voicemail |

#### Invoicing & Payments
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `9` | `InvoiceStatus` | Invoice lifecycle states | Draft, Sent, Paid, Overdue, Cancelled |
| `10` | `InvoiceItemType` | Line item categories | Service, Product, Fee, Discount, Tax |
| `81` | `Paid` | Payment fully received | (Likely a status code, not lookup category) |
| `83` | `PartialPayment` | Payment partially received | (Likely a status code, not lookup category) |

#### Service Categories
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `11` | `ServiceTypeCategory` | High-level service grouping | Restoration, Maintenance, Repair, Installation |
| `91` | `Service` | General service identifier | (Context-dependent) |
| `101` | `Restoration` | Marble/stone restoration | (Specific service category) |
| `102` | `Maintenance` | Maintenance contracts | (Specific service category) |

#### Fees & Charges
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `14` | `LateFee` | Late payment penalties | 5% Late Fee, $25 Flat Fee |
| `123` | `LateFees` | ⚠️ **Duplicate** of `14` | (Use `14` for new code) |
| `21` | `ServiceFeeType` | Recurring fee structures | Monthly, Annual, Per-Job, Percentage |
| `92` | `RoyaltyFee` | Franchise royalty charges | (Specific fee category) |
| `24` | `InterestIncome` | Interest on late payments | (Revenue category) |

#### Scheduling
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `22` | `RepeatFrequency` | Recurring job schedules | One-Time, Daily, Weekly, Bi-Weekly, Monthly, Quarterly, Annually |

#### Field Service Photos
| Value | Enum Name | Purpose | When Captured |
|-------|-----------|---------|---------------|
| `203` | `BeforeWork` | Before job photos | Start of service visit |
| `204` | `AfterWork` | After completion photos | End of service visit |
| `205` | `DuringWork` | In-progress documentation | During active work |
| `206` | `ExteriorBuilding` | Outside building photos | Site assessment |
| `207` | `InvoiceImages` | Invoice attachments | Linked to billing |

#### Document Management
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `24` | `DocumentCategory` | Document classification | Contract, Estimate, Warranty, Receipt |

#### Email Routing
| Value | Enum Name | Purpose | Email Field |
|-------|-----------|---------|-------------|
| `127` | `TO` | Primary recipients | To: field |
| `128` | `CC` | Carbon copy recipients | Cc: field |

#### Credits & Adjustments
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `20` | `AccountCreditType` | Credit reason codes | Refund, Discount, Goodwill, Adjustment |

#### Audit Trail
| Value | Enum Name | Purpose | Example Lookup Values |
|-------|-----------|---------|---------------------|
| `19` | `AuditActionType` | User action tracking | Create, Read, Update, Delete, Export, Print |

#### System Flags
| Value | Enum Name | Purpose | Notes |
|-------|-----------|---------|-------|
| `254` | `FRONTOFFICECALLMANAGEMENT` | Front office routing | Special system-level flag |

---

### MessageType

| Value | Enum Name | UI Color | Icon | Use Case |
|-------|-----------|----------|------|----------|
| `1` | `Success` | Green | ✓ | Operation completed successfully |
| `2` | `Warning` | Yellow/Orange | ⚠ | Non-critical issues, proceed with caution |
| `3` | `Error` | Red | ✗ | Operation failed, user action required |

**Important**: Values `2` and `3` are auto-incremented by the compiler. Do not insert new values between existing ones.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage Patterns -->
## 🛠 Common Usage Patterns

### Pattern 1: Building Dropdowns

**Scenario**: Populate a dropdown list for user selection

```csharp
public IEnumerable<DropdownListItem> GetInvoiceStatusDropdown()
{
    var lookupRepo = _unitOfWork.Repository<Lookup>();
    
    return lookupRepo
        .Fetch(x => x.LookupTypeId == (long)LookupTypes.InvoiceStatus && x.IsActive)
        .OrderBy(x => x.RelativeOrder)
        .Select(x => new DropdownListItem 
        { 
            Display = x.Name,   // User-visible text
            Value = x.Id.ToString()  // Hidden value for form submission
        })
        .ToArray();
}
```

**Why the cast?** `LookupTypeId` is `BIGINT` (64-bit) in the database, but enums are `int` (32-bit). The `(long)` cast ensures type compatibility.

---

### Pattern 2: Filtering by Multiple Types

**Scenario**: Exclude certain categories from a query

```csharp
// Get all job services EXCEPT invoice images
var jobEstimates = _jobEstimateServices.Table
    .Where(x => 
        x.CategoryId.HasValue 
        && validCategories.Contains(x.CategoryId.Value)
        && x.TypeId != (long)LookupTypes.InvoiceImages  // Exclude this type
    )
    .ToList();
```

---

### Pattern 3: Image Categorization Workflow

**Scenario**: Technician uploads before/during/after photos via mobile app

```csharp
public void UploadJobPhoto(long jobId, IFormFile photo, string stage)
{
    // Determine type based on workflow stage
    LookupTypes photoType = stage switch
    {
        "before" => LookupTypes.BeforeWork,
        "during" => LookupTypes.DuringWork,
        "after" => LookupTypes.AfterWork,
        _ => throw new ArgumentException($"Invalid stage: {stage}")
    };
    
    // Create image record
    var image = new JobEstimateImage
    {
        ServiceId = jobId,
        FileId = savedFileId,
        TypeId = (long)photoType,
        UploadedAt = DateTime.UtcNow
    };
    
    _repository.Add(image);
}
```

---

### Pattern 4: Email Recipient Segregation

**Scenario**: Send invoice notification with proper TO/CC recipients

```csharp
public void SendInvoiceEmail(long invoiceId)
{
    var invoice = _invoiceRepo.GetById(invoiceId);
    var recipients = invoice.NotificationEmail.Recipients;
    
    // Separate TO and CC recipients using LookupTypes
    var toAddresses = recipients
        .Where(x => x.RecipientTypeId == (long)LookupTypes.TO)
        .Select(x => x.RecipientEmail)
        .ToList();
    
    var ccAddresses = recipients
        .Where(x => x.RecipientTypeId == (long)LookupTypes.CC)
        .Select(x => x.RecipientEmail)
        .ToList();
    
    _emailService.Send(
        to: toAddresses, 
        cc: ccAddresses, 
        subject: $"Invoice #{invoice.Number}",
        body: GenerateInvoiceHtml(invoice)
    );
}
```

---

### Pattern 5: Typed Feedback Messages

**Scenario**: API controller returns operation result with appropriate styling

```csharp
[HttpPost]
public IHttpActionResult CreateCustomer(CustomerViewModel model)
{
    try
    {
        // Validate input
        if (!ModelState.IsValid)
        {
            return Ok(FeedbackMessageModel.CreateWarningMessage(
                "Please correct validation errors before submitting"
            ));
        }
        
        // Attempt to save
        var customer = _customerService.Create(model);
        
        return Ok(FeedbackMessageModel.CreateSuccessMessage(
            $"Customer '{customer.Name}' created successfully"
        ));
    }
    catch (DuplicateCustomerException ex)
    {
        return Ok(FeedbackMessageModel.CreateWarningMessage(
            $"Customer already exists: {ex.Message}"
        ));
    }
    catch (Exception ex)
    {
        _logger.Error("Failed to create customer", ex);
        return Ok(FeedbackMessageModel.CreateErrorMessage(
            "An error occurred. Please contact support."
        ));
    }
}
```

**Frontend Handling** (AngularJS example):
```javascript
$http.post('/api/customers', customerData).then(function(response) {
    var feedback = response.data;
    
    // Display message with appropriate styling
    switch (feedback.MessageType) {
        case 1: // Success
            toastr.success(feedback.Message);
            break;
        case 2: // Warning
            toastr.warning(feedback.Message);
            break;
        case 3: // Error
            toastr.error(feedback.Message);
            break;
    }
});
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Best Practices -->
## ✅ Best Practices

### DO ✓

1. **Always cast enum to `long` when comparing with database columns**
   ```csharp
   x.LookupTypeId == (long)LookupTypes.ChargeCardType  // ✓ Correct
   ```

2. **Use explicit enum names for readability**
   ```csharp
   if (status == MessageType.Success) { ... }  // ✓ Clear
   ```

3. **Filter by `IsActive` when querying Lookup table**
   ```csharp
   .Where(x => x.LookupTypeId == (long)LookupTypes.Phone && x.IsActive)  // ✓ Correct
   ```

4. **Order dropdown results by `RelativeOrder`**
   ```csharp
   .OrderBy(x => x.RelativeOrder)  // ✓ Respects admin-defined sort order
   ```

5. **Use factory methods for MessageType**
   ```csharp
   FeedbackMessageModel.CreateSuccessMessage("Done");  // ✓ Type-safe
   ```

### DON'T ✗

1. **Never modify enum integer values**
   ```csharp
   ChargeCardType = 99,  // ✗ BREAKING CHANGE—breaks existing data!
   ```

2. **Don't use magic numbers instead of enums**
   ```csharp
   if (lookupTypeId == 6) { ... }  // ✗ What is 6?
   if (lookupTypeId == (long)LookupTypes.ChargeCardType) { ... }  // ✓ Clear
   ```

3. **Don't skip the `IsActive` check**
   ```csharp
   .Where(x => x.LookupTypeId == (long)LookupTypes.Phone)  // ✗ May return deactivated items
   ```

4. **Don't create `new FeedbackMessageModel()` directly**
   ```csharp
   var msg = new FeedbackMessageModel { Message = "...", MessageType = 1 };  // ✗ Constructor is private
   var msg = FeedbackMessageModel.CreateSuccessMessage("...");  // ✓ Use factory
   ```

5. **Don't assume enum values are sequential**
   ```csharp
   for (int i = 1; i <= 30; i++)  // ✗ Many gaps in LookupTypes
   ```

### ⚠️ CRITICAL WARNING

**NEVER DELETE OR REORDER ENUM VALUES**

These values are persisted in the database. Changing them will cause:
- ❌ Foreign key constraint violations
- ❌ Incorrect dropdown selections
- ❌ Data corruption in production

**If a type is deprecated**:
1. Mark as `[Obsolete("Use XYZ instead")]`
2. Migrate data to new value
3. Leave old enum in place (for backward compatibility)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Integration Guide -->
## 🔗 Integration Guide

### For New Features

#### Adding a New Lookup Category

1. **Choose an ID**: Pick the next available value (suggest starting at `500+` for new features to avoid conflicts)
   
2. **Add to enum**:
   ```csharp
   public enum LookupTypes
   {
       // ... existing values ...
       
       MyNewCategory = 501,  // Description of what this categorizes
   }
   ```

3. **Create database migration**:
   ```sql
   -- Seed LookupType table (optional—may already exist from earlier migration)
   INSERT INTO LookupType (Id, Name) VALUES (501, 'MyNewCategory');
   
   -- Seed default Lookup values
   INSERT INTO Lookup (LookupTypeId, Name, Alias, RelativeOrder, IsActive)
   VALUES 
       (501, 'Option 1', 'OPT1', 1, 1),
       (501, 'Option 2', 'OPT2', 2, 1),
       (501, 'Option 3', 'OPT3', 3, 1);
   ```

4. **Add service method** (in `DropDownHelperService` or your module's service):
   ```csharp
   public IEnumerable<DropdownListItem> GetMyNewCategoryDropdown()
   {
       var lookupRepo = _unitOfWork.Repository<Lookup>();
       return lookupRepo
           .Fetch(x => x.LookupTypeId == (long)LookupTypes.MyNewCategory && x.IsActive)
           .OrderBy(x => x.RelativeOrder)
           .Select(x => new DropdownListItem { Display = x.Name, Value = x.Id.ToString() })
           .ToArray();
   }
   ```

5. **Update documentation**: Add your new type to this README's reference table

---

### For API Consumers

#### Retrieving Dropdown Data

**Endpoint**: `GET /api/dropdown/cardtypes` (example)

**Response**:
```json
[
  { "Display": "Visa", "Value": "42" },
  { "Display": "Mastercard", "Value": "43" },
  { "Display": "American Express", "Value": "44" }
]
```

**AngularJS Usage**:
```javascript
$http.get('/api/dropdown/cardtypes').then(function(response) {
    $scope.cardTypes = response.data;
});
```

```html
<select ng-model="payment.cardTypeId" 
        ng-options="card.Value as card.Display for card in cardTypes">
</select>
```

---

#### Handling Feedback Messages

**API Response Format**:
```json
{
  "Message": "Invoice created successfully",
  "MessageType": 1
}
```

**JavaScript Interpretation**:
```javascript
function displayFeedback(feedback) {
    const icons = { 1: '✓', 2: '⚠', 3: '✗' };
    const colors = { 1: 'green', 2: 'orange', 3: 'red' };
    
    showNotification({
        text: icons[feedback.MessageType] + ' ' + feedback.Message,
        color: colors[feedback.MessageType],
        duration: feedback.MessageType === 1 ? 3000 : 5000  // Success shorter
    });
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Troubleshooting -->
## 🔧 Troubleshooting

### Issue: "Cannot convert int to long"

**Error Message**:
```
Cannot implicitly convert type 'int' to 'long'
```

**Cause**: Enum is `int` (32-bit) but database column is `BIGINT` (64-bit)

**Solution**: Add explicit cast
```csharp
x.LookupTypeId == (long)LookupTypes.ChargeCardType  // Cast enum to long
```

---

### Issue: Dropdown Returns Deactivated Items

**Symptom**: Dropdown shows "Inactive" or deleted options

**Cause**: Missing `IsActive` filter in query

**Solution**: Always include `&& x.IsActive` in LINQ query
```csharp
.Fetch(x => x.LookupTypeId == (long)LookupTypes.Phone && x.IsActive)  // Add IsActive check
```

---

### Issue: Wrong Enum Value Used

**Symptom**: Photos appear in wrong category or dropdowns show incorrect items

**Cause**: Using wrong `LookupTypes` enum value (e.g., `LateFee` vs `LateFees`)

**Solution**: 
1. Grep the codebase for existing usage: `grep -r "LookupTypes.LateFee" src/`
2. Use the most common value
3. Check this README's reference table for canonical value

---

### Issue: Enum Value Changed, Data Corrupted

**Symptom**: Dropdowns broken, queries return no results, foreign key errors

**Cause**: Someone modified an enum value (e.g., changed `ChargeCardType = 6` to `ChargeCardType = 99`)

**Solution**: 
1. **Immediate**: Revert the enum change in code
2. **Recovery**: If database was migrated, restore from backup or run data fix script
3. **Prevention**: Add code review rule: "Enum value changes require DBA approval"

---

### Issue: MessageType Not Displaying Correctly

**Symptom**: Frontend shows wrong icon/color for feedback message

**Cause**: Frontend JavaScript expects different integer values

**Solution**: Ensure frontend constants match C# enum:
```javascript
// Must match MessageType enum exactly
const MessageType = {
    SUCCESS: 1,
    WARNING: 2,
    ERROR: 3
};
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: FAQ -->
## ❓ FAQ

### Why are LookupTypes values non-sequential?

The gaps (1, 2, 5, 6, 7...) occur because:
- Some types were removed from earlier versions
- Different dev teams reserved ID ranges
- IDs were manually assigned rather than auto-incremented

**Impact**: No functional issue—just a quirk of the system's evolution.

---

### Can I add a new value between existing ones?

**No!** For example, don't insert a value between `Phone = 1` and `Address = 2`.

**Why?** The auto-increment behavior of C# enums would shift all subsequent values:
```csharp
Phone = 1,
NewType,     // This would be 2
Address,     // This would now be 3 (was 2—breaks data!)
```

**Instead**: Add new values at the end or in gaps (e.g., use `3` or `4` which are unused).

---

### Why doesn't LookupTypes have comments in code?

**Historical reason**: Original developers didn't document enums (common in legacy .NET projects).

**Mitigation**: This README serves as the definitive documentation. Link to it in code reviews and onboarding.

---

### Should I use LookupTypes or hardcoded constants?

**Use LookupTypes when**:
- Values already exist in the `Lookup` table
- Dropdown logic is needed
- Multi-tenancy (different franchisees might have custom values)

**Use hardcoded constants when**:
- Values never change (e.g., `MessageType`)
- No database lookup needed
- Small, fixed set of options

---

### What's the difference between LateFee and LateFees?

**Short answer**: They're duplicates—use `LateFee = 14` for consistency.

**Long answer**: 
- `LateFee = 14` is the original value
- `LateFees = 123` was added later (likely during refactoring)
- Both refer to late payment penalties
- Using `14` is more common in the codebase

**Future**: Should be consolidated in a future tech debt sprint.
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Related Resources -->
## 📖 Related Documentation

- **[AI-CONTEXT.md](./AI-CONTEXT.md)**: Deep architectural analysis and design insights
- **[Application.Domain Module](../Domain/README.md)**: `Lookup` and `LookupType` entity definitions
- **[DropDownHelperService](../../../../API/Areas/Application/Impl/DropDownHelperService.cs)**: Primary consumer of `LookupTypes`
- **[FeedbackMessageModel](../ViewModel/FeedbackMessageModel.cs)**: `MessageType` implementation

---

**💬 Questions or Issues?**
- Check `AI-CONTEXT.md` for deeper technical details
- Search codebase for usage examples: `grep -r "LookupTypes.YourType" src/`
- Consult with the Core team before modifying enum values
<!-- END CUSTOM SECTION -->
