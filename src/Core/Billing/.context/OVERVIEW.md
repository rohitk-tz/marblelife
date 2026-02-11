<!-- AUTO-GENERATED: Header -->
# Core.Billing Module
> Complete franchisee billing system with invoice generation, multi-channel payment processing, automated late fees, and Authorize.Net integration
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Core.Billing module is the financial heartbeat of the MarbleLife franchise management system. Think of it as a comprehensive accounts receivable system specifically designed for franchise operations. It handles everything from generating invoices based on franchisee sales data and royalty agreements, to processing payments through multiple channels (credit cards, eChecks, paper checks, account credits), to automatically calculating and applying late fees for overdue payments.

### The Big Picture

**Why This Exists**: Franchisors need to bill franchisees for royalties (typically a percentage of sales), advertising fund contributions, service fees, and other charges. This module automates that entire workflow, ensuring accurate billing, secure payment processing, and proper financial record-keeping.

**How It Works**: The system operates on a monthly billing cycle:
1. Sales data is uploaded by franchisees
2. Invoices are auto-generated with royalty fees, ad-fund charges, and service fees
3. Franchisees make payments via credit card, eCheck, or check
4. If invoices go unpaid past the due date, late fees are automatically added
5. All transactions are audited and can be exported to QuickBooks

**Key Benefits**:
- **Automated Royalty Calculations**: No manual invoice creation - the system calculates based on sales data
- **Secure Payment Processing**: PCI-compliant integration with Authorize.Net for credit card and ACH payments
- **Late Fee Enforcement**: Automatic late fee generation with configurable grace periods
- **Audit Trail**: Complete history of all financial transactions for compliance
- **Multi-Currency Support**: Currency exchange rates applied at transaction time
- **QuickBooks Integration**: Export invoices and payments for accounting reconciliation

### Real-World Analogy

Imagine you're running a property management company (the franchisor) that collects rent (royalties) from multiple tenants (franchisees). Each month:
- You automatically generate rent invoices based on each tenant's lease terms
- Tenants can pay online with credit card or bank transfer, or mail a check
- If someone's late, you automatically add late fees
- Everything is recorded in your accounting system

This module does exactly that, but for franchise royalties and payments.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup

This module requires:
- SQL Server database with Entity Framework migrations applied
- Authorize.Net merchant account with API credentials
- Background job scheduler configured for polling agents (late fees, invoice generation)
- SMTP server for payment reminder emails

### Example: Processing a Credit Card Payment

```csharp
// Inject services via dependency injection
IPaymentService paymentService;
IInvoiceService invoiceService;

// 1. Franchisee owes $5,000 in royalties (Invoice #12345)
long invoiceId = 12345;
long franchiseeId = 67;

// 2. Create payment model with credit card details
var paymentModel = new ChargeCardPaymentEditModel
{
    InvoiceId = invoiceId,
    Amount = 5000.00m,
    ChargeCardEditModel = new ChargeCardEditModel
    {
        NameOnCard = "John Smith",
        Number = "4111111111111111",  // Test Visa
        ExpiryMonth = 12,
        ExpiryYear = 2025,
        TypeId = (long)ChargeCardType.Visa,
        CVV = "123"
    },
    ProfileTypeId = 45  // ChargeCardOnFile - save for future use
};

// 3. Process payment through Authorize.Net
ProcessorResponse response = paymentService.MakePaymentByNewChargeCard(
    paymentModel, 
    franchiseeId, 
    invoiceId
);

// 4. Check result
if (response.ProcessorResult == ProcessorResponseResult.Accepted)
{
    Console.WriteLine($"Payment successful! Transaction ID: {response.CustomerProfileId}");
    
    // Invoice status automatically updated to Paid
    var invoice = invoiceService.InvoiceDetails(invoiceId);
    Console.WriteLine($"Invoice Status: {invoice.StatusId}"); // 81 = Paid
}
else
{
    Console.WriteLine($"Payment failed: {response.Message}");
    // Card might be declined, insufficient funds, etc.
}
```

### Example: Generating Late Fees (Background Agent)

```csharp
// This runs automatically on a schedule (e.g., daily at 2 AM)
IInvoiceLateFeePollingAgent lateFeeAgent;

// The agent scans all overdue invoices and applies late fees
lateFeeAgent.LateFeeGenerator();

// Behind the scenes:
// 1. Finds all invoices with DueDate < CurrentDate - GracePeriod
// 2. Calculates late fee (e.g., 1.5% of outstanding balance or flat $50)
// 3. Creates LateFeeInvoiceItem attached to the invoice
// 4. Sends email notification to franchisee
// 5. Updates invoice total
```

### Example: Creating an Invoice with Royalty and Ad-Fund

```csharp
IInvoiceFactory invoiceFactory;
IFranchiseeInvoiceFactory franchiseeInvoiceFactory;

// Sales data for the month: $100,000 in revenue
decimal monthlySales = 100000m;
long franchiseeId = 67;

// Royalty rate: 6%, Ad-Fund: 2%
decimal royaltyRate = 0.06m;
decimal adFundRate = 0.02m;

// Create invoice with calculated fees
var invoiceModel = new InvoiceEditModel
{
    GeneratedOn = DateTime.UtcNow,
    DueDate = DateTime.UtcNow.AddDays(30),
    StatusId = (long)InvoiceStatus.Unpaid,
    InvoiceItems = new List<InvoiceItemEditModel>
    {
        new InvoiceItemEditModel
        {
            ItemTypeId = (long)InvoiceItemType.RoyaltyFee,
            Description = "Monthly Royalty (6% of $100,000)",
            Quantity = 1,
            Rate = monthlySales * royaltyRate,
            Amount = monthlySales * royaltyRate  // $6,000
        },
        new InvoiceItemEditModel
        {
            ItemTypeId = (long)InvoiceItemType.AdFund,
            Description = "Advertising Fund (2% of $100,000)",
            Quantity = 1,
            Rate = monthlySales * adFundRate,
            Amount = monthlySales * adFundRate  // $2,000
        }
    }
};

var invoice = invoiceService.Save(invoiceModel);
// Total invoice amount: $8,000 due in 30 days
```

### Example: Using Saved Payment Method

```csharp
// Franchisee has saved payment profile from previous transaction
var savedPaymentModel = new InstrumentOnFileEditModel
{
    InvoiceId = 12346,
    Amount = 8000.00m,
    CustomerProfileId = "123456789",  // From ProcessorResponse
    PaymentProfileId = "987654321"
};

// Charges the saved card without re-entering details
ProcessorResponse response = paymentService.MakePaymentOnFileChargeCard(
    savedPaymentModel,
    franchiseeId,
    12346
);
```

### Example: Exporting Invoices to QuickBooks

```csharp
IInvoiceService invoiceService;

// Export all invoices for January 2025
var filter = new InvoiceListFilter
{
    StartDate = new DateTime(2025, 1, 1),
    EndDate = new DateTime(2025, 1, 31),
    StatusId = (long)InvoiceStatus.Paid
};

// Generate Excel file with royalty data
string fileName;
bool success = invoiceService.CreateExcelRoyality(
    new long[] { 12345, 12346, 12347 },
    out fileName
);

if (success)
{
    Console.WriteLine($"Export ready: {fileName}");
    // File contains columns: Invoice ID, Franchisee, Amount, Date, etc.
    // Import into QuickBooks for reconciliation
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Core Services

| Service | Method | Description |
|---------|--------|-------------|
| **IInvoiceService** | `Save(InvoiceEditModel)` | Create/update invoice with items |
| | `InvoiceDetails(long)` | Get full invoice details |
| | `GetInvoiceList(filter, page, size)` | Paginated invoice list with filters |
| | `CreateExcelRoyality(ids, out fileName)` | Export royalty invoices to Excel |
| | `CreateExcelAdfund(ids, out fileName)` | Export ad-fund invoices to Excel |
| | `MarkInvoicesAsDownloaded(ids)` | Mark invoices as downloaded |
| | `SaveInvoiceReconciliationNotes(model)` | Save QB reconciliation notes |
| **IPaymentService** | `MakePaymentByNewChargeCard(model, franchiseeId, invoiceId)` | Process credit card payment |
| | `MakePaymentOnFileChargeCard(model, franchiseeId, invoiceId)` | Charge saved credit card |
| | `MakePaymentByECheck(model, franchiseeId, invoiceId)` | Process ACH payment |
| | `AccountCreditPayment(amount, invoice, franchiseeId)` | Apply account credit |
| | `CreateOverPaymentInvoiceItem(amount, franchiseeId, invoiceId)` | Handle overpayment |
| **IChargeCardPaymentService** | `ChargeCardPayment(model, franchiseeId, out charge, out amount)` | Process credit card via Authorize.Net |
| | `ChargeCardOnFile(model, response, franchiseeId)` | Charge saved card profile |
| | `RollbackPayment(accountTypeId, transactionId)` | Void/refund transaction |
| **IECheckPaymentService** | `MakeECheckPayment(model, franchiseeId)` | Process ACH via Authorize.Net |
| | `Save(model, payment)` | Save eCheck payment details |
| **IInvoiceLateFeePollingAgent** | `LateFeeGenerator()` | Scheduled late fee generation |
| | `SaveRoyalityLateFee(invoice, date)` | Calculate royalty late fee |
| **IFranchiseeSalesPaymentService** | `Save(model)` | Create payment from sales data |
| | `GetPaymentInstrument(list)` | Get payment method names |
| **ICalculateLoanScheduleService** | `CalculateSchedule()` | Recalculate loan schedules |
| | `CheckingForOverPaidLoan(list)` | Detect loan overpayments |

### Factory Services

| Factory | Method | Description |
|---------|--------|-------------|
| **IInvoiceFactory** | `Create(model)` | Create Invoice entity |
| **IPaymentFactory** | `Create(model)` | Create Payment entity |
| **IChargeCardFactory** | `CreateChargeCard(model)` | Create ChargeCard entity |
| **IECheckFactory** | `CreateECheck(model)` | Create ECheck entity |
| **ICheckFactory** | `CreateCheck(model)` | Create Check entity |
| **IInvoiceItemFactory** | `Create(model)` | Create InvoiceItem entity |
| **IPaymentItemFactory** | `Create(model)` | Create PaymentItem entity |
| **IAuditFactory** | `CreateAudit*(entity)` | Create audit trail entities |

### Supporting Services

| Service | Method | Description |
|---------|--------|-------------|
| **IInvoiceItemService** | `Save(model)` | Manage invoice line items |
| **IInvoicePaymentService** | `Link(payment, invoice)` | Create payment-to-invoice link |
| **IChargeCardService** | `Save(model)` | Save credit card record |
| **IEcheckService** | `Save(model)` | Save eCheck record |
| **ICheckService** | `Save(model)` | Save check record |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Module Structure -->
## Module Structure

### Folder Organization

```
Core/Billing/
├── .context/               # AI-generated documentation (this folder)
├── Constants/             # Service type aliases and billing constants
├── Domain/                # 31 entity classes (Invoice, Payment, ChargeCard, etc.)
│   ├── Audit*.cs         # 10 audit trail entities for compliance
│   ├── Invoice*.cs       # Invoice and related entities
│   ├── Payment*.cs       # Payment and related entities
│   └── *InvoiceItem.cs   # Polymorphic invoice line items
├── Enum/                 # 13 enumerations for billing business logic
├── Impl/                 # 21 service implementations
│   ├── *Factory.cs       # Factory implementations (10 files)
│   ├── *Service.cs       # Service implementations (11 files)
│   └── *PollingAgent.cs  # Background jobs (2 files)
├── ViewModel/            # 27 DTOs for API layer
│   ├── *EditModel.cs     # Input models for creates/updates
│   ├── *ViewModel.cs     # Output models for reads
│   └── ProcessorResponse.cs  # Critical: Payment gateway response
└── I*.cs                 # 25 interface definitions (root level)
```

### Key Design Patterns

**Polymorphic Entities**: 
- `Payment` has child entities (`ChargeCardPayment`, `ECheckPayment`, `CheckPayment`) based on `InstrumentTypeId`
- `InvoiceItem` has child entities (`RoyaltyInvoiceItem`, `LateFeeInvoiceItem`, etc.) based on `ItemTypeId`

**Factory Pattern**: 
- Every domain entity has a factory interface for creation
- Factories handle validation and default value assignment
- Separates creation logic from business logic

**Audit Trail**:
- Every financial entity has a corresponding `Audit*` entity
- Snapshots captured at transaction time for regulatory compliance
- Never modify audit records - append-only

**Background Agents**:
- `InvoiceLateFeePollingAgent`: Scheduled late fee generation
- `FranchiseeInvoiceGenerationPollingAgent`: Scheduled invoice creation from sales data

### Data Relationships

```
Franchisee (Organizations)
    ↓ has many
FranchiseeInvoice
    ↓ references
Invoice ←→ InvoiceItem (1:many, cascade delete)
    ↓ links via
InvoicePayment (junction table)
    ↓ references
Payment ←→ PaymentItem (1:many, cascade delete)
    ↓ has one child (polymorphic)
ChargeCardPayment / ECheckPayment / CheckPayment
    ↓ references
ChargeCard / ECheck / Check
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem**: Payment shows as Approved but invoice still shows Unpaid
- **Cause**: `InvoicePayment` junction record not created or invoice status update failed
- **Solution**: Check `InvoicePayments` collection on Invoice entity. If missing, manually create junction record and recalculate invoice status.

**Problem**: Late fees generated multiple times for same invoice
- **Cause**: `LateFeePollingAgent` doesn't check for existing late fee items before creating new ones
- **Solution**: Query `LateFeeInvoiceItem` by invoice ID and date range before creating new late fee.

**Problem**: Authorize.Net returns "Duplicate Transaction" error
- **Cause**: Same payment amount submitted within 2 minutes (Authorize.Net duplicate detection)
- **Solution**: Wait 2 minutes or change payment amount by a penny to bypass duplicate check.

**Problem**: Invoice total doesn't match sum of InvoiceItems
- **Cause**: Currency exchange rate drift or manual item deletion without recalculation
- **Solution**: Recalculate invoice total from items: `invoice.InvoiceItems.Sum(x => x.Amount)` and update.

**Problem**: Saved payment profile (ChargeCardOnFile) fails with "Customer Profile Not Found"
- **Cause**: Authorize.Net profile deleted or expired, or wrong API credentials used
- **Solution**: Check `AuthorizeNetApiMaster` for correct credentials. If profile deleted, prompt user to re-enter card.

### Debugging Tips

**Enable Authorize.Net Sandbox Mode**: 
- Use test credentials in `AuthorizeNetApiMaster` table
- Test card: 4111111111111111, any future expiration
- Test eCheck: Routing 121042882, Account 123456789

**Trace Payment Flow**:
1. Check `Payment` table for record creation
2. Check `ChargeCardPayment` or `ECheckPayment` for child record
3. Check `InvoicePayment` for junction record
4. Check `AuditPayment` for audit trail
5. Check `Invoice.StatusId` for status update

**Late Fee Testing**:
- Temporarily set `Invoice.DueDate` to past date (e.g., 30 days ago)
- Manually trigger `LateFeePollingAgent.LateFeeGenerator()`
- Verify `LateFeeInvoiceItem` created with correct amount
- Check that email notification sent

**Excel Export Issues**:
- Exports create temp files in server temp directory
- Check disk space and permissions
- Files are NOT automatically cleaned up - implement cleanup job

### Performance Optimization

**Slow Invoice Listing**:
- Add indexes: `Invoice.GeneratedOn`, `Invoice.StatusId`, `FranchiseeInvoice.FranchiseeId`
- Use pagination - don't load all invoices at once
- Consider caching for frequently-accessed data

**Large Payment Processing**:
- Authorize.Net has rate limits (varies by account type)
- Implement retry with exponential backoff
- Consider async processing for bulk payments

<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Business Rules -->
## Business Rules Reference

### Royalty Calculation
- **Standard Rate**: 6% of gross sales (configurable per franchisee)
- **Ad-Fund Rate**: 2% of gross sales (configurable per franchisee)
- **Minimum Royalty**: Some franchisees have minimum monthly royalty (e.g., $500)
- **Service Types**: Royalty rates may vary by service type (StoneLife, Groutlife, etc.)

### Late Fee Calculation
- **Grace Period**: Typically 15 days after invoice due date (configurable)
- **Royalty Late Fee**: 1.5% of outstanding balance per month OR flat $50 (whichever is greater)
- **Sales Data Late Fee**: Flat $100 for late sales data submission
- **Frequency**: Late fees calculated once per month, not daily
- **Notification**: Email sent when late fee applied

### Payment Application Order
1. **Account Credit**: Applied first to reduce payment amount
2. **Oldest Invoice First**: Payments applied to oldest outstanding invoice
3. **Principal Before Fees**: Applied to original charges before late fees
4. **Overpayment**: Excess creates credit on account or negative invoice item

### Invoice Status Transitions
- **Unpaid (82)**: No payments received, amount due > 0
- **PartialPaid (83)**: Payments received but amount still due
- **Paid (81)**: Full payment received, balance = 0
- **ZeroDue (230)**: Invoice created with $0 amount (e.g., credit memo)
- **Canceled (84)**: Invoice voided, no payment required

### Payment Gateway Limits
- **Credit Card**: Max $99,999.99 per transaction (Authorize.Net limit)
- **eCheck**: Max $1,000,000 per transaction (bank limits vary)
- **Daily Limit**: Varies by merchant account settings
- **Retry Policy**: Failed transactions can retry after 24 hours

<!-- END CUSTOM SECTION -->
