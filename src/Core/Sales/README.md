<!-- AUTO-GENERATED: Header -->
# Core.Sales
> Data Import & Customer CRM
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module governs **Bulk Sales Data Imports** and **Customer Management**.
It acts as the ingestion engine where Franchisees report their numbers.

### Key Features
-   **Sales Uploads**: Tracking status of Excel file uploads (Parsed, Failed, InProgress).
-   **Customer Database**: Storing end-user homeowner details.
-   **Marketing Classification**: Categorizing sales (Residential, Commercial, etc.).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Looking up a Customer
```csharp
var customerService = IoC.Resolve<ICustomerService>();
var customer = customerService.GetCustomerByEmail("john@example.com");
```

### Checking Upload Status
```csharp
var upload = repo.Get<SalesDataUpload>(id);
if(upload.Status == SalesDataUploadStatus.Failed) {
   // Check Logs
}
```

<!-- END AUTO-GENERATED -->
