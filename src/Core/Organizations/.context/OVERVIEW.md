<!-- AUTO-GENERATED: Header -->
# Organizations Module
> Central hub for franchisee lifecycle management, billing configuration, document workflows, and service fee tracking in the MarbleLife franchise system
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Organizations module is the backbone of the MarbleLife franchise management system. Think of it as the "franchisee headquarters" where everything about franchise operations is managed—from initial onboarding to daily financial tracking.

**Why it exists**: Franchise systems require complex management of multiple business entities, each with unique billing structures, service offerings, and document requirements. This module centralizes all franchisee data while maintaining the flexibility to handle diverse fee structures (tiered royalties, fixed fees, minimum guarantees) and business models (different service types, payment frequencies, special charges).

**Key capabilities**:
- **Franchisee Management**: Complete CRUD operations with organization metadata, contact management, and activation/deactivation workflows
- **Dynamic Fee Structures**: Support for sales-based tiered royalties OR fixed fees, with minimum guarantees and ad fund contributions
- **Service Fee Tracking**: Recurring and one-time charges (loans, bookkeeping, SEO, phone services, etc.) with status management
- **Document Management**: Upload, categorize, and report on franchisee documents (contracts, tax forms, compliance records)
- **Sales Integration**: Process sales data to calculate royalties based on configured fee profiles
- **Reporting**: Excel exports for franchisee directories, financial reports, tax documents
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating a New Franchisee

```csharp
// 1. Build the edit model
var editModel = new FranchiseeEditModel
{
    // Organization basics
    Name = "MarbleLife - Dallas",
    Email = "dallas@marblelife.com",
    TypeId = organizationTypeId,
    
    // Franchisee specifics
    BusinessId = 12345,
    QuickBookIdentifier = "ML-DAL-001",
    DisplayName = "Dallas Franchise",
    Currency = "USD",
    EIN = "12-3456789",
    LegalEntity = "Dallas Stone Care LLC",
    
    // Contacts
    ContactFirstName = "John",
    ContactLastName = "Smith",
    ContactEmail = "john@dallasstone.com",
    
    // Fee Profile
    FeeProfile = new FeeProfileEditModel
    {
        SalesBasedRoyalty = true,
        MinimumRoyaltyPerMonth = 1000,
        AdFundPercentage = 2.5m,
        PaymentFrequencyId = PaymentFrequency.Monthly,
        RoyaltyFeeSlabs = new List<RoyaltyFeeSlabsEditModel>
        {
            new RoyaltyFeeSlabsEditModel { MinSalesAmount = 0, MaxSalesAmount = 50000, RoyaltyPercentage = 5.0m },
            new RoyaltyFeeSlabsEditModel { MinSalesAmount = 50001, MaxSalesAmount = 100000, RoyaltyPercentage = 6.0m },
            new RoyaltyFeeSlabsEditModel { MinSalesAmount = 100001, MaxSalesAmount = null, RoyaltyPercentage = 7.0m }
        }
    },
    
    // Services offered
    FranchiseeServices = new List<FranchiseeServiceEditModel>
    {
        new FranchiseeServiceEditModel { ServiceTypeId = ServiceType.StoneLife, IsActive = true },
        new FranchiseeServiceEditModel { ServiceTypeId = ServiceType.Counterlife, IsActive = true }
    }
};

// 2. Save via service
var franchiseeService = container.Resolve<IFranchiseeInfoService>();
franchiseeService.Save(editModel);  // Validates, creates Organization + Franchisee + User
```

### Calculating Royalties from Sales

```csharp
// Example: $75,000 in sales with tiered fee structure
var feeProfile = franchiseeService.GetFranchiseeFeeProfile(franchiseeId);

// Sales breakdown:
// Tier 1: $0-$50,000 @ 5% = $2,500
// Tier 2: $50,001-$75,000 @ 6% = $1,500
// Total Royalty: $4,000

// Since $4,000 > MinimumRoyaltyPerMonth ($1,000), charge $4,000
// Ad Fund: $75,000 * 2.5% = $1,875
// Total Fees: $5,875
```

### Querying Franchisees

```csharp
var service = container.Resolve<IFranchiseeInfoService>();

// Get paginated, filtered list
var result = service.GetFranchiseeCollection(
    new FranchiseeListFilter
    {
        SearchText = "Dallas",
        IsActive = true,
        StateId = texasStateId
    },
    pageNumber: 1,
    pageSize: 25
);

Console.WriteLine($"Found {result.TotalCount} franchisees");
foreach (var franchisee in result.Franchisees)
{
    Console.WriteLine($"{franchisee.DisplayName} - {franchisee.Email}");
}
```

### Exporting Data

```csharp
var filter = new FranchiseeListFilter { IsActive = true };
string fileName;

if (service.DownloadFranchisee(filter, out fileName))
{
    // Excel file generated at fileName path
    SendFileToUser(fileName);
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Core Services

| Service | Key Methods | Purpose |
|---------|-------------|---------|
| **IFranchiseeInfoService** | Get, Save, Delete, GetFranchiseeCollection | Main CRUD operations |
|  | DeactivateFranchisee, ActivateFranchisee | Lifecycle management |
|  | DownloadFranchisee, DownloadFranchiseeDirectory | Excel exports |
|  | GetFranchiseeFeeProfile | Retrieve billing configuration |
| **IFranchiseeSalesService** | ProcessSalesData, GetSalesReport | Sales data import and reporting |
| **IFranchiseeServiceFeeService** | GetServiceFeeCollection, OneTimeProjectChangeStatus | Service charge management |
| **IFranchiseeDocumentService** | UploadDocument, GetDocumentReport | Document lifecycle |
| **IOrganizationRoleUserService** | AssignUserToOrganization, GetUserRoles | User-organization relationships |

### Factory Pattern

| Factory | Purpose |
|---------|---------|
| **IFranchiseeFactory** | Transform between Franchisee domain ↔ FranchiseeEditModel/ViewModel |
| **IOrganizationFactory** | Transform between Organization domain ↔ OrganizationEditModel/ViewModel |
| **IFeeProfileFactory** | Transform between FeeProfile domain ↔ FeeProfileEditModel/ViewModel |
| **IFranchiseeServiceFeeFactory** | Transform service fee entities for different contexts |
| **IFranchiseeDocumentFactory** | Transform document entities with metadata |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Module Structure -->
## Module Organization

```
Organizations/
├── .context/                          # This documentation
├── [Root Level] (24 files)            # Interfaces + 1 service implementation
│   ├── IFranchiseeInfoService.cs      # Main franchisee service contract
│   ├── IFranchiseeFactory.cs          # Franchisee transformation contract
│   ├── IOrganizationFactory.cs        # Organization transformation contract
│   ├── I*Factory.cs                   # 9 other factory interfaces
│   ├── I*Service.cs                   # 7 other service interfaces
│   └── OrganizationRoleUserInfoService.cs
│
├── Domain/ (36 files)                 # Entity definitions
│   ├── Franchisee.cs                  # Main franchisee entity (118 lines)
│   ├── organization.cs                # Base organization entity
│   ├── FeeProfile.cs                  # Billing configuration
│   ├── RoyaltyFeeSlabs.cs             # Tiered royalty rates
│   ├── FranchiseeSales.cs             # Sales records
│   ├── FranchiseeServiceFee.cs        # Service charges
│   ├── FranchiseeNotes.cs             # Internal notes
│   └── [30 other domain entities]
│
├── Enum/ (13 files)                   # Type-safe constants
│   ├── ServiceType.cs                 # 38 service categories
│   ├── ServiceFeeType.cs              # 7 fee types (Loan, Bookkeeping, SEO, etc.)
│   ├── DocumentType.cs                # Document categories
│   ├── PaymentFrequency.cs            # Billing cycles
│   └── [9 other enums]
│
├── Impl/ (24 files)                   # Concrete implementations
│   ├── FranchiseeInfoService.cs       # 2700+ lines - main business logic
│   ├── FranchiseeServiceFeeService.cs # 1900+ lines - fee calculations
│   ├── FranchiseeFactory.cs           # Domain transformations
│   ├── *Validator.cs (3 files)        # FluentValidation validators
│   └── [20 other implementations]
│
└── ViewModel/ (58 files)              # DTOs for API/UI
    ├── FranchiseeEditModel.cs         # Full franchisee edit form
    ├── FranchiseeViewModel.cs         # Display data
    ├── FranchiseeListModel.cs         # Paginated list response
    ├── FranchiseeListFilter.cs        # Query parameters
    ├── FeeProfileEditModel.cs         # Fee configuration form
    └── [53 other DTOs]
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Issue**: "BusinessId already exists"
- **Cause**: BusinessId must be unique across all franchisees
- **Solution**: Use `IsUniqueBusinessId()` to validate before saving

**Issue**: Franchisee has zero royalties despite sales
- **Cause**: FeeProfile.SalesBasedRoyalty might be false, or RoyaltyFeeSlabs not configured
- **Solution**: Check FeeProfile settings; ensure slabs cover full sales range (0 to null)

**Issue**: Cannot deactivate franchisee
- **Cause**: Active invoices or open sales records exist
- **Solution**: Close all open financial records before deactivation

**Issue**: Excel export times out
- **Cause**: Large dataset (1000+ franchisees)
- **Solution**: Apply filters to reduce result set, or use async processing

### Development Tips

1. **Testing Fee Calculations**: Use FeeProfileFactory to create test scenarios with different slab configurations
2. **Cascade Operations**: Pay attention to [CascadeEntity] attributes—saves and deletes cascade automatically
3. **Edit Model Validation**: Always run validators before calling Save(); they enforce complex business rules
4. **Performance**: FranchiseeInfoService.Get() loads many relationships; profile queries and use projections for list views
<!-- END CUSTOM SECTION -->
