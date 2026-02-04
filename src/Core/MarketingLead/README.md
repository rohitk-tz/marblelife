<!-- AUTO-GENERATED: Header -->
# Core.MarketingLead
> Lead Management & HomeAdvisor Integration
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module manages the **Sales Pipeline** before a customer is acquired.
It tracks **Leads** from various sources (Web, Phone, HomeAdvisor) and manages their lifecycle until they convert to a Job or are lost.

### Key Features
-   **Lead Ingestion**: Parsing leads from external emails (HomeAdvisor).
-   **Pipeline Management**: Tracking status (New -> Contacted -> Estimate -> Converted).
-   **Assignment**: Routing leads to the correct Franchisee based on territory.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Converting a Lead
```csharp
var leadService = IoC.Resolve<IMarketingLeadsService>();
leadService.ConvertToCustomer(leadId); 
// This creates a Customer record in Core.Sales
```

<!-- END AUTO-GENERATED -->
