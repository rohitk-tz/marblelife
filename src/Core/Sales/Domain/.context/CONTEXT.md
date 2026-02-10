<!-- AUTO-GENERATED: Header -->
# Sales/Domain — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2025-02-10T15:35:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Domain folder contains 27 entity classes representing the core data model for the Sales module. These entities define the database schema through Entity Framework Code-First approach, establishing relationships between customers, sales uploads, invoices, credits, marketing classifications, and estimate documents. Each entity inherits from `DomainBase` (providing Id, soft delete tracking) and uses EF annotations for foreign keys, cascade behaviors, and navigation properties.

### Design Patterns
- **Active Record Pattern**: Entities contain both data and database mapping metadata (via EF attributes)
- **Aggregate Root**: `Customer` acts as aggregate root for `CustomerEmail` collection
- **Aggregate Root**: `AccountCredit` acts as aggregate root for `AccountCreditItem` collection
- **Aggregate Root**: `EstimateInvoice` contains `EstimateInvoiceService` and `EstimateInvoiceCustomer` child entities
- **Hierarchy Pattern**: `EstimateInvoiceService` supports parent-child relationships via `ParentId` for service bundling
- **Inheritance**: `RoyaltyInvoiceItem`, `NationalInvoiceItem`, `AdFundInvoiceItem`, `FranchiseeFeeEmailInvoiceItem` all inherit from or reference `InvoiceItem` as specialized invoice line types
- **Soft Delete**: Entities track deletion status without physical removal from database

### Entity Categories

#### **Customer Entities**
- `Customer`: Master customer record with sales metrics
- `CustomerEmail`: Email addresses associated with customers (many-to-one)
- `CustomerDataUpload`: Tracks customer-only file uploads

#### **Sales Upload Entities**
- `SalesDataUpload`: Batch upload of sales data with parsing status and metrics
- `AnnualSalesDataUpload`: Year-end sales upload with audit tracking
- `InvoiceFileUpload`: Tracks invoice file uploads
- `UpdateMarketingClassfileupload`: Tracks marketing class bulk update uploads

#### **Marketing Classification**
- `MarketingClass`: Customer segmentation categories (18 types)
- `MasterMarketingClass`: Top-level classification hierarchy
- `SubClassMarketingClass`: Sub-category classifications

#### **Invoice Line Items** (extending Core.Billing.InvoiceItem)
- `RoyaltyInvoiceItem`: Royalty charges with percentage and date range
- `NationalInvoiceItem`: National account invoice items
- `AdFundInvoiceItem`: Advertising fund contributions
- `FranchiseeFeeEmailInvoiceItem`: Email service fees

#### **Estimate/Quote Entities**
- `EstimateInvoice`: Pre-sale quote document
- `EstimateInvoiceCustomer`: Customer information on estimate (may differ from master Customer)
- `EstimateInvoiceService`: Line items on estimate with service details
- `EstimateInvoiceServiceDescription`: Service description templates

#### **Credit/Payment Entities**
- `AccountCredit`: Credit memo with line items
- `AccountCreditItem`: Individual credit line item
- `FranchiseeSalesPayment`: Links sales records to invoices and payments

#### **Royalty/Reporting**
- `AnnualRoyality`: Annual royalty calculation results
- `AnnualReportType`: Report type categorization

#### **Configuration**
- `MlfsConfigurationSetting`: System configuration settings
- `HoningMeasurement`: Honing measurement data
- `HoningMeasurementDefault`: Default honing measurements
- `SystemAuditRecord`: Audit trail for system changes

### Relationships
```
Customer (1) ──→ (n) CustomerEmail
Customer (1) ──→ (1) Address [Core.Geo]
Customer (1) ──→ (1) MarketingClass
Customer (1) ──→ (n) AccountCredit

SalesDataUpload (1) ──→ (1) Franchisee [Core.Organizations]
SalesDataUpload (1) ──→ (1) File [Core.Application]
SalesDataUpload (1) ──→ (n) FranchiseeSalesPayment
SalesDataUpload (1) ──→ (1) CurrencyExchangeRate [Core.Billing]

AnnualSalesDataUpload (1) ──→ (1) SalesDataUpload (optional)
AnnualSalesDataUpload (1) ──→ (1) Franchisee

AccountCredit (1) ──→ (n) AccountCreditItem
AccountCredit (1) ──→ (1) Customer

EstimateInvoice (1) ──→ (n) EstimateInvoiceService
EstimateInvoice (1) ──→ (1) Customer (optional)
EstimateInvoice (1) ──→ (1) EstimateInvoiceCustomer (optional)
EstimateInvoice (1) ──→ (1) JobEstimate [Core.Scheduler]
EstimateInvoice (1) ──→ (1) MarketingClass

EstimateInvoiceService (self-referencing) ──→ ParentId for bundling

RoyaltyInvoiceItem (1) ──→ (1) InvoiceItem [Core.Billing]
NationalInvoiceItem (1) ──→ (1) InvoiceItem [Core.Billing]
AdFundInvoiceItem (1) ──→ (1) InvoiceItem [Core.Billing]
FranchiseeFeeEmailInvoiceItem (1) ──→ (1) InvoiceItem [Core.Billing]

FranchiseeSalesPayment ──→ FranchiseeSales, Invoice, Payment, SalesDataUpload
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Entity Definitions

### Customer.cs
```csharp
public class Customer : DomainBase
{
    public string Name { get; set; }                    // Business or person name
    public string ContactPerson { get; set; }           // Primary contact
    public long? AddressId { get; set; }                
    public virtual ICollection<CustomerEmail> CustomerEmails { get; set; } // Multiple emails
    public string Phone { get; set; }
    public long DataRecorderMetaDataId { get; set; }
    public virtual Address Address { get; set; }        // [Core.Geo]
    public long ClassTypeId { get; set; }               // MarketingClass FK
    public virtual MarketingClass MarketingClass { get; set; }
    public bool ReceiveNotification { get; set; }       // Marketing opt-in
    public DateTime? DateCreated { get; set; }
    // Aggregated sales metrics (updated during parsing)
    public decimal? TotalSales { get; set; }            // Sum of all invoices
    public int? NoOfSales { get; set; }                 // Count of invoices
    public decimal? AvgSales { get; set; }              // Average invoice amount
}
```

**Purpose**: Master customer record tracking contact information, marketing classification, and aggregated sales history.

**Key Behaviors**:
- `CustomerEmails` collection managed via cascade operations
- `TotalSales`, `NoOfSales`, `AvgSales` computed during sales data parsing
- Soft delete supported through `DomainBase.IsDeleted`

---

### CustomerEmail.cs
```csharp
public class CustomerEmail : DomainBase
{
    public long CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
    public DateTime DateCreated { get; set; }
    public string Email { get; set; }                   // Email address
}
```

**Purpose**: Supports multiple email addresses per customer for notifications and communications.

---

### SalesDataUpload.cs
```csharp
public class SalesDataUpload : DomainBase
{
    public long FranchiseeId { get; set; }
    public long FileId { get; set; }                    // Uploaded file reference
    public DateTime PeriodStartDate { get; set; }       // Sales period start
    public DateTime PeriodEndDate { get; set; }         // Sales period end
    public long StatusId { get; set; }                  // SalesDataUploadStatus enum
    public long? ParsedLogFileId { get; set; }          // Parsing error log
    public int? NumberOfCustomers { get; set; }         // Customers found in file
    public int? NumberOfInvoices { get; set; }          // Invoices parsed
    public int? NumberOfFailedRecords { get; set; }     // Parse errors
    public int? NumberOfParsedRecords { get; set; }     // Successful parses
    public decimal? TotalAmount { get; set; }           // Sum of invoice amounts
    public decimal? PaidAmount { get; set; }            // Sum of payments
    public decimal? AccruedAmount { get; set; }         // Unpaid balance
    public long DataRecorderMetaDataId { get; set; }
    public long CurrencyExchangeRateId { get; set; }
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    public virtual Franchisee Franchisee { get; set; }
    public virtual File File { get; set; }
    public virtual Lookup Lookup { get; set; }          // Status lookup
    public bool IsActive { get; set; }
    public bool IsInvoiceGenerated { get; set; }        // Royalty invoices created
}
```

**Purpose**: Tracks a batch upload of sales data from a franchisee, including parsing status, metrics, and error tracking.

**Key Metrics**:
- Counts populated by `ISalesDataParsePollingAgent` during background parsing
- Status progresses: Uploaded → ParseInProgress → Parsed/Failed
- `IsInvoiceGenerated` flag indicates royalty invoices have been created

---

### AnnualSalesDataUpload.cs
```csharp
public class AnnualSalesDataUpload : DomainBase
{
    public long FranchiseeId { get; set; }
    public long FileId { get; set; }
    public long? SalesDataUploadId { get; set; }        // Optional link to regular upload
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public long StatusId { get; set; }
    public long? ParsedLogFileId { get; set; }
    public int? NoOfFailedRecords { get; set; }
    public int? NoOfParsedRecords { get; set; }
    public int? NoOfMismatchedRecords { get; set; }     // Address/data mismatches
    public decimal? WeeklyRoyality { get; set; }        // Calculated weekly royalty
    public decimal? AnnualRoyality { get; set; }        // Calculated annual royalty
    public decimal? TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public bool? IsAuditAddressParsing { get; set; }    // Trigger address validation
    public long DataRecorderMetaDataId { get; set; }
    public long CurrencyExchangeRateId { get; set; }
    public long AuditActionId { get; set; }             // Approve/Reject status
    public virtual SalesDataUpload SalesDataUpload { get; set; }
    public virtual Franchisee Franchisee { get; set; }
    public virtual File File { get; set; }
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    public virtual Lookup Lookup { get; set; }          // Status
    public virtual Lookup AuditAction { get; set; }     // Audit action
}
```

**Purpose**: Year-end sales data upload with specialized audit workflow for address validation and manual review before acceptance.

**Audit Workflow**:
- `NoOfMismatchedRecords` tracks data quality issues
- `AuditActionId` indicates approval status (pending/approved/rejected)
- `IsAuditAddressParsing` triggers address validation against customer master

---

### AccountCredit.cs
```csharp
public class AccountCredit : DomainBase
{
    public long CustomerId { get; set; }
    public string QbInvoiceNumber { get; set; }         // QuickBooks reference
    public DateTime CreditedOn { get; set; }
    public virtual Customer Customer { get; set; }
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<AccountCreditItem> CreditMemoItems { get; set; }
}
```

**Purpose**: Credit memo header linking customer, QB invoice, and line items for refunds/adjustments.

---

### AccountCreditItem.cs
```csharp
public class AccountCreditItem : DomainBase
{
    public long AccountCreditId { get; set; }
    public string Description { get; set; }             // Line item description
    public decimal Amount { get; set; }                 // Credit amount
    public long CurrencyExchangeRateId { get; set; }
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
}
```

**Purpose**: Individual line item on a credit memo with description and amount.

---

### MarketingClass.cs
```csharp
public class MarketingClass : DomainBase
{
    public string Name { get; set; }                    // e.g., "Commercial", "Residential"
    public string Description { get; set; }
    public string ColorCode { get; set; }               // Hex color for UI display
    public string Alias { get; set; }                   // Short name
    public long? CategoryId { get; set; }               // Optional category lookup
    public virtual Lookup Category { get; set; }
    public int? NewOrderBy { get; set; }                // Sort order
}
```

**Purpose**: Customer segmentation categories (18 types) for marketing campaigns and revenue reporting.

**Types**: Commercial, Education, Hotel, Residential, BuilderTile, Church, Club, Janitorial, MedicalLegal, Restaurant, Unclassified, Condo, Bank, Government, Flooring, Builder, National, MLD

---

### EstimateInvoice.cs
```csharp
public class EstimateInvoice : DomainBase
{
    public long DataRecorderMetaDataId { get; set; }
    public long? CustomerId { get; set; }               // Link to master Customer
    public virtual Customer Customer { get; set; }
    public long? InvoiceCustomerId { get; set; }        // Estimate-specific customer info
    public virtual EstimateInvoiceCustomer EstimateInvoiceCustomer { get; set; }
    public float PriceOfService { get; set; }
    public float LessDeposit { get; set; }
    public long ClassTypeId { get; set; }               // Marketing class
    public virtual MarketingClass MarketingClass { get; set; }
    public long? FranchiseeId { get; set; }
    public virtual Franchisee Franchisee { get; set; }
    public long? EstimateId { get; set; }
    public virtual JobEstimate JobEstimate { get; set; } // [Core.Scheduler]
    public long? SchedulerId { get; set; }
    public virtual JobScheduler JobScheduler { get; set; }
    public long? NumberOfInvoices { get; set; }
    public string Option { get; set; }
    public string Notes { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public bool IsInvoiceChanged { get; set; }
    public bool? IsCustomerAvailable { get; set; }
    public bool IsInvoiceParsing { get; set; }          // Parsing in progress
}
```

**Purpose**: Pre-sale estimate/quote document with pricing options and service details.

**Notes**:
- Can reference master `Customer` or use estimate-specific `EstimateInvoiceCustomer`
- `Option1/2/3` store pricing alternatives
- Links to job estimate and scheduler for workflow integration

---

### EstimateInvoiceService.cs
```csharp
public class EstimateInvoiceService : DomainBase
{
    public string ServiceName { get; set; }
    public string ServiceType { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }                // Room/area location
    public string TypeOfService { get; set; }           // Polish, seal, restore, etc.
    public string StoneType { get; set; }               // Marble, granite, etc.
    public string StoneColor { get; set; }
    public string Price { get; set; }
    public long EstimateInvoiceId { get; set; }
    public virtual EstimateInvoice EstimateInvoice { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Notes { get; set; }
    public string PriceNotes { get; set; }
    public string StoneType2 { get; set; }
    public int InvoiceNumber { get; set; }
    public bool IsCross { get; set; }
    public long? ParentId { get; set; }                 // For bundled services
    public virtual EstimateInvoiceService EstimateInvoiceServiceDomain { get; set; }
    public bool IsBundle { get; set; }
    public bool IsActive { get; set; }
    public string BundleName { get; set; }
    public bool IsMainBundle { get; set; }
    public string Alias { get; set; }
    public long? InvoiceImageId { get; set; }
    public virtual JobEstimateImage JobEstimateImage { get; set; }
    public long? ServiceTagId { get; set; }
    public virtual Lookup ServiceTag { get; set; }
}
```

**Purpose**: Line item on estimate invoice with detailed service specification (stone type, location, pricing options).

**Bundling**: Supports service bundles via `ParentId` self-reference (e.g., "Marble Restoration Package" containing polish + seal services).

---

### RoyaltyInvoiceItem.cs / NationalInvoiceItem.cs / AdFundInvoiceItem.cs
```csharp
public class RoyaltyInvoiceItem : DomainBase
{
    [ForeignKey("InvoiceItem")]
    public override long Id { get; set; }               // Shares PK with InvoiceItem
    public decimal? Percentage { get; set; }            // Royalty percentage
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public virtual InvoiceItem InvoiceItem { get; set; } // [Core.Billing]
}
```

**Purpose**: Specialized invoice line items extending `InvoiceItem` with domain-specific fields.

**Pattern**: Table-per-type inheritance where specialized entities share primary key with base `InvoiceItem` entity.

---

### FranchiseeSalesPayment.cs
```csharp
public class FranchiseeSalesPayment : DomainBase
{
    public long FranchiseeSalesId { get; set; }
    public long InvoiceId { get; set; }
    public long PaymentId { get; set; }
    public long SalesDataUploadId { get; set; }
    public virtual FranchiseeSales FranchiseeSales { get; set; }
    public virtual Invoice Invoice { get; set; }
    public virtual Payment Payment { get; set; }
    public virtual SalesDataUpload SalesDataUpload { get; set; }
}
```

**Purpose**: Junction table linking sales records to invoices, payments, and upload batches for traceability.

---

### Other Supporting Entities

**CustomerDataUpload**: Tracks customer-only file uploads (no invoice data).

**InvoiceFileUpload**: Tracks invoice file uploads.

**UpdateMarketingClassfileupload**: Tracks bulk marketing class update uploads.

**EstimateInvoiceCustomer**: Customer snapshot for estimates (may differ from master Customer).

**EstimateInvoiceServiceDescription**: Service description templates.

**MasterMarketingClass** / **SubClassMarketingClass**: Hierarchical marketing classification.

**AnnualRoyality**: Annual royalty calculation results.

**AnnualReportType**: Categorizes annual report types.

**HoningMeasurement** / **HoningMeasurementDefault**: Stone honing measurement data.

**MlfsConfigurationSetting**: System configuration key-value pairs.

**SystemAuditRecord**: Audit trail for system changes.

**FranchiseeFeeEmailInvoiceItem**: Email service fee invoice items.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application](../../../Application/.context/CONTEXT.md)** — `DomainBase`, `DataRecorderMetaData`, `Lookup`, `File`
- **[Core.Organizations](../../../Organizations/.context/CONTEXT.md)** — `Franchisee`, `FranchiseeSales`
- **[Core.Geo](../../../Geo/.context/CONTEXT.md)** — `Address` entity
- **[Core.Billing](../../../Billing/.context/CONTEXT.md)** — `Invoice`, `InvoiceItem`, `Payment`, `CurrencyExchangeRate`
- **[Core.Scheduler](../../../Scheduler/.context/CONTEXT.md)** — `JobEstimate`, `JobScheduler`, `JobEstimateImage`
- **[Core.Reports](../../../Reports/.context/CONTEXT.md)** — `FranchiseeSales`

### External Dependencies
- **Entity Framework** — Code-First ORM with annotations
- **System.ComponentModel.DataAnnotations** — `[ForeignKey]`, `[Table]` attributes
- **Core.Application.Attribute** — `[CascadeEntity]` for cascade delete configuration
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Schema Design Notes
- **Soft Delete**: All entities inherit `DomainBase.IsDeleted` - never hard delete records
- **Audit Tracking**: `DataRecorderMetaData` tracks creation/modification timestamp and user
- **Currency**: Multi-currency support via `CurrencyExchangeRateId` on credits and uploads
- **Cascade Operations**: `[CascadeEntity]` attribute configures EF cascade deletes

### Data Integrity
- **Customer.ClassTypeId**: Required field - customers without marketing class cause reporting errors
- **SalesDataUpload Date Ranges**: Must not overlap for same franchisee
- **InvoiceItem Inheritance**: Specialized items (`RoyaltyInvoiceItem`, etc.) share PK with base `InvoiceItem`

### Performance Considerations
- **Lazy Loading**: Navigation properties load on access - use `.Include()` to eager load
- **Customer Aggregates**: `TotalSales`, `AvgSales`, `NoOfSales` denormalized for query performance
- **Upload Metrics**: Counts populated asynchronously during parsing - don't rely on them until status = Parsed

### Common Patterns
- **Aggregate Roots**: `Customer`, `AccountCredit`, `EstimateInvoice` manage child collections
- **Junction Tables**: `FranchiseeSalesPayment` provides many-to-many with additional data
- **Status Tracking**: Uploads use `StatusId` lookup (Uploaded/ParseInProgress/Parsed/Failed)
- **File References**: Uploads link to `File` entity for blob storage access
<!-- END CUSTOM SECTION -->
