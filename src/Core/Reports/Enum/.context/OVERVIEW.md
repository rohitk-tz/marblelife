<!-- AUTO-GENERATED: Header -->
# Reports/Enum
> Strongly-typed enumeration constants for service pricing categories, customer acquisition tracking, and lookup table classification
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Enum subfolder contains **compile-time constants** that replace magic numbers throughout the Reports module. These enumerations serve two purposes:

1. **Database Key Mapping** — Integer values correspond to specific database table primary keys
2. **Type Safety** — Prevent invalid values and typos in business logic

Think of these as a **contract between code and database**: the enum values must match specific database IDs, ensuring referential integrity and semantic clarity.

### Why Enums Instead of Lookup Tables?

The system uses a **hybrid approach**:
- **Enums** for critical business constants that never change (service pricing types, core categories)
- **Lookup Tables** for dynamic values that admins can modify (franchisee list, payment methods)

**Enums Win When**:
- Value set is stable (adding new service pricing type is rare, requires business process change)
- Compile-time validation needed (prevent typos like `serviceTagId == 283` when you meant `282`)
- Performance matters (no database join required)

**Lookup Tables Win When**:
- Admins need to add values via UI without code changes
- Value set varies by environment (dev vs prod)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Service Tag Category in Price Calculations

```csharp
using Core.Reports.Enum;

public decimal CalculateServiceCost(PriceEstimateViewModel priceEstimate, decimal quantity)
{
    switch ((ServiceTagCategory)priceEstimate.CategoryId)
    {
        case ServiceTagCategory.AREA:
            // Square footage pricing: base price + (additional price * overage)
            var threshold = 500m;  // First 500 sqft
            if (quantity <= threshold)
                return priceEstimate.FranchiseeCorporatePrice ?? 0;
            else
                return (priceEstimate.FranchiseeCorporatePrice ?? 0) 
                     + ((quantity - threshold) * (priceEstimate.FranchiseeAdditionalCorporatePrice ?? 0));
        
        case ServiceTagCategory.LINEARFT:
            // Linear footage pricing: similar to AREA
            var linearThreshold = 100m;  // First 100 linear ft
            if (quantity <= linearThreshold)
                return priceEstimate.FranchiseeCorporatePrice ?? 0;
            else
                return (priceEstimate.FranchiseeCorporatePrice ?? 0)
                     + ((quantity - linearThreshold) * (priceEstimate.FranchiseeAdditionalCorporatePrice ?? 0));
        
        case ServiceTagCategory.EVENT:
            // Flat per-event pricing (quantity ignored)
            return priceEstimate.FranchiseeCorporatePrice ?? 0;
        
        case ServiceTagCategory.TIME:
            // Hourly pricing
            return (priceEstimate.FranchiseeCorporatePrice ?? 0) * quantity;
        
        case ServiceTagCategory.MAINTAINANCE:
            // Monthly maintenance fee (quantity = months)
            return (priceEstimate.FranchiseeCorporatePrice ?? 0) * quantity;
        
        case ServiceTagCategory.PRODUCTPRICE:
            // Product retail price * quantity
            return (priceEstimate.FranchiseeCorporatePrice ?? 0) * quantity;
        
        case ServiceTagCategory.TAXRATE:
            // Tax rate lookup (not a billable service)
            throw new InvalidOperationException("TAXRATE is not a billable service");
        
        default:
            throw new ArgumentException($"Unknown service tag category: {priceEstimate.CategoryId}");
    }
}
```

### Example 2: Customer Acquisition Source Filtering

```csharp
using Core.Reports.Enum;

public class CustomerAnalyticsService
{
    private readonly IRepository<Customer> _customerRepo;
    
    public int GetJobEstimateCustomerCount(DateTime startDate, DateTime endDate)
    {
        return _customerRepo.Table
            .Count(x => x.CreatedDate >= startDate 
                     && x.CreatedDate <= endDate
                     && x.CustomerFromId == (long)CustomerComeFromCategory.JobEstimate);
    }
    
    // When full enumeration is migrated, this becomes:
    public Dictionary<string, int> GetCustomerCountBySource(DateTime startDate, DateTime endDate)
    {
        return _customerRepo.Table
            .Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
            .GroupBy(x => x.CustomerFromId)
            .Select(g => new 
            { 
                Source = (CustomerComeFromCategory)g.Key, 
                Count = g.Count() 
            })
            .ToDictionary(
                x => x.Source.ToString(), 
                x => x.Count
            );
    }
}
```

### Example 3: Querying Lookup Table by Category

```csharp
using Core.Reports.Enum;

public class LookupService
{
    private readonly IRepository<Lookup> _lookupRepo;
    
    public List<LookupViewModel> GetServiceTagCategories()
    {
        // Get all service tag categories from Lookup table
        return _lookupRepo.Table
            .Where(x => x.LookupTypeId == (long)LookUpTypeCategory.ListOfServiceTag)
            .Select(x => new LookupViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToList();
    }
    
    public List<LookupViewModel> GetUnitsOfMeasure()
    {
        // Get all units (sqft, linear ft, hour, event)
        return _lookupRepo.Table
            .Where(x => x.LookupTypeId == (long)LookUpTypeCategory.Units)
            .OrderBy(x => x.Name)
            .Select(x => new LookupViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();
    }
    
    public List<LookupViewModel> GetCustomerAcquisitionSources()
    {
        // Get all customer acquisition sources
        return _lookupRepo.Table
            .Where(x => x.LookupTypeId == (long)LookUpTypeCategory.CustomerFrom)
            .Select(x => new LookupViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();
    }
}
```

### Example 4: Validating Enum Values

```csharp
using Core.Reports.Enum;

public class PriceEstimateValidator
{
    public bool IsValidServiceTagCategory(long categoryId)
    {
        // Check if value matches any ServiceTagCategory enum
        return Enum.IsDefined(typeof(ServiceTagCategory), (int)categoryId);
    }
    
    public string GetServiceTagCategoryName(long categoryId)
    {
        if (!Enum.IsDefined(typeof(ServiceTagCategory), (int)categoryId))
            return "Unknown";
        
        return ((ServiceTagCategory)categoryId).ToString();
    }
    
    // Usage in validation logic
    public ValidationResult ValidatePriceEstimate(PriceEstimateViewModel model)
    {
        if (!IsValidServiceTagCategory(model.CategoryId))
        {
            return ValidationResult.Fail($"Invalid service tag category: {model.CategoryId}. " +
                $"Must be one of: {string.Join(", ", Enum.GetNames(typeof(ServiceTagCategory)))}");
        }
        
        // Additional validation based on category
        var category = (ServiceTagCategory)model.CategoryId;
        if (category == ServiceTagCategory.AREA || category == ServiceTagCategory.LINEARFT)
        {
            if (model.FranchiseeAdditionalCorporatePrice == null)
                return ValidationResult.Fail("Additional price required for AREA/LINEARFT pricing");
        }
        
        return ValidationResult.Success();
    }
}
```

### Example 5: Reporting Service Tag Distribution

```csharp
using Core.Reports.Enum;

public class ServiceAnalyticsService
{
    private readonly IRepository<PriceEstimateServices> _priceEstimateRepo;
    
    public Dictionary<string, int> GetServiceCountByCategory()
    {
        // Count active services by pricing category
        var services = _priceEstimateRepo.Table
            .Where(x => x.IsActive)
            .GroupBy(x => x.ServiceTag.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                Count = g.Count()
            })
            .ToList();
        
        // Convert to human-readable names
        return services.ToDictionary(
            x => Enum.GetName(typeof(ServiceTagCategory), (int)x.CategoryId) ?? "Unknown",
            x => x.Count
        );
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Enum Value Not Found in Database
**Symptom**: Exception "Invalid cast from 'Int64' to 'ServiceTagCategory'"

**Solution**: Enum value doesn't match database ID
- Query database: `SELECT Id, Name FROM ServiceTag WHERE Id IN (282, 283, 284, 285, 286, 287, 288)`
- Verify IDs match enum values
- If mismatch, update enum to match database (enums follow database, not vice versa)

### Adding New Service Tag Category
**Scenario**: Business wants to add "SUBSCRIPTION" pricing model

**Steps**:
1. Insert into database: `INSERT INTO ServiceTag (Id, Name, CategoryId) VALUES (289, 'Subscription', 44)`
2. Update enum:
```csharp
public enum ServiceTagCategory
{
    AREA = 282,
    LINEARFT = 283,
    EVENT = 284,
    TIME = 285,
    MAINTAINANCE = 286,
    PRODUCTPRICE = 287,
    TAXRATE = 288,
    SUBSCRIPTION = 289  // New
}
```
3. Update price calculation switch statement to handle new category
4. Deploy code + database migration together

### CustomerComeFromCategory Missing Values
**Symptom**: Customer has `CustomerFromId = 301`, but enum only defines `300`

**Solution**: Partial enum migration in progress
- Use fallback logic:
```csharp
if (Enum.IsDefined(typeof(CustomerComeFromCategory), (int)customer.CustomerFromId))
{
    var source = (CustomerComeFromCategory)customer.CustomerFromId;
    // Handle migrated enum value
}
else
{
    // Fallback to Lookup table for unmigrated values
    var source = _lookupRepo.GetById(customer.CustomerFromId);
}
```

### LookupTypeCategory vs ServiceTagCategory Confusion
**Symptom**: Querying `WHERE CategoryId = 44` returns no results

**Issue**: Mixing up `LookUpTypeCategory` (meta-category for Lookup table) vs `ServiceTagCategory` (actual service pricing types)

**Clarification**:
- `LookUpTypeCategory.ListOfServiceTag = 44` → This is a **LookupType** ID
- `ServiceTagCategory.AREA = 282` → This is a **ServiceTag** ID (which has `CategoryId = 44`)

**Correct Query**:
```csharp
// Get all service tags that have CategoryId = ListOfServiceTag
var serviceTags = _serviceTagRepo.Table
    .Where(x => x.CategoryId == (long)LookUpTypeCategory.ListOfServiceTag)
    .ToList();
```

<!-- END CUSTOM SECTION -->
