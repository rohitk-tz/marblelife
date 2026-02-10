<!-- AUTO-GENERATED: Header -->
# Attribute
> Custom .NET attributes for dependency injection, validation, entity cascading, and Excel export configuration
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Attribute** module contains five custom .NET attributes that declaratively control application behavior. Think of these as "instruction stickers" you attach to classes and properties - the framework reads these stickers and adjusts how it processes those elements.

**Why these attributes exist:**
- **Reduce Boilerplate**: Instead of manually registering validators or configuring Excel columns in code, attributes let you declare intent right where the type is defined
- **Convention over Configuration**: Most classes follow default rules, but attributes let you opt out (e.g., "don't validate this DTO")
- **Type Safety**: Compile-time attribute application prevents runtime configuration errors

**Key Use Cases:**
1. **Dependency Injection**: `DefaultImplementationAttribute` auto-registers interface implementations
2. **Validation Control**: `NoValidatorRequired` and `NoValidationResolveAtStartAttribute` manage validator lifecycle
3. **ORM Cascading**: `CascadeEntityAttribute` defines parent-child entity relationships
4. **Data Export**: `DownloadFieldAttribute` controls Excel column formatting and inclusion
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Skip Validation for a Simple DTO
```csharp
using Core.Application.Attribute;

[NoValidatorRequired]  // Don't look for a FileModelValidator
public class FileModel : EditModelBase
{
    public long Id { get; set; }
    public string Name { get; set; }
    // Properties are simple and pre-validated on client side
}
```

### Example 2: Configure Entity Cascading
```csharp
using Core.Application.Attribute;

public class Invoice : DomainBase
{
    // When Invoice is deleted, all LineItems are also deleted (composite ownership)
    [CascadeEntity(isComposite: true, isCollection: true)]
    public List<InvoiceLineItem> LineItems { get; set; }
    
    // When Invoice is deleted, Customer is NOT deleted (just a reference)
    [CascadeEntity(isComposite: false)]
    public Customer Customer { get; set; }
}
```

### Example 3: Customize Excel Export Columns
```csharp
using Core.Application.Attribute;

public class InvoiceReportModel
{
    [DownloadField(displayName: "Invoice #", currencyType: null)]
    public string InvoiceNumber { get; set; }
    
    [DownloadField(displayName: "Total Amount", currencyType: "USD")]
    public decimal Total { get; set; }
    
    [DownloadField(required: false)]  // Omit column if all values are null
    public string Notes { get; set; }
}
```

### Example 4: Mark Default Implementation
```csharp
using Core.Application.Attribute;

[DefaultImplementation]
public class FileService : IFileService
{
    public Type Interface { get; } = typeof(IFileService);
    
    // DI container automatically registers FileService as IFileService
}
```

### Example 5: Defer Validator Registration
```csharp
using Core.Application.Attribute;
using FluentValidation;

// Don't register this validator at startup (only used in admin area)
[NoValidationResolveAtStart]
public class AdminUserValidator : AbstractValidator<AdminUserModel>
{
    public AdminUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Attribute | Applied To | Purpose |
|-----------|------------|---------|
| `NoValidatorRequiredAttribute` | Classes | Skip automatic validator registration for this type |
| `CascadeEntityAttribute` | Entity properties | Configure ORM cascade behavior (isComposite, isCollection) |
| `DownloadFieldAttribute` | ViewModel properties | Control Excel export (column name, currency format, required) |
| `NoValidationResolveAtStartAttribute` | Validator classes | Defer validator registration until explicitly needed |
| `DefaultImplementationAttribute` | Implementation classes | Mark as default provider for an interface in DI |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Validator still runs despite NoValidatorRequired
**Cause**: The attribute only affects automatic registration. Manual `serviceCollection.AddValidator<T>()` calls override it.
**Solution**: Remove manual validator registration or remove the attribute.

### Issue: CascadeEntity doesn't cascade deletes
**Cause**: `IsComposite` might be set to false, or the repository implementation doesn't support attribute-based cascading.
**Solution**: Verify `IsComposite = true` and check that your repository layer reads this attribute.

### Issue: DownloadField properties missing from Excel export
**Cause**: The ExcelFileCreator might be filtering properties based on `Required = false` and null values.
**Solution**: Either provide non-null values or set `Required = true` to force column inclusion.

### Issue: DefaultImplementation not registering in DI
**Cause**: The `Interface` property might not be set, or the DI scanning logic isn't reading this attribute.
**Solution**: Ensure `Interface` property is set correctly and that SessionFactory or DI setup scans for this attribute.
<!-- END CUSTOM SECTION -->
