# Scheduler/Impl - AI Context

## Purpose

This folder contains the concrete implementations of service interfaces and factories for the **Scheduler** module. These classes contain the actual business logic.

## Contents

Implementation classes in this folder:
- Service implementations (I*Service → *Service)
- Factory implementations (I*Factory → *Factory)
- Business logic and validation
- Data access via repositories
- External service integration

## For AI Agents

**Implementation Pattern**:
```csharp
public class EntityService : IEntityService
{
    private readonly IRepository<Entity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogService _logService;
    
    public EntityService(
        IRepository<Entity> repository,
        IUnitOfWork unitOfWork,
        ILogService logService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logService = logService;
    }
    
    public Entity Get(int id)
    {
        // Implementation with logging, validation, etc.
    }
}
```

**Adding New Implementation**:
1. Create interface in parent folder
2. Implement interface in this folder
3. Register in DependencyInjection project
4. Add unit tests (if test project exists)

**Best Practices**:
- Use dependency injection for all dependencies
- Implement proper error handling
- Log important operations
- Use transactions for data modifications
- Validate all inputs
- Keep methods focused (Single Responsibility)
- Use async/await for I/O operations

## For Human Developers

Service implementations contain business logic. Follow these guidelines:
- Inject dependencies via constructor
- Use IRepository<T> for data access
- Wrap data changes in IUnitOfWork transactions
- Log exceptions and important operations
- Validate inputs before processing
- Return appropriate ViewModels from API-facing methods
- Keep business logic testable
