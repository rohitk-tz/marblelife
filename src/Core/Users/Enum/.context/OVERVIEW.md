<!-- AUTO-GENERATED: Header -->
# Users/Enum
> Strongly-typed enumerations for user roles, phone types, and sorting conventions
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Enum folder contains strongly-typed enumerations that standardize categorical values across the Users module. Instead of using "magic numbers" like `roleId = 4`, you use `RoleType.Technician`, making code self-documenting and preventing invalid values.

### Why Enumerations?

**Type Safety**: Compiler prevents invalid values. You can't accidentally pass `roleId = 99` when only 1-7 are valid.

**Refactoring Safety**: Renaming `Technician` to `FieldTechnician` updates all references automatically. Renaming database value 4 requires manual SQL updates.

**IntelliSense**: IDE shows available values. Type `RoleType.` and see all options.

**Self-Documenting**: `if (role == RoleType.SuperAdmin)` is clearer than `if (role == 1)`.

### Database Integration

These enums map to database tables:
- **RoleType** → `Role` table (Id = enum value, Alias = enum name)
- **PhoneType** → `Lookup` table (Id = enum value, Type = "PhoneType")
- **SortingOrder** → Not persisted, used for query logic only
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Role-Based Authorization

```csharp
// Check if user is admin
public bool IsAdmin(long userId, long organizationId)
{
    var orgRoleUser = _orgRoleUserRepository.Get(x => 
        x.UserId == userId && 
        x.OrganizationId == organizationId &&
        x.IsActive
    );
    
    return orgRoleUser?.RoleId == (long)RoleType.SuperAdmin ||
           orgRoleUser?.RoleId == (long)RoleType.FranchiseeAdmin;
}

// Get users by role
var technicians = _orgRoleUserRepository.Fetch(x => 
    x.OrganizationId == franchiseeId &&
    x.RoleId == (long)RoleType.Technician &&
    x.IsActive
).ToList();

// Check multiple roles
var adminRoles = new[] 
{ 
    (long)RoleType.SuperAdmin, 
    (long)RoleType.FranchiseeAdmin, 
    (long)RoleType.OperationsManager 
};

var hasAdminAccess = _orgRoleUserRepository.Any(x =>
    x.UserId == userId &&
    adminRoles.Contains(x.RoleId) &&
    x.IsActive
);
```

### Phone Type Management

```csharp
// Create phone with type
var cellPhone = new Phone
{
    TypeId = (long)PhoneType.Cell,
    AreaCode = "614",
    Number = "5551234",
    CountryCode = "1"
};

// Or use factory method
var officePhone = Phone.Create("(614) 555-9999", (long)PhoneType.Office, 0);

// Filter active phone numbers
var activePhones = person.Phones
    .Where(p => p.TypeId != (long)PhoneType.NonFunctional)
    .ToList();

// Get preferred contact method (cell first)
var preferredPhone = person.Phones
    .OrderBy(p => p.TypeId == (long)PhoneType.Cell ? 0 : 1)  // Cell first
    .ThenBy(p => p.TypeId == (long)PhoneType.Office ? 0 : 1) // Office second
    .FirstOrDefault();

// Get SMS-capable numbers only
var smsNumbers = person.Phones
    .Where(p => p.TypeId == (long)PhoneType.Cell)
    .ToList();
```

### Sort Order Application

```csharp
// User list sorting
public UserListModel GetUsers(UserListFilter filter)
{
    var query = _personRepository.Table;
    
    // Apply filter
    if (!string.IsNullOrEmpty(filter.SearchText))
    {
        query = query.Where(p => 
            p.FirstName.Contains(filter.SearchText) ||
            p.LastName.Contains(filter.SearchText)
        );
    }
    
    // Apply sort
    switch (filter.SortBy)
    {
        case "LastName":
            query = filter.SortOrder == SortingOrder.Asc
                ? query.OrderBy(p => p.LastName)
                : query.OrderByDescending(p => p.LastName);
            break;
        
        case "Email":
            query = filter.SortOrder == SortingOrder.Asc
                ? query.OrderBy(p => p.Email)
                : query.OrderByDescending(p => p.Email);
            break;
        
        case "CreatedDate":
            query = filter.SortOrder == SortingOrder.Asc
                ? query.OrderBy(p => p.DataRecorderMetaData.RecordedOn)
                : query.OrderByDescending(p => p.DataRecorderMetaData.RecordedOn);
            break;
    }
    
    return PaginateResults(query, filter.PageNumber, filter.PageSize);
}
```

### Switch Statements with Enums

```csharp
// Role-specific logic
public string GetRoleHomePage(RoleType role)
{
    switch (role)
    {
        case RoleType.SuperAdmin:
        case RoleType.FranchiseeAdmin:
            return "/admin/dashboard";
        
        case RoleType.Technician:
            return "/technician/jobs";
        
        case RoleType.SalesRep:
            return "/sales/leads";
        
        case RoleType.FrontOfficeExecutive:
            return "/office/schedule";
        
        case RoleType.OperationsManager:
            return "/operations/overview";
        
        case RoleType.Equipment:
            return "/equipment/status";
        
        default:
            throw new ArgumentException($"Unknown role type: {role}");
    }
}

// Phone formatting by type
public string FormatPhoneDisplay(Phone phone)
{
    var formatted = $"({phone.AreaCode}) {phone.Number.Substring(0, 3)}-{phone.Number.Substring(3)}";
    
    if (!string.IsNullOrEmpty(phone.Extension))
    {
        formatted += $" x{phone.Extension}";
    }
    
    var typeLabel = ((PhoneType)phone.TypeId) switch
    {
        PhoneType.Office => "Office",
        PhoneType.Cell => "Mobile",
        PhoneType.Fax => "Fax",
        PhoneType.TollFree => "Toll-Free",
        PhoneType.CallCenter => "Call Center",
        _ => "Other"
    };
    
    return $"{formatted} ({typeLabel})";
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Enum Reference

### RoleType
| Value | Name | Description | Access Level |
|-------|------|-------------|--------------|
| 1 | SuperAdmin | System-wide administrator | Highest |
| 2 | FranchiseeAdmin | Organization administrator | High |
| 3 | SalesRep | Sales representative | Medium |
| 4 | Technician | Field technician | Medium |
| 5 | FrontOfficeExecutive | Office staff | Medium |
| 6 | OperationsManager | Operations management | High |
| 7 | Equipment | Equipment/automated system | Limited |

### PhoneType
| Value | Name | Description | SMS Capable |
|-------|------|-------------|-------------|
| 1 | Office | Office landline | No |
| 2 | Cell | Mobile phone | Yes |
| 3 | Fax | Fax number | No |
| 4 | TollFree | 1-800 numbers | No |
| 5 | BusinessDirectory | Public directory listing | No |
| 6 | NonFunctional | Deprecated number | No |
| 7 | CallCenter | Call center line | No |

### SortingOrder
| Value | Name | Description |
|-------|------|-------------|
| 0 | Asc | Ascending (A→Z, 0→9, oldest→newest) |
| 1 | Desc | Descending (Z→A, 9→0, newest→oldest) |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Invalid cast from 'Int32' to 'RoleType'" error
**Cause**: Trying to cast database int directly to enum without explicit cast.
**Solution**: 
```csharp
// BAD: Implicit cast fails
RoleType role = orgRoleUser.RoleId;

// GOOD: Explicit cast
RoleType role = (RoleType)orgRoleUser.RoleId;

// BEST: Compare as long (avoid casting)
if (orgRoleUser.RoleId == (long)RoleType.SuperAdmin) { }
```

### Role check returns false despite user having role
**Cause**: Forgetting to check `IsActive` flag on OrganizationRoleUser.
**Solution**: Always include `x.IsActive` in role queries.

### Phone type dropdown shows numbers instead of names
**Cause**: Displaying enum value instead of fetching Lookup table display name.
**Solution**: Query Lookup table for display names, or use `.ToString()` on enum for basic display.

### User has multiple roles, authorization fails
**Cause**: Checking for specific role without considering multi-role scenarios.
**Solution**: Use `.Any()` instead of single-role checks:
```csharp
// BAD: Assumes single role
var role = (RoleType)orgRoleUsers.First().RoleId;
if (role == RoleType.Technician) { }

// GOOD: Supports multi-role
if (orgRoleUsers.Any(x => x.RoleId == (long)RoleType.Technician)) { }
```

### Enum value not found in database
**Cause**: Database Role/Lookup tables not seeded with enum values.
**Solution**: Run database seed scripts to populate Role and Lookup tables with matching enum values and names.

### Sort order not applying
**Cause**: `SortingOrder` enum value treated as boolean.
**Solution**: Use explicit enum comparison:
```csharp
// BAD: Treats Desc (1) as true, Asc (0) as false
if (sortOrder) { query.OrderByDescending(...); }

// GOOD: Explicit enum comparison
if (sortOrder == SortingOrder.Desc) { query.OrderByDescending(...); }
```
<!-- END CUSTOM SECTION -->
