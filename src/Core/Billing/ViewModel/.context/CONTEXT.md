<!-- AUTO-GENERATED: Header -->
# ViewModel — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2026-02-10T12:21:23Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
This module defines **26 Data Transfer Objects (DTOs)** that serve as the API contract layer between the billing system and external consumers (web controllers, API endpoints, client applications). ViewModels decouple the domain model from presentation concerns, providing flattened, UI-friendly representations of complex domain entities with validation, formatting, and client-specific fields.

### Design Patterns
- **DTO Pattern**: ViewModels are pure data containers with no business logic
- **Edit vs Display Models**: `*EditModel` for input/forms, `*ViewModel` for read-only display
- **Model Binding**: Decorated with validation attributes for ASP.NET MVC/API model binding
- **Flattening**: Complex domain graphs flattened into single objects (e.g., `InvoiceDetailsViewModel` combines Invoice + InvoiceItems + Franchisee + Address)
- **Inheritance**: Base classes (`EditModelBase`, `EPaymentEditModel`) provide common fields

### Data Flow

#### Input (Client → Server)
```
1. Client submits form/API request with ViewModel
2. ASP.NET model binding deserializes JSON/form data to *EditModel
3. Validation attributes execute (`[Required]`, `[Range]`, `[CreditCard]`)
4. Controller passes validated model to service layer
5. Service/Factory converts EditModel → Domain entity
6. Repository persists entity
```

#### Output (Server → Client)
```
1. Service layer queries domain entities via repository
2. Service/Controller maps Domain entity → *ViewModel
3. ViewModel serialized to JSON/XML
4. Client receives DTO (no domain internals exposed)
```

### Key ViewModel Categories

**Input Models (Forms/API Requests)**:
- `InvoiceEditModel` — Create/update invoice
- `ChargeCardPaymentEditModel` — Credit card payment form
- `ECheckEditModel` — eCheck payment form
- `CheckPaymentEditModel` — Manual check entry
- `InvoiceItemEditModel` — Invoice line item

**Output Models (Display/API Response)**:
- `InvoiceDetailsViewModel` — Full invoice details for display
- `InvoiceViewModel` — Invoice summary for lists
- `InvoiceListModel` — Paginated invoice collection
- `PaymentModeDetailViewModel` — Payment details display
- `FranchiseePaymentInstrumentViewModel` — Saved payment methods

**Integration Models**:
- `ProcessorResponse` — Payment gateway response wrapper
- `CurrencyExchangeRateViewModel` — Exchange rate data
- `DownloadInvoiceModel` — PDF download request
- `InvoiceListFilter` — Search/filter criteria
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Input Models

#### InvoiceEditModel
```csharp
/// <summary>
/// Create or update invoice - typically from admin/franchisee portal
/// </summary>
public class InvoiceEditModel : EditModelBase
{
    public long Id { get; set; }  // 0 for new invoice
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
    public long? InvoiceId { get; set; }  // Reference to existing invoice (for audit)
    public long AnnualSalesDataUploadId { get; set; }  // Source sales data
    public long StatusId { get; set; }  // InvoiceStatus enum value
    
    public IList<FranchiseeSalesPaymentEditModel> Payments { get; set; }
    public IList<InvoiceItemEditModel> InvoiceItems { get; set; }
    
    // Constructor initializes collections
    public InvoiceEditModel()
    {
        Payments = new List<FranchiseeSalesPaymentEditModel>();
        InvoiceItems = new List<InvoiceItemEditModel>();
    }
}
```

#### InvoiceItemEditModel
```csharp
/// <summary>
/// Invoice line item - service, fee, or charge
/// </summary>
public class InvoiceItemEditModel
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public long ItemTypeId { get; set; }  // InvoiceItemType enum
    public long? ItemId { get; set; }  // ServiceType ID (nullable for fees)
    public string Description { get; set; }  // Line item description
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }  // Unit price
    public decimal Amount { get; set; }  // Calculated: Quantity * Rate
    public long CurrencyExchangeRateId { get; set; }
}
```

#### ChargeCardPaymentEditModel
```csharp
/// <summary>
/// Credit card payment form submission
/// Inherits from EPaymentEditModel (amount, invoice, franchisee)
/// </summary>
[NoValidatorRequired]  // Custom validation in service layer
public class ChargeCardPaymentEditModel : EPaymentEditModel
{
    public ChargeCardEditModel ChargeCardEditModel { get; set; }  // Card details
    public long ProfileTypeId { get; set; }  // New card vs saved profile
}

/// <summary>
/// Credit card details (nested in ChargeCardPaymentEditModel)
/// </summary>
public class ChargeCardEditModel
{
    [Required]
    [CreditCard]
    public string Number { get; set; }  // Full card number (masked after processing)
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }  // Name on card
    
    [Required]
    public long TypeId { get; set; }  // ChargeCardType enum (Visa/MC/etc)
    
    [Required]
    [Range(1, 12)]
    public string ExpireMonth { get; set; }  // "01" - "12"
    
    [Required]
    [Range(2020, 2100)]
    public string ExpireYear { get; set; }  // "2025"
    
    [Required]
    [StringLength(4)]
    public string Cvv { get; set; }  // Card security code (3-4 digits)
    
    public bool SaveProfile { get; set; }  // Store for future use
}
```

#### ECheckEditModel
```csharp
/// <summary>
/// Electronic check (ACH) payment form
/// </summary>
public class ECheckEditModel
{
    [Required]
    [StringLength(9, MinimumLength = 9)]
    [RegularExpression(@"^\d{9}$")]
    public string RoutingNumber { get; set; }  // 9-digit ABA routing number
    
    [Required]
    public string AccountNumber { get; set; }  // Bank account number
    
    [Required]
    public long AccountTypeId { get; set; }  // AccountType enum (Checking/Savings)
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }  // Account holder name
    
    public string BankName { get; set; }
    public bool SaveProfile { get; set; }
}
```

#### CheckPaymentEditModel
```csharp
/// <summary>
/// Manual paper check payment entry
/// </summary>
public class CheckPaymentEditModel
{
    public long InvoiceId { get; set; }
    public long FranchiseeId { get; set; }
    public decimal Amount { get; set; }
    
    [Required]
    public string CheckNumber { get; set; }
    
    public string BankName { get; set; }
    public DateTime? CheckDate { get; set; }
    public DateTime ReceivedDate { get; set; }
}
```

#### EPaymentEditModel (Base Class)
```csharp
/// <summary>
/// Base class for electronic payment models
/// </summary>
public class EPaymentEditModel
{
    public long InvoiceId { get; set; }
    public long FranchiseeId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    public bool IsLoanOverPayment { get; set; }  // Overpayment handling flag
    public decimal OverPaymentAmount { get; set; }
}
```

### Output Models

#### InvoiceDetailsViewModel
```csharp
/// <summary>
/// Comprehensive invoice details for display/PDF generation
/// Flattens Invoice + Items + Payments + Franchisee + Customer
/// </summary>
[NoValidatorRequired]
public class InvoiceDetailsViewModel
{
    // Invoice IDs
    public long? InvoiceId { get; set; }
    public long AuditInvoiceId { get; set; }
    public long AnnualUploadId { get; set; }
    public string QBInvoiceNumber { get; set; }
    public string QBInvoiceNumbers { get; set; }  // Multiple QB IDs (comma-separated)
    
    // Customer/Franchisee info
    public string Customer { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string FranchiseeName { get; set; }
    public string PhoneNumber { get; set; }
    public string FranchiseePhone { get; set; }
    public IEnumerable<PhoneEditModel> PhoneNumbers { get; set; }
    public IEnumerable<EmailEditModel> CustomerEmails { get; set; }
    public AddressViewModel Address { get; set; }
    public AddressViewModel FranchiseeAddress { get; set; }
    
    // Financial totals
    public decimal TotalAmount { get; set; }  // Sum of line items
    public decimal TotalPayment { get; set; }  // Sum of payments applied
    public decimal GrandTotal { get; set; }  // Balance due
    public decimal? SalesAmount { get; set; }  // Source sales data
    public string CurrencyCode { get; set; }  // USD/CAD
    
    // Dates
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime UploadEndDate { get; set; }
    
    // Status
    public long StatusId { get; set; }  // InvoiceStatus enum
    
    // Collections
    public ICollection<FranchiseeSalesPaymentEditModel> Payments { get; set; }
    public ICollection<InvoiceItemEditModel> InvoiceItems { get; set; }
    
    // Loan-related (if applicable)
    public decimal LoanAmount { get; set; }
    public decimal RemainingLoanAmount { get; set; }
    public decimal CurrentLoanAmount { get; set; }
    
    // Reporting
    public decimal ReportId { get; set; }
    public string ReportName { get; set; }
    public SumByCategory SumByCategory { get; set; }  // Line item categorization
}
```

#### InvoiceViewModel
```csharp
/// <summary>
/// Invoice summary for list views
/// </summary>
public class InvoiceViewModel
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public long StatusId { get; set; }
    public string StatusName { get; set; }  // "Paid", "Unpaid", etc.
    public string FranchiseeName { get; set; }
    public string CustomerName { get; set; }
}
```

#### InvoiceListModel
```csharp
/// <summary>
/// Paginated invoice collection with filters
/// </summary>
[NoValidatorRequired]
public class InvoiceListModel
{
    public IEnumerable<InvoiceViewModel> Collection { get; set; }  // Invoices on current page
    public InvoiceListFilter Filter { get; set; }  // Applied search filters
    public PagingModel PagingModel { get; set; }  // Page number, size, total count
    public decimal? TotalUnPaidAmount { get; set; }  // Sum of outstanding balances
    public string FranchiseeName { get; set; }
}
```

#### InvoiceListFilter
```csharp
/// <summary>
/// Search/filter criteria for invoice queries
/// </summary>
public class InvoiceListFilter
{
    public long? FranchiseeId { get; set; }
    public long? StatusId { get; set; }  // InvoiceStatus enum
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string CustomerName { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool? IsOverdue { get; set; }
}
```

#### PaymentModeDetailViewModel
```csharp
/// <summary>
/// Payment transaction details for display
/// </summary>
public class PaymentModeDetailViewModel
{
    public long PaymentId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string InstrumentType { get; set; }  // "Credit Card", "eCheck", "Check"
    public string TransactionId { get; set; }  // Gateway transaction ID
    public string Status { get; set; }  // "Approved", "Declined", etc.
    
    // Instrument details (only one populated)
    public string CardLastFour { get; set; }  // Credit card: "****1234"
    public string CardType { get; set; }  // "Visa", "MasterCard"
    public string CheckNumber { get; set; }  // Paper check
    public string BankName { get; set; }  // eCheck/Check
}
```

### Gateway Integration Models

#### ProcessorResponse
```csharp
/// <summary>
/// Payment gateway response wrapper - returned by all payment service methods
/// Normalizes Authorize.Net responses into domain-specific format
/// </summary>
public class ProcessorResponse
{
    public ProcessorResponseResult ProcessorResult { get; set; }  // Success/Fail/Error enum
    public string Message { get; set; }  // User-friendly message ("Payment approved", "Card declined")
    public string RawResponse { get; set; }  // Gateway transaction ID
    public long InstrumentId { get; set; }  // Created Payment/ChargeCard/ECheck entity ID
    public string CustomerProfileId { get; set; }  // Authorize.Net CIM profile ID (if saved)
}
```

### Utility Models

#### CurrencyExchangeRateViewModel
```csharp
/// <summary>
/// Currency conversion rate display
/// </summary>
public class CurrencyExchangeRateViewModel
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string FromCurrency { get; set; }  // "CAD"
    public string ToCurrency { get; set; }  // "USD"
    public decimal Rate { get; set; }  // Conversion multiplier
}
```

#### DownloadInvoiceModel
```csharp
/// <summary>
/// Request to download invoice PDF(s)
/// </summary>
public class DownloadInvoiceModel
{
    public List<long> InvoiceIds { get; set; }  // Multiple invoices → ZIP file
    public bool IncludeAttachments { get; set; }
    public string Format { get; set; }  // "PDF", "Excel"
}
```

#### StartEndDateViewModel
```csharp
/// <summary>
/// Date range selector
/// </summary>
public class StartEndDateViewModel
{
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Typical Usage in Controllers

#### Invoice List with Filtering
```csharp
[HttpGet]
public ActionResult InvoiceList(InvoiceListFilter filter, int page = 1, int pageSize = 20)
{
    var model = _invoiceService.GetInvoiceList(filter, page, pageSize);
    return View(model);  // InvoiceListModel
}
```

#### Invoice Details Display
```csharp
[HttpGet]
public ActionResult InvoiceDetails(long id)
{
    var model = _invoiceService.GetInvoiceDetails(id);
    return View(model);  // InvoiceDetailsViewModel
}
```

#### Create Invoice (Form POST)
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult CreateInvoice(InvoiceEditModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    var invoice = _invoiceService.Create(model);
    return RedirectToAction("InvoiceDetails", new { id = invoice.Id });
}
```

#### Process Payment (Form POST)
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult ProcessCardPayment(ChargeCardPaymentEditModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    var response = _paymentService.MakePaymentByNewChargeCard(
        model, 
        model.FranchiseeId, 
        model.InvoiceId
    );
    
    if (response.ProcessorResult == ProcessorResponseResult.Success)
    {
        TempData["Success"] = "Payment approved!";
        return RedirectToAction("PaymentReceipt", new { id = response.InstrumentId });
    }
    else
    {
        ModelState.AddModelError("", response.Message);
        return View(model);
    }
}
```

#### API Endpoint (JSON)
```csharp
[HttpPost]
[Route("api/invoices")]
public IHttpActionResult CreateInvoice([FromBody] InvoiceEditModel model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    try
    {
        var invoice = _invoiceService.Create(model);
        var viewModel = _mapper.Map<InvoiceViewModel>(invoice);
        return CreatedAtRoute("GetInvoice", new { id = invoice.Id }, viewModel);
    }
    catch (Exception ex)
    {
        return InternalServerError(ex);
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

**Internal**:
- [Domain entities](../Domain/.context/CONTEXT.md) — ViewModels map to/from domain entities
- [Enums](../Enum/.context/CONTEXT.md) — StatusId, ItemTypeId, InstrumentTypeId reference enums
- [Core.Application.ViewModel](../../../Application/ViewModel/.context/CONTEXT.md) — Base classes (`EditModelBase`)
- [Core.Geo.ViewModel](../../../Geo/ViewModel/.context/CONTEXT.md) — `AddressViewModel`, `PhoneEditModel`
- [Core.Organizations.ViewModel](../../../Organizations/ViewModel/.context/CONTEXT.md) — `FranchiseeViewModel`, `EmailEditModel`
- [Core.Sales.ViewModel](../../../Sales/ViewModel/.context/CONTEXT.md) — `SalesDataUploadViewModel`

**External**:
- `System.ComponentModel.DataAnnotations` — Validation attributes (`[Required]`, `[Range]`, `[CreditCard]`)
- ASP.NET MVC/Web API — Model binding and validation
- `Core.Application.Attribute.NoValidatorRequired` — Marks ViewModels that skip FluentValidation

**Validation**:
- FluentValidation (optional) — Custom validators for complex business rules
- ASP.NET Data Annotations — Built-in validation attributes
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### ViewModel Design Patterns

#### EditModel vs ViewModel Naming
```csharp
// ❌ Inconsistent naming
public class InvoiceModel { }  // Ambiguous - for input or output?

// ✅ Clear naming convention
public class InvoiceEditModel { }  // Input: Create/update form
public class InvoiceViewModel { }  // Output: Display/API response
public class InvoiceDetailsViewModel { }  // Output: Full details
public class InvoiceListModel { }  // Output: Collection with metadata
```

#### Flattening Complex Domain Graphs
```csharp
// Domain model (complex graph)
var invoice = _invoiceRepository.Get(id);
var franchisee = invoice.FranchiseeInvoice.Franchisee;
var customer = franchisee.Customer;
var address = customer.Address;

// ViewModel (flattened)
var viewModel = new InvoiceDetailsViewModel
{
    InvoiceId = invoice.Id,
    FranchiseeName = invoice.FranchiseeInvoice.Franchisee.Name,
    Customer = invoice.FranchiseeInvoice.Franchisee.Customer.Name,
    Address = _mapper.Map<AddressViewModel>(invoice.FranchiseeInvoice.Franchisee.Customer.Address),
    TotalAmount = invoice.InvoiceItems.Sum(i => i.Amount),
    InvoiceItems = _mapper.Map<List<InvoiceItemEditModel>>(invoice.InvoiceItems)
};

// Benefits:
// - Single object to serialize (no lazy loading issues)
// - No circular references
// - Client gets exactly what it needs
```

#### Validation Strategy
```csharp
// Basic validation via Data Annotations
public class ChargeCardEditModel
{
    [Required(ErrorMessage = "Card number is required")]
    [CreditCard(ErrorMessage = "Invalid card number")]
    public string Number { get; set; }
    
    [Required]
    [Range(1, 12, ErrorMessage = "Month must be 1-12")]
    public string ExpireMonth { get; set; }
}

// Complex validation via FluentValidation
public class ChargeCardEditModelValidator : AbstractValidator<ChargeCardEditModel>
{
    public ChargeCardEditModelValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Card number required")
            .CreditCard().WithMessage("Invalid card number")
            .Must(BeValidLuhn).WithMessage("Card number failed checksum");
        
        RuleFor(x => x.ExpireYear)
            .Must((model, year) => BeNotExpired(model.ExpireMonth, year))
            .WithMessage("Card has expired");
    }
    
    private bool BeValidLuhn(string cardNumber)
    {
        // Luhn algorithm implementation
    }
    
    private bool BeNotExpired(string month, string year)
    {
        var expiry = new DateTime(int.Parse(year), int.Parse(month), 1).AddMonths(1).AddDays(-1);
        return expiry >= DateTime.Today;
    }
}
```

### Known Issues & Best Practices

#### 1. PCI Compliance Warning ⚠️
```csharp
// ❌ DANGEROUS: Full card number in ViewModel
public class ChargeCardEditModel
{
    public string Number { get; set; }  // Accepts full PAN
}

// Problem: Full card number exists in memory/logs/request dumps
// Solution: Use tokenization BEFORE creating ViewModel

// ✅ SECURE: Token-based approach
public class ChargeCardEditModel
{
    public string Token { get; set; }  // Gateway token (one-time use)
    public string LastFour { get; set; }  // Display only
}

// Client-side tokenization (e.g., Stripe.js, Authorize.Net Accept.js)
// 1. Client submits card to gateway directly
// 2. Gateway returns token
// 3. Client submits token to server (not card number)
```

#### 2. Circular Reference in Serialization
```csharp
// Problem: Invoice → InvoiceItems → Invoice (circular reference)
public class InvoiceDetailsViewModel
{
    public ICollection<InvoiceItemEditModel> InvoiceItems { get; set; }
}

public class InvoiceItemEditModel
{
    public long InvoiceId { get; set; }
    public InvoiceDetailsViewModel Invoice { get; set; }  // ❌ Circular!
}

// Solution: Remove navigation property from child ViewModel
public class InvoiceItemEditModel
{
    public long InvoiceId { get; set; }  // ✅ Just the ID, not the object
}
```

#### 3. Overfetching Data
```csharp
// ❌ BAD: Return full InvoiceDetailsViewModel for list
public List<InvoiceDetailsViewModel> GetAllInvoices()
{
    // Includes all items, payments, addresses, etc. per invoice
    // Huge payload for simple list view
}

// ✅ GOOD: Use lightweight InvoiceViewModel for lists
public List<InvoiceViewModel> GetAllInvoices()
{
    // Only essential fields: ID, number, date, total, status
}
```

#### 4. Missing Validation Attributes
```csharp
// ❌ NO VALIDATION
public class ChargeCardPaymentEditModel
{
    public decimal Amount { get; set; }  // Could be negative!
    public string Number { get; set; }  // Could be empty!
}

// ✅ WITH VALIDATION
public class ChargeCardPaymentEditModel
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }
    
    [Required]
    [CreditCard]
    public string Number { get; set; }
}
```

#### 5. Inconsistent Null Handling
```csharp
// InvoiceEditModel has:
public long? InvoiceId { get; set; }  // Nullable
public long StatusId { get; set; }  // Not nullable

// InvoiceDetailsViewModel has:
public long? InvoiceId { get; set; }  // Nullable

// Recommendation: Document nullability semantics
// - Nullable = Optional/Legacy/Might not exist
// - Not nullable = Always present/Required
```

### Mapping Recommendations

#### AutoMapper Configuration
```csharp
// Create mapping profiles
public class BillingMappingProfile : Profile
{
    public BillingMappingProfile()
    {
        // Domain → ViewModel
        CreateMap<Invoice, InvoiceDetailsViewModel>()
            .ForMember(dest => dest.TotalAmount, 
                opt => opt.MapFrom(src => src.InvoiceItems.Sum(i => i.Amount)))
            .ForMember(dest => dest.TotalPayment,
                opt => opt.MapFrom(src => src.InvoicePayments.Sum(ip => ip.Amount)))
            .ForMember(dest => dest.GrandTotal,
                opt => opt.MapFrom(src => src.InvoiceItems.Sum(i => i.Amount) - src.InvoicePayments.Sum(ip => ip.Amount)));
        
        CreateMap<Invoice, InvoiceViewModel>()
            .ForMember(dest => dest.StatusName,
                opt => opt.MapFrom(src => ((InvoiceStatus)src.StatusId).ToString()));
        
        // ViewModel → Domain
        CreateMap<InvoiceEditModel, Invoice>()
            .ForMember(dest => dest.InvoiceItems, opt => opt.Ignore());  // Handle separately
    }
}

// Usage in service
public InvoiceDetailsViewModel GetInvoiceDetails(long id)
{
    var invoice = _invoiceRepository.Get(id);
    return _mapper.Map<InvoiceDetailsViewModel>(invoice);
}
```

#### Manual Mapping (When AutoMapper Isn't Enough)
```csharp
public InvoiceDetailsViewModel BuildInvoiceDetails(Invoice invoice)
{
    var franchiseeInvoice = invoice.FranchiseeInvoices.FirstOrDefault();
    var franchisee = franchiseeInvoice?.Franchisee;
    
    return new InvoiceDetailsViewModel
    {
        InvoiceId = invoice.Id,
        QBInvoiceNumber = invoice.CustomerQbInvoiceIdString,
        GeneratedOn = invoice.GeneratedOn,
        DueDate = invoice.DueDate,
        StatusId = invoice.StatusId,
        
        // Aggregations
        TotalAmount = invoice.InvoiceItems.Sum(i => i.Amount),
        TotalPayment = invoice.InvoicePayments.Sum(ip => ip.Amount),
        GrandTotal = invoice.InvoiceItems.Sum(i => i.Amount) - invoice.InvoicePayments.Sum(ip => ip.Amount),
        
        // Related entities
        FranchiseeName = franchisee?.Name,
        FranchiseePhone = franchisee?.Phone,
        Customer = franchisee?.Customer?.Name,
        Address = franchisee?.Customer?.Address != null 
            ? _mapper.Map<AddressViewModel>(franchisee.Customer.Address) 
            : null,
        
        // Collections
        InvoiceItems = invoice.InvoiceItems.Select(i => _mapper.Map<InvoiceItemEditModel>(i)).ToList(),
        Payments = invoice.InvoicePayments.Select(ip => new FranchiseeSalesPaymentEditModel
        {
            Amount = ip.Amount,
            Date = ip.Payment.Date,
            InstrumentType = ((InstrumentType)ip.Payment.InstrumentTypeId).ToString()
        }).ToList()
    };
}
```
<!-- END CUSTOM SECTION -->
