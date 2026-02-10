<!-- AUTO-GENERATED: Header -->
# Billing Service Implementations
> Business logic services for invoice generation, payment processing, and gateway integration
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Impl** (Implementation) module contains 21 service classes that execute all billing business logic. These are the "action" layer — generating invoices, processing payments through Authorize.Net, calculating late fees, managing loan schedules, and running background jobs.

**Why it exists**: This module separates business logic from data models (Domain) and API contracts (ViewModel). Services orchestrate complex workflows involving multiple repositories, factories, external APIs, and business rules. This separation enables testability, maintainability, and adherence to SOLID principles.

**Real-world analogy**: Think of this like a restaurant kitchen. The Domain entities are ingredients, the ViewModels are menu items, and the Impl services are the chefs who know the recipes. `InvoiceService` is the chef who "cooks" invoices from raw sales data. `PaymentService` is the cashier who processes different payment methods (cash, card, credit). `InvoiceLateFeePollingAgent` is the manager who checks nightly for overdue tabs and adds late fees.

### Key Service Categories

1. **Core Services**: Invoice/Payment CRUD and lifecycle management
2. **Payment Gateway Services**: Credit card & eCheck processing via Authorize.Net
3. **Factory Services**: Entity creation from ViewModels
4. **Background Agents**: Scheduled tasks (late fees, auto-invoicing)
5. **Specialized Services**: Loan calculations, franchisee sales processing
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
Services are registered via dependency injection and resolved from IoC container:

```csharp
// Registration (Startup.cs or similar)
container.Register<IInvoiceService, InvoiceService>();
container.Register<IPaymentService, PaymentService>();
// ... other services

// Resolution
var invoiceService = container.Resolve<IInvoiceService>();
```

### Example: Generate Invoice from Sales Upload
```csharp
using Core.Billing.Impl;
using Core.Billing.ViewModel;
using Core.Billing.Enum;

public class FranchiseeController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IInvoiceFactory _invoiceFactory;
    
    public FranchiseeController(IInvoiceService invoiceService, IInvoiceFactory invoiceFactory)
    {
        _invoiceService = invoiceService;
        _invoiceFactory = invoiceFactory;
    }
    
    [HttpPost]
    public ActionResult GenerateWeeklyInvoice(long franchiseeId, long salesUploadId)
    {
        // Get sales data
        var salesData = _franchiseeSalesService.GetByUploadId(salesUploadId);
        
        // Build invoice model
        var model = new InvoiceEditModel
        {
            FranchiseeId = franchiseeId,
            GeneratedOn = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Items = new List<InvoiceItemEditModel>()
        };
        
        // Calculate royalty (5% of sales)
        var royaltyAmount = salesData.TotalRevenue * 0.05m;
        model.Items.Add(new InvoiceItemEditModel
        {
            ItemTypeId = (long)InvoiceItemType.RoyaltyFee,
            Description = $"Royalty Fee - Week of {salesData.WeekStartDate:d}",
            Amount = royaltyAmount,
            Quantity = 1,
            Rate = royaltyAmount
        });
        
        // Calculate ad fund (2% of sales)
        var adFundAmount = salesData.TotalRevenue * 0.02m;
        model.Items.Add(new InvoiceItemEditModel
        {
            ItemTypeId = (long)InvoiceItemType.AdFund,
            Description = "Advertising Fund Contribution",
            Amount = adFundAmount,
            Quantity = 1,
            Rate = adFundAmount
        });
        
        // Create invoice
        var invoice = _invoiceService.Create(model);
        
        // Send email notification
        _invoiceService.SendInvoiceEmail(invoice.Id);
        
        return RedirectToAction("InvoiceDetails", new { id = invoice.Id });
    }
}
```

### Example: Process Credit Card Payment
```csharp
using Core.Billing.Impl;
using Core.Billing.ViewModel;
using Core.Billing.Enum;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    
    [HttpPost]
    public ActionResult ProcessCardPayment(ChargeCardPaymentEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Process payment through Authorize.Net
        var response = _paymentService.MakePaymentByNewChargeCard(
            model, 
            franchiseeId: model.FranchiseeId, 
            invoiceId: model.InvoiceId
        );
        
        if (response.ProcessorResult == ProcessorResponseResult.Success)
        {
            TempData["Success"] = $"Payment of ${model.Amount:N2} approved. Transaction ID: {response.RawResponse}";
            
            // Optionally save card for future use
            if (model.SaveProfile)
            {
                _chargeCardProfileService.CreateProfile(model.FranchiseeId, response.InstrumentId);
            }
            
            return RedirectToAction("PaymentReceipt", new { paymentId = response.InstrumentId });
        }
        else
        {
            ModelState.AddModelError("", response.Message);
            return View(model);
        }
    }
}
```

### Example: Apply Account Credit to Invoice
```csharp
public ActionResult ApplyAccountCredit(long franchiseeId, long invoiceId)
{
    var paymentService = container.Resolve<IPaymentService>();
    
    // Check available credit
    var creditBalance = paymentService.GetAccountCreditBalance(franchiseeId);
    var invoice = _invoiceService.Get(invoiceId);
    var invoiceBalance = invoice.InvoiceItems.Sum(i => i.Amount) 
                       - invoice.InvoicePayments.Sum(ip => ip.Amount);
    
    if (creditBalance <= 0)
    {
        TempData["Error"] = "No account credit available.";
        return RedirectToAction("InvoiceDetails", new { id = invoiceId });
    }
    
    // Apply credit (full or partial)
    var amountToApply = Math.Min(creditBalance, invoiceBalance);
    paymentService.ApplyAccountCredit(franchiseeId, invoiceId, amountToApply);
    
    if (amountToApply >= invoiceBalance)
    {
        TempData["Success"] = $"Invoice paid in full with account credit (${amountToApply:N2})";
    }
    else
    {
        TempData["Success"] = $"${amountToApply:N2} credit applied. Remaining balance: ${invoiceBalance - amountToApply:N2}";
    }
    
    return RedirectToAction("InvoiceDetails", new { id = invoiceId });
}
```

### Example: Pay with Saved Payment Profile
```csharp
public ActionResult PayWithSavedCard(long franchiseeId, long invoiceId, long profileId)
{
    var paymentService = container.Resolve<IPaymentService>();
    
    // Process payment using stored card profile (Authorize.Net CIM)
    var response = paymentService.MakePaymentByStoredChargeCard(
        profileId, 
        franchiseeId, 
        invoiceId
    );
    
    if (response.ProcessorResult == ProcessorResponseResult.Success)
    {
        TempData["Success"] = "Payment successful!";
        return RedirectToAction("PaymentReceipt");
    }
    else
    {
        TempData["Error"] = $"Payment failed: {response.Message}";
        return RedirectToAction("InvoiceDetails", new { id = invoiceId });
    }
}
```

### Example: Generate Late Fees (Background Agent)
```csharp
// Scheduled job (runs daily at midnight via scheduler like Hangfire or Quartz)
public class BillingBackgroundJobs
{
    private readonly IInvoiceLateFeePollingAgent _lateFeeAgent;
    
    public BillingBackgroundJobs(IInvoiceLateFeePollingAgent lateFeeAgent)
    {
        _lateFeeAgent = lateFeeAgent;
    }
    
    [RecurringJob("0 0 * * *")]  // Cron: Daily at midnight
    public void GenerateLateFees()
    {
        try
        {
            _lateFeeAgent.LateFeeGenerator();
            _logger.Info("Late fee generation completed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error($"Late fee generation failed: {ex.Message}", ex);
        }
    }
}
```

### Example: Calculate Loan Amortization Schedule
```csharp
public ActionResult ViewLoanSchedule(long franchiseeId)
{
    var loanService = container.Resolve<ICalculateLoanScheduleService>();
    
    var loanDetails = new LoanCalculationModel
    {
        PrincipalAmount = 50000.00m,
        AnnualInterestRate = 0.06m,  // 6%
        TermMonths = 60,  // 5 years
        StartDate = DateTime.Today
    };
    
    var schedule = loanService.CalculateAmortizationSchedule(loanDetails);
    
    return View(schedule);
}
```

### Example: Bulk Invoice Payment Allocation
```csharp
// Allocate a single payment across multiple invoices
public ActionResult AllocatePayment(long paymentId, List<InvoicePaymentAllocation> allocations)
{
    var paymentService = container.Resolve<IPaymentService>();
    
    // Validate total allocation matches payment amount
    var payment = _paymentRepository.Get(paymentId);
    var totalAllocated = allocations.Sum(a => a.Amount);
    
    if (totalAllocated != payment.Amount)
    {
        ModelState.AddModelError("", "Allocation total must equal payment amount");
        return View(allocations);
    }
    
    // Apply allocations
    paymentService.AllocatePaymentToInvoices(paymentId, allocations);
    
    TempData["Success"] = $"Payment of ${payment.Amount:N2} allocated to {allocations.Count} invoices";
    return RedirectToAction("PaymentDetails", new { id = paymentId });
}
```

### Example: Export Invoices to Excel
```csharp
public ActionResult ExportInvoices(InvoiceSearchModel searchModel)
{
    var invoiceService = container.Resolve<IInvoiceService>();
    
    // Generate Excel file
    var excelData = invoiceService.ExportInvoicesToExcel(searchModel);
    
    return File(
        excelData, 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"Invoices_{DateTime.Now:yyyyMMdd}.xlsx"
    );
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Core Services

| Service | Key Methods | Description |
|---------|-------------|-------------|
| `InvoiceService` | `Create()`, `Get()`, `Update()`, `Delete()`, `MarkAsPaid()`, `GetOverdueInvoices()`, `ExportInvoicesToExcel()` | Invoice CRUD, status management, reporting |
| `PaymentService` | `MakePaymentByNewChargeCard()`, `MakePaymentByStoredChargeCard()`, `MakePaymentByNewECheck()`, `ApplyAccountCredit()`, `AllocatePaymentToInvoices()` | Payment processing orchestration |
| `InvoicePaymentService` | `LinkPaymentToInvoice()`, `GetInvoicePayments()`, `GetPaymentAllocations()` | Manages invoice-payment relationships |
| `InvoiceItemService` | `AddItem()`, `UpdateItem()`, `DeleteItem()`, `RecalculateTotals()` | Invoice line item management |

### Payment Gateway Services

| Service | Key Methods | Description |
|---------|-------------|-------------|
| `ChargeCardService` | `ProcessPayment()`, `AuthorizePayment()`, `CapturePayment()`, `VoidPayment()`, `RefundPayment()` | Credit card gateway integration |
| `ECheckService` | `ProcessPayment()`, `VoidPayment()`, `GetTransactionStatus()` | ACH/eCheck gateway integration |
| `CheckService` | `RecordCheckPayment()`, `MarkCheckCleared()`, `MarkCheckBounced()` | Manual check payment recording |
| `ChargeCardProfileService` | `CreateProfile()`, `GetProfile()`, `DeleteProfile()`, `ListProfiles()` | Authorize.Net CIM profile management |
| `EcheckProfileService` | `CreateProfile()`, `GetProfile()`, `DeleteProfile()`, `ListProfiles()` | Saved eCheck profile management |

### Factory Services

| Service | Key Methods | Description |
|---------|-------------|-------------|
| `InvoiceFactory` | `CreateInvoice()`, `CreateFromSalesData()` | Creates Invoice entities |
| `PaymentFactory` | `CreatePayment()`, `CreateFromGatewayResponse()` | Creates Payment entities |
| `ChargeCardFactory` | `CreateChargeCard()`, `CreateChargeCardPayment()` | Creates ChargeCard entities |
| `InvoiceItemFactory` | `CreateServiceItem()`, `CreateRoyaltyItem()`, `CreateAdFundItem()`, `CreateLateFeeItem()`, `CreateInterestItem()` | Creates specialized InvoiceItem entities |
| `PaymentItemFactory` | `CreatePaymentItem()` | Creates PaymentItem entities |
| `FranchiseeInvoiceFactory` | `CreateFranchiseeInvoice()` | Links Invoice to Franchisee |
| `AuditFactory` | `CreateAuditInvoice()`, `CreateAuditPayment()` | Creates audit trail entities |

### Specialized Services

| Service | Key Methods | Description |
|---------|-------------|-------------|
| `FranchiseeSalesPaymentService` | `ProcessSalesUpload()`, `GenerateInvoiceFromSales()`, `ReconcileSalesPayments()` | Processes franchisee sales data for invoicing |
| `CalculateLoanScheduleService` | `CalculateAmortizationSchedule()`, `GetLoanBalance()`, `CalculateMonthlyPayment()` | Loan amortization calculations |

### Background Agents

| Service | Key Methods | Description |
|---------|-------------|-------------|
| `InvoiceLateFeePollingAgent` | `LateFeeGenerator()` | Scheduled task: Generates late fees for overdue invoices |
| `FranchiseeInvoiceGenerationPollingAgent` | `GenerateInvoices()` | Scheduled task: Auto-generates invoices from sales uploads |

### Common Return Types

| Type | Description |
|------|-------------|
| `ProcessorResponse` | Gateway response wrapper (`ProcessorResult`, `Message`, `RawResponse`, `TransactionId`) |
| `Invoice` | Created/updated invoice entity |
| `Payment` | Created/updated payment entity |
| `List<InvoiceListViewModel>` | Invoice search results |
| `byte[]` | Excel export data |
| `ZipFile` | Bundled PDF invoices |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Payment gateway timeout
**Symptom**: `ProcessorResponse.ProcessorResult == ProcessorResponseResult.Error` with timeout message
**Causes**:
1. Authorize.Net API is down or slow
2. Network connectivity issues
3. Firewall blocking gateway HTTPS requests

**Solution**:
```csharp
// Implement retry logic with exponential backoff
public ProcessorResponse ProcessPaymentWithRetry(ChargeCard card, decimal amount, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        var response = _chargeCardService.ProcessPayment(card, amount, franchiseeId);
        
        if (response.ProcessorResult != ProcessorResponseResult.Error)
        {
            return response;  // Success or decline (not a network error)
        }
        
        if (attempt < maxRetries)
        {
            var delayMs = (int)Math.Pow(2, attempt) * 1000;  // 2s, 4s, 8s
            Thread.Sleep(delayMs);
        }
    }
    
    return new ProcessorResponse 
    { 
        ProcessorResult = ProcessorResponseResult.Error,
        Message = "Payment gateway unavailable after multiple retries" 
    };
}
```

### Issue: Invoice status not updating after payment
**Symptom**: Invoice remains `Unpaid` even after successful payment
**Cause**: Missing `_unitOfWork.Commit()` after updating invoice status
**Solution**:
```csharp
// Always commit after multi-step operations
var payment = _paymentService.MakePaymentByNewChargeCard(model, franchiseeId, invoiceId);

if (payment.ProcessorResult == ProcessorResponseResult.Success)
{
    var invoice = _invoiceRepository.Get(invoiceId);
    invoice.StatusId = (long)InvoiceStatus.Paid;
    _invoiceRepository.Save(invoice);
    _unitOfWork.Commit();  // ⚠️ Critical: Persist changes
}
```

### Issue: Duplicate late fees applied
**Symptom**: Multiple late fee line items on same invoice
**Cause**: `InvoiceLateFeePollingAgent` running multiple times or missing idempotency check
**Solution**:
```csharp
// Add idempotency check in LateFeeGenerator
public void LateFeeGenerator()
{
    var overdueInvoices = GetOverdueInvoices();
    
    foreach (var invoice in overdueInvoices)
    {
        // Check if late fee already applied THIS MONTH
        var lateFeeThisMonth = invoice.InvoiceItems.Any(i => 
            i.ItemTypeId == (long)InvoiceItemType.LateFees 
            && i.Created.Month == _clock.Now.Month
            && i.Created.Year == _clock.Now.Year
        );
        
        if (lateFeeThisMonth)
        {
            continue;  // Skip - already processed this month
        }
        
        // Generate late fee...
    }
}
```

### Issue: CVV mismatch not flagged
**Symptom**: Payments approved despite CVV not matching
**Cause**: CVV response code not checked after gateway approval
**Solution**:
```csharp
// Always validate CVV response
var response = _chargeCardService.ProcessPayment(card, amount, franchiseeId);

if (response.ProcessorResult == ProcessorResponseResult.Success)
{
    // Check CVV result
    var cvvCode = (CvvResponseCode)response.CvvResultCode;
    
    if (cvvCode != CvvResponseCode.SuccessfullyMatch)
    {
        // Flag for manual review
        _fraudService.FlagTransaction(
            payment.Id, 
            $"CVV mismatch: {cvvCode}",
            FraudRiskLevel.Medium
        );
        
        // Optionally void transaction
        if (cvvCode == CvvResponseCode.DoesNotMatch)
        {
            _chargeCardService.VoidPayment(response.RawResponse);
            return new ProcessorResponse 
            {
                ProcessorResult = ProcessorResponseResult.Fail,
                Message = "CVV verification failed"
            };
        }
    }
}
```

### Issue: Account credit not reducing balance
**Symptom**: Account credit applied but invoice balance unchanged
**Cause**: Credit application creates Payment record but doesn't link to Invoice via InvoicePayment
**Solution**:
```csharp
public void ApplyAccountCredit(long franchiseeId, long invoiceId, decimal amount)
{
    // 1. Deduct from account credit
    var credit = _franchiseeAccountCreditRepository
        .Query()
        .FirstOrDefault(c => c.FranchiseeId == franchiseeId && c.Balance >= amount);
    
    if (credit == null)
    {
        throw new InvalidOperationException("Insufficient account credit");
    }
    
    credit.Balance -= amount;
    _franchiseeAccountCreditRepository.Save(credit);
    
    // 2. Create Payment record
    var payment = new Payment
    {
        Date = _clock.Now,
        Amount = amount,
        InstrumentTypeId = (long)InstrumentType.AccountCredit
    };
    _paymentRepository.Save(payment);
    
    // 3. Link payment to invoice (⚠️ CRITICAL STEP)
    var invoicePayment = new InvoicePayment
    {
        InvoiceId = invoiceId,
        PaymentId = payment.Id,
        Amount = amount
    };
    _invoicePaymentRepository.Save(invoicePayment);
    
    // 4. Update invoice status if fully paid
    var invoice = _invoiceRepository.Get(invoiceId);
    var totalPaid = invoice.InvoicePayments.Sum(ip => ip.Amount);
    var totalDue = invoice.InvoiceItems.Sum(i => i.Amount);
    
    if (totalPaid >= totalDue)
    {
        invoice.StatusId = (long)InvoiceStatus.Paid;
    }
    else if (totalPaid > 0)
    {
        invoice.StatusId = (long)InvoiceStatus.PartialPaid;
    }
    
    _invoiceRepository.Save(invoice);
    _unitOfWork.Commit();
}
```

### Issue: PCI compliance violation
**Symptom**: Security audit finds full credit card numbers in logs
**Cause**: Logging full `ChargeCardPaymentEditModel.Number` before masking
**Solution**:
```csharp
// ❌ NEVER log full card number
_logger.Info($"Processing payment with card {model.Number}");

// ✅ Always mask before logging
_logger.Info($"Processing payment with card ****{model.Number.Substring(model.Number.Length - 4)}");

// ✅ Or use structured logging with masking
_logger.Info("Processing payment", new { 
    CardLastFour = model.Number.Substring(model.Number.Length - 4),
    Amount = model.Amount,
    FranchiseeId = franchiseeId 
});
```

### Issue: Background agent not running
**Symptom**: Late fees never generate, invoices not auto-created
**Cause**: Polling agents not registered with scheduler
**Solution**:
```csharp
// Register background jobs in Startup.cs (using Hangfire example)
public void ConfigureServices(IServiceCollection services)
{
    // ... other services
    
    services.AddHangfire(config => 
        config.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire")));
    
    services.AddHangfireServer();
}

public void Configure(IApplicationBuilder app)
{
    // ... other middleware
    
    app.UseHangfireDashboard();
    
    // Schedule recurring jobs
    RecurringJob.AddOrUpdate<IInvoiceLateFeePollingAgent>(
        "generate-late-fees",
        agent => agent.LateFeeGenerator(),
        Cron.Daily(0)  // Run daily at midnight
    );
    
    RecurringJob.AddOrUpdate<IFranchiseeInvoiceGenerationPollingAgent>(
        "generate-invoices",
        agent => agent.GenerateInvoices(),
        Cron.Weekly(DayOfWeek.Monday, 6)  // Run Mondays at 6am
    );
}
```
<!-- END CUSTOM SECTION -->
