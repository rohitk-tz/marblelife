<!-- AUTO-GENERATED: Header -->
# Enum — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Provides strongly-typed enumerations that define the valid values for message types, file categories, and lookup table categories throughout the application. These enums ensure type safety, prevent magic numbers, and serve as the contract between application code and database lookup tables.

### Design Patterns
- **Type-Safe Constants**: Enums replace string literals and magic numbers with compile-time checked values
- **Database-Code Synchronization**: `LookupTypes` enum values match primary keys in the `LookupType` database table
- **Semantic Naming**: Enum values use business domain terminology (e.g., `Restoration`, `Maintenance`, `RoyaltyFee`)

### Data Flow
1. Code references enum values (e.g., `LookupTypes.InvoiceStatus`)
2. Enum value is cast to underlying type (int/long) when querying database: `(long)LookupTypes.InvoiceStatus`
3. Database queries filter on numeric ID matching enum value
4. UI displays human-readable names while storing enum values in DTOs
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### MessageType.cs
```csharp
// Defines the severity level for user feedback messages
public enum MessageType
{
    Success = 1,  // Operation completed successfully (green notification)
    Warning = 2,  // Operation completed but requires attention (yellow notification)
    Error = 3     // Operation failed (red notification)
}
```
**Usage**: Controls color and icon in notification toasts/alerts

### FileTypes.cs
```csharp
// Categorizes uploaded files by their nature
public enum FileTypes
{
    Document = 19,  // PDFs, Word docs, spreadsheets
    Image = 20,     // Photos, screenshots, diagrams
    Other = 21      // Any file not falling into above categories
}
```
**Note**: Values 19, 20, 21 match `Lookup.Id` in the database `Lookup` table where `LookupTypeId` corresponds to a file type category.

### LookupTypes.cs
```csharp
// Maps to LookupType.Id in database - defines all lookup categories in the system
public enum LookupTypes
{
    // Contact Information
    Phone = 1,
    Address = 2,
    
    // Financial Instruments
    InstrumentType = 5,         // Payment instrument (check, cash, wire)
    ChargeCardType = 6,         // Credit/debit card types (Visa, Mastercard)
    AccountType = 7,            // Bank account types (checking, savings)
    RoutingNumberCategory = 23, // Bank routing classifications
    
    // Invoice & Billing
    InvoiceStatus = 9,          // Draft, Sent, Paid, Overdue
    InvoiceItemType = 10,       // Service, Product, Discount
    ServiceFeeType = 21,        // Fee categories
    LateFee = 14,               // Late fee configurations
    LateFees = 123,             // Alternate late fee category (possibly historical)
    InterestIncome = 24,        // Interest charge types
    Paid = 81,                  // Fully paid status
    PartialPayment = 83,        // Partially paid status
    
    // Service Management
    ServiceTypeCategory = 11,   // Restoration, Maintenance, etc.
    Restoration = 101,          // Service type: restoration work
    Maintenance = 102,          // Service type: maintenance work
    RepeatFrequency = 22,       // One-time, weekly, monthly, yearly
    
    // Sales & Marketing
    SalesDataUploadStatus = 8,  // Status of bulk data imports
    CallType = 17,              // Inbound, outbound, follow-up
    
    // System & Audit
    AuditActionType = 19,       // Created, Updated, Deleted, Viewed
    
    // Credits & Adjustments
    AccountCreditType = 20,     // Refund, discount, promotional credit
    
    // Document Management
    DocumentCategory = 24,      // Contracts, invoices, receipts
    
    // Communication
    TO = 127,                   // Email TO recipients
    CC = 128,                   // Email CC recipients
    
    // Image Categories (for photo management)
    BeforeWork = 203,           // Before-work photos
    AfterWork = 204,            // After-work photos
    DuringWork = 205,           // In-progress photos
    ExteriorBuilding = 206,     // Building exterior photos
    InvoiceImages = 207,        // Invoice attachments
    
    // Service Types (specific numeric IDs)
    Service = 91,               // General service category
    RoyaltyFee = 92,            // Franchise royalty charges
    
    // Special Systems
    FRONTOFFICECALLMANAGEMENT = 254  // Front office call handling system
}
```
**Critical**: These numeric values MUST match `LookupType.Id` in the database. Any mismatch causes lookup failures.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### MessageType
- **Purpose**: Controls UI notification appearance
- **Values**:
  - `Success`: Green toast, checkmark icon
  - `Warning`: Yellow/orange alert, warning icon
  - `Error`: Red toast, X icon
- **Usage**: `FeedbackMessageModel.CreateErrorMessage(message)` sets `MessageType.Error`

### FileTypes
- **Purpose**: Categorizes uploaded files for filtering and icon display
- **Values**:
  - `Document`: Office files, PDFs, text files
  - `Image`: JPEG, PNG, GIF, BMP
  - `Other`: Everything else (archives, videos, unknown types)
- **Usage**: `FileService` sets this based on MIME type analysis
- **Database Mapping**: Values match `Lookup.Id` for file type lookups

### LookupTypes
- **Purpose**: Type-safe access to lookup table categories
- **Usage Pattern**:
  ```csharp
  var lookupTypeId = (long)LookupTypes.InvoiceStatus;
  var statuses = db.Lookups.Where(l => l.LookupTypeId == lookupTypeId);
  ```
- **Critical Groups**:
  1. **Financial** (6, 7, 14, 20, 21, 23, 92, 123): Payment and fee types
  2. **Invoice** (9, 10, 81, 83): Invoice statuses and line item types
  3. **Service** (11, 22, 91, 101, 102): Service categories and frequencies
  4. **Images** (203-207): Photo categorization for before/after/during work
  5. **Communication** (17, 127, 128): Call and email types
  6. **Audit** (19): System action logging
- **Side Effects**: Querying with wrong enum value returns empty results (fails silently)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
None - pure enum definitions

### External Dependencies
None - standard C# language feature

### Referenced By
- **Core.Application.Domain.Lookup** — `Lookup.LookupTypeId` references these enum values
- **Core.Application.Domain.LookupType** — `LookupType.Id` must match these values
- **Core.Application.ViewModel.FeedbackMessageModel** — Uses `MessageType` enum
- **Core.Application.ViewModel.ResponseModel** — Displays messages with `MessageType`
- **Repositories** — Query filters cast enums to long: `(long)LookupTypes.InvoiceStatus`
- **Services** — Business logic uses enums for type-safe lookup queries
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### LookupTypes Enum Strategy
**Why numeric IDs instead of starting from 0?**
These values mirror primary keys from a legacy database schema. The numeric gaps (e.g., 1, 2, 5, 6) indicate:
- Historical deletions (IDs 3, 4 removed from enum but preserved in DB)
- Logical grouping (200s for images, 100s for service types)
- Franchise-specific values (FRONTOFFICECALLMANAGEMENT = 254)

**When to add new values:**
1. First, insert into database `LookupType` table and note the auto-generated `Id`
2. Then, add to `LookupTypes` enum with that exact ID
3. Never reuse deleted enum values - database foreign keys may still reference old IDs

### MessageType Usage Pattern
Always create messages via factory methods to ensure consistency:
```csharp
// ✅ GOOD - Type-safe
var message = FeedbackMessageModel.CreateSuccessMessage("Saved!");

// ❌ BAD - Direct instantiation breaks factory pattern
var message = new FeedbackMessageModel { MessageType = MessageType.Success, Message = "Saved!" };
```

### FileTypes Extension
If adding new file categories:
1. Add enum value with next available ID (22, 23, etc.)
2. Seed `Lookup` table with new entry: `INSERT INTO Lookup (Id, LookupTypeId, Name) VALUES (22, 1, 'Video')`
3. Update `FileService` logic to detect MIME types and assign new category

### Common Pitfalls
- **Casting errors**: Forgetting to cast enum to long: `l.LookupTypeId == LookupTypes.Phone` (compiler error) vs. `l.LookupTypeId == (long)LookupTypes.Phone` (correct)
- **Database sync**: Adding enum without database seed causes lookups to fail silently
- **Magic numbers**: Hardcoding `9` instead of `(long)LookupTypes.InvoiceStatus` makes code unmaintainable
<!-- END CUSTOM SECTION -->
