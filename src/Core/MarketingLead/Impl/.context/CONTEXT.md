# MarketingLead Implementation Services - AI/Agent Context

## Purpose

This folder contains concrete implementations of marketing lead services including API integration, data transformation, reporting, and background processing. These services orchestrate lead capture, conversion tracking, and comprehensive analytics.

**Architecture Pattern**: Service layer implementing interfaces with dependency injection, transactional repository access, external API clients.

---

## Service Inventory

### 1. **MarketingLeadsService** (Master orchestrator)
**File**: `MarketingLeadsService.cs`
**Interface**: `IMarketingLeadsService`
**Purpose**: Main entry point for scheduled lead import from all external sources.

**Dependencies** (15 repositories + 5 services):
- 5 MarketingLeadCallDetail repositories (V1-V5)
- WebLead, Franchisee, Organization, ZipCode, County repositories
- Lookup repository
- CallDetailsReportNotes repository
- ILogService, ISettings, IClock, IMarketingLeadsFactory

**Public Method**:
```csharp
void GetMarketingLeads()
```

**Execution Flow**:
1. Check `ISettings.GetWebLeads` → Call `GetWebLeads()`
2. Check `ISettings.GetCallDetails` → Call `GetCallDetailsV3()` (DialogTech)
3. Check `ISettings.GetCallDetails` → Call `GetCallDetailsInvoca()` (Invoca)
4. Call `SyncMarketingLeadCallDetailsIdWithCallNotes()`

**GetWebLeads() Implementation**:
- Validates `ISettings.WebLeadsAPIkey`
- HTTP GET to external web lead API
- Deserializes JSON response to `WebLeadViewModel[]`
- Deduplicates by `WebLeadId` (checks existing in database)
- Factory transforms to `WebLead` domain entities
- Saves in single transaction
- Logs success/failure

**GetCallDetailsV3() Implementation** (DialogTech):
- Validates `ISettings.AccessKey` and `ISettings.SecretKey`
- Builds API URL with access keys
- HTTP GET returns XML
- Deserializes to `CallRetailRecordV3[]`
- For each record:
  - Checks `SessionId` for duplicates (skip if exists)
  - Factory creates `MarketingLeadCallDetail` + V2/V3/V4/V5 entities
  - Saves all 5 versions in parallel transactions
- Logs per-record and batch-level metrics

**GetCallDetailsInvoca() Implementation**:
- Validates `ISettings.InvocaAPIKey`
- HTTP GET returns XML
- Deserializes to `InvocaCallDetails[]`
- For each record:
  - Checks `complete_call_id` for duplicates
  - Factory creates all 5 versions of CallDetail
  - Extracts additional fields: `verified_zip`, `office`, `find_me_list`
  - Saves with enhanced franchisee mapping data
- More comprehensive than DialogTech (60+ XML elements)

**SyncMarketingLeadCallDetailsIdWithCallNotes()**:
- Links existing `CallDetailsReportNotes` to newly imported calls
- Matches by `CallerId` (phone number)
- Updates `MarketingLeadId` FK on note records

**Error Handling**:
- Try-catch per record (continues batch on individual failures)
- Logs errors but doesn't stop import
- No retry logic - waits for next scheduled run

**Performance**:
- Synchronous HTTP calls (no async/await)
- Single large transaction per import type
- No rate limiting or circuit breaker

---

### 2. **MarketingLeadsReportService** (Mega reporting service)
**File**: `MarketingLeadsReportService.cs`
**Interface**: `IMarketingLeadsReportService`
**Purpose**: Provides 48 methods for lead retrieval, filtering, export, and management.

**Dependencies** (25 repositories + 8 services):
- All MarketingLead repositories
- Customer, Product, Sales, Billing repositories
- IExcelFileCreator for CSV/Excel export
- ISortingHelper for dynamic column sorting

**Key Method Groups**:

**List/Retrieval Methods** (8 methods):
```csharp
CallDetailListModel GetCallDetailList(CallDetailFilter, pageNumber, pageSize)
CallDetailListModelV2 GetCallDetailListV2(CallDetailFilter) // Enhanced version
RoutingNumberListModel GetRoutingNumberList(CallDetailFilter, pageNumber, pageSize)
WebLeadListViewModel GetWebLeadList(WebLeadFilter, pageNumber, pageSize)
FranchiseePhoneCallListModel GetFranchiseePhoneCalls(PhoneCallFilter)
```

**Report Methods** (7 methods):
```csharp
CallDetailReportListModel GetCallDetailReport(MarketingLeadReportFilter)
CallDetailReportListModel GetCallDetailReportAdjustedData(MarketingLeadReportFilter)
IQueryable<CallDetailReportViewModel> GetCallDetailReportListAdjustedData(MarketingLeadReportFilter)
WebLeadReportListModel GetWebLeadReport(MarketingLeadReportFilter)
HomeAdvisorReportListModel GetHomeAdvisorReport(HomeAdvisorReportFilter)
AutomationBackUpCallListModel GetAutomationBackUpReport(AutomationBackUpFilter, userId, roleUserId)
```

**Download/Export Methods** (8 methods):
```csharp
bool DownloadMarketingLeads(CallDetailFilter, out fileName)
bool DownloadMarketingLeadsWithNewRows(CallDetailFilter, out fileName)
bool DownloadWebLeads(WebLeadFilter, out fileName)
bool DownloadRoutingNumber(CallDetailFilter, out fileName)
bool DownloadCallDetailReport(MarketingLeadReportFilter, out fileName)
bool DownloadWebLeadReport(MarketingLeadReportFilter, out fileName)
bool DownloadLeadFlow(CallDetailFilter, out fileName)
bool DownloadCallNotesHistory(CallDetailNotesFilter, out fileName)
```

**Update/Management Methods** (10 methods):
```csharp
bool UpdateFranchisee(long id, long? franchiseeId)
bool UpdateTag(long id, long? tagId)
bool EditFranchiseePhoneCalls(PhoneCallEditModel)
bool EditFranchiseePhoneCallsByBulk(PhoneCallEditByBulkModel)
bool SaveFranchiseePhoneCallsByBulk(PhoneCallEditByBulkList)
bool GeneratePhoneCallInvoice(PhoneCallInvoiceEditModel)
bool SaveCallDetailsReportNotes(CallDetailsReportNotesViewModel)
bool EditCallDetailsReportNotes(EditCallDetailsReportNotesViewModel)
```

**Utility Methods** (3 methods):
```csharp
IEnumerable<FranchiseeDropdownListItem> GetOfficeCollection()
IEnumerable<FranchiseeDropdownListItem> GetFranchiseeNameValuePair()
CallDetailsReportNotesListViewModel GetCallDetailsReportNotes(CallDetailNotesFilter)
```

**GetCallDetailList Implementation Details**:
1. Builds IQueryable with filters (franchisee, date range, call type, tag)
2. Applies pagination (skip/take)
3. Fetches related V2, V3, V4, V5 records via parallel queries
4. Fetches all CallDetailsReportNotes (no filtering - performance concern)
5. Factory transforms to comprehensive ViewModels with all data joined
6. Returns CallDetailListModel with pagination metadata

**Download Methods Pattern**:
- Query data with filters
- Transform to flat ViewModels (for CSV columns)
- Call `IExcelFileCreator.Create()` with column definitions
- Returns filename on success, false on failure
- Files written to configured export directory

**Update Methods Pattern**:
- Load entity by ID
- Modify property
- Call `_unitOfWork.SaveChanges()`
- Return boolean success

**Performance Considerations**:
- `GetCallDetailList` loads all call notes (N+1 risk)
- No query caching
- Large exports may timeout
- V2 methods likely optimized with pre-joins

---

### 3. **MarketingLeadsFactory** (Data transformation)
**File**: `MarketingLeadsFactory.cs`
**Interface**: `IMarketingLeadsFactory`
**Purpose**: Bidirectional transformations between external API formats, domain entities, and view models.

**Dependencies**:
- ICustomerService, IClock

**Factory Method Groups**:

**API → ViewModel → Domain (CallDetails)**:
```csharp
// DialogTech V3 format
MarketingLeadCallDetailViewModel CreateModel(CallRetailRecord, callTypeId)
MarketingLeadCallDetail CreateDomain(MarketingLeadCallDetailViewModel)

// DialogTech V2 format  
MarketingLeadCallDetailV2 CreateModel(CallDetailV2)

// DialogTech V3 enhanced
MarketingLeadCallDetailViewModel CreateModelForNewApi(CallRetailRecordV3, callType)
MarketingLeadCallDetailV2 CreateModelForNewAPI(CallRetailRecordV3, sid, dateTime)
MarketingLeadCallDetailV3 CreateModelForNewAPI3(CallRetailRecordV3, sid, callDate)
MarketingLeadCallDetailV4 CreateModelForNewAPI4(CallRetailRecordV2, sid, callDate)
MarketingLeadCallDetailV5 CreateModelForNewAPI5(CallRetailRecordV2, sid, callDate)

// Invoca format
MarketingLeadCallDetailViewModel CreateModelForInvoca(InvocaCallDetails, callTypeId)
MarketingLeadCallDetailV2 CreateModelForInvocaAPI(InvocaCallDetails, sid, route)
MarketingLeadCallDetailV3 CreateModelForInvocaAPI3(InvocaCallDetails, sid)
MarketingLeadCallDetailV4 CreateModelForInvocaAPI4(InvocaCallDetails, sid)
MarketingLeadCallDetailV5 CreateModelForInvocaAPI5(InvocaCallDetails, sid)
```

**Domain → ViewModel**:
```csharp
CallDetailViewModel CreateViewModel(MarketingLeadCallDetail)
CallDetailViewModelV2 CreateViewModel(MarketingLeadCallDetailV2)

// Enhanced version with all V1-V5 data joined
CallDetailViewModel CreateNewViewModel(
    MarketingLeadCallDetail domain,
    MarketingLeadCallDetailV3 domain3,
    MarketingLeadCallDetailV4 domain4,
    MarketingLeadCallDetailV5 domain5,
    MarketingLeadCallDetailV2 domain2,
    List<CallDetailsReportNotes> callNoteList)
```

**Web Leads**:
```csharp
WebLead CreateDomain(WebLeadViewModel)
WebLeadInfoModel CreateViewModel(WebLead)
```

**Routing Numbers**:
```csharp
RoutingNumber CreateDomain(string phoneNumber, string phoneLabel)
RoutingNumberViewModel CreateViewModel(RoutingNumber)
```

**HomeAdvisor**:
```csharp
HomeAdvisorParentModel CreateViewModelForHomeAdvisor(HomeAdvisor)
```

**Phone Charges**:
```csharp
PhoneCallViewModel CreatePhoneViewModel(FranchiseeTechMailEmail, Phonechargesfee)
PhoneCallVInvoiceiewModel CreatePhoneInvoiceViewModel(Phonechargesfee)
```

**Key Transformations**:

**DateTime Conversion**:
```csharp
DateTime dateAdded = DateTime.ParseExact(record.DateAdded, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
model.DateAdded = _clock.ToUtc(dateAdded);
```

**Phone Number Normalization**:
```csharp
var phone = Regex.Replace(model.Phone, @"[\s()-]", "");
```

**Default Values**:
```csharp
TagId = (long)TagType.National,  // Default all leads to National
PhoneLabel = model.PhoneLabel != null ? model.PhoneLabel : "",
```

**String Concatenation**:
```csharp
StreetAddress = model.Addr + " " + model.Addr2
```

---

### 4. **UpdateConvertedLeadsService** (Lead assignment & conversion)
**File**: `UpdateConvertedLeadsService.cs`
**Interface**: `IUpdateConvertedLeadsService`
**Purpose**: Background job to assign leads to franchisees and track conversions to sales.

**Dependencies** (8 repositories):
- MarketingLeadCallDetail, MarketingLeadCallDetailV2
- WebLead, RoutingNumber, Franchisee
- FranchiseeSales
- ICustomerService, ILogService

**Public Method**:
```csharp
void UpdateLeads()
```

**Execution Flow**:
1. `UpdateCallConvertedLeads()` → `UpdateFranchisee()` + `UpdateInvoice()`
2. `UpdateConvertedWebLeads()` → Similar pattern for web leads
3. `MapFranchiseeToFranchiseePhoneWithFranchiseeId()` (if enabled)

**UpdateFranchisee() Logic (Calls)**:
```
For each unmapped call (FranchiseeId == null):
  1. Match DialedNumber → RoutingNumber.PhoneNumber
  2. If found, assign RoutingNumber.FranchiseeId
  3. If not found, match PhoneLabel (case-insensitive)
  4. If still not found, use EnteredZipCode → County → Franchisee mapping
  5. Save with _unitOfWork.SaveChanges()
```

**UpdateInvoice() Logic (Calls)**:
```
For each call in last 2 months with InvoiceId == null:
  1. Query FranchiseeSales where Phone == CallerId
  2. Filter: InvoiceDate within ±2 months of DateAdded
  3. If match found, assign FranchiseeSales.InvoiceId
  4. Save call detail record
```

**UpdateConvertedWebLeads() Logic**:
```
For each unmapped web lead (FranchiseeId == null):
  1. Match ZipCode → Franchisee.ZipCodes collection
  2. Assign first matching franchisee
  3. For invoice: Match Email OR Phone to FranchiseeSales
  4. Date range: ±2 months from CreatedDate
```

**MapFranchiseeToFranchiseePhoneWithFranchiseeId()**:
- Enhanced mapping using direct franchisee ID from call data
- Overrides routing number logic if available
- Checks V2 record for direct franchisee assignment

**Transaction Management**:
- Single transaction for entire update batch
- Rollback on any error (all-or-nothing)
- Logs errors but doesn't stop processing

**Performance**:
- Queries 2 months of data (configurable via `AddMonths(-2)`)
- Individual saves per lead (potential N+1)
- No batching or bulk updates

---

### 5. **GetRoutingNumberService** (Routing number sync)
**File**: `GetRoutingNumberService.cs`
**Interface**: `IGetRoutingNumberService`
**Purpose**: Sync phone routing numbers from DialogTech API.

**Dependencies**:
- RoutingNumber repository
- ILogService, ISettings, IClock, IMarketingLeadsFactory

**Public Method**:
```csharp
void GetRoutingNumber()
```

**Implementation**:
1. Check `ISettings.GetRoutingNumbers` flag
2. Call `GetRoutingPhoneNumbers()`

**GetRoutingPhoneNumbers() Logic**:
```
1. Validate access_key and secret_key from settings
2. Build API URL: https://secure.dialogtech.com/ibp_api.php?access_key={key}&secret_access_key={secret}&action=routing.numbers&format=xml
3. HTTP GET with WebClient
4. Parse XML to RoutingNumberList
5. For each routing number:
   - Check if PhoneNumber + PhoneLabel already exists
   - If new, create domain via factory
   - Save in individual transaction
   - Skip duplicates silently
```

**Deduplication**:
- Unique constraint: PhoneNumber + PhoneLabel combination
- Case-sensitive comparison on PhoneNumber
- Case-insensitive comparison on PhoneLabel (`.ToLower()`)

**Update Strategy**:
- Only inserts new records
- Never updates existing records
- Manual updates must be done in database

---

### 6. **HomeAdvisorFileParser** (CSV import)
**File**: `HomeAdvisorFileParser.cs`
**Interface**: `IHomeAdvisorFileParser`
**Purpose**: Parse and import HomeAdvisor lead CSV files.

**Dependencies**:
- IStateService, ICityService
- IUnitOfWork

**Key Methods**:
```csharp
bool CheckForValidHeader(DataTable, out message)
bool CheckForBlankFile(DataTable)
```

**CheckForValidHeader() Logic**:
1. Check for blank file first
2. Validate required columns exist:
   - "debit", "credit", "paid amount"
   - "class" (marketing class)
   - "sales price", "original amount"
   - "name", "name contact", or "source name" (customer name)
3. Return validation result and error message

**Processing Flow** (inferred from usage):
```
1. Load CSV to DataTable
2. CheckForValidHeader() → validate structure
3. For each row:
   - Extract company name, task, zip code
   - Match City + State to geo entities
   - Calculate NetLeadDollar from pricing columns
   - Create HomeAdvisor domain entity
   - Assign to franchisee by geographic match
   - Save record
```

**Error Handling**:
- Returns validation messages for UI display
- Continues processing valid rows if some fail

---

### 7. **MarketingLeadChartReportService** (Analytics)
**File**: `MarketingLeadChartReportService.cs`
**Interface**: `IMarketingLeadChartReportService`
**Purpose**: Generate aggregated chart data for dashboards.

**11 Report Methods**:
```csharp
MarketingLeadChartListModel GetPhoneVsWebReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetBusVsPhoneReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetLocalVsNationalReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetSpamVsPhoneReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetLocalVsNationalPhoneReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetDailyPhoneReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetSeasonalLeadReport(MarketingLeadReportFilter)
CallDetailReportListModel GetSummaryReport(MarketingLeadReportFilter)
CallDetailReportListModel GetAdjustedSummaryReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetCallDetailsReport(MarketingLeadReportFilter)
MarketingLeadChartListModel GetLocalSitePerformanceReport(MarketingLeadReportFilter)
ManagementVsLocalChartListModel GetManagementVsLocalReport(ManagementChartReportFilter)
ManagementCharViewModel GetManagementReport(ManagementChartReportFilter)
```

**Typical Report Pattern**:
1. Apply filters (franchisee, date range, tag)
2. Group by dimension (day, week, month, phone label)
3. Aggregate counts
4. Calculate percentages or ratios
5. Return MarketingLeadChartListModel with chart-ready data

**Example Query Structure** (conceptual):
```csharp
var phoneLeads = _callDetailRepository.Table
    .Where(filter conditions)
    .GroupBy(x => x.PhoneLabel)
    .Select(g => new { Label = g.Key, Count = g.Count() });

var webLeads = _webLeadRepository.Table
    .Where(filter conditions)
    .GroupBy(x => x.URL)
    .Select(g => new { Label = g.Key, Count = g.Count() });

return new MarketingLeadChartListModel {
    PhoneData = phoneLeads,
    WebData = webLeads
};
```

---

### 8. **UpdateMarketingLeadReportDataService** (Report cache)
**File**: `UpdateMarketingLeadReportDataService.cs`
**Interface**: `IUpdateMarketingLeadReportDataService`
**Purpose**: Refresh pre-aggregated report data (CallDetailData, WebLeadData tables).

**Pattern**:
- Background scheduled job
- Aggregates raw lead data into summary tables
- Enables fast report queries without scanning full lead tables

---

### 9. **UpdateAdjustedMarketingLeadReportDataService** (Adjusted report cache)
**File**: `UpdateAdjustedMarketingLeadReportDataService.cs`
**Purpose**: Similar to UpdateMarketingLeadReportDataService but for "adjusted" reports (likely with business logic adjustments or filters).

---

### 10. **ReviewPushLocationAPI** (Google review integration)
**File**: `ReviewPushLocationAPI.cs`
**Interface**: `IReviewPushLocationAPI`
**Purpose**: Push Google review request links to franchisees/customers.

**Public Method**:
```csharp
void ProcessRecord()
```

**Pattern**:
- Fetches FranchsieeGoogleReviewUrlAPI records
- Sends review request emails or SMS
- Integrates with Google Business Profile review URLs

---

## Common Patterns Across Services

### 1. **Settings-Based Feature Flags**
```csharp
if (_settings.GetWebLeads) { ... }
if (_settings.GetCallDetails) { ... }
if (_settings.GetRoutingNumbers) { ... }
```

### 2. **Extensive Logging**
```csharp
_logService.Info($"Web Lead Started At: {_clock.UtcNow}");
_logService.Info($"No Data Found!");
_logService.Error($"Error: {ex.Message}");
```

### 3. **Transaction Management**
```csharp
_unitOfWork.StartTransaction();
try {
    // operations
    _unitOfWork.SaveChanges();
} catch {
    _unitOfWork.Rollback();
}
```

### 4. **Synchronous API Calls**
```csharp
using (var client = new WebClient()) {
    string result = client.DownloadString(url);
}
```

### 5. **Factory-Based Transformations**
```csharp
var domain = _factory.CreateDomain(viewModel);
var viewModel = _factory.CreateViewModel(domain);
```

---

## Integration Points

### External APIs
- **DialogTech**: `https://secure.dialogtech.com/ibp_api.php`
- **Invoca**: URL from ISettings
- **Web Lead API**: URL/key from ISettings

### Internal Services
- **ICustomerService**: Customer matching for conversion tracking
- **IExcelFileCreator**: CSV/Excel generation
- **IProductReportService**: Product data for reports

### Database Tables
- All MarketingLead domain entities
- Franchisee, Invoice (from Organizations/Billing)
- FranchiseeSales (from Sales)
- Lookup (from Application)

---

## Performance & Scalability Considerations

### Current Limitations
1. **No Async/Await**: All API calls block thread pool
2. **Large Transactions**: Single transaction per import batch
3. **N+1 Queries**: CallDetailsReportNotes loaded without filtering
4. **No Caching**: Repeated franchisee/routing lookups
5. **No Rate Limiting**: API calls may be throttled by provider
6. **Synchronous Scheduled Jobs**: Block runner thread

### Optimization Opportunities
1. Convert to async/await pattern
2. Implement batching for saves
3. Add query result caching (MemoryCache)
4. Pre-load routing numbers in memory
5. Implement Polly for retry policies
6. Use bulk insert for large batches

---

## File Inventory

1. **MarketingLeadsService.cs** - Master orchestrator (3 import methods)
2. **MarketingLeadsReportService.cs** - Mega report service (48 methods)
3. **MarketingLeadsFactory.cs** - Data transformations (39 factory methods)
4. **UpdateConvertedLeadsService.cs** - Lead assignment & conversion tracking
5. **GetRoutingNumberService.cs** - Routing number sync from DialogTech
6. **HomeAdvisorFileParser.cs** - CSV file import and validation
7. **MarketingLeadChartReportService.cs** - Chart data aggregation (11 reports)
8. **UpdateMarketingLeadReportDataService.cs** - Report cache refresh
9. **UpdateAdjustedMarketingLeadReportDataService.cs** - Adjusted report cache
10. **ReviewPushLocationAPI.cs** - Google review push integration

**Total**: 11 implementation files

---

## Metadata

```json
{
  "version": "1.1",
  "last_commit": "64667c5c8c4ab9b3d804e48deb14e9b70895fc42",
  "generated_at": "2025-01-10T00:00:00Z",
  "file_count": 11,
  "changed_files": []
}
```
