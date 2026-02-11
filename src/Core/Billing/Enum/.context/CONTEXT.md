<!-- AUTO-GENERATED: Header -->
# Enum — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2026-02-10T12:21:23Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
This module defines **strongly-typed enumerations** for all billing status codes, payment instrument types, transaction responses, and categorization values. These enums replace magic numbers throughout the codebase and provide type-safe constants that map to database lookup table IDs. The numeric values match the `Lookup.Id` primary keys in the database.

### Design Patterns
- **Database-Backed Enums**: Integer values correspond to database Lookup table IDs (e.g., `InvoiceStatus.Paid = 81` → `SELECT * FROM Lookup WHERE Id = 81`)
- **Explicit Value Assignment**: All enum values explicitly defined to prevent accidental renumbering
- **Gateway Integration**: Authorize.Net response codes (CVV, transaction status) mapped to local enums for parsing

### Data Flow
1. Domain entities store `long` type IDs (e.g., `Invoice.StatusId = 81`)
2. Business logic casts to enum for readability: `(InvoiceStatus)invoice.StatusId == InvoiceStatus.Paid`
3. Lookup queries use enum values: `WHERE StatusId = (long)InvoiceStatus.Unpaid`
4. Payment gateway responses converted to enums: `TransactionResponseType.Approved`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Invoice & Payment Status

#### InvoiceStatus
```csharp
/// <summary>
/// Invoice payment status - maps to Lookup table IDs
/// </summary>
public enum InvoiceStatus
{
    Paid = 81,          // Invoice fully paid
    Unpaid = 82,        // Invoice not paid (may have no payments)
    PartialPaid = 83,   // Invoice partially paid (balance remaining)
    Canceled = 84,      // Invoice voided/canceled
    ZeroDue = 230       // Invoice issued with $0 balance
}
```

#### PaymentStatus
```csharp
/// <summary>
/// Payment transaction status - lifecycle tracking
/// </summary>
public enum PaymentStatus
{
    Submitted = 141,   // Payment submitted to gateway
    Approved = 142,    // Payment approved by gateway
    Processing = 143,  // Payment being processed (batch settlement)
    Rejected = 144     // Payment declined/failed
}
```

### Payment Instruments

#### InstrumentType
```csharp
/// <summary>
/// Payment method type - determines which payment entity to use
/// </summary>
public enum InstrumentType
{
    ChargeCard = 41,         // New credit/debit card payment
    ECheck = 42,             // New electronic check (ACH) payment
    Cash = 43,               // Cash payment (manual entry)
    Check = 44,              // Paper check payment
    ChargeCardOnFile = 45,   // Use saved credit card profile
    ECheckOnFile = 46,       // Use saved eCheck profile
    AccountCredit = 47       // Apply account credit balance
}
```

#### ChargeCardType
```csharp
/// <summary>
/// Credit card brand/network
/// </summary>
public enum ChargeCardType
{
    Visa = 51,
    MasterCard = 52,
    Discover = 53,
    AmericanExpress = 54
    // Note: Missing Diners Club, JCB, other networks
}
```

#### AccountType
```csharp
/// <summary>
/// Bank account type for ACH/eCheck payments
/// </summary>
public enum AccountType
{
    Checking = 61,
    Savings = 62,
    BusinessChecking = 63  // Typically has different processing fees
}
```

#### AuthorizeNetAccountType
```csharp
/// <summary>
/// Authorize.Net-specific account type codes
/// Maps to eCheck account type in gateway requests
/// </summary>
public enum AuthorizeNetAccountType
{
    Checking = 1,
    Savings = 2,
    BusinessChecking = 3
}
```

### Invoice Line Item Types

#### InvoiceItemType
```csharp
/// <summary>
/// Type of charge on an invoice line item
/// Determines which specialized InvoiceItem entity to use
/// </summary>
public enum InvoiceItemType
{
    Service = 91,                                // Service/product sale
    RoyaltyFee = 92,                            // Franchise royalty percentage
    AdFund = 93,                                // Advertising fund contribution
    Discount = 94,                              // Discount/credit applied
    LateFees = 123,                             // Late payment penalty
    InterestRatePerAnnum = 124,                 // Annual interest charge
    ServiceFee = 95,                            // Processing/service fee
    NationalCharge = 96,                        // National franchise fee
    LoanServiceFee = 208,                       // Loan servicing fee
    LoanServiceFeeInterestRatePerAnnum = 209,   // Loan interest
    FranchiseeTechMail = 215                    // Technology/email fee
}
```

### Payment Gateway Response Codes

#### TransactionResponseType
```csharp
/// <summary>
/// Authorize.Net transaction response codes
/// </summary>
public enum TransactionResponseType
{
    Approved = 1,        // Transaction approved
    Declined = 2,        // Transaction declined by issuer
    Error = 3,           // Gateway/network error
    HeldForReview = 4    // Fraud review hold
}
```

#### CvvResponseCode
```csharp
/// <summary>
/// Card Verification Value (CVV) response codes from Authorize.Net
/// </summary>
public enum CvvResponseCode
{
    SuccessfullyMatch = 'M',                              // CVV matches
    DoesNotMatch = 'N',                                   // CVV does not match
    NotProcessed = 'P',                                   // CVV not processed
    ShouldBeOnCardButNotIndicated = 'S',                 // Issuer requires CVV but not provided
    IssuerNotCertifiedOrNotProvidedEncryptionKey = 'U'   // Issuer not certified for CVV
}
```

#### ProcessorResponseResult
```csharp
/// <summary>
/// Detailed processor response codes
/// Maps to AVS (Address Verification System) and processor-specific codes
/// </summary>
public enum ProcessorResponseResult
{
    Approved = 1,
    Declined = 2,
    Error = 3,
    FraudReview = 4
    // Note: Actual implementation may have more detailed codes
}
```

### Other Enumerations

#### LateFeeType
```csharp
/// <summary>
/// Late fee calculation method
/// </summary>
public enum LateFeeType
{
    Percentage = 1,      // Percentage of outstanding balance
    FlatFee = 2,         // Fixed dollar amount
    Tiered = 3           // Tiered based on days overdue
}
```

#### AccountCreditType
```csharp
/// <summary>
/// Type of account credit (refunds, adjustments, etc.)
/// </summary>
public enum AccountCreditType
{
    Refund = 1,          // Customer refund
    Adjustment = 2,      // Manual adjustment/correction
    Overpayment = 3,     // Applied from overpayment
    Promotion = 4        // Promotional credit
}
```

#### ServiceTypes
```csharp
/// <summary>
/// Service category enumeration
/// Note: May duplicate ServiceType entity from Organizations.Domain
/// </summary>
public enum ServiceTypes
{
    GroutRestoration = 1,
    StoneRestoration = 2,
    ConcreteRestoration = 3,
    VinylRestoration = 4
    // Note: Should reference ServiceTypeAlias constants instead
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Usage Patterns

#### Casting Between Database IDs and Enums
```csharp
// Store enum as long in database
invoice.StatusId = (long)InvoiceStatus.Unpaid;

// Read database ID as enum
var status = (InvoiceStatus)invoice.StatusId;

// Conditional logic
if (status == InvoiceStatus.Paid)
{
    SendReceiptEmail(invoice);
}
```

#### Gateway Response Parsing
```csharp
// Parse Authorize.Net response
var responseCode = (TransactionResponseType)gatewayResponse.ResponseCode;

if (responseCode == TransactionResponseType.Approved)
{
    payment.StatusId = (long)PaymentStatus.Approved;
}
else if (responseCode == TransactionResponseType.Declined)
{
    payment.StatusId = (long)PaymentStatus.Rejected;
    LogDeclineReason(gatewayResponse);
}

// CVV validation
var cvvCode = (CvvResponseCode)gatewayResponse.CvvResultCode;
if (cvvCode != CvvResponseCode.SuccessfullyMatch)
{
    FlagForFraudReview(payment);
}
```

#### Payment Instrument Type Switching
```csharp
var instrumentType = (InstrumentType)payment.InstrumentTypeId;

switch (instrumentType)
{
    case InstrumentType.ChargeCard:
    case InstrumentType.ChargeCardOnFile:
        return await ProcessCreditCard(payment);
    
    case InstrumentType.ECheck:
    case InstrumentType.ECheckOnFile:
        return await ProcessECheck(payment);
    
    case InstrumentType.Check:
        return await ProcessPaperCheck(payment);
    
    case InstrumentType.AccountCredit:
        return await ApplyAccountCredit(payment);
    
    default:
        throw new InvalidOperationException($"Unknown instrument type: {instrumentType}");
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

**Internal**:
- [Domain entities](../Domain/.context/CONTEXT.md) — StatusId, InstrumentTypeId, ItemTypeId fields reference these enums
- Database `Lookup` table — Enum values must match Lookup.Id primary keys

**External**:
- `System.ComponentModel` — For `[Description]` attribute (used in some enums)
- Authorize.Net Gateway API — Response codes map to `TransactionResponseType`, `CvvResponseCode`

**Database Schema**:
```sql
-- Enum values MUST match Lookup table IDs
SELECT Id, Category, Name FROM Lookup WHERE Category = 'InvoiceStatus';
-- 81 | InvoiceStatus | Paid
-- 82 | InvoiceStatus | Unpaid
-- 83 | InvoiceStatus | PartialPaid
-- 84 | InvoiceStatus | Canceled
-- 230 | InvoiceStatus | ZeroDue
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Database-Backed Enum Pattern

These enums are **tightly coupled** to the database `Lookup` table. The pattern is:

1. Database has `Lookup` table with `Id`, `Category`, `Name` columns
2. Enum integer values = Lookup.Id values
3. Domain entities store `long StatusId` (not enum type)
4. Business logic casts: `(InvoiceStatus)entity.StatusId`

**Critical**: If Lookup table IDs change, enums must be updated or the application will break.

### Known Issues

#### 1. Magic Numbers Still Present
Despite having enums, the codebase may still use raw integers:
```csharp
// ❌ BAD: Magic number
if (invoice.StatusId == 81) { ... }

// ✅ GOOD: Use enum
if (invoice.StatusId == (long)InvoiceStatus.Paid) { ... }
```

#### 2. Inconsistent Formatting
```csharp
// InvoiceStatus - single line, no spacing
public enum InvoiceStatus
{
    Paid = 81, Unpaid = 82, PartialPaid = 83, Canceled = 84,ZeroDue = 230
}

// ChargeCardType - multi-line, proper spacing
public enum ChargeCardType
{
    Visa = 51,
    MasterCard = 52,
    Discover = 53,
    AmericanExpress = 54
}
```

#### 3. Duplicate Service Type Definitions
`ServiceTypes` enum may duplicate `ServiceType` entity from `Core.Organizations.Domain`. Prefer using the entity for database-backed service types.

#### 4. Missing Enum Values
- **ChargeCardType**: Missing Diners Club, JCB, UnionPay
- **InstrumentType**: Missing wire transfer, money order, cryptocurrency
- **TransactionResponseType**: Simplified — actual gateway responses are more detailed

#### 5. CVV Enum Uses char Type
```csharp
public enum CvvResponseCode
{
    SuccessfullyMatch = 'M',  // Character literal, not int
    DoesNotMatch = 'N',
    // ...
}
```
Unusual choice — typically enums use `int` base type. To use:
```csharp
var cvvCode = (CvvResponseCode)gatewayResponse.CvvResultCode;  // CvvResultCode must be char
```

### Best Practices

#### Always Use Enums for Status Checks
```csharp
// ❌ AVOID: String comparison (fragile)
if (invoice.Lookup.Name == "Paid") { ... }

// ❌ AVOID: Magic numbers (unreadable)
if (invoice.StatusId == 81) { ... }

// ✅ GOOD: Type-safe enum
if ((InvoiceStatus)invoice.StatusId == InvoiceStatus.Paid) { ... }
```

#### Extension Methods for Enum Display
```csharp
public static class InvoiceStatusExtensions
{
    public static string ToDisplayString(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Paid => "Paid",
            InvoiceStatus.Unpaid => "Unpaid",
            InvoiceStatus.PartialPaid => "Partially Paid",
            InvoiceStatus.Canceled => "Canceled",
            InvoiceStatus.ZeroDue => "Zero Balance",
            _ => "Unknown"
        };
    }
}

// Usage:
var displayText = ((InvoiceStatus)invoice.StatusId).ToDisplayString();
```

#### Database Seed Script
```csharp
// Ensure Lookup table matches enum definitions
public class LookupSeeder
{
    public void SeedInvoiceStatuses(DbContext context)
    {
        var statuses = new[]
        {
            new Lookup { Id = 81, Category = "InvoiceStatus", Name = "Paid" },
            new Lookup { Id = 82, Category = "InvoiceStatus", Name = "Unpaid" },
            new Lookup { Id = 83, Category = "InvoiceStatus", Name = "PartialPaid" },
            new Lookup { Id = 84, Category = "InvoiceStatus", Name = "Canceled" },
            new Lookup { Id = 230, Category = "InvoiceStatus", Name = "ZeroDue" }
        };
        
        context.Lookups.AddRange(statuses);
        context.SaveChanges();
    }
}
```

#### Unit Test Enum-Database Sync
```csharp
[Test]
public void EnumValuesShouldMatchDatabase()
{
    // Verify InvoiceStatus enum matches Lookup table
    var dbStatuses = context.Lookups
        .Where(l => l.Category == "InvoiceStatus")
        .ToDictionary(l => l.Id, l => l.Name);
    
    foreach (InvoiceStatus status in Enum.GetValues(typeof(InvoiceStatus)))
    {
        var enumValue = (long)status;
        Assert.That(dbStatuses.ContainsKey(enumValue), 
            $"Lookup table missing ID {enumValue} for {status}");
    }
}
```
<!-- END CUSTOM SECTION -->
