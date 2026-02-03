<!-- AUTO-GENERATED: Header -->
# Core.Organizations Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T23:50:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Franchisee Management** and **Tenant Hierarchy** domain. It tracks who the franchisees are, their configuration, their staff, and their sales data.

### Key Entities
-   `Franchisee`: The central Tenant entity. Represents a business unit.
-   `Organization`: The base class for Franchisees and potentially other org types.
-   `OrganizationRoleUser`: A composite entity linking **People** (`Core.Users`) to **Organizations** with a specific **Role**.
    -   *Example*: "John Doe is a Technician at NY Franchise".
-   `FranchiseeSales`: Stores sales transaction headers linked to a Franchisee.

### Design Patterns
-   **Factory Pattern**: `OrganizationFactory`, `FranchiseeFactory` used heavily to convert between Domain Entities, Edit Models, and View Models.
-   **Inheritance**: `Franchisee` inherits from `DomainBase` (standard) but conceptually extends `Organization`.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `Franchisee`: Huge aggregation root. Contains `FeeProfile`, `FranchiseeServices`, `FranchiseeInvoices`, `FranchiseeNotes`.
-   `FranchiseeService`: Mappings of what services (e.g., StoneLife, TileLok) a specific franchisee offers.

### ViewModels
-   `FranchiseeEditModel`: Used for CRUD forms.
-   `FranchiseeInfoViewModel`: Lightweight DTO for dashboard lists.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Users](../Users/AI-CONTEXT.md)**: Links to `Person` via `OrganizationRoleUser`.
-   **[Core.Billing](../Billing/AI-CONTEXT.md)**: `Franchisee` owns `FranchiseeInvoices`.
-   **[Core.Geo](../Geo/AI-CONTEXT.md)**: Addresses and Location data.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Complex Relationships
The "User" concept is split. A "User" (Login) belongs to a "Person" (Identity), who is assigned to an "Organization" (Franchisee) via `OrganizationRoleUser`.
**Warning**: querying "All users for Franchisee X" requires joining `OrganizationRoleUser` -> `Person`.

### Sales Confusion
`FranchiseeSales` is defined here in `Organizations`, NOT in `Sales`. This implies that "Sales" are considered a property of the Organization hierarchy rather than an independent transactional module in this legacy design.
<!-- END CUSTOM SECTION -->
