<!-- AUTO-GENERATED: Header -->
# CustomerDataUpload
> Bulk customer sales data import utility with deduplication and validation
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
CustomerDataUpload automates the process of importing franchisee sales data from Excel files into the Marblelife database. It acts as a data pipeline that validates customer information, prevents duplicate entries, normalizes addresses and phone numbers, and creates auditable sales records linked to specific franchisees and time periods.

Think of it as an intelligent CSV importer that understands your business rules — it knows how to find duplicate customers (even with slight variations in name/address), validates that marketing classes and service types exist, and ensures every sales transaction is properly attributed to the correct franchisee with appropriate currency conversion.

**Key Benefits**:
- **Automated processing**: Runs as a console job, no manual intervention
- **Duplicate prevention**: Multi-strategy matching (email → phone → address)
- **Data validation**: Ensures referential integrity before database writes
- **Error recovery**: Transaction-based processing with detailed error logs
- **International support**: Currency exchange rate application for multi-country franchisees

**Typical Workflow**:
1. Franchisee uploads sales Excel file via web UI
2. File stored in database with status `Uploaded`
3. This console application runs (scheduled task or on-demand)
4. Processes pending uploads sequentially
5. Updates status to `Parsed` (success) or `Failed` (validation errors)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Prerequisites
1. .NET Framework 4.5.2+
2. MySQL database connection
3. Excel files uploaded via Marblelife web UI (stored in `SalesDataUpload` table)

### Configuration

**App.config**:
```xml
<connectionStrings>
  <add name="ConnectionString" 
       connectionString="server=localhost; database=Makalu; User ID=root; Password=Pass@123; Port=3308;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

### Running the Utility

**Manual Execution**:
```bash
CustomerDataUpload.exe
# Processes all pending uploads in SalesDataUpload table
```

**Scheduled Task (Windows)**:
```
Task Scheduler → Create Basic Task
Trigger: Daily at 3:00 AM
Action: Start Program → CustomerDataUpload.exe
```

**Console Output**:
```
Starting data save for Customer John Doe
Found existing Customer by email. Found Records [Id:12345, Name: John Doe]
Validation successful for row 1
Starting data save for Customer Acme Corp
No existing customer found, creating new record
Validation successful for row 2
Parse completed: 15 parsed, 2 failed, 13 customers created/updated
```

### Excel File Format

**Required Columns**:
```
Customer Name | Email OR Phone | Service Type | Amount
```

**Optional Columns**:
```
Address | City | State | Zip | Country | Marketing Class | Sales Rep | QB Invoice#
```

**Example File**:
```
Customer Name   Email              Phone        Service Type  Amount   Marketing Class  Sales Rep
John Doe        john@example.com   555-123-4567 StoneLife     350.00   Residential      Jane Smith
Acme Corp       info@acme.com      555-987-6543 ColorSeal     1250.00  Commercial       Bob Jones
Mary Johnson    mary@example.com   555-234-5678 Tile Lok      480.00   Residential      Jane Smith
```

### Supported Service Types
Must match names in `ServiceType` database table:
- StoneLife
- ColorSeal
- Tile Lok (or TileLok)
- Vinyl Guard
- Enduracrete
- CleanShield
- Wood
- CarpetLife
- Fabricators
- TileInstall

### Supported Marketing Classes
Must match names in `MarketingClass` table:
- Residential
- Commercial
- Builder
- Property Management

### Data Validation Rules
1. **Customer Name**: Required, max 255 characters
2. **Email OR Phone**: At least one required for duplicate detection
3. **Service Type**: Must exist in `ServiceType` table (name or alias)
4. **Marketing Class**: Must exist in `MarketingClass` table (optional)
5. **State**: Must be valid two-letter abbreviation (US states)
6. **Amount**: Must be valid decimal number
7. **QB Invoice #**: Unique within franchisee (optional)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `ParseCustomerData()` | Main entry point; queries pending uploads and processes sequentially |
| `PrepareDomainFromDataTable()` | Converts Excel DataTable to validated domain models |
| `SaveModel()` | Persists single customer record with duplicate detection and sales linkage |
| `SearchCustomer()` | Multi-strategy customer lookup (email → phone → address) |
| `ValidateAddress()` | Geocodes and normalizes address data |
| `ValidateMarketingClass()` | Maps class name to database ID |
| `ValidateServiceType()` | Maps service name/alias to database ID |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Upload Stuck in "ParseInProgress"
**Cause**: Application crashed mid-processing without resetting status.  
**Solution**: Manually update `SalesDataUpload.StatusId` back to `Uploaded` (status ID = 1):
```sql
UPDATE SalesDataUpload 
SET StatusId = 1 
WHERE StatusId = 2 AND Id = <upload_id>;
```

### "No file found for parsing"
**Cause**: No pending uploads in database with `StatusId = Uploaded`.  
**Solution**: 
- Verify file uploaded via web UI
- Check `SalesDataUpload` table for records with `IsActive = true`
- Ensure file path exists in `File.RelativeLocation`

### "Column not found: CustomerName"
**Cause**: Excel file missing required header columns.  
**Solution**: 
- Verify Excel file has header row
- Ensure required columns present (case-insensitive): `CustomerName`, `Email` or `Phone`, `ServiceType`, `Amount`
- Remove hidden characters or extra spaces from headers

### "MarketingClass not found: Residential"
**Cause**: Database value doesn't match normalized name.  
**Solution**:
- Check `MarketingClass` table values
- Ensure names are stored uppercase without spaces: `RESIDENTIAL`
- Or update code to handle different normalization

### "ServiceType not found: Stone Life"
**Cause**: Service name/alias doesn't match database.  
**Solution**:
- Check `ServiceType` table for exact name or alias match
- Add alias to database: `UPDATE ServiceType SET Alias = 'StoneLife' WHERE Name = 'Stone Life'`
- Or use exact name from database in Excel file

### "Duplicate customer created"
**Cause**: Email/phone normalization failed or address too different.  
**Solution**:
- Verify email addresses are valid format (no spaces, proper domain)
- Ensure phone numbers include area code
- Check city/state spelling matches existing records
- Consider manual merge of duplicate customers

### "Currency exchange rate not found"
**Cause**: No exchange rate record for franchisee's country.  
**Solution**:
- Run `ConsoleApplication1` (CurrencyExchangeRateService) to populate rates
- Or manually insert rate into `CurrencyExchangeRate` table for specific country/date

### File parsing errors
**Cause**: Invalid Excel format or corrupted file.  
**Solution**:
- Re-save Excel file as `.xlsx` (not `.xls` or `.csv`)
- Remove formula cells (convert to values)
- Check for merged cells or hidden rows
- Verify file not corrupted (open in Excel to confirm)

### Performance degradation with large files
**Cause**: 5000+ rows cause memory/timeout issues.  
**Solution**:
- Split into multiple smaller files (1000 rows each)
- Increase application timeout in App.config
- Run during off-hours to reduce database load
- Consider batch processing in chunks (code modification)

### Transaction rollback on single error
**Cause**: One invalid row fails entire file.  
**Solution**:
- Fix validation error in Excel file
- Re-upload corrected file
- Or implement row-level error handling (code modification) to skip invalid rows

### Testing Upload Process
**Development Testing**:
1. Create test Excel file with 5-10 sample records
2. Upload via web UI to staging database
3. Run console application manually
4. Verify records in `Customer` and `FranchiseeSales` tables
5. Check logs for validation messages
6. Test duplicate detection by re-uploading same file
<!-- END CUSTOM SECTION -->
