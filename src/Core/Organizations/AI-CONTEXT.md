# Core/Organizations - AI Context

## Purpose

The **Organizations** module manages franchisee operations, organizational hierarchies, fee structures, and franchisee-related services. This is a critical domain for the MarbleLife platform's franchise management system.

## Key Entities (Domain/)

### Primary Entities
- **Organization**: Company/franchisee organization details
- **Franchisee**: Franchisee-specific information and settings
- **FranchiseeServices**: Services offered by each franchisee
- **FranchiseeServiceFee**: Fee structure for services
- **RoyaltyFeeSlabs**: Tiered royalty fee configuration
- **LateFee**: Late payment fee rules
- **FeeProfile**: Fee calculation profiles
- **FranchiseeDocument**: Document management for franchisees
- **FranchiseeNotes**: Internal notes about franchisees
- **FranchiseeSales**: Sales performance tracking
- **FranchiseeAccountCredit**: Credit/balance management
- **FranchiseeTechnicianMail**: Technician communication records
- **OrganizationRoleUser**: User-role-organization assignments

## Service Interfaces

### Core Services
- **IOrganizationFactory**: Organization entity creation
- **IFranchiseeFactory**: Franchisee entity creation with validation
- **IFranchiseeInfoService**: Read-only franchisee information queries
- **IFranchiseeSalesService**: Sales data and performance metrics
- **IOrganizationRoleUserService**: User-organization-role management
- **ILeadPerformanceFranchiseeDetailsService**: Lead conversion analytics

### Financial Services
- **IFeeProfileFactory**: Fee profile configuration
- **IRoyaltyFeeSlabsFactory**: Royalty fee structure management
- **ILateFeeFactory**: Late fee calculation
- **IFranchiseeServiceFeeService**: Service pricing management
- **IFranchiseeAccountCreditFactory**: Account balance operations

### Document & Communication
- **IFranchiseeDocumentFactory**: Document upload and management
- **IFranchiseeNotesFactory**: Internal note creation
- **IFranchiseeTechnicianMailService**: Technician email tracking

### Integration Services
- **IReviewPushTaazaaFranchiseeMapping**: Review system integration mapping
- **IFranchiseeLeadPerformanceFactory**: Performance metrics generation

## Implementations (Impl/)

All service interfaces are implemented in the `Impl/` subfolder with concrete business logic, validation rules, and data access operations.

## Enumerations (Enum/)

- **FranchiseeStatus**: Active, Inactive, Suspended, Pending
- **OrganizationType**: Corporate, Franchisee, Branch
- **FeeType**: Royalty, Service, Late, Processing
- **PaymentStatus**: Paid, Pending, Overdue, Waived
- **DocumentType**: Contract, License, Insurance, W9

## ViewModels (ViewModel/)

DTOs for API communication:
- **OrganizationViewModel**: Organization data transfer
- **FranchiseeViewModel**: Complete franchisee information
- **FranchiseeInfoViewModel**: Summary information
- **FeeProfileViewModel**: Fee structure data
- **FranchiseeSalesViewModel**: Sales metrics
- **FranchiseeDocumentViewModel**: Document metadata

## Business Rules

### Franchisee Management
1. Each franchisee must have a unique identifier and service area
2. Franchisees can have multiple service offerings
3. Fee structures are customizable per franchisee
4. Geographic territories are exclusive (no overlap)

### Financial Rules
1. Royalty fees calculated based on revenue slabs
2. Late fees apply after grace period
3. Service fees can be franchisee-specific or default
4. Account credits must be tracked for refunds/adjustments

### User-Organization Mapping
1. Users can belong to multiple organizations with different roles
2. Roles determine access permissions
3. Franchisee users have restricted access to their data only
4. Corporate users have access to all franchisee data

## Dependencies

- **Core/Application**: Repository pattern, utilities
- **Core/Users**: User management integration
- **Core/Geo**: Territory and location services
- **Core/Billing**: Payment processing integration
- **Infrastructure**: Data persistence

## For AI Agents

### Adding New Franchisee Features
1. Define entity in `Domain/` if new data structure needed
2. Create factory interface for entity creation: `I[Entity]Factory`
3. Create service interface for operations: `I[Entity]Service`
4. Implement in `Impl/` with validation and business rules
5. Add ViewModel in `ViewModel/` for API communication
6. Update Infrastructure layer for data access
7. Create API endpoints in API/Areas/Organizations

### Modifying Fee Structures
```csharp
// Example: Updating royalty calculation
public class RoyaltyFeeCalculator
{
    public decimal Calculate(decimal revenue, RoyaltyFeeSlabs slabs)
    {
        // Implement tiered calculation
        // Validate against business rules
        // Log calculations for audit
    }
}
```

## For Human Developers

### Common Operations

#### Creating a New Franchisee
```csharp
var franchisee = _franchiseeFactory.Create(new FranchiseeViewModel
{
    Name = "ABC Marble Restoration",
    Email = "contact@abc.com",
    ServiceArea = "New York Metro",
    // ... other properties
});

_unitOfWork.SaveChanges();
```

#### Querying Franchisee Information
```csharp
var info = _franchiseeInfoService.GetFranchiseeInfo(franchiseeId);
var sales = _franchiseeSalesService.GetSalesMetrics(franchiseeId, startDate, endDate);
var performance = _leadPerformanceService.GetLeadConversionRate(franchiseeId);
```

#### Managing Fee Structures
```csharp
var feeProfile = _feeProfileFactory.Create(new FeeProfileViewModel
{
    FranchiseeId = franchiseeId,
    RoyaltyPercentage = 8.5m,
    LateFeeGraceDays = 10,
    // ... fee configuration
});
```

### Best Practices
- Always validate franchisee status before operations
- Log all financial calculations for audit trails
- Enforce territory exclusivity in factory methods
- Use transactions for multi-entity updates
- Cache frequently accessed franchisee data
- Validate user permissions for cross-franchisee operations
