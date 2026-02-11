<!-- AUTO-GENERATED: Header -->
# Reports/Domain — Module Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-11T06:35:45Z
**File Count**: 6 domain entity files
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Domain subfolder contains **persistent domain entities** that represent the database schema for report-related business data. These entities track aggregated sales metrics, audit trails for file uploads, email API synchronization status, and notification delivery records. Unlike transactional entities in Billing or Sales modules, these domain models are **analytics-focused** — they store pre-calculated aggregations, batch job metadata, and integration status for efficient report generation without real-time calculation overhead.

### Design Patterns
- **Domain-Driven Design (DDD)**: Each entity inherits from `DomainBase`, providing common audit fields (Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
- **Foreign Key Navigation**: All relationships use `[ForeignKey]` attributes with virtual navigation properties for lazy loading via Entity Framework
- **Aggregate Roots**: `FranchiseeSalesInfo` is an aggregate root representing monthly sales summaries
- **Value Objects**: Status fields (IsSynced, IsFailed, IsCorrectUploaded) act as value objects for state tracking

### Data Flow
1. **Write Path**: Services (ReportService, EmailAPIIntegrationService, BatchUploadService) create/update these entities
2. **Read Path**: Repositories expose these entities for report queries, filtered by franchisee, date ranges, status flags
3. **Aggregation**: `FranchiseeSalesInfo` populated by scheduled jobs that sum `FranchiseeSales` records monthly
4. **Audit Trail**: `BatchUploadRecord`, `PriceEstimateFileUpload`, `CustomerEmailAPIRecord` track external integrations and file imports

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### FranchiseeSalesInfo.cs
**Purpose**: Pre-aggregated monthly sales totals per franchisee/service/class for growth analytics

```csharp
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class FranchiseeSalesInfo : DomainBase
    {
        // Composite key: FranchiseeId + Month + Year + ClassTypeId + ServiceTypeId
        public long FranchiseeId { get; set; }
        public int Month { get; set; }            // 1-12
        public int Year { get; set; }             // YYYY format
        
        // Sales amounts (dual currency for international franchisees)
        public decimal SalesAmount { get; set; }              // USD (corporate standard)
        public decimal AmountInLocalCurrency { get; set; }    // Franchisee's currency
        
        public DateTime UpdatedDate { get; set; }  // Last aggregation run timestamp
        
        // Dimensions for multi-dimensional reporting
        public long ClassTypeId { get; set; }      // FK to MarketingClass
        public long ServiceTypeId { get; set; }    // FK to ServiceType
        
        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }
        
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType Service { get; set; }
        
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
```

**Key Business Rules**:
- Updated by scheduled aggregation job (runs nightly)
- Each record represents one franchisee's sales for one month, one service, one class
- Zero-sales months omitted (records only created if SalesAmount > 0)
- `UpdatedDate` tracks when aggregation last ran for stale data detection

### BatchUploadRecord.cs
**Purpose**: Audit trail for franchisee sales data batch uploads with compliance tracking

```csharp
using Core.Application.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class BatchUploadRecord : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long? PaymentFrequencyId { get; set; }  // FK to Lookup (Weekly, Monthly, Quarterly)
        
        // Wait period after period end before upload is required
        public int WaitPeriod { get; set; }            // Days (e.g., 3 days for weekly, 7 for monthly)
        
        // Sales data date range
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Compliance tracking
        public DateTime ExpectedUploadDate { get; set; }  // Calculated: EndDate + WaitPeriod
        public DateTime? UploadedOn { get; set; }         // Null if not yet uploaded
        public bool IsCorrectUploaded { get; set; }       // False if validation failed
        
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        
        [ForeignKey("PaymentFrequencyId")]
        public virtual Lookup PaymentFrequency { get; set; } 
    }
}
```

**Key Business Rules**:
- Created automatically when new billing period starts
- `ExpectedUploadDate` calculated on insert: `EndDate.AddDays(WaitPeriod)`
- `IsCorrectUploaded` set to true only after validation passes:
  - Sum of uploaded sales matches invoice totals (±2% tolerance)
  - All required columns present
  - No duplicate records
- Used by compliance reports to flag late/missing uploads

### CustomerEmailAPIRecord.cs
**Purpose**: Tracks synchronization status of customer emails with external email marketing API

```csharp
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class CustomerEmailAPIRecord : DomainBase
    {
        // Core identifiers
        public long CustomerId { get; set; } 
        public long FranchiseeId { get; set; } 
        public string CustomerEmail { get; set; }
        
        // External API identifiers (from email service provider)
        public string ApiCustomerId { get; set; }    // Provider's customer ID
        public string APIListId { get; set; }        // Mailing list ID
        public string APIEmailId { get; set; }       // Email address ID in provider
        
        // Sync status tracking
        public string ErrorResponse { get; set; }    // JSON error response if sync failed
        public string Status { get; set; }           // "Pending", "Synced", "Failed", "Retry"
        public bool IsSynced { get; set; }           // True if successfully synced
        public bool IsFailed { get; set; }           // True if sync failed after retries
        public DateTime? DateCreated { get; set; }   // First sync attempt timestamp
        
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
```

**Key Business Rules**:
- Created when customer opts in for email marketing
- Updated by scheduled sync job (runs hourly)
- `IsSynced = true` only after successful API confirmation
- `IsFailed = true` after 3 failed sync attempts
- Failed records trigger alert notification to IT
- `ErrorResponse` stores full API error payload for debugging

### PriceEstimateFileUpload.cs
**Purpose**: Audit trail for Excel file imports of price estimate data

```csharp
using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain  // Note: In Scheduler namespace, not Reports
{
    public class PriceEstimateFileUpload : DomainBase
    {
        public long FileId { get; set; }                   // FK to File entity (blob storage)
        public long StatusId { get; set; }                 // FK to Lookup (Pending, Processing, Completed, Failed)
        public long DataRecorderMetaDataId { get; set; }   // FK to metadata (upload user, date)
        
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        
        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }
        
        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        
        public string Notes { get; set; }  // User-entered notes about the upload
        
        // Parsed results log file
        public long? ParsedLogFileId { get; set; }
        [ForeignKey("ParsedLogFileId")]
        public virtual File LogFile { get; set; }  // JSON file with success/failure details
        
        public bool IsFranchiseeAdmin { get; set; }  // True if uploaded by franchisee (vs corporate)
    }
}
```

**Key Business Rules**:
- Created when user initiates Excel upload
- `StatusId` transitions: Pending → Processing → Completed/Failed
- `ParsedLogFileId` references JSON log file with:
  - Total rows parsed
  - Successful inserts/updates count
  - Failed rows with error messages
  - Validation errors (invalid service tag IDs, price out of range)
- `IsFranchiseeAdmin` determines if upload is restricted to single franchisee vs bulk corporate update

### WeeklyNotification.cs
**Purpose**: Tracks scheduled weekly notification emails to prevent duplicate sends

```csharp
using Core.Notification.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class WeeklyNotification : DomainBase
    {
        public DateTime NotificationDate { get; set; }    // Week ending date (e.g., Saturday)
        public long NotificationTypeId { get; set; }      // FK to NotificationType lookup
        
        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationType { get; set; }
    }
}
```

**Key Business Rules**:
- Created after weekly notification email successfully sent
- Composite uniqueness constraint: `NotificationDate + NotificationTypeId` (prevents duplicates)
- Checked before sending: `WHERE NotificationDate = @date AND NotificationTypeId = @type` → if exists, skip send
- Notification types: WeeklyARReport, WeeklyPerformanceDigest, WeeklyPhotoReport
- Records never deleted (audit trail)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **[../Organizations/Domain/](../../../Organizations/Domain/.context/CONTEXT.md)** — `Franchisee`, `MarketingClass` entities
- **[../Sales/Domain/](../../../Sales/Domain/.context/CONTEXT.md)** — `ServiceType`, `Customer` entities
- **[../Application/Domain/](../../../Application/Domain/.context/CONTEXT.md)** — `DomainBase` base class, `Lookup` entity, `File` entity, `DataRecorderMetaData`
- **[../Notification/Domain/](../../../Notification/Domain/.context/CONTEXT.md)** — `NotificationType` entity

### External Package Dependencies
- **System.ComponentModel.DataAnnotations** — `[ForeignKey]` attribute for ORM relationships
- **Entity Framework Core** — ORM for database persistence (lazy loading via virtual navigation properties)

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Database Indexes
For optimal query performance, ensure these indexes exist:

```sql
-- FranchiseeSalesInfo: Growth report queries filter by franchisee + year
CREATE INDEX IX_FranchiseeSalesInfo_Franchisee_Year 
ON FranchiseeSalesInfo (FranchiseeId, Year, Month) 
INCLUDE (SalesAmount, ClassTypeId, ServiceTypeId);

-- BatchUploadRecord: Compliance reports filter by expected upload date
CREATE INDEX IX_BatchUploadRecord_ExpectedUploadDate 
ON BatchUploadRecord (ExpectedUploadDate, FranchiseeId) 
WHERE UploadedOn IS NULL;

-- CustomerEmailAPIRecord: Email sync job queries failed records
CREATE INDEX IX_CustomerEmailAPIRecord_Sync_Status 
ON CustomerEmailAPIRecord (IsSynced, IsFailed, FranchiseeId);

-- WeeklyNotification: Duplicate check query
CREATE UNIQUE INDEX UX_WeeklyNotification_Date_Type 
ON WeeklyNotification (NotificationDate, NotificationTypeId);
```

### Data Retention
- **FranchiseeSalesInfo**: Keep 7 years for trend analysis
- **BatchUploadRecord**: Keep 2 years for audit compliance
- **CustomerEmailAPIRecord**: Purge synced records older than 90 days (keep failed records indefinitely)
- **PriceEstimateFileUpload**: Keep 1 year for audit trail
- **WeeklyNotification**: Keep indefinitely (small dataset)

### Currency Handling
`FranchiseeSalesInfo` stores both USD and local currency amounts:
- `SalesAmount` always in USD for network-wide aggregations
- `AmountInLocalCurrency` uses franchisee's currency for local reporting
- Conversion happens at time of aggregation (nightly job)
- Exchange rate stored in `Franchisee.ExchangeRate` field (updated daily)

### Composite Keys
While all entities inherit `Id` as primary key from `DomainBase`, some have **logical composite keys**:
- `FranchiseeSalesInfo`: `(FranchiseeId, Month, Year, ClassTypeId, ServiceTypeId)`
- `WeeklyNotification`: `(NotificationDate, NotificationTypeId)`

Queries should filter on all composite key columns for index efficiency.

<!-- END CUSTOM SECTION -->
