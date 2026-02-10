<!-- AUTO-GENERATED: Header -->
# Users/ViewModels
> Data Transfer Objects (DTOs) for user management operations - forms, display, authentication, filtering
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

ViewModels are the "messengers" of your application—they carry data between layers without exposing internal domain complexity. Think of them as specialized envelopes designed for specific purposes: one for login forms, one for user profiles, one for search filters, etc.

### Why Separate ViewModels from Domain Entities?

**Problem**: Directly exposing domain entities (`Person`, `UserLogin`) to UI/API creates tight coupling:
- UI changes force database schema changes
- Sensitive data (password hashes) exposed to client
- Validation rules mixed with business logic
- Circular reference issues in serialization

**Solution**: ViewModels as intermediaries:
- UI-friendly property names and structure
- Only necessary fields (no password hashes in display models)
- Validation attributes tailored to forms
- Flat structure for easy serialization

### View Model Categories

**Edit Models** (`*EditModel`): Bidirectional—UI forms ↔ domain entities
- Example: `UserEditModel` for user registration form, transforms to `Person` + `UserLogin` entities

**View Models** (`*ViewModel`): Read-only—domain entities → UI display
- Example: `UserViewModel` for user list grid, optimized for rendering

**Filter Models** (`*Filter`): Query parameters—UI filters → database queries
- Example: `UserListFilter` with search text, role filter, sort order

**Session Models**: Authentication context—stored in session/JWT, used for authorization
- Example: `UserSessionModel` with user ID, role, organization, preferences
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### User Registration Form

```csharp
// Client submits registration form
var formData = new UserEditModel
{
    PersonEditModel = new PersonEditModel
    {
        Name = new Name("Alice", "M", "Developer"),
        Email = "alice@example.com",
        PhoneNumbers = new List<PhoneEditModel>
        {
            new PhoneEditModel { Phone = "(555) 123-4567", PhoneTypeId = (long)PhoneType.Cell }
        },
        Address = new List<AddressEditModel>
        {
            new AddressEditModel { Line1 = "123 Main St", City = "Columbus", State = "OH", Zip = "43215" }
        }
    },
    UserLoginEditModel = new UserLoginEditModel
    {
        UserName = "alice@example.com",
        Password = "TempPassword123!",
        ConfirmPassword = "TempPassword123!",
        SendUserLoginViaEmail = true
    },
    OrganizationId = franchiseeId,
    RoleId = (long)RoleType.Technician,
    CreateLogin = true
};

// Server-side processing
var validator = new UserEditModelValidator();
var validationResult = validator.Validate(formData);

if (!validationResult.IsValid)
{
    return BadRequest(validationResult.Errors);
}

// Transform to domain entities and save
userService.Save(formData);
// Internally: PersonFactory.CreateDomain(PersonEditModel)
//             UserLoginFactory.CreateDomain(UserLoginEditModel) → password hashing
//             Repository.Save(Person + UserLogin + OrganizationRoleUser)
//             SendLoginCredentialsService (if SendUserLoginViaEmail = true)
```

### Login Flow

```csharp
// Client submits login form
var loginRequest = new LoginAuthenticationModel
{
    Username = "alice@example.com",
    Password = "TempPassword123!"
};

// Validate credentials
if (!loginAuthValidator.IsValid(loginRequest))
{
    // loginRequest.Message populated with error:
    // - "Invalid credentials"
    // - "Account locked"
    // - "User deactivated"
    return Unauthorized(loginRequest.Message.Text);
}

// Create session
var userLogin = userLoginService.GetbyUserName(loginRequest.Username);
var session = BuildUserSession(userLogin);  // → UserSessionModel

// Store session (choose one approach)
HttpContext.Session.Set("UserSession", session);  // Server-side session
// OR
var jwt = GenerateJwt(session);  // JWT token
return Ok(new { token = jwt, user = session });

// Subsequent requests use session for authorization
var currentSession = HttpContext.Session.Get<UserSessionModel>("UserSession");
bool canManageUsers = currentSession.RoleId == (long)RoleType.SuperAdmin ||
                      currentSession.RoleId == (long)RoleType.FranchiseeAdmin;
```

### User Profile Display

```csharp
// Load user for display (detail page)
var userViewModel = userService.GetUserDetails(userId);
// Returns UserViewModel with:
// - User name, email, username
// - Role name (e.g., "Technician")
// - Organization/franchisee name
// - Active/locked status
// - Last login date
// - Address and phone numbers

// Render in view
@model UserViewModel
<h1>@Model.Name.FullName</h1>
<p>Email: @Model.Email</p>
<p>Role: @Model.Role (@Model.FranchiseeName)</p>
<p>Status: @(Model.IsLocked ? "Locked" : Model.IsActive ? "Active" : "Inactive")</p>
<p>Last Login: @Model.LastLoggedIn?.ToString("g")</p>
```

### User List with Filtering

```csharp
// Build filter from UI inputs
var filter = new UserListFilter
{
    Text = Request.Query["search"],  // Search name, email, username
    FranchiseeId = currentUser.OrganizationId,
    RoleId = Request.Query["roleFilter"],  // Optional role filter
    IsActive = true,  // Active users only
    SortingColumn = Request.Query["sortBy"] ?? "LastName",
    SortingOrder = Request.Query["sortDesc"] == "true" ? (long)SortingOrder.Desc : (long)SortingOrder.Asc
};

var result = userService.GetUsers(filter, pageNumber: pageNum, pageSize: 25);
// Returns UserListModel:
// - result.Users: IEnumerable<UserViewModel>
// - result.TotalCount: Total matching records
// - result.PageNumber: Current page
// - result.TotalPages: Calculated from TotalCount / PageSize

// Render paginated grid
foreach (var user in result.Users)
{
    // Display user row
}
// Render pagination controls using result.TotalPages
```

### Password Reset Workflow

```csharp
// Step 1: User requests reset
[HttpPost("api/password/forgot")]
public IActionResult ForgotPassword(string email)
{
    passwordResetService.SendPasswordLink(email);
    // Generates GUID token, stores in UserLogin.ResetToken
    // Emails link: {SiteRootUrl}#/password/reset/{token}
    return Ok("Reset link sent to email");
}

// Step 2: User clicks link, validate token
[HttpGet("api/password/validate/{token}")]
public IActionResult ValidateResetToken(string token)
{
    var model = new ChangePasswordEditModel { Key = token };
    bool isValid = passwordResetService.ResetPasswordExpire(model);
    
    if (!isValid)
    {
        return BadRequest("Reset link expired or invalid");
    }
    
    return Ok("Token valid");
}

// Step 3: User submits new password
[HttpPost("api/password/reset")]
public IActionResult ResetPassword([FromBody] ChangePasswordEditModel model)
{
    // model.Key = token from URL
    // model.Password = new password (plaintext)
    
    bool success = passwordResetService.ResetPassword(model);
    // Validates token < 24 hours old
    // Hashes new password
    // Clears token from UserLogin
    
    if (success)
    {
        return Ok("Password reset successful");
    }
    else
    {
        return BadRequest("Invalid or expired token");
    }
}
```

### Session-Based Authorization

```csharp
// Middleware or action filter
public class RequireRoleAttribute : ActionFilterAttribute
{
    private readonly RoleType[] _allowedRoles;
    
    public RequireRoleAttribute(params RoleType[] roles)
    {
        _allowedRoles = roles;
    }
    
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session.Get<UserSessionModel>("UserSession");
        
        if (session == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        if (!_allowedRoles.Any(role => session.RoleId == (long)role))
        {
            context.Result = new ForbidResult();
        }
    }
}

// Usage on controller actions
[RequireRole(RoleType.SuperAdmin, RoleType.FranchiseeAdmin)]
[HttpDelete("api/users/{id}")]
public IActionResult DeleteUser(long id)
{
    userService.Delete(id);
    return NoContent();
}
```

### Email Signature Management

```csharp
// Load user's signatures
var signatures = userService.GetUserSignature(orgId: currentOrgId, userId: currentUserId);

// User creates new signature
var newSignature = new UserSignatureSaveModel
{
    SignatureName = "Professional",
    Signature = "<p><strong>Alice Developer</strong><br/>Technician<br/>alice@example.com</p>",
    IsDefault = true  // Make this the default signature
};

var saveModel = new UserSignatureListSaveModel
{
    Signatures = new[] { newSignature }
};

userService.SaveUserSignature(saveModel, orgId: currentOrgId, userId: currentUserId);
// Deactivates old default signature if IsDefault = true
// Persists new EmailSignatures entity
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## View Model Summary

### Edit Models (Forms → Domain)
| Model | Purpose | Key Properties | Validated By |
|-------|---------|----------------|--------------|
| `UserEditModel` | User registration/update form | PersonEditModel, UserLoginEditModel, RoleIds, OrganizationIds | UserEditModelValidator |
| `PersonEditModel` | Person contact info form | Name, Email, PhoneNumbers, Address | PersonEditModelValidator |
| `UserLoginEditModel` | Login credentials form | UserName, Password, ConfirmPassword, SendUserLoginViaEmail | UserLoginEditModelValidator |
| `ChangePasswordEditModel` | Password reset form | Key (token), Password | None (validated in service) |
| `OrganizationOwnerEditModel` | Organization owner creation | OwnerFirstName, OwnerLastName, Password | N/A |

### View Models (Domain → Display)
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `UserViewModel` | User display (lists/details) | UserId, Name, Email, Role, IsActive, IsLocked, LastLoggedIn |
| `UserSessionModel` | Authenticated session context | UserId, RoleId, OrganizationId, RoleName, TimeZoneId, CurrencyCode |

### Filter/Query Models
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `UserListFilter` | User list search/filter | Text, FranchiseeId, RoleId, IsActive, SortingColumn, SortingOrder |
| `UserListModel` | Paginated user list wrapper | Users (IEnumerable<UserViewModel>), TotalCount, PageNumber, PageSize |

### Authentication Models
| Model | Purpose | Key Properties |
|-------|---------|----------------|
| `LoginAuthenticationModel` | Login form submission | Username, Password, Message (errors) |
| `LoginCustomerAuthenticationModel` | Customer portal login | Code (auth code), Message |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Object reference not set" when accessing nested model
**Cause**: Nested edit models (PersonEditModel, UserLoginEditModel) not initialized in constructor.
**Solution**: Always initialize nested models in `UserEditModel` constructor:
```csharp
public UserEditModel()
{
    PersonEditModel = new PersonEditModel();
    UserLoginEditModel = new UserLoginEditModel();
}
```

### Validation errors not displaying for nested properties
**Cause**: FluentValidation not configured to validate nested models.
**Solution**: Use `RuleFor(x => x.PersonEditModel).SetValidator(new PersonEditModelValidator())` in parent validator.

### UserSessionModel serialization fails
**Cause**: Circular references or complex types in session model.
**Solution**: 
- Avoid storing navigation properties (Person.UserLogin, etc.) in session
- Use `[XmlIgnore]` or `[JsonIgnore]` on properties not needed in serialization
- Consider storing only UserSessionModel.UserId and loading full context on-demand

### Password reset token always invalid
**Cause**: Token case mismatch (GUIDs are case-sensitive in some comparisons) or token already used.
**Solution**: 
- Use case-insensitive string comparison: `token.Equals(storedToken, StringComparison.OrdinalIgnoreCase)`
- Clear token after successful reset to prevent reuse
- Check `ResetTokenIssueDate` calculation against server time zone

### User list filter returns no results
**Cause**: Multiple filters applied with AND logic may be too restrictive.
**Solution**: Apply filters incrementally:
```csharp
if (!string.IsNullOrEmpty(filter.Text)) query = query.Where(...);
if (filter.RoleId > 0) query = query.Where(...);
if (filter.IsActive.HasValue) query = query.Where(...);
```

### Email uniqueness validation fails on update
**Cause**: Not passing existing `PersonId` to `UniqueEmailValidator`.
**Solution**: 
```csharp
bool isUnique = uniqueEmailValidator.IsValid(personId: existingUser.PersonId, email: newEmail);
```

### UserEditModel.RoleIds empty after form submission
**Cause**: Multi-select form input not binding to `ICollection<long>`.
**Solution**: Ensure form sends array of long values, not comma-separated string. Use model binder or custom conversion.

### Session expires immediately after login
**Cause**: Session cookie not persisted or session timeout too short.
**Solution**: Configure session timeout in web.config/appsettings.json. Ensure cookies are enabled and HTTPS configured for secure cookies.
<!-- END CUSTOM SECTION -->
