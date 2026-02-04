# CustomerDataUpload - Sales Data Import and Processing Service

## Overview
The CustomerDataUpload service is a polling agent that monitors for uploaded Excel files containing customer and sales data, parses the files, validates the data, and imports it into the system. This enables franchisees to bulk upload historical sales data and customer information.

## Purpose
- Monitor for new sales data uploads
- Parse Excel files containing customer and sales information
- Validate and normalize data
- Create/update customer records
- Create sales transactions (invoices, payments)
- Handle data transformation and mapping
- Provide detailed error reporting

## Technology Stack
- **.NET Framework**: C# Console Application
- **Excel Processing**: EPPlus or ClosedXML
- **Database**: Entity Framework Core
- **Dependency Injection**: Custom DI container
- **Validation**: FluentValidation
- **Logging**: Core.Application.ILogService

## Project Structure
```
/CustomerDataUpload
├── CustomerDataUpload.csproj              # Project file
├── Program.cs                             # Entry point
├── CustomerDataUploadPollingAgent.cs      # Main service implementation
├── ICustomerDataUploadPollingAgent.cs     # Service interface
├── AppContextStore.cs                     # Context management
├── WinJobSessionContext.cs                # Session handling
├── App.config                             # Configuration
└── /Properties
    └── AssemblyInfo.cs
```

## Main Service Implementation

### CustomerDataUploadPollingAgent.cs (Excerpt)
```csharp
using Core.Application;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CustomerDataUpload
{
    [DefaultImplementation]
    public class CustomerDataUploadPollingAgent : ICustomerDataUploadPollingAgent
    {
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private readonly ILogService _logService;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private IUnitOfWork _unitOfWork;
        private List<ParsedFileParentModel> _parentModelCollection;
        private IStateService _stateService;
        private readonly ICustomerService _customerService;
        private List<MarketingClass> _marketingClasses;
        private List<ServiceType> _serviceTypes;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private IFileService _fileService;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<Customer> _customerRepository;

        public CustomerDataUploadPollingAgent(
            IUnitOfWork unitOfWork, 
            IFileService fileService, 
            ILogService logService, 
            IStateService stateService,
            ICustomerService customerService)
        {
            _logService = logService;
            _stateService = stateService;
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _customerService = customerService;
            _unitOfWork = unitOfWork;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _fileService = fileService;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _customerRepository = unitOfWork.Repository<Customer>();

            // Preload reference data for faster lookups
            _marketingClasses = unitOfWork.Repository<MarketingClass>()
                .Table.ToList()
                .Select(x => {
                    x.Name = x.Name.ToUpper().Replace(" ", "");
                    return x;
                }).ToList();

            _serviceTypes = unitOfWork.Repository<ServiceType>()
                .Table.ToList()
                .Select(x => {
                    x.Name = x.Name.ToUpper().Replace(" ", "");
                    x.Alias = x.Alias;
                    return x;
                }).ToList();
        }
        
        public void ParseCustomerData()
        {
            // Get next file to process
            var salesDataUpload = _salesDataUploadRepository.Table
                .Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded && x.IsActive)
                .OrderBy(x => x.FranchiseeId)
                .OrderBy(x => x.PeriodStartDate)
                .FirstOrDefault();

            if (salesDataUpload == null)
            {
                _logService.Debug("No file found for parsing");
                return;
            }

            // Mark as in progress
            salesDataUpload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
            _salesDataUploadRepository.Save(salesDataUpload);
            _unitOfWork.SaveChanges();

            DataTable data;
            IList<ParsedFileParentModel> collection;
            var sb = new StringBuilder();

            try
            {
                // Read Excel file
                var filePath = MediaLocationHelper.FilePath(
                    salesDataUpload.File.RelativeLocation, 
                    salesDataUpload.File.Name
                ).ToFullPath();
                
                data = ExcelFileParser.ReadExcel(filePath);
                
                // Parse data into domain models
                collection = PrepareDomainFromDataTable(data);
                
                // Validate parsed data
                var validationErrors = ValidateData(collection);
                if (validationErrors.Any())
                {
                    sb.Append(string.Join("\n", validationErrors));
                    throw new ValidationException("Data validation failed");
                }
                
                // Import data
                ImportData(collection, salesDataUpload);
                
                // Mark as completed
                salesDataUpload.StatusId = (long)SalesDataUploadStatus.ParseCompleted;
                salesDataUpload.ParsedDate = _clock.UtcNow;
                _salesDataUploadRepository.Save(salesDataUpload);
                _unitOfWork.SaveChanges();
                
                _logService.Info($"Successfully processed file: {salesDataUpload.File.Name}");
            }
            catch (Exception ex)
            {
                sb.Append(Log("Error occurred in file parsing: " + ex.Message));
                LogException(sb, ex);
                
                // Mark as failed
                salesDataUpload.StatusId = (long)SalesDataUploadStatus.ParseFailed;
                salesDataUpload.ErrorLog = sb.ToString();
                _salesDataUploadRepository.Save(salesDataUpload);
                _unitOfWork.SaveChanges();
                
                _logService.Error($"Failed to process file: {salesDataUpload.File.Name}", ex);
            }
        }
    }
}
```

## Data Models

### ParsedFileParentModel
```csharp
public class ParsedFileParentModel
{
    public CustomerCreateEditModel Customer { get; set; }
    public List<ParsedFileChildModel> Sales { get; set; }
    public long FranchiseeId { get; set; }
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
}

public class ParsedFileChildModel
{
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public long ServiceTypeId { get; set; }
    public long MarketingClassId { get; set; }
    public string Description { get; set; }
    public List<InvoiceItem> Items { get; set; }
    public List<Payment> Payments { get; set; }
}
```

## Excel File Format

### Expected Columns
```
| Customer Name | Email | Phone | Address | City | State | Zip | 
| Invoice # | Invoice Date | Amount | Service Type | Marketing Class | 
| Description | Payment Method | Payment Amount | Payment Date |
```

### File Parsing
```csharp
private IList<ParsedFileParentModel> PrepareDomainFromDataTable(DataTable data)
{
    var result = new List<ParsedFileParentModel>();
    
    // Build header dictionary
    BuildHeaderDictionary(data);
    
    // Group by customer
    var groupedRows = GroupRowsByCustomer(data);
    
    foreach (var customerGroup in groupedRows)
    {
        try
        {
            var model = new ParsedFileParentModel
            {
                Customer = ParseCustomer(customerGroup.First()),
                Sales = customerGroup.Select(ParseSale).ToList(),
                FranchiseeId = GetFranchiseeId(),
                PeriodStartDate = GetPeriodStartDate(),
                PeriodEndDate = GetPeriodEndDate()
            };
            
            result.Add(model);
        }
        catch (Exception ex)
        {
            _logService.Warning($"Failed to parse customer group: {ex.Message}");
        }
    }
    
    return result;
}

private void BuildHeaderDictionary(DataTable data)
{
    _headersDictionary.Clear();
    
    for (int i = 0; i < data.Columns.Count; i++)
    {
        var columnName = data.Columns[i].ColumnName.ToUpper().Trim();
        _headersDictionary[columnName] = i;
    }
}

private CustomerCreateEditModel ParseCustomer(DataRow row)
{
    return new CustomerCreateEditModel
    {
        FirstName = GetCellValue(row, "CUSTOMER NAME")?.Split(' ').FirstOrDefault() ?? "",
        LastName = GetCellValue(row, "CUSTOMER NAME")?.Split(' ').Skip(1).FirstOrDefault() ?? "",
        Email = GetCellValue(row, "EMAIL"),
        PrimaryPhone = GetCellValue(row, "PHONE"),
        Address = new AddressCreateEditModel
        {
            Street1 = GetCellValue(row, "ADDRESS"),
            City = GetCellValue(row, "CITY"),
            State = GetCellValue(row, "STATE"),
            ZipCode = GetCellValue(row, "ZIP")
        }
    };
}

private ParsedFileChildModel ParseSale(DataRow row)
{
    return new ParsedFileChildModel
    {
        InvoiceNumber = GetCellValue(row, "INVOICE #"),
        InvoiceDate = ParseDate(GetCellValue(row, "INVOICE DATE")),
        TotalAmount = ParseDecimal(GetCellValue(row, "AMOUNT")),
        ServiceTypeId = MapServiceType(GetCellValue(row, "SERVICE TYPE")),
        MarketingClassId = MapMarketingClass(GetCellValue(row, "MARKETING CLASS")),
        Description = GetCellValue(row, "DESCRIPTION")
    };
}

private string GetCellValue(DataRow row, string columnName)
{
    if (_headersDictionary.TryGetValue(columnName.ToUpper(), out int index))
    {
        var value = row[index];
        return value == DBNull.Value ? null : value.ToString()?.Trim();
    }
    return null;
}
```

## Data Validation

```csharp
private List<string> ValidateData(IList<ParsedFileParentModel> collection)
{
    var errors = new List<string>();
    int rowNumber = 2; // Start at 2 (Excel row 1 is headers)
    
    foreach (var parent in collection)
    {
        // Validate customer
        if (string.IsNullOrWhiteSpace(parent.Customer.FirstName))
            errors.Add($"Row {rowNumber}: Customer name is required");
            
        if (string.IsNullOrWhiteSpace(parent.Customer.Email) && 
            string.IsNullOrWhiteSpace(parent.Customer.PrimaryPhone))
            errors.Add($"Row {rowNumber}: Either email or phone is required");
        
        // Validate sales
        foreach (var sale in parent.Sales)
        {
            if (string.IsNullOrWhiteSpace(sale.InvoiceNumber))
                errors.Add($"Row {rowNumber}: Invoice number is required");
                
            if (sale.TotalAmount <= 0)
                errors.Add($"Row {rowNumber}: Amount must be greater than zero");
                
            if (sale.ServiceTypeId == 0)
                errors.Add($"Row {rowNumber}: Invalid service type");
                
            if (sale.InvoiceDate > DateTime.Now)
                errors.Add($"Row {rowNumber}: Invoice date cannot be in the future");
            
            rowNumber++;
        }
    }
    
    return errors;
}
```

## Data Import

```csharp
private void ImportData(IList<ParsedFileParentModel> collection, SalesDataUpload upload)
{
    int customersCreated = 0;
    int customersUpdated = 0;
    int salesCreated = 0;
    
    foreach (var parent in collection)
    {
        _unitOfWork.StartTransaction();
        
        try
        {
            // Find or create customer
            var customer = FindOrCreateCustomer(parent.Customer);
            if (customer.Id == 0)
                customersCreated++;
            else
                customersUpdated++;
            
            // Create sales records
            foreach (var sale in parent.Sales)
            {
                CreateSaleTransaction(customer, sale, parent.FranchiseeId);
                salesCreated++;
            }
            
            _unitOfWork.SaveChanges();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            _logService.Error($"Failed to import customer: {parent.Customer.Email}", ex);
            throw;
        }
    }
    
    _logService.Info($"Import complete: {customersCreated} customers created, " +
                     $"{customersUpdated} updated, {salesCreated} sales created");
}

private Customer FindOrCreateCustomer(CustomerCreateEditModel model)
{
    // Try to find by email or phone
    var customer = _customerRepository.Get(c => 
        c.Email == model.Email || 
        c.PrimaryPhone == model.PrimaryPhone
    );
    
    if (customer == null)
    {
        customer = _customerService.Create(model);
    }
    else
    {
        // Update existing customer
        customer = _customerService.Update(customer.Id, model);
    }
    
    return customer;
}

private void CreateSaleTransaction(Customer customer, ParsedFileChildModel sale, long franchiseeId)
{
    var franchiseeSale = new FranchiseeSales
    {
        CustomerId = customer.Id,
        FranchiseeId = franchiseeId,
        InvoiceId = sale.InvoiceNumber,
        InvoiceDate = sale.InvoiceDate,
        TotalAmount = sale.TotalAmount,
        ClassTypeId = sale.MarketingClassId,
        ServiceTypeId = sale.ServiceTypeId,
        Description = sale.Description,
        CreatedDate = DateTime.UtcNow,
        IsActive = true
    };
    
    _franchiseeSalesRepository.Save(franchiseeSale);
    
    // Create invoice items and payments if provided
    // ... additional logic
}
```

## Currency Conversion

```csharp
private decimal ConvertToBaseCurrency(decimal amount, long countryId, DateTime invoiceDate)
{
    var exchangeRate = _currencyExchangeRateRepository
        .Table
        .Where(x => x.CountryId == countryId && x.Date <= invoiceDate)
        .OrderByDescending(x => x.Date)
        .FirstOrDefault();
    
    if (exchangeRate != null)
    {
        return amount * exchangeRate.Rate;
    }
    
    return amount; // No conversion if rate not found
}
```

## Error Logging

```csharp
private string Log(string message)
{
    _logService.Warning(message);
    return message + Environment.NewLine;
}

private void LogException(StringBuilder sb, Exception ex)
{
    sb.Append("Exception: " + ex.Message + Environment.NewLine);
    sb.Append("Stack Trace: " + ex.StackTrace + Environment.NewLine);
    
    if (ex.InnerException != null)
    {
        sb.Append("Inner Exception: " + ex.InnerException.Message + Environment.NewLine);
    }
    
    _logService.Error("CustomerDataUpload error", ex);
}
```

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <add key="PollingIntervalMinutes" value="5" />
    <add key="MaxRetryAttempts" value="3" />
    <add key="ArchiveProcessedFiles" value="true" />
    <add key="ArchivePath" value="C:\Import\Archive" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Deployment

### Running as Windows Service
```csharp
// Program.cs
static void Main(string[] args)
{
    try
    {
        DependencyRegistrar.RegisterDependencies();
        ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
        ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
        DependencyRegistrar.SetupCurrentContextWinJob();
        
        var agent = ApplicationManager.DependencyInjection
            .Resolve<ICustomerDataUploadPollingAgent>();
        
        while (true)
        {
            agent.ParseCustomerData();
            Thread.Sleep(TimeSpan.FromMinutes(5)); // Poll every 5 minutes
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Service failed: {ex.Message}");
        Environment.Exit(1);
    }
}
```

## Best Practices

1. **Transaction Management**: Use transactions for data integrity
2. **Validation**: Comprehensive validation before import
3. **Error Recovery**: Continue processing on non-critical errors
4. **Logging**: Detailed logging for troubleshooting
5. **Deduplication**: Check for existing records before creating
6. **Normalization**: Standardize data formats
7. **Performance**: Batch operations where possible
8. **Monitoring**: Track success/failure rates

## Related Services
- See `/CalendarImportService/AI-CONTEXT.md` for event import
- See `/UpdateInvoiceItemInfo/AI-CONTEXT.md` for invoice updates
- See `/FranchiseeMigration/AI-CONTEXT.md` for franchisee migration
