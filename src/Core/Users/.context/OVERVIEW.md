<!-- AUTO-GENERATED: Header -->
# Users Module
> Complete user lifecycle management including authentication, authorization, role-based access control, and person/contact management
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Users module is the security foundation of the Marble Life application, managing who can access the system and what they can do. Think of it as the "bouncer and ID system" for your application—it checks credentials at the door (authentication), verifies access levels (authorization), and keeps track of everyone's contact information.

### Key Concepts

**Person vs UserLogin**: The module separates "who you are" (Person—name, email, phone) from "how you log in" (UserLogin—username, password). This allows contacts to exist in the system without login access (customers, leads) while enabling secure authentication for staff.

**Multi-Organization Roles**: Users can have different roles in different organizations. A technician at Franchisee A might be an Operations Manager at Franchisee B. The `OrganizationRoleUser` table manages these relationships.

**Security-First Design**: Passwords are never stored in plaintext. Each password is hashed with a unique salt, making rainbow table attacks ineffective. Failed login attempts are tracked, and accounts automatically lock after 5 failures.

### Why This Architecture?

- **Separation of Concerns**: Person data (contact info) changes independently from authentication credentials
- **Scalability**: Multi-organization support without duplicating user records
- **Auditability**: Soft deletes and login tracking maintain complete audit trails
- **Security**: Salted password hashing, account lockout, time-limited reset tokens
- **Flexibility**: Factory pattern allows different authentication strategies (standard, customer codes, API tokens)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Creating a New User

```csharp
// Build user model
var userModel = new UserEditModel
{
    PersonEditModel = new PersonEditModel
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@marblelife.com",
        PhoneNumbers = new List<PhoneNumberEditModel>
        {
            new PhoneNumberEditModel { Phone = "(555) 123-4567", PhoneTypeId = (long)PhoneType.Cell }
        }
    },
    UserLoginEditModel = new UserLoginEditModel
    {
        UserName = "john.doe@marblelife.com",
        Password = "TempPassword123!",
        SendUserLoginViaEmail = true  // Email credentials to user
    },
    OrganizationId = franchiseeId,
    RoleId = (long)RoleType.Technician,
    CreateLogin = true
};

// Save user (validates, hashes password, creates entities, sends email)
userService.Save(userModel);
```

### Authenticating a User

```csharp
// Create authentication request
var loginModel = new LoginAuthenticationModel
{
    Username = "john.doe@marblelife.com",
    Password = "TempPassword123!"
};

// Validate credentials
if (loginAuthValidator.IsValid(loginModel))
{
    // Get user session data
    var user = userLoginService.GetbyUserName(loginModel.Username);
    var sessionId = Guid.NewGuid().ToString();
    
    // Log the session
    userLogService.SaveLoginSession(
        user.Id,
        sessionId,
        clientIp: Request.UserHostAddress,
        loginDateTime: DateTime.UtcNow,
        browser: "Chrome",
        os: "Windows 10"
    );
    
    // Return session token to client
    return new UserSessionModel { UserId = user.Id, Token = sessionId };
}
else
{
    // loginModel.Message contains error:
    // "Invalid credentials", "Account locked", or "User deactivated"
    throw new UnauthorizedException(loginModel.Message.Text);
}
```

### Password Reset Workflow

```csharp
// Step 1: User requests reset
passwordResetService.SendPasswordLink("john.doe@marblelife.com");
// Email sent with link: https://app.marblelife.com#/password/reset/{guid-token}

// Step 2: User clicks link, your UI checks if token is valid
bool isValid = passwordResetService.ResetPasswordExpire(new ChangePasswordEditModel 
{ 
    Key = tokenFromUrl 
});

if (!isValid)
{
    ShowMessage("Reset link has expired. Please request a new one.");
    return;
}

// Step 3: User submits new password
bool success = passwordResetService.ResetPassword(new ChangePasswordEditModel
{
    Key = tokenFromUrl,
    Password = "NewSecurePassword456!"
});
// Password is hashed, token is cleared, user can now log in
```

### Managing User Access

```csharp
// Get users for a franchisee with filtering
var filter = new UserListFilter
{
    OrganizationId = franchiseeId,
    RoleType = RoleType.Technician,
    SearchText = "john",  // Searches name, email, username
    ShowActive = true,
    ShowInactive = false
};

var users = userService.GetUsers(filter, pageNumber: 1, pageSize: 25);

// Lock a user account (manual intervention required to unlock)
bool lockResult;
bool isEquipment;
userLoginService.Lock(userId, isLocked: true, out lockResult, out isEquipment);

// Soft-delete a user (deactivates all organization roles)
userService.Delete(userId);

// Assign user to multiple franchisees
userService.ManageAccount(userId, new long[] { franchisee1Id, franchisee2Id, franchisee3Id });
```

### Working with Email Signatures

```csharp
// Get user's signatures
var signatures = userService.GetUserSignature(orgId: franchiseeId, userId: userId);

// Save new signature
var signatureModel = new UserSignatureListSaveModel
{
    Signatures = new List<UserSignatureSaveModel>
    {
        new UserSignatureSaveModel
        {
            SignatureName = "Professional",
            Signature = "<p><strong>John Doe</strong><br/>Technician<br/>Marble Life<br/>Phone: (555) 123-4567</p>",
            IsDefault = true
        }
    }
};

userService.SaveUserSignature(signatureModel, orgId: franchiseeId, userId: userId);
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### User Management (IUserService)
| Method | Description |
|--------|-------------|
| `Get(userId, franchiseeId)` | Retrieve user profile with person, login, and role data |
| `Save(UserEditModel)` | Create or update user with validation and password hashing |
| `Delete(userId)` | Soft-delete user by deactivating all organization roles |
| `GetUsers(filter, page, size)` | Paginated user list with filtering by role, org, search text |
| `GetUserDetails(userId)` | Get user view model with role and organization info |
| `ManageAccount(userId, franchiseeIds)` | Assign user to multiple franchisees |
| `SaveUserSignature(model, orgId, userId)` | Save email signature for user |

### Authentication (IUserLoginService)
| Method | Description |
|--------|-------------|
| `GetbyUserName(username)` | Retrieve UserLogin by username (case-insensitive) |
| `GetbyUserId(userId)` | Retrieve UserLogin by user ID |
| `UpdateforInvalidAttempt(login)` | Increment attempt count, lock after 5 failures |
| `UpdateforValidAttempt(login)` | Reset attempt count, update last login date |
| `IsUniqueUserName(username, userId)` | Check username availability |
| `IsUniqueEmailAddress(email, userId)` | Check email availability |
| `Lock(userId, isLocked, out lockResult, out isEquipment)` | Manually lock/unlock user account |

### Password Management (IPasswordResetService)
| Method | Description |
|--------|-------------|
| `SendPasswordLink(email)` | Generate reset token and email reset link (24-hour expiration) |
| `ResetPassword(model)` | Validate token and set new password |
| `ResetPasswordExpire(model)` | Check if reset token is still valid |

### Validation (ILoginAuthenticationModelValidator)
| Method | Description |
|--------|-------------|
| `IsValid(model)` | Validate login credentials, update attempt count |
| `IsValidForReviewAPI(model)` | Validate credentials without updating last login date |
| `IsValidForCustomer(model)` | Validate customer authentication code |

### Session Management (IUserLogService)
| Method | Description |
|--------|-------------|
| `SaveLoginSession(userId, sessionId, clientIp, loginDateTime, browser, os)` | Create audit log for login event |
| `IsTokenValid(token)` | Check if session token is active |
| `GetUserId(token)` | Get user ID from session token |
| `EndLoggedinSession(token)` | Mark session as ended (logout) |

### Factories
| Factory | Description |
|---------|-------------|
| `IUserFactory` | Create user view models and domain entities |
| `IPersonFactory` | Create person view models and domain entities |
| `IUserLoginFactory` | Create UserLogin with password hashing |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Account locked" error
**Cause**: User entered wrong password 5 times.
**Solution**: Use `IUserLoginService.Lock(userId, false, ...)` to manually unlock the account. Login attempt count will reset on next successful login.

### Password reset link not working
**Cause**: Token expired (24-hour limit) or token already used.
**Solution**: User must request a new reset link via `SendPasswordLink()`. Each token is single-use.

### User can't log in despite correct credentials
**Check**:
1. Is `UserLogin.IsActive = true`?
2. Does user have at least one `OrganizationRoleUser` with `IsActive = true`?
3. Is account locked (`UserLogin.IsLocked = true`)?
4. Has user been soft-deleted (all org roles deactivated)?

### Duplicate email error
**Cause**: Email must be unique across all Person records.
**Solution**: Check `IUniqueEmailValidator.IsValid()` before saving. If updating existing user, pass their `personId` to exclude them from uniqueness check.

### Phone number validation fails
**Cause**: Phone parsing expects at least 10 digits (3-digit area code + 7-digit number).
**Solution**: Use format "(555) 123-4567" or "5551234567". Extension format: "x890" or "#890".

### Multi-organization user sees wrong role
**Cause**: Role context not scoped to current organization.
**Solution**: Always filter `OrganizationRoleUser` by `OrganizationId` when checking roles. Use `IUserService.Get(userId, franchiseeId)` to scope by organization.
<!-- END CUSTOM SECTION -->
