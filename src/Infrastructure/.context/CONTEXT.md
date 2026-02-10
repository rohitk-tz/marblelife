# Infrastructure Module - AI/Agent Context

## Module Identity

**Name**: Infrastructure  
**Type**: Implementation Layer  
**Primary Responsibility**: Concrete implementations of Core module service interfaces, providing data persistence, logging, PDF generation, and payment processing capabilities.

## Architecture

### Purpose
The Infrastructure module implements service interfaces defined in the Core module, acting as the technical implementation layer that handles:
- Data persistence via Entity Framework ORM
- Repository and Unit of Work patterns for domain entities
- Logging services using NLog
- PDF generation using wkhtmltopdf
- Payment processing via Authorize.Net integration
- Payment method management (credit cards and eCheck)

### Design Patterns

#### Repository Pattern
- **Implementation**: `Repository<T>` (generic repository)
- **Base Constraint**: `T : DomainBase`
- **Features**:
  - CRUD operations (Create, Read, Update, Delete)
  - Query support with LINQ expressions
  - Change tracking and no-tracking queries
  - Pagination support
  - Async operations
  - Eager loading with `IncludeMultiple()`
  - Cascade save handling via `SaveCascadeHelper`

#### Unit of Work Pattern
- **Implementation**: `UnitOfWork`
- **Purpose**: Coordinates transactions across multiple repositories
- **Features**:
  - Transaction management (Begin, Commit, Rollback)
  - Repository factory method `Repository<T>()`
  - DbContext lifecycle management
  - Connection management
  - Context reset capability

#### Service Layer Pattern
- **Attribute-based registration**: `[DefaultImplementation]`
- **Dependency injection**: Constructor injection of dependencies
- **Separation of concerns**: Business logic in Core, implementation in Infrastructure

### Component Structure

```
Infrastructure/
├── Application/
│   └── Impl/
│       ├── Repository.cs           # Generic repository implementation
│       ├── UnitOfWork.cs           # Transaction coordinator
│       ├── LogService.cs           # NLog wrapper
│       ├── PdfGenerator.cs         # wkhtmltopdf wrapper
│       ├── SaveCascadeHelper.cs    # Entity cascade operations
│       └── TransactionHelper.cs    # Transaction utilities
└── Billing/
    ├── IAuthorizeNetCustomerProfileService.cs
    └── Impl/
        ├── AuthorizeNetCustomerProfileService.cs  # Profile management
        ├── ChargeCardPaymentService.cs            # Credit card processing
        ├── ChargeCardProfileService.cs            # Card profile management
        ├── ECheckPaymentService.cs                # ECheck processing
        ├── ECheckProfileService.cs                # ECheck profile management
        └── CurrencyRateService.cs                 # Currency conversion
```

## Type Definitions

### Core Classes

#### Repository&lt;T&gt;
```csharp
class Repository<T> : Repository, IRepository<T> where T: DomainBase
{
    // Properties
    IQueryable<T> Table { get; }
    IQueryable<T> TableNoTracking { get; }
    
    // Query Methods
    T Get(long id);
    T Get(Expression<Func<T, bool>> expression);
    IEnumerable<T> Fetch(Expression<Func<T, bool>> expression);
    IEnumerable<T> Fetch(Expression<Func<T, bool>> expression, int pageSize, int pageNumber);
    Task<List<T>> TableAsync(Expression<Func<T, bool>> match);
    long Count(Expression<Func<T, bool>> expression);
    IQueryable<T> IncludeMultiple(params Expression<Func<T, object>>[] includes);
    
    // Persistence Methods
    void Save(T entity);          // Insert or Update based on IsNew
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Delete(long id);
    void Delete(Expression<Func<T, bool>> expression);
}
```

#### UnitOfWork
```csharp
public class UnitOfWork : IUnitOfWork
{
    // Methods
    IRepository<T> Repository<T>() where T : DomainBase;
    void SaveChanges();           // Commit transaction
    void Rollback();              // Rollback transaction
    void StartTransaction();
    void ResetContext();          // Recreate DbContext
    void Cleanup();               // Dispose resources
    void Dispose();
}
```

#### LogService
```csharp
public class LogService : ILogService
{
    void Trace(string message);
    void Debug(string message);
    void Info(string message);
    void Error(string message);
    void Error(string message, Exception exception);
    void Error(Exception exception);
}
```

#### PdfGenerator
```csharp
public class PdfGenerator : IPdfGenerator
{
    WkHtmltoPdfSwitches Switches { get; set; }
    bool AllowLoadingJavascriptbeforePdfGenerate { get; set; }
    
    string Generate(string sourcePath, string destinationPath, 
                   string pdfConverterPath = "", string fileName = "");
    string Generate(StringBuilder htmlStream, string destinationPath, 
                   string pdfConverterPath = "", string fileName = "");
}
```

### Billing Classes

#### AuthorizeNetCustomerProfileService
Manages Authorize.Net customer profiles and payment processing.

**Key Methods**:
- `CreateNewProfile()` - Create credit card profile
- `CreateECheckProfile()` - Create eCheck profile
- `CreateAdditionalPaymentProfile()` - Add card to existing profile
- `CreateAdditionalEChaekPaymentProfile()` - Add eCheck to profile
- `DeleteCustomerProfile()` - Remove payment profile
- `ChargeNewCard()` - Process one-time card payment
- `ChargeCustomerProfile()` - Charge saved payment method
- `DebitBankAccount()` - Process one-time eCheck payment
- `VoidPayment()` - Reverse a transaction
- `IfErrorHandleErrorResponse()` - Centralized error handling

**Environment Management**:
- Sandbox mode: `_settings.AuthNetTestMode = true`
- Production mode: `_settings.AuthNetTestMode = false`

#### ChargeCardPaymentService
Handles credit card payment processing workflow.

**Key Methods**:
- `ChargeCardPayment()` - Process new card payment
- `ChargeCardOnFile()` - Process saved card payment
- `Save()` - Persist payment record
- `RollbackPayment()` - Void failed transaction

**Features**:
- CVV code normalization (padding to 3 digits)
- Expiration date formatting (MMYY)
- Profile management integration
- Transaction rollback on errors

#### ECheckPaymentService
Handles electronic check payment processing.

**Key Methods**:
- `MakeECheckPayment()` - Main payment processing method
- `Save()` - Persist eCheck payment record

**Payment Types**:
1. **ECheck On File**: Charge saved bank account
2. **Save On File**: Process and save bank account
3. **One-Time**: Process without saving

**Rollback Strategy**: Automatically voids payment if profile save fails

## Public Interfaces

### IRepository&lt;T&gt;
Generic data access interface for domain entities.

**Query Operations**:
- `Table` - Tracked queryable
- `TableNoTracking` - Read-only queryable
- `Get()` - Single entity retrieval
- `Fetch()` - Multiple entity retrieval with filtering
- `Count()` - Count matching entities
- `IncludeMultiple()` - Eager loading

**Persistence Operations**:
- `Save()` - Smart save (insert or update)
- `Insert()` - Add new entity
- `Update()` - Modify existing entity
- `Delete()` - Remove entity

### IUnitOfWork
Transaction and repository coordinator.

**Core Methods**:
- `Repository<T>()` - Get/create repository instance
- `SaveChanges()` - Commit all changes
- `Rollback()` - Abort transaction
- `ResetContext()` - Refresh DbContext

### ILogService
Application logging interface.

**Log Levels**:
- `Trace()` - Detailed diagnostic info
- `Debug()` - Debugging information
- `Info()` - General informational messages
- `Error()` - Error conditions

### IPdfGenerator
PDF document generation interface.

**Methods**:
- `Generate(string sourcePath, ...)` - Convert file to PDF
- `Generate(StringBuilder htmlStream, ...)` - Convert HTML to PDF

**Configuration**:
- `Switches` - wkhtmltopdf command-line options
- `AllowLoadingJavascriptbeforePdfGenerate` - JS execution flag

### IAuthorizeNetCustomerProfileService
Payment gateway profile management.

**Profile Operations**:
- Create customer profiles (card/eCheck)
- Add payment methods to profiles
- Delete payment methods
- Update customer information

**Transaction Operations**:
- Charge new card/eCheck
- Charge saved payment method
- Void transactions

### IChargeCardPaymentService
Credit card payment processing.

**Methods**:
- `ChargeCardPayment()` - New card payment
- `ChargeCardOnFile()` - Saved card payment
- `Save()` - Persist payment
- `RollbackPayment()` - Void transaction

### IECheckPaymentService
Electronic check payment processing.

**Methods**:
- `MakeECheckPayment()` - Process eCheck
- `Save()` - Persist eCheck payment

## Dependencies

### External Dependencies

#### Entity Framework 6
- **Purpose**: ORM for data persistence
- **Usage**: `DbContext`, `DbSet<T>`, LINQ queries
- **Transaction**: `Database.UseTransaction()`

#### NLog
- **Purpose**: Structured logging
- **Usage**: `LogManager.GetLogger()`, `LogEventInfo`
- **Configuration**: External NLog.config file

#### Authorize.Net SDK
- **Package**: `AuthorizeNet.Api.Contracts.V1`
- **Purpose**: Payment gateway integration
- **Components**:
  - `ApiOperationBase` - API client base
  - `merchantAuthenticationType` - Authentication
  - `createTransactionController` - Transaction processing
  - `createCustomerProfileController` - Profile management

#### wkhtmltopdf
- **Purpose**: HTML to PDF conversion
- **Type**: External executable
- **Location**: Configurable path or system PATH
- **Process Management**: System.Diagnostics.Process

### Internal Dependencies

#### Core Module
- **Interfaces**: `IRepository<T>`, `IUnitOfWork`, `ILogService`, `IPdfGenerator`
- **Domain**: `DomainBase`, payment domain entities
- **Attributes**: `[DefaultImplementation]`, `[CascadeEntityAttribute]`
- **Settings**: `ISettings` for configuration

#### ORM Module
- **Purpose**: DbContext implementation
- **Component**: `MakaluDbContext`
- **Usage**: Entity mappings, database configuration

## Implementation Notes

### Repository Pattern Details

**State Management**:
- `Save()` checks `entity.IsNew` to determine Insert vs Update
- `Update()` retrieves original entity from database
- Cascade updates handled by `SaveCascadeHelper`

**Query Optimization**:
- Use `TableNoTracking` for read-only queries (better performance)
- Use `IncludeMultiple()` to avoid N+1 query problems
- Pagination via `Skip()` and `Take()`

### Unit of Work Lifecycle

**Transaction Flow**:
1. First `Repository<T>()` call opens connection and begins transaction
2. Multiple repositories share same transaction
3. `SaveChanges()` commits transaction
4. `Rollback()` or exception triggers rollback
5. `Cleanup()` or `Dispose()` closes connection

**Context Reset**:
- Call `ResetContext()` to recover from context errors
- Recreates `MakaluDbContext` and updates all repository references

### Payment Processing

**Authorize.Net Integration**:
- API keys loaded from `AuthorizeNetApiMaster` table by `accountTypeId`
- Environment set via `_settings.AuthNetTestMode`
- Customer profile ID stored as `FranchiseePaymentProfile.CMID`
- Payment profile ID stored as `PaymentInstrument.InstrumentProfileId`

**Error Handling**:
- `IfErrorHandleErrorResponse()` centralizes error logging
- Response codes checked: `messageTypeEnum.Ok`, `messageTypeEnum.Error`
- Transaction response codes: `1` = Approved, other = Declined
- CVV codes validated in non-test mode

**Rollback Strategy**:
- Payment processed first, then profile saved
- If profile save fails, payment is voided
- Uses `VoidPayment()` method

### PDF Generation

**Process Flow**:
1. Create `ProcessStartInfo` with wkhtmltopdf.exe path
2. Pass HTML source (file or stdin) and output path
3. Wait for process (50s timeout for file, 30s for stream)
4. Kill process if timeout exceeded
5. Check exit code (0 = success)
6. Handle Qt-specific errors (warnings that don't prevent generation)

**Common Issues**:
- Missing wkhtmltopdf.exe in PATH
- JavaScript loading timeout (set `AllowLoadingJavascriptbeforePdfGenerate`)
- Qt warnings ending in "done" are acceptable

### Cascade Save Operations

**SaveCascadeHelper**:
- Handles `[CascadeEntity]` attribute on navigation properties
- Manages collection additions/deletions
- Sets appropriate entity states (Added, Modified, Deleted)
- Recursively processes nested entities

**Collection Handling**:
- Matches entities by `Id`
- Deleted: In old collection but not new
- Modified: In both collections
- Added: In new collection with `Id < 1` or `IsNew = true`

## Configuration

### Required Settings

**Database**:
- Connection string in `MakaluDbContext`
- Entity Framework provider

**Logging**:
- NLog.config file in application root
- Log targets (file, console, database)

**Payment Gateway**:
- Authorize.Net API credentials in `AuthorizeNetApiMaster` table
- Test mode flag in `ISettings.AuthNetTestMode`

**PDF Generation**:
- wkhtmltopdf.exe location (PATH or explicit path)
- Destination folder with write permissions

### Environment Variables
- None directly used; configuration via `ISettings` interface and database

## Error Handling

### Repository Errors
- `SingleOrDefault()` returns null if not found
- `Find()` returns null for missing entities
- Database errors propagate to caller

### Unit of Work Errors
- Transaction rollback on `Dispose()` if not committed
- Connection errors logged and propagated
- Context disposal ensures cleanup

### Payment Processing Errors
- `ProcessorResponse.ProcessorResult` enum: `Accepted`, `Fail`
- `ProcessorResponse.Message` contains error details
- All errors logged via `ILogService`
- Rollback on profile save failures

### PDF Generation Errors
- Returns `null` on failure
- Checks exit code and stderr output
- Timeout kills process and returns null
- Qt warnings checked for "done" suffix

## Testing Considerations

### Unit Testing
- Mock `DbContext` and `DbSet<T>`
- Use in-memory database or repository mocks
- Test Repository methods independently
- Verify cascade save behavior

### Integration Testing
- Use test database with known data
- Test transaction commit/rollback
- Verify payment gateway in sandbox mode
- Test PDF generation with sample HTML

### Common Test Scenarios
1. Repository CRUD operations
2. Unit of Work transaction management
3. Payment processing (success/failure)
4. Profile creation and management
5. PDF generation with various HTML inputs
6. Error handling and rollback

## Performance Considerations

### Database
- Use `TableNoTracking` for read-only queries
- Implement pagination for large result sets
- Use `IncludeMultiple()` to avoid N+1 queries
- Consider caching for frequently accessed data

### Payment Processing
- API calls are synchronous (network latency)
- Consider async implementations for high volume
- Implement retry logic for transient failures

### PDF Generation
- Process spawning has overhead
- Large PDFs increase generation time
- Consider queue-based processing for bulk generation

## Security Notes

- API credentials stored in database (ensure encryption)
- CVV codes not persisted (PCI compliance)
- Use parameterized queries (EF prevents SQL injection)
- Validate all input in service layer
- Log sensitive operations without exposing card data

## Migration Path

**From Direct DbContext Usage**:
1. Inject `IUnitOfWork` instead of `DbContext`
2. Use `unitOfWork.Repository<T>()` instead of `dbContext.Set<T>()`
3. Replace `dbContext.SaveChanges()` with `unitOfWork.SaveChanges()`

**From Manual Repository**:
1. Replace custom repositories with `IRepository<T>`
2. Remove duplicate CRUD code
3. Leverage built-in query methods

## Related Modules

- **Core**: Defines interfaces implemented here
- **ORM**: Provides `MakaluDbContext`
- **Web/API**: Consumes these services via dependency injection

---

*Last Updated: 2024 (Commit: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366)*
