<!-- AUTO-GENERATED: Header -->
# Application Enum Module Context
**Version**: 0a67ee9afcea8c38fe993a5778468349af995029
**Generated**: 2026-02-04T06:19:42Z
**Path**: `src/Core/Application/Enum`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module defines **application-wide enumeration types** that establish standardized categorical values used throughout the Marblelife system. These enums serve as the backbone for:
- **Dynamic Lookup System**: The `LookupTypes` enum defines categories for the dynamic `Lookup` entity pattern, enabling database-driven dropdown values without code changes
- **File Type Classification**: The `FileTypes` enum categorizes uploaded documents and images for storage routing and display logic
- **User Feedback Messaging**: The `MessageType` enum controls the visual presentation (success/warning/error) of system notifications to users

### Design Patterns

#### 1. **Enum-Driven Lookup Pattern**
The system uses a sophisticated two-tier lookup architecture:
- **Static Layer**: `LookupTypes` enum defines hardcoded category IDs
- **Dynamic Layer**: `Lookup` database table stores actual values associated with each `LookupTypeId`
- **Benefit**: Business users can modify dropdown values (e.g., add new credit card types) without code deployment

**How it Works**:
```csharp
// Code references the enum
var cardTypes = lookupRepository.Fetch(x => 
    x.LookupTypeId == (long)LookupTypes.ChargeCardType && x.IsActive
);
// Database stores: { Id: 42, LookupTypeId: 6, Name: "Visa", IsActive: true }
```

#### 2. **Magic Number Elimination**
All enum values use **explicit integer assignments** to ensure database referential integrity. This is critical because:
- Values are stored in SQL Server tables as foreign keys
- Changing enum order would break existing data relationships
- Example: `Document = 19` ensures ID 19 always means "Document" across all environments

#### 3. **Type-Safe Categorical Data**
Enums replace brittle string comparisons and prevent invalid state:
```csharp
// ❌ Bad: if (messageType == "success") 
// ✅ Good: if (messageType == MessageType.Success)
```

### Data Flow

#### Lookup Resolution Flow
1. **API Controller** receives request for dropdown data (e.g., "Get card types")
2. **DropDownHelperService** queries: `LookupTypeId == (long)LookupTypes.ChargeCardType`
3. **Repository** fetches matching `Lookup` records from database
4. **Transform** to `DropdownListItem` view models
5. **Return** JSON to frontend for UI rendering

#### File Type Routing Flow
1. **User uploads** file via API endpoint
2. **FileService** inspects content type (MIME)
3. **FileFactory** assigns `FileTypes.Image` or `FileTypes.Document`
4. **Storage layer** routes to appropriate folder/bucket based on type
5. **Retrieval** uses type to determine display rendering (thumbnail vs. download link)

#### Message Type Display Flow
1. **Business logic** executes operation (e.g., save invoice)
2. **Success/failure** determines message type
3. **FeedbackMessageModel.CreateSuccessMessage()** constructs typed model
4. **API serializes** `{ "Message": "Saved", "MessageType": 1 }`
5. **Frontend** displays colored alert box based on `MessageType` value

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Enum Definitions

### `FileTypes` Enum
```csharp
namespace Core.Application.Enum
{
    public enum FileTypes
    {
        Document = 19,  // PDF, Word, Excel files
        Image = 20,     // JPEG, PNG, GIF images
        Other = 21      // Fallback for unclassified files
    }
}
```

**Purpose**: Categorizes uploaded files for storage and retrieval logic.

**Value Meanings**:
| Value | Name | Use Case |
|-------|------|----------|
| `19` | `Document` | Contracts, invoices, reports (non-image files) |
| `20` | `Image` | Photos, screenshots, technician work photos |
| `21` | `Other` | Miscellaneous or unknown file types |

**Key Characteristics**:
- Values `19-21` align with legacy `ContentType` or `FileCategory` table IDs
- Sparse numbering suggests removed intermediate types from earlier versions
- Used primarily in `FileFactory` and file upload controllers

---

### `LookupTypes` Enum
```csharp
namespace Core.Application.Enum
{
    public enum LookupTypes
    {
        // Contact & Address
        Phone = 1,
        Address = 2,
        
        // Financial Instruments
        InstrumentType = 5,          // Check, ACH, Credit Card
        ChargeCardType = 6,          // Visa, Mastercard, Amex
        AccountType = 7,             // Checking, Savings
        RoutingNumberCategory = 23,  // Bank routing validation
        
        // Sales & Upload
        SalesDataUploadStatus = 8,   // Pending, Processed, Failed
        CallType = 17,               // Inbound, Outbound, Follow-up
        
        // Invoicing
        InvoiceStatus = 9,           // Draft, Sent, Paid, Overdue
        InvoiceItemType = 10,        // Service, Product, Fee, Discount
        Paid = 81,                   // Payment status: fully paid
        PartialPayment = 83,         // Payment status: partially paid
        
        // Service Categories
        ServiceTypeCategory = 11,    // Restoration vs. Maintenance
        Service = 91,                // General service category
        Restoration = 101,           // Marble/stone restoration
        Maintenance = 102,           // Maintenance contracts
        
        // Fees
        LateFee = 14,                // Late payment penalty
        LateFees = 123,              // (Duplicate?) Late fees plural
        ServiceFeeType = 21,         // Monthly, Annual, Per-job fees
        RoyaltyFee = 92,             // Franchise royalty charges
        InterestIncome = 24,         // Interest on late payments
        
        // Scheduling
        RepeatFrequency = 22,        // One-time, Weekly, Monthly, Quarterly
        
        // Image Categories (Field Service)
        BeforeWork = 203,            // Photos taken before job starts
        AfterWork = 204,             // Photos after completion
        DuringWork = 205,            // In-progress documentation
        ExteriorBuilding = 206,      // Outside building photos
        InvoiceImages = 207,         // Photos attached to invoices
        
        // Document Management
        DocumentCategory = 24,       // Contract, Estimate, Warranty
        
        // Email
        TO = 127,                    // Email "To" recipient
        CC = 128,                    // Email "CC" recipient
        
        // Credits & Adjustments
        AccountCreditType = 20,      // Refund, Discount, Adjustment
        
        // Audit Trail
        AuditActionType = 19,        // Create, Update, Delete, View
        
        // System Flags
        FRONTOFFICECALLMANAGEMENT = 254  // Special front office routing flag
    }
}
```

**Purpose**: Defines **category identifiers** for the dynamic `Lookup` table, enabling flexible dropdown values without code changes.

**Architecture Notes**:
- **Non-Sequential IDs**: Values are not contiguous (1,2,5,6,7...), indicating:
  - Legacy evolution with removed categories
  - Multi-team development with reserved ID ranges
  - Database migration constraints from older systems
- **Duplicate Semantics**: `LateFee=14` vs `LateFees=123` suggests refactoring debt
- **Domain Clustering**:
  - `1-24`: Core application lookups
  - `81-102`: Business logic categories
  - `123+`: Later additions or special cases
  - `203-207`: Image workflow types
  - `254`: System-level flags

**Critical Relationships**:
```csharp
// Lookup table structure
public class Lookup : DomainBase
{
    public long LookupTypeId { get; set; }  // References LookupTypes enum
    public string Name { get; set; }        // Display value
    public string Alias { get; set; }       // Optional short code
    public byte? RelativeOrder { get; set; } // Sort order in dropdown
    public bool IsActive { get; set; }      // Soft delete flag
}
```

**Usage Example**:
```csharp
// In DropDownHelperService.cs
public IEnumerable<DropdownListItem> GetCardType()
{
    var lookupRepository = _unitOfWork.Repository<Lookup>();
    return lookupRepository
        .Fetch(x => x.LookupTypeId == (long)LookupTypes.ChargeCardType && x.IsActive)
        .OrderBy(x => x.RelativeOrder)
        .Select(s => new DropdownListItem { 
            Display = s.Name, 
            Value = s.Id.ToString() 
        })
        .ToArray();
}
```

---

### `MessageType` Enum
```csharp
namespace Core.Application.Enum
{
    public enum MessageType
    {
        Success = 1,  // Green success notifications
        Warning,      // Yellow/orange warnings (auto-increments to 2)
        Error         // Red error messages (auto-increments to 3)
    }
}
```

**Purpose**: Controls the visual styling and semantic meaning of user-facing feedback messages.

**Value Meanings**:
| Value | Name | Use Case | Typical UI |
|-------|------|----------|-----------|
| `1` | `Success` | Operation completed successfully | Green checkmark, "✓ Invoice saved" |
| `2` | `Warning` | Non-fatal issues, proceed with caution | Yellow warning icon, "⚠ Data incomplete" |
| `3` | `Error` | Operation failed, user action required | Red X, "✗ Payment declined" |

**Design Notes**:
- **Implicit Numbering**: Only `Success=1` is explicit; `Warning` and `Error` auto-increment
- **C# Enum Behavior**: Compiler assigns `Warning=2` and `Error=3` automatically
- **Trade-off**: More concise code, but fragile—inserting new values changes subsequent IDs

**Integration**:
```csharp
// From FeedbackMessageModel.cs
[DataContract]
public class FeedbackMessageModel
{
    [DataMember]
    public string Message { get; private set; }
    
    [DataMember]
    public MessageType MessageType { get; private set; }
    
    public static FeedbackMessageModel CreateSuccessMessage(string message)
    {
        return CreateModel(message, MessageType.Success);
    }
    
    public static FeedbackMessageModel CreateWarningMessage(string message)
    {
        return CreateModel(message, MessageType.Warning);
    }
    
    public static FeedbackMessageModel CreateErrorMessage(string message)
    {
        return CreateModel(message, MessageType.Error);
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public API & Consumption Patterns

### Enum Access Pattern
All enums are **public** and **static by nature**, accessible from any code that references `Core.Application.Enum` namespace.

### Common Usage Patterns

#### Pattern 1: Lookup Filtering
```csharp
using Core.Application.Enum;

// Get all active credit card types
var cardTypes = _unitOfWork.Repository<Lookup>()
    .Fetch(x => x.LookupTypeId == (long)LookupTypes.ChargeCardType && x.IsActive)
    .OrderBy(x => x.RelativeOrder);
```

**Consumers**:
- `Api.Areas.Application.Impl.DropDownHelperService`
- `Core.Organizations.Impl.FranchiseeServicesFactory`
- `API.Areas.Application.Controller.DropdownController`

#### Pattern 2: Image Categorization
```csharp
// Assign before/after/during work photo types
var jobEstimateImage = new JobEstimateImage 
{ 
    ServiceId = serviceId, 
    FileId = fileId, 
    TypeId = (long)LookupTypes.BeforeWork,  // Or AfterWork, DuringWork
    IsNew = true 
};
```

**Consumers**:
- `Core.Scheduler.Impl.JobService`
- `Core.Reports.Impl.BeforeAfterImagesUploadwithS3Bucket`
- `Core.Reports.Impl.CalendarImagesMigration`

#### Pattern 3: Email Recipients
```csharp
// Separate TO and CC recipients
var toRecipients = domain.NotificationEmail.Recipients
    .Where(x => x.RecipientTypeId == (long)LookupTypes.TO)
    .Select(x => x.RecipientEmail);

var ccRecipients = domain.NotificationEmail.Recipients
    .Where(x => x.RecipientTypeId == (long)LookupTypes.CC)
    .Select(x => x.RecipientEmail);
```

**Consumers**:
- `Core.Reports.Impl.ReportFactory`
- `Core.Notification.Impl.EmailService`

#### Pattern 4: Invoice Status Filtering
```csharp
// Exclude invoice images from calculation
var jobEstimateServices = _jobEstimateServices.Table
    .Where(x => x.CategoryId.HasValue 
        && categoryList.Contains(x.CategoryId.Value) 
        && x.TypeId != (long)LookupTypes.InvoiceImages);
```

**Consumers**:
- `Core.Scheduler.Impl.JobService`
- `Core.Scheduler.Impl.EstimateInvoiceServices`
- `Core.Sales.Impl.SalesDataParsePollingAgent`

#### Pattern 5: Feedback Messages
```csharp
// Return success message to API client
return FeedbackMessageModel.CreateSuccessMessage("Invoice created successfully");

// Return error with details
return FeedbackMessageModel.CreateErrorMessage($"Payment failed: {errorDetails}");
```

**Consumers**:
- All API Controllers that return operation status
- Background jobs that log execution results

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Dependencies

#### Upstream (This Module Depends On)
- **None**: This is a leaf module with zero dependencies—pure enum definitions

#### Downstream (Modules That Depend On This)

##### Direct Consumers
1. **[Core.Application.Domain](../Domain/AI-CONTEXT.md)**
   - `Lookup` entity uses `LookupTypes` as foreign key category
   - `File` entity may reference `FileTypes` for categorization

2. **[Core.Application.ViewModel](../ViewModel/AI-CONTEXT.md)**
   - `FeedbackMessageModel` directly uses `MessageType` enum

3. **[API.Areas.Application](../../../../API/Areas/Application/AI-CONTEXT.md)**
   - `DropDownHelperService` heavily uses `LookupTypes` for all dropdown methods
   - Returns dropdowns for invoicing, scheduling, CRM modules

4. **[Core.Scheduler](../../Scheduler/AI-CONTEXT.md)**
   - `JobService` uses `BeforeWork`, `AfterWork`, `DuringWork` image types
   - `EstimateInvoiceServices` filters by `InvoiceImages` type
   - `JobReminderNotification` references `RepeatFrequency`

5. **[Core.Reports](../../Reports/AI-CONTEXT.md)**
   - `ReportFactory` uses `TO` and `CC` for email routing
   - `BeforeAfterImagesUploadwithS3Bucket` categorizes photos by work stage
   - `CustomerEmailReportService` constructs recipient lists

6. **[Core.Sales](../../Sales/AI-CONTEXT.md)**
   - `SalesDataParsePollingAgent` checks `SalesDataUploadStatus`
   - `RoyaltyReportFactory` filters by `RoyaltyFee` type
   - `SalesFunnelNationalService` aggregates by service categories

7. **[Core.Organizations](../../Organizations/AI-CONTEXT.md)**
   - `FranchiseeServicesFactory` references `ServiceTypeCategory`
   - `FranchiseeServiceFeeFactory` uses `ServiceFeeType` and `LateFee`

8. **[Core.Billing](../../Billing/AI-CONTEXT.md)** (Inferred)
   - Invoice processing uses `InvoiceStatus`, `Paid`, `PartialPayment`
   - Payment gateways reference `ChargeCardType`, `AccountType`

9. **[Core.MarketingLead](../../MarketingLead/AI-CONTEXT.md)**
   - `MarketingLeadsService` references lookup types for lead categorization

10. **[Core.ReviewApi](../../ReviewApi/AI-CONTEXT.md)**
    - `ReviewApiService` uses lookup types (specific usage unclear from grep)

##### Indirect Consumers (Via Repository Queries)
Any module that queries the `Lookup` table indirectly depends on `LookupTypes` enum for filtering.

### External Dependencies
- **None**: Standard C# enums require no external packages

### Database Schema Dependencies

#### Tables Referencing Enum Values
```sql
-- Lookup table (multi-tenant master data)
CREATE TABLE Lookup (
    Id BIGINT PRIMARY KEY,
    LookupTypeId BIGINT NOT NULL,  -- References LookupTypes enum values
    Name NVARCHAR(255),
    Alias NVARCHAR(50),
    RelativeOrder TINYINT,
    IsActive BIT DEFAULT 1
);

-- Files table (document management)
CREATE TABLE Files (
    Id BIGINT PRIMARY KEY,
    FileTypeId INT,  -- References FileTypes enum (19, 20, 21)
    FileName NVARCHAR(255),
    ContentType NVARCHAR(100),
    -- ... other fields
);

-- JobEstimateImage (field service photos)
CREATE TABLE JobEstimateImage (
    Id BIGINT PRIMARY KEY,
    ServiceId BIGINT,
    FileId BIGINT,
    TypeId BIGINT,  -- References LookupTypes (BeforeWork=203, AfterWork=204, etc.)
    -- ... other fields
);

-- NotificationEmailRecipient (email routing)
CREATE TABLE NotificationEmailRecipient (
    Id BIGINT PRIMARY KEY,
    NotificationEmailId BIGINT,
    RecipientTypeId BIGINT,  -- References LookupTypes (TO=127, CC=128)
    RecipientEmail NVARCHAR(255)
);
```

#### Critical Constraint
**⚠️ NEVER CHANGE ENUM VALUES**: The integer values are persisted in the database as foreign keys. Changing `ChargeCardType = 6` to `ChargeCardType = 99` would orphan all existing credit card records.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Design Insights -->
## 💡 Deep Architectural Insights

### Why This Design?

#### The Lookup Pattern Rationale
The `LookupTypes` enum + `Lookup` table pattern solves a common SaaS problem:
- **HQ Needs Flexibility**: Marketing wants to add "Discover Card" without deploying code
- **Developers Need Type Safety**: Code must compile-time validate that "charge card type" logic uses the right category
- **Database Needs Referential Integrity**: Foreign keys prevent orphaned records

**Trade-off**:
- ✅ Business users can modify dropdown values via admin UI
- ✅ Developers get IntelliSense and compile-time checks
- ❌ Enum IDs must never change (migration risk)
- ❌ Category names are hardcoded (adding new categories requires code + DB seed)

#### Sparse ID Numbering Strategy
Notice the gaps: `1, 2, 5, 6, 7, 8, 9, 10, 11, 14, ...`

**Likely Reasons**:
1. **Removed Types**: IDs 3, 4, 12, 13 may have been deprecated categories
2. **Range Reservation**: Different developers/teams may have been assigned ID ranges (1-50, 51-100, etc.)
3. **Legacy Migration**: IDs preserved from older system to maintain data continuity
4. **Manual Assignment**: No auto-increment; developers manually pick "next available" ID

#### Duplicate Enum Values Problem
```csharp
LateFee = 14,       // Singular
LateFees = 123,     // Plural—same semantic meaning?
```

**Root Cause Analysis**:
- Original system had `LateFee = 14` for lookup table
- Later feature needed late fee categorization, added `LateFees = 123`
- No refactoring to consolidate because changing IDs breaks production data

**Impact**:
- Developers must know which ID to use in which context
- Grep searches for "late fee" logic require checking both values
- Documentation (like this) is essential to prevent bugs

#### MessageType Auto-Increment Hazard
```csharp
Success = 1,  // Explicit
Warning,      // Implicit = 2
Error         // Implicit = 3
```

**Why This is Fragile**:
If a developer inserts a new value:
```csharp
Success = 1,
Info,         // Now = 2 (new!)
Warning,      // Now = 3 (was 2—breaks stored values!)
Error         // Now = 4 (was 3—breaks stored values!)
```

**Mitigation**:
- If `MessageType` values are ever stored in DB, **all must be explicit**
- Current usage (in `FeedbackMessageModel`) is transient—values only exist in API responses, not persisted

### Evolution Over Time

#### Phase 1: Early System (Inferred)
- Likely had `1-50` reserved for core lookups
- Simple categories: Phone, Address, Invoice Status

#### Phase 2: Financial Module (IDs 5-10)
- Payment gateway integration added `ChargeCardType`, `AccountType`, `InstrumentType`
- Invoicing expanded with `InvoiceStatus`, `InvoiceItemType`

#### Phase 3: Field Service Photos (IDs 203-207)
- Technician app required before/after photo tracking
- Image workflow types added in high-numbered range (200+)

#### Phase 4: System Flags (ID 254)
- `FRONTOFFICECALLMANAGEMENT` at highest value suggests:
  - Latest addition
  - Special system-level flag (not user-facing dropdown)
  - Possibly a feature flag or routing hint

### Performance Considerations

#### Why `(long)LookupTypes.X` Everywhere?
```csharp
x.LookupTypeId == (long)LookupTypes.ChargeCardType
```

**Explanation**:
- Enums are implicitly `int` (32-bit) in C#
- Database column `LookupTypeId` is `BIGINT` (64-bit)
- Cast required to match types for LINQ expression compilation

**Impact**:
- Slight overhead in LINQ query generation (negligible)
- More verbose code, but type-safe

#### Index Strategy
```sql
-- Critical for dropdown query performance
CREATE INDEX IX_Lookup_LookupTypeId_IsActive 
ON Lookup(LookupTypeId, IsActive)
INCLUDE (Name, RelativeOrder);
```

Without this composite index, dropdown queries would scan entire `Lookup` table (potentially 10,000+ rows across all franchisees).

### Security Implications

#### No Sensitive Data in Enums
- Enums are compiled into assemblies (readable via decompilers)
- Never put API keys, secrets, or PII in enum definitions
- Current enums only contain business logic constants ✅

#### Enum Injection Attack Vector
**Potential Vulnerability**:
```csharp
// ❌ BAD: Accepting enum values from untrusted input
var status = (InvoiceStatus)Request.QueryString["status"];
```

**Why Dangerous**: Attacker could pass invalid integer (e.g., `999`) causing undefined behavior.

**Mitigation**:
```csharp
// ✅ GOOD: Validate before casting
if (Enum.IsDefined(typeof(InvoiceStatus), statusValue))
{
    var status = (InvoiceStatus)statusValue;
}
```

**Current System**: Uses `LookupTypes` for filtering DB queries, not user input validation ✅

### Testing Strategy

#### Unit Test Considerations
```csharp
[Test]
public void LookupTypes_ShouldHaveUniqueValues()
{
    var values = Enum.GetValues(typeof(LookupTypes)).Cast<int>().ToList();
    var distinctValues = values.Distinct().ToList();
    
    Assert.AreEqual(values.Count, distinctValues.Count, 
        "LookupTypes enum has duplicate values!");
}
```

#### Integration Test Example
```csharp
[Test]
public void DropDownHelper_GetCardType_ReturnsOnlyActiveCards()
{
    // Arrange
    SeedLookups(LookupTypes.ChargeCardType, 
        new[] { ("Visa", true), ("Mastercard", true), ("Deactivated", false) });
    
    // Act
    var result = _service.GetCardType();
    
    // Assert
    Assert.AreEqual(2, result.Count(), "Should exclude inactive cards");
}
```

### Migration Guidance

#### Adding a New Lookup Type
1. **Choose ID**: Pick next available ID in appropriate range (or 500+ for new features)
2. **Add Enum Value**: `NewCategory = 501,  // Description of purpose`
3. **Seed Database**: Add migration to populate `LookupType` table
4. **Create Lookups**: Seed default `Lookup` records for the category
5. **Add Service Method**: Create getter in `DropDownHelperService`
6. **Update This Doc**: Document the new type in this AI-CONTEXT.md

#### Deprecating an Enum Value
**❌ NEVER DELETE**: Existing data references the ID.

**✅ Instead**:
1. Add `[Obsolete("Use XYZ instead")]` attribute
2. Update consuming code to new value
3. Run data migration to update DB references
4. Leave enum value in place (mark as legacy in comments)

### Known Issues & Tech Debt

#### Issue 1: Duplicate Late Fee Values
- **Problem**: `LateFee = 14` vs `LateFees = 123`
- **Impact**: Developer confusion, inconsistent usage
- **Fix**: Requires data migration + code audit (high risk, low priority)

#### Issue 2: Unclear Category Boundaries
- **Problem**: Some values are lookup categories (e.g., `ChargeCardType`), others are actual values (e.g., `Paid = 81`)
- **Impact**: Developers must memorize which pattern applies
- **Fix**: Refactor into `LookupCategory` vs `StatusCode` enums (breaking change)

#### Issue 3: No Enum Documentation in Code
- **Problem**: Zero XML comments on enum values
- **Impact**: Developers must grep codebase to understand usage
- **Fix**: Add `/// <summary>` tags (low effort, high value)

### Future Considerations

#### When to Add vs. Extend
**Add New Enum** if:
- Values are static and will never change (e.g., `MessageType`)
- Type safety is critical (compile-time enforcement)
- Small set of values (< 20)

**Extend Lookup Table** if:
- Business users need to modify values
- Different franchisees need custom options (multi-tenancy)
- Large or evolving set (e.g., service types)

#### Enum Flags Pattern (Not Used)
Current enums are **discrete values**. If future requirements need combinations:
```csharp
[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    Admin = Read | Write | Delete  // = 7
}
```

**Use Case**: User roles, feature flags, bitwise operations.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Historical Context -->
## 📜 Historical Notes

### Version History (Inferred)
- **v1.0 (2015?)**: Initial enum definitions with core lookups (1-24)
- **v2.0 (2016?)**: Financial module expansion (5-10), service categories (91-102)
- **v3.0 (2017?)**: Image workflow types added (203-207) for mobile app
- **v3.5 (2018?)**: Late fee refactoring introduces `LateFees = 123` duplicate
- **v4.0 (2019?)**: Front office routing flag `FRONTOFFICECALLMANAGEMENT = 254`

### Original Design Decisions
- **Why not a database table for LookupTypes?**
  - Compile-time type safety was deemed more valuable than runtime flexibility
  - Seeding `LookupType` table would still require migrations for each new category
  - Enum approach reduces one layer of indirection

- **Why explicit integer assignments?**
  - System migrated from legacy platform with hardcoded IDs in stored procedures
  - Preserving IDs ensured backward compatibility during transition

<!-- END CUSTOM SECTION -->

---

**🔗 Related Documentation**:
- [Application.Domain Module](../Domain/AI-CONTEXT.md) - `Lookup` and `LookupType` entities
- [Application.ViewModel Module](../ViewModel/AI-CONTEXT.md) - `FeedbackMessageModel` usage
- [DropDownHelperService](../../../../API/Areas/Application/Impl/AI-CONTEXT.md) - Primary consumer

**📝 Maintenance Notes**:
- This document should be updated when new enum values are added
- Coordinate enum changes with DBA for database migration planning
- Code reviewers: Flag any enum value modifications as high-risk changes
