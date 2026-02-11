# Core/Application - Developer Guide

## Overview

The **Core.Application** module is the foundational infrastructure layer of the MarbleLife franchise management system. Think of it as the "toolbox" that every other module uses - it provides essential services like data access, file management, logging, authentication, and report generation.

This module implements the **Application Services Layer** in Clean Architecture, sitting between your domain logic and external infrastructure (databases, file systems, external APIs).

### What Problems Does It Solve?

1. **Data Access** - Repository pattern abstracts database operations
2. **File Management** - Unified API for file uploads, storage, and retrieval
3. **Report Generation** - Excel and PDF creation from templates
4. **Authentication** - Session management and password hashing
5. **Time Handling** - Timezone-aware date/time operations for global franchises
6. **Configuration** - Centralized application settings
7. **Logging** - Structured logging abstraction
8. **Validation** - Reusable validation logic and error handling

---

## Quick Start

### Using the Repository Pattern

```csharp
using Core.Application;

public class FranchiseeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Franchisee> _repo;
    
    public FranchiseeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repo = _unitOfWork.Repository<Franchisee>();
    }
    
    // Get single record
    public Franchisee GetById(long id)
    {
        return _repo.Get(id);
    }
    
    // Query with filtering
    public IEnumerable<Franchisee> GetActive()
    {
        return _repo.Fetch(f => f.IsActive && !f.IsDeleted);
    }
    
    // Paginated query
    public IEnumerable<Franchisee> GetPage(int pageNumber, int pageSize)
    {
        return _repo.Fetch(f => f.IsActive, pageSize, pageNumber);
    }
    
    // Count records
    public long CountActive()
    {
        return _repo.Count(f => f.IsActive);
    }
    
    // Save changes with transaction
    public void UpdateFranchisee(Franchisee franchisee)
    {
        _unitOfWork.StartTransaction();
        try
        {
            _repo.Save(franchisee);
            _unitOfWork.SaveChanges();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
    
    // Complex queries with LINQ
    public IEnumerable<Franchisee> SearchByRegion(string region)
    {
        return _repo.Table
            .Where(f => f.Region == region)
            .OrderBy(f => f.Name)
            .ToList();
    }
    
    // Eager loading relationships
    public Franchisee GetWithDetails(long id)
    {
        return _repo.IncludeMultiple(
            f => f.Address,
            f => f.PrimaryContact,
            f => f.Invoices
        ).FirstOrDefault(f => f.Id == id);
    }
}
```

### File Upload and Management

```csharp
using Core.Application;
using Core.Application.Impl;
using Core.Application.ViewModel;

public class DocumentService
{
    private readonly IFileService _fileService;
    private readonly ISettings _settings;
    
    public DocumentService(IFileService fileService, ISettings settings)
    {
        _fileService = fileService;
        _settings = settings;
    }
    
    // Upload file
    public FileModel UploadDocument(FileModel fileModel)
    {
        // Define storage location
        var location = new MediaLocation("documents/invoices");
        
        // Save file with auto-generated name
        var savedFile = _fileService.SaveFile(
            fileModel, 
            location, 
            fileNamePrefix: "INV"
        );
        
        return savedFile;
    }
    
    // Get file metadata
    public FileModel GetDocument(long fileId)
    {
        return _fileService.Get(fileId);
    }
    
    // Move file to different location
    public string MoveToArchive(string sourceFilePath)
    {
        var archiveLocation = new MediaLocation("documents/archive");
        
        return _fileService.MoveFile(
            sourceFilePath,
            archiveLocation,
            fileNamePrefix: "ARCH",
            ext: ".pdf"
        );
    }
}
```

### Generating Excel Reports

```csharp
using Core.Application;

public class ReportService
{
    private readonly IExcelFileCreator _excelCreator;
    private readonly IRepository<Sale> _saleRepo;
    private readonly IUnitOfWork _unitOfWork;
    
    public ReportService(
        IExcelFileCreator excelCreator,
        IUnitOfWork unitOfWork)
    {
        _excelCreator = excelCreator;
        _unitOfWork = unitOfWork;
        _saleRepo = _unitOfWork.Repository<Sale>();
    }
    
    // Generate Excel from list
    public bool GenerateSalesReport(DateTime startDate, DateTime endDate)
    {
        // Get data
        var sales = _saleRepo.Fetch(s => 
            s.SaleDate >= startDate && 
            s.SaleDate <= endDate
        ).ToList();
        
        // Create Excel file
        string filePath = $"C:\\Reports\\Sales_{DateTime.Now:yyyyMMdd}.xlsx";
        return _excelCreator.CreateExcelDocument(sales, filePath);
    }
    
    // Export to CSV
    public void ExportToCSV(List<Sale> sales, string filePath)
    {
        _excelCreator.ToCSVWriter(sales, filePath);
    }
    
    // Generate from DataSet (multiple sheets)
    public bool GenerateMultiSheetReport(DataSet data, string filePath)
    {
        return _excelCreator.CreateExcelDocument(data, filePath);
    }
}
```

### PDF Generation from Templates

```csharp
using Core.Application;

public class InvoiceService
{
    private readonly IPdfFileService _pdfService;
    private readonly ISettings _settings;
    
    public InvoiceService(IPdfFileService pdfService, ISettings settings)
    {
        _pdfService = pdfService;
        _settings = settings;
    }
    
    public string GenerateInvoicePDF(InvoiceViewModel invoice)
    {
        // Model for template
        var model = new
        {
            InvoiceNumber = invoice.Number,
            CustomerName = invoice.CustomerName,
            LineItems = invoice.Items,
            Total = invoice.Total
        };
        
        // Generate PDF from Razor template
        string templatePath = _settings.TemplateRootPath + "\\InvoiceTemplate.cshtml";
        string destinationFolder = _settings.MediaRootPath + "\\invoices";
        string fileName = $"Invoice_{invoice.Number}.pdf";
        
        return _pdfService.GeneratePdfFromTemplateAndModel(
            model,
            destinationFolder,
            fileName,
            templatePath
        );
    }
}
```

### Authentication and Session Management

```csharp
using Core.Application;

public class AuthenticationService
{
    private readonly ISessionFactory _sessionFactory;
    private readonly ISessionContext _sessionContext;
    private readonly ICryptographyOneWayHashService _cryptoService;
    private readonly IRepository<User> _userRepo;
    
    public AuthenticationService(
        ISessionFactory sessionFactory,
        ISessionContext sessionContext,
        ICryptographyOneWayHashService cryptoService,
        IUnitOfWork unitOfWork)
    {
        _sessionFactory = sessionFactory;
        _sessionContext = sessionContext;
        _cryptoService = cryptoService;
        _userRepo = unitOfWork.Repository<User>();
    }
    
    // Login user
    public bool Login(string email, string password)
    {
        var user = _userRepo.Get(u => u.Email == email);
        
        if (user == null)
            return false;
        
        // Validate password
        if (!_cryptoService.Validate(password, user.PasswordHash))
            return false;
        
        // Create session
        _sessionFactory.BuildSession(_sessionContext, user.UserLogin);
        
        return true;
    }
    
    // Register new user
    public void RegisterUser(string email, string password)
    {
        // Hash password
        var hashedPassword = _cryptoService.CreateHash(password);
        
        // Create user
        var user = new User
        {
            Email = email,
            PasswordHash = hashedPassword
        };
        
        _userRepo.Save(user);
    }
    
    // Get current user
    public UserSessionModel GetCurrentUser()
    {
        return _sessionContext.UserSession;
    }
}
```

### Time and Timezone Handling

```csharp
using Core.Application;

public class SchedulingService
{
    private readonly IClock _clock;
    private readonly ISettings _settings;
    
    public SchedulingService(IClock clock, ISettings settings)
    {
        _clock = clock;
        _settings = settings;
    }
    
    // Get current time in UTC (always use for storage)
    public DateTime GetCurrentTimeUTC()
    {
        return _clock.UtcNow;
    }
    
    // Get current time in user's timezone
    public DateTime GetCurrentTimeLocal()
    {
        return _clock.Now;
    }
    
    // Convert UTC to specific timezone
    public DateTime ConvertToFranchiseeTime(DateTime utcTime, string timezoneId)
    {
        _clock.SetTimeZoneInfo(timezoneId);
        return _clock.ToLocal(utcTime);
    }
    
    // Convert UTC to browser timezone (from offset)
    public DateTime ConvertToBrowserTime(DateTime utcTime, double offsetHours)
    {
        return _clock.ToLocal(utcTime, offsetHours);
    }
    
    // Schedule task for future (store in UTC)
    public void ScheduleTask(DateTime localScheduledTime)
    {
        var utcScheduledTime = _clock.ToUtc(localScheduledTime);
        // Store utcScheduledTime in database
    }
}
```

### Logging

```csharp
using Core.Application;

public class OrderService
{
    private readonly ILogService _logger;
    private readonly IRepository<Order> _orderRepo;
    
    public OrderService(ILogService logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _orderRepo = unitOfWork.Repository<Order>();
    }
    
    public void ProcessOrder(long orderId)
    {
        _logger.Trace($"Starting order processing for order {orderId}");
        
        try
        {
            var order = _orderRepo.Get(orderId);
            
            _logger.Debug($"Order {orderId} retrieved: {order.Status}");
            
            // Process order...
            
            _logger.Info($"Order {orderId} processed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process order {orderId}", ex);
            throw;
        }
    }
}
```

### Using Configuration Settings

```csharp
using Core.Application;

public class EmailService
{
    private readonly ISettings _settings;
    private readonly ILogService _logger;
    
    public EmailService(ISettings settings, ILogService logger)
    {
        _settings = settings;
        _logger = logger;
    }
    
    public void SendEmail(string to, string subject, string body)
    {
        // Use configuration
        var smtpClient = new SmtpClient(_settings.SmtpHost)
        {
            Port = _settings.SmtpPort,
            Credentials = new NetworkCredential(
                _settings.SmtpUserName, 
                _settings.SmtpPassword
            ),
            EnableSsl = _settings.EnableSsl
        };
        
        var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail),
            To = { new MailAddress(to) },
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        
        smtpClient.Send(message);
        
        _logger.Info($"Email sent to {to}: {subject}");
    }
}
```

---

## API Reference

### Core Interfaces

| Interface | Purpose | Key Methods |
|-----------|---------|-------------|
| `IRepository<T>` | Data access abstraction | `Get()`, `Fetch()`, `Save()`, `Delete()`, `Count()` |
| `IUnitOfWork` | Transaction management | `Repository<T>()`, `SaveChanges()`, `StartTransaction()`, `Rollback()` |
| `IFileService` | File operations | `SaveFile()`, `Get()`, `MoveFile()` |
| `IExcelFileCreator` | Excel generation | `CreateExcelDocument<T>()`, `ToCSV()` |
| `IPdfFileService` | PDF generation | `GeneratePdfFromTemplateAndModel()` |
| `ISessionFactory` | Session management | `BuildSession()`, `GetUserSessionModel()` |
| `ICryptographyOneWayHashService` | Password hashing | `CreateHash()`, `Validate()` |
| `IClock` | Time abstraction | `UtcNow`, `ToLocal()`, `ToUtc()` |
| `ILogService` | Logging | `Trace()`, `Debug()`, `Info()`, `Error()` |
| `ISettings` | Configuration | 267 properties for all app settings |

### Value Objects

| Type | Purpose | Properties |
|------|---------|------------|
| `SecureHash` | Password hash + salt | `HashedText`, `Salt` |
| `Dimension` | Image dimensions | `Width`, `Height` |
| `Name` | Validated name | `FirstName`, `LastName` |

### Domain Models

| Type | Purpose | Key Properties |
|------|---------|----------------|
| `File` | File metadata | `Name`, `Size`, `RelativeLocation`, `MimeType` |
| `Folder` | Directory structure | `Name`, `Path`, `ParentFolderId` |
| `Lookup` | Key-value data | `Key`, `Value`, `LookupTypeId` |

---

## Architecture Patterns

### Repository Pattern
Abstracts data access logic behind a clean interface.

**Benefits:**
- Testable (easy to mock)
- Consistent API across entities
- Centralized query logic

### Unit of Work Pattern
Manages transactions and change tracking.

**Benefits:**
- ACID guarantees
- Rollback on error
- Multiple repositories in single transaction

### Factory Pattern
Creates and transforms objects.

**Benefits:**
- Encapsulates creation logic
- Maps between layers (domain ↔ view model)

### Value Object Pattern
Immutable objects representing domain concepts.

**Benefits:**
- Validation at creation
- Type safety (no primitive obsession)
- Immutability prevents bugs

---

## Design Principles

### 1. Separation of Concerns
- **Interfaces** define contracts (root level)
- **Implementations** provide concrete behavior (`Impl/`)
- **Domain models** represent business entities (`Domain/`)
- **View models** represent API contracts (`ViewModel/`)

### 2. Dependency Inversion
All dependencies point to abstractions (interfaces), not concrete types.

### 3. Open/Closed Principle
Interfaces are stable; add new implementations without modifying existing code.

### 4. Single Responsibility
Each interface has one reason to change (logging, file storage, data access, etc.).

---

## Common Scenarios

### Scenario 1: Adding a New Entity

1. Create domain class inheriting from `DomainBase`
2. Add to DbContext
3. Use existing `IRepository<T>` - no custom repository needed
4. Create view model for API
5. Create factory to map domain ↔ view model

### Scenario 2: Custom Query Logic

```csharp
// Use Table or TableNoTracking for complex queries
var complexQuery = _repo.TableNoTracking
    .Where(e => e.Status == "Active")
    .Include(e => e.RelatedEntities)
    .GroupBy(e => e.Category)
    .Select(g => new { Category = g.Key, Count = g.Count() })
    .ToList();
```

### Scenario 3: Multiple Repositories in Transaction

```csharp
public void TransferFunds(long fromAccountId, long toAccountId, decimal amount)
{
    var accountRepo = _unitOfWork.Repository<Account>();
    var transactionRepo = _unitOfWork.Repository<Transaction>();
    
    _unitOfWork.StartTransaction();
    try
    {
        var fromAccount = accountRepo.Get(fromAccountId);
        var toAccount = accountRepo.Get(toAccountId);
        
        fromAccount.Balance -= amount;
        toAccount.Balance += amount;
        
        accountRepo.Save(fromAccount);
        accountRepo.Save(toAccount);
        
        var transaction = new Transaction
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            Date = _clock.UtcNow
        };
        transactionRepo.Save(transaction);
        
        _unitOfWork.SaveChanges();
    }
    catch
    {
        _unitOfWork.Rollback();
        throw;
    }
}
```

---

## Troubleshooting

<!-- CUSTOM SECTION: troubleshooting -->
<!-- Preserve manual notes below this line -->

### "Object reference not set to an instance of an object" when accessing Repository

**Cause:** `IUnitOfWork` not injected or disposed prematurely.

**Solution:** Ensure `IUnitOfWork` is registered in DI container and has appropriate lifetime (typically scoped per request).

### Files not saving to disk

**Cause:** Invalid `MediaLocation` path or insufficient permissions.

**Solution:**
1. Check `ISettings.MediaRootPath` is correctly configured
2. Ensure application has write permissions to folder
3. Verify folder exists (create if necessary)

### Timezone conversions incorrect

**Cause:** Not calling `SetTimeZoneInfo()` or using wrong offset.

**Solution:**
1. Always store dates in UTC in database
2. Call `IClock.SetTimeZoneInfo(timezoneId)` before conversions
3. Use `ToLocal()` when displaying to users
4. Use `ToUtc()` when storing user input

### Password validation fails

**Cause:** Hash algorithm changed or salt not stored.

**Solution:**
1. Verify `SecureHash` contains both `HashedText` and `Salt`
2. Don't change hashing algorithm without migrating existing passwords
3. Always use `ICryptographyOneWayHashService.CreateHash()` for new passwords

### Excel generation fails with "File in use"

**Cause:** File handle not released or Excel process running.

**Solution:**
1. Ensure `IExcelFileCreator` implementation uses `using` blocks
2. Close Excel if file is open
3. Use unique file names to avoid conflicts

### Session lost after restart

**Cause:** Session stored in memory only.

**Solution:**
1. Implement persistent session storage (database or Redis)
2. Use token-based authentication (JWT) for stateless sessions
3. Check session timeout configuration

### Repository returns stale data

**Cause:** Using cached query results or wrong context lifetime.

**Solution:**
1. Use `TableNoTracking` for read-only queries
2. Call `_unitOfWork.ResetContext()` if needed
3. Ensure `IUnitOfWork` has correct lifetime scope

### PDF generation produces blank pages

**Cause:** Template path incorrect or model properties don't match template.

**Solution:**
1. Verify `ISettings.TemplateRootPath` is correct
2. Check template file exists
3. Ensure model properties match template placeholders
4. Check wkhtmltopdf is installed and in PATH

<!-- END CUSTOM SECTION -->

---

## Best Practices

1. **Always use `IUnitOfWork` for transactions** - Don't bypass it
2. **Use `IClock` instead of `DateTime.Now`** - Enables testing and timezone handling
3. **Log before rethrowing exceptions** - `_logger.Error(ex); throw;`
4. **Validate inputs early** - Use value objects or custom exceptions at boundaries
5. **Don't return domain entities from APIs** - Always map to view models
6. **Use async/await for I/O operations** - `TableAsync()` for queries
7. **Dispose `IUnitOfWork` properly** - Use `using` blocks or DI lifetime management

---

## Related Modules

- **Core.Users** - User management and authentication
- **Core.Organizations** - Franchisee/organization management
- **Core.Sales** - Sales and invoicing
- **Core.MarketingLead** - Lead tracking and management

---

## Getting Help

### Internal Resources
- Architecture documentation: `/docs/architecture.md`
- API documentation: `/docs/api/`
- Database schema: `/docs/database/`

### External Resources
- Entity Framework: https://docs.microsoft.com/en-us/ef/
- EPPlus Excel library: https://github.com/EPPlusSoftware/EPPlus
- wkhtmltopdf: https://wkhtmltopdf.org/

---

**Last Updated:** Auto-generated from source code analysis  
**Module Version:** Tracked in `metadata.json`
