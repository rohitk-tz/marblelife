<!-- AUTO-GENERATED: Header -->
# Core.Organizations
> Franchisee & Staff Management
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module manages the business hierarchy. The primary entity is the **Franchisee**.
It also handles the **Role-Based Access Control (RBAC)** links between Users and Organizations.

### Key Features
-   **Franchisee Management**: Create/Edit Franchisees, assigned Territories, and contact info.
-   **Service Configuration**: Define which services (StoneLife, TileLok) a franchisee is licensed to perform.
-   **Staffing**: Assign Users to Franchisees (e.g., Owner, Technician, Sales Rep).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Fetching a Franchisee
```csharp
var repo = unitOfWork.Repository<Franchisee>();
var franchisee = repo.Get(id);
// Access lazy-loaded properties carefully
var services = franchisee.FranchiseeServices; 
```

### Adding a User to a Franchisee
Use `IOrganizationRoleUserService` (if available) or `OrganizationFactory` to create the `OrganizationRoleUser` link entity.

<!-- END AUTO-GENERATED -->
