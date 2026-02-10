<!-- AUTO-GENERATED: Header -->
# Users/Impl — Implementation Services Context
**Version**: a07029e2c5e0a107bdc5d26050dd43aab2001d6b
**Generated**: 2026-02-10T15:35:17Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Impl folder contains concrete implementations of all service interfaces, validators, and factories defined in the Users module root. This is where business logic lives—authentication workflows, password hashing, user creation/update, validation rules, and audit logging. All classes are marked with `[DefaultImplementation]` attribute for dependency injection registration.

### Design Patterns
- **Service Layer Pattern**: Services (`UserService`, `UserLoginService`, `PasswordResetService`) coordinate repositories, factories, and external services
- **Factory Pattern**: Factories (`PersonFactory`, `UserFactory`, `UserLoginFactory`) encapsulate complex object creation with validation and transformation logic
- **Validator Pattern**: FluentValidation validators (`LoginAuthenticationModelValidator`, `UniqueEmailValidator`) separate validation from business logic
- **Repository Pattern**: All data access through injected `IRepository<T>` instances, no direct database queries
- **Dependency Injection**: All dependencies injected via constructor, resolved by IoC container

### Data Flow Patterns

**User Creation Flow**:
```
UserEditModel → UserEditModelValidator → UserService
                                              ↓
                              PersonFactory.CreateDomain(PersonEditModel)
                                              ↓
                              UserLoginFactory.CreateDomain(UserLoginEditModel) 
                                              ↓ (password hashing)
                              Repository.Save(Person + UserLogin + OrganizationRoleUser)
                                              ↓
                              SendLoginCredentialsService (if SendUserLoginViaEmail)
```

**Authentication Flow**:
```
LoginAuthenticationModel → LoginAuthenticationModelValidator.IsValid()
                                              ↓
                              UserLoginService.GetbyUserName()
                                              ↓
                              Check: IsLocked, IsActive, Password Hash
                                              ↓
                      Valid: UpdateforValidAttempt() → UserLogService.SaveLoginSession()
                      Invalid: UpdateforInvalidAttempt() → Increment count, possibly lock
```

**Password Reset Flow**:
```
Email → PasswordResetService.SendPasswordLink()
                 ↓
        Generate GUID token → Store in UserLogin.ResetToken
                 ↓
        Email reset link → User clicks → PasswordResetService.ResetPassword()
                 ↓
        UserLoginFactory.CreateResetPasswordDomain() → Hash new password → Clear token
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Implementation Classes

### Services

#### UserService.cs
**Purpose**: Orchestrates user lifecycle management including creation, updates, deletion, and multi-organization role assignment.

**Key Methods**:
- `Get(userId, franchiseeId)`: Retrieves user profile with roles scoped to franchisee
- `Save(UserEditModel)`: Creates/updates Person, UserLogin, and OrganizationRoleUser records with validation
- `Delete(userId)`: Soft-deletes by deactivating all OrganizationRoleUser records
- `GetUsers(filter, page, size)`: Filtered, paginated user list with role information
- `ManageAccount(userId, franchiseeIds)`: Assigns user to multiple franchisees via OrganizationRoleUserFranchisee
- `SaveUserSignature(model, orgId, userId)`: Manages email signatures with default flag handling

**Dependencies**: 14 repository types, 4 factories, 6 services including UserLoginService, PhoneService, SendLoginCredentialsService

**Validation**: Orchestrates PersonEditModelValidator, UserEditModelValidator, UserLoginEditModelValidator

#### UserLoginService.cs
**Purpose**: Manages authentication state, login attempts, account locking, and timezone tracking.

**Key Methods**:
- `GetbyUserName(username)`: Case-insensitive username lookup
- `UpdateforInvalidAttempt(login)`: Increments attempt count, locks at 5 attempts
- `UpdateforValidAttempt(login)`: Resets count, updates last login date, stores timezone offset
- `IsUniqueUserName(username, userId)`: Username availability check
- `Lock(userId, isLocked, out lockResult, out isEquipment)`: Manual account lock/unlock with equipment validation

**Algorithm - Timezone Detection**:
```csharp
bool IsEST() {
    // Calculates second Sunday in March (EDT starts) and first Sunday in November (EST starts)
    // Returns true if current date is between Nov first Sunday and March second Sunday
    // Used to determine which offset to store (ESTOffset vs EDTOffset)
}
```

**Security Features**:
- Max 5 login attempts before lockout
- Automatic attempt reset on successful login
- Timezone tracking for audit compliance

#### PasswordResetService.cs
**Purpose**: Handles password reset workflow with time-limited tokens and email notifications.

**Key Methods**:
- `SendPasswordLink(email)`: Generates GUID token, stores with issue date, emails reset link
- `ResetPassword(model)`: Validates token age (<24 hours), hashes new password, clears token
- `ResetPasswordExpire(model)`: Checks if token is still valid before showing reset form

**Token Expiration**: 1440 minutes (24 hours) from `ResetTokenIssueDate`

**Error Handling**: Throws `InvalidDataProvidedException` for missing email or expired token

#### UserLogService.cs
**Purpose**: Audit trail management for user and customer login sessions.

**Key Methods**:
- `SaveLoginSession(userId, sessionId, clientIp, loginDateTime, browser, os, userAgent)`: Creates UserLog record
- `SaveLoginCustomerSession(...)`: Creates CustomerLog for customer portal access
- `IsTokenValid(token)`: Checks if session token is active (LoggedOutAt == null)
- `GetUserId(token)`: Retrieves user ID from session token
- `EndLoggedinSession(token)`: Sets LoggedOutAt timestamp (logout event)
- `IsFirstTimeLogin(userId)`: Returns true if user has logged in < 2 times

**Usage**: Session tokens are GUIDs stored in UserLog.SessionId, used for stateless authentication

#### SendLoginCredentialsService.cs
**Purpose**: Sends login credentials via email using notification system.

**Key Method**:
- `SendLoginCredentials(person, password, includeSetupGuide)`: Delegates to `IUserNotificationModelFactory.CreateLoginCredentialNotification()`

**Note**: Password is sent in plaintext via email (security consideration for initial setup only)

### Factories

#### PersonFactory.cs
**Purpose**: Converts between Person domain entities and PersonEditModel view models with associated Address and Phone collections.

**Key Methods**:
- `CreateModel(domain)`: Domain → View Model (for editing)
- `CreateDomain(PersonEditModel)`: View Model → Domain (for persistence)
- `CreateDomain(OrganizationOwnerEditModel, UserLogin, email)`: Creates Person from organization owner data
- `CreateDomain(PersonEquipmentEditModel)`: Creates Person for equipment users

**Transformation Logic**:
- Delegates Address/Phone conversion to `IAddressFactory` and `IPhoneFactory`
- Sets `IsNew = true` when `PersonId <= 0`
- Handles Name value object mapping (FirstName, MiddleName, LastName)
- File path construction: `{RelativeLocation}\{Name}` converted to full path

#### UserLoginFactory.cs
**Purpose**: Creates UserLogin entities with password hashing and random password generation.

**Key Methods**:
- `CreateDomain(UserLoginEditModel, Person, UserLogin)`: Creates/updates UserLogin with hashed password
- `CreateResetPasswordDomain(ChangePasswordEditModel, UserLogin)`: Hashes new password for reset workflow
- `CreateEditModel(UserLogin)`: Domain → View Model
- `CreateDomain(OrganizationOwnerEditModel, email)`: Creates UserLogin for organization owner

**Password Hashing**:
```csharp
// Uses ICryptographyOneWayHashService
var hash = _cryptographyService.CreateHash(plaintextPassword);
userLogin.Password = hash.HashedText;  // Salted hash
userLogin.Salt = hash.Salt;            // Unique salt
```

**Random Password Generation**:
- Length from `_settings.PasswordLength`
- Generates random uppercase letters, converts to lowercase
- Used when `SendUserLoginViaEmail = true`

**Conventions**:
- Username always converted to lowercase
- `IsLocked = false` on creation
- `IsNew` flag set based on `Id <= 0`

#### UserFactory.cs
**Purpose**: Creates UserViewModel and SalesRep domain entities.

**Key Methods**:
- `CreateViewModel(OrganizationRoleUser)`: Maps OrganizationRoleUser to UserViewModel for display
- `CreateDomain(OrganizationRoleUser, UserEditModel)`: Creates SalesRep entity when role is SalesRep
- `CreateSignatureViewModel(EmailSignatures)`: Maps email signature to view model
- `CreateSignatureSaveModel(signature, userId, orgId)`: Converts view model to EmailSignatures entity

### Validators

#### LoginAuthenticationModelValidator.cs
**Purpose**: Validates login credentials and orchestrates authentication workflow with attempt tracking.

**Validation Rules**:
1. Username/Password not null and not empty (FluentValidation rules)
2. UserLogin exists for username
3. Account not locked (`IsLocked = false`)
4. User has at least one active OrganizationRoleUser (`IsActive = true`, `IsDefault = true`)
5. Password hash matches (`ICryptographyOneWayHashService.Validate()`)

**Error Messages**:
- "Invalid credentials" (username not found or wrong password)
- "Account locked" (after 5 failed attempts)
- "User deactivated" (no active organization roles)
- "Invalid credentials - Last 2 attempts remaining" (at 3 failed attempts)

**Side Effects**:
- Success: Calls `UserLoginService.UpdateforValidAttempt()` (resets count, updates last login)
- Failure: Calls `UserLoginService.UpdateforInvalidAttempt()` (increments count, may lock)

**IsValidForReviewAPI**: Same validation but does NOT update LastLoggedInDate (read-only auth check)

**IsValidForCustomer**: Validates customer authentication code instead of username/password

#### UserEditModelValidator.cs
**Purpose**: Validates UserEditModel for user creation/update.

```csharp
public class UserEditModelValidator : AbstractValidator<UserEditModel>
{
    // FluentValidation rules for:
    // - Required fields (name, email, organization, role)
    // - Email format and uniqueness
    // - Phone number format
    // - Password complexity (if CreateLogin or ChangePassword)
}
```

#### PersonEditModelValidator.cs
**Purpose**: Validates PersonEditModel for person data.

**Validation**: Name parts, email format, address completeness, phone format

#### UserLoginEditModelValidator.cs
**Purpose**: Validates UserLoginEditModel for login credentials.

**Validation**: Username uniqueness, password strength requirements, email format

#### UniqueEmailValidator.cs
**Purpose**: FluentValidation custom validator for email uniqueness.

**Implementation**:
- Extends `EmailValidator` (validates email format first)
- Calls `IUserLoginService.IsUniqueEmailAddress()` for uniqueness check
- Throws `InvalidDataProvidedException` for duplicate emails

**Public Method**: `IsValid(personId, email)` for standalone uniqueness checks (excludes current person)

#### PhoneNumberTextValidator.cs
**Purpose**: Validates phone number format before parsing with `Phone.Create()`.

**Validation**: Minimum 10 digits, numeric-only after sanitization
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Service Dependencies
- **Core.Application** — IRepository, IUnitOfWork, IClock, ICryptographyOneWayHashService, ISettings
- **Core.Notification** — IUserNotificationModelFactory (email notifications)
- **Core.Geo** — IAddressFactory (address conversion)
- **Core.Organizations** — IOrganizationFactory, IPhoneFactory, IPhoneService
- **Core.Scheduler** — IJobService, IOrganizationRoleUserInfoService (job validation for account locking)

### External Dependencies
- **FluentValidation** — AbstractValidator<T>, PropertyValidatorContext for declarative validation
- **DocumentFormat.OpenXml** — Used in UserLoginService (specific usage unclear from current analysis)

### Cross-Module References
- Factories depend on other module factories (AddressFactory, PhoneFactory)
- Services coordinate across multiple modules (Sales, Scheduler, Organizations)
- Validators use dependency injection to resolve services at runtime
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Security Implementation Details

**Password Hashing Algorithm**:
- Uses `ICryptographyOneWayHashService` (implementation in Core.Application)
- Generates unique salt per user
- Salt + password → one-way hash (bcrypt, PBKDF2, or similar)
- Validation: hash(inputPassword + storedSalt) === storedPassword

**Account Lockout Logic**:
```csharp
// UserLoginService.UpdateforInvalidAttempt()
login.LoginAttemptCount++;
if (login.LoginAttemptCount >= UserLogin.MaxAttempts) {  // MaxAttempts = 5
    login.IsLocked = true;
}
```

**Token Security**:
- Reset tokens are GUIDs (128-bit random, not predictable)
- 24-hour expiration enforced in `PasswordResetService.ResetPassword()`
- Tokens cleared after single use (even if expired)

### Timezone Handling

**EST/EDT Detection Algorithm**:
```csharp
// Calculates daylight saving transition dates
// Second Sunday in March = EDT starts (spring forward)
// First Sunday in November = EST starts (fall back)
// Stores browser offset in appropriate field based on current date
```

**Usage**: Enables server-side timezone conversion for audit logs and scheduling without relying on client clock accuracy

### Validation Architecture

**Two-Level Validation**:
1. **Syntax Validation** (FluentValidation): Field formats, required fields, string lengths
2. **Business Rule Validation** (Services): Uniqueness checks, referential integrity, state machine rules

**Validator Resolution**:
- `UniqueEmailValidator` uses `ApplicationManager.DependencyInjection.Resolve<IUserLoginService>()` to access services at validation time
- Enables validators to execute database queries for business rule validation

### Factory Pattern Implementation

**Why Separate Factories?**:
- `PersonFactory`: Handles Person entity with complex child collections (Addresses, Phones)
- `UserLoginFactory`: Isolated password hashing logic (security concern separation)
- `UserFactory`: Organization-specific view model construction

**Factory Responsibility**: Transform data, not validate—factories assume inputs are pre-validated

### Common Patterns

**Soft Delete**:
```csharp
// UserService.Delete() - never hard-deletes
var orgRoleUsers = _orgRoleUserRepository.Fetch(x => x.UserId == userId);
foreach (var oru in orgRoleUsers) {
    oru.IsActive = false;
    _orgRoleUserRepository.Save(oru);
}
```

**Audit Trail Creation**:
```csharp
// Every login creates UserLog record
var log = new UserLog {
    UserId = userId,
    SessionId = Guid.NewGuid().ToString(),
    LoggedInAt = _clock.UtcNow,
    ClientIp = ipAddress,
    Browser = browserString,
    OS = osString,
    UserAgent = rawUserAgent
};
```

**Multi-Organization Assignment**:
```csharp
// UserService.ManageAccount() creates cross-references
foreach (var franchiseeId in franchiseeIds) {
    var assignment = new OrganizationRoleUserFranchisee {
        OrganizationRoleUserId = orgRoleUserId,
        FranchiseeId = franchiseeId,
        IsActive = true,
        IsDefault = (franchiseeId == defaultFranchiseeId)
    };
    _orgRoleUserFranchiseeRepository.Save(assignment);
}
```

### Performance Considerations

**Password Hashing**: CPU-intensive operation. Cache results if validating repeatedly (e.g., during session management).

**Eager Loading**: `UserService.Get()` loads multiple navigation properties. Use `.Include()` to avoid N+1 queries:
```csharp
var person = _personRepository.Get(
    predicate: p => p.Id == userId,
    include: p => p.UserLogin,
             p => p.Addresses,
             p => p.Phones,
             p => p.File
);
```

**Validator Database Queries**: `UniqueEmailValidator` queries database per field validation. Consider caching email lookups in high-traffic scenarios.

### Testing Considerations

**Mock Dependencies**: All services take interfaces in constructors—easy to mock repositories, factories, and services for unit tests.

**Validator Testing**: FluentValidation supports `.ShouldHaveValidationErrorFor()` for isolated validation rule testing.

**Factory Testing**: Test transformations with known input/output pairs. Verify `IsNew` flag logic and null handling.
<!-- END CUSTOM SECTION -->
