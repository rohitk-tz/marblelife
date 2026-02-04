<!-- AUTO-GENERATED: Header -->
# Sales Implementation
> Business Logic Layer - CRM Orchestration, File Parsing, Royalty Calculation, and Background Jobs
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Sales Implementation** module is the **business logic orchestration layer** for Marblelife's CRM system. It transforms uploaded Excel/CSV files into structured domain entities, manages customer lifecycles, calculates progressive royalty fees, and coordinates multi-step import workflows through background polling agents.

**Core Capabilities**:
- 📊 **File Parsing** - Excel/CSV → Domain entities with validation and error handling
- 🏭 **Factory Pattern** - Bidirectional Domain ↔ ViewModel transformations
- 🔄 **Background Jobs** - Async file processing via Quartz.NET polling agents
- 💰 **Royalty Calculation** - Progressive tier calculations with YTD tracking
- 🔍 **Annual Reconciliation** - Tax filing validation against weekly submissions

**Architecture**: Service Facade + Factory Pattern + Polling Agent Pattern. All data access via `IRepository<T>`, all file operations via `IFileService`, all notifications via `INotificationService`.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Quick Start

### Uploading Sales Data (Franchisee Weekly Submission)
```csharp
// 1. User uploads Excel file via API
var model = new SalesDataUploadCreateModel
{
    FranchiseeId = 42,
    PeriodStartDate = new DateTime(2024, 1, 1),
    PeriodEndDate = new DateTime(2024, 1, 7),
    File = new FileUploadModel
    {
        FileName = "sales-week1.xlsx",
        Content = fileBytes
    }
};

// 2. Service validates and saves upload
var service = container.Resolve<ISalesDataUploadService>();
service.Save(model); // Creates SalesDataUpload with StatusId = Uploaded (71)

// 3. Background job processes (runs every 5 minutes via Quartz.NET)
var agent = container.Resolve<ISalesDataParsePollingAgent>();
agent.Execute();
// ↑ Parses file, creates customers/invoices, updates status to Parsed (72)

// 4. Check parsing results
var upload = service.GetBatchList(new SalesDataListFilter { FranchiseeId = 42 }, 1, 10);
Console.WriteLine($"Parsed {upload.NumberOfCustomers} customers, {upload.NumberOfInvoices} invoices");
```

### Customer Management (CRUD Operations)
```csharp
var customerService = container.Resolve<ICustomerService>();

// Create new customer
var customer = customerService.SaveCustomer(new CustomerCreateEditModel
{
    Name = "Acme Corporation",
    ContactPerson = "John Doe",
    Phone = "555-1234",
    MarketingClassId = (long)MarketingClassType.Commercial,
    Address = new AddressEditModel
    {
        Street = "123 Main St",
        CityName = "New York",
        StateName = "NY",
        Zip = "10001"
    },
    CustomerEmails = new List<CustomerEmailEditModel>
    {
        new CustomerEmailEditModel { Email = "john@acme.com" }
    }
});

// Update existing customer (upsert logic)
var existingCustomer = customerService.Get(customer.Id);
existingCustomer.Phone = "555-5678";
customerService.Save(existingCustomer);

// Search customers
var searchResults = customerService.GetCustomers(
    new CustomerListFilter
    {
        SearchText = "Acme", // Searches Name, Email, Phone
        MarketingClassId = (long)MarketingClassType.Commercial
    },
    pageNumber: 1,
    pageSize: 20
);

// Export to Excel
if (customerService.DownloadCustomerFile(filter, out string fileName))
{
    Console.WriteLine($"Exported to {fileName}");
}
```

### Generating Royalty Report (HQ Review Before Billing)
```csharp
var royaltyService = container.Resolve<IRoyaltyReportService>();

// Get aggregated report for a parsed upload
var report = royaltyService.GetRoyaltyReport(salesDataUploadId: 123);

// Report includes:
Console.WriteLine($"Franchisee: {report.FranchiseeeName}");
Console.WriteLine($"Period: {report.StartDate:MM/dd/yyyy} - {report.EndDate:MM/dd/yyyy}");
Console.WriteLine($"YTD Sales: ${report.YTDSales:N2}");
Console.WriteLine($"Currency: {report.CurrencyCode} (Rate: {report.CurrencyRate})");

// Breakdown by marketing class and service type
foreach (var item in report.Collection)
{
    Console.WriteLine($"{item.MarketingClass}: ${item.TotalSales:N2} ({item.InvoiceCount} invoices)");
}

// Royalty fee slabs (progressive tiers)
foreach (var slab in report.RoyaltyFeeSlabs)
{
    Console.WriteLine($"${slab.FromAmount:N0} - ${slab.ToAmount:N0}: {slab.Percentage}%");
}
```

### Generating Royalty Invoice (HQ Billing Franchisee)
```csharp
var salesFunnelService = container.Resolve<ISalesFunnelNationalService>();

// Generate invoice for parsed upload
long invoiceId = salesFunnelService.GenerateInvoice(
    salesDataUploadId: 123,
    invoiceTypeId: (long)InvoiceType.Royalty
);

// Creates:
// - Invoice (parent entity)
// - InvoiceItem + RoyaltyInvoiceItem (8% of sales)
// - InvoiceItem + AdFundInvoiceItem (2% of sales)
// - InvoiceItem + NationalInvoiceItem (if applicable)
// - Marks SalesDataUpload.IsInvoiceGenerated = true

Console.WriteLine($"Created invoice #{invoiceId}");
```

### Annual Reconciliation (HQ Validating Year-End Totals)
```csharp
var annualService = container.Resolve<IAnnualSalesDataUploadService>();

// Upload franchisee's annual tax filing documents
var annualModel = new SalesDataUploadCreateModel
{
    FranchiseeId = 42,
    Year = "2024",
    IsAnnualUpload = true,
    AnnualFile = new FileUploadModel
    {
        FileName = "tax-filing-2024.pdf",
        Content = pdfBytes
    }
};

// Validation checks
var validationResult = annualService.GetAnnualUploadInfo(new AnnualUploadValidationModel
{
    FranchiseeId = 42,
    Year = 2024
});

if (!validationResult.IsValid)
{
    Console.WriteLine($"Missing documents: {string.Join(", ", validationResult.MissingDocuments)}");
    return;
}

// Save annual upload
annualService.Save(annualModel);

// Background job compares against weekly uploads
var annualAgent = container.Resolve<IAnnualSalesDataParsePollingAgent>();
annualAgent.Execute();
// ↑ Creates SystemAuditRecord if mismatch detected
```

### Bulk Customer Import (No Invoices)
```csharp
var customerService = container.Resolve<ICustomerService>();

// Upload customer CSV
var uploadModel = new CustomerFileUploadCreateModel
{
    File = new FileUploadModel
    {
        FileName = "customers.csv",
        Content = csvBytes
    }
};

customerService.Save(uploadModel); // Creates CustomerFileUpload

// Background job processes
var customerAgent = container.Resolve<ICustomerFileUploadPollingAgent>();
customerAgent.Execute();
// ↑ Parses CSV, upserts customers (match by name + address)
```

### Account Credits (Customer Refunds)
```csharp
var creditService = container.Resolve<IAccountCreditService>();

// Create credit memo
creditService.Save(new AccountCreditEditModel
{
    CustomerId = 100,
    QbInvoiceNumber = "CM-2024-001",
    CreditedOn = DateTime.Now,
    Items = new List<AccountCreditItemEditModel>
    {
        new AccountCreditItemEditModel
        {
            Description = "Overpayment refund",
            Amount = 250.00m
        }
    }
});
```

### Reparsing Failed Upload
```csharp
var service = container.Resolve<ISalesDataUploadService>();

// Reset upload to reprocess
bool success = service.Reparse(uploadId: 123);
// ↑ Sets StatusId = Uploaded, clears stats, deletes related invoices
// Next polling agent run will reprocess
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## 📚 Service Catalog

### Core Services
| Service | Primary Responsibility | Key Methods |
|---------|------------------------|-------------|
| `CustomerService` | Customer CRUD + bulk operations | `SaveCustomer()`, `GetCustomers()`, `DownloadCustomerFile()` |
| `SalesDataUploadService` | Upload lifecycle management | `Save()`, `GetBatchList()`, `Reparse()`, `CheckForExpiringDocument()` |
| `RoyaltyReportService` | Royalty aggregation for HQ review | `GetRoyaltyReport()` |
| `SalesFunnelNationalService` | Royalty invoice generation | `GenerateInvoice()` |
| `MarketingClassService` | Marketing class lookup utilities | `GetMarketingClassByInvoiceId()` |
| `AccountCreditService` | Credit memo management | `Save()`, `GetList()` |
| `InvoiceItemUpdateInfoService` | Bulk invoice corrections | `UpdateInvoiceItems()` |

### Background Polling Agents (Quartz.NET Jobs)
| Agent | Trigger | Responsibility |
|-------|---------|----------------|
| `SalesDataParsePollingAgent` | Every 5 min | Parse uploaded sales files, create customers/invoices |
| `AnnualSalesDataParsePollingAgent` | Every 10 min | Parse annual tax filings, validate against weekly uploads |
| `CustomerFileUploadPollingAgent` | Every 5 min | Parse customer-only CSV imports |
| `SalesDataUploadReminderPollingAgent` | Daily at 9 AM | Send email reminders for missing uploads |

### File Parsers
| Parser | Input Format | Output |
|--------|--------------|--------|
| `SalesDataFileParser` | Excel (.xlsx) | `ParsedFileParentModel` (Customer + Invoice + Payment) |
| `CustomerFileParser` | CSV | `CustomerCreateEditModel` |
| `UpdateInvoiceFileParser` | Excel (.xlsx) | `InvoiceInfoEditModel` (corrections) |

### Factories (Domain ↔ ViewModel)
| Factory | Purpose |
|---------|---------|
| `CustomerFactory` | Customer entity transformations |
| `SalesDataUploadFactory` | SalesDataUpload entity transformations |
| `SalesInvoiceFactory` | Invoice entity for sales data |
| `RoyaltyReportFactory` | Royalty report view models |
| `AccountCreditFactory` | AccountCredit entity transformations |
| `AccountCreditItemFactory` | AccountCreditItem entity transformations |
| `SalesFunnelFactory` | Sales funnel aggregations |

### Validators
| Validator | Validates |
|-----------|-----------|
| `SalesDataUploadCreateModelValidator` | Upload model (file, dates, overlaps) |

### Helper Services
| Service | Purpose |
|---------|---------|
| `DownloadFileHelperService` | Excel/CSV export generation |
| `InvoiceFileParserService` | Wrapper for invoice file parsing |
| `UpdatingInvoiceNotificationServices` | Email notifications for invoice updates |
| `UpdatingInvoiceIdsNotificationServices` | Email notifications for ID corrections |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🏗 Architecture Patterns

### Factory Pattern
```csharp
// Bidirectional transformations
public interface ICustomerFactory
{
    Customer CreateDomain(CustomerEditModel model); // ViewModel → Domain
    CustomerEditModel CreateEditModel(Customer domain); // Domain → ViewModel
}

// Usage in service layer
public void Save(CustomerEditModel model)
{
    var domain = _customerFactory.CreateDomain(model);
    _customerRepository.Save(domain);
}
```

### Polling Agent Pattern
```csharp
// Background job interface
public interface ISalesDataParsePollingAgent
{
    void Execute(); // Called by Quartz.NET scheduler
}

// Typical implementation
public void Execute()
{
    var pendingUploads = _repository.Table
        .Where(u => u.StatusId == (long)SalesDataUploadStatus.Uploaded)
        .OrderBy(u => u.DataRecorderMetaData.DateCreated)
        .Take(10) // Batch size
        .ToList();
    
    foreach (var upload in pendingUploads)
    {
        ProcessUpload(upload);
    }
}
```

### Service Facade Pattern
```csharp
// High-level orchestration
public class SalesDataUploadService : ISalesDataUploadService
{
    // Aggregates multiple operations
    public void Save(SalesDataUploadCreateModel model)
    {
        // 1. Validate
        _validator.Validate(model);
        
        // 2. Transform
        var domain = _factory.CreateDomain(model);
        
        // 3. Persist file
        var file = _fileService.SaveModel(model.File);
        domain.FileId = file.Id;
        
        // 4. Get currency rate
        domain.CurrencyExchangeRateId = GetCurrentRate(model.FranchiseeId);
        
        // 5. Save domain
        _repository.Save(domain);
        
        // 6. Handle annual upload if applicable
        if (model.IsAnnualUpload)
        {
            SaveAnnualUpload(model, domain);
        }
    }
}
```

### Parser Strategy Pattern
```csharp
// Different parsers for different file types
public interface ISalesDataFileParser
{
    IList<ParsedFileParentModel> PrepareDomainFromDataTable(DataTable dt);
}

public interface ICustomerFileParser
{
    IList<CustomerCreateEditModel> PrepareDomainFromDataTable(DataTable dt);
}

// Parsers maintain internal state (header mapping, lookups)
private Dictionary<string, int> _headersDictionary;
private List<MarketingClass> _marketingClasses; // Pre-loaded for fuzzy matching
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Best Practices -->
## ✅ Best Practices

### 1. File Parsing Error Handling
```csharp
// Always wrap parsing in try-catch
try
{
    var dataTable = ExcelFileParser.ParseExcel(fileContent);
    var models = _parser.PrepareDomainFromDataTable(dataTable);
    
    foreach (var model in models)
    {
        _customerService.SaveCustomer(model.CustomerModel);
    }
    
    upload.StatusId = (long)SalesDataUploadStatus.Parsed;
}
catch (Exception ex)
{
    upload.StatusId = (long)SalesDataUploadStatus.Failed;
    upload.ParsedLogFileId = _fileService.SaveErrorLog(ex.ToString());
    _logService.Error($"Parsing failed for upload {upload.Id}", ex);
}
```

### 2. Currency Exchange Rates
```csharp
// ALWAYS query rate by franchisee country + date
var rate = _currencyExchangeRateRepository.Table
    .Where(r => r.CountryId == franchisee.Organization.Address.First().CountryId)
    .Where(r => r.DateTime <= salesData.PeriodEndDate)
    .OrderByDescending(r => r.DateTime)
    .First();

salesdataUpload.CurrencyExchangeRateId = rate.Id;
```

### 3. Duplicate Customer Detection
```csharp
// Use GetCustomerByNameAndAddress for upsert logic
var existing = _customerService.GetCustomerByNameAndAddress(
    name: model.Name,
    address: model.Address,
    customerRepository: _customerRepository.Table.ToList()
);

if (existing != null)
{
    // Merge: Update existing, add new emails
    model.Id = existing.Id;
    model.DataRecorderMetaData = existing.DataRecorderMetaData;
}
```

### 4. Progressive Royalty Calculation
```csharp
// Calculate YTD sales first for proper tier placement
var ytdSales = _franchiseeSalesRepository.Table
    .Where(fs => fs.SalesDataUpload.FranchiseeId == franchiseeId)
    .Where(fs => fs.Invoice.GeneratedOn < currentPeriodStart)
    .Where(fs => fs.Invoice.GeneratedOn.Year == currentPeriodStart.Year)
    .SelectMany(fs => fs.Invoice.InvoiceItems)
    .Sum(ii => ii.Amount);

// Then apply slabs
var royaltyAmount = CalculateRoyaltyWithSlabs(currentSales, ytdSales, feeProfile.RoyaltyFeeSlabs);
```

### 5. Shared PK Invoice Extensions
```csharp
// Always save parent InvoiceItem first
var invoiceItem = new InvoiceItem { Amount = 1500 };
_invoiceItemRepository.Save(invoiceItem);
_unitOfWork.SaveChanges(); // Flush to get ID

// Then create child with same ID
var royaltyDetail = new RoyaltyInvoiceItem
{
    Id = invoiceItem.Id, // ← Must match parent
    Percentage = 8.0m,
    Amount = 1500
};
_royaltyInvoiceItemRepository.Save(royaltyDetail);
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Troubleshooting -->
## 🔧 Troubleshooting

### Issue: Polling agent not processing files
**Symptoms**: Files stay in "Uploaded" status indefinitely.

**Diagnosis**:
1. Check Quartz.NET job is running (`Jobs` project logs)
2. Verify connection string in `Web.config` or `App.config`
3. Check agent is registered in DI container (`DependencyInjection` module)

**Solution**:
```csharp
// Check job registration in Jobs/Program.cs
scheduler.ScheduleJob(
    JobBuilder.Create<SalesDataParsePollingAgentJob>().Build(),
    TriggerBuilder.Create().WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever()).Build()
);
```

### Issue: Parsing fails with "Column not found"
**Symptoms**: StatusId = Failed (73), error log shows column mapping exception.

**Cause**: Excel file has different column names than expected.

**Solution**: Update parser header mappings or provide column name aliases:
```csharp
// SalesDataFileParser.cs
private void PrepareHeaderIndex(DataTable dt)
{
    var columnAliases = new Dictionary<string, string>
    {
        { "Cust Name", "Customer" }, // Accept "Cust Name" as "Customer"
        { "Inv #", "Invoice Number" }
    };
    
    // Map with aliases...
}
```

### Issue: Duplicate customers created
**Symptoms**: Same customer appears multiple times with slight name variations.

**Cause**: Fuzzy matching logic not catching variations.

**Solution**: Improve matching in `CustomerService.GetCustomerByNameAndAddress()`:
```csharp
// Use Levenshtein distance or phonetic matching
var existing = customerRepo.ToList().FirstOrDefault(c =>
    c.Name.ToLower().Replace(" ", "") == name.ToLower().Replace(" ", "") &&
    c.Address.Street.ToLower() == address.Street.ToLower()
);
```

### Issue: Currency conversion incorrect
**Symptoms**: Royalty invoice amounts don't match franchisee currency.

**Cause**: Using wrong `CurrencyExchangeRateId` (latest rate instead of period-specific).

**Solution**: Query rate by upload end date:
```csharp
var rate = _currencyRepo.Table
    .Where(r => r.CountryId == countryId)
    .Where(r => r.DateTime <= salesData.PeriodEndDate) // ← Key: Use period date
    .OrderByDescending(r => r.DateTime)
    .FirstOrDefault();
```

### Issue: Royalty calculation doesn't match expected amount
**Symptoms**: HQ-generated invoice amount differs from manual calculation.

**Cause**: YTD sales not included in progressive tier calculation.

**Solution**: Always pass YTD to slab calculator:
```csharp
var ytdSales = GetYTDSales(franchiseeId, currentPeriodStart);
var royalty = CalculateRoyaltyWithSlabs(currentSales, ytdSales, slabs);
```

### Issue: Shared PK violation on RoyaltyInvoiceItem save
**Symptoms**: SQL error "Cannot insert duplicate key".

**Cause**: Trying to save RoyaltyInvoiceItem before parent InvoiceItem is flushed.

**Solution**: Force SaveChanges after parent:
```csharp
_invoiceItemRepository.Save(invoiceItem);
_unitOfWork.SaveChanges(); // ← Flush to get ID

var royalty = new RoyaltyInvoiceItem { Id = invoiceItem.Id };
_royaltyInvoiceItemRepository.Save(royalty);
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Related Links -->
## 🔗 Related Documentation
- [Sales/Domain README](../Domain/README.md) - Entity definitions
- [Sales/Enum README](../Enum/README.md) - Status codes and constants
- [Jobs Module](../../../Jobs/README.md) - Quartz.NET scheduler setup
- [API Module](../../../API/README.md) - REST endpoints for upload
- [Infrastructure](../../../Infrastructure/README.md) - FileService, Logging
<!-- END CUSTOM SECTION -->
