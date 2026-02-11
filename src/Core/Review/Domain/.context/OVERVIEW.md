<!-- AUTO-GENERATED: Header -->
# Review/Domain
> Persistent data model entities for customer feedback tracking, review requests, and multi-source review aggregation
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Domain subfolder contains the **database entity definitions** that power the Review module's data persistence layer. Think of these entities as the "blueprint" for the database tables that store every customer review request, every review response, and all the linkage data that connects requests to responses across multiple external review platforms.

### Why These Entities Exist

When Marblelife sends a review request to a customer, multiple things need to be tracked:
1. **Who** was asked (customer, franchisee, sale)
2. **When** they were asked (request date)
3. **Where** the request was sent (GatherUp customer ID, email)
4. **Did they respond?** (response linkage)
5. **What did they say?** (review content, rating)
6. **Where did the review come from?** (GatherUp, Google, ReviewPush)
7. **Is it approved for marketing?** (audit status)

These 6 entity classes provide the relational structure to answer all those questions while supporting:
- **Multi-source aggregation** (3 different review APIs)
- **Request/response linkage** (matching inbound reviews to outbound requests)
- **Audit workflows** (approval before public display)
- **Reporting** (denormalized views for fast queries)

### Entity Relationships at a Glance

```
┌─────────────────────────────────────────────────────────────┐
│                     Customer (Sales)                        │
│                    Franchisee (Orgs)                        │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼
    CustomerReviewSystemRecord
             │ (Maps internal customer to external review system ID)
             │
             ▼
    CustomerFeedbackRequest ──────────┐
             │                         │ (Links to response when received)
             │ (IsQueued = true)       ▼
             │                   CustomerFeedbackResponse
             ▼                         │
      SendFeedBackRequest             │ (Review from GatherUp/Google/ReviewPush)
      (Polling Agent)                 │
                                      ▼
                              AllCustomerFeedback
                              (Aggregated reporting view)
```

### Key Concepts

**1. Request/Response Separation**:
- `CustomerFeedbackRequest` tracks outbound requests (what we asked for)
- `CustomerFeedbackResponse` tracks inbound reviews (what customers said)
- `CustomerFeedbackRequest.ResponseId` foreign key links the two when a match is found

**2. Multi-Source Storage**:
- Reviews matching internal customers → `CustomerFeedbackResponse`
- Reviews from unknown customers (ReviewPush) → `ReviewPushCustomerFeedback` (in Organizations module)
- All sources aggregated → `AllCustomerFeedback` for unified reporting

**3. External System Linkage**:
- `CustomerReviewSystemRecord.ReviewSystemCustomerId` stores GatherUp's customer ID
- This is the "foreign key" to the external system — required for all API calls

**4. Audit Trail**:
- Every entity has `AuditActionId` (or `AuditStatusId`) linking to a Lookup table
- Controls approval workflow before reviews appear on marketing website

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating a Review Request (Example from Service Layer)

```csharp
// After a sale is completed, create a review request
var reviewSystemRecord = new CustomerReviewSystemRecord
{
    CustomerId = customer.Id,
    FranchiseeId = franchisee.Id,
    BusinessId = franchisee.BusinessId,
    ReviewSystemCustomerId = gatherUpCustomerId,  // From GatherUp API response
    IsNew = true
};
_customerReviewSystemRecordRepository.Save(reviewSystemRecord);

var feedbackRequest = new CustomerFeedbackRequest
{
    FranchiseeSalesId = sale.Id,
    DateSend = DateTime.UtcNow,
    CustomerEmail = customerEmail,
    QBInvoiceId = invoiceId,
    IsQueued = true,                             // Polling agent will process this
    IsSystemGenerated = false,                   // Triggered by Mailtropolis
    FranchiseeId = franchisee.Id,
    CustomerId = customer.Id,
    CustomerReviewSystemRecordId = reviewSystemRecord.Id,
    ResponseId = null,                           // Will be set when review is received
    IsNew = true
};
_customerFeedbackRequestRepository.Save(feedbackRequest);
_unitOfWork.SaveChanges();
```

### Storing an Inbound Review

```csharp
// Polling agent fetched a review from GatherUp API
var feedbackResponse = new CustomerFeedbackResponse
{
    ResponseContent = "Great service! Very professional.",
    DateOfReview = DateTime.Parse("2024-01-15"),
    CustomerId = matchedCustomer.Id,             // Found by email lookup
    FranchiseeId = franchisee.Id,
    Rating = 5.0m,                               // GatherUp rating (1-10 scale)
    Recommend = 10.0,                            // GatherUp recommendation score
    ShowReview = true,                           // Customer opted to share publicly
    FeedbackId = 12345,                          // GatherUp's feedback ID
    ReviewId = 67890,                            // GatherUp's review ID
    IsFromNewReviewSystem = false,               // FALSE = GatherUp (legacy system)
    IsFromGoogleAPI = false,
    IsFromSystemReviewSystem = false,
    DateOfDataInDataBase = DateTime.UtcNow,      // Sync timestamp
    AuditActionId = pendingStatusId,             // Awaiting approval
    IsNew = true
};
_customerFeedbackResponseRepository.Save(feedbackResponse);

// Link back to original request
var originalRequest = _customerFeedbackRequestRepository.Table
    .Where(x => x.CustomerEmail == customerEmail 
                && x.DateSend <= feedbackResponse.DateOfReview
                && x.ResponseId == null)
    .OrderByDescending(x => x.DateSend)
    .FirstOrDefault();

if (originalRequest != null)
{
    originalRequest.ResponseId = feedbackResponse.Id;
    _customerFeedbackRequestRepository.Save(originalRequest);
}

_unitOfWork.SaveChanges();
```

### Querying Reviews for Reports

```csharp
// Get all approved reviews for a franchisee in the last 30 days
var startDate = DateTime.UtcNow.AddDays(-30);
var approvedStatusId = (long)AuditActionType.Approved;

var reviews = _customerFeedbackResponseRepository.Table
    .Where(x => x.FranchiseeId == franchiseeId
                && x.DateOfReview >= startDate
                && x.AuditActionId == approvedStatusId)
    .OrderByDescending(x => x.DateOfReview)
    .ToList();

foreach (var review in reviews)
{
    Console.WriteLine($"{review.Customer.Name}: {review.Rating} stars");
    Console.WriteLine($"Comment: {review.ResponseContent}");
    Console.WriteLine($"Source: {(review.IsFromGoogleAPI ? "Google" : review.IsFromNewReviewSystem ? "ReviewPush" : "GatherUp")}");
}
```

### Finding Unlinked Requests (SLA Tracking)

```csharp
// Find requests sent >7 days ago with no response yet
var cutoffDate = DateTime.UtcNow.AddDays(-7);

var unrespondedRequests = _customerFeedbackRequestRepository.Table
    .Where(x => !x.IsQueued                      // Already sent
                && x.ResponseId == null          // No response yet
                && x.DateSend <= cutoffDate)     // Sent >7 days ago
    .Include(x => x.Customer)
    .Include(x => x.Franchisee)
    .ToList();

Console.WriteLine($"Found {unrespondedRequests.Count} pending requests (>7 days old)");
foreach (var request in unrespondedRequests)
{
    Console.WriteLine($"{request.Customer.Name} - Sent: {request.DateSend:yyyy-MM-dd}");
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

These are Entity Framework entities — they don't expose methods. All interactions are via repository pattern:

| Entity | Purpose | Key Fields |
|--------|---------|-----------|
| **CustomerReviewSystemRecord** | Links internal customer to external review system | `ReviewSystemCustomerId` (GatherUp ID), `CustomerId`, `FranchiseeId` |
| **CustomerFeedbackRequest** | Tracks outbound review requests | `IsQueued` (send status), `ResponseId` (links to response), `CustomerEmail`, `DateSend` |
| **CustomerFeedbackResponse** | Stores inbound reviews from any source | `ResponseContent`, `Rating`, `Recommend`, `IsFromNewReviewSystem`, `IsFromGoogleAPI`, `AuditActionId` |
| **AllCustomerFeedback** | Denormalized aggregation view for reporting | `From` (source name), `FromTable` (source table), `CustomerName`, `FranchiseeName`, `Rating` |
| **CustomerReviewForMarketing** | Lightweight DTO for marketing website | `AuditStatus`, `Rating`, `ResponseContent`, `CustomerName` |
| **PartialPaymentEmailApiRecord** | Tracks partial payment notification emails | `InvoiceId`, `CustomerEmail`, `IsSynced`, `IsFailed` |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Foreign Key Constraint Violations

**Symptom**: EF throws `FK_CustomerFeedbackRequest_CustomerReviewSystemRecord` constraint error.

**Cause**: Attempting to create `CustomerFeedbackRequest` with `CustomerReviewSystemRecordId` that doesn't exist.

**Fix**:
```csharp
// Always create CustomerReviewSystemRecord FIRST
var record = new CustomerReviewSystemRecord { /* ... */ };
_customerReviewSystemRecordRepository.Save(record);
_unitOfWork.SaveChanges();  // ← MUST save to get record.Id

// THEN create request
var request = new CustomerFeedbackRequest 
{ 
    CustomerReviewSystemRecordId = record.Id,  // Now valid FK
    /* ... */
};
_customerFeedbackRequestRepository.Save(request);
_unitOfWork.SaveChanges();
```

---

### Lazy Loading Not Working

**Symptom**: `CustomerFeedbackRequest.Customer` is null even though `CustomerId` has value.

**Cause**: Navigation properties only populate with lazy loading enabled AND Entity Framework proxy creation enabled.

**Fix**:
```csharp
// Option 1: Enable lazy loading (in DbContext constructor)
Configuration.LazyLoadingEnabled = true;
Configuration.ProxyCreationEnabled = true;

// Option 2: Explicit eager loading
var request = _customerFeedbackRequestRepository.Table
    .Include(x => x.Customer)
    .Include(x => x.Franchisee)
    .Include(x => x.CustomerReviewSystemRecord)
    .FirstOrDefault(x => x.Id == requestId);
```

---

### Rating Display Inconsistency

**Symptom**: Report shows different ratings than external review platform.

**Cause**: GatherUp uses 0-10 scale (`Recommend`) but should display as 0-5 stars.

**Fix**:
```csharp
// DON'T display raw Recommend value
Console.WriteLine($"Rating: {response.Recommend} stars");  // ❌ Shows "10 stars"

// DO convert to 5-star scale
decimal displayRating = (decimal)response.Recommend / 2;    // ✅ Shows "5.0 stars"
Console.WriteLine($"Rating: {displayRating} stars");
```

---

### Duplicate Review Entries

**Symptom**: Same review appears multiple times in `CustomerFeedbackResponse` table.

**Cause**: Polling agent runs multiple times and doesn't check for existing `ReviewId`.

**Fix**:
```csharp
// BEFORE creating new CustomerFeedbackResponse, check if already exists
var existingReview = _customerFeedbackResponseRepository.Table
    .FirstOrDefault(x => x.ReviewId == apiReviewId && x.IsFromNewReviewSystem);

if (existingReview != null)
{
    // Update existing record instead of creating new
    existingReview.ResponseContent = newContent;
    // ...
}
else
{
    // Create new record
    var newReview = new CustomerFeedbackResponse { /* ... */ };
    _customerFeedbackResponseRepository.Save(newReview);
}
```

---

### AllCustomerFeedback Not Updating

**Symptom**: New reviews appear in `CustomerFeedbackResponse` but not in `AllCustomerFeedback`.

**Cause**: `AllCustomerFeedback` is likely a denormalized table populated by:
- Database trigger (on INSERT/UPDATE to source tables)
- Scheduled ETL job
- SQL view (not a table)

**Diagnosis**:
```sql
-- Check if it's a table or view
SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'AllCustomerFeedback';

-- If it's a table, check for triggers
SELECT * FROM sys.triggers WHERE parent_class_desc = 'DATABASE' 
AND name LIKE '%AllCustomerFeedback%';
```

**Fix**:
- If trigger-based: Verify trigger is enabled and not erroring
- If ETL-based: Check ETL job logs and schedule
- If view-based: Ensure view definition includes all source tables
<!-- END CUSTOM SECTION -->
