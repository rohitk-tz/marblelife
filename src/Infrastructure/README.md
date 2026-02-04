<!-- AUTO-GENERATED: Header -->
# Infrastructure Layer
> Plumbing, Data Access, and External Integrations
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Infrastructure layer provides the technical foundation for the MarbleLife/Makalu application. It implements the interfaces defined in the Core layer, handling:

*   **Database Access**: A robust Repository/UnitOfWork abstraction over Entity Framework.
*   **Payments**: Full integration with Authorize.Net for credit cards and eChecks.
*   **Utilities**: PDF Generation and Logging.

This layer ensures that business logic (Core) remains agnostic of specific implementation details like "How do we talk to the database?" or "Which payment gateway are we using?".

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Data Access (Unit of Work)

The `UnitOfWork` is the primary entry point for database operations.

```csharp
using Infrastructure.Application.Impl;

// 1. Inject or Instantiate
using (var uow = new UnitOfWork()) 
{
    // 2. Get Repository
    var userRepo = uow.Repository<Person>();
    
    // 3. Query
    var admin = userRepo.Fetch(u => u.Email == "admin@example.com").FirstOrDefault();
    
    // 4. Modify
    if (admin != null) {
        admin.LastLogin = DateTime.UtcNow;
        userRepo.Update(admin);
    }
    
    // 5. Commit
    uow.SaveChanges(); 
} // Dispose() automatically rolls back if SaveChanges wasn't called
```

### Payment Processing (Authorize.Net)

```csharp
// Inject IAuthorizeNetCustomerProfileService
var response = authNetService.ChargeNewCard(
    accountTypeId: 1, 
    cardNumber: "4007000000027", 
    cvv: "123", 
    expiryDate: "1225", 
    invoiceId: 101, 
    amount: 99.99m, 
    payeeId: 500, 
    name: "John Doe", 
    franchiseeName: "MarbleLife FL"
);

if (response.messages.resultCode == messageTypeEnum.Ok) {
    // Success
    string authCode = response.transactionResponse.authCode;
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Sub-Modules -->
## 📂 Key Sub-Modules

| Folder | Description |
| :--- | :--- |
| **Application** | Core infrastructure components: `UnitOfWork`, `Repository`, `PdfGenerator`. |
| **Billing** | Payment gateway implementations, specifically `AuthorizeNetCustomerProfileService`. |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Configuration -->
## ⚙️ Configuration

### Authorize.Net
This module does **not** hardcode API keys. Instead, it looks them up from the `AuthorizeNetApiMaster` database table based on the provided `accountTypeId`. 

*   **Table**: `AuthorizeNetApiMaster`
*   **Columns**: `ApiLoginID`, `ApiTransactionKey`, `AccountTypeId`

Ensure this table is populated for the environment.
<!-- END CUSTOM SECTION -->
