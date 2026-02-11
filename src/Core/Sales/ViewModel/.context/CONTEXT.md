<!-- AUTO-GENERATED: Header -->
# Sales/ViewModel — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2025-02-10T15:50:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The ViewModel folder contains 53 Data Transfer Object (DTO) classes that facilitate communication between the UI layer and the Sales module's business logic. These classes define the shape of data flowing in (CreateModels, EditModels) and out (ViewModels, ListModels) of the system, providing validation, filtering, and display formatting capabilities.

### Design Patterns
- **DTO Pattern**: ViewModels act as data carriers between layers, decoupling UI from domain model
- **Command/Query Separation**: 
  - **Command Models**: `*CreateModel`, `*EditModel` for mutations
  - **Query Models**: `*ViewModel`, `*ListModel` for data retrieval
- **Filter Objects**: `*ListFilter` classes encapsulate search/filter criteria
- **List/Collection Pattern**: `*ListModel` wraps collection with pagination and filter metadata
- **Validation Attributes**: Models use `[Required]`, `[StringLength]`, etc. for input validation
- **Display Attributes**: `[DownloadField]` controls CSV/Excel export formatting

### ViewModel Categories

#### **Customer ViewModels**
- `CustomerCreateEditModel`: Create/update customer with address and emails
- `CustomerEditModel`: Edit existing customer
- `CustomerViewModel`: Display customer with sales aggregates
- `CustomerListModel`: Paginated customer list with filtering
- `CustomerListFilter`: Customer search/filter criteria
- `CustomerFileUploadCreateModel`: Customer-only file upload

#### **Sales Upload ViewModels**
- `SalesDataUploadCreateModel`: Create sales data upload
- `SalesDataUploadListModel`: List of upload batches
- `SalesDataListFilter`: Upload search/filter criteria
- `AnnualDataUploadCreateModel`: Annual report upload
- `AnnualSalesDataListFiltercs`: Annual data filtering
- `AnnualUploadValidationModel`: Annual upload validation info

#### **Account Credit ViewModels**
- `AccountCreditEditModel`: Create/edit credit memo
- `AccountCreditItemEditModel`: Credit line item
- `AccountCreditViewModel`: Display credit memo
- `AccountCreditItemViewModel`: Display credit line item
- `AccountCreditListModel`: List of credits
- `AccountCreditListFilter`: Credit search criteria
- `FranchiseeAccountCreditViewModel`: Franchisee-specific credit view
- `FranchiseeAccountCreditListModel`: Franchisee credit list

#### **Annual Audit ViewModels**
- `AnnualAuditSalesViewModel`: Annual data requiring review
- `AnnualAuditSalesListModel`: List of audit records
- `AuditInvoiceViewModel`: Invoice details for audit
- `AnnualSalesDataCustomerViewModel`: Customer data in annual report
- `AnnualSalesDataCustonerListModel`: Annual customer list
- `AnnualGroupedReport`: Grouped annual report data

#### **Estimate/Invoice ViewModels**
- `EstimateInvoiceEditModel`: Create/edit estimate
- `EstimateInvoiceViewModel`: Display estimate
- `EstimateInvoiceEditMailModel`: Email estimate to customer
- `EstimateServiceDetailEditModel`: Estimate service line
- `InvoiceInfoEditModel`: Invoice information
- `DownloadAllInvoiceModel`: Bulk invoice download

#### **Royalty Reporting**
- `RoyaltyReportListModel`: Royalty report data
- `RoyaltyReportViewModel`: Royalty details
- `SalesFunnelReportViewModel`: Sales funnel metrics
- `SalesFunnelReportFilterModel`: Funnel report filters

#### **Supporting ViewModels**
- `EmailEditModel`: Email address editing
- `CustomersignatureViewModel`: Customer signature capture
- `AnnualDownloadFilter`: Annual data export criteria
- `FeedbackMessageModel`: User feedback/messages
- `FileModel`: File upload metadata
- `PagingModel`: Pagination metadata

### Data Flow Patterns

#### **Create/Update Flow**
```
UI Form → *CreateModel/*EditModel → Service → Factory → Domain Entity → Database
```

#### **Query/Display Flow**
```
Database → Domain Entity → Factory → *ViewModel → UI Display
```

#### **List/Search Flow**
```
UI Filters → *ListFilter → Service.Get(*Filter, page, pageSize) → *ListModel → UI Grid
```

#### **File Upload Flow**
```
UI Upload → *UploadCreateModel (with FileModel) → Service → Parser → Domain Entities
```

### Key Features

**Validation Attributes**:
- `[Required]`, `[StringLength]`, `[Range]` for input validation
- `[EmailAddress]`, `[Phone]` for format validation
- Custom validators via `IValidator<T>` implementations

**Export Attributes**:
- `[DownloadField]` controls CSV/Excel column inclusion
- `[DownloadField(Required = false)]` excludes from export
- `[DownloadField(CurrencyType = "$")]` formats as currency
- `[DownloadField(false, true)]` nested object flattening

**Pagination Support**:
- `PagingModel` provides page number, page size, total count
- Supports SQL Server efficient pagination with ROW_NUMBER()

**Filtering Support**:
- Search text (wildcard matching)
- Date range filtering
- Status filtering (enums)
- Marketing class filtering
- Franchisee filtering
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Key ViewModel Definitions

### Customer ViewModels

#### CustomerCreateEditModel
```csharp
public class CustomerCreateEditModel : EditModelBase
{
    public long Id { get; set; }
    public string Name { get; set; }                         // Business/person name
    public string ContactPerson { get; set; }                // Primary contact
    public ICollection<CustomerEmail> CustomerEmails { get; set; } // Multiple emails
    public string Phone { get; set; }
    public long? MarketingClassId { get; set; }              // Classification
    public long? SubMarketingClassId { get; set; }           // Sub-classification
    public AddressEditModel Address { get; set; }            // Cascaded address
    public DateTime? DateCreated { get; set; }
    public bool ReceiveNotification { get; set; }            // Marketing opt-in
    public decimal? TotalSales { get; set; }                 // Aggregate (read-only)
    public int? NoOfSales { get; set; }                      // Aggregate (read-only)
    public decimal? AvgSales { get; set; }                   // Aggregate (read-only)
    public int Status { get; set; }                          // Customer status
    public long? LastInvoiceId { get; set; }
    public string QbInvoiceId { get; set; }                  // QuickBooks reference
}
```

**Usage**: Create or update customer via `ICustomerService.SaveCustomer()`.

**Validation**:
- Name required
- At least one email required
- Phone format validation
- MarketingClassId must be valid enum value

---

#### CustomerViewModel
```csharp
public class CustomerViewModel
{
    public long CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }                        // Primary email
    [DownloadField(Required = false)]
    public IList<string> Emails { get; set; }                // All emails (export)
    [DownloadField(false, true)]
    public AddressViewModel Address { get; set; }            // Flattened in export
    public string ContactPerson { get; set; }
    public string PhoneNumber { get; set; }
    [DownloadField(CurrencyType = "$")]
    public decimal Amount { get; set; }                      // Last invoice amount
    public string MarketingClass { get; set; }               // Class name
    [DownloadField(CurrencyType = "$")]
    public decimal? TotalSales { get; set; }                 // Lifetime sales
    [DownloadField(Required = false)]
    public int? NoOfSales { get; set; }                      // Invoice count
    [DownloadField(CurrencyType = "$")]
    public decimal? AvgSales { get; set; }                   // Average invoice
    public DateTime? DateCreated { get; set; }
    public DateTime? LastServicedDate { get; set; }
    public bool IsSynced { get; set; }                       // QuickBooks sync status
}
```

**Usage**: Display customer in lists and detail views. Export to CSV/Excel with `[DownloadField]` formatting.

---

#### CustomerListModel
```csharp
public class CustomerListModel
{
    public IEnumerable<CustomerViewModel> Collection { get; set; } // Paginated results
    public CustomerListFilter Filter { get; set; }                 // Applied filters
    public PagingModel PagingModel { get; set; }                   // Pagination metadata
}
```

**Usage**: Returned by `ICustomerService.GetCustomers()` for paginated lists.

---

#### CustomerListFilter
```csharp
public class CustomerListFilter
{
    public string SearchText { get; set; }                   // Name/email wildcard search
    public long? ClassTypeId { get; set; }                   // Marketing class filter
    public DateTime? StartDate { get; set; }                 // Date created range
    public DateTime? EndDate { get; set; }
    public decimal? MinTotalSales { get; set; }              // Sales threshold
    public long? FranchiseeId { get; set; }                  // Franchisee filter
    public bool IncludeSalesMetrics { get; set; }            // Load aggregates
}
```

**Usage**: Pass to `GetCustomers()` to filter results.

---

### Sales Upload ViewModels

#### SalesDataUploadCreateModel
```csharp
public class SalesDataUploadCreateModel : EditModelBase
{
    public long Id { get; set; }
    public long FranchiseeId { get; set; }
    public long FileId { get; set; }
    public DateTime PeriodStartDate { get; set; }            // Upload period start
    public DateTime PeriodEndDate { get; set; }              // Upload period end
    public long StatusId { get; set; }                       // SalesDataUploadStatus
    public decimal? AccruedAmount { get; set; }
    public FileModel File { get; set; }                      // Uploaded file
    public FileModel AnnualFile { get; set; }                // Annual file (if applicable)
    public bool IsUpdate { get; set; }                       // Update vs. new upload
    public bool IsAnnualUpload { get; set; }                 // Annual report flag
    public long AuditActionId { get; set; }                  // Audit status (annual only)
    public long CurrencyExchareRateId { get; set; }          // Currency rate
    public bool IsInvoiceGenerated { get; set; }             // Royalty invoices created
    public string Year { get; set; }                         // Year (annual uploads)
}
```

**Usage**: Submit sales data upload via `ISalesDataUploadService.Save()`.

**Validation**:
- FranchiseeId required
- PeriodStartDate and PeriodEndDate must match payment frequency
- File required
- No overlapping date ranges for franchisee

---

#### SalesDataUploadListModel / SalesDataListFilter
```csharp
public class SalesDataUploadListModel
{
    public IEnumerable<SalesDataUploadViewModel> List { get; set; }
    public PagingModel PagingModel { get; set; }
    public SalesDataListFilter Filter { get; set; }
}

public class SalesDataListFilter
{
    public long? FranchiseeId { get; set; }
    public long? StatusId { get; set; }                      // Filter by upload status
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IncludeMetrics { get; set; }                 // Load parsed metrics
    public int Year { get; set; }                            // Year filter (annual)
}
```

**Usage**: Retrieve upload batches with `GetBatchList()`.

---

### Account Credit ViewModels

#### AccountCreditEditModel
```csharp
public class AccountCreditEditModel
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string QbInvoiceNumber { get; set; }              // QuickBooks reference
    public DateTime CreditedOn { get; set; }
    public List<AccountCreditItemEditModel> Items { get; set; } // Line items
}
```

**Usage**: Create credit memo via `IAccountCreditService.Save()`.

---

#### AccountCreditItemEditModel
```csharp
public class AccountCreditItemEditModel
{
    public string Description { get; set; }                  // Line item description
    public decimal Amount { get; set; }                      // Credit amount
    public int Quantity { get; set; }                        // Quantity (typically 1)
}
```

**Usage**: Line item within `AccountCreditEditModel`.

---

#### FranchiseeAccountCreditViewModel
```csharp
public class FranchiseeAccountCreditViewModel
{
    public long FranchiseeId { get; set; }
    public decimal TotalCreditAvailable { get; set; }        // Sum of unused credits
    public List<AccountCreditViewModel> Credits { get; set; } // Individual credits
}
```

**Usage**: Display available credits for franchisee/invoice via `GetCreditForInvoice()`.

---

### Annual Audit ViewModels

#### AnnualAuditSalesViewModel
```csharp
public class AnnualAuditSalesViewModel
{
    public long UploadId { get; set; }
    public long FranchiseeId { get; set; }
    public string FranchiseeName { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public int NoOfParsedRecords { get; set; }
    public int NoOfFailedRecords { get; set; }
    public int NoOfMismatchedRecords { get; set; }           // Address mismatches
    public decimal WeeklyRoyality { get; set; }
    public decimal AnnualRoyality { get; set; }
    public long AuditActionId { get; set; }                  // Pending/Approved/Rejected
    public List<AnnualSalesDataCustomerViewModel> Customers { get; set; } // Flagged customers
}
```

**Usage**: Display annual uploads requiring review via `GetAnnualAuditRecord()`.

**Workflow**:
1. Staff reviews `NoOfMismatchedRecords`
2. Inspects `Customers` list for address issues
3. Corrects via `UpdateCustomerAddress()`
4. Approves/rejects via `ManageBatch()`

---

### Estimate Invoice ViewModels

#### EstimateInvoiceEditModel
```csharp
public class EstimateInvoiceEditModel
{
    public long Id { get; set; }
    public long? CustomerId { get; set; }
    public long FranchiseeId { get; set; }
    public float PriceOfService { get; set; }
    public float LessDeposit { get; set; }
    public long ClassTypeId { get; set; }                    // Marketing class
    public string Notes { get; set; }
    public string Option1 { get; set; }                      // Pricing option 1
    public string Option2 { get; set; }                      // Pricing option 2
    public string Option3 { get; set; }                      // Pricing option 3
    public List<EstimateServiceDetailEditModel> Services { get; set; } // Line items
}
```

**Usage**: Create or update estimate invoice.

---

### Supporting ViewModels

#### FileModel
```csharp
public class FileModel
{
    public long Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] FileContent { get; set; }                  // File bytes
    public long FileSize { get; set; }
}
```

**Usage**: Encapsulates uploaded files in create models.

---

#### PagingModel
```csharp
public class PagingModel
{
    public int PageNumber { get; set; }                      // Current page (1-based)
    public int PageSize { get; set; }                        // Records per page
    public int TotalRecords { get; set; }                    // Total count
    public int TotalPages => (TotalRecords + PageSize - 1) / PageSize;
}
```

**Usage**: Pagination metadata in list models.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application.ViewModel](../../../Application/.context/CONTEXT.md)** — `EditModelBase`, `PagingModel`, validation attributes
- **[Core.Geo.ViewModel](../../../Geo/.context/CONTEXT.md)** — `AddressEditModel`, `AddressViewModel`
- **Sales.Domain** (sibling folder) — `Customer`, `CustomerEmail`, `MarketingClass` entities

### External Dependencies
- **System.ComponentModel.DataAnnotations** — `[Required]`, `[StringLength]`, `[EmailAddress]` attributes
- **Custom Attributes** — `[DownloadField]` for CSV/Excel export formatting
- **Web.Mvc** (implied) — `HttpPostedFileBase` for file uploads
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Naming Conventions
- **CreateModel**: Used for inserting new records (Id typically 0)
- **EditModel**: Used for updating existing records (Id > 0)
- **ViewModel**: Read-only display model for UI consumption
- **ListModel**: Wraps collection with pagination and filter metadata
- **ListFilter**: Encapsulates search/filter criteria

### Validation Patterns

**Standard Validation**:
```csharp
[Required(ErrorMessage = "Customer name is required")]
[StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
public string Name { get; set; }

[EmailAddress(ErrorMessage = "Invalid email format")]
public string Email { get; set; }

[Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1,000,000")]
public decimal Amount { get; set; }
```

**Custom Validators**: Implement `IValidator<TModel>` for complex rules (e.g., `ISalesDataUploadCreateModelValidator`).

### Export Formatting

**CSV/Excel Export Control**:
```csharp
[DownloadField]                              // Include in export (default)
public string Name { get; set; }

[DownloadField(Required = false)]            // Exclude from export
public bool IsInternal { get; set; }

[DownloadField(CurrencyType = "$")]          // Format as currency: $1,234.56
public decimal TotalSales { get; set; }

[DownloadField(false, true)]                 // Flatten nested object
public AddressViewModel Address { get; set; } // Becomes: Address_City, Address_State, etc.
```

### Pagination Best Practices

**Efficient Pagination**:
```csharp
// Controller
var filter = new CustomerListFilter { SearchText = searchText };
var result = _customerService.GetCustomers(filter, pageNumber: 1, pageSize: 50);

// Service uses SQL Server ROW_NUMBER() for efficient paging:
// SELECT * FROM (
//   SELECT *, ROW_NUMBER() OVER (ORDER BY Name) AS RowNum FROM Customers
// ) WHERE RowNum BETWEEN 1 AND 50
```

**Display Pagination**:
```csharp
// View
Total: @Model.PagingModel.TotalRecords customers
Page @Model.PagingModel.PageNumber of @Model.PagingModel.TotalPages
```

### Common Patterns

**List with Filters**:
```csharp
var filter = new CustomerListFilter
{
    SearchText = "hotel",
    ClassTypeId = (long)MarketingClassType.Hotel,
    MinTotalSales = 50000
};
var customers = _customerService.GetCustomers(filter, 1, 50);
```

**Create with Cascade**:
```csharp
var model = new CustomerCreateEditModel
{
    Name = "Grand Hotel",
    Address = new AddressEditModel
    {
        AddressLine1 = "123 Main St",
        City = "Chicago",
        StateId = 14,
        Zip = "60601"
    },
    CustomerEmails = new List<CustomerEmail>
    {
        new CustomerEmail { Email = "info@grandhotel.com" }
    }
};
var customer = _customerService.SaveCustomer(model);
```

### Anti-Patterns to Avoid

❌ **Modifying ViewModels directly from UI**:
```csharp
// Don't do this:
var customer = _customerService.GetCustomerViewModel(id);
customer.TotalSales = 99999; // ViewModels should be read-only
```

✅ **Use EditModels for mutations**:
```csharp
var editModel = _customerService.Get(id); // Returns CustomerEditModel
editModel.Name = "Updated Name";
_customerService.Save(editModel);
```

❌ **Exposing sensitive data in ViewModels**:
```csharp
// Don't include passwords, API keys, internal flags in ViewModels
```

✅ **Filter sensitive data in factories**:
```csharp
public CustomerViewModel CreateViewModel(Customer customer)
{
    return new CustomerViewModel
    {
        Name = customer.Name,
        // Don't include customer.InternalNotes, customer.CreditScore, etc.
    };
}
```

### Performance Tips

- **Projection**: Use `Select()` to project only needed fields instead of loading full ViewModels
- **AsNoTracking**: For read-only ViewModels, use EF's `.AsNoTracking()` to avoid change tracking overhead
- **Lazy Loading**: Be cautious of N+1 queries when accessing navigation properties in ViewModels
- **Caching**: Cache static lookup data (marketing classes, states) used in filters
<!-- END CUSTOM SECTION -->
