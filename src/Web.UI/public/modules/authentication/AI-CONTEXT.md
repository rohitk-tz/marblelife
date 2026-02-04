# Authentication Module

## Overview
The Authentication module handles all user authentication and authorization features including login, logout, password recovery, and session management. It provides both employee login and customer portal access.

## Module Structure

```
/authentication
├── module.js                          # Module definition and routing
├── /controllers
│   ├── authentication.layout.client.controller.js
│   ├── login.client.controller.js
│   ├── login-customer.client.controller.js
│   ├── forget-password.client.controller.js
│   ├── reset-password.client.controller.js
│   └── confirmation.client.controller.js
├── /services
│   ├── user.authentication.client.service.js
│   └── url.authentication.client.service.js
└── /views
    ├── default.client.view.html
    ├── login.client.view.html
    ├── login-customer.client.view.html
    ├── forget-password.client.view.html
    ├── reset-password.client.view.html
    ├── confirmation.client.view.html
    └── invalid-link.client.view.html
```

## Routing Configuration

```javascript
angular.module('authentication').config(["$stateProvider", "$urlRouterProvider", routeConfig]);

function routeConfig($stateProvider, $urlRouterProvider) {
    var viewsPathPrefix = '/modules/authentication/views/';

    $stateProvider
        .state('authentication', {
            abstract: true,
            templateUrl: viewsPathPrefix + 'default.client.view.html',
            controller: 'AuthenticationLayoutController',
            controllerAs: 'vm'
        })
        .state('authentication.login', {
            url: '/login',
            templateUrl: viewsPathPrefix + 'login.client.view.html',
            controller: 'LoginController',
            controllerAs: 'vm'
        })
        .state('authentication.loginCustomer', {
            url: '/login/customer',
            templateUrl: viewsPathPrefix + 'login-customer.client.view.html',
            controller: 'LoginCustomerController',
            controllerAs: 'vm'
        })
        .state('authentication.forget', {
            url: '/password/forget',
            templateUrl: viewsPathPrefix + 'forget-password.client.view.html',
            controller: 'ForgetPasswordController',
            controllerAs: 'vm'
        })
        .state('authentication.reset', {
            url: '/password/reset/:token',
            templateUrl: viewsPathPrefix + 'reset-password.client.view.html',
            controller: 'ResetPasswordController',
            controllerAs: 'vm'
        })
        .state('authentication.confirmation', {
            url: '/confirmation/:id/:status',
            templateUrl: viewsPathPrefix + 'confirmation.client.view.html',
            controller: 'ConfirmationController',
            controllerAs: 'vm'
        })
        .state('authentication.invalidLink', {
            url: '/invalidLink',
            templateUrl: viewsPathPrefix + 'invalid-link.client.view.html',
            controllerAs: 'vm'
        });
}
```

## Services

### UserAuthenticationService
The primary authentication service that manages user sessions and credentials.

```javascript
angular.module('authentication').service("UserAuthenticationService",
    ["HttpWrapper", "$cookies", "APP_CONFIG", "$rootScope",
    function (httpWrapper, $cookies, config, $rootScope) {

        // Login with username and password
        var login = function (userName, password) {
            var model = { userName: userName, password: password };
            return httpWrapper.post({
                url: "/user/login",
                data: model,
                showOnSuccess: false
            });
        };

        // Logout current user
        var logout = function () {
            return httpWrapper.get({
                url: "/user/logout",
                showOnSuccess: false,
                showOnFailure: false
            }).then(function (result) {
                setTokenAndIdentity($rootScope, null);
                return result;
            });
        };

        // Get user identity by session ID
        var getUserIdentity = function (sessionId) {
            return httpWrapper.get({
                url: "/users/identity/" + sessionId,
                showOnSuccess: false,
                showOnFailure: false
            });
        };

        // Retrieve and validate session
        function getUserSessionId(callback) {
            var sessionId = $cookies.get(config.clientTokenName);
            getUserIdentity(sessionId).then(function (result) {
                var data = result.data;
                if (data != null) {
                    setTokenAndIdentity($rootScope, data, sessionId);
                }
                if (callback != null) {
                    callback();
                }
            });
        }

        // Set or clear authentication token and user identity
        var setTokenAndIdentity = function ($rootScope, identity, token) {
            if (identity == null) {
                $cookies.remove(config.clientTokenName);
                $rootScope.identity = null;
            }
            else {
                $cookies.put(config.clientTokenName, token);
                $rootScope.identity = identity;
                $rootScope.$broadcast('identity');
            }
        };

        // Password recovery functions
        function forgotPassword(email) {
            return httpWrapper.post({ 
                url: "/users/login/SendPasswordLink?email=" + email 
            });
        }

        function resetPassword(user) {
            return httpWrapper.post({ 
                url: "/users/login/ResetPassword", 
                data: user 
            });
        }

        function resetPasswordExpire(data) {
            return httpWrapper.post({ 
                url: "/users/login/ResetPasswordExpire", 
                data: data 
            });
        }

        // Customer login with access code
        var loginCustomer = function (code) {
            var model = { Code: code };
            return httpWrapper.post({
                url: "/user/login/customer",
                data: model,
                showOnSuccess: false
            });
        };

        return {
            login: login,
            logout: logout,
            getUserIdentity: getUserIdentity,
            getUserSessionId: getUserSessionId,
            setTokenAndIdentity: setTokenAndIdentity,
            forgotPassword: forgotPassword,
            resetPassword: resetPassword,
            resetPasswordExpire: resetPasswordExpire,
            loginCustomer: loginCustomer
        };
}]);
```

### URLAuthenticationService
Manages URL-based authentication parameters and tokens.

## Controllers

### LoginController
Handles employee login form:
```javascript
angular.module('authentication').controller('LoginController',
    ['UserAuthenticationService', '$state', function(authService, $state) {
        var vm = this;
        
        vm.credentials = {
            userName: '',
            password: ''
        };
        
        vm.submit = function() {
            authService.login(vm.credentials.userName, vm.credentials.password)
                .then(function(response) {
                    if (response.data) {
                        authService.setTokenAndIdentity($rootScope, response.data, response.data.token);
                        $state.go('core.layout.home');
                    }
                })
                .catch(function(error) {
                    vm.error = 'Invalid username or password';
                });
        };
}]);
```

### LoginCustomerController
Handles customer portal access with unique access code:
- Single-use or time-limited codes
- Simplified authentication for customers
- Direct access to customer-specific features

### ForgetPasswordController
Initiates password recovery process:
- Email validation
- Password reset link generation
- Confirmation messaging

### ResetPasswordController
Completes password reset:
- Token validation
- New password entry with confirmation
- Password strength requirements
- Token expiration handling

### ConfirmationController
Handles various confirmation scenarios:
- Email verification
- Account activation
- Password reset confirmation
- Status-based messaging

## Authentication Flow

### Standard Login Flow
1. User enters credentials in login form
2. `LoginController` calls `UserAuthenticationService.login()`
3. Service sends credentials to `/user/login` API
4. API validates credentials and returns user data + session token
5. Service stores token in cookie via `setTokenAndIdentity()`
6. User identity stored in `$rootScope.identity`
7. `identity` event broadcast to notify application
8. User redirected to dashboard

### Customer Login Flow
1. Customer receives unique access code (via email or SMS)
2. Customer enters code in customer login form
3. `LoginCustomerController` calls `UserAuthenticationService.loginCustomer()`
4. API validates code and returns customer session
5. Customer redirected to their portal view

### Password Recovery Flow
1. User clicks "Forgot Password" link
2. User enters email address
3. `ForgetPasswordController` calls `forgotPassword(email)`
4. System sends email with reset token
5. User clicks link in email with token parameter
6. `ResetPasswordController` validates token
7. User enters new password
8. Password updated in system
9. User redirected to login

### Session Management
- Session token stored in cookie (name from `APP_CONFIG.clientTokenName`)
- Token sent with each API request via HTTP interceptor
- Token validated on server for each protected endpoint
- Automatic logout on token expiration
- Token cleared on explicit logout

## Security Features

### Password Security
- Passwords transmitted over HTTPS only
- Server-side password hashing (bcrypt/PBKDF2)
- Password complexity requirements enforced
- Password reset tokens expire after set time
- One-time use reset tokens

### Session Security
- HttpOnly cookies to prevent XSS attacks
- Secure flag for HTTPS transmission
- Session timeout after inactivity
- Concurrent session limits (optional)
- Token rotation on sensitive operations

### Protection Mechanisms
- CSRF token validation
- Rate limiting on login attempts
- Account lockout after failed attempts
- IP-based blocking (optional)
- Brute force protection

## Cookie Management

```javascript
// Cookie configuration
var cookieOptions = {
    expires: 7, // 7 days
    path: '/',
    secure: true, // HTTPS only
    sameSite: 'strict'
};

// Set authentication cookie
$cookies.put(config.clientTokenName, token, cookieOptions);

// Get authentication cookie
var token = $cookies.get(config.clientTokenName);

// Remove authentication cookie
$cookies.remove(config.clientTokenName);
```

## Integration with Other Modules

### Core Module
- Uses `HttpWrapper` service for API calls
- Uses `Toaster` service for notifications
- Integrates with routing guards

### Header/Navigation
- Identity displayed in header
- Logout action in user menu
- Role-based menu items

### Route Protection
```javascript
// Protected route example
.state('core.layout.protectedPage', {
    url: '/protected',
    templateUrl: 'protected.html',
    resolve: {
        factory: 'checkRouting' // Validates authentication
    }
});
```

## Error Handling

### Common Error Scenarios
- **Invalid credentials**: Display error message below form
- **Expired token**: Redirect to login with message
- **Network error**: Display generic error, retry option
- **Account locked**: Display lockout message and support contact
- **Invalid reset token**: Redirect to invalid link page

### Error Messages
```javascript
var errorMessages = {
    invalidCredentials: 'Invalid username or password',
    accountLocked: 'Account locked due to multiple failed attempts',
    tokenExpired: 'Password reset link has expired',
    networkError: 'Unable to connect. Please try again.'
};
```

## Testing Considerations

### Unit Tests
- Test login with valid credentials
- Test login with invalid credentials
- Test password reset flow
- Test token validation
- Test session management

### E2E Tests
- Complete login flow
- Logout flow
- Password recovery flow
- Customer login flow
- Session timeout behavior

## Configuration

### App Config
```javascript
APP_CONFIG = {
    clientTokenName: 'ml-session-token',
    tokenExpiration: 60 * 60 * 1000, // 1 hour in milliseconds
    loginUrl: '/login',
    logoutUrl: '/user/logout'
};
```

## Best Practices

1. **Never log credentials** - Even in debug mode
2. **Always use HTTPS** - For authentication endpoints
3. **Validate on server** - Client-side validation is supplementary
4. **Clear sensitive data** - After form submission
5. **Handle errors gracefully** - User-friendly messages
6. **Implement timeout** - For password reset tokens
7. **Use secure cookies** - HttpOnly, Secure, SameSite flags

## Related Documentation
- See `/modules/core/AI-CONTEXT.md` for HttpWrapper details
- See `/modules/users/AI-CONTEXT.md` for user management
- See parent `/Web.UI/AI-CONTEXT.md` for application architecture
