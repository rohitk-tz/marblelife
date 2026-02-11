<!-- AUTO-GENERATED: Header -->
# Users/Domain — Domain Entities Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-10T15:35:17Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Domain folder contains all entity classes that represent the core business objects for user management, authentication, and contact information. These entities map directly to database tables and form the persistence layer for the Users module. Each entity extends `DomainBase` (provides Id, IsNew tracking) and uses Entity Framework attributes for relationship mapping.

### Design Patterns
- **Domain Model Pattern**: Rich domain entities with behavior (e.g., `Phone.Create()` factory method, `Person.FullNameUser` computed property)
- **Value Object Pattern**: `Name` value type encapsulates first/middle/last name as immutable composite
- **Entity Relationship Mapping**: Foreign key attributes define relationships (`[ForeignKey("UserId")]`)
- **Shared Primary Key Pattern**: `UserLogin` and `Person` share the same ID (1:1 relationship), `SalesRep` shares ID with `OrganizationRoleUser`

### Entity Relationships
```
Person (1:1) UserLogin
Person (1:M) Address
Person (1:M) Phone
Person (1:M) EmailSignatures
Person (1:1) File (profile image)
Person (M:N) Organization via OrganizationRoleUser

UserLogin (1:M) UserLog (audit trail)

OrganizationRoleUser (1:1) SalesRep (for sales role)
OrganizationRoleUser (1:M) OrganizationRoleUserFranchisee (multi-franchisee access)

Customer (1:M) CustomerLog (customer portal sessions)
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Entity Definitions

### Person.cs
**Purpose**: Represents an individual person with contact information and optional login credentials.

```csharp
public class Person : DomainBase
{
    // Identity
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string Email { get; set; }
    
    // Associated data
    public long DataRecorderMetaDataId { get; set; }  // Audit metadata
    public long? FileId { get; set; }  // Profile image
    public bool IsRecruitmentFeeApplicable { get; set; }  // HR/payroll flag
    public string CalendarPreference { get; set; }  // UI preference
    
    // Relationships
    public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    public virtual File File { get; set; }
    public virtual UserLogin UserLogin { get; set; }  // Optional: null for contacts without login
    public virtual ICollection<Address> Addresses { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
    
    // Computed properties
    [NotMapped]
    public Name Name { get; }  // Value object: FirstName, MiddleName, LastName
    
    [NotMapped]
    public string FullNameUser { get; }  // Formatted: "First Middle Last"
}
```

**Key Behaviors**:
- `Name` property returns immutable value object for structured name representation
- `FullNameUser` provides display-ready full name
- Cascade delete configured for Addresses and Phones via `[CascadeEntity]` attribute

### UserLogin.cs
**Purpose**: Authentication credentials and account status for a Person. Shares primary key with Person (1:1 relationship).

```csharp
public class UserLogin : DomainBase
{
    [ForeignKey("Person")]
    public override long Id { get; set; }  // Same ID as Person
    
    // Credentials
    public string UserName { get; set; }  // Always lowercase, unique
    public string Password { get; set; }   // Salted hash, never plaintext
    public string Salt { get; set; }       // Unique per user
    
    // Account status
    public bool IsLocked { get; set; }     // True after 5 failed attempts
    public bool IsActive { get; set; }     // Defaults to true
    public int? LoginAttemptCount { get; set; }  // Increments on failure, resets on success
    public DateTime? LastLoggedInDate { get; set; }
    
    // Password reset
    public string ResetToken { get; set; }       // GUID for reset link
    public DateTime? ResetTokenIssueDate { get; set; }  // Used to enforce 24-hour expiration
    
    // Timezone tracking
    public double? ESTOffset { get; set; }  // Offset from UTC during EST
    public double? EDTOffset { get; set; }  // Offset from UTC during EDT
    
    // Relationships
    public virtual Person Person { get; set; }
    
    // Constants
    public const int MaxAttempts = 5;  // Lock threshold
}
```

**Key Behaviors**:
- Constructor sets `IsActive = true` by default
- `MaxAttempts` constant defines lockout threshold
- Reset token workflow: issue date + 1440 minutes (24 hours) = expiration

### Role.cs
**Purpose**: System-defined user roles (SuperAdmin, FranchiseeAdmin, Technician, etc.).

```csharp
public class Role : DomainBase
{
    public string Name { get; set; }    // Display name: "Super Admin"
    public string Alias { get; set; }   // Code name: "SuperAdmin"
    public int AccessOrder { get; set; } // Hierarchy/priority (lower = higher privilege)
}
```

**Usage**: Roles are seeded data, referenced by `RoleType` enum. `AccessOrder` determines permission hierarchy.

### Phone.cs
**Purpose**: Phone number entity with parsing and formatting logic.

```csharp
public class Phone : DomainBase
{
    public long TypeId { get; set; }      // PhoneType: Cell, Office, Home, Fax
    public string CountryCode { get; set; } // Default "1" (US/Canada)
    public string AreaCode { get; set; }    // 3 digits
    public string Number { get; set; }      // 7 digits (no area code)
    public string Extension { get; set; }   // Optional
    public bool IsTransferable { get; set; }
    
    // Relationships (many-to-many)
    public virtual ICollection<Person> Persons { get; set; }
    public virtual ICollection<Organization> Organizations { get; set; }
    public virtual Lookup Lookup { get; set; }  // Phone type lookup
    
    // Factory method
    public static Phone Create(string phone, long phoneType, long id);
}
```

**Key Behaviors**:
- `Create()` factory method parses various formats: "(555) 123-4567 x890", "5551234567", etc.
- Handles extension extraction (looks for "x" or "#" prefix)
- Validates minimum 10 digits and numeric-only after sanitization
- `IsTransferable`: indicates if phone can be assigned to multiple entities

### UserLog.cs
**Purpose**: Audit trail for user login/logout events.

```csharp
public class UserLog : DomainBase
{
    public long UserId { get; set; }
    public DateTime LoggedInAt { get; set; }
    public DateTime? LoggedOutAt { get; set; }  // Null = session still active
    public string SessionId { get; set; }       // GUID for session tracking
    public string DeviceKey { get; set; }
    public string ClientIp { get; set; }
    public string Browser { get; set; }         // User agent parsed
    public string OS { get; set; }              // Operating system
    public string UserAgent { get; set; }       // Raw user agent string
    
    [ForeignKey("UserId")]
    public virtual UserLogin UserLogin { get; set; }
}
```

**Usage**: Created by `IUserLogService.SaveLoginSession()`. Used for security auditing, session validation, and analytics.

### CustomerLog.cs
**Purpose**: Audit trail for customer portal login events (non-staff users).

```csharp
public class CustomerLog : DomainBase
{
    public long? CustomerId { get; set; }
    public long? EstimateCustomerId { get; set; }
    public long? EstimateInvoiceId { get; set; }
    public DateTime LoggedInAt { get; set; }
    public DateTime? LoggedOutAt { get; set; }
    public string SessionId { get; set; }
    public string DeviceKey { get; set; }
    public string ClientIp { get; set; }
    public string Browser { get; set; }
    public bool? IsPostSignature { get; set; }  // Logged in to sign document
    public long? TypeId { get; set; }
    public string Code { get; set; }  // Authentication code (instead of username/password)
    
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
    
    [ForeignKey("EstimateCustomerId")]
    public virtual EstimateInvoiceCustomer EstimateInvoiceCustomer { get; set; }
    
    [ForeignKey("EstimateInvoiceId")]
    public virtual EstimateInvoice EstimateInvoice { get; set; }
}
```

**Usage**: Customers authenticate via code (not username/password). Tracks which estimate/invoice triggered login.

### EmailSignatures.cs
**Purpose**: User-defined email signatures for correspondence.

```csharp
public class EmailSignatures : DomainBase
{
    public long? UserId { get; set; }
    public long? OrganizationRoleUserId { get; set; }
    public string SignatureName { get; set; }  // E.g., "Professional", "Casual"
    public string Signature { get; set; }      // HTML content
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    
    [ForeignKey("UserId")]
    public virtual Person Person { get; set; }
    
    [ForeignKey("OrganizationRoleUserId")]
    public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }
}
```

**Usage**: Users can have multiple signatures. `IsDefault` marks primary signature. HTML content supports rich formatting.

### SalesRep.cs
**Purpose**: Extended profile for users with SalesRep role.

```csharp
public class SalesRep : DomainBase
{
    [Key, ForeignKey("OrganizationRoleUser")]
    public override long Id { get; set; }  // Shares ID with OrganizationRoleUser
    public string Alias { get; set; }
    
    public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }
}
```

**Usage**: 1:1 relationship with `OrganizationRoleUser` when role is SalesRep. Extends base user role with sales-specific data.

### EquipmentUserDetails.cs
**Purpose**: Tracks equipment-specific user accounts (e.g., machinery login).

```csharp
public class EquipmentUserDetails : DomainBase
{
    public long UserId { get; set; }
    public bool IsActive { get; set; }
    public bool IsLock { get; set; }  // Equipment-specific lock (separate from UserLogin.IsLocked)
    
    [ForeignKey("UserId")]
    public virtual Person Person { get; set; }
}
```

**Usage**: Users with Equipment role type. `IsLock` prevents equipment operation without affecting web login.

### OrganizationRoleUserFranchisee.cs
**Purpose**: Maps users to multiple franchisees within an organization.

```csharp
public class OrganizationRoleUserFranchisee : DomainBase
{
    public long OrganizationRoleUserId { get; set; }
    public long FranchiseeId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }  // Default franchisee for user
    
    [ForeignKey("OrganizationRoleUserId")]
    public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }
    
    [ForeignKey("FranchiseeId")]
    public virtual Franchisee Franchisee { get; set; }
}
```

**Usage**: Enables multi-location access. A technician can service multiple franchisees. `IsDefault` determines initial login context.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Domain Dependencies
- **DomainBase** (Core.Application.Domain) — Base class providing Id, IsNew properties
- **Address** (Core.Geo.Domain) — Physical address entity for Person
- **File** (Core.Application.Domain) — File storage entity for profile images
- **DataRecorderMetaData** (Core.Application.Domain) — Audit metadata (created by, modified by, timestamps)
- **Lookup** (Core.Organizations) — Reference data for phone types, etc.
- **Organization, OrganizationRoleUser, Franchisee** (Core.Organizations.Domain) — Organization structure
- **Customer, EstimateInvoice, EstimateInvoiceCustomer** (Core.Sales.Domain) — Customer and sales entities

### Value Objects
- **Name** (Core.Application.ValueType) — Immutable value object for person names

### Attributes
- **CascadeEntity** (Core.Application.Attribute) — Marks navigation properties for cascade operations
- **ForeignKey** (System.ComponentModel.DataAnnotations.Schema) — Specifies foreign key relationships
- **NotMapped** (System.ComponentModel.DataAnnotations.Schema) — Excludes computed properties from database mapping
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Entity Relationship Patterns

**Shared Primary Key (1:1)**:
- `Person` ↔ `UserLogin`: UserLogin.Id = Person.Id (explicit via `[ForeignKey("Person")]` on override)
- `OrganizationRoleUser` ↔ `SalesRep`: SalesRep.Id = OrganizationRoleUser.Id

**Navigation Properties**:
- Virtual properties enable lazy loading (Entity Framework proxy generation)
- Collections initialized in constructors (e.g., `Person()` initializes `Addresses`)

### Data Integrity Rules

1. **Person without UserLogin is valid**: Represents contacts, customers, leads without system access
2. **UserLogin without Person is invalid**: UserLogin must reference existing Person
3. **Phone.IsTransferable**: When true, phone can be shared across multiple Persons/Organizations
4. **EmailSignatures uniqueness**: No database constraint, enforced by application logic (one IsDefault per user)

### Common Queries

```csharp
// Get Person with all related data
var person = repository.Get(
    predicate: p => p.Id == userId,
    include: p => p.Addresses,
             p => p.Phones,
             p => p.UserLogin,
             p => p.File
);

// Check if UserLogin exists for Person
var hasLogin = person.UserLogin != null;

// Get active user sessions
var activeSessions = userLogRepository.Fetch(
    predicate: ul => ul.LoggedOutAt == null,
    orderBy: ul => ul.LoggedInAt.Descending()
);
```

### Audit Trail

- **UserLog**: Staff authentication events, session management
- **CustomerLog**: Customer portal access, document signing workflows
- Both track IP, browser, device for security analysis

### Performance Considerations

- **Lazy Loading**: Virtual navigation properties load on first access. Use `.Include()` to eager load.
- **Phone Parsing**: `Phone.Create()` uses regex validation—cache results if parsing repeatedly.
- **UserLogin Queries**: Username lookups are case-insensitive (application layer converts to lowercase). Add index on `UserName` column.
<!-- END CUSTOM SECTION -->
