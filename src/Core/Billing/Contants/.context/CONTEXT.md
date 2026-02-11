<!-- AUTO-GENERATED: Header -->
# Contants — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2026-02-10T12:21:23Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
This module defines service type classification constants used throughout the billing system to categorize franchise services into product families. It provides a centralized mapping between service names and their corresponding business units (Stone Restoration, Grout Restoration, Concrete, Vinyl).

### Design Patterns
- **Static Class Pattern**: Immutable service type aliases shared across the application to ensure consistency
- **Array-based Lookup**: Groups related service names into readonly arrays for easy iteration and validation

### Data Flow
1. Service type strings are defined at compile-time as readonly arrays
2. Business logic queries `ServiceTypeAlias` to determine which product family a service belongs to
3. Billing calculations, reporting, and invoice categorization use these mappings to group revenue by business unit
4. No runtime modification — these are fixed configuration values
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

```csharp
namespace Core.Billing.Contants
{
    /// <summary>
    /// Static lookup table mapping service names to their business unit categories.
    /// Used for revenue classification, reporting, and product-specific billing logic.
    /// </summary>
    public static class ServiceTypeAlias
    {
        /// <summary>
        /// Stone Restoration business unit services
        /// </summary>
        public static readonly string[] StoneLife = { "Stone Restoration" };
        
        /// <summary>
        /// Grout/Tile business unit services including restoration, maintenance, and specialty coatings
        /// </summary>
        public static readonly string[] Groutlife = { 
            "Grout Restoration", 
            "Grout Maintenance", 
            "Colorseal",      // Grout color sealing service
            "TileLok"         // Tile protection service
        };
        
        /// <summary>
        /// Concrete business unit services
        /// Note: Contains typo "Concerte Maintenance" in original implementation
        /// </summary>
        public static readonly string[] Enduracrete = { 
            "Concrete", 
            "Concrete Restoration", 
            "Concerte Maintenance"  // WARNING: Typo in source - "Concerte" vs "Concrete"
        };
        
        /// <summary>
        /// Vinyl business unit services
        /// Note: Inconsistent casing "maintenance" vs "Restoration"
        /// </summary>
        public static readonly string[] VinylGuard = { 
            "Vinyl", 
            "Vinyl Restoration", 
            "Vinyl maintenance"  // WARNING: Lowercase "maintenance" (inconsistent casing)
        };
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `ServiceTypeAlias.StoneLife`
- **Type**: `string[]` (readonly)
- **Values**: `["Stone Restoration"]`
- **Purpose**: Identifies stone restoration services for billing categorization
- **Usage**: Check if a service belongs to StoneLife business unit

### `ServiceTypeAlias.Groutlife`
- **Type**: `string[]` (readonly)
- **Values**: `["Grout Restoration", "Grout Maintenance", "Colorseal", "TileLok"]`
- **Purpose**: Identifies grout/tile services including specialty treatments
- **Usage**: Revenue reporting for grout-related service lines

### `ServiceTypeAlias.Enduracrete`
- **Type**: `string[]` (readonly)
- **Values**: `["Concrete", "Concrete Restoration", "Concerte Maintenance"]`
- **Purpose**: Identifies concrete services
- **Warning**: Contains typo "Concerte" — may cause service lookup failures

### `ServiceTypeAlias.VinylGuard`
- **Type**: `string[]` (readonly)
- **Values**: `["Vinyl", "Vinyl Restoration", "Vinyl maintenance"]`
- **Purpose**: Identifies vinyl services
- **Warning**: Inconsistent casing ("maintenance" lowercase) — may cause case-sensitive lookup issues
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

**Internal**: None — this is a leaf module with no dependencies

**External**: 
- `System` namespace (implicit for basic types)
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Known Issues
1. **Typo in Enduracrete**: "Concerte Maintenance" should likely be "Concrete Maintenance"
2. **Casing Inconsistency**: "Vinyl maintenance" uses lowercase while others use titlecase
3. **Missing Services**: May not cover all franchise services (e.g., Wood restoration, Travertine)

### Usage Patterns
```csharp
// Check if service belongs to business unit
bool isGroutService = ServiceTypeAlias.Groutlife.Contains(serviceName);

// Categorize invoice items by business unit
var groutTotal = invoiceItems
    .Where(i => ServiceTypeAlias.Groutlife.Contains(i.ServiceType))
    .Sum(i => i.Amount);

// Reporting by product line
foreach (var serviceType in ServiceTypeAlias.Enduracrete)
{
    var revenue = CalculateRevenue(serviceType);
    report.Add(serviceType, revenue);
}
```

### Recommendations
1. **Fix Typos**: Correct "Concerte" → "Concrete" and standardize casing
2. **Consider Enum**: Replace string arrays with enum for type safety
3. **Extensibility**: Consider loading from configuration for easier updates
4. **Case-Insensitive Lookups**: Always use `StringComparer.OrdinalIgnoreCase` when matching
<!-- END CUSTOM SECTION -->
