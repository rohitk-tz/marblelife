<!-- AUTO-GENERATED: Header -->
# Users/Impl
> Concrete implementations of services, factories, and validators for user management and authentication
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Impl folder contains all the "how" of the Users module—the concrete implementations that bring interface contracts to life. While the root folder defines *what* services do (interfaces), Impl defines *how* they do it.

### What's Inside

**Services** (`*Service.cs`): Business logic orchestration
- `UserService`: User CRUD operations, multi-organization assignment, profile management
- `UserLoginService`: Authentication state, login attempts, account locking
- `PasswordResetService`: Reset token generation, password updates with expiration
- `UserLogService`: Session tracking, audit trail creation
- `SendLoginCredentialsService`: Email credential distribution

**Factories** (`*Factory.cs`): Object construction and transformation
- `PersonFactory`: Person ↔ PersonEditModel conversion with addresses and phones
- `UserLoginFactory`: UserLogin creation with password hashing
- `UserFactory`: UserViewModel construction, SalesRep entity creation

**Validators** (`*Validator.cs`): FluentValidation rule implementations
- `LoginAuthenticationModelValidator`: Credential validation, attempt tracking, error messaging
- `UniqueEmailValidator`: Email format and uniqueness checks
- `UserEditModelValidator`, `PersonEditModelValidator`, `UserLoginEditModelValidator`: Form validation

### Architecture Principles

**Separation of Concerns**: Each class has a single, well-defined responsibility
**Dependency Injection**: All dependencies injected via constructor, resolved by IoC container
**Interface-Based Design**: Services implement interfaces from root folder, enabling testability
**No Direct DB Access**: All data access through `IRepository<T>`, ensuring abstraction

Think of this folder as the "engine room" of the Users module—hidden complexity that makes the public interfaces work smoothly.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Service Usage Examples

```csharp
// User creation with validation and email notification
var userEditModel = new UserEditModel
{
    PersonEditModel = new PersonEditModel
    {
        FirstName = "Sarah",
        LastName = "Johnson",
        Email = "sarah.johnson@example.com"
    },
    UserLoginEditModel = new UserLoginEditModel
    {
        UserName = "sarah.johnson@example.com",
        SendUserLoginViaEmail = true  // Triggers SendLoginCredentialsService
    },
    OrganizationId = franchiseeId,
    RoleId = (long)RoleType.Technician,
    CreateLogin = true
};

// UserService orchestrates:
// 1. Validation (UserEditModelValidator, PersonEditModelValidator, UniqueEmailValidator)
// 2. PersonFactory.CreateDomain() - converts view model to entity
// 3. UserLoginFactory.CreateDomain() - creates UserLogin with hashed password
// 4. Repository.Save() - persists Person, UserLogin, OrganizationRoleUser
// 5. SendLoginCredentialsService - emails credentials
userService.Save(userEditModel);
```

### Authentication Workflow

```csharp
// Login attempt
var loginModel = new LoginAuthenticationModel
{
    Username = "sarah.johnson@example.com",
    Password = "UserPassword123!"
};

// LoginAuthenticationModelValidator.IsValid() orchestrates:
// 1. UserLoginService.GetbyUserName() - retrieve UserLogin
// 2. Check IsLocked, IsActive flags
// 3. ICryptographyOneWayHashService.Validate() - verify password hash
// 4. On success: UserLoginService.UpdateforValidAttempt()
// 5. On failure: UserLoginService.UpdateforInvalidAttempt() - may lock account
bool isAuthenticated = loginAuthValidator.IsValid(loginModel);

if (isAuthenticated)
{
    var userLogin = userLoginService.GetbyUserName(loginModel.Username);
    var sessionId = Guid.NewGuid().ToString();
    
    // UserLogService creates audit record
    userLogService.SaveLoginSession(
        userId: userLogin.Id,
        sessionId: sessionId,
        clientIp: "192.168.1.100",
        loginDateTime: DateTime.UtcNow,
        browser: "Chrome",
        os: "Windows 10",
        userAgent: "Mozilla/5.0..."
    );
    
    return sessionId;  // Return to client for subsequent requests
}
else
{
    // loginModel.Message contains error:
    // - "Invalid credentials"
    // - "Account locked"
    // - "User deactivated"
    throw new UnauthorizedException(loginModel.Message.Text);
}
```

### Password Reset Implementation

```csharp
// Step 1: User requests password reset
try
{
    passwordResetService.SendPasswordLink("sarah.johnson@example.com");
    // PasswordResetService:
    // 1. Finds Person by email
    // 2. Generates GUID reset token
    // 3. Stores in UserLogin.ResetToken with ResetTokenIssueDate
    // 4. Sends email: {SiteRootUrl}#/password/reset/{token}
}
catch (InvalidDataProvidedException ex)
{
    // "Email does not exist" or "Email id does not exists. Please update your email or contact your administrator."
}

// Step 2: User clicks link, check token validity
string tokenFromUrl = Request.QueryString["token"];
bool isValid = passwordResetService.ResetPasswordExpire(new ChangePasswordEditModel 
{ 
    Key = tokenFromUrl 
});

if (!isValid)
{
    return View("ResetLinkExpired");  // Token > 24 hours old
}

// Step 3: User submits new password
var resetModel = new ChangePasswordEditModel
{
    Key = tokenFromUrl,
    Password = "NewSecurePassword456!"
};

bool success = passwordResetService.ResetPassword(resetModel);
// UserLoginFactory.CreateResetPasswordDomain():
// 1. Hashes new password with new salt
// 2. Updates UserLogin.Password and UserLogin.Salt
// 3. Clears ResetToken and ResetTokenIssueDate
// 4. Persists changes

if (success)
{
    return RedirectToAction("Login", new { message = "Password reset successful" });
}
```

### Factory Usage Examples

```csharp
// Person domain to edit model (for displaying in form)
var person = personRepository.Get(userId);
var editModel = personFactory.CreateModel(person);
// editModel.Name.FirstName = person.FirstName
// editModel.PhoneNumbers = [converted Phone entities]
// editModel.Address = [converted Address entities]

// Edit model to domain (after form submission)
var updatedPerson = personFactory.CreateDomain(editModel);
// updatedPerson.FirstName = editModel.Name.FirstName
// updatedPerson.Phones = [converted PhoneEditModel objects]
// updatedPerson.Addresses = [converted AddressEditModel objects]
personRepository.Save(updatedPerson);

// UserLogin creation with password hashing
var userLoginEditModel = new UserLoginEditModel
{
    UserName = "test@example.com",
    Password = "TempPassword123",
    ChangePassword = true,
    SendUserLoginViaEmail = false
};

var person = new Person { Id = 123 };
var existingUserLogin = userLoginRepository.Get(123);

var userLogin = userLoginFactory.CreateDomain(userLoginEditModel, person, existingUserLogin);
// userLogin.UserName = "test@example.com" (lowercase)
// userLogin.Password = "{hashed-value}"
// userLogin.Salt = "{random-salt}"
// userLogin.IsLocked = false
```

### Validator Usage Examples

```csharp
// Standalone email uniqueness check
bool isUnique = uniqueEmailValidator.IsValid(personId: 0, email: "new@example.com");
if (!isUnique)
{
    ModelState.AddModelError("Email", "Email already exists");
}

// FluentValidation integration (automatic)
var userEditModel = new UserEditModel { /* ... */ };
var validator = new UserEditModelValidator();
var result = validator.Validate(userEditModel);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }
    return View(userEditModel);
}

// LoginAuthenticationModelValidator (orchestrates authentication)
var loginModel = new LoginAuthenticationModel 
{ 
    Username = "user@example.com", 
    Password = "wrongpassword" 
};

bool isValid = loginAuthValidator.IsValid(loginModel);
// On failure, loginModel.Message is populated:
if (!isValid)
{
    Console.WriteLine(loginModel.Message.Text);
    // Examples:
    // - "Invalid credentials"
    // - "Account locked after 5 failed attempts"
    // - "Invalid credentials - 2 attempts remaining"
}
```

### Session Management

```csharp
// Validate session token (on subsequent requests)
string sessionToken = Request.Headers["Authorization"];

if (!userLogService.IsTokenValid(sessionToken))
{
    return Unauthorized("Session expired or invalid");
}

long userId = userLogService.GetUserId(sessionToken);
// Use userId for authorization checks

// Logout - end session
userLogService.EndLoggedinSession(sessionToken);
// Sets UserLog.LoggedOutAt = DateTime.UtcNow

// Check first-time login (show onboarding)
if (userLogService.IsFirstTimeLogin(userId))
{
    return RedirectToAction("Onboarding");
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Implementation Summary

### Services
| Class | Primary Responsibility | Key Dependencies |
|-------|------------------------|------------------|
| `UserService` | User lifecycle (CRUD, roles, profiles) | 14 repositories, 4 factories, UserLoginService, PhoneService |
| `UserLoginService` | Authentication state, login attempts | UserLogin repository, IClock, IJobService |
| `PasswordResetService` | Password reset tokens and workflow | Person repository, UserLoginFactory, IUserNotificationModelFactory |
| `UserLogService` | Session tracking, audit logs | UserLog/CustomerLog repositories, IClock |
| `SendLoginCredentialsService` | Email login credentials | IUserNotificationModelFactory |

### Factories
| Class | Transformation | Key Logic |
|-------|----------------|-----------|
| `PersonFactory` | Person ↔ PersonEditModel | Address/Phone collection mapping, Name value object |
| `UserLoginFactory` | UserLogin creation | Password hashing, random password generation |
| `UserFactory` | OrganizationRoleUser → UserViewModel | Role-specific entity creation (SalesRep) |

### Validators
| Class | Validation Type | Rules |
|-------|-----------------|-------|
| `LoginAuthenticationModelValidator` | Authentication credentials | Username exists, not locked, active, password hash match |
| `UniqueEmailValidator` | Email uniqueness | Format validation + database uniqueness check |
| `UserEditModelValidator` | User form data | Required fields, email format, role assignment |
| `PersonEditModelValidator` | Person data | Name parts, address completeness |
| `UserLoginEditModelValidator` | Login credentials | Username uniqueness, password strength |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Password hashing fails with null reference exception
**Cause**: `ICryptographyOneWayHashService` not registered in IoC container or password is null.
**Solution**: Ensure password is not null/empty before passing to factory. Check IoC registration for cryptography service.

### "Email already exists" even for new users
**Cause**: `UniqueEmailValidator` using `ApplicationManager.DependencyInjection.Resolve<>()` may resolve stale instance.
**Solution**: Pass `personId = 0` for new users to exclude from uniqueness check. For updates, pass existing `personId`.

### Account locks after one failed attempt instead of five
**Cause**: `LoginAttemptCount` not being reset on successful login.
**Solution**: Ensure `UpdateforValidAttempt()` is called on successful authentication. Check database for pre-existing high attempt count.

### Password reset link works after 24 hours
**Cause**: Clock drift between application server and database server.
**Solution**: Use `IClock.UtcNow` consistently. Verify database server timezone matches application timezone.

### User can't log in despite correct password
**Cause**: `IsLocked = true` or no active `OrganizationRoleUser` records.
**Solution**: Check `UserLogin.IsLocked` and `OrganizationRoleUser.IsActive WHERE IsDefault = true`. Use `UserLoginService.Lock()` to unlock manually.

### Session token validation fails immediately after login
**Cause**: `UserLog.LoggedOutAt` set prematurely or session token mismatch.
**Solution**: Verify session token from `SaveLoginSession()` matches token in subsequent `IsTokenValid()` calls. Check for duplicate session IDs.

### PersonFactory throws null reference on Address/Phone
**Cause**: `IAddressFactory` or `IPhoneFactory` not injected or null collections in edit model.
**Solution**: Ensure factories are registered in IoC. Initialize empty collections if null before calling `CreateDomain()`.

### Validator using wrong database context
**Cause**: `ApplicationManager.DependencyInjection.Resolve<>()` in `UniqueEmailValidator` may resolve different UnitOfWork.
**Solution**: Consider passing `IUserLoginService` directly to validator constructor instead of runtime resolution.

### Multi-franchisee assignment doesn't persist
**Cause**: `IsDefault` flag missing or all franchisee assignments have `IsActive = false`.
**Solution**: Ensure at least one `OrganizationRoleUserFranchisee` has `IsDefault = true` and `IsActive = true`.
<!-- END CUSTOM SECTION -->
