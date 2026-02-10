<!-- AUTO-GENERATED: Header -->
# Organizations Module — Core Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-02-10T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Organizations module manages the complete lifecycle of franchisee organizations, their fee structures, document management, sales tracking, and service fee calculations. It implements a factory pattern for domain/view model transformations, service layer for business logic, and validator pattern for data integrity. This is the central hub for franchisee financial operations, combining organization metadata, billing profiles, service tracking, and document workflows.

### Design Patterns
- **Factory Pattern**: Separate factories for each domain entity (Franchisee, Organization, FeeProfile, etc.) to handle bidirectional transformations between domain models and view models
- **Service Layer**: Business logic encapsulated in service interfaces (IFranchiseeInfoService, IFranchiseeServiceFeeService, etc.) with default implementations
- **Repository Pattern**: Data access abstracted through IRepository<T> for all domain entities
- **Validator Pattern**: FluentValidation-based validators for edit models (FranchiseeEditModelValidator, FeeProfileEditModelValidator, RoyaltyFeeSlabsEditModelValidator)
- **DTO Pattern**: Separate ViewModel namespace for data transfer objects used in API/UI communication

### Data Flow
1. **Franchisee Creation/Update**:
   - Input: FranchiseeEditModel (from UI/API)
   - Validation: FranchiseeEditModelValidator checks business rules
   - Factory: IFranchiseeFactory transforms view model → domain entity
   - Service: IFranchiseeInfoService orchestrates persistence via repositories
   - Domain: Franchisee entity with cascading relationships (FeeProfile, FranchiseeServices, LateFee, etc.)
   
2. **Fee Profile Management**:
   - FeeProfile defines royalty structure: sales-based percentage OR fixed amount
   - RoyaltyFeeSlabs configure tiered royalty rates based on sales ranges
   - MinimumRoyaltyPerMonth ensures base revenue regardless of sales
   - Payment frequency controls billing cycles (monthly/quarterly/etc.)

3. **Service Fee Tracking**:
   - FranchiseeServiceFee tracks recurring/one-time charges (loans, bookkeeping, SEO, etc.)
   - ServiceFeeType enum defines charge categories
   - FranchiseeServiceFeeService handles complex fee calculations and status changes

4. **Document Management**:
   - FranchiseDocument stores franchisee-specific documents
   - DocumentType defines document categories
   - IFranchiseeDocumentService handles document lifecycle and reporting
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: File Inventory -->
## File Inventory (24 Root Files)

### Interfaces (Contracts)
| File | Purpose |
|------|---------|
| `IFranchiseeInfoService.cs` | Main franchisee CRUD operations, reporting, activation/deactivation |
| `IFranchiseeFactory.cs` | Franchisee entity ↔ view model transformations |
| `IOrganizationFactory.cs` | Organization entity ↔ view model transformations |
| `IFeeProfileFactory.cs` | Fee profile entity ↔ view model transformations |
| `IFranchiseeSalesService.cs` | Sales data processing and reporting |
| `IFranchiseeSalesFactory.cs` | Sales entity ↔ view model transformations |
| `IFranchiseeServiceFeeService.cs` | Service fee calculations and management |
| `IFranchiseeServiceFeeFactory.cs` | Service fee entity ↔ view model transformations |
| `IFranchiseeDocumentService.cs` | Document upload, retrieval, and reporting |
| `IFranchiseeDocumentFactory.cs` | Document entity ↔ view model transformations |
| `IFranchiseeTechnicianMailService.cs` | Technician email notification management |
| `IFranchiseeTechnicianMailFactory.cs` | Technician mail entity ↔ view model transformations |
| `ILeadPerformanceFranchiseeDetailsService.cs` | Lead performance tracking and analytics |
| `IFranchiseeLeadPerformanceFactory.cs` | Lead performance entity ↔ view model transformations |
| `IOrganizationRoleUserService.cs` | User role assignments within organizations |
| `IOrganizationRoleUserInfoService.cs` | Organization role user information retrieval |
| `IFranchiseeServicesFactory.cs` | Franchisee service offerings transformations |
| `IFranchiseeNotesFactory.cs` | Franchisee notes entity ↔ view model transformations |
| `IFranchiseeAccountCreditFactory.cs` | Account credit entity ↔ view model transformations |
| `ILateFeeFactory.cs` | Late fee configuration transformations |
| `IRoyaltyFeeSlabsFactory.cs` | Royalty fee tier configuration transformations |
| `IReviewPushTaazaaFranchiseeMapping.cs` | Review Push integration service |

### Implementation
| File | Purpose |
|------|---------|
| `OrganizationRoleUserInfoService.cs` | Concrete implementation of organization role user info service |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Key Domain Entities

### Organization (organization.cs in Domain/)
Core organization entity representing any business entity (corporate, franchisee, etc.).

```csharp
public class Organization : DomainBase
{
    public string Name { get; set; }
    public long TypeId { get; set; }  // Lookup reference
    public string Email { get; set; }
    public string About { get; set; }
    public bool IsActive { get; set; }
    public string DeactivationnNote { get; set; }
    
    // Relationships
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    public virtual ICollection<Address> Address { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
    public virtual Franchisee Franchisee { get; set; }  // One-to-one
}
```

### Franchisee (Domain/Franchisee.cs)
Extends Organization with franchisee-specific properties and relationships.

```csharp
public class Franchisee : DomainBase
{
    [ForeignKey("Organization")]
    public override long Id { get; set; }  // Shared primary key with Organization
    
    // Business Identity
    public long? BusinessId { get; set; }  // Unique business identifier
    public string QuickBookIdentifier { get; set; }
    public string DisplayName { get; set; }
    public string EIN { get; set; }
    public string LegalEntity { get; set; }
    
    // Financial
    public string Currency { get; set; }
    public decimal? RenewalFee { get; set; }
    public decimal? TransferFee { get; set; }
    public decimal? OriginalFranchiseeFee { get; set; }
    public DateTime? DateOfRenewal { get; set; }
    
    // Contacts (multiple contact types)
    public string ContactFirstName/LastName/Email { get; set; }
    public string AccountPersonFirstName/LastName/Email { get; set; }
    public string MarketingPersonFirstName/LastName/Email { get; set; }
    public string SchedulerFirstName/LastName/Email { get; set; }
    
    // Features
    public bool IsReviewFeedbackEnabled { get; set; }
    public bool SetGeoCode { get; set; }
    public bool IsRoyality { get; set; }
    public bool IsMinRoyalityFixed { get; set; }
    public bool IsSEOActive { get; set; }
    
    // Cascade Relationships
    public virtual Organization Organization { get; set; }
    public virtual FeeProfile FeeProfile { get; set; }
    public virtual LateFee LateFee { get; set; }
    public virtual ICollection<FranchiseeService> FranchiseeServices { get; set; }
    public virtual ICollection<FranchiseeServiceFee> FranchiseeServiceFee { get; set; }
    public virtual ICollection<FranchiseeSales> FranchiseeSales { get; set; }
    public virtual ICollection<FranchiseeAccountCredit> FranchiseeAccountCredit { get; set; }
    public virtual ICollection<FranchiseeNotes> FranchiseeNotes { get; set; }
    public virtual ICollection<FranchiseeDocumentType> FranchiseeDocumentType { get; set; }
}
```

### FeeProfile (Domain/FeeProfile.cs)
Defines royalty calculation structure for a franchisee.

```csharp
public class FeeProfile : DomainBase
{
    [ForeignKey("Franchisee")]
    public override long Id { get; set; }  // One-to-one with Franchisee
    
    public decimal MinimumRoyaltyPerMonth { get; set; }  // Floor value
    public bool SalesBasedRoyalty { get; set; }  // If true: use RoyaltyFeeSlabs, else use FixedAmount
    public decimal? FixedAmount { get; set; }  // Used when SalesBasedRoyalty = false
    public decimal AdFundPercentage { get; set; }  // Marketing contribution
    public long? PaymentFrequencyId { get; set; }  // Monthly, Quarterly, etc.
    
    public virtual ICollection<RoyaltyFeeSlabs> RoyaltyFeeSlabs { get; set; }  // Tiered rates
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Public Interfaces -->
## Primary Service Interfaces

### IFranchiseeInfoService
Main service for franchisee lifecycle management.

#### `Get(long userId) → FranchiseeEditModel`
- **Input**: User ID
- **Output**: Edit model with franchisee data, organization, fee profile, services, documents
- **Behavior**: Loads franchisee by associated user, includes all cascade entities

#### `Save(FranchiseeEditModel) → void`
- **Input**: Complete franchisee edit model
- **Behavior**: 
  - Validates via FranchiseeEditModelValidator
  - Creates/updates Organization, Franchisee, FeeProfile, FranchiseeServices
  - Handles user account creation if new franchisee
  - Manages cascade saves for all child entities
- **Side Effects**: DB transaction, user account creation, email notifications

#### `GetFranchiseeCollection(filter, pageNumber, pageSize) → FranchiseeListModel`
- **Input**: Filter criteria (name, status, date range), pagination
- **Output**: Paginated list with total count
- **Behavior**: Supports sorting, filtering by active/inactive, date ranges

#### `DeactivateFranchisee(franchiseeId, note) → bool`
- **Behavior**: Sets IsActive = false on Organization, stores deactivation note
- **Validation**: Cannot deactivate if active invoices exist

#### `DownloadFranchisee(filter) → bool, out fileName`
- **Output**: Excel file path with franchisee data export
- **Behavior**: Generates Excel via IExcelFileCreator with filtered franchisees

#### `GetFranchiseeFeeProfile(franchiseeId) → FeeProfileViewModel`
- **Output**: Fee profile with royalty slabs, payment frequency
- **Behavior**: Used for displaying billing configuration

### IFranchiseeServiceFeeService
Manages recurring and one-time service fees.

#### `GetFranchiseeServiceFeeCollection(filter, page, size) → FranchiseeServiceFeeListModel`
- **Input**: Filter by franchisee, service type, date range
- **Output**: Paginated list of service fees with status, amounts

#### Service Fee Operations
- **OneTimeProjectChangeStatus**: Activate/deactivate one-time project fees
- **SEOChargesChangeStatus**: Activate/deactivate SEO charges
- Complex fee calculations based on ServiceFeeType enum

### IFranchiseeSalesService
Sales data processing and reporting.

#### `ProcessSalesData(franchiseeId, salesData) → void`
- **Behavior**: Imports sales records, validates against FranchiseeServices
- **Calculations**: Applies fee profile rules to calculate royalties

### IFranchiseeDocumentService
Document management for franchisee records.

#### `UploadDocument(editModel) → bool`
- **Input**: Document metadata + file stream
- **Behavior**: Saves to file system, creates database record
- **Validation**: Document type must match franchisee configuration

#### `GetFranchiseeDocumentReport(filter) → DocumentListModel`
- **Output**: Filtered document list with metadata
- **Use Case**: Tax document reporting, compliance tracking
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Users](../../Users/.context/CONTEXT.md)** — Person, UserLogin, authentication/authorization
- **[Core.Geo](../../Geo/.context/CONTEXT.md)** — Address, Phone, Country, geo-coding services
- **[Core.Application](../../Application/.context/CONTEXT.md)** — DomainBase, Repository, UnitOfWork, validators
- **[Core.Billing](../../Billing/.context/CONTEXT.md)** — FranchiseeInvoice, payment processing
- **[Core.Sales](../../Sales/.context/CONTEXT.md)** — SalesDataUpload, sales reporting
- **[Core.Scheduler](../../Scheduler/.context/CONTEXT.md)** — FranchiseeTechMailService integration
- **[Core.Reports](../../Reports/.context/CONTEXT.md)** — File generation, Excel exports

### External Packages
- **FluentValidation** — Edit model validation
- **Entity Framework** — ORM for domain persistence
- **NodaTime** — Date/time handling with time zones
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Submodules -->
## Submodules

| Folder | Files | Purpose |
|--------|-------|---------|
| **[Domain/](.context/Domain/CONTEXT.md)** | 36 | Entity classes: Franchisee, Organization, FeeProfile, FranchiseeSales, etc. |
| **[Enum/](.context/Enum/CONTEXT.md)** | 13 | Enumerations: ServiceType, ServiceFeeType, DocumentType, PaymentFrequency, etc. |
| **[Impl/](.context/Impl/CONTEXT.md)** | 24 | Service implementations, factories, validators |
| **[ViewModel/](.context/ViewModel/CONTEXT.md)** | 58 | DTOs for API/UI: Edit models, view models, filters, list models |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Critical Business Rules
1. **BusinessId Uniqueness**: BusinessId must be unique across all franchisees (validated via IsUniqueBusinessId)
2. **Organization-Franchisee Relationship**: Shared primary key pattern (Franchisee.Id = Organization.Id)
3. **Fee Calculation Logic**:
   - If `SalesBasedRoyalty = true`: Use RoyaltyFeeSlabs (tiered percentages)
   - Else: Use FixedAmount
   - Always enforce MinimumRoyaltyPerMonth floor
4. **Cascade Deletes**: Organization deletion cascades to Franchisee and all children (marked with [CascadeEntity] attribute)
5. **Deactivation vs Deletion**: Franchisees are deactivated (IsActive = false), not deleted, to preserve historical records

### Common Workflows
1. **New Franchisee Onboarding**:
   - Create Organization → Create Franchisee → Configure FeeProfile → Assign Services → Create User Account
2. **Sales Processing**:
   - Upload sales data → Match to FranchiseeServices → Calculate royalties via FeeProfile → Generate invoice
3. **Fee Structure Changes**:
   - Update FeeProfile → Adjust RoyaltyFeeSlabs → Set effective date → Recalculate pending invoices

### Performance Considerations
- FranchiseeInfoService loads many relationships: use eager loading (.Include()) judiciously
- Excel exports can timeout for large datasets: consider pagination or async processing
- GeoCode lookups (SetGeoCode flag) make external API calls: batch operations when possible
<!-- END CUSTOM SECTION -->
