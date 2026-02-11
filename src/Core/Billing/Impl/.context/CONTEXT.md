<!-- AUTO-GENERATED: Header -->
# Impl — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2026-02-10T12:21:23Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
This module contains **21 service implementation classes** that execute all billing business logic — invoice generation, payment processing, payment gateway integration (Authorize.Net), late fee calculation, loan schedule management, and background polling agents. These are the "verbs" of the billing system, orchestrating domain entities, factories, repositories, and external services to fulfill business requirements.

### Design Patterns
- **Repository Pattern**: All services inject `IRepository<T>` for data access, never using `DbContext` directly
- **Factory Pattern**: Factories (`InvoiceFactory`, `PaymentFactory`, `ChargeCardFactory`) encapsulate entity creation logic
- **Service Layer**: Services coordinate multiple repositories and factories to implement complex workflows
- **Unit of Work**: `IUnitOfWork` manages transaction boundaries and provides repository instances
- **Polling Agents**: Background workers (`InvoiceLateFeePollingAgent`, `FranchiseeInvoiceGenerationPollingAgent`) run scheduled tasks
- **Gateway Abstraction**: Payment services (`ChargeCardService`, `ECheckService`) wrap Authorize.Net API calls
- **Dependency Injection**: All services use constructor injection with `[DefaultImplementation]` attribute

### Data Flow

#### Invoice Generation Flow
```
1. FranchiseeSalesPaymentService receives sales data upload
2. InvoiceFactory.CreateInvoice() builds Invoice entity
3. InvoiceItemFactory creates line items (royalty, ad fund, fees)
4. InvoiceService.Save() persists via Repository<Invoice>
5. FranchiseeInvoiceFactory links Invoice to Franchisee
6. Email notification sent via IEmailFactory
```

#### Payment Processing Flow
```
1. PaymentService.MakePaymentByNewChargeCard() receives payment request
2. ChargeCardFactory creates ChargeCard entity
3. ChargeCardService.ProcessPayment() calls Authorize.Net gateway
4. PaymentFactory creates Payment entity with gateway response
5. InvoicePaymentService links Payment to Invoice(s)
6. InvoiceService updates Invoice.StatusId based on balance
7. Payment receipt email sent
```

#### Late Fee Generation Flow (Background Agent)
```
1. InvoiceLateFeePollingAgent.LateFeeGenerator() runs on schedule
2. Query overdue invoices: StatusId = Unpaid/PartialPaid, DueDate < Today
3. For each overdue invoice:
   a. Calculate late fee (percentage of balance * days overdue)
   b. InvoiceItemFactory.CreateLateFeeItem()
   c. Attach to invoice
   d. Send late fee notification email
4. Update invoice totals
```

### Key Responsibilities by Service

**Core Services**:
- `InvoiceService` — CRUD, invoice lifecycle, status management, reporting
- `PaymentService` — Payment orchestration, instrument routing, allocation
- `InvoicePaymentService` — Links payments to invoices (many-to-many)
- `InvoiceItemService` — Line item management

**Payment Instrument Services**:
- `ChargeCardService` — Credit card payment gateway integration
- `ECheckService` — ACH/eCheck payment gateway integration
- `CheckService` — Manual check payment recording
- `ChargeCardProfileService` — Saved payment profiles (Authorize.Net CIM)
- `EcheckProfileService` — Saved eCheck profiles

**Factory Services**:
- `InvoiceFactory` — Creates Invoice entities from view models
- `PaymentFactory` — Creates Payment entities
- `ChargeCardFactory` — Creates ChargeCard and ChargeCardPayment entities
- `InvoiceItemFactory` — Creates InvoiceItem entities with specialized types
- `PaymentItemFactory` — Creates PaymentItem entities
- `FranchiseeInvoiceFactory` — Creates FranchiseeInvoice linking entities
- `AuditFactory` — Creates Audit* entities for reconciliation

**Specialized Services**:
- `FranchiseeSalesPaymentService` — Processes franchisee sales data for invoicing
- `CalculateLoanScheduleService` — Computes loan amortization schedules

**Background Agents**:
- `InvoiceLateFeePollingAgent` — Generates late fees for overdue invoices
- `FranchiseeInvoiceGenerationPollingAgent` — Auto-generates invoices from sales uploads
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Service Signatures

#### InvoiceService
```csharp
[DefaultImplementation]
public class InvoiceService : IInvoiceService
{
    // Core CRUD
    Invoice Get(long id);
    Invoice Create(InvoiceEditModel model);
    void Update(Invoice invoice);
    void Delete(long id);
    
    // Status management
    void MarkAsPaid(long invoiceId);
    void MarkAsPartialPaid(long invoiceId);
    void CancelInvoice(long invoiceId);
    
    // Reporting
    List<InvoiceListViewModel> GetFranchiseeInvoices(long franchiseeId, InvoiceSearchModel search);
    decimal GetOutstandingBalance(long franchiseeId);
    List<InvoiceListViewModel> GetOverdueInvoices(DateTime asOfDate);
    
    // Export
    byte[] ExportInvoicesToExcel(InvoiceSearchModel search);
    ZipFile DownloadInvoicesPdf(List<long> invoiceIds);
}
```

#### PaymentService
```csharp
[DefaultImplementation]
public class PaymentService : IPaymentService
{
    // Payment processing
    ProcessorResponse MakePaymentByNewChargeCard(ChargeCardPaymentEditModel model, long franchiseeId, long invoiceId);
    ProcessorResponse MakePaymentByStoredChargeCard(long profileId, long franchiseeId, long invoiceId);
    ProcessorResponse MakePaymentByNewECheck(ECheckPaymentEditModel model, long franchiseeId, long invoiceId);
    ProcessorResponse MakePaymentByStoredECheck(long profileId, long franchiseeId, long invoiceId);
    ProcessorResponse RecordCheckPayment(CheckPaymentEditModel model, long franchiseeId, long invoiceId);
    
    // Account credit
    decimal GetAccountCreditBalance(long franchiseeId);
    void ApplyAccountCredit(long franchiseeId, long invoiceId, decimal amount);
    
    // Payment allocation
    void AllocatePaymentToInvoices(long paymentId, List<InvoicePaymentAllocation> allocations);
}
```

#### ChargeCardService
```csharp
[DefaultImplementation]
public class ChargeCardService : IChargeCardService
{
    // Gateway integration
    ProcessorResponse ProcessPayment(ChargeCard card, decimal amount, long franchiseeId);
    ProcessorResponse AuthorizePayment(ChargeCard card, decimal amount);
    ProcessorResponse CapturePayment(string transactionId, decimal amount);
    ProcessorResponse VoidPayment(string transactionId);
    ProcessorResponse RefundPayment(string transactionId, decimal amount);
    
    // Card management
    ChargeCard SaveCard(ChargeCardEditModel model);
    void DeleteCard(long cardId);
}
```

#### InvoiceLateFeePollingAgent
```csharp
[DefaultImplementation]
public class InvoiceLateFeePollingAgent : IInvoiceLateFeePollingAgent
{
    // Background worker entry point
    void LateFeeGenerator();  // Called by scheduler (e.g., daily at midnight)
    
    // Internal methods
    List<FranchiseeInvoice> GetOverdueInvoices();
    InvoiceItem CreateLateFeeItem(Invoice invoice, int daysPastDue);
    void SendLateFeeNotification(Franchisee franchisee, Invoice invoice, InvoiceItem lateFeeItem);
}
```

#### Factory Interfaces

```csharp
// InvoiceFactory
public interface IInvoiceFactory
{
    Invoice CreateInvoice(InvoiceEditModel model, long franchiseeId);
    Invoice CreateFromSalesData(FranchiseeSales sales, long franchiseeId);
}

// PaymentFactory
public interface IPaymentFactory
{
    Payment CreatePayment(PaymentEditModel model, long instrumentTypeId);
    Payment CreateFromGatewayResponse(ProcessorResponse response, long instrumentTypeId);
}

// ChargeCardFactory
public interface IChargeCardFactory
{
    ChargeCard CreateChargeCard(ChargeCardEditModel model);
    ChargeCardPayment CreateChargeCardPayment(ProcessorResponse response, Payment payment);
}

// InvoiceItemFactory
public interface IInvoiceItemFactory
{
    InvoiceItem CreateServiceItem(long serviceTypeId, decimal amount);
    InvoiceItem CreateRoyaltyItem(decimal salesAmount, decimal royaltyPercentage);
    InvoiceItem CreateAdFundItem(decimal salesAmount, decimal adFundPercentage);
    InvoiceItem CreateLateFeeItem(decimal invoiceBalance, decimal lateFeePercentage, int daysPastDue);
    InvoiceItem CreateInterestItem(decimal principal, decimal annualRate, DateTime startDate, DateTime endDate);
}
```

### Key Data Structures

#### ProcessorResponse
```csharp
/// <summary>
/// Gateway response wrapper - returned by all payment processing methods
/// </summary>
public class ProcessorResponse
{
    public ProcessorResponseResult ProcessorResult { get; set; }  // Success/Fail/Error
    public string Message { get; set; }  // User-friendly message
    public string RawResponse { get; set; }  // Gateway transaction ID
    public long InstrumentId { get; set; }  // Created ChargeCard/ECheck ID
    public string ErrorCode { get; set; }  // Gateway error code
    public TransactionResponseType? TransactionResponse { get; set; }  // Gateway response type
}
```

#### InvoiceEditModel
```csharp
public class InvoiceEditModel
{
    public long Id { get; set; }
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
    public long FranchiseeId { get; set; }
    public List<InvoiceItemEditModel> Items { get; set; }
}
```

#### ChargeCardPaymentEditModel
```csharp
public class ChargeCardPaymentEditModel
{
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Number { get; set; }  // Card number (will be masked)
    public string Name { get; set; }  // Name on card
    public long TypeId { get; set; }  // ChargeCardType enum value
    public string ExpireMonth { get; set; }  // "01" - "12"
    public string ExpireYear { get; set; }  // "2025"
    public string Cvv { get; set; }  // Security code (not stored)
    public bool SaveProfile { get; set; }  // Create Authorize.Net CIM profile
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Typical Usage Patterns

#### Generate Invoice from Sales Data
```csharp
var invoiceService = container.Resolve<IInvoiceService>();
var franchiseeId = 123;
var salesData = GetFranchiseeSalesForWeek(franchiseeId);

var model = new InvoiceEditModel {
    FranchiseeId = franchiseeId,
    GeneratedOn = DateTime.Now,
    DueDate = DateTime.Now.AddDays(30),
    Items = new List<InvoiceItemEditModel>()
};

// Add royalty line item
model.Items.Add(new InvoiceItemEditModel {
    ItemTypeId = (long)InvoiceItemType.RoyaltyFee,
    Description = "Weekly Royalty - 5%",
    Amount = salesData.TotalSales * 0.05m
});

var invoice = invoiceService.Create(model);
```

#### Process Credit Card Payment
```csharp
var paymentService = container.Resolve<IPaymentService>();

var paymentModel = new ChargeCardPaymentEditModel {
    InvoiceId = 456,
    Amount = 1000.00m,
    Number = "4111111111111111",  // Test Visa
    Name = "John Doe",
    TypeId = (long)ChargeCardType.Visa,
    ExpireMonth = "12",
    ExpireYear = "2025",
    Cvv = "123",
    SaveProfile = true  // Save for future use
};

var response = paymentService.MakePaymentByNewChargeCard(paymentModel, franchiseeId, invoiceId);

if (response.ProcessorResult == ProcessorResponseResult.Success)
{
    Console.WriteLine($"Payment approved. Transaction ID: {response.RawResponse}");
}
else
{
    Console.WriteLine($"Payment failed: {response.Message}");
}
```

#### Apply Account Credit to Invoice
```csharp
var paymentService = container.Resolve<IPaymentService>();
var creditBalance = paymentService.GetAccountCreditBalance(franchiseeId);

if (creditBalance >= invoiceAmount)
{
    paymentService.ApplyAccountCredit(franchiseeId, invoiceId, invoiceAmount);
    // Invoice fully paid with account credit
}
else if (creditBalance > 0)
{
    // Partial payment with credit, collect remaining via card
    paymentService.ApplyAccountCredit(franchiseeId, invoiceId, creditBalance);
    var remaining = invoiceAmount - creditBalance;
    // ... process card payment for remaining
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

**Internal**:
- [Domain entities](../Domain/.context/CONTEXT.md) — All services manipulate domain entities
- [Enums](../Enum/.context/CONTEXT.md) — Status codes, instrument types
- [ViewModels](../ViewModel/.context/CONTEXT.md) — Input/output DTOs
- [Core.Application](../../../Application/.context/CONTEXT.md) — `IRepository<T>`, `IUnitOfWork`, `IClock`
- [Core.Organizations](../../../Organizations/.context/CONTEXT.md) — `Franchisee`, `Customer`, `ServiceType`
- [Core.Sales](../../../Sales/.context/CONTEXT.md) — `FranchiseeSales`, `SalesDataUpload`
- [Core.Notification](../../../Notification/.context/CONTEXT.md) — Email services

**External**:
- Authorize.Net SDK — Payment gateway integration (ChargeCardService, ECheckService)
- Ionic.Zip (DotNetZip) — Invoice PDF bundling
- Entity Framework Core — Implicit via `IRepository<T>`

**Configuration**:
- `ISettings` — App configuration (gateway credentials, late fee rates, etc.)
- `IClock` — Testable time provider
- `ILogService` — Logging framework
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Service Architecture Patterns

#### Repository Injection Pattern
All services follow this pattern:
```csharp
public class InvoiceService : IInvoiceService
{
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IRepository<InvoiceItem> _invoiceItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public InvoiceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepository = unitOfWork.Repository<Invoice>();
        _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
    }
    
    public void Save(Invoice invoice)
    {
        _invoiceRepository.Save(invoice);
        _unitOfWork.Commit();  // Explicit transaction commit
    }
}
```

**Benefits**:
- Testable (mock repositories)
- Transaction control via UnitOfWork
- No direct DbContext coupling

#### Factory Encapsulation
Factories hide complex entity creation logic:
```csharp
// ❌ BAD: Business logic in service
var invoiceItem = new InvoiceItem {
    InvoiceId = invoiceId,
    ItemTypeId = (long)InvoiceItemType.LateFees,
    Amount = invoiceBalance * lateFeePercentage,
    Description = $"Late Fee ({lateFeePercentage * 100}%)",
    Quantity = 1,
    Rate = invoiceBalance * lateFeePercentage,
    CurrencyExchangeRateId = GetDefaultExchangeRateId()
};
invoiceItem.LateFeeInvoiceItem = new LateFeeInvoiceItem {
    LateFeePercentage = lateFeePercentage,
    DaysPastDue = daysPastDue
};

// ✅ GOOD: Factory handles complexity
var invoiceItem = _invoiceItemFactory.CreateLateFeeItem(
    invoiceBalance, 
    lateFeePercentage, 
    daysPastDue
);
```

#### Gateway Abstraction
Payment services wrap Authorize.Net API:
```csharp
// ChargeCardService abstracts gateway details
public ProcessorResponse ProcessPayment(ChargeCard card, decimal amount, long franchiseeId)
{
    try
    {
        // Build Authorize.Net request
        var request = new CreateTransactionRequest {
            TransactionRequest = new TransactionRequestType {
                Amount = amount,
                Payment = new PaymentType {
                    Item = new CreditCardType {
                        CardNumber = card.Number,
                        ExpirationDate = $"{card.ExpiryMonth:00}{card.ExpiryYear}",
                        CardCode = card.Cvv
                    }
                }
            }
        };
        
        // Call gateway
        var controller = new CreateTransactionController(request);
        controller.Execute();
        var response = controller.GetApiResponse();
        
        // Convert to domain response
        return new ProcessorResponse {
            ProcessorResult = response.TransactionResponse.ResponseCode == "1" 
                ? ProcessorResponseResult.Success 
                : ProcessorResponseResult.Fail,
            RawResponse = response.TransactionResponse.TransId,
            Message = response.TransactionResponse.Errors?.FirstOrDefault()?.ErrorText
        };
    }
    catch (Exception ex)
    {
        return new ProcessorResponse {
            ProcessorResult = ProcessorResponseResult.Error,
            Message = ex.Message
        };
    }
}
```

### Critical Business Logic

#### Late Fee Calculation (InvoiceLateFeePollingAgent)
```csharp
public void LateFeeGenerator()
{
    var overdueInvoices = _franchiseeInvoiceRepository
        .Query()
        .Where(fi => fi.Invoice.StatusId == (long)InvoiceStatus.Unpaid 
                  || fi.Invoice.StatusId == (long)InvoiceStatus.PartialPaid)
        .Where(fi => fi.Invoice.DueDate < _clock.Now)
        .ToList();
    
    foreach (var franchiseeInvoice in overdueInvoices)
    {
        var invoice = franchiseeInvoice.Invoice;
        var franchisee = franchiseeInvoice.Franchisee;
        
        // Skip if late fee already applied this billing cycle
        var hasLateFee = invoice.InvoiceItems.Any(i => 
            i.ItemTypeId == (long)InvoiceItemType.LateFees);
        if (hasLateFee) continue;
        
        // Calculate late fee
        var daysPastDue = (_clock.Now - invoice.DueDate).Days;
        var invoiceBalance = invoice.InvoiceItems.Sum(i => i.Amount) 
                           - invoice.InvoicePayments.Sum(ip => ip.Amount);
        var lateFeePercentage = _settings.GetDecimal("LateFeePercentage", 0.015m);  // 1.5%
        
        // Create late fee item
        var lateFeeItem = _invoiceItemFactory.CreateLateFeeItem(
            invoiceBalance, 
            lateFeePercentage, 
            daysPastDue
        );
        
        invoice.InvoiceItems.Add(lateFeeItem);
        _invoiceRepository.Save(invoice);
        
        // Send notification
        _lateFeeNotificationService.SendLateFeeEmail(franchisee, invoice, lateFeeItem);
    }
    
    _unitOfWork.Commit();
}
```

#### Payment Allocation Logic
```csharp
// PaymentService.MakePaymentByNewChargeCard
public ProcessorResponse MakePaymentByNewChargeCard(
    ChargeCardPaymentEditModel model, 
    long franchiseeId, 
    long invoiceId)
{
    // 1. Apply account credit first (if available)
    var invoice = _invoiceRepository.Get(invoiceId);
    var invoiceTotal = invoice.InvoiceItems.Sum(i => i.Amount);
    var existingPayments = invoice.InvoicePayments.Sum(ip => ip.Amount);
    var balanceDue = invoiceTotal - existingPayments;
    
    var creditBalance = _franchiseeAccountCreditRepository
        .Query()
        .Where(c => c.FranchiseeId == franchiseeId && c.Balance > 0)
        .Sum(c => c.Balance);
    
    if (creditBalance > 0)
    {
        var creditApplied = Math.Min(creditBalance, balanceDue);
        ApplyAccountCredit(franchiseeId, invoiceId, creditApplied);
        balanceDue -= creditApplied;
    }
    
    // 2. Process credit card for remaining balance
    if (balanceDue > 0)
    {
        model.Amount = balanceDue;
        var response = _chargeCardPaymentService.ProcessPayment(model, franchiseeId);
        
        if (response.ProcessorResult != ProcessorResponseResult.Success)
        {
            return response;  // Payment failed, don't create payment record
        }
        
        // 3. Create Payment entity
        var payment = _paymentFactory.CreateFromGatewayResponse(
            response, 
            (long)InstrumentType.ChargeCard
        );
        _paymentRepository.Save(payment);
        
        // 4. Link payment to invoice
        _invoicePaymentService.LinkPaymentToInvoice(payment.Id, invoiceId, balanceDue);
        
        // 5. Update invoice status
        if (balanceDue >= invoiceTotal - existingPayments)
        {
            invoice.StatusId = (long)InvoiceStatus.Paid;
        }
        else
        {
            invoice.StatusId = (long)InvoiceStatus.PartialPaid;
        }
        
        _invoiceRepository.Save(invoice);
        _unitOfWork.Commit();
        
        return response;
    }
    
    // Invoice fully paid with account credit
    return new ProcessorResponse { 
        ProcessorResult = ProcessorResponseResult.Success, 
        Message = "Paid with account credit" 
    };
}
```

### Known Issues & Gotchas

1. **PCI Compliance Risk** ⚠️
   - `ChargeCardFactory.CreateChargeCard()` stores last 4 digits only: `model.Number.Substring(model.Number.Length - 4)`
   - BUT: `ChargeCardPaymentEditModel.Number` accepts full card number
   - Full card numbers may be logged/cached in memory — requires PCI DSS audit

2. **Transaction Boundary Confusion**
   - Some services call `_unitOfWork.Commit()` explicitly
   - Others rely on implicit commit (UnitOfWork disposal)
   - **Recommendation**: Always use explicit `_unitOfWork.Commit()` after multi-step operations

3. **Late Fee Double Application**
   - `InvoiceLateFeePollingAgent` checks `hasLateFee` but only for same billing cycle
   - If invoice remains unpaid for 60 days, are multiple late fees applied?
   - **Recommendation**: Add `LastLateFeeDate` to track frequency

4. **Currency Conversion Missing**
   - Services don't consistently apply `CurrencyExchangeRate` when creating invoice items
   - CAD invoices may not convert to USD for reporting
   - **Recommendation**: Add `ICurrencyConverter` service

5. **Async/Await Not Used**
   - All methods are synchronous despite I/O-bound operations (DB, HTTP gateway calls)
   - May cause thread starvation under high load
   - **Recommendation**: Migrate to async pattern (`Task<ProcessorResponse> ProcessPaymentAsync(...)`)

### Testing Recommendations

#### Mock Gateway Responses
```csharp
[Test]
public void ProcessPayment_ShouldHandleDeclinedCard()
{
    // Arrange
    var mockGateway = new Mock<IAuthorizeNetGateway>();
    mockGateway.Setup(g => g.ChargeCard(It.IsAny<ChargeCard>(), It.IsAny<decimal>()))
        .Returns(new ProcessorResponse {
            ProcessorResult = ProcessorResponseResult.Fail,
            Message = "Card declined",
            TransactionResponse = TransactionResponseType.Declined
        });
    
    var service = new ChargeCardService(mockGateway.Object, ...);
    
    // Act
    var result = service.ProcessPayment(testCard, 100.00m, franchiseeId);
    
    // Assert
    Assert.AreEqual(ProcessorResponseResult.Fail, result.ProcessorResult);
    Assert.AreEqual("Card declined", result.Message);
}
```

#### Test Late Fee Generation
```csharp
[Test]
public void LateFeeGenerator_ShouldApplyFeeToOverdueInvoices()
{
    // Arrange
    var clock = new Mock<IClock>();
    clock.Setup(c => c.Now).Returns(new DateTime(2025, 2, 10));  // Fixed time
    
    var overdueInvoice = CreateTestInvoice(
        dueDate: new DateTime(2025, 1, 10),  // 31 days overdue
        statusId: (long)InvoiceStatus.Unpaid,
        balance: 1000.00m
    );
    
    // Act
    var agent = new InvoiceLateFeePollingAgent(unitOfWork, clock.Object, ...);
    agent.LateFeeGenerator();
    
    // Assert
    var lateFeeItem = overdueInvoice.InvoiceItems
        .FirstOrDefault(i => i.ItemTypeId == (long)InvoiceItemType.LateFees);
    
    Assert.IsNotNull(lateFeeItem);
    Assert.AreEqual(15.00m, lateFeeItem.Amount);  // 1.5% of $1000
}
```
<!-- END CUSTOM SECTION -->
