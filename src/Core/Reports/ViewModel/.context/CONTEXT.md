<!-- AUTO-GENERATED: Header -->
# Reports/ViewModel — Module Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-11T06:35:45Z
**File Count**: 55 view model files
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The ViewModel subfolder contains **Data Transfer Objects (DTOs)** used for API communication between backend services and frontend UI. These classes define the shape of data returned by report services, input filters for queries, and models for Excel exports. View models are **presentation-layer entities** — they format data for display (currency symbols, date formatting, calculated fields) and validate user input before passing to business logic.

### Design Patterns
- **DTO Pattern**: Immutable data containers with no business logic
- **Filter Pattern**: Filter models (e.g., `ServiceReportListFilter`) capture query parameters
- **List Pattern**: List models (e.g., `ServiceReportListModel`) wrap collections with pagination metadata
- **Validation Attributes**: `[NoValidatorRequired]`, `[DownloadField]` attributes control validation and Excel export behavior
- **Nested Models**: Complex reports use hierarchical view models (e.g., `PriceEstimateViewModel` contains `List<PriceEstimateServiceModel>`)

### ViewModel Categories

#### 1. Report View Models (Display Data)
- **ServiceReportViewModel** — Sales report row (franchisee, service, class, total sales)
- **GrowthReportViewModel** — Growth analytics row (YTD sales, YoY comparison, growth %)
- **LateFeeReportViewModel** — Late fee aging row (invoice, overdue amount, days late)
- **CustomerEmailReportViewModel** — Email coverage row (franchisee, customer count, email %)
- **ProductChannelReportViewModel** — Product sales row (product type, revenue)
- **PriceEstimateViewModel** — Price estimate matrix (service, corporate price, franchisee overrides)

#### 2. Filter Models (Query Parameters)
- **ServiceReportListFilter** — Date range, franchisee, service type, class type, sorting
- **GrowthReportFilter** — Year, franchisee, service type, class type
- **LateFeeReportFilter** — Date range, franchisee, minimum overdue amount
- **CustomerEmailReportFilter** — Month, year, franchisee
- **ArReportFilter** — Date range, franchisee, aging buckets
- **PriceEstimateGetModel** — Franchisee, service tag, class type, include disabled

#### 3. List Models (Paginated Results)
- **ServiceReportListModel** — `{ Collection, Filter, PagingModel }`
- **GrowthReportListModel** — `{ Collection, Filter, PagingModel }`
- **LateFeeReportListModel** — `{ Collection, Filter, PagingModel, Total }`
- **CustomerEmailReportListModel** — `{ Collection, Filter, BestFranchisee, Total }`
- **ARReportListModel** — `{ Collection, Filter, AgingBuckets }`

#### 4. Save Models (Write Operations)
- **PriceEstimateSaveModel** — Save franchisee price override
- **PriceEstimateBulkUpdateModel** — Bulk update corporate prices
- **PriceEstimateSaveCorporatePriceModel** — Save corporate base prices
- **PriceEstimateExcelUploadModel** — Excel file upload metadata
- **FloorGrindingAdjustNoteSaveModel** — Save price adjustment notes
- **SeoNotesModel** — Save SEO pricing notes

#### 5. Chart Data Models (Time-Series Data)
- **ChartViewModel** — Generic chart data (labels, datasets)
- **EmailChartDataViewModel** — Email coverage trend data
- **EmailChartDataListModel** — Collection of chart data points
- **ReviewChartDataViewModel** — Review rating trend data

#### 6. Configuration Models
- **MLFSReportConfigurationViewModel** — Multi-location franchise report config
- **FranchiseeGroupModel** — Franchisee grouping configuration
- **FranchiseeSalesInfoList** — Sales info collection for growth reports

#### 7. API Integration Models
- **CustomerEmailAPIRecordRequestModel** — Email API sync request
- **CustomerEmailAPIRecordResponseModel** — Email API sync response
- **EmailApiMergeFieldModel** — Email personalization fields

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Key View Models -->
## Key View Model Details

### ServiceReportViewModel.cs
**Purpose**: Sales report row with franchisee, service type, and sales amount

```csharp
[NoValidatorRequired]
public class ServiceReportViewModel
{
    public long FranchiseeId { get; set; }
    public string Franchisee { get; set; }
    public string MarketingClass { get; set; }
    public string Service { get; set; }
    
    [DownloadField(CurrencyType = "$")]  // Excel formatting
    public decimal TotalSales { get; set; }
    
    public string PrimaryContact { get; set; }
    public string FranchiseeEmail { get; set; }
    public string PhoneNumber { get; set; }
}
```

**Attributes**:
- `[NoValidatorRequired]` — Skip automatic validation (read-only DTO)
- `[DownloadField(CurrencyType = "$")]` — Format as currency in Excel export
- `[DownloadField(Required = false)]` — Exclude from Excel export

### GrowthReportViewModel.cs
**Purpose**: Year-over-year growth analytics with calculated metrics

```csharp
public class GrowthReportViewModel
{
    public string Franchisee { get; set; }
    
    [DownloadField(CurrencyType = "$")]
    public decimal TotalSalesLastYear { get; set; }
    
    [DownloadField(CurrencyType = "$")]
    public decimal YTDSalesLastYear { get; set; }
    
    [DownloadField(CurrencyType = "$")]
    public decimal YTDSalesCurrentYear { get; set; }
    
    [DownloadField(CurrencyType = "$")]
    public decimal AmountDifference { get; set; }  // Current - Last
    
    public decimal PercentageDifference { get; set; }  // (Diff / Last) * 100
    
    [DownloadField(CurrencyType = "$")]
    public decimal AverageGrowth { get; set; }  // Primary sort metric
}
```

**Calculated Fields**:
- `AmountDifference = YTDSalesCurrentYear - YTDSalesLastYear`
- `PercentageDifference = (AmountDifference / YTDSalesLastYear) * 100`
- `AverageGrowth = (YTDSalesCurrentYear / monthsElapsed) - (YTDSalesLastYear / monthsElapsed)`

### PriceEstimateViewModel.cs
**Purpose**: Price estimate matrix with corporate and franchisee pricing

```csharp
public class PriceEstimateViewModel
{
    public string Service { get; set; }
    public string ServiceType { get; set; }
    public string Category { get; set; }  // AREA, LINEARFT, EVENT, TIME
    public string Unit { get; set; }      // "sqft", "linear ft", "hour"
    public long ServiceTagId { get; set; }
    
    // Corporate pricing
    public decimal? FranchiseeCorporatePrice { get; set; }
    public decimal? FranchiseeAdditionalCorporatePrice { get; set; }
    
    // Bulk pricing
    public decimal? BulkCorporatePrice { get; set; }
    public decimal? BulkCorporateAdditionalPrice { get; set; }
    
    // Network metrics
    public decimal? AverageFranchiseePrice { get; set; }
    public decimal? MaximumFranchiseePrice { get; set; }
    public string MaximumFranchiseePriceName { get; set; }
    
    // Franchisee-specific overrides
    public List<PriceEstimateServiceModel> PriceEstimateServices { get; set; }
    
    public bool HasTwoPriceColumns { get; set; }  // Base + Additional
    public bool IsActiveService { get; set; }
}
```

**Nested Model**:
```csharp
public class PriceEstimateServiceModel
{
    public long? FranchiseeId { get; set; }
    public string Franchisee { get; set; }
    public decimal? FranchiseePrice { get; set; }  // Override if != CorporatePrice
    public decimal? FranchiseeAdditionalPrice { get; set; }
}
```

### Filter Models Pattern

**Consistent Structure**:
```csharp
public class ServiceReportListFilter
{
    public long FranchiseeId { get; set; }  // 0 = all
    public long ClassTypeId { get; set; }   // 0 = all
    public long ServiceTypeId { get; set; } // 0 = all
    public DateTime? PaymentDateStart { get; set; }
    public DateTime? PaymentDateEnd { get; set; }
    public string SortColumn { get; set; }   // "TotalSales", "Franchisee"
    public string SortDirection { get; set; } // "asc" or "desc"
}
```

**Usage Pattern**:
- Nullable properties mean "no filter" (e.g., `null` date = no date filter)
- ID = 0 or ID <= 0 means "all" (convention across all filters)
- Sort properties optional (default sort applied if null)

### List Models Pattern

**Consistent Structure**:
```csharp
public class ServiceReportListModel
{
    public List<ServiceReportViewModel> Collection { get; set; }
    public ServiceReportListFilter Filter { get; set; }  // Echo filter back
    public PagingModel PagingModel { get; set; }  // Page, size, total count
}
```

**PagingModel**:
```csharp
public class PagingModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Module Dependencies
- **[../Enum/](../Enum/.context/CONTEXT.md)** — ServiceTagCategory enum for CategoryId validation
- **[../../Application/ViewModel/](../../../Application/ViewModel/.context/CONTEXT.md)** — PagingModel, base view model classes

### External Package Dependencies
- **System.ComponentModel.DataAnnotations** — Validation attributes (`[Required]`, `[Range]`, etc.)
- **Core.Application.Attribute** — Custom attributes (`[NoValidatorRequired]`, `[DownloadField]`)

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### DownloadField Attribute for Excel Export
The `[DownloadField]` attribute controls Excel export behavior:

```csharp
[DownloadField(CurrencyType = "$")]  // Format as $1,234.56
public decimal TotalSales { get; set; }

[DownloadField(Required = false)]  // Exclude from Excel
public long InternalId { get; set; }
```

**Excel Column Generation**:
- Properties without `[DownloadField]` included by default
- `Required = false` excludes property
- `CurrencyType` applies Excel currency formatting
- Column order matches property declaration order

### Filter Model Conventions
All filter models follow consistent patterns:

**ID-based Filtering**:
```csharp
public long FranchiseeId { get; set; }  // 0 or negative = all franchisees
public long ServiceTypeId { get; set; } // 0 = all service types
```

**Date Range Filtering**:
```csharp
public DateTime? StartDate { get; set; }  // Null = no start date filter
public DateTime? EndDate { get; set; }    // Null = no end date filter
```

**Sorting**:
```csharp
public string SortColumn { get; set; }     // Property name to sort by
public string SortDirection { get; set; }  // "asc" or "desc"
```

### Nested vs Flat View Models
**Flat Model** (simple report):
```csharp
public class ServiceReportViewModel
{
    public string Franchisee { get; set; }
    public decimal TotalSales { get; set; }
}
```

**Nested Model** (complex report):
```csharp
public class PriceEstimateViewModel
{
    public string Service { get; set; }
    public List<PriceEstimateServiceModel> PriceEstimateServices { get; set; }
}
```

**Rule**: Use nested models when parent-child relationship exists (one service → many franchisee prices).

### Chart Data Model Pattern
All chart models follow consistent structure for frontend charting libraries:

```csharp
public class ChartViewModel
{
    public List<string> Labels { get; set; }        // X-axis labels
    public List<decimal> Data { get; set; }         // Y-axis values
    public string ChartType { get; set; }           // "line", "bar", "pie"
    public string Label { get; set; }               // Dataset label
    public string BackgroundColor { get; set; }     // CSS color
}
```

**Frontend Usage** (Chart.js):
```javascript
new Chart(ctx, {
    type: viewModel.chartType,
    data: {
        labels: viewModel.labels,
        datasets: [{
            label: viewModel.label,
            data: viewModel.data,
            backgroundColor: viewModel.backgroundColor
        }]
    }
});
```

### NoValidatorRequired Attribute
`[NoValidatorRequired]` skips automatic model validation:

**Use Cases**:
- Read-only DTOs (report view models)
- DTOs with dynamic validation (validated in service layer)
- Legacy models migrating to new validation framework

**Do Not Use** for save models — these need validation.

### Save Model Validation
Save models use standard DataAnnotations:

```csharp
public class PriceEstimateSaveModel
{
    [Required]
    [Range(1, long.MaxValue)]
    public long FranchiseeId { get; set; }
    
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
}
```

### API Integration Models
Email API models map to external service provider APIs:

```csharp
public class CustomerEmailAPIRecordRequestModel
{
    public long CustomerId { get; set; }
    public string EmailAddress { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    // Maps to external API request body
    public object ToExternalApiRequest() => new
    {
        email = EmailAddress,
        first_name = FirstName,
        last_name = LastName
    };
}
```

<!-- END CUSTOM SECTION -->
