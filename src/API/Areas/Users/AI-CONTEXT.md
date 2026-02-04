# API/Areas/Users - AI Context

## Purpose

The **Users** area handles authentication, user management, role assignment, and access control. It provides endpoints for login, registration, password management, and user profile operations.

## Key Functionality

### Authentication
- User login with email/password
- Token generation and validation
- Token refresh
- Password reset workflow
- Logout and session termination

### User Management
- User registration
- Profile updates
- Role assignment
- Organization assignment
- User activation/deactivation

### Access Control
- Role-based permissions
- Organization-based access
- Feature flags
- Permission validation

## Key Controllers

### LoginController.cs
Handles authentication operations.

**Endpoints**:
- `POST /Users/Login` - Authenticate user and generate token
- `POST /Users/Login/Refresh` - Refresh authentication token
- `POST /Users/Login/Logout` - Terminate session
- `POST /Users/Login/ForgotPassword` - Initiate password reset
- `POST /Users/Login/ResetPassword` - Complete password reset

**Login Flow**:
```
1. Client sends credentials
2. Server validates credentials
3. Server generates token
4. Token includes: UserId, RoleId, OrganizationId, Expiration
5. Client stores token
6. Client includes token in all subsequent requests
```

### UserController.cs
User profile and management operations.

**Endpoints**:
- `GET /Users/User/{id}` - Get user profile
- `GET /Users/User/GetList` - Get user list (admin only)
- `POST /Users/User` - Create/update user
- `POST /Users/User/ChangePassword` - Change user password
- `POST /Users/User/UpdateProfile` - Update user profile
- `DELETE /Users/User/{id}` - Deactivate user

### RoleController.cs
Role management and assignment.

**Endpoints**:
- `GET /Users/Role` - Get available roles
- `POST /Users/Role/Assign` - Assign role to user
- `GET /Users/Role/GetPermissions` - Get role permissions

## Key ViewModels

```csharp
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserViewModel User { get; set; }
}

public class UserViewModel
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public long RoleId { get; set; }
    public string RoleName { get; set; }
    public long? OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
```

## Roles

### SuperAdmin
- Full system access
- Manage all franchisees
- System configuration
- User management across all organizations

### FranchiseeAdmin
- Manage their franchisee
- Manage franchisee users
- View franchisee data
- Cannot access other franchisees

### Technician
- View assigned jobs
- Update job status
- Capture photos and signatures
- View customer information for assigned jobs

### ReadOnly
- View-only access
- Cannot modify data
- Typically for reporting purposes

### Customer (if applicable)
- Book appointments
- View their jobs and invoices
- Update profile
- Make payments

## Security

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

### Token Management
- JWT tokens with expiration
- Tokens stored in database for validation
- Tokens expire after configurable period (default: 24 hours)
- Refresh tokens for extended sessions
- Tokens invalidated on logout or password change

### Session Security
- HTTPS required in production
- Token transmitted in header (not URL)
- Failed login attempt tracking
- Account lockout after multiple failed attempts
- IP address logging for security audit

## Business Rules

- Email must be unique across system
- Users must belong to at least one organization
- Users must have exactly one role
- Cannot delete users with historical data
- Deactivated users cannot login but data is retained
- Password reset links expire after 24 hours
- New users receive welcome email with setup instructions

## Authorization Patterns

### Endpoint Authorization
```csharp
[BasicAuthentication]
public class UserController : BaseController
{
    [HttpGet]
    public UserViewModel Get(long id)
    {
        // Users can view their own profile
        if (id == _sessionContext.UserSession.UserId)
            return _userService.Get(id);
        
        // Admins can view users in their organization
        if (_sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin)
        {
            var user = _userService.Get(id);
            if (user.OrganizationId == _sessionContext.UserSession.OrganizationId)
                return user;
        }
        
        // Super admins can view any user
        if (_sessionContext.UserSession.RoleId == (long)RoleType.SuperAdmin)
            return _userService.Get(id);
        
        throw new UnauthorizedAccessException();
    }
}
```

## Integration Points

- **Organizations**: User-franchisee assignments
- **Scheduler**: Technician assignments to jobs
- **Sales**: User activity tracking
- **Notification**: User notifications and alerts
- **All Areas**: Authentication and authorization

## Testing

```csharp
[Test]
public void Login_ValidCredentials_ReturnsToken()
{
    var request = new LoginRequest
    {
        Email = "test@example.com",
        Password = "Test123!"
    };
    
    var response = _loginController.Login(request);
    
    Assert.IsNotNull(response.Token);
    Assert.IsTrue(response.ExpiresAt > DateTime.Now);
}

[Test]
public void Login_InvalidCredentials_ThrowsException()
{
    var request = new LoginRequest
    {
        Email = "test@example.com",
        Password = "WrongPassword"
    };
    
    Assert.Throws<UnauthorizedException>(() => 
        _loginController.Login(request)
    );
}
```
