# Infrastructure/Application - AI Context

## Purpose

This folder contains infrastructure implementations for core application services, including repository patterns, data access, and utility service implementations.

## Contents

Infrastructure implementations:
- Repository pattern implementations
- Unit of Work implementations
- Data access utilities
- External service integrations
- File handling implementations

## Structure

- **Impl/**: Concrete implementations of Core/Application interfaces

## For AI Agents

**Repository Implementation Pattern**:
```csharp
public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public T Get(int id)
    {
        return _dbSet.Find(id);
    }
    
    public IEnumerable<T> GetAll()
    {
        return _dbSet.Where(x => !((dynamic)x).IsDeleted).ToList();
    }
    
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }
    
    public void Delete(T entity)
    {
        // Soft delete
        ((dynamic)entity).IsDeleted = true;
        _context.Entry(entity).State = EntityState.Modified;
    }
}
```

## For Human Developers

Infrastructure layer implements Core interfaces with actual data access logic.

### Best Practices:
- Implement interfaces from Core/Application
- Use Entity Framework for database operations
- Handle soft deletes at repository level
- Include proper error handling
- Log data access operations
- Use async/await for I/O operations
