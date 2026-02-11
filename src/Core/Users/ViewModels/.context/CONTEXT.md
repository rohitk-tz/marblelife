<!-- AUTO-GENERATED: Header -->
# Users/ViewModels — Data Transfer Objects Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-10T15:35:17Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
ViewModels (also called DTOs - Data Transfer Objects) are data containers that shuttle information between layers of the application. They decouple the domain model (database entities) from presentation logic (UI) and API contracts. Each view model is tailored to a specific use case—user registration, login, profile editing, list filtering—containing only the fields needed for that operation.

### Design Patterns
- **Data Transfer Object (DTO) Pattern**: Plain data containers with no business logic, just properties
- **Form Object Pattern**: Edit models (`*EditModel`) map directly to HTML forms with validation attributes
- **Query Object Pattern**: Filter models (`UserListFilter`) encapsulate search criteria for queries
- **Session Object Pattern**: `UserSessionModel` holds authenticated user context for request processing
- **View Model Pattern**: Read-only models (`UserViewModel`) optimized for display with computed/formatted fields

### Model Categories

**Edit Models** (`*EditModel`): Used for create/update operations, bidirectional transformation with domain entities
- `UserEditModel`, `PersonEditModel`, `UserLoginEditModel`, `ChangePasswordEditModel`
- Extend `EditModelBase` (provides DataRecorderMetaData, validation metadata)
- Often contain nested edit models (composition)

**View Models** (`*ViewModel`): Read-only models for display, optimized for UI rendering
- `UserViewModel`, `UserSessionModel`
- May include computed properties, formatted strings, denormalized data
- Marked with `[NoValidatorRequired]` attribute

**Filter Models** (`*Filter`): Query parameters for search/filter operations
- `UserListFilter`
- Contains search text, pagination, sorting, active status flags

**Authentication Models**: Specialized for login/auth workflows
- `LoginAuthenticationModel`, `LoginCustomerAuthenticationModel`
- Include error messaging via `FeedbackMessageModel`

**List Models** (`*ListModel`): Paginated collection wrappers
- `UserListModel` (contains list of UserViewModel + pagination metadata)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## View Model Definitions

### UserEditModel.cs
**Purpose**: Complete user profile for create/update operations including person, login, roles, and organization assignments.

```csharp
public class UserEditModel : EditModelBase
{
    public UserLoginEditModel UserLoginEditModel { get; set; }  // Login credentials
    public PersonEditModel PersonEditModel { get; set; }        // Contact info
    
    // Role and organization assignment
    public long OrganizationId { get; set; }      // Primary organization
    public long RoleId { get; set; }               // Primary role
    public ICollection<long> RoleIds { get; set; } // Multi-role assignment
    public ICollection<long> OrganizationIds { get; set; }  // Multi-org access
    
    // Login settings
    public bool CreateLogin { get; set; }  // Create UserLogin record
    
    // Profile data
    public string Alias { get; set; }
    public string FileName { get; set; }
    public long? FileId { get; set; }  // Profile image
    public string Css { get; set; }     // Profile image CSS
    
    // Flags
    public bool IsExecutive { get; set; }
    public bool IsDefault { get; set; }
    public bool? isImageChanged { get; set; }
    public long? UserId { get; set; }
    public bool IsChanged { get; set; }
    
    // Nested objects
    public FileUploadModel FileUploadModel { get; set; }
    public FrabchiseeDocumentEditModel franchiseeDocument { get; set; }
    public ICollection<long> info { get; set; }
}
```

**Usage**: Sent from UI for user registration/update. Validated by `UserEditModelValidator`. Transformed to domain entities by `PersonFactory`, `UserLoginFactory`.

### PersonEditModel.cs
**Purpose**: Contact information and personal details for a person (with or without login).

```csharp
public class PersonEditModel : EditModelBase
{
    public long PersonId { get; set; }
    public Name Name { get; set; }  // Value object: FirstName, MiddleName, LastName
    public string Email { get; set; }
    
    // File attachment
    public long? FileId { get; set; }
    public string FileName { get; set; }
    
    // Contact info
    public IEnumerable<AddressEditModel> Address { get; set; }
    public IEnumerable<PhoneEditModel> PhoneNumbers { get; set; }
    
    // Flags
    public bool IsActive { get; set; }
    public bool IsRecruitmentFeeApplicable { get; set; }
    
    // Display properties
    public string ColorCodeSale { get; set; }
    public string Color { get; set; }
}
```

**Default Values**: `IsActive = true`, `IsRecruitmentFeeApplicable = false`, empty collections initialized in constructor.

### UserLoginEditModel.cs
**Purpose**: Authentication credentials for create/update operations.

```csharp
public class UserLoginEditModel : EditModelBase
{
    public long Id { get; set; }  // Matches Person.Id (shared PK)
    public string UserName { get; set; }  // Will be converted to lowercase
    
    // Password fields
    public string Password { get; set; }         // Plaintext (will be hashed)
    public string ConfirmPassword { get; set; }  // For validation
    public bool ChangePassword { get; set; }     // Flag to trigger password hash
    
    // Email notification
    public bool SendUserLoginViaEmail { get; set; }  // Triggers credential email
    
    // Account status
    public bool IsLocked { get; set; }
}
```

**Security Note**: Password is sent as plaintext from UI, hashed by `UserLoginFactory` before persistence. Never logs or persists plaintext passwords.

### LoginAuthenticationModel.cs
**Purpose**: Login credentials input with error message container.

```csharp
public class LoginAuthenticationModel : EditModelBase
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DeviceKey { get; set; }
    public string Code { get; set; }  // For customer authentication
    
    public FeedbackMessageModel Message { get; set; }  // Validation error messages
}
```

**Error Messages**: Populated by `LoginAuthenticationModelValidator`:
- "Invalid credentials"
- "Account locked"
- "User deactivated"
- "Invalid credentials - 2 attempts remaining"

### ChangePasswordEditModel.cs
**Purpose**: Password reset request with token validation.

```csharp
[NoValidatorRequired]
public class ChangePasswordEditModel
{
    public string Key { get; set; }      // Reset token (GUID)
    public string Password { get; set; }  // New password (plaintext, will be hashed)
}
```

**Usage**: Sent from password reset form. `Key` is GUID from email link. Validated and processed by `PasswordResetService`.

### UserViewModel.cs
**Purpose**: Read-only user display model with role and organization context.

```csharp
[NoValidatorRequired]
public class UserViewModel
{
    public long UserId { get; set; }
    public long OrganizationRoleUserId { get; set; }
    public string UserName { get; set; }
    public Name Name { get; set; }
    public string Email { get; set; }
    
    // Role context
    public long RoleId { get; set; }
    public string Role { get; set; }  // Role display name
    
    // Organization context
    public long? FranchiseeId { get; set; }
    public string FranchiseeName { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LastLoggedIn { get; set; }
    public DateTime CreatedOn { get; set; }
    
    // Contact info
    public AddressViewModel Address { get; set; }
    public IEnumerable<object> PhoneNumbers { get; set; }  // Generic collection
}
```

**Usage**: Created by `UserFactory.CreateViewModel()`. Used in user lists, detail pages.

### UserSessionModel.cs
**Purpose**: Authenticated user session context with role, organization, and preferences.

```csharp
public class UserSessionModel
{
    // User identity
    public long UserId { get; set; }
    public Name Name { get; set; }
    
    // Organization context
    public long OrganizationRoleUserId { get; set; }
    public long OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public long OrganizationType { get; set; }
    public long? LoggedInOrganizationId { get; set; }
    public bool IsDefault { get; set; }
    
    // Role context
    public long RoleId { get; set; }           // [XmlIgnore]
    public string RoleAlias { get; set; }      // e.g., "SuperAdmin"
    public string RoleName { get; set; }       // e.g., "Super Admin"
    public int AccessOrder { get; set; }       // [XmlIgnore]
    
    // Session data
    public DateTime? LastLoginAt { get; set; }
    public string CurrentTime { get; set; }
    public string TimeZoneId { get; set; }
    
    // Localization
    public string CurrencyCode { get; set; }
    public decimal CurrencyExchangeRate { get; set; }
    public bool IsTodayCurrencyExchangeRate { get; set; }
    
    // UI preferences
    public string FileName { get; set; }      // Profile image
    public string TeamFileName { get; set; }
    public string Css { get; set; }
    public long TodayToDoCount { get; set; }
    public string Signature { get; set; }
    
    // Customer portal context (when applicable)
    public long? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public long EstimateInvoiceId { get; set; }
    public long EstimateCustomerId { get; set; }
    public bool IsSigned { get; set; }
    public long? SchedulerId { get; set; }
    public long? JobOrginialSchedulerId { get; set; }
    public bool? IsPostSignature { get; set; }
    public long? TypeId { get; set; }
}
```

**Usage**: Stored in server-side session or returned in JWT token. Contains all context needed to render UI and authorize operations.

### UserListFilter.cs
**Purpose**: Search/filter criteria for user list queries.

```csharp
[NoValidatorRequired]
public class UserListFilter
{
    // Status filters
    public bool? IsActive { get; set; }  // Null = all, true = active only, false = inactive only
    public long? StatusId { get; set; }
    
    // Search text
    public string Text { get; set; }      // General search (name, email, username)
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    
    // Context filters
    public long FranchiseeId { get; set; }  // Organization scope
    public long RoleId { get; set; }         // Role filter
    public bool IsFrontOfficeExecutive { get; set; }
    
    // Sorting
    public string SortingColumn { get; set; }  // e.g., "LastName", "Email", "CreatedDate"
    public long? SortingOrder { get; set; }    // 0 = Asc, 1 = Desc (SortingOrder enum)
}
```

**Usage**: Passed to `UserService.GetUsers()` for filtered, paginated results.

### UserListModel.cs
**Purpose**: Paginated list wrapper containing users and pagination metadata.

```csharp
public class UserListModel
{
    public IEnumerable<UserViewModel> Users { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
```

**Usage**: Returned by `UserService.GetUsers()`. Contains users for current page + metadata for pagination controls.

### OrganizationOwnerEditModel.cs
**Purpose**: Organization owner profile for franchisee/organization creation.

```csharp
public class OrganizationOwnerEditModel : EditModelBase
{
    public long OwnerId { get; set; }
    public string OwnerFirstName { get; set; }
    public string OwnerLastName { get; set; }
    public string Password { get; set; }
    public bool SendUserLoginViaEmail { get; set; }
    public bool IsRecruitmentFeeApplicable { get; set; }
    // DataRecorderMetaData inherited from EditModelBase
}
```

**Usage**: Used when creating organization and owner simultaneously. Transformed by `PersonFactory.CreateDomain(OrganizationOwnerEditModel, UserLogin, email)`.

### PersonEquipmentEditModel.cs, UserEquipmentEditModel.cs
**Purpose**: Specialized edit models for equipment users (machinery/automated systems).

**Usage**: Equipment users have limited profile fields and different workflow than staff users.

### UserSignatureEditModel.cs
**Purpose**: Email signature management.

```csharp
public class UserSignatureEditModel
{
    public long Id { get; set; }
    public string SignatureName { get; set; }  // e.g., "Professional", "Casual"
    public string Signature { get; set; }      // HTML content
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
```

**Usage**: Created/updated via `UserService.SaveUserSignature()`.

### LoginCustomerAuthenticationModel.cs
**Purpose**: Customer portal authentication (code-based, not username/password).

```csharp
public class LoginCustomerAuthenticationModel : EditModelBase
{
    public string Code { get; set; }  // Authentication code from email/SMS
    public FeedbackMessageModel Message { get; set; }
}
```

**Usage**: Validated by `LoginAuthenticationModelValidator.IsValidForCustomer()`.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Application.ViewModel** — `EditModelBase` (base class with DataRecorderMetaData, validation metadata)
- **Core.Application.ValueType** — `Name` value object (FirstName, MiddleName, LastName)
- **Core.Geo.ViewModel** — `AddressEditModel`, `AddressViewModel` for contact addresses
- **Core.Scheduler.ViewModel** — `FileUploadModel` for profile image uploads

### Attributes
- **`[NoValidatorRequired]`** — Marks view models that don't need FluentValidation validators (read-only models, filters)
- **`[XmlIgnore]`** — Excludes properties from XML serialization in `UserSessionModel`

### External Dependencies
- **System.Xml.Serialization** — For XML serialization attributes in `UserSessionModel`
- **System.Collections.Generic** — For collection properties (IEnumerable, ICollection)
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### View Model Design Principles

**1. Single Responsibility**: Each view model serves one use case
- `UserEditModel` = user registration/profile update form
- `LoginAuthenticationModel` = login form submission
- `UserViewModel` = user display in lists/details
- `UserListFilter` = user list search/filter criteria

**2. Composition Over Inheritance**: Edit models compose child edit models
- `UserEditModel` contains `PersonEditModel` + `UserLoginEditModel`
- Mirrors UI forms with nested sections (personal info, login credentials, role assignment)

**3. Bidirectional Transformation**: Edit models transform to/from domain entities
- ViewModel → Domain: via Factories (`PersonFactory.CreateDomain()`)
- Domain → ViewModel: via Factories (`PersonFactory.CreateModel()`)

**4. Validation-Friendly**: Properties designed for validation attributes and FluentValidation rules
- `Password` + `ConfirmPassword` for password match validation
- `IsActive` for conditional validation (e.g., require email if active)

### Common Patterns

**Edit Model Initialization**:
```csharp
// Always initialize collections in constructor
public PersonEditModel()
{
    PhoneNumbers = new List<PhoneEditModel>();
    Address = new List<AddressEditModel>();
    Name = new Name();
    IsActive = true;  // Set sensible defaults
}
```

**Session Context Access**:
```csharp
// UserSessionModel typically stored in HTTP session or JWT
var session = HttpContext.Session.Get<UserSessionModel>("UserSession");
long currentUserId = session.UserId;
long currentOrgId = session.OrganizationId;
bool isSuperAdmin = session.RoleId == (long)RoleType.SuperAdmin;
```

**Filter Model Usage**:
```csharp
// UI builds filter from query parameters or form inputs
var filter = new UserListFilter
{
    Text = "john",  // Search across name, email, username
    FranchiseeId = currentFranchiseeId,
    IsActive = true,  // Active users only
    RoleId = (long)RoleType.Technician,
    SortingColumn = "LastName",
    SortingOrder = (long)SortingOrder.Asc
};

var result = userService.GetUsers(filter, pageNumber: 1, pageSize: 25);
```

### Security Considerations

**Password Handling**:
- Passwords are plaintext in edit models (client → server transmission)
- ALWAYS use HTTPS/TLS for transport encryption
- Factories hash passwords immediately upon receipt
- Never log or persist plaintext passwords
- `ConfirmPassword` field never reaches database (validation only)

**Session Model Sensitivity**:
- `UserSessionModel` contains authorization context (role, organization)
- Tampering with session data = privilege escalation
- Use server-side session storage or signed/encrypted JWTs
- Don't trust client-provided `RoleId` or `OrganizationId`—validate against database

**Email Uniqueness**:
- `PersonEditModel.Email` validated by `UniqueEmailValidator`
- Case-insensitive uniqueness (converted to lowercase before check)
- Pass existing `PersonId` when updating to exclude self from uniqueness check

### Performance Notes

**View Model Overhead**: View models add memory overhead (duplicate data in memory). Trade-off for decoupling and flexibility.

**Nested Collections**: `PersonEditModel` with many addresses/phones can be large. Consider lazy loading or pagination for bulk operations.

**UserSessionModel Size**: Contains 30+ properties. Evaluate what's truly needed in every request vs. load-on-demand from database.

### Testing Strategies

**Model Mapping Tests**:
```csharp
[Test]
public void PersonFactory_CreateModel_MapsAllProperties()
{
    var domain = new Person { FirstName = "John", Email = "john@example.com", /* ... */ };
    var model = personFactory.CreateModel(domain);
    
    Assert.AreEqual(domain.FirstName, model.Name.FirstName);
    Assert.AreEqual(domain.Email, model.Email);
    // Verify all properties mapped
}
```

**Validation Tests**:
```csharp
[Test]
public void UserEditModel_MissingEmail_FailsValidation()
{
    var model = new UserEditModel { /* PersonEditModel.Email = null */ };
    var validator = new UserEditModelValidator();
    var result = validator.Validate(model);
    
    Assert.IsFalse(result.IsValid);
    Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PersonEditModel.Email"));
}
```
<!-- END CUSTOM SECTION -->
