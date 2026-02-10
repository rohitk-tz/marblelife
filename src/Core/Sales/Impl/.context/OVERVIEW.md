<!-- AUTO-GENERATED: Header -->
# Sales Implementation Layer
> 30 concrete service implementations, factories, parsers, and polling agents that power the Sales module
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Impl folder is where the "magic happens" - it contains all the actual code that makes the Sales module work. While the parent folder defines interfaces (the "what"), this folder implements them (the "how").

**The Four Pillars:**
1. **Services**: Business logic for customers, uploads, credits, reports
2. **Factories**: Object creation and ViewModel ↔ Domain transformations  
3. **Parsers**: Extract data from uploaded CSV/Excel files
4. **Polling Agents**: Background workers that continuously process queued uploads

**Real-World Analogy:**
Think of a mail sorting facility:
- **Services** are the workers who handle customer requests (CustomerService), manage shipments (SalesDataUploadService), and handle returns (AccountCreditService)
- **Factories** are the packaging stations that transform items between shipping boxes (ViewModels) and storage units (Domain entities)
- **Parsers** are the machines that read barcodes and labels on incoming packages (uploaded files)
- **Polling Agents** are the conveyor belts that continuously move packages through the system

**The Upload Processing Story:**
When a franchisee uploads their monthly sales file:

```
1. User clicks "Upload" → SalesDataUploadService.Save()
   - Validates date range (must be full month or exact week)
   - Checks for overlaps (no duplicate uploads for same period)
   - Creates SalesDataUpload entity (status = Uploaded)
   - File saved to storage

2. Background worker wakes up → SalesDataParsePollingAgent.ParseFile()
   - Queries for uploads with status = Uploaded
   - Updates status to ParseInProgress
   - Opens file with SalesDataFileParser
   
3. Parser reads file line-by-line:
   - Line 1: "John's Hotel, john@hotel.com, 555-1234, $2,500"
   - Creates/updates Customer via CustomerFactory
   - Creates Invoice and InvoiceItem entities
   - Links everything via FranchiseeSalesPayment
   
4. Aggregates updated:
   - Customer.TotalSales += $2,500
   - Customer.NoOfSales += 1
   - Customer.AvgSales recalculated
   
5. Upload finalized:
   - NumberOfCustomers = 45
   - NumberOfInvoices = 127
   - TotalAmount = $45,230.50
   - Status = Parsed
```

All of this happens automatically - franchisees just upload a file and wait for the "Processing complete" notification.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example: Using CustomerService

```csharp
using Core.Sales;
using Core.Sales.Impl;
using Core.Sales.ViewModel;

// Injected via DI (marked with [DefaultImplementation])
public class CustomerController
{
    private readonly ICustomerService _customerService;
    
    public ActionResult CreateCustomer()
    {
        var model = new CustomerCreateEditModel
        {
            Name = "Luxury Spa & Resort",
            ContactPerson = "Amanda Chen",
            Emails = new List<string> { "amanda@luxuryspa.com" },
            Phone = "555-9876",
            ClassTypeId = (long)MarketingClassType.Hotel,
            Address = new AddressEditModel
            {
                AddressLine1 = "456 Ocean Drive",
                City = "Miami Beach",
                StateId = 10,
                Zip = "33139"
            }
        };
        
        // Service handles:
        // - Email uniqueness validation
        // - Address cascade creation
        // - Marketing class assignment
        // - DataRecorderMetaData population
        Customer customer = _customerService.SaveCustomer(model);
        
        return Json(new { success = true, customerId = customer.Id });
    }
    
    public ActionResult SearchCustomers(string searchText)
    {
        var filter = new CustomerListFilter
        {
            SearchText = searchText,
            ClassTypeId = (long)MarketingClassType.Hotel, // Hotels only
            MinTotalSales = 10000 // Minimum $10k in sales
        };
        
        var result = _customerService.GetCustomers(filter, pageNumber: 1, pageSize: 50);
        
        // result.Customers contains CustomerViewModel with aggregates:
        // - TotalSales, AvgSales, NoOfSales
        // - Address, MarketingClass, Emails populated
        
        return View(result);
    }
}
```

### Example: Upload Processing Flow

```csharp
using Core.Sales;
using Core.Sales.Impl;

public class SalesUploadController
{
    private readonly ISalesDataUploadService _uploadService;
    private readonly ISalesDataUploadCreateModelValidator _validator;
    
    [HttpPost]
    public ActionResult Upload(HttpPostedFileBase file, long franchiseeId)
    {
        var model = new SalesDataUploadCreateModel
        {
            FranchiseeId = franchiseeId,
            PeriodStartDate = new DateTime(2025, 2, 1),
            PeriodEndDate = new DateTime(2025, 2, 28),
            File = file,
            PaymentFrequencyId = 2 // Monthly
        };
        
        // Validation: full month (Feb 1-28)
        if (!_validator.ValidateDates(model))
        {
            return Json(new { success = false, error = "Invalid date range for monthly upload" });
        }
        
        // Check for overlaps
        if (_uploadService.DoesOverlappingDatesExist(franchiseeId, model.PeriodStartDate, model.PeriodEndDate))
        {
            return Json(new { success = false, error = "Upload already exists for this period" });
        }
        
        // Create upload (status = Uploaded, queued for parsing)
        _uploadService.Save(model);
        
        return Json(new { 
            success = true, 
            message = "Upload successful. Processing will begin shortly." 
        });
    }
    
    public ActionResult CheckStatus(long uploadId)
    {
        var upload = _uploadService.GetById(uploadId);
        var status = (SalesDataUploadStatus)upload.StatusId;
        
        return Json(new
        {
            status = status.ToString(),
            customersProcessed = upload.NumberOfCustomers,
            invoicesProcessed = upload.NumberOfInvoices,
            totalAmount = upload.TotalAmount,
            errors = upload.NumberOfFailedRecords,
            isComplete = status == SalesDataUploadStatus.Parsed || status == SalesDataUploadStatus.Failed
        });
    }
}
```

### Example: Polling Agent (Background Worker)

```csharp
// This runs continuously in background (scheduled task or Windows Service)
using Core.Sales;
using Core.Sales.Impl;

public class BackgroundJobScheduler
{
    private readonly ISalesDataParsePollingAgent _parsingAgent;
    private readonly IAnnualSalesDataParsePollingAgent _annualAgent;
    private readonly ICustomerFileUploadPollingAgent _customerAgent;
    
    public void Start()
    {
        // Run every 30 seconds
        Timer timer = new Timer(30000);
        timer.Elapsed += (sender, e) =>
        {
            try
            {
                // Process regular sales uploads
                _parsingAgent.ParseFile();
                
                // Process annual uploads
                _annualAgent.ParseFile();
                
                // Process customer-only uploads
                _customerAgent.ProcessUploads();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        };
        timer.Start();
    }
}

// Inside SalesDataParsePollingAgent.ParseFile():
// 1. Query: SELECT * FROM SalesDataUpload WHERE StatusId = 71 (Uploaded)
// 2. foreach (var upload in uploads)
//    {
//        update.StatusId = 74 (ParseInProgress)
//        var fileContent = _fileService.ReadFile(upload.FileId)
//        var rows = _parser.Parse(fileContent)
//        
//        foreach (var row in rows)
//        {
//            // Create/update customer
//            var customer = _customerService.FindOrCreateCustomer(row)
//            
//            // Create invoice
//            var invoice = _invoiceService.CreateInvoice(row, customer)
//            
//            // Update metrics
//            customer.TotalSales += row.Amount
//            customer.NoOfSales++
//        }
//        
//        upload.StatusId = 72 (Parsed)
//        upload.NumberOfCustomers = customers.Count
//        upload.TotalAmount = rows.Sum(r => r.Amount)
//    }
```

### Example: Account Credit Creation

```csharp
using Core.Sales;
using Core.Sales.Impl;

public class CreditController
{
    private readonly IAccountCreditService _creditService;
    
    public ActionResult IssueRefund(long customerId)
    {
        var model = new AccountCreditEditModel
        {
            CustomerId = customerId,
            QbInvoiceNumber = "INV-2025-123",
            CreditedOn = DateTime.Now,
            Items = new List<AccountCreditItemEditModel>
            {
                new AccountCreditItemEditModel
                {
                    Description = "Refund for service issue",
                    Amount = 500.00m,
                    Quantity = 1
                }
            }
        };
        
        long currencyRateId = 1; // USD
        
        // Factory creates AccountCredit with AccountCreditItem collection
        var credit = _creditService.Save(model, currencyRateId);
        
        // Credit now available for invoice application
        return Json(new { success = true, creditId = credit.Id });
    }
    
    public ActionResult GetApplicableCredits(long franchiseeId, long invoiceId)
    {
        // Retrieves credits that can be applied to this invoice
        var credits = _creditService.GetCreditForInvoice(franchiseeId, invoiceId);
        
        return Json(credits);
    }
}
```

### Example: Annual Upload with Audit

```csharp
using Core.Sales;
using Core.Sales.Impl;

public class AnnualReportController
{
    private readonly IAnnualSalesDataUploadService _annualService;
    
    [HttpPost]
    public ActionResult UploadAnnualReport(HttpPostedFileBase file, long franchiseeId)
    {
        var model = new AnnualDataUploadCreateModel
        {
            FranchiseeId = franchiseeId,
            Year = 2024,
            File = file
        };
        
        if (_annualService.isValidUpload(model))
        {
            // Creates upload with AuditActionId = Pending
            // Parsing will flag address mismatches for review
            _annualService.SaveUpload(model);
            
            return Json(new { success = true, message = "Annual report uploaded. Parsing in progress..." });
        }
        
        return Json(new { success = false, error = "Invalid upload" });
    }
    
    // Staff reviews audit queue
    public ActionResult AuditQueue()
    {
        var filter = new SalesDataListFilter
        {
            AuditStatus = (long)AuditActionType.Pending
        };
        
        // Get uploads requiring review
        var audits = _annualService.GetAnnualAuditRecord(filter);
        
        // audits.List contains records with:
        // - NoOfMismatchedRecords > 0 (address issues)
        // - Data quality flags
        // - Calculated royalties
        
        return View(audits);
    }
    
    [HttpPost]
    public ActionResult ApproveAudit(long batchId)
    {
        // After staff reviews and fixes issues
        bool success = _annualService.ManageBatch(isAccept: true, batchId: batchId);
        
        if (success)
        {
            // AuditActionId = Approved
            // Data flows into royalty calculations
            return Json(new { success = true, message = "Batch approved" });
        }
        
        return Json(new { success = false, error = "Approval failed" });
    }
}
```

### Example: File Export

```csharp
using Core.Sales;
using Core.Sales.Impl;

public class ReportController
{
    private readonly ICustomerService _customerService;
    private readonly IAnnualSalesDataUploadService _annualService;
    
    public ActionResult ExportCustomers()
    {
        var filter = new CustomerListFilter
        {
            ClassTypeId = (long)MarketingClassType.Hotel,
            MinTotalSales = 50000 // High-value hotels
        };
        
        string fileName;
        bool success = _customerService.DownloadCustomerFile(filter, out fileName);
        
        if (success)
        {
            // fileName = "Customers_2025-02-10.xlsx"
            return File(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        
        return HttpNotFound();
    }
    
    public ActionResult ExportAnnualData(long franchiseeId, int year)
    {
        var filter = new SalesDataListFilter
        {
            FranchiseeId = franchiseeId,
            Year = year
        };
        
        string fileName;
        bool success = _annualService.DownloadAnnualDataFileFormatted(filter, out fileName);
        
        if (success)
        {
            return File(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        
        return HttpNotFound();
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Implementation Summary

### Service Implementations
| Class | Interface | Purpose |
|-------|-----------|---------|
| `CustomerService` | `ICustomerService` | Customer CRUD, search, marketing class updates, file export |
| `SalesDataUploadService` | `ISalesDataUploadService` | Upload validation, batch management, overlap detection |
| `AnnualSalesDataUploadService` | `IAnnualSalesDataUploadService` | Annual upload processing, audit workflow |
| `AccountCreditService` | `IAccountCreditService` | Credit memo creation and management |
| `MarketingClassService` | `IMarketingClassService` | Marketing classification lookups |
| `RoyaltyReportService` | `IRoyaltyReportService` | Royalty report generation |
| `SalesFunnelNationalService` | `ISalesFunnelNationalService` | National account sales funnel |
| `SalesInvoiceService` | `ISalesInvoiceService` | Invoice file processing |
| `DownloadFileHelperService` | `IDownloadFileHelperService` | CSV/Excel export utilities |

### Polling Agents
| Class | Purpose | Frequency |
|-------|---------|-----------|
| `SalesDataParsePollingAgent` | Parse regular sales uploads | Every 30 seconds (typical) |
| `AnnualSalesDataParsePollingAgent` | Parse annual reports with audit | Every 60 seconds |
| `CustomerFileUploadPollingAgent` | Parse customer-only uploads | Every 30 seconds |
| `SalesDataUploadReminderPollingAgent` | Send overdue upload reminders | Daily |

### Factories
| Class | Creates | From |
|-------|---------|------|
| `CustomerFactory` | Customer entities | CustomerCreateEditModel, CustomerEditModel |
| `AccountCreditFactory` | AccountCredit entities | AccountCreditEditModel |
| `SalesDataUploadFactory` | SalesDataUpload entities | SalesDataUploadCreateModel |
| `SalesInvoiceFactory` | Invoice entities | Parsed file data |
| `RoyaltyReportFactory` | RoyaltyReportViewModel | Database queries |

### Parsers
| Class | Parses | Format |
|-------|--------|--------|
| `SalesDataFileParser` | Sales data with invoices | CSV, Excel |
| `CustomerFileParser` | Customer-only data | CSV, Excel |
| `UpdateInvoiceFileParser` | Invoice updates | CSV, Excel |
| `InvoiceFileParserService` | Invoice files | CSV, Excel |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem: Uploads stuck in "Uploaded" status, never parse**
- **Cause**: Polling agent not running or crashed
- **Solution**: Check background job scheduler, restart polling agent service

**Problem: Parsing fails with "Invalid file format" error**
- **Cause**: File doesn't match expected column layout
- **Solution**: Verify CSV/Excel columns match parser expectations, check for encoding issues (UTF-8 required)

**Problem: Customer duplicates created during parsing**
- **Cause**: Email/phone matching failed, fell back to name+address which didn't match exactly
- **Solution**: Improve data quality in uploaded files, ensure consistent customer information

**Problem: Sales aggregates (TotalSales, AvgSales) incorrect**
- **Cause**: Upload parsed but aggregate calculation failed, or overlapping uploads created duplicate data
- **Solution**: Check for overlapping uploads, trigger reparse, manually recalculate aggregates

**Problem: Annual upload approved but data not showing in reports**
- **Cause**: `AuditActionId` still set to Pending, not Approved
- **Solution**: Verify `ManageBatch(isAccept = true)` was called successfully

**Problem: Memory errors during large file parsing**
- **Cause**: Loading entire file into memory
- **Solution**: Implement streaming parser, process line-by-line, commit in batches

### Performance Tips

- **Batch Commits**: Commit every 100-200 records during parsing to avoid transaction timeouts
- **Eager Loading**: Use `.Include()` for navigation properties to avoid N+1 queries
- **Disable Change Tracking**: For read-only queries, use `.AsNoTracking()`
- **Index Foreign Keys**: Ensure indexes on `CustomerId`, `FranchiseeId`, `StatusId`
- **Cache Lookups**: Cache marketing classes, states, cities during parsing session

### Debugging Workflow

1. **Check Upload Status**: Query `SalesDataUpload` table, verify `StatusId`
2. **Review Error Logs**: Check `ParsedLogFileId` for detailed error messages
3. **Inspect Parsed Counts**: `NumberOfParsedRecords` vs `NumberOfFailedRecords`
4. **Verify File Format**: Open uploaded file, check column layout and data quality
5. **Test Parser Independently**: Unit test parser with sample file
6. **Check Dependencies**: Ensure all services (Customer, Invoice, etc.) are working
7. **Monitor Background Jobs**: Verify polling agents are running and not crashed
<!-- END CUSTOM SECTION -->
