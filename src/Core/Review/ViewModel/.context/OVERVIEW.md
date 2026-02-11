<!-- AUTO-GENERATED: Header -->
# Review/ViewModel
> Data Transfer Objects (DTOs) for API integration, report display, and query filtering
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The ViewModel subfolder contains 14 **lightweight data containers** (POCOs) that serve as the "messenger molecules" of the Review module. These DTOs carry data between:
- **Services ↔ External APIs** (request/response models for GatherUp and ReviewPush)
- **Services ↔ Controllers** (report view models for web UI)
- **Controllers ↔ Users** (filter models for search criteria)

Think of ViewModels as the "diplomatic envelopes" — they define the exact format for sending data out (API requests) and receiving data in (API responses, UI queries), ensuring all parties speak the same language.

### Categories

**1. API Request Models** (What We Send)
- `CreateCustomerForReviewModel` — Create customer in GatherUp
- `UpdateCustomerRecordViewModel` — Update customer in GatherUp
- `GetCustomerForReviewModel` — Fetch customer from GatherUp
- `FeedbackRequestModel` — Fetch reviews from ReviewPush

**2. API Response Models** (What We Receive)
- `ReviewAPIResponseModel` — GatherUp API responses (success/error)
- `FeedbackResponseViewModel` — Single GatherUp review
- `FeedbackResponseListModel` — GatherUp review collection
- `ReviewPushResponseViewModel` — Single ReviewPush review
- `ReviewPushResponseListModel` — ReviewPush review collection

**3. Report Models** (What Users See)
- `CustomerFeedbackReportViewModel` — Unified report row (all sources)
- `CustomerFeedbackReportListModel` — Paginated report collection
- `CustomerFeedbackReportFilter` — Search/filter criteria

**4. Legacy Models** (Deprecated/Unused)
- `CreateBusinessForReviewModel` — GatherUp business creation (commented out)
- `GetBusinessReviewModel` — GatherUp business fetch (commented out)

### Key Characteristics

**Anemic POCOs**: No behavior, only properties — logic lives in services and factories.

**Camel vs Pascal Case**: API models use camelCase (`customerEmail`) to match external JSON; internal models use PascalCase (`CustomerId`).

**Mixed Responsibility**: Some models (e.g., `ReviewAPIResponseModel`) contain both API response fields AND service-populated fields — a code smell indicating coupling between layers.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating API Request Models

```csharp
// Factory creates model for GatherUp customer creation
var model = new CreateCustomerForReviewModel
{
    businessId = 12345,
    clientId = "marblelife-client-id",
    customerEmail = "john.doe@example.com",
    customerFirstName = "John",
    customerLastName = "Doe",
    customerPhone = "555-1234",
    sendFeedbackRequest = 0,                     // Don't send immediately
    communicationPreference = "email",
    customerCustomId = internalCustomerId,
    hash = GenerateSHA256Hash(...)               // Computed by service
};

// Serialize and send to API
var json = JsonSerializer.Serialize(model);
var response = webClient.UploadString("https://app.gatherup.com/api/customer/create", "POST", json);
```

### Parsing API Response Models

```csharp
// Receive JSON from GatherUp API
var jsonResponse = webClient.DownloadString("https://app.gatherup.com/api/customer/get");
var apiResponse = JsonSerializer.Deserialize<ReviewAPIResponseModel>(jsonResponse);

if (apiResponse.errorCode == 0) 
{
    Console.WriteLine($"Customer: {apiResponse.firstName} {apiResponse.lastName}");
    Console.WriteLine($"Email: {apiResponse.email}");
}
else 
{
    Console.WriteLine($"Error {apiResponse.errorCode}: {apiResponse.errorMessage}");
}
```

### Building Report Filters

```csharp
// User submits search form
var filter = new CustomerFeedbackReportFilter
{
    FranchiseeId = selectedFranchiseeId ?? 0,    // 0 = all
    Text = searchBox.Text,                        // "john" searches name/email
    StartDate = datePickerStart.Value,
    EndDate = datePickerEnd.Value,
    ResponseFrom = dropdownSource.SelectedValue,  // 0=all, 1=Google, 2=GatherUp
    Response = checkboxHasResponse.Checked ? 1 : (int?)null,
    SortingColumn = "Rating",
    SortingOrder = (long)SortingOrder.Desc
};

var report = _reportService.GetCustomerFeedbackList(filter, pageNumber: 1, pageSize: 50);
```

### Displaying Report Results

```csharp
// Bind to data grid
dataGridView.DataSource = report.Collection;

// Custom rendering
foreach (var row in report.Collection)
{
    var starRating = new string('⭐', (int)Math.Ceiling(row.Rating));  // Convert 4.5 → "⭐⭐⭐⭐⭐"
    var sourceIcon = row.From switch 
    {
        "Google" => "🔍",
        "GatherUp" => "📧",
        "ReviewSystem" => "🌟",
        _ => "❓"
    };
    
    Console.WriteLine($"{sourceIcon} {row.Customer} - {starRating} ({row.Rating:F1})");
    Console.WriteLine($"   \"{row.ResponseContent}\"");
    Console.WriteLine($"   Status: {row.AuditStatus} | From: {row.FromTable}");
}

// Pagination controls
lblPageInfo.Text = $"Page {report.PagingModel.PageNumber} of {report.PagingModel.TotalPages} ({report.PagingModel.TotalRecords} total)";
```

### Parsing ReviewPush Responses

```csharp
// ReviewPush API response
var reviewPushResponse = JsonSerializer.Deserialize<ReviewPushResponseListModel>(json);

if (reviewPushResponse.result == "SUCCESS")
{
    foreach (var review in reviewPushResponse.info)
    {
        // Extract email from URL
        if (review.Url.Contains("mailto:"))
        {
            var parts = review.Url.Split(new[] { "mailto:" }, StringSplitOptions.RemoveEmptyEntries);
            review.Email = parts[0];
        }
        
        // Parse rating string
        int rating = int.Parse(review.Rating);  // "5" → 5
        
        Console.WriteLine($"{review.Name} - {rating} stars");
        Console.WriteLine($"Review: {review.Review}");
        Console.WriteLine($"Date: {review.Rp_date:yyyy-MM-dd}");
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Model | Direction | Purpose |
|-------|-----------|---------|
| **CreateCustomerForReviewModel** | Outbound → GatherUp | Create customer in review system |
| **UpdateCustomerRecordViewModel** | Outbound → GatherUp | Update customer details |
| **GetCustomerForReviewModel** | Outbound → GatherUp | Fetch customer by ID |
| **FeedbackRequestModel** | Outbound → ReviewPush | Fetch reviews by date range |
| **ReviewAPIResponseModel** | Inbound ← GatherUp | API response with error code |
| **FeedbackResponseViewModel** | Inbound ← GatherUp | Single review details |
| **FeedbackResponseListModel** | Inbound ← GatherUp | Paginated review collection |
| **ReviewPushResponseViewModel** | Inbound ← ReviewPush | Single review details |
| **ReviewPushResponseListModel** | Inbound ← ReviewPush | Review collection |
| **CustomerFeedbackReportViewModel** | Internal | Unified report row (all sources) |
| **CustomerFeedbackReportListModel** | Internal | Paginated report with filters |
| **CustomerFeedbackReportFilter** | Inbound ← UI | Search/filter criteria |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### JSON Serialization Errors

**Symptom**: `JsonSerializationException` when calling GatherUp API

**Cause**: Property names don't match API expectations

**Fix**: Ensure camelCase for GatherUp models:
```csharp
// CORRECT (camelCase)
public string customerEmail { get; set; }

// WRONG (PascalCase)
public string CustomerEmail { get; set; }
```

Alternatively, use `[JsonProperty]` attributes:
```csharp
[JsonProperty("customerEmail")]
public string CustomerEmail { get; set; }
```

---

### Rating Scale Confusion

**Symptom**: Report displays "10 stars" instead of "5 stars"

**Cause**: Using raw `Recommend` value (0-10 scale) instead of `Rating` (0-5 scale)

**Fix**: Always display `CustomerFeedbackReportViewModel.Rating` (pre-converted to 0-5):
```csharp
// Factory converts
Rating = (decimal)(response.Recommend / 2)  // 10 → 5.0
```

---

### Email Extraction Failures (ReviewPush)

**Symptom**: `IndexOutOfRangeException` when parsing `ReviewPushResponseViewModel.Url`

**Cause**: Malformed URL (missing "mailto:" or empty after split)

**Fix**: Add defensive checks:
```csharp
if (model.Url?.Contains("mailto:") == true)
{
    var parts = model.Url.Split(new[] { "mailto:" }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
        email = parts[0];
}
```

---

### Filter Not Working (ResponseFrom)

**Symptom**: Setting `filter.ResponseFrom = 1` (Google) still shows GatherUp reviews

**Cause**: Service layer logic checks multiple conditions:
```csharp
if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)  // GatherUp OR All
    // Fetch GatherUp reviews
if (filter.ResponseFrom == 1 || filter.ResponseFrom == 0)  // Google OR All
    // Fetch Google reviews
```

**Expected Behavior**: 
- `ResponseFrom = 0` → All sources
- `ResponseFrom = 1` → Google only
- `ResponseFrom = 2` → GatherUp/ReviewPush only

---

### Null Reference Exception (PagingModel)

**Symptom**: `NullReferenceException` accessing `report.PagingModel.TotalPages`

**Cause**: Service didn't populate PagingModel

**Fix**: Ensure service creates PagingModel:
```csharp
return new CustomerFeedbackReportListModel
{
    Collection = reviews,
    Filter = filter,
    PagingModel = new PagingModel(pageNumber, pageSize, totalRecords)
};
```
<!-- END CUSTOM SECTION -->
