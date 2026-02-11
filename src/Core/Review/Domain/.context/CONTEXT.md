<!-- AUTO-GENERATED: Header -->
# Review/Domain — Module Context
**Version**: 64667c5c8c4ab9b3d804e48deb14e9b70895fc42
**Generated**: 2025-01-19T12:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Domain subfolder defines the **persistent data model** for the Review module — 6 entity classes that represent customer feedback requests, responses, and tracking records across multiple review platforms. These entities form the normalized database schema that unifies data from GatherUp, ReviewPush, and Google APIs into a cohesive relational model.

This is the "single source of truth" for review data persistence, with entities inheriting from `DomainBase` (providing `Id`, `IsNew`, audit fields) and using EF6 data annotations for foreign key relationships.

### Design Patterns
- **Domain-Driven Design**: Entities are anemic POCOs (Plain Old CLR Objects) with no behavior — they represent database tables, not business logic. All behavior lives in services (Impl folder).
- **Foreign Key Navigation Properties**: Extensive use of `[ForeignKey]` and `virtual` navigation properties for lazy loading related entities (Customer, Franchisee, FranchiseeSales, Lookup tables).
- **Multi-Table Aggregation**: `AllCustomerFeedback` is a denormalized view entity that aggregates data from `CustomerFeedbackRequest`, `CustomerFeedbackResponse`, and `ReviewPushCustomerFeedback` for reporting efficiency.
- **Soft Deletes**: `IsActive` and `AuditStatusId` fields control logical deletion and visibility rather than physical deletion.

### Data Flow
1. **Request Creation**: `CustomerFeedbackRequest` is created when a sale completes → stores queue status, customer email, QuickBooks invoice ID
2. **Response Collection**: External APIs return reviews → stored in `CustomerFeedbackResponse` (matched customer) or `ReviewPushCustomerFeedback` (unmatched)
3. **Linking**: `CustomerFeedbackRequest.ResponseId` foreign key links request to response when match is found
4. **Aggregation**: `AllCustomerFeedback` combines all sources for unified reporting
5. **Audit Trail**: Every entity has `AuditActionId` (Lookup) to track approval status

### Critical Constraints
- **Email Format**: `PartialPaymentEmailApiRecord` and review responses use email as primary matching key — case-insensitive comparison required
- **Date Precision**: `DateSend`, `DateOfReview`, `Rp_date`, `Db_date` — multiple date fields track request sent, review submitted, and sync timestamps for SLA tracking
- **Rating Scale Variation**: `Rating` decimal vs. `Recommend` double — different entities use different scales (1-5 vs 0-10) requiring conversion in services
- **Nullable Foreign Keys**: Many FKs are nullable (`long?`) because reviews from external systems may not match internal customers initially

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### CustomerReviewSystemRecord.cs
**Purpose**: Links a Marblelife customer to their external review system customer ID (GatherUp).

```csharp
public class CustomerReviewSystemRecord : DomainBase
{
    public long CustomerId { get; set; }                  // Internal Marblelife customer ID
    public long FranchiseeId { get; set; }                // Franchisee who owns the customer
    public long? BusinessId { get; set; }                 // GatherUp business ID (may be null if not yet synced)
    public long ReviewSystemCustomerId { get; set; }      // GatherUp's customer ID (critical for API calls)
    
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}
```

**Key Relationships**:
- Each Marblelife `Customer` can have one `CustomerReviewSystemRecord` per franchisee (same customer, different franchises)
- `ReviewSystemCustomerId` is returned by GatherUp API's `POST /api/customer/create` response
- Used as lookup table before every API call to fetch the external customer ID

**Constraints**:
- `ReviewSystemCustomerId` must be unique per franchisee (one external ID per customer per location)
- `BusinessId` is nullable because it's only set if franchisee has GatherUp business account

---

### CustomerFeedbackRequest.cs
**Purpose**: Tracks outbound review request emails sent to customers; acts as queue for polling agent.

```csharp
public class CustomerFeedbackRequest : DomainBase
{
    public long FranchiseeSalesId { get; set; }           // Links to sale that triggered this request
    public DateTime DateSend { get; set; }                // When request was created (not necessarily sent)
    public string DataPacket { get; set; }                // JSON payload sent to GatherUp API (for debugging)
    
    [ForeignKey("FranchiseeSalesId")]
    public virtual FranchiseeSales FranchiseeSales { get; set; }
    
    public bool IsQueued { get; set; }                    // TRUE = waiting to be sent; FALSE = already sent
    public string CustomerEmail { get; set; }             // Email where request will be sent
    
    public long CustomerReviewSystemRecordId { get; set; } // Link to external system customer ID
    public long? ResponseId { get; set; }                 // NULL until review is received; then links to CustomerFeedbackResponse.Id
    
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public string QBInvoiceId { get; set; }               // QuickBooks invoice number
    
    [ForeignKey("CustomerReviewSystemRecordId")]
    public virtual CustomerReviewSystemRecord CustomerReviewSystemRecord { get; set; }
    
    [ForeignKey("ResponseId")]
    public virtual CustomerFeedbackResponse CustomerFeedbackResponse { get; set; }  // Nullable; set when review received
    
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    public bool IsSystemGenerated { get; set; }           // TRUE = kiosk link; FALSE = Mailtropolis triggered
    
    public long AuditActionId { get; set; }               // Approval status
    [ForeignKey("AuditActionId")]
    public virtual Lookup AuditAction { get; set; }
    
    public long? StatusId { get; set; }                   // Request lifecycle status (pending, sent, failed)
    [ForeignKey("StatusId")]
    public virtual Lookup Status { get; set; }
}
```

**Key Workflows**:
1. **Creation**: Service sets `IsQueued = true`, `DateSend = now`, `ResponseId = null`
2. **Polling Agent Pickup**: `SendFeedBackRequestPollingAgent` queries `WHERE IsQueued = true`
3. **Delivery**: Agent calls GatherUp API; on success sets `IsQueued = false`
4. **Response Linking**: `GetCustomerFeedbackService` finds matching review by email + date, sets `ResponseId`

**Critical Fields**:
- `IsQueued`: Queue status — TRUE means polling agent should process it
- `IsSystemGenerated`: Differentiates kiosk-generated requests (immediate) from scheduled Mailtropolis requests
- `ResponseId`: Foreign key to `CustomerFeedbackResponse` — NULL until review is received

---

### CustomerFeedbackResponse.cs
**Purpose**: Stores inbound customer reviews from any source (GatherUp, Google, ReviewPush).

```csharp
public class CustomerFeedbackResponse : DomainBase
{
    public string ResponseContent { get; set; }           // Review text (comments from customer)
    public DateTime DateOfReview { get; set; }            // When customer submitted the review
    public long? CustomerId { get; set; }                 // Nullable; NULL if review doesn't match internal customer
    public long? FranchiseeId { get; set; }               // Nullable; NULL if location can't be determined
    public decimal Rating { get; set; }                   // Star rating (scale varies by source)
    public double Recommend { get; set; }                 // GatherUp's 0-10 recommendation score
    public bool ShowReview { get; set; }                  // Customer opted to make review public
    public long? FeedbackId { get; set; }                 // External system's feedback ID
    public DateTime? DateOfDataInDataBase { get; set; }   // When this record was synced from API
    public string Url { get; set; }                       // API-provided URL (ReviewPush embeds email as "mailto:email@example.com")
    public long? ReviewId { get; set; }                   // External system's review ID (primary key in their system)
    public bool IsFromNewReviewSystem { get; set; }       // TRUE = ReviewPush; FALSE = GatherUp
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
    
    public string CustomerName { get; set; }              // Name from API (may differ from internal Customer.Name)
    
    public long AuditActionId { get; set; }               // Approval status for marketing website
    [ForeignKey("AuditActionId")]
    public virtual Lookup AuditAction { get; set; }
    
    public bool IsFromGoogleAPI { get; set; }             // TRUE = fetched from Google Reviews API
    public bool IsFromSystemReviewSystem { get; set; }    // TRUE = ReviewPush system-generated review
    public bool IsSentToMarketingWebsite { get; set; }    // TRUE = approved and published
}
```

**Source Identification Flags**:
- `IsFromNewReviewSystem = true` → ReviewPush
- `IsFromGoogleAPI = true` → Google Reviews
- `IsFromSystemReviewSystem = true` → ReviewPush system-specific
- All false → GatherUp (legacy system)

**Rating Scale**:
- `Rating`: Direct star rating (1-5 scale for ReviewPush, 0-10 scale for GatherUp)
- `Recommend`: GatherUp-specific (0-10 scale); for display, use `Recommend / 2` to convert to 5-star scale
- ReviewPush only populates `Rating` (not `Recommend`)

**Email Extraction**:
- ReviewPush API embeds email in `Url` field: `"mailto:john.doe@example.com"`
- Service must parse `Url` to extract email for customer matching

---

### AllCustomerFeedback.cs
**Purpose**: Denormalized aggregation view for unified reporting across all review sources.

```csharp
public class AllCustomerFeedback : DomainBase
{
    public long? CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    public string CustomerName { get; set; }              // Denormalized from Customer or API
    public string CustomerEmail { get; set; }             // Denormalized for fast filtering
    public DateTime? ResponseReceivedDate { get; set; }   // When review was submitted
    public DateTime? ResponseSyncingDate { get; set; }    // When review was fetched from API
    public string ResponseContent { get; set; }           // Review text
    
    public long? FranchiseeId { get; set; }
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
    
    public string FranchiseeName { get; set; }            // Denormalized for fast display
    public decimal? Rating { get; set; }
    public long? Recommend { get; set; }
    public string ContactPerson { get; set; }
    public string CustomerNameFromAPI { get; set; }       // Original name from external system
    
    public long AuditStatusId { get; set; }
    [ForeignKey("AuditStatusId")]
    public virtual Lookup AuditAction { get; set; }
    
    public string From { get; set; }                      // "GatherUp", "Google", "ReviewSystem"
    public string FromTable { get; set; }                 // "CustomerFeedbackRequest", "CustomerFeedbackResponse", "ReviewPushCustomerFeedback"
    
    // Foreign key links to source tables (only one will be populated)
    public long? ReviewPushCustomerFeedbackId { get; set; }
    [ForeignKey("ReviewPushCustomerFeedbackId")]
    public virtual ReviewPushCustomerFeedback ReviewPushCustomerFeedback { get; set; }
    
    public long? CustomerFeedbackRequestId { get; set; }
    [ForeignKey("CustomerFeedbackRequestId")]
    public virtual CustomerFeedbackRequest CustomerFeedbackRequest { get; set; }
    
    public long? CustomerFeedbackResponseId { get; set; }
    [ForeignKey("CustomerFeedbackResponseId")]
    public virtual CustomerFeedbackResponse CustomerFeedbackResponse { get; set; }
    
    public bool IsOldReview { get; set; }                 // Historical data migration flag
    public bool IsSentToMarketingWebsite { get; set; }
    public bool IsEmailSent { get; set; }
    public bool IsActive { get; set; }                    // Soft delete flag
}
```

**Purpose**:
- **Reporting Performance**: Pre-aggregated data avoids complex JOINs across 5 tables during report queries
- **Source Tracking**: `From` and `FromTable` identify origin without querying source tables
- **Audit Trail**: Tracks publication status (`IsSentToMarketingWebsite`)

**Population Strategy**:
- Likely populated by database trigger or scheduled ETL job (not directly via services)
- Alternative: Could be a SQL view instead of physical table

---

### CustomerReviewForMarketing.cs
**Purpose**: Lightweight DTO for marketing website display (subset of AllCustomerFeedback).

```csharp
public class CustomerReview
{
    public CustomerReview()
    {
        Collection = new List<CustomerReviewForMarketing>();
    }
    public List<CustomerReviewForMarketing> Collection { get; set; }
}

public class CustomerReviewForMarketing
{
    public long Id { get; set; }
    public long? FranchiseeId { get; set; }
    public string FranchiseeName { get; set; }
    public long? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string ContactPerson { get; set; }
    public DateTime? ResponseReceivedDate { get; set; }
    public DateTime? ResponseSyncingDate { get; set; }
    public string ResponseContent { get; set; }
    public decimal? Rating { get; set; }
    public long? Recommend { get; set; }
    public string CustomerNameFromAPI { get; set; }
    public long AuditStatusId { get; set; }
    public string AuditStatus { get; set; }              // Denormalized Lookup.Name
    public string from { get; set; }                     // "GatherUp", "Google", "ReviewSystem"
}
```

**Usage**:
- **Marketing Website API**: Endpoint returns `CustomerReview.Collection` for public display
- **Filtering**: Only approved reviews (`AuditStatusId` = approved status)
- **Denormalization**: Includes `FranchiseeName` and `AuditStatus` strings to avoid additional API calls

---

### PartialPaymentEmailApiRecord.cs
**Purpose**: Tracks partial payment notification emails sent to customers (related to invoicing, not reviews).

```csharp
public class PartialPaymentEmailApiRecord : DomainBase
{
    public long CustomerId { get; set; }
    public long FranchiseeId { get; set; }
    public string CustomerEmail { get; set; }
    public string ApiCustomerId { get; set; }             // External email API's customer ID
    public string APIListId { get; set; }                 // Email list identifier
    public string APIEmailId { get; set; }                // Email campaign ID
    public string ErrorResponse { get; set; }             // API error message if failed
    public string Status { get; set; }                    // "sent", "failed", "pending"
    public bool IsSynced { get; set; }
    public bool IsFailed { get; set; }
    public long InvoiceId { get; set; }                   // Links to billing invoice
    public long statusId { get; set; }                    // Status lookup
    public DateTime? DateCreated { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
    
    [ForeignKey("InvoiceId")]
    public virtual Invoice Invoice { get; set; }
    
    [ForeignKey("statusId")]
    public virtual Lookup Lookup { get; set; }
}
```

**Note**: This entity is tangentially related to Review module — it's in the Review/Domain folder but serves the Billing module's partial payment notification feature. May have been placed here due to shared email tracking patterns.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

These are **entities** (data models), not APIs. They expose no methods — all operations are performed via Entity Framework repositories in the service layer.

**Key Repository Queries**:
```csharp
// Find queued requests
_customerFeedbackRequestRepository.Table.Where(x => x.IsQueued && !x.IsSystemGenerated)

// Find unlinked responses
_customerFeedbackResponseRepository.Table.Where(x => x.IsFromNewReviewSystem && x.ReviewId == apiReviewId)

// Match review to request by email + date
_customerFeedbackRequestRepository.Table
    .Where(x => x.CustomerEmail.ToLower() == email.ToLower() 
                && x.DateSend <= reviewDate 
                && x.ResponseId == null)
    .OrderByDescending(x => x.DateSend)
    .FirstOrDefault()
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application.Domain](../../Application/Domain/.context/CONTEXT.md)** — `DomainBase` (base class for all entities)
- **[Core.Sales.Domain](../../Sales/Domain/.context/CONTEXT.md)** — `Customer`, `FranchiseeSales` entities
- **[Core.Organizations.Domain](../../Organizations/Domain/.context/CONTEXT.md)** — `Franchisee`, `Organization`, `Lookup`, `ReviewPushCustomerFeedback`, `ReviewPushAPILocation` entities
- **[Core.Billing.Domain](../../Billing/Domain/.context/CONTEXT.md)** — `Invoice` entity (for PartialPaymentEmailApiRecord)

### External Dependencies
- **Entity Framework 6**: Data annotations (`[ForeignKey]`, `[Table]`), lazy loading (`virtual` navigation properties)
- **System.ComponentModel.DataAnnotations**: Attribute-based validation and mapping

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Database Schema Notes

**Table Relationships**:
```
Customer ←──┐
            │
CustomerReviewSystemRecord ←── CustomerFeedbackRequest ───→ CustomerFeedbackResponse
                                        │
                                        ├─→ FranchiseeSales
                                        ├─→ Franchisee
                                        └─→ Lookup (AuditAction, Status)
```

**Indexing Recommendations**:
- `CustomerFeedbackRequest`: Composite index on `(IsQueued, IsSystemGenerated)` for polling agent queries
- `CustomerFeedbackResponse`: Index on `(ReviewId, IsFromNewReviewSystem)` for duplicate detection
- `AllCustomerFeedback`: Index on `(FranchiseeId, ResponseReceivedDate)` for report queries

**Nullable Foreign Keys**:
- `CustomerFeedbackResponse.CustomerId` — NULL when review doesn't match internal customer (email mismatch)
- `CustomerFeedbackResponse.FranchiseeId` — NULL when location can't be determined from API data
- `CustomerFeedbackRequest.ResponseId` — NULL until review is received

### Rating Scale Conversions

**GatherUp**:
- API returns `Recommend` (0-10 integer)
- Store in `CustomerFeedbackResponse.Recommend` as `double`
- Display as stars: `Rating = Recommend / 2` (converts to 0-5 scale)

**ReviewPush**:
- API returns `Rating` (string: "1" to "5")
- Parse to `long`, store in `CustomerFeedbackResponse.Rating` as `decimal`
- Display directly (already 1-5 scale)

**Google**:
- Assumed 1-5 scale (store in `Rating`)

### Email Extraction Logic

ReviewPush embeds customer email in `Url` field:
```
"mailto:john.doe@example.com"  // Standard format
"mailto:invalid"                // Malformed (no @ sign)
```

Extraction in `GetCustomerFeedbackService`:
```csharp
var isContainMail = model.Url.Contains("mailto:");
if (isContainMail)
{
    var urlSplit = model.Url.Split(new string[] { "mailto:" }, StringSplitOptions.RemoveEmptyEntries);
    if (urlSplit.Length > 0)
        email = urlSplit[0];
}
```

**Gotcha**: If `Url` doesn't contain "mailto:", email remains null → review won't match customer → goes to `ReviewPushCustomerFeedback` table instead of `CustomerFeedbackResponse`.

### Audit Workflow States

`AuditActionId` (Lookup table) likely values:
- **Pending** (default): Awaiting manager review
- **Approved**: Published to marketing website (`IsSentToMarketingWebsite = true`)
- **Rejected**: Hidden from public view
- **Flagged**: Requires attention (inappropriate content)

Services query `WHERE AuditActionId = {approvedId}` to filter published reviews.

### Migration Notes

`AllCustomerFeedback.IsOldReview` flag suggests historical data import:
- Legacy reviews from previous system
- May have incomplete data (missing foreign keys)
- Likely imported during platform migration
<!-- END CUSTOM SECTION -->
