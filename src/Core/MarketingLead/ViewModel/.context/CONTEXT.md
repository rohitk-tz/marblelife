# MarketingLead ViewModels - AI/Agent Context

## Purpose

This folder contains 65 Data Transfer Objects (DTOs) for API communication, UI rendering, and data filtering. ViewModels bridge external APIs, domain entities, and presentation layers with validation, formatting, and serialization logic.

**Architecture Pattern**: POCO classes with attributes for display, validation, and serialization (XML/JSON). Organized by functional area.

---

## ViewModel Categories

### Category 1: Call Detail Views (15 files)

#### **CallDetailViewModel.cs**
**Purpose**: Comprehensive call detail representation with all V1-V5 data merged.

**Key Property Groups** (307 properties total):
- **Core** (14): Id, DateOfCall, Ani, Dnis, TransferToNumber, CallType, CallDuration, ValidCall, Franchisee, Tag, InvoiceId
- **V2 Data** (25): FindMeList, SetName, EnteredZip, IvrResults, TalkMinutes, CallRoute, CallStatus
- **V3 Call Flow** (28): CallFlowSetName, CallFlowDestination, CallFlowSource, CallFlowState, CallFlowReroute, KeywordGroups, RecordingUrl
- **V3 Reverse Lookup** (8): FirstName_ReverseLookUp, LastName_ReverseLookUp, StreetLine1, City, State, PostalArea
- **V4 Attribution** (8): Channel_Attribution, Status_Attribution, Rank, PID, BID
- **V4 First/Last Touch** (8): DocumentTitle_FirstTouch, DocumentUrl_FirstTouch, DocumentTimeStamp (both touches)
- **V4 Visitor Data** (20): IPAddress, Device, Browser, OS, SearchTerm, Platform, GoogleUaClientId, GClid
- **V5 UTM Parameters** (5): UtmSource, UtmMedium, UtmCampaign, UtmTerm, UtmContent
- **V5 Value Track** (15): VtKeyword, VtMatchType, VtNetwork, VtDevice, VtCreative, VtPlacement, etc.
- **V5 SourceIQ** (6): DomainSetName, PoolName, LocationName, CustomValue
- **V5 Paid Search** (12): Campaign, AdGroup, Keywords, ClickId, KeyWordMatchType
- **Call Notes** (11): CallNote, PreferredContactNumber, Email, FirstName, LastName, Company, Office, ZipCode, ResultingAction

**Display Attributes**: All properties decorated with `[DisplayName]` for UI rendering

**Usage**: Primary ViewModel for call detail list/detail views, combines all API versions into single flat structure

---

#### **CallDetailViewModelV2.cs**
**Purpose**: V2-specific call detail representation (likely simplified version).

---

#### **CallRetailRecord.cs, CallRetailRecordV2.cs, CallRetailRecordV3.cs**
**Purpose**: External API request/response models for DialogTech API formats.

**CallRetailRecordV3 Structure**:
```csharp
[XmlRoot("call_details")]
public class CallRetailRecordV3
{
    [XmlElement("start_time")] public string start_time;
    [XmlElement("sid")] public string SessionId;
    [XmlElement("dialed_number")] public string dialed_number;
    [XmlElement("caller_id")] public string caller_id;
    [XmlElement("phone_label")] public string phone_label;
    [XmlElement("call_duration_rounded_minutes")] public decimal call_duration_rounded_minutes;
    
    // Nested complex objects
    [XmlElement("advanced_routing")] public AdvancedRouting advanced_routing;
    [XmlElement("call_transfer")] public CallTransfer call_transfer;
    [XmlElement("call_analytics")] public CallAnalytics call_analytics;
    [XmlElement("reverse_lookup")] public List<ReverseLookUp> reverse_lookup;
    [XmlElement("call_metrics")] public CallMetrics call_metrics;
}

public class AdvancedRouting { /* routing fields */ }
public class CallTransfer { public string transfer_to_number; public string transfer_type; }
public class CallAnalytics { public List<string> keyword_groups; public string transcription_status; }
public class CallMetrics { public string MissedCall; }
```

**XML Serialization**: Designed for DialogTech XML API responses

---

#### **InvocaCallDetails.cs**
**Purpose**: Invoca API format (380 properties, XML-based).

**Key Sections**:
- **Core** (15): complete_call_id, start_time, calling_phone_number, destination_phone_number, duration, connect_duration
- **Call Result** (10): call_result_description_detail, signal_name, hangup_cause
- **Data Append** (15): first_name_data_append, last_name_data_append, address_full_street, home_owner_status, home_market_value
- **Campaign** (10): advertiser_campaign_name, advertiser_campaign_id, affiliate_name
- **Dynamic Pool** (15): dynamic_number_pool_id, dynamic_number_pool_referrer_search_type, dynamic_number_pool_referrer_referrer_campaign
- **Paid Search** (10): dynamic_number_pool_referrer_search_keywords, dynamic_number_pool_referrer_keyword_match_type
- **Tracking** (10): utm_source, utm_medium, utm_campaign, utm_term, utm_content, gclid, g_cid
- **Custom Fields** (15): region, office, verified_zip, caller_zip, find_me_list
- **Session Data** (10): landing_title, landing_page, calling_page, calling_path
- **Recording** (5): recording, notes, Answered_By_Agent

**XML Attributes**: Every property has `[XmlElement("element_name")]` attribute

**Note**: Many commented-out duplicate properties (API version changes)

---

#### **CallDetailFilter.cs**
**Purpose**: Filter criteria for call detail queries.

```csharp
public class CallDetailFilter
{
    public long? FranchiseeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? CallTypeId { get; set; }
    public string CallerId { get; set; }  // Phone number search
    public string PhoneLabel { get; set; }
    public string TagId { get; set; }
    public string CategoryIds { get; set; }  // Comma-separated list
    public string Text { get; set; }  // General search
    public string SortingColumn { get; set; }
    public long? SortingOrder { get; set; }
}
```

---

#### **CallDetailListModel.cs, CallDetailListModelV2.cs**
**Purpose**: Paginated call detail list containers.

```csharp
public class CallDetailListModel
{
    public List<CallDetailViewModel> Collection { get; set; }
    public CallDetailFilter Filter { get; set; }
    public PagingModel PagingModel { get; set; }
}
```

---

#### **CallDetailListV2.cs, CallDetailListV3.cs**
**Purpose**: Alternative list formats (likely different column sets or optimizations).

---

#### **CallDetailReportViewModel.cs, CallDetailReportListModel.cs**
**Purpose**: Aggregated report format (grouped/summarized data).

---

#### **CallDetailsReportNotesViewModel.cs, CallDetailNotesFilter.cs**
**Purpose**: Call note annotations and filtering.

```csharp
public class CallDetailsReportNotesViewModel
{
    public long? Id { get; set; }
    public string CallerId { get; set; }
    public string Notes { get; set; }
    public long? PreferredContactNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string Office { get; set; }
    public string ZipCode { get; set; }
    public string ResultingAction { get; set; }
    public string HouseNumber { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string EmailId { get; set; }
}
```

---

### Category 2: Web Lead Views (8 files)

#### **WebLeadViewModel.cs**
**Purpose**: Web form submission from external API.

```csharp
public class WebLeadViewModel
{
    public long Id { get; set; }
    public long franchise_id { get; set; }
    public string name_first { get; set; }
    public string name_last { get; set; }
    public string franchise_name { get; set; }
    public string franchise_email { get; set; }
    public string Country { get; set; }
    public string Province { get; set; }
    public string County { get; set; }
    public string zip { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Addr { get; set; }
    public string Addr2 { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string type_property { get; set; }  // Residential, Commercial
    public string type_surface { get; set; }   // Marble, Granite, etc.
    public string surface_desc { get; set; }   // Service description
    public string source_contact { get; set; } // Preferred contact method
    public string AddEmail { get; set; }       // Newsletter opt-in
    public string source_url { get; set; }     // Form submission URL
    public int? status { get; set; }
    public string date_created { get; set; }
    public string femail { get; set; }         // Franchisee email
}
```

**Naming Convention**: Lowercase with underscores (matches external API)

---

#### **WebLeadInfoModel.cs**
**Purpose**: Domain entity → ViewModel transformation.

---

#### **WebLeadFilter.cs**
**Purpose**: Web lead query filters.

```csharp
public class WebLeadFilter
{
    public long? FranchiseeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ZipCode { get; set; }
    public string Text { get; set; }
}
```

---

#### **WebLeadListViewModel.cs, WebLeadListModel.cs**
**Purpose**: Paginated web lead lists.

---

#### **WebLeadReportViewModel.cs, WebleadReportListModel.cs**
**Purpose**: Aggregated web lead reports.

---

#### **WebLeadFranchiseeList.cs**
**Purpose**: Franchisee-specific web lead view.

---

### Category 3: Routing Number Views (4 files)

#### **RoutingNumberViewModel.cs**
**Purpose**: Phone routing configuration display.

```csharp
public class RoutingNumberViewModel
{
    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string PhoneLabel { get; set; }
    public string FranchiseeName { get; set; }
    public string TagName { get; set; }
    public string CategoryName { get; set; }
}
```

---

#### **RoutingNumberRecord.cs**
**Purpose**: External API format for routing number sync.

```csharp
public class RoutingNumberRecord
{
    [XmlElement("PhoneNumber")]
    public string PhoneNumber { get; set; }
    
    [XmlElement("PhoneLabel")]
    public string PhoneLabel { get; set; }
}
```

---

#### **RoutingNumberList.cs**
**Purpose**: XML container for routing number API response.

```csharp
[XmlRoot("RoutingNumbers")]
public class RoutingNumberList
{
    [XmlElement("RoutingNumber")]
    public List<RoutingNumberRecord> record { get; set; }
}
```

---

#### **RoutingNumberListModel.cs**
**Purpose**: Paginated routing number list.

---

### Category 4: Report Filter Views (6 files)

#### **MarketingLeadReportFilter.cs**
**Purpose**: Universal report filter (used by most report methods).

```csharp
[NoValidatorRequired]
public class MarketingLeadReportFilter
{
    public long? FranchiseeId { get; set; }
    public string Text { get; set; }
    public string SortingColumn { get; set; }
    public long? SortingOrder { get; set; }
    public string URL { get; set; }
    public int PageNumber { get; set; }
    public int WebPageNumber { get; set; }
    public int PageSize { get; set; }
    public int ViewTypeId { get; set; }
    public int Month { get; set; }
    public int Week { get; set; }
    public int Day { get; set; }
    public int Count { get; set; }
    public long CallTypeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ConvertedLead { get; set; }
    public int? MappedFranchisee { get; set; }
    public long? TagId { get; set; }
    public string Year { get; set; }
}
```

---

#### **HomeAdvisorReportFilter.cs**
**Purpose**: HomeAdvisor-specific report filters.

---

#### **ManagementChartReportFilter.cs**
**Purpose**: Executive dashboard filters.

---

#### **LeadPerformanceFranchiseeFilter.cs**
**Purpose**: Franchisee performance analysis filters.

---

#### **PhoneCallFilter.cs**
**Purpose**: Phone call management filters.

---

#### **AutomationBackUpFilter.cs**
**Purpose**: Automation/backup report filters.

---

### Category 5: Chart/Analytics Views (28 files)

#### **Lead Performance Views** (6 files):
- **LeadPerformanceFranchiseeViewModel.cs**: Franchisee performance metrics
- **LeadPerformanceFranchiseeNationalViewModel.cs**: National-level metrics
- **LeadPerformanceListModel.cs**: List container
- **LeadPerformanceNationalListModel.cs**: National list
- **LeadWebSiteFranchiseeViewModel.cs**: Website performance by franchisee
- **LeadWebSiteFranchiseeLocalViewModel.cs**: Local site performance

**Typical Structure**:
```csharp
public class LeadPerformanceFranchiseeViewModel
{
    public string FranchiseeName { get; set; }
    public int TotalLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageCost { get; set; }
    public decimal ROI { get; set; }
    public int PhoneLeads { get; set; }
    public int WebLeads { get; set; }
}
```

---

#### **Management Chart Views** (13 files):
- **ManagementChartViewModel.cs**: Executive dashboard data
- **ManagementChartListModel.cs**: Chart list container
- **ManagementChartListModelForChart.cs**: Chart-optimized format
- **ManagementChartBusinessHourListModel.cs**: Business hours analysis
- **ManagementChartDayListModel.cs, ManagementChartDayViewModel.cs**: Day-of-week analysis
- **ManagementChartGroupedListModel.cs**: Grouped metrics
- **ManagementVsLocalChartListModel.cs**: Corporate vs franchisee comparison
- **ManagementAndLocalChartViewModel.cs, ManagementAndLocalChartGraphViewModel.cs**: Combined views

**Purpose**: Dashboard charts for management (trends, comparisons, seasonal patterns)

---

#### **Marketing Lead Chart Views** (3 files):
- **MarketingLeadChartViewModel.cs**: General chart data
- **MarketingLeadChartListModel.cs**: Chart list
- **MarketingLeadCallDetailViewModel.cs**: Call-specific chart data

---

#### **HomeAdvisor Views** (2 files):
- **HomeAdvisorReportListModel.cs**: HomeAdvisor report data
- **HomeAdvisorParentModel.cs**: Parent model for nested data

---

### Category 6: Phone Call Management Views (10 files)

#### **PhoneCallEditModel.cs**
**Purpose**: Single call record edit.

```csharp
public class PhoneCallEditModel
{
    public long Id { get; set; }
    public long? FranchiseeId { get; set; }
    public string Notes { get; set; }
}
```

---

#### **PhoneCallEditByBulkModel.cs, PhoneCallEditByBulkList.cs**
**Purpose**: Bulk edit operations.

---

#### **PhoneCallInvoiceEditModel.cs**
**Purpose**: Generate invoice from call records.

```csharp
public class PhoneCallInvoiceEditModel
{
    public long FranchiseeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<long> CallIds { get; set; }
}
```

---

#### **FranchiseePhoneCallViewModel.cs, FranchiseePhoneCallListModel.cs**
**Purpose**: Franchisee-specific call views.

---

#### **FranchiseePhoneCallBulkListModel.cs, AutomationBackUpCallListModel.cs**
**Purpose**: Bulk operations and backup reports.

---

### Category 7: Specialized Views (4 files)

#### **MarketingLeadCallDetailViewModel.cs, MarketingLeadCallDetailViewModelV2.cs**
**Purpose**: Factory input models (intermediate transformation step).

---

#### **CallDetailsViewModel.cs**
**Purpose**: Alternative call details format.

---

#### **CallNotesHistoryViewModel.cs** (embedded in CallDetailsReportNotes.cs)
**Purpose**: Call note history with full call context.

---

## Common Patterns

### 1. **Display Attributes**
```csharp
[DisplayName("Date/Time Of Call")]
public DateTime DateOfCall { get; set; }
```

### 2. **XML Serialization**
```csharp
[XmlElement("start_time")]
public string start_time { get; set; }
```

### 3. **Validation Attributes**
```csharp
[NoValidatorRequired]  // Skip validation
[DownloadField(Required = false)]  // Optional in exports
```

### 4. **List Container Pattern**
```csharp
public class XListModel
{
    public List<XViewModel> Collection { get; set; }
    public XFilter Filter { get; set; }
    public PagingModel PagingModel { get; set; }
}
```

### 5. **Naming Conventions**
- **External APIs**: lowercase_with_underscores (WebLeadViewModel, InvocaCallDetails)
- **Internal**: PascalCase (CallDetailViewModel)
- **Filters**: Suffix with "Filter"
- **Lists**: Suffix with "ListModel" or "ViewModel"

---

## Usage by Component

### MarketingLeadsService (API import)
- **Input**: CallRetailRecordV3, InvocaCallDetails, WebLeadViewModel, RoutingNumberList
- **Output**: None (transforms to domain entities)

### MarketingLeadsReportService (Reporting)
- **Input**: All *Filter ViewModels
- **Output**: All *ListModel and *ViewModel types

### MarketingLeadsFactory (Transformation)
- **Input/Output**: All ViewModels ↔ Domain entities

### UI Controllers (Presentation)
- **Input**: All *Filter ViewModels (from forms)
- **Output**: All *ListModel and *ViewModel (for grids/charts)

---

## File Organization

**Total**: 65 files organized by function:
- Call Details: 15 files
- Web Leads: 8 files
- Routing Numbers: 4 files
- Filters: 6 files
- Charts/Analytics: 28 files
- Phone Management: 10 files
- Specialized: 4 files

---

## Metadata

```json
{
  "version": "1.1",
  "last_commit": "64667c5c8c4ab9b3d804e48deb14e9b70895fc42",
  "generated_at": "2025-01-10T00:00:00Z",
  "file_count": 65,
  "changed_files": []
}
```
