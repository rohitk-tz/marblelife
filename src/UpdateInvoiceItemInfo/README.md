<!-- AUTO-GENERATED: Header -->
# Update Invoice Item Info
> Bulk Excel Data Correction Tool
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

A console utility for bulk-updating **Marketing Class** and **Service Types** on existing Invoices and Sales records.

It is used when data has been entered incorrectly (e.g., all "StoneLife" jobs were accidentally recorded as "TileLok") and needs to be mass-corrected via a spreadsheet.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

1.  **Prepare Excel**: Must match the format expected by `InvoiceFileParser` (Columns: `InvoiceId`, `Class`, `Service`, etc.).
2.  **Config**: Update `FilePath` in the `Settings` table (or App.config) to point to the Excel file.
3.  **Run**: Execute the console app. it will print "Updating Invoice#..." for each processed record.

<!-- END AUTO-GENERATED -->
