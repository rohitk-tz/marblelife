<!-- AUTO-GENERATED: Header -->
# Reports/Enum — Module Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-11T06:35:45Z
**File Count**: 3 enumeration files
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Enum subfolder defines **strongly-typed enumeration constants** for report categorization, service tag types, and lookup data classification. These enumerations provide compile-time type safety for database values and prevent magic number usage throughout the codebase. Integer values correspond to database primary keys in `Lookup` tables or hardcoded category identifiers.

### Design Patterns
- **Enumeration as Database Key**: Integer enum values match database table primary keys (IDs) for direct comparison without lookups
- **Type-Safe Constants**: Replace magic numbers (e.g., `if (serviceTagId == 282)`) with semantic names (`if (serviceTagId == ServiceTagCategory.AREA)`)
- **Namespace Segregation**: Enums isolated in dedicated namespace (`Core.Reports.Enum`) to avoid naming conflicts

### Data Flow
1. **Usage**: Business logic uses enum values for filtering, branching, validation
2. **Persistence**: Enum integer values stored directly in database columns
3. **Validation**: Enum types enforce valid value set at compile-time

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### ServiceTagCategory.cs
**Purpose**: Defines pricing unit types for service estimates (area-based, linear, event, hourly, product, tax)

```csharp
namespace Core.Reports.Enum
{
    public enum ServiceTagCategory
    {
        AREA = 282,            // Square footage pricing (e.g., floor restoration per sqft)
        LINEARFT = 283,        // Linear footage pricing (e.g., baseboard trim per linear ft)
        EVENT = 284,           // Per-event pricing (e.g., fixed price per job)
        TIME = 285,            // Hourly pricing (e.g., labor per hour)
        MAINTAINANCE = 286,    // Maintenance contract pricing (e.g., monthly service fee)
        PRODUCTPRICE = 287,    // Product retail pricing (e.g., sealant bottle price)
        TAXRATE = 288          // Tax rate lookup (not a service, but pricing component)
    }
}
```

**Key Business Rules**:
- Values 282-288 correspond to `ServiceTag.Id` records in database
- Used in price estimate calculations to determine unit of measure
- `AREA` and `LINEARFT` have base + additional pricing (first X units, then per-additional-unit)
- `EVENT` and `TIME` have flat pricing structure
- `TAXRATE` special case: not a service, references tax lookup table

**Usage Example**:
```csharp
if (serviceTag.CategoryId == (long)ServiceTagCategory.AREA)
{
    // Calculate: (basePrice * area) + (additionalPrice * (area - threshold))
    var cost = CalculateAreaBasedPrice(priceEstimate);
}
else if (serviceTag.CategoryId == (long)ServiceTagCategory.EVENT)
{
    // Flat per-event price
    var cost = priceEstimate.CorporatePrice;
}
```

### CustomerComeFromCategory.cs
**Purpose**: Tracks customer acquisition source for marketing ROI analysis

```csharp
namespace Core.Reports.Enum
{
    public enum CustomerComeFromCategory
    {
        JobEstimate = 300  // Customer acquired via job estimate request
    }
}
```

**Key Business Rules**:
- Value 300 matches `Lookup.Id` where `LookupTypeId = 47` (CustomerFrom)
- Additional values expected (Website, Referral, Advertisement) but not yet migrated from Lookup table
- Used in customer analytics reports to segment by acquisition channel
- Partial enumeration — full set still in dynamic Lookup table

**Expected Full Enumeration** (pending migration):
```csharp
public enum CustomerComeFromCategory
{
    JobEstimate = 300,
    Website = 301,
    Referral = 302,
    Advertisement = 303,
    RepeatCustomer = 304
}
```

### LookUpTypeCategory.cs
**Purpose**: Categorizes lookup table types for dynamic enumeration management

```csharp
namespace Core.Reports.Enum
{
    public enum LookUpTypeCategory
    {
        ListOfServiceTag = 44,   // Lookup type for service tag categories
        Units = 46,              // Lookup type for unit of measure (sqft, linear ft, hour)
        CustomerFrom = 47        // Lookup type for customer acquisition sources
    }
}
```

**Key Business Rules**:
- Values 44, 46, 47 correspond to `LookupType.Id` in database
- Used to query `Lookup` table: `WHERE LookupTypeId = (long)LookUpTypeCategory.Units`
- Acts as a "lookup of lookups" — categorizes dynamic enumeration data
- Prevents hardcoded lookup type IDs scattered throughout codebase

**Usage Example**:
```csharp
// Get all service tag categories dynamically
var serviceTagCategories = _lookupRepository.Table
    .Where(x => x.LookupTypeId == (long)LookUpTypeCategory.ListOfServiceTag)
    .ToList();

// Get all units of measure
var units = _lookupRepository.Table
    .Where(x => x.LookupTypeId == (long)LookUpTypeCategory.Units)
    .Select(x => new { x.Id, x.Name })
    .ToList();
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **None** — Enumerations are leaf-level types with no dependencies

### Database Dependencies
- **Lookup Table** — Enum integer values match `Lookup.Id` for specific `LookupTypeId` categories
- **ServiceTag Table** — `ServiceTagCategory` values match `ServiceTag.Id` primary keys

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Why Integer Values Match Database IDs
This is a **hybrid enumeration approach**:
- Hard-coded enums for **critical business constants** that never change (service tag categories, pricing types)
- Database `Lookup` table for **dynamic enumerations** that may expand (payment frequencies, notification types)
- Enum values intentionally match database IDs to avoid mapping overhead

**Advantages**:
- Compile-time type safety for critical values
- Direct database comparisons without joins: `WHERE ServiceTagId = 282` (no need to query Lookup table)
- Semantic code: `ServiceTagCategory.AREA` instead of magic number `282`

**Disadvantages**:
- Database schema must not reuse these ID values for other purposes
- Adding new service tag categories requires code change + deployment (can't add via admin UI)

### Partial Enumeration Pattern
`CustomerComeFromCategory` has only 1 value but represents a full set in Lookup table. This is a **migration in progress**:
1. Legacy system stored all acquisition sources in Lookup table
2. New system migrating critical values to enums for type safety
3. Interim state: partial enum + fallback to Lookup table for unmigrated values

**Query Pattern During Migration**:
```csharp
// Check if customer source is JobEstimate (migrated to enum)
if (customer.CustomerFromId == (long)CustomerComeFromCategory.JobEstimate)
{
    // Handle job estimate source
}
else
{
    // Fallback to Lookup table for other sources
    var source = _lookupRepository.GetById(customer.CustomerFromId);
}
```

### ServiceTagCategory vs Units
- `ServiceTagCategory` defines **pricing logic type** (area-based, event-based, hourly)
- `LookUpTypeCategory.Units` references **unit of measure** (sqft, linear ft, hour, event)
- Relationship: Each ServiceTagCategory may have multiple Units
  - AREA → sqft, square meter
  - LINEARFT → linear ft, linear meter
  - TIME → hour, day

### Tax Rate as Service Tag Category
`TAXRATE = 288` is an outlier:
- Not a billable service
- References tax lookup table for jurisdiction-based tax rates
- Included in ServiceTagCategory because tax is a **pricing component** in estimate calculations
- Used to retrieve correct tax rate: `SELECT TaxRate FROM Lookup WHERE Id = 288 AND Jurisdiction = @zip`

### Enum Naming Conventions
- `ServiceTagCategory` uses **ALL_CAPS** (legacy convention, likely from Java origins)
- `CustomerComeFromCategory` uses **PascalCase** (C# standard)
- `LookUpTypeCategory` uses **PascalCase**

Inconsistent naming suggests iterative development by different teams over time.

<!-- END CUSTOM SECTION -->
