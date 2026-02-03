<!-- AUTO-GENERATED: Header -->
# Core.Users Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T23:55:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Identity and Authentication** domain. It manages "Who is who" (Person) and "How they log in" (UserLogin).

### Key Entities
-   `Person`: The real-world human. Contains Name, Email, Address, Phone.
-   `UserLogin`: The security principal. Contains Username, Password (Hashed), Salt, Lockout status. 1:1 relationship with `Person`.
-   **No "User" Class**: There is no single `User` class. The concept of a "User" is the aggregate of `Person + UserLogin`.

### Logic Flow
1.  **Authentication**: Handled by `UserLoginService` (verifies password hash and salt).
2.  **Context**: The `Person.Id` is often used as the "UserId" throughout the system.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Domain
-   `Person`: Fields: `FirstName`, `LastName`, `Email`. Not mapped properties for helpers (`FullName`).
-   `UserLogin`: Fields: `UserName`, `Password`, `IsLocked`, `LastLoggedInDate`.
-   `UserLog`: Audit trail for login attempts.

### Enum
-   `RoleType`: Defines hardcoded roles (Admin, FranchiseeOwner, Technician, etc.).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Organizations](../Organizations/AI-CONTEXT.md)**: `OrganizationRoleUser` in that module links `Person` to specific contexts.
-   **[Core.Application](../Application/AI-CONTEXT.md)**: Inherits `DomainBase`.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Authentication Mechanism
It uses a custom Salt + Hash mechanism, not ASP.NET Identity.
Password reset tokens and lockouts (5 attempts) are handled manually in `UserLogin`.

### Usage Note
When creating a new user, you must create a `Person` first, then a `UserLogin`. They share the same ID (Primary Key is usually `Person.Id`, and `UserLogin.Id` is a ForeignKey to `Person.Id`).
<!-- END CUSTOM SECTION -->
