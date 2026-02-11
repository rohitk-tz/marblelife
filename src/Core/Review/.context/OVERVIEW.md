<!-- AUTO-GENERATED: Header -->
# Review Module
> Customer feedback collection and review management system integrating with multiple external review platforms (GatherUp, ReviewPush, Google) for Marblelife franchise operations
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Review module is Marblelife's **customer feedback orchestration engine** — think of it as a "review aggregator + autopilot" that automatically solicits customer reviews after service completion and consolidates incoming feedback from multiple platforms into a unified reporting system.

### Why This Module Exists

When a Marblelife franchisee completes a service, the business wants to:
1. **Automatically** ask the customer for feedback (without manual intervention)
2. **Collect** reviews from multiple sources (GatherUp, Google, ReviewPush)
3. **Track** which customers received requests and which responded
4. **Report** on feedback trends across franchises with filtering and approval workflows

This module bridges the gap between Marblelife's internal CRM (Sales/Customer modules) and external review platforms, ensuring:
- No customer falls through the cracks (automated polling agents)
- All reviews are centralized regardless of source
- Management can audit and approve reviews before marketing usage

### Real-World Workflow

**Scenario**: A customer just had their marble floors cleaned and the invoice is finalized.

1. **Trigger** (Sales module): `ICustomerFeedbackService.TriggerEmail()` is called with customer details
2. **Validation**: System checks if franchisee has review feedback enabled and customer has valid first + last name
3. **Queue**: A `CustomerFeedbackRequest` is created with `IsQueued = true`
4. **Delivery** (15 min later): `SendFeedBackRequestPollingAgent` runs, calls GatherUp API to send review request email
5. **Customer Action**: Customer receives email, clicks link, submits 5-star review with comments
6. **Collection** (1 hour later): `GetCustomerFeedbackService` polls GatherUp API, finds new review, creates `CustomerFeedbackResponse`
7. **Linking**: System matches review to original request by email + date, sets `CustomerFeedbackRequest.ResponseId`
8. **Reporting**: Manager runs feedback report, sees review alongside other Google/ReviewPush reviews, approves for marketing website

### Key Concepts

- **Multi-Source Reviews**: The module doesn't "own" the review data — it aggregates from 3 external systems (GatherUp, ReviewPush, Google) into a unified view. Each source has its own API, data format, and timing.

- **Request/Response Lifecycle**: Reviews aren't just stored — they're linked back to the original request that triggered them. This enables "conversion rate" analytics (% of requests that received responses).

- **Polling Agents**: Instead of real-time webhooks, the system uses scheduled background jobs to:
  - Send queued requests (`ISendFeedBackRequestPollingAgent`)
  - Fetch new reviews (`IGetCustomerFeedbackService`)
  
  This design is resilient to API downtime (will retry on next poll) but introduces latency (reviews appear within 1 hour).

- **Audit Workflow**: All reviews have an `AuditActionId` status. Unapproved reviews aren't sent to the marketing website — this prevents negative or inappropriate reviews from appearing publicly.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Triggering a Review Request (from Sales Module)

```csharp
// After invoice finalization
var customer = _customerRepository.Get(customerId);
var franchisee = _franchiseeRepository.Get(franchiseeId);
var customerModel = new CustomerCreateEditModel 
{ 
    ContactPerson = "Doe, John",  // Must have comma or space-separated first/last
    // ... other fields
};

// Trigger review request
var result = _customerFeedbackService.TriggerEmail(
    customer, 
    customerModel, 
    franchiseeId,
    qbInvoiceId: "INV-12345",
    customerEmail: "john.doe@example.com",
    marketingClassId: (long)MarketingClassType.Residential
);

if (result.errorCode == 0) 
{
    Console.WriteLine($"Review request queued. Record ID: {result.ReviewSystemRecordId}");
    // Request will be sent within 15 minutes by polling agent
}
else 
{
    Console.WriteLine($"Error: {result.errorMessage}");
}
```

### Generating Feedback Reports

```csharp
// Report: All reviews for Franchisee #42 in December with ratings >= 4 stars
var filter = new CustomerFeedbackReportFilter 
{
    FranchiseeId = 42,
    StartDate = new DateTime(2024, 12, 1),
    EndDate = new DateTime(2024, 12, 31),
    ResponseFrom = 0,  // 0 = all sources, 1 = Google, 2 = GatherUp/ReviewPush
    Response = 1       // 1 = only show requests with responses
};

var reportData = _customerFeedbackReportService.GetCustomerFeedbackList(
    filter, 
    pageNumber: 1, 
    pageSize: 50
);

foreach (var review in reportData.Collection) 
{
    Console.WriteLine($"{review.Customer} ({review.CustomerEmail}) - {review.Rating} stars");
    Console.WriteLine($"From: {review.From} | Status: {review.AuditStatus}");
    Console.WriteLine($"Review: {review.ResponseContent}");
    Console.WriteLine($"Received: {review.ResponseReceivedDate}, Synced: {review.ResponseSyncingDate}");
    Console.WriteLine("---");
}

// Pagination info
Console.WriteLine($"Page {reportData.PagingModel.PageNumber} of {reportData.PagingModel.TotalPages}");
Console.WriteLine($"Total reviews: {reportData.PagingModel.TotalRecords}");
```

### Approving/Rejecting Reviews

```csharp
// Approve a review for marketing website use
bool success = _customerFeedbackReportService.ManageCustomerFeedbackStatus(
    isAccept: true,
    customerId: 123,
    id: 456,  // Feedback ID
    fromTable: "CustomerFeedbackResponse"  // or "ReviewPushCustomerFeedback" or "CustomerFeedbackRequest"
);

if (success) 
{
    Console.WriteLine("Review approved and will appear on marketing website");
}
```

### Downloading Excel Report

```csharp
string fileName;
bool success = _customerFeedbackReportService.DownloadFeedbackReport(filter, out fileName);

if (success) 
{
    Console.WriteLine($"Excel report generated: {fileName}");
    // Return file to user for download
}
```

### Polling Agent Setup (Scheduler Configuration)

```csharp
// In your job scheduler (e.g., Hangfire, Quartz.NET, Windows Task Scheduler)

// Send queued feedback requests every 15 minutes
Schedule.Every(15).Minutes().Do(() => 
{
    var agent = DependencyResolver.Resolve<ISendFeedBackRequestPollingAgent>();
    agent.SendFeedback();
});

// Fetch new reviews every 60 minutes
Schedule.Every(60).Minutes().Do(() => 
{
    var service = DependencyResolver.Resolve<IGetCustomerFeedbackService>();
    service.GetFeedbackResponse();
});
```

### Configuration Settings (appsettings.json)

```json
{
  "ReviewSettings": {
    "ClientId": "marblelife-client-123",
    "ReviewApiKey": "your-gatherup-api-key",
    "ReviewPushApiKey": "your-reviewpush-api-key",
    "SendFeedbackEnabled": true,
    "GetFeedbackEnabled": true,
    "IsReviewPushParseAllDataOn": false,
    "IsFromQA": false
  }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Interface | Method | Description |
|-----------|--------|-------------|
| **ICustomerFeedbackService** | `TriggerEmail()` | Initiate review request after service completion; validates customer name, creates review system record, queues request |
| | `GetCustomer()` | Fetch customer details from GatherUp API by external customer ID |
| | `SendFeedbackRequest()` | Trigger GatherUp to send review request email to customer |
| | `GetFeedback()` | Fetch reviews from ReviewPush API within date range |
| | `GetFeedbackForAllData()` | Bulk fetch all available reviews from ReviewPush |
| **ICustomerFeedbackReportService** | `GetCustomerFeedbackList()` | Generate paginated feedback report with filtering (franchisee, customer, date, source, response status) and sorting |
| | `GetCustomerFeedbackDetail()` | Fetch single feedback record with full details by ID and source |
| | `DownloadFeedbackReport()` | Export feedback report to Excel file based on filter criteria |
| | `ManageCustomerFeedbackStatus()` | Approve or reject review for marketing website display (updates AuditActionId) |
| **IGetCustomerFeedbackService** | `GetFeedbackResponse()` | Polling agent that fetches new reviews from external APIs and syncs to local database; matches reviews to original requests |
| **ISendFeedBackRequestPollingAgent** | `SendFeedback()` | Polling agent that sends queued feedback requests via GatherUp API; updates queue status on success/failure |
| **ICustomerFeedbackFactory** | `CreateDomain()` | Factory methods to create domain entities (CustomerReviewSystemRecord, CustomerFeedbackRequest, CustomerFeedbackResponse) from API responses |
| | `CreateModel()` | Factory methods to create API request models (CreateCustomerForReviewModel, GetCustomerForReviewModel, UpdateCustomerRecordViewModel) |
| **ICustomerFeedbackReportFactory** | `CreateViewModel()` | Transform domain entities into report view models with normalized rating scales and source identification |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Reviews Not Being Sent

**Symptom**: Feedback requests are created but customers never receive emails.

**Diagnosis**:
1. Check `SendFeedbackEnabled` setting — must be `true`
2. Verify polling agent is running: Look for logs from `SendFeedBackRequestPollingAgent`
3. Query database: `SELECT * FROM CustomerFeedbackRequest WHERE IsQueued = 1 AND IsSystemGenerated = 0`
   - If many queued requests exist, polling agent may not be running
4. Check franchisee setting: `Franchisee.IsReviewFeedbackEnabled` must be `true`
5. Verify GatherUp API key: Invalid key causes all requests to fail silently

**Fix**:
- Ensure polling agent is scheduled (every 15 minutes recommended)
- Manually trigger: `_sendFeedBackRequestPollingAgent.SendFeedback()`
- Check API key in settings

---

### Reviews Fetched but Not Linked to Requests

**Symptom**: `CustomerFeedbackResponse` records exist but `CustomerFeedbackRequest.ResponseId` is null.

**Diagnosis**:
- Linking logic in `GetCustomerFeedbackService.SavingData()` matches by:
  1. Email (case-insensitive)
  2. Review date >= Request date
  3. Picks most recent request if multiple matches
- If customer changed email between request and response, link fails
- If review date is before request date (rare), link fails

**Fix**:
- Check email consistency: `SELECT r.CustomerEmail, resp.Url FROM CustomerFeedbackRequest r JOIN CustomerFeedbackResponse resp ...`
- Manually link: `UPDATE CustomerFeedbackRequest SET ResponseId = {respId} WHERE Id = {reqId}`

---

### Rating Display Mismatch

**Symptom**: Reports show different ratings than external systems.

**Root Cause**: Rating scale conversion inconsistency
- GatherUp API returns `Recommend` (0-10 scale)
- Report displays stars (0-5 scale) via `Rating = Recommend / 2`
- ReviewPush API returns `Rating` (1-5 string)

**Fix**:
- Always use `CustomerFeedbackReportViewModel.Rating` for display (normalized to 0-5)
- Don't display raw `Recommend` or `CustomerFeedbackResponse.Rating` directly

---

### Duplicate Reviews Appearing

**Symptom**: Same review appears multiple times in reports.

**Root Cause**: Report aggregates 5 data sources without deduplication.

**Diagnosis**:
- Check `FromTable` and `From` fields — duplicates should have different values
- If same `FromTable` + `Id`, polling agent may have run twice during sync

**Fix**:
- Add unique constraint on `CustomerFeedbackResponse.ReviewId` (external system's review ID)
- Modify report query to use `DISTINCT` on external review ID

---

### QA Environment Email Issues

**Symptom**: Reviews not appearing in QA/test environment.

**Root Cause**: `IsFromQA = true` replaces emails with `@yopmail.com`, breaking customer matching.

**Fix**:
- Ensure test customers also have `@yopmail.com` emails in database
- Or set `IsFromQA = false` in QA environment settings
- Check email extraction logic in `GetCustomerFeedbackService.IsFromDataBaseOrNot()`

---

### Name Validation Failures

**Symptom**: `TriggerEmail()` returns error "Can't create customer without full Name".

**Root Cause**: Customer name can't be split into first + last name.

**Examples of Failing Names**:
- Single name: "Madonna"
- Company name: "ABC Corp"
- Empty ContactPerson with empty Customer.Name

**Fix**:
- Ensure ContactPerson or Customer.Name contains comma or space-separated first/last: "Doe, John" or "John Doe"
- Handle company customers differently (use `marketingClassId` check)
- Fallback: Set default "FirstName" + "Unknown" for companies
<!-- END CUSTOM SECTION -->
