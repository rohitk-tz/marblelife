<!-- AUTO-GENERATED: Header -->
# Sales Domain Module Context
**Version**: 3f7ca98653b76ee0fca84e0a126043097a12de5d
**Generated**: 2026-02-04T06:51:55Z
**Module Path**: src/Core/Sales/Domain
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
The Sales Domain module defines the **Customer Relationship Management (CRM) data model** for the Marblelife franchisee system. It manages:
1. **Customer Master Data** - Customer entities with contact information, addresses, and marketing classifications
2. **Sales Data Upload Pipeline** - Excel/CSV import entities tracking franchisee sales submissions and parsing workflows
3. **Royalty Calculation Entities** - Invoice line items for royalty, advertising fund, and national account fees
4. **Financial Credits** - Account credit memos and line items for customer refunds/adjustments
5. **Estimate/Quote Management** - Estimate invoices linked to job scheduler and customer data

This is a **pure domain layer** - no business logic, only entity definitions with EF6 navigation properties and audit trails.

### Design Patterns
- **Aggregate Root (Customer)**: Customer → CustomerEmail (1:N), Customer → Address (1:1 nullable), Customer → MarketingClass (N:1)
- **Cascade Entity Pattern**: Uses `[CascadeEntity]` attribute to signal automatic save propagation for child collections (CustomerEmails, AccountCreditItems)
- **Audit Trail Pattern**: Every mutable entity extends `DomainBase` and references `DataRecorderMetaData` for created/modified tracking
- **Shared Primary Key (One-to-One Extension)**: `RoyaltyInvoiceItem`, `NationalInvoiceItem`, `AdFundInvoiceItem` share PK with parent `InvoiceItem` via `[ForeignKey("InvoiceItem")] public override long Id`
- **Status Enum via Lookup Table**: `SalesDataUpload.StatusId → Lookup` (values in SalesDataUploadStatus enum: 71=Uploaded, 72=Parsed, 73=Failed, 74=ParseInProgress)
- **Multi-Tenancy**: Most entities indirectly link to `Franchisee` for tenant isolation

### Data Flow
1. **Customer Import**: CSV → `CustomerFileUpload` (status tracking) → Parsed into `Customer` + `CustomerEmail` + `Address`
2. **Sales Data Upload**: Excel → `SalesDataUpload` → Parsing creates `EstimateInvoice` records (from Invoice sheet) → Background job processes rows
3. **Annual Reconciliation**: `AnnualSalesDataUpload` validates year-end totals against weekly uploads → Creates `SystemAuditRecord` for mismatches
4. **Royalty Billing**: `EstimateInvoice` aggregation → Creates `RoyaltyInvoiceItem` + `AdFundInvoiceItem` + `NationalInvoiceItem` on HQ billing cycle
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Core Entities

### Customer Aggregate (CRM)
```csharp
public class Customer : DomainBase
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public long? AddressId { get; set; }
    public string Phone { get; set; }
    public long ClassTypeId { get; set; } // FK to MarketingClass
    public long DataRecorderMetaDataId { get; set; }
    
    // Navigation Properties
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<CustomerEmail> CustomerEmails { get; set; }
    
    [CascadeEntity]
    [ForeignKey("AddressId")]
    public virtual Address Address { get; set; }
    
    [ForeignKey("ClassTypeId")]
    public virtual MarketingClass MarketingClass { get; set; }
    
    [ForeignKey("DataRecorderMetaDataId")]
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    
    // Computed Fields (denormalized aggregates)
    public bool ReceiveNotification { get; set; }
    public DateTime? DateCreated { get; set; }
    public decimal? TotalSales { get; set; }
    public decimal? NoOfSales { get; set; }
    public decimal? AvgSales { get; set; } // Computed: TotalSales / NoOfSales
}

public class CustomerEmail : DomainBase
{
    public long CustomerId { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}
```

### Sales Data Upload Pipeline
```csharp
public class SalesDataUpload : DomainBase
{
    public long FranchiseeId { get; set; }
    public long FileId { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public long StatusId { get; set; } // Lookup: 71=Uploaded, 72=Parsed, 73=Failed, 74=ParseInProgress
    public long? ParsedLogFileId { get; set; } // Error log file
    
    // Parsing Statistics
    public int? NumberOfCustomers { get; set; }
    public int? NumberOfInvoices { get; set; }
    public int? NumberOfFailedRecords { get; set; }
    public int? NumberOfParsedRecords { get; set; }
    
    // Financial Aggregates
    public decimal? TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? AccruedAmount { get; set; }
    
    // Audit/State
    public long DataRecorderMetaDataId { get; set; }
    public long CurrencyExchangeRateId { get; set; } // For CAD/USD conversion
    public bool IsActive { get; set; }
    public bool IsInvoiceGenerated { get; set; } // Has royalty invoice been created?
    
    // Navigation
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
    
    [ForeignKey("FileId")]
    public virtual File File { get; set; }
    
    [ForeignKey("StatusId")]
    public virtual Lookup Lookup { get; set; }
    
    [ForeignKey("CurrencyExchangeRateId")]
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
}

public class CustomerFileUpload : DomainBase
{
    public long FileId { get; set; }
    public long StatusId { get; set; } // Reuses same status enum
    public long DataRecorderMetaDataId { get; set; }
    
    [ForeignKey("FileId")]
    public virtual File File { get; set; }
    
    [ForeignKey("StatusId")]
    public virtual Lookup Lookup { get; set; }
}

public class InvoiceFileUpload : DomainBase
{
    public long FileId { get; set; }
    public long StatusId { get; set; }
    public long DataRecorderMetaDataId { get; set; }
    
    [ForeignKey("FileId")]
    public virtual File File { get; set; }
    
    [ForeignKey("StatusId")]
    public virtual Lookup Lookup { get; set; }
}
```

### Annual Reconciliation
```csharp
public class AnnualSalesDataUpload : DomainBase
{
    public long FranchiseeId { get; set; }
    public long FileId { get; set; }
    public long? SalesDataUploadId { get; set; } // Links to weekly upload if applicable
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public long StatusId { get; set; }
    public long? ParsedLogFileId { get; set; }
    
    // Parsing Stats
    public int? NoOfFailedRecords { get; set; }
    public int? NoOfParsedRecords { get; set; }
    public int? NoOfMismatchedRecords { get; set; }
    
    // Royalty Calculation
    public decimal? WeeklyRoyality { get; set; } // Sum from weekly uploads
    public decimal? AnnualRoyality { get; set; } // From tax filing docs
    public decimal? TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    
    // Audit Workflow
    public long AuditActionId { get; set; } // 151=Approved, 152=Rejected, 153=Pending
    public bool? IsAuditAddressParsing { get; set; }
    
    public long DataRecorderMetaDataId { get; set; }
    public long CurrencyExchangeRateId { get; set; }
    
    [ForeignKey("SalesDataUploadId")]
    public virtual SalesDataUpload SalesDataUpload { get; set; }
    
    [ForeignKey("AuditActionId")]
    public virtual Lookup AuditAction { get; set; }
}

public class SystemAuditRecord : DomainBase
{
    public long InvoiceId { get; set; }
    public long FranchiseeId { get; set; }
    public long AnnualUploadId { get; set; }
    public string QBIdentifier { get; set; } // QuickBooks Invoice Number
    public long? AnnualReportTypeId { get; set; } // Enum: Type1, Type6, Type14, etc.
    
    [ForeignKey("InvoiceId")]
    public virtual Invoice Invoice { get; set; }
    
    [ForeignKey("AnnualUploadId")]
    public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; }
    
    [ForeignKey("AnnualReportTypeId")]
    public virtual AnnualReportType AnnualReportType { get; set; }
}
```

### Estimate/Quote Management
```csharp
public class EstimateInvoice : DomainBase
{
    public long? CustomerId { get; set; }
    public long? InvoiceCustomerId { get; set; } // Inline customer (not in master)
    public float PriceOfService { get; set; }
    public float LessDeposit { get; set; }
    public long ClassTypeId { get; set; } // MarketingClass
    public long? FranchiseeId { get; set; }
    public long? EstimateId { get; set; } // Links to JobEstimate
    public long? SchedulerId { get; set; } // Links to JobScheduler
    public long? NumberOfInvoices { get; set; }
    public string Option { get; set; }
    public string Notes { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public bool IsInvoiceChanged { get; set; }
    public bool? IsCustomerAvailable { get; set; }
    public bool IsInvoiceParsing { get; set; } // Import flag
    public long DataRecorderMetaDataId { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("InvoiceCustomerId")]
    public virtual EstimateInvoiceCustomer EstimateInvoiceCustomer { get; set; }
    
    [ForeignKey("EstimateId")]
    public virtual JobEstimate JobEstimate { get; set; }
    
    [ForeignKey("SchedulerId")]
    public virtual JobScheduler JobScheduler { get; set; }
}

public class EstimateInvoiceCustomer : DomainBase
{
    public long DataRecorderMetaDataId { get; set; }
    public string CustomerName { get; set; }
    public string StreetAddress { get; set; }
    public string CityName { get; set; }
    public string StateName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber1 { get; set; }
    public string PhoneNumber2 { get; set; }
    public string CCEmail { get; set; }
}
```

### Royalty Invoice Line Items (One-to-One Extensions)
```csharp
// IMPORTANT: All three entities share the same PK as parent InvoiceItem
// EF6 maps these as one-to-one extensions of Billing.InvoiceItem

public class RoyaltyInvoiceItem : DomainBase
{
    [ForeignKey("InvoiceItem")]
    public override long Id { get; set; } // Shared PK pattern
    
    public decimal? Percentage { get; set; } // Royalty rate (e.g., 8%)
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}

public class AdFundInvoiceItem : DomainBase
{
    [ForeignKey("InvoiceItem")]
    public override long Id { get; set; }
    
    public decimal Percentage { get; set; } // Ad fund rate (e.g., 2%)
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}

public class NationalInvoiceItem : DomainBase
{
    [ForeignKey("InvoiceItem")]
    public override long Id { get; set; }
    
    public decimal? Percentage { get; set; } // National account fee
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}

public class FranchiseeFeeEmailInvoiceItem : DomainBase
{
    [ForeignKey("InvoiceItem")]
    public override long Id { get; set; }
    
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    
    public virtual InvoiceItem InvoiceItem { get; set; }
}
```

### Account Credits (Refunds/Adjustments)
```csharp
public class AccountCredit : DomainBase
{
    public long CustomerId { get; set; }
    public string QbInvoiceNumber { get; set; } // QuickBooks ref
    public DateTime CreditedOn { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<AccountCreditItem> CreditMemoItems { get; set; }
}

public class AccountCreditItem : DomainBase
{
    public long AccountCreditId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public long CurrencyExchangeRateId { get; set; }
    
    [ForeignKey("CurrencyExchangeRateId")]
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
}
```

### Marketing Classification
```csharp
public class MarketingClass : DomainBase
{
    public string Name { get; set; } // "Commercial", "Residential", "Hotel"
    public string Description { get; set; }
    public string ColorCode { get; set; } // UI color hex
    public string Alias { get; set; }
    public long? CategoryId { get; set; } // Lookup category grouping
    public int? NewOrderBy { get; set; } // Display order
    
    [ForeignKey("CategoryId")]
    public virtual Application.Domain.Lookup Category { get; set; }
}

public class MasterMarketingClass : DomainBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ColorCode { get; set; }
}

public class SubClassMarketingClass : DomainBase
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public long? MarketingclassId { get; set; }
    
    [ForeignKey("MarketingclassId")]
    public virtual MasterMarketingClass MasterMarketingClass { get; set; }
}
```

### Annual Royalty Tracking
```csharp
public class AnnualRoyality : DomainBase
{
    public long FranchiseeId { get; set; }
    public DateTime Date { get; set; }
    public decimal? Royality { get; set; }
    public decimal? Sales { get; set; }
    public bool? isMinRoyalityReached { get; set; }
    public decimal? MonthlyRoyality { get; set; }
    public decimal? Payment { get; set; }
}

public class AnnualReportType : DomainBase
{
    public string Name { get; set; }
}
```

### Legacy/Helper Entities
```csharp
public class FranchiseeSalesPayment : DomainBase
{
    // Links sales invoices to payments (legacy entity)
}

public class HoningMeasurement : DomainBase
{
    // Floor honing service measurements
}

public class HoningMeasurementDefault : DomainBase
{
    // Default honing rates
}

public class MlfsConfigurationSetting : DomainBase
{
    // Module configuration settings
}

public class UpdateMarketingClassfileupload : DomainBase
{
    // Bulk marketing class update tracking
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Module Dependencies
- **Core.Application.Domain** - `DomainBase`, `DataRecorderMetaData`, `File`, `Lookup`
- **Core.Billing.Domain** - `Invoice`, `InvoiceItem`, `CurrencyExchangeRate`
- **Core.Organizations.Domain** - `Franchisee`, `ServiceType`
- **Core.Scheduler.Domain** - `JobEstimate`, `JobScheduler`
- **Core.Geo.Domain** - `Address`, `State`, `City`, `Zip`, `Country`

### External Dependencies
- **Entity Framework 6** - All classes extend `DomainBase` for EF Code First
- **System.ComponentModel.DataAnnotations** - `[ForeignKey]`, `[Table]` attributes
- **Core.Application.Attribute** - Custom `[CascadeEntity]` for save propagation

### Cross-Module Data Flow
```
┌─────────────────────────────────────────────────────────────┐
│                    Sales Data Upload Flow                     │
└─────────────────────────────────────────────────────────────┘
    SalesDataUpload (Status: Uploaded)
           ↓
    SalesDataParsePollingAgent (background job)
           ↓
    Parse Excel → Create/Update:
      - Customer + CustomerEmail + Address
      - EstimateInvoice (from invoice rows)
      - Link to JobScheduler (if job exists)
           ↓
    Update Status: Parsed
           ↓
    Royalty Invoice Generation (monthly/weekly)
      - Query EstimateInvoice aggregate by FranchiseeId + DateRange
      - Calculate royalty basis (PriceOfService - LessDeposit)
      - Create RoyaltyInvoiceItem + AdFundInvoiceItem + NationalInvoiceItem

┌─────────────────────────────────────────────────────────────┐
│                 Annual Reconciliation Flow                    │
└─────────────────────────────────────────────────────────────┘
    AnnualSalesDataUpload (tax filing docs)
           ↓
    Compare AnnualRoyality vs SUM(WeeklyRoyality)
           ↓
    IF mismatch → Create SystemAuditRecord
           ↓
    HQ reviews → Set AuditActionId (Approved/Rejected)
```

### Related Documentation
- [Sales/Impl](../Impl/AI-CONTEXT.md) - Business logic factories and services
- [Sales/Enum](../Enum/AI-CONTEXT.md) - Status codes and type constants
- [Billing Domain](../../Billing/Domain/AI-CONTEXT.md) - Invoice and payment entities
- [Organizations Domain](../../Organizations/Domain/AI-CONTEXT.md) - Franchisee and service types
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Key Gotchas
1. **Shared PK Pattern**: `RoyaltyInvoiceItem.Id == InvoiceItem.Id`. Do NOT auto-generate IDs. Must manually assign from parent.
2. **Typo in Entity Name**: `InvoiceFileUplaod` (missing second 'l' in Upload). Present in DB schema.
3. **Denormalized Aggregates**: `Customer.TotalSales`, `AvgSales` are **not** computed properties. Must be recalculated on save.
4. **Currency Handling**: All financial entities link to `CurrencyExchangeRate`. CAD franchisees require conversion for reporting.
5. **Status Enum Mapping**: `SalesDataUploadStatus` enum values (71, 72, 73, 74) map to `Lookup.Id` primary keys. Must cast to `long` in queries.
6. **Cascade Delete**: `[CascadeEntity]` is a **custom attribute**, not EF's native `CASCADE DELETE`. Handled in repository layer.
7. **Multi-Tenant Isolation**: No explicit `FranchiseeId` on `Customer`. Isolation happens via `EstimateInvoice.FranchiseeId` queries.

### Historical Context
- **Pre-EF Migration**: Some entities have legacy field names (e.g., `Royality` instead of `Royalty`).
- **QuickBooks Integration**: `QbInvoiceNumber` fields are remnants of legacy QuickBooks sync (still used for reconciliation).
- **HomeAdvisor Leads**: Customer import pipeline was extended to support HomeAdvisor lead CSV format (handled in Impl layer).

### Performance Considerations
- **N+1 Query Risk**: `Customer.CustomerEmails` is lazy-loaded. Use `.Include(c => c.CustomerEmails)` for list queries.
- **Large File Parsing**: `SalesDataUpload` with 10K+ rows can timeout. Parsing happens in background job with progress tracking.
- **Annual Audit Queries**: `SystemAuditRecord` joins across 5 tables. Indexed on `FranchiseeId + AnnualUploadId`.
<!-- END CUSTOM SECTION -->
