<!-- AUTO-GENERATED: Header -->
# Organizations Enum Module Context
**Version**: 031e3874a7879f58c3e6246dec6be36473b0e3e4  
**Generated**: 2026-02-04T06:50:00Z  
**Module Path**: `src/Core/Organizations/Enum/`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module defines **13 enumeration types** that serve as **Lookup Table Primary Key Constants** within the multi-tenant franchisee management system. These enums map to rows in the `Lookup` table (via `Core.Application.Domain.Lookup`) and provide type-safe constants for querying and filtering domain entities across the Organizations module.

**Critical Pattern**: All enum values use **explicit integer assignments** that correspond to database `Lookup.Id` primary keys. This enables:
- Type-safe querying: `serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping`
- Foreign key reference validation
- Compile-time constant usage instead of magic numbers

### Design Patterns
- **Lookup Table Pattern**: Enums act as strongly-typed facades for database reference data
- **Magic Number Elimination**: Replaces hardcoded IDs throughout the codebase (e.g., `244` becomes `LoanType.ISQFT`)
- **Namespace Scoping**: `Core.Organizations.Enum` contains only organization-domain enumerations (distinct from `Core.Application.Enum` which contains cross-cutting types)

### Enum ID Allocation Strategy
The enum values use **non-sequential IDs** distributed across different numeric ranges:
- **1-99**: Core service types and document types (e.g., `DocumentType.W9 = 1`)
- **101-199**: Category groupings (e.g., `ServiceTypeCategory.Restoration = 101`)
- **171-199**: Service fee types (e.g., `ServiceFeeType.Loan = 171`)
- **201-210**: Document categories and franchisee call types
- **219-220**: Lead performance metrics
- **237-251**: Miscellaneous (franchisee notes, languages, loan types, payment frequency)
- **253-254**: Late additions (call management categories)
- **296-298**: Recent extensions (SEO charges, payment frequency variants)

**Why Non-Sequential?**: Numeric gaps allow for future insertions without breaking existing integrations or requiring enum value reassignments.

### Data Flow
1. **Entity Definition**: Domain entities (e.g., `FranchiseeLoan`, `FeeProfile`) declare `Lookup` foreign keys
2. **Enum Casting**: Service layer casts enum values to `long` for comparison: `(long)ServiceFeeType.Bookkeeping`
3. **Query Filtering**: Repository queries filter by enum-derived IDs: `where x.ServiceFeeTypeId == (long)ServiceFeeType.Loan`
4. **View Model Hydration**: Factories map Lookup entities back to human-readable strings via `Lookup.Name`
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Enum Definitions -->
## 🧬 Enum Type Definitions

### 1. **ServiceFeeType** (11 Values)
**Purpose**: Categorizes recurring and one-time service charges billed to franchisees beyond base royalty fees.

```csharp
namespace Core.Organizations.Enum
{
    public enum ServiceFeeType
    {
        Loan = 171,                  // Loan repayment principal
        Bookkeeping = 172,           // Fixed bookkeeping service fee
        PayrollProcessing = 173,     // Payroll service charges
        Recruiting = 174,            // Recruiting/staffing fees
        OneTimeProject = 175,        // Ad-hoc project fees (non-recurring)
        NationalCharge = 176,        // National account charges
        InterestAmount = 177,        // Loan interest component (separate from principal)
        VarBookkeeping = 178,        // Variable/percentage-based bookkeeping
        FRANCHISEETECHMAIL = 214,    // Technician email service
        PHONECALLCHARGES = 251,      // Call center phone charges
        SEOCharges = 296             // SEO marketing service fees
    }
}
```

**Key Relationships**:
- Referenced by: `FranchiseeServiceFee.ServiceFeeTypeId` (Domain)
- Used in: `FranchiseeServiceFeeService.cs` (billing logic)
- Special Logic:
  - `Bookkeeping` auto-creates a companion `VarBookkeeping` entry when configured with percentage
  - `OneTimeProject` is excluded from recurring fee aggregations
  - `InterestAmount` and `Loan` are handled separately in loan amortization schedules

**Usage Pattern**:
```csharp
// Filter out non-recurring fee types
var recurringFees = fees.Where(x => 
    x.Id != (long)ServiceFeeType.Loan &&
    x.Id != (long)ServiceFeeType.OneTimeProject &&
    x.Id != (long)ServiceFeeType.InterestAmount);

// Conditional processing based on fee type
if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping)
{
    // Create variable bookkeeping companion if percentage > 0
    var varFee = new FranchiseeServiceFee {
        TypeId = (long)ServiceFeeType.VarBookkeeping,
        Percentage = model.Percentage
    };
}
```

---

### 2. **ServiceType** (38 Values)
**Purpose**: Defines the catalog of restoration, maintenance, and product services offered by franchisees.

```csharp
namespace Core.Organizations.Enum
{
    public enum ServiceType
    {
        // Restoration Services (1-5)
        StoneLife = 1,
        Enduracrete = 2,
        Tilelok = 3,
        Vinylguard = 4,
        Counterlife = 5,
        
        // Maintenance Frequency (6-8)
        Monthly = 6,
        BiMonthly = 7,
        Quarterly = 8,
        
        // Specialized Services (9-17)
        Other = 9,
        ColorSeal = 10,
        CleanShield = 11,
        Wood = 12,
        Cleanair = 13,
        Fabricators = 14,
        MetalLife = 15,
        CarpetLife = 16,
        TileInstall = 17,
        
        // Product Channels (18-30)
        Product = 18,
        SalesTax = 19,
        WebMld = 20,
        WebFranchiseeSales = 21,
        WebJet = 22,
        WebWalmart = 23,
        WebAmazon = 24,
        WebAmazonPrime = 25,
        WebAmazonCanada = 26,
        Hardware = 27,
        Retail = 28,
        Testing = 29,
        MldWarehouse = 30,
        
        // Commercial Segments (31-32)
        Government = 31,
        Hotel = 32,
        
        // System (36-37)
        Admin = 36,
        OTHER = 37
    }
}
```

**Key Relationships**:
- Referenced by: `ServiceType` domain entity (maps to Lookup table via `CategoryId`)
- Used in: Job scheduling, invoice line items, sales tracking, service fee calculations
- Categorized by: `ServiceTypeCategory` enum (groups related services)

**Usage Pattern**:
```csharp
// Service types are stored in the ServiceType domain entity (confusing naming!)
// The enum provides IDs for filtering specific service categories
var restorationServices = services.Where(s => 
    s.CategoryId == (long)ServiceTypeCategory.Restoration);

// Product channel services (e.g., ecommerce)
var webChannels = services.Where(s => 
    s.Id >= (long)ServiceType.WebMld && 
    s.Id <= (long)ServiceType.WebAmazonCanada);
```

---

### 3. **ServiceTypeCategory** (5 Values)
**Purpose**: High-level grouping of `ServiceType` values for reporting and business logic segmentation.

```csharp
namespace Core.Organizations.Enum
{
    public enum ServiceTypeCategory
    {
        Restoration = 101,                 // Stone/tile restoration services
        Maintenance = 102,                 // Recurring maintenance contracts
        ProductChannel = 103,              // E-commerce and retail product sales
        FRONTOFFICECALLMANAGEMENT = 254,   // Call center operations
        MLDAndMLFS = 253                   // MLD warehouse and franchise sales
    }
}
```

**Key Relationships**:
- Referenced by: `ServiceType.CategoryId`, `ServiceType.SubCategoryId` (supports two-level hierarchy)
- Used in: Dashboard aggregations, sales reports, royalty calculation segmentation

**Business Logic**:
- **Restoration** services typically generate higher per-job revenue
- **Maintenance** contracts provide predictable recurring revenue
- **ProductChannel** sales may have different royalty percentage calculations
- **FRONTOFFICECALLMANAGEMENT** tracks call center service fees (not customer-facing services)

---

### 4. **DocumentType** (13 Values)
**Purpose**: Categorizes legal and compliance documents associated with franchisees.

```csharp
namespace Core.Organizations.Enum
{
    public enum DocumentType
    {
        W9 = 1,                          // Tax form W-9 (vendor info)
        LoanAgreement = 2,               // Franchisee loan contract
        AnnualTaxFilling = 3,            // Annual tax filing documents
        FranchiseeContract = 4,          // Franchise agreement
        COI = 5,                         // Certificate of Insurance
        EMPLOYEEHANDBOOK = 6,            // Employee handbook (compliance)
        HAZOMMANUAL = 7,                 // Hazmat/OSHA manual
        LICENSE = 8,                     // Business license (state/local)
        UPLOADTAXES = 9,                 // Tax document uploads
        FRANCHISEAGREEMENTSRENEWALS = 10,// Renewal contracts
        RESALECERTIFICATE = 11,          // Sales tax resale certificate
        NCA = 12,                        // National Contract Account docs
        NationalAccountAgreement = 13    // National account agreement
    }
}
```

**Key Relationships**:
- Referenced by: `FranchiseDocument.DocumentTypeId`, `FranchiseeDocumentType.TypeId`
- Used in: Document management service, compliance tracking, expiry notifications

**Special Behaviors**:
```csharp
// Documents requiring perpetuity tracking (never expire)
if (documentType.Id == (long)DocumentType.RESALECERTIFICATE || 
    documentType.Id == (long)DocumentType.LICENSE)
{
    franchiseeDocument.IsPerpetuity = true;
}

// Documents with role-based access control
if (documentType.Id == (long)DocumentType.NCA)
{
    // Only visible to owner who uploaded it
    visibleDocuments = documents.Where(x => x.UserId == currentUserId);
}

// System-excluded document types (not user-selectable)
var userSelectableTypes = documentTypes.Where(t => 
    t.CategoryId != (long)DocumentType.LoanAgreement &&
    t.CategoryId != (long)DocumentType.AnnualTaxFilling &&
    t.CategoryId != (long)DocumentType.FranchiseeContract);
```

---

### 5. **DocumentReportType** (9 Values)
**Purpose**: Subcategory of document types used specifically for financial and regulatory reporting.

```csharp
namespace Core.Organizations.Enum
{
    public enum DocumentReportType
    {
        AnnualTaxFiling = 3,             // Annual tax returns
        ProfitAndLossStatement = 15,     // P&L financial statement
        AnnualBalanceSheet = 16,         // Balance sheet
        AccountsPayable = 17,            // AP aging report
        AccountsReceivable = 18,         // AR aging report
        AnnualMaterialPurchase = 19,     // Material procurement report
        COI = 5,                         // Certificate of Insurance (overlaps DocumentType)
        License = 8,                     // Business license (overlaps DocumentType)
        ResaleCertificate = 11           // Sales tax cert (overlaps DocumentType)
    }
}
```

**Key Relationships**:
- Subset overlap with `DocumentType` (values 3, 5, 8, 11 exist in both enums)
- Used in: Financial reporting filters, compliance dashboards

**Why Two Document Enums?**:
- `DocumentType`: Broad categorization for document storage and retrieval
- `DocumentReportType`: Specialized subset for report generation and filtering
- The overlap (COI, License, ResaleCertificate) suggests these documents serve dual purposes: compliance storage + reporting artifacts

---

### 6. **DocumentCategory** (2 Values)
**Purpose**: Top-level categorization for document organization (organizational hierarchy).

```csharp
namespace Core.Organizations.Enum
{
    public enum DocumentCategory
    {
        FranchiseeManagementDocument = 201,  // Internal operations docs
        NationalAccountDocuments = 202       // National account contracts
    }
}
```

**Key Relationships**:
- Referenced by: `FranchiseeDocumentType.CategoryId` (via foreign key to Lookup table)
- Used in: Document filtering, access control segmentation

**Usage Pattern**:
```csharp
// Filter documents by top-level category
var managementDocs = documents.Where(d => 
    d.DocumentType.CategoryId == (long)DocumentCategory.FranchiseeManagementDocument);

// National account documents may have different retention policies
if (document.CategoryId == (long)DocumentCategory.NationalAccountDocuments)
{
    // Apply extended retention rules
}
```

---

### 7. **PaymentFrequency** (5 Values)
**Purpose**: Defines the schedule for royalty fee collection and service charge billing.

```csharp
namespace Core.Organizations.Enum
{
    public enum PaymentFrequency
    {
        Weekly = 31,         // Every week
        Monthly = 32,        // Once per month
        TwiceAMonth = 33,    // Semi-monthly (2x)
        FirstWeek = 297,     // First week of month only
        SecondWeek = 298     // Second week of month only
    }
}
```

**Key Relationships**:
- Referenced by: `FeeProfile.PaymentFrequencyId` (determines royalty billing cycle)
- Used in: Invoice generation scheduling, royalty calculation timing, payment reminders

**Business Logic**:
```csharp
// Monthly royalty invoice generation
if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly)
{
    // Generate single invoice on 1st of month
}
else if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.TwiceAMonth)
{
    // Generate invoices on 1st and 15th
}
```

**Notes**:
- `FirstWeek` and `SecondWeek` (IDs 297-298) are late additions (higher IDs), suggesting more granular scheduling needs emerged
- `Weekly = 31` aligns with other payment-related enums in the 31-33 range

---

### 8. **LoanType** (4 Values)
**Purpose**: Categorizes the purpose/program of loans issued to franchisees by corporate HQ.

```csharp
namespace Core.Organizations.Enum
{
    public enum LoanType
    {
        ISQFT = 244,            // iSquareFeet marketing program loan
        SurgicalStrike = 245,   // Targeted market penetration loan
        Geofence = 246,         // Geographic expansion loan
        Other = 247             // Miscellaneous/uncategorized
    }
}
```

**Key Relationships**:
- Referenced by: `FranchiseeLoan.LoanTypeId` (via Lookup foreign key)
- Used in: Loan origination forms, loan performance reporting

**Business Context**:
- **ISQFT**: iSquareFeet is a marketing/lead generation platform; loan funds franchisee participation
- **SurgicalStrike**: Loan for aggressive market entry strategies (e.g., heavy initial advertising)
- **Geofence**: Loan for expanding into new geographic territories
- **Other**: Catch-all for non-standard loan purposes

**Usage Pattern**:
```csharp
// Loan reporting by program type
var marketingLoans = loans.Where(l => 
    l.LoanTypeId == (long)LoanType.ISQFT ||
    l.LoanTypeId == (long)LoanType.SurgicalStrike);

// Geofence loans may have different terms
if (loan.LoanTypeId == (long)LoanType.Geofence)
{
    // Apply geographic expansion incentives
}
```

---

### 9. **LanguageEnum** (2 Values)
**Purpose**: Specifies the language for franchisee communications and multilingual support.

```csharp
namespace Core.Organizations.Enum
{
    public enum LanguageEnum
    {
        English = 249,
        Spanish = 250
    }
}
```

**Key Relationships**:
- Likely referenced by: `Franchisee` entity (though not explicitly found in Domain folder - may be in UI layer)
- Used in: Email template selection, document generation localization, customer communication

**Notes**:
- Only 2 languages currently supported (English + Spanish)
- IDs 249-250 suggest this was a late addition to the system
- Spanish support indicates significant Hispanic franchisee/customer base

**Assumed Usage**:
```csharp
// Select email template based on franchisee language preference
var templateKey = franchisee.LanguageId == (long)LanguageEnum.Spanish 
    ? "invoice_reminder_es" 
    : "invoice_reminder_en";
```

---

### 10. **LeadPerformanceEnum** (2 Values)
**Purpose**: Tracks marketing spend categories for lead generation performance analysis.

```csharp
namespace Core.Organizations.Enum
{
    public enum LeadPerformanceEnum
    {
        PPCSPEND = 219,     // Pay-Per-Click advertising spend
        SEOCOST = 220       // SEO service/optimization costs
    }
}
```

**Key Relationships**:
- Referenced by: `LeadPerformanceFranchiseeDetails` domain entity
- Used in: `LeadPerformanceFranchiseeDetailsService.cs` (marketing ROI tracking)

**Business Logic**:
```csharp
// Calculate cost-per-lead for PPC campaigns
var ppcDetails = leadPerformance.Where(l => 
    l.LeadTypeId == (long)LeadPerformanceEnum.PPCSPEND);

var costPerLead = ppcDetails.Sum(x => x.Amount) / ppcDetails.Sum(x => x.LeadCount);

// SEO costs are often monthly fixed fees
var seoCosts = leadPerformance.Where(l => 
    l.LeadTypeId == (long)LeadPerformanceEnum.SEOCOST);
```

**Notes**:
- Part of the marketing analytics subsystem
- IDs 219-220 are in a mid-range cluster, suggesting these were added during a marketing feature buildout
- Integrates with `Core.MarketingLead` module for full lead tracking

---

### 11. **FranchiseeNotesEnum** (3 Values)
**Purpose**: Categorizes types of notes/comments attached to franchisee records.

```csharp
namespace Core.Organizations.Enum
{
    public enum FranchiseeNotesEnum
    {
        FRANCHISEEDURATION = 237,       // Notes about franchise term/duration
        NOTESFROMOWNER = 238,           // Notes added by franchisee owner
        NOTESFROMCALLCENTER = 239       // Notes added by call center staff
    }
}
```

**Key Relationships**:
- Referenced by: `FranchiseeNotes.NoteTypeId` (domain entity)
- Used in: Note filtering, role-based note visibility

**Usage Pattern**:
```csharp
// Filter notes by source
var ownerNotes = notes.Where(n => 
    n.NoteTypeId == (long)FranchiseeNotesEnum.NOTESFROMOWNER);

// Call center notes may have different edit permissions
if (note.NoteTypeId == (long)FranchiseeNotesEnum.NOTESFROMCALLCENTER)
{
    // Only call center role can edit
}

// Duration notes are system-generated during term changes
if (note.NoteTypeId == (long)FranchiseeNotesEnum.FRANCHISEEDURATION)
{
    note.IsSystemGenerated = true;
}
```

---

### 12. **FranchiseeCallCategory** (4 Values)
**Purpose**: Categorizes franchisee contact preferences for call center routing and scheduling.

```csharp
namespace Core.Organizations.Enum
{
    public enum FranchiseeCallCategory
    {
        FRONTOFFICE = 210,           // Route calls to front office staff
        OFFICEPERSON = 211,          // Route to specific office contact
        RESPONDWHENAVAILABLE = 212,  // No rush, respond when convenient
        RESPONDSNEXTDAY = 213        // Respond within next business day
    }
}
```

**Key Relationships**:
- Likely referenced by: Franchisee communication preferences (not found in Domain folder - possibly in UI/Settings)
- Used in: Call center routing logic, SLA tracking

**Business Rules**:
- **FRONTOFFICE**: Default routing to front desk/receptionist
- **OFFICEPERSON**: Direct routing to specific staff member
- **RESPONDWHENAVAILABLE**: Low priority, no SLA
- **RESPONDSNEXTDAY**: 24-hour SLA for response

**Assumed Usage**:
```csharp
// Call routing logic
switch (franchisee.CallCategoryId)
{
    case (long)FranchiseeCallCategory.FRONTOFFICE:
        RouteToFrontDesk(call);
        break;
    case (long)FranchiseeCallCategory.OFFICEPERSON:
        RouteToSpecificPerson(call, franchisee.PreferredContactId);
        break;
    case (long)FranchiseeCallCategory.RESPONDSNEXTDAY:
        SetSLA(call, hours: 24);
        break;
}
```

---

### 13. **OrganizationNames** (4 Values)
**Purpose**: Hardcoded lookup IDs for specific franchisee organizations (appears to be legacy/test data).

```csharp
namespace Core.Organizations.Enum
{
    public enum OrganizationNames
    {
        MIDetroit = 62,
        PAPittsburgh = 64,
        PAPhiladelphia = 42,
        MIGrandRapids = 27
    }
}
```

**Key Relationships**:
- References specific `Organization.Id` primary key values (not Lookup table)
- Used in: Legacy code, specific business logic tied to pilot franchisees

**⚠️ Anti-Pattern Warning**:
This enum violates the Lookup Table Pattern used by other enums. Instead of mapping to a Lookup type, it hardcodes specific Organization PKs. This suggests:
1. These 4 franchisees were pilot/test locations
2. Special business logic was written specifically for them
3. The enum should likely be **refactored out** or converted to configuration

**Why This Exists**:
- Early development likely used these 4 franchisees for testing
- Code paths with `if (franchiseeId == (long)OrganizationNames.MIDetroit)` may still exist
- Modern approach: Use feature flags or configuration instead of hardcoded IDs

**Recommended Search**:
```bash
# Find usages of this enum to assess refactoring impact
grep -r "OrganizationNames\." src/
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage Patterns -->
## 🔌 Common Usage Patterns

### Pattern 1: Enum-to-Lookup Casting
**Problem**: Need to query domain entities by specific Lookup table rows.  
**Solution**: Cast enum value to `long` and compare to foreign key property.

```csharp
// ✅ Correct: Type-safe constant with explicit cast
var bookkeepingFees = serviceFees.Where(x => 
    x.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping);

// ❌ Incorrect: Magic number (fragile, unreadable)
var bookkeepingFees = serviceFees.Where(x => 
    x.ServiceFeeTypeId == 172);

// ✅ Multiple enum filtering
var excludedTypes = new[] {
    (long)ServiceFeeType.Loan,
    (long)ServiceFeeType.OneTimeProject,
    (long)ServiceFeeType.InterestAmount
};
var recurringFees = fees.Where(f => !excludedTypes.Contains(f.TypeId));
```

### Pattern 2: Conditional Business Logic Branching
**Problem**: Different enum values require different processing logic.  
**Solution**: Switch or if-else on enum-cast comparisons.

```csharp
// Conditional fee processing
if (fee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping)
{
    // Bookkeeping can be fixed or percentage-based
    if (fee.Percentage > 0)
    {
        // Create companion VarBookkeeping entry
        var variableFee = new FranchiseeServiceFee {
            ServiceFeeTypeId = (long)ServiceFeeType.VarBookkeeping,
            Percentage = fee.Percentage,
            Amount = 0
        };
        repository.Add(variableFee);
    }
}
else if (fee.ServiceFeeTypeId == (long)ServiceFeeType.Loan)
{
    // Loan fees require amortization schedule generation
    GenerateLoanSchedule(fee);
}
```

### Pattern 3: Enum-Driven View Model Hydration
**Problem**: UI needs human-readable names, but DB stores integer IDs.  
**Solution**: Factories join Lookup table and map to ViewModel strings.

```csharp
// Factory pattern for ViewModel population
public FranchiseeServiceFeeViewModel CreateViewModel(FranchiseeServiceFee domain)
{
    return new FranchiseeServiceFeeViewModel
    {
        Id = domain.Id,
        ServiceFeeTypeId = domain.ServiceFeeTypeId,
        ServiceFeeTypeName = domain.ServiceFeeType?.Name ?? "Unknown", // Lookup.Name
        Amount = domain.Amount,
        Percentage = domain.Percentage,
        IsLoanType = domain.ServiceFeeTypeId == (long)ServiceFeeType.Loan,
        IsRecurring = domain.ServiceFeeTypeId != (long)ServiceFeeType.OneTimeProject
    };
}
```

### Pattern 4: Repository Query Filters
**Problem**: Need to filter domain entities by enum-categorized attributes.  
**Solution**: Use enum casts directly in LINQ `Where()` clauses.

```csharp
// Query filtering by enum values
public IEnumerable<FranchiseDocument> GetComplianceDocuments(long franchiseeId)
{
    var complianceTypes = new[] {
        (long)DocumentType.COI,
        (long)DocumentType.LICENSE,
        (long)DocumentType.RESALECERTIFICATE
    };
    
    return _documentRepository.Table
        .Where(d => d.FranchiseeId == franchiseeId)
        .Where(d => complianceTypes.Contains(d.DocumentTypeId))
        .Where(d => d.IsActive)
        .OrderBy(d => d.DocumentType.Name);
}
```

### Pattern 5: Enum-Based Validation Rules
**Problem**: Certain enum values have special validation requirements.  
**Solution**: Implement conditional validation in FluentValidation or service layer.

```csharp
// Conditional validation based on enum value
public class FranchiseeServiceFeeValidator : AbstractValidator<FranchiseeServiceFeeEditModel>
{
    public FranchiseeServiceFeeValidator()
    {
        // Bookkeeping requires either fixed amount OR percentage (not both)
        When(x => x.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping, () =>
        {
            RuleFor(x => x)
                .Must(x => (x.Amount > 0 && x.Percentage == 0) || 
                           (x.Amount == 0 && x.Percentage > 0))
                .WithMessage("Bookkeeping must specify either Amount or Percentage, not both.");
        });

        // Loan type requires loan reference
        When(x => x.ServiceFeeTypeId == (long)ServiceFeeType.Loan, () =>
        {
            RuleFor(x => x.LoanId)
                .NotNull()
                .WithMessage("Loan service fee requires a valid LoanId.");
        });
    }
}
```

### Pattern 6: Enum Exclusion Lists
**Problem**: Certain operations should skip specific enum values.  
**Solution**: Define exclusion arrays and use `!Contains()` or `Where()` negation.

```csharp
// Exclude non-user-selectable document types
var systemDocumentTypes = new[] {
    (long)DocumentType.LoanAgreement,
    (long)DocumentType.AnnualTaxFilling,
    (long)DocumentType.FranchiseeContract
};

var userSelectableTypes = _documentTypeRepository.Table
    .Where(dt => !systemDocumentTypes.Contains(dt.Id))
    .Where(dt => dt.IsActive)
    .ToList();

// Exclude non-recurring service fees from monthly totals
var nonRecurringFees = new[] {
    (long)ServiceFeeType.Loan,
    (long)ServiceFeeType.OneTimeProject,
    (long)ServiceFeeType.InterestAmount
};

var monthlyRecurringTotal = fees
    .Where(f => !nonRecurringFees.Contains(f.ServiceFeeTypeId))
    .Sum(f => f.Amount);
```

### Pattern 7: Enum-to-Lookup FK Assignment
**Problem**: Creating new domain entities requires setting Lookup foreign keys.  
**Solution**: Assign enum cast directly to `XxxId` foreign key property.

```csharp
// Creating a new service fee with enum-based type assignment
var newFee = new FranchiseeServiceFee
{
    FranchiseeId = franchiseeId,
    ServiceFeeTypeId = (long)ServiceFeeType.PayrollProcessing, // Enum cast to FK
    Amount = 150.00m,
    StartDate = DateTime.UtcNow,
    IsActive = true
};
_repository.Add(newFee);

// Creating loan with specific type
var loan = new FranchiseeLoan
{
    FranchiseeId = franchiseeId,
    LoanTypeId = (long)LoanType.ISQFT, // Enum cast
    Amount = 50000m,
    Duration = 36,
    InterestratePerAnum = 5.5m
};
_loanRepository.Add(loan);
```

### Pattern 8: Enum-Driven UI Behavior
**Problem**: Frontend needs to show/hide fields based on enum selection.  
**Solution**: Pass enum IDs to ViewModel and use client-side logic.

```csharp
// ViewModel includes enum constants for UI logic
public class FranchiseeServiceFeeViewModel
{
    public long ServiceFeeTypeId { get; set; }
    public string ServiceFeeTypeName { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
    
    // Helper properties for UI conditional rendering
    public bool IsBookkeeping => ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping ||
                                 ServiceFeeTypeId == (long)ServiceFeeType.VarBookkeeping;
    public bool IsLoan => ServiceFeeTypeId == (long)ServiceFeeType.Loan;
    public bool SupportsPercentage => IsBookkeeping || 
                                      ServiceFeeTypeId == (long)ServiceFeeType.NationalCharge;
}
```

**AngularJS Client-Side**:
```javascript
// In controller/directive
$scope.isBookkeepingFee = function(fee) {
    return fee.ServiceFeeTypeId === 172 || fee.ServiceFeeTypeId === 178; // Bookkeeping or VarBookkeeping
};

$scope.showPercentageField = function(fee) {
    return $scope.isBookkeepingFee(fee) || fee.ServiceFeeTypeId === 176; // NationalCharge
};
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Cross-Module Linking

### Internal Dependencies
| Module | Relationship | Usage |
|--------|--------------|-------|
| [Core.Application.Domain](../../Application/Domain/AI-CONTEXT.md) | **Upstream** | `Lookup` entity is the target of enum FK references |
| [Core.Organizations.Domain](../Domain/AI-CONTEXT.md) | **Downstream** | Domain entities reference these enums via `Lookup` FKs |
| [Core.Organizations.Impl](../Impl/AI-CONTEXT.md) | **Downstream** | Service/Factory layers cast enums for queries and validation |
| [Core.Organizations.ViewModel](../ViewModel/) | **Downstream** | ViewModels expose enum IDs and names to UI |

### External Dependencies
None (pure C# enums with no external package dependencies).

### Reverse Dependencies (Who Uses This Module)
- **ORM Layer** (`src/ORM`): Entity Framework mappings for Lookup table
- **API Layer** (`src/API`): Controllers receive enum IDs in request DTOs
- **Web.UI** (`src/Web.UI`): AngularJS dropdowns bind to enum-derived Lookup lists

### Cross-Cutting Concerns
- **Lookup Table Seeding**: These enum values must exist in the `Lookup` table via SQL migration scripts
- **Database Constraints**: Foreign keys in Domain entities reference `Lookup.Id` where `Lookup.LookupTypeId` matches the enum's logical grouping
- **Enum Value Immutability**: Changing enum values requires coordinated database migration + code deployment

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Gotchas & Edge Cases -->
## ⚠️ Critical Gotchas

### 1. **Enum ID Stability**
**Problem**: Enum values are hardcoded integers that correspond to database primary keys.  
**Impact**: Changing an enum value (e.g., `ServiceFeeType.Loan = 171` → `172`) will break all references unless the database is also updated.  
**Mitigation**: Treat enum values as **immutable**. Add new values with new IDs instead of modifying existing ones.

### 2. **OrganizationNames Anti-Pattern**
**Problem**: `OrganizationNames` hardcodes specific franchisee `Organization.Id` values, not `Lookup.Id` values.  
**Impact**: Violates the Lookup Table Pattern; creates brittle coupling to specific franchisee records.  
**Mitigation**: Refactor to use feature flags or configuration. Search for usages via `grep -r "OrganizationNames\."` before making changes.

### 3. **Enum Overlap: DocumentType vs DocumentReportType**
**Problem**: Values 3, 5, 8, 11 exist in **both** `DocumentType` and `DocumentReportType` enums.  
**Impact**: Using the wrong enum in a query can lead to subtle bugs (e.g., filtering by `DocumentReportType.COI = 5` instead of `DocumentType.COI = 5`).  
**Mitigation**: Prefer `DocumentType` for storage/retrieval; use `DocumentReportType` only for report-specific filters.

### 4. **Missing Enum Members**
**Problem**: Not all Lookup table rows have corresponding enum members (e.g., dynamic user-created lookups).  
**Impact**: Queries using enum casts will miss dynamically-added lookup values.  
**Mitigation**: Use defensive coding:
```csharp
// ✅ Safe: Handle unknown lookups gracefully
var serviceTypeName = serviceType.Lookup?.Name ?? "Unknown";

// ❌ Unsafe: Assumes all lookups have enum equivalents
var enumValue = (ServiceFeeType)serviceType.LookupId; // Throws InvalidCastException if no match
```

### 5. **ServiceFeeType.Bookkeeping Auto-Expansion**
**Problem**: Creating a `Bookkeeping` fee with percentage > 0 auto-creates a `VarBookkeeping` companion entry.  
**Impact**: Deleting the `Bookkeeping` fee may leave orphaned `VarBookkeeping` records.  
**Mitigation**: Service layer must handle cascade deletes:
```csharp
if (deletedFee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping)
{
    var varFee = fees.FirstOrDefault(f => 
        f.ServiceFeeTypeId == (long)ServiceFeeType.VarBookkeeping &&
        f.FranchiseeId == deletedFee.FranchiseeId);
    if (varFee != null) repository.Delete(varFee);
}
```

### 6. **Non-Sequential ID Gaps**
**Problem**: Enum IDs have large gaps (e.g., `ServiceType.Admin = 36`, `ServiceType.OTHER = 37`, but no 33-35).  
**Impact**: Cannot assume sequential iteration (e.g., `for (int i = 1; i <= 37; i++)`).  
**Mitigation**: Use `Enum.GetValues()` or query the Lookup table dynamically:
```csharp
// ✅ Safe: Get all actual enum values
var allServiceTypes = Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>();

// ❌ Unsafe: Assumes sequential IDs
for (int i = 1; i <= 37; i++) {
    var type = (ServiceType)i; // Fails for 33-35 (undefined values)
}
```

### 7. **Case Sensitivity: Enum Naming**
**Problem**: Some enums use `ALLCAPS` (e.g., `FRONTOFFICE`), others use `PascalCase` (e.g., `Bookkeeping`).  
**Impact**: Inconsistent naming can cause typos and makes code harder to read.  
**Mitigation**: Follow C# convention (PascalCase) for new enum members. Consider refactoring legacy `ALLCAPS` names (breaking change).

### 8. **Enum vs Lookup Name Mismatch**
**Problem**: Enum member names may not match `Lookup.Name` in the database (e.g., `DocumentType.COI` vs `Lookup.Name = "Certificate of Insurance"`).  
**Impact**: Cannot reliably use `Enum.ToString()` for UI display.  
**Mitigation**: Always hydrate `Lookup.Name` from the database for user-facing strings:
```csharp
// ✅ Correct: Use database name
documentViewModel.TypeName = document.DocumentType.Name;

// ❌ Incorrect: Enum name is not user-friendly
documentViewModel.TypeName = ((DocumentType)document.DocumentTypeId).ToString(); // "COI" instead of "Certificate of Insurance"
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Migration Guide -->
## 📦 Database Migration Coordination

### Enum-to-Lookup Table Seeding
These enums are **NOT** dynamically generated from the database. They are **hardcoded constants** that must match the `Lookup` table. When deploying enum changes:

1. **Step 1: SQL Migration** (add Lookup row)
   ```sql
   -- Example: Adding new ServiceFeeType
   INSERT INTO Lookup (Id, Name, LookupTypeId, IsActive)
   VALUES (299, 'Website Hosting', 170, 1); -- LookupTypeId 170 = ServiceFeeType
   ```

2. **Step 2: Update Enum** (add C# enum member)
   ```csharp
   public enum ServiceFeeType
   {
       // ... existing members
       WebsiteHosting = 299  // NEW
   }
   ```

3. **Step 3: Deploy** (code + database must sync)
   - Deploy SQL migration first (backward compatible)
   - Then deploy code with new enum member
   - **Never** deploy code before database (causes FK constraint violations)

### LookupType Mappings
Each enum corresponds to a specific `LookupType.Id` in the database:

| Enum | LookupType.Id | LookupType.Name |
|------|---------------|-----------------|
| ServiceFeeType | 170 | ServiceFeeTypes |
| ServiceType | 1 | ServiceTypes |
| ServiceTypeCategory | 100 | ServiceCategories |
| DocumentType | 200 | DocumentTypes |
| DocumentReportType | 215 | ReportTypes |
| DocumentCategory | 201 | DocumentCategories |
| PaymentFrequency | 30 | PaymentFrequencies |
| LoanType | 243 | LoanTypes |
| LanguageEnum | 248 | Languages |
| LeadPerformanceEnum | 218 | LeadPerformanceTypes |
| FranchiseeNotesEnum | 236 | NoteTypes |
| FranchiseeCallCategory | 209 | CallCategories |

**⚠️ Note**: These `LookupTypeId` values are inferred from enum ID ranges and may not be exact. Verify against actual `LookupType` table data.

<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Refactoring Considerations -->
## 🔧 Future Refactoring Opportunities

### 1. Convert to Strongly-Typed Lookup Repository
**Current State**: Manual enum casting and FK comparisons scattered throughout codebase.  
**Proposed**: Generic `LookupRepository<TEnum>` that handles enum-to-Lookup mapping.

```csharp
// Proposed API
public interface ILookupRepository<TEnum> where TEnum : Enum
{
    Lookup GetByEnum(TEnum enumValue);
    IEnumerable<Lookup> GetAll();
    bool Exists(TEnum enumValue);
}

// Usage
var bookkeepingLookup = _lookupRepo<ServiceFeeType>.GetByEnum(ServiceFeeType.Bookkeeping);
var allServiceFees = _lookupRepo<ServiceFeeType>.GetAll();
```

### 2. Eliminate OrganizationNames Enum
**Problem**: Hardcoded franchisee PKs violate abstraction and create test brittleness.  
**Solution**: Use feature flags or configuration:

```csharp
// Replace enum with config-driven approach
public class PilotFranchiseeConfig
{
    public long[] PilotFranchiseeIds { get; set; } // From appsettings.json
}

// Usage
var isPilotFranchisee = _pilotConfig.PilotFranchiseeIds.Contains(franchiseeId);
```

### 3. Normalize DocumentType Overlap
**Problem**: `DocumentType` and `DocumentReportType` have overlapping values (COI, License, etc.).  
**Solution**: Merge into single enum with category flags or create inheritance hierarchy.

```csharp
// Option A: Flags-based approach
[Flags]
public enum DocumentTypeFlags
{
    None = 0,
    Storage = 1,        // Stored in document repository
    Reporting = 2,      // Used in financial reports
    Compliance = 4      // Subject to expiry/renewal tracking
}

public class DocumentTypeMetadata
{
    public DocumentType Type { get; set; }
    public DocumentTypeFlags Flags { get; set; }
}
```

### 4. Add Enum Member Descriptions
**Problem**: UI displays require human-readable descriptions beyond `Lookup.Name`.  
**Solution**: Use `[Description]` attributes for richer metadata.

```csharp
using System.ComponentModel;

public enum ServiceFeeType
{
    [Description("Loan principal repayment installment")]
    Loan = 171,
    
    [Description("Fixed-rate bookkeeping service fee")]
    Bookkeeping = 172,
    
    [Description("Variable bookkeeping based on revenue percentage")]
    VarBookkeeping = 178
}

// Usage via extension method
public static string GetDescription(this Enum enumValue)
{
    var field = enumValue.GetType().GetField(enumValue.ToString());
    var attr = field.GetCustomAttribute<DescriptionAttribute>();
    return attr?.Description ?? enumValue.ToString();
}
```

<!-- END CUSTOM SECTION -->
