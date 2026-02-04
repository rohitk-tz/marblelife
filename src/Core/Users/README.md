<!-- AUTO-GENERATED: Header -->
# Core.Users
> Identity & Authentication
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module handles User Identity (`Person`) and Authentication credentials (`UserLogin`).
It is decoupled from the Organization hierarchy (a Person can exist without an Organization).

### Key Features
-   **Person Profile**: Name, Email, Address.
-   **Credentials**: Username/Password management.
-   **Security**: Account Locking (after 5 failed attempts) and Password Reset tokens.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Authenticating a User
```csharp
var loginService = dependency.Resolve<IUserLoginService>();
var user = loginService.Login(username, password);
if (user != null) {
   // Success
}
```

### Creating a User
You likely use `IUserService` or `PersonFactory` + `UserLoginFactory`.
1.  Create `Person`.
2.  Create `UserLogin` linked to that Person.
3.  (Optional) Assign Role via `Core.Organizations`.

<!-- END AUTO-GENERATED -->
