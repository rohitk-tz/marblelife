<!-- AUTO-GENERATED: Header -->
# Sales — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2025-02-10T15:30:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Sales module manages the complete lifecycle of customer data, sales tracking, royalty calculations, and marketing classifications for a franchisee network. It orchestrates file-based data ingestion (CSV/Excel), parsing and validation, customer relationship management, account credits, estimate invoicing, and annual sales reporting. The module enforces business rules around sales data upload frequency, validates date ranges against payment schedules, and maintains marketing classifications that drive revenue categorization.

### Design Patterns
- **Factory Pattern**: Used extensively for domain object creation (`ICustomerFactory`, `ISalesDataUploadFactory`, `ISalesInvoiceFactory`, `IAccountCreditFactory`) to encapsulate complex object construction logic and ViewModel ↔ Domain transformations
- **Service Layer**: Business logic encapsulated in service interfaces (`ICustomerService`, `ISalesDataUploadService`, `IAccountCreditService`) with implementations in the `Impl/` folder
- **Polling Agent Pattern**: Background workers (`ISalesDataParsePollingAgent`, `IAnnualSalesDataParsePollingAgent`, `ICustomerFileUploadPollingAgent`) continuously monitor and process uploaded files
- **Parser Strategy**: File parsing abstracted through dedicated interfaces (`ISalesDataFileParser`, `ICustomerFileParser`, `IUpdateInvoiceFileParser`) allowing different file format handling
- **Repository Pattern**: Services interact with domain entities through the repository layer (implied through DomainBase inheritance)
- **Validator Pattern**: Dedicated validators (`ISalesDataUploadCreateModelValidator`) separate validation logic from business services

### Data Flow

#### Customer Management Flow
1. Customer data uploaded via `ICustomerService.Save(CustomerFileUploadCreateModel)`
2. File processed by `ICustomerFileParser` extracting customer records
3. `ICustomerFactory` transforms parsed data into `Customer` domain entities
4. `ICustomerFileUploadPollingAgent` monitors and triggers processing
5. Customers persisted with associated `Address`, `CustomerEmail`, and `MarketingClass` references
6. Marketing class can be updated via `ICustomerService.UpdateMarketingClass()`

#### Sales Data Upload Flow
1. Franchisee uploads sales data through `ISalesDataUploadService.Save(SalesDataUploadCreateModel)`
2. `ISalesDataUploadCreateModelValidator` validates date ranges against payment frequency
3. System checks for overlapping periods via `DoesOverlappingDatesExist()`
4. `ISalesDataFileParser` parses invoice and customer data from file
5. `ISalesDataParsePollingAgent.ParseFile()` continuously processes queued uploads
6. Parsed data creates/updates `Customer` entities and invoice records
7. `SalesDataUpload` entity tracks parsing status, record counts, and calculated totals
8. Invoices generated based on parsed data, royalty calculations triggered

#### Annual Sales Reporting Flow
1. Annual data uploaded via `IAnnualSalesDataUploadService.SaveUpload()`
2. `IAnnualSalesDataParsePollingAgent` processes annual reports with specialized parsing logic
3. System validates customer addresses, flags mismatches
4. Audit records created for review (`GetAnnualAuditRecord()`)
5. Staff reviews and accepts/rejects batches via `ManageBatch()`
6. Accepted data flows into royalty calculations and reporting

#### Account Credit Flow
1. Credits created through `IAccountCreditService.Save(AccountCreditEditModel)`
2. `IAccountCreditFactory` constructs `AccountCredit` domain with associated `AccountCreditItem` entries
3. Credits linked to customers and invoice numbers
4. `GetCreditForInvoice()` retrieves applicable credits during invoice processing

### Key Business Rules
- **Sales Upload Validation**: Start/end dates must align with franchisee payment frequency (weekly/monthly)
- **No Overlapping Periods**: System prevents duplicate sales data for same franchisee/date range
- **Marketing Classification**: All customers must have a marketing class (residential, commercial, etc.)
- **Currency Exchange**: Sales uploads must reference a valid currency exchange rate
- **Annual Reporting**: Special parsing rules for year-end reports with address validation
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Domain Entities (in Domain/)

```csharp
// Customer entity with marketing classification and sales aggregates
public class Customer : DomainBase
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public long? AddressId { get; set; }
    public virtual ICollection<CustomerEmail> CustomerEmails { get; set; }
    public string Phone { get; set; }
    public long ClassTypeId { get; set; } // Marketing class (residential, commercial, etc.)
    public virtual MarketingClass MarketingClass { get; set; }
    public bool ReceiveNotification { get; set; }
    public DateTime? DateCreated { get; set; }
    public decimal? TotalSales { get; set; } // Aggregated sales amount
    public int? NoOfSales { get; set; }      // Count of invoices
    public decimal? AvgSales { get; set; }   // Average invoice amount
}

// Sales data upload batch tracking parsing status and metrics
public class SalesDataUpload : DomainBase
{
    public long FranchiseeId { get; set; }
    public long FileId { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public long StatusId { get; set; } // SalesDataUploadStatus enum
    public int? NumberOfCustomers { get; set; }
    public int? NumberOfInvoices { get; set; }
    public int? NumberOfFailedRecords { get; set; }
    public int? NumberOfParsedRecords { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? AccruedAmount { get; set; }
    public long CurrencyExchangeRateId { get; set; }
    public bool IsActive { get; set; }
    public bool IsInvoiceGenerated { get; set; }
}

// Account credit memo for customer refunds/adjustments
public class AccountCredit : DomainBase
{
    public long CustomerId { get; set; }
    public string QbInvoiceNumber { get; set; } // QuickBooks reference
    public DateTime CreditedOn { get; set; }
    public virtual ICollection<AccountCreditItem> CreditMemoItems { get; set; }
}

// Marketing classification for customer segmentation
public class MarketingClass : DomainBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ColorCode { get; set; } // UI display color
    public string Alias { get; set; }
    public long? CategoryId { get; set; }
    public int? NewOrderBy { get; set; }
}

// Estimate invoice for pre-sale quotations
public class EstimateInvoice : DomainBase
{
    public long? CustomerId { get; set; }
    public long? InvoiceCustomerId { get; set; }
    public float PriceOfService { get; set; }
    public float LessDeposit { get; set; }
    public long ClassTypeId { get; set; } // Marketing class
    public long? FranchiseeId { get; set; }
    public long? EstimateId { get; set; }
    public long? SchedulerId { get; set; }
    public long? NumberOfInvoices { get; set; }
    public bool IsInvoiceChanged { get; set; }
    public bool? IsCustomerAvailable { get; set; }
    public bool IsInvoiceParsing { get; set; }
}
```

### Key Enumerations (in Enum/)

```csharp
public enum SalesDataUploadStatus
{
    Uploaded = 71,        // File uploaded, awaiting parsing
    ParseInProgress = 74, // Currently being parsed
    Parsed = 72,          // Successfully parsed
    Failed = 73           // Parsing failed
}

public enum MarketingClassType
{
    Commercial = 1, Education = 2, Hotel = 3, Residential = 4,
    BuilderTile = 5, Church = 6, Club = 7, Janitorial = 8,
    MedicalLegal = 9, Restaurant = 10, Unclassified = 11,
    Condo = 12, Bank = 13, Goverment = 14, Flooring = 15,
    Builder = 16, National = 17, Mld = 18
}
```

### View Models (in ViewModel/)

```csharp
// Sales data upload request
public class SalesDataUploadCreateModel
{
    public long FranchiseeId { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public HttpPostedFileBase File { get; set; }
    public long? PaymentFrequencyId { get; set; } // Weekly/Monthly determines validation rules
}

// Customer creation/editing
public class CustomerCreateEditModel
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public AddressEditModel Address { get; set; }
    public List<string> Emails { get; set; }
    public string Phone { get; set; }
    public long ClassTypeId { get; set; }
    public bool ReceiveNotification { get; set; }
}

// Account credit creation
public class AccountCreditEditModel
{
    public long CustomerId { get; set; }
    public string QbInvoiceNumber { get; set; }
    public DateTime CreditedOn { get; set; }
    public List<AccountCreditItemEditModel> Items { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Customer Management

#### `ICustomerService.SaveCustomer(CustomerCreateEditModel model)`
- **Input**: `CustomerCreateEditModel` with name, contact, address, emails, phone, marketing class
- **Output**: `Customer` domain entity
- **Behavior**: Creates or updates customer, cascades address and email entities, validates email uniqueness, assigns marketing class
- **Side Effects**: Persists to database, triggers cascade saves for related entities

#### `ICustomerService.DoesCustomerExists(string email)`
- **Input**: Email address string
- **Output**: Boolean indicating existence
- **Behavior**: Checks if any customer has the specified email in their CustomerEmails collection

#### `ICustomerService.GetCustomers(CustomerListFilter filter, int pageNumber, int pageSize)`
- **Input**: Filter criteria (search text, marketing class, date ranges), pagination params
- **Output**: `CustomerListModel` with paged results and total count
- **Behavior**: Queries customers with filtering, sorting, and pagination

#### `ICustomerService.UpdateMarketingClass(long id, long classTypeId)`
- **Input**: Customer ID and new marketing class ID
- **Output**: Boolean success indicator
- **Behavior**: Updates customer's marketing classification, used for customer segmentation changes

#### `ICustomerService.DownloadCustomerFile(CustomerListFilter filter, out string fileName)`
- **Input**: Filter criteria, output filename parameter
- **Output**: Boolean success indicator, filename via out parameter
- **Behavior**: Generates CSV/Excel export of filtered customer list

### Sales Data Management

#### `ISalesDataUploadService.Save(SalesDataUploadCreateModel model)`
- **Input**: `SalesDataUploadCreateModel` with franchisee ID, date range, file
- **Output**: Void (throws exceptions on failure)
- **Behavior**: 
  - Validates date range against payment frequency using `ISalesDataUploadCreateModelValidator`
  - Checks for overlapping periods via `DoesOverlappingDatesExist()`
  - Validates upload is not expired via `CheckForExpiringDocument()`
  - Creates `SalesDataUpload` entity with Uploaded status
  - Queues for background parsing by `ISalesDataParsePollingAgent`
- **Side Effects**: File upload, database insert, triggers background processing
- **Errors**: Throws if overlapping periods, invalid date range, expired document

#### `ISalesDataUploadService.DoesOverlappingDatesExist(long franchiseeId, DateTime startDate, DateTime endDate)`
- **Input**: Franchisee ID and date range
- **Output**: Boolean indicating overlap
- **Behavior**: Queries existing uploads for the franchisee, checks for date range conflicts

#### `ISalesDataUploadService.GetBatchList(SalesDataListFilter filter, int pageNumber, int pageSize)`
- **Input**: Filter (franchisee, status, date range), pagination
- **Output**: `SalesDataUploadListModel` with paged uploads
- **Behavior**: Retrieves sales data upload batches with metrics

#### `ISalesDataUploadService.Reparse(long id)`
- **Input**: SalesDataUpload ID
- **Output**: Boolean success indicator
- **Behavior**: Resets upload status to trigger re-parsing, useful for failed uploads

#### `IAnnualSalesDataUploadService.SaveUpload(AnnualDataUploadCreateModel model)`
- **Input**: Annual upload model with year, franchisee, file
- **Output**: Boolean success indicator
- **Behavior**: Handles year-end sales data upload with specialized validation and parsing rules
- **Side Effects**: Creates annual upload record, triggers `IAnnualSalesDataParsePollingAgent`

#### `IAnnualSalesDataUploadService.GetAnnualAuditRecord(SalesDataListFilter filter)`
- **Input**: Filter criteria
- **Output**: `AnnualAuditSalesListModel` with audit records
- **Behavior**: Retrieves annual data requiring staff review (address mismatches, data anomalies)

#### `IAnnualSalesDataUploadService.ManageBatch(bool isAccept, long batchId)`
- **Input**: Accept/reject flag and batch ID
- **Output**: Boolean success
- **Behavior**: Approves or rejects annual data batch after review, triggers downstream processing if accepted

### Account Credit Management

#### `IAccountCreditService.Save(AccountCreditEditModel model, long currencyExchangeRateId)`
- **Input**: Credit memo details and currency exchange rate
- **Output**: `AccountCredit` domain entity
- **Behavior**: Creates account credit with line items, links to customer and QB invoice
- **Side Effects**: Persists credit memo, affects customer balance calculations

#### `IAccountCreditService.GetCreditForInvoice(long franchiseeId, long invoiceId)`
- **Input**: Franchisee and invoice IDs
- **Output**: `FranchiseeAccountCreditViewModel` with applicable credits
- **Behavior**: Retrieves credits that can be applied to the specified invoice

#### `IAccountCreditService.DeleteAccountCredit(long accountCreditId)`
- **Input**: Account credit ID
- **Output**: Boolean success
- **Behavior**: Soft deletes account credit, marks as inactive

### Marketing Classification

#### `IMarketingClassService.GetMarketingClassByInvoiceId(long invoiceId)`
- **Input**: Invoice ID
- **Output**: Marketing class name string
- **Behavior**: Retrieves marketing classification from invoice's customer

#### `IMarketingClassService.GetMarketingClassByPaymentId(long paymentId)`
- **Input**: Payment ID
- **Output**: Marketing class name string
- **Behavior**: Navigates payment → invoice → customer → marketing class

### File Parsing (Background Processing)

#### `ISalesDataParsePollingAgent.ParseFile()`
- **Input**: None (polls for pending uploads)
- **Output**: Void
- **Behavior**: 
  - Queries `SalesDataUpload` entities with Uploaded status
  - Uses `ISalesDataFileParser` to parse file contents
  - Creates/updates `Customer` and invoice entities
  - Updates upload status to Parsed or Failed
  - Populates NumberOfCustomers, NumberOfInvoices, TotalAmount metrics
- **Side Effects**: Heavy database writes, updates upload status
- **Errors**: Logs parsing failures, updates FailedRecords count

#### `ICustomerFileUploadPollingAgent` (similar pattern)
- Processes customer-only file uploads
- Uses `ICustomerFileParser` for parsing

#### `IAnnualSalesDataParsePollingAgent` (similar pattern)
- Processes annual reports with specialized validation
- Creates audit records for manual review

### Factory Interfaces

#### `ICustomerFactory.CreateDomain(CustomerCreateEditModel model, Customer domain)`
- **Input**: View model and optional existing domain entity
- **Output**: `Customer` domain entity
- **Behavior**: Maps ViewModel → Domain, handles address cascade, email collection updates

#### `ISalesDataUploadFactory`, `ISalesInvoiceFactory`, `IAccountCreditFactory`
- Similar factory patterns for their respective domain entities
- Encapsulate complex mapping and validation logic

### Validators

#### `ISalesDataUploadCreateModelValidator.ValidateDates(SalesDataUploadCreateModel model)`
- **Input**: Upload model
- **Output**: Boolean validity
- **Behavior**: Checks if date range matches payment frequency (weekly = 7 days, monthly = full month)

### Helpers

#### `IDownloadFileHelperService`
- Provides file download utilities for customer lists, invoice exports, annual reports
- Generates CSV/Excel files with proper formatting

#### `FileParserHelper` (static class)
- Shared parsing utilities
- Date parsing, text cleaning, CSV/Excel reading helpers
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application](../../Application/.context/CONTEXT.md)** — Base domain classes (`DomainBase`, `DataRecorderMetaData`), lookups
- **[Core.Organizations](../../Organizations/.context/CONTEXT.md)** — `Franchisee` entity, franchisee management
- **[Core.Geo](../../Geo/.context/CONTEXT.md)** — `Address` entity for customer locations
- **[Core.Billing](../../Billing/.context/CONTEXT.md)** — Invoice entities, currency exchange rates, payment processing
- **[Core.Scheduler](../../Scheduler/.context/CONTEXT.md)** — `JobEstimate`, `JobScheduler` for estimate invoices
- **[Core.Reports](../../Reports/.context/CONTEXT.md)** — `FranchiseeSales` reporting aggregates

### External Dependencies
- **Entity Framework** — ORM for database persistence, lazy loading of navigation properties
- **System.ComponentModel.DataAnnotations** — Validation attributes, foreign key annotations
- **CSV/Excel Parsing Libraries** (implied) — For file upload parsing

### Database Schema Notes
- Cascade deletes configured via `[CascadeEntity]` attributes
- Foreign key constraints enforce referential integrity
- `DataRecorderMetaData` tracks created/modified timestamps and users
- `Lookup` tables provide enumeration-like reference data
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Performance Considerations
- **File Parsing**: Large sales files (1000+ invoices) can take minutes to parse; polling agents should have adequate delays
- **Customer Queries**: `GetCustomerByEmail()` accepts a `customerRepository` parameter for in-memory filtering (optimization for batch operations)
- **Lazy Loading**: Be cautious of N+1 queries when accessing navigation properties like `Customer.Address` or `Customer.MarketingClass`

### Common Pitfalls
- **Overlapping Uploads**: Always call `DoesOverlappingDatesExist()` before creating uploads to prevent data integrity issues
- **Marketing Class Required**: All customers MUST have a valid `ClassTypeId`; unclassified customers cause reporting failures
- **Date Validation**: Weekly uploads must be exactly 7 days, monthly uploads must cover a full calendar month (start = 1st, end = last day)

### Testing Notes
- Mock `ISalesDataFileParser` in unit tests to avoid file I/O dependencies
- Use in-memory entity collections for repository parameters in customer lookup methods
- Test overlapping date scenarios thoroughly (same dates, partial overlaps, contained ranges)

### Extension Points
- New file formats: Implement `ISalesDataFileParser` or `ICustomerFileParser`
- Custom marketing classes: Add to `MarketingClassType` enum and update classification logic
- Additional validations: Extend `ISalesDataUploadCreateModelValidator`
<!-- END CUSTOM SECTION -->
