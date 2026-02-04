<!-- AUTO-GENERATED: Header -->
# Organizations Domain Context
**Version**: 6890d8748ae984a2ce18a43e7e51b30752daadc1  
**Generated**: 2024-02-04T06:25:00Z  
**Location**: `src/Core/Organizations/Domain`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This domain implements the **multi-tenant franchisee management system** that forms the foundation of Marblelife's business model. It manages:

1. **Organizational Hierarchy**: HQ → Franchisees → Users, with role-based access control (RBAC)
2. **Financial Structures**: Complex royalty fee slabs, late fees, loans, service fees, and one-time charges
3. **Service Configuration**: Franchisee-specific service offerings, pricing, and certifications
4. **Audit & History**: Comprehensive tracking of franchisee registration, duration changes, loan adjustments, and perpetuity status
5. **Integration Points**: ReviewPush API for customer feedback, QuickBooks sync, and HomeAdvisor lead tracking

### Design Patterns

- **Aggregate Root Pattern**: `Organization` is the root, with `Franchisee` as a 1:1 extension (shared primary key via `ForeignKey("Organization")`). The `Franchisee.Id` IS the `Organization.Id`.
- **Composite Key Relationships**: `OrganizationRoleUser` maps the many-to-many-to-many relationship between Users, Roles, and Organizations.
- **Value Object Composition**: `FeeProfile` acts as a value object for royalty calculation rules, embedded with `RoyaltyFeeSlabs` for progressive fee structures.
- **Cascade Entity Pattern**: Uses custom `[CascadeEntity]` attributes to define deletion/save behavior for child entities (e.g., `FeeProfile`, `LateFee`, `FranchiseeNotes`).
- **Soft Delete**: All entities inherit `DomainBase.IsDeleted` flag; no hard deletes in production.
- **Lookup Table Pattern**: Extensive use of `Lookup` entities (via `LookupType` system) for Type, Category, Language, Frequency, Status, etc.
- **Audit Trail**: `DataRecorderMetaData` tracks who/when for all mutable entities.

### Data Flow

#### Franchisee Onboarding
1. Create `Organization` → `Franchisee` (1:1 relationship via shared PK)
2. Initialize `FeeProfile` with `RoyaltyFeeSlabs` (defines revenue-based fee tiers)
3. Create `LateFee` with default penalties (Royalty: $50 after 2 days, 18% APR; SalesData: $50 after 1 day)
4. Assign `FranchiseeService` records (e.g., Marble Polishing, Grout Cleaning, Certified = true)
5. Configure `FranchiseeServiceFee` for recurring charges (Bookkeeping, SEO, etc.)
6. Link `OrganizationRoleUser` records for Owner, Technicians, Schedulers

#### Royalty Calculation
1. Sales data uploaded → `FranchiseeSales` records created
2. `FeeProfile.SalesBasedRoyalty` flag determines calculation mode:
   - **TRUE**: Use `RoyaltyFeeSlabs` (e.g., 0-$10k = 8%, $10k-$50k = 6%, $50k+ = 4%)
   - **FALSE**: Use `FeeProfile.FixedAmount`
3. Apply `MinimumRoyaltyPerMonth` floor (can vary via `MinRoyaltyFeeSlabs` based on sales range)
4. Add `AdFundPercentage` charge (e.g., 2% of sales)
5. If late → apply `LateFee` charges and interest

#### Service Pricing (Franchisee-Specific Overrides)
1. HQ sets base prices in `PriceEstimateServices` (`CorporatePrice`, `BulkCorporatePrice`)
2. Franchisee can override via `FranchiseePrice` + `IsPriceChangedByFranchisee` flag
3. Linked to `ServicesTag` (MaterialType, Service category)
4. Similar overrides for `MaintenanceCharges`, `ReplacementCharges`, `ShiftCharges`, `FloorGrindingAdjustment`

### Multi-Tenancy Strategy
- **Franchisee-Scoped Queries**: Nearly all queries filter by `FranchiseeId` (stored in session/JWT)
- **Data Isolation**: Each franchisee operates in a logical silo; no cross-franchisee data access except for HQ role
- **Shared Reference Data**: `ServiceType`, `DocumentType`, `ReviewPushAPILocation` are global; overrides stored per-franchisee
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Core Entities

#### Organization
```csharp
public class Organization : DomainBase
{
    long Id { get; set; }                        // Primary Key
    string Name { get; set; }                    // Franchisee business name
    long TypeId { get; set; }                    // FK to Lookup (Franchisee, HQ, etc.)
    string About { get; set; }
    string Email { get; set; }
    string DeactivationnNote { get; set; }       // Reason for closure
    bool IsActive { get; set; }                  // Soft delete flag
    
    // Navigation Properties
    Lookup Type { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
    ICollection<Address> Address { get; set; }   // [CascadeEntity] physical locations
    ICollection<Phone> Phones { get; set; }      // [CascadeEntity]
    Franchisee Franchisee { get; set; }          // [CascadeEntity] 1:1 extension
}
```

#### Franchisee
```csharp
public class Franchisee : DomainBase
{
    long Id { get; set; }                        // Shares PK with Organization (FK)
    string OwnerName { get; set; }
    string QuickBookIdentifier { get; set; }     // QB sync reference
    string Currency { get; set; }                // USD, CAD
    string DisplayName { get; set; }             // Short name for UI
    string EIN { get; set; }                     // Tax ID
    string LegalEntity { get; set; }             // LLC, Corp, etc.
    string RegistrationNumber { get; set; }
    DateTime? RegistrationDate { get; set; }
    decimal? Duration { get; set; }              // Contract duration (months?)
    DateTime? DateOfRenewal { get; set; }
    decimal? RenewalFee { get; set; }
    decimal? TransferFee { get; set; }           // Fee if ownership transferred
    decimal? OriginalFranchiseeFee { get; set; }
    decimal? SalesTax { get; set; }              // Default tax rate
    decimal? LessDeposit { get; set; }
    
    // Contact Information
    string ContactFirstName { get; set; }        // Primary contact
    string ContactLastName { get; set; }
    string ContactEmail { get; set; }
    string AccountPersonFirstName { get; set; }  // Accounting contact
    string AccountPersonLastName { get; set; }
    string AccountPersonEmail { get; set; }
    string MarketingPersonFirstName { get; set; }
    string MarketingPersonLastName { get; set; }
    string MarketingPersonEmail { get; set; }
    string SchedulerFirstName { get; set; }      // Scheduling/dispatch contact
    string SchedulerLastName { get; set; }
    string SchedulerEmail { get; set; }
    
    // Flags
    bool IsReviewFeedbackEnabled { get; set; }   // Enables ReviewPush integration
    bool SetGeoCode { get; set; }                // Geocoding enabled for jobs
    bool IsRoyality { get; set; }                // Subject to royalty fees
    bool IsMinRoyalityFixed { get; set; }        // Use fixed min royalty vs. slabs
    bool IsSEOActive { get; set; }               // SEO service subscription
    
    // Business Intelligence
    string WebLeadFranchiseeId { get; set; }     // External lead system ID
    long? BusinessId { get; set; }
    long? CategoryId { get; set; }               // FK to Lookup (franchise category)
    long? ReviewpushId { get; set; }             // FK to ReviewPushAPILocation
    string WebSite { get; set; }
    string NotesFromCallCenter { get; set; }
    string NotesFromOwner { get; set; }
    string Description { get; set; }
    string CategoryNotes { get; set; }
    
    long? FileId { get; set; }                   // Logo/avatar
    long? LanguageId { get; set; }               // FK to Lookup (English, Spanish)
    
    // Navigation Properties
    Organization Organization { get; set; }
    Lookup Category { get; set; }
    Lookup Language { get; set; }
    ReviewPushAPILocation Reviewpush { get; set; }
    File File { get; set; }
    
    // [CascadeEntity] Children
    FeeProfile FeeProfile { get; set; }                            // Royalty structure
    LateFee LateFee { get; set; }                                  // Late payment rules
    ICollection<FranchiseeService> FranchiseeServices { get; set; }
    ICollection<FranchiseeDocumentType> FranchiseeDocumentType { get; set; }
    ICollection<FranchiseeNotes> FranchiseeNotes { get; set; }
    
    // Non-Cascade Children
    ICollection<FranchiseeInvoice> FranchiseeInvoices { get; set; }
    ICollection<SalesDataUpload> SalesDataUploads { get; set; }
    ICollection<FranchiseeSales> FranchiseeSales { get; set; }
    ICollection<FranchiseeAccountCredit> FranchiseeAccountCredit { get; set; }
    ICollection<BatchUploadRecord> BatchUploadRecord { get; set; }
    ICollection<FranchiseeServiceFee> FranchiseeServiceFee { get; set; }
}
```

#### FeeProfile
```csharp
public class FeeProfile : DomainBase
{
    long Id { get; set; }                        // Shares PK with Franchisee (1:1)
    long? PaymentFrequencyId { get; set; }       // FK to Lookup (Weekly=31, Monthly=32, TwiceAMonth=33)
    decimal MinimumRoyaltyPerMonth { get; set; } // Floor for royalty (e.g., $500)
    bool SalesBasedRoyalty { get; set; }         // TRUE = use slabs, FALSE = fixed
    decimal? FixedAmount { get; set; }           // Used if SalesBasedRoyalty = FALSE
    decimal AdFundPercentage { get; set; }       // Marketing fund % (e.g., 2%)
    
    Franchisee Franchisee { get; set; }
    Lookup Lookup { get; set; }                  // PaymentFrequency
    ICollection<RoyaltyFeeSlabs> RoyaltyFeeSlabs { get; set; } // [CascadeEntity]
}
```

#### RoyaltyFeeSlabs
```csharp
public class RoyaltyFeeSlabs : DomainBase
{
    long RoyaltyFeeProfileId { get; set; }       // FK to FeeProfile
    decimal? MinValue { get; set; }              // Sales range start (e.g., $0)
    decimal? MaxValue { get; set; }              // Sales range end (e.g., $10,000)
    decimal ChargePercentage { get; set; }       // Fee % for this tier (e.g., 8%)
    
    FeeProfile RoyaltyFeeProfile { get; set; }
}
```
**Example Slab Configuration**:
| MinValue | MaxValue | ChargePercentage |
|----------|----------|------------------|
| 0        | 10000    | 8%               |
| 10000    | 50000    | 6%               |
| 50000    | null     | 4%               |

#### MinRoyaltyFeeSlabs
```csharp
public class MinRoyaltyFeeSlabs : DomainBase
{
    long FranchiseeId { get; set; }
    decimal? StartValue { get; set; }            // Sales range start
    decimal? EndValue { get; set; }              // Sales range end
    decimal MinRoyality { get; set; }            // Minimum royalty for this range
    
    Franchisee Franchisee { get; set; }
}
```
**Purpose**: Overrides `FeeProfile.MinimumRoyaltyPerMonth` based on sales volume. Example: If sales < $5k, MinRoyalty = $200; if $5k-$20k, MinRoyalty = $500.

#### LateFee
```csharp
public class LateFee : DomainBase
{
    long Id { get; set; }                        // Shares PK with Franchisee (1:1)
    
    // Royalty Late Penalties
    decimal RoyalityLateFee { get; set; }        // Default: $50
    int RoyalityWaitPeriodInDays { get; set; }   // Default: 2 days grace period
    decimal RoyalityInterestRatePercentagePerAnnum { get; set; } // Default: 18% APR
    
    // Sales Data Late Penalties
    decimal SalesDataLateFee { get; set; }       // Default: $50
    int SalesDataWaitPeriodInDays { get; set; }  // Default: 1 day grace period
    
    Franchisee Franchisee { get; set; }
}
```

#### OrganizationRoleUser
```csharp
public class OrganizationRoleUser : DomainBase
{
    long UserId { get; set; }                    // FK to Person
    long RoleId { get; set; }                    // FK to Role (Owner, Technician, Scheduler)
    long OrganizationId { get; set; }            // FK to Organization
    bool IsDefault { get; set; }                 // Default org when user logs in
    bool IsActive { get; set; }                  // Soft delete
    string ColorCode { get; set; }               // Calendar color for scheduler view
    string ColorCodeSale { get; set; }           // Sales calendar color
    
    Organization Organization { get; set; }
    Person Person { get; set; }
    Role Role { get; set; }
    ICollection<OrganizationRoleUserFranchisee> OrganizationRoleUserFranchisee { get; set; }
}
```
**Purpose**: Implements many-to-many-to-many relationship. A user can have multiple roles across multiple organizations. Example: John (UserId=5) is a Technician (RoleId=10) for Detroit (OrgId=27) and Pittsburgh (OrgId=64).

### Service Management

#### ServiceType
```csharp
public class ServiceType : DomainBase
{
    string Name { get; set; }                    // "Marble Polishing", "Grout Cleaning"
    string Description { get; set; }
    string Alias { get; set; }                   // Short code
    bool IsActive { get; set; }
    long CategoryId { get; set; }                // FK to Lookup (Restoration, Maintenance)
    long? SubCategoryId { get; set; }            // FK to Lookup
    string ColorCode { get; set; }               // UI display color
    int? OrderBy { get; set; }                   // Sort order (legacy)
    int? NewOrderBy { get; set; }                // Sort order (current)
    bool DashboardServices { get; set; }         // Show on dashboard
    
    Lookup Category { get; set; }
    Lookup SubCategory { get; set; }
}
```

#### FranchiseeService
```csharp
public class FranchiseeService : DomainBase
{
    long FranchiseeId { get; set; }
    long ServiceTypeId { get; set; }
    bool CalculateRoyalty { get; set; }          // Include this service in royalty calc
    bool IsActive { get; set; }                  // Franchisee offers this service
    bool IsCertified { get; set; }               // Franchisee certified for this service
    
    Franchisee Franchisee { get; set; }
    ServiceType ServiceType { get; set; }
}
```

#### FranchiseeServiceFee
```csharp
public class FranchiseeServiceFee : DomainBase
{
    long FranchiseeId { get; set; }
    long ServiceFeeTypeId { get; set; }          // FK to Lookup (Loan=171, Bookkeeping=172, SEO=296, etc.)
    decimal Amount { get; set; }
    decimal? Percentage { get; set; }            // Alternative to fixed amount
    bool IsActive { get; set; }
    long? FrequencyId { get; set; }              // FK to Lookup (Weekly, Monthly)
    DateTime? SaveDateForSeoCost { get; set; }
    DateTime? InvoiceDateForSeoCost { get; set; }
    
    Franchisee Franchisee { get; set; }
    Lookup ServiceFeeType { get; set; }
    Lookup Frequency { get; set; }
}
```
**Common ServiceFeeType Values** (from `ServiceFeeType` enum):
- `171` = Loan
- `172` = Bookkeeping
- `173` = PayrollProcessing
- `174` = Recruiting
- `175` = OneTimeProject
- `176` = NationalCharge
- `177` = InterestAmount
- `178` = VarBookkeeping (variable bookkeeping)
- `214` = FRANCHISEETECHMAIL
- `251` = PHONECALLCHARGES
- `296` = SEOCharges

### Financial Entities

#### FranchiseeLoan
```csharp
public class FranchiseeLoan : DomainBase
{
    decimal Amount { get; set; }
    int Duration { get; set; }                   // Months
    decimal InterestratePerAnum { get; set; }    // APR
    long FranchiseeId { get; set; }
    string Description { get; set; }
    DateTime DateCreated { get; set; }
    DateTime? StartDate { get; set; }
    bool? IsRoyality { get; set; }               // Loan tied to royalty account?
    bool? IsCompleted { get; set; }
    long CurrencyExchangeRateId { get; set; }
    long? LoanTypeId { get; set; }               // FK to Lookup
    
    Franchisee Franchisee { get; set; }
    CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    Lookup LoanType { get; set; }
    ICollection<FranchiseeLoanSchedule> FranchiseeLoanSchedule { get; set; } // [CascadeEntity]
}
```

#### FranchiseeLoanSchedule
```csharp
public class FranchiseeLoanSchedule : DomainBase
{
    int LoanTerm { get; set; }                   // Payment #
    long LoanId { get; set; }
    DateTime DueDate { get; set; }
    decimal Balance { get; set; }                // Remaining principal
    decimal Interest { get; set; }               // Interest due this period
    decimal Principal { get; set; }              // Principal due this period
    decimal PayableAmount { get; set; }          // Total due (Principal + Interest)
    decimal OverPaidAmount { get; set; }
    decimal TotalPrincipal { get; set; }         // Cumulative principal paid
    bool IsPrePaid { get; set; }
    bool IsOverPaid { get; set; }
    bool CalculateReschedule { get; set; }       // Trigger amortization recalc
    bool IsRoyality { get; set; }
    
    long? InvoiceItemId { get; set; }            // FK to InvoiceItem (principal)
    long? InterestAmountInvoiceItemId { get; set; } // FK to InvoiceItem (interest)
    
    FranchiseeLoan FranchiseeLoan { get; set; }
    InvoiceItem PrincipalInvoiceItem { get; set; }
    InvoiceItem InterestRateInvoiceItem { get; set; }
}
```

#### OneTimeProjectFee
```csharp
public class OneTimeProjectFee : DomainBase
{
    decimal Amount { get; set; }
    long FranchiseeId { get; set; }
    string Description { get; set; }
    DateTime DateCreated { get; set; }
    long CurrencyExchangeRateId { get; set; }
    long? InvoiceItemId { get; set; }            // FK to InvoiceItem (billed)
    
    Franchisee Franchisee { get; set; }
    CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    InvoiceItem InvoiceItem { get; set; }
}
```

#### FranchiseeAccountCredit
```csharp
public class FranchiseeAccountCredit : DomainBase
{
    DateTime CreditedOn { get; set; }
    long FranchiseeId { get; set; }
    long? InvoiceId { get; set; }                // FK to Invoice (applied credit)
    string Description { get; set; }
    decimal Amount { get; set; }                 // Original credit
    decimal RemainingAmount { get; set; }        // Unapplied balance
    long CurrencyExchangeRateId { get; set; }
    long? CreditTypeId { get; set; }             // FK to Lookup
    long? PersonId { get; set; }                 // FK to Person (who issued credit)
    
    CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    Lookup CreditType { get; set; }
    Person Person { get; set; }
}
```

#### FranchiseeSales
```csharp
public class FranchiseeSales : DomainBase
{
    long FranchiseeId { get; set; }
    long CustomerId { get; set; }
    long? InvoiceId { get; set; }                // FK to Invoice (franchisee's invoice to customer)
    long? AccountCreditId { get; set; }          // FK to AccountCredit (if credit applied)
    long ClassTypeId { get; set; }               // FK to MarketingClass (ResidentialRepair, Commercial, etc.)
    long? SubClassTypeId { get; set; }           // FK to SubClassMarketingClass
    string SalesRep { get; set; }                // Tech name or sales person
    decimal Amount { get; set; }
    long? SalesDataUploadId { get; set; }        // FK to SalesDataUpload (bulk import)
    string QbInvoiceNumber { get; set; }         // QuickBooks sync reference
    long CustomerInvoiceId { get; set; }
    string CustomerInvoiceIdString { get; set; }
    long CustomerQbInvoiceId { get; set; }
    string CustomerQbInvoiceIdString { get; set; }
    long DataRecorderMetaDataId { get; set; }
    long CurrencyExchangeRateId { get; set; }
    
    Franchisee Franchisee { get; set; }
    Customer Customer { get; set; }
    Invoice Invoice { get; set; }
    SalesDataUpload SalesDataUpload { get; set; }
    MarketingClass MarketingClass { get; set; }
    SubClassMarketingClass SubClassMarketingClass { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
    CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    AccountCredit AccountCredit { get; set; }
}
```
**Purpose**: Records all franchisee sales transactions for royalty calculation. Linked to customers, invoices, and marketing classification.

### Pricing & Configuration

#### PriceEstimateServices
```csharp
public class PriceEstimateServices : DomainBase
{
    long? FranchiseeId { get; set; }             // NULL = HQ default, NOT NULL = override
    long? ServiceTagId { get; set; }             // FK to ServicesTag
    
    // HQ Pricing Tiers
    decimal? BulkCorporatePrice { get; set; }    // Bulk corporate rate
    decimal? BulkCorporateAdditionalPrice { get; set; }
    decimal? CorporatePrice { get; set; }        // Standard corporate rate
    decimal? CorporateAdditionalPrice { get; set; }
    
    // Franchisee Pricing
    decimal? FranchiseePrice { get; set; }       // Franchisee's rate
    decimal? FranchiseeAdditionalPrice { get; set; }
    string AlternativeSolution { get; set; }
    
    // Flags
    bool IsPriceChangedByFranchisee { get; set; }
    bool IsPriceChangedByAdmin { get; set; }
    bool IsFranchiseePriceExceed { get; set; }   // Franchisee price > corporate
    bool IsFranchiseePriceExceedForEmail { get; set; } // Send alert to HQ
    
    Franchisee Franchisee { get; set; }
    ServicesTag ServicesTag { get; set; }
}
```

#### ServicesTag
```csharp
public class ServicesTag : DomainBase
{
    long ServiceTypeId { get; set; }             // FK to ServiceType
    long CategoryId { get; set; }                // FK to Lookup (Material category)
    string MaterialType { get; set; }            // "Marble", "Granite", "Travertine"
    string Service { get; set; }                 // "Polishing", "Honing", "Sealing"
    string Notes { get; set; }
    long? NotesSavedBy { get; set; }             // FK to User
    bool IsActive { get; set; }
    
    Lookup Category { get; set; }
    ServiceType ServiceType { get; set; }
}
```

#### MaintenanceCharges
```csharp
public class MaintenanceCharges : DomainBase
{
    long? FranchiseeId { get; set; }             // NULL = HQ default
    string Material { get; set; }                // "Marble Floor Polish", "Grout Sealer"
    decimal? High { get; set; }                  // Price range high
    decimal? Low { get; set; }                   // Price range low
    string UOM { get; set; }                     // Unit of measure (sq ft, linear ft)
    string Notes { get; set; }
    int Order { get; set; }                      // Sort order
    bool IsPriceChangedByFranchisee { get; set; }
    bool IsActive { get; set; }
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

#### ReplacementCharges
```csharp
public class ReplacementCharges : DomainBase
{
    long? FranchiseeId { get; set; }
    string Material { get; set; }                // Tile type
    decimal? CostOfRemovingTile { get; set; }
    decimal? CostOfInstallingTile { get; set; }
    decimal? CostOfTileMaterial { get; set; }
    decimal? TotalReplacementCost { get; set; }  // Sum of above
    int Order { get; set; }
    bool IsPriceChangedByFranchisee { get; set; }
    bool IsActive { get; set; }
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

#### ShiftCharges
```csharp
public class ShiftCharges : DomainBase
{
    long? FranchiseeId { get; set; }
    decimal? TechDayShiftPrice { get; set; }
    decimal? CommercialRestorationShiftPrice { get; set; }
    decimal? MaintenanceTechNightShiftPrice { get; set; }
    bool IsPriceChangedByFranchisee { get; set; }
    bool IsActive { get; set; }
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

#### FloorGrindingAdjustment
```csharp
public class FloorGrindingAdjustment : DomainBase
{
    long? FranchiseeId { get; set; }
    string DiameterOfGrindingPlate { get; set; } // Equipment size
    decimal? Area { get; set; }                  // Sq ft coverage
    decimal? AdjustmentFactor { get; set; }      // Pricing multiplier
    bool IsPriceChangedByFranchisee { get; set; }
    bool IsActive { get; set; }
    
    Franchisee Franchisee { get; set; }
}
```

#### TaxRates
```csharp
public class TaxRates : DomainBase
{
    long? FranchiseeId { get; set; }
    decimal? TaxForServices { get; set; }        // Service tax %
    decimal? TaxForProducts { get; set; }        // Product tax %
    
    Franchisee Franchisee { get; set; }
}
```

### Documents & Tracking

#### FranchiseDocument
```csharp
public class FranchiseDocument : DomainBase
{
    long? FileId { get; set; }                   // FK to File
    long FranchiseeId { get; set; }
    DateTime? ExpiryDate { get; set; }           // For certifications, insurance, etc.
    string UploadFor { get; set; }               // Purpose description
    bool IsImportant { get; set; }
    bool ShowToUser { get; set; }                // Visible to franchisee
    bool IsPerpetuity { get; set; }              // Never expires
    bool IsRejected { get; set; }                // HQ rejected document
    long DataRecorderMetaDataId { get; set; }
    long? DocumentTypeId { get; set; }           // FK to DocumentType
    long? UserId { get; set; }                   // FK to OrganizationRoleUser
    
    File File { get; set; }
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
    DocumentType DocumentType { get; set; }
    OrganizationRoleUser OrganizationRoleUser { get; set; }
}
```

#### FranchiseeDocumentType
```csharp
public class FranchiseeDocumentType : DomainBase
{
    long FranchiseeId { get; set; }
    long DocumentTypeId { get; set; }
    bool IsActive { get; set; }
    bool IsPerpetuity { get; set; }
    bool IsRejected { get; set; }
    
    Organization Franchisee { get; set; }        // Note: FK to Organization, not Franchisee
    DocumentType DocumentType { get; set; }
}
```

#### DocumentType
```csharp
public class DocumentType : DomainBase
{
    string Name { get; set; }                    // "Insurance Certificate", "W9", "License"
    long CategoryId { get; set; }                // FK to Lookup
    long Order { get; set; }                     // Sort order
}
```

#### FranchiseeNotes
```csharp
public class FranchiseeNotes : DomainBase
{
    long FranchiseeId { get; set; }
    string Text { get; set; }                    // Note content
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

### History & Audit Entities

#### FranchiseeRegistrationHistry
```csharp
public class FranchiseeRegistrationHistry : DomainBase
{
    DateTime RegistrationDate { get; set; }
    long FranchiseeId { get; set; }
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; } // [CascadeEntity]
}
```

#### FranchiseeDurationNotesHistry
```csharp
public class FranchiseeDurationNotesHistry : DomainBase
{
    string Description { get; set; }
    long FranchiseeId { get; set; }
    long UserId { get; set; }                    // Who made the change
    long StatusId { get; set; }                  // FK to Lookup (Approved, Pending, Rejected)
    long TypeId { get; set; }                    // FK to Lookup (Extension, Reduction, Transfer)
    long RoleId { get; set; }                    // FK to Role (who requested)
    decimal? Duration { get; set; }              // New duration value
    long? ApprovedById { get; set; }             // FK to Person (who approved)
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    Person Person { get; set; }                  // Requester
    Person ApprovedBy { get; set; }              // Approver
    Lookup Status { get; set; }
    Lookup Type { get; set; }
    Role Role { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; } // [CascadeEntity]
}
```

#### Perpetuitydatehistry
```csharp
public class Perpetuitydatehistry : DomainBase
{
    DateTime LastDateChecked { get; set; }
    long? FranchiseeId { get; set; }
    bool? IsPerpetuity { get; set; }             // Contract is perpetual
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

#### LoanAdjustmentAudit
```csharp
public class LoanAdjustmentAudit : DomainBase
{
    long UserId { get; set; }
    long LoanId { get; set; }
    bool BeforeLoanAdjustment { get; set; }      // Snapshot flag (before/after)
    bool AfterLoanAdjustment { get; set; }
    DateTime CreatedOn { get; set; }
    
    Person Person { get; set; }
    FranchiseeLoan FranchiseeLoan { get; set; }
}
```

#### OnetimeprojectfeeAddFundRoyality
```csharp
public class OnetimeprojectfeeAddFundRoyality : DomainBase
{
    long FranchiseeId { get; set; }
    bool IsInRoyality { get; set; }              // Include one-time fees in royalty calc
    bool IsSEOInRoyalty { get; set; }            // Include SEO fees in royalty calc
    long DataRecorderMetaDataId { get; set; }
    
    Organization Organization { get; set; }      // Note: FK to Organization, not Franchisee
    DataRecorderMetaData DataRecorderMetaData { get; set; } // [CascadeEntity]
}
```

### Integration & Reporting

#### ReviewPushAPILocation
```csharp
public class ReviewPushAPILocation : DomainBase
{
    string Name { get; set; }                    // Location name
    long Location_Id { get; set; }               // ReviewPush location ID
    long Rp_ID { get; set; }                     // ReviewPush account ID (legacy)
    string NewRp_ID { get; set; }                // ReviewPush account ID (current)
}
```

#### ReviewPushCustomerFeedback
```csharp
public class ReviewPushCustomerFeedback : DomainBase
{
    string Name { get; set; }                    // Customer name
    long? Review_Id { get; set; }
    long? Location_Id { get; set; }
    long? Rp_ID { get; set; }
    long? FranchiseeId { get; set; }
    string FranchiseeName { get; set; }
    long? Rating { get; set; }                   // 1-5 stars
    DateTime? Rp_date { get; set; }              // Date from ReviewPush API
    DateTime? Db_date { get; set; }              // Date stored in DB
    string Url { get; set; }                     // Review URL
    string Review { get; set; }                  // Review text
    string Email { get; set; }                   // Customer email
    long AuditActionId { get; set; }             // FK to Lookup (Created, Updated, Deleted)
    
    Franchisee Franchisee { get; set; }
    Lookup AuditAction { get; set; }
}
```

#### LeadPerformanceFranchiseeDetails
```csharp
public class LeadPerformanceFranchiseeDetails : DomainBase
{
    long FranchiseeId { get; set; }
    double Amount { get; set; }                  // Lead spend
    int Month { get; set; }
    int Year { get; set; }                       // [NotMapped] calculated field
    long? week { get; set; }
    DateTime DateTime { get; set; }
    long? CategoryId { get; set; }               // FK to Lookup (Lead source category)
    bool IsActive { get; set; }
    bool IsSEOActive { get; set; }
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    Lookup Category { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

#### SalesDataMailReminder
```csharp
public class SalesDataMailReminder : DomainBase
{
    long FranchiseeId { get; set; }
    DateTime Date { get; set; }                  // Last reminder sent
    
    Franchisee Franchisee { get; set; }
}
```

#### FloorGrindingAdjustmentNotes
```csharp
public class FloorGrindingAdjustmentNotes : DomainBase
{
    long? FranchiseeId { get; set; }
    string Note { get; set; }
    bool IsChangedByFranchisee { get; set; }
    bool IsActive { get; set; }
    long DataRecorderMetaDataId { get; set; }
    
    Franchisee Franchisee { get; set; }
    DataRecorderMetaData DataRecorderMetaData { get; set; }
}
```

### Base Class
```csharp
public abstract class DomainBase
{
    long Id { get; set; }                        // Primary Key
    bool IsNew { get; set; }                     // [NotMapped] Flag for insert vs update
    bool IsDeleted { get; set; }                 // [NotMapped] Soft delete flag
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Module Dependencies

#### Direct References
- **Core.Application.Domain** - `DomainBase`, `Lookup`, `DataRecorderMetaData`, `File`, `Folder`, `ContentType`
- **Core.Users.Domain** - `Person`, `Role` for RBAC
- **Core.Geo.Domain** - `Address`, `Phone` for organization locations
- **Core.Billing.Domain** - `Invoice`, `InvoiceItem`, `FranchiseeInvoice`, `CurrencyExchangeRate`, `AccountCredit` for financial transactions
- **Core.Sales.Domain** - `Customer`, `MarketingClass`, `SubClassMarketingClass`, `SalesDataUpload`, `BatchUploadRecord` for CRM
- **Core.Reports.Domain** - Reporting aggregation (no direct FK, but consumed by reports)
- **Core.Application.Attribute** - `[CascadeEntity]` for ORM cascade rules

### External Dependencies
None directly. This is a pure domain layer with no external package dependencies (EF6 annotations are in `System.ComponentModel.DataAnnotations`).

### Consumed By
- **API Controllers**: `OrganizationController`, `FranchiseeController`, `FeeProfileController`
- **Services**: 
  - `IFranchiseeInfoService` - CRUD + complex queries
  - `IOrganizationRoleUserService` - User-role-org assignments
  - `IFranchiseeSalesService` - Sales data upload & processing
  - `IFranchiseeServiceFeeService` - Recurring fee management
  - `ILeadPerformanceFranchiseeDetailsService` - Lead spend tracking
  - `IFranchiseeDocumentService` - Document management
- **Background Jobs**: 
  - Royalty invoice generation (monthly)
  - Late fee application (daily)
  - Sales data reminder emails
  - ReviewPush feedback sync
- **Reports**: Growth tracking, royalty reports, product mix, franchisee performance dashboards

### Database Schema Notes
- **Schema**: Likely `dbo` (no explicit schema attribute)
- **Table Naming**: Follows class name (e.g., `Organization`, `Franchisee`, `RoyaltyFeeSlabs`)
- **Indexes**: Not defined in domain models (assumed to be in ORM config or migrations)
- **Cascade Deletes**: Controlled via `[CascadeEntity]` attribute, NOT database FKs (application-level only)
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## 💡 Developer Insights

### Critical Business Logic

#### Royalty Calculation Flow
The royalty engine is the heart of the franchisee financial model:
1. **Sales Aggregation**: Sum `FranchiseeSales.Amount` for the billing period
2. **Slab Lookup**: If `FeeProfile.SalesBasedRoyalty == true`, walk `RoyaltyFeeSlabs` to calculate progressive fee
3. **Minimum Floor**: Apply `MinimumRoyaltyPerMonth` OR `MinRoyaltyFeeSlabs` based on sales volume
4. **Ad Fund**: Add `FeeProfile.AdFundPercentage` (e.g., 2% of gross sales)
5. **Late Fees**: If past due date + grace period → add `LateFee` + interest

**Example**:
- Sales: $35,000
- Slabs: 0-$10k = 8%, $10k-$50k = 6%
- Royalty: ($10,000 * 8%) + ($25,000 * 6%) = $800 + $1,500 = $2,300
- Min Royalty: $500 → Ignored (calculated > min)
- Ad Fund: $35,000 * 2% = $700
- **Total**: $3,000

#### Pricing Override Hierarchy
1. **HQ Default**: `PriceEstimateServices` with `FranchiseeId = NULL`
2. **Franchisee Override**: `PriceEstimateServices` with `FranchiseeId = X` + `IsPriceChangedByFranchisee = true`
3. **Alert**: If franchisee price exceeds corporate → set `IsFranchiseePriceExceedForEmail` for HQ notification

### Gotchas & Edge Cases

#### Shared Primary Key Pattern
- `Franchisee.Id` **IS** `Organization.Id` (not a separate FK)
- Deleting `Organization` will cascade-delete `Franchisee` if ORM configured correctly
- **Risk**: Direct SQL queries must join on `Franchisee.Id = Organization.Id`, NOT a FK column

#### Soft Delete Confusion
- `DomainBase.IsDeleted` is `[NotMapped]` → **Not persisted in DB**
- Actual soft delete may use `Organization.IsActive = false`
- Check service layer for actual deletion logic

#### Lookup Table Overload
- `LookupType` system used for 50+ entity types
- **Performance**: Ensure indexes on `Lookup.TypeId` and `Lookup.Value`
- **Pitfall**: Magic numbers scattered across codebase (e.g., `ServiceFeeType.SEOCharges = 296`)

#### History Tables Have No FK Cascade
- `FranchiseeRegistrationHistry`, `Perpetuitydatehistry`, etc. are append-only audit logs
- **Never delete** history records; they violate audit trail integrity

#### Loan Amortization Complexity
- `FranchiseeLoanSchedule.CalculateReschedule` flag triggers recalculation of all future payments
- **Warning**: Modifying a past payment can corrupt future schedule if not careful
- Always recalculate from `FranchiseeLoan.StartDate` forward

### Performance Considerations

#### Eager Loading Required
Always use `.Include()` for these relationships (N+1 query hazard):
```csharp
context.Franchisees
    .Include(f => f.Organization.Address)
    .Include(f => f.FeeProfile.RoyaltyFeeSlabs)
    .Include(f => f.FranchiseeServices.Select(fs => fs.ServiceType))
    .Include(f => f.FranchiseeServiceFee)
```

#### Slow Queries
- `FranchiseeSales` table grows indefinitely → Always filter by date range
- `ReviewPushCustomerFeedback` can have millions of rows → Use pagination
- `FranchiseeLoanSchedule` has 240+ rows per loan (20-year amortization) → Index on `LoanId` + `DueDate`

### Security Notes
- **Row-Level Security**: Every query must filter by `FranchiseeId` from JWT token (unless HQ role)
- **Price Manipulation**: `IsPriceChangedByFranchisee` flag should trigger approval workflow (not implemented in domain)
- **Document Visibility**: `FranchiseDocument.ShowToUser` controls franchisee access → Enforce in API layer

### Testing Recommendations
- **Unit Test**: Royalty calculation with various slab configurations
- **Integration Test**: Loan amortization schedule generation
- **E2E Test**: Franchisee onboarding workflow (Org → Franchisee → FeeProfile → Services)
- **Perf Test**: Sales data import for 10k rows

### Migration History
- Originally flat `Franchisee` table; split into `Organization` + `Franchisee` for multi-tenancy
- `RoyaltyFeeSlabs` added in v2.0 to replace hardcoded 8% fee
- `MinRoyaltyFeeSlabs` added in v3.5 to support variable minimums
- `FranchiseeServiceFee` enum values (171-296) are frozen; do NOT reuse deleted values

### Known Technical Debt
- **Naming Inconsistency**: `Perpetuitydatehistry`, `FranchiseeDurationNotesHistry` → Should be `History`
- **Typo**: `InterestratePerAnum` → Should be `InterestRatePerAnnum`
- **Magic Strings**: `UploadFor`, `ColorCode`, `ColorCodeSale` have no enum constraints
- **Null Ambiguity**: `FranchiseeId = NULL` means "HQ default" in pricing tables, but nullable FK elsewhere
- **Overloaded Flags**: `IsRoyality` appears in multiple contexts (loans, sales, fees) with slightly different meanings
<!-- END CUSTOM SECTION -->
