# MarketingLead Module - Developer Guide

## What This Module Does

Think of this as MarbleLife's **lead management headquarters**. Every potential customer interaction—whether they fill out a web form, call a phone number, or come through HomeAdvisor—flows through this system. It's like a sophisticated call center + CRM hybrid that:

1. **Captures leads** from multiple channels (web, phone, HomeAdvisor)
2. **Routes them intelligently** to the right franchisee based on location
3. **Tracks conversions** when leads become paying customers
4. **Provides analytics** so management can see which marketing channels work

**Real-world analogy**: Imagine an airport traffic control system, but for customer leads. Each lead is a "plane" that needs to be identified, tracked, and directed to the correct "gate" (franchisee), while keeping detailed logs of every step for analysis.

---

## Getting Started

### Prerequisites
- Access to DialogTech and/or Invoca call tracking accounts
- Web lead API credentials
- Database with franchisee territory configurations
- Scheduled job runner for background processes

### Configuration

Add these to your `appsettings.json` or `ISettings` implementation:

```json
{
  "GetWebLeads": true,
  "GetCallDetails": true,
  "WebLeadsAPIkey": "your-web-lead-api-key",
  "AccessKey": "dialogtech-access-key",
  "SecretKey": "dialogtech-secret-key",
  "InvocaAPIKey": "invoca-api-key",
  "IsMapFranchiseeToFranchiseePhoneWithFranchiseeId": true
}
```

### Basic Usage

```csharp
// Dependency injection setup
services.AddTransient<IMarketingLeadsService, MarketingLeadsService>();
services.AddTransient<IMarketingLeadsReportService, MarketingLeadsReportService>();
services.AddTransient<IMarketingLeadsFactory, MarketingLeadsFactory>();

// Import all leads (typically run as scheduled job)
var leadService = serviceProvider.GetService<IMarketingLeadsService>();
leadService.GetMarketingLeads(); // Fetches web leads, call details, routing numbers

// Convert leads to franchisees (run after import)
var conversionService = serviceProvider.GetService<IUpdateConvertedLeadsService>();
conversionService.UpdateLeads(); // Maps leads to franchisees, links to invoices
```

---

## Common Use Cases

### 1. Get Call Details for a Franchisee

```csharp
var reportService = serviceProvider.GetService<IMarketingLeadsReportService>();

var filter = new CallDetailFilter
{
    FranchiseeId = 123,
    StartDate = DateTime.Now.AddMonths(-1),
    EndDate = DateTime.Now,
    CallTypeId = (long)CallType.Inbound
};

var result = reportService.GetCallDetailList(filter, pageNumber: 1, pageSize: 50);

foreach (var call in result.List)
{
    Console.WriteLine($"{call.DateOfCall}: {call.Ani} → {call.Dnis} ({call.CallDuration} min)");
}
```

### 2. Generate Lead Performance Report

```csharp
var chartService = serviceProvider.GetService<IMarketingLeadChartReportService>();

var filter = new MarketingLeadReportFilter
{
    FranchiseeId = 123,
    StartDate = new DateTime(2024, 1, 1),
    EndDate = new DateTime(2024, 12, 31)
};

var phoneVsWeb = chartService.GetPhoneVsWebReport(filter);
var localVsNational = chartService.GetLocalVsNationalReport(filter);
var seasonalTrends = chartService.GetSeasonalLeadReport(filter);
```

### 3. Export Call Details to CSV

```csharp
var reportService = serviceProvider.GetService<IMarketingLeadsReportService>();

var filter = new CallDetailFilter
{
    StartDate = DateTime.Now.AddMonths(-3),
    EndDate = DateTime.Now
};

string fileName;
if (reportService.DownloadMarketingLeads(filter, out fileName))
{
    Console.WriteLine($"CSV exported to: {fileName}");
}
```

### 4. Manually Assign a Lead to a Franchisee

```csharp
var reportService = serviceProvider.GetService<IMarketingLeadsReportService>();

long leadId = 5678;
long franchiseeId = 123;

if (reportService.UpdateFranchisee(leadId, franchiseeId))
{
    Console.WriteLine("Franchisee assigned successfully");
}
```

### 5. Add Call Notes

```csharp
var reportService = serviceProvider.GetService<IMarketingLeadsReportService>();

var note = new CallDetailsReportNotesViewModel
{
    MarketingLeadCallDetailId = 9876,
    Notes = "Customer requested estimate for marble restoration",
    CreatedBy = "JohnDoe",
    CreatedDate = DateTime.Now
};

reportService.SaveCallDetailsReportNotes(note);
```

### 6. Process HomeAdvisor Leads

```csharp
// Upload CSV file
var parser = serviceProvider.GetService<IHomeAdvisorFileParser>();

// Validate file
DataTable dt = LoadCsvToDataTable("homeadvisor-leads.csv");
string message;
if (parser.CheckForValidHeader(dt, out message))
{
    parser.ProcessRecords(); // Imports and maps to franchisees
}
```

---

## Architecture Overview

### Data Flow Diagram

```
External Sources                Module Components               Database
─────────────────              ─────────────────────           ────────────
                                                              
Web Forms ──────┐                                             
                │──→ MarketingLeadsService ──→ Factory ──→ WebLead
DialogTech API ─┤       (Orchestrator)           ↓           
                │                                └──→ MarketingLeadCallDetail
Invoca API ─────┤                                             (V1-V5)
                │                                     
HomeAdvisor CSV─┘                                             
                                                              
                                ↓                             
                                                              
Scheduled Job ────→ UpdateConvertedLeadsService              
                            │                                 
                            ├──→ Map to Franchisees (via geo)
                            └──→ Link to Invoices (conversion)
                                                              
                                ↓                             
                                                              
UI/API Requests ─→ MarketingLeadsReportService ──→ ViewModels
                        (Filtering, Export)           
```

### Module Structure

```
MarketingLead/
├── Domain/               ← Core business entities (15 files)
│   ├── MarketingLeadCallDetail.cs
│   ├── WebLead.cs
│   ├── HomeAdvisor.cs
│   └── RoutingNumber.cs
│
├── Enum/                ← Type definitions (5 files)
│   ├── CallType.cs
│   └── RoutingNumberCategory.cs
│
├── Impl/                ← Service implementations (11 files)
│   ├── MarketingLeadsService.cs        (API orchestrator)
│   ├── UpdateConvertedLeadsService.cs  (franchisee mapping)
│   ├── MarketingLeadsReportService.cs  (48 report methods)
│   └── HomeAdvisorFileParser.cs        (CSV import)
│
├── ViewModel/           ← DTOs for API/UI (65 files)
│   ├── CallDetailViewModel.cs
│   ├── WebLeadViewModel.cs
│   └── *Filter.cs, *ListModel.cs
│
└── *.cs                 ← Public interfaces (13 files)
    ├── IMarketingLeadsService.cs
    ├── IMarketingLeadsReportService.cs
    └── IMarketingLeadsFactory.cs
```

---

## API Reference

### Core Services

#### IMarketingLeadsService
**Purpose**: Master orchestrator for lead imports.

```csharp
void GetMarketingLeads();
```

**When to call**: Scheduled job (hourly or daily). Fetches leads from all configured sources.

**Side effects**: 
- Inserts new WebLead records
- Inserts new MarketingLeadCallDetail records (V1-V5)
- Syncs RoutingNumbers
- Logs extensively

---

#### IMarketingLeadsReportService
**Purpose**: Retrieve, filter, and export lead data.

**Key Methods**:

| Method | Purpose | Returns |
|--------|---------|---------|
| `GetCallDetailList(filter, page, size)` | Paginated call list | CallDetailListModel |
| `GetWebLeadList(filter, page, size)` | Paginated web lead list | WebLeadListViewModel |
| `GetCallDetailReport(filter)` | Aggregated call report | CallDetailReportListModel |
| `UpdateFranchisee(id, franchiseeId)` | Reassign lead | bool |
| `DownloadMarketingLeads(filter)` | Export to CSV | bool + fileName |
| `SaveCallDetailsReportNotes(note)` | Add call note | bool |

**Filtering Example**:
```csharp
var filter = new CallDetailFilter
{
    FranchiseeId = 123,              // Specific franchisee
    StartDate = DateTime.Now.AddDays(-30),
    EndDate = DateTime.Now,
    CallTypeId = (long)CallType.Inbound,
    TagId = (long)TagType.National,  // National vs local
    Text = "555-1234"                // Search by phone number
};
```

---

#### IMarketingLeadChartReportService
**Purpose**: Generate aggregated analytics for dashboards.

**Available Reports**:

| Method | What It Shows |
|--------|---------------|
| `GetPhoneVsWebReport()` | Phone leads vs web leads over time |
| `GetBusVsPhoneReport()` | Business hours vs after-hours calls |
| `GetLocalVsNationalReport()` | Local site traffic vs national campaigns |
| `GetSpamVsPhoneReport()` | Valid calls vs spam detection |
| `GetDailyPhoneReport()` | Calls per day trend |
| `GetSeasonalLeadReport()` | Monthly lead volume over year |
| `GetManagementVsLocalReport()` | Corporate vs franchisee marketing performance |

---

#### IMarketingLeadsFactory
**Purpose**: Transform between Domain entities and ViewModels.

**Common Transformations**:
```csharp
// API → Domain
MarketingLeadCallDetail CreateDomain(MarketingLeadCallDetailViewModel model);
WebLead CreateDomain(WebLeadViewModel model);

// Domain → ViewModel
CallDetailViewModel CreateViewModel(MarketingLeadCallDetail domain);
WebLeadInfoModel CreateViewModel(WebLead domain);

// External API → Domain
MarketingLeadCallDetailV2 CreateModel(CallDetailV2 record);
MarketingLeadCallDetail CreateModelForInvoca(InvocaCallDetails record, long callTypeId);
```

---

## Troubleshooting

### "No leads are being imported"

**Checklist**:
1. Verify `ISettings.GetWebLeads` and `ISettings.GetCallDetails` are `true`
2. Check API keys are configured correctly
3. Review logs for API connection errors
4. Verify external APIs are reachable (firewall, VPN)
5. Check if scheduled job is actually running

**Debug command**:
```csharp
_logService.Info($"API Key present: {!string.IsNullOrEmpty(_settings.WebLeadsAPIkey)}");
```

---

### "Leads are not assigned to franchisees"

**Causes**:
1. No matching RoutingNumber for the dialed number
2. ZipCode not mapped to any franchisee territory
3. `UpdateConvertedLeadsService` hasn't run yet
4. Franchisee territory configuration is incorrect

**Fix**:
```csharp
// Manually trigger conversion
var service = serviceProvider.GetService<IUpdateConvertedLeadsService>();
service.UpdateLeads();

// Check routing number configuration
var routingService = serviceProvider.GetService<IMarketingLeadsReportService>();
var routingNumbers = routingService.GetRoutingNumberList(new CallDetailFilter(), 1, 100);
// Verify PhoneLabel matches call PhoneLabel exactly
```

---

### "Duplicate leads appearing"

**For Calls**: Check `SessionId` uniqueness. DialogTech may send same call multiple times.

**For Web Leads**: No automatic deduplication. Duplicate submissions are expected if customer submits form multiple times.

**Solution**:
```csharp
// Query for duplicates
var duplicates = _marketingLeadCallDetailRepository.Table
    .GroupBy(x => x.SessionId)
    .Where(g => g.Count() > 1)
    .Select(g => new { SessionId = g.Key, Count = g.Count() });
```

---

### "Conversion tracking not working"

**Check**:
1. `InvoiceId` should populate automatically after `UpdateConvertedLeadsService` runs
2. Verify lead date is within ±2 months of invoice date
3. Check CallerId/Email/Phone matches between lead and `FranchiseeSales` record
4. Case sensitivity may cause match failures

**Debug**:
```csharp
var callDetail = _marketingLeadCallDetailRepository.Get(leadId);
var sales = _franchiseeSalesRepository.Table
    .Where(s => s.Phone == callDetail.CallerId)
    .Where(s => s.InvoiceDate >= callDetail.DateAdded.Value.AddMonths(-2))
    .Where(s => s.InvoiceDate <= callDetail.DateAdded.Value.AddMonths(2));
// Should return matching sale
```

---

### "CSV export is empty"

**Causes**:
1. Filter is too restrictive (no records match)
2. Date range is inverted (EndDate < StartDate)
3. FranchiseeId filter excludes all data

**Test**:
```csharp
// Remove all filters
var filter = new CallDetailFilter
{
    StartDate = DateTime.Now.AddYears(-1),
    EndDate = DateTime.Now
};
// Should return data if any exists in database
```

---

## Performance Tips

### 1. Use Pagination
```csharp
// Bad: Load all records
var all = reportService.GetCallDetailList(filter, 1, int.MaxValue);

// Good: Load page by page
for (int page = 1; page <= totalPages; page++)
{
    var batch = reportService.GetCallDetailList(filter, page, 100);
    ProcessBatch(batch.List);
}
```

### 2. Filter by Date Range
```csharp
// Always specify date range to avoid full table scans
var filter = new MarketingLeadReportFilter
{
    StartDate = DateTime.Now.AddMonths(-1),  // ← Important!
    EndDate = DateTime.Now
};
```

### 3. Use V2 Endpoints When Available
```csharp
// Optimized version with better query performance
var result = reportService.GetCallDetailListV2(filter);
```

### 4. Schedule Heavy Reports Off-Peak
```csharp
// Run large exports during low-traffic hours
if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
{
    reportService.DownloadMarketingLeads(filter, out fileName);
}
```

---

## Best Practices

### 1. Always Check Return Values
```csharp
bool success = reportService.UpdateFranchisee(leadId, franchiseeId);
if (!success)
{
    _logService.Error($"Failed to update franchisee for lead {leadId}");
    // Handle error
}
```

### 2. Use Factory for Transformations
```csharp
// Don't manually map properties
// Bad:
var viewModel = new CallDetailViewModel
{
    Id = domain.Id,
    DateOfCall = domain.DateAdded.Value,
    // ... 50 more properties
};

// Good:
var viewModel = _factory.CreateViewModel(domain);
```

### 3. Wrap Bulk Operations in Transactions
```csharp
_unitOfWork.StartTransaction();
try
{
    foreach (var lead in leads)
    {
        _repository.Save(lead);
    }
    _unitOfWork.SaveChanges();
}
catch (Exception ex)
{
    _unitOfWork.Rollback();
    _logService.Error($"Bulk save failed: {ex.Message}");
}
```

### 4. Handle Nullable FranchiseeId
```csharp
var lead = _repository.Get(id);
if (lead.FranchiseeId.HasValue)
{
    var franchisee = _franchiseeRepository.Get(lead.FranchiseeId.Value);
    // Process assigned lead
}
else
{
    // Lead is unmapped - may need manual assignment
}
```

---

<!-- CUSTOM SECTION: Additional Notes -->
<!-- Add your own troubleshooting tips, team conventions, or deployment notes here -->
<!-- This section will be preserved across regeneration -->

---

## Related Documentation

- **Domain/**: Entity definitions and relationships
- **Impl/**: Service implementation details and API integration
- **ViewModel/**: DTO structures and filtering options
- **Enum/**: Type definitions and lookup values

---

## Support & Contact

For questions about:
- **Lead import issues**: Check logs in `ILogService` for API errors
- **Franchisee assignment**: Review territory configuration in Organizations module
- **Conversion tracking**: Verify Sales module integration
- **Report accuracy**: Run `UpdateMarketingLeadReportDataService` to refresh cache

**Common log search patterns**:
- "Error saving" → Record-level failures
- "Invalid Api Key" → Configuration issues
- "No Data Found" → Empty API responses
- "Duplicate" → Deduplication conflicts
