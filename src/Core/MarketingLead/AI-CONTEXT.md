<!-- AUTO-GENERATED: Header -->
# Core.MarketingLead Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:20:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Lead Management / CRM** domain. It tracks potential customers before they become actual customers with jobs. It integrates with external lead sources like **HomeAdvisor**.

### Key Entities
-   `MarketingLead`: A potential sales opportunity.
-   `MarketingLeadStatus`: New, Contacted, Converted, Lost.
-   `HomeAdvisorImport`: Records of leads imported from HomeAdvisor emails/APIs.

### Logic Flow
1.  **Ingestion**: Leads enter via Email Parser (HomeAdvisor) or Manual Entry.
2.  **Assignment**: Leads are routed to Franchisees based on Zip Codes (managed in `Organizations`).
3.  **Conversion**: If a Lead accepts an Estimate, they are converted to a `Customer` (in `Sales`) and a `Job` (in `Scheduler`).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `MarketingLead`: Contains Contact Info, Source (Web, Referral, HomeAdvisor), and Status.
-   `ReviewPushLocationAPI`: Integration with ReviewPush (though this entity seems misplaced here, might be shared).

### Services
-   `IMarketingLeadsService`: CRUD for leads.
-   `IHomeAdvisorParser`: Specific logic to parse HomeAdvisor lead emails.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Sales](../Sales/AI-CONTEXT.md)**: Converted leads become Customers.
-   **[Core.Organizations](../Organizations/AI-CONTEXT.md)**: Leads are owned by Franchisees.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Lead Sources
The logic heavily anticipates **HomeAdvisor** as a primary source. There are dedicated parsers (`HomeAdvisorParser`) likely used by the `Jobs` module to read incoming lead emails.

### Conversion Logic
Pay attention to the `UpdateConvertedLeadsService`. Moving data from a Lead (Transient) to a Customer (Permanent) is a critical transition point that often has data mapping bugs.
<!-- END CUSTOM SECTION -->
