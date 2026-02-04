<!-- AUTO-GENERATED: Header -->
# Sales Implementation Module Context
**Version**: 3f7ca98653b76ee0fca84e0a126043097a12de5d
**Generated**: 2026-02-04T06:51:55Z
**Module Path**: src/Core/Sales/Impl
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
The Sales Implementation module provides **business logic orchestration** for the CRM pipeline. It transforms uploaded Excel/CSV files into domain entities, manages customer lifecycles, calculates royalty fees, and coordinates multi-step import workflows.

**Key Responsibilities**:
1. **File Parsing** - Transform Excel/CSV rows into domain ViewModels (`SalesDataFileParser`, `CustomerFileParser`, `UpdateInvoiceFileParser`)
2. **Factory Pattern** - Domain ↔ ViewModel transformations (`CustomerFactory`, `SalesDataUploadFactory`, `RoyaltyReportFactory`)
3. **Service Layer** - CRUD operations with business rules (`CustomerService`, `SalesDataUploadService`, `RoyaltyReportService`)
4. **Background Polling Agents** - Quartz.NET jobs for async file processing (`SalesDataParsePollingAgent`, `AnnualSalesDataParsePollingAgent`, `CustomerFileUploadPollingAgent`)
5. **Royalty Calculation** - Aggregate sales data and generate HQ invoices (`SalesFunnelNationalService`, `SalesInvoiceService`)

### Design Patterns
- **Factory Pattern**: Bidirectional Domain ↔ ViewModel conversion (e.g., `CustomerFactory.CreateDomain()` / `CreateEditModel()`)
- **Polling Agent Pattern**: Background jobs check for `StatusId == Uploaded`, parse files, update status to `Parsed` or `Failed`
- **Parser Strategy Pattern**: Each file type has dedicated parser (`SalesDataFileParser`, `CustomerFileParser`, `UpdateInvoiceFileParser`)
- **Service Facade**: High-level orchestration (`CustomerService.Save()` calls factories, repositories, and validation)
- **Validator Pattern**: FluentValidation-style validators (`SalesDataUploadCreateModelValidator`)
- **Repository Pattern**: All data access via `IRepository<T>` from UnitOfWork

### Data Flow
```
┌──────────────────────────────────────────────────────────────┐
│               Sales Data Upload Workflow                       │
└──────────────────────────────────────────────────────────────┘
1. API receives SalesDataUploadCreateModel
2. SalesDataUploadService.Save()
   ├─ SalesDataUploadFactory.CreateDomain()
   ├─ FileService.SaveModel() → Save Excel to File entity
   ├─ Set StatusId = Uploaded (71)
   └─ Save SalesDataUpload entity

3. SalesDataParsePollingAgent.Execute() (Quartz job, runs every N minutes)
   ├─ Query: StatusId == Uploaded
   ├─ Update StatusId = ParseInProgress (74)
   ├─ FileService.ReadExcel() → DataTable
   ├─ SalesDataFileParser.PrepareDomainFromDataTable()
   │   ├─ Parse each row → CustomerCreateEditModel, InvoiceEditModel
   │   ├─ CustomerService.SaveCustomer() → Upsert Customer + Address + Emails
   │   ├─ InvoiceService.Save() → Create Invoice + InvoiceItems
   │   └─ FranchiseeSalesService.Save() → Link invoice to upload
   ├─ Update statistics (NumberOfCustomers, NumberOfInvoices, TotalAmount)
   └─ Update StatusId = Parsed (72) or Failed (73)

4. RoyaltyReportService.GetRoyaltyReport()
   ├─ Query FranchiseeSales by SalesDataUploadId
   ├─ Aggregate by MarketingClass and ServiceType
   └─ Return RoyaltyReportListModel for HQ review

5. SalesFunnelNationalService.GenerateInvoice()
   ├─ Calculate royalty basis (sum of PriceOfService - LessDeposit)
   ├─ Apply FeeProfile royalty slabs (progressive tiers)
   ├─ Create Invoice with RoyaltyInvoiceItem + AdFundInvoiceItem
   └─ Set SalesDataUpload.IsInvoiceGenerated = true
```

```
┌──────────────────────────────────────────────────────────────┐
│             Annual Reconciliation Workflow                     │
└──────────────────────────────────────────────────────────────┘
1. HQ uploads franchisee tax filing (P&L, Balance Sheet)
2. AnnualSalesDataUploadService.Save()
   ├─ Validate: No duplicate year upload
   ├─ Check: All required documents uploaded
   └─ Create AnnualSalesDataUpload (StatusId = Uploaded)

3. AnnualSalesDataParsePollingAgent.Execute()
   ├─ Parse tax documents → Extract total sales, royalty paid
   ├─ Query: SUM(SalesDataUpload.TotalAmount) WHERE Year = X
   ├─ Compare: AnnualRoyality vs WeeklyRoyality
   ├─ IF mismatch > threshold:
   │   ├─ Create SystemAuditRecord
   │   ├─ Set AuditActionId = Pending (153)
   │   └─ Send notification to HQ
   └─ Update StatusId = Parsed
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Key Service Interfaces & Implementations

### Customer Management
```csharp
[DefaultImplementation]
public class CustomerService : ICustomerService
{
    // Dependencies (14 repositories injected via UnitOfWork)
    private readonly IRepository<Customer> _customerRepository;
    private readonly ICustomerFactory _customerFactory;
    private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
    private readonly IExcelFileCreator _excelFileCreator;
    
    // Core Operations
    public Customer SaveCustomer(CustomerCreateEditModel model)
    {
        // Upsert logic:
        // 1. Find existing by Name + Address (fuzzy match)
        // 2. Merge emails (avoid duplicates)
        // 3. Update denormalized fields (TotalSales, NoOfSales, AvgSales)
        // 4. Save cascade: Customer → Address → CustomerEmails
    }
    
    public CustomerListModel GetCustomers(CustomerListFilter filter, int pageNumber, int pageSize)
    {
        // Multi-field search: Name, Email, Phone, MarketingClass
        // Pagination with ISortingHelper
        // Return lightweight ListModel (no nav properties)
    }
    
    public bool DownloadCustomerFile(CustomerListFilter filter, out string fileName)
    {
        // Export to Excel via IExcelFileCreator
        // Includes: Name, Address, Emails, MarketingClass, TotalSales
    }
}

[DefaultImplementation]
public class CustomerFactory : ICustomerFactory
{
    // Bidirectional transformations
    public Customer CreateDomain(CustomerCreateEditModel model, Customer existingDomain = null)
    {
        // Merge logic if existingDomain provided
        // Create Address via AddressFactory
        // Transform emails via EmailFactory
        // Calculate AvgSales = TotalSales / NoOfSales
    }
    
    public CustomerEditModel CreateEditModel(Customer domain)
    {
        // Flatten for UI editing
        // Include nested Address, Emails, MarketingClass
    }
}
```

### Sales Data Upload Pipeline
```csharp
[DefaultImplementation]
public class SalesDataUploadService : ISalesDataUploadService
{
    private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
    private readonly ISalesDataUploadFactory _salesDataUploadFactory;
    private readonly IFileService _fileService;
    private readonly IRepository<FeeProfile> _feeProfileRepository;
    private readonly IRepository<AnnualSalesDataUpload> _annualSalesDataUploadRepository;
    
    public void Save(SalesDataUploadCreateModel model)
    {
        // 1. Validate date range (no overlaps)
        // 2. Check document expiration (License, COI)
        // 3. Get currency exchange rate for franchisee country
        // 4. Save file via FileService
        // 5. Create SalesDataUpload (StatusId = Uploaded)
        // 6. If IsAnnualUpload: Create AnnualSalesDataUpload link
    }
    
    public bool DoesOverlappingDatesExist(long franchiseeId, DateTime startDate, DateTime endDate)
    {
        // Check: No existing upload with overlapping date range
        // Critical: Prevents duplicate royalty billing
    }
    
    public bool CheckForExpiringDocument(SalesDataUploadCreateModel model)
    {
        // Query: Franchisee documents (License, COI, Resale Certificate)
        // If expires within 30 days → Warning message
        // If expired → Block upload
    }
    
    public bool Reparse(long id)
    {
        // Reset StatusId = Uploaded
        // Clear parsing stats (NumberOfCustomers, NumberOfInvoices)
        // Delete related EstimateInvoice records
        // Polling agent will reprocess
    }
}

[DefaultImplementation]
public class SalesDataUploadFactory : ISalesDataUploadFactory
{
    public SalesDataUpload CreateDomain(SalesDataUploadCreateModel model)
    {
        // Map: Model → Domain entity
        // Set: StatusId = Uploaded, IsActive = true
        // Initialize: Parsing stats to null
    }
}
```

### File Parsers
```csharp
[DefaultImplementation]
public class SalesDataFileParser : ISalesDataFileParser
{
    // State: Header column mapping, Marketing/Service lookups
    private Dictionary<string, int> _headersDictionary;
    private List<MarketingClass> _marketingClasses; // Pre-loaded
    private List<SubClassMarketingClass> _subMarketingClasses;
    private List<ServiceType> _serviceTypes;
    
    public IList<ParsedFileParentModel> PrepareDomainFromDataTable(DataTable dt)
    {
        // 1. PrepareHeaderIndex() → Map column names to indices
        // 2. Foreach row:
        //    ├─ ParseRow() → Extract customer, invoice, payment data
        //    ├─ Validate: Required fields, data types
        //    ├─ Lookup: MarketingClass by name (case-insensitive, no spaces)
        //    ├─ Lookup: ServiceType by alias
        //    └─ Build: ParsedFileParentModel (grouped by invoice)
        // 3. Return collection for batch save
    }
    
    public bool CheckForValidClassName(DataTable dt, out string result)
    {
        // Validation pass: All class names exist in MarketingClass table
        // Returns first invalid class name in 'result'
    }
    
    // Key Parsing Logic
    private void ParseRow(DataRow row)
    {
        // Extract fields with null-safe accessors
        var customerName = GetValueOrEmpty(row, "Customer");
        var className = GetValueOrEmpty(row, "Class").ToUpper().Replace(" ", "");
        var invoiceNumber = GetValueOrEmpty(row, "Invoice Number");
        
        // Fuzzy match marketing class
        var marketingClass = _marketingClasses
            .FirstOrDefault(mc => mc.Name == className);
        
        // Group by QbInvoiceNumber (multiple rows = multiple line items)
        var existingInvoice = _parentModelCollection
            .FirstOrDefault(p => p.InvoiceModel.QbInvoiceNumber == invoiceNumber);
        
        if (existingInvoice != null)
        {
            // Add line item to existing invoice
            existingInvoice.InvoiceModel.InvoiceItems.Add(invoiceItemModel);
        }
        else
        {
            // Create new invoice with customer + payment
            _parentModelCollection.Add(new ParsedFileParentModel
            {
                CustomerModel = customerModel,
                InvoiceModel = invoiceModel,
                PaymentModel = paymentModel
            });
        }
    }
}

[DefaultImplementation]
public class CustomerFileParser : ICustomerFileParser
{
    // Simplified parser for customer-only imports (no invoices)
    public IList<CustomerCreateEditModel> PrepareDomainFromDataTable(DataTable dt)
    {
        // Parse: Name, Address, Email, Phone, MarketingClass
        // Validate: Email format, phone format
        // Return: List of CustomerCreateEditModel for batch upsert
    }
}

[DefaultImplementation]
public class UpdateInvoiceFileParser : IUpdateInvoiceFileParser
{
    // Corrects parsing errors from previous upload
    // Matches by QBInvoiceNumber → Update InvoiceItems
}
```

### Background Polling Agents
```csharp
[DefaultImplementation]
public class SalesDataParsePollingAgent : ISalesDataParsePollingAgent
{
    // Scheduled via Quartz.NET (Jobs module)
    // Runs every N minutes (configurable in Settings)
    
    public void Execute()
    {
        // 1. Query: SalesDataUpload WHERE StatusId == Uploaded (71)
        var pendingUploads = _salesDataUploadRepository.Table
            .Where(u => u.StatusId == (long)SalesDataUploadStatus.Uploaded)
            .OrderBy(u => u.DataRecorderMetaData.DateCreated)
            .Take(10) // Batch processing (configurable)
            .ToList();
        
        foreach (var upload in pendingUploads)
        {
            try
            {
                // 2. Update status to ParseInProgress (74)
                upload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
                _salesDataUploadRepository.Save(upload);
                
                // 3. Load file content
                var fileContent = _fileService.GetFileContent(upload.FileId);
                var dataTable = ExcelFileParser.ParseExcel(fileContent);
                
                // 4. Parse rows into domain models
                var parsedModels = _salesDataFileParser.PrepareDomainFromDataTable(dataTable);
                
                // 5. Save entities (transactional)
                var stats = new SaveModelStats();
                foreach (var model in parsedModels)
                {
                    // Save Customer (upsert by name + address)
                    var customer = _customerService.SaveCustomer(model.CustomerModel);
                    stats.NumberOfCustomers++;
                    
                    // Save Invoice + InvoiceItems
                    model.InvoiceModel.CustomerId = customer.Id;
                    _invoiceService.Save(model.InvoiceModel);
                    stats.NumberOfInvoices++;
                    
                    // Save FranchiseeSales (links invoice to upload)
                    _franchiseeSalesService.Save(new FranchiseeSalesEditModel
                    {
                        InvoiceId = model.InvoiceModel.Id,
                        SalesDataUploadId = upload.Id,
                        MarketingClassId = model.CustomerModel.MarketingClassId
                    });
                    
                    // Save Payment (if exists)
                    if (model.PaymentModel != null)
                    {
                        _paymentService.Save(model.PaymentModel);
                    }
                }
                
                // 6. Update upload statistics
                upload.NumberOfCustomers = stats.NumberOfCustomers;
                upload.NumberOfInvoices = stats.NumberOfInvoices;
                upload.NumberOfParsedRecords = parsedModels.Count;
                upload.NumberOfFailedRecords = stats.FailedRecords;
                upload.TotalAmount = stats.TotalAmount;
                upload.PaidAmount = stats.PaidAmount;
                upload.StatusId = (long)SalesDataUploadStatus.Parsed; // 72
                
                _salesDataUploadRepository.Save(upload);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                // 7. On error: Set status to Failed (73), log error file
                upload.StatusId = (long)SalesDataUploadStatus.Failed;
                upload.ParsedLogFileId = _fileService.SaveErrorLog(ex.ToString());
                _salesDataUploadRepository.Save(upload);
                _logService.Error($"Failed to parse upload {upload.Id}", ex);
            }
        }
    }
}

[DefaultImplementation]
public class AnnualSalesDataParsePollingAgent : IAnnualSalesDataParsePollingAgent
{
    // Similar to SalesDataParsePollingAgent, but for annual uploads
    public void Execute()
    {
        // 1. Query: AnnualSalesDataUpload WHERE StatusId == Uploaded
        // 2. Parse tax filing documents (P&L, Balance Sheet)
        // 3. Extract: Total sales, Royalty paid
        // 4. Compare: AnnualRoyality vs SUM(SalesDataUpload.TotalAmount WHERE Year = X)
        // 5. IF mismatch > 5%: Create SystemAuditRecord
        // 6. Update StatusId = Parsed
    }
}

[DefaultImplementation]
public class CustomerFileUploadPollingAgent : ICustomerFileUploadPollingAgent
{
    // Processes CustomerFileUpload (bulk customer import, no invoices)
    public void Execute()
    {
        // Parse customer CSV → CustomerFileParser
        // Batch upsert via CustomerService
    }
}

[DefaultImplementation]
public class SalesDataUploadReminderPollingAgent : ISalesDataUploadReminderPollingAgent
{
    // Sends email reminders to franchisees with missing uploads
    public void Execute()
    {
        // Query: Franchisees WHERE LastUploadDate < ExpectedUploadDate - 3 days
        // Send: Notification email via NotificationService
    }
}
```

### Royalty Calculation
```csharp
[DefaultImplementation]
public class RoyaltyReportService : IRoyaltyReportService
{
    public RoyaltyReportListModel GetRoyaltyReport(long salesDataUploadId)
    {
        // 1. Query: FranchiseeSales by SalesDataUploadId
        var franchiseeSales = _franchiseeSalesRepository.Table
            .Where(fs => fs.SalesDataUploadId == salesDataUploadId)
            .Include(fs => fs.Invoice.InvoiceItems)
            .Include(fs => fs.MarketingClass)
            .ToList();
        
        // 2. Aggregate by MarketingClass and ServiceType
        var aggregates = franchiseeSales
            .GroupBy(fs => new { fs.MarketingClassId, fs.ServiceTypeId })
            .Select(g => new
            {
                MarketingClass = g.Key.MarketingClassId,
                ServiceType = g.Key.ServiceTypeId,
                TotalSales = g.Sum(fs => fs.Invoice.InvoiceItems.Sum(ii => ii.Amount)),
                InvoiceCount = g.Count()
            })
            .ToList();
        
        // 3. Calculate YTD sales (for progressive royalty tiers)
        var ytdSales = _franchiseeSalesRepository.Table
            .Where(fs => fs.SalesDataUpload.FranchiseeId == salesData.FranchiseeId)
            .Where(fs => fs.Invoice.GeneratedOn < salesData.PeriodStartDate)
            .Where(fs => fs.Invoice.GeneratedOn.Year == salesData.PeriodStartDate.Year)
            .SelectMany(fs => fs.Invoice.InvoiceItems)
            .Sum(ii => ii.Amount);
        
        // 4. Return RoyaltyReportListModel with slabs for UI review
        return new RoyaltyReportListModel
        {
            FranchiseeId = salesData.FranchiseeId,
            FranchiseeeName = salesData.Franchisee.Organization.Name,
            StartDate = salesData.PeriodStartDate,
            EndDate = salesData.PeriodEndDate,
            YTDSales = ytdSales,
            Collection = aggregates,
            RoyaltyFeeSlabs = salesData.Franchisee.FeeProfile.RoyaltyFeeSlabs,
            CurrencyCode = salesData.Franchisee.Currency,
            CurrencyRate = salesData.CurrencyExchangeRate.Rate
        };
    }
}

[DefaultImplementation]
public class SalesFunnelNationalService : ISalesFunnelNationalService
{
    // Generates HQ invoice for franchisee royalty/ad fund
    public long GenerateInvoice(long salesDataUploadId, long invoiceTypeId)
    {
        var salesData = _salesDataUploadRepository.Get(salesDataUploadId);
        var franchisee = salesData.Franchisee;
        var feeProfile = franchisee.FeeProfile;
        
        // 1. Calculate royalty basis (sum of sales in upload)
        var totalSales = _franchiseeSalesRepository.Table
            .Where(fs => fs.SalesDataUploadId == salesDataUploadId)
            .SelectMany(fs => fs.Invoice.InvoiceItems)
            .Sum(ii => ii.Amount);
        
        // 2. Apply progressive royalty slabs (tiered calculation)
        var royaltyAmount = CalculateRoyaltyWithSlabs(totalSales, ytdSales, feeProfile.RoyaltyFeeSlabs);
        
        // 3. Calculate ad fund (flat percentage)
        var adFundAmount = totalSales * (feeProfile.AdFundPercentage / 100);
        
        // 4. Calculate national account fee (if applicable)
        var nationalFeeAmount = totalSales * (feeProfile.NationalPercentage / 100);
        
        // 5. Create Invoice
        var invoice = new Invoice
        {
            OrganizationId = franchisee.OrganizationId,
            GeneratedOn = DateTime.Now,
            DueDate = DateTime.Now.AddDays(feeProfile.PaymentTermsDays),
            StatusId = (long)InvoiceStatus.Unpaid
        };
        _invoiceRepository.Save(invoice);
        
        // 6. Create InvoiceItems with extension entities
        var royaltyItem = new InvoiceItem
        {
            InvoiceId = invoice.Id,
            Description = $"Royalty ({salesData.PeriodStartDate:MM/dd/yyyy} - {salesData.PeriodEndDate:MM/dd/yyyy})",
            Amount = royaltyAmount
        };
        _invoiceItemRepository.Save(royaltyItem);
        
        // Create RoyaltyInvoiceItem with shared PK
        _royaltyInvoiceItemRepository.Save(new RoyaltyInvoiceItem
        {
            Id = royaltyItem.Id,
            Percentage = feeProfile.RoyaltyFeeSlabs.First().Percentage,
            StartDate = salesData.PeriodStartDate,
            EndDate = salesData.PeriodEndDate,
            Amount = royaltyAmount
        });
        
        // Repeat for AdFundInvoiceItem, NationalInvoiceItem
        
        // 7. Mark upload as invoiced
        salesData.IsInvoiceGenerated = true;
        _salesDataUploadRepository.Save(salesData);
        
        return invoice.Id;
    }
    
    private decimal CalculateRoyaltyWithSlabs(decimal currentSales, decimal ytdSales, IEnumerable<RoyaltyFeeSlab> slabs)
    {
        // Progressive tier calculation:
        // Slab 1: $0 - $100K @ 8%
        // Slab 2: $100K - $500K @ 6%
        // Slab 3: $500K+ @ 4%
        
        var totalRoyalty = 0m;
        var remainingSales = currentSales;
        var cumulativeSales = ytdSales;
        
        foreach (var slab in slabs.OrderBy(s => s.FromAmount))
        {
            var slabStart = Math.Max(slab.FromAmount - cumulativeSales, 0);
            var slabEnd = slab.ToAmount - cumulativeSales;
            var slabRange = Math.Min(remainingSales, slabEnd - slabStart);
            
            if (slabRange > 0)
            {
                totalRoyalty += slabRange * (slab.Percentage / 100);
                remainingSales -= slabRange;
                cumulativeSales += slabRange;
            }
        }
        
        return totalRoyalty;
    }
}
```

### Marketing Class Service
```csharp
[DefaultImplementation]
public class MarketingClassService : IMarketingClassService
{
    // Helper service to retrieve marketing class for invoices/payments
    public string GetMarketingClassByInvoiceId(long invoiceId)
    {
        // Query: FranchiseeSales → MarketingClass
        var franchiseeSale = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(invoiceId);
        return franchiseeSale?.MarketingClass?.Name ?? "";
    }
    
    public string GetSubMarketingClassByInvoiceId(long invoiceId)
    {
        // Returns: "MasterClass:SubClass" (e.g., "Commercial:Office Building")
        var franchiseeSale = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(invoiceId);
        if (franchiseeSale?.SubClassMarketingClass != null)
        {
            return $"{franchiseeSale.SubClassMarketingClass.MasterMarketingClass.Name}:{franchiseeSale.SubClassMarketingClass.Name}";
        }
        return "";
    }
}
```

### Account Credits
```csharp
[DefaultImplementation]
public class AccountCreditService : IAccountCreditService
{
    public void Save(AccountCreditEditModel model)
    {
        var domain = _accountCreditFactory.CreateDomain(model);
        _accountCreditRepository.Save(domain);
    }
    
    public AccountCreditListModel GetList(AccountCreditListFilter filter, int pageNumber, int pageSize)
    {
        // Query: AccountCredit filtered by Customer, DateRange
        // Include: AccountCreditItems
        // Return: Paginated list
    }
}

[DefaultImplementation]
public class AccountCreditFactory : IAccountCreditFactory
{
    public AccountCredit CreateDomain(AccountCreditEditModel model)
    {
        return new AccountCredit
        {
            Id = model.Id,
            CustomerId = model.CustomerId,
            QbInvoiceNumber = model.QbInvoiceNumber,
            CreditedOn = model.CreditedOn,
            CreditMemoItems = model.Items.Select(i => new AccountCreditItem
            {
                Description = i.Description,
                Amount = i.Amount,
                CurrencyExchangeRateId = model.CurrencyExchangeRateId
            }).ToList()
        };
    }
}
```

### Helper Services
```csharp
[DefaultImplementation]
public class DownloadFileHelperService : IDownloadFileHelperService
{
    // Generates Excel/CSV export files for various entities
    public bool DownloadSalesDataFile(SalesDataListFilter filter, out string fileName)
    {
        // Export: SalesDataUpload list with stats
    }
}

[DefaultImplementation]
public class InvoiceFileParserService : IInvoiceFileParserService
{
    // Wrapper for UpdateInvoiceFileParser
    // Used for bulk invoice corrections
}

[DefaultImplementation]
public class InvoiceItemUpdateInfoService : IInvoiceItemUpdateInfoService
{
    // Bulk update invoice items from uploaded correction file
    public void UpdateInvoiceItems(long uploadId)
    {
        // Parse: UpdateInvoiceFileUpload
        // Match: QBInvoiceNumber → InvoiceItem
        // Update: Amount, Description, ServiceType
    }
}

[DefaultImplementation]
public class UpdatingInvoiceNotificationServices : IUpdatingInvoiceNotificationServices
{
    // Sends email notifications when invoices are bulk-updated
}

[DefaultImplementation]
public class UpdatingInvoiceIdsNotificationServices : IUpdatingInvoiceIdsNotificationServices
{
    // Sends email notifications when invoice IDs are corrected
}
```

### Validators
```csharp
[DefaultImplementation]
public class SalesDataUploadCreateModelValidator : ISalesDataUploadCreateModelValidator
{
    public bool Validate(SalesDataUploadCreateModel model, out FeedbackMessageModel message)
    {
        // Check: File not null
        // Check: Date range valid (StartDate < EndDate)
        // Check: No overlapping uploads for franchisee
        // Check: File format (Excel .xlsx)
        // Check: Required columns present
        
        if (/* validation fails */)
        {
            message = FeedbackMessageModel.CreateErrorMessage("Validation error");
            return false;
        }
        
        message = null;
        return true;
    }
}
```

### Helper Models
```csharp
public class ParsedFileParentModel
{
    // Grouped data from single invoice (multiple rows)
    public CustomerCreateEditModel CustomerModel { get; set; }
    public InvoiceEditModel InvoiceModel { get; set; }
    public FranchiseeSalesEditModel FranchiseeSalesModel { get; set; }
    public PaymentModel PaymentModel { get; set; }
}

public class SaveModelStats
{
    // Parsing statistics
    public int NumberOfCustomers { get; set; }
    public int NumberOfInvoices { get; set; }
    public int FailedRecords { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
}

public class InvoiceInfoEditModel
{
    // Internal edit model for invoice corrections
    public long InvoiceId { get; set; }
    public string QBInvoiceNumber { get; set; }
    public List<InvoiceItemEditModel> Items { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Module Dependencies
- **Core.Sales.Domain** - All domain entities (Customer, SalesDataUpload, etc.)
- **Core.Sales.ViewModel** - Edit/List models for UI
- **Core.Sales.Enum** - Status codes, constants
- **Core.Application** - `IRepository<T>`, `IUnitOfWork`, `ILogService`, `IClock`, `ISettings`
- **Core.Application.Impl** - `ISortingHelper`, `IExcelFileCreator`, `ExcelFileParser`
- **Core.Billing** - `IInvoiceService`, `IInvoiceItemService`, `IPaymentService`
- **Core.Organizations** - `IFranchiseeSalesService`, `IFeeProfileFactory`, `IRoyaltyFeeSlabsFactory`
- **Core.Geo** - `IAddressFactory`, `IStateService`
- **Core.Notification** - `INotificationService` for email reminders
- **Core.Scheduler** - `IJobService` for linking estimates to jobs

### External Dependencies
- **FluentValidation** - Validator pattern (via custom `ISalesDataUploadCreateModelValidator`)
- **System.Data** - `DataTable` for Excel parsing
- **NPOI** or **EPPlus** - Excel file reading (via `ExcelFileParser`)

### Cross-Module Data Flow
```
┌──────────────────────────────────────────────────────────────┐
│          Sales Data Upload to Royalty Invoice                 │
└──────────────────────────────────────────────────────────────┘
API Layer
  └─> SalesDataUploadController.Upload()
       └─> SalesDataUploadService.Save()
            ├─> FileService.SaveModel() [Application]
            ├─> SalesDataUploadFactory.CreateDomain()
            └─> Repository<SalesDataUpload>.Save()

Background Job (Quartz.NET)
  └─> SalesDataParsePollingAgent.Execute()
       ├─> ExcelFileParser.ParseExcel() [Application.Impl]
       ├─> SalesDataFileParser.PrepareDomainFromDataTable()
       ├─> CustomerService.SaveCustomer()
       │    ├─> CustomerFactory.CreateDomain()
       │    ├─> AddressFactory.CreateDomain() [Geo]
       │    └─> Repository<Customer>.Save()
       ├─> InvoiceService.Save() [Billing]
       └─> FranchiseeSalesService.Save() [Organizations]

Royalty Calculation
  └─> RoyaltyReportService.GetRoyaltyReport()
       └─> RoyaltyReportFactory.CreateCollectionModel()
  
  └─> SalesFunnelNationalService.GenerateInvoice()
       ├─> Calculate royalty with progressive slabs
       ├─> InvoiceService.Save() [Billing]
       ├─> Repository<RoyaltyInvoiceItem>.Save()
       └─> Update SalesDataUpload.IsInvoiceGenerated = true
```

### Related Documentation
- [Sales/Domain AI-CONTEXT](../Domain/AI-CONTEXT.md) - Entity definitions
- [Sales/Enum AI-CONTEXT](../Enum/AI-CONTEXT.md) - Status codes
- [Billing/Impl](../../Billing/Impl/AI-CONTEXT.md) - Invoice service layer
- [Organizations/Impl](../../Organizations/Impl/AI-CONTEXT.md) - Franchisee services
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Key Gotchas
1. **File Parsing is NOT Transactional**: If parsing fails midway, some customers/invoices may already be saved. Use `Reparse()` to reset.
2. **Case-Insensitive Lookups**: `MarketingClass.Name` matching removes spaces and converts to uppercase. "Comm ercial" matches "COMMERCIAL".
3. **Duplicate Customer Detection**: Uses fuzzy logic (Name + Address). Small variations (e.g., "John Doe" vs "J. Doe") may create duplicates.
4. **Currency Conversion**: Always query `CurrencyExchangeRate` by franchisee country + date. Don't assume latest rate.
5. **Shared PK Pattern**: `RoyaltyInvoiceItem.Id` must be manually assigned from parent `InvoiceItem.Id`. Factory pattern handles this.
6. **Polling Agent Interval**: Configured in `Settings`. Too frequent = DB load, too slow = user frustration.
7. **Excel Column Mapping**: Parser expects exact column names. Case-sensitive, no extra spaces. Mismatch = parse failure.

### Performance Considerations
- **Batch Size**: `SalesDataParsePollingAgent` processes 10 uploads per run (configurable). Prevents timeout on large backlogs.
- **N+1 Query Risk**: Always use `.Include()` for navigation properties (Customer.Address, Invoice.InvoiceItems).
- **Large File Handling**: 10K+ row Excel files can timeout. Consider chunked parsing (future enhancement).
- **Royalty Calculation**: Progressive slab calculation is O(n) where n = number of slabs. Typically 3-5 slabs, negligible impact.

### Historical Context
- **QuickBooks Legacy**: `QbInvoiceNumber` fields are remnants of direct QuickBooks integration (deprecated). Still used for reconciliation.
- **HomeAdvisor Integration**: `CustomerFileParser` was extended to support HomeAdvisor lead CSV format (different column names).
- **Annual Audit Feature**: Added after several franchisees under-reported sales. Compares weekly uploads vs tax filings.

### Testing Notes
- **Unit Tests**: Most factories have unit tests in `Core.Tests`.
- **Integration Tests**: Polling agents require Quartz.NET context, tested via `Jobs` project test suite.
- **Manual Testing**: `ReviewSystemAPITest` console app provides sandbox for file parsing.
<!-- END CUSTOM SECTION -->
