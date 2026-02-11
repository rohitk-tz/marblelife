<!-- AUTO-GENERATED: Header -->
# Domain — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2026-02-10T12:21:23Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
This module defines the **domain entity layer** for the entire billing system. It contains 31 Entity Framework models representing invoices, payments, payment instruments (credit cards, checks, eChecks), audit trails, and franchisee billing relationships. These entities map directly to database tables and enforce referential integrity through foreign key relationships and cascade attributes.

### Design Patterns
- **Domain-Driven Design (DDD)**: Entities inherit from `DomainBase` providing Id, Created, Modified tracking
- **Aggregate Roots**: `Invoice` and `Payment` are aggregate roots with cascade-managed child collections
- **Polymorphic Payment Types**: `Payment` entity has optional navigation properties for `ChargeCardPayment`, `ECheckPayment`, `CheckPayment` (one-to-one discriminator pattern)
- **Audit Pattern**: Parallel `Audit*` entities (AuditInvoice, AuditPayment, etc.) track historical snapshots for reconciliation
- **Type-Object Pattern**: ItemTypeId, InstrumentTypeId, StatusId reference `Lookup` table for extensible categorization

### Data Flow
1. **Invoice Creation**: `Invoice` → `InvoiceItem[]` (line items) → `ServiceType` (what was sold)
2. **Payment Recording**: `Payment` → `PaymentItem[]` → Payment Instrument (ChargeCard/ECheck/Check)
3. **Invoice-Payment Linking**: `InvoicePayment` join entity connects many invoices to many payments
4. **Franchisee Billing**: `FranchiseeInvoice` links `Invoice` to `Franchisee` and `SalesDataUpload` (source data)
5. **Audit Trail**: `AuditInvoice`, `AuditPayment` capture snapshots during annual reconciliation
6. **Currency Conversion**: `CurrencyExchangeRate` applied at invoice/payment item level for CAD/USD handling

### Key Relationships
```
Invoice (1) ←→ (N) InvoiceItem ←→ (1) ServiceType
Invoice (N) ←→ (N) Payment [via InvoicePayment]
Payment (1) ←→ (1) ChargeCardPayment ←→ (1) ChargeCard
Payment (1) ←→ (1) ECheckPayment ←→ (1) ECheck
Payment (1) ←→ (1) CheckPayment ←→ (1) Check
Invoice (N) ←→ (1) FranchiseeInvoice ←→ (1) Franchisee
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Billing Entities

#### Invoice
```csharp
/// <summary>
/// Aggregate root for customer invoices with line items and payment tracking
/// </summary>
public class Invoice : DomainBase
{
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
    public long StatusId { get; set; }  // FK to Lookup (Draft/Sent/Paid/Void)
    public long DataRecorderMetaDataId { get; set; }  // Audit metadata
    
    // QuickBooks integration fields
    public long CustomerInvoiceId { get; set; }
    public string CustomerInvoiceIdString { get; set; }
    public long CustomerQbInvoiceId { get; set; }
    public string CustomerQbInvoiceIdString { get; set; }
    public string ReconciliationNotes { get; set; }
    
    // Navigation properties
    public virtual ICollection<InvoicePayment> InvoicePayments { get; set; }
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }  // Cascade delete
    public virtual ICollection<PaymentMailReminder> PaymentMailReminders { get; set; }
    public virtual Lookup Lookup { get; set; }  // Status lookup
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

#### InvoiceItem
```csharp
/// <summary>
/// Line items on invoices - supports royalty, ad fund, late fees, interest, service fees
/// Uses Type-Object pattern with specialized child entities
/// </summary>
public class InvoiceItem : DomainBase
{
    public long InvoiceId { get; set; }
    public long? ItemId { get; set; }  // FK to ServiceType (nullable for fees)
    public long ItemTypeId { get; set; }  // FK to Lookup (Royalty/AdFund/LateFee/etc)
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }  // Calculated: Quantity * Rate
    public string ItemOriginal { get; set; }  // Original QB line item data
    public long CurrencyExchangeRateId { get; set; }
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; }
    public virtual ServiceType ServiceType { get; set; }
    public virtual Lookup Lookup { get; set; }  // Item type
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    
    // Specialized item types (one-to-one, cascade delete)
    public virtual RoyaltyInvoiceItem RoyaltyInvoiceItem { get; set; }
    public virtual AdFundInvoiceItem AdFundInvoiceItem { get; set; }
    public virtual LateFeeInvoiceItem LateFeeInvoiceItem { get; set; }
    public virtual InterestRateInvoiceItem InterestRateInvoiceItem { get; set; }
    public virtual ServiceFeeInvoiceItem ServiceFeeInvoiceItem { get; set; }
    public virtual FranchiseeFeeEmailInvoiceItem FranchiseeFeeEmailInvoiceItem { get; set; }
}
```

#### Payment
```csharp
/// <summary>
/// Aggregate root for payments - polymorphic instrument (CC/eCheck/Check)
/// </summary>
public class Payment : DomainBase
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public long InstrumentTypeId { get; set; }  // FK to Lookup (ChargeCard/ECheck/Check)
    public long CurrencyExchangeRateId { get; set; }
    
    // Navigation properties
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    public virtual ICollection<InvoicePayment> InvoicePayments { get; set; }
    public virtual ICollection<PaymentItem> PaymentItems { get; set; }  // Cascade delete
    
    // Polymorphic payment instrument (only one populated)
    public virtual ChargeCardPayment ChargeCardPayment { get; set; }
    public virtual ECheckPayment ECheckPayment { get; set; }
    public virtual CheckPayment CheckPayment { get; set; }
}
```

### Payment Instruments

#### ChargeCard
```csharp
/// <summary>
/// Credit/debit card details (PCI-compliant - stores last 4 digits only in production)
/// </summary>
public class ChargeCard : DomainBase
{
    public string NameOnCard { get; set; }
    public long TypeId { get; set; }  // FK to Lookup (Visa/MC/Amex/Discover)
    public string Number { get; set; }  // Should be tokenized/masked in production
    public int ExpiryMonth { get; set; }  // 1-12
    public int ExpiryYear { get; set; }  // YYYY format
    
    public virtual Lookup CardType { get; set; }
}
```

#### ECheck
```csharp
/// <summary>
/// Electronic check (ACH) payment details
/// </summary>
public class ECheck : DomainBase
{
    public string RoutingNumber { get; set; }  // 9-digit ABA routing number
    public long AccountTypeId { get; set; }  // FK to Lookup (Checking/Savings)
    public string Name { get; set; }  // Account holder name
    public string AccountNumber { get; set; }  // Should be encrypted in production
    public string BankName { get; set; }
    
    public virtual Lookup Lookup { get; set; }  // Account type
}
```

#### Check
```csharp
/// <summary>
/// Paper check payment details
/// </summary>
public class Check : DomainBase
{
    public string CheckNumber { get; set; }
    public string BankName { get; set; }
    public DateTime? CheckDate { get; set; }
}
```

### Linking Entities

#### InvoicePayment
```csharp
/// <summary>
/// Many-to-many join table linking invoices to payments
/// Supports partial payments across multiple invoices
/// </summary>
public class InvoicePayment : DomainBase
{
    public long InvoiceId { get; set; }
    public long PaymentId { get; set; }
    public decimal Amount { get; set; }  // How much of the payment applies to this invoice
    
    public virtual Invoice Invoice { get; set; }
    public virtual Payment Payment { get; set; }
}
```

#### PaymentItem
```csharp
/// <summary>
/// Links payments to specific service types for revenue categorization
/// </summary>
public class PaymentItem : DomainBase
{
    public long PaymentId { get; set; }
    public long ItemId { get; set; }  // FK to ServiceType
    public long ItemTypeId { get; set; }  // FK to Lookup
    
    public virtual Payment Payment { get; set; }
    public virtual ServiceType ServiceType { get; set; }
}
```

### Franchisee-Specific Entities

#### FranchiseeInvoice
```csharp
/// <summary>
/// Links invoices to franchisees and sales data uploads
/// Tracks download status for franchisee portal
/// </summary>
public class FranchiseeInvoice : DomainBase
{
    public long FranchiseeId { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public long InvoiceId { get; set; }
    public long? SalesDataUploadId { get; set; }  // Source sales data file
    public bool IsDownloaded { get; set; }  // Franchisee portal tracking
    
    public virtual Franchisee Franchisee { get; set; }
    public virtual Invoice Invoice { get; set; }
    public virtual SalesDataUpload SalesDataUpload { get; set; }
}
```

### Audit Entities (Historical Snapshots)

#### AuditInvoice
```csharp
/// <summary>
/// Snapshot of invoice state during annual reconciliation
/// Preserves QB invoice numbers and reconciliation history
/// </summary>
public class AuditInvoice : DomainBase
{
    public long? InvoiceId { get; set; }  // Reference to live invoice
    public long AnnualUploadId { get; set; }  // Which annual upload created this
    public DateTime GeneratedOn { get; set; }
    public string QBInvoiceNumber { get; set; }
    public string CustomerQbInvoiceId { get; set; }
    public string QBInvoiceNumbers { get; set; }  // Multiple QB invoice IDs (comma-separated?)
    public long StatusId { get; set; }
    public long? ReportTypeId { get; set; }
    public bool isActive { get; set; }
    
    public virtual ICollection<AuditInvoicePayment> AuditInvoicePayments { get; set; }
    public virtual ICollection<AuditInvoiceItem> AuditInvoiceItems { get; set; }  // Cascade
    public virtual Lookup Lookup { get; set; }
    public virtual Invoice Invoice { get; set; }
    public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; }
    public virtual AnnualReportType AnnualReportType { get; set; }
}
```

### Supporting Entities

#### CurrencyExchangeRate
```csharp
/// <summary>
/// Historical exchange rates for CAD/USD conversion
/// Referenced by Invoice, InvoiceItem, Payment
/// </summary>
public class CurrencyExchangeRate : DomainBase
{
    public DateTime Date { get; set; }
    public string FromCurrency { get; set; }  // e.g., "CAD"
    public string ToCurrency { get; set; }  // e.g., "USD"
    public decimal Rate { get; set; }  // Conversion multiplier
}
```

#### PaymentMailReminder
```csharp
/// <summary>
/// Tracks automated payment reminder emails sent to customers
/// </summary>
public class PaymentMailReminder : DomainBase
{
    public long InvoiceId { get; set; }
    public DateTime SentDate { get; set; }
    public string EmailAddress { get; set; }
    public bool IsSuccess { get; set; }
    
    public virtual Invoice Invoice { get; set; }
}
```

#### InvoiceAddress
```csharp
/// <summary>
/// Billing address snapshot for invoice (preserves address even if customer moves)
/// </summary>
public class InvoiceAddress : DomainBase
{
    public long InvoiceId { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    
    public virtual Invoice Invoice { get; set; }
}
```

### Specialized Invoice Item Types

#### LateFeeInvoiceItem
```csharp
public class LateFeeInvoiceItem : DomainBase
{
    public long InvoiceItemId { get; set; }
    public decimal LateFeePercentage { get; set; }
    public int DaysPastDue { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}
```

#### InterestRateInvoiceItem
```csharp
public class InterestRateInvoiceItem : DomainBase
{
    public long InvoiceItemId { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}
```

#### ServiceFeeInvoiceItem
```csharp
public class ServiceFeeInvoiceItem : DomainBase
{
    public long InvoiceItemId { get; set; }
    public decimal FeePercentage { get; set; }
    public string FeeDescription { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

These entities are accessed via repositories and services in the `Impl/` folder. Direct entity instantiation is typically done through factory interfaces.

### Entity Lifecycle Patterns

**Invoice Creation**:
```csharp
var invoice = new Invoice {
    GeneratedOn = DateTime.Now,
    DueDate = DateTime.Now.AddDays(30),
    StatusId = lookupService.GetId("Invoice.Status.Draft")
};
invoice.InvoiceItems.Add(new InvoiceItem { ... });
context.Invoices.Add(invoice);
context.SaveChanges();  // Cascade saves InvoiceItems
```

**Payment Recording**:
```csharp
var payment = new Payment {
    Date = DateTime.Now,
    Amount = 100.00m,
    InstrumentTypeId = lookupService.GetId("Payment.Instrument.ChargeCard")
};
payment.ChargeCardPayment = new ChargeCardPayment { ChargeCardId = cardId };
payment.InvoicePayments.Add(new InvoicePayment { InvoiceId = invoiceId, Amount = 100.00m });
context.Payments.Add(payment);
context.SaveChanges();
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

**Internal**:
- [Core.Application.Domain](../../../Application/Domain/.context/CONTEXT.md) — Base entity classes (`DomainBase`, `Lookup`)
- [Core.Organizations.Domain](../../../Organizations/Domain/.context/CONTEXT.md) — `Franchisee`, `ServiceType`
- [Core.Sales.Domain](../../../Sales/Domain/.context/CONTEXT.md) — `SalesDataUpload`, `AnnualSalesDataUpload`
- [Core.Notification.ViewModel](../../../Notification/ViewModel/.context/CONTEXT.md) — Email templates

**External**:
- `System.ComponentModel.DataAnnotations` — Validation attributes
- `System.ComponentModel.DataAnnotations.Schema` — EF foreign key attributes
- Entity Framework Core — ORM (implied)

**Custom Attributes**:
- `Core.Application.Attribute.CascadeEntity` — Marks child entities for cascade delete/save
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Data Model Patterns

#### Polymorphic Payment Instruments
Payments use a discriminator pattern with optional navigation properties:
```csharp
// Only one of these will be populated:
payment.ChargeCardPayment  // For credit card payments
payment.ECheckPayment      // For ACH payments
payment.CheckPayment       // For paper check payments

// Query pattern:
var creditCardPayments = context.Payments
    .Include(p => p.ChargeCardPayment)
        .ThenInclude(ccp => ccp.ChargeCard)
    .Where(p => p.ChargeCardPayment != null);
```

#### Cascade Delete Hierarchy
```
Invoice [Cascade Delete]
├── InvoiceItem [Cascade Delete]
│   ├── RoyaltyInvoiceItem
│   ├── AdFundInvoiceItem
│   ├── LateFeeInvoiceItem
│   └── ...
└── InvoicePayment [Manual Delete Required]
    └── Payment [Separate Aggregate]

Payment [Cascade Delete]
├── PaymentItem
├── ChargeCardPayment
├── ECheckPayment
└── CheckPayment
```

⚠️ **Warning**: Deleting an `Invoice` does NOT delete linked `Payment` records. You must manually remove `InvoicePayment` join records first.

#### QuickBooks Integration Fields
Multiple QB ID fields exist for legacy reconciliation:
- `CustomerInvoiceId` / `CustomerInvoiceIdString` — Original QB customer invoice reference
- `CustomerQbInvoiceId` / `CustomerQbInvoiceIdString` — QB desktop sync ID
- Both numeric and string versions exist for backward compatibility

### Known Issues

1. **Security: PCI Compliance** ⚠️
   - `ChargeCard.Number` and `ECheck.AccountNumber` should NEVER store full values in production
   - Requires tokenization (e.g., Authorize.Net CIM, Stripe tokens)
   - Current model allows full card storage — MAJOR SECURITY RISK

2. **Data Type: Currency Precision**
   - `decimal` type used for amounts — correct for financial calculations
   - Ensure database columns are `DECIMAL(19,4)` or higher precision

3. **Nullable ItemId in InvoiceItem**
   - `ItemId` is nullable to support late fees, interest charges (not tied to ServiceType)
   - Always check `ItemTypeId` before accessing `ServiceType` navigation property

4. **Audit Entity Naming**
   - `isActive` field uses lowercase — inconsistent with C# naming conventions
   - `QBInvoiceNumbers` appears to store comma-separated values — should be relational table

5. **Missing Indexes**
   - Ensure indexes on: `Invoice.StatusId`, `Invoice.GeneratedOn`, `Payment.Date`, `InvoicePayment.InvoiceId`, `InvoicePayment.PaymentId`

### Usage Recommendations

#### Query Performance
```csharp
// ❌ BAD: N+1 query problem
var invoices = context.Invoices.Where(i => i.StatusId == paidStatusId).ToList();
foreach (var invoice in invoices) {
    var total = invoice.InvoiceItems.Sum(i => i.Amount);  // Lazy load per invoice
}

// ✅ GOOD: Eager loading
var invoices = context.Invoices
    .Include(i => i.InvoiceItems)
    .Where(i => i.StatusId == paidStatusId)
    .ToList();
var totals = invoices.Select(i => new { 
    InvoiceId = i.Id, 
    Total = i.InvoiceItems.Sum(item => item.Amount) 
});
```

#### Payment Allocation
```csharp
// Allocate a $500 payment across multiple invoices
var payment = new Payment { Amount = 500.00m, Date = DateTime.Now };
var unpaidInvoices = context.Invoices
    .Include(i => i.InvoiceItems)
    .Where(i => i.StatusId == unpaidStatusId)
    .OrderBy(i => i.DueDate)
    .ToList();

decimal remainingAmount = 500.00m;
foreach (var invoice in unpaidInvoices) {
    var invoiceBalance = invoice.InvoiceItems.Sum(i => i.Amount) 
                       - invoice.InvoicePayments.Sum(ip => ip.Amount);
    var allocationAmount = Math.Min(remainingAmount, invoiceBalance);
    
    payment.InvoicePayments.Add(new InvoicePayment {
        InvoiceId = invoice.Id,
        Amount = allocationAmount
    });
    
    remainingAmount -= allocationAmount;
    if (remainingAmount <= 0) break;
}
```

#### Currency Conversion
```csharp
// Apply exchange rate to CAD invoices
var cadInvoices = context.Invoices
    .Include(i => i.InvoiceItems)
        .ThenInclude(ii => ii.CurrencyExchangeRate)
    .Where(i => i.InvoiceItems.Any(ii => ii.CurrencyExchangeRate.FromCurrency == "CAD"));

foreach (var invoice in cadInvoices) {
    foreach (var item in invoice.InvoiceItems) {
        var usdAmount = item.Amount * item.CurrencyExchangeRate.Rate;
        // Use usdAmount for reporting
    }
}
```
<!-- END CUSTOM SECTION -->
