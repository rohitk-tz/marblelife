# Core Module - AI Context

## Purpose

The **Core** module is the heart of the MarbleLife application's business logic layer. It contains domain models, business rules, service interfaces, and their implementations following Domain-Driven Design (DDD) principles.

## Architecture

This module is organized by business domains, each containing:
- **Domain**: Entity models and domain objects
- **Impl**: Service implementation classes
- **Enum**: Enumeration types for the domain
- **ViewModel**: Data transfer objects (DTOs) for API communication

## Domain Modules

### Business Operations
1. **Organizations**: Franchisee and organization management
2. **Sales**: Customer relationship and sales pipeline
3. **Scheduler**: Job scheduling and appointments
4. **MarketingLead**: Lead generation and tracking
5. **Billing**: Payment processing and invoicing
6. **Reports**: Business intelligence and reporting

### System Functions
7. **Users**: User authentication and authorization
8. **Dashboard**: Aggregated metrics and KPIs
9. **Notification**: Multi-channel notifications
10. **Review**: Customer review management
11. **ToDo**: Task management system
12. **Geo**: Geographic and location services

### Infrastructure Support
13. **Application**: Core application services and utilities
14. **AWS**: Amazon Web Services integration
15. **Localization**: Internationalization support
16. **ReviewApi**: External review platform integration

## Key Interfaces

### Universal Interfaces (Application folder)
- `IRepository<T>`: Generic repository pattern
- `IUnitOfWork`: Transaction management
- `ISessionContext`: User session management
- `ISettings`: Application configuration
- `ILogService`: Logging abstraction
- `IFileService`: File operations
- `ICryptographyOneWayHashService`: Security/hashing

### Domain-Specific Patterns
Each domain follows this pattern:
- `I[Entity]Factory`: Entity creation with validation
- `I[Entity]Service`: Business logic operations
- `I[Entity]InfoService`: Read-only information services

## Design Patterns Used

1. **Factory Pattern**: Entity creation (`I*Factory` interfaces)
2. **Service Layer Pattern**: Business logic encapsulation
3. **Repository Pattern**: Data access abstraction
4. **Unit of Work Pattern**: Transaction management
5. **DTO Pattern**: ViewModels for data transfer
6. **Dependency Injection**: All dependencies via constructor injection

## Dependencies

- **ORM**: Entity Framework models and database context
- **Third-party Libraries**:
  - Ionic.Zip: File compression
  - Various NuGet packages (see packages.config)

## For AI Agents

When working in Core:
1. **Adding New Features**:
   - Create domain entity in `[Domain]/Domain/`
   - Define factory interface `I[Entity]Factory.cs`
   - Define service interface `I[Entity]Service.cs`
   - Implement in `[Domain]/Impl/`
   - Add ViewModels in `[Domain]/ViewModel/`
   - Add enums in `[Domain]/Enum/`

2. **Modifying Existing Features**:
   - Update domain entity first
   - Update service interface if needed
   - Modify implementation
   - Update ViewModels if data contract changes
   - Update dependent modules (Infrastructure, API)

3. **Business Rules**:
   - All validation logic belongs in Core
   - No database access directly (use IRepository)
   - No HTTP concerns (handled in API layer)
   - No UI concerns (handled in Web.UI)

4. **Testing Considerations**:
   - Core should be unit-testable
   - Mock IRepository for tests
   - Business logic should be pure (deterministic)

## For Human Developers

### Common Tasks

#### Adding a New Domain Entity
```csharp
// 1. Create entity in Domain folder
public class NewEntity : DomainBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    // ... properties
}

// 2. Create factory interface
public interface INewEntityFactory
{
    NewEntity Create(NewEntityViewModel model);
    void Update(NewEntity entity, NewEntityViewModel model);
}

// 3. Create service interface
public interface INewEntityService
{
    NewEntity Get(int id);
    IEnumerable<NewEntity> GetAll();
    void Save(NewEntity entity);
    void Delete(int id);
}

// 4. Implement in Impl folder
public class NewEntityFactory : INewEntityFactory
{
    // Implementation
}

// 5. Create ViewModel
public class NewEntityViewModel
{
    // Properties for API communication
}
```

#### Best Practices
- Use async/await for I/O operations
- Validate inputs in factory methods
- Throw domain-specific exceptions (Application/Exceptions)
- Log important operations via ILogService
- Use enums instead of magic strings/numbers
- Keep business logic pure (no side effects)

### File Organization
```
Core/
├── [Domain]/
│   ├── Domain/              # Entity models
│   ├── Impl/                # Service implementations
│   ├── Enum/                # Enumerations
│   └── ViewModel/           # DTOs
├── Application/             # Core utilities
│   ├── Domain/              # Base classes
│   ├── Impl/                # Utility implementations
│   ├── Exceptions/          # Custom exceptions
│   └── Extensions/          # Extension methods
└── Properties/              # Assembly info
```

## Configuration

- **app.config**: Application settings
- **packages.config**: NuGet dependencies
- **StyleCop.Cache**: Code style enforcement

## Testing

While no test project is currently in Core, implementations should be:
- Unit testable (dependencies injected)
- Pure functions where possible
- Documented with XML comments for public APIs
