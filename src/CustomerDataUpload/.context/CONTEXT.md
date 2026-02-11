<!-- AUTO-GENERATED: Header -->
# CustomerDataUpload — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
CustomerDataUpload is a bulk data import utility that processes Excel files containing customer sales data, validates and deduplicates records, and persists them to the Marblelife database. It handles franchisee-specific sales uploads, matches customers by email/phone to prevent duplicates, validates addresses and marketing classifications, and tracks upload status throughout the pipeline.

### Design Patterns
- **Polling Agent Pattern**: Queries for pending uploads and processes sequentially
- **Transaction-per-File**: Each Excel file processes within a database transaction
- **State Machine**: Tracks upload status (Uploaded → ParseInProgress → Parsed/Failed)
- **Duplicate Detection**: Multi-strategy customer matching (email → phone → address proximity)
- **Error Accumulation**: Collects all validation errors before marking upload as failed

### Data Flow
1. Query `SalesDataUpload` table for records with `StatusId = Uploaded` and `IsActive = true`
2. Mark as `ParseInProgress` to prevent concurrent processing
3. Parse Excel file via `ExcelFileParser.ReadExcel()` into `DataTable`
4. Convert rows to `ParsedFileParentModel` collection with validation
5. For each record:
   - Search for existing customer (email → phone → address)
   - Validate address, marketing class, service type
   - Create or update customer
   - Create `FranchiseeSales` record linking customer to franchisee
6. Update upload status: `Parsed` (success) or `Failed` (validation errors)
7. Log detailed processing results

### Duplicate Detection Strategy
```
1. Match by email (exact match)
2. If not found → Match by phone (normalized)
3. If multiple matches → Filter by address proximity (city + state)
4. If still ambiguous → Create new customer record
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### ParsedFileParentModel
```csharp
public class ParsedFileParentModel
{
    public CustomerCreateEditModel Customer { get; set; }
    public string QbIdentifier { get; set; }        // QuickBooks invoice number
    public long MarketingClassId { get; set; }      // Classification (Residential, Commercial)
    public string SalesRep { get; set; }            // Sales representative name
    public List<ServiceLineItem> Services { get; set; }  // Service types and amounts
}
```

### CustomerCreateEditModel
```csharp
public class CustomerCreateEditModel
{
    public string Name { get; set; }
    public AddressCreateEditModel Address { get; set; }
    public List<EmailModel> CustomerEmails { get; set; }
    public List<PhoneModel> Phone { get; set; }
}
```

### SalesDataUpload (Domain Entity)
```csharp
public class SalesDataUpload
{
    public long Id { get; set; }
    public long FranchiseeId { get; set; }
    public long StatusId { get; set; }  // Uploaded, ParseInProgress, Parsed, Failed
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public File File { get; set; }      // Reference to uploaded Excel file
    public int NumberOfParsedRecords { get; set; }
    public int NumberOfFailedRecords { get; set; }
    public int NumberOfCustomers { get; set; }
}
```

### FranchiseeSales (Domain Entity)
```csharp
public class FranchiseeSales
{
    public long Id { get; set; }
    public long FranchiseeId { get; set; }
    public long CustomerId { get; set; }
    public string QbInvoiceNumber { get; set; }
    public long ClassTypeId { get; set; }       // Marketing classification
    public string SalesRep { get; set; }
    public decimal Amount { get; set; }
    public long? CurrencyExchangeRateId { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `CustomerDataUploadPollingAgent.ParseCustomerData()`
- **Input**: None (queries database for pending uploads)
- **Output**: void
- **Behavior**:
  - Finds oldest pending upload (ordered by FranchiseeId, PeriodStartDate)
  - Marks as `ParseInProgress` to lock
  - Parses Excel file and validates each row
  - Creates/updates customers and sales records
  - Updates upload status with statistics
- **Side-effects**: Database writes, status updates, detailed logging

### `PrepareDomainFromDataTable(DataTable data)`
- **Input**: Parsed Excel data table
- **Output**: `IList<ParsedFileParentModel>`
- **Behavior**:
  - Maps Excel columns to domain models
  - Validates required fields (Name, Email/Phone, Service Type)
  - Normalizes phone numbers and email addresses
  - Maps marketing class and service type names to IDs
  - Accumulates validation errors
- **Side-effects**: Populates `_headersDictionary` for column mapping

### `SaveModel(ParsedFileParentModel record, SalesDataUpload salesDataUpload)`
- **Input**: Validated customer record, upload context
- **Output**: `SaveModelStats` (logs, counts)
- **Behavior**:
  - Searches for existing customer (email → phone → address)
  - Creates or updates customer via `ICustomerService.SaveCustomer()`
  - Creates `FranchiseeSales` record linking customer to franchisee
  - Applies currency exchange rate for international franchisees
- **Side-effects**: Customer and sales records persisted, detailed logs

### `SearchCustomer(CustomerCreateEditModel customer)`
- **Input**: Customer model with email/phone/address
- **Output**: Existing `Customer` entity or null
- **Behavior**:
  - Searches by email (exact match)
  - Falls back to phone number (normalized)
  - If multiple matches, filters by address proximity (city + state)
- **Side-effects**: Logs search strategy and results
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Application](../../Core/Application/.context/CONTEXT.md)** — IUnitOfWork, ILogService, IClock, ISettings
- **[Core.Sales](../../Core/Sales/.context/CONTEXT.md)** — ICustomerService, FranchiseeSales domain
- **[Core.Billing](../../Core/Billing/.context/CONTEXT.md)** — CurrencyExchangeRate domain
- **[Core.Geo](../../Core/Geo/.context/CONTEXT.md)** — Address validation, State/Country lookups
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration

### External
- **EPPlus or similar** — Excel file parsing (`.xlsx` format)
- **System.Data** — DataTable processing

### Database Tables
- `SalesDataUpload` — Upload tracking and status
- `FranchiseeSales` — Sales records linked to franchisees
- `Customer` — Customer master data
- `MarketingClass` — Classification lookup (Residential, Commercial, etc.)
- `ServiceType` — Service offerings lookup
- `CurrencyExchangeRate` — International currency conversion rates
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Excel File Format Requirements
The service expects specific column headers (case-insensitive, space-insensitive):
- **Required**: `CustomerName`, `Email` OR `Phone`, `ServiceType`, `Amount`
- **Optional**: `Address`, `City`, `State`, `Zip`, `MarketingClass`, `SalesRep`, `QBInvoice#`

Example:
```
Customer Name | Email              | Phone       | Service Type | Amount | Marketing Class
John Doe      | john@example.com   | 555-1234    | StoneLife    | 250.00 | Residential
Acme Corp     | info@acme.com      | 555-5678    | ColorSeal    | 1200.00| Commercial
```

### Marketing Class and Service Type Mapping
Names are normalized before lookup:
```csharp
// "Stone Life" → "STONELIFE" (uppercase, spaces removed)
_marketingClasses.FirstOrDefault(x => x.Name == normalized)
_serviceTypes.FirstOrDefault(x => x.Name == normalized || x.Alias == normalized)
```

Ensure database values match this normalization or uploads will fail.

### Duplicate Customer Scenarios

**Scenario 1: Exact Email Match**
```
Existing: john@example.com
Uploaded: john@example.com
Result: Updates existing customer
```

**Scenario 2: Phone Match**
```
Existing: (555) 123-4567
Uploaded: 5551234567
Result: Normalizes phone, matches existing
```

**Scenario 3: Address Proximity**
```
Existing: 123 Main St, Dallas, TX
Uploaded: 123 Main Street, Dallas, TX
Result: Matches by city + state (fuzzy address match)
```

**Scenario 4: No Match**
```
Result: Creates new customer record
```

### Error Handling and Rollback
- **File-level transaction**: If any record fails validation, entire file rolls back
- **Detailed logging**: Each validation error captured with row number
- **Status tracking**: Upload marked as `Failed` with error count
- **Manual recovery**: Failed uploads can be corrected and re-uploaded

### Performance Considerations
- **Large files**: 1000+ rows may cause memory pressure (DataTable loaded fully)
- **Database round-trips**: Each customer search queries database (consider caching)
- **Concurrent uploads**: Status locking prevents race conditions
- **Currency rate lookup**: Single query per file (most recent rate)

### Common Upload Failures
1. **Missing required columns**: `CustomerName`, `Email` or `Phone`
2. **Invalid MarketingClass**: Name doesn't match database values
3. **Invalid ServiceType**: Name/Alias not found in lookup table
4. **Invalid state abbreviation**: Two-letter code not in `State` table
5. **Duplicate invoice numbers**: Same `QbInvoiceNumber` within file
<!-- END CUSTOM SECTION -->
