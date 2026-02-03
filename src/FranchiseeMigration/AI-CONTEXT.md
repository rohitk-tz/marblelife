<!-- AUTO-GENERATED: Header -->
# FranchiseeMigration Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T23:05:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is a **Legacy Data Seeding / Migration Tool**. It hardcodes a massive list of initial Franchisees, their addresses, phone numbers, fee structures, and service offerings directly in C# code.

### Purpose
-   **Initial Population**: It was likely used to populate the database when the system was first launched or migrated from an older system where these franchisees existed.
-   **Static Data**: Unlike user-entered data, this is "Golden Data" for the business, representing all their real-world territories at the time.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Models Used
It reuses ViewModels from **Core.Organizations** (`FranchiseeEditModel`, `FeeProfileEditModel`, `AddressEditModel`).

-   **Fee Profiles**: Defines hardcoded royalty structures (e.g., `FeeProfile1` has different slabs than `FeeProfile2`).
-   **Services**: Maps what services each franchisee offers (e.g., `StoneLife`, `ColorSeal`, `Tilelok`).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core](../Core/AI-CONTEXT.md)** - Uses `Core.Organizations.*` ViewModels massively.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Read-Only Artifact
-   **Do not modify**: This file represents historical data state. Modifying it will not update current production data unless the migration tool is re-run (which might duplicate or crash).
-   **Reference Only**: Useful for looking up the original "default" 2026-era settings for franchisees if the production DB is lost or corrupted.
<!-- END CUSTOM SECTION -->
