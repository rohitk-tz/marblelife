<!-- AUTO-GENERATED: Header -->
# Organizations/Enum — Type Enumerations
**Version**: 8b6d86e55b597ea3e4d50ae8311308d72fcc87df
**Generated**: 2025-02-10T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Type-safe enumeration definitions for organization module business logic. These enums provide strongly-typed constants for service types, fee categories, document classifications, payment frequencies, and status values. Most enums use specific integer values (not auto-incrementing) to maintain database compatibility and external system integrations (e.g., QuickBooks, ReviewPush).

### Design Patterns
- **Explicit Value Assignment**: All enums specify integer values to ensure database consistency
- **Lookup Integration**: Many enums map to Lookup table entries for runtime extensibility
- **Business Domain Modeling**: Enums represent business concepts, not technical states
- **Integration Anchoring**: Some values (like ServiceFeeType) match external system IDs

### Usage Context
- **Service Configuration**: ServiceType defines 38 service offerings (StoneLife, Counterlife, etc.)
- **Financial Categorization**: ServiceFeeType categorizes recurring/one-time charges
- **Document Management**: DocumentType classifies required franchisee documents
- **Billing Cycles**: PaymentFrequency defines royalty billing schedules
- **Status Tracking**: Various enums control workflow states
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: File Inventory -->
## Complete Enum Inventory (13 Files)

| File | Values | Purpose | Key Values |
|------|--------|---------|-----------|
| **ServiceType.cs** | 38 | Service offerings franchisees can provide | StoneLife(1), Enduracrete(2), Counterlife(5), Monthly(6), ColorSeal(10), WebSales(20-26), Admin(36), OTHER(37) |
| **ServiceFeeType.cs** | 7 | Categorizes service charges | Loan(171), Bookkeeping(172), PayrollProcessing(173), Recruiting(174), OneTimeProject(175), NationalCharge(176), SEOCharges(296) |
| **DocumentType.cs** | 13 | Required franchisee documents | W9(1), LoanAgreement(2), AnnualTaxFilling(3), FranchiseeContract(4), COI(5), License(8), TaxUpload(9), ResaleCertificate(11) |
| **DocumentCategory.cs** | ~5 | Document categorization grouping | (View file for specific values) |
| **DocumentReportType.cs** | ~5 | Document reporting classifications | (View file for specific values) |
| **PaymentFrequency.cs** | 5 | Billing cycle frequencies | Weekly(31), Monthly(32), TwiceAMonth(33), FirstWeek(297), SecondWeek(298) |
| **ServiceTypeCategory.cs** | ~4 | High-level service groupings | (View file for specific values) |
| **FranchiseeCallCategory.cs** | ~6 | Call center interaction types | (View file for specific values) |
| **FranchiseeNotes.cs** | ~4 | Note categorization | (View file for specific values) |
| **LeadPerformanceEnum.cs** | ~4 | Lead tracking metrics | (View file for specific values) |
| **LoanType.cs** | ~4 | Loan classification types | (View file for specific values) |
| **LanguageEnum.cs** | ~4 | Language preferences | (View file for specific values) |
| **OrganizationNames.cs** | 4 | Specific organization identifiers | MIDetroit(62), PAPittsburgh(64), PAPhiladelphia(42), MIGrandRapids(27) |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Critical Enums Detail -->
## Key Enum Definitions

### ServiceType.cs (38 Values)
Defines all service types franchisees can offer. Mix of service products, recurring maintenance, and sales channels.

```csharp
public enum ServiceType
{
    // Core Service Products
    StoneLife = 1,           // Stone restoration/sealing
    Enduracrete = 2,         // Concrete sealing
    Tilelok = 3,             // Tile restoration
    Vinylguard = 4,          // Vinyl floor protection
    Counterlife = 5,         // Countertop restoration
    ColorSeal = 10,          // Color sealing service
    CleanShield = 11,        // Protective coating
    Wood = 12,               // Wood floor care
    Cleanair = 13,           // Air quality services
    MetalLife = 15,          // Metal surface restoration
    CarpetLife = 16,         // Carpet cleaning/protection
    TileInstall = 17,        // Tile installation
    
    // Recurring Maintenance Plans
    Monthly = 6,             // Monthly maintenance contract
    BiMonthly = 7,           // Bi-monthly service
    Quarterly = 8,           // Quarterly service
    Other = 9,               // Custom service
    
    // Sales Channels (Web)
    Product = 18,            // Product sales
    SalesTax = 19,           // Tax line item
    WebMld = 20,             // MarbleLife.com direct sales
    WebFranchiseeSales = 21, // Franchisee web sales
    WebJet = 22,             // Jet.com sales
    WebWalmart = 23,         // Walmart.com sales
    WebAmazon = 24,          // Amazon sales
    WebAmazonPrime = 25,     // Amazon Prime sales
    WebAmazonCanada = 26,    // Amazon Canada sales
    
    // Other Categories
    Fabricators = 14,        // Fabrication services
    Hardware = 27,           // Hardware sales
    Retail = 28,             // Retail sales
    Testing = 29,            // Testing/samples
    MldWarehouse = 30,       // Warehouse operations
    Government = 31,         // Government contracts
    Hotel = 32,              // Hotel/hospitality contracts
    Admin = 36,              // Administrative
    OTHER = 37               // Catch-all
}
```

**Usage**: 
- FranchiseeServices.ServiceTypeId references these values
- Sales reports group by ServiceType
- Fee calculations may vary by service category

### ServiceFeeType.cs (7 Values)
Categorizes recurring and one-time service charges. Values match Lookup table IDs.

```csharp
public enum ServiceFeeType
{
    Loan = 171,                  // Franchisee loan payments
    Bookkeeping = 172,           // Bookkeeping service fee
    PayrollProcessing = 173,     // Payroll processing fee
    Recruiting = 174,            // Recruiting assistance fee
    OneTimeProject = 175,        // One-time project charges
    NationalCharge = 176,        // National account charges
    InterestAmount = 177,        // Loan interest charges
    VarBookkeeping = 178,        // Variable bookkeeping fee
    FRANCHISEETECHMAIL = 214,    // Technician email service
    PHONECALLCHARGES = 251,      // Phone service charges
    SEOCharges = 296             // SEO/marketing charges
}
```

**Usage**:
- FranchiseeServiceFee.ServiceFeeTypeId references these
- Determines calculation logic (fixed amount vs percentage)
- Controls activation/deactivation workflows (e.g., SEO can be toggled)

### DocumentType.cs (13 Values)
Required franchisee documentation for compliance and legal purposes.

```csharp
public enum DocumentType
{
    W9 = 1,                          // Tax form W9
    LoanAgreement = 2,               // Loan contract documents
    AnnualTaxFilling = 3,            // Annual tax returns
    FranchiseeContract = 4,          // Franchise agreement
    COI = 5,                         // Certificate of Insurance
    EMPLOYEEHANDBOOK = 6,            // Employee handbook
    HAZOMMANUAL = 7,                 // Hazardous materials manual
    LICENSE = 8,                     // Business license
    UPLOADTAXES = 9,                 // Tax document uploads
    FRANCHISEAGREEMENTSRENEWALS = 10,// Renewal agreements
    RESALECERTIFICATE = 11,          // Resale certificate
    NCA = 12,                        // Non-compete agreement
    NationalAccountAgreement = 13    // National account contracts
}
```

**Usage**:
- FranchiseeDocumentType.DocumentTypeId references these
- Document upload workflow validates against expected types
- Compliance reporting checks for required documents

### PaymentFrequency.cs (5 Values)
Defines billing cycle frequencies for royalty payments.

```csharp
public enum PaymentFrequency
{
    Weekly = 31,         // Weekly billing
    Monthly = 32,        // Standard monthly billing
    TwiceAMonth = 33,    // Bi-weekly billing
    FirstWeek = 297,     // First week of month billing
    SecondWeek = 298     // Second week of month billing
}
```

**Usage**:
- FeeProfile.PaymentFrequencyId references these (via Lookup table)
- Determines invoice generation schedule
- Affects royalty calculation period aggregation

### OrganizationNames.cs (4 Values)
Specific organization identifiers for legacy integrations.

```csharp
public enum OrganizationNames
{
    MIDetroit = 62,          // Detroit, Michigan franchisee
    PAPittsburgh = 64,       // Pittsburgh, Pennsylvania franchisee
    PAPhiladelphia = 42,     // Philadelphia, Pennsylvania franchisee
    MIGrandRapids = 27       // Grand Rapids, Michigan franchisee
}
```

**Usage**: Hardcoded references for specific business logic or reporting for these franchisees. Consider refactoring to database-driven configuration.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **Core.Application.Domain.Lookup** — Many enums stored as Lookup references for UI display names
- **Domain entities** — Referenced via `long TypeId` properties

### External
None - standard C# enumerations
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Value Assignment Strategy
**Why explicit values?**
1. **Database Stability**: Enum values stored in DB as integers; auto-increment would break on recompilation
2. **Integration Contracts**: Some values (ServiceFeeType) match external systems (QuickBooks, legacy DB)
3. **Lookup Synchronization**: Values correspond to Lookup table primary keys

### Best Practices
1. **Never Reuse Values**: Once assigned, an enum value is permanent (even if deprecated)
2. **Gap Handling**: Large gaps (e.g., 214, 251, 296 in ServiceFeeType) indicate intermediate values used elsewhere
3. **Naming Conventions**: 
   - PascalCase for enum names (ServiceType)
   - PascalCase for values (StoneLife, not STONE_LIFE)
   - Exception: All-caps for acronyms (COI, NCA, OTHER)

### Common Pitfalls
- **OrganizationNames Anti-Pattern**: Hardcoding specific organizations breaks scalability. Use configuration instead.
- **ServiceType Sprawl**: 38 values indicate domain complexity; consider hierarchical categorization
- **Enum vs Lookup**: Use enums for compile-time constants, Lookups for runtime extensibility (user-defined values)

### Extension Strategy
**Adding New Service Types**:
1. Add to ServiceType enum with next available integer
2. Update domain model validation
3. Add corresponding Lookup entry with matching ID
4. Update reporting/billing logic if special handling needed

**Adding New Document Types**:
1. Add to DocumentType enum
2. Configure in FranchiseeDocumentType for affected franchisees
3. Update compliance reports
<!-- END CUSTOM SECTION -->
