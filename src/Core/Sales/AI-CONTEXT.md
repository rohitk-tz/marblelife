<!-- AUTO-GENERATED: Header -->
# Core.Sales Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:00:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Data Ingestion & Customer** domain. Despite the name "Sales", it primarily focuses on:
1.  **Importing Sales Data**: From Excel files uploaded by Franchisees.
2.  **Customer Management**: The B2C customers ("Home Owners").
3.  **Reporting**: Generating annual/monthly royalty reports based on ingested data.

### Key Entities
-   `SalesDataUpload`: Represents an uploaded Excel file containing batched sales transactions.
-   `Customer`: The end-client who received the service.
-   `FranchiseeSalesPayment`: Tracks payments related to sales (confusingly split from `FranchiseeSales` in Organizations).
-   `MarketingClass`: Classification of the sale (e.g., Residential vs Commercial).

### Logic Flow
-   **Upload**: Franchisee uploads Excel -> `SalesDataUpload` created.
-   **Process**: Background Job (in `Details` or `CustomerDataUpload`) parses file -> Creates `Customer` & `FranchiseeSales` (in Org module).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `SalesDataUpload`: Metadata about the uploaded file (Status, Parsed Count, Failed Count).
-   `Customer`: Basic CRM info (Name, Email, Phone).
-   `MarketingClass` / `ServiceType` (Enum wrapper): Attributes for tagging sales.

### Services
-   `ICustomerService`: Logic for De-duping and finding customers by Email/Phone.
-   `ISalesDataUploadService`: Manages the lifecycle of the upload process.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Organizations](../Organizations/AI-CONTEXT.md)**: The actual `FranchiseeSales` entity lives there, but this module creates them during import.
-   **[Core.Billing](../Billing/AI-CONTEXT.md)**: `FranchiseeSalesPayment` connects here.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Legacy Design Split
The separation of `FranchiseeSales` (Organizations) and `FranchiseeSalesPayment` (Sales/Billing?) is messy.
Historically, this module seems to have started as just "The Excel Import Tool" and grew to own the Customer entity.

### Customer De-duplication
A critical part of this module is the heuristics used to match uploaded Excel rows to existing Customers to avoid duplicates. This logic resides in `CustomerService` or the Polling Agent.
<!-- END CUSTOM SECTION -->
