# Organizations Implementation Module

> **Concrete implementations for franchisee management, royalty calculations, service pricing, and role-based access control.**

## Overview

This module contains all the business logic implementations for the Marblelife franchise management system. It orchestrates complex workflows between franchisees, fee structures, services, loans, and documents. Think of this as the "engine room" where all the organizational business rules come to life.

The implementation follows a clear **Factory + Service** pattern:
- **Factories** handle data transformation between Domain ↔ EditModel ↔ ViewModel
- **Services** orchestrate multi-step workflows and transaction boundaries

## Key Components

### 🏭 Factories (Data Transformation)

| Factory | Purpose |
|---------|---------|
| **FranchiseeFactory** | Main orchestrator - constructs full Franchisee aggregate with all nested entities (FeeProfile, Services, Documents) |
| **OrganizationFactory** | Base organizational data (Name, Address, Phones) + OrganizationRoleUser management |
| **FeeProfileFactory** | Royalty calculation configuration with tiered RoyaltyFeeSlabs |
| **RoyaltyFeeSlabsFactory** | Manages tiered percentage structure (e.g., 0-50K @ 7%, 50K-100K @ 6%) |
| **LateFeeFactory** | Late payment penalty configuration (flat fee + interest rate) |
| **FranchiseeServiceFeeFactory** | Service-specific pricing (Bookkeeping, Website, SEO, Loans) |
| **FranchiseeServicesFactory** | Service offerings configuration (Restoration, Maintenance, etc.) |
| **FranchiseeDocumentFactory** | Document metadata (Franchise Agreement, NCA, Insurance, etc.) |

### 🎯 Services (Business Orchestration)

| Service | Purpose |
|---------|---------|
| **FranchiseeInfoService** | Main CRUD service for franchisee lifecycle management (1,580 LOC - largest service) |
| **FranchiseeServiceFeeService** | Complex service fee and loan management with amortization (1,331 LOC) |
| **FranchiseeSalesService** | Sales data tracking and reporting for royalty calculation basis |
| **FranchiseeDocumentService** | Document upload, categorization, expiry tracking, and notifications |
| **LeadPerformanceFranchiseeDetailsService** | Lead tracking and performance metrics |
| **FranchiseeTechnicianMailService** | Technician email service configuration |

### ✅ Validators (FluentValidation)

| Validator | Rules |
|-----------|-------|
| **FranchiseeEditModelValidator** | Ensures at least one service is selected |
| **RoyaltyFeeSlabsEditModelValidator** | MinValue < MaxValue, ChargePercentage > 0 |
| **FeeProfileEditModelValidator** | Payment frequency and royalty configuration validation |

## 🚀 Quick Start Guide

### Creating a Franchisee

```csharp
// Inject dependencies
var franchiseeService = container.Resolve<IFranchiseeInfoService>();

// Prepare edit model
var model = new FranchiseeEditModel
{
    Name = "NY-Manhattan",
    Email = "manhattan@marblelife.com",
    Currency = "USD",
    Address = new AddressEditModel { /* ... */ },
    PhoneNumbers = new List<PhoneEditModel> { /* ... */ },
    
    // Royalty Configuration
    FeeProfile = new FeeProfileEditModel
    {
        SalesBasedRoyalty = true,
        MinimumRoyaltyPerMonth = 1000,
        AdFundPercentage = 2.0,
        Slabs = new List<RoyaltyFeeSlabsEditModel>
        {
            new() { MinValue = 0, MaxValue = 50000, ChargePercentage = 7.0 },
            new() { MinValue = 50001, MaxValue = 100000, ChargePercentage = 6.0 },
            new() { MinValue = 100001, MaxValue = null, ChargePercentage = 5.0 }
        }
    },
    
    // Late Fee Configuration
    LateFee = new LateFeeEditModel
    {
        RoyalityLateFee = 50,
        RoyalityWaitPeriodInDays = 15,
        RoyalityInterestRate = 18.0,
        SalesDataLateFee = 25,
        SalesDataWaitPeriodInDays = 7
    },
    
    // Service Offerings
    FranchiseeServices = new List<FranchiseeServiceEditModel>
    {
        new() { ServiceTypeId = 1, IsActive = true, IsCertified = true, CalculateRoyalty = true }
    }
};

// Save
var franchisee = franchiseeService.Save(model);
```

### Understanding Royalty Calculation

```csharp
// Example: $75,000 in monthly sales

// 1. Find applicable slab
var slab = feeProfile.RoyaltyFeeSlabs
    .FirstOrDefault(s => salesAmount >= s.MinValue && 
                        (s.MaxValue == null || salesAmount <= s.MaxValue));
// Result: { MinValue: 50001, MaxValue: 100000, ChargePercentage: 6.0 }

// 2. Calculate royalty
decimal royalty = salesAmount * (slab.ChargePercentage / 100);
// $75,000 * 6% = $4,500

// 3. Apply minimum floor
if (royalty < feeProfile.MinimumRoyaltyPerMonth)
    royalty = feeProfile.MinimumRoyaltyPerMonth;
// If minimum is $5,000, charge $5,000

// 4. Add advertising fund
decimal adFund = salesAmount * (feeProfile.AdFundPercentage / 100);
// $75,000 * 2% = $1,500

// Total invoice: $5,000 (royalty) + $1,500 (ad fund) = $6,500
```

### Managing Service Fees

```csharp
var serviceFeeService = container.Resolve<IFranchiseeServiceFeeService>();

// Configure monthly fees
var serviceFees = new List<FranchiseeServiceFeeEditModel>
{
    new() 
    { 
        TypeId = (long)ServiceFeeType.Bookkeeping,
        Amount = 200,
        FrequencyId = (long)Frequency.Monthly,
        IsApplicable = true
        // Note: This automatically creates a VarBookkeeping fee as well!
    },
    new() 
    { 
        TypeId = (long)ServiceFeeType.Website,
        Amount = 50,
        FrequencyId = (long)Frequency.Monthly,
        IsApplicable = true
    }
};

serviceFeeService.SaveServiceFee(serviceFees, franchiseeId);
```

### Creating a Loan

```csharp
var loanModel = new FranchiseeServiceFeeEditModel
{
    Amount = 10000,              // $10,000 principal
    Duration = 24,               // 24 months
    Percentage = 6.5,            // 6.5% APR
    StartDate = DateTime.Today,
    IsRoyality = true,           // Adjust royalty (not adfund)
    LoanTypeId = 1,              // Equipment loan
    CurrencyExchangeRateId = 1
};

var loan = serviceFeeService.SaveLoan(loanModel, franchiseeId);
// This creates a FranchiseeLoan with 24 FranchiseeLoanSchedule records (amortization table)
```

### Role-Based Access Example

```csharp
// Check if user can access franchisee
var userOrgIds = organizationRoleUserService.GetAccessibleFranchiseeIds(userId);
var userRole = userService.GetUserRole(userId);

if (userRole == RoleType.SuperAdmin || userRole == RoleType.FrontOfficeExecutive)
{
    // Full access to all franchisees
    allFranchisees = franchiseeService.GetFranchisees(filter, pageNumber, pageSize);
}
else
{
    // Filtered to accessible franchisees only
    filter.FranchiseeIds = userOrgIds;
    accessibleFranchisees = franchiseeService.GetFranchisees(filter, pageNumber, pageSize);
}
```

## 📐 Architecture Patterns

### Factory Pattern Usage
Factories handle bidirectional transformations:
```
Domain Entity ─────────────► ViewModel (for display)
     ▲                            │
     │                            │
     │                            ▼
     └────────────────────── EditModel (for CRUD)
```

**Example Flow:**
```csharp
// 1. Load for editing (Domain → EditModel)
var domain = repository.Get(franchiseeId);
var editModel = franchiseeFactory.CreateEditModel(domain, personDomain);

// 2. User modifies editModel in UI

// 3. Save changes (EditModel → Domain)
var updatedDomain = franchiseeFactory.CreateDomain(editModel, domain);
repository.Save(updatedDomain);

// 4. Display confirmation (Domain → ViewModel)
var viewModel = franchiseeFactory.CreateViewModel(updatedDomain);
```

### Service Layer Responsibilities
Services manage:
- **Transaction Boundaries**: Using `IUnitOfWork.StartTransaction()`
- **Validation**: Calling FluentValidation validators
- **Cross-Entity Logic**: Coordinating between multiple repositories
- **Business Rules**: Enforcing constraints (e.g., can't delete with active sales data)
- **Audit Trail**: Recording who/when for changes

### Repository Abstraction
All data access goes through `IRepository<T>`:
```csharp
private readonly IRepository<Franchisee> _franchiseeRepository;
private readonly IRepository<FeeProfile> _feeProfileRepository;

// No direct Entity Framework usage in this layer
// Keeps implementation independent of ORM
```

## 🔍 Important Implementation Details

### Hierarchical Relationships

```
Organization (Base: Name, Email, Address, Phones)
    ↓ 1:1
Franchisee (Adds: Currency, FeeProfile, Services, etc.)
    ↓ 1:N
OrganizationRoleUser (User Assignment with Role)
    ↓ 0:N
OrganizationRoleUserFranchisee (Additional Franchisee Access)
```

### Special Business Rules

1. **"0" Prefix Franchisees**: Franchisees starting with "0" are marked as test/inactive
2. **Hardcoded Bold List**: MI-Detroit, PA-Philadelphia, MI-Grand Rapids, OH-Cleveland display in bold
3. **NCA Document Restriction**: Non-Compete Agreements only visible to uploader (unless SuperAdmin)
4. **Bookkeeping Auto-Expansion**: Bookkeeping service fee auto-creates VarBookkeeping fee
5. **Delete Protection**: Cannot delete franchisee with active sales data uploads

### Currency Handling

All financial entities track exchange rates at creation time:
```csharp
// Loan created with CAD to USD rate of 1.35
loan.CurrencyExchangeRateId = 5;
loan.CurrencyExchangeRate.Rate = 1.35m;

// Later, when displaying in USD:
amountInUSD = loan.Amount * loan.CurrencyExchangeRate.Rate;
```

## 📊 Key Workflows

### Franchisee Creation Workflow
```
1. User submits FranchiseeEditModel
2. FranchiseeEditModelValidator validates
3. FranchiseeInfoService.Save() starts transaction
4. FranchiseeFactory.CreateOrgDomain() → Organization
5. FranchiseeFactory.CreateDomain() → Franchisee
   ├─> FeeProfileFactory → FeeProfile + RoyaltyFeeSlabs
   ├─> LateFeeFactory → LateFee
   ├─> FranchiseeServicesFactory → FranchiseeServices[]
   └─> FranchiseeServiceFeeFactory → FranchiseeServiceFee[]
6. Repository.Save() persists all
7. OrganizationFactory.CreateDomain() → OrganizationRoleUser (owner)
8. Commit transaction
9. Return Franchisee with IDs
```

### Monthly Royalty Calculation Workflow
```
1. Sales data uploaded via FranchiseeSalesService
2. Billing job queries FeeProfile for franchisee
3. Sum monthly sales from FranchiseeSales
4. Match sales amount to RoyaltyFeeSlabs range
5. Calculate: sales × ChargePercentage
6. Apply MinimumRoyaltyPerMonth floor
7. Calculate AdFund: sales × AdFundPercentage
8. Generate InvoiceItems for royalty + adfund
9. Check LateFee configuration for penalties
10. Generate Invoice
```

### Document Upload Workflow
```
1. User selects document, type, expiry date, franchisee(s)
2. FranchiseeDocumentService.IsExpiryValid() validates
3. If multi-franchisee, loop through FranchiseeIds
4. FileService.MoveFile() to FranchiseeDocument location
5. FranchiseeDocumentFactory.CreateDomain() → FranchiseDocument
6. Repository.Save() for each franchisee
7. If IsImportant, send notification to franchisee users
```

## 📝 Configuration Examples

### Tiered Royalty Structure
```json
{
  "SalesBasedRoyalty": true,
  "MinimumRoyaltyPerMonth": 1500,
  "AdFundPercentage": 2.5,
  "RoyaltyFeeSlabs": [
    { "MinValue": 0, "MaxValue": 25000, "ChargePercentage": 8.0 },
    { "MinValue": 25001, "MaxValue": 75000, "ChargePercentage": 7.0 },
    { "MinValue": 75001, "MaxValue": 150000, "ChargePercentage": 6.0 },
    { "MinValue": 150001, "MaxValue": null, "ChargePercentage": 5.0 }
  ]
}
```

### Late Fee Structure
```json
{
  "RoyalityLateFee": 75.00,
  "RoyalityWaitPeriodInDays": 15,
  "RoyalityInterestRate": 18.0,
  "SalesDataLateFee": 50.00,
  "SalesDataWaitPeriodInDays": 10
}
```

### Service Fee Configuration
```json
{
  "ServiceFees": [
    {
      "TypeId": 1, // Bookkeeping
      "Amount": 250,
      "FrequencyId": 2, // Monthly
      "IsApplicable": true
    },
    {
      "TypeId": 3, // Website
      "Amount": 75,
      "FrequencyId": 2, // Monthly
      "IsApplicable": true
    },
    {
      "TypeId": 5, // SEO
      "Amount": 500,
      "FrequencyId": 2, // Monthly
      "IsApplicable": true
    }
  ]
}
```

## ⚠️ Common Pitfalls

### 1. Royalty Slab Gaps
**Problem**: If slabs have gaps (e.g., 0-50K, 100K-200K), sales of $75K won't match any slab.

**Solution**: Always ensure continuous ranges:
```csharp
[0 - 50,000], [50,001 - 100,000], [100,001 - null]
```

### 2. Bookkeeping Fee Deletion
**Problem**: Deleting Bookkeeping service leaves orphaned VarBookkeeping fee.

**Solution**: Always delete VarBookkeeping when deleting Bookkeeping:
```csharp
var bookkeepingFee = fees.FirstOrDefault(f => f.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping);
var varBookkeepingFee = fees.FirstOrDefault(f => f.ServiceFeeTypeId == (long)ServiceFeeType.VarBookkeeping);
if (bookkeepingFee != null && varBookkeepingFee != null)
{
    // Delete both
}
```

### 3. Role Check Hardcoding
**Problem**: Role checks are hardcoded throughout:
```csharp
if (roleId == (long)RoleType.SuperAdmin || roleId == (long)RoleType.FrontOfficeExecutive)
```

**Solution**: Centralize role permission checking or use claims-based authorization.

### 4. Currency Exchange Rate Timing
**Problem**: Loans and fees are created with current exchange rate, but rate changes over time.

**Solution**: Exchange rate is captured at creation and never changes for that entity:
```csharp
loan.CurrencyExchangeRateId = currentRateId;  // Fixed for life of loan
```

### 5. Document Expiry Validation
**Problem**: System validates new document expiry must be > current document expiry, but what if you need to correct a mistake?

**Solution**: Delete incorrect document first, then upload corrected version.

## 🔧 Troubleshooting

### "Cannot delete franchisee"
**Cause**: Active sales data exists.
**Check**: 
```sql
SELECT * FROM SalesDataUpload WHERE FranchiseeId = @id AND IsActive = 1
```
**Fix**: Deactivate sales data or cascade delete if appropriate.

### "Royalty calculation is $0"
**Cause**: Sales amount doesn't match any RoyaltyFeeSlabs range.
**Check**: 
```csharp
var matchingSlab = feeProfile.RoyaltyFeeSlabs
    .FirstOrDefault(s => salesAmount >= s.MinValue && 
                        (s.MaxValue == null || salesAmount <= s.MaxValue));
if (matchingSlab == null) // No match!
```
**Fix**: Ensure continuous slab ranges with null MaxValue for last slab.

### "User can't see franchisee"
**Cause**: Missing OrganizationRoleUserFranchisee record or incorrect IsActive flag.
**Check**:
```sql
SELECT * FROM OrganizationRoleUserFranchisee 
WHERE OrganizationRoleUserId = @oruId AND FranchiseeId = @franchiseeId AND IsActive = 1
```
**Fix**: Add missing record or activate existing.

### "Loan schedule incorrect"
**Cause**: Amortization calculation error or currency exchange rate issue.
**Check**: Verify `InterestratePerAnum` is annual rate (not monthly) and `Duration` is in months.
**Fix**: Recalculate schedule using financial library or delete and recreate loan.

## 📚 Related Documentation

- **[Parent Module](../AI-CONTEXT.md)** - Organizations module overview
- **[Domain Entities](../Domain/AI-CONTEXT.md)** - Entity definitions and relationships
- **[Core.Billing](../../Billing/AI-CONTEXT.md)** - Invoice generation and payment processing
- **[Core.Users](../../Users/AI-CONTEXT.md)** - User management and authentication
- **[Deployment Guide](/docs/deploy.md)** - How to deploy changes to this module

## 🤝 Contributing

When modifying implementations:
1. **Update both Factory AND Service** if changing data structure
2. **Add validator rules** for new required fields
3. **Update all three transformations**: Domain → EditModel → ViewModel → Domain
4. **Test royalty calculations** thoroughly with edge cases
5. **Check role-based access** for new features
6. **Update integration tests** in corresponding test project

---

**Module Statistics:**
- **24 Implementation Files** (6,553 total LOC)
- **13 Factories** for data transformation
- **6 Services** for business orchestration
- **3 Validators** for model validation
- **Largest Service**: FranchiseeInfoService (1,580 LOC)
- **Most Complex**: FranchiseeServiceFeeService (1,331 LOC)
