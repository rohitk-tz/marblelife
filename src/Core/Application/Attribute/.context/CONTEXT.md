<!-- AUTO-GENERATED: Header -->
# Attribute — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Provides custom .NET attributes that control dependency injection behavior, validation conventions, entity cascading rules, and Excel file download field metadata. These attributes are applied declaratively to classes and properties throughout the application to configure runtime behavior without code duplication.

### Design Patterns
- **Metadata Attributes**: All classes inherit from `System.Attribute` to leverage .NET's declarative programming model
- **Convention-over-Configuration**: Attributes mark exceptions to default behaviors (e.g., `NoValidatorRequired` opts out of validation)
- **Reflection-Based Discovery**: Attributes are discovered at runtime via reflection to configure DI containers, validators, and data exporters

### Data Flow
1. Attributes are applied to classes/properties at compile time
2. Framework code (DI container, validation pipeline, Excel exporters) discovers attributes via reflection
3. Behavior is modified based on attribute presence and property values
4. No direct instantiation - attributes are metadata read by other components
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### NoValidatorRequiredAttribute.cs
```csharp
// Marks a class to skip automatic validator registration in the DI container
public class NoValidatorRequiredAttribute : System.Attribute
{
    // Marker attribute - no properties
}
```

### CascadeEntityAttribute.cs
```csharp
// Marks entity properties that should be cascaded during save/delete operations
public class CascadeEntityAttribute : System.Attribute
{
    // Indicates if the property is a composite entity (owned by parent)
    public bool IsComposite { get; set; }
    
    // Indicates if the property is a collection of entities
    public bool IsCollection { get; set; }
    
    // Constructor: CascadeEntityAttribute(bool isComposite, bool isCollection = false)
    // Default constructor sets isComposite = false
}
```

### DownloadFieldAttribute.cs
```csharp
// Configures how a property should be exported to Excel downloads
public class DownloadFieldAttribute : System.Attribute
{
    public bool IsCollection { get; set; }      // Property contains multiple values
    public bool IsComplexType { get; set; }     // Property is a complex object (not primitive)
    public bool Required { get; set; }          // Field must have a value in export
    public string DisplayName { get; set; }     // Custom column header name
    public string CurrencyType { get; set; }    // Currency formatting hint (e.g., "USD")
    
    // Multiple constructors support various configuration scenarios:
    // - DownloadFieldAttribute() - defaults Required = true
    // - DownloadFieldAttribute(bool required)
    // - DownloadFieldAttribute(bool isCollection, bool complexType)
    // - DownloadFieldAttribute(string displayName, string currencyType)
}
```

### NoValidationResolveAtStartAttribute.cs
```csharp
// Marks validators that should NOT be automatically resolved/registered at application startup
public class NoValidationResolveAtStartAttribute : System.Attribute
{
    // Marker attribute - no properties
    // Used for lazy-loaded or conditionally-registered validators
}
```

### DefaultImplementationAttribute.cs
```csharp
// Specifies the default implementation type for an interface in DI registration
public class DefaultImplementationAttribute : System.Attribute
{
    public Type Interface { get; set; }  // The interface type this implementation satisfies
    
    // Applied to implementation classes to auto-register them as default providers
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### NoValidatorRequiredAttribute
- **Applied To**: Classes (ViewModels, EditModels)
- **Purpose**: Prevents automatic FluentValidation validator discovery
- **Example**: `[NoValidatorRequired] public class FileModel : EditModelBase`
- **Side Effects**: Class is skipped during validator registration scan

### CascadeEntityAttribute
- **Applied To**: Entity properties (navigation properties)
- **Purpose**: Configures ORM cascading behavior for save/delete operations
- **Properties**:
  - `IsComposite`: If true, child entity lifecycle is bound to parent (delete parent → delete child)
  - `IsCollection`: If true, property contains multiple child entities (List<T>, IEnumerable<T>)
- **Example**: `[CascadeEntity(isComposite: true, isCollection: true)] public List<LineItem> Items { get; set; }`
- **Side Effects**: Repository layer uses this to determine cascade depth

### DownloadFieldAttribute
- **Applied To**: ViewModel/DTO properties
- **Purpose**: Controls Excel export behavior and column formatting
- **Properties**:
  - `Required`: If false, column is omitted if all values are null
  - `DisplayName`: Overrides property name for column header
  - `CurrencyType`: Applies currency formatting (e.g., "$1,234.56")
  - `IsComplexType`: If true, nested object properties are flattened into columns
  - `IsCollection`: If true, multiple rows are generated per item
- **Example**: `[DownloadField(displayName: "Total Amount", currencyType: "USD")] public decimal Total { get; set; }`
- **Side Effects**: ExcelFileCreator reads this to generate spreadsheet columns

### NoValidationResolveAtStartAttribute
- **Applied To**: Validator classes
- **Purpose**: Defers validator registration until explicitly requested
- **Use Case**: Validators for rarely-used models or those with expensive dependencies
- **Side Effects**: Class is excluded from startup DI scan, must be manually resolved

### DefaultImplementationAttribute
- **Applied To**: Implementation classes
- **Purpose**: Marks a class as the default provider for an interface
- **Property**: `Interface` - the interface type being implemented
- **Example**: `[DefaultImplementation] public class FileService : IFileService`
- **Side Effects**: DI container registers this as the default resolution for `Interface` type
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **System.Attribute** (BCL) — Base class for all custom attributes
- **System.ComponentModel.DataAnnotations** (implied) — Validation attribute patterns

### External Dependencies
None - pure .NET metadata types

### Referenced By
- **Core.Application.Impl.SessionFactory** — Reads `DefaultImplementationAttribute` during DI registration
- **Core.Application.Impl.ExcelFileCreator** — Reads `DownloadFieldAttribute` to configure column exports
- **Core.Application.ViewModel** — Many ViewModels use `NoValidatorRequiredAttribute`
- **Core.Application.Domain** — Domain entities use `CascadeEntityAttribute` on navigation properties
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Attribute Usage Patterns
1. **Validation Attributes** (`NoValidatorRequired`, `NoValidationResolveAtStart`):
   - Use `NoValidatorRequired` on DTOs that are pre-validated or have simple constraints
   - Use `NoValidationResolveAtStart` on validators for admin-only or rare operations

2. **Entity Cascade Attributes** (`CascadeEntityAttribute`):
   - Set `IsComposite = true` for owned entities (e.g., OrderLineItems owned by Order)
   - Set `IsComposite = false` for references (e.g., Order referencing Customer)
   - Always set `IsCollection = true` for `List<T>` or `IEnumerable<T>` properties

3. **Excel Export Attributes** (`DownloadFieldAttribute`):
   - Combine `DisplayName` and `CurrencyType` for financial reports
   - Set `Required = false` for optional columns to reduce noise
   - Use `IsComplexType = true` to flatten nested objects (e.g., `Address.Street`, `Address.City`)

### Common Pitfalls
- Forgetting `IsCollection = true` on collection properties leads to runtime errors in cascade logic
- Overusing `NoValidatorRequired` can bypass important business rule validation
- `DefaultImplementationAttribute.Interface` must be set explicitly; null value causes DI registration failure
<!-- END CUSTOM SECTION -->
