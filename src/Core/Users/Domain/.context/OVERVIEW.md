<!-- AUTO-GENERATED: Header -->
# Users/Domain
> Domain entities for user management, authentication, and contact information persistence
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Domain folder contains all entity classes that map to database tables for the Users module. These are the "nouns" of the system—Person, UserLogin, Role, Phone, etc.—representing core business objects with their properties and relationships.

Think of these entities as the blueprint for your database schema. Each class defines what data is stored, how entities relate to each other (1:1, 1:M, M:N), and includes some domain logic (like `Phone.Create()` for parsing phone numbers or `Person.FullNameUser` for formatting names).

### Key Design Principles

**Separation of Identity and Authentication**: 
- `Person` = who you are (name, email, phone)
- `UserLogin` = how you authenticate (username, password)
- This allows contacts in the system without login credentials

**Audit Trail**:
- `UserLog` tracks every staff login/logout with IP, browser, session data
- `CustomerLog` tracks customer portal access for estimates and document signing

**Extensibility**:
- `SalesRep` extends `OrganizationRoleUser` with sales-specific data
- `EquipmentUserDetails` tracks equipment-specific user status
- Pattern: create entity with shared primary key for role-specific extensions
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating Entity Instances

```csharp
// Person with contact info
var person = new Person
{
    FirstName = "Jane",
    LastName = "Smith",
    Email = "jane.smith@example.com",
    CalendarPreference = "week",
    Addresses = new Collection<Address>
    {
        new Address { Line1 = "123 Main St", City = "Columbus", State = "OH", Zip = "43215" }
    },
    Phones = new Collection<Phone>
    {
        Phone.Create("(614) 555-1234", (long)PhoneType.Cell, 0)
    }
};

// UserLogin with shared primary key
var userLogin = new UserLogin
{
    Id = person.Id,  // MUST match Person.Id for 1:1 relationship
    UserName = "jane.smith@example.com",
    Password = "{hashed-password}",  // Never store plaintext
    Salt = "{unique-salt}",
    IsActive = true,
    LoginAttemptCount = 0
};
person.UserLogin = userLogin;

// Save Person (cascade saves UserLogin)
personRepository.Save(person);
```

### Phone Number Parsing

```csharp
// Various input formats—all parse correctly
var phone1 = Phone.Create("(614) 555-1234", (long)PhoneType.Cell, 0);
var phone2 = Phone.Create("6145551234", (long)PhoneType.Office, 0);
var phone3 = Phone.Create("614-555-1234 x890", (long)PhoneType.Office, 0);  // Extension supported

// phone3 result:
// CountryCode = "1"
// AreaCode = "614"
// Number = "5551234"
// Extension = "890"
```

### Working with Names

```csharp
var person = new Person
{
    FirstName = "John",
    MiddleName = "Q",
    LastName = "Public"
};

// Computed property (not stored in database)
string fullName = person.FullNameUser;  // "John Q Public"

// Value object access
Name nameObj = person.Name;
string lastName = nameObj.LastName;  // "Public"
```

### Querying Relationships

```csharp
// Get Person with UserLogin
var person = personRepository.Get(
    predicate: p => p.Id == userId,
    include: p => p.UserLogin
);

if (person.UserLogin != null)
{
    // Person has login credentials
    bool canLogin = person.UserLogin.IsActive && !person.UserLogin.IsLocked;
}

// Get active user sessions
var activeSessions = userLogRepository.Fetch(
    predicate: ul => ul.LoggedOutAt == null,
    orderBy: ul => ul.OrderByDescending(x => x.LoggedInAt)
).ToList();

foreach (var session in activeSessions)
{
    Console.WriteLine($"User {session.UserId} logged in from {session.ClientIp} using {session.Browser}");
}
```

### Email Signatures

```csharp
// Create default signature
var signature = new EmailSignatures
{
    UserId = person.Id,
    SignatureName = "Professional",
    Signature = "<p><strong>Jane Smith</strong><br/>Senior Technician<br/>Email: jane.smith@example.com</p>",
    IsDefault = true,
    IsActive = true
};

emailSignaturesRepository.Save(signature);
```

### Multi-Franchisee Access

```csharp
// Assign user to multiple franchisees
var orgRoleUserId = 123;  // From OrganizationRoleUser table

var assignments = new[]
{
    new OrganizationRoleUserFranchisee
    {
        OrganizationRoleUserId = orgRoleUserId,
        FranchiseeId = 100,
        IsActive = true,
        IsDefault = true  // Primary franchisee
    },
    new OrganizationRoleUserFranchisee
    {
        OrganizationRoleUserId = orgRoleUserId,
        FranchiseeId = 101,
        IsActive = true,
        IsDefault = false
    }
};

foreach (var assignment in assignments)
{
    orgRoleUserFranchiseeRepository.Save(assignment);
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Entity Summary

| Entity | Description | Key Relationships |
|--------|-------------|-------------------|
| `Person` | Individual with contact info | 1:1 UserLogin, 1:M Address, 1:M Phone, 1:1 File |
| `UserLogin` | Authentication credentials | 1:1 Person (shared PK), 1:M UserLog |
| `Role` | System-defined user roles | Referenced by RoleType enum |
| `Phone` | Contact phone number | M:N Person, M:N Organization |
| `UserLog` | Staff login audit trail | M:1 UserLogin |
| `CustomerLog` | Customer portal audit trail | M:1 Customer, M:1 EstimateInvoice |
| `EmailSignatures` | User email signatures | M:1 Person, M:1 OrganizationRoleUser |
| `SalesRep` | Sales rep extended profile | 1:1 OrganizationRoleUser (shared PK) |
| `EquipmentUserDetails` | Equipment user tracking | 1:1 Person |
| `OrganizationRoleUserFranchisee` | Multi-franchisee access | M:1 OrganizationRoleUser, M:1 Franchisee |

## Entity Properties

### Person
- `FirstName`, `LastName`, `MiddleName`, `Email`
- `CalendarPreference` (UI setting)
- `IsRecruitmentFeeApplicable` (HR flag)
- `FileId` (profile image)
- `FullNameUser` (computed: "First Middle Last")

### UserLogin
- `UserName` (lowercase, unique)
- `Password`, `Salt` (hashed credentials)
- `IsLocked` (after 5 failed attempts)
- `IsActive` (account enabled)
- `LoginAttemptCount` (resets on success)
- `LastLoggedInDate`
- `ResetToken`, `ResetTokenIssueDate` (24-hour expiry)
- `ESTOffset`, `EDTOffset` (timezone tracking)

### Phone
- `TypeId` (Cell/Office/Home/Fax)
- `CountryCode`, `AreaCode`, `Number`, `Extension`
- `IsTransferable` (can share across entities)

### UserLog / CustomerLog
- `LoggedInAt`, `LoggedOutAt` (session duration)
- `SessionId` (GUID)
- `ClientIp`, `Browser`, `OS`, `UserAgent`

### EmailSignatures
- `SignatureName`, `Signature` (HTML)
- `IsDefault`, `IsActive`
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Foreign key constraint violation" on Person.Save()
**Cause**: UserLogin.Id doesn't match Person.Id (1:1 shared primary key).
**Solution**: Always set `userLogin.Id = person.Id` before assigning `person.UserLogin = userLogin`.

### Phone.Create() throws InvalidDataProvidedException
**Cause**: Input has less than 10 digits or contains non-numeric characters after sanitization.
**Solution**: Ensure phone string contains at least area code (3 digits) + number (7 digits). Format "(555) 123-4567" is safe.

### Person.UserLogin is null even though user can log in
**Cause**: Lazy loading not triggered, or Include() not used in query.
**Solution**: Use `.Include(p => p.UserLogin)` in repository query, or access `person.UserLogin` property to trigger lazy load.

### Multiple default signatures for same user
**Cause**: No database constraint on IsDefault uniqueness.
**Solution**: Application logic must set existing signatures' `IsDefault = false` before saving new default signature.

### UserLog.LoggedOutAt never set
**Cause**: Application must explicitly call `IUserLogService.EndLoggedinSession()` on logout.
**Solution**: Implement logout endpoint that calls service method to set LoggedOutAt timestamp.

### EquipmentUserDetails.IsLock vs UserLogin.IsLocked confusion
**Cause**: Two separate lock flags for different purposes.
**Solution**: 
- `EquipmentUserDetails.IsLock` = can't operate equipment
- `UserLogin.IsLocked` = can't log into web application
- They can be independent (web access but no equipment, or vice versa)
<!-- END CUSTOM SECTION -->
