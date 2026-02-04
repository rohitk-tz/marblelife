# Core/Users - AI Context

## Purpose

The **Users** module manages user authentication, authorization, roles, permissions, and user profile management for the MarbleLife platform. It handles all aspects of user identity and access control.

## Key Entities (Domain/)

### User Management
- **User**: Core user entity with authentication credentials
- **UserProfile**: Extended user information and preferences
- **UserRole**: Role assignments (Admin, Franchisee, Technician, Customer, etc.)
- **UserPermission**: Granular permissions
- **UserSession**: Active user sessions and tokens
- **UserActivity**: Activity tracking and audit logs

### Authentication
- **LoginHistory**: Login attempts and history
- **PasswordReset**: Password reset token management
- **TwoFactorAuth**: 2FA configuration and verification
- **RefreshToken**: Token refresh for authentication
- **AuthenticationProvider**: OAuth/external auth providers

### User Settings
- **UserPreference**: User-specific settings
- **NotificationPreference**: Communication preferences
- **UserDevice**: Registered devices for push notifications
- **UserAvatar**: Profile picture management

## Service Interfaces

### Core User Services
- **IUserFactory**: User creation with validation
- **IUserService**: User CRUD operations
- **IUserProfileService**: Profile management
- **IUserRoleService**: Role assignment and management
- **IUserPermissionService**: Permission checks

### Authentication Services
- **IAuthenticationService**: Login/logout operations
- **IPasswordService**: Password management and validation
- **IPasswordHashService**: Secure password hashing
- **ITwoFactorAuthService**: 2FA setup and verification
- **ITokenService**: JWT token generation and validation
- **IRefreshTokenService**: Token refresh management

### Authorization Services
- **IAuthorizationService**: Permission checking
- **IRoleService**: Role definition and management
- **IPermissionService**: Permission management
- **IAccessControlService**: Resource-level access control

### User Management
- **IUserSearchService**: User lookup and search
- **IUserActivityService**: Activity tracking
- **IUserDeviceService**: Device management
- **IUserPreferenceService**: Settings management

## Implementations (Impl/)

Business logic including:
- BCrypt password hashing
- JWT token generation and validation
- Role-based access control (RBAC)
- Session management
- Failed login detection and lockout
- Password strength validation
- 2FA implementation (TOTP)

## Enumerations (Enum/)

- **UserRole**: Admin, FranchiseeOwner, FranchiseeManager, Technician, Customer, Support
- **UserStatus**: Active, Inactive, Suspended, PendingVerification, Locked
- **PermissionType**: Read, Write, Delete, Execute, Admin
- **LoginResult**: Success, InvalidCredentials, AccountLocked, RequiresTwoFactor, PasswordExpired
- **AuthProviderType**: Local, Google, Facebook, Microsoft
- **DeviceType**: Web, iOS, Android, API

## ViewModels (ViewModel/)

- **UserViewModel**: Complete user data
- **UserProfileViewModel**: Profile information
- **LoginViewModel**: Login credentials
- **RegisterViewModel**: New user registration
- **PasswordResetViewModel**: Password reset request
- **TwoFactorViewModel**: 2FA setup and verification
- **UserPermissionViewModel**: Permission assignments
- **UserRoleViewModel**: Role details

## Business Rules

### User Registration
1. Email must be unique across the system
2. Password must meet complexity requirements (8+ chars, upper, lower, number, special)
3. Email verification required for activation
4. Default role assigned based on registration type
5. Franchisee users require franchisee approval

### Authentication
1. Maximum 5 failed login attempts before account lockout
2. Lockout duration: 15 minutes
3. Session timeout: 8 hours of inactivity
4. JWT tokens expire after 1 hour (refresh token: 7 days)
5. 2FA required for admin and franchisee accounts
6. Password must be changed every 90 days for admin users

### Authorization
1. Hierarchical role structure: Admin > Franchisee Owner > Manager > Technician
2. Franchisee users can only access their own data
3. Admin users have full system access
4. Technicians have read-only access to schedule and customers
5. Customers have access only to their own data

### Password Management
1. Passwords hashed using BCrypt (cost factor: 12)
2. Password history maintained (last 5 passwords)
3. Cannot reuse previous 5 passwords
4. Password reset links valid for 1 hour
5. Force password change on first login

## Dependencies

- **Core/Organizations**: Franchisee user associations
- **Core/Notification**: Email/SMS for verification and alerts
- **Infrastructure**: Password hashing, token generation
- **Core/Application**: Session management

## For AI Agents

### User Registration
```csharp
// Register new user
var user = _userFactory.Create(new RegisterViewModel
{
    Email = "tech@example.com",
    Password = "SecureP@ssw0rd",
    FirstName = "John",
    LastName = "Smith",
    Phone = "(555) 123-4567",
    Role = UserRole.Technician,
    FranchiseeId = franchiseeId
});

// Send verification email
_authenticationService.SendVerificationEmail(user.Id);
```

### Authentication Flow
```csharp
// Login
var loginResult = await _authenticationService.Login(new LoginViewModel
{
    Email = "user@example.com",
    Password = "password",
    RememberMe = true
});

if (loginResult.RequiresTwoFactor)
{
    // Send 2FA code
    _twoFactorAuthService.SendCode(loginResult.UserId);
    
    // Verify code
    var verified = _twoFactorAuthService.VerifyCode(loginResult.UserId, userEnteredCode);
    
    if (verified)
    {
        // Complete login
        var tokens = _tokenService.GenerateTokens(user);
    }
}
else if (loginResult.Success)
{
    // Generate JWT tokens
    var accessToken = _tokenService.GenerateAccessToken(user);
    var refreshToken = _tokenService.GenerateRefreshToken(user);
    
    // Track login
    _userActivityService.LogLogin(user.Id, ipAddress, userAgent);
}
```

### Authorization Check
```csharp
// Check if user has permission
var hasPermission = _authorizationService.CheckPermission(
    userId, 
    PermissionType.Write, 
    "Scheduler.Job"
);

if (!hasPermission)
{
    throw new UnauthorizedException("User lacks permission to modify jobs");
}

// Check if user can access resource
var canAccessCustomer = _accessControlService.CanAccessResource(
    userId, 
    ResourceType.Customer, 
    customerId
);
```

### Role Management
```csharp
// Assign role to user
_userRoleService.AssignRole(userId, UserRole.FranchiseeManager);

// Add specific permission
_userPermissionService.GrantPermission(
    userId, 
    "Reports.Financial.View"
);

// Check user's role
var isAdmin = _userRoleService.IsInRole(userId, UserRole.Admin);
```

### Password Management
```csharp
// Request password reset
var resetToken = _passwordService.RequestReset(email);

// Send reset email
_notificationService.SendPasswordResetEmail(email, resetToken);

// Reset password
var result = _passwordService.ResetPassword(new PasswordResetViewModel
{
    Token = resetToken,
    NewPassword = "NewSecureP@ssw0rd"
});

// Validate password strength
var isStrong = _passwordService.ValidatePasswordStrength("password123");
// Returns: false (too weak)
```

## For Human Developers

### Common Operations

#### 1. User Management
```csharp
// Search users
var users = _userSearchService.Search(new UserSearchViewModel
{
    SearchTerm = "john",
    Role = UserRole.Technician,
    FranchiseeId = franchiseeId,
    Status = UserStatus.Active
});

// Update user profile
_userProfileService.Update(userId, new UserProfileViewModel
{
    FirstName = "John",
    LastName = "Smith",
    Phone = "(555) 123-4567",
    NotificationPreferences = new NotificationPreferenceViewModel
    {
        EmailEnabled = true,
        SMSEnabled = false,
        PushEnabled = true
    }
});

// Deactivate user
_userService.Deactivate(userId, reason: "Terminated employment");
```

#### 2. Session Management
```csharp
// Get active sessions
var sessions = _userSessionService.GetActiveSessions(userId);

// Revoke all sessions (force logout everywhere)
_userSessionService.RevokeAllSessions(userId);

// Token refresh
var newTokens = _refreshTokenService.RefreshTokens(oldRefreshToken);
```

#### 3. Activity Auditing
```csharp
// Log user activity
_userActivityService.Log(new UserActivityViewModel
{
    UserId = userId,
    Action = "InvoiceCreated",
    ResourceType = "Invoice",
    ResourceId = invoiceId,
    IPAddress = ipAddress,
    UserAgent = userAgent
});

// Get user activity history
var activity = _userActivityService.GetActivity(userId, startDate, endDate);
```

### Security Best Practices
- Always hash passwords using BCrypt or Argon2
- Never log passwords or tokens
- Use HTTPS for all authentication endpoints
- Implement CSRF protection
- Rate limit authentication endpoints
- Use secure session cookies (HttpOnly, Secure, SameSite)
- Implement account lockout after failed attempts
- Validate JWT signatures on every request
- Use short-lived access tokens with refresh tokens
- Implement IP whitelisting for admin accounts
- Log all authentication events
- Implement suspicious activity detection
- Use 2FA for privileged accounts
- Never expose user IDs in URLs (use GUIDs)
- Sanitize all user input
