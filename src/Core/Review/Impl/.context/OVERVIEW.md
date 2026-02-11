<!-- AUTO-GENERATED: Header -->
# Review/Impl
> Service implementations for review request/response lifecycle, external API integration, and reporting
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Impl subfolder is the **engine room** of the Review module — where all the business logic executes. These 6 service classes orchestrate the complete review workflow: integrating with external APIs (GatherUp, ReviewPush), running polling agents to automate asynchronous tasks, generating complex multi-source reports, and transforming data between API formats and internal domain models.

Think of this folder as the "conductors" of the review orchestra — they don't play instruments (that's the Domain entities) or define the song (that's the interfaces), but they coordinate all the musicians to create the symphony.

### Key Services

**1. CustomerFeedbackService** — The API Integration Hub
- Talks to GatherUp API for customer CRUD and feedback request triggering
- Talks to ReviewPush API for bulk review fetching
- Handles SHA256 authentication, JSON serialization, error responses

**2. GetCustomerFeedbackService** — The Review Collector (Polling Agent)
- Scheduled job that fetches new reviews from external APIs every hour
- Matches reviews to internal customers by email
- Creates `CustomerFeedbackResponse` or `ReviewPushCustomerFeedback` entities
- Links responses back to original requests

**3. SendFeedBackRequestPollingAgent** — The Request Sender (Polling Agent)
- Scheduled job that processes queued feedback requests every 15 minutes
- Calls GatherUp API to send review emails to customers
- Updates queue status on success/failure

**4. CustomerFeedbackReportService** — The Report Generator
- Aggregates data from 5 different sources (requests, responses, multiple APIs)
- Provides filtering, sorting, pagination
- Exports to Excel
- Manages audit approval workflow

**5. CustomerFeedbackFactory** — The Object Builder
- Creates domain entities from API responses
- Creates API request models from domain entities
- Handles date parsing, rating normalization, name splitting

**6. CustomerFeedbackReportFactory** — The Report Transformer
- Converts domain entities into unified report view models
- Normalizes ratings across different scales (0-10 → 0-5 stars)
- Identifies data source (GatherUp, Google, ReviewPush)

### Real-World Example

**Scenario**: Franchisee completes marble cleaning service, invoice finalized.

1. **Sales Module** calls `CustomerFeedbackService.TriggerEmail(customer, franchisee, ...)`
2. **CustomerFeedbackService** validates customer name, calls GatherUp API to create/update customer
3. **CustomerFeedbackFactory** creates `CustomerFeedbackRequest` with `IsQueued = true`
4. **15 minutes later**: `SendFeedBackRequestPollingAgent.SendFeedback()` runs
5. **Agent** queries queued requests, calls `SendFeedbackRequest()` for each
6. **GatherUp** sends email to customer with review link
7. **Customer** submits 5-star review on GatherUp platform
8. **1 hour later**: `GetCustomerFeedbackService.GetFeedbackResponse()` runs
9. **Agent** fetches new reviews via `GetFeedback()`, creates `CustomerFeedbackResponse`
10. **Agent** links response to original request by matching email + date
11. **Manager** runs report via `CustomerFeedbackReportService.GetCustomerFeedbackList()`
12. **ReportFactory** transforms entities into `CustomerFeedbackReportViewModel`
13. **Manager** approves review via `ManageCustomerFeedbackStatus(isAccept: true)`
14. **Marketing website** displays approved review

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage Examples

### Triggering Review Requests

```csharp
var feedbackService = new CustomerFeedbackService(unitOfWork, logService, settings, factory, clock, apiRecordService);

var result = feedbackService.TriggerEmail(
    customer: customer,
    customerModel: new CustomerCreateEditModel { ContactPerson = "Doe, John" },
    franchiseeId: 42,
    qBinvoiceId: "INV-2024-001",
    customerEmail: "john.doe@example.com",
    marketingClassId: (long)MarketingClassType.Residential
);

if (result.errorCode != 0) 
{
    Console.WriteLine($"Error: {result.errorMessage}");
}
```

### Running Polling Agents (Scheduler Setup)

```csharp
// Hangfire / Quartz.NET / Windows Task Scheduler
RecurringJob.AddOrUpdate(
    "send-review-requests",
    () => _sendFeedBackRequestPollingAgent.SendFeedback(),
    "*/15 * * * *"  // Every 15 minutes
);

RecurringJob.AddOrUpdate(
    "get-review-responses",
    () => _getCustomerFeedbackService.GetFeedbackResponse(),
    "0 * * * *"     // Every hour at :00
);
```

### Generating Reports with Filters

```csharp
var reportService = new CustomerFeedbackReportService(unitOfWork, sortingHelper, reportFactory, excelCreator, clock);

var filter = new CustomerFeedbackReportFilter 
{
    FranchiseeId = 42,
    StartDate = new DateTime(2024, 1, 1),
    EndDate = new DateTime(2024, 12, 31),
    ResponseFrom = 2,        // GatherUp/ReviewPush only
    Response = 1,            // Only show requests with responses
    Text = "john",           // Search in customer name/email
    SortingColumn = "Rating",
    SortingOrder = (long)SortingOrder.Desc
};

var report = reportService.GetCustomerFeedbackList(filter, pageNumber: 1, pageSize: 50);

foreach (var review in report.Collection) 
{
    Console.WriteLine($"{review.Customer}: {review.Rating} stars");
    Console.WriteLine($"From: {review.From} | Table: {review.FromTable}");
}
```

### Approving Reviews

```csharp
bool success = reportService.ManageCustomerFeedbackStatus(
    isAccept: true,
    customerId: 123,
    id: 456,
    fromTable: "CustomerFeedbackResponse"
);
```

### Exporting to Excel

```csharp
string fileName;
bool success = reportService.DownloadFeedbackReport(filter, out fileName);

if (success) 
{
    // Serve file to user: /downloads/{fileName}
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Class | Key Methods | Responsibility |
|-------|-------------|----------------|
| **CustomerFeedbackService** | `TriggerEmail`, `GetCustomer`, `SendFeedbackRequest`, `GetFeedback` | GatherUp/ReviewPush API integration |
| **GetCustomerFeedbackService** | `GetFeedbackResponse` | Polling agent: fetch reviews from APIs, sync to database |
| **SendFeedBackRequestPollingAgent** | `SendFeedback` | Polling agent: send queued review requests |
| **CustomerFeedbackReportService** | `GetCustomerFeedbackList`, `DownloadFeedbackReport`, `ManageCustomerFeedbackStatus` | Report generation and audit management |
| **CustomerFeedbackFactory** | `CreateDomain`, `CreateModel` | Entity/ViewModel factory methods |
| **CustomerFeedbackReportFactory** | `CreateViewModel` | Report view model transformations |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Polling Agents Not Running

**Check Scheduler**: Ensure Hangfire/Quartz.NET is configured and running

**Check Feature Flags**:
- `SendFeedbackEnabled` must be `true`
- `GetFeedbackEnabled` must be `true`

**Check Logs**: Look for "Feedback request is Disabled!" or "Feedback Response is Disabled."

### API Authentication Failures

**Symptom**: All API calls return error code 403 or "Invalid hash"

**Cause**: SHA256 hash mismatch

**Fix**:
- Verify `_settings.ReviewApiKey` is correct
- Check parameter sorting logic (must be alphabetical)
- Test hash generation: `Console.WriteLine(hash)`

### Reviews Not Matching Customers

**Symptom**: Reviews appear in `ReviewPushCustomerFeedback` but not linked to `Customer`

**Cause**: Email mismatch

**Diagnosis**:
```sql
-- Check if customer email exists
SELECT * FROM CustomerEmail WHERE Email = 'john.doe@example.com';

-- Check ReviewPush URL format
SELECT Url FROM ReviewPushCustomerFeedback WHERE Id = 123;
```

**Fix**: Ensure customer has matching email in `CustomerEmail` table (case-insensitive)
<!-- END CUSTOM SECTION -->
