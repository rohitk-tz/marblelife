<!-- AUTO-GENERATED: Header -->
# Organizations/Impl — Service Implementations
**Version**: 8b6d86e55b597ea3e4d50ae8311308d72fcc87df
**Generated**: 2025-02-10T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Concrete implementations of service interfaces and factory classes. This folder contains the actual business logic for franchisee management, financial calculations, document processing, and data transformations. Implements dependency injection via `[DefaultImplementation]` attribute for interface resolution.

### Design Patterns
- **Service Layer Pattern**: Business logic encapsulated in service classes (suffixed with `Service`)
- **Factory Pattern**: Bidirectional transformations between domain entities and view models (suffixed with `Factory`)
- **Validator Pattern**: FluentValidation-based edit model validators (suffixed with `Validator`)
- **Repository Pattern**: Data access via injected `IRepository<T>` instances
- **Unit of Work**: Transaction management via `IUnitOfWork`
- **Dependency Injection**: All dependencies constructor-injected, resolved via `[DefaultImplementation]`

### Component Categories
1. **Services** (6 files): Complex business operations, orchestration, reporting
2. **Factories** (14 files): Domain ↔ ViewModel transformations
3. **Validators** (3 files): Edit model validation rules
4. **Migration** (1 file): Data migration utilities
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: File Inventory -->
## Complete File Inventory (24 Files)

### Service Implementations (Business Logic)
| File | Lines | Purpose |
|------|-------|---------|
| **FranchiseeInfoService.cs** | ~2,700 | Main franchisee CRUD, reporting, Excel exports, activation/deactivation |
| **FranchiseeServiceFeeService.cs** | ~1,900 | Service fee calculations, status changes, billing integration |
| **FranchiseeSalesService.cs** | ~500 | Sales data processing, royalty calculations |
| **FranchiseeDocumentService.cs** | ~400 | Document upload, retrieval, compliance reporting |
| **FranchiseeTechnicianMailService.cs** | ~150 | Technician email notification management |
| **LeadPerformanceFranchiseeDetailsService.cs** | ~1,000 | Lead tracking, conversion analytics, performance reporting |

### Factory Implementations (Transformations)
| File | Purpose |
|------|---------|
| **FranchiseeFactory.cs** | Franchisee entity ↔ FranchiseeEditModel/ViewModel transformations |
| **OrganizationFactory.cs** | Organization entity ↔ OrganizationEditModel/ViewModel transformations |
| **FeeProfileFactory.cs** | FeeProfile entity ↔ FeeProfileEditModel/ViewModel transformations |
| **RoyaltyFeeSlabsFactory.cs** | RoyaltyFeeSlabs entity ↔ edit model transformations |
| **LateFeeFactory.cs** | LateFee entity ↔ LateFeeEditModel transformations |
| **FranchiseeSalesFactory.cs** | FranchiseeSales entity ↔ view model transformations |
| **FranchiseeServiceFeeFactory.cs** | FranchiseeServiceFee entity ↔ edit model transformations |
| **FranchiseeServicesFactory.cs** | FranchiseeService entity ↔ service edit model transformations |
| **FranchiseeDocumentFactory.cs** | Document entity ↔ edit/view model transformations |
| **FranchiseeNotesFactory.cs** | FranchiseeNotes entity ↔ view model transformations |
| **FranchiseeAccountCreditFactory.cs** | Account credit entity ↔ edit model transformations |
| **FranchiseeTechnicianMailFactory.cs** | Technician mail entity ↔ edit model transformations |
| **FranchiseeLeadPerformanceFactory.cs** | Lead performance entity ↔ view model transformations |

### Validators (Data Integrity)
| File | Purpose |
|------|---------|
| **FranchiseeEditModelValidator.cs** | Validates FranchiseeEditModel business rules |
| **FeeProfileEditModelValidator.cs** | Validates fee profile configuration |
| **RoyaltyFeeSlabsEditModelValidator.cs** | Validates royalty slab tiers (no gaps/overlaps) |

### Utilities
| File | Purpose |
|------|---------|
| **FranchiseeMigrationService.cs** | Data migration scripts and legacy data transformation |
| **ReviewPushGettingCustomerFeedback.cs** | ReviewPush API integration for customer feedback |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Key Services Detail -->
## Critical Service Implementations

### FranchiseeInfoService.cs (~2,700 lines)
**Main franchisee management service** - orchestrates all franchisee operations.

**Key Dependencies** (23 repositories + 12 factories/services):
```csharp
- IRepository<Franchisee, Organization, FeeProfile, FranchiseeService, etc.>
- IFranchiseeFactory, IOrganizationFactory, IFeeProfileFactory
- IUserService, IPhoneService, IFileService
- IExcelFileCreator, IExcelFranchiseeFileFormaterCreator
```

**Core Methods**:

#### `Get(long userId) → FranchiseeEditModel`
- Loads franchisee by associated user ID
- Includes: Organization, FeeProfile, RoyaltyFeeSlabs, LateFee, Services, Documents
- Returns fully-populated edit model for UI binding

#### `Save(FranchiseeEditModel model) → void`
**Complex orchestration**:
1. Validate edit model via FranchiseeEditModelValidator
2. Load or create Organization entity
3. Create/update Franchisee entity via FranchiseeFactory
4. Save Organization (cascade saves Franchisee, FeeProfile, etc.)
5. Save FranchiseeServices (delete removed, add new)
6. Save FranchiseeServiceFee entries
7. Save FranchiseeDocumentType configurations
8. If new franchisee: create user account, assign role
9. Commit transaction

**Validation Rules**:
- BusinessId must be unique (IsUniqueBusinessId check)
- Organization name required
- At least one service must be selected
- Fee profile configuration must be valid (validator checks)

#### `GetFranchiseeCollection(filter, pageNumber, pageSize) → FranchiseeListModel`
**Filtering & Pagination**:
- Filters: IsActive, State, SearchText (name/email), DateRange
- Sorting: Name, State, DateCreated, etc.
- Includes: Organization, Address (for state), FeeProfile
- Returns: Paginated results + total count

#### `DeactivateFranchisee(franchiseeId, note) → bool`
**Deactivation Workflow**:
1. Load franchisee with Organization
2. Check for active invoices (block if any exist)
3. Set Organization.IsActive = false
4. Store deactivation note
5. Return success status

**Business Rule**: Cannot deactivate with pending financial obligations

#### `DownloadFranchisee(filter) → bool, out fileName`
**Excel Export**:
1. Query franchisees with filter
2. Transform to FranchiseeViewModelForDownload via factory
3. Generate Excel via IExcelFileCreator
4. Return file path

**Variations**:
- `DownloadFranchiseeDirectory()`: Different column set for directory
- `DownloadTaxReport()`: Tax-specific document report
- `DownloadFileFranchiseeDirectoryRedesign()`: Redesigned directory format

### FranchiseeServiceFeeService.cs (~1,900 lines)
**Service fee management and calculations**.

**Key Methods**:

#### `GetFranchiseeServiceFeeCollection(filter, page, size) → ListModel`
- Filters: FranchiseeId, ServiceFeeTypeId, IsActive, DateRange
- Returns paginated service fee records

#### `OneTimeProjectChangeStatus(filter) → bool`
**One-Time Project Fee Activation**:
1. Load franchisee and validate
2. Create or activate FranchiseeServiceFee with ServiceFeeType.OneTimeProject
3. Set amount, frequency, dates
4. Trigger invoice generation if applicable

#### `SEOChargesChangeStatus(filter) → bool`
**SEO Charge Toggle**:
1. Load existing SEO service fee
2. Toggle IsActive flag
3. Update SaveDateForSeoCost and InvoiceDateForSeoCost
4. Return status

**Complex Calculation Logic** (internal methods):
- Tiered fee calculations based on sales volume
- Frequency-based proration (monthly, quarterly, etc.)
- Integration with billing cycle dates

### FranchiseeSalesService.cs (~500 lines)
**Sales data processing and royalty calculation**.

**Key Methods**:

#### `ProcessSalesData(franchiseeId, salesData) → void`
1. Validate sales data format
2. Match to FranchiseeServices (reject if service not enabled)
3. Create FranchiseeSales records
4. Link to Customer, Invoice, MarketingClass
5. Apply currency exchange rates if applicable
6. Trigger royalty calculation

#### `GetSalesReport(franchiseeId, dateRange) → Report`
- Aggregates sales by service type
- Calculates royalties using FeeProfile
- Returns formatted report data

### FranchiseeDocumentService.cs (~400 lines)
**Document lifecycle management**.

#### `UploadDocument(editModel) → bool`
1. Validate document type against FranchiseeDocumentType configuration
2. Save file to filesystem via IFileService
3. Create FranchiseDocument record
4. Associate with franchisee
5. Return success status

#### `GetFranchiseeDocumentReport(filter) → DocumentListModel`
- Filters: DocumentTypeId, FranchiseeId, DateRange
- Used for compliance reporting (W9s, COIs, tax documents)
- Returns document metadata with download links

### LeadPerformanceFranchiseeDetailsService.cs (~1,000 lines)
**Lead performance tracking and analytics**.

**Responsibilities**:
- Track lead sources and conversion rates
- Calculate ROI on marketing spend
- Generate performance dashboards
- Compare franchisee performance metrics
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Factory Pattern Detail -->
## Factory Pattern Implementation

### FranchiseeFactory.cs
**Bidirectional transformations** for Franchisee entity.

**Dependencies**: Injects 9 other factories (Organization, FeeProfile, Services, etc.)

#### `CreateEditModel(Franchisee domain, Person person) → FranchiseeEditModel`
**Domain → View Model**:
- Maps Franchisee properties to edit model
- Calls OrganizationFactory for organization data
- Calls FeeProfileFactory for fee profile (including RoyaltyFeeSlabs)
- Transforms collections: FranchiseeServices, ServiceFees, Documents
- Returns fully-populated edit model for UI

#### `CreateDomain(FranchiseeEditModel model, Franchisee inDb) → Franchisee`
**View Model → Domain**:
- Updates existing entity (inDb) or creates new
- Sets scalar properties (Currency, QuickBookIdentifier, etc.)
- Calls sub-factories for complex children:
  - FeeProfileFactory.CreateDomain() → FeeProfile
  - LateFeeFactory.CreateDomain() → LateFee
  - FranchiseeServicesFactory.CreateDomain() → ICollection<FranchiseeService>
- Handles cascade relationships (bidirectional references)
- Returns domain entity ready for persistence

#### `CreateViewModel(Franchisee domain, durationList) → FranchiseeViewModel`
**Read-Only Display Model**:
- Simplified model for display (not editing)
- Includes calculated fields (e.g., duration notes summary)
- Used in lists, reports, detail views

**Pattern**: All factories follow this structure:
1. `CreateEditModel()` → For forms
2. `CreateDomain()` → For persistence
3. `CreateViewModel()` → For display

### OrganizationFactory.cs
**Handles Organization transformations** (base entity).

**Key Method**:
```csharp
CreateDomain(OrganizationEditModel model) → Organization
```
- Maps Name, Email, About, TypeId, IsActive
- Transforms Address collection via IAddressFactory
- Transforms Phone collection via IPhoneFactory
- Sets DataRecorderMetaData (audit fields)

**Used By**: FranchiseeFactory calls this for Organization portion of Franchisee

### FeeProfileFactory.cs
**Handles FeeProfile and RoyaltyFeeSlabs transformations**.

```csharp
CreateDomain(FeeProfileEditModel model, franchiseeId, inDb) → FeeProfile
```
- Maps MinimumRoyaltyPerMonth, SalesBasedRoyalty, FixedAmount, AdFundPercentage
- Calls RoyaltyFeeSlabsFactory for each slab
- Validates slab configuration (no gaps, continuous coverage)
- Returns FeeProfile with collection of RoyaltyFeeSlabs

**Critical**: Slab validation ensures correct royalty calculation
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Validator Pattern Detail -->
## Validator Implementations

### FranchiseeEditModelValidator.cs
**FluentValidation rules** for FranchiseeEditModel.

**Sample Rules**:
```csharp
RuleFor(x => x.BusinessId)
    .NotEmpty().WithMessage("Business ID is required")
    .Must(BeUniqueBusinessId).WithMessage("Business ID already exists");

RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Organization name is required")
    .MaximumLength(200);

RuleFor(x => x.FeeProfile)
    .NotNull().WithMessage("Fee profile is required")
    .SetValidator(new FeeProfileEditModelValidator());

RuleFor(x => x.FranchiseeServices)
    .Must(x => x != null && x.Any()).WithMessage("At least one service required");
```

**Custom Validators**:
- `BeUniqueBusinessId()`: Queries database to ensure uniqueness
- Cascade validation: FeeProfile validator called automatically

### FeeProfileEditModelValidator.cs
**Validates fee profile configuration**.

**Rules**:
```csharp
RuleFor(x => x.MinimumRoyaltyPerMonth)
    .GreaterThanOrEqualTo(0).WithMessage("Minimum royalty cannot be negative");

RuleFor(x => x.AdFundPercentage)
    .InclusiveBetween(0, 100).WithMessage("Ad fund percentage must be 0-100%");

When(x => x.SalesBasedRoyalty, () => {
    RuleFor(x => x.RoyaltyFeeSlabs)
        .NotEmpty().WithMessage("Slabs required for sales-based royalty")
        .SetValidator(new RoyaltyFeeSlabsCollectionValidator());
});

When(x => !x.SalesBasedRoyalty, () => {
    RuleFor(x => x.FixedAmount)
        .NotNull().WithMessage("Fixed amount required when not sales-based");
});
```

### RoyaltyFeeSlabsEditModelValidator.cs
**Validates royalty slab tiers** (most complex).

**Rules**:
1. **No Gaps**: MaxValue[i] + 1 = MinValue[i+1]
2. **No Overlaps**: Ranges must be mutually exclusive
3. **Full Coverage**: First slab starts at 0 (or null), last slab has MaxValue = null
4. **Valid Percentages**: ChargePercentage must be 0-100

**Implementation**:
```csharp
RuleFor(x => x)
    .Must(HaveNoGaps).WithMessage("Slabs must have continuous coverage")
    .Must(HaveNoOverlaps).WithMessage("Slab ranges cannot overlap")
    .Must(CoverFullRange).WithMessage("Slabs must cover 0 to infinity");

RuleForEach(x => x.RoyaltyFeeSlabs).ChildRules(slab => {
    slab.RuleFor(x => x.ChargePercentage)
        .InclusiveBetween(0, 100);
});
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **Core.Application** — IRepository, IUnitOfWork, IClock, ISortingHelper
- **Core.Users** — IUserService, IPersonFactory
- **Core.Geo** — IPhoneService, IAddressFactory
- **Core.Billing** — Invoice generation, payment processing
- **Core.Sales** — ISalesDataUploadService, customer management
- **Core.Scheduler** — Technician mail integration
- **Core.Reports** — IExcelFileCreator, reporting engines

### External Packages
- **FluentValidation** — Edit model validation
- **Entity Framework Core** — Repository pattern implementation
- **NodaTime** — Date/time handling
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Service Layer Best Practices
1. **Transaction Management**: All Save operations wrapped in UnitOfWork transaction
2. **Cascade Saves**: Organization.Save() cascades to Franchisee and children via [CascadeEntity]
3. **Factory Usage**: Always use factories for transformations; never manually map properties
4. **Validation**: Run validators before calling Save(); they enforce business rules

### Performance Considerations
- **FranchiseeInfoService.Get()**: Loads 10+ related entities; use projections for lists
- **Excel Exports**: Large datasets (1000+ records) may timeout; implement pagination or async
- **Lead Performance Queries**: Complex aggregations; consider materialized views for dashboards

### Common Patterns
**Service Method Structure**:
```csharp
public void Save(FranchiseeEditModel model)
{
    // 1. Validate
    var validator = new FranchiseeEditModelValidator();
    var result = validator.Validate(model);
    if (!result.IsValid) throw new ValidationException(result.Errors);
    
    // 2. Load existing or create new
    var franchisee = model.Id > 0 
        ? _franchiseeRepository.Get(model.Id) 
        : null;
    
    // 3. Transform via factory
    var domain = _franchiseeFactory.CreateDomain(model, franchisee);
    
    // 4. Save (cascade)
    _organizationRepository.Save(domain.Organization);
    
    // 5. Handle collections
    SaveServices(model, domain);
    SaveServiceFees(model, domain);
    
    // 6. Commit
    _unitOfWork.Commit();
}
```

### Testing Strategies
1. **Unit Test Factories**: Mock repositories, test transformations independently
2. **Integration Test Services**: Use in-memory database, test full Save/Get cycle
3. **Validator Tests**: Test each business rule in isolation

### Migration & Maintenance
- **FranchiseeMigrationService**: Use for one-time data fixes; do not add permanent business logic
- **Deprecation Strategy**: Mark old methods `[Obsolete]`, document replacement in comments
- **Breaking Changes**: Version APIs if changing FranchiseeEditModel structure
<!-- END CUSTOM SECTION -->
