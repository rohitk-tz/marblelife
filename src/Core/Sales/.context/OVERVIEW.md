<!-- AUTO-GENERATED: Header -->
# Sales Module
> Comprehensive customer management, sales tracking, and royalty calculation system for franchisee networks
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Sales module is the financial heartbeat of the MarbleLife franchisee management system. Think of it as the combination of a CRM (customer relationship management), accounting ledger, and data pipeline all in one.

**What It Does:**
- **Customer Database**: Maintains the master customer list with contact info, addresses, and business classifications (residential vs. commercial vs. hotel, etc.)
- **Sales Data Ingestion**: Franchisees upload CSV/Excel files of their sales; the system parses them to extract invoices and customer data
- **Royalty Tracking**: Calculates royalties owed based on sales data, considering payment frequencies and date ranges
- **Credit Management**: Handles refunds and adjustments through account credit memos
- **Marketing Intelligence**: Classifies customers into 18 marketing segments to drive targeted campaigns and revenue analysis

**Why It Exists:**
Instead of franchisees manually entering every invoice, they upload a batch file (weekly or monthly). The system automatically:
1. Parses the file to extract customer and sales data
2. Creates or updates customer records
3. Tracks sales metrics per customer (total sales, average sale, number of transactions)
4. Calculates royalty obligations
5. Generates reports for HQ and franchisees

This automation saves hundreds of hours monthly and ensures consistent, accurate royalty calculations across the network.

**Real-World Analogy:**
Imagine a franchise network like McDonald's, but for stone restoration services. Each franchisee runs jobs (cleaning marble floors, countertops, etc.) and tracks sales in QuickBooks or similar software. At the end of each week/month, they export a file and upload it. The system then:
- Identifies customers (some existing, some new)
- Categorizes them (is this a hotel contract? A residential homeowner?)
- Tallies sales figures
- Calculates the royalty payment due to corporate

The Marketing Class system is like tagging customers as "retail" vs. "wholesale" in traditional retail—it drives pricing, reporting, and strategic decisions.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup

This module depends on:
- Entity Framework with SQL Server database
- Core.Application, Core.Organizations, Core.Billing, Core.Geo modules
- File upload infrastructure (ASP.NET MVC or similar)

```bash
# Install dependencies via NuGet (in Visual Studio Package Manager Console)
Install-Package EntityFramework
```

### Example: Creating a Customer

```csharp
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Geo.ViewModel;

public class CustomerController
{
    private readonly ICustomerService _customerService;
    
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    public ActionResult Create()
    {
        var model = new CustomerCreateEditModel
        {
            Name = "Grand Hotel Downtown",
            ContactPerson = "Sarah Johnson",
            Emails = new List<string> { "sarah@grandhotel.com", "maintenance@grandhotel.com" },
            Phone = "555-0123",
            ClassTypeId = (long)MarketingClassType.Hotel, // Marketing classification
            ReceiveNotification = true,
            Address = new AddressEditModel
            {
                AddressLine1 = "123 Main Street",
                City = "Chicago",
                StateId = 14, // Illinois
                Zip = "60601"
            }
        };
        
        // Validates email uniqueness, creates customer with cascaded address and emails
        Customer customer = _customerService.SaveCustomer(model);
        
        return RedirectToAction("Details", new { id = customer.Id });
    }
}
```

### Example: Uploading Sales Data

```csharp
using Core.Sales;
using Core.Sales.ViewModel;

public class SalesUploadController
{
    private readonly ISalesDataUploadService _uploadService;
    
    [HttpPost]
    public ActionResult UploadSalesFile(HttpPostedFileBase file, long franchiseeId)
    {
        var model = new SalesDataUploadCreateModel
        {
            FranchiseeId = franchiseeId,
            PeriodStartDate = new DateTime(2025, 1, 1),  // January 1-31
            PeriodEndDate = new DateTime(2025, 1, 31),
            File = file,
            PaymentFrequencyId = 2 // Monthly
        };
        
        // Validation checks:
        // 1. Date range = full month (Jan 1-31)
        // 2. No overlapping uploads exist for this franchisee
        // 3. Upload not expired (within allowable time window)
        if (!_uploadService.DoesOverlappingDatesExist(franchiseeId, model.PeriodStartDate, model.PeriodEndDate))
        {
            _uploadService.Save(model); // Queues for background parsing
            return Json(new { success = true, message = "Upload successful, processing..." });
        }
        
        return Json(new { success = false, message = "Overlapping upload exists for this period" });
    }
}
```

### Example: Retrieving Customer Sales History

```csharp
public ActionResult CustomerReport(string email)
{
    // Check if customer exists
    if (_customerService.DoesCustomerExists(email))
    {
        // Get customer list (will return single result for exact email match)
        var filter = new CustomerListFilter
        {
            SearchText = email,
            IncludeSalesMetrics = true
        };
        
        var result = _customerService.GetCustomers(filter, pageNumber: 1, pageSize: 1);
        var customer = result.Customers.FirstOrDefault();
        
        // Display aggregated sales data
        ViewBag.TotalSales = customer.TotalSales;      // e.g., $45,230.00
        ViewBag.AverageSale = customer.AvgSales;       // e.g., $1,507.67
        ViewBag.NumberOfJobs = customer.NoOfSales;     // e.g., 30
        ViewBag.MarketingClass = customer.MarketingClass; // e.g., "Hotel"
        
        return View(customer);
    }
    
    return HttpNotFound();
}
```

### Example: Creating an Account Credit

```csharp
using Core.Sales;
using Core.Sales.ViewModel;

public ActionResult IssueRefund(long customerId)
{
    var model = new AccountCreditEditModel
    {
        CustomerId = customerId,
        QbInvoiceNumber = "INV-2025-001",
        CreditedOn = DateTime.Now,
        Items = new List<AccountCreditItemEditModel>
        {
            new AccountCreditItemEditModel
            {
                Description = "Refund for incomplete marble polish job",
                Amount = 350.00m,
                Quantity = 1
            },
            new AccountCreditItemEditModel
            {
                Description = "Service recovery credit",
                Amount = 50.00m,
                Quantity = 1
            }
        }
    };
    
    long currencyRateId = 1; // USD at current rate
    var credit = _accountCreditService.Save(model, currencyRateId);
    
    // Credit will be automatically applied to future invoices
    return RedirectToAction("CustomerDetails", new { id = customerId });
}
```

### Example: Annual Sales Report Processing

```csharp
using Core.Sales;
using Core.Sales.ViewModel;

public class AnnualReportController
{
    private readonly IAnnualSalesDataUploadService _annualService;
    
    [HttpPost]
    public ActionResult UploadAnnualReport(HttpPostedFileBase file, long franchiseeId, int year)
    {
        var model = new AnnualDataUploadCreateModel
        {
            FranchiseeId = franchiseeId,
            Year = year,
            File = file
        };
        
        if (_annualService.isValidUpload(model))
        {
            _annualService.SaveUpload(model); // Triggers specialized parsing and audit record creation
            return RedirectToAction("AuditQueue");
        }
        
        return View("UploadError");
    }
    
    // Staff reviews flagged records (e.g., address mismatches)
    public ActionResult ReviewAudit(long batchId, bool approve)
    {
        bool success = _annualService.ManageBatch(isAccept: approve, batchId: batchId);
        
        if (approve && success)
        {
            // Batch data flows into royalty calculations
            return Json(new { message = "Annual data approved and processed" });
        }
        
        return Json(new { message = "Batch rejected, no data imported" });
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### Customer Management (ICustomerService)
| Method | Description |
|--------|-------------|
| `SaveCustomer(CustomerCreateEditModel)` | Create/update customer with cascaded address and emails |
| `DoesCustomerExists(string email)` | Check if customer exists by email |
| `GetCustomers(CustomerListFilter, int, int)` | Paginated customer list with filtering |
| `GetCustomerByEmail(IList<string>, List<Customer>)` | Find customers by email list (batch operation) |
| `GetCustomerByPhone(string, List<Customer>)` | Find customers by phone number |
| `UpdateMarketingClass(long, long)` | Change customer's marketing classification |
| `DownloadCustomerFile(CustomerListFilter, out string)` | Export customer list to CSV/Excel |

### Sales Data Upload (ISalesDataUploadService)
| Method | Description |
|--------|-------------|
| `Save(SalesDataUploadCreateModel)` | Upload sales data file for parsing |
| `GetBatchList(SalesDataListFilter, int, int)` | Retrieve upload batches with status |
| `DoesOverlappingDatesExist(long, DateTime, DateTime)` | Check for date range conflicts |
| `Reparse(long)` | Retry failed upload parsing |
| `CheckValidRangeForSalesUpload(SalesDataUploadCreateModel)` | Validate date range against payment frequency |
| `GetLastUploadedBatch(long)` | Get most recent upload date for franchisee |

### Annual Sales Data (IAnnualSalesDataUploadService)
| Method | Description |
|--------|-------------|
| `SaveUpload(AnnualDataUploadCreateModel)` | Upload annual sales report |
| `GetAnnualAuditRecord(SalesDataListFilter)` | Retrieve records requiring manual review |
| `ManageBatch(bool, long)` | Approve or reject annual data batch |
| `ReparseAnnualReport(long?)` | Reprocess annual report |
| `UpdateCustomerAddress(AnnualSalesDataCustomerViewModel)` | Correct flagged address mismatches |

### Account Credits (IAccountCreditService)
| Method | Description |
|--------|-------------|
| `Save(AccountCreditEditModel, long)` | Create credit memo with line items |
| `Get(AccountCreditListFilter, int, int)` | Paginated credit list |
| `GetCreditForInvoice(long, long)` | Retrieve applicable credits for invoice |
| `DeleteAccountCredit(long)` | Remove credit memo |

### Marketing Classification (IMarketingClassService)
| Method | Description |
|--------|-------------|
| `GetMarketingClassByInvoiceId(long)` | Get class from invoice's customer |
| `GetMarketingClassByPaymentId(long)` | Get class from payment's invoice's customer |

### Background Processing (Polling Agents)
| Interface | Description |
|-----------|-------------|
| `ISalesDataParsePollingAgent.ParseFile()` | Process queued sales uploads |
| `ICustomerFileUploadPollingAgent` | Process customer-only uploads |
| `IAnnualSalesDataParsePollingAgent` | Process annual reports with audit flagging |
| `ISalesDataUploadReminderPollingAgent` | Send reminder notifications for overdue uploads |

### Factories
| Interface | Description |
|-----------|-------------|
| `ICustomerFactory` | ViewModel ↔ Domain transformations for customers |
| `ISalesDataUploadFactory` | Create SalesDataUpload domain entities |
| `ISalesInvoiceFactory` | Create invoice entities from parsed data |
| `IAccountCreditFactory` | Create AccountCredit domain entities |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem: "Overlapping upload exists" error when uploading sales data**
- **Cause**: Another upload already covers part of the date range for this franchisee
- **Solution**: Check existing uploads with `GetBatchList()`, delete or adjust date range

**Problem: Parsing fails with "Invalid date range" error**
- **Cause**: Date range doesn't match payment frequency (e.g., 8-day range for weekly frequency)
- **Solution**: 
  - Weekly: Exactly 7 days (Monday-Sunday typical)
  - Monthly: Full calendar month (1st to last day)
  - Use `CheckValidRangeForSalesUpload()` to validate before uploading

**Problem: Customer sales metrics not updating**
- **Cause**: `TotalSales`, `AvgSales`, `NoOfSales` are calculated fields updated during invoice parsing
- **Solution**: Ensure parsing completed successfully (status = Parsed), trigger reparse if needed

**Problem: Annual report batch stuck in audit queue**
- **Cause**: Address mismatches or data anomalies require manual review
- **Solution**: 
  1. Call `GetAnnualAuditRecord()` to see flagged records
  2. Use `UpdateCustomerAddress()` to correct mismatches
  3. Call `ManageBatch(true, batchId)` to approve after review

**Problem: Marketing class showing as "Unclassified" for new customers**
- **Cause**: Parser couldn't determine classification from uploaded data
- **Solution**: Manually set with `UpdateMarketingClass(customerId, classTypeId)` or update source data to include classification hints

### Performance Tips

- **Large file uploads**: Files with 1000+ invoices can take 5-10 minutes to parse; ensure polling agent timeout is adequate
- **Batch operations**: When checking multiple customers by email, pass entire list to `GetCustomerByEmail()` rather than calling `DoesCustomerExists()` in a loop
- **Exports**: Use `DownloadCustomerFile()` instead of loading all customers into memory for reports
<!-- END CUSTOM SECTION -->
