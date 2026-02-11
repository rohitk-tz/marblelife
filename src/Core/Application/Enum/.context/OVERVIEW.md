<!-- AUTO-GENERATED: Header -->
# Enum
> Strongly-typed enumerations for message types, file categories, and database lookup table mappings
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Enum** module defines three enumerations that provide type safety and semantic clarity throughout the application. These enums eliminate magic numbers and string literals, making code more maintainable and less error-prone.

**Three Key Enums:**
1. **MessageType**: UI notification severity (Success, Warning, Error)
2. **FileTypes**: File category classification (Document, Image, Other)
3. **LookupTypes**: Database lookup table category IDs (40+ business domain categories)

**Why enums matter here:**
- **Type Safety**: Compiler catches invalid values at build time
- **IntelliSense**: Developers see valid options while coding
- **Refactoring**: Rename enum values and all usages update automatically
- **Database Sync**: `LookupTypes` enum values match `LookupType.Id` primary keys in database
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Display User Feedback Message
```csharp
using Core.Application.Enum;
using Core.Application.ViewModel;

// Create success notification (green toast)
var response = new ResponseModel();
response.SetSuccessMessage("Invoice saved successfully!");
// Internally sets: Message.MessageType = MessageType.Success

// Create error notification (red toast)
response.SetErrorMessage("Failed to save invoice.", ResponseErrorCodeType.ValidationFailure);
// Internally sets: Message.MessageType = MessageType.Error
```

### Example 2: Categorize Uploaded File
```csharp
using Core.Application.Enum;

string mimeType = "application/pdf";

FileTypes category = mimeType.StartsWith("image/") 
    ? FileTypes.Image 
    : mimeType.StartsWith("application/") 
        ? FileTypes.Document 
        : FileTypes.Other;

// category = FileTypes.Document (value 19)
```

### Example 3: Query Lookup Values by Type
```csharp
using Core.Application.Enum;

// Get all invoice statuses (Draft, Sent, Paid, Overdue, etc.)
var invoiceStatuses = dbContext.Lookups
    .Where(l => l.LookupTypeId == (long)LookupTypes.InvoiceStatus)  // InvoiceStatus = 9
    .Where(l => l.IsActive)
    .OrderBy(l => l.RelativeOrder)
    .ToList();

// Get credit card types (Visa, Mastercard, Amex, etc.)
var cardTypes = dbContext.Lookups
    .Where(l => l.LookupTypeId == (long)LookupTypes.ChargeCardType)  // ChargeCardType = 6
    .ToList();
```

### Example 4: Filter Images by Category
```csharp
using Core.Application.Enum;

// Get all before-work photos for an invoice
var beforePhotos = dbContext.Files
    .Join(dbContext.Lookups, 
        f => f.CategoryId, 
        l => l.Id, 
        (f, l) => new { File = f, Lookup = l })
    .Where(x => x.Lookup.LookupTypeId == (long)LookupTypes.BeforeWork)  // BeforeWork = 203
    .Select(x => x.File)
    .ToList();
```

### Example 5: Check Service Type
```csharp
using Core.Application.Enum;

long serviceTypeId = 101;  // From database

if (serviceTypeId == (long)LookupTypes.Restoration)
{
    Console.WriteLine("This is a restoration service");
}
else if (serviceTypeId == (long)LookupTypes.Maintenance)
{
    Console.WriteLine("This is a maintenance service");
}
```

### Example 6: Seed Lookup Values
```csharp
using Core.Application.Enum;
using Core.Application.Domain;

// Ensure LookupType exists with correct ID
var invoiceStatusType = new LookupType 
{ 
    Id = (long)LookupTypes.InvoiceStatus,  // 9
    Name = "Invoice Status" 
};

// Add lookup values for this type
var lookups = new[]
{
    new Lookup { LookupTypeId = (long)LookupTypes.InvoiceStatus, Name = "Draft", RelativeOrder = 1 },
    new Lookup { LookupTypeId = (long)LookupTypes.InvoiceStatus, Name = "Sent", RelativeOrder = 2 },
    new Lookup { LookupTypeId = (long)LookupTypes.InvoiceStatus, Name = "Paid", RelativeOrder = 3 }
};
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### MessageType
| Value | Int | UI Representation |
|-------|-----|-------------------|
| `Success` | 1 | Green notification, checkmark icon |
| `Warning` | 2 | Yellow/orange alert, warning triangle |
| `Error` | 3 | Red notification, X icon |

### FileTypes
| Value | Int | Description |
|-------|-----|-------------|
| `Document` | 19 | PDFs, Word docs, Excel, text files |
| `Image` | 20 | JPEG, PNG, GIF, BMP, SVG |
| `Other` | 21 | Archives, videos, unknown types |

### LookupTypes (Key Categories)
| Category | Value | Description |
|----------|-------|-------------|
| **Financial** | | |
| `ChargeCardType` | 6 | Credit/debit card types |
| `AccountType` | 7 | Bank account types |
| `LateFee` | 14 | Late fee configurations |
| `RoyaltyFee` | 92 | Franchise royalty charges |
| **Invoice** | | |
| `InvoiceStatus` | 9 | Draft, Sent, Paid, Overdue |
| `InvoiceItemType` | 10 | Service, Product, Discount |
| `Paid` | 81 | Fully paid status |
| `PartialPayment` | 83 | Partially paid status |
| **Service** | | |
| `ServiceTypeCategory` | 11 | Service categories |
| `Restoration` | 101 | Restoration services |
| `Maintenance` | 102 | Maintenance services |
| **Images** | | |
| `BeforeWork` | 203 | Before-work photos |
| `AfterWork` | 204 | After-work photos |
| `DuringWork` | 205 | In-progress photos |
| **Communication** | | |
| `CallType` | 17 | Inbound, outbound calls |
| `TO` | 127 | Email TO recipients |
| `CC` | 128 | Email CC recipients |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Query returns no results despite data existing
**Cause**: Forgot to cast enum to long: `l.LookupTypeId == LookupTypes.InvoiceStatus`
**Solution**: Always cast: `l.LookupTypeId == (long)LookupTypes.InvoiceStatus`
```csharp
// ❌ WRONG - Compiler error or runtime comparison failure
var lookups = db.Lookups.Where(l => l.LookupTypeId == LookupTypes.InvoiceStatus);

// ✅ CORRECT
var lookups = db.Lookups.Where(l => l.LookupTypeId == (long)LookupTypes.InvoiceStatus);
```

### Issue: Added new enum value but lookups fail
**Cause**: Database `LookupType` or `Lookup` tables not seeded.
**Solution**: Run migration to insert matching database records:
```sql
-- Add LookupType if new category
INSERT INTO LookupType (Id, Name) VALUES (25, 'NewCategory');

-- Add Lookup values for the new type
INSERT INTO Lookup (LookupTypeId, Name, IsActive, RelativeOrder) 
VALUES (25, 'Option1', 1, 1);
```

### Issue: FileTypes enum doesn't match file categories
**Cause**: Values 19, 20, 21 must match `Lookup.Id` in database where `LookupTypeId` refers to file categories.
**Solution**: Verify database has these exact IDs:
```sql
SELECT * FROM Lookup WHERE Id IN (19, 20, 21);
```

### Issue: MessageType not displaying correct color in UI
**Cause**: Frontend code may be checking enum name instead of value.
**Solution**: Ensure frontend checks `MessageType` property (1, 2, 3) not the string name:
```javascript
// ✅ CORRECT
if (response.Message.MessageType === 1) { /* green */ }

// ❌ WRONG
if (response.Message.MessageType === "Success") { /* fails */ }
```

### Issue: "Magic number" code smell warnings
**Cause**: Using raw numbers instead of enums: `if (lookupTypeId == 9)`
**Solution**: Always use enum: `if (lookupTypeId == (long)LookupTypes.InvoiceStatus)`

### Issue: Which LookupTypes value to use?
**Reference**: Check `LookupType` table in database for canonical list, or use IntelliSense to browse enum values with descriptions.
<!-- END CUSTOM SECTION -->
