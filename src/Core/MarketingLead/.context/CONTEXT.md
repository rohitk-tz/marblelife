# MarketingLead Module - AI/Agent Context

## Module Overview

**Purpose**: Complete marketing lead management system for tracking, capturing, routing, and converting leads from multiple sources (web forms, phone calls, HomeAdvisor integration). Manages the entire lead lifecycle from acquisition through conversion tracking and franchisee attribution.

**Architecture Pattern**: Service-oriented with factory pattern for domain/ViewModel transformations, repository pattern for data access, scheduled background processing for external API synchronization.

**Key Responsibilities**:
- **Lead Capture**: Web lead capture from customer forms, phone call tracking via DialogTech/Invoca APIs
- **HomeAdvisor Integration**: Parse and process HomeAdvisor CSV files to import external leads
- **Call Tracking**: Multi-version call detail capture (V1-V5) with progressive enhancement of call analytics, attribution, visitor tracking
- **Lead Conversion**: Auto-map leads to franchisees based on geography/routing, track conversion to sales
- **Routing Management**: Dynamic phone number routing configuration tied to marketing campaigns
- **Reporting**: Comprehensive analytics on lead sources, performance, franchisee attribution

---

## Core Architectural Components

### 1. **Domain Layer** (Domain/)
Central business entities representing leads and their metadata:

- **MarketingLeadCallDetail**: Primary call tracking entity - stores phone call metadata (caller ID, dialed number, duration, transfer details, franchisee attribution)
- **MarketingLeadCallDetailV2-V5**: Progressive enhancement versions capturing additional call data:
  - V2: Basic call flow data (set name, transfer, IVR results, geo data)
  - V3: Call flow routing, analytics, reverse lookup
  - V4: Call metrics, attribution channels, first/last touch tracking, visitor data
  - V5: UTM parameters, value track parameters, SourceIQ data, paid search details
- **WebLead**: Web form submission entity (name, contact, property details, surface type, service requested)
- **HomeAdvisor**: External lead source entity (account, SRID, task, location, net lead cost)
- **RoutingNumber**: Phone number configuration (number, label, category, franchisee/tag assignment)
- **Tag**: Lead categorization (FranchiseDirect vs National)
- **CallDetailData**: Aggregated call detail metrics for reporting

### 2. **Service Layer Interfaces** (Root Level)
Primary public API contracts:

- **IMarketingLeadsService**: Orchestrates lead acquisition from all sources (`GetMarketingLeads()`)
- **IMarketingLeadsReportService**: 48 reporting methods - list/filter/download leads, manage franchisee attribution, handle call notes
- **IMarketingLeadChartReportService**: 11 chart report generators (phone vs web, business hours, local vs national, seasonal trends)
- **IHomeAdvisorParser**: Process HomeAdvisor CSV files (`ProcessRecords()`)
- **IGetRoutingNumberService**: Sync routing numbers from DialogTech API (`GetRoutingNumber()`)
- **IUpdateConvertedLeadsService**: Background job to map leads to franchisees and track conversions (`UpdateLeads()`)
- **IMarketingLeadsFactory**: 39 factory methods for Domain ↔ ViewModel transformations
- **IReviewPushLocationAPI**: Push Google review requests to franchisees (`ProcessRecord()`)

### 3. **Implementation Layer** (Impl/)
Concrete service implementations with external API integrations:

- **MarketingLeadsService**: Main orchestrator - calls web lead API, DialogTech API (V3), Invoca API for call details
- **HomeAdvisorFileParser**: CSV parser with header validation, franchisee matching by geo location
- **GetRoutingNumberService**: DialogTech API client (XML-based) to fetch phone routing configuration
- **UpdateConvertedLeadsService**: Maps call leads to franchisees by phone number routing, maps web leads by zip code, links leads to invoices
- **MarketingLeadsReportService**: 48-method mega-service for filtering, pagination, CSV export, call note management
- **MarketingLeadChartReportService**: Aggregated analytics queries for dashboards
- **MarketingLeadsFactory**: Bidirectional transformations between retail API formats and domain entities
- **ReviewPushLocationAPI**: Sends Google review URLs to franchisees
- **UpdateMarketingLeadReportDataService** / **UpdateAdjustedMarketingLeadReportDataService**: Background jobs to refresh reporting cache tables

### 4. **ViewModel Layer** (ViewModel/)
65 DTOs organized by purpose:

**Call Detail Views** (15 files):
- CallDetailViewModel, CallDetailViewModelV2: API responses for call detail lists
- CallRetailRecord, CallRetailRecordV2, CallRetailRecordV3: External API request models (DialogTech formats)
- InvocaCallDetails: Invoca API format (XML attributes, ~380 properties)
- CallDetailFilter, CallDetailListModel, CallDetailReportViewModel: Filtering and list presentation

**Web Lead Views** (8 files):
- WebLeadViewModel: Web form submission structure
- WebLeadInfoModel, WebLeadListViewModel, WebLeadReportViewModel: List/detail views
- WebLeadFilter: Search/filter criteria

**Routing Number Views** (4 files):
- RoutingNumberViewModel, RoutingNumberRecord, RoutingNumberList, RoutingNumberListModel

**Reporting/Analytics Views** (28 files):
- LeadPerformance* (6 files): Franchisee performance metrics (local vs national)
- ManagementChart* (13 files): Executive dashboards (business hours, day-of-week, grouped metrics)
- MarketingLeadChart* (3 files): Lead source charts
- HomeAdvisorReport* (2 files): HomeAdvisor lead analytics

**Other Views** (10 files):
- PhoneCallEditModel, PhoneCallInvoiceEditModel: Bulk editing call records
- CallDetailsReportNotesViewModel: Call note annotations
- FranchiseePhoneCallViewModel: Franchisee-specific call views

### 5. **Enumerations** (Enum/)
Lookup value types:

- **CallType**: Inbound(140), Outbound(141), Other(142), ClickTo(143), AgentOutbound(144)
- **RoutingNumberCategory**: PrintMedia(191), PhoneWebLocal(192), PhoneWebNational(193), BusinessDirectories(194), WRAPVan(195)
- **TagType**: FranchiseDirect(1), National(2)
- **PerformanceParameter**: SEOCOST(219), PPCSPEND(220)

---

## Data Flow Architecture

### Lead Capture Flow (Web Leads)
```
1. Customer submits form → External Web Lead API (configured via ISettings.WebLeadsAPIkey)
2. MarketingLeadsService.GetWebLeads() → HTTP GET to API
3. Deserialize JSON to WebLeadViewModel
4. Factory creates WebLead domain entity
5. Save to WebLead repository
6. UpdateConvertedLeadsService.UpdateConvertedWebLeads() → Map to franchisee by zip code
7. Link to Invoice if customer converts
```

### Lead Capture Flow (Call Tracking - Multi-Source)
```
A. DialogTech (Legacy V3 API):
   1. MarketingLeadsService.GetCallDetailsV3()
   2. HTTP GET → Parse XML to CallRetailRecordV3
   3. Factory creates MarketingLeadCallDetail + V2/V3/V4/V5 entities (parallel save)
   4. Deduplicate by SessionId

B. Invoca API (Primary):
   1. MarketingLeadsService.GetCallDetailsInvoca()
   2. HTTP GET → Parse XML to InvocaCallDetails
   3. Factory creates all 5 versions of CallDetail entities
   4. Additional: verified_zip, office, find_me_list from Invoca
   5. SyncMarketingLeadCallDetailsIdWithCallNotes() links existing notes

Post-Processing:
   → UpdateConvertedLeadsService runs (scheduled):
      - UpdateFranchisee(): Match calls to franchisees via RoutingNumber lookup
      - UpdateInvoice(): Link calls to invoices via CallerId + date matching
      - MapFranchiseeToFranchiseePhoneWithFranchiseeId(): Direct franchisee ID mapping
```

### HomeAdvisor Integration Flow
```
1. Upload CSV file → HomeAdvisorFileParser.CheckForValidHeader()
2. Validate columns: debit, credit, name, class, sales price
3. Parse rows → Match city/state to geo entities
4. Create HomeAdvisor domain objects
5. Auto-assign to franchisee by location match
```

### Routing Number Sync Flow
```
1. Scheduled job → GetRoutingNumberService.GetRoutingNumber()
2. HTTP GET to DialogTech API (access_key, secret_key)
3. Parse XML to RoutingNumberList
4. Deduplicate by PhoneNumber + PhoneLabel
5. Save new RoutingNumber entities (no update of existing)
```

---

## Critical Integration Points

### External APIs
1. **Web Lead API**: 
   - Auth: API key from `ISettings.WebLeadsAPIkey`
   - Format: JSON
   - Fields: franchise_id, name, contact, address, property/surface types

2. **DialogTech Call Tracking**:
   - Auth: access_key + secret_access_key from `ISettings`
   - Endpoints: 
     - `call.retail` (V3 format)
     - `routing.numbers` (phone config)
   - Format: XML
   - Rate limits: Unknown (no retry logic present)

3. **Invoca Call Tracking**:
   - Auth: API key from `ISettings`
   - Format: XML with 60+ XML elements
   - Enhanced data: Caller name/address, UTM params, paid search attribution, visitor analytics

### Database Dependencies
- **Franchisee** (Core.Organizations): Primary entity for lead assignment
- **Invoice** (Core.Billing): Track lead conversion to revenue
- **Lookup** (Core.Application): CallType values
- **ZipCode, County, City, State** (Core.Geo): Geo-based franchisee matching
- **Organization** (Core.Organizations): Company-level aggregation
- **FranchiseeSales** (Core.Sales): Conversion tracking (phone/email match)

### Scheduled Jobs
- **GetMarketingLeads**: Master orchestrator (runs GetWebLeads, GetCallDetailsV3, GetCallDetailsInvoca)
- **UpdateLeads**: Post-processing for franchisee mapping and invoice linking
- **UpdateMarketingLeadReportDataService**: Refresh reporting cache
- **ProcessRecords**: HomeAdvisor file processing

---

## State Management & Business Rules

### Lead Deduplication
- **Calls**: Deduplicate by `SessionId` (DialogTech) or `complete_call_id` (Invoca)
- **Web Leads**: No automatic deduplication (allows duplicate submissions)
- **Routing Numbers**: Deduplicate by `PhoneNumber + PhoneLabel` combination

### Franchisee Assignment Logic
1. **Calls**: 
   - Match `DialedNumber` or `PhoneLabel` → RoutingNumber.FranchiseeId
   - Fallback: ZipCode → County → Franchisee.CountyId
   - Special case: Invoca provides `office` field for direct mapping
   
2. **Web Leads**:
   - Match ZipCode → Franchisee.ZipCodes collection
   - If no match, leave FranchiseeId null (visible in reports as unmapped)

3. **HomeAdvisor**:
   - Match City + State → Franchisee by geographic territory

### Valid Call Detection
- `ValidCall` flag on MarketingLeadCallDetail
- Logic embedded in factory methods (not documented in viewed code)
- Likely based on: CallDuration threshold, CallType, successful transfer

### Call Note Synchronization
- `CallDetailsReportNotes` entity (separate table)
- Linked to MarketingLeadCallDetail by Id
- `SyncMarketingLeadCallDetailsIdWithCallNotes()` runs after Invoca import to match notes to new call records

### Invoice Linking
- **Calls**: Match CallerId (phone number) to FranchiseeSales.Phone within ±2 months of call date
- **Web Leads**: Match Email or Phone to FranchiseeSales within date range
- Updates InvoiceId field on lead entity

---

## Public API Summary

### Lead Retrieval
```csharp
// Get paginated call details with filters
CallDetailListModel GetCallDetailList(CallDetailFilter filter, int pageNumber, int pageSize);
CallDetailListModelV2 GetCallDetailListV2(CallDetailFilter filter); // Enhanced version

// Get web leads
WebLeadListViewModel GetWebLeadList(WebLeadFilter filter, int pageNumber, int pageSize);

// Get routing numbers
RoutingNumberListModel GetRoutingNumberList(CallDetailFilter filter, int pageNumber, int pageSize);
```

### Reporting & Analytics
```csharp
// Chart reports (return aggregated data for visualization)
MarketingLeadChartListModel GetPhoneVsWebReport(MarketingLeadReportFilter filter);
MarketingLeadChartListModel GetLocalVsNationalReport(MarketingLeadReportFilter filter);
MarketingLeadChartListModel GetDailyPhoneReport(MarketingLeadReportFilter filter);
MarketingLeadChartListModel GetSeasonalLeadReport(MarketingLeadReportFilter filter);
ManagementVsLocalChartListModel GetManagementVsLocalReport(ManagementChartReportFilter filter);

// Summary reports
CallDetailReportListModel GetCallDetailReport(MarketingLeadReportFilter filter);
CallDetailReportListModel GetSummaryReport(MarketingLeadReportFilter filter);
CallDetailReportListModel GetAdjustedSummaryReport(MarketingLeadReportFilter filter);
WebLeadReportListModel GetWebLeadReport(MarketingLeadReportFilter filter);
HomeAdvisorReportListModel GetHomeAdvisorReport(HomeAdvisorReportFilter filter);
```

### Data Export
```csharp
bool DownloadMarketingLeads(CallDetailFilter filter, out string fileName);
bool DownloadWebLeads(WebLeadFilter filter, out string fileName);
bool DownloadRoutingNumber(CallDetailFilter filter, out string fileName);
bool DownloadCallDetailReport(MarketingLeadReportFilter filter, out string fileName);
bool DownloadWebLeadReport(MarketingLeadReportFilter filter, out string fileName);
bool DownloadLeadFlow(CallDetailFilter filter, out string fileName);
bool DownloadCallNotesHistory(CallDetailNotesFilter filter, out string fileName);
```

### Lead Management
```csharp
// Update franchisee assignment
bool UpdateFranchisee(long id, long? franchiseeId);
bool UpdateTag(long id, long? tagId);

// Bulk edit call records
bool EditFranchiseePhoneCalls(PhoneCallEditModel filter);
bool EditFranchiseePhoneCallsByBulk(PhoneCallEditByBulkModel filter);
bool SaveFranchiseePhoneCallsByBulk(PhoneCallEditByBulkList filter);

// Invoice generation
bool GeneratePhoneCallInvoice(PhoneCallInvoiceEditModel filter);

// Call notes
bool SaveCallDetailsReportNotes(CallDetailsReportNotesViewModel filter);
bool EditCallDetailsReportNotes(EditCallDetailsReportNotesViewModel filter);
CallDetailsReportNotesListViewModel GetCallDetailsReportNotes(CallDetailNotesFilter filter);
```

### Scheduled Operations
```csharp
void GetMarketingLeads(); // Master orchestrator for lead imports
void GetRoutingNumber();  // Sync routing numbers from DialogTech
void UpdateLeads();       // Map leads to franchisees and invoices
void ProcessRecords();    // Process HomeAdvisor CSV files
void ProcessRecord();     // Push Google review requests
```

---

## Error Handling & Edge Cases

### API Failure Handling
- **Strategy**: Log and continue (no retry logic)
- Missing API keys → Log error, skip that import type
- API response parsing errors → Log exception, continue to next record
- Individual record save failures → Rollback transaction, log, continue batch

### Null Handling
- FranchiseeId nullable throughout → Reports show "unmapped" leads
- InvoiceId nullable → Leads without conversions visible in performance reports
- TagId nullable with default fallback

### Timezone Handling
- `MarketingLeadCallDetail.TimeZoneStartTime`: Uses browser timezone from IClock service
- All DateAdded/CallDate stored as UTC in database

### Data Quality Issues
- **Duplicate SessionIds**: First record wins (subsequent duplicates logged and skipped)
- **Invalid routing numbers**: Factory returns null, skipped
- **Missing geo data**: Lead saved but franchisee mapping deferred to manual assignment

### Transactional Boundaries
- Single transaction per lead batch (rollback on exception)
- Routing number sync: Individual transactions per number (failure doesn't block others)
- Conversion update: Single large transaction (all leads or none)

---

## Performance Considerations

### Query Optimization
- CallDetailListModel queries use pagination (pageNumber, pageSize)
- Report queries filter by date ranges (avoid full table scans)
- V2 endpoints use enhanced queries (likely pre-joins)

### Batch Processing
- Web leads: Fetch all from API, process batch
- Call details: Fetch all from API, deduplicate, then batch save
- Routing numbers: Individual saves (no bulk insert)

### Caching
- No in-memory caching detected
- Report data cache via UpdateMarketingLeadReportDataService (database materialized views pattern)

### N+1 Query Risks
- Factory methods that create ViewModels likely trigger lazy loading (Franchisee, CallType, Tag FK navigations)
- Recommendation: Use `.Include()` in repository queries before ViewModel transformation

---

## Dependencies

### Internal Module References
- `Core.Application`: Base services (IUnitOfWork, IRepository, ILogService, ISettings, IClock)
- `Core.Organizations`: Franchisee, Organization entities
- `Core.Geo`: ZipCode, County, City, State
- `Core.Billing`: Invoice, Phonechargesfee
- `Core.Sales`: FranchiseeSales, ICustomerService
- `Core.Scheduler`: FranchiseeTechMailEmail (for phone view models)

### External Libraries
- **RestSharp**: HTTP client for API calls
- **Newtonsoft.Json**: JSON parsing for web leads
- **System.Xml.Serialization**: XML parsing for DialogTech/Invoca
- **System.Web.Script.Serialization**: Legacy JSON serialization

### Configuration Dependencies
- **ISettings.GetWebLeads**: Enable web lead import
- **ISettings.GetCallDetails**: Enable call tracking imports
- **ISettings.WebLeadsAPIkey**: Web lead API authentication
- **ISettings.AccessKey**, **ISettings.SecretKey**: DialogTech credentials
- **ISettings.IsMapFranchiseeToFranchiseePhoneWithFranchiseeId**: Enable enhanced franchisee mapping

---

## Testing Considerations

### Unit Test Boundaries
- Factory methods: Pure transformations (easy to test)
- Service layer: Mock IRepository, ISettings, ILogService
- API clients: Mock WebClient/RestSharp responses

### Integration Test Scenarios
1. Full lead import cycle (web + calls + routing sync)
2. Franchisee assignment logic with various geo configs
3. Invoice linking with date range edge cases
4. Duplicate detection for call SessionIds

### Test Data Requirements
- Sample XML responses from DialogTech/Invoca
- Sample JSON from web lead API
- Sample HomeAdvisor CSV with valid/invalid headers
- Geo data (ZipCode, County mappings) for franchisee assignment
- Franchisee configurations with routing numbers

---

## Security Considerations

### Authentication
- API keys stored in ISettings (likely encrypted configuration)
- No user-level authentication in this module (handled by parent application)

### Authorization
- No explicit authorization checks in service layer
- Assumes caller has already verified permissions
- FranchiseeId filtering in queries for multi-tenant isolation

### Data Privacy
- Call recordings: URLs stored (recordings hosted externally)
- PII fields: FirstName, LastName, Email, Phone, Address across entities
- No encryption at module level (database encryption assumed)

### Input Validation
- Minimal validation detected
- CSV header validation in HomeAdvisorFileParser
- No SQL injection risk (uses EF parameterized queries)
- XSS risk in ViewModels (output encoding must happen in presentation layer)

---

## Known Limitations & Technical Debt

1. **No Retry Logic**: API failures permanently lose that import cycle's data until next scheduled run
2. **Monolithic Report Service**: 48 methods violates SRP, difficult to maintain
3. **No Async/Await**: All API calls synchronous (blocks thread pool)
4. **Version Sprawl**: 5 parallel CallDetail tables (V1-V5) - no schema migration strategy
5. **Factory Method Explosion**: 39 factory methods (consider builder pattern)
6. **Magic Numbers**: Enum values (140, 141, etc.) hardcoded throughout
7. **No Rate Limiting**: DialogTech/Invoca APIs may throttle without warning
8. **Large ViewModels**: InvocaCallDetails has 380 properties (serialization overhead)
9. **Transaction Scope**: Large batch transactions may cause deadlocks under load
10. **No Idempotency**: Re-running imports may create duplicates if SessionId changes

---

## Future Enhancement Opportunities

1. Implement async API calls with Polly retry policies
2. Split MarketingLeadsReportService into focused services
3. Consolidate CallDetailV1-V5 into single table with JSON column for version-specific data
4. Add real-time webhooks for lead capture (eliminate polling)
5. Implement in-memory caching for routing number lookups
6. Add comprehensive validation using FluentValidation
7. Extract API client interfaces for better testability
8. Implement event-driven architecture for lead assignment notifications
9. Add rate limiting and circuit breaker patterns
10. Migrate to modern CSV parser (CsvHelper) for HomeAdvisor integration

---

## File Inventory

### Root Level (13 files)
- **IMarketingLeadsService.cs**: Master orchestrator interface
- **IMarketingLeadsFactory.cs**: 39 factory method signatures
- **IMarketingLeadsReportService.cs**: 48 report/management methods
- **IMarketingLeadChartReportService.cs**: 11 chart report methods
- **IHomeAdvisorParser.cs**: CSV processing interface
- **IHomeAdvisorFileParser.cs**: Enhanced CSV parser interface
- **IGetRoutingNumberService.cs**: Routing sync interface
- **IUpdateConvertedLeadsService.cs**: Lead conversion interface
- **IUpdateMarketingLeadReportDataService.cs**: Report cache refresh
- **IReviewPushLocationAPI.cs**: Google review push interface
- **INotificationToFA.cs**: Franchisee notification interface

### Domain/ (15 files)
Core business entities - see Domain CONTEXT.md

### Enum/ (5 files)
Type enumerations - see Enum CONTEXT.md

### Impl/ (11 files)
Service implementations - see Impl CONTEXT.md

### ViewModel/ (65 files)
Data transfer objects - see ViewModel CONTEXT.md

---

## Quick Reference: Common Operations

### Add New Lead Source
1. Create ViewModel for external API format
2. Add factory methods in IMarketingLeadsFactory
3. Add import method in MarketingLeadsService
4. Add conversion logic in UpdateConvertedLeadsService
5. Add reporting methods in MarketingLeadsReportService

### Add New Report
1. Define filter ViewModel
2. Add method to IMarketingLeadChartReportService
3. Implement in MarketingLeadChartReportService
4. Add download method if CSV export needed

### Modify Franchisee Assignment Logic
1. Update UpdateConvertedLeadsService.UpdateFranchisee()
2. Consider impact on routing number lookups
3. Test with various geo configurations

### Debug Lead Import Issues
1. Check ISettings for enabled flags (GetWebLeads, GetCallDetails)
2. Verify API keys in configuration
3. Review logs for API call failures
4. Check for SessionId duplicates in database
5. Verify timezone conversions using IClock.BrowserTimeZone
