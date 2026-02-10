<!-- AUTO-GENERATED: Header -->
# UpdateInvoiceItemInfo — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
UpdateInvoiceItemInfo is a data correction utility that updates invoice and payment records from an Excel file. It modifies service types and marketing classifications for existing invoices, ensuring payment items match updated invoice items.

### Design Patterns
- **File-Based Update**: Reads correction instructions from Excel file
- **Transaction-per-Record**: Each invoice update wrapped in transaction for rollback safety
- **Multi-Table Update**: Synchronizes FranchiseeSales, InvoiceItem, and PaymentItem records
- **Matching Algorithm**: Uses amount-based matching to link payments to invoice items

### Data Flow
1. Read Excel file path from ISettings.FilePath
2. Parse Excel via `FileParserHelper.ReadExcel()`
3. Convert to `InvoiceInfoEditModel` collection via `InvoiceFileParser`
4. For each record:
   - Update `FranchiseeSales.ClassTypeId` (marketing classification)
   - Update `InvoiceItem.ItemId` (service type)
   - Find matching payments by amount
   - Update `PaymentItem.ItemId` to match invoice
5. Commit transaction or rollback on error
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### InvoiceInfoEditModel
```csharp
public class InvoiceInfoEditModel
{
    public long InvoiceId { get; set; }          // FranchiseeSales.InvoiceId
    public long InvoiceItemId { get; set; }      // InvoiceItem.Id
    public long ServiceTypeId { get; set; }      // New service type
    public long ClassTypeId { get; set; }        // New marketing class
}
```

### Excel File Format
```
InvoiceId | InvoiceItemId | ServiceType | MarketingClass
12345     | 67890         | StoneLife   | Residential
12346     | 67891         | ColorSeal   | Commercial
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `UpdateInvoiceItemInfoService.UpdateReport()`
- **Input**: None (reads file from settings)
- **Output**: void (console logging)
- **Behavior**:
  - Parses Excel file with invoice corrections
  - Updates FranchiseeSales, InvoiceItem, PaymentItem records
  - Uses transaction for rollback safety
  - Logs each update to console
- **Side-effects**: Database updates, console output

### `SaveModel(InvoiceInfoEditModel model)`
- **Input**: Invoice correction model
- **Output**: void
- **Behavior**:
  - Updates marketing class on FranchiseeSales
  - Updates service type on InvoiceItem
  - Finds matching PaymentItems by amount
  - Updates payment service types to match invoice
- **Side-effects**: Multi-table database updates

### `GetTargetSum(long invoiceId, decimal target)`
- **Input**: Invoice ID, target amount
- **Output**: List<FranchiseeSalesPayment>
- **Behavior**:
  - Finds payments that sum to target amount
  - Tries single payment match first
  - Falls back to two-payment combination
- **Side-effects**: None (read-only)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Application](../../Core/Application/.context/CONTEXT.md)** — IUnitOfWork, ISettings, ILogService
- **[Core.Billing](../../Core/Billing/.context/CONTEXT.md)** — InvoiceItem, PaymentItem domains
- **[Core.Sales](../../Core/Sales/.context/CONTEXT.md)** — FranchiseeSales domain
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration

### External
- **EPPlus or similar** — Excel file parsing

### Configuration
Requires `FilePath` in App.config:
```xml
<add key="FilePath" value="file:///D:/corrections.xlsx" />
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Purpose: Data Correction Tool
This utility corrects misclassified invoice and payment records. Common scenarios:
- **Wrong service type**: Invoice recorded as "StoneLife" but was actually "ColorSeal"
- **Wrong marketing class**: Invoice marked "Residential" but was "Commercial"
- **Bulk corrections**: Multiple invoices need same fix

### Excel File Requirements
**Required Columns**:
- `InvoiceId`: FranchiseeSales.InvoiceId (or InvoiceNumber)
- `InvoiceItemId`: InvoiceItem.Id
- `ServiceTypeId`: New service type ID (or name for lookup)
- `ClassTypeId`: New marketing class ID (or name for lookup)

**Example**:
```csv
InvoiceId,InvoiceItemId,ServiceTypeId,ClassTypeId
INV-12345,67890,1,2
INV-12346,67891,3,1
```

### Payment Matching Logic
The service uses amount-based matching to find related payments:

**Strategy 1: Exact Match**
```sql
WHERE InvoiceId = ? AND Payment.Amount = InvoiceItem.Amount
```

**Strategy 2: Sum Match**
```
If single payment not found, try combinations of two payments
that sum to invoice amount
```

This ensures payment records stay synchronized with invoice corrections.

### Transaction Safety
Each invoice update runs in a transaction:
```csharp
_unitOfWork.StartTransaction();
try {
    // Update FranchiseeSales, InvoiceItem, PaymentItem
    _unitOfWork.SaveChanges();
} catch {
    _unitOfWork.Rollback();
    // Log error, continue to next record
}
```

Failed updates don't affect other records.

### Common Use Cases

**Case 1: Service Type Correction**
Franchisee recorded service as wrong type:
```
InvoiceItemId: 12345
Wrong: ServiceTypeId = 1 (StoneLife)
Correct: ServiceTypeId = 2 (ColorSeal)
```

**Case 2: Marketing Class Reclassification**
Customer type changed after initial invoice:
```
InvoiceId: INV-67890
Wrong: ClassTypeId = 1 (Residential)
Correct: ClassTypeId = 2 (Commercial)
```

**Case 3: Bulk Historical Corrections**
Audit revealed 100+ invoices misclassified in 2023:
- Create Excel with all corrections
- Run utility once to fix all

### Safety Considerations
1. **Backup First**: Always backup database before running bulk corrections
2. **Test Sample**: Test with 2-3 invoices before full file
3. **Verify Results**: Check database after completion
4. **Log Review**: Check console output for errors

### Limitations
- **No validation**: Assumes Excel data is correct
- **No rollback across records**: Failed record doesn't stop processing
- **Amount matching**: May fail if payment split doesn't match invoice amount
- **No audit trail**: Doesn't log who made changes or when (use database triggers if needed)
<!-- END CUSTOM SECTION -->
