<!-- AUTO-GENERATED: Header -->
# UpdateInvoiceItemInfo
> Bulk invoice and payment record correction utility
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
UpdateInvoiceItemInfo corrects misclassified invoice and payment records using an Excel file of corrections. It's used for data cleanup when service types or marketing classifications were recorded incorrectly.

**Use Cases**:
- Correcting service type errors (e.g., StoneLife recorded as ColorSeal)
- Reclassifying marketing classes (Residential → Commercial)
- Bulk historical data cleanup after audits
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Configuration
```xml
<add key="FilePath" value="file:///D:/corrections.xlsx" />
```

### Excel Format
```
InvoiceId | InvoiceItemId | ServiceTypeId | ClassTypeId
12345     | 67890         | 2             | 1
12346     | 67891         | 3             | 2
```

### Running
```bash
UpdateInvoiceItemInfo.exe
# Output: "updating Invoice# 12345"
#         "Finished Update!"
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `UpdateReport()` | Main entry point; processes Excel file |
| `SaveModel()` | Updates single invoice and related payments |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Unable to update Class/Service for InvoiceItem#"
**Cause**: Invoice or payment record not found, or transaction conflict.  
**Solution**: Verify IDs in Excel match database, check logs for specific error.

### Payment items not updating
**Cause**: Amount matching failed (no payment matches invoice amount).  
**Solution**: Manually update PaymentItems or adjust matching logic.
<!-- END CUSTOM SECTION -->
