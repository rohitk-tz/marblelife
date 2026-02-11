<!-- AUTO-GENERATED: Header -->
# Reports/ViewModel
> Data Transfer Objects (DTOs) for report display, filtering, and Excel export
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The ViewModel subfolder contains **55 DTO classes** that define the contract between backend services and frontend UI. These models serve three primary purposes:

1. **API Response Shape** — Define JSON structure returned by report APIs
2. **Query Parameter Capture** — Filter and sort criteria from UI
3. **Excel Export Formatting** — Control column order, formatting, exclusions

View models are **pure data containers** — no business logic, only properties and validation attributes.

### Model Taxonomy

| Category | Count | Examples |
|----------|-------|----------|
| Report View Models | ~20 | ServiceReportViewModel, GrowthReportViewModel, LateFeeReportViewModel |
| Filter Models | ~15 | ServiceReportListFilter, GrowthReportFilter, ArReportFilter |
| List Models | ~10 | ServiceReportListModel, GrowthReportListModel (wrap collections + paging) |
| Save Models | ~8 | PriceEstimateSaveModel, PriceEstimateBulkUpdateModel |
| Chart Models | ~5 | ChartViewModel, EmailChartDataViewModel |
| Configuration Models | ~4 | MLFSReportConfigurationViewModel, FranchiseeGroupModel |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Creating a Custom Report View Model

```csharp
using Core.Application.Attribute;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]  // Read-only DTO, no validation needed
    public class MyReportViewModel
    {
        // Display properties
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public DateTime ReportDate { get; set; }
        
        // Formatted for display
        public string FormattedDate => ReportDate.ToString("MM/dd/yyyy");
        
        // Excel export control
        [DownloadField(Required = false)]  // Exclude from Excel
        public long InternalId { get; set; }
        
        [DownloadField(CurrencyType = "$")]  // Format as currency
        public decimal TotalRevenue { get; set; }
        
        // Calculated property (not in database)
        public decimal AverageDaily => TotalRevenue / 30;
    }
}
```

### Example 2: Creating a Filter Model

```csharp
namespace Core.Reports.ViewModel
{
    public class MyReportFilter
    {
        // ID = 0 means "all" (convention)
        public long FranchiseeId { get; set; }
        
        // Nullable dates = no filter
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Optional sorting
        public string SortColumn { get; set; }  // "TotalRevenue", "Franchisee"
        public string SortDirection { get; set; }  // "asc" or "desc"
        
        // Enum-based filtering
        public ServiceTagCategory? CategoryFilter { get; set; }
    }
}
```

### Example 3: Creating a List Model

```csharp
using Core.Application.ViewModel;

namespace Core.Reports.ViewModel
{
    public class MyReportListModel
    {
        // Result collection
        public List<MyReportViewModel> Collection { get; set; }
        
        // Echo filter back to UI (for form state)
        public MyReportFilter Filter { get; set; }
        
        // Pagination metadata
        public PagingModel PagingModel { get; set; }
        
        // Optional: Aggregate metrics
        public decimal TotalRevenue { get; set; }
        public int TotalRecords { get; set; }
    }
}
```

### Example 4: Creating a Save Model with Validation

```csharp
using System.ComponentModel.DataAnnotations;

namespace Core.Reports.ViewModel
{
    public class MyReportSaveModel
    {
        [Required(ErrorMessage = "Franchisee is required")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid franchisee")]
        public long FranchiseeId { get; set; }
        
        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be between $0.01 and $999,999.99")]
        public decimal Amount { get; set; }
        
        [Required]
        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }
    }
}
```

### Example 5: Using Chart Data Models

```csharp
namespace Core.Reports.ViewModel
{
    public class MyChartViewModel
    {
        // X-axis labels (dates, categories, etc.)
        public List<string> Labels { get; set; }
        
        // Y-axis values
        public List<decimal> Data { get; set; }
        
        // Chart configuration
        public string ChartType { get; set; }  // "line", "bar", "pie"
        public string Label { get; set; }      // Dataset label
        public string BackgroundColor { get; set; }  // CSS color
    }
}

// Usage in controller
public IActionResult GetChartData(long franchiseeId)
{
    var data = _reportService.GetMonthlyRevenue(franchiseeId);
    
    var chartViewModel = new MyChartViewModel
    {
        Labels = data.Select(x => x.Month.ToString("MMM yyyy")).ToList(),
        Data = data.Select(x => x.Revenue).ToList(),
        ChartType = "line",
        Label = "Monthly Revenue",
        BackgroundColor = "rgba(75, 192, 192, 0.2)"
    };
    
    return Ok(chartViewModel);
}
```

### Example 6: API Integration Model

```csharp
namespace Core.Reports.ViewModel
{
    // Request model (sent to external API)
    public class MyApiRequestModel
    {
        public long CustomerId { get; set; }
        public string EmailAddress { get; set; }
        public Dictionary<string, string> MergeFields { get; set; }
        
        // Transform to external API format
        public object ToExternalFormat() => new
        {
            customer_id = CustomerId.ToString(),
            email = EmailAddress,
            merge_fields = MergeFields
        };
    }
    
    // Response model (received from external API)
    public class MyApiResponseModel
    {
        public string ExternalId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess => Status == "success";
    }
}
```

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Excel Export Missing Columns
**Symptom**: Expected column not appearing in Excel export

**Solution**: Check for `[DownloadField(Required = false)]` attribute
```csharp
[DownloadField(Required = false)]  // This excludes from Excel
public long InternalId { get; set; }
```

**Remove attribute** or set `Required = true` to include column.

### Excel Export Column Order Wrong
**Symptom**: Columns appear in unexpected order

**Cause**: Excel column order matches **property declaration order** in class

**Solution**: Reorder properties in class definition:
```csharp
public class MyViewModel
{
    public string Franchisee { get; set; }  // Column 1
    public decimal Revenue { get; set; }    // Column 2
    public DateTime Date { get; set; }      // Column 3
}
```

### Filter Not Working (Returns All Records)
**Symptom**: Applying filter still returns all records

**Cause**: Service logic treats ID = 0 as "all", but UI sent `null`

**Solution**: Ensure filter model uses correct default:
```csharp
public class MyReportFilter
{
    public long FranchiseeId { get; set; } = 0;  // Default to 0 (all)
}
```

Or in service logic:
```csharp
query = query.Where(x => filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId);
```

### Validation Not Firing on Save Model
**Symptom**: Invalid data accepted by API

**Cause**: Missing `[ApiController]` attribute on controller (auto-validates models)

**Solution**: Add attribute to controller:
```csharp
[ApiController]
[Route("api/[controller]")]
public class MyReportController : ControllerBase
{
    // Model validation now automatic
}
```

### Chart Data Not Rendering
**Symptom**: Frontend chart shows "No data"

**Cause**: Chart model properties don't match expected frontend format

**Solution**: Verify property names match frontend expectations:
```csharp
// Frontend expects: labels, data, type
public class ChartViewModel
{
    public List<string> Labels { get; set; }   // Lowercase 'l' in JSON
    public List<decimal> Data { get; set; }
    public string ChartType { get; set; }
}
```

Use `[JsonProperty("labels")]` if property name differs.

### Nested Model Serialization Fails
**Symptom**: `PriceEstimateViewModel.PriceEstimateServices` returns null

**Cause**: Circular reference in entity navigation properties

**Solution**: Ensure factory creates view models, not returning entities directly:
```csharp
// Bad: Returns entity with circular references
return _priceEstimateRepo.Table.Include(x => x.ServiceTag).ToList();

// Good: Returns view model with no circular references
return _priceEstimateRepo.Table
    .Select(x => new PriceEstimateViewModel
    {
        ServiceTagId = x.ServiceTagId,
        Service = x.ServiceTag.Name  // Flatten navigation
    })
    .ToList();
```

<!-- END CUSTOM SECTION -->
