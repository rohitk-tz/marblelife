<!-- AUTO-GENERATED: Header -->
# Organizations/Domain — Entity Definitions
**Version**: 8b6d86e55b597ea3e4d50ae8311308d72fcc87df
**Generated**: 2025-02-10T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Domain entities represent the persistent data model for franchisee organizations. These are EF Core entities that map directly to database tables, defining relationships, constraints, and cascade behaviors. This folder contains 36 entity classes covering:
- Core organization/franchisee structure
- Financial profiles and fee calculations
- Sales tracking and reporting
- Service fee management
- Document management
- Lead performance tracking
- Review/feedback integration

### Design Patterns
- **Entity Framework Core Entities**: All inherit from `DomainBase` (provides Id, audit fields)
- **Foreign Key Relationships**: Explicit `[ForeignKey]` attributes for navigation properties
- **Cascade Operations**: `[CascadeEntity]` attribute marks child entities for automatic cascade save/delete
- **Shared Primary Key Pattern**: Franchisee shares Id with Organization (one-to-one relationship)
- **Lookup Pattern**: Many enums stored as Lookup references for extensibility

### Entity Relationships
```
Organization (base entity)
    ↓ 1:1 (shared PK)
Franchisee
    ├─→ 1:1 FeeProfile
    │       └─→ 1:N RoyaltyFeeSlabs (tiered rates)
    ├─→ 1:1 LateFee
    ├─→ 1:N FranchiseeServices (enabled service types)
    ├─→ 1:N FranchiseeServiceFee (recurring/one-time charges)
    ├─→ 1:N FranchiseeSales (sales records)
    ├─→ 1:N FranchiseeAccountCredit (credit adjustments)
    ├─→ 1:N FranchiseeNotes (internal notes)
    ├─→ 1:N FranchiseeDocumentType (document configurations)
    └─→ 1:N FranchiseeInvoice (billing records)
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: File Inventory -->
## Complete File Inventory (36 Entities)

### Core Organization Entities
| File | Lines | Purpose |
|------|-------|---------|
| **organization.cs** | 49 | Base organization entity - name, type, contact info, addresses, phones |
| **OrganizationRoleUser.cs** | ~40 | Links users to organizations with role assignments |
| **Franchisee.cs** | 118 | Main franchisee entity extending organization with billing, services, contacts |

### Fee & Billing Entities
| File | Purpose |
|------|---------|
| **FeeProfile.cs** | Royalty calculation configuration (sales-based vs fixed, minimums, ad fund %) |
| **RoyaltyFeeSlabs.cs** | Tiered royalty rates: MinValue-MaxValue range → ChargePercentage |
| **MinRoyaltyFeeSlabs.cs** | Minimum royalty tier configuration |
| **LateFee.cs** | Late payment fee configuration per franchisee |
| **FranchiseeServiceFee.cs** | Recurring/one-time service charges (loan, bookkeeping, SEO, etc.) |
| **OneTimeProjectFee.cs** | One-time project charge records |
| **OnetimeprojectfeeAddFundRoyality.cs** | One-time fee add-fund and royalty tracking |
| **FranchiseeLoan.cs** | Loan details: principal, interest rate, payment schedule |
| **FranchiseeLoanSchedule.cs** | Loan amortization schedule entries |
| **LoanadjustmentAudit.cs** | Audit trail for loan adjustments |
| **TaxRates.cs** | Tax rate configuration |
| **ShiftCharges.cs** | Shift-based charge configuration |
| **MaintenanceCharges.cs** | Maintenance fee configuration |
| **ReplacementCharges.cs** | Equipment replacement charge configuration |
| **FloorGrindingAdjustment.cs** | Floor grinding service price adjustments |
| **FloorGrindingAdjustmentNotes.cs** | Notes for floor grinding adjustments |

### Sales & Revenue Entities
| File | Purpose |
|------|---------|
| **FranchiseeSales.cs** | Individual sales transaction records (customer, amount, invoice, marketing class) |
| **SalesDataMailReminder.cs** | Email reminders for sales data submission |
| **FranchiseeAccountCredit.cs** | Credit adjustments applied to franchisee accounts |

### Service Management Entities
| File | Purpose |
|------|---------|
| **FranchiseeServices.cs** | Services enabled for franchisee (StoneLife, Counterlife, etc.) |
| **ServiceType.cs** | Service type definitions and metadata |
| **ServicesTag.cs** | Tagging system for services |
| **PriceEstimateServices.cs** | Service pricing for estimates |

### Document Management Entities
| File | Purpose |
|------|---------|
| **FranchiseDocument.cs** | Uploaded documents (contracts, tax forms, compliance) |
| **DocumentType.cs** | Document category definitions |
| **FranchiseeDocumentType.cs** | Document types enabled per franchisee |

### Lead Performance & Marketing Entities
| File | Purpose |
|------|---------|
| **LeadPerformanceDetails.cs** | Lead generation and conversion tracking |
| **ReviewPushAPILocation.cs** | ReviewPush integration - API location mapping |
| **ReviewPushCustomerFeedback.cs** | Customer feedback collected via ReviewPush |

### History & Audit Entities
| File | Purpose |
|------|---------|
| **FranchiseeNotes.cs** | Internal notes about franchisee (call center, owner notes) |
| **FranchiseeRegistrationHistry.cs** | Historical registration data snapshots |
| **FranchiseeDurationNotesHistry.cs** | Duration/contract extension history |
| **Perpetuitydatehistry.cs** | Perpetuity date change history |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Key Entities Detail -->
## Critical Entity Definitions

### organization.cs
Base entity for all organizations (corporate, franchisees, partners).

```csharp
public class Organization : DomainBase
{
    public string Name { get; set; }
    public long DataRecorderMetaDataId { get; set; }  // Audit metadata
    public long TypeId { get; set; }  // Organization type (franchisee, corporate, etc.)
    public string About { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public string DeactivationnNote { get; set; }  // Reason for deactivation
    
    // Navigation Properties
    public virtual Lookup Type { get; set; }
    [CascadeEntity] public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    [CascadeEntity(IsCollection = true)] public virtual ICollection<Address> Address { get; set; }
    [CascadeEntity(IsCollection = true)] public virtual ICollection<Phone> Phones { get; set; }
    [CascadeEntity] public virtual Franchisee Franchisee { get; set; }  // One-to-one
}
```

### Franchisee.cs (118 lines)
Extends Organization with franchisee-specific properties. **Shared Primary Key**: Id = Organization.Id.

**Key Properties**:
- **Identity**: BusinessId (unique), QuickBookIdentifier, DisplayName, EIN, LegalEntity
- **Financial**: Currency, RenewalFee, TransferFee, OriginalFranchiseeFee, RenewalDate
- **Multiple Contact Types**: Contact, AccountPerson, MarketingPerson, Scheduler (each with FirstName/LastName/Email)
- **Features**: IsReviewFeedbackEnabled, SetGeoCode (geocoding), IsRoyality, IsMinRoyalityFixed, IsSEOActive
- **Relationships** (all cascade):
  - FeeProfile (1:1) - billing configuration
  - LateFee (1:1) - late payment rules
  - FranchiseeServices (1:N) - enabled service types
  - FranchiseeServiceFee (1:N) - recurring/one-time charges
  - FranchiseeSales (1:N) - sales transactions
  - FranchiseeAccountCredit (1:N) - account adjustments
  - FranchiseeNotes (1:N) - internal notes
  - FranchiseeDocumentType (1:N) - document configurations

### FeeProfile.cs
Defines how royalties are calculated for a franchisee.

```csharp
public class FeeProfile : DomainBase
{
    [ForeignKey("Franchisee")]
    public override long Id { get; set; }  // Shared PK with Franchisee
    
    public decimal MinimumRoyaltyPerMonth { get; set; }  // Floor - always charged
    public bool SalesBasedRoyalty { get; set; }  // true: use slabs, false: use FixedAmount
    public decimal? FixedAmount { get; set; }  // Used when SalesBasedRoyalty = false
    public decimal AdFundPercentage { get; set; }  // Marketing fund % of sales
    public long? PaymentFrequencyId { get; set; }  // Monthly, Quarterly, etc.
    
    public virtual Franchisee Franchisee { get; set; }
    public virtual Lookup Lookup { get; set; }  // Payment frequency lookup
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<RoyaltyFeeSlabs> RoyaltyFeeSlabs { get; set; }
}
```

**Calculation Logic**:
1. If `SalesBasedRoyalty = true`:
   - Calculate royalty using RoyaltyFeeSlabs (tiered percentages)
   - Apply each slab's ChargePercentage to sales in that range
   - Sum all tiers
2. Else: Use `FixedAmount`
3. Compare to `MinimumRoyaltyPerMonth`, charge whichever is higher
4. Add `AdFundPercentage` of total sales

### RoyaltyFeeSlabs.cs
Tiered royalty structure: defines percentage charged for sales within a range.

```csharp
public class RoyaltyFeeSlabs : DomainBase
{
    public long RoyaltyFeeProfileId { get; set; }
    public decimal? MinValue { get; set; }  // Tier start (null = 0)
    public decimal? MaxValue { get; set; }  // Tier end (null = infinity)
    public decimal ChargePercentage { get; set; }  // Royalty % for this tier
    
    public virtual FeeProfile RoyaltyFeeProfile { get; set; }
}
```

**Example**:
- Slab 1: MinValue=0, MaxValue=50000, ChargePercentage=5.0 → First $50K @ 5%
- Slab 2: MinValue=50001, MaxValue=100000, ChargePercentage=6.0 → Next $50K @ 6%
- Slab 3: MinValue=100001, MaxValue=null, ChargePercentage=7.0 → Everything above @ 7%

### FranchiseeSales.cs
Individual sales transaction record.

```csharp
public class FranchiseeSales : DomainBase
{
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public decimal Amount { get; set; }
    public long ClassTypeId { get; set; }  // Marketing class (residential, commercial, etc.)
    public long? SubClassTypeId { get; set; }  // Sub-classification
    public string SalesRep { get; set; }
    public string QbInvoiceNumber { get; set; }  // QuickBooks reference
    
    // Relationships
    public long? InvoiceId { get; set; }  // Billing invoice
    public long? AccountCreditId { get; set; }  // Applied credit
    public long? SalesDataUploadId { get; set; }  // Batch import reference
    public long CurrencyExchangeRateId { get; set; }  // Multi-currency support
    
    // Navigation properties
    public virtual Franchisee Franchisee { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual Invoice Invoice { get; set; }
    public virtual MarketingClass MarketingClass { get; set; }
}
```

### FranchiseeServiceFee.cs
Recurring or one-time service charges.

```csharp
public class FranchiseeServiceFee : DomainBase
{
    public long FranchiseeId { get; set; }
    public long ServiceFeeTypeId { get; set; }  // Enum: Loan, Bookkeeping, SEO, etc.
    public decimal Amount { get; set; }  // Fixed amount
    public decimal? Percentage { get; set; }  // Or percentage-based
    public bool IsActive { get; set; }  // Can be toggled on/off
    public long? FrequencyId { get; set; }  // Billing frequency
    
    // Special fields for SEO charges
    public DateTime? SaveDateForSeoCost { get; set; }
    public DateTime? InvoiceDateForSeoCost { get; set; }
    
    public virtual Franchisee Franchisee { get; set; }
    public virtual Lookup ServiceFeeType { get; set; }
    public virtual Lookup Frequency { get; set; }
}
```

**ServiceFeeType Examples**: Loan (171), Bookkeeping (172), PayrollProcessing (173), Recruiting (174), OneTimeProject (175), SEOCharges (296)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **Core.Application.Domain** — DomainBase, DataRecorderMetaData, Lookup, File
- **Core.Users.Domain** — Person, UserLogin
- **Core.Geo.Domain** — Address, Phone, Country
- **Core.Billing.Domain** — Invoice, FranchiseeInvoice, AccountCredit
- **Core.Sales.Domain** — Customer, SalesDataUpload, MarketingClass, CurrencyExchangeRate
- **Core.Scheduler.Domain** — FranchiseeTechMailService, FranchiseeTechMailEmail
- **Core.Application.Attribute** — [CascadeEntity] for automatic persistence

### External Packages
- **Entity Framework Core** — ORM annotations, navigation properties
- **System.ComponentModel.DataAnnotations.Schema** — [ForeignKey], [Table] attributes
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Critical Relationships
1. **Organization ↔ Franchisee**: Shared primary key (Franchisee.Id = Organization.Id). Always create Organization first, then Franchisee with matching Id.
2. **FeeProfile Cascade**: Deleting FeeProfile does NOT delete Franchisee (optional 1:1). However, deleting Franchisee cascades to FeeProfile.
3. **RoyaltyFeeSlabs Order**: Must be ordered by MinValue ascending. Gaps or overlaps will cause calculation errors.
4. **FranchiseeSales Volume**: High-volume table. Index on FranchiseeId, CustomerId, DataRecorderMetaDataId (creation date).

### Common Query Patterns
```csharp
// Load franchisee with all financial data
var franchisee = _franchiseeRepository
    .GetAll()
    .Include(f => f.Organization)
    .Include(f => f.FeeProfile)
        .ThenInclude(fp => fp.RoyaltyFeeSlabs)
    .Include(f => f.LateFee)
    .Include(f => f.FranchiseeServiceFee)
    .FirstOrDefault(f => f.Id == franchiseeId);

// Get sales for royalty calculation period
var sales = _salesRepository
    .GetAll()
    .Where(s => s.FranchiseeId == franchiseeId)
    .Where(s => s.DataRecorderMetaData.DateCreated >= startDate 
             && s.DataRecorderMetaData.DateCreated <= endDate)
    .Sum(s => s.Amount);
```

### Validation Rules
- **BusinessId**: Must be unique across all Franchisees (enforced in service layer)
- **RoyaltyFeeSlabs**: MaxValue of one slab must equal MinValue - 1 of next slab (continuous coverage)
- **FeeProfile**: If SalesBasedRoyalty = true, must have at least one RoyaltyFeeSlab
- **Organization.IsActive**: Must be true for Franchisee to process sales or generate invoices

### Performance Considerations
- **FranchiseeSales**: Partition by year if > 1M records
- **Eager Loading**: Use `.Include()` judiciously; FeeProfile + RoyaltyFeeSlabs + Organization = 3 joins
- **Cascade Saves**: CascadeEntity attribute triggers recursive saves; be mindful of transaction size
<!-- END CUSTOM SECTION -->
