# MarketingLead Domain Entities - AI/Agent Context

## Purpose

This folder contains the core business entities representing marketing leads from multiple acquisition channels. These are the persistent data models that capture every touchpoint in the customer journey from initial contact through conversion.

**Architecture Pattern**: Domain-Driven Design with Entity Framework navigation properties for relationships.

---

## Entity Inventory & Relationships

### Primary Entities

#### 1. **MarketingLeadCallDetail** (Core call tracking entity)
**Purpose**: Master record for every phone call received through call tracking systems (DialogTech/Invoca).

**Key Properties**:
- `SessionId` (string): Unique identifier from call tracking API - used for deduplication
- `DateAdded` (DateTime?): Call timestamp in UTC
- `DialedNumber` (string): Number customer called (DNIS - Dialed Number Identification Service)
- `CallerId` (string): Customer's phone number (ANI - Automatic Number Identification)
- `CallTypeId` (long): FK to Lookup (Inbound=140, Outbound=141, ClickTo=143, etc.)
- `CallTransferType` (string): How call was transferred ("Forward", "Conference", etc.)
- `PhoneLabel` (string): Marketing campaign identifier ("Local SEO", "National PPC", etc.)
- `TransferToNumber` (string): Franchisee phone number call was routed to
- `ClickDescription` (string): Web click source data
- `CallDuration` (int): Total call length in minutes
- `ValidCall` (bool): Whether call meets quality thresholds (duration, completion)
- `TagId` (long): FK to Tag (FranchiseDirect=1, National=2)
- `FranchiseeId` (long?): Assigned franchisee (nullable - may be unmapped initially)
- `CalledFranchiseeId` (long?): Franchisee actually reached by call
- `InvoiceId` (long?): FK to Invoice if lead converted to sale
- `IsFromNewAPI` (bool): Flag for DialogTech V3 API vs legacy
- `IsFromInvoca` (bool): Flag for Invoca API source

**Computed Property**:
- `TimeZoneStartTime` (DateTime, NotMapped): DateAdded adjusted to browser timezone via IClock.BrowserTimeZone

**Relationships**:
- → `Lookup` (CallType)
- → `Franchisee` (assigned franchisee)
- → `Franchisee` (called franchisee)
- → `Invoice` (conversion tracking)
- → `Tag` (lead category)
- ← `MarketingLeadCallDetailV2-V5` (1:1 child records with extended data)

**Business Rules**:
- SessionId must be unique across all records
- DateAdded stored as UTC, display conversions use IClock
- FranchiseeId assigned post-import via routing number lookup or zip code match
- InvoiceId populated when lead converts (matched by CallerId + date range)

---

#### 2. **MarketingLeadCallDetailV2** (Basic call flow data)
**Purpose**: Extends call detail with DialogTech V2 API data - basic call flow metadata.

**Key Properties**:
- `Sid` (string): SessionID reference
- `CallDate` (DateTime): Call timestamp
- `SetName` (string): Call flow set identifier
- `PhoneLabel` (string): Campaign label
- `TransferNumber` (string): Transfer destination
- `CallerId` (string): Customer ANI
- `EnteredZipCode` (string): Zip code entered in IVR
- `FirstName`, `LastName` (strings): Reverse lookup data
- `StreetAddress`, `City`, `State`, `ZipCode` (strings): Customer geo data
- `Reroute` (string): If call was rerouted
- `TalkMinutes`, `TalkSeconds` (strings): Talk time breakdown
- `TotalMinutes`, `TotalSeconds` (strings): Total duration
- `CallDuration` (string): Duration formatted
- `IvrResults` (string): IVR menu selections
- `Source`, `SourceNumber` (strings): Call origin
- `Destination` (string): Final destination
- `CallRoute` (string): Routing path taken
- `CallStatus` (string): Final status
- `APPState` (string): Application state
- `RepeaSourceCaller` (string): Repeat caller flag
- `SourceCap` (string): Source capacity limits
- `CallRouteQualified` (string): Qualified routing flag
- `SourceQualified` (string): Source qualification
- `FindMeList` (string): Find-me-follow-me routing list
- `FranchiseeId` (long?): Assigned franchisee
- `MarketingLeadCallDetailId` (long?): FK to parent MarketingLeadCallDetail

**Relationships**:
- → `Franchisee`
- → `MarketingLeadCallDetail` (parent record)
- ← `CallDetailsReportNotes` (1:many notes)

---

#### 3. **MarketingLeadCallDetailV3** (Call analytics & routing)
**Purpose**: Extends with DialogTech V3 data - advanced call flow, analytics, reverse lookup.

**Key Properties**:

**Call Flow Fields**:
- `CallflowSetName`, `CallflowSetId`: Call flow configuration
- `CallflowDestination`, `CallflowDestinationId`: Where call went
- `CallflowSource`, `CallflowSourceId`: Where call originated
- `CallflowSourceQualified` (string): Source qualification status
- `CallflowRepeatSourceCaller` (string): Repeat caller detection
- `CallflowSourceCap` (string): Source capacity
- `CallflowSourceRoute`, `CallflowSourceRouteId`: Routing path
- `CallflowSourceRouteQualified` (string): Route qualification
- `CallflowState` (string): Application state
- `CallflowEnteredZip` (string): IVR-entered zip
- `CallflowReroute` (string): Reroute information

**Transfer Fields**:
- `TransferToNumber` (string): Transfer destination
- `TransferToNumber_CallFlow` (string): Call flow transfer
- `TransferType_CallFlow` (string): Transfer mechanism
- `CallTransferStatus_CallFlow` (string): Transfer outcome
- `RingSeconds_CallFlow` (decimal?): Ring duration
- `RingCount_CallFlow` (decimal?): Number of rings

**Analytics Fields**:
- `KeywordGroups_CallAnalytics` (string): Detected keyword groups
- `KeywordSpottingComplete_CallAnalytics` (string): Keyword analysis complete flag
- `TranscriptionStatus_CallAnalytics` (string): Transcription status
- `CallNote_CallAnalytics` (string): System-generated call notes

**Recording Fields**:
- `RecordingUrl_Recording` (string): URL to call recording
- `RecordedSeconds_Recording` (string): Recording length
- `DialogAnalytics_Recording` (string): Dialog analytics summary

**Reverse Lookup Fields**:
- `FirstName_ReverseLookUp`, `LastName_ReverseLookUp`: Caller name
- `StreetLine1_ReverseLookUp`: Address
- `City_ReverseLookUp`, `StateCode_ReverseLookUp`, `PostalArea_ReverseLookUp`: Location
- `GeoLookupAttempt_ReverseLookUp`, `GeoLookupResult_ReverseLookUp`: Geo lookup status

**Homeowner Data** (Invoca additions):
- `home_owner_status_data_append` (string): Homeowner verification
- `home_market_value_data_append` (string): Property value estimate

**Relationships**:
- → `MarketingLeadCallDetail` (parent)

---

#### 4. **MarketingLeadCallDetailV4** (Attribution & visitor tracking)
**Purpose**: Marketing attribution, first/last touch tracking, visitor analytics.

**Key Properties**:

**Call Metrics**:
- `MissedCall_CallMetrics` (string): Missed call indicator
- `CallActivities` (string): Call activity log

**Attribution**:
- `Channel_Attribution` (string): Marketing channel
- `Status_Attribution` (string): Attribution status
- `Rank_Attribution` (string): Channel ranking
- `Pid_Attribution`, `Bid_Attribution` (strings): Tracking IDs

**First Touch**:
- `DocumentTitle_FirstTouch` (string): First page title
- `DocumentUrl_FirstTouch` (string): First page URL
- `DocumentPath_FirstTouch` (string): First page path
- `DocumentTimeStamp_FirstTouch` (string): First touch time

**Last Touch**:
- `DocumentTitle_LastTouch` (string): Last page before call
- `DocumentUrl_LastTouch` (string): Last page URL
- `DocumentPath_LastTouch` (string): Last page path
- `DocumentTimeStamp_LastTouch` (string): Last touch time

**Visitor Data**:
- `IPAddress_VisitorData` (string): Visitor IP
- `Device_VisitorData` (string): Device type
- `Browser_VisitorData`, `BrowserVersion_VisitorData`: Browser info
- `Os_VisitorData`, `OsVersion_VisitorData`: OS info
- `SearchTerm_VisitorData` (string): Search keyword
- `ActivityValue_VisitorData` (string): Activity value
- `ActivityTypeId_VisitorData` (string): Activity type
- `ActivityKeyword_VisitorData` (string): Activity keyword
- `ActivityTag_VisitorData` (string): Activity tag
- `Campaign_VisitorData` (string): Campaign name
- `Platform_VisitorData` (string): Platform
- `SourceGuard_VisitorData` (string): Source verification
- `VisitorLogUrl_VisitorData` (string): Visitor session URL
- `GoogleUaClientId_VisitorData` (string): Google Analytics client ID
- `GClid_VisitorData` (string): Google Click ID

**Relationships**:
- → `MarketingLeadCallDetail` (parent)

---

#### 5. **MarketingLeadCallDetailV5** (UTM & paid search)
**Purpose**: UTM parameters, Google Ads value track parameters, SourceIQ data, paid search attribution.

**Key Properties**:

**UTM Parameters**:
- `UtmSource_DefaultUtmParameters` (string): utm_source
- `UtmMedium_DefaultUtmParameters` (string): utm_medium
- `UtmCampaign_DefaultUtmParameters` (string): utm_campaign
- `UtmTerm_DefaultUtmParameters` (string): utm_term
- `UtmContent_DefaultUtmParameters` (string): utm_content

**Google Ads Value Track**:
- `VtKeyword_ValueTrackParameters` (string): {keyword}
- `VtMatchType_ValueTrackParameters` (string): {matchtype}
- `VtNetwork_ValueTrackParameters` (string): {network}
- `VtDevice_ValueTrackParameters` (string): {device}
- `VtDeviceModel_ValueTrackParameters` (string): {devicemodel}
- `VtCreative_ValueTrackParameters` (string): {creative}
- `VtPlacement_ValueTrackParameters` (string): {placement}
- `VtTarget_ValueTrackParameters` (string): {target}
- `VtParam1_ValueTrackParameters` (string): {param1}
- `VtParam2_ValueTrackParameters` (string): {param2}
- `VtRandom_ValueTrackParameters` (string): {random}
- `VtAceid_ValueTrackParameters` (string): {aceid}
- `VtAdposition_ValueTrackParameters` (string): {adposition}
- `VtProductTargetId_ValueTrackParameters` (string): {producttargetid}
- `VtAdType_ValueTrackParameters` (string): {adtype}

**SourceIQ Data**:
- `DomainSetName_SourceIqData` (string): Domain set name
- `DomainSetId_SourceIqData` (string): Domain set ID
- `PoolName_SourceIqData` (string): Number pool name
- `LocationName_SourceIqData` (string): Location name
- `CustomValue_SourceIqData` (string): Custom value
- `CustomId_SourceIqData` (string): Custom ID

**Paid Search**:
- `Campaign_PaidSearch` (string): Campaign name
- `CampaignId_PaidSearch` (string): Campaign ID
- `Adgroup_PaidSearch` (string): Ad group name
- `AdgroupId_PaidSearch` (string): Ad group ID
- `Ads_PaidSearch` (string): Ad creative
- `AdId_PaidSearch` (string): Ad ID
- `Keywords_PaidSearch` (string): Matched keyword
- `KeywordId_PaidSearch` (string): Keyword ID
- `ClickId_PaidSearch` (string): Click ID
- `KeyWordMatchType_PaidSearch` (string): Match type (exact, phrase, broad)
- `CallInlyFlag_PaidSearch` (string): Call-only ad flag
- `Type_PaidSearch` (string): Ad type

**Relationships**:
- → `MarketingLeadCallDetail` (parent)

---

#### 6. **WebLead** (Web form submissions)
**Purpose**: Capture leads from website contact forms.

**Key Properties**:
- `WebLeadId` (long): External API ID
- `WebLeadFranchiseeId` (long): External franchisee ID from API
- `Firstname`, `LastName` (strings): Customer name
- `Country`, `ProvinceName`, `County`, `ZipCode` (strings): Location
- `Email`, `Phone` (strings): Contact info
- `StreetAddress`, `SuiteNumber`, `City` (strings): Address
- `PropertyType` (string): Residential, Commercial, etc.
- `SurfaceType` (string): Marble, Granite, Concrete, etc.
- `ServiceDesc` (string): Service requested description
- `Contact` (string): Preferred contact method
- `AddEmail` (bool?): Add to mailing list flag
- `URL` (string): Submission source URL
- `Status` (int): Lead status code
- `CreatedDate` (DateTime): Submission timestamp
- `FEmail` (string): Franchisee email
- `FranchiseeId` (long?): Assigned franchisee (nullable initially)
- `InvoiceId` (long?): FK to Invoice if converted

**Computed Property**:
- `FullName` (string, get-only): Concatenation of Firstname + LastName

**Relationships**:
- → `Franchisee`
- → `Invoice`

**Business Rules**:
- FranchiseeId assigned via zip code → franchisee territory match
- InvoiceId populated when email/phone matches FranchiseeSales record

---

#### 7. **HomeAdvisor** (External lead provider)
**Purpose**: Leads purchased from HomeAdvisor marketplace.

**Key Properties**:
- `HAAccount` (string): HomeAdvisor account number
- `CompanyName` (string): Lead company name
- `SRID` (string): Service Request ID
- `SRSubmittedDate` (DateTime): Submission date
- `Task` (string): Service type requested
- `NetLeadDollar` (decimal?): Cost of lead
- `CityId` (long?): FK to City
- `StateId` (long?): FK to State
- `ZipCode` (string): Lead zip code
- `LeadType` (string): Lead category
- `CityName`, `StateName` (strings): Location text
- `FranchiseeId` (long?): Assigned franchisee

**Relationships**:
- → `City`
- → `State`
- → `Franchisee`

**Business Rules**:
- Imported from CSV files via HomeAdvisorFileParser
- FranchiseeId assigned by City + State geographic match
- No invoice tracking (external lead source)

---

#### 8. **RoutingNumber** (Phone number configuration)
**Purpose**: Configure marketing phone numbers for call tracking and franchisee assignment.

**Key Properties**:
- `PhoneNumber` (string): Tracking phone number
- `PhoneLabel` (string): Campaign/source identifier
- `FranchiseeId` (long?): Default franchisee for this number
- `TagId` (long?): Tag classification (FranchiseDirect, National)
- `CategoryId` (long?): FK to Lookup (PrintMedia, PhoneWebLocal, PhoneWebNational, etc.)

**Relationships**:
- → `Franchisee`
- → `Tag`
- → `Lookup` (Category)

**Business Rules**:
- Unique constraint: PhoneNumber + PhoneLabel combination
- Used for call detail franchisee assignment (match DialedNumber or PhoneLabel)
- Synced from DialogTech API (new records only, no updates)

---

#### 9. **Tag** (Lead categorization)
**Purpose**: Classify leads as franchise-direct or national campaigns.

**Key Properties**:
- `Name` (string): Tag name
- `IsActive` (bool): Active status

**Usage**:
- TagType.FranchiseDirect (Id=1): Lead generated by franchisee's local marketing
- TagType.National (Id=2): Lead generated by corporate national campaigns

---

#### 10. **CallDetailData** (Aggregated call metrics)
**Purpose**: Pre-computed call counts for reporting performance.

**Key Properties**:
- `FranchiseeId` (long?): Franchisee
- `PhoneLabel` (string): Campaign label
- `PhoneNumber` (string): Tracking number
- `StartDate`, `EndDate` (DateTime): Report period
- `Count` (int): Call count
- `IsWeekly` (bool): Weekly vs daily aggregation
- `DataRecorderMetaDataId` (long): FK to metadata

**Relationships**:
- → `DataRecorderMetaData` (audit trail)

**Usage**:
- Populated by UpdateMarketingLeadReportDataService background job
- Used for fast report generation without querying raw call details

---

#### 11. **WebLeadData** (Aggregated web metrics)
**Purpose**: Pre-computed web lead counts for reporting.

**Key Properties**:
- `FranchiseeId` (long?): Franchisee
- `Url` (string): Source URL
- `StartDate`, `EndDate` (DateTime): Report period
- `Count` (int): Lead count
- `IsWeekly` (bool): Aggregation period
- `DataRecorderMetaDataId` (long): FK to metadata

**Relationships**:
- → `DataRecorderMetaData`

**Usage**:
- Similar to CallDetailData but for web leads
- Enables fast dashboard queries

---

#### 12. **CallDetailsReportNotes** (Call annotations)
**Purpose**: User-generated notes and follow-up actions for calls.

**Key Properties**:
- `CallerId` (string): Phone number for note
- `Notes` (string): Free-text note
- `PreferredContactNumber` (long?): Preferred callback number
- `FirstName`, `LastName`, `Company`, `Office` (strings): Contact info
- `ZipCode`, `HouseNumber`, `Street`, `City`, `State`, `Country` (strings): Address
- `ResultingAction` (string): Follow-up action taken
- `Timestamp` (DateTime): Note creation time
- `EditTimestamp` (DateTime?): Last edit time
- `IsActive` (bool): Soft delete flag
- `CreatedBy` (string): User who created note
- `UserRole` (string): Creator's role
- `EmailId` (string): Contact email
- `MarketingLeadId` (long?): FK to MarketingLeadCallDetailV2
- `MarketingLeadIdFromCallDetailsReport` (long?): Alternate FK
- `DataRecorderMetaDataId` (long): Audit trail

**Relationships**:
- → `MarketingLeadCallDetailV2` (two FK fields for different linking scenarios)
- → `DataRecorderMetaData`

**Business Rules**:
- Linked to call records via MarketingLeadId after import
- Can exist before call record (manual entry by phone number)
- Supports edit history via EditTimestamp

**Related ViewModel**:
- `CallNotesHistoryViewModel` (embedded in same file): Comprehensive view joining call data + notes

---

#### 13. **Phonechargesfee** (Phone call billing)
**Purpose**: Track phone call costs for franchisee invoicing.

**Key Properties**:
- `Amount` (decimal): Call cost
- `FranchiseeId` (long): Billed franchisee
- `InvoiceItemId` (long?): FK to invoice line item
- `Description` (string): Charge description
- `DateCreated` (DateTime): Charge date
- `CurrencyExchangeRateId` (long?): Currency rate for international
- `FranchiseetechmailserviceId` (long?): Related email service
- `IsInvoiceGenerated` (bool): Invoice created flag
- `IsInvoiceInQueue` (bool): Queued for invoicing
- `FranchiseeservicefeeId` (long?): Related service fee

**Relationships**:
- → `Franchisee`
- → `InvoiceItem`
- → `CurrencyExchangeRate`
- → `FranchiseeTechMailEmail`
- → `FranchiseeServiceFee`

**Usage**:
- Separate from MarketingLeadCallDetail (billing vs tracking)
- Used to generate phone call invoices for franchisees

---

#### 14. **FranchsieeGoogleReviewUrlAPI** (Google review integration)
**Purpose**: Store Google review links for franchisees.

**Key Properties**:
- `FranchiseeId` (long): Franchisee
- `BusinessName` (string): Google Business Profile name
- `BrightLocalLink` (string): Review URL

**Relationships**:
- → `Franchisee` (note: property name typo "Francbhisee")

**Usage**:
- Used by ReviewPushLocationAPI to send review request links
- Links customers to franchisee Google Business Profile

---

## Data Flow Patterns

### Call Detail Import Flow
```
External API (DialogTech/Invoca)
    ↓
MarketingLeadsService.GetCallDetailsV3/Invoca()
    ↓
Factory creates MarketingLeadCallDetail + V2/V3/V4/V5 entities
    ↓
Repository saves all 5 versions (parallel)
    ↓
UpdateConvertedLeadsService assigns FranchiseeId
    ↓
UpdateConvertedLeadsService assigns InvoiceId
```

### Web Lead Import Flow
```
External API (Web Form)
    ↓
MarketingLeadsService.GetWebLeads()
    ↓
Factory creates WebLead entities
    ↓
Repository saves
    ↓
UpdateConvertedLeadsService assigns FranchiseeId (via zip)
    ↓
UpdateConvertedLeadsService assigns InvoiceId (via email/phone)
```

### Franchisee Assignment Logic
```
For Calls:
1. Match DialedNumber → RoutingNumber.PhoneNumber → RoutingNumber.FranchiseeId
2. If no match, match PhoneLabel → RoutingNumber.PhoneLabel → RoutingNumber.FranchiseeId
3. If no match, check Invoca.office field (direct franchisee name)
4. If no match, use EnteredZipCode → ZipCode.CountyId → Franchisee.CountyId
5. If no match, leave FranchiseeId null (unmapped lead)

For Web Leads:
1. Match ZipCode → Franchisee.ZipCodes collection
2. If no match, leave FranchiseeId null
```

### Invoice Linking Logic
```
For Calls:
- Find FranchiseeSales where Phone == CallerId
- AND InvoiceDate within ±2 months of DateAdded
- Set InvoiceId to FranchiseeSales.InvoiceId

For Web Leads:
- Find FranchiseeSales where Phone == Phone OR Email == Email
- AND InvoiceDate within ±2 months of CreatedDate
- Set InvoiceId to FranchiseeSales.InvoiceId
```

---

## Critical Business Rules

### Call Detail Versioning Strategy
**Why 5 versions?**: Progressive API enhancement over time
- V1 (MarketingLeadCallDetail): Core call data (always present)
- V2: Basic call flow data (DialogTech V2)
- V3: Advanced routing, analytics, reverse lookup (DialogTech V3)
- V4: Attribution, first/last touch, visitor analytics
- V5: UTM parameters, paid search attribution

**Storage Pattern**: All 5 versions saved for every call (even if some fields null)
**Rationale**: Historical data preservation - can't backfill if not saved initially

### Deduplication Rules
- **SessionId uniqueness**: First record wins, duplicates logged and skipped
- **No web lead deduplication**: Allows multiple submissions from same customer
- **RoutingNumber uniqueness**: PhoneNumber + PhoneLabel must be unique

### Null Handling
- **FranchiseeId nullable**: Common for new leads before assignment runs
- **InvoiceId nullable**: Common for non-converted leads
- **All V2-V5 fields nullable**: API may not provide all data

### Timezone Handling
- All DateTime fields stored as UTC
- Display conversions use IClock.BrowserTimeZone
- TimeZoneStartTime computed property handles display conversion

---

## Entity Framework Relationships

### One-to-One
- MarketingLeadCallDetail → MarketingLeadCallDetailV2 (MarketingLeadCallDetailId FK)
- MarketingLeadCallDetail → MarketingLeadCallDetailV3 (MarketingLeadCallDetailId FK)
- MarketingLeadCallDetail → MarketingLeadCallDetailV4 (MarketingLeadCallDetailId FK)
- MarketingLeadCallDetail → MarketingLeadCallDetailV5 (MarketingLeadCallDetailId FK)

### Many-to-One
- MarketingLeadCallDetail → Franchisee (FranchiseeId FK)
- MarketingLeadCallDetail → Franchisee (CalledFranchiseeId FK)
- MarketingLeadCallDetail → Invoice (InvoiceId FK)
- MarketingLeadCallDetail → Tag (TagId FK)
- MarketingLeadCallDetail → Lookup (CallTypeId FK)
- WebLead → Franchisee (FranchiseeId FK)
- WebLead → Invoice (InvoiceId FK)
- HomeAdvisor → Franchisee (FranchiseeId FK)
- HomeAdvisor → City (CityId FK)
- HomeAdvisor → State (StateId FK)
- RoutingNumber → Franchisee (FranchiseeId FK)
- RoutingNumber → Tag (TagId FK)
- RoutingNumber → Lookup (CategoryId FK)
- CallDetailsReportNotes → MarketingLeadCallDetailV2 (MarketingLeadId FK)
- Phonechargesfee → Franchisee (FranchiseeId FK)
- Phonechargesfee → InvoiceItem (InvoiceItemId FK)
- FranchsieeGoogleReviewUrlAPI → Franchisee (FranchiseeId FK)

### One-to-Many
- MarketingLeadCallDetailV2 ← CallDetailsReportNotes (multiple notes per call)

---

## Performance Considerations

### Indexing Recommendations
- MarketingLeadCallDetail: Index on SessionId (unique), DateAdded, FranchiseeId, PhoneLabel
- MarketingLeadCallDetailV2-V5: Index on MarketingLeadCallDetailId, Sid
- WebLead: Index on ZipCode, FranchiseeId, Email, Phone, CreatedDate
- RoutingNumber: Unique index on PhoneNumber + PhoneLabel
- CallDetailData, WebLeadData: Index on FranchiseeId, StartDate, EndDate

### Query Optimization
- Use CallDetailData/WebLeadData for reports (pre-aggregated)
- Avoid joining all V1-V5 tables unless necessary
- Filter by DateAdded/CreatedDate to limit result sets

### N+1 Query Risks
- Loading CallType, Tag, Franchisee navigation properties lazily
- Recommendation: Use `.Include()` for bulk loads

---

## File Inventory

1. **MarketingLeadCallDetail.cs** - Core call tracking entity
2. **MarketingLeadCallDetailV2.cs** - Basic call flow extension
3. **MarketingLeadCallDetailV3.cs** - Advanced analytics extension
4. **MarketingLeadCallDetailV4.cs** - Attribution/visitor extension
5. **MarketingLeadCallDetailV5.cs** - UTM/paid search extension
6. **WebLead.cs** - Web form submission entity
7. **WebLeadData.cs** - Aggregated web metrics
8. **HomeAdvisor.cs** - External lead source entity
9. **RoutingNumber.cs** - Phone routing configuration
10. **Tag.cs** - Lead categorization
11. **CallDetailData.cs** - Aggregated call metrics
12. **CallDetailsReportNotes.cs** - Call annotation entity + CallNotesHistoryViewModel
13. **Phonechargesfee.cs** - Call billing entity
14. **FranchsieeGoogleReviewUrlAPI.cs** - Review URL entity

---

## Common Queries

### Get Call Details with All Extensions
```csharp
var call = _marketingLeadCallDetailRepository.Table
    .Include(x => x.CallType)
    .Include(x => x.Franchisee)
    .Include(x => x.Tag)
    .FirstOrDefault(x => x.Id == callId);

var v2 = _marketingLeadCallDetailV2Repository.Table
    .FirstOrDefault(x => x.MarketingLeadCallDetailId == callId);
// Repeat for V3, V4, V5
```

### Find Unmapped Leads
```csharp
var unmappedCalls = _marketingLeadCallDetailRepository.Table
    .Where(x => x.FranchiseeId == null)
    .Where(x => x.DateAdded >= startDate);

var unmappedWebLeads = _webLeadRepository.Table
    .Where(x => x.FranchiseeId == null)
    .Where(x => x.CreatedDate >= startDate);
```

### Get Converted Leads
```csharp
var convertedCalls = _marketingLeadCallDetailRepository.Table
    .Where(x => x.InvoiceId.HasValue)
    .Where(x => x.DateAdded >= startDate)
    .Include(x => x.Invoice);
```

### Match Routing Number
```csharp
var routingNumber = _routingNumberRepository.Table
    .FirstOrDefault(x => x.PhoneNumber == dialedNumber && 
                        x.PhoneLabel.ToLower() == phoneLabel.ToLower());
```
