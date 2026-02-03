<!-- AUTO-GENERATED: Header -->
# Core.Billing Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:05:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Financial Ledger** of the application. It handles **Invoicing** (what is owed) and **Payments** (what is paid). It supports multiple payment methods including Credit Cards (Authorize.Net) and eChecks.

### Key Entities
-   `Invoice`: The request for payment. Can be for Royalty Fees, Supplies, etc.
-   `Payment`: The actual transaction.
-   `PaymentInstrument`: Abstract base for `Check`, `ECheck`, `ChargeCard`.
-   `InvoiceItem`: Line items within an invoice.
-   **Audit Entities**: extensive `AuditInvoice`, `AuditPayment` tables to track history.

### Logic Flow
1.  **Generation**: Invoices are generated (likely by `Jobs` based on Sales Data).
2.  **Payment Processing**:
    -   User selects Invoices.
    -   Selects/Adds Payment Profile (Card/Bank).
    -   System charges Gateway (via `Infrastructure` layer).
    -   Creates `Payment` record and links to `Invoice` via `InvoicePayment`.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `Invoice`: Status (Open, Paid, Void), Due Date, Amount.
-   `FranchiseeInvoice`: Polymorphic extension specifically for Franchisee bills.
-   `ChargeCard` / `ECheck`: Store tokenized or masked payment info.

### Services
-   `IInvoiceService`: Managing invoice lifecycle.
-   `IPaymentService`: Recording payments.
-   `IChargeCardService`: Managing stored cards.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Infrastructure](../Infrastructure/AI-CONTEXT.md)**: Heavily relies on `AuthorizeNetCustomerProfileService` for actual Gateway calls.
-   **[Core.Organizations](../Organizations/AI-CONTEXT.md)**: Invoices are usually billed to a `Franchisee`.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Complex Inheritance
The `PaymentInstrument` hierarchy (`ChargeCard` : `PaymentInstrument`) is a classic OO pattern.
Be careful with **Polymorphic Queries**.

### Soft Deletes & Audits
This module has the most extensive Audit trailing (`AuditInvoice`, etc.).
Almost every financial change is recorded in a shadow table.

### Currency
Includes `CurrencyExchangeRate` entity, implying multi-currency support (likely CAD/USD given the Franchisee locations in Canada).
<!-- END CUSTOM SECTION -->
