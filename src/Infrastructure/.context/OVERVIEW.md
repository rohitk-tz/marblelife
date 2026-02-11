# Infrastructure Module - Overview

## What is the Infrastructure Module?

The Infrastructure module is the **technical implementation layer** of the MarbleLife application. It provides concrete implementations of service interfaces defined in the Core module, handling data persistence, logging, document generation, and payment processing.

Think of it as the "engine room" where abstract concepts from the Core module are translated into working code that interacts with databases, external services, and system resources.

## Key Responsibilities

### 1. Data Persistence
Implements the **Repository** and **Unit of Work** patterns to manage database operations:
- CRUD operations for all domain entities
- Transaction management across multiple operations
- Query optimization with change tracking control
- Cascade save operations for complex entity graphs

### 2. Application Services
Provides essential application-level services:
- **Logging**: Structured logging using NLog
- **PDF Generation**: HTML to PDF conversion using wkhtmltopdf

### 3. Payment Processing
Integrates with Authorize.Net payment gateway:
- Credit card processing (one-time and saved cards)
- Electronic check (eCheck) processing
- Customer payment profile management
- Transaction management (charge, void, refund)

## Module Structure

```
Infrastructure/
├── Application/
│   └── Impl/
│       ├── Repository.cs          → Generic data access
│       ├── UnitOfWork.cs          → Transaction coordinator
│       ├── LogService.cs          → Application logging
│       ├── PdfGenerator.cs        → PDF document generation
│       └── SaveCascadeHelper.cs   → Complex entity save operations
│
└── Billing/
    ├── IAuthorizeNetCustomerProfileService.cs
    └── Impl/
        ├── AuthorizeNetCustomerProfileService.cs  → Payment gateway API
        ├── ChargeCardPaymentService.cs            → Credit card workflow
        ├── ECheckPaymentService.cs                → ECheck workflow
        ├── ChargeCardProfileService.cs            → Card profile management
        └── ECheckProfileService.cs                → ECheck profile management
```

## Core Components

### Repository<T>

**Purpose**: Generic data access layer for any domain entity.

**Key Features**:
- Type-safe CRUD operations
- LINQ query support
- Change tracking control
- Pagination
- Eager loading

**Example Usage**:
```csharp
// Get repository from UnitOfWork
var customerRepo = unitOfWork.Repository<Customer>();

// Query with LINQ
var activeCustomers = customerRepo.Table
    .Where(c => c.IsActive)
    .OrderBy(c => c.Name)
    .ToList();

// Get single entity
var customer = customerRepo.Get(customerId);

// Save changes
customer.Name = "Updated Name";
customerRepo.Save(customer);
```

### UnitOfWork

**Purpose**: Coordinates database transactions across multiple repositories.

**Key Features**:
- Single transaction for multiple operations
- Automatic rollback on errors
- Repository factory
- DbContext lifecycle management

**Example Usage**:
```csharp
using (var unitOfWork = new UnitOfWork())
{
    try
    {
        var customerRepo = unitOfWork.Repository<Customer>();
        var orderRepo = unitOfWork.Repository<Order>();
        
        // Multiple operations in single transaction
        customerRepo.Save(customer);
        orderRepo.Save(order);
        
        // Commit transaction
        unitOfWork.SaveChanges();
    }
    catch (Exception ex)
    {
        // Automatic rollback on exception
        unitOfWork.Rollback();
        throw;
    }
}
```

### LogService

**Purpose**: Centralized logging using NLog.

**Log Levels**:
- **Trace**: Detailed diagnostic information
- **Debug**: Debugging information
- **Info**: General informational messages
- **Error**: Error conditions

**Example Usage**:
```csharp
private readonly ILogService _logService;

public MyService(ILogService logService)
{
    _logService = logService;
}

public void ProcessOrder(Order order)
{
    _logService.Info($"Processing order {order.Id}");
    
    try
    {
        // Process order
        _logService.Info($"Order {order.Id} processed successfully");
    }
    catch (Exception ex)
    {
        _logService.Error($"Failed to process order {order.Id}", ex);
        throw;
    }
}
```

### PdfGenerator

**Purpose**: Generate PDF documents from HTML content.

**Features**:
- File-based HTML conversion
- Stream-based HTML conversion
- Configurable output options
- JavaScript support

**Example Usage**:
```csharp
var pdfGenerator = new PdfGenerator();
pdfGenerator.Switches = new WkHtmltoPdfSwitches
{
    PageSize = "Letter",
    Orientation = "Portrait"
};

// From HTML string
var html = new StringBuilder("<html><body><h1>Invoice</h1></body></html>");
string fileName = pdfGenerator.Generate(
    html, 
    @"C:\PDFs", 
    pdfConverterPath: @"C:\Tools\wkhtmltopdf",
    fileName: "invoice-123.pdf"
);

if (fileName != null)
{
    Console.WriteLine($"PDF generated: {fileName}");
}
```

### AuthorizeNetCustomerProfileService

**Purpose**: Direct integration with Authorize.Net payment gateway API.

**Key Operations**:
1. **Profile Management**: Create customer profiles with payment methods
2. **Transaction Processing**: Charge cards and eChecks
3. **Profile Updates**: Add/remove payment methods
4. **Transaction Management**: Void transactions

**Example Usage**:
```csharp
// Process one-time credit card payment
var response = authorizeNetService.ChargeNewCard(
    accountTypeId: 1,
    cardNumber: "4111111111111111",
    cvv: "123",
    expiryDate: "1225",
    invoiceId: invoice.Id,
    amount: 100.00m,
    payeeId: customer.Id,
    name: "John Doe",
    franchiseeName: "MarbleLife Seattle"
);

if (response.messages.resultCode == messageTypeEnum.Ok)
{
    var txnResponse = ((createTransactionResponse)response).transactionResponse;
    Console.WriteLine($"Payment approved: {txnResponse.transId}");
}
```

### ChargeCardPaymentService

**Purpose**: High-level credit card payment workflow orchestration.

**Features**:
- New card processing
- Saved card processing
- Profile management integration
- Automatic rollback on errors
- Payment record persistence

**Example Usage**:
```csharp
// Process new credit card payment
var model = new ChargeCardPaymentEditModel
{
    ProfileTypeId = 1,
    InvoiceId = invoice.Id,
    Amount = invoice.TotalAmount,
    ChargeCardEditModel = new ChargeCardEditModel
    {
        Number = "4111111111111111",
        CVV = "123",
        ExpireMonth = "12",
        ExpireYear = "2025",
        Name = "John Doe"
    }
};

decimal creditCardCharge, chargedAmount;
var response = chargeCardService.ChargeCardPayment(
    model, 
    franchiseeId: customer.Id,
    out creditCardCharge,
    out chargedAmount
);

if (response.ProcessorResult == ProcessorResponseResult.Accepted)
{
    Console.WriteLine($"Payment accepted: {response.RawResponse}");
}
else
{
    Console.WriteLine($"Payment failed: {response.Message}");
}
```

### ECheckPaymentService

**Purpose**: Electronic check payment processing workflow.

**Payment Types**:
1. **ECheck On File**: Charge saved bank account
2. **Save On File**: Process payment and save bank account
3. **One-Time**: Process without saving

**Example Usage**:
```csharp
// Process eCheck payment with save option
var model = new ECheckEditModel
{
    ProfileTypeId = 1,
    InvoiceId = invoice.Id,
    Amount = invoice.TotalAmount,
    AccountNumber = "123456789",
    Number = "111000025",  // Routing number
    Name = "John Doe",
    BankName = "Test Bank",
    InstrumentTypeId = (long)InstrumentType.ECheck,
    SaveOnFile = true  // Save for future use
};

var response = eCheckService.MakeECheckPayment(model, franchiseeId: customer.Id);

if (response.ProcessorResult == ProcessorResponseResult.Accepted)
{
    Console.WriteLine($"ECheck accepted: {response.RawResponse}");
    Console.WriteLine($"Instrument ID: {response.InstrumentId}");
}
```

## Common Workflows

### 1. Create and Save Entity

```csharp
using (var unitOfWork = new UnitOfWork())
{
    var customerRepo = unitOfWork.Repository<Customer>();
    
    var customer = new Customer
    {
        Name = "John Doe",
        Email = "john@example.com",
        IsActive = true
    };
    
    customerRepo.Save(customer);
    unitOfWork.SaveChanges();
    
    Console.WriteLine($"Created customer with ID: {customer.Id}");
}
```

### 2. Query with Eager Loading

```csharp
var orderRepo = unitOfWork.Repository<Order>();

// Avoid N+1 queries by including related entities
var orders = orderRepo
    .IncludeMultiple(
        o => o.Customer,
        o => o.OrderItems
    )
    .Where(o => o.OrderDate >= startDate)
    .ToList();
```

### 3. Update Multiple Entities in Transaction

```csharp
using (var unitOfWork = new UnitOfWork())
{
    try
    {
        var customerRepo = unitOfWork.Repository<Customer>();
        var orderRepo = unitOfWork.Repository<Order>();
        
        var customer = customerRepo.Get(customerId);
        customer.TotalOrders++;
        customerRepo.Update(customer);
        
        var order = new Order
        {
            CustomerId = customerId,
            OrderDate = DateTime.Now,
            TotalAmount = 100.00m
        };
        orderRepo.Insert(order);
        
        // Both operations committed together
        unitOfWork.SaveChanges();
    }
    catch
    {
        unitOfWork.Rollback();
        throw;
    }
}
```

### 4. Process Payment with Saved Card

```csharp
// Use saved payment method
var model = new InstrumentOnFileEditModel
{
    ProfileTypeId = 1,
    InstrumentId = savedCardId,
    InvoiceId = invoice.Id,
    Amount = invoice.TotalAmount
};

var response = chargeCardService.ChargeCardOnFile(
    model, 
    new ProcessorResponse(), 
    franchiseeId: customer.Id
);

if (response.ProcessorResult == ProcessorResponseResult.Accepted)
{
    // Save payment record
    var payment = CreatePayment(invoice, response);
    chargeCardService.Save(response, payment);
}
```

## API Reference

### Repository<T> Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Get(long id)` | Get entity by ID | `T` or `null` |
| `Get(Expression<Func<T, bool>>)` | Get single entity by predicate | `T` or `null` |
| `Fetch(Expression<Func<T, bool>>)` | Get all matching entities | `IEnumerable<T>` |
| `Fetch(predicate, pageSize, pageNumber)` | Get page of entities | `IEnumerable<T>` |
| `Count(Expression<Func<T, bool>>)` | Count matching entities | `long` |
| `Save(T entity)` | Insert or update entity | `void` |
| `Insert(T entity)` | Add new entity | `void` |
| `Update(T entity)` | Modify existing entity | `void` |
| `Delete(T entity)` | Remove entity | `void` |
| `Delete(long id)` | Remove by ID | `void` |
| `TableAsync(Expression<Func<T, bool>>)` | Async query | `Task<List<T>>` |
| `IncludeMultiple(params Expression[])` | Eager load relationships | `IQueryable<T>` |

### UnitOfWork Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Repository<T>()` | Get/create repository | `IRepository<T>` |
| `SaveChanges()` | Commit transaction | `void` |
| `Rollback()` | Abort transaction | `void` |
| `StartTransaction()` | Begin transaction | `void` |
| `ResetContext()` | Recreate DbContext | `void` |
| `Dispose()` | Cleanup resources | `void` |

### LogService Methods

| Method | Description |
|--------|-------------|
| `Trace(string message)` | Log trace message |
| `Debug(string message)` | Log debug message |
| `Info(string message)` | Log info message |
| `Error(string message)` | Log error message |
| `Error(string message, Exception)` | Log error with exception |
| `Error(Exception exception)` | Log exception |

### PdfGenerator Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Generate(sourcePath, destPath, converterPath, fileName)` | Generate from file | `string` (filename) or `null` |
| `Generate(StringBuilder html, destPath, converterPath, fileName)` | Generate from HTML | `string` (filename) or `null` |

### Payment Service Methods

#### AuthorizeNetCustomerProfileService

| Method | Description | Returns |
|--------|-------------|---------|
| `CreateNewProfile(...)` | Create card profile | `createCustomerProfileResponse` |
| `CreateECheckProfile(...)` | Create eCheck profile | `createCustomerProfileResponse` |
| `CreateAdditionalPaymentProfile(...)` | Add card to profile | `createCustomerPaymentProfileResponse` |
| `CreateAdditionalEChaekPaymentProfile(...)` | Add eCheck to profile | `createCustomerPaymentProfileResponse` |
| `DeleteCustomerProfile(...)` | Remove payment method | `deleteCustomerPaymentProfileResponse` |
| `ChargeNewCard(...)` | Process one-time card | `ANetApiResponse` |
| `ChargeCustomerProfile(...)` | Charge saved method | `ANetApiResponse` |
| `DebitBankAccount(...)` | Process one-time eCheck | `createTransactionResponse` |
| `VoidPayment(...)` | Reverse transaction | `ANetApiResponse` |

#### ChargeCardPaymentService

| Method | Description | Returns |
|--------|-------------|---------|
| `ChargeCardPayment(model, franchiseeId, out charges, out amount)` | Process new card | `ProcessorResponse` |
| `ChargeCardOnFile(model, response, franchiseeId)` | Process saved card | `ProcessorResponse` |
| `Save(response, payment)` | Persist payment record | `void` |
| `RollbackPayment(accountTypeId, transactionId)` | Void transaction | `bool` |

#### ECheckPaymentService

| Method | Description | Returns |
|--------|-------------|---------|
| `MakeECheckPayment(model, franchiseeId)` | Process eCheck | `ProcessorResponse` |
| `Save(model, payment)` | Persist eCheck record | `void` |

## Troubleshooting

### Issue: "Object reference not set to an instance of an object" in Repository

**Cause**: DbContext is null or disposed.

**Solution**:
```csharp
// Ensure UnitOfWork is not disposed
using (var unitOfWork = new UnitOfWork())
{
    // Use repositories inside using block
    var repo = unitOfWork.Repository<Customer>();
    // ... perform operations ...
    unitOfWork.SaveChanges();
} // Dispose happens here
```

### Issue: Transaction Deadlock

**Cause**: Multiple transactions accessing same resources.

**Solution**:
- Keep transaction scope minimal
- Access entities in consistent order
- Use `ResetContext()` if context is corrupted

```csharp
try
{
    unitOfWork.SaveChanges();
}
catch (DbUpdateConcurrencyException)
{
    unitOfWork.ResetContext();
    // Retry operation
}
```

### Issue: Payment Processing Returns "Some error occurred"

**Cause**: Authorize.Net API error or invalid credentials.

**Solution**:
1. Check logs for detailed error message
2. Verify API credentials in `AuthorizeNetApiMaster` table
3. Ensure correct test/production mode setting
4. Validate card number format and expiration date

```csharp
// Enable detailed logging
_logService.Info($"Processing payment for franchisee {franchiseeId}");
var response = authorizeNetService.ChargeNewCard(...);
_logService.Info($"Response code: {response.messages.resultCode}");
_logService.Info($"Response message: {response.messages.message[0].text}");
```

### Issue: PDF Generation Returns Null

**Cause**: wkhtmltopdf.exe not found or HTML parsing error.

**Solution**:
1. Verify wkhtmltopdf.exe is installed and in PATH
2. Check destination folder exists and is writable
3. Increase timeout for complex HTML
4. Check stderr for detailed error

```csharp
// Explicit path to wkhtmltopdf
var fileName = pdfGenerator.Generate(
    html,
    @"C:\PDFs",
    pdfConverterPath: @"C:\Tools\wkhtmltopdf\bin",
    fileName: "output.pdf"
);

if (fileName == null)
{
    _logService.Error("PDF generation failed");
}
```

### Issue: "A transaction is already in progress on this DbContext"

**Cause**: Attempting to start a new transaction when one exists.

**Solution**:
```csharp
// Don't manually start transaction - UnitOfWork handles it
using (var unitOfWork = new UnitOfWork())
{
    // Transaction starts on first Repository<T>() call
    var repo = unitOfWork.Repository<Customer>();
    
    // Just use repositories normally
    repo.Save(customer);
    
    // Commit at the end
    unitOfWork.SaveChanges();
}
```

### Issue: SaveChanges() Not Persisting Changes

**Cause**: Changes made outside repository or transaction not committed.

**Solution**:
```csharp
// Use repository methods for persistence
var customer = repo.Get(customerId);
customer.Name = "Updated Name";
repo.Update(customer);  // Or repo.Save(customer)

// Must call SaveChanges to commit
unitOfWork.SaveChanges();
```

## Configuration

### Database Connection
Edit connection string in `MakaluDbContext` or app.config:
```xml
<connectionStrings>
  <add name="MakaluDbContext" 
       connectionString="Data Source=server;Initial Catalog=MarbleLife;..." 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Logging (NLog.config)
```xml
<nlog>
  <targets>
    <target name="file" type="File" fileName="logs/app.log" />
  </targets>
  <rules>
    <logger name="*" minlevel="Info" writeTo="file" />
  </rules>
</nlog>
```

### Authorize.Net Credentials
Stored in `AuthorizeNetApiMaster` table:
```sql
INSERT INTO AuthorizeNetApiMaster (AccountTypeId, ApiLoginID, ApiTransactionKey)
VALUES (1, 'your-api-login-id', 'your-transaction-key');
```

Set test mode in configuration:
```csharp
ISettings.AuthNetTestMode = true;  // Sandbox
ISettings.AuthNetTestMode = false; // Production
```

## Best Practices

1. **Always use UnitOfWork**: Ensures proper transaction management
2. **Dispose resources**: Use `using` statements for UnitOfWork
3. **Use TableNoTracking**: For read-only queries (better performance)
4. **Eager load relationships**: Avoid N+1 query problems with `IncludeMultiple()`
5. **Log all errors**: Use `ILogService.Error()` for troubleshooting
6. **Validate input**: Always validate before payment processing
7. **Handle rollback**: Void payments if subsequent operations fail
8. **Test in sandbox**: Use Authorize.Net sandbox before production

---

*For detailed technical documentation, see CONTEXT.md*
