<!-- AUTO-GENERATED: Header -->
# Core.Billing
> Invoices, Payments, & Audit Logs
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module manages the money flow. It handles the lifecycle of **Invoices** (Generation -> Sent -> Paid) and the processing of **Payments** (Credit Card, eCheck, Physical Check).

### Key Features
-   **Invoicing**: Multi-line item invoices (Royalty, supplies, fees).
-   **Payment Processing**: Integration with Payment Gateways (Authorize.Net) via Infrastructure.
-   **Audit Trail**: Strict logging of all financial modifications.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Creating a Payment
1.  Retrieve Unpaid Invoices.
2.  Use `IPaymentFactory` to create a `Payment` domain object.
3.  Use `IPaymentService.ProcessPayment()` (conceptual) which delegates to the Gateway and saves the result.

### Fetching Invoice History
```csharp
var invoices = invoiceRepo.Fetch(x => x.FranchiseeId == id && x.IsPaid);
```

<!-- END AUTO-GENERATED -->
