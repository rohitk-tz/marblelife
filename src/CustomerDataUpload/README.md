<!-- AUTO-GENERATED: Header -->
# Customer Data Upload
> Bulk Excel Importer for Sales Data
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This polling agent processes "Sales Data Upload" files (Excel). It is typically used by Franchisees to upload their monthly/weekly sales reports if they don't have a direct API integration.

It converts these spreadsheets into:
1.  **Customers**: Creates or updates customer profiles.
2.  **Sales**: Records transactions (`FranchiseeSales`) linked to these customers.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Triggering
This is usually triggered by the **Jobs** service, which likely invokes the `CustomerDataUploadPollingAgent`.

### Input Format
The Excel file must have columns that match the expected headers (case-insensitive fuzzy match):
-   Customer Name (`name`, `contact`)
-   Phone (`name phone #`)
-   Email (`name e-mail`)
-   Address (`street1`, `city`, `state`, `zip`)
-   Class/Type (`type`, `class`)

### Logic
-   **De-duplication**: It attempts to match existing customers by Email, then Phone, then Address.
-   **Isolation**: Each row is processed in its own transaction. One bad row won't fail the whole file.

<!-- END AUTO-GENERATED -->
