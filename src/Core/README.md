<!-- AUTO-GENERATED: Header -->
# Core Layer
> Business Logic, Domain Entities, and Contracts
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Core layer is the heart of the application. It defines **WHAT** the application does (Business Logic) and **WHAT** data it manages (Domain Entities). It is independent of the UI and Database implementation details.

It is organized by functional modules (Bounded Contexts):
*   **Users**: Authentication, Roles, Profiles.
*   **Billing**: Invoicing, Payments, Fees.
*   **Sales**: Leads, Sales Data, Franchisee Performance.
*   **Organizations**: Franchisee management.
*   **Scheduler**: Job scheduling and resource management.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Creating a New Service

1.  **Define Interface**: Create `IMyFeatureService.cs` in the module root.
2.  **Implement**: Create `MyFeatureService.cs` in `Impl/`.
3.  **Attribute**: Add `[DefaultImplementation]` class attribute.
4.  **Inject**: Use constructor injection to get Repositories.

```csharp
[DefaultImplementation]
public class MyFeatureService : IMyFeatureService 
{
    private readonly IRepository<MyEntity> _repo;
    
    public MyFeatureService(IUnitOfWork uow) {
        _repo = uow.Repository<MyEntity>();
    }
    
    public void DoBusinessLogic() {
        // ...
    }
}
```

### Adding a Domain Entity

1.  Create class in `Domain/`.
2.  Inherit from `DomainBase`.
3.  Add `virtual` properties for lazy-loading relationships.
4.  Add `DbSet` in `MakaluDbContext` (in ORM layer).

```csharp
public class MyEntity : DomainBase 
{
    public string Name { get; set; }
    public virtual ICollection<OtherEntity> Others { get; set; }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Sub-Modules -->
## 📂 Key Modules

| Module | Description |
| :--- | :--- |
| **Users** | Login logic (`UserLoginService`), Password resets (`PasswordResetService`), User management. |
| **Organizations** | Hierarchy logic (Franchisors -> Franchisees). |
| **Billing** | Complex billing rules, Late fees, Royalty slabs. |
| **Notifications** | Logic for queuing emails (`NotificationService`). |

<!-- END AUTO-GENERATED -->
