# UpdateInvoiceItemInfo - Invoice Data Correction Service

## Overview
The UpdateInvoiceItemInfo service is a console application that reads correction data from an Excel file and updates invoice item information in the database. This utility is used to fix data quality issues, update service type classifications, and correct marketing class assignments for historical invoices.

## Purpose
- Correct invoice item classifications
- Update service type assignments
- Fix marketing class categorizations
- Bulk update invoice data
- Maintain data consistency across invoices and payments

## Technology Stack
- **.NET Framework**: C# Console Application
- **Excel Processing**: EPPlus or ClosedXML
- **Database**: Entity Framework Core
- **Dependency Injection**: Custom DI container
- **Logging**: Core.Application.ILogService

## Project Structure
```
/UpdateInvoiceItemInfo
├── UpdateInvoiceItemInfo.csproj           # Project file
├── Program.cs                             # Entry point
├── UpdateInvoiceItemInfoService.cs        # Main service implementation
├── IUpdateInvoiceItemInfoService.cs       # Service interface
├── FileParserHelper.cs                    # Excel file parsing
├── InvoiceFileParser.cs                   # Invoice-specific parsing
├── IInvoiceFileParser.cs                  # Parser interface
├── InvoiceInfoEditModel.cs                # Data model
├── AppContextStore.cs                     # Context management
├── WinJobSessionContext.cs                # Session handling
├── App.config                             # Configuration
└── /Properties
    └── AssemblyInfo.cs
```

## Main Service Implementation

### UpdateInvoiceItemInfoService.cs
```csharp
using Core.Application;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace UpdateInvoiceItemInfo
{
    [DefaultImplementation]
    public class UpdateInvoiceItemInfoService : IUpdateInvoiceItemInfoService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        public readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        public readonly IRepository<PaymentItem> _paymentItemRepository;
        
        public UpdateInvoiceItemInfoService()
        {
            _unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _setting = ApplicationManager.DependencyInjection.Resolve<ISettings>();

            _franchiseeSalesRepository = _unitOfWork.Repository<FranchiseeSales>();
            _invoiceItemRepository = _unitOfWork.Repository<InvoiceItem>();
            _franchiseeSalesPaymentRepository = _unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentItemRepository = _unitOfWork.Repository<PaymentItem>();
        }
        
        public void UpdateReport()
        {
            DataTable data;
            IList<InvoiceInfoEditModel> collection;
            
            try
            {
                // Read Excel file
                var filePath = _setting.FilePath;
                var path = new Uri(filePath).LocalPath;
                data = FileParserHelper.ReadExcel(path);

                // Parse invoice corrections
                var invoiceFileParser = ApplicationManager.DependencyInjection
                    .Resolve<InvoiceFileParser>();
                collection = invoiceFileParser.PrepareDomainFromDataTable(data);

                // Apply updates
                foreach (var record in collection)
                {
                    Console.WriteLine($"Updating Invoice# {record.InvoiceId}");
                    SaveModel(record);
                }
                
                Console.WriteLine("Finished Update!");
                _logService.Info($"Successfully updated {collection.Count} invoices");
            }
            catch (Exception ex)
            {
                _logService.Error($"Error updating invoices: {ex.Message}", ex);
                Console.WriteLine($"ERROR: {ex.Message}");
                throw;
            }
        }

        private void SaveModel(InvoiceInfoEditModel model)
        {
            try
            {
                _unitOfWork.StartTransaction();

                // Update FranchiseeSales (Marketing Class)
                var franchiseeSales = _franchiseeSalesRepository
                    .Get(x => x.InvoiceId == model.InvoiceId);
                    
                if (franchiseeSales != null && model.ClassTypeId > 0)
                {
                    var oldClassTypeId = franchiseeSales.ClassTypeId;
                    franchiseeSales.ClassTypeId = model.ClassTypeId;
                    franchiseeSales.ModifiedDate = _clock.UtcNow;
                    _franchiseeSalesRepository.Save(franchiseeSales);
                    
                    _logService.Debug($"Invoice {model.InvoiceId}: " +
                        $"ClassType changed from {oldClassTypeId} to {model.ClassTypeId}");
                }

                // Update InvoiceItem (Service Type)
                var invoiceItem = _invoiceItemRepository.Get(model.InvoiceItemId);
                if (invoiceItem != null && model.ServiceTypeId > 0)
                {
                    var oldServiceTypeId = invoiceItem.ItemId;
                    invoiceItem.ItemId = model.ServiceTypeId;
                    invoiceItem.ModifiedDate = _clock.UtcNow;
                    _invoiceItemRepository.Save(invoiceItem);
                    
                    _logService.Debug($"InvoiceItem {model.InvoiceItemId}: " +
                        $"ServiceType changed from {oldServiceTypeId} to {model.ServiceTypeId}");

                    // Update related payment items
                    UpdateRelatedPaymentItems(model.InvoiceId, model.ServiceTypeId);
                }

                _unitOfWork.SaveChanges();
                Console.WriteLine($"✓ Invoice {model.InvoiceId} updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error($"Failed to update invoice {model.InvoiceId}", ex);
                Console.WriteLine($"✗ Failed to update invoice {model.InvoiceId}: {ex.Message}");
                throw;
            }
        }
        
        private void UpdateRelatedPaymentItems(string invoiceId, long serviceTypeId)
        {
            // Get all payments for this invoice
            var franchiseeSalesPayments = _franchiseeSalesPaymentRepository.Table
                .Where(x => x.InvoiceId == invoiceId && x.Payment != null)
                .ToList();

            foreach (var fsp in franchiseeSalesPayments)
            {
                // Get payment items for this payment
                var paymentItems = _paymentItemRepository.Table
                    .Where(x => x.PaymentId == fsp.PaymentId)
                    .ToList();

                foreach (var paymentItem in paymentItems)
                {
                    paymentItem.ItemId = serviceTypeId;
                    paymentItem.ModifiedDate = _clock.UtcNow;
                    _paymentItemRepository.Save(paymentItem);
                }
            }
            
            _logService.Debug($"Updated {franchiseeSalesPayments.Count} related payment records");
        }
    }
}
```

## Data Models

### InvoiceInfoEditModel.cs
```csharp
namespace UpdateInvoiceItemInfo
{
    public class InvoiceInfoEditModel
    {
        public string InvoiceId { get; set; }
        public long InvoiceItemId { get; set; }
        public long ServiceTypeId { get; set; }
        public long ClassTypeId { get; set; }
    }
}
```

## Excel File Format

### Expected Columns
```
| Invoice ID | Invoice Item ID | Service Type ID | Class Type ID |
|------------|-----------------|-----------------|---------------|
| INV-12345  | 1001           | 5               | 3             |
| INV-12346  | 1002           | 7               | 2             |
```

### File Parsing

#### FileParserHelper.cs
```csharp
using System;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace UpdateInvoiceItemInfo
{
    public static class FileParserHelper
    {
        public static DataTable ReadExcel(string filePath)
        {
            var dataTable = new DataTable();
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                
                if (worksheet == null)
                {
                    throw new Exception("No worksheet found in Excel file");
                }

                // Read headers from first row
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    var headerValue = worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}";
                    dataTable.Columns.Add(headerValue);
                }

                // Read data rows
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var dataRow = dataTable.NewRow();
                    
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value;
                    }
                    
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
    }
}
```

#### InvoiceFileParser.cs
```csharp
using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Data;

namespace UpdateInvoiceItemInfo
{
    [DefaultImplementation]
    public class InvoiceFileParser : IInvoiceFileParser
    {
        private Dictionary<string, int> _headersDictionary;
        
        public IList<InvoiceInfoEditModel> PrepareDomainFromDataTable(DataTable data)
        {
            var result = new List<InvoiceInfoEditModel>();
            
            // Build header dictionary for column lookup
            BuildHeaderDictionary(data);
            
            // Parse each row
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    var model = ParseRow(row);
                    
                    if (IsValidModel(model))
                    {
                        result.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to parse row - {ex.Message}");
                }
            }
            
            return result;
        }
        
        private void BuildHeaderDictionary(DataTable data)
        {
            _headersDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            for (int i = 0; i < data.Columns.Count; i++)
            {
                var columnName = data.Columns[i].ColumnName.Trim();
                _headersDictionary[columnName] = i;
            }
        }
        
        private InvoiceInfoEditModel ParseRow(DataRow row)
        {
            return new InvoiceInfoEditModel
            {
                InvoiceId = GetCellValue<string>(row, "Invoice ID"),
                InvoiceItemId = GetCellValue<long>(row, "Invoice Item ID"),
                ServiceTypeId = GetCellValue<long>(row, "Service Type ID"),
                ClassTypeId = GetCellValue<long>(row, "Class Type ID")
            };
        }
        
        private T GetCellValue<T>(DataRow row, string columnName)
        {
            if (!_headersDictionary.TryGetValue(columnName, out int index))
            {
                throw new Exception($"Column not found: {columnName}");
            }
            
            var value = row[index];
            
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                throw new Exception($"Cannot convert value '{value}' to type {typeof(T).Name} for column {columnName}");
            }
        }
        
        private bool IsValidModel(InvoiceInfoEditModel model)
        {
            return !string.IsNullOrWhiteSpace(model.InvoiceId) &&
                   model.InvoiceItemId > 0 &&
                   (model.ServiceTypeId > 0 || model.ClassTypeId > 0);
        }
    }
}
```

## Validation

```csharp
private List<string> ValidateUpdates(IList<InvoiceInfoEditModel> collection)
{
    var errors = new List<string>();
    int rowNumber = 2; // Excel row 1 is headers
    
    foreach (var model in collection)
    {
        if (string.IsNullOrWhiteSpace(model.InvoiceId))
            errors.Add($"Row {rowNumber}: Invoice ID is required");
            
        if (model.InvoiceItemId <= 0)
            errors.Add($"Row {rowNumber}: Invoice Item ID must be greater than zero");
            
        if (model.ServiceTypeId <= 0 && model.ClassTypeId <= 0)
            errors.Add($"Row {rowNumber}: At least one update (Service Type or Class Type) is required");
        
        // Verify invoice exists
        var invoice = _franchiseeSalesRepository.Get(x => x.InvoiceId == model.InvoiceId);
        if (invoice == null)
            errors.Add($"Row {rowNumber}: Invoice '{model.InvoiceId}' not found");
            
        // Verify invoice item exists
        var invoiceItem = _invoiceItemRepository.Get(model.InvoiceItemId);
        if (invoiceItem == null)
            errors.Add($"Row {rowNumber}: Invoice Item '{model.InvoiceItemId}' not found");
        
        rowNumber++;
    }
    
    return errors;
}
```

## Logging and Reporting

```csharp
private class UpdateReport
{
    public int TotalRecords { get; set; }
    public int SuccessfulUpdates { get; set; }
    public int FailedUpdates { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public void PrintSummary()
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("UPDATE SUMMARY");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Total Records: {TotalRecords}");
        Console.WriteLine($"Successful: {SuccessfulUpdates}");
        Console.WriteLine($"Failed: {FailedUpdates}");
        Console.WriteLine($"Duration: {(EndTime - StartTime).TotalSeconds:F2} seconds");
        
        if (Errors.Any())
        {
            Console.WriteLine("\nERRORS:");
            foreach (var error in Errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }
        
        Console.WriteLine(new string('=', 50) + "\n");
    }
}
```

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <add key="FilePath" value="file:///C:/Updates/InvoiceCorrections.xlsx" />
    <add key="BackupBeforeUpdate" value="true" />
    <add key="BackupPath" value="C:\Backups\InvoiceUpdates" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Usage

### Program.cs
```csharp
using Core.Application;
using DependencyInjection;
using System;

namespace UpdateInvoiceItemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Invoice Update Service Starting...\n");
                
                // Setup dependency injection
                DependencyRegistrar.RegisterDependencies();
                ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
                ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
                DependencyRegistrar.SetupCurrentContextWinJob();
                
                // Run update service
                var service = ApplicationManager.DependencyInjection
                    .Resolve<IUpdateInvoiceItemInfoService>();
                
                service.UpdateReport();
                
                Console.WriteLine("\nUpdate completed successfully!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(1);
            }
        }
    }
}
```

## Backup Strategy

```csharp
private void BackupAffectedRecords(IList<InvoiceInfoEditModel> collection)
{
    var backupData = new List<dynamic>();
    
    foreach (var model in collection)
    {
        var franchiseeSales = _franchiseeSalesRepository
            .Get(x => x.InvoiceId == model.InvoiceId);
        var invoiceItem = _invoiceItemRepository.Get(model.InvoiceItemId);
        
        if (franchiseeSales != null || invoiceItem != null)
        {
            backupData.Add(new
            {
                InvoiceId = model.InvoiceId,
                InvoiceItemId = model.InvoiceItemId,
                OldServiceTypeId = invoiceItem?.ItemId,
                OldClassTypeId = franchiseeSales?.ClassTypeId,
                BackupDate = DateTime.UtcNow
            });
        }
    }
    
    // Save backup to JSON file
    var backupPath = Path.Combine(
        _setting.BackupPath,
        $"InvoiceBackup_{DateTime.Now:yyyyMMdd_HHmmss}.json"
    );
    
    File.WriteAllText(backupPath, JsonConvert.SerializeObject(backupData, Formatting.Indented));
    _logService.Info($"Backup created: {backupPath}");
}
```

## Error Handling

```csharp
private void ProcessWithErrorHandling(InvoiceInfoEditModel model, UpdateReport report)
{
    try
    {
        SaveModel(model);
        report.SuccessfulUpdates++;
    }
    catch (Exception ex)
    {
        report.FailedUpdates++;
        report.Errors.Add($"Invoice {model.InvoiceId}: {ex.Message}");
        _logService.Error($"Failed to update invoice {model.InvoiceId}", ex);
    }
}
```

## Best Practices

1. **Backup First**: Always create backups before bulk updates
2. **Validation**: Validate all data before processing
3. **Transaction Management**: Use transactions for atomicity
4. **Logging**: Comprehensive logging for audit trail
5. **Dry Run Mode**: Test updates without committing (optional)
6. **Progress Reporting**: Show progress for large batches
7. **Error Recovery**: Continue processing on non-critical errors

## Related Services
- See `/CustomerDataUpload/AI-CONTEXT.md` for data import
- See `/FranchiseeMigration/AI-CONTEXT.md` for migration utilities
- See Core.Billing domain for invoice models
