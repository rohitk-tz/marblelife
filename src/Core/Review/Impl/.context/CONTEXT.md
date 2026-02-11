<!-- AUTO-GENERATED: Header -->
# Review/Impl — Module Context
**Version**: 64667c5c8c4ab9b3d804e48deb14e9b70895fc42
**Generated**: 2025-01-19T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Impl subfolder contains the **concrete implementations** of all Review module service interfaces. These 6 classes orchestrate the entire review lifecycle: API integration with external platforms (GatherUp, ReviewPush), business logic for request/response processing, report generation with complex multi-source aggregation, and polling agents for asynchronous workflow automation.

This is where the "business logic" lives — services coordinate between repositories, factories, external APIs, and configuration settings to implement the workflows defined by the interface contracts.

### Design Patterns
- **Service Layer Pattern**: Each service class implements a corresponding interface (e.g., `CustomerFeedbackService : ICustomerFeedbackService`)
- **Factory Pattern Integration**: Services delegate object creation to factories (`ICustomerFeedbackFactory`, `ICustomerFeedbackReportFactory`)
- **Repository Pattern**: All data access via `IRepository<T>` from Unit of Work
- **Dependency Injection**: Services receive all dependencies via constructor injection
- **Polling Agent Pattern**: `SendFeedBackRequestPollingAgent` and `GetCustomerFeedbackService` run on scheduled intervals
- **SHA256 Authentication**: GatherUp API requires hash-based auth (API key + sorted parameters)

### Key Files Overview

| File | Purpose | Lines of Code |
|------|---------|---------------|
| **CustomerFeedbackService.cs** | Core API integration for GatherUp (create customer, send request, fetch reviews) | ~460 |
| **GetCustomerFeedbackService.cs** | Polling agent that fetches reviews from APIs and syncs to database | ~340 |
| **SendFeedBackRequestPollingAgent.cs** | Polling agent that sends queued review requests | ~90 |
| **CustomerFeedbackReportService.cs** | Report generation with multi-source aggregation, filtering, sorting, Excel export | ~600+ |
| **CustomerFeedbackFactory.cs** | Creates domain entities and view models from disparate data sources | ~170 |
| **CustomerFeedbackReportFactory.cs** | Transforms domain entities into report view models | ~140 |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces Implementation

### CustomerFeedbackService.cs
**Implements**: `ICustomerFeedbackService`

**Core Methods**:
- `TriggerEmail()`: Validates customer name, creates/updates GatherUp customer record, queues feedback request
- `GetCustomer()`: Fetches customer details from GatherUp API with SHA256 auth
- `SendFeedbackRequest()`: Triggers GatherUp to email review request to customer
- `GetFeedback()` / `GetFeedbackForAllData()`: Fetches reviews from ReviewPush API

**External API Integration**:
```csharp
// GatherUp endpoints (formerly GetFiveStars)
const string CreateCustomerUrl = "https://app.gatherup.com/api/customer/create";
const string UpdateCustomerUrl = "https://app.gatherup.com/api/customer/update";
const string GetCustomerUrl = "https://app.gatherup.com/api/customer/get";
const string SendFeedbackUrl = "https://app.gatherup.com/api/customer/feedback/send";

// ReviewPush endpoint
string reviewPushUrl = $"https://marblelife.com/ziplocator/API/getreviews/sdate/{from}/edate/{to}/token/{apiKey}";
```

**Authentication**: SHA256 hash of `apiKey + sortedParameters` (e.g., `"apiKey" + "businessId123clientIdabccustomerEmailjohn@example.com..."`)

**Name Validation Logic**:
1. Try splitting by comma: `"Doe, John"` → firstName="John", lastName="Doe"
2. If no comma, split by space: `"John Doe"` → firstName="John", lastName="Doe"
3. If either name missing, return error

**Error Handling**: Returns `ReviewAPIResponseModel` with `errorCode` (0 = success, non-zero = error) and `errorMessage`

---

### GetCustomerFeedbackService.cs
**Implements**: `IGetCustomerFeedbackService`

**Core Method**: `GetFeedbackResponse()` — Polling agent that:
1. Checks `GetFeedbackEnabled` setting (exits if disabled)
2. Queries franchisees with `IsReviewFeedbackEnabled = true`
3. Calls `GetFeedback()` or `GetFeedbackForAllData()` based on `IsReviewPushParseAllDataOn` setting
4. For each `ReviewPushResponseViewModel`:
   - Extracts email from `Url` field (`mailto:email@example.com`)
   - Matches to internal customer via email lookup
   - Determines franchisee via `Location_id` or `Taz_franchise_name`
   - Creates `CustomerFeedbackResponse` (if matched) or `ReviewPushCustomerFeedback` (if unmatched)
   - Links response to original `CustomerFeedbackRequest` via `ResponseId` if found
5. Uses individual transactions per review (prevents partial batch rollback)

**Email Extraction**:
```csharp
if (model.Url.Contains("mailto:"))
{
    var urlSplit = model.Url.Split(new string[] { "mailto:" }, StringSplitOptions.RemoveEmptyEntries);
    email = urlSplit[0];  // "john.doe@example.com"
}
```

**QA Environment Logic**: If `IsFromQA = true`, replaces non-taazaa emails with `{localpart}@yopmail.com` for testing

**Matching Logic**:
```csharp
// Find most recent request with matching email and date <= review date
var matchedRequest = _customerFeedbcakRequestRepository.Table
    .Where(x => x.CustomerEmail.ToLower() == email.ToLower() 
                && x.DateSend <= reviewDate 
                && x.ResponseId == null)
    .OrderByDescending(x => x.DateSend)
    .FirstOrDefault();
```

---

### SendFeedBackRequestPollingAgent.cs
**Implements**: `ISendFeedBackRequestPollingAgent`

**Core Method**: `SendFeedback()` — Polling agent that:
1. Checks `SendFeedbackEnabled` setting (exits if disabled)
2. Queries `CustomerFeedbackRequest WHERE IsQueued = true AND IsSystemGenerated = false`
3. For each request:
   - Validates franchisee has `IsReviewFeedbackEnabled = true`
   - Fetches external customer ID via `GetCustomer()`
   - Calls `SendFeedbackRequest()` to trigger email
   - On success: sets `IsQueued = false`
   - On failure: leaves `IsQueued = true` for retry
4. Updates `DataPacket` field with request JSON (for debugging)
5. Uses transactions with rollback on failure

**Scheduling**: Should run every 15 minutes to balance responsiveness with API rate limits

---

### CustomerFeedbackReportService.cs
**Implements**: `ICustomerFeedbackReportService`

**Core Methods**:

#### `GetCustomerFeedbackList(filter, pageNumber, pageSize)`
Aggregates data from **5 distinct queries** based on filter criteria:

1. **CustomerFeedbackRequest** (GatherUp via Mailtropolis):
   ```csharp
   WHERE !IsQueued && !IsSystemGenerated && !IsFromNewReviewSystem
   ```

2. **ReviewPushCustomerFeedback** (unmatched ReviewPush reviews):
   ```csharp
   // Via CustomerFeedbackReportFactory.CreateViewModel(ReviewPushCustomerFeedback)
   ```

3. **CustomerFeedbackResponse** (GatherUp responses):
   ```csharp
   WHERE IsFromNewReviewSystem && !IsFromGoogleAPI && !IsFromSystemReviewSystem
   ```

4. **CustomerFeedbackResponse** (Google API responses):
   ```csharp
   WHERE IsFromGoogleAPI
   ```

5. **CustomerFeedbackResponse** (ReviewPush system responses):
   ```csharp
   WHERE IsFromSystemReviewSystem
   ```

**Filtering Logic**:
- `filter.ResponseFrom`: 0=all, 1=Google, 2=GatherUp/ReviewPush
- `filter.FranchiseeId`, `filter.CustomerId`: Exact match
- `filter.Text`: Searches `CustomerEmail`, `ContactPerson`, `Customer.Name` (case-sensitive LIKE)
- `filter.StartDate/EndDate`: Request sent date range
- `filter.ResponseStartDate/ResponseEndDate`: Response received date range
- `filter.Response`: 1=has response, null/other=no response

**Sorting**: Dynamic via `ISortingHelper` on columns: Franchisee, Customer, CustomerEmail, DateReceived, DateSend, Rating, ContactName

**Pagination**: In-memory (`Skip((page-1) * pageSize).Take(pageSize)`) AFTER aggregating all sources

#### `GetCustomerFeedbackDetail(responseId, isFromNewReviewSystem, isFromCustomerReviewTable)`
Fetches single record based on source flags:
- `isFromCustomerReviewTable == 1` → `ReviewPushCustomerFeedback`
- `isFromNewReviewSystem == true` → `CustomerFeedbackResponse` (ReviewPush)
- Otherwise → `CustomerFeedbackRequest`

#### `DownloadFeedbackReport(filter, out fileName)`
Same aggregation as `GetCustomerFeedbackList` but no pagination; exports to Excel via `IExcelFileCreator`

#### `ManageCustomerFeedbackStatus(isAccept, customerId, id, fromTable)`
Updates `AuditActionId` on feedback record:
```csharp
if (fromTable == "CustomerFeedbackRequest")
    _customerFeedbackRequestRepository.Get(id).AuditActionId = newStatusId;
else if (fromTable == "CustomerFeedbackResponse")
    _customerFeedbackResponseRepository.Get(id).AuditActionId = newStatusId;
else if (fromTable == "ReviewPushCustomerFeedback")
    _reviewPushCustomerFeedbackRepository.Get(id).AuditActionId = newStatusId;
```

---

### CustomerFeedbackFactory.cs
**Implements**: `ICustomerFeedbackFactory`

**Factory Methods**:

#### Domain Entity Creators
- `CreateDomain(customer, franchisee, reviewSystemCustomerId)` → `CustomerReviewSystemRecord`
- `CreateDomain(customerId, franchiseeSalesId, date, email, qbInvoiceId, isSystemGenerated, franchiseeId, recordId)` → `CustomerFeedbackRequest`
- `CreateDomain(FeedbackResponseViewModel)` → `CustomerFeedbackResponse` (GatherUp legacy)
- `CreateDomain(ReviewPushResponseViewModel)` → `CustomerFeedbackResponse` (ReviewPush)
- `CreateDomainForFeedBack(ReviewPushResponseViewModel)` → `ReviewPushCustomerFeedback`

#### View Model Creators
- `CreateModel(customer, responseModel, businessId)` → `CreateCustomerForReviewModel`
- `CreateModel(customerId, clientId)` → `GetCustomerForReviewModel`
- `CreateModel(email, firstName, lastName, businessId, clientId, customerId)` → `UpdateCustomerRecordViewModel`
- `CreateModel(clientId, businessId, from, to)` → `FeedbackRequestModel`

**Date Parsing Logic** (GatherUp API):
```csharp
// API returns: "2024-01-15T10:30:00"
var dateString = review.dateOfReview.Split('T');  // ["2024-01-15", "10:30:00"]
var date = dateString[0].ToString();              // "2024-01-15"
var reviewDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
```

**Rating Conversion** (ReviewPush):
```csharp
// ReviewPush API returns Rating as string: "5"
Rating = long.Parse(review.Rating),                    // Store as decimal
Recommend = long.Parse(review.Rating) + 5,            // Normalize to 0-10 scale
```

---

### CustomerFeedbackReportFactory.cs
**Implements**: `ICustomerFeedbackReportFactory`

**View Model Creators**:

#### `CreateViewModel(CustomerFeedbackRequest)`
Transforms request into report row:
- Uses linked `CustomerFeedbackResponse` if exists (`request.CustomerFeedbackResponse`)
- Falls back to `ResponseId` lookup if navigation property is null
- Calculates display rating: `Rating = Recommend / 2` (converts 0-10 to 0-5 stars)
- Sets `From = "GatherUp"`, `FromTable = "CustomerFeedbackRequest"`

#### `CreateViewModel(CustomerFeedbackResponse)`
Transforms response into report row:
- Extracts email from `Url` field (ReviewPush) or `Customer.CustomerEmails` (GatherUp)
- Determines source via flags:
  - `IsFromGoogleAPI = true` → `From = "Google"`
  - `IsFromNewReviewSystem = true` → `From = "ReviewSystem"`
  - Otherwise → `From = "GatherUp"`
- Sets `FromTable = "CustomerFeedbackResponse"`

#### `CreateViewModel(ReviewPushCustomerFeedback)`
Transforms ReviewPush-specific entity:
- Uses denormalized `Name`, `Email`, `FranchiseeName` fields
- Direct rating (no conversion needed)
- Sets `From = "ReviewSystem"`, `FromTable = "ReviewPushCustomerFeedback"`

#### `CreateModel(CustomerFeedbackRequest)` → `ReviewSystemRecordViewModel`
Detailed drill-down view:
- Shows request mode: `IsSystemGenerated ? "Kiosk Link" : "Mailtropolis Reviewability"`
- Formats dates as short strings: `DateSend.ToShortDateString()`
- Displays rating as `"{Recommend}/10"` format

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application](../../Application/.context/CONTEXT.md)** — `IRepository`, `IUnitOfWork`, `IClock`, `ISettings`, `ILogService`, `ISortingHelper`, `IExcelFileCreator`
- **[Core.Review.Domain](../Domain/.context/CONTEXT.md)** — All entity classes
- **[Core.Review.ViewModel](../ViewModel/.context/CONTEXT.md)** — All DTOs and view models
- **[Core.Sales](../../Sales/.context/CONTEXT.md)** — `Customer`, `FranchiseeSales` entities
- **[Core.Organizations](../../Organizations/.context/CONTEXT.md)** — `Franchisee`, `Organization`, `ReviewPushCustomerFeedback`, `ReviewPushAPILocation`

### External APIs
- **GatherUp API** (formerly GetFiveStars): `https://app.gatherup.com/api/*`
- **ReviewPush API**: `https://marblelife.com/ziplocator/API/getreviews/*`

### External Packages
- `System.Web.Script.Serialization.JavaScriptSerializer` — JSON serialization
- `System.Net.WebClient` — HTTP API calls
- `System.Security.Cryptography.SHA256` — API authentication

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Performance Considerations

**Report Aggregation**: `GetCustomerFeedbackList()` executes 5 separate SQL queries then aggregates in memory. For large datasets:
- Consider stored procedure to perform aggregation in database
- Add composite indexes: `(FranchiseeId, DateSend)`, `(FranchiseeId, DateOfReview)`
- Implement caching for frequently-accessed date ranges

**Polling Agent Frequency**:
- `SendFeedback()`: Every 15 minutes (balances responsiveness with API rate limits)
- `GetFeedbackResponse()`: Every 60 minutes (reviews don't appear immediately)

### Error Handling Patterns

**Transactional Safety**:
```csharp
// Per-review transaction (GetCustomerFeedbackService)
foreach (var review in reviews)
{
    _unitOfWork.StartTransaction();
    try 
    {
        // Process review
        _unitOfWork.SaveChanges();
    }
    catch (Exception e) 
    {
        _logService.Info($"Error processing review {review.Id}: {e.Message}");
        _unitOfWork.Rollback();
        // Continue to next review
    }
}
```

**Silent Failures**: Polling agents log errors but don't throw — prevents one bad review from halting entire batch

### Configuration Settings

**Required Settings**:
```csharp
_settings.ClientId              // GatherUp client ID
_settings.ReviewApiKey          // GatherUp API key
_settings.ReviewPushApiKey      // ReviewPush API token
_settings.SendFeedbackEnabled   // Feature flag for sending requests
_settings.GetFeedbackEnabled    // Feature flag for fetching reviews
_settings.IsReviewPushParseAllDataOn  // Bulk fetch vs date-range fetch
_settings.IsFromQA              // QA environment email override
```

### Testing Strategies

**Mock External APIs**:
```csharp
// Test CustomerFeedbackService without real API calls
var mockResponse = new ReviewAPIResponseModel 
{ 
    errorCode = 0, 
    CustomerId = 12345,
    firstName = "John",
    lastName = "Doe"
};
_mockWebClient.Setup(x => x.UploadString(It.IsAny<string>(), "POST", It.IsAny<string>()))
              .Returns(JsonSerializer.Serialize(mockResponse));
```

**Test Email Extraction**:
```csharp
[Test]
public void ExtractEmail_FromReviewPushUrl()
{
    var url = "mailto:john.doe@example.com";
    var parts = url.Split(new[] { "mailto:" }, StringSplitOptions.RemoveEmptyEntries);
    Assert.AreEqual("john.doe@example.com", parts[0]);
}
```

### Common Gotchas

1. **SHA256 Hash Order**: Parameters MUST be sorted alphabetically before hashing (e.g., `"businessId"` before `"clientId"`)

2. **Email Case Sensitivity**: Use `.ToLower()` for all email comparisons

3. **Date Parsing**: GatherUp API returns ISO 8601 format (`"2024-01-15T10:30:00"`); ReviewPush returns nullable `DateTime?`

4. **Rating Scale**: Always convert GatherUp's `Recommend` (0-10) to display rating via `/2`

5. **Null Navigation Properties**: Always check `if (request.CustomerFeedbackResponse == null)` before accessing — EF lazy loading may not be enabled
<!-- END CUSTOM SECTION -->
