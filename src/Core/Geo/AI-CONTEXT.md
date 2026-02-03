<!-- AUTO-GENERATED: Header -->
# Core.Geo Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:40:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Geographic Data** domain.
It handles physical locations, address normalization, and lookup data for Countries and States.

### Key Entities
-   `Address`: Street, City, State, Zip.
-   `Country` / `State`: Lookup tables.
-   `ZipCode`: Mailing code logic (possibly linked to Territory Assignment in other modules).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `Address`: The workhorse value object (technically an Entity with ID).
-   `ZipCode`: Contains Latitude/Longitude for distance calculations? (Need to check field definitions, but likely).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Users](../Users/AI-CONTEXT.md)**: People have Addresses.
-   **[Core.Organizations](../Organizations/AI-CONTEXT.md)**: Franchisees have Service Areas defined by Zip Codes.

<!-- END AUTO-GENERATED -->
