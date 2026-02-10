<!-- AUTO-GENERATED: Header -->
# Organizations/ViewModel — Data Transfer Objects
**Version**: 8b6d86e55b597ea3e4d50ae8311308d72fcc87df
**Generated**: 2025-02-10T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
View Models (DTOs - Data Transfer Objects) define the data contracts for API/UI communication. These classes decouple the presentation layer from domain entities, providing:
- **Edit Models**: Form data structures for create/update operations
- **View Models**: Read-only display data with calculated/formatted fields
- **Filter Models**: Query parameter structures for list/search operations
- **List Models**: Paginated response containers with metadata

### Design Patterns
- **DTO Pattern**: Separate models for API/UI vs. domain persistence
- **Form Models**: Edit models mirror UI form structure, not database schema
- **Projection Pattern**: View models contain only fields needed for display
- **Filter Pattern**: Dedicated classes for search/filter parameters
- **List Response Pattern**: Container with collection + pagination metadata
- **Marker Attributes**: `[NoValidatorRequired]` for read-only models, validators for edit models

### Model Categories (58 Files)
1. **Edit Models** (~20 files): Form data for create/update operations (suffix: EditModel)
2. **View Models** (~20 files): Display data (suffix: ViewModel)
3. **Filter Models** (~6 files): Search/query parameters (suffix: Filter)
4. **List Models** (~6 files): Paginated response containers (suffix: ListModel)
5. **Specialized Models** (~6 files): Domain-specific DTOs (schedules, approvals, etc.)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: File Inventory -->
## Complete File Inventory (58 Files)

### Core Franchisee Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeEditModel.cs** | Edit | Main franchisee create/update form (extends OrganizationEditModel) |
| **FranchiseeViewModel.cs** | View | Display data for franchisee details (extends OrganizationViewModel) |
| **FranchiseeListModel.cs** | List | Paginated franchisee list response with filter + paging |
| **FranchiseeListFilter.cs** | Filter | Query parameters: text search, status, role filters |
| **FranchiseeModel.cs** | View | Simplified franchisee display model |
| **FranchiseeDetailsModel.cs** | View | Detailed franchisee information |
| **FranchiseeRedesignViewModel.cs** | View | Redesigned franchisee display format |
| **FranchiseeResignListModel.cs** | List | Resignation/termination franchisee list |
| **FranchiseeGroupModel.cs** | View | Franchisee grouping/categorization |
| **FranchiseeInfoViewModel.cs** | View | Franchisee information summary |
| **FranchiseeInfoListModel.cs** | List | Franchisee info list container |
| **DeactivateFranchisee.cs** | Command | Franchisee deactivation request model |

### Download/Export Models
| File | Purpose |
|------|---------|
| **FranchiseeViewModelForDownload.cs** | Excel export data structure |
| **FranchiseeViewModelForFranchiseeDirectoryDownload.cs** | Directory export format |
| **FranchiseeViewModelForReport.cs** | Reporting-specific format |

### Fee Profile Models
| File | Type | Purpose |
|------|------|---------|
| **FeeProfileEditModel.cs** | Edit | Fee profile configuration form |
| **FeeProfileViewModel.cs** | View | Fee profile display data |
| **RoyaltyFeeSlabsEditModel.cs** | Edit | Royalty tier configuration |
| **MinRoyaltyFeeSlabsEditModel.cs** | Edit | Minimum royalty tier configuration |
| **LateFeeEditModel.cs** | Edit | Late fee configuration |

### Sales Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeSalesEditModel.cs** | Edit | Sales transaction entry form |
| **FranchiseeSalesViewModel.cs** | View | Sales transaction display |

### Service Fee Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeServiceFeeEditModel.cs** | Edit | Service fee configuration |
| **FranchiseeServiceEditModel.cs** | Edit | Service type assignment |
| **FranchiseeChangeServiceFee.cs** | Command | Service fee update request |

### Account Credit Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeAccountCreditEditModel.cs** | Edit | Credit adjustment entry |
| **FranchiseeAccountCreditModel.cs** | View | Credit display data |
| **FranchiseeAccountCreditList.cs** | List | Credit list container |
| **FranchiseeAccountCreditPaymentViewModel.cs** | View | Credit payment details |
| **ManageFranchiseeAccountModel.cs** | View | Account management summary |

### Document Models
| File | Type | Purpose |
|------|------|---------|
| **DocumentEditModel.cs** | Edit | Document upload form |
| **DocumentViewModel.cs** | View | Document display data |
| **DocumentListModel.cs** | List | Document list container |
| **DocumentListFilter.cs** | Filter | Document search parameters |
| **DocumentTypeEditModel.cs** | Edit | Document type configuration |
| **DocumentTypeViewModel.cs** | View | Document type display |
| **FranchiseeDocumentEditModel.cs** | Edit | Franchisee-specific document upload |
| **FranchiseeDocumentListModel.cs** | List | Franchisee document list |
| **FranchiseeDocumentFilter.cs** | Filter | Franchisee document search |

### Notes & History Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeNotesViewModel.cs** | View | Internal notes display |
| **FranchiseeNotesDurationViewModel.cs** | View | Duration-related notes |
| **FranchiseeRegistrationHistryViewModel.cs** | View | Registration history |
| **DurationApprovalViewModel.cs** | View | Contract duration approval data |

### Lead Performance Models
| File | Type | Purpose |
|------|------|---------|
| **LeadPerformanceEditModel.cs** | Edit | Lead performance data entry |
| **LeadPerformanceFranchiseeDetailsModel.cs** | View | Franchisee lead performance analytics |

### Review/Feedback Models
| File | Type | Purpose |
|------|------|---------|
| **ReviewPushCustomerFeedbackViewModel.cs** | View | Customer feedback display |
| **ReviewPushCustomerFeedbackListModel.cs** | List | Feedback list container |
| **ReviewPushFranchiseeViewModel.cs** | View | ReviewPush integration data |
| **ReviewPushFranchiseeListModel.cs** | List | ReviewPush franchisee list |

### Loan Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeLoanViewModel.cs** | View | Loan details display |
| **LoanScheduleViewModel.cs** | View | Payment schedule display |
| **AmortPaymentSchedule.cs** | View | Amortization schedule |

### Organization Models
| File | Type | Purpose |
|------|------|---------|
| **OrganizationEditModel.cs** | Edit | Base organization form (inherited by FranchiseeEditModel) |
| **OrganizationViewModel.cs** | View | Base organization display (inherited by FranchiseeViewModel) |

### Email Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeEmailEditModel.cs** | Edit | Email configuration form |

### Team Models
| File | Type | Purpose |
|------|------|---------|
| **FranchiseeTeamImageEditModel.cs** | Edit | Team photo upload |
| **FranchiseeTeamImageViewModel.cs** | View | Team photo display |

### Special Purpose Models
| File | Type | Purpose |
|------|------|---------|
| **OneTimeProjectFilter.cs** | Filter | One-time project fee queries |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Key Model Definitions -->
## Critical Model Structures

### FranchiseeEditModel.cs (Extends OrganizationEditModel)
**Primary form model** for franchisee create/update operations.

**Key Properties**:
```csharp
public class FranchiseeEditModel : OrganizationEditModel
{
    // Identity
    public long? UserId { get; set; }
    public long? BusinessId { get; set; }
    public string QuickBookIdentifier { get; set; }
    public string DisplayName { get; set; }
    public string EIN { get; set; }
    public string LegalEntity { get; set; }
    
    // Financial
    public string Currency { get; set; }
    public decimal? RenewalFee { get; set; }
    public DateTime? RenewalDate { get; set; }
    public decimal? TransferFee { get; set; }
    public decimal? OriginalFee { get; set; }
    
    // Contacts (4 types)
    public string ContactFirstName/LastName/Email { get; set; }
    public string AccountPersonFirstName/LastName/Email { get; set; }
    public string MarketingPersonFirstName/LastName/Email { get; set; }
    public string SchedulerFirstName/LastName/Email { get; set; }
    
    // Complex Children (nested edit models)
    public FeeProfileEditModel FeeProfile { get; set; }
    public MinRoyaltyFeeSlabsEditModel MinRoyalityFeeProfile { get; set; }
    public LateFeeEditModel LateFee { get; set; }
    public IEnumerable<FranchiseeServiceEditModel> FranchiseeServices { get; set; }
    public ICollection<FranchiseeServiceFeeEditModel> ServiceFees { get; set; }
    public IEnumerable<DocumentTypeEditModel> Documents { get; set; }
    
    // Email Configuration
    public FranchiseeEmailEditModel FranchiseeEmailEditModel { get; set; }
    public FranchiseeEmailEditModel franchiseeTechMailServiceEditModel { get; set; }
    
    // File Upload
    public FileUploadModel FileUploadModel { get; set; }
    
    // Features
    public bool IsReviewFeedbackEnabled { get; set; }
    public bool SetGeoCode { get; set; }
    public bool IsRoyality { get; set; }
    public bool IsActive { get; set; }
    public bool IsSEOActive { get; set; }
    
    // Organization Owner
    public OrganizationOwnerEditModel OrganizationOwner { get; set; }
    
    // Notes
    public string DeactivationNote { get; set; }
    public string Description { get; set; }
}
```

**Validation**: Validated by FranchiseeEditModelValidator (FluentValidation)

**Inheritance**: Inherits from OrganizationEditModel (Name, TypeId, About, Email, Address, Phones)

### FranchiseeViewModel.cs (Extends OrganizationViewModel)
**Display model** for franchisee read-only views.

```csharp
public class FranchiseeViewModel : OrganizationViewModel
{
    [DisplayName("Owner Name")]
    public string OwnerName { get; set; }
    
    [DisplayName("Quick Book Identifier")]
    public string QuickBookIdentifier { get; set; }
    
    [DisplayName("Sales Report Status")]
    public string SalesReportStatus { get; set; }  // Calculated field
    
    [DisplayName("Currency")]
    public string Currency { get; set; }
    
    [DisplayName("Account Credit")]
    public decimal? AccountCredit { get; set; }  // Calculated field
    
    [DisplayName("Business Id")]
    public long? BusinessId { get; set; }
    
    [DisplayName("Deactivation Note")]
    public string DeactivationNote { get; set; }
    
    // Duration tracking
    public long? FranchiseeDurationCount { get; set; }
    public decimal? Duration { get; set; }
    
    // Notes
    public string NoteFromCallCenter { get; set; }
    public string NoteFromOwner { get; set; }
}
```

**Attributes**: `[DisplayName]` for UI label binding, `[NoValidatorRequired]` to skip validation

### FranchiseeListModel.cs
**Paginated list response** container.

```csharp
public class FranchiseeListModel
{
    public IEnumerable<FranchiseeViewModel> Collection { get; set; }  // Result set
    public FranchiseeListFilter Filter { get; set; }  // Applied filters
    public PagingModel PagingModel { get; set; }  // Pagination metadata
}
```

**PagingModel Structure** (from Core.Application):
```csharp
public class PagingModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
```

### FranchiseeListFilter.cs
**Query parameters** for franchisee list requests.

```csharp
public class FranchiseeListFilter
{
    public string Text { get; set; }  // Full-text search (name, email)
    public long? FranchiseeId { get; set; }  // Specific franchisee
    public string Franchisee { get; set; }  // Franchisee name filter
    public string Email { get; set; }  // Email filter
    
    // Sorting
    public string SortingColumn { get; set; }  // Column name
    public long? SortingOrder { get; set; }  // 0=ASC, 1=DESC
    
    // Status
    public long? FranchiseeStatus { get; set; }  // Status enum value
    public bool? status { get; set; }  // IsActive filter
    
    // Role-based filtering
    public long? RoleId { get; set; }
    public long? LoggedInRoleId { get; set; }
    public long? LoggedInUserId { get; set; }
}
```

### FeeProfileEditModel.cs
**Fee configuration form** model.

```csharp
public class FeeProfileEditModel
{
    public long? Id { get; set; }
    public decimal MinimumRoyaltyPerMonth { get; set; }  // Floor value
    public bool SalesBasedRoyalty { get; set; }  // Tiered vs fixed
    public decimal? FixedAmount { get; set; }  // Used if SalesBasedRoyalty = false
    public decimal AdFundPercentage { get; set; }  // Marketing contribution
    public long? PaymentFrequencyId { get; set; }  // Billing cycle
    
    // Child collection
    public IEnumerable<RoyaltyFeeSlabsEditModel> RoyaltyFeeSlabs { get; set; }
}
```

**Validation**: FeeProfileEditModelValidator ensures:
- MinimumRoyaltyPerMonth >= 0
- AdFundPercentage 0-100%
- If SalesBasedRoyalty = true: RoyaltyFeeSlabs required
- If SalesBasedRoyalty = false: FixedAmount required

### RoyaltyFeeSlabsEditModel.cs
**Tier configuration** for sales-based royalties.

```csharp
public class RoyaltyFeeSlabsEditModel
{
    public long? Id { get; set; }
    public decimal? MinValue { get; set; }  // Tier start (null = 0)
    public decimal? MaxValue { get; set; }  // Tier end (null = infinity)
    public decimal ChargePercentage { get; set; }  // Royalty % for tier
}
```

**Example**:
```csharp
// Tier 1: $0 - $50,000 @ 5%
new RoyaltyFeeSlabsEditModel { MinValue = 0, MaxValue = 50000, ChargePercentage = 5.0m }

// Tier 2: $50,001 - $100,000 @ 6%
new RoyaltyFeeSlabsEditModel { MinValue = 50001, MaxValue = 100000, ChargePercentage = 6.0m }

// Tier 3: $100,001+ @ 7%
new RoyaltyFeeSlabsEditModel { MinValue = 100001, MaxValue = null, ChargePercentage = 7.0m }
```

### FranchiseeSalesEditModel.cs
**Sales transaction entry** form.

```csharp
public class FranchiseeSalesEditModel
{
    public long? Id { get; set; }
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public decimal Amount { get; set; }
    public long ClassTypeId { get; set; }  // Marketing class
    public long? SubClassTypeId { get; set; }
    public string SalesRep { get; set; }
    public string QbInvoiceNumber { get; set; }
    public long? SalesDataUploadId { get; set; }  // Batch reference
}
```

### FranchiseeServiceFeeEditModel.cs
**Service fee configuration** form.

```csharp
public class FranchiseeServiceFeeEditModel
{
    public long? Id { get; set; }
    public long FranchiseeId { get; set; }
    public long ServiceFeeTypeId { get; set; }  // Enum: Loan, Bookkeeping, SEO, etc.
    public decimal Amount { get; set; }
    public decimal? Percentage { get; set; }
    public bool IsActive { get; set; }
    public long? FrequencyId { get; set; }
    
    // SEO-specific
    public DateTime? SaveDateForSeoCost { get; set; }
    public DateTime? InvoiceDateForSeoCost { get; set; }
}
```

### DocumentEditModel.cs
**Document upload** form.

```csharp
public class DocumentEditModel
{
    public long? Id { get; set; }
    public long FranchiseeId { get; set; }
    public long DocumentTypeId { get; set; }  // W9, COI, etc.
    public string DocumentName { get; set; }
    public string Description { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public FileUploadModel FileUpload { get; set; }  // File stream
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **Core.Application.ViewModel** — PagingModel, FileUploadModel
- **Core.Application.Attribute** — [NoValidatorRequired] marker
- **Core.Users.ViewModels** — OrganizationOwnerEditModel, UserEditModel
- **Core.Scheduler.ViewModel** — Email/notification models
- **Core.Geo.ViewModel** — AddressEditModel, PhoneEditModel

### External
- **System.ComponentModel.DataAnnotations** — [DisplayName], [Required], etc.
- **FluentValidation** — Validation rules for edit models
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Model Naming Conventions
- **EditModel**: User input for create/update operations → validated, mutable
- **ViewModel**: Display data → read-only, may include calculated fields
- **ListModel**: Response container → includes collection + pagination + filter
- **Filter**: Query parameters → defines search/sort criteria

### Edit Model vs View Model
**Edit Models**:
- Used for forms (Create, Update)
- Include all writable fields
- May include nested edit models (e.g., FeeProfileEditModel inside FranchiseeEditModel)
- Validated by FluentValidation validators
- Transformed to domain entities by factories

**View Models**:
- Used for display (List, Detail, Reports)
- Include only readable fields
- May include calculated/aggregated fields (e.g., AccountCredit, SalesReportStatus)
- Marked with `[NoValidatorRequired]`
- Transformed from domain entities by factories
- Include `[DisplayName]` attributes for UI binding

### Common Patterns

**List Request/Response**:
```csharp
// Request
var filter = new FranchiseeListFilter { Text = "Dallas", status = true };
var result = service.GetFranchiseeCollection(filter, pageNumber: 1, pageSize: 25);

// Response
FranchiseeListModel {
    Collection: [ FranchiseeViewModel, ... ],  // 25 items
    Filter: { Text: "Dallas", status: true },  // Applied filter
    PagingModel: { PageNumber: 1, PageSize: 25, TotalCount: 157, TotalPages: 7 }
}
```

**Nested Edit Models**:
```csharp
// Single form submission includes entire object graph
var editModel = new FranchiseeEditModel {
    Name = "Dallas Franchise",
    FeeProfile = new FeeProfileEditModel {
        RoyaltyFeeSlabs = new List<RoyaltyFeeSlabsEditModel> { ... }
    },
    FranchiseeServices = new List<FranchiseeServiceEditModel> { ... }
};
service.Save(editModel);  // Saves entire graph in one transaction
```

### Validation Strategy
- **Edit Models**: Have validators (e.g., FranchiseeEditModelValidator)
- **View Models**: Marked with `[NoValidatorRequired]` (no validation needed)
- **Nested Validation**: Parent validator calls child validators automatically

### Attribute Usage
- **[NoValidatorRequired]**: Skips validator lookup (performance optimization)
- **[DisplayName]**: UI label text (e.g., "Owner Name" instead of "OwnerName")
- **[Required]**: Field required (alternative to FluentValidation)

### Performance Considerations
- **View Models**: Only include fields needed for display; avoid loading full domain graph
- **List Models**: Use projections in repository queries; don't load navigation properties unnecessarily
- **Filter Models**: Index database columns used in filter properties

### Testing
- **Edit Model Tests**: Validate form submission scenarios with validators
- **View Model Tests**: Verify factory transformations produce expected display data
- **Filter Tests**: Ensure query parameters generate correct SQL predicates
<!-- END CUSTOM SECTION -->
