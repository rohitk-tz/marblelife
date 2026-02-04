# Infrastructure Module - AI Context

## Purpose

The **Infrastructure** module is the data access and external integration layer of the MarbleLife application. It provides concrete implementations of Core interfaces for repository pattern, database operations, logging, PDF generation, and payment processing integrations. This layer bridges the gap between business logic (Core) and physical data storage/external services (ORM, APIs).

## Architecture Overview

The Infrastructure module implements the **Repository Pattern** and **Unit of Work Pattern** using Entity Framework 6 with MySQL. It provides a clean separation between domain logic and data persistence, following the Dependency Inversion Principle (business logic depends on abstractions, not concrete implementations).

### Key Architectural Patterns

1. **Repository Pattern**: Generic data access through `IRepository<T>`
2. **Unit of Work Pattern**: Transaction management via `IUnitOfWork`
3. **Adapter Pattern**: Wraps external APIs (Authorize.Net, wkhtmltopdf)
4. **Service Layer Pattern**: Business service implementations for payment processing
5. **Dependency Injection**: All implementations use constructor injection
6. **Cascade Updates**: Custom attribute-based cascade save/delete logic

## Project Structure

```
Infrastructure/
├── Application/            # Core infrastructure implementations
│   └── Impl/              # Data access and utility implementations
├── Billing/               # Payment processing interfaces
│   └── Impl/              # Payment gateway integrations
├── Properties/            # Assembly metadata
├── packages.config        # NuGet dependencies
└── Infrastructure.csproj  # Project configuration
```

## Application/Impl - Core Infrastructure Layer

### UnitOfWork.cs

**Purpose**: Manages database context lifecycle and coordinates multiple repositories within a single transaction.

**Key Responsibilities**:
- Creates and manages `MakaluDbContext` instance
- Provides lazy-loaded `Repository<T>` instances
- Manages database transactions (ReadCommitted isolation level)
- Tracks transaction depth for nested operations
- Handles SaveChanges() and Rollback() operations

**Usage Pattern**:
```csharp
using (var unitOfWork = new UnitOfWork())
{
    var customerRepo = unitOfWork.Repository<Customer>();
    var invoiceRepo = unitOfWork.Repository<Invoice>();
    
    // Perform operations
    customerRepo.Insert(newCustomer);
    invoiceRepo.Update(existingInvoice);
    
    // Commit all changes in single transaction
    unitOfWork.SaveChanges();
}
```

**Transaction Management**:
- Automatically begins transaction on first repository access
- Uses `TransactionHelper` to track nested transaction count
- Supports rollback for error scenarios
- Cleans up resources properly via `Dispose()`

### Repository.cs

**Purpose**: Generic repository implementation providing CRUD operations and LINQ query support for all domain entities.

**Key Features**:
- **Query Methods**: `Table`, `TableNoTracking` (returns IQueryable for LINQ)
- **Read Operations**: `Get(id)`, `Get(expression)`, `Fetch(expression)`
- **Pagination**: `Fetch(expression, pageSize, pageNumber)`
- **Async Support**: `TableAsync(expression)`
- **Eager Loading**: `IncludeMultiple(params expressions)` for related entities
- **Count Operations**: `Count(expression)`
- **Write Operations**: `Insert`, `Update`, `Delete`

**Usage Examples**:
```csharp
// Query with LINQ
var activeCustomers = repository.Table
    .Where(c => c.IsActive && c.FranchiseeId == franchiseeId)
    .OrderBy(c => c.Name)
    .ToList();

// Paginated fetch with expression
var pagedCustomers = repository.Fetch(
    c => c.IsActive, 
    pageSize: 20, 
    pageNumber: 1
);

// Eager loading related entities
var customersWithAddresses = repository.IncludeMultiple(
    c => c.Address,
    c => c.Phone,
    c => c.Email
).Where(c => c.Id == customerId).FirstOrDefault();

// Update with cascade
repository.Update(customer); // Automatically handles cascade updates
```

**Cascade Update Logic** (`SaveCascadeHelper`):
- Processes entities marked with `[CascadeEntity]` attribute
- Handles collections (add/remove/modify child items)
- Recursively updates related entities
- Manages entity states (Added, Modified, Deleted)

### LogService.cs

**Purpose**: NLog-based logging implementation providing structured logging across the application.

**Features**:
- Wraps NLog Logger for consistent logging interface
- Supports all log levels (Trace, Debug, Info, Warn, Error, Fatal)
- Thread-safe logging operations
- Marked with `[DefaultImplementation]` for automatic DI registration

**Usage**:
```csharp
public class MyService
{
    private readonly ILogService _log;
    
    public MyService(ILogService log)
    {
        _log = log;
    }
    
    public void ProcessData()
    {
        _log.Info("Starting data processing");
        try
        {
            // Business logic
            _log.Debug("Processing step 1 complete");
        }
        catch (Exception ex)
        {
            _log.Error("Data processing failed", ex);
            throw;
        }
    }
}
```

### PdfGenerator.cs

**Purpose**: HTML-to-PDF conversion using wkhtmltopdf command-line tool.

**Features**:
- Supports HTML string or file path as input
- Configurable page size, margins, orientation
- Custom header/footer support
- Process-based execution with error handling

**Usage**:
```csharp
var pdfGenerator = new PdfGenerator(settings);
byte[] pdfBytes = pdfGenerator.Generate(
    htmlContent: invoiceHtml,
    header: "Invoice #12345",
    footer: "Page [page] of [toPage]"
);
```

### TransactionHelper.cs

**Purpose**: Manages transaction counting for nested transaction scenarios.

**Functionality**:
- Tracks depth of nested transactions
- Prevents premature commits in nested contexts
- Thread-safe operation tracking

### SaveCascadeHelper.cs

**Purpose**: Handles cascade saves for complex entity graphs marked with `[CascadeEntity]` attribute.

**Features**:
- Reflection-based property inspection
- Handles single entities and collections
- Manages entity state transitions (Added/Modified/Deleted)
- Recursive cascade processing

**Example Entity with Cascade**:
```csharp
public class Organization : DomainBase
{
    public string Name { get; set; }
    
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<Address> Addresses { get; set; }
    
    [CascadeEntity]
    public virtual DataRecorderMetaData MetaData { get; set; }
}
```

## Billing - Payment Processing Layer

### External Integration: Authorize.Net

The Infrastructure module integrates with **Authorize.Net Payment Gateway** for credit card and eCheck processing.

**SDK**: AuthorizeNet.dll v1.9.0

### IAuthorizeNetCustomerProfileService

**Purpose**: Manages customer payment profiles (credit cards, eChecks) in Authorize.Net vault.

**Key Operations**:
- `CreateCustomerProfile`: Create new customer profile
- `CreateCustomerPaymentProfile`: Add payment method to profile
- `UpdateCustomerPaymentProfile`: Update payment method details
- `DeleteCustomerPaymentProfile`: Remove payment method
- `GetCustomerProfile`: Retrieve profile details
- `ChargeCustomerProfile`: Charge existing payment method
- `ValidateCustomerPaymentProfile`: Test payment method validity

### Payment Service Implementations

#### ChargeCardPaymentService.cs

**Purpose**: Processes credit card payments through Authorize.Net.

**Features**:
- New card charges (one-time payments)
- Stored profile charges (repeat payments)
- Transaction logging and error handling
- Invoice association and payment recording

**Workflow**:
1. Validate payment information
2. Call Authorize.Net API (charge new card or charge profile)
3. Log transaction result
4. Create payment record in database
5. Update invoice status
6. Handle errors and return result

**Usage**:
```csharp
var result = chargeCardPaymentService.ChargeCard(
    franchiseeId: 123,
    customerId: 456,
    amount: 250.00m,
    cardNumber: "************1234",
    expirationDate: "12/25",
    cvv: "123",
    invoiceIds: new[] { 789, 790 }
);
```

#### ChargeCardProfileService.cs

**Purpose**: Manages credit card profiles for recurring/stored payments.

**Operations**:
- Create card profile with validation
- Update existing profiles
- Delete outdated profiles
- Retrieve profile information

#### ECheckPaymentService.cs

**Purpose**: Processes electronic check (ACH) payments.

**Features**:
- Bank account validation
- Routing number verification
- eCheck debit processing
- Transaction result logging

#### ECheckProfileService.cs

**Purpose**: Manages eCheck profiles for recurring ACH payments.

### ICurrencyRateService

**Purpose**: Manages currency exchange rates for multi-currency support.

**Operations**:
- Fetch current exchange rates from external API
- Store historical rate data
- Calculate conversions between currencies

## Dependencies

### Core Module
- `IRepository<T>` - Repository interface
- `IUnitOfWork` - Transaction management interface
- `ILogService` - Logging abstraction
- `IPdfGenerator` - PDF generation interface
- Domain entities (Customer, Invoice, Organization, etc.)
- Service interfaces (IChargeCardFactory, IECheckFactory)
- ViewModels and DTOs

### ORM Module
- `MakaluDbContext` - Entity Framework DbContext
- Entity Framework 6.1.3 for data access
- MySQL provider for EF6

### External Dependencies
- **NLog 4.3.7** - Logging framework
- **AuthorizeNet SDK 1.9.0** - Payment gateway integration
- **wkhtmltopdf** - HTML-to-PDF conversion tool
- **Entity Framework 6.1.3** - ORM framework
- **MySQL.Data** - MySQL database provider

## For AI Agents

### Adding New Repository Operations

1. **Define interface method in Core**:
```csharp
// In Core/Application/IRepository.cs or custom interface
public interface ICustomerService
{
    IEnumerable<Customer> GetActiveCustomers(int franchiseeId);
}
```

2. **Implement in Infrastructure**:
```csharp
[DefaultImplementation]
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IEnumerable<Customer> GetActiveCustomers(int franchiseeId)
    {
        return _unitOfWork.Repository<Customer>()
            .Table
            .Where(c => c.IsActive && c.FranchiseeId == franchiseeId)
            .ToList();
    }
}
```

3. **Mark with `[DefaultImplementation]` attribute** for automatic DI registration

### Adding External Service Integration

1. **Create interface in Infrastructure/[Domain]/**
2. **Implement adapter in Infrastructure/[Domain]/Impl/**
3. **Handle external API errors gracefully**
4. **Log all external API calls**
5. **Use settings/configuration for API credentials**

### Best Practices

- **Always use UnitOfWork**: Never instantiate DbContext directly
- **Dispose properly**: Use `using` blocks for UnitOfWork
- **Query efficiently**: Use `TableNoTracking` for read-only queries
- **Eager load wisely**: Use `IncludeMultiple` to prevent N+1 queries
- **Log external calls**: Log all payment gateway interactions
- **Handle errors**: Wrap external API calls in try-catch with logging
- **Test transactions**: Verify rollback behavior for error scenarios

## For Human Developers

### Common Tasks

#### Implementing New Repository Method

```csharp
// 1. Add method to generic repository (if applicable)
public class Repository<TEntity> : IRepository<TEntity>
{
    public IEnumerable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression)
    {
        return _dbContext.Set<TEntity>()
            .Where(expression)
            .ToList();
    }
}

// 2. Or create custom service
[DefaultImplementation]
public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public IEnumerable<Invoice> GetOverdueInvoices(int franchiseeId)
    {
        return _unitOfWork.Repository<Invoice>()
            .Table
            .Where(i => i.FranchiseeId == franchiseeId 
                && i.DueDate < DateTime.UtcNow
                && i.Balance > 0)
            .ToList();
    }
}
```

#### Adding Payment Processing Logic

```csharp
[DefaultImplementation]
public class NewPaymentService : INewPaymentService
{
    private readonly IAuthorizeNetCustomerProfileService _authNet;
    private readonly ILogService _log;
    private readonly IUnitOfWork _unitOfWork;
    
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        try
        {
            _log.Info($"Processing payment for customer {request.CustomerId}");
            
            // Call Authorize.Net
            var response = _authNet.ChargeCustomerProfile(
                request.ProfileId,
                request.PaymentProfileId,
                request.Amount
            );
            
            if (response.ResultCode == MessageTypeEnum.Ok)
            {
                // Create payment record
                var payment = new Payment
                {
                    CustomerId = request.CustomerId,
                    Amount = request.Amount,
                    TransactionId = response.TransactionResponse.TransId
                };
                
                _unitOfWork.Repository<Payment>().Insert(payment);
                _unitOfWork.SaveChanges();
                
                return PaymentResult.Success(response.TransactionResponse.TransId);
            }
            
            return PaymentResult.Failure(response.Messages[0].Text);
        }
        catch (Exception ex)
        {
            _log.Error("Payment processing failed", ex);
            throw;
        }
    }
}
```

### Performance Optimization

#### Avoid N+1 Queries
```csharp
// Bad - N+1 queries
var customers = repository.Table.Where(c => c.FranchiseeId == id).ToList();
foreach (var customer in customers)
{
    var address = customer.Address; // Triggers separate query
}

// Good - Eager loading
var customers = repository.IncludeMultiple(c => c.Address, c => c.Phone)
    .Where(c => c.FranchiseeId == id)
    .ToList();
```

#### Use NoTracking for Read-Only
```csharp
// For reporting/read-only scenarios
var report = repository.TableNoTracking
    .Where(/* filters */)
    .Select(/* projection */)
    .ToList();
```

### Testing Considerations

- Mock `IRepository<T>` for unit tests
- Mock `IUnitOfWork` to avoid database dependencies
- Test transaction rollback scenarios
- Mock external services (Authorize.Net) for payment tests
- Verify cascade save logic with complex entity graphs

## Configuration

### App.config Settings

```xml
<appSettings>
  <!-- Authorize.Net Settings -->
  <add key="AuthorizeNetApiUrl" value="https://api.authorize.net/xml/v1/request.api" />
  <add key="AuthorizeNetMode" value="Test" /> <!-- or "Production" -->
  
  <!-- wkhtmltopdf Path -->
  <add key="WkHtmlToPdfPath" value="C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe" />
</appSettings>

<connectionStrings>
  <add name="MakaluConnection" 
       connectionString="Server=localhost;Database=Makalu;Uid=root;Pwd=password;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

## Architecture Relationship

```
API Layer (Controllers)
    ↓ Uses
Core Layer (Business Logic & Interfaces)
    ↓ Implemented by
Infrastructure Layer (This Module)
    ├── Repository Pattern → Data Access
    ├── UnitOfWork → Transaction Management
    ├── Payment Services → External Integrations
    └── Utilities → Logging, PDF, etc.
    ↓ Uses
ORM Layer (Entity Framework & DbContext)
    ↓ Connects to
Database (MySQL)
```

## Troubleshooting

### Common Issues

**Transaction Errors**:
- Ensure `SaveChanges()` is called on UnitOfWork
- Verify nested transactions are handled correctly
- Check for disposed DbContext instances

**Payment Processing Failures**:
- Verify Authorize.Net credentials in configuration
- Check network connectivity to payment gateway
- Review transaction logs in database
- Validate credit card/eCheck information format

**PDF Generation Issues**:
- Ensure wkhtmltopdf.exe is installed and path is correct
- Verify HTML is well-formed
- Check for missing images or resources in HTML

**Repository Query Performance**:
- Use NoTracking for read-only queries
- Eager load related entities to prevent N+1
- Consider adding database indexes for frequently queried fields
