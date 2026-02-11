<!-- AUTO-GENERATED: Header -->
# Billing Enumerations
> Strongly-typed status codes and payment instrument types mapped to database lookup values
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Enum** module provides type-safe enumerations for all billing-related status codes, payment types, and gateway response codes. These enums eliminate magic numbers and provide IntelliSense support while maintaining synchronization with the database `Lookup` table.

**Why it exists**: Instead of scattering magic numbers like `81`, `82` throughout the code, enums provide readable names: `InvoiceStatus.Paid`, `InvoiceStatus.Unpaid`. The integer values match database primary keys, allowing seamless integration with Entity Framework entities.

**Real-world analogy**: Think of these like HTTP status codes (200 = OK, 404 = Not Found). Instead of remembering that `81 = Paid`, you write `InvoiceStatus.Paid`. The compiler prevents typos, and the code self-documents.

### Key Concepts

1. **Database-Backed**: Enum values = Lookup table IDs (must stay in sync)
2. **Type Safety**: Compiler prevents invalid status codes
3. **Gateway Integration**: Maps Authorize.Net response codes to local types
4. **Explicit Values**: All enum values explicitly assigned (prevents renumbering bugs)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
Add using statement in files that need billing enums:
```csharp
using Core.Billing.Enum;
```

### Example: Invoice Status Checking
```csharp
using Core.Billing.Domain;
using Core.Billing.Enum;

public async Task<List<Invoice>> GetOverdueInvoices()
{
    var unpaidStatusId = (long)InvoiceStatus.Unpaid;
    var partialPaidStatusId = (long)InvoiceStatus.PartialPaid;
    
    return await context.Invoices
        .Where(i => (i.StatusId == unpaidStatusId || i.StatusId == partialPaidStatusId)
                    && i.DueDate < DateTime.Now)
        .ToListAsync();
}

public void MarkAsPaid(Invoice invoice)
{
    invoice.StatusId = (long)InvoiceStatus.Paid;
    context.SaveChanges();
}
```

### Example: Payment Processing with Gateway Response
```csharp
using Core.Billing.Enum;

public async Task<PaymentResult> ProcessCreditCard(Payment payment, ChargeCard card)
{
    // Submit to Authorize.Net
    var gatewayResponse = await authorizeNetClient.ChargeCard(card, payment.Amount);
    
    // Parse response code
    var responseType = (TransactionResponseType)gatewayResponse.ResponseCode;
    var cvvCode = (CvvResponseCode)gatewayResponse.CvvResultCode;
    
    // Update payment status based on gateway response
    switch (responseType)
    {
        case TransactionResponseType.Approved:
            payment.StatusId = (long)PaymentStatus.Approved;
            
            // Check CVV for fraud risk
            if (cvvCode != CvvResponseCode.SuccessfullyMatch)
            {
                await FlagForFraudReview(payment, $"CVV mismatch: {cvvCode}");
            }
            
            return PaymentResult.Success(gatewayResponse.TransactionId);
        
        case TransactionResponseType.Declined:
            payment.StatusId = (long)PaymentStatus.Rejected;
            return PaymentResult.Failure("Card declined by issuer");
        
        case TransactionResponseType.Error:
            payment.StatusId = (long)PaymentStatus.Rejected;
            return PaymentResult.Failure($"Gateway error: {gatewayResponse.ErrorMessage}");
        
        case TransactionResponseType.HeldForReview:
            payment.StatusId = (long)PaymentStatus.Processing;
            await NotifyFraudTeam(payment);
            return PaymentResult.Pending("Payment held for fraud review");
        
        default:
            throw new InvalidOperationException($"Unknown response: {responseType}");
    }
}
```

### Example: Payment Instrument Type Switching
```csharp
public async Task<IPaymentProcessor> GetPaymentProcessor(Payment payment)
{
    var instrumentType = (InstrumentType)payment.InstrumentTypeId;
    
    return instrumentType switch
    {
        InstrumentType.ChargeCard => new CreditCardProcessor(authorizeNetClient),
        InstrumentType.ChargeCardOnFile => new StoredCardProcessor(authorizeNetClient),
        InstrumentType.ECheck => new ECheckProcessor(authorizeNetClient),
        InstrumentType.ECheckOnFile => new StoredECheckProcessor(authorizeNetClient),
        InstrumentType.Check => new ManualCheckProcessor(),
        InstrumentType.Cash => new CashProcessor(),
        InstrumentType.AccountCredit => new AccountCreditProcessor(),
        _ => throw new NotSupportedException($"Unsupported payment type: {instrumentType}")
    };
}
```

### Example: Invoice Item Type Classification
```csharp
public decimal CalculateTaxableAmount(Invoice invoice)
{
    decimal taxableAmount = 0;
    
    foreach (var item in invoice.InvoiceItems)
    {
        var itemType = (InvoiceItemType)item.ItemTypeId;
        
        // Only services and service fees are taxable
        if (itemType == InvoiceItemType.Service || itemType == InvoiceItemType.ServiceFee)
        {
            taxableAmount += item.Amount;
        }
        
        // Royalties, ad fund, late fees are not taxable
    }
    
    return taxableAmount;
}
```

### Example: Card Type Validation
```csharp
public bool IsAmexCard(ChargeCard card)
{
    var cardType = (ChargeCardType)card.TypeId;
    return cardType == ChargeCardType.AmericanExpress;
}

public decimal GetProcessingFee(ChargeCard card, decimal amount)
{
    var cardType = (ChargeCardType)card.TypeId;
    
    // Amex has higher processing fees
    return cardType switch
    {
        ChargeCardType.AmericanExpress => amount * 0.035m,  // 3.5%
        ChargeCardType.Visa => amount * 0.029m,              // 2.9%
        ChargeCardType.MasterCard => amount * 0.029m,
        ChargeCardType.Discover => amount * 0.029m,
        _ => amount * 0.029m
    };
}
```

### Example: Enum Display String Extension
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
    
    public static string ToCssClass(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Paid => "badge-success",
            InvoiceStatus.Unpaid => "badge-danger",
            InvoiceStatus.PartialPaid => "badge-warning",
            InvoiceStatus.Canceled => "badge-secondary",
            InvoiceStatus.ZeroDue => "badge-info",
            _ => "badge-light"
        };
    }
}

// Usage in view:
var status = (InvoiceStatus)invoice.StatusId;
<span class="@status.ToCssClass()">@status.ToDisplayString()</span>
```

### Example: Database Lookup Synchronization
```csharp
// Unit test to ensure enums match database
[Test]
public void InvoiceStatus_ShouldMatchLookupTable()
{
    var dbLookups = context.Lookups
        .Where(l => l.Category == "InvoiceStatus")
        .ToDictionary(l => l.Id, l => l.Name);
    
    foreach (InvoiceStatus status in Enum.GetValues(typeof(InvoiceStatus)))
    {
        var enumValue = (long)status;
        var enumName = status.ToString();
        
        Assert.IsTrue(dbLookups.ContainsKey(enumValue), 
            $"Lookup table missing entry for {enumName} (ID: {enumValue})");
        
        // Optionally verify name matches
        Assert.AreEqual(enumName, dbLookups[enumValue].Replace(" ", ""),
            $"Lookup name mismatch for {enumName}");
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Invoice & Payment Status

| Enum | Values | Description |
|------|--------|-------------|
| `InvoiceStatus` | `Paid(81)`, `Unpaid(82)`, `PartialPaid(83)`, `Canceled(84)`, `ZeroDue(230)` | Invoice payment status |
| `PaymentStatus` | `Submitted(141)`, `Approved(142)`, `Processing(143)`, `Rejected(144)` | Payment transaction lifecycle |

### Payment Instruments

| Enum | Values | Description |
|------|--------|-------------|
| `InstrumentType` | `ChargeCard(41)`, `ECheck(42)`, `Cash(43)`, `Check(44)`, `ChargeCardOnFile(45)`, `ECheckOnFile(46)`, `AccountCredit(47)` | Payment method type |
| `ChargeCardType` | `Visa(51)`, `MasterCard(52)`, `Discover(53)`, `AmericanExpress(54)` | Credit card brand |
| `AccountType` | `Checking(61)`, `Savings(62)`, `BusinessChecking(63)` | Bank account type for ACH |
| `AuthorizeNetAccountType` | `Checking(1)`, `Savings(2)`, `BusinessChecking(3)` | Gateway-specific account codes |

### Invoice Line Items

| Enum | Values | Description |
|------|--------|-------------|
| `InvoiceItemType` | `Service(91)`, `RoyaltyFee(92)`, `AdFund(93)`, `Discount(94)`, `LateFees(123)`, `InterestRatePerAnnum(124)`, `ServiceFee(95)`, `NationalCharge(96)`, `LoanServiceFee(208)`, `LoanServiceFeeInterestRatePerAnnum(209)`, `FranchiseeTechMail(215)` | Type of invoice line item |
| `LateFeeType` | `Percentage(1)`, `FlatFee(2)`, `Tiered(3)` | Late fee calculation method |

### Gateway Response Codes

| Enum | Values | Description |
|------|--------|-------------|
| `TransactionResponseType` | `Approved(1)`, `Declined(2)`, `Error(3)`, `HeldForReview(4)` | Authorize.Net transaction result |
| `CvvResponseCode` | `SuccessfullyMatch('M')`, `DoesNotMatch('N')`, `NotProcessed('P')`, `ShouldBeOnCardButNotIndicated('S')`, `IssuerNotCertifiedOrNotProvidedEncryptionKey('U')` | CVV validation result |
| `ProcessorResponseResult` | `Approved(1)`, `Declined(2)`, `Error(3)`, `FraudReview(4)` | Detailed processor response |

### Other

| Enum | Values | Description |
|------|--------|-------------|
| `AccountCreditType` | `Refund(1)`, `Adjustment(2)`, `Overpayment(3)`, `Promotion(4)` | Type of account credit |
| `ServiceTypes` | `GroutRestoration(1)`, `StoneRestoration(2)`, `ConcreteRestoration(3)`, `VinylRestoration(4)` | Service category (may be deprecated) |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: "Invalid cast" exception when casting StatusId to enum
**Symptom**: `InvalidCastException` when doing `(InvoiceStatus)invoice.StatusId`
**Cause**: StatusId value doesn't match any enum value (database out of sync)
**Solution**:
```csharp
// Check if valid before casting
if (Enum.IsDefined(typeof(InvoiceStatus), (int)invoice.StatusId))
{
    var status = (InvoiceStatus)invoice.StatusId;
}
else
{
    // Log error and use default
    logger.LogWarning($"Unknown InvoiceStatus ID: {invoice.StatusId}");
    var status = InvoiceStatus.Unpaid;  // Default fallback
}
```

### Issue: Enum values don't match database Lookup table
**Symptom**: Queries return no results when filtering by enum values
**Cause**: Enum definitions out of sync with database
**Solution**: Create a database migration to sync Lookup table:
```sql
-- Verify current Lookup values
SELECT Id, Category, Name FROM Lookup WHERE Category = 'InvoiceStatus';

-- Update if mismatch
UPDATE Lookup SET Id = 81 WHERE Category = 'InvoiceStatus' AND Name = 'Paid';
UPDATE Lookup SET Id = 82 WHERE Category = 'InvoiceStatus' AND Name = 'Unpaid';
-- ... etc

-- Or insert if missing
INSERT INTO Lookup (Id, Category, Name) VALUES (230, 'InvoiceStatus', 'ZeroDue');
```

### Issue: CVV enum type mismatch
**Symptom**: Cannot cast `CvvResponseCode` from integer
**Cause**: Enum uses `char` base type, not `int`
**Solution**:
```csharp
// ❌ WRONG: Treat as int
var cvvCode = (CvvResponseCode)response.CvvCode;  // Error if CvvCode is int

// ✅ CORRECT: Parse as char
char cvvChar = response.CvvResultCode[0];  // Get first char of string
var cvvCode = (CvvResponseCode)cvvChar;

// Or use switch on string
switch (response.CvvResultCode)
{
    case "M": cvvCode = CvvResponseCode.SuccessfullyMatch; break;
    case "N": cvvCode = CvvResponseCode.DoesNotMatch; break;
    // ...
}
```

### Issue: Missing enum values for new card types
**Symptom**: Cannot process Diners Club, JCB, or other card brands
**Solution**: Extend `ChargeCardType` enum and add to Lookup table:
```csharp
public enum ChargeCardType
{
    Visa = 51,
    MasterCard = 52,
    Discover = 53,
    AmericanExpress = 54,
    DinersClub = 55,         // Add new values
    JCB = 56,
    UnionPay = 57
}

// Add corresponding Lookup entries
INSERT INTO Lookup (Id, Category, Name) VALUES 
    (55, 'ChargeCardType', 'DinersClub'),
    (56, 'ChargeCardType', 'JCB'),
    (57, 'ChargeCardType', 'UnionPay');
```

### Issue: Ambiguous enum value (duplicate IDs)
**Symptom**: Two enum members have the same value
**Cause**: Copy-paste error or merge conflict
**Solution**: Ensure all enum values are unique:
```csharp
// ❌ BAD: Duplicate values
public enum InvoiceStatus
{
    Paid = 81,
    Unpaid = 81,  // ERROR: Same as Paid
    // ...
}

// ✅ GOOD: Unique values
public enum InvoiceStatus
{
    Paid = 81,
    Unpaid = 82,  // Unique
    // ...
}
```

### Issue: Enum doesn't show in IntelliSense
**Symptom**: IDE doesn't suggest enum values
**Cause**: Missing `using Core.Billing.Enum;` directive
**Solution**:
```csharp
// Add to top of file
using Core.Billing.Enum;

// Now IntelliSense works:
invoice.StatusId = (long)InvoiceStatus.  // <-- Shows: Paid, Unpaid, etc.
```
<!-- END CUSTOM SECTION -->
