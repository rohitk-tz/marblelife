<!-- AUTO-GENERATED: Header -->
# Review Module — Module Context
**Version**: 64667c5c8c4ab9b3d804e48deb14e9b70895fc42
**Generated**: 2025-01-19T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Review module orchestrates the entire customer feedback lifecycle for the Marblelife franchise system. It manages the integration with external review platforms (GatherUp/GetFiveStars and ReviewPush), automates feedback request delivery via polling agents, synchronizes incoming reviews from multiple sources (GatherUp API, Google API, ReviewPush), tracks response/request linkage, generates comprehensive reports with filtering/sorting, and provides audit status management for review approval workflows.

This module acts as a **Review Aggregation Hub** — consolidating customer feedback from disparate external systems, normalizing the data into a unified domain model, and providing business intelligence through reporting and analytics.

### Design Patterns
- **Factory Pattern**: `ICustomerFeedbackFactory` and `ICustomerFeedbackReportFactory` encapsulate complex object creation, particularly when transforming between API response models, domain entities, and view models across multiple review platforms.
- **Repository Pattern**: All data access goes through `IRepository<T>` from the Unit of Work, ensuring transactional consistency when saving feedback requests, responses, and review system records.
- **Polling Agent Pattern**: `ISendFeedBackRequestPollingAgent` and `IGetCustomerFeedbackService` run on scheduled intervals to asynchronously send queued feedback requests and fetch new reviews, decoupling the review workflow from synchronous user actions.
- **Service Layer Pattern**: Clear separation between service interfaces (business logic orchestration) and implementation details, with factories handling transformations.
- **Multi-Source Aggregation**: The module normalizes data from three distinct sources (GatherUp, Google, ReviewPush) into a unified `CustomerFeedbackResponse` and `AllCustomerFeedback` model, using flags like `IsFromNewReviewSystem`, `IsFromGoogleAPI`, `IsFromSystemReviewSystem` to track origin.

### Data Flow
1. **Feedback Request Initiation**: 
   - `TriggerEmail()` is invoked when a customer completes a service (typically from Sales module)
   - Customer name is validated (must have first + last name)
   - If franchisee has review feedback enabled, a `CustomerReviewSystemRecord` is created to link the customer to the external review platform
   - A `CustomerFeedbackRequest` is queued with `IsQueued = true`

2. **Request Delivery (Polling Agent)**:
   - `SendFeedBackRequestPollingAgent.SendFeedback()` runs periodically
   - Queries all `CustomerFeedbackRequest` records where `IsQueued = true` and `IsSystemGenerated = false`
   - For each request: fetches customer from GatherUp API, calls `SendFeedbackRequest()` to trigger email
   - On success, sets `IsQueued = false`

3. **Response Collection (Polling Agent)**:
   - `GetCustomerFeedbackService.GetFeedbackResponse()` runs periodically
   - Queries franchisees with review feedback enabled
   - Calls external APIs (`GetFeedback()` or `GetFeedbackForAllData()`)
   - Parses `ReviewPushResponseViewModel` data
   - Determines if review matches an existing customer (email lookup)
   - Creates `CustomerFeedbackResponse` or `ReviewPushCustomerFeedback` domain entity
   - Links response back to original request via `ResponseId` foreign key

4. **Report Generation**:
   - `GetCustomerFeedbackList()` aggregates data from:
     - `CustomerFeedbackRequest` (requests with or without responses)
     - `CustomerFeedbackResponse` (direct API responses)
     - `ReviewPushCustomerFeedback` (ReviewPush-specific responses)
   - Applies filters (franchisee, customer, date ranges, response status, source)
   - Uses `ISortingHelper` for dynamic column sorting
   - Returns paginated `CustomerFeedbackReportListModel`

5. **Audit Management**:
   - `ManageCustomerFeedbackStatus()` updates `AuditActionId` on feedback records
   - Controls visibility of reviews (accept/reject workflow)

### Critical Dependencies
- **External APIs**:
  - GatherUp (formerly GetFiveStars): `https://app.gatherup.com/api/*` — customer CRUD, feedback request sending
  - ReviewPush: `https://marblelife.com/ziplocator/API/getreviews/*` — bulk review fetching
  - Google API: Reviews fetched via separate integration (flagged with `IsFromGoogleAPI`)
  
- **Authentication**: SHA256 hash-based authentication for GatherUp API using `_settings.ReviewApiKey`
- **Configuration Settings**:
  - `ClientId`, `ReviewApiKey`, `ReviewPushApiKey`
  - `GetFeedbackEnabled`, `SendFeedbackEnabled` (feature flags)
  - `IsReviewPushParseAllDataOn`, `IsFromQA` (environment flags)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Domain Entities

```csharp
// Links a customer to their external review system ID
public class CustomerReviewSystemRecord : DomainBase
{
    public long CustomerId { get; set; }
    public long FranchiseeId { get; set; }
    public long? BusinessId { get; set; }  // GatherUp business ID
    public long ReviewSystemCustomerId { get; set; }  // External system's customer ID
}

// Tracks outbound feedback requests
public class CustomerFeedbackRequest : DomainBase
{
    public long FranchiseeSalesId { get; set; }  // Links to sale that triggered request
    public DateTime DateSend { get; set; }
    public string DataPacket { get; set; }  // JSON sent to API
    public bool IsQueued { get; set; }  // True = pending delivery
    public string CustomerEmail { get; set; }
    public long CustomerReviewSystemRecordId { get; set; }
    public long? ResponseId { get; set; }  // Links to CustomerFeedbackResponse when review received
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public string QBInvoiceId { get; set; }
    public bool IsSystemGenerated { get; set; }  // False = triggered by Mailtropolis, True = kiosk link
    public long AuditActionId { get; set; }  // Approval status
    public long? StatusId { get; set; }
}

// Stores inbound customer reviews from any source
public class CustomerFeedbackResponse : DomainBase
{
    public string ResponseContent { get; set; }  // Review text
    public DateTime DateOfReview { get; set; }
    public long? CustomerId { get; set; }
    public long? FranchiseeId { get; set; }
    public decimal Rating { get; set; }  // GatherUp uses 1-10 scale
    public double Recommend { get; set; }  // GatherUp recommendation score
    public bool ShowReview { get; set; }
    public long? FeedbackId { get; set; }  // External system's feedback ID
    public DateTime? DateOfDataInDataBase { get; set; }  // Sync timestamp
    public string Url { get; set; }  // Often contains customer email as "mailto:email@example.com"
    public long? ReviewId { get; set; }
    public bool IsFromNewReviewSystem { get; set; }  // ReviewPush vs GatherUp
    public bool IsFromGoogleAPI { get; set; }
    public bool IsFromSystemReviewSystem { get; set; }
    public long AuditActionId { get; set; }  // Approval workflow
    public bool IsSentToMarketingWebsite { get; set; }
}

// Unified view for all customer feedback (aggregates multiple sources)
public class AllCustomerFeedback : DomainBase
{
    public long? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime? ResponseReceivedDate { get; set; }
    public DateTime? ResponseSyncingDate { get; set; }
    public string ResponseContent { get; set; }
    public long? FranchiseeId { get; set; }
    public decimal? Rating { get; set; }
    public long? Recommend { get; set; }
    public string ContactPerson { get; set; }
    public string CustomerNameFromAPI { get; set; }
    public long AuditStatusId { get; set; }
    public string From { get; set; }  // "GatherUp", "Google", "ReviewSystem"
    public string FromTable { get; set; }  // Source table name
    public long? ReviewPushCustomerFeedbackId { get; set; }
    public long? CustomerFeedbackRequestId { get; set; }
    public long? CustomerFeedbackResponseId { get; set; }
    public bool IsOldReview { get; set; }
    public bool IsSentToMarketingWebsite { get; set; }
    public bool IsEmailSent { get; set; }
    public bool IsActive { get; set; }
}
```

### Key View Models

```csharp
// Report row model (unified view across all sources)
public class CustomerFeedbackReportViewModel
{
    public long Id { get; set; }
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public string Customer { get; set; }
    public string ContactPerson { get; set; }
    public string CustomerEmail { get; set; }
    public string Franchisee { get; set; }
    public DateTime? ResponseReceivedDate { get; set; }
    public DateTime? ResponseSyncingDate { get; set; }
    public decimal Rating { get; set; }
    public long ResponseId { get; set; }
    public string ResponseContent { get; set; }
    public string CustomerNameFromAPI { get; set; }
    public long AuditStatusId { get; set; }
    public string AuditStatus { get; set; }
    public string FromTable { get; set; }  // "CustomerFeedbackRequest", "CustomerFeedbackResponse", "ReviewPushCustomerFeedback"
    public string From { get; set; }  // "GatherUp", "Google", "ReviewSystem"
}

// Filter model for report queries
public class CustomerFeedbackReportFilter
{
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public string Text { get; set; }  // Search term (customer name, email, contact person)
    public DateTime? StartDate { get; set; }  // Request sent date range
    public DateTime? EndDate { get; set; }
    public DateTime? ResponseStartDate { get; set; }  // Response received date range
    public DateTime? ResponseEndDate { get; set; }
    public int? Response { get; set; }  // 1 = has response, null/other = no response
    public int ResponseFrom { get; set; }  // 0 = all, 1 = Google, 2 = GatherUp/ReviewSystem
}

// GatherUp API response wrapper
public class ReviewAPIResponseModel
{
    public string businessId { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public int errorCode { get; set; }  // 0 = success
    public string errorMessage { get; set; }
    public long CustomerId { get; set; }  // External system's ID
    public long ReviewSystemRecordId { get; set; }  // Internal linking ID
    public string DataPacket { get; set; }
    public bool IsvalidName { get; set; }
}

// ReviewPush API response
public class ReviewPushResponseViewModel
{
    public string Name { get; set; }
    public long? Id { get; set; }  // Review ID
    public long? Location_id { get; set; }  // Maps to ReviewPushAPILocation
    public string Rating { get; set; }  // String-encoded rating
    public DateTime? Rp_date { get; set; }  // Review date
    public DateTime? Db_date { get; set; }  // Database sync date
    public string Url { get; set; }  // Contains "mailto:email"
    public string Review { get; set; }  // Review text
    public string Email { get; set; }
    public string Taz_franchise_name { get; set; }  // Franchisee name override
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### ICustomerFeedbackService
Primary service for interacting with external review APIs and managing the feedback request/response lifecycle.

#### `TriggerEmail(Customer, CustomerCreateEditModel, franchiseeId, qBinvoiceId, customerEmail, marketingClassId)`
- **Input**: Customer entity, customer model, franchisee ID, QuickBooks invoice ID, email, marketing class ID
- **Output**: `ReviewAPIResponseModel` (contains error code, review system record ID)
- **Behavior**: 
  - Validates franchisee has review feedback enabled
  - Validates customer name (must have first + last name; splits on comma or space)
  - Creates or updates customer record in GatherUp via API
  - Creates `CustomerReviewSystemRecord` to link customer to external system
  - Creates `CustomerFeedbackRequest` with `IsQueued = true`
  - Returns review system record ID for linking
- **Side Effects**: Database writes (CustomerReviewSystemRecord, may create CustomerFeedbackRequest)
- **Error Handling**: Returns error code 101 if franchisee has feedback disabled; error message if name validation fails

#### `GetCustomer(customerId, clientId)`
- **Input**: External system's customer ID, client ID
- **Output**: `ReviewAPIResponseModel` with customer details (name, email)
- **Behavior**: Calls GatherUp API `GET /api/customer/get` with SHA256 hash authentication
- **Side Effects**: None (read-only API call)

#### `SendFeedbackRequest(customerId, clientId)`
- **Input**: External system's customer ID, client ID
- **Output**: `ReviewAPIResponseModel` with success/error status
- **Behavior**: Triggers GatherUp to send review request email to customer via `POST /api/customer/feedback/send`
- **Side Effects**: External email sent to customer
- **Error Handling**: Returns error code and message if API call fails

#### `GetFeedback(clientId, businessId, from, to)`
- **Input**: Client ID, business ID, date range (yyyy-MM-dd format)
- **Output**: `ReviewPushResponseListModel` containing collection of reviews
- **Behavior**: 
  - Calls ReviewPush API to fetch reviews within date range
  - Used by GetCustomerFeedbackService polling agent
- **Side Effects**: None (read-only)

#### `GetFeedbackForAllData()`
- **Input**: None (uses API key from settings)
- **Output**: `ReviewPushResponseListModel` with all available reviews
- **Behavior**: Bulk fetch of all reviews from ReviewPush (no date filter)
- **Side Effects**: None (read-only)

---

### ICustomerFeedbackReportService
Report generation and audit management for customer feedback.

#### `GetCustomerFeedbackList(filter, pageNumber, pageSize)`
- **Input**: `CustomerFeedbackReportFilter`, page number, page size
- **Output**: `CustomerFeedbackReportListModel` (paginated collection + filter + paging metadata)
- **Behavior**:
  - Aggregates data from 5 sources based on filter.ResponseFrom:
    1. `CustomerFeedbackRequest` (GatherUp requests via Mailtropolis)
    2. `CustomerFeedbackResponse` (GatherUp responses)
    3. `CustomerFeedbackResponse` (Google API responses, `IsFromGoogleAPI = true`)
    4. `CustomerFeedbackResponse` (ReviewPush system responses, `IsFromSystemReviewSystem = true`)
    5. `ReviewPushCustomerFeedback` (ReviewPush-specific table)
  - Filters by franchisee, customer, text search, date ranges, response status
  - Applies dynamic sorting via `ISortingHelper` (Franchisee, Customer, CustomerEmail, DateReceived, DateSend, Rating, ContactName)
  - Paginates results
- **Side Effects**: None (read-only)

#### `GetCustomerFeedbackDetail(responseId, isFromNewReviewSystem, isFromCustomerReviewTable)`
- **Input**: Response ID, source flags
- **Output**: `CustomerFeedbackReportViewModel` with full details
- **Behavior**: Fetches single feedback record based on source table (uses flags to determine which repository to query)
- **Side Effects**: None (read-only)

#### `DownloadFeedbackReport(filter, out fileName)`
- **Input**: `CustomerFeedbackReportFilter`
- **Output**: `bool` success status; fileName set to generated Excel file path
- **Behavior**: 
  - Uses same aggregation logic as `GetCustomerFeedbackList` (no pagination)
  - Creates Excel file via `IExcelFileCreator`
  - Returns file name for download
- **Side Effects**: Excel file written to disk

#### `ManageCustomerFeedbackStatus(isAccept, customerId, id, fromTable)`
- **Input**: Accept/reject flag, customer ID, feedback ID, source table name
- **Output**: `bool` success status
- **Behavior**:
  - Updates `AuditActionId` on feedback record
  - Based on `fromTable`, updates correct repository (CustomerFeedbackRequest, CustomerFeedbackResponse, ReviewPushCustomerFeedback)
  - Sets approved/rejected audit status
- **Side Effects**: Database write (audit status update)

---

### IGetCustomerFeedbackService
Polling agent that synchronizes reviews from external APIs into local database.

#### `GetFeedbackResponse()`
- **Input**: None (reads from settings)
- **Output**: `void`
- **Behavior**:
  - Checks `GetFeedbackEnabled` setting; exits if disabled
  - Determines mode: bulk fetch (`IsReviewPushParseAllDataOn`) or date-range fetch
  - Calls `GetFeedback()` or `GetFeedbackForAllData()` from ICustomerFeedbackService
  - For each `ReviewPushResponseViewModel`:
    - Extracts email from `Url` field (format: "mailto:email@example.com")
    - Matches to local customer by email lookup
    - Determines franchisee via `Location_id` or `Taz_franchise_name`
    - Creates `CustomerFeedbackResponse` or `ReviewPushCustomerFeedback` entity
    - Links response to original `CustomerFeedbackRequest` via `ResponseId` if matching email + date found
  - Environment-specific: If `IsFromQA`, replaces non-taazaa emails with "@yopmail.com"
- **Side Effects**: Database writes (CustomerFeedbackResponse, ReviewPushCustomerFeedback, updates CustomerFeedbackRequest.ResponseId)
- **Error Handling**: Logs errors per review but continues processing; uses transactions per review

---

### ISendFeedBackRequestPollingAgent
Polling agent that sends queued feedback requests to external review systems.

#### `SendFeedback()`
- **Input**: None (reads from database)
- **Output**: `void`
- **Behavior**:
  - Checks `SendFeedbackEnabled` setting; exits if disabled
  - Queries all `CustomerFeedbackRequest` where `IsQueued = true` and `IsSystemGenerated = false`
  - For each request:
    - Validates franchisee has review feedback enabled
    - Calls `GetCustomer()` to fetch external customer ID
    - Calls `SendFeedbackRequest()` to trigger review email
    - On success: sets `IsQueued = false`
    - On failure: leaves `IsQueued = true` for retry
  - Updates `DataPacket` field with request JSON
- **Side Effects**: Database writes (CustomerFeedbackRequest status updates), external emails sent
- **Error Handling**: Logs errors, uses transactions, rolls back on failure

---

### ICustomerFeedbackFactory
Factory for creating domain entities and view models from disparate data sources.

#### `CreateDomain(customer, franchisee, reviewSystemCustomerId)`
- **Output**: `CustomerReviewSystemRecord`
- **Usage**: Links customer to external review system

#### `CreateDomain(customerId, franchiseeSalesId, date, customerEmail, qBInvoiceId, isSystemGenerated, franchiseeId, reviewSystemRecordId)`
- **Output**: `CustomerFeedbackRequest`
- **Usage**: Creates queued feedback request

#### `CreateDomain(FeedbackResponseViewModel)` / `CreateDomain(ReviewPushResponseViewModel)`
- **Output**: `CustomerFeedbackResponse`
- **Usage**: Converts API response to domain entity; handles date parsing, rating normalization

#### `CreateDomainForFeedBack(ReviewPushResponseViewModel)`
- **Output**: `ReviewPushCustomerFeedback`
- **Usage**: Creates ReviewPush-specific feedback entity when email doesn't match existing customer

#### `CreateViewModel(CustomerFeedbackRequest)` / `CreateViewModel(CustomerFeedbackResponse)` / `CreateViewModel(ReviewPushCustomerFeedback)`
- **Output**: `CustomerFeedbackReportViewModel`
- **Usage**: Used by ICustomerFeedbackReportFactory; normalizes data from different sources for report display

---

### ICustomerFeedbackReportFactory
Factory for report view model creation.

#### `CreateViewModel(CustomerFeedbackRequest)` / `CreateViewModel(CustomerFeedbackResponse)` / `CreateViewModel(ReviewPushCustomerFeedback)`
- **Output**: `CustomerFeedbackReportViewModel`
- **Behavior**: Transforms domain entities into unified report format; handles rating conversion (GatherUp uses 0-10 recommend scale, converts to 0-5 star rating via `Recommend/2`)

#### `CreateModel(CustomerFeedbackRequest)`
- **Output**: `ReviewSystemRecordViewModel`
- **Usage**: Detailed view of single review request/response for drill-down reports

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Sales](../../Sales/.context/CONTEXT.md)** — Customer, FranchiseeSales entities; triggered by sales completion
- **[Core.Organizations](../../Organizations/.context/CONTEXT.md)** — Franchisee, Organization entities; ReviewPushCustomerFeedback, ReviewPushAPILocation
- **[Core.Application](../../Application/.context/CONTEXT.md)** — Repository pattern, Unit of Work, IClock, ISettings, ILogService, ISortingHelper
- **[Core.Notification](../../Notification/.context/CONTEXT.md)** — Review data may trigger marketing website updates

### External APIs
- **GatherUp (formerly GetFiveStars)**: `https://app.gatherup.com/api/*`
  - `/customer/create` — Create customer in review system
  - `/customer/update` — Update customer details
  - `/customer/get` — Fetch customer by ID
  - `/customer/feedback/send` — Trigger review request email
  - Authentication: SHA256 hash of API key + sorted parameters
  
- **ReviewPush**: `https://marblelife.com/ziplocator/API/getreviews/*`
  - `/sdate/{from}/edate/{to}/token/{apiKey}` — Fetch reviews by date range
  - `/token/{apiKey}` — Bulk fetch all reviews
  - Authentication: API token in URL

- **Google Reviews API** (indirect integration flagged via `IsFromGoogleAPI`)

### External Packages
- `System.Web.Script.Serialization.JavaScriptSerializer` — JSON serialization for API requests/responses
- `System.Net.WebClient` — HTTP API calls
- `System.Security.Cryptography.SHA256` — API authentication hash generation

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Key Gotchas
1. **Rating Scale Mismatch**: GatherUp uses 0-10 scale for `Recommend`, but reports display 0-5 stars. Conversion is `Recommend / 2`. ReviewPush uses direct star ratings (1-5).

2. **Email Extraction from URL**: ReviewPush API returns customer email embedded in `Url` field as `mailto:email@example.com`. Must parse to extract email for customer matching.

3. **Multi-Source Aggregation**: The same customer feedback may exist in 3 different tables:
   - `CustomerFeedbackRequest` (request sent, response not yet linked)
   - `CustomerFeedbackResponse` (response received from GatherUp or Google)
   - `ReviewPushCustomerFeedback` (response from ReviewPush for non-matched customers)
   
   Reports aggregate ALL three sources using `FromTable` and `From` fields to identify origin.

4. **Polling Agent Timing**: `SendFeedBackRequestPollingAgent` and `GetCustomerFeedbackService` must be scheduled appropriately to avoid:
   - Sending too many requests at once (could be rate-limited by GatherUp)
   - Missing reviews (GetFeedbackResponse uses 3-month lookback by default)

5. **Name Validation Edge Cases**: Customer creation fails if name can't be split into first + last. System tries comma-separated first (e.g., "Doe, John"), then space-separated. Single-name customers cannot be created.

6. **QA Environment Email Override**: When `IsFromQA = true`, all non-taazaa emails are replaced with `{localpart}@yopmail.com` for testing.

7. **Transaction Management**: Each review in bulk fetch uses individual transaction to prevent partial failures from rolling back entire batch.

### Performance Considerations
- **Report Query Optimization**: `GetCustomerFeedbackList` performs 5 separate queries (one per source) then aggregates in memory. For large datasets (>10K reviews), consider:
  - Adding composite indexes on (FranchiseeId, DateSend) and (FranchiseeId, DateOfReview)
  - Implementing stored procedure for unified query
  
- **Polling Agent Frequency**: Recommendation:
  - `SendFeedback()`: Every 15 minutes (balances responsiveness with API rate limits)
  - `GetFeedbackResponse()`: Every 1 hour (reviews don't appear immediately)

### Testing Notes
- Mock `ISettings` to control API keys and feature flags
- Mock `IClock` for time-based filtering tests
- Use `ReviewAPIResponseModel` with `errorCode = 0` for success scenarios
- Test email extraction with malformed URLs (missing "mailto:", multiple @ signs)
<!-- END CUSTOM SECTION -->
