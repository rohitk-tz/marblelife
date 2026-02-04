# Core/Application - AI Context

## Purpose

The **Application** folder contains core infrastructure services, utilities, and base classes used across all domain modules. This is the foundation layer providing essential cross-cutting concerns.

## Contents

### Base Classes & Interfaces
- **DomainBase.cs**: Base class for all domain entities
- **IRepository<T>**: Generic repository pattern interface
- **IUnitOfWork**: Transaction and session management
- **ISessionContext**: Current user session information
- **ISessionFactory**: Session creation and management

### Application Services
- **ApplicationManager.cs**: Central application lifecycle manager
- **IAppContextStore**: Application-wide context storage
- **ISettings**: Configuration and application settings
- **IDependencyInjectionHelper**: DI container helper utilities

### File & Document Services
- **IFileService** & **IFileFactory**: File operations
- **IExcelFileCreator**: Excel file generation
- **IExcelFileFormaterCreator**: Excel formatting
- **IExcelFileCreatorMarketingLead**: Lead-specific Excel exports
- **IPdfFileService** & **IPdfGenerator**: PDF generation
- **IImageHelper**: Image processing utilities

### Infrastructure Services
- **ILogService**: Logging abstraction (supports multiple providers)
- **ICryptographyOneWayHashService**: Password hashing and security
- **IClock**: Time abstraction for testability

### Sub-folders

#### **Domain/**
Base domain classes and shared entity definitions:
- `DomainBase`: Base entity with common properties (Id, CreatedDate, etc.)
- `File`: File metadata entity
- Other shared domain objects

#### **Impl/**
Implementations of application service interfaces:
- `AppContextStore`: In-memory context storage
- `CryptographyOneWayHashService`: BCrypt/SHA implementations
- `ExcelFileCreator`: Excel generation using EPPlus/NPOI
- `PdfGenerator`: PDF generation using iTextSharp
- `LogService`: Logging implementation
- Repository implementations

#### **Enum/**
Application-wide enumerations:
- `FileType`: Document type categories
- `LogLevel`: Logging levels
- `ErrorCode`: Application error codes
- Status and state enumerations

#### **ViewModel/**
Shared ViewModels/DTOs:
- `FileViewModel`: File metadata transfer
- `ErrorViewModel`: Error response structure
- `PaginationViewModel`: Pagination metadata
- `SearchViewModel`: Common search parameters

#### **Exceptions/**
Custom exception types:
- `BusinessRuleViolationException`: Business logic errors
- `EntityNotFoundException`: Resource not found
- `ValidationException`: Input validation failures
- `UnauthorizedException`: Access denied scenarios

#### **Extensions/**
C# extension methods:
- String extensions (validation, formatting)
- DateTime extensions
- Collection extensions
- Enum extensions

#### **Attribute/**
Custom attributes:
- Validation attributes
- Authorization attributes
- Metadata attributes

#### **ValueType/**
Value objects (immutables):
- `EmailAddress`: Email value object
- `PhoneNumber`: Phone value object
- `Money`: Currency value object
- Other domain value types

## Key Design Patterns

1. **Repository Pattern**: `IRepository<T>` for data access
2. **Unit of Work Pattern**: `IUnitOfWork` for transactions
3. **Factory Pattern**: `IFileFactory` for object creation
4. **Strategy Pattern**: Different file generators (Excel, PDF)
5. **Value Object Pattern**: Immutable value types

## Dependencies

- Entity Framework (ORM)
- EPPlus or NPOI (Excel generation)
- iTextSharp (PDF generation)
- BCrypt.NET or similar (cryptography)
- Log4Net or NLog (logging)

## For AI Agents

### Adding New Utilities
1. Define interface in root `Application/` folder
2. Implement in `Impl/` subfolder
3. Register in DependencyInjection module
4. Use dependency injection in consuming classes

### Creating Value Objects
```csharp
// In ValueType/
public class EmailAddress
{
    public string Value { get; private set; }
    
    private EmailAddress(string email)
    {
        if (!IsValid(email))
            throw new ArgumentException("Invalid email");
        Value = email;
    }
    
    public static EmailAddress Create(string email) => new EmailAddress(email);
    private static bool IsValid(string email) => /* validation */;
}
```

### Adding Custom Exceptions
```csharp
// In Exceptions/
public class CustomBusinessException : Exception
{
    public ErrorCode Code { get; set; }
    
    public CustomBusinessException(string message, ErrorCode code) 
        : base(message)
    {
        Code = code;
    }
}
```

## For Human Developers

### Common Use Cases

#### Using Repository Pattern
```csharp
public class MyService
{
    private readonly IRepository<MyEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public MyService(IRepository<MyEntity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public void SaveEntity(MyEntity entity)
    {
        using (var transaction = _unitOfWork.BeginTransaction())
        {
            _repository.Add(entity);
            _unitOfWork.SaveChanges();
            transaction.Commit();
        }
    }
}
```

#### Logging
```csharp
_logService.LogInfo("Operation started", new { UserId = 123 });
_logService.LogError("Operation failed", exception);
```

#### File Generation
```csharp
var excelFile = _excelFileCreator.Create(data, template);
_fileService.Save(excelFile, "reports/monthly.xlsx");
```

### Best Practices
- Always use `IUnitOfWork` for transactions
- Log at appropriate levels (Debug, Info, Warning, Error)
- Validate inputs using ValueTypes or Exceptions
- Use extension methods for reusable logic
- Prefer immutable value objects over primitives
- Use `IClock` instead of `DateTime.Now` for testability
