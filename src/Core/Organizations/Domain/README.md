<!-- AUTO-GENERATED: Header -->
# Organizations Domain
> Multi-tenant franchisee management system with hierarchical organization structure, complex royalty fee modeling, and comprehensive financial tracking
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Organizations Domain** is the cornerstone of Marblelife's franchise management system. It models the relationship between corporate headquarters and individual franchisees, managing everything from initial onboarding to ongoing financial obligations.

Think of it as a **franchise-in-a-box**: each franchisee gets their own:
- **Identity** (`Organization` + `Franchisee`)
- **Fee Structure** (`FeeProfile` with progressive `RoyaltyFeeSlabs`)
- **Service Portfolio** (`FranchiseeService` linked to certified `ServiceType`s)
- **Pricing Overrides** (per-franchisee pricing for estimates, maintenance, shift charges)
- **Financial Instruments** (`FranchiseeLoan`, `OneTimeProjectFee`, `FranchiseeAccountCredit`)
- **Staff & Roles** (`OrganizationRoleUser` for technicians, schedulers, accountants)

### Key Concepts

#### Multi-Tenancy by Design
Every franchisee operates in an isolated data silo. The domain enforces this through:
- Scoped queries (all data filtered by `FranchiseeId`)
- Shared reference data (global `ServiceType`, `DocumentType`) with per-franchisee overrides
- Role-based access control via `OrganizationRoleUser`

#### Progressive Royalty Fees
Franchisees pay royalties based on sales volume using **tiered pricing slabs**:
```
Sales $0-$10k    → 8% royalty
Sales $10k-$50k  → 6% royalty
Sales $50k+      → 4% royalty
```
Additionally, a **minimum royalty floor** ensures baseline revenue even in slow months. The system also supports **variable minimums** based on historical performance via `MinRoyaltyFeeSlabs`.

#### Pricing Flexibility
HQ sets corporate pricing, but franchisees can override rates for their market:
- **Estimate Pricing**: `PriceEstimateServices` (linked to `ServicesTag`)
- **Maintenance Costs**: `MaintenanceCharges` (materials, labor)
- **Replacement Costs**: `ReplacementCharges` (tile removal/installation)
- **Shift Premiums**: `ShiftCharges` (day shift, night shift, commercial restoration)
- **Floor Grinding**: `FloorGrindingAdjustment` (equipment-specific pricing)
- **Tax Rates**: `TaxRates` (services vs. products)

When a franchisee overrides pricing, the `IsPriceChangedByFranchisee` flag is set. If their price exceeds corporate rates, an alert flag (`IsFranchiseePriceExceedForEmail`) notifies HQ.

#### Financial Instruments
The domain supports complex financial scenarios:
1. **Loans**: `FranchiseeLoan` with amortized `FranchiseeLoanSchedule` (principal + interest)
2. **One-Time Fees**: `OneTimeProjectFee` for special projects
3. **Recurring Fees**: `FranchiseeServiceFee` (bookkeeping, SEO, payroll, recruiting)
4. **Credits**: `FranchiseeAccountCredit` (refunds, adjustments, overpayments)
5. **Late Fees**: `LateFee` with grace periods and APR interest

#### Audit & History
Every significant change is tracked:
- **Registration History**: `FranchiseeRegistrationHistry`
- **Duration Changes**: `FranchiseeDurationNotesHistry` (contract extensions/reductions)
- **Loan Adjustments**: `LoanAdjustmentAudit` (before/after snapshots)
- **Perpetuity Status**: `Perpetuitydatehistry` (perpetual contracts)
- **General Notes**: `FranchiseeNotes` (timestamped observations)

All audit entities include `DataRecorderMetaData` to capture who made the change and when.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Basic Franchisee Onboarding

```csharp
using Core.Organizations;
using Core.Organizations.Domain;

// Step 1: Create Organization (the legal entity)
var organization = new Organization
{
    Name = "Marblelife Detroit",
    Email = "contact@marblelife-detroit.com",
    TypeId = 1, // Lookup: Franchisee type
    IsActive = true,
    Address = new List<Address>
    {
        new Address
        {
            Street = "123 Main St",
            City = "Detroit",
            StateId = 26, // Michigan
            Zip = "48201"
        }
    },
    Phones = new List<Phone>
    {
        new Phone { Number = "313-555-1234", TypeId = 10 } // Office
    }
};

// Step 2: Create Franchisee (shares PK with Organization)
var franchisee = new Franchisee
{
    // Id will be set to Organization.Id after save
    OwnerName = "John Smith",
    DisplayName = "Detroit",
    Currency = "USD",
    EIN = "12-3456789",
    LegalEntity = "Marblelife Detroit LLC",
    RegistrationNumber = "MI-2024-001",
    RegistrationDate = DateTime.UtcNow,
    Duration = 240, // 20-year contract (months)
    IsActive = true,
    IsRoyality = true, // Subject to royalty fees
    
    // Contact Information
    ContactFirstName = "John",
    ContactLastName = "Smith",
    ContactEmail = "john@marblelife-detroit.com",
    
    AccountPersonFirstName = "Jane",
    AccountPersonLastName = "Doe",
    AccountPersonEmail = "accounting@marblelife-detroit.com",
    
    SchedulerFirstName = "Mike",
    SchedulerLastName = "Johnson",
    SchedulerEmail = "scheduler@marblelife-detroit.com",
    
    // Features
    IsReviewFeedbackEnabled = true,
    SetGeoCode = true,
    IsSEOActive = true
};

organization.Franchisee = franchisee;

// Step 3: Initialize Fee Profile with Royalty Slabs
franchisee.FeeProfile = new FeeProfile
{
    PaymentFrequencyId = 32, // Monthly (from PaymentFrequency enum)
    SalesBasedRoyalty = true,
    MinimumRoyaltyPerMonth = 500.00m,
    AdFundPercentage = 2.0m, // 2% marketing fund
    RoyaltyFeeSlabs = new List<RoyaltyFeeSlabs>
    {
        new RoyaltyFeeSlabs { MinValue = 0, MaxValue = 10000, ChargePercentage = 8.0m },
        new RoyaltyFeeSlabs { MinValue = 10000, MaxValue = 50000, ChargePercentage = 6.0m },
        new RoyaltyFeeSlabs { MinValue = 50000, MaxValue = null, ChargePercentage = 4.0m }
    }
};

// Step 4: Configure Late Fees (defaults are fine, but can customize)
franchisee.LateFee = new LateFee
{
    RoyalityLateFee = 50.00m,
    RoyalityWaitPeriodInDays = 2,
    RoyalityInterestRatePercentagePerAnnum = 18.0m,
    SalesDataLateFee = 50.00m,
    SalesDataWaitPeriodInDays = 1
};

// Step 5: Assign Services (certified offerings)
franchisee.FranchiseeServices = new List<FranchiseeService>
{
    new FranchiseeService
    {
        ServiceTypeId = 1, // Marble Polishing
        CalculateRoyalty = true,
        IsActive = true,
        IsCertified = true
    },
    new FranchiseeService
    {
        ServiceTypeId = 5, // Grout Cleaning
        CalculateRoyalty = true,
        IsActive = true,
        IsCertified = true
    }
};

// Step 6: Add Recurring Service Fees (optional)
franchisee.FranchiseeServiceFee = new List<FranchiseeServiceFee>
{
    new FranchiseeServiceFee
    {
        ServiceFeeTypeId = 172, // Bookkeeping (from ServiceFeeType enum)
        Amount = 150.00m,
        FrequencyId = 32, // Monthly
        IsActive = true
    },
    new FranchiseeServiceFee
    {
        ServiceFeeTypeId = 296, // SEO Charges
        Amount = 500.00m,
        FrequencyId = 32, // Monthly
        IsActive = true
    }
};

// Save via repository
await franchiseeRepository.CreateAsync(organization);
```

### Assigning Users & Roles

```csharp
// Assign John Smith as Owner
var ownerRoleUser = new OrganizationRoleUser
{
    UserId = 123, // John Smith's Person.Id
    RoleId = 5,   // Owner role
    OrganizationId = organization.Id,
    IsDefault = true, // This is John's primary organization
    IsActive = true,
    ColorCode = "#FF5733" // Scheduler calendar color
};

// Assign Mike Johnson as Technician
var techRoleUser = new OrganizationRoleUser
{
    UserId = 456, // Mike's Person.Id
    RoleId = 8,   // Technician role
    OrganizationId = organization.Id,
    IsDefault = false,
    IsActive = true,
    ColorCode = "#3498DB" // Different color for Mike's jobs
};

await organizationRoleUserRepository.CreateAsync(ownerRoleUser);
await organizationRoleUserRepository.CreateAsync(techRoleUser);
```

### Recording Sales Data (for Royalty Calculation)

```csharp
// Sales data typically imported via SalesDataUpload, but can be created manually
var sale = new FranchiseeSales
{
    FranchiseeId = franchisee.Id,
    CustomerId = 789, // From CRM
    InvoiceId = 1001, // Franchisee's invoice to customer
    ClassTypeId = 15, // MarketingClass (Residential Repair, Commercial, etc.)
    SubClassTypeId = 42, // SubClassMarketingClass (optional)
    SalesRep = "Tech Mike Johnson",
    Amount = 2500.00m,
    QbInvoiceNumber = "INV-2024-001",
    CustomerInvoiceId = 1001,
    CustomerInvoiceIdString = "1001",
    DataRecorderMetaDataId = metadataId,
    CurrencyExchangeRateId = 1 // USD = 1.0
};

await franchiseeSalesRepository.CreateAsync(sale);

// This sale will be included in next royalty calculation
```

### Creating a Loan

```csharp
// Franchisee needs equipment loan
var loan = new FranchiseeLoan
{
    FranchiseeId = franchisee.Id,
    Amount = 50000.00m,
    Duration = 60, // 60 months (5 years)
    InterestratePerAnum = 6.5m, // 6.5% APR
    Description = "Equipment Purchase - Floor Grinder & Polisher",
    DateCreated = DateTime.UtcNow,
    StartDate = DateTime.UtcNow.AddDays(30), // First payment in 30 days
    IsRoyality = false, // Not tied to royalty account
    IsCompleted = false,
    CurrencyExchangeRateId = 1,
    LoanTypeId = 180 // Equipment Loan (from LoanType enum)
};

// Generate amortization schedule
var scheduleGenerator = new LoanScheduleGenerator();
loan.FranchiseeLoanSchedule = scheduleGenerator.GenerateSchedule(loan);

await franchiseeLoanRepository.CreateAsync(loan);

// Schedule will have 60 rows, each with DueDate, Principal, Interest, Balance
```

### Overriding Pricing (Franchisee-Specific)

```csharp
// HQ sets default pricing for "Marble Floor Polishing - Residential"
var hqPricing = new PriceEstimateServices
{
    FranchiseeId = null, // NULL = HQ default
    ServiceTagId = 25, // "Marble Floor Polishing"
    CorporatePrice = 4.50m, // per sq ft
    CorporateAdditionalPrice = 3.00m,
    BulkCorporatePrice = 3.75m,
    BulkCorporateAdditionalPrice = 2.50m
};

// Detroit franchisee wants to charge more due to higher local costs
var detroitPricing = new PriceEstimateServices
{
    FranchiseeId = franchisee.Id,
    ServiceTagId = 25,
    FranchiseePrice = 5.25m, // $0.75 more per sq ft
    FranchiseeAdditionalPrice = 3.50m,
    IsPriceChangedByFranchisee = true,
    IsFranchiseePriceExceed = true, // Exceeds corporate rate
    IsFranchiseePriceExceedForEmail = true // Send alert to HQ
};

await priceEstimateRepository.CreateAsync(detroitPricing);
```

### Adding Notes & Documents

```csharp
// Add general note
var note = new FranchiseeNotes
{
    FranchiseeId = franchisee.Id,
    Text = "Owner requested quarterly financial reviews instead of annual.",
    DataRecorderMetaDataId = metadataId // Tracks who/when
};

// Upload insurance certificate
var document = new FranchiseDocument
{
    FranchiseeId = franchisee.Id,
    FileId = 567, // File.Id from file upload
    DocumentTypeId = 10, // Insurance Certificate
    ExpiryDate = DateTime.UtcNow.AddYears(1),
    UploadFor = "General Liability Insurance - $2M coverage",
    IsImportant = true,
    ShowToUser = true, // Franchisee can see this
    IsPerpetuity = false, // Expires in 1 year
    DataRecorderMetaDataId = metadataId
};

await franchiseeNotesRepository.CreateAsync(note);
await franchiseDocumentRepository.CreateAsync(document);
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## 📚 API Summary

### Core Entities

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `Organization` | Legal entity (HQ or franchisee) | → `Franchisee` (1:1), `OrganizationRoleUser` |
| `Franchisee` | Franchisee-specific data | → `Organization` (1:1), `FeeProfile`, `LateFee` |
| `FeeProfile` | Royalty calculation rules | → `Franchisee` (1:1), `RoyaltyFeeSlabs` |
| `RoyaltyFeeSlabs` | Progressive fee tiers | → `FeeProfile` |
| `MinRoyaltyFeeSlabs` | Variable minimum royalty | → `Franchisee` |
| `LateFee` | Late payment penalties | → `Franchisee` (1:1) |
| `OrganizationRoleUser` | User-role-org mapping | → `Organization`, `Person`, `Role` |

### Services & Fees

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `ServiceType` | Available service catalog | Global reference data |
| `FranchiseeService` | Services offered by franchisee | → `Franchisee`, `ServiceType` |
| `FranchiseeServiceFee` | Recurring fees (bookkeeping, SEO) | → `Franchisee`, `Lookup` (ServiceFeeType) |
| `ServicesTag` | Service categorization | → `ServiceType`, `Lookup` |

### Financial

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `FranchiseeSales` | Sales transactions for royalty | → `Franchisee`, `Customer`, `Invoice` |
| `FranchiseeLoan` | Equipment/working capital loans | → `Franchisee`, `FranchiseeLoanSchedule` |
| `FranchiseeLoanSchedule` | Amortization schedule | → `FranchiseeLoan`, `InvoiceItem` |
| `OneTimeProjectFee` | Special project charges | → `Franchisee`, `InvoiceItem` |
| `FranchiseeAccountCredit` | Refunds, credits, overpayments | → `Franchisee`, `Invoice` |

### Pricing Overrides

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `PriceEstimateServices` | Estimate pricing per franchisee | → `Franchisee`, `ServicesTag` |
| `MaintenanceCharges` | Maintenance material costs | → `Franchisee` |
| `ReplacementCharges` | Tile replacement costs | → `Franchisee` |
| `ShiftCharges` | Shift premiums | → `Franchisee` |
| `FloorGrindingAdjustment` | Equipment-specific pricing | → `Franchisee` |
| `TaxRates` | Tax rates per franchisee | → `Franchisee` |

### Documents & Tracking

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `FranchiseDocument` | Uploaded documents | → `Franchisee`, `File`, `DocumentType` |
| `FranchiseeDocumentType` | Required doc types | → `Organization`, `DocumentType` |
| `DocumentType` | Document categories | Global reference data |
| `FranchiseeNotes` | General notes | → `Franchisee`, `DataRecorderMetaData` |

### History & Audit

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `FranchiseeRegistrationHistry` | Registration date changes | → `Franchisee` |
| `FranchiseeDurationNotesHistry` | Contract duration changes | → `Franchisee`, `Person` (requester/approver) |
| `Perpetuitydatehistry` | Perpetual contract status | → `Franchisee` |
| `LoanAdjustmentAudit` | Loan modification history | → `FranchiseeLoan`, `Person` |
| `OnetimeprojectfeeAddFundRoyality` | Fee royalty inclusion flags | → `Organization` |

### Integration

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `ReviewPushAPILocation` | ReviewPush location mapping | Referenced by `Franchisee.ReviewpushId` |
| `ReviewPushCustomerFeedback` | Customer reviews | → `Franchisee`, `Lookup` (AuditAction) |
| `LeadPerformanceFranchiseeDetails` | Lead spend tracking | → `Franchisee`, `Lookup` (Category) |
| `SalesDataMailReminder` | Sales data reminder log | → `Franchisee` |

### Enumerations

| Enum | Values | Purpose |
|------|--------|---------|
| `OrganizationNames` | MIDetroit=62, PAPittsburgh=64, etc. | Hardcoded franchisee IDs |
| `ServiceFeeType` | Loan=171, Bookkeeping=172, SEO=296, etc. | Fee type lookup IDs |
| `PaymentFrequency` | Weekly=31, Monthly=32, TwiceAMonth=33 | Payment schedule lookup IDs |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## 🔧 Troubleshooting

### Common Issues

#### Issue: Royalty Calculation Incorrect
**Symptoms**: Franchisee invoice shows wrong royalty amount.

**Debugging Steps**:
1. Check `FeeProfile.SalesBasedRoyalty` flag (true = slabs, false = fixed)
2. Verify `RoyaltyFeeSlabs` configuration (no gaps in MinValue/MaxValue)
3. Confirm `FranchiseeSales` records for billing period (sum amounts)
4. Check if `MinimumRoyaltyPerMonth` or `MinRoyaltyFeeSlabs` applies
5. Verify `AdFundPercentage` is added

**Common Causes**:
- Missing or overlapping royalty slabs
- Sales data not imported for period
- Wrong `PaymentFrequencyId` (weekly vs. monthly)
- `MinRoyaltyFeeSlabs` overriding expected floor

#### Issue: Franchisee Can't See Pricing Overrides
**Symptoms**: Estimates use HQ pricing instead of franchisee overrides.

**Solution**:
- Check `PriceEstimateServices` has record with `FranchiseeId = X`
- If exists, verify `IsPriceChangedByFranchisee = true`
- If still using HQ pricing, check query logic filters by `FranchiseeId` (not just `ServiceTagId`)

#### Issue: Loan Schedule Missing Payments
**Symptoms**: `FranchiseeLoanSchedule` has fewer than expected rows.

**Debugging**:
1. Check `FranchiseeLoan.Duration` (should equal # of payments)
2. Verify `StartDate` is set (required for schedule generation)
3. Look for `CalculateReschedule = true` rows (indicates manual adjustment needed)
4. Check if `IsCompleted = true` (no new schedules generated)

**Fix**:
```csharp
// Regenerate schedule
var loan = await repository.GetByIdAsync(loanId);
loan.IsCompleted = false;
loan.FranchiseeLoanSchedule.Clear();
loan.FranchiseeLoanSchedule = scheduleGenerator.GenerateSchedule(loan);
await repository.UpdateAsync(loan);
```

#### Issue: OrganizationRoleUser Not Working
**Symptoms**: User can't access franchisee data despite role assignment.

**Checklist**:
- `IsActive = true` on `OrganizationRoleUser`
- `OrganizationId` matches `Franchisee.Id` (not `Franchisee.Organization.Id`)
- User's JWT token includes correct `OrganizationId` claim
- Role has appropriate permissions in `Role` table

#### Issue: Late Fees Not Applied
**Symptoms**: Overdue invoices don't show late charges.

**Debugging**:
1. Check `LateFee` record exists for franchisee
2. Verify grace period: `RoyalityWaitPeriodInDays` or `SalesDataWaitPeriodInDays`
3. Confirm background job is running (late fee application service)
4. Check invoice `DueDate` vs. current date

**Example**:
- DueDate: March 1
- LateFee.RoyalityWaitPeriodInDays: 2
- Late fee applies: March 4 (March 1 + 2 days grace)

### Performance Tips

#### Slow Franchisee Queries
**Problem**: Loading franchisee details takes 5+ seconds.

**Solution**:
```csharp
// BAD: Lazy loading causes N+1 queries
var franchisee = await repository.GetByIdAsync(id);
// Each access to FeeProfile, LateFee, etc. hits DB

// GOOD: Eager load related entities
var franchisee = await context.Franchisees
    .Include(f => f.Organization.Address)
    .Include(f => f.FeeProfile.RoyaltyFeeSlabs)
    .Include(f => f.LateFee)
    .Include(f => f.FranchiseeServices.Select(fs => fs.ServiceType))
    .FirstOrDefaultAsync(f => f.Id == id);
```

#### Sales Data Queries Timeout
**Problem**: Royalty calculation times out due to large `FranchiseeSales` table.

**Solution**:
```csharp
// Always filter by date range and franchisee
var sales = await context.FranchiseeSales
    .Where(s => s.FranchiseeId == franchiseeId)
    .Where(s => s.DataRecorderMetaData.CreatedOn >= startDate)
    .Where(s => s.DataRecorderMetaData.CreatedOn <= endDate)
    .Where(s => !s.IsDeleted) // If soft delete used
    .ToListAsync();

// Add index:
// CREATE INDEX IX_FranchiseeSales_Franchisee_CreatedOn 
// ON FranchiseeSales(FranchiseeId, DataRecorderMetaDataId)
```

### Data Integrity Checks

Run these queries periodically to catch corruption:

```sql
-- Orphaned Franchisees (no Organization)
SELECT * FROM Franchisee 
WHERE Id NOT IN (SELECT Id FROM Organization);

-- Fee Profiles without Slabs
SELECT f.Id, f.DisplayName 
FROM Franchisee f
INNER JOIN FeeProfile fp ON f.Id = fp.Id
WHERE fp.SalesBasedRoyalty = 1 
  AND NOT EXISTS (SELECT 1 FROM RoyaltyFeeSlabs WHERE RoyaltyFeeProfileId = fp.Id);

-- Overlapping Royalty Slabs
SELECT rfs1.RoyaltyFeeProfileId, rfs1.MinValue, rfs1.MaxValue, rfs2.MinValue, rfs2.MaxValue
FROM RoyaltyFeeSlabs rfs1
INNER JOIN RoyaltyFeeSlabs rfs2 ON rfs1.RoyaltyFeeProfileId = rfs2.RoyaltyFeeProfileId
WHERE rfs1.Id <> rfs2.Id
  AND rfs1.MaxValue > rfs2.MinValue 
  AND rfs1.MinValue < rfs2.MaxValue;

-- Loan Schedules with Negative Balances
SELECT * FROM FranchiseeLoanSchedule WHERE Balance < 0;
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Examples -->
## 📖 Real-World Examples

### Example 1: Monthly Royalty Calculation

**Scenario**: Detroit franchisee had $42,000 in sales for March 2024.

```csharp
// Step 1: Load franchisee with fee profile
var franchisee = await context.Franchisees
    .Include(f => f.FeeProfile.RoyaltyFeeSlabs.OrderBy(s => s.MinValue))
    .FirstOrDefaultAsync(f => f.Id == 62); // MIDetroit

// Step 2: Get sales for March
var startDate = new DateTime(2024, 3, 1);
var endDate = new DateTime(2024, 3, 31, 23, 59, 59);
var salesTotal = await context.FranchiseeSales
    .Where(s => s.FranchiseeId == 62)
    .Where(s => s.DataRecorderMetaData.CreatedOn >= startDate)
    .Where(s => s.DataRecorderMetaData.CreatedOn <= endDate)
    .SumAsync(s => s.Amount);

// salesTotal = $42,000

// Step 3: Calculate royalty using slabs
decimal royalty = 0;
foreach (var slab in franchisee.FeeProfile.RoyaltyFeeSlabs)
{
    var slabMin = slab.MinValue ?? 0;
    var slabMax = slab.MaxValue ?? decimal.MaxValue;
    
    if (salesTotal > slabMin)
    {
        var slabSales = Math.Min(salesTotal, slabMax) - slabMin;
        royalty += slabSales * (slab.ChargePercentage / 100);
    }
}

// Slab 1: ($10,000 - $0) * 8% = $800
// Slab 2: ($42,000 - $10,000) * 6% = $1,920
// Total royalty = $2,720

// Step 4: Apply minimum royalty
royalty = Math.Max(royalty, franchisee.FeeProfile.MinimumRoyaltyPerMonth);
// $2,720 > $500 → Use calculated royalty

// Step 5: Add ad fund
var adFund = salesTotal * (franchisee.FeeProfile.AdFundPercentage / 100);
// $42,000 * 2% = $840

// Step 6: Total invoice amount
var totalDue = royalty + adFund;
// $2,720 + $840 = $3,560
```

### Example 2: Equipment Loan Amortization

**Scenario**: Franchisee borrows $50,000 at 6.5% APR for 60 months.

```csharp
public class LoanScheduleGenerator
{
    public List<FranchiseeLoanSchedule> GenerateSchedule(FranchiseeLoan loan)
    {
        var schedule = new List<FranchiseeLoanSchedule>();
        var monthlyRate = loan.InterestratePerAnum / 12 / 100; // 6.5% / 12 = 0.00542
        var numPayments = loan.Duration;
        
        // Monthly payment formula: P * [r(1+r)^n] / [(1+r)^n - 1]
        var monthlyPayment = loan.Amount * 
            (monthlyRate * Math.Pow(1 + monthlyRate, numPayments)) /
            (Math.Pow(1 + monthlyRate, numPayments) - 1);
        // monthlyPayment ≈ $976.98
        
        var balance = loan.Amount;
        var dueDate = loan.StartDate ?? loan.DateCreated;
        
        for (int i = 1; i <= numPayments; i++)
        {
            var interest = balance * monthlyRate;
            var principal = monthlyPayment - interest;
            balance -= principal;
            
            schedule.Add(new FranchiseeLoanSchedule
            {
                LoanTerm = i,
                DueDate = dueDate.AddMonths(i),
                Interest = Math.Round(interest, 2),
                Principal = Math.Round(principal, 2),
                PayableAmount = Math.Round(monthlyPayment, 2),
                Balance = Math.Round(balance, 2),
                TotalPrincipal = Math.Round(loan.Amount - balance, 2),
                IsRoyality = loan.IsRoyality ?? false
            });
        }
        
        return schedule;
    }
}

// Example output for first 3 months:
// Month 1: Interest=$270.83, Principal=$706.15, Balance=$49,293.85
// Month 2: Interest=$267.00, Principal=$709.98, Balance=$48,583.87
// Month 3: Interest=$263.16, Principal=$713.82, Balance=$47,870.05
```

### Example 3: Franchisee-Specific Pricing Query

**Scenario**: Get pricing for "Marble Floor Polishing" for Detroit franchisee.

```csharp
public async Task<PriceEstimateServices> GetFranchiseePricing(
    long franchiseeId, 
    long serviceTagId)
{
    // Step 1: Try franchisee-specific pricing
    var franchiseePricing = await context.PriceEstimateServices
        .FirstOrDefaultAsync(p => 
            p.FranchiseeId == franchiseeId && 
            p.ServiceTagId == serviceTagId);
    
    if (franchiseePricing != null)
    {
        return franchiseePricing;
    }
    
    // Step 2: Fall back to HQ default pricing
    var hqPricing = await context.PriceEstimateServices
        .FirstOrDefaultAsync(p => 
            p.FranchiseeId == null && 
            p.ServiceTagId == serviceTagId);
    
    return hqPricing;
}

// Usage:
var pricing = await GetFranchiseePricing(62, 25);
var pricePerSqFt = pricing.FranchiseePrice ?? pricing.CorporatePrice;
// Use FranchiseePrice if set, otherwise CorporatePrice
```
<!-- END CUSTOM SECTION -->
