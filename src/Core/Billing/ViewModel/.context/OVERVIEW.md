<!-- AUTO-GENERATED: Header -->
# Billing ViewModels (DTOs)
> Data Transfer Objects for API contracts, forms, and display models
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **ViewModel** module contains 26 Data Transfer Objects that serve as the communication layer between the billing system and external consumers (web browsers, mobile apps, API clients). These DTOs provide clean, validated, presentation-friendly representations of complex billing data without exposing internal domain implementation details.

**Why it exists**: ViewModels decouple your domain model from your API contract. This means you can change internal database schema without breaking client applications, optimize API payloads for specific use cases (list vs detail views), and add client-specific fields (formatting, calculations) without polluting domain entities.

**Real-world analogy**: Think of ViewModels like restaurant menus. The kitchen (domain layer) has complex recipes and raw ingredients, but the menu (ViewModel) shows customers simple descriptions, prices, and photos. `InvoiceDetailsViewModel` is the full menu item description with ingredients and nutritional info. `InvoiceViewModel` is the abbreviated menu board listing.

### Key Concepts

1. **Input vs Output**: `*EditModel` for forms/API requests, `*ViewModel` for display/responses
2. **Flattening**: Complex domain graphs compressed into single objects (no N+1 lazy loading)
3. **Validation**: Decorated with `[Required]`, `[Range]`, `[CreditCard]` for automatic validation
4. **Immutability**: Output ViewModels are read-only snapshots (no business logic)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
ViewModels are used in controllers, API endpoints, and services:

```csharp
using Core.Billing.ViewModel;
using System.Web.Mvc;

public class InvoiceController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IMapper _mapper;
    
    public InvoiceController(IInvoiceService invoiceService, IMapper mapper)
    {
        _invoiceService = invoiceService;
        _mapper = mapper;
    }
}
```

### Example: Display Invoice List
```csharp
[HttpGet]
public ActionResult InvoiceList(InvoiceListFilter filter, int page = 1)
{
    // Service returns InvoiceListModel (ViewModel)
    var model = _invoiceService.GetInvoiceList(filter, page, pageSize: 20);
    
    // Model contains:
    // - Collection: List<InvoiceViewModel> (paginated invoices)
    // - Filter: Applied search criteria
    // - PagingModel: Page number, total count, etc.
    // - TotalUnPaidAmount: Sum of outstanding balances
    
    return View(model);
}

// View (Razor)
@model Core.Billing.ViewModel.InvoiceListModel

<h2>Invoices for @Model.FranchiseeName</h2>
<p>Total Unpaid: $@Model.TotalUnPaidAmount.Value.ToString("N2")</p>

<table>
    <thead>
        <tr>
            <th>Invoice #</th>
            <th>Date</th>
            <th>Amount</th>
            <th>Status</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var invoice in Model.Collection)
        {
            <tr>
                <td>@invoice.InvoiceNumber</td>
                <td>@invoice.GeneratedOn.ToShortDateString()</td>
                <td>$@invoice.TotalAmount.ToString("N2")</td>
                <td><span class="badge-@invoice.StatusName.ToLower()">@invoice.StatusName</span></td>
            </tr>
        }
    </tbody>
</table>

<div class="pagination">
    @Html.PagedListPager(Model.PagingModel)
</div>
```

### Example: Create Invoice Form
```csharp
[HttpGet]
public ActionResult CreateInvoice(long franchiseeId)
{
    var model = new InvoiceEditModel
    {
        GeneratedOn = DateTime.Today,
        DueDate = DateTime.Today.AddDays(30),
        InvoiceItems = new List<InvoiceItemEditModel>()
    };
    
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult CreateInvoice(InvoiceEditModel model)
{
    // ASP.NET automatically validates [Required], [Range], etc.
    if (!ModelState.IsValid)
    {
        return View(model);  // Return with validation errors
    }
    
    try
    {
        // Service converts EditModel → Domain entity
        var invoice = _invoiceService.Create(model);
        
        TempData["Success"] = $"Invoice #{invoice.Id} created successfully";
        return RedirectToAction("InvoiceDetails", new { id = invoice.Id });
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", ex.Message);
        return View(model);
    }
}

// View (Razor)
@model Core.Billing.ViewModel.InvoiceEditModel

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    
    <div class="form-group">
        @Html.LabelFor(m => m.GeneratedOn)
        @Html.TextBoxFor(m => m.GeneratedOn, new { @class = "form-control datepicker" })
        @Html.ValidationMessageFor(m => m.GeneratedOn)
    </div>
    
    <div class="form-group">
        @Html.LabelFor(m => m.DueDate)
        @Html.TextBoxFor(m => m.DueDate, new { @class = "form-control datepicker" })
        @Html.ValidationMessageFor(m => m.DueDate)
    </div>
    
    <h4>Line Items</h4>
    <div id="invoice-items">
        @for (int i = 0; i < Model.InvoiceItems.Count; i++)
        {
            @Html.Partial("_InvoiceItemRow", Model.InvoiceItems[i], new ViewDataDictionary { { "Index", i } })
        }
    </div>
    
    <button type="button" id="add-item">Add Line Item</button>
    <button type="submit" class="btn btn-primary">Create Invoice</button>
}
```

### Example: Process Credit Card Payment
```csharp
[HttpGet]
public ActionResult PayInvoice(long invoiceId)
{
    var invoice = _invoiceService.Get(invoiceId);
    var balance = invoice.InvoiceItems.Sum(i => i.Amount) 
                - invoice.InvoicePayments.Sum(ip => ip.Amount);
    
    var model = new ChargeCardPaymentEditModel
    {
        InvoiceId = invoiceId,
        Amount = balance,
        ChargeCardEditModel = new ChargeCardEditModel()
    };
    
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult PayInvoice(ChargeCardPaymentEditModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    // Process payment through service
    var response = _paymentService.MakePaymentByNewChargeCard(
        model, 
        franchiseeId: GetCurrentFranchiseeId(), 
        invoiceId: model.InvoiceId
    );
    
    if (response.ProcessorResult == ProcessorResponseResult.Success)
    {
        TempData["Success"] = $"Payment of ${model.Amount:N2} approved! Transaction ID: {response.RawResponse}";
        return RedirectToAction("PaymentReceipt", new { paymentId = response.InstrumentId });
    }
    else
    {
        ModelState.AddModelError("", response.Message);
        return View(model);
    }
}

// View (Razor)
@model Core.Billing.ViewModel.ChargeCardPaymentEditModel

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    @Html.HiddenFor(m => m.InvoiceId)
    @Html.HiddenFor(m => m.Amount)
    
    <h3>Pay $@Model.Amount.ToString("N2")</h3>
    
    <div class="form-group">
        @Html.LabelFor(m => m.ChargeCardEditModel.Number, "Card Number")
        @Html.TextBoxFor(m => m.ChargeCardEditModel.Number, new { @class = "form-control", placeholder = "1234 5678 9012 3456" })
        @Html.ValidationMessageFor(m => m.ChargeCardEditModel.Number)
    </div>
    
    <div class="form-group">
        @Html.LabelFor(m => m.ChargeCardEditModel.Name, "Name on Card")
        @Html.TextBoxFor(m => m.ChargeCardEditModel.Name, new { @class = "form-control" })
        @Html.ValidationMessageFor(m => m.ChargeCardEditModel.Name)
    </div>
    
    <div class="row">
        <div class="col-md-4">
            @Html.LabelFor(m => m.ChargeCardEditModel.ExpireMonth, "Month")
            @Html.TextBoxFor(m => m.ChargeCardEditModel.ExpireMonth, new { @class = "form-control", placeholder = "MM" })
        </div>
        <div class="col-md-4">
            @Html.LabelFor(m => m.ChargeCardEditModel.ExpireYear, "Year")
            @Html.TextBoxFor(m => m.ChargeCardEditModel.ExpireYear, new { @class = "form-control", placeholder = "YYYY" })
        </div>
        <div class="col-md-4">
            @Html.LabelFor(m => m.ChargeCardEditModel.Cvv, "CVV")
            @Html.TextBoxFor(m => m.ChargeCardEditModel.Cvv, new { @class = "form-control", placeholder = "123" })
        </div>
    </div>
    
    <div class="checkbox">
        @Html.CheckBoxFor(m => m.ChargeCardEditModel.SaveProfile)
        @Html.LabelFor(m => m.ChargeCardEditModel.SaveProfile, "Save card for future use")
    </div>
    
    <button type="submit" class="btn btn-primary">Pay $@Model.Amount.ToString("N2")</button>
}
```

### Example: API Endpoint (JSON)
```csharp
[HttpGet]
[Route("api/invoices/{id}")]
public IHttpActionResult GetInvoice(long id)
{
    var invoice = _invoiceService.Get(id);
    
    if (invoice == null)
    {
        return NotFound();
    }
    
    // Map domain entity → ViewModel
    var viewModel = _mapper.Map<InvoiceDetailsViewModel>(invoice);
    
    return Ok(viewModel);
}

// Response JSON:
{
    "invoiceId": 12345,
    "qbInvoiceNumber": "INV-2025-001",
    "franchiseeName": "MarbleLife Houston",
    "customer": "ABC Corporation",
    "totalAmount": 1500.00,
    "totalPayment": 500.00,
    "grandTotal": 1000.00,
    "generatedOn": "2025-02-01T00:00:00Z",
    "dueDate": "2025-03-01T00:00:00Z",
    "statusId": 82,
    "currencyCode": "USD",
    "invoiceItems": [
        {
            "description": "Grout Restoration - 5%",
            "quantity": 1,
            "rate": 1000.00,
            "amount": 1000.00
        },
        {
            "description": "Advertising Fund - 2%",
            "quantity": 1,
            "rate": 500.00,
            "amount": 500.00
        }
    ]
}
```

### Example: API Create Invoice
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
    catch (ValidationException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.Error("Invoice creation failed", ex);
        return InternalServerError(ex);
    }
}

// Request JSON:
{
    "generatedOn": "2025-02-10T00:00:00Z",
    "dueDate": "2025-03-10T00:00:00Z",
    "statusId": 82,
    "invoiceItems": [
        {
            "itemTypeId": 92,
            "description": "Royalty Fee - 5%",
            "quantity": 1,
            "rate": 1000.00,
            "amount": 1000.00
        }
    ]
}

// Response: 201 Created
Location: /api/invoices/12346
{
    "id": 12346,
    "invoiceNumber": "INV-2025-002",
    "generatedOn": "2025-02-10T00:00:00Z",
    "totalAmount": 1000.00,
    "statusId": 82,
    "statusName": "Unpaid"
}
```

### Example: Search/Filter Invoices
```csharp
[HttpGet]
[Route("api/invoices")]
public IHttpActionResult SearchInvoices([FromUri] InvoiceListFilter filter, int page = 1, int pageSize = 20)
{
    var model = _invoiceService.GetInvoiceList(filter, page, pageSize);
    
    return Ok(new
    {
        data = model.Collection,
        page = model.PagingModel.CurrentPage,
        pageSize = model.PagingModel.PageSize,
        totalCount = model.PagingModel.TotalCount,
        totalPages = model.PagingModel.TotalPages,
        totalUnpaid = model.TotalUnPaidAmount
    });
}

// Request: GET /api/invoices?statusId=82&startDate=2025-01-01&endDate=2025-01-31&page=1&pageSize=20
// Response:
{
    "data": [
        { "id": 123, "invoiceNumber": "INV-001", "totalAmount": 1500.00, ... },
        { "id": 124, "invoiceNumber": "INV-002", "totalAmount": 2000.00, ... }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3,
    "totalUnpaid": 67500.00
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Input Models (Forms/API Requests)

| Model | Purpose | Key Fields |
|-------|---------|------------|
| `InvoiceEditModel` | Create/update invoice | `GeneratedOn`, `DueDate`, `InvoiceItems[]` |
| `InvoiceItemEditModel` | Invoice line item | `ItemTypeId`, `Description`, `Amount` |
| `ChargeCardPaymentEditModel` | Credit card payment | `ChargeCardEditModel`, `Amount`, `InvoiceId` |
| `ChargeCardEditModel` | Card details | `Number`, `Name`, `ExpireMonth`, `ExpireYear`, `Cvv` |
| `ECheckEditModel` | eCheck details | `RoutingNumber`, `AccountNumber`, `AccountTypeId` |
| `CheckPaymentEditModel` | Paper check entry | `CheckNumber`, `BankName`, `Amount` |
| `EPaymentEditModel` | Base electronic payment | `Amount`, `InvoiceId`, `FranchiseeId` |
| `InvoiceListFilter` | Search criteria | `StatusId`, `StartDate`, `EndDate`, `FranchiseeId` |

### Output Models (Display/API Response)

| Model | Purpose | Key Fields |
|-------|---------|------------|
| `InvoiceDetailsViewModel` | Full invoice details | `InvoiceId`, `TotalAmount`, `InvoiceItems[]`, `Payments[]`, `Address` |
| `InvoiceViewModel` | Invoice summary | `Id`, `InvoiceNumber`, `TotalAmount`, `StatusName` |
| `InvoiceListModel` | Paginated collection | `Collection[]`, `Filter`, `PagingModel`, `TotalUnPaidAmount` |
| `PaymentModeDetailViewModel` | Payment details | `PaymentDate`, `Amount`, `TransactionId`, `CardLastFour` |
| `FranchiseePaymentInstrumentViewModel` | Saved payment methods | `ProfileId`, `InstrumentType`, `CardLastFour`, `IsDefault` |
| `CurrencyExchangeRateViewModel` | Exchange rate | `Date`, `FromCurrency`, `ToCurrency`, `Rate` |

### Integration Models

| Model | Purpose | Key Fields |
|-------|---------|------------|
| `ProcessorResponse` | Gateway response | `ProcessorResult`, `Message`, `RawResponse`, `InstrumentId` |
| `DownloadInvoiceModel` | PDF download request | `InvoiceIds[]`, `Format` |
| `StartEndDateViewModel` | Date range picker | `StartDate`, `EndDate` |

### Common Validation Attributes

| Attribute | Usage |
|-----------|-------|
| `[Required]` | Field cannot be empty/null |
| `[Range(min, max)]` | Numeric value within range |
| `[CreditCard]` | Valid credit card number (Luhn check) |
| `[StringLength(max)]` | String max length |
| `[RegularExpression(pattern)]` | Regex pattern match |
| `[EmailAddress]` | Valid email format |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Model validation fails with no error message
**Symptom**: `ModelState.IsValid == false` but no specific error
**Cause**: Missing validation attributes or incorrect property names
**Solution**:
```csharp
if (!ModelState.IsValid)
{
    // Debug: Log all validation errors
    var errors = ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage);
    
    _logger.Error($"Validation failed: {string.Join(", ", errors)}");
    return BadRequest(ModelState);
}
```

### Issue: JSON deserialization error "Cannot convert null to non-nullable type"
**Symptom**: API POST fails with "The InvoiceId field is required"
**Cause**: Client sends `null` for non-nullable property
**Solution**:
```csharp
// ❌ Non-nullable long (error if client sends null)
public class InvoiceEditModel
{
    public long StatusId { get; set; }  // Error if missing from JSON
}

// ✅ Make optional with default
public class InvoiceEditModel
{
    public long StatusId { get; set; } = (long)InvoiceStatus.Unpaid;  // Default value
}

// Or nullable with validation
public class InvoiceEditModel
{
    [Required]
    public long? StatusId { get; set; }  // Required but nullable
}
```

### Issue: Credit card validation passes for invalid numbers
**Symptom**: `[CreditCard]` attribute accepts invalid numbers
**Cause**: Only performs Luhn check, doesn't validate IIN ranges
**Solution**:
```csharp
// Use FluentValidation for stronger checks
public class ChargeCardEditModelValidator : AbstractValidator<ChargeCardEditModel>
{
    public ChargeCardEditModelValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .CreditCard()  // Luhn check
            .Must(BeValidCardType).WithMessage("Card type not supported");
    }
    
    private bool BeValidCardType(string number)
    {
        if (string.IsNullOrEmpty(number)) return false;
        
        // Check IIN (Issuer Identification Number)
        if (number.StartsWith("4")) return true;  // Visa
        if (number.StartsWith("5")) return true;  // MasterCard
        if (number.StartsWith("34") || number.StartsWith("37")) return true;  // Amex
        if (number.StartsWith("6011") || number.StartsWith("65")) return true;  // Discover
        
        return false;
    }
}
```

### Issue: Circular reference during JSON serialization
**Symptom**: "Self referencing loop detected for property 'Invoice'"
**Cause**: Navigation properties create circular references
**Solution**:
```csharp
// Option 1: Configure JSON serializer to ignore loops
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling 
            = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    }
}

// Option 2: Remove navigation properties from ViewModels
public class InvoiceItemEditModel
{
    public long InvoiceId { get; set; }  // ✅ Just ID
    // public InvoiceEditModel Invoice { get; set; }  // ❌ Remove this
}

// Option 3: Use [JsonIgnore] attribute
public class InvoiceItemEditModel
{
    [JsonIgnore]
    public InvoiceEditModel Invoice { get; set; }
}
```

### Issue: Date/time timezone confusion
**Symptom**: Invoice dates off by several hours
**Cause**: Client sends local time, server interprets as UTC (or vice versa)
**Solution**:
```csharp
// Always use UTC on server
public class InvoiceEditModel
{
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
}

// In service layer, convert to UTC if needed
public Invoice Create(InvoiceEditModel model)
{
    var invoice = new Invoice
    {
        GeneratedOn = model.GeneratedOn.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(model.GeneratedOn, DateTimeKind.Utc)
            : model.GeneratedOn.ToUniversalTime(),
        DueDate = model.DueDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(model.DueDate, DateTimeKind.Utc)
            : model.DueDate.ToUniversalTime()
    };
    
    return invoice;
}

// Client-side: Always send ISO 8601 with timezone
// "2025-02-10T12:00:00Z" (UTC)
// "2025-02-10T07:00:00-05:00" (EST)
```

### Issue: Large payload size for list views
**Symptom**: `/api/invoices` endpoint slow, returns megabytes of data
**Cause**: Using `InvoiceDetailsViewModel` (includes items, payments, etc.) instead of lightweight `InvoiceViewModel`
**Solution**:
```csharp
// ❌ BAD: Heavy model for list
public IHttpActionResult GetInvoices()
{
    var invoices = _invoiceService.GetAll();
    var models = invoices.Select(i => _mapper.Map<InvoiceDetailsViewModel>(i));
    return Ok(models);  // Huge payload!
}

// ✅ GOOD: Lightweight model for list
public IHttpActionResult GetInvoices()
{
    var invoices = _invoiceService.GetAll();
    var models = invoices.Select(i => _mapper.Map<InvoiceViewModel>(i));
    return Ok(models);  // Only essential fields
}
```

### Issue: PCI compliance - card data in logs
**Symptom**: Security audit finds full card numbers in application logs
**Cause**: Logging ViewModel properties
**Solution**:
```csharp
// ❌ NEVER log full card data
_logger.Info($"Processing payment: {JsonConvert.SerializeObject(model)}");

// ✅ Mask sensitive data before logging
public class ChargeCardEditModel
{
    public string Number { get; set; }
    
    [JsonIgnore]
    public string MaskedNumber => string.IsNullOrEmpty(Number) 
        ? "" 
        : $"****{Number.Substring(Number.Length - 4)}";
}

_logger.Info($"Processing payment with card {model.ChargeCardEditModel.MaskedNumber}");

// Or use custom JSON converter
public class SensitiveDataJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is string str && str.Length > 4)
        {
            writer.WriteValue($"****{str.Substring(str.Length - 4)}");
        }
        else
        {
            writer.WriteValue("****");
        }
    }
}

public class ChargeCardEditModel
{
    [JsonConverter(typeof(SensitiveDataJsonConverter))]
    public string Number { get; set; }
}
```
<!-- END CUSTOM SECTION -->
