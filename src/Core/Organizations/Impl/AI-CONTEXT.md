<!-- AUTO-GENERATED: Header -->
# Organizations.Impl Module Context
**Version**: a0d980921a03670c322f97f209626ffcaf1e9e66
**Generated**: 2026-02-04T06:40:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module contains the **concrete implementations** of all business logic related to franchisee management, organizational hierarchy, role-based access control, royalty fee calculations, and service pricing management. It serves as the **operational brain** of the Organizations domain, orchestrating complex workflows between franchisees, fee structures, services, and financial calculations.

### Design Patterns
- **Factory Pattern**: Extensive use across all factories (`FranchiseeFactory`, `OrganizationFactory`, `FeeProfileFactory`, `RoyaltyFeeSlabsFactory`) to handle:
  - Domain ↔ EditModel ↔ ViewModel transformations
  - Complex object graph construction with proper relationship management
  - Conditional logic based on create vs. update operations
  
- **Service Layer Pattern**: Service classes (`FranchiseeInfoService`, `FranchiseeServiceFeeService`, `FranchiseeSalesService`) encapsulate:
  - Multi-step business workflows
  - Transaction boundary management
  - Cross-cutting concerns (logging, validation)
  
- **Repository Pattern**: All implementations depend on `IRepository<T>` for data access, maintaining separation of concerns
  
- **Validation Strategy**: FluentValidation used for declarative model validation (`FranchiseeEditModelValidator`, `RoyaltyFeeSlabsEditModelValidator`)

- **Dependency Injection**: All classes use constructor injection with `[DefaultImplementation]` attribute for Unity container registration

### Data Flow
1. **Franchisee Creation/Update Flow**:
   ```
   API Controller → FranchiseeInfoService.Save()
   ├─> FranchiseeFactory.CreateDomain() [Transform EditModel]
   │   ├─> OrganizationFactory.CreateDomain() [Base org data]
   │   ├─> FeeProfileFactory.CreateDomain() [Royalty structure]
   │   │   └─> RoyaltyFeeSlabsFactory.CreateDomainCollection() [Tiered rates]
   │   ├─> LateFeeFactory.CreateDomain() [Penalty structure]
   │   ├─> FranchiseeServicesFactory.CreateDomainCollection() [Service offerings]
   │   └─> FranchiseeServiceFeeFactory.CreateDomain() [Service pricing]
   └─> Repository.Save() → Database Persistence
   ```

2. **Royalty Calculation Flow**:
   ```
   Sales Data Upload → FranchiseeSalesService
   ├─> Load FeeProfile with RoyaltyFeeSlabs
   ├─> Apply tiered percentage calculation based on sales ranges
   ├─> Calculate MinimumRoyaltyPerMonth enforcement
   ├─> Apply AdFundPercentage
   └─> Generate Invoice Items
   ```

3. **Hierarchical Access Control**:
   ```
   User Authentication → OrganizationRoleUser lookup
   ├─> Load default franchisee (IsDefault=true)
   ├─> Retrieve accessible franchisees via OrganizationRoleUserFranchisee
   ├─> Apply RoleType-based permissions (SuperAdmin, FranchiseeAdmin, etc.)
   └─> Filter data visibility
   ```

### Key Architectural Decisions
- **Composite Pattern**: `Franchisee` is an aggregate root containing multiple child entities (FeeProfile, LateFee, Services, Documents)
- **Optimistic Updates**: Factories check for `inDb` parameter to determine if entity is new or updating existing
- **Soft Deletes**: `IsDeleted` flag used throughout rather than hard deletes
- **Multi-Tenancy**: Every entity scoped to `FranchiseeId` for data isolation
- **Currency Support**: Built-in currency exchange rate handling in loan and fee calculations

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Core Domain Entities (Managed by Factories)
```csharp
// Central aggregate root
public class Franchisee : DomainBase
{
    public Organization Organization { get; set; }  // 1:1 - Base organizational data
    public FeeProfile FeeProfile { get; set; }      // 1:1 - Royalty calculation rules
    public LateFee LateFee { get; set; }            // 1:1 - Late payment penalties
    public ICollection<FranchiseeService> FranchiseeServices { get; set; }  // 1:N - Service offerings
    public ICollection<FranchiseeServiceFee> FranchiseeServiceFee { get; set; }  // 1:N - Service pricing
    public ICollection<FranchiseeNotes> FranchiseeNotes { get; set; }
    public ICollection<FranchiseeDocumentType> FranchiseeDocumentType { get; set; }
    
    // Financial Fields
    public string Currency { get; set; }  // USD, CAD, etc.
    public bool IsRoyality { get; set; }  // Whether royalty fees apply
    public decimal? SalesTax { get; set; }
    
    // Business Metadata
    public string QuickBookIdentifier { get; set; }
    public long? BusinessId { get; set; }
    public string DisplayName { get; set; }
    public string WebSite { get; set; }
    
    // Contact Information
    public string ContactEmail { get; set; }
    public string AccountPersonEmail { get; set; }
    public string MarketingPersonEmail { get; set; }
    public string SchedulerEmail { get; set; }
}

// Royalty calculation configuration
public class FeeProfile : DomainBase
{
    public long Id { get; set; }  // Same as FranchiseeId (1:1 relationship)
    
    // Calculation Strategy
    public bool SalesBasedRoyalty { get; set; }  // true = tiered slabs, false = fixed
    public decimal? FixedAmount { get; set; }    // Used when SalesBasedRoyalty = false
    
    // Sales-Based Configuration
    public ICollection<RoyaltyFeeSlabs> RoyaltyFeeSlabs { get; set; }  // Tiered percentage structure
    public decimal? MinimumRoyaltyPerMonth { get; set; }  // Floor amount enforcement
    
    // Additional Fees
    public decimal? AdFundPercentage { get; set; }  // Advertising fund contribution
    
    // Billing Configuration
    public long? PaymentFrequencyId { get; set; }  // Monthly, Quarterly, etc.
}

// Tiered royalty percentage structure
public class RoyaltyFeeSlabs : DomainBase
{
    public long RoyaltyFeeProfileId { get; set; }
    public decimal MinValue { get; set; }          // Sales range start (e.g., $0)
    public decimal? MaxValue { get; set; }         // Sales range end (e.g., $50,000) - null = infinity
    public decimal ChargePercentage { get; set; }  // Percentage to apply in this range (e.g., 7%)
}

// Minimum royalty enforcement by sales range
public class MinRoyaltyFeeSlabs : DomainBase
{
    public long FranchiseeId { get; set; }
    public decimal StartValue { get; set; }    // Sales threshold start
    public decimal? EndValue { get; set; }     // Sales threshold end (null = 2900+)
    public decimal MinRoyality { get; set; }   // Minimum royalty amount for this range
}

// Late payment penalty configuration
public class LateFee : DomainBase
{
    public long Id { get; set; }  // Same as FranchiseeId
    
    // Royalty Late Fees
    public decimal RoyalityLateFee { get; set; }                     // Flat penalty amount
    public int RoyalityWaitPeriodInDays { get; set; }               // Grace period before penalty
    public decimal RoyalityInterestRatePercentagePerAnnum { get; set; }  // Annual interest rate
    
    // Sales Data Late Fees
    public decimal SalesDataLateFee { get; set; }                    // Penalty for late reporting
    public int SalesDataWaitPeriodInDays { get; set; }              // Grace period for sales submission
}

// Service-specific pricing
public class FranchiseeServiceFee : DomainBase
{
    public long FranchiseeId { get; set; }
    public long ServiceFeeTypeId { get; set; }  // Lookup: Bookkeeping, Website, SEO, etc.
    public long? FrequencyId { get; set; }      // Monthly, Quarterly, Annual
    public decimal Amount { get; set; }         // Fixed amount
    public decimal Percentage { get; set; }     // Or percentage-based
    public bool IsActive { get; set; }          // Whether fee is currently applied
}

// Loan management for franchisees
public class FranchiseeLoan : DomainBase
{
    public long FranchiseeId { get; set; }
    public decimal Amount { get; set; }                      // Principal
    public int Duration { get; set; }                        // Term in months
    public decimal InterestratePerAnum { get; set; }        // Annual interest rate
    public DateTime StartDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool? IsRoyality { get; set; }                   // Adjust royalty (true) or adfund (false)
    public long? LoanTypeId { get; set; }                   // Equipment, Technology, etc.
    public long CurrencyExchangeRateId { get; set; }
    public ICollection<FranchiseeLoanSchedule> FranchiseeLoanSchedule { get; set; }  // Amortization schedule
}
```

### ViewModel/EditModel Patterns
```csharp
// Used for CRUD operations
public class FranchiseeEditModel
{
    // Inherits Organization fields
    public string Name { get; set; }
    public string Email { get; set; }
    public AddressEditModel Address { get; set; }
    public List<PhoneEditModel> PhoneNumbers { get; set; }
    
    // Franchisee-specific
    public string Currency { get; set; }
    public FeeProfileEditModel FeeProfile { get; set; }
    public LateFeeEditModel LateFee { get; set; }
    public List<FranchiseeServiceEditModel> FranchiseeServices { get; set; }
    public List<FranchiseeServiceFeeEditModel> ServiceFees { get; set; }
    
    // Owner Information
    public FranchiseeOwnerModel OrganizationOwner { get; set; }  // Links to Person
}

// Lightweight for list views
public class FranchiseeViewModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string OwnerName { get; set; }
    public string Currency { get; set; }
    public decimal AccountCredit { get; set; }  // Sum of remaining credits
    public bool IsActive { get; set; }
    public string SalesReportStatus { get; set; }  // Latest upload status
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces & Key Methods

### IFranchiseeFactory
**Purpose**: Orchestrates complex Franchisee domain object construction
```csharp
// Domain Creation
Franchisee CreateDomain(FranchiseeEditModel model, Franchisee inDb);
// - Constructs full aggregate including FeeProfile, LateFee, Services
// - Handles create vs. update logic via inDb parameter
// - Delegates to specialized factories for nested objects

Organization CreateOrgDomain(FranchiseeEditModel model);
// - Creates base Organization entity
// - Includes Address and Phone collections

// ViewModel Creation
FranchiseeEditModel CreateEditModel(Franchisee domain, Person personDomain);
// - Flattens aggregate for editing
// - Joins owner Person data
// - Includes all nested fee and service configurations

FranchiseeViewModel CreateViewModel(Franchisee domain, List<FranchiseeDurationNotesHistry> list);
// - Lightweight summary for listings
// - Calculates AccountCredit sum
// - Includes latest notes and sales report status

FranchiseeRedesignViewModel CreateResignViewModel(Franchisee domain, List<long> orgIdList, long? roleId, List<FranchiseeDurationNotesHistry> durationList);
// - Enhanced view with address breakdown
// - Phone categorization (Business, Cell, Office, CallCenter)
// - Access control via orgIdList and roleId
// - Hardcoded "bold" list for special franchisees (MI-Detroit, etc.)
```

### IFranchiseeInfoService
**Purpose**: Main orchestration service for franchisee lifecycle
```csharp
// CRUD Operations
Franchisee Save(FranchiseeEditModel model);
// - Creates or updates franchisee with full cascade
// - Transaction boundary management
// - Returns persisted entity with IDs

bool Delete(long franchiseeId);
// - Soft cascading delete
// - Checks for active sales data (blocks delete if exists)
// - Cleans up FeeProfile, RoyaltyFeeSlabs, Services, ServiceFees, PaymentProfile, OrganizationRoleUser

// Retrieval
Franchisee Get(long franchiseeId);
FranchiseeEditModel GetEditModel(long franchiseeId);
FranchiseeViewModel GetFranchiseeByOrganizationId(long organizationId);

// List/Search Operations
FranchiseeListViewModel GetFranchisees(FranchiseeListFilter filter, int pageNumber, int pageSize);
// - Advanced filtering (state, city, country, BusinessId, IsActive)
// - Sorting support (Name, OwnerName, Email, AccountCredit)
// - Pagination
// - Includes credit balance calculation

// Reporting
List<FranchiseeViewModel> DownloadList(FranchiseeListFilter filter);
// - Excel export of franchisee list
// - Formatted with addresses and phone numbers

// Document Management
List<FranchiseeDocumentEditModel> GetFranchiseeDocuments(long franchiseeId);
bool SaveFranchiseeDocument(FranchiseeDocumentEditModel model);
```

### IFeeProfileFactory
**Purpose**: Manages royalty fee structure
```csharp
FeeProfile CreateDomain(FeeProfileEditModel model, long franchiseeId, FeeProfile inDb);
// - Creates/updates FeeProfile with RoyaltyFeeSlabs collection
// - Validates MinValue < MaxValue for slabs
// - Handles slab addition/removal logic

FeeProfileEditModel CreateEditModel(FeeProfile domain, List<MinRoyaltyFeeSlabs> minRoyalitySlabs);
// - Includes both regular slabs and minimum royalty slabs
// - Used for configuration UI
```

### IRoyaltyFeeSlabsFactory
**Purpose**: Manages tiered royalty percentage structure
```csharp
IEnumerable<RoyaltyFeeSlabs> CreateDomainCollection(IEnumerable<RoyaltyFeeSlabsEditModel> modelCollection, FeeProfile feeProfile);
// - Synchronizes collection: updates existing, adds new, deletes removed
// - Maintains order of slabs (index-based matching)
// - **Critical**: If slabs are reduced, deletes excess from DB
```

### IFranchiseeServiceFeeFactory
**Purpose**: Handles service-specific pricing and loans
```csharp
ICollection<FranchiseeServiceFee> CreateDomain(ICollection<FranchiseeServiceFeeEditModel> serviceFeeList, Franchisee franchisee);
// - Creates service fees for applicable services
// - **Special Logic**: If Bookkeeping service exists, auto-creates VarBookkeeping fee
// - Filters out inactive or zero-amount fees

OneTimeProjectFee CreateOneTimeProject(FranchiseeServiceFeeEditModel serviceFee);
// - Creates one-time project charges
// - Tracks CurrencyExchangeRate at time of creation

FranchiseeLoan CreateFranchiseeLoan(FranchiseeServiceFeeEditModel serviceFee);
// - Creates loan with amortization schedule
// - Tracks IsRoyality flag (adjusts royalty vs. adfund)
// - Includes interest rate and duration
```

### IFranchiseeServiceFeeService
**Purpose**: Complex service fee and loan management
```csharp
bool SaveServiceFee(List<FranchiseeServiceFeeEditModel> model, long franchiseeId);
// - Saves/updates multiple service fees
// - Handles Bookkeeping → VarBookkeeping special case

FranchiseeLoan SaveLoan(FranchiseeServiceFeeEditModel model, long franchiseeId);
// - Creates loan with full amortization schedule
// - Calculates payment schedule based on interest rate and duration

bool UpdateLoanAdjustment(FranchiseeChangeServiceFee model);
// - Switches loan between Royalty and Adfund adjustment
// - Creates audit trail in LoanAdjustmentAudit
// - Updates all unpaid loan schedule items

bool PayLoan(LoanScheduleViewModel model);
// - Processes loan payment
// - Updates Principal, Interest, Balance
// - Handles overpayment
// - Marks schedule item as paid
```

### IFranchiseeSalesService
**Purpose**: Sales data management and reporting
```csharp
FranchiseeSales Save(FranchiseeSalesEditModel model);
// - Records sales transactions
// - Links to Customer, MarketingClass, Invoice

SalesDataListViewModel GetSalesData(SalesDataListFilter filter, int pageNumber, int pageSize);
// - Advanced filtering (franchisee, customer, date range, marketing class)
// - Sorting support
// - Used for sales reporting and royalty calculation basis

FranchiseeSales Get(string qbInvoiceNumber, long franchiseeId, string customerName);
// - Finds existing sales record
// - **Special Logic**: Customer name matching with comma/space normalization
```

### IFranchiseeDocumentService
**Purpose**: Document management for franchisees
```csharp
bool SaveDocuments(FranchiseeDocumentEditModel model);
// - Uploads and categorizes documents (NCA, Franchise Agreement, Insurance, etc.)
// - Supports multi-franchisee upload (same doc to multiple franchisees)
// - Tracks expiry dates
// - Sends notification if document is marked important
// - File storage in FranchiseeDocument location

string IsExpiryValid(FranchiseeDocumentEditModel model);
// - Validates new document expiry against existing documents
// - Prevents upload of documents expiring before current version
```

### Validation Interfaces
```csharp
// FluentValidation Validators
IValidator<FranchiseeEditModel> // FranchiseeEditModelValidator
// - Ensures at least one service is selected

IValidator<RoyaltyFeeSlabsEditModel> // RoyaltyFeeSlabsEditModelValidator
// - MinValue >= 0
// - MinValue < MaxValue
// - ChargePercentage > 0
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Implementation Details -->
## 🔍 Implementation Deep Dive

### Franchisee Hierarchy Management

#### Organization Structure
The system implements a **single-level franchise hierarchy**:
```
Organization (Base)
    ↓
Franchisee (Extends Organization)
    ↓
OrganizationRoleUser (Links Users to Franchisee with Roles)
```

**Key Implementation Details:**
- `FranchiseeFactory.CreateDomain()` calls `OrganizationFactory.CreateDomain()` to create the base Organization first
- `Franchisee.Id` and `Organization.Id` are the same value (1:1 relationship)
- Organization contains: Name, Email, Address collection, Phone collection, About, IsActive
- Franchisee adds: Currency, FeeProfile, LateFee, Services, BusinessId, Contact info

#### Multi-Franchisee Access
Users can be associated with multiple franchisees:
```csharp
// Primary franchisee assignment
OrganizationRoleUser {
    UserId, 
    OrganizationId (= FranchiseeId),
    RoleId,
    IsDefault = true,  // Primary franchisee for user
    IsActive = true
}

// Additional franchisee access
OrganizationRoleUserFranchisee {
    OrganizationRoleUserId,
    FranchiseeId,
    IsDefault,
    IsActive
}
```

**Implemented in:**
- `OrganizationFactory.CreateDomain(long orgId, OrganizationRoleUser orgRoleUser, OrganizationRoleUserFranchisee inDb)`
- `FranchiseeFactory.CreateResignViewModel()` - filters by `orgIdList` for accessible franchisees

### Role-Based Access Control (RBAC)

#### Role Types (from `Core.Users.Enum.RoleType`)
- **SuperAdmin**: Full system access, bypasses franchisee restrictions
- **FrontOfficeExecutive**: HQ staff, can access all franchisees
- **FranchiseeAdmin**: Full access to assigned franchisee(s)
- **Technician**: Limited to assigned jobs/customers
- **SalesPerson**: Limited to sales and customers

#### Access Control Logic (from `FranchiseeFactory.CreateResignViewModel`)
```csharp
// Check if user has access to franchisee
model.IsAccessible = orgIdList.Any(x => x == domain.Id) ? true : false;

// Bypass for SuperAdmin and FrontOfficeExecutive
if (roleId == ((long?)RoleType.SuperAdmin) || roleId == ((long?)RoleType.FrontOfficeExecutive))
{
    model.IsAccessible = true;
}
```

#### Owner Assignment
```csharp
// OrganizationFactory.CreateDomain - Default to FranchiseeAdmin role
return new OrganizationRoleUser
{
    UserId = person.Id,
    RoleId = (long)RoleType.FranchiseeAdmin,  // Default role for franchisee owner
    OrganizationId = organization.Id,
    IsActive = true,
    IsDefault = true  // This is their primary franchisee
};
```

### Royalty Fee Calculation Logic

#### Calculation Strategy Selection
```csharp
// FeeProfile determines calculation method
if (feeProfile.SalesBasedRoyalty)
{
    // Use tiered RoyaltyFeeSlabs
    foreach (var slab in feeProfile.RoyaltyFeeSlabs.OrderBy(x => x.MinValue))
    {
        if (salesAmount >= slab.MinValue && (slab.MaxValue == null || salesAmount <= slab.MaxValue))
        {
            royaltyAmount = salesAmount * (slab.ChargePercentage / 100);
            break;
        }
    }
    
    // Apply minimum royalty floor
    if (royaltyAmount < feeProfile.MinimumRoyaltyPerMonth)
    {
        royaltyAmount = feeProfile.MinimumRoyaltyPerMonth;
    }
}
else
{
    // Use fixed amount
    royaltyAmount = feeProfile.FixedAmount;
}

// Add advertising fund
adFundAmount = salesAmount * (feeProfile.AdFundPercentage / 100);
```

#### Tiered Slab Structure Example
```csharp
// Example RoyaltyFeeSlabs configuration:
[
    { MinValue: 0,      MaxValue: 50000,  ChargePercentage: 7.0 },
    { MinValue: 50001,  MaxValue: 100000, ChargePercentage: 6.0 },
    { MinValue: 100001, MaxValue: null,   ChargePercentage: 5.0 }  // null = infinity
]

// For $75,000 in sales:
// - Falls in second slab (50001-100000)
// - Royalty = $75,000 * 6% = $4,500
// - If MinimumRoyaltyPerMonth = $5,000, actual charge = $5,000
```

#### Minimum Royalty Enforcement by Sales Range
```csharp
// MinRoyaltyFeeSlabs - different minimum based on sales volume
[
    { StartValue: 0,     EndValue: 50000,  MinRoyality: 1000 },
    { StartValue: 50001, EndValue: 100000, MinRoyality: 2000 },
    { StartValue: 100001, EndValue: null,  MinRoyality: 3000 }
]

// Logic (conceptual - actual implementation in billing module):
var minRoyaltySlab = minRoyaltyFeeSlabs.FirstOrDefault(s => 
    salesAmount >= s.StartValue && (s.EndValue == null || salesAmount <= s.EndValue));
    
if (calculatedRoyalty < minRoyaltySlab.MinRoyality)
{
    calculatedRoyalty = minRoyaltySlab.MinRoyality;
}
```

#### Validation Rules
```csharp
// RoyaltyFeeSlabsEditModelValidator
RuleFor(x => x.MinValue)
    .GreaterThanOrEqualTo(0)
    .LessThan(x => x.MaxValue);
    
RuleFor(x => x.MaxValue)
    .GreaterThanOrEqualTo(0);
    
RuleFor(x => x.ChargePercentage)
    .GreaterThan(0);
```

### Service and Pricing Management

#### Service Offerings Configuration
```csharp
// FranchiseeService - What services the franchisee offers
public class FranchiseeService
{
    public long ServiceTypeId { get; set; }  // Restoration, Maintenance, etc.
    public bool IsActive { get; set; }       // Currently offered
    public bool IsCertified { get; set; }    // Franchisee is certified for this service
    public bool CalculateRoyalty { get; set; }  // Include in royalty calculation
}

// FranchiseeServicesFactory.CreateDomainCollection logic:
// - Syncs with master ServiceType list
// - Categories: Restoration, Maintenance, FRONTOFFICECALLMANAGEMENT
// - Only includes services where DashboardServices = true
```

#### Service Fee Types (from `Core.Organizations.Enum.ServiceFeeType`)
- **Bookkeeping**: Fixed monthly accounting fee
- **VarBookkeeping**: Variable bookkeeping (auto-created with Bookkeeping)
- **Website**: Website hosting/maintenance
- **SEO**: Search engine optimization
- **FRANCHISEETECHMAIL**: Technician email service (per-tech pricing)
- **OneTimeProject**: Ad-hoc project charges
- **Loan**: Equipment/technology loans
- **InterestAmount**: Loan interest charges
- **PHONECALLCHARGES**: Call center usage fees

#### Special Service Fee Logic

**Bookkeeping Auto-Expansion:**
```csharp
// FranchiseeServiceFeeFactory.CreateDomain
if (domain.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping)
{
    // Automatically create VarBookkeeping service with same amount
    var VarBookkeeepingService = new FranchiseeServiceFeeEditModel
    {
        Amount = serviceFee.Amount,
        Percentage = serviceFee.Percentage,
        FrequencyId = serviceFee.FrequencyId,
        FranchiseeId = serviceFee.FranchiseeId,
        IsApplicable = true,
        TypeId = (long)ServiceFeeType.VarBookkeeping
    };
    feeInDb.Add(CreateDomainForServiceFee(VarBookkeeepingService));
}
```

**Technician Email Pricing:**
```csharp
// FranchiseeTechMailService
{
    Amount,                      // Base amount
    TechCount,                   // Number of technicians
    MultiplicationFactor,        // Multiplier per tech
    IsGeneric,                   // Flat rate vs. per-tech
    // Actual charge = IsGeneric ? Amount : (Amount * TechCount * MultiplicationFactor)
}
```

#### Loan Management

**Loan Creation:**
```csharp
public FranchiseeLoan CreateFranchiseeLoan(FranchiseeServiceFeeEditModel serviceFee)
{
    return new FranchiseeLoan
    {
        Amount = serviceFee.Amount,                  // Principal
        Duration = serviceFee.Duration.Value,        // Months
        InterestratePerAnum = serviceFee.Percentage.Value,
        IsRoyality = serviceFee.IsRoyality,         // true = adjust royalty, false = adjust adfund
        LoanTypeId = serviceFee.LoanTypeId,         // Equipment, Technology, etc.
        CurrencyExchangeRateId = serviceFee.CurrencyExchangeRateId,
        IsCompleted = false,
        StartDate = serviceFee.StartDate
    };
}
```

**Amortization Schedule:**
```csharp
public FranchiseeLoanSchedule CreateDomain(AmortPaymentSchedule schedule)
{
    return new FranchiseeLoanSchedule
    {
        LoanId = schedule.LoanId,
        LoanTerm = schedule.TermNumber,              // Payment number (1, 2, 3...)
        Balance = Convert.ToDecimal(schedule.Balance),
        DueDate = schedule.Date,
        Interest = Convert.ToDecimal(schedule.Interest),
        Principal = Convert.ToDecimal(schedule.Principal),
        PayableAmount = Convert.ToDecimal(schedule.ScheduledPayment),
        TotalPrincipal = Convert.ToDecimal(schedule.Totalprincipal),
        IsPrePaid = schedule.ScheduledPayment > 0 ? false : true,
        InvoiceItemId = null,                        // Linked when invoice generated
        OverPaidAmount = 0
    };
}
```

**Loan Adjustment Tracking:**
```csharp
// LoanAdjustmentAudit - Track changes between Royalty/Adfund
public LoanAdjustmentAudit CreateDomain(FranchiseeChangeServiceFee franchiseeChange, FranchiseeLoan franchiseeLoan)
{
    return new LoanAdjustmentAudit
    {
        LoanId = franchiseeChange.LoanId,
        BeforeLoanAdjustment = franchiseeLoan.IsRoyality.GetValueOrDefault(),
        AfterLoanAdjustment = franchiseeChange.IsRoyality.GetValueOrDefault(),
        CreatedOn = _clock.ToUtc(DateTime.Now),
        UserId = franchiseeChange.UserId
    };
}
```

### Late Fee Calculation

```csharp
// LateFee structure
public class LateFee
{
    // Royalty Payment Late Fees
    public decimal RoyalityLateFee { get; set; }              // Flat penalty
    public int RoyalityWaitPeriodInDays { get; set; }        // Grace period (e.g., 15 days)
    public decimal RoyalityInterestRatePercentagePerAnnum { get; set; }  // Annual interest rate
    
    // Sales Data Reporting Late Fees
    public decimal SalesDataLateFee { get; set; }             // Penalty for late reporting
    public int SalesDataWaitPeriodInDays { get; set; }       // Grace period (e.g., 7 days)
}

// Calculation logic (conceptual - implemented in billing):
if (DateTime.UtcNow > invoiceDueDate.AddDays(lateFee.RoyalityWaitPeriodInDays))
{
    decimal lateFeeAmount = lateFee.RoyalityLateFee;  // Flat fee
    
    int daysLate = (DateTime.UtcNow - invoiceDueDate.AddDays(lateFee.RoyalityWaitPeriodInDays)).Days;
    decimal interestAmount = outstandingAmount * (lateFee.RoyalityInterestRatePercentagePerAnnum / 100) * (daysLate / 365);
    
    totalLateFee = lateFeeAmount + interestAmount;
}
```

### Document Management

#### Document Types
- **NCA**: Non-Compete Agreement (special handling - only visible to uploader unless Super Admin)
- **Franchise Agreement**
- **Insurance Certificate**
- **Business License**
- **Tax Documents**

#### Document Service Implementation
```csharp
public bool SaveDocuments(FranchiseeDocumentEditModel model)
{
    // Multi-franchisee upload support
    foreach (var id in model.FranchiseeIds.Distinct())
    {
        model.FranchiseeId = id;
        
        // File handling
        if (model.FileModel != null)
        {
            // Move to FranchiseeDocument location
            var destination = MediaLocationHelper.GetFranchiseeDocumentLocation();
            var fileName = _fileService.MoveFile(sourcePath, destination, caption, extension);
            fileId = _fileService.SaveModel(model.FileModel);
        }
        
        // Create document record
        var document = _franchiseeDocumentFactory.CreateDomain(model, fileId);
        _franchiseeDocumentRepository.Save(document);
    }
    
    // Notification for important documents
    if (model.IsImportant)
    {
        _documentNotificationPollingAgent.CreateDocumentUploadNotification(
            model.FileModel.Caption, 
            model.FranchiseeIds, 
            model.DataRecorderMetaData.CreatedBy
        );
    }
}
```

#### Expiry Validation
```csharp
public string IsExpiryValid(FranchiseeDocumentEditModel model)
{
    // Find latest document of same type
    var latestDoc = _franchiseeDocumentRepository.Table
        .Where(x => x.FranchiseeId == franchiseeId && x.DocumentTypeId == model.DocumentTypeId)
        .OrderByDescending(x => x.Id)
        .FirstOrDefault();
    
    // New document must expire after current document
    if (latestDoc != null && latestDoc.ExpiryDate >= model.ExpiryDate)
    {
        return latestDoc.ExpiryDate.Value.ToShortDateString();  // Return error
    }
    
    return "";  // Valid
}
```

### Currency Exchange Support

All financial entities include currency exchange rate tracking:
```csharp
// OneTimeProjectFee
public long CurrencyExchangeRateId { get; set; }
public CurrencyExchangeRate CurrencyExchangeRate { get; set; }

// FranchiseeLoan
public long CurrencyExchangeRateId { get; set; }
public CurrencyExchangeRate CurrencyExchangeRate { get; set; }

// Usage in ViewModel
model.CurrencyRate = loan.CurrencyExchangeRate.Rate;  // e.g., 1.35 for CAD to USD
model.CurrencyCode = franchisee.Currency;  // USD, CAD, etc.
```

### Special Business Rules

#### Hardcoded Franchisee Lists
```csharp
// FranchiseeFactory.CreateResignViewModel
var franchiseeToBeBold = new List<string>() 
{ 
    "MI-Detroit", 
    "PA-Philadelphia", 
    "MI-Grand Rapids", 
    "OH-Cleveland" 
};
model.IsBold = franchiseeToBeBold.Contains(orgRoleUser.Organization.Name);
```

#### "0" Prefix Franchisees
```csharp
// FranchiseeFactory.CreateEditModel
model.Is0Franchisee = domain.Organization.Name.StartsWith("0") ? true : false;
// Used for special testing/inactive franchisees
```

#### Sales Data Upload Enforcement
```csharp
// FranchiseeInfoService.Delete
var salesDataUpload = _salesDataUploadRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.IsActive).ToList();
if (salesDataUpload.Any())
    return false;  // Cannot delete franchisee with active sales data
```

#### Review Push Integration
```csharp
// FranchiseeFactory.CreateEditModel - GetReviewURL
private string GetReviewURL(Franchisee domain)
{
    var reviewPushApiLocation = _reviewPushAPILocationRepository.Table
        .FirstOrDefault(x => x.Id == domain.ReviewpushId);
    return reviewPushApiLocation?.NewRp_ID ?? "";
}
```

#### SEO Charges Tracking
```csharp
// FranchiseeFactory - GetSEOChargesStatus
private int GetSEOChargesStatus(Franchisee domain)
{
    var model = _onetimeprojectfeeAddFundRoyalityRepository.Table
        .FirstOrDefault(x => x.FranchiseeId == domain.Id);
    
    if (model == null)
        return 1;  // Not configured
    else
        return model.IsSEOInRoyalty == true ? 2 : 1;  // 2 = in royalty, 1 = separate
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Module Dependencies
- **[Core.Organizations](../AI-CONTEXT.md)** - Parent module containing interfaces and domain entities
- **[Core.Organizations.Domain](../Domain/AI-CONTEXT.md)** - Domain entity definitions (Franchisee, FeeProfile, etc.)
- **[Core.Organizations.ViewModel](../ViewModel/)** - ViewModels and EditModels for data transfer
- **[Core.Users](../../Users/AI-CONTEXT.md)** - Person, OrganizationRoleUser, RoleType for RBAC
- **[Core.Geo](../../Geo/AI-CONTEXT.md)** - Address, Phone, Location entities
- **[Core.Billing](../../Billing/AI-CONTEXT.md)** - Invoice, InvoiceItem for financial transactions
- **[Core.Sales](../../Sales/AI-CONTEXT.md)** - Customer, MarketingClass for sales data
- **[Core.Scheduler](../../Scheduler/AI-CONTEXT.md)** - Job, Technician for scheduling context
- **[Core.Application](../../Application/AI-CONTEXT.md)** - IRepository, IUnitOfWork, IClock, IFileService

### External Dependencies
- **FluentValidation** - Declarative model validation framework
- **Entity Framework 6** - ORM for data access (via IRepository abstraction)
- **Unity Container** - Dependency injection via `[DefaultImplementation]` attribute

### Factory Dependency Graph
```
FranchiseeFactory
├─> OrganizationFactory (base org data)
├─> FeeProfileFactory (royalty structure)
│   └─> RoyaltyFeeSlabsFactory (tiered rates)
├─> LateFeeFactory (penalties)
├─> FranchiseeServicesFactory (service offerings)
├─> FranchiseeServiceFeeFactory (service pricing)
├─> FranchiseeDocumentFactory (documents)
└─> FranchiseeNotesFactory (notes)
```

### Service Dependency Graph
```
FranchiseeInfoService
├─> FranchiseeFactory
├─> OrganizationFactory
├─> FranchiseeServiceFeeFactory
├─> FranchiseeDocumentFactory
├─> IUserService (user management)
├─> IOrganizationRoleUserInfoService (access control)
├─> IPhoneService (phone management)
└─> IExcelFileCreator (reporting)

FranchiseeServiceFeeService
├─> FranchiseeServiceFeeFactory
├─> IRepository<FranchiseeServiceFee>
├─> IRepository<FranchiseeLoan>
├─> IRepository<OneTimeProjectFee>
└─> LoanCalculationService (external - amortization)

FranchiseeSalesService
├─> FranchiseeSalesFactory
├─> IRepository<FranchiseeSales>
└─> IExcelFileCreator (reporting)
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Gotchas -->
## ⚠️ Gotchas & Edge Cases

### Data Consistency Issues
1. **Bookkeeping Auto-Creation**: When saving a Bookkeeping service fee, a VarBookkeeping fee is auto-created. If you delete Bookkeeping, must manually delete VarBookkeeping or it becomes orphaned.

2. **Royalty Slab Order Matters**: `RoyaltyFeeSlabsFactory.CreateDomainCollection` uses **index-based matching**. If you reorder slabs in the UI, existing DB records are updated in order, which can cause data corruption. Always delete and recreate if reordering.

3. **Organization/Franchisee ID Collision**: `Franchisee.Id` and `Organization.Id` are the same value. If you try to create a Franchisee with an existing Organization.Id, you'll get a primary key violation.

4. **Soft Delete Cascade**: Deleting a Franchisee doesn't cascade delete related entities like FranchiseeSales, Invoices, or Jobs. The Delete method checks for active sales data and refuses deletion.

### Security Concerns
1. **NCA Documents**: Special handling needed - only visible to uploader unless SuperAdmin. This logic is in FranchiseeDocumentService but must be enforced in API layer too.

2. **Multi-Franchisee Access**: Users can have access to multiple franchisees via `OrganizationRoleUserFranchisee`. Always filter by `orgIdList` when displaying franchisee data, except for SuperAdmin/FrontOfficeExecutive.

3. **Role Bypass Logic**: Hardcoded role checks (`RoleType.SuperAdmin`, `RoleType.FrontOfficeExecutive`) scatter the codebase. Changing role definitions breaks these checks.

### Performance Issues
1. **Eager Loading**: `FranchiseeFactory.CreateEditModel` loads full object graph including FeeProfile, RoyaltyFeeSlabs, FranchiseeServices, ServiceFees, Documents. For list views, use `CreateViewModel` instead.

2. **N+1 Queries**: `FranchiseeSalesService.GetSalesData` joins across FranchiseeSales → Customer → Address → City/State/Zip. Without proper EF Include statements, this generates hundreds of queries.

3. **Large Collections**: Franchisees with 100+ documents or 50+ service fees slow down edit model creation. Consider lazy loading or pagination for nested collections.

### Validation Gaps
1. **Royalty Slab Gaps**: No validation for overlapping or missing ranges. If slabs are [0-50K, 100K-200K], sales of $75K will fail to match any slab.

2. **Currency Mismatch**: No validation that FranchiseeServiceFee.CurrencyExchangeRateId matches Franchisee.Currency. Can cause incorrect calculations.

3. **Loan Duration**: No validation that loan duration is reasonable (e.g., 1-360 months). Can accept negative or zero values.

### Business Logic Bugs
1. **MinRoyaltyFeeSlabs EndValue=2900**: Hardcoded magic number in `RoyaltyFeeSlabsFactory.CreateEditModelForMinRoyality`:
   ```csharp
   EndValue = domain.EndValue != 2900 ? domain.EndValue : null
   ```
   Why 2900? Unknown. Likely a bug.

2. **Franchisee Name Parsing**: `FranchiseeSalesService.Get` does customer name matching with space/comma normalization. This breaks for names with hyphens, apostrophes, or special characters.

3. **Duplicate Prevention**: No check for duplicate franchisee names during creation. Can create "NY-Manhattan" twice.

### Migration Service Issues
- `FranchiseeMigrationService` is **not** marked with `[DefaultImplementation]`, so it's not registered in DI container
- Used only for bulk import scenarios
- Missing transaction rollback on user creation failure

<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Testing Notes -->
## 🧪 Testing Considerations

### Critical Test Scenarios

**Royalty Calculation:**
- Test tiered slab transitions (e.g., $49,999 vs. $50,000)
- Test minimum royalty enforcement
- Test null MaxValue (infinity) in last slab
- Test AdFundPercentage calculation

**Loan Amortization:**
- Verify payment schedule calculation
- Test loan adjustment (Royalty ↔ Adfund)
- Test prepayment handling
- Test overpayment allocation

**Multi-Franchisee Access:**
- Test user with multiple franchisees
- Test IsDefault franchisee selection
- Test role-based access (SuperAdmin, FranchiseeAdmin, Technician)

**Document Management:**
- Test multi-franchisee upload
- Test expiry date validation
- Test NCA visibility restrictions
- Test important document notifications

**Service Fee Logic:**
- Test Bookkeeping → VarBookkeeping auto-creation
- Test service fee deactivation
- Test technician email pricing calculation

### Mock Requirements
```csharp
// Key interfaces to mock for unit tests
Mock<IRepository<Franchisee>>
Mock<IRepository<FeeProfile>>
Mock<IRepository<RoyaltyFeeSlabs>>
Mock<IRepository<FranchiseeServiceFee>>
Mock<IOrganizationFactory>
Mock<IFeeProfileFactory>
Mock<IUnitOfWork>
Mock<IClock>
```

<!-- END CUSTOM SECTION -->
