<!-- AUTO-GENERATED: Header -->
# Billing Constants
> Service type classification lookup tables for revenue categorization and reporting
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Contants** (Constants) module provides a centralized registry of service type classifications for the MarbleLife franchise billing system. Think of it as a "rosetta stone" that maps individual service names to their parent business units — StoneLife, Groutlife, Enduracrete, and VinylGuard.

**Why it exists**: Instead of hardcoding service-to-business-unit mappings throughout the codebase, this module provides a single source of truth. When generating reports, calculating business unit revenue, or categorizing invoice line items, the system queries these arrays to determine which "family" a service belongs to.

**Real-world analogy**: Like a grocery store's department mapping where "Milk", "Cheese", and "Yogurt" all belong to "Dairy" — this module maps "Grout Restoration", "Colorseal", and "TileLok" to the "Groutlife" business unit.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup
No installation required — this is a static class available anywhere in the Core.Billing namespace.

### Example: Check Service Category
```csharp
using Core.Billing.Contants;

public class InvoiceService
{
    public string GetBusinessUnit(string serviceName)
    {
        if (ServiceTypeAlias.StoneLife.Contains(serviceName))
            return "StoneLife";
        
        if (ServiceTypeAlias.Groutlife.Contains(serviceName))
            return "Groutlife";
        
        if (ServiceTypeAlias.Enduracrete.Contains(serviceName))
            return "Enduracrete";
        
        if (ServiceTypeAlias.VinylGuard.Contains(serviceName))
            return "VinylGuard";
        
        return "Unknown";
    }
}
```

### Example: Revenue Report by Business Unit
```csharp
using Core.Billing.Contants;
using System.Linq;

public decimal CalculateGroutlifeRevenue(List<InvoiceItem> items)
{
    return items
        .Where(item => ServiceTypeAlias.Groutlife.Contains(item.ServiceType))
        .Sum(item => item.Amount);
}

public Dictionary<string, decimal> GetRevenueByBusinessUnit(List<InvoiceItem> items)
{
    return new Dictionary<string, decimal>
    {
        ["StoneLife"] = items.Where(i => ServiceTypeAlias.StoneLife.Contains(i.ServiceType)).Sum(i => i.Amount),
        ["Groutlife"] = items.Where(i => ServiceTypeAlias.Groutlife.Contains(i.ServiceType)).Sum(i => i.Amount),
        ["Enduracrete"] = items.Where(i => ServiceTypeAlias.Enduracrete.Contains(i.ServiceType)).Sum(i => i.Amount),
        ["VinylGuard"] = items.Where(i => ServiceTypeAlias.VinylGuard.Contains(i.ServiceType)).Sum(i => i.Amount)
    };
}
```

### Example: Case-Insensitive Lookup (Recommended)
```csharp
using System;

public bool IsGroutService(string serviceName)
{
    // Use case-insensitive comparison due to "Vinyl maintenance" casing issue
    return ServiceTypeAlias.Groutlife
        .Any(s => s.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Property | Type | Values | Description |
|----------|------|--------|-------------|
| `StoneLife` | `string[]` | `["Stone Restoration"]` | Stone restoration services |
| `Groutlife` | `string[]` | `["Grout Restoration", "Grout Maintenance", "Colorseal", "TileLok"]` | Grout/tile services including specialty coatings |
| `Enduracrete` | `string[]` | `["Concrete", "Concrete Restoration", "Concerte Maintenance"]` | Concrete services (⚠️ contains typo) |
| `VinylGuard` | `string[]` | `["Vinyl", "Vinyl Restoration", "Vinyl maintenance"]` | Vinyl services (⚠️ inconsistent casing) |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Service not found in any category
**Symptom**: `GetBusinessUnit()` returns "Unknown" for a valid service
**Causes**:
1. Typo in service name ("Concerte" vs "Concrete")
2. Case sensitivity ("Vinyl maintenance" vs "Vinyl Maintenance")
3. Service not yet added to alias mappings

**Solution**:
```csharp
// Always use case-insensitive comparison
bool found = ServiceTypeAlias.Groutlife
    .Any(s => s.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

// Or normalize input before lookup
string normalized = serviceName.Trim();
bool found = ServiceTypeAlias.VinylGuard.Contains(normalized);
```

### Issue: Missing service types
**Symptom**: Some franchise services aren't categorized (e.g., Wood restoration, Travertine)
**Solution**: Extend the arrays or create new business unit properties:
```csharp
public static readonly string[] WoodCare = { "Wood Restoration", "Wood Maintenance" };
public static readonly string[] TravertineCare = { "Travertine Restoration", "Travertine Sealing" };
```

### Issue: Runtime modification needed
**Symptom**: Need to add services without recompiling
**Solution**: Consider refactoring to load from configuration:
```csharp
// Instead of readonly arrays, load from appsettings.json or database
public static Dictionary<string, string[]> ServiceCategories { get; set; }
```
<!-- END CUSTOM SECTION -->
