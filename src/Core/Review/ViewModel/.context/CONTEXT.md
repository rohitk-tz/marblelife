<!-- AUTO-GENERATED: Header -->
# Review/ViewModel — Module Context
**Version**: 64667c5c8c4ab9b3d804e48deb14e9b70895fc42
**Generated**: 2025-01-19T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The ViewModel subfolder defines **14 Data Transfer Objects (DTOs)** that facilitate communication between the Review module and external systems — both outbound (API request models for GatherUp/ReviewPush) and inbound (API response models, report view models for UI display). These POCOs act as the "contract" between the service layer and consumers (APIs, web controllers, report generators).

ViewModels serve three purposes:
1. **API Integration**: Models sent to and received from external review platforms
2. **Report Display**: Unified view models for multi-source report aggregation
3. **Data Validation**: Filtering and search criteria for report queries

### Design Patterns
- **DTO Pattern**: Anemic POCOs with public properties, no behavior
- **API Contract Pattern**: Models mirror external API JSON structure (e.g., `customerEmail` instead of `CustomerEmail` to match GatherUp API)
- **Filter/Criteria Pattern**: `CustomerFeedbackReportFilter` encapsulates complex query parameters
- **List Wrapper Pattern**: `*ListModel` classes wrap collections with pagination metadata

### File Categories

**API Request Models** (sent TO external systems):
- `CreateBusinessForReviewModel` — GatherUp business creation
- `CreateCustomerForReviewModel` — GatherUp customer creation
- `UpdateCustomerRecordViewModel` — GatherUp customer update
- `GetCustomerForReviewModel` — GatherUp customer fetch
- `GetBusinessReviewModel` — GatherUp business fetch
- `FeedbackRequestModel` — ReviewPush feedback fetch

**API Response Models** (received FROM external systems):
- `ReviewAPIResponseModel` — GatherUp API responses
- `FeedbackResponseViewModel` — GatherUp individual review
- `FeedbackResponseListModel` — GatherUp review collection
- `ReviewPushResponseViewModel` — ReviewPush individual review
- `ReviewPushResponseListModel` — ReviewPush review collection

**Report Models** (for internal reporting/UI):
- `CustomerFeedbackReportViewModel` — Unified report row (all sources)
- `CustomerFeedbackReportListModel` — Paginated report collection
- `CustomerFeedbackReportFilter` — Report query criteria

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### API Request Models (GatherUp)

#### CreateCustomerForReviewModel.cs
```csharp
public class CreateCustomerForReviewModel
{
    public long businessId { get; set; }              // GatherUp business ID
    public string clientId { get; set; }              // GatherUp client ID
    public string customerEmail { get; set; }
    public string customerFirstName { get; set; }
    public string customerLastName { get; set; }
    public string customerPhone { get; set; }
    public string hash { get; set; }                  // SHA256 authentication hash
    public int sendFeedbackRequest { get; set; }      // 0 = don't send, 1 = send immediately
    public string communicationPreference { get; set; }  // "email", "sms"
    public long customerCustomId { get; set; }        // Internal Marblelife customer ID
}
```
**Note**: Property names use camelCase to match GatherUp API JSON format.

---

#### UpdateCustomerRecordViewModel.cs
```csharp
public class UpdateCustomerRecordViewModel
{
    public long businessId { get; set; }
    public string clientId { get; set; }
    public string customerEmail { get; set; }
    public string customerFirstName { get; set; }
    public long customerId { get; set; }              // GatherUp's customer ID (not Marblelife's)
    public string customerLastName { get; set; }
    public string hash { get; set; }
}
```

---

#### GetCustomerForReviewModel.cs
```csharp
public class GetCustomerForReviewModel
{
    public string clientId { get; set; }
    public long customerId { get; set; }              // GatherUp's customer ID
    public string hash { get; set; }
}
```

---

### API Request Models (ReviewPush)

#### FeedbackRequestModel.cs
```csharp
public class FeedbackRequestModel
{
    public long businessId { get; set; }
    public string clientId { get; set; }
    public string from { get; set; }                  // Date string: "yyyy-MM-dd"
    public string hash { get; set; }                  // Not used by ReviewPush (legacy field)
    public int page { get; set; }
    public string to { get; set; }                    // Date string: "yyyy-MM-dd"
}
```
**Note**: ReviewPush uses URL parameters, not POST body. This model is partially legacy.

---

### API Response Models (GatherUp)

#### ReviewAPIResponseModel.cs
```csharp
public class ReviewAPIResponseModel
{
    public string businessId { get; set; }
    public string firstName { get; set; }             // From customer fetch/create
    public string lastName { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string jobId { get; set; }
    public string CustomId { get; set; }
    public int errorCode { get; set; }                // 0 = success, non-zero = error
    public string errorMessage { get; set; }
    public long CustomerId { get; set; }              // GatherUp's customer ID (populated after API call)
    public long ReviewSystemRecordId { get; set; }    // Internal linking ID (set by service layer)
    public string DataPacket { get; set; }            // JSON sent to API (for debugging)
    public bool IsvalidName { get; set; }             // Internal flag (not from API)
    public bool IsQueued { get; set; }                // Internal flag (not from API)
}
```
**Mixed Purpose**: Contains both API response fields and internal service-layer fields.

---

#### FeedbackResponseViewModel.cs
```csharp
public class FeedbackResponseViewModel
{
    public int rating { get; set; }                   // 1-10 scale
    public int recommend { get; set; }                // 0-10 scale
    public string dateOfReview { get; set; }          // ISO 8601: "2024-01-15T10:30:00"
    public bool showReview { get; set; }
    public string authorEmail { get; set; }
    public string authorName { get; set; }
    public string body { get; set; }                  // Review text
    public long customId { get; set; }                // GatherUp's custom customer ID
    public long FeedbackId { get; set; }              // GatherUp's feedback ID
}
```

---

#### FeedbackResponseListModel.cs
```csharp
public class FeedbackResponseListModel
{
    public int page { get; set; }
    public int perPage { get; set; }
    public int pages { get; set; }
    public int count { get; set; }
    public int errorCode { get; set; }
    public string errorMessage { get; set; }
    public ICollection<FeedbackResponseViewModel> reviews { get; set; }
}
```

---

### API Response Models (ReviewPush)

#### ReviewPushResponseViewModel.cs
```csharp
public class ReviewPushResponseViewModel
{
    public string Name { get; set; }                  // Customer name from API
    public long? Id { get; set; }                     // Review ID
    public long? Location_id { get; set; }            // Maps to ReviewPushAPILocation
    public long? Customer_id { get; set; }            // ReviewPush customer ID (not Marblelife ID)
    public long? FranchiseeId { get; set; }           // Populated by service layer
    public string Franchise_name { get; set; }
    public string Rating { get; set; }                // String: "1" to "5"
    public DateTime? Rp_date { get; set; }            // Review submitted date
    public DateTime? Db_date { get; set; }            // Database sync date
    public string Url { get; set; }                   // Contains "mailto:email@example.com"
    public string Review { get; set; }                // Review text
    public string Email { get; set; }                 // Populated by service after parsing Url
    public long? CustomerId { get; set; }             // Marblelife customer ID (set by service)
    public string Taz_franchise_name { get; set; }    // Taazaa internal franchisee name
}
```

---

#### ReviewPushResponseListModel.cs
```csharp
public class ReviewPushResponseListModel
{
    public string result { get; set; }                // "SUCCESS" or error message
    public ICollection<ReviewPushResponseViewModel> info { get; set; }
}
```

---

### Report View Models

#### CustomerFeedbackReportViewModel.cs
**Purpose**: Unified report row representing feedback from any source (GatherUp, Google, ReviewPush).

```csharp
public class CustomerFeedbackReportViewModel
{
    public long Id { get; set; }
    [DownloadField(Required = false)]
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public string Customer { get; set; }              // Customer name
    public string ContactPerson { get; set; }
    public string CustomerEmail { get; set; }
    public string Franchisee { get; set; }            // Franchisee organization name
    public DateTime? ResponseReceivedDate { get; set; }
    public DateTime? ResponseSyncingDate { get; set; }
    public decimal Rating { get; set; }               // Normalized to 0-5 scale
    [DownloadField(Required = false)]
    public long ResponseId { get; set; }
    public string ResponseContent { get; set; }       // Review text
    [DownloadField(Required = false)]
    public double Recommend { get; set; }             // GatherUp 0-10 score
    [DownloadField(Required = false)]
    public bool IsFromNewReviewSystem { get; set; }
    [DownloadField(Required = false)]
    public int IsFromCustomerReviewTable { get; set; }
    public string CustomerNameFromAPI { get; set; }   // Original name from API (may differ)
    public long AuditStatusId { get; set; }
    public string AuditStatus { get; set; }           // "Pending", "Approved", "Rejected"
    public string FromTable { get; set; }             // "CustomerFeedbackRequest", "CustomerFeedbackResponse", "ReviewPushCustomerFeedback"
    public string From { get; set; }                  // "GatherUp", "Google", "ReviewSystem"
}
```

**Key Fields**:
- `From` / `FromTable`: Identifies data source
- `Rating`: Normalized to 0-5 star scale (GatherUp's 0-10 `Recommend` is divided by 2)
- `AuditStatus`: Human-readable status for approval workflow
- `[DownloadField(Required = false)]`: Excludes field from Excel export

---

#### CustomerFeedbackReportFilter.cs
**Purpose**: Query criteria for report filtering.

```csharp
[NoValidatorRequired]
public class CustomerFeedbackReportFilter
{
    public long FranchiseeId { get; set; }            // 0 = all franchisees
    public long CustomerId { get; set; }              // 0 = all customers
    public string Text { get; set; }                  // Search term (name, email, contact)
    public DateTime? StartDate { get; set; }          // Request sent date range (nullable = no filter)
    public DateTime? EndDate { get; set; }
    public DateTime? ResponseStartDate { get; set; }  // Response received date range
    public DateTime? ResponseEndDate { get; set; }
    public string SortingColumn { get; set; }         // "Franchisee", "Customer", "Rating", etc.
    public long? SortingOrder { get; set; }           // 0 = Asc, 1 = Desc
    public int? Response { get; set; }                // 1 = has response, null/other = no response
    public int ResponseFrom { get; set; }             // 0 = all, 1 = Google, 2 = GatherUp/ReviewPush
}
```

---

#### CustomerFeedbackReportListModel.cs
**Purpose**: Paginated report collection with filter and pagination metadata.

```csharp
[NoValidatorRequired]
public class CustomerFeedbackReportListModel
{
    public IEnumerable<CustomerFeedbackReportViewModel> Collection { get; set; }
    public CustomerFeedbackReportFilter Filter { get; set; }      // Echo back filter for state preservation
    public PagingModel PagingModel { get; set; }                  // PageNumber, PageSize, TotalPages, TotalRecords
}
```

---

### Business Logic Models

#### CreateBusinessForReviewModel.cs
**Purpose**: GatherUp business creation (commented out in service — may be legacy).

```csharp
public class CreateBusinessForReviewModel
{
    public string businessName { get; set; }
    public string businessOwnerEmail { get; set; }
    public string city { get; set; }
    public string clientId { get; set; }
    public string country { get; set; }
    public string hash { get; set; }
    public string phone { get; set; }
    public string state { get; set; }
    public string streetAddress { get; set; }
    public string zip { get; set; }
    public string package { get; set; }               // "basic", "premium", etc.
    public int businessId { get; set; }
    public int page { get; set; }
}
```

---

#### GetBusinessReviewModel.cs
**Purpose**: GatherUp business fetch (commented out — may be legacy).

```csharp
public class GetBusinessReviewModel
{
    // public int businessId { get; set; }            // Commented out
    public string clientId { get; set; }
    public int page { get; set; }
    public string hash { get; set; }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application.ViewModel](../../Application/ViewModel/.context/CONTEXT.md)** — `PagingModel`
- **[Core.Application.Attribute](../../Application/Attribute/.context/CONTEXT.md)** — `[NoValidatorRequired]`, `[DownloadField]`

### External Dependencies
- **System**: Standard C# primitives (`DateTime`, `string`, `long`, `int`, `bool`, `decimal`)
- **System.Collections.Generic**: `ICollection<T>`, `IEnumerable<T>`

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Naming Conventions

**GatherUp API Models**: Use camelCase (`customerEmail`, `businessId`) to match JSON serialization format without `[JsonProperty]` attributes.

**Internal Models**: Use PascalCase (`CustomerId`, `ResponseId`) following C# conventions.

### Field Purpose Ambiguity

**ReviewAPIResponseModel** mixes concerns:
- **API Response Fields**: `firstName`, `lastName`, `email`, `errorCode`, `errorMessage`
- **Service-Populated Fields**: `CustomerId` (set after API call), `ReviewSystemRecordId`, `DataPacket`, `IsvalidName`, `IsQueued`

**Recommendation**: Split into `GatherUpApiResponse` (pure API) and `ReviewRequestResult` (service-enriched).

### Rating Scale Mapping

**GatherUp**:
- API: `recommend` (0-10 integer), `rating` (1-10 integer)
- Storage: `Recommend` (double), `Rating` (decimal)
- Display: `Rating / 2` → 0-5 stars

**ReviewPush**:
- API: `Rating` (string: "1" to "5")
- Storage: `Rating` (decimal)
- Display: Direct (1-5 stars)

**Google**:
- API: Assumed 1-5 stars
- Storage: `Rating` (decimal)
- Display: Direct

### Email Extraction Pattern

ReviewPush embeds email in `Url`:
```csharp
// API returns
Url = "mailto:john.doe@example.com"

// Service parses
var parts = model.Url.Split(new[] { "mailto:" }, StringSplitOptions.RemoveEmptyEntries);
model.Email = parts[0];  // "john.doe@example.com"
```

### Filter Default Values

```csharp
// Unset filters use sentinel values
filter.FranchiseeId = 0;    // 0 = all franchisees (not null to avoid database NULL semantics)
filter.CustomerId = 0;       // 0 = all customers
filter.ResponseFrom = 0;     // 0 = all sources, 1 = Google, 2 = GatherUp/ReviewPush
filter.Response = null;      // null = all (with or without response)
```

### Validation Attributes

`[NoValidatorRequired]`: Disables automatic model validation (used for filter/search models where empty values are valid).

`[DownloadField(Required = false)]`: Excludes field from Excel export (e.g., internal IDs not relevant for business users).

### Legacy Fields

**CreateBusinessForReviewModel**, **GetBusinessReviewModel**: Related to GatherUp business management API — all usage is commented out in `CustomerFeedbackService.cs`. May be deprecated or future feature.

**FeedbackRequestModel.hash**: ReviewPush doesn't use hash authentication (token-based instead), but field remains for consistency with GatherUp models.
<!-- END CUSTOM SECTION -->
