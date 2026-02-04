<!-- AUTO-GENERATED: Header -->
# CustomerDataUpload Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T23:10:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This module handles the **Bulk Import of Customer & Sales Data** from External Excel files.
It acts as a **Polling Agent** (likely triggered by a background job) that:
1.  Finds `SalesDataUpload` records with status "Uploaded".
2.  Parses the associated Excel file using `ExcelFileParser` (in `Core` presumably).
3.  Creates or Updates `Customer` records.
4.  Creates `FranchiseeSales` records.

### Logic Flow
1.  **Poll**: `ParseCustomerData()` finds the next pending file.
2.  **Parse**: Converts Excel rows into `ParsedFileParentModel` (Custom POCO).
    -   Smart Header Mapping (`name`, `type`, `class`).
    -   Extracts Marketing Class and Service Type from the "class" column string (e.g., "Residential - StoneLife").
3.  **Process Rows**:
    -   **Match Customer**: Tries to find existing customer by Email --> Phone --> Name+Address.
    -   **Sales Record**: Creates a `FranchiseeSales` entry linked to the Customer.
4.  **Reporting**: Logs progress to text files and updates the `SalesDataUpload` status (Parsed/Failed).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Input Data
Excel file with column headers (flexible mapping):
-   `name`, `name contact`: Customer Name.
-   `name phone #`: Phone.
-   `name e-mail`: Email (supports comma/semicolon separation).
-   `class`: Combined Marketing/Service info (e.g., "Residential-StoneLife").
-   `name street1`, `name city`, `name state`, `name zip`: Address.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core](../Core/AI-CONTEXT.md)**: Uses `ExcelFileParser`, `ICustomerService`, `IStateService`.
-   **[Infrastructure](../Infrastructure/AI-CONTEXT.md)**: Uses `IRepository<SalesDataUpload>`, `IFileService`.

### External
-   **Excel Parsing Lib**: Likely generic `System.Data.OleDb` or a library wrapped by `ExcelFileParser`.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Resilience
-   **Transaction per Row**: The code uses `_unitOfWork.StartTransaction()` inside the loop for each record. This implies **row-level isolation**. If one row fails, it rolls back *that row only* and continues to the next.
-   **Logging**: Extensive string-based logging (`sb.Append`) which is written to a file at the end.

### Potential Issues
-   **Address Matching**: `CompareAddress` is case-insensitive but sensitive to whitespace/formatting. "Street" vs "St" mismatch could create duplicate customers.
-   **Performance**: Querying `currencyExchangeRate` and `_stateService` inside the loop might be slow for massive files (N+1 query problem).
<!-- END CUSTOM SECTION -->
