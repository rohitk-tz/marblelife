<!-- AUTO-GENERATED: Header -->
# Sales/Impl — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2025-02-10T15:45:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Impl folder contains 30 concrete implementation classes for the Sales module's service interfaces. These classes implement the business logic for customer management, sales data parsing, file uploads, account credits, marketing classifications, and royalty reporting. The implementations orchestrate database operations, file I/O, data transformations, and workflow state management.

### Design Patterns
- **Service Layer Pattern**: All classes implement corresponding interfaces from the parent folder (e.g., `CustomerService` implements `ICustomerService`)
- **Dependency Injection**: Services use constructor injection with `[DefaultImplementation]` attribute for auto-wiring
- **Repository Pattern**: Database access via `IRepository<T>` and `IUnitOfWork` for transaction management
- **Factory Pattern**: Dedicated factory classes (`CustomerFactory`, `AccountCreditFactory`, etc.) encapsulate ViewModel ↔ Domain transformations
- **Polling Agent Pattern**: Three polling agents continuously monitor for pending uploads and process them in the background
- **Parser Strategy Pattern**: Multiple parser implementations (`SalesDataFileParser`, `CustomerFileParser`, `UpdateInvoiceFileParser`) handle different file formats
- **Validator Pattern**: `SalesDataUploadCreateModelValidator` separates validation logic from service methods

### Implementation Categories

#### **Service Implementations** (Business Logic)
- `CustomerService`: Customer CRUD, search, email management, marketing class updates
- `SalesDataUploadService`: Upload validation, batch management, overlap detection
- `AnnualSalesDataUploadService`: Year-end upload processing, audit workflow
- `AccountCreditService`: Credit memo creation, retrieval, deletion
- `MarketingClassService`: Marketing classification lookups
- `RoyaltyReportService`: Royalty report generation
- `SalesFunnelNationalService`: National account funnel reporting
- `SalesInvoiceService`: Invoice file downloads and processing
- `InvoiceItemUpdateInfoService`: Invoice item update tracking
- `DownloadFileHelperService`: File export utilities (CSV/Excel)

#### **Factory Implementations** (Object Creation)
- `CustomerFactory`: Customer entity ↔ ViewModel transformations
- `AccountCreditFactory`: AccountCredit entity creation from ViewModels
- `AccountCreditItemFactory`: AccountCreditItem line item creation
- `SalesDataUploadFactory`: SalesDataUpload entity creation
- `SalesInvoiceFactory`: Invoice entity creation from parsed data
- `SalesFunnelFactory`: Sales funnel report ViewModel creation
- `RoyaltyReportFactory`: Royalty report ViewModel construction

#### **Polling Agents** (Background Processing)
- `SalesDataParsePollingAgent`: Processes queued sales data uploads (main parser)
- `AnnualSalesDataParsePollingAgent`: Processes annual reports with audit validation
- `CustomerFileUploadPollingAgent`: Processes customer-only file uploads
- `SalesDataUploadReminderPollingAgent`: Sends reminders for overdue uploads

#### **File Parsers** (Data Extraction)
- `SalesDataFileParser`: Parses CSV/Excel files with invoice and customer data
- `CustomerFileParser`: Parses customer-only files
- `UpdateInvoiceFileParser`: Parses invoice update files
- `InvoiceFileParserService`: Invoice file parsing orchestration

#### **Validators**
- `SalesDataUploadCreateModelValidator`: Validates date ranges against payment frequencies

#### **Notification Services**
- `UpdatingInvoiceNotificationServices`: Sends notifications for invoice updates
- `UpdatingInvoiceIdsNotificationServices`: Tracks invoice update notifications

#### **Support Classes**
- `InvoiceInfoEditModel`: ViewModel for invoice editing
- `SaveModelStats`: Statistics tracking for save operations

### Data Flow

#### Sales Data Upload Processing Flow
```
1. User uploads file → SalesDataUploadService.Save()
   ↓
2. SalesDataUploadCreateModelValidator validates date range
   ↓
3. SalesDataUploadFactory creates SalesDataUpload entity (status = Uploaded)
   ↓
4. Entity saved to database
   ↓
5. SalesDataParsePollingAgent polls for Uploaded status
   ↓
6. SalesDataFileParser extracts customers and invoices from file
   ↓
7. CustomerService creates/updates Customer entities
   ↓
8. InvoiceService creates Invoice and InvoiceItem entities
   ↓
9. FranchiseeSalesPayment links created for traceability
   ↓
10. SalesDataUpload updated: status = Parsed, metrics populated
```

#### Annual Upload Audit Flow
```
1. User uploads annual file → AnnualSalesDataUploadService.SaveUpload()
   ↓
2. AnnualSalesDataUpload entity created (AuditActionId = Pending)
   ↓
3. AnnualSalesDataParsePollingAgent processes file
   ↓
4. Parser validates customer addresses against master data
   ↓
5. Mismatches flagged (NoOfMismatchedRecords incremented)
   ↓
6. Staff reviews via GetAnnualAuditRecord()
   ↓
7. Staff fixes issues, calls ManageBatch(isAccept = true/false)
   ↓
8. If accepted: AuditActionId = Approved, data flows into royalty calculations
   If rejected: AuditActionId = Rejected, data discarded
```

#### Customer Management Flow
```
1. CustomerService.SaveCustomer(CustomerCreateEditModel)
   ↓
2. CustomerFactory.CreateDomain() transforms ViewModel → Domain
   ↓
3. Address cascaded via AddressFactory
   ↓
4. CustomerEmail collection managed (add/update/delete)
   ↓
5. Customer entity persisted with all relationships
   ↓
6. Sales metrics (TotalSales, AvgSales) updated during parsing
```

### Key Business Logic

**Upload Validation**:
- `SalesDataUploadCreateModelValidator` ensures date ranges match payment frequency
  - Weekly: Exactly 7 days (Monday-Sunday typical)
  - Monthly: Full calendar month (1st to last day)
- `DoesOverlappingDatesExist()` prevents duplicate uploads for same period
- `CheckForExpiringDocument()` validates upload within allowable time window

**Parsing Logic**:
- **SalesDataParsePollingAgent**: 
  - Queries uploads with StatusId = Uploaded
  - Updates status to ParseInProgress
  - Reads file line-by-line, extracting customer and invoice data
  - Creates/updates Customer entities (matches by email/phone/address)
  - Creates Invoice and InvoiceItem entities
  - Links via FranchiseeSalesPayment
  - Updates Customer.TotalSales, NoOfSales, AvgSales aggregates
  - Handles errors: logs to ParsedLogFileId, increments NumberOfFailedRecords
  - Final status: Parsed (success) or Failed (errors)

**Account Credits**:
- `AccountCreditService.Save()` creates credit memos with line items
- Credits linked to customer and QB invoice number
- `GetCreditForInvoice()` retrieves applicable credits during invoice processing
- Credits affect customer balance calculations

**Download/Export**:
- `DownloadFileHelperService` generates CSV/Excel exports
- `CustomerService.DownloadCustomerFile()` exports filtered customer lists
- `SalesInvoiceService.DownloadInvoiceFile()` exports invoice data
- `AnnualSalesDataUploadService.DownloadAnnualDataFile()` exports annual reports
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Key Implementation Classes

### Service Implementations

#### CustomerService.cs
**Implements**: `ICustomerService`

**Dependencies**: 
- `IRepository<Customer>`, `ICustomerFactory`, `IExcelFileCreator`, `IFileService`
- Address/Email factories and repositories for cascade operations

**Key Methods**:
```csharp
Customer SaveCustomer(CustomerCreateEditModel model)
// Creates or updates customer with cascaded address and email entities
// Validates email uniqueness, assigns marketing class
// Updates customer aggregates during sales parsing

bool DoesCustomerExists(string email)
// Checks if email exists in CustomerEmail collection

CustomerListModel GetCustomers(CustomerListFilter filter, int pageNumber, int pageSize)
// Paginated customer search with filtering by name, email, marketing class, date range
// Includes TotalSales, AvgSales, NoOfSales metrics

bool UpdateMarketingClass(long id, long classTypeId)
// Changes customer marketing classification

bool DownloadCustomerFile(CustomerListFilter filter, out string fileName)
// Exports filtered customer list to Excel
```

**Business Logic**:
- Email uniqueness validation across CustomerEmails
- Customer matching during parsing: email → phone → name+address
- Sales aggregates updated asynchronously during upload parsing
- Marketing class drives segmentation and reporting

---

#### SalesDataUploadService.cs
**Implements**: `ISalesDataUploadService`

**Dependencies**:
- `IRepository<SalesDataUpload>`, `ISalesDataUploadCreateModelValidator`
- `ISalesDataUploadFactory`, `IFileService`

**Key Methods**:
```csharp
void Save(SalesDataUploadCreateModel model)
// Validates date range, checks for overlaps, creates upload entity
// Throws exceptions if validation fails

bool DoesOverlappingDatesExist(long franchiseeId, DateTime startDate, DateTime endDate)
// Queries existing uploads for date range conflicts

bool CheckValidRangeForSalesUpload(SalesDataUploadCreateModel model)
// Validates date range matches payment frequency

bool Reparse(long id)
// Resets upload status to trigger re-parsing

void SaveAnnualUpload(SalesDataUploadCreateModel model, SalesDataUpload salesdataUpload)
// Handles annual upload creation with special validation

bool CheckForExpiringDocument(SalesDataUploadCreateModel model)
// Validates upload is within allowable time window
```

**Validation Rules**:
- Weekly uploads: `PeriodEndDate - PeriodStartDate == 7 days`
- Monthly uploads: `PeriodStartDate.Day == 1 && PeriodEndDate == LastDayOfMonth`
- No overlapping date ranges for same franchisee
- Upload must be within document expiration window

---

#### AnnualSalesDataUploadService.cs
**Implements**: `IAnnualSalesDataUploadService`

**Dependencies**:
- `IRepository<AnnualSalesDataUpload>`, file services, factories

**Key Methods**:
```csharp
bool SaveUpload(AnnualDataUploadCreateModel model)
// Creates annual upload with AuditActionId = Pending

AnnualAuditSalesListModel GetAnnualAuditRecord(SalesDataListFilter filter)
// Retrieves uploads requiring manual review (address mismatches, data anomalies)

bool ManageBatch(bool isAccept, long batchId)
// Approves or rejects annual batch after staff review
// If approved: AuditActionId = Approved, data flows into system
// If rejected: AuditActionId = Rejected, data discarded

bool UpdateCustomerAddress(AnnualSalesDataCustomerViewModel filter)
// Corrects flagged customer address mismatches

bool ReparseAnnualReport(long? Id)
// Triggers re-parsing of annual report
```

**Audit Workflow**:
1. Upload parsed with `IsAuditAddressParsing = true`
2. Address validation compares against Customer master data
3. Mismatches flagged in `NoOfMismatchedRecords`
4. Staff reviews via `GetAnnualAuditRecord()`
5. Staff corrects issues with `UpdateCustomerAddress()`
6. Staff approves with `ManageBatch(isAccept = true)`
7. Data flows into royalty calculations

---

#### AccountCreditService.cs
**Implements**: `IAccountCreditService`

**Dependencies**:
- `IRepository<AccountCredit>`, `IAccountCreditFactory`, `IAccountCreditItemFactory`

**Key Methods**:
```csharp
AccountCredit Save(AccountCreditEditModel model, long currencyExchangeRateId)
// Creates credit memo with line items
// Links to customer and QB invoice

FranchiseeAccountCreditViewModel GetCreditForInvoice(long franchiseeId, long invoiceId)
// Retrieves credits applicable to specified invoice

bool DeleteAccountCredit(long accountCreditId)
// Soft deletes credit memo

FranchiseeAccountCreditList GetFranchiseeAccountCredit(long franchiseeId, int pageNumber, int pageSize)
// Paginated credit list for franchisee
```

**Business Logic**:
- Credits linked by `QbInvoiceNumber` to QuickBooks
- Line items track description, amount, currency rate
- Credits applied automatically during invoice processing
- Affects customer balance calculations

---

### Polling Agent Implementations

#### SalesDataParsePollingAgent.cs
**Implements**: `ISalesDataParsePollingAgent`

**Dependencies**: 20+ repositories and services for comprehensive data processing

**Key Method**:
```csharp
void ParseFile()
// Continuously polls for uploads with StatusId = Uploaded
// Processes each file:
//   1. Update status to ParseInProgress
//   2. Read file (CSV/Excel)
//   3. Parse line-by-line:
//      - Extract customer data → CustomerService.SaveCustomer()
//      - Extract invoice data → InvoiceService
//      - Extract payment data → PaymentService
//      - Link via FranchiseeSalesPayment
//   4. Update Customer aggregates (TotalSales, AvgSales, NoOfSales)
//   5. Update upload metrics (NumberOfCustomers, NumberOfInvoices, TotalAmount)
//   6. Handle errors: log to ParsedLogFileId, increment NumberOfFailedRecords
//   7. Set final status: Parsed or Failed
```

**Error Handling**:
- Tries to parse each record independently - one bad record doesn't fail entire batch
- Logs errors to separate file referenced by `ParsedLogFileId`
- Increments `NumberOfFailedRecords` for each error
- Populates `NumberOfParsedRecords` for successful records
- Sets status to Failed if critical errors occur

**Performance**:
- Processes files with 1000+ invoices
- Uses bulk operations where possible
- Commits in batches to avoid long-running transactions

---

#### AnnualSalesDataParsePollingAgent.cs
**Implements**: `IAnnualSalesDataParsePollingAgent`

**Specialized Parsing**:
- Extends base parsing with address validation
- Compares parsed customer addresses against Customer master data
- Flags mismatches in `NoOfMismatchedRecords`
- Calculates `WeeklyRoyality` and `AnnualRoyality`
- Creates audit records for manual review
- Sets `AuditActionId = Pending` for staff approval

---

### Factory Implementations

#### CustomerFactory.cs
**Implements**: `ICustomerFactory`

**Key Methods**:
```csharp
Customer CreateDomain(CustomerCreateEditModel model, Customer domain)
// Maps ViewModel → Domain entity
// Handles address cascade
// Manages CustomerEmail collection updates (add/update/delete)

CustomerViewModel CreateViewModel(Customer customer)
// Maps Domain → ViewModel
// Includes related entities (Address, MarketingClass, CustomerEmails)
```

**Mapping Logic**:
- Address created/updated via `IAddressFactory`
- Emails: compare existing vs. new, add/update/soft delete as needed
- Marketing class ID validation
- DataRecorderMetaData populated with current user/timestamp

---

#### SalesDataUploadFactory.cs
**Implements**: `ISalesDataUploadFactory`

**Creation Logic**:
- Sets initial `StatusId = Uploaded`
- Links to uploaded file entity
- Captures period dates and franchisee
- Sets `IsActive = true`, `IsInvoiceGenerated = false`
- References current currency exchange rate

---

### Parser Implementations

#### SalesDataFileParser.cs
**Implements**: `ISalesDataFileParser`

**Parsing Logic**:
- Reads CSV/Excel file line-by-line
- Extracts columns: CustomerName, Email, Phone, Address, InvoiceNumber, InvoiceDate, Amount, PaymentAmount, etc.
- Data cleaning: trim whitespace, validate emails, parse dates
- Error tolerance: skips malformed lines, logs errors
- Returns structured data: `List<SalesDataRow>`

**File Format Support**:
- CSV (comma-separated)
- Excel (.xls, .xlsx)
- Configurable column mappings

---

#### CustomerFileParser.cs
**Implements**: `ICustomerFileParser`

**Customer-Only Parsing**:
- Similar to `SalesDataFileParser` but only extracts customer data
- No invoice or payment information
- Used for customer list imports/updates

---

### Validator Implementation

#### SalesDataUploadCreateModelValidator.cs
**Implements**: `ISalesDataUploadCreateModelValidator`

**Validation Methods**:
```csharp
bool ValidateDates(SalesDataUploadCreateModel model)
// Main validation: checks if dates match payment frequency

bool CheckDatesAreValidMonth(DateTime startDate, DateTime endDate)
// Monthly: start.Day == 1 && end == last day of month

bool CheckIfDatesAreValidWeek(DateTime startDate, DateTime endDate)
// Weekly: (endDate - startDate).Days == 6 (7-day week, inclusive)
```

**Business Rules**:
- Weekly frequency: uploads must cover exactly 7 days
- Monthly frequency: uploads must cover full calendar month
- No partial weeks or months allowed
- Date range must not overlap with existing uploads

---

### Helper Services

#### DownloadFileHelperService.cs
**Implements**: `IDownloadFileHelperService`

**Export Capabilities**:
- CSV generation with proper escaping
- Excel generation with formatting
- Large dataset support (streaming)
- Custom column selections
- Filtering and sorting support

**Usage**: Called by various services to generate file exports (customers, invoices, annual reports).
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application](../../../Application/.context/CONTEXT.md)** — `IRepository<T>`, `IUnitOfWork`, `ILogService`, `IFileService`, `ISettings`
- **[Core.Organizations](../../../Organizations/.context/CONTEXT.md)** — `IFranchiseeService`, Franchisee entity
- **[Core.Billing](../../../Billing/.context/CONTEXT.md)** — `IInvoiceService`, `IInvoiceItemService`, Invoice/Payment entities
- **[Core.Geo](../../../Geo/.context/CONTEXT.md)** — `IAddressFactory`, Address/City/State/Zip entities
- **[Core.Scheduler](../../../Scheduler/.context/CONTEXT.md)** — `IJobService`, JobScheduler entity
- **[Core.Reports](../../../Reports/.context/CONTEXT.md)** — `IFranchiseeSalesService`, FranchiseeSales entity
- **[Core.Notification](../../../Notification/.context/CONTEXT.md)** — `INotificationService` for reminders
- **[Core.Review](../../../Review/.context/CONTEXT.md)** — `ICustomerFeedbackService` for review requests

### External Dependencies
- **Entity Framework** — Database access via `IRepository<T>`
- **EPPlus / ClosedXML** (implied) — Excel file generation
- **System.IO** — File reading and writing
- **System.Data** — DataTable manipulation for parsing

### Design Patterns
- **Dependency Injection**: Constructor injection with `[DefaultImplementation]` attribute
- **Unit of Work**: Transaction management via `IUnitOfWork`
- **Repository**: Database access abstraction
- **Factory**: Object creation and transformation
- **Strategy**: File parser implementations
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Performance Considerations

**Large File Uploads**:
- Files with 1000+ invoices can take 5-10 minutes to parse
- Polling agents should have adequate timeouts
- Use bulk insert operations where possible
- Commit in batches (e.g., every 100 records) to avoid transaction timeouts

**Customer Matching**:
- Email matching is fastest (indexed)
- Phone matching is secondary (indexed)
- Name+Address matching is slowest (full scan) - use sparingly
- Consider caching customer lookups during single parsing session

**Sales Aggregates**:
- `Customer.TotalSales`, `AvgSales`, `NoOfSales` are denormalized for performance
- Updated during parsing, not real-time
- Don't rely on these values until upload status = Parsed

### Common Pitfalls

**Overlapping Uploads**:
- Always call `DoesOverlappingDatesExist()` before creating uploads
- Overlaps cause duplicate data and incorrect royalty calculations

**Date Validation**:
- Weekly: Must be exactly 7 days (not 6 or 8)
- Monthly: Must start on 1st and end on last day of month
- Use `SalesDataUploadCreateModelValidator` - don't implement custom validation

**Email Uniqueness**:
- Emails are unique across `CustomerEmail` table, not per customer
- One email can only belong to one customer
- Parser may fail if duplicate emails found in file

**Cascade Deletes**:
- Soft delete customers - never hard delete
- Hard deleting customers breaks foreign key references in invoices
- Use `customer.IsDeleted = true` instead

### Testing Strategies

**Unit Testing**:
- Mock `IRepository<T>` for isolated service testing
- Mock `IFileService` to avoid actual file I/O
- Use in-memory lists for customer repositories in parser tests

**Integration Testing**:
- Test full parsing flow with sample CSV files
- Verify customer aggregates update correctly
- Test error handling with malformed files

**Performance Testing**:
- Test with files of 1000+ records
- Monitor memory usage during parsing
- Verify transaction commit strategies

### Extension Points

**Adding New File Formats**:
1. Implement `ISalesDataFileParser` or `ICustomerFileParser`
2. Register implementation with DI container
3. Update file upload UI to accept new format

**Custom Validations**:
- Extend `SalesDataUploadCreateModelValidator`
- Add new validation methods
- Call from `SalesDataUploadService.Save()`

**Additional Parsers**:
- Create new polling agent implementing appropriate interface
- Register with background job scheduler
- Configure polling frequency

### Debugging Tips

**Upload Not Parsing**:
1. Check `StatusId` - must be Uploaded
2. Verify polling agent is running
3. Check logs for parsing errors
4. Inspect `ParsedLogFileId` for detailed error messages

**Customer Not Found During Parsing**:
1. Check email format in file
2. Verify phone number format
3. Ensure address matches exactly (case-sensitive)
4. Review customer matching logic in parser

**Aggregates Not Updating**:
1. Confirm upload status = Parsed
2. Check for parsing errors (NumberOfFailedRecords)
3. Verify invoice records were created
4. Re-run aggregate calculation if needed
<!-- END CUSTOM SECTION -->
