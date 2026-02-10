<!-- AUTO-GENERATED: Header -->
# Users/Enum — Enumerations Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-10T15:35:17Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Defines strongly-typed enumerations for user management domain, providing type safety and preventing magic numbers in code. These enums standardize role types, phone classifications, and sort order conventions across the Users module.

### Design Patterns
- **Enumeration Pattern**: Integer-backed enums for database storage and type-safe code references
- **Lookup Table Pattern**: Enum values map to database Lookup table records for display names and metadata
- **Convention-Based Mapping**: Enum names match database Role.Alias values, int values match Role.Id/Lookup.Id

### Usage Context
- **RoleType**: Authorization logic, user role assignment, access control checks
- **PhoneType**: Contact information categorization, display formatting, communication preferences
- **SortingOrder**: Query result ordering, user list sorting, grid column sorting
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Enumeration Definitions

### RoleType.cs
**Purpose**: System-defined user roles for authorization and access control.

```csharp
public enum RoleType
{
    SuperAdmin = 1,
    FranchiseeAdmin = 2,
    SalesRep = 3,
    Technician = 4,
    FrontOfficeExecutive = 5,
    OperationsManager = 6,
    Equipment = 7
}
```

**Values & Meanings**:
- `SuperAdmin (1)`: System-wide administrator, highest privilege level, can manage all organizations
- `FranchiseeAdmin (2)`: Administrator for a specific franchisee, can manage users and settings within their organization
- `SalesRep (3)`: Sales representative, manages leads, estimates, and customer interactions
- `Technician (4)`: Field technician, views and completes job assignments, limited administrative access
- `FrontOfficeExecutive (5)`: Office staff, handles scheduling, customer service, administrative tasks
- `OperationsManager (6)`: Operations management role, oversees technicians and job workflows
- `Equipment (7)`: Special role for equipment-based users (machinery, automated systems), limited web access

**Database Mapping**: Maps to `Role` table with matching Id values. `Role.Alias` = enum name (e.g., "SuperAdmin").

**Authorization Hierarchy**: Lower enum values generally indicate higher privilege, but specific permissions are role-based, not purely hierarchical.

### PhoneType.cs
**Purpose**: Categorizes phone numbers by usage type for contact management and communication routing.

```csharp
public enum PhoneType
{
    Office = 1,
    Cell = 2,
    Fax = 3,
    TollFree = 4,
    BusinessDirectory = 5,
    NonFunctional = 6,
    CallCenter = 7
}
```

**Values & Meanings**:
- `Office (1)`: Office landline, primary business contact
- `Cell (2)`: Mobile phone, SMS-capable, preferred for field staff
- `Fax (3)`: Fax number, legacy communication method
- `TollFree (4)`: 1-800 numbers, customer service lines
- `BusinessDirectory (5)`: Public-facing directory listing number
- `NonFunctional (6)`: Deprecated/inactive number, kept for records
- `CallCenter (7)`: Call center routing number

**Database Mapping**: Maps to `Lookup` table with `TypeId = PhoneType` enum value. `Phone.TypeId` foreign key references these lookup records.

**UI Display**: Lookup table contains display names (e.g., "Office", "Mobile") for user-facing labels.

### SortingOrder.cs
**Purpose**: Standardizes ascending/descending sort direction across the application.

```csharp
public enum SortingOrder
{
    Asc = 0,   // Ascending: A-Z, 0-9, oldest-newest
    Desc = 1   // Descending: Z-A, 9-0, newest-oldest
}
```

**Values & Meanings**:
- `Asc (0)`: Ascending order (A→Z, 0→9, oldest first)
- `Desc (1)`: Descending order (Z→A, 9→0, newest first)

**Usage**: Applied in `UserListFilter` and other filter models for controlling query result ordering.

**Convention**: 0 = Asc (default), 1 = Desc (explicit reversal).
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Enum Usage Patterns

### Checking User Roles

```csharp
// Check if user has specific role
var organizationRoleUsers = _organizationRoleUserRepository.Fetch(x => x.UserId == userId && x.IsActive);

bool isSuperAdmin = organizationRoleUsers.Any(x => x.RoleId == (long)RoleType.SuperAdmin);
bool isTechnician = organizationRoleUsers.Any(x => x.RoleId == (long)RoleType.Technician);
bool isExecutive = organizationRoleUsers.Any(x => x.RoleId == (long)RoleType.FrontOfficeExecutive);

// Check for multiple roles
var hasAdminAccess = organizationRoleUsers.Any(x => 
    x.RoleId == (long)RoleType.SuperAdmin || 
    x.RoleId == (long)RoleType.FranchiseeAdmin
);
```

### Phone Type Filtering

```csharp
// Get all cell phones for a person
var cellPhones = person.Phones.Where(p => p.TypeId == (long)PhoneType.Cell).ToList();

// Create phone with specific type
var phone = Phone.Create("(555) 123-4567", (long)PhoneType.Office, 0);

// Exclude non-functional numbers
var activePhones = person.Phones
    .Where(p => p.TypeId != (long)PhoneType.NonFunctional)
    .ToList();
```

### Sort Order Application

```csharp
// Apply sort order to user query
var filter = new UserListFilter 
{ 
    SortBy = "LastName",
    SortOrder = SortingOrder.Asc 
};

var query = _personRepository.Table;

if (filter.SortOrder == SortingOrder.Asc)
{
    query = query.OrderBy(p => p.LastName);
}
else
{
    query = query.OrderByDescending(p => p.LastName);
}
```

### Role-Based Authorization

```csharp
// Authorize operation based on role
public bool CanManageUsers(long userId)
{
    var roles = GetUserRoles(userId);
    
    return roles.Any(r => 
        r.RoleId == (long)RoleType.SuperAdmin ||
        r.RoleId == (long)RoleType.FranchiseeAdmin ||
        r.RoleId == (long)RoleType.OperationsManager
    );
}

// Equipment role check
public bool IsEquipmentUser(long userId)
{
    var roles = GetUserRoles(userId);
    return roles.Any(r => r.RoleId == (long)RoleType.Equipment);
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Users.Domain.Role** — Role entity with matching Id values
- **Core.Organizations.Domain.Lookup** — Lookup table for PhoneType display names and metadata
- **Core.Organizations.Domain.OrganizationRoleUser** — Maps users to roles via RoleId foreign key
- **Core.Users.Domain.Phone** — Uses PhoneType via TypeId foreign key

### Database Schema
```sql
-- Role table (seeded data)
Role: Id (int), Name (varchar), Alias (varchar), AccessOrder (int)
-- Example: 1, "Super Admin", "SuperAdmin", 1

-- Lookup table (seeded data)
Lookup: Id (int), Type (varchar), Name (varchar), Value (varchar)
-- Example: 1, "PhoneType", "Office", "Office"
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Role Type Design Decisions

**Why Equipment Role?**
- Equipment users represent machinery/automated systems that need authentication but limited web UI access
- Separate from staff roles to prevent confusion in scheduling and assignment logic
- Equipment-specific locking mechanism via `EquipmentUserDetails.IsLock` independent of `UserLogin.IsLocked`

**Role Hierarchy Is Not Strict**:
- Lower enum values don't automatically grant all permissions of higher values
- SuperAdmin (1) has system-wide access, FranchiseeAdmin (2) is scoped to organization
- Permissions are role-specific, not cumulative (Technician can't do SalesRep tasks despite higher enum value)

### Phone Type Usage Patterns

**SMS Capable Types**: Only `Cell (2)` should be used for SMS notifications.

**Display Priority**: 
1. Cell (primary contact for field staff)
2. Office (business hours contact)
3. TollFree (customer-facing)
4. Fax (legacy, rarely used)

**NonFunctional Status**: 
- Used for deprecated numbers, preserving history without displaying in active contact lists
- Filter out with `.Where(p => p.TypeId != (long)PhoneType.NonFunctional)` in UI queries

### Sort Order Convention

**Default Behavior**: When `SortOrder` not specified, assume `Asc` (0 is default int value).

**Date Sorting**: 
- `Asc` = oldest first (e.g., user created dates: 2020 → 2025)
- `Desc` = newest first (e.g., last login dates: today → 1 year ago)

**Name Sorting**: Always case-insensitive (use `.OrderBy(x => x.Name.ToLower())` or database collation).

### Common Pitfalls

1. **Role Check Without Organization Context**: Users can have different roles in different organizations. Always include `OrganizationId` filter.

```csharp
// BAD: May return roles from other organizations
var isTech = orgRoleUserRepo.Any(x => x.UserId == userId && x.RoleId == (long)RoleType.Technician);

// GOOD: Scoped to current organization
var isTech = orgRoleUserRepo.Any(x => 
    x.UserId == userId && 
    x.OrganizationId == currentOrgId && 
    x.RoleId == (long)RoleType.Technician &&
    x.IsActive
);
```

2. **Assuming Role Hierarchy**: Don't use `>` or `<` comparisons on RoleType enum values for permission checks. Use explicit role checks.

3. **Phone Type Display Names**: Don't hardcode enum names in UI. Fetch display names from Lookup table for internationalization and customization support.

4. **Enum Persistence**: Always cast to `(long)` when comparing with database foreign keys (`RoleId`, `TypeId`). Entity Framework doesn't auto-cast enum to int in all LINQ contexts.
<!-- END CUSTOM SECTION -->
