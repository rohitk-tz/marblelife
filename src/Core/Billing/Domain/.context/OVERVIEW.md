<!-- AUTO-GENERATED: Header -->
# Billing Domain Entities
> Entity Framework models for invoices, payments, payment instruments, and audit trails
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Domain** module is the heart of the billing system's data model, containing 31 Entity Framework entity classes that represent the complete lifecycle of franchise billing — from invoice generation through payment processing to audit reconciliation.

**Why it exists**: This module defines the "nouns" of the billing system. Every invoice, payment, credit card, and transaction flows through these entities. They enforce business rules through foreign key relationships, provide cascade delete behavior, and maintain referential integrity across the complex web of billing data.

**Real-world analogy**: Think of this like a general ledger system in accounting — `Invoice` is your accounts receivable, `Payment` is your cash receipts, and the various `InvoiceItem` types (royalty, ad fund, late fees) are your journal entry line items. The `Audit*` entities are like year-end closing snapshots that freeze financial state for compliance.

### Key Concepts

1. **Aggregate Roots**: `Invoice` and `Payment` are the main entities with child collections
2. **Polymorphic Payments**: One payment can be credit card, eCheck, or paper check
3. **Many-to-Many**: Invoices and Payments link through `InvoicePayment` (supports partial payments)
4. **Audit Trail**: Parallel `Audit*` entities snapshot data during annual reconciliation
5. **Type-Object Pattern**: Lookup tables provide extensible categorization (status, item types, instrument types)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
These entities are used with Entity Framework Core. Configure in `DbContext`:

```csharp
public class BillingContext : DbContext
{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentItem> PaymentItems { get; set; }
    public DbSet<InvoicePayment> InvoicePayments { get; set; }
    public DbSet<ChargeCard> ChargeCards { get; set; }
    public DbSet<ECheck> EChecks { get; set; }
    public DbSet<Check> Checks { get; set; }
    // ... other entities
}
```

### Example: Create Invoice with Line Items
```csharp
using Core.Billing.Domain;
using System;
using System.Linq;

// Create invoice with royalty and ad fund charges
var invoice = new Invoice
{
    GeneratedOn = DateTime.Now,
    DueDate = DateTime.Now.AddDays(30),
    StatusId = lookupService.GetStatusId("Draft"),
    InvoiceItems = new List<InvoiceItem>
    {
        new InvoiceItem
        {
            ItemTypeId = lookupService.GetItemTypeId("Royalty"),
            ItemId = groutServiceTypeId,
            Description = "Grout Restoration - 5%",
            Quantity = 1,
            Rate = 1000.00m,
            Amount = 1000.00m,
            CurrencyExchangeRateId = usdRateId,
            RoyaltyInvoiceItem = new RoyaltyInvoiceItem { RoyaltyPercentage = 0.05m }
        },
        new InvoiceItem
        {
            ItemTypeId = lookupService.GetItemTypeId("AdFund"),
            Description = "Advertising Fund - 2%",
            Quantity = 1,
            Rate = 400.00m,
            Amount = 400.00m,
            CurrencyExchangeRateId = usdRateId,
            AdFundInvoiceItem = new AdFundInvoiceItem { AdFundPercentage = 0.02m }
        }
    }
};

context.Invoices.Add(invoice);
await context.SaveChangesAsync();  // Cascade saves InvoiceItems and child entities
```

### Example: Record Credit Card Payment
```csharp
// Record a credit card payment and link to invoice
var chargeCard = new ChargeCard
{
    NameOnCard = "John Doe",
    TypeId = lookupService.GetCardTypeId("Visa"),
    Number = "xxxxxxxxxxxx1234",  // Tokenized in production!
    ExpiryMonth = 12,
    ExpiryYear = 2025
};
context.ChargeCards.Add(chargeCard);
await context.SaveChangesAsync();

var payment = new Payment
{
    Date = DateTime.Now,
    Amount = 1400.00m,
    InstrumentTypeId = lookupService.GetInstrumentTypeId("ChargeCard"),
    CurrencyExchangeRateId = usdRateId,
    ChargeCardPayment = new ChargeCardPayment { ChargeCardId = chargeCard.Id },
    InvoicePayments = new List<InvoicePayment>
    {
        new InvoicePayment 
        { 
            InvoiceId = invoice.Id, 
            Amount = 1400.00m  // Full payment
        }
    }
};

context.Payments.Add(payment);
await context.SaveChangesAsync();

// Update invoice status to Paid
invoice.StatusId = lookupService.GetStatusId("Paid");
await context.SaveChangesAsync();
```

### Example: Query Unpaid Invoices with Eager Loading
```csharp
var unpaidStatusId = lookupService.GetStatusId("Unpaid");
var unpaidInvoices = await context.Invoices
    .Include(i => i.InvoiceItems)
        .ThenInclude(ii => ii.ServiceType)
    .Include(i => i.InvoicePayments)
        .ThenInclude(ip => ip.Payment)
    .Include(i => i.Lookup)  // Status
    .Where(i => i.StatusId == unpaidStatusId && i.DueDate < DateTime.Now)
    .OrderBy(i => i.DueDate)
    .ToListAsync();

foreach (var invoice in unpaidInvoices)
{
    var totalDue = invoice.InvoiceItems.Sum(i => i.Amount);
    var totalPaid = invoice.InvoicePayments.Sum(ip => ip.Amount);
    var balance = totalDue - totalPaid;
    
    Console.WriteLine($"Invoice #{invoice.Id}: ${balance} overdue (due {invoice.DueDate:d})");
}
```

### Example: Partial Payment Allocation
```csharp
// Customer pays $500 toward $1000 invoice
var partialPayment = new Payment
{
    Date = DateTime.Now,
    Amount = 500.00m,
    InstrumentTypeId = lookupService.GetInstrumentTypeId("Check"),
    CurrencyExchangeRateId = usdRateId,
    CheckPayment = new CheckPayment 
    { 
        CheckId = checkId 
    },
    InvoicePayments = new List<InvoicePayment>
    {
        new InvoicePayment 
        { 
            InvoiceId = invoice.Id, 
            Amount = 500.00m  // Partial - $500 still owed
        }
    }
};

context.Payments.Add(partialPayment);
await context.SaveChangesAsync();

// Invoice status stays "Unpaid" until fully paid
```

### Example: Multi-Currency Invoice
```csharp
// Canadian franchisee billed in CAD, converted to USD
var cadRate = await context.CurrencyExchangeRates
    .Where(r => r.Date.Date == DateTime.Today && r.FromCurrency == "CAD" && r.ToCurrency == "USD")
    .FirstOrDefaultAsync();

if (cadRate == null)
{
    // Fetch from API and create rate record
    cadRate = new CurrencyExchangeRate
    {
        Date = DateTime.Today,
        FromCurrency = "CAD",
        ToCurrency = "USD",
        Rate = 0.74m  // Example: 1 CAD = 0.74 USD
    };
    context.CurrencyExchangeRates.Add(cadRate);
    await context.SaveChangesAsync();
}

var cadInvoice = new Invoice
{
    GeneratedOn = DateTime.Now,
    DueDate = DateTime.Now.AddDays(30),
    StatusId = draftStatusId,
    InvoiceItems = new List<InvoiceItem>
    {
        new InvoiceItem
        {
            Description = "Grout Restoration (CAD)",
            Amount = 1000.00m,  // CAD amount
            CurrencyExchangeRateId = cadRate.Id,  // Links to conversion rate
            // ... other fields
        }
    }
};

// Report in USD:
var usdAmount = cadInvoice.InvoiceItems.Sum(i => i.Amount * i.CurrencyExchangeRate.Rate);
// usdAmount = 1000.00 * 0.74 = 740.00 USD
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Core Entities

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `Invoice` | Aggregate root for customer invoices | `GeneratedOn`, `DueDate`, `StatusId`, `InvoiceItems[]` |
| `InvoiceItem` | Line items on invoices | `Description`, `Quantity`, `Rate`, `Amount`, `ItemTypeId` |
| `Payment` | Aggregate root for payments | `Date`, `Amount`, `InstrumentTypeId`, `InvoicePayments[]` |
| `PaymentItem` | Links payments to service types | `PaymentId`, `ItemId`, `ServiceType` |
| `InvoicePayment` | Links invoices to payments (M:N) | `InvoiceId`, `PaymentId`, `Amount` |

### Payment Instruments

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `ChargeCard` | Credit/debit card details | `NameOnCard`, `Number`, `ExpiryMonth`, `ExpiryYear` |
| `ChargeCardPayment` | Links Payment to ChargeCard | `PaymentId`, `ChargeCardId` |
| `ECheck` | Electronic check (ACH) details | `RoutingNumber`, `AccountNumber`, `BankName` |
| `ECheckPayment` | Links Payment to ECheck | `PaymentId`, `ECheckId` |
| `Check` | Paper check details | `CheckNumber`, `BankName`, `CheckDate` |
| `CheckPayment` | Links Payment to Check | `PaymentId`, `CheckId` |

### Specialized Invoice Items

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `LateFeeInvoiceItem` | Late payment charges | `LateFeePercentage`, `DaysPastDue` |
| `InterestRateInvoiceItem` | Interest charges | `InterestRate`, `StartDate`, `EndDate` |
| `ServiceFeeInvoiceItem` | Service/processing fees | `FeePercentage`, `FeeDescription` |
| `RoyaltyInvoiceItem` | Franchise royalty fees | `RoyaltyPercentage` |
| `AdFundInvoiceItem` | Advertising fund contributions | `AdFundPercentage` |

### Franchisee Entities

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `FranchiseeInvoice` | Links invoices to franchisees | `FranchiseeId`, `InvoiceId`, `SalesDataUploadId`, `IsDownloaded` |
| `FranchiseePaymentProfile` | Stored payment profiles | `FranchiseeId`, `ProfileId`, `IsDefault` |

### Audit Entities

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `AuditInvoice` | Historical invoice snapshot | `InvoiceId`, `AnnualUploadId`, `QBInvoiceNumber`, `isActive` |
| `AuditInvoiceItem` | Historical invoice item snapshot | `AuditInvoiceId`, `Description`, `Amount` |
| `AuditPayment` | Historical payment snapshot | `PaymentId`, `Date`, `Amount` |
| `AuditInvoicePayment` | Historical invoice-payment link | `AuditInvoiceId`, `AuditPaymentId`, `Amount` |

### Supporting Entities

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `CurrencyExchangeRate` | CAD/USD conversion rates | `Date`, `FromCurrency`, `ToCurrency`, `Rate` |
| `InvoiceAddress` | Billing address snapshot | `Address1`, `City`, `State`, `ZipCode` |
| `PaymentMailReminder` | Reminder email tracking | `InvoiceId`, `SentDate`, `IsSuccess` |
| `AuthorizeNetApiMaster` | Payment gateway config | `ApiLoginId`, `TransactionKey` |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Cascade Delete Not Working
**Symptom**: Child records remain after deleting parent
**Cause**: Missing `[CascadeEntity]` attribute or not using `DbContext.SaveChanges()`
**Solution**:
```csharp
// Ensure child collections have [CascadeEntity] attribute
[CascadeEntity(IsCollection = true)]
public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }

// Use SaveChanges to trigger cascade
context.Invoices.Remove(invoice);
await context.SaveChangesAsync();  // Deletes InvoiceItems automatically
```

### Issue: N+1 Query Performance
**Symptom**: Slow queries when accessing navigation properties
**Cause**: Lazy loading triggers individual queries for each child record
**Solution**: Always use `.Include()` for eager loading:
```csharp
// ❌ BAD: Lazy loads InvoiceItems for each invoice
var invoices = context.Invoices.Where(i => i.StatusId == id).ToList();
foreach (var inv in invoices) 
{
    var total = inv.InvoiceItems.Sum(i => i.Amount);  // N+1 queries
}

// ✅ GOOD: Single query with join
var invoices = context.Invoices
    .Include(i => i.InvoiceItems)
    .Where(i => i.StatusId == id)
    .ToList();
```

### Issue: PCI Compliance Violation
**Symptom**: Full credit card numbers stored in database
**Cause**: `ChargeCard.Number` allows full PAN storage
**Solution**: **NEVER** store full card numbers:
```csharp
// ❌ NEVER DO THIS
var card = new ChargeCard { Number = "4111111111111111" };

// ✅ Use tokenization
var card = new ChargeCard 
{ 
    Number = "xxxxxxxxxxxx1111",  // Last 4 only
    AuthorizeNetProfileId = "12345"  // Token reference
};
```

### Issue: Currency Conversion Missing
**Symptom**: CAD invoices displayed in wrong currency
**Cause**: Missing or incorrect `CurrencyExchangeRateId`
**Solution**: Always set exchange rate:
```csharp
var rate = await context.CurrencyExchangeRates
    .Where(r => r.Date.Date == DateTime.Today 
                && r.FromCurrency == currency
                && r.ToCurrency == "USD")
    .FirstOrDefaultAsync();

if (rate == null)
{
    // Fetch from external API or use fallback
    rate = await FetchExchangeRate(currency, "USD");
    context.CurrencyExchangeRates.Add(rate);
    await context.SaveChangesAsync();
}

invoiceItem.CurrencyExchangeRateId = rate.Id;
```

### Issue: Orphaned InvoicePayment Records
**Symptom**: Deleting invoice fails with FK constraint error
**Cause**: `InvoicePayment` join table not cascade-deleted
**Solution**: Manually delete join records first:
```csharp
var invoicePayments = context.InvoicePayments.Where(ip => ip.InvoiceId == invoiceId);
context.InvoicePayments.RemoveRange(invoicePayments);
await context.SaveChangesAsync();

// Now safe to delete invoice
context.Invoices.Remove(invoice);
await context.SaveChangesAsync();
```

### Issue: Polymorphic Payment Query Confusion
**Symptom**: Can't filter by payment instrument type
**Cause**: Discriminator pattern requires checking navigation property existence
**Solution**:
```csharp
// Query all credit card payments
var ccPayments = await context.Payments
    .Include(p => p.ChargeCardPayment)
        .ThenInclude(ccp => ccp.ChargeCard)
    .Where(p => p.ChargeCardPayment != null)  // Discriminator check
    .ToListAsync();

// Query all eCheck payments
var echeckPayments = await context.Payments
    .Include(p => p.ECheckPayment)
        .ThenInclude(ecp => ecp.ECheck)
    .Where(p => p.ECheckPayment != null)
    .ToListAsync();
```
<!-- END CUSTOM SECTION -->
