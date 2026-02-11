<!-- AUTO-GENERATED: Header -->
# Sales View Models (DTOs)
> 53 Data Transfer Objects for API requests, responses, filtering, and UI display
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The ViewModel folder is the "interface" between the Sales module and the outside world (UI, API, external systems). Think of ViewModels as the "contracts" that define what data comes in (CreateModels, EditModels) and what goes out (ViewModels, ListModels).

**Why ViewModels Exist:**
Domain entities (from the Domain folder) are designed for database persistence and business logic. They often have:
- Lazy-loaded navigation properties (causes serialization issues)
- Internal flags and sensitive data (shouldn't be exposed to UI)
- Awkward structures for display (nested collections, circular references)

ViewModels solve this by providing "flat", UI-friendly representations.

**The Four ViewModel Types:**

1. **CreateModels**: New record data from forms
   - `CustomerCreateEditModel`: "I want to add a new hotel customer"
   - `SalesDataUploadCreateModel`: "I'm uploading February sales data"

2. **EditModels**: Existing record modifications
   - `CustomerEditModel`: "Update this customer's phone number"
   - `AccountCreditEditModel`: "Issue a $500 refund"

3. **ViewModels**: Display data for UI
   - `CustomerViewModel`: "Show me customer details with sales totals"
   - `AccountCreditViewModel`: "Display this credit memo"

4. **ListModels + Filters**: Paginated search results
   - `CustomerListModel` + `CustomerListFilter`: "Show page 2 of hotels with sales over $50k"
   - `SalesDataUploadListModel` + `SalesDataListFilter`: "Show all failed uploads"

**Real-World Example:**

When a franchisee views their customer list:
```
1. UI sends: CustomerListFilter { SearchText = "hotel", ClassTypeId = 3 }
   ↓
2. Service queries database, applies filters
   ↓
3. Factory converts: Customer entities → CustomerViewModel objects
   ↓
4. Service returns: CustomerListModel { Collection = [50 customers], PagingModel = {page 1 of 10} }
   ↓
5. UI displays grid with:
   - Name, Email, Phone (from CustomerViewModel)
   - TotalSales: $125,430.00 (aggregate field)
   - MarketingClass: "Hotel" (friendly name, not ID)
```

When they create a new customer:
```
1. UI form data → CustomerCreateEditModel { Name = "Grand Hotel", Emails = [...], Address = {...} }
   ↓
2. Service validates (required fields, email format, etc.)
   ↓
3. Factory converts: CustomerCreateEditModel → Customer entity
   ↓
4. Service saves to database
   ↓
5. Service returns: Customer entity (or CustomerViewModel)
   ↓
6. UI shows success: "Customer Grand Hotel created with ID 12345"
```

**Export Feature Example:**

ViewModels with `[DownloadField]` attributes control CSV/Excel exports:

```csharp
public class CustomerViewModel
{
    public string Name { get; set; }                         // ✅ Export: "Name" column
    
    [DownloadField(CurrencyType = "$")]
    public decimal TotalSales { get; set; }                  // ✅ Export: "$125,430.00"
    
    [DownloadField(Required = false)]
    public bool IsInternal { get; set; }                     // ❌ Excluded from export
    
    [DownloadField(false, true)]
    public AddressViewModel Address { get; set; }            // ✅ Flattened: "Address_City", "Address_State"
}
```

Result CSV:
```
Name,TotalSales,Address_City,Address_State,Address_Zip
Grand Hotel,$125430.00,Chicago,IL,60601
Luxury Spa,$98250.00,Miami,FL,33139
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example: Customer Creation

```csharp
using Core.Sales.ViewModel;
using Core.Geo.ViewModel;

public ActionResult CreateCustomer()
{
    // Build CreateModel from form data
    var model = new CustomerCreateEditModel
    {
        Name = "Luxury Resort & Spa",
        ContactPerson = "Maria Garcia",
        Phone = "555-1234",
        MarketingClassId = (long)MarketingClassType.Hotel,
        ReceiveNotification = true,
        Address = new AddressEditModel
        {
            AddressLine1 = "789 Ocean Drive",
            City = "Miami Beach",
            StateId = 10, // Florida
            Zip = "33139"
        },
        CustomerEmails = new List<CustomerEmail>
        {
            new CustomerEmail { Email = "maria@luxuryresort.com" },
            new CustomerEmail { Email = "info@luxuryresort.com" }
        }
    };
    
    // Validation happens automatically via attributes
    if (ModelState.IsValid)
    {
        // Service handles Factory → Domain → Database
        var customer = _customerService.SaveCustomer(model);
        return RedirectToAction("Details", new { id = customer.Id });
    }
    
    return View(model); // Show validation errors
}
```

### Example: Customer Search with Filters

```csharp
using Core.Sales.ViewModel;

public ActionResult SearchCustomers(string search, long? classTypeId, int page = 1)
{
    // Build filter from query parameters
    var filter = new CustomerListFilter
    {
        SearchText = search,           // Wildcard: %hotel%
        ClassTypeId = classTypeId,     // 3 = Hotel
        MinTotalSales = 10000,         // Customers with $10k+ sales
        StartDate = new DateTime(2024, 1, 1),
        EndDate = new DateTime(2024, 12, 31),
        IncludeSalesMetrics = true     // Load TotalSales, AvgSales, NoOfSales
    };
    
    // Service returns paginated results
    var result = _customerService.GetCustomers(filter, pageNumber: page, pageSize: 50);
    
    // result is CustomerListModel with:
    // - Collection: IEnumerable<CustomerViewModel>
    // - Filter: CustomerListFilter (echo back for UI state)
    // - PagingModel: { PageNumber, PageSize, TotalRecords, TotalPages }
    
    return View(result);
}

// View (Razor):
// @model CustomerListModel
// 
// <table>
//   @foreach (var customer in Model.Collection)
//   {
//     <tr>
//       <td>@customer.Name</td>
//       <td>@customer.Email</td>
//       <td>@customer.PhoneNumber</td>
//       <td>@customer.MarketingClass</td>
//       <td>$@customer.TotalSales?.ToString("N2")</td>
//       <td>@customer.NoOfSales invoices</td>
//     </tr>
//   }
// </table>
// 
// <div>Page @Model.PagingModel.PageNumber of @Model.PagingModel.TotalPages</div>
```

### Example: Sales Data Upload

```csharp
using Core.Sales.ViewModel;

[HttpPost]
public ActionResult UploadSales(HttpPostedFileBase file, long franchiseeId)
{
    var model = new SalesDataUploadCreateModel
    {
        FranchiseeId = franchiseeId,
        PeriodStartDate = new DateTime(2025, 2, 1),  // February 1-28
        PeriodEndDate = new DateTime(2025, 2, 28),
        File = new FileModel
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileContent = ReadFileBytes(file),
            FileSize = file.ContentLength
        },
        CurrencyExchareRateId = 1, // USD rate
        IsAnnualUpload = false
    };
    
    // Validation checks:
    // - Date range matches payment frequency
    // - No overlapping uploads exist
    // - File format is valid
    try
    {
        _uploadService.Save(model);
        return Json(new { 
            success = true, 
            message = "Upload successful. Processing will begin shortly." 
        });
    }
    catch (ValidationException ex)
    {
        return Json(new { success = false, error = ex.Message });
    }
}
```

### Example: Account Credit Issuance

```csharp
using Core.Sales.ViewModel;

public ActionResult IssueCredit(long customerId)
{
    var model = new AccountCreditEditModel
    {
        CustomerId = customerId,
        QbInvoiceNumber = "INV-2025-456",
        CreditedOn = DateTime.Now,
        Items = new List<AccountCreditItemEditModel>
        {
            new AccountCreditItemEditModel
            {
                Description = "Refund - Marble polish unsatisfactory",
                Amount = 350.00m,
                Quantity = 1
            },
            new AccountCreditItemEditModel
            {
                Description = "Service recovery goodwill credit",
                Amount = 100.00m,
                Quantity = 1
            }
        }
    };
    
    long currencyRateId = 1;
    var credit = _accountCreditService.Save(model, currencyRateId);
    
    // Return ViewModel for display
    var viewModel = new AccountCreditViewModel
    {
        CreditId = credit.Id,
        CustomerName = credit.Customer.Name,
        TotalAmount = credit.CreditMemoItems.Sum(item => item.Amount),
        CreditedOn = credit.CreditedOn,
        QbInvoiceNumber = credit.QbInvoiceNumber
    };
    
    return Json(viewModel);
}
```

### Example: Annual Audit Review

```csharp
using Core.Sales.ViewModel;

public ActionResult AuditQueue()
{
    var filter = new SalesDataListFilter
    {
        AuditStatus = (long)AuditActionType.Pending,
        Year = 2024
    };
    
    // Get annual uploads requiring staff review
    var audits = _annualService.GetAnnualAuditRecord(filter);
    
    // audits is AnnualAuditSalesListModel with:
    // - List: IEnumerable<AnnualAuditSalesViewModel>
    
    return View(audits);
}

// View displays flagged records:
// @model AnnualAuditSalesListModel
// 
// @foreach (var audit in Model.List)
// {
//   <div class="audit-record">
//     <h3>@audit.FranchiseeName - $@audit.AnnualRoyality.ToString("N2")</h3>
//     <p>Parsed: @audit.NoOfParsedRecords</p>
//     <p>Failed: @audit.NoOfFailedRecords</p>
//     <p class="alert">⚠️ Mismatches: @audit.NoOfMismatchedRecords</p>
//     
//     @if (audit.Customers.Any())
//     {
//       <table>
//         <tr><th>Customer</th><th>Issue</th></tr>
//         @foreach (var customer in audit.Customers)
//         {
//           <tr>
//             <td>@customer.Name</td>
//             <td>Address mismatch</td>
//           </tr>
//         }
//       </table>
//     }
//     
//     <button onclick="approve(@audit.UploadId)">Approve</button>
//     <button onclick="reject(@audit.UploadId)">Reject</button>
//   </div>
// }
```

### Example: Export to CSV/Excel

```csharp
using Core.Sales.ViewModel;

public ActionResult ExportCustomers()
{
    var filter = new CustomerListFilter
    {
        ClassTypeId = (long)MarketingClassType.Hotel,
        MinTotalSales = 50000
    };
    
    string fileName;
    bool success = _customerService.DownloadCustomerFile(filter, out fileName);
    
    if (success)
    {
        // fileName = "Customers_2025-02-10.xlsx"
        // File generated using [DownloadField] attributes on CustomerViewModel:
        // - Name, Email, Phone: included
        // - TotalSales: formatted as $XXX.XX
        // - Address: flattened to Address_City, Address_State, Address_Zip
        // - IsInternal: excluded (Required = false)
        
        return File(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    Path.GetFileName(fileName));
    }
    
    return HttpNotFound();
}
```

### Example: Estimate Invoice Creation

```csharp
using Core.Sales.ViewModel;

public ActionResult CreateEstimate(long customerId)
{
    var model = new EstimateInvoiceEditModel
    {
        CustomerId = customerId,
        FranchiseeId = currentFranchiseeId,
        ClassTypeId = (long)MarketingClassType.Residential,
        PriceOfService = 1250.00f,
        LessDeposit = 250.00f,
        Notes = "Master bathroom marble restoration",
        Option1 = "Standard polish - $950",
        Option2 = "Premium seal + polish - $1,250",
        Option3 = "Full restoration with repair - $1,850",
        Services = new List<EstimateServiceDetailEditModel>
        {
            new EstimateServiceDetailEditModel
            {
                ServiceName = "Marble Floor Polish",
                TypeOfService = "Polish",
                Location = "Master Bathroom",
                StoneType = "Carrara Marble",
                Price = "850.00",
                Description = "Hone and polish 120 sq ft"
            },
            new EstimateServiceDetailEditModel
            {
                ServiceName = "Grout Sealing",
                TypeOfService = "Seal",
                Location = "Master Bathroom",
                StoneType = "Ceramic Tile",
                Price = "150.00",
                Description = "Seal grout lines"
            }
        }
    };
    
    var estimate = _estimateService.Save(model);
    return RedirectToAction("ViewEstimate", new { id = estimate.Id });
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## ViewModel Summary

### Customer ViewModels
| ViewModel | Purpose | Used In |
|-----------|---------|---------|
| `CustomerCreateEditModel` | Create/update customer | POST /customers |
| `CustomerEditModel` | Edit existing customer | PUT /customers/{id} |
| `CustomerViewModel` | Display customer details | GET /customers/{id}, list views |
| `CustomerListModel` | Paginated customer list | GET /customers?page=1 |
| `CustomerListFilter` | Customer search criteria | Query parameters |
| `CustomerFileUploadCreateModel` | Customer file upload | POST /customers/upload |

### Sales Upload ViewModels
| ViewModel | Purpose | Used In |
|-----------|---------|---------|
| `SalesDataUploadCreateModel` | Upload sales data | POST /sales/upload |
| `SalesDataUploadListModel` | List upload batches | GET /sales/uploads |
| `SalesDataListFilter` | Upload search filters | Query parameters |
| `AnnualDataUploadCreateModel` | Annual report upload | POST /sales/annual-upload |
| `AnnualUploadValidationModel` | Annual upload validation | Validation response |

### Account Credit ViewModels
| ViewModel | Purpose | Used In |
|-----------|---------|---------|
| `AccountCreditEditModel` | Create credit memo | POST /credits |
| `AccountCreditViewModel` | Display credit memo | GET /credits/{id} |
| `AccountCreditListModel` | List credit memos | GET /credits |
| `FranchiseeAccountCreditViewModel` | Franchisee credits | GET /franchisees/{id}/credits |

### Annual Audit ViewModels
| ViewModel | Purpose | Used In |
|-----------|---------|---------|
| `AnnualAuditSalesViewModel` | Audit record details | Audit queue display |
| `AnnualAuditSalesListModel` | List audit records | GET /sales/annual-audit |
| `AuditInvoiceViewModel` | Invoice audit details | Audit drill-down |

### Estimate Invoice ViewModels
| ViewModel | Purpose | Used In |
|-----------|---------|---------|
| `EstimateInvoiceEditModel` | Create/edit estimate | POST/PUT /estimates |
| `EstimateInvoiceViewModel` | Display estimate | GET /estimates/{id} |
| `EstimateInvoiceEditMailModel` | Email estimate | POST /estimates/{id}/send |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem: ModelState.IsValid returns false with no error messages**
- **Cause**: Validation attribute on ViewModel property but no error message defined
- **Solution**: Add `ErrorMessage` to attributes: `[Required(ErrorMessage = "Name is required")]`

**Problem: Circular reference exception during JSON serialization**
- **Cause**: ViewModel contains navigation properties that reference each other
- **Solution**: Use flattened ViewModels without navigation properties, or configure JSON serializer to handle references

**Problem: Export missing columns**
- **Cause**: Properties marked with `[DownloadField(Required = false)]`
- **Solution**: Remove attribute or set `Required = true`

**Problem: Date/time values showing in UTC instead of local time**
- **Cause**: DateTime properties not converted to local timezone
- **Solution**: Convert in factory: `DateCreated = customer.DateCreated?.ToLocalTime()`

**Problem: Pagination showing wrong total pages**
- **Cause**: `TotalRecords` not set correctly in `PagingModel`
- **Solution**: Ensure service queries total count: `var total = query.Count(); pagingModel.TotalRecords = total;`

### Best Practices

**ViewModel Design**:
- Keep ViewModels flat - avoid deep nesting
- Use primitive types where possible (string, int, decimal) over complex objects
- Include only properties needed for the specific UI view
- Use `[DownloadField]` to control export columns

**Validation**:
- Always use `ModelState.IsValid` before processing CreateModels/EditModels
- Provide clear error messages in validation attributes
- Use custom validators for complex business rules

**Performance**:
- Don't lazy-load navigation properties in ViewModels (causes N+1 queries)
- Use projections: `Select(c => new CustomerViewModel { ... })` instead of mapping entire entities
- Cache static lookup data (marketing classes, states) used in filters

**Security**:
- Never expose sensitive data in ViewModels (passwords, API keys, internal flags)
- Validate all input CreateModels/EditModels server-side (never trust client)
- Use DTOs to control exactly what data is sent to clients
<!-- END CUSTOM SECTION -->
