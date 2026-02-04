# MarketingLead/ViewModel - AI Context

## Purpose

This folder contains Data Transfer Objects (DTOs) / ViewModels for the **MarketingLead** module. These classes are used to transfer data between the API layer and client applications.

## Contents

ViewModel classes for:
- API request payloads
- API response data
- Data transfer between layers
- Client-side data binding
- Form submissions

## For AI Agents

**ViewModel Pattern**:
```csharp
public class EntityViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    // Navigation properties as IDs or nested ViewModels
    public int RelatedEntityId { get; set; }
    public RelatedEntityViewModel RelatedEntity { get; set; }
}
```

**Creating ViewModels**:
1. Mirror domain entity structure but simplified
2. Add validation attributes
3. Use primitive types where possible
4. Include only necessary fields (no sensitive data)
5. Use IDs for relationships, not full entities

**Best Practices**:
- ViewModels should be serializable (no circular references)
- Add DataAnnotations for validation
- Use appropriate data types for JSON serialization
- Don't expose internal implementation details
- Create separate ViewModels for Create/Update/Read if needed
- Include XML comments for API documentation
- Use [JsonProperty] for custom JSON field names if needed

## For Human Developers

ViewModels are the contract between API and clients:
- Keep ViewModels simple and focused
- Add validation attributes at this level
- Don't include business logic
- Use automapper or manual mapping to/from domain entities
- Version ViewModels if breaking changes needed
- Document all properties for API consumers
- Consider using separate models for different operations
