<!-- AUTO-GENERATED: Header -->
# Core
> Domain models, business logic, and service contracts for MarbleLife franchise management platform
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Core module is the **heart of the application's business logic**. Think of it as a library that defines "what the system does" without worrying about "how it's stored" (that's ORM) or "how users interact with it" (that's API/Web.UI).

**Analogy**: If MarbleLife were a restaurant, Core would be the recipe book and kitchen rules. It knows how to create an invoice, process a payment, schedule a job, and manage franchisees—but it doesn't know about databases, HTTP requests, or user interfaces.

**Why it's organized this way**:
- **15 Business Domains**: Each subfolder (Billing, Organizations, Sales, Users, etc.) represents a distinct business capability
- **Clear Boundaries**: A change to billing logic doesn't affect job scheduling logic
- **Testable**: All business rules are in plain C# classes, easy to unit test without databases or web servers
- **Reusable**: Services can be used by API controllers, background jobs, console apps, or future mobile apps

**Key Concepts**:
- **Domain Entities** (`DomainBase`): Business objects like `Invoice`, `Customer`, `Franchisee`
- **Services** (`I*Service`): Business operations like "Create Invoice", "Process Payment"
- **Factories** (`I*Factory`): Object creation and ViewModel ↔ Domain mapping
- **Validators** (`AbstractValidator<T>`): Input validation rules
- **ViewModels**: Data contracts for API/UI (DTOs with validation attributes)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
Core is a class library consumed by other projects. You typically interact with it through dependency injection in API controllers or services.

```csharp
// In your controller/service (via DI)
private readonly IInvoiceService _invoiceService;
private readonly IInvoiceFactory _invoiceFactory;
private readonly IValidator<InvoiceViewModel> _invoiceValidator;

public InvoiceController(
    IInvoiceService invoiceService,
    IInvoiceFactory invoiceFactory,
    IValidator<InvoiceViewModel> invoiceValidator)
{
    _invoiceService = invoiceService;
    _invoiceFactory = invoiceFactory;
    _invoiceValidator = invoiceValidator;
}
```

### Example: Create an invoice with validation
```csharp
[HttpPost]
public ActionResult CreateInvoice(InvoiceViewModel model)
{
    // 1. Validate input
    var validationResult = _invoiceValidator.Validate(model);
    if (!validationResult.IsValid)
    {
        return BadRequest(validationResult.Errors);
    }
    
    // 2. Convert ViewModel to Domain entity
    var invoice = _invoiceFactory.CreateDomain(model);
    
    // 3. Execute business logic
    var createdInvoice = _invoiceService.Create(invoice);
    
    // 4. Convert back to ViewModel for response
    var responseModel = _invoiceFactory.CreateViewModel(createdInvoice);
    
    return Ok(responseModel);
}
```

### Example: Query with business logic
```csharp
public ActionResult GetOverdueInvoices(long franchiseeId)
{
    // Service encapsulates complex query logic
    var overdueInvoices = _invoiceService.GetOverdueInvoices(franchiseeId);
    
    // Factory handles collection mapping
    var viewModels = overdueInvoices.Select(i => _invoiceFactory.CreateViewModel(i));
    
    return Ok(viewModels);
}
```

### Example: Complex validation with composition
```csharp
// UserEditModelValidator composes smaller validators
public class UserEditModelValidator : AbstractValidator<UserEditModel>
{
    public UserEditModelValidator(
        IValidator<UserLoginEditModel> loginValidator,
        IValidator<PersonEditModel> personValidator,
        IUniqueEmailValidator uniqueEmailValidator)
    {
        // Delegate validation to specialized validators
        RuleFor(x => x.UserLoginEditModel).SetValidator(loginValidator);
        RuleFor(x => x.PersonEditModel).SetValidator(personValidator);
        
        // Custom business rule
        RuleFor(x => x.PersonEditModel.Email)
            .SetValidator(uniqueEmailValidator)
            .WithMessage("Email address is already in use");
    }
}
```

### Example: Factory for complex object creation
```csharp
// Factory handles mapping complexity
public class InvoiceFactory : IInvoiceFactory
{
    public Invoice CreateDomain(InvoiceViewModel viewModel)
    {
        var invoice = new Invoice
        {
            IsNew = true, // IMPORTANT: Set for audit tracking
            FranchiseeId = viewModel.FranchiseeId,
            InvoiceDate = viewModel.InvoiceDate,
            DueDate = viewModel.InvoiceDate.AddDays(30),
            Status = InvoiceStatus.Pending,
            Items = viewModel.Items.Select(i => new InvoiceItem
            {
                IsNew = true,
                Description = i.Description,
                Amount = i.Amount,
                Quantity = i.Quantity
            }).ToList()
        };
        
        invoice.Total = invoice.Items.Sum(i => i.Amount * i.Quantity);
        return invoice;
    }
    
    public InvoiceViewModel CreateViewModel(Invoice domain)
    {
        return new InvoiceViewModel
        {
            Id = domain.Id,
            InvoiceNumber = domain.InvoiceNumber,
            FranchiseeId = domain.FranchiseeId,
            InvoiceDate = domain.InvoiceDate,
            DueDate = domain.DueDate,
            Total = domain.Total,
            Status = domain.Status.ToString(),
            Items = domain.Items.Select(i => new InvoiceItemViewModel
            {
                Description = i.Description,
                Amount = i.Amount,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Module | Key Services | Key Factories | Purpose |
|--------|-------------|---------------|---------|
| **Billing** | `IInvoiceService`, `IPaymentService`, `IChargeCardService` | `IInvoiceFactory`, `IPaymentFactory` | Process payments, manage invoices |
| **Organizations** | `IFranchiseeService`, `IOrganizationService`, `IFeeProfileService` | `IFranchiseeFactory`, `IOrganizationFactory` | Manage franchisees and organizational structure |
| **Sales** | `ICustomerService`, `ISalesDataService`, `IEstimateInvoiceService` | `ICustomerFactory`, `IEstimateInvoiceFactory` | Track sales, manage customers, create estimates |
| **Users** | `IUserService`, `IPersonService`, `IAuthenticationService` | `IUserFactory`, `IPersonFactory` | User authentication, profile management |
| **Scheduler** | `IJobService`, `IJobSchedulerService`, `IWorkOrderService` | `IJobFactory`, `IJobSchedulerFactory` | Schedule jobs, manage technician work orders |
| **Notification** | `INotificationService`, `IEmailTemplateService` | `INotificationFactory`, `IEmailTemplateFactory` | Send emails, manage notification queue |
| **Reports** | Report factories for various financial/sales reports | Various report factories | Generate financial and sales reports |
| **Review** | `ICustomerFeedbackService`, `IFeedbackRequestService` | `ICustomerFeedbackFactory` | Manage customer reviews and feedback |
| **Geo** | `IAddressService`, `ICityService`, `IZipService` | `IAddressFactory` | Geocoding, address validation |
| **MarketingLead** | `IMarketingLeadService`, `ILeadConversionService` | `IMarketingLeadFactory` | Lead tracking and conversion |

For detailed documentation on each module, see the links in the CONTEXT.md file's "Submodule Documentation" table.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Problem: Audit fields (DateCreated, CreatedBy) are NULL after saving
**Solution**: Always set `IsNew = true` on new entities before saving. Factories should do this automatically in `CreateDomain()` methods.

```csharp
// Wrong
var invoice = new Invoice { FranchiseeId = 123 };
_invoiceService.Create(invoice); // DateCreated will be NULL

// Right - via factory
var invoice = _invoiceFactory.CreateDomain(viewModel); // Factory sets IsNew = true
_invoiceService.Create(invoice); // Audit fields populated correctly
```

### Problem: Validation not firing in controller
**Solution**: Ensure FluentValidation is registered in MVC pipeline and validators are injected correctly.

```csharp
// In DependencyInjection module registration
container.RegisterType<IValidator<InvoiceViewModel>, InvoiceViewModelValidator>();

// In controller - inject validator
private readonly IValidator<InvoiceViewModel> _validator;

// In action method - manually validate
var validationResult = _validator.Validate(model);
if (!validationResult.IsValid)
{
    return BadRequest(validationResult.Errors);
}
```

### Problem: Circular dependency between services
**Solution**: Extract shared logic into a separate service, or use events/messaging to decouple modules.

```csharp
// Bad - circular dependency
IInvoiceService depends on IPaymentService
IPaymentService depends on IInvoiceService

// Good - extract shared logic
IInvoiceService depends on IFinancialCalculatorService
IPaymentService depends on IFinancialCalculatorService
```

### Problem: ViewModel has more properties than needed, over-posting risk
**Solution**: Create separate ViewModels for Create/Edit/Display scenarios.

```csharp
// Instead of one InvoiceViewModel for all scenarios
public class InvoiceCreateViewModel { /* only writable properties */ }
public class InvoiceEditViewModel { /* writable + Id */ }
public class InvoiceDisplayViewModel { /* all properties including computed */ }
```

### Problem: Factory mapping is tedious and error-prone
**Solution**: Consider using AutoMapper or a mapping convention for simple cases, but keep manual mapping for complex business rules.

```csharp
// Simple cases - AutoMapper config
config.CreateMap<Invoice, InvoiceViewModel>();

// Complex cases - manual factory
public InvoiceViewModel CreateViewModel(Invoice domain)
{
    // Complex logic: calculate totals, format dates, include related data
    return new InvoiceViewModel { /* ... */ };
}
```
<!-- END CUSTOM SECTION -->
