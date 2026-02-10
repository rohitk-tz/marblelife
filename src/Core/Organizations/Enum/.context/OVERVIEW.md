# Organizations/Enum
> Type-safe enumerations for organization module business logic

## Quick Reference

**13 Enum Files**:
- **ServiceType** (38 values): Service offerings (StoneLife, Counterlife, Web sales, etc.)
- **ServiceFeeType** (7 values): Fee categories (Loan, Bookkeeping, SEO, etc.)
- **DocumentType** (13 values): Required documents (W9, COI, Contracts, etc.)
- **PaymentFrequency** (5 values): Billing cycles (Weekly, Monthly, etc.)
- **OrganizationNames** (4 values): Specific organization IDs (legacy)
- **Plus 8 more** for categorization and status tracking

## Why Explicit Values?

All enums use explicit integer values (not auto-increment) to:
1. Maintain database compatibility across deployments
2. Match external system IDs (QuickBooks, ReviewPush)
3. Synchronize with Lookup table primary keys

## Usage

```csharp
// Reference in domain entities
public long ServiceTypeId { get; set; }  // Stores enum value

// Cast from enum
var serviceTypeId = (long)ServiceType.StoneLife;  // = 1

// Use in queries
var fees = fees.Where(f => f.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges);
```

See [CONTEXT.md](CONTEXT.md) for complete enum definitions and value mappings.
