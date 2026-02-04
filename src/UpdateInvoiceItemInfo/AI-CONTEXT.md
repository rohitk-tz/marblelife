<!-- AUTO-GENERATED: Header -->
# UpdateInvoiceItemInfo Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T23:20:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is a **Bulk Data Correction Utility**. It reads an Excel file containing invoice updates and applies changes to:
1.  **FranchiseeSales**: Updates `ClassTypeId` (Marketing Class).
2.  **InvoiceItems**: Updates `ItemId` (Service Type).
3.  **PaymentItems**: Propagates the Service Type change to related payments.

### Logic Flow
1.  **Read File**: Uses `FileParserHelper.ReadExcel` to load data.
2.  **Parse**: Converts to `InvoiceInfoEditModel`.
3.  **Transaction**: For each record, opens a transaction.
4.  **Update Sales**: Finds `FranchiseeSales` by Invoice ID and updates `ClassTypeId`.
5.  **Update Invoice**: Finds `InvoiceItem` and updates `ItemId`.
6.  **Heuristic Payment Match**:
    -   It tries to find related payments (`FranchiseeSalesPayment`) that match the `InvoiceItem.Amount`.
    -   It features a **Two-Sum Algorithm** (`GetTargetSum`) to find if two partial payments add up to the target amount.
7.  **Propagate**: Updates `PaymentItem` to match the new `ItemId`.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### InvoiceInfoEditModel
Input model parsed from Excel.
-   `InvoiceId`: Key for lookup.
-   `InvoiceItemId`: Key for item lookup.
-   `ClassTypeId`: New Marketing Class ID.
-   `ServiceTypeId`: New Service Type ID.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core](../Core/AI-CONTEXT.md)**: Uses domain entities (`FranchiseeSales`, `InvoiceItem`) and `UnitOfWork`.
-   **[UpdateInvoiceItemInfo](../UpdateInvoiceItemInfo/AI-CONTEXT.md)**: Self-contained logic for this specific fix.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Complex Payment Logic
The `GetTargetSum` method is interesting. It attempts to find *any pair* of payments that sum up to the invoice item amount.
`list.Add(paymentList[i]); list.Add(paymentList[k]);`
This is a specific heuristic to handle cases where a single invoice item was paid for by split payments.

### Manual Fix Tool
This tool was likely built to correct a specific batch of erroneous data where Invoices were assigned the wrong Class or Service Type. It depends on a specific Excel format defined in `InvoiceFileParser`.
<!-- END CUSTOM SECTION -->
