<!-- AUTO-GENERATED: Header -->
# Users Module — Core Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-10T15:35:17Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Users module manages the complete user lifecycle including authentication, authorization, person/contact management, and role-based access control. It enforces security policies through password hashing with salts, implements account lockout after failed login attempts (5 max), and manages password reset workflows with time-limited tokens (24-hour expiration). The module distinguishes between Person entities (contact information) and UserLogin entities (authentication credentials), enabling separation of identity from authentication concerns.

### Design Patterns
- **Factory Pattern**: `IUserFactory`, `IPersonFactory`, `IUserLoginFactory` - Encapsulate complex object creation logic, especially for password hashing during UserLogin creation
- **Service Layer Pattern**: Services (`IUserService`, `IUserLoginService`, `IPasswordResetService`) coordinate between repositories, factories, and validators to implement business logic
- **Repository Pattern**: Data access abstracted through `IRepository<T>` for Person, UserLogin, OrganizationRoleUser entities
- **Validator Pattern**: FluentValidation-based validators (`ILoginAuthenticationModelValidator`, `IUniqueEmailValidator`) separate validation logic from business logic
- **Strategy Pattern**: Different authentication strategies (standard login, customer login via code, review API login)

### Data Flow

#### User Registration/Creation
1. Input: `UserEditModel` with person and login details
2. Validated by `UserEditModelValidator`, `PersonEditModelValidator`, `UniqueEmailValidator`
3. `PersonFactory` creates Person domain entity with addresses and phones
4. `UserLoginFactory` creates UserLogin with hashed password (using `ICryptographyOneWayHashService`)
5. `UserService.Save()` persists Person → UserLogin → OrganizationRoleUser linkage
6. Optional: `SendLoginCredentialsService` emails credentials if `SendUserLoginViaEmail` is true

#### Authentication Flow
1. Input: `LoginAuthenticationModel` with username/password
2. `LoginAuthenticationModelValidator.IsValid()` orchestrates validation:
   - Retrieve UserLogin by username
   - Check if account is locked (`IsLocked`)
   - Check if user is active in any organization (`OrganizationRoleUser.IsActive`)
   - Validate password hash using `ICryptographyOneWayHashService`
   - On success: Reset `LoginAttemptCount`, update `LastLoggedInDate`, clear `ResetToken`
   - On failure: Increment `LoginAttemptCount`, lock account if >= 5 attempts
3. `UserLogService.SaveLoginSession()` creates audit trail with session token, IP, browser info
4. Return user session with authorization context

#### Password Reset Flow
1. User requests reset via email
2. `PasswordResetService.SendPasswordLink()`:
   - Generate GUID reset token
   - Store in `UserLogin.ResetToken` with `ResetTokenIssueDate`
   - Email link: `{SiteRootUrl}#/password/reset/{token}`
3. User clicks link and submits new password
4. `PasswordResetService.ResetPassword()`:
   - Validate token exists and is < 24 hours old
   - `UserLoginFactory.CreateResetPasswordDomain()` hashes new password
   - Clear `ResetToken` and `ResetTokenIssueDate`
   - Persist updated UserLogin

#### User Management Flow
1. `UserService.GetUsers()` with filtering (role, organization, search text, pagination)
2. `UserService.Delete()` soft-deletes by setting `OrganizationRoleUser.IsActive = false`
3. `UserService.Lock()` sets `UserLogin.IsLocked` flag
4. `UserService.ManageAccount()` assigns user to multiple franchisees (OrganizationRoleUserFranchisee)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Core Domain Entities

```csharp
// Person - Represents an individual with contact information
public class Person : DomainBase
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string Email { get; set; }
    public long? FileId { get; set; }  // Profile image
    public bool IsRecruitmentFeeApplicable { get; set; }
    public string CalendarPreference { get; set; }
    
    public virtual UserLogin UserLogin { get; set; }  // 1:1 relationship
    public virtual ICollection<Address> Addresses { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
    
    // Computed properties
    public Name Name { get; }  // Value object combining First/Middle/Last
    public string FullNameUser { get; }  // Formatted full name
}

// UserLogin - Authentication credentials (1:1 with Person)
public class UserLogin : DomainBase
{
    [ForeignKey("Person")]
    public override long Id { get; set; }  // Shares primary key with Person
    
    public string UserName { get; set; }  // Always stored lowercase
    public string Password { get; set; }   // Hashed password
    public string Salt { get; set; }       // Salt for password hash
    
    // Account status
    public bool IsLocked { get; set; }     // True after 5 failed attempts
    public bool IsActive { get; set; }     // Defaults to true
    public int? LoginAttemptCount { get; set; }
    public DateTime? LastLoggedInDate { get; set; }
    
    // Password reset
    public string ResetToken { get; set; }  // GUID for password reset
    public DateTime? ResetTokenIssueDate { get; set; }
    
    // Timezone offsets
    public double? ESTOffset { get; set; }
    public double? EDTOffset { get; set; }
    
    public virtual Person Person { get; set; }
    
    public const int MaxAttempts = 5;  // Account locks after 5 failed attempts
}

// Role - Predefined user roles
public class Role : DomainBase
{
    public string Name { get; set; }    // e.g., "Super Admin"
    public string Alias { get; set; }   // e.g., "SuperAdmin"
    public int AccessOrder { get; set; } // Priority/hierarchy
}

// Phone - Contact phone number with extension support
public class Phone : DomainBase
{
    public long TypeId { get; set; }      // PhoneType enum: Cell, Office, Home, Fax
    public string CountryCode { get; set; } // Default "1"
    public string AreaCode { get; set; }    // 3 digits
    public string Number { get; set; }      // 7 digits
    public string Extension { get; set; }   // Optional
    public bool IsTransferable { get; set; }
    
    // Factory method parses "(555) 123-4567 x890" format
    public static Phone Create(string phone, long phoneType, long id);
}

// EmailSignatures - User email signatures
public class EmailSignatures : DomainBase
{
    public long? UserId { get; set; }
    public long? OrganizationRoleUserId { get; set; }
    public string SignatureName { get; set; }
    public string Signature { get; set; }  // HTML content
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
```

### Enumerations

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

public enum PhoneType
{
    Cell = 1,
    Office = 2,
    Home = 3,
    Fax = 4
}
```

### View Models (DTOs)

```csharp
// UserEditModel - Complete user profile for create/update
public class UserEditModel : EditModelBase
{
    public UserLoginEditModel UserLoginEditModel { get; set; }
    public PersonEditModel PersonEditModel { get; set; }
    public long OrganizationId { get; set; }
    public long RoleId { get; set; }
    public ICollection<long> RoleIds { get; set; }  // Multi-role support
    public ICollection<long> OrganizationIds { get; set; }  // Multi-org access
    public bool CreateLogin { get; set; }
    public long? FileId { get; set; }  // Profile image
    public bool IsExecutive { get; set; }
}

// LoginAuthenticationModel - Login credentials input
public class LoginAuthenticationModel : EditModelBase
{
    public string Username { get; set; }
    public string Password { get; set; }
    public FeedbackMessageModel Message { get; set; }  // Validation error messages
}

// ChangePasswordEditModel - Password reset input
public class ChangePasswordEditModel
{
    public string Key { get; set; }      // Reset token (GUID)
    public string Password { get; set; }  // New password (plaintext, will be hashed)
}

// UserSessionModel - Authenticated user session data
public class UserSessionModel
{
    public long UserId { get; set; }
    public string Token { get; set; }  // Session token
    public ICollection<UserOrganizationRole> Roles { get; set; }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### IUserService - User Management

#### `UserEditModel Get(long userId, long franchiseeId = default(long))`
- **Input**: User ID, optional franchisee ID for scoping
- **Output**: `UserEditModel` with person, login, roles, and organization assignments
- **Behavior**: Returns user profile including contact info, login credentials (username only, no password), role assignments within specified franchisee. Returns empty model with default phone numbers if user not found.
- **Side-effects**: None (read-only)

#### `void Save(UserEditModel userEditModel)`
- **Input**: `UserEditModel` with person, login, and role data
- **Output**: None (throws on validation failure)
- **Behavior**: 
  - Validates email uniqueness, phone format, required fields
  - Creates/updates Person entity with addresses and phones
  - Creates/updates UserLogin with hashed password (if `CreateLogin` or `ChangePassword`)
  - Manages OrganizationRoleUser assignments (creates/deactivates as needed)
  - Sends login credentials email if `SendUserLoginViaEmail` is true
- **Side-effects**: Database persistence, email notification, password hashing

#### `void Delete(long userId)`
- **Input**: User ID to delete
- **Output**: None
- **Behavior**: Soft-delete by setting all `OrganizationRoleUser.IsActive = false`. Does NOT delete Person or UserLogin records (audit trail preservation).
- **Side-effects**: User cannot log in after deletion

#### `UserListModel GetUsers(UserListFilter filter, int pageNumber, int pageSize)`
- **Input**: Filter criteria (organization, role, search text, active status), pagination params
- **Output**: Paginated user list with role information
- **Behavior**: Filtered query with sorting support. Includes user name, email, roles, organizations, active status.
- **Side-effects**: None

#### `bool ManageAccount(long userId, long[] franchiseeIds)`
- **Input**: User ID and array of franchisee (organization) IDs
- **Output**: Success boolean
- **Behavior**: Assigns user to multiple franchisees by creating OrganizationRoleUserFranchisee records. Used for multi-location access control.
- **Side-effects**: Database persistence of franchisee assignments

#### `void SaveUserSignature(UserSignatureListSaveModel model, long? orgId, long? userId)`
- **Input**: Signature data (name, HTML content, default flag), organization ID, user ID
- **Output**: None
- **Behavior**: Creates/updates email signature for user. Sets `IsDefault` flag, deactivates old signatures if needed.
- **Side-effects**: Database persistence

### IUserLoginService - Authentication & Account Status

#### `UserLogin GetbyUserName(string userName)`
- **Input**: Username (case-insensitive)
- **Output**: `UserLogin` entity or null
- **Behavior**: Retrieves login record by username. Used during authentication flow.
- **Side-effects**: None

#### `UserLogin UpdateforInvalidAttempt(UserLogin login)`
- **Input**: UserLogin entity
- **Output**: Updated UserLogin
- **Behavior**: Increments `LoginAttemptCount`. Sets `IsLocked = true` if count >= 5 (MaxAttempts).
- **Side-effects**: Database persistence, potential account lockout

#### `UserLogin UpdateforValidAttempt(UserLogin login)`
- **Input**: UserLogin entity
- **Output**: Updated UserLogin
- **Behavior**: Resets `LoginAttemptCount = 0`, updates `LastLoggedInDate`, clears `ResetToken`, stores timezone offset (EST/EDT based on current date).
- **Side-effects**: Database persistence

#### `bool IsUniqueUserName(string userName, long userId = 0)`
- **Input**: Username, optional user ID (for updates)
- **Output**: True if username is available
- **Behavior**: Case-insensitive uniqueness check. Excludes current user when userId provided.
- **Side-effects**: None

#### `bool Lock(long userId, bool isLocked, out bool lockResult, out bool isEquipment)`
- **Input**: User ID, lock status boolean
- **Output**: Success boolean, lock result, equipment flag
- **Behavior**: Sets `UserLogin.IsLocked`. Checks if user has Equipment role. Validates user has no active job assignments before locking.
- **Side-effects**: Database persistence, may prevent user login

### IPasswordResetService - Password Management

#### `bool SendPasswordLink(string email)`
- **Input**: User's email address
- **Output**: Success boolean
- **Behavior**: 
  - Validates email exists in system
  - Generates GUID reset token
  - Stores token and issue date in UserLogin
  - Sends email with reset link: `{SiteRootUrl}#/password/reset/{token}`
  - Throws `InvalidDataProvidedException` if email not found
- **Side-effects**: Database persistence, email notification

#### `bool ResetPassword(ChangePasswordEditModel model)`
- **Input**: Reset token (Key) and new password
- **Output**: Success boolean
- **Behavior**: 
  - Validates token exists and is < 24 hours old (1440 minutes)
  - Hashes new password with new salt
  - Clears ResetToken and ResetTokenIssueDate
  - Returns false if token invalid/expired (doesn't throw)
- **Side-effects**: Database persistence, password change

#### `bool ResetPasswordExpire(ChangePasswordEditModel model)`
- **Input**: Reset token (Key)
- **Output**: True if token valid (not expired)
- **Behavior**: Checks if token exists and is < 24 hours old. Used to validate reset link before showing password form.
- **Side-effects**: None (read-only check)

### IUserFactory - User Object Construction

#### `UserViewModel CreateViewModel(OrganizationRoleUser organizationRoleUser)`
- **Input**: OrganizationRoleUser entity with loaded navigation properties
- **Output**: `UserViewModel` for display
- **Behavior**: Maps domain entity to view model including role name, organization info, user details.
- **Side-effects**: None

#### `SalesRep CreateDomain(OrganizationRoleUser organizationRoleUser, UserEditModel userEditModel)`
- **Input**: Organization role user, user edit model
- **Output**: SalesRep domain entity
- **Behavior**: Creates SalesRep entity when user has SalesRep role. Sets active status, organization linkage.
- **Side-effects**: None (creation only, persistence happens in service)

### IPersonFactory - Person Entity Construction

#### `PersonEditModel CreateModel(Person domain)`
- **Input**: Person domain entity
- **Output**: `PersonEditModel` for editing
- **Behavior**: Maps Person entity to edit model including addresses, phones, email, name parts.
- **Side-effects**: None

#### `Person CreateDomain(PersonEditModel model)`
- **Input**: PersonEditModel from form
- **Output**: Person domain entity
- **Behavior**: Creates Person with associated Address and Phone collections. Handles entity state (IsNew) for repository.
- **Side-effects**: None (creation only)

### IUserLoginFactory - UserLogin Entity Construction

#### `UserLogin CreateDomain(UserLoginEditModel model, Person person, UserLogin inPersistence)`
- **Input**: Login edit model, Person entity, existing UserLogin (or null)
- **Output**: UserLogin domain entity with hashed password
- **Behavior**: 
  - Updates existing or creates new UserLogin
  - Generates random password if `SendUserLoginViaEmail = true`
  - Hashes password using `ICryptographyOneWayHashService` (creates hash + salt)
  - Converts username to lowercase
  - Sets `IsLocked = false` on creation
- **Side-effects**: Password hashing (CPU-intensive operation)

#### `UserLogin CreateResetPasswordDomain(ChangePasswordEditModel model, UserLogin inPersistence)`
- **Input**: Password reset model, existing UserLogin
- **Output**: Updated UserLogin with new hashed password
- **Behavior**: Hashes new password with new salt, updates UserLogin entity.
- **Side-effects**: Password hashing

### ILoginAuthenticationModelValidator - Authentication Validation

#### `bool IsValid(LoginAuthenticationModel model)`
- **Input**: Login credentials (username, password)
- **Output**: True if authentication successful
- **Behavior**: 
  - Validates username exists
  - Checks account not locked
  - Validates user active in at least one organization
  - Validates password hash matches
  - On success: Calls `UpdateforValidAttempt()`, returns true
  - On failure: Calls `UpdateforInvalidAttempt()`, sets appropriate error message in `model.Message`
  - Error messages: "Invalid credentials", "Account locked", "User deactivated", "Last 2 attempts remaining"
- **Side-effects**: Updates login attempt count, may lock account, updates last login date

#### `bool IsValidForReviewAPI(LoginAuthenticationModel model)`
- **Input**: Login credentials
- **Output**: True if authentication successful
- **Behavior**: Same as `IsValid()` but does NOT update `LastLoggedInDate` (read-only auth check for API).
- **Side-effects**: Only updates attempt count on failure, no success side-effects

#### `bool IsValidForCustomer(LoginCustomerAuthenticationModel model)`
- **Input**: Customer authentication code
- **Output**: True if code valid and active
- **Behavior**: Validates customer signature code exists and is active (not expired).
- **Side-effects**: None

### IUniqueEmailValidator - Email Validation

#### `bool IsValid(long personId, string email)`
- **Input**: Person ID (0 for new users), email address
- **Output**: True if email unique
- **Behavior**: Case-insensitive email uniqueness check. Excludes current person when personId > 0.
- **Side-effects**: None

### ISendLoginCredentialsService - Credential Distribution

#### `bool SendLoginCredentials(Person person, string password, bool includeSetupGuide)`
- **Input**: Person entity, plaintext password, setup guide flag
- **Output**: Success boolean
- **Behavior**: Sends email with username and password to user's email address. Optionally includes setup guide attachment.
- **Side-effects**: Email notification

### IUserLogService - Session & Audit Trail

#### `void SaveLoginSession(long userId, string sessionId, string clientIp, DateTime loginDateTime, string browser, string os, string userAgent)`
- **Input**: User ID, session token (GUID), client IP, login timestamp, browser info
- **Output**: None
- **Behavior**: Creates audit log record for login event. Session token used for subsequent authentication.
- **Side-effects**: Database persistence of audit record

#### `bool IsTokenValid(string token)`
- **Input**: Session token
- **Output**: True if session active
- **Behavior**: Validates session token exists and is not expired/ended.
- **Side-effects**: None

#### `long GetUserId(string token)`
- **Input**: Session token
- **Output**: User ID associated with session
- **Behavior**: Retrieves user ID from active session token.
- **Side-effects**: None

#### `void EndLoggedinSession(string token)`
- **Input**: Session token
- **Output**: None
- **Behavior**: Marks session as ended (logout event).
- **Side-effects**: Database persistence
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[Core.Application](../../Application/.context/CONTEXT.md)** — Base domain entities (DomainBase), repository pattern (IRepository, IUnitOfWork), cryptography services (ICryptographyOneWayHashService), clock abstraction (IClock), settings (ISettings)
- **[Core.Organizations](../../Organizations/.context/CONTEXT.md)** — Organization entities (Organization, OrganizationRoleUser, Franchisee), organization management services
- **[Core.Geo](../../Geo/.context/CONTEXT.md)** — Address entity for person contact information
- **[Core.Notification](../../Notification/.context/CONTEXT.md)** — Email notification services (IUserNotificationModelFactory for password reset emails)
- **[Core.Scheduler](../../Scheduler/.context/CONTEXT.md)** — JobScheduler entity for validating user assignments before locking accounts, CustomerSignatureInfo for customer portal authentication

### External Dependencies
- **FluentValidation** — Declarative validation framework for view models
- **DocumentFormat.OpenXml** — Used in IUserLoginService (likely for document generation, unclear from interface)
- **System.ComponentModel.DataAnnotations** — Data annotations for entity mapping ([ForeignKey], [NotMapped])

### Security Dependencies
- **ICryptographyOneWayHashService** — Password hashing using salted hash algorithm (implementation details in Core.Application)
- **ISettings** — Configuration for password length, site root URL, and other security parameters
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Security Considerations
1. **Password Storage**: All passwords are hashed using salted one-way hashing. Salt is stored per-user in `UserLogin.Salt`. Never store plaintext passwords.
2. **Account Lockout**: Automatic lockout after 5 failed login attempts. Manual unlock required through admin interface or `IUserLoginService.Lock()`.
3. **Password Reset Token**: 24-hour expiration on reset tokens. Tokens are GUIDs, not predictable sequences.
4. **Username Case Sensitivity**: Usernames are always stored lowercase, case-insensitive lookups prevent duplicate accounts with different casing.
5. **Soft Delete**: Users are never hard-deleted. `OrganizationRoleUser.IsActive = false` preserves audit trail and referential integrity.

### Common Pitfalls
1. **Person vs UserLogin Confusion**: Person represents identity/contact info, UserLogin represents authentication. Not all Persons have UserLogin (e.g., customers without login access).
2. **Multi-Organization Users**: A single Person can have multiple OrganizationRoleUser records (different roles in different organizations). Always filter by organization context.
3. **Timezone Handling**: `UserLogin.ESTOffset` and `EDTOffset` store browser timezone offsets. System determines EST vs EDT based on current date and daylight saving transition dates.
4. **Phone Number Parsing**: Use `Phone.Create()` factory method, not direct construction. Handles various formats: "(555) 123-4567 x890" → AreaCode="555", Number="1234567", Extension="890".
5. **Email Uniqueness**: Email validation happens at Person level, not UserLogin level. Check `IUniqueEmailValidator` before saving.

### Performance Notes
- Password hashing (during login and save) is CPU-intensive. Avoid re-hashing on updates unless password actually changed.
- `UserService.GetUsers()` with pagination—always provide page size limits to avoid large result sets.
- Lazy loading of navigation properties (Person.Addresses, Person.Phones) may cause N+1 queries. Use `.Include()` in repositories when loading collections.
<!-- END CUSTOM SECTION -->
