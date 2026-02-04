# Core/Application/Attribute - AI Context

## Purpose

This folder contains custom C# attributes used across the MarbleLife application for metadata, validation, authorization, and behavior modification.

## Contents

Custom attribute classes for:
- **Validation Attributes**: Custom validation logic
- **Authorization Attributes**: Access control and permissions
- **Metadata Attributes**: Descriptive information for properties
- **Behavior Attributes**: Modify runtime behavior
- **API Documentation Attributes**: Swagger/API metadata

## Common Attributes

### Validation
- Custom validators extending `ValidationAttribute`
- Complex validation rules not covered by standard DataAnnotations
- Cross-property validation

### Authorization
- Role-based access attributes
- Resource-level permission attributes
- Franchisee-specific access control

### Metadata
- Display formatting attributes
- JSON serialization control
- Database mapping hints

## For AI Agents

**Creating Custom Attribute**:
```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CustomValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        // Validation logic
        return true;
    }
    
    public override string FormatErrorMessage(string name)
    {
        return $"{name} failed custom validation";
    }
}
```

**Using Custom Attributes**:
```csharp
public class MyViewModel
{
    [CustomValidation]
    [RequiredIf("OtherProperty", "SomeValue")]
    public string PropertyName { get; set; }
}
```

## For Human Developers

Best practices for custom attributes:
- Inherit from appropriate base attribute class
- Use AttributeUsage to specify where attribute can be applied
- Make attributes reusable and configurable
- Provide meaningful error messages
- Test attributes thoroughly
- Document attribute parameters and behavior
