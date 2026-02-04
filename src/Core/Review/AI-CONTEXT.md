<!-- AUTO-GENERATED: Header -->
# Core.Review Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:50:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Customer Feedback / Reputation Management** domain.
It integrates with third-party tools (like **ReviewPush**) to send feedback requests to customers after a job is complete and track their ratings.

### Key Entities
-   `CustomerFeedback`: Stores the rating (1-5 stars) and comments.
-   `ReviewPushLocation`: Maps a Franchisee to a location ID in the ReviewPush system.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Sales](../Sales/AI-CONTEXT.md)**: Feedback is linked to Customers.
-   **[Core.Scheduler](../Scheduler/AI-CONTEXT.md)**: Triggers feedback request when Job is done.

<!-- END AUTO-GENERATED -->
