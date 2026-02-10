<!-- AUTO-GENERATED: Header -->
# ReviewSystemAPITest — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
ReviewSystemAPITest is a diagnostic and integration testing utility for the ReviewPush customer feedback API. It provides a manual testing harness to validate API connectivity, authentication, and core operations (retrieving feedback, triggering requests) without relying on the automated job scheduler.

### Design Patterns
- **Manual Test Harness**: Standalone console application for ad-hoc API testing
- **Hardcoded Test Data**: Uses predefined client IDs and business IDs for consistent testing
- **Dependency Injection**: Leverages Core services for actual API implementation
- **Developer Utility**: Not intended for production use, strictly for development/debugging

### Data Flow
1. Application entry via `Program.Main()` → Dependency registration
2. Hardcoded test parameters set (clientId, businessId, customerId)
3. Resolve `ICustomerFeedbackService` from Core.Review
4. Call API methods (commented out by default)
5. Observe console output and/or database changes
6. Exit manually

### Use Cases
- **API Connectivity**: Verify ReviewPush API is reachable and credentials work
- **Schema Validation**: Confirm response format matches expectations
- **Data Verification**: Check customer feedback is retrieved correctly
- **Integration Debugging**: Isolate issues in review collection workflow
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### Test Configuration (Hardcoded in Program.cs)
```csharp
string clientId = "5a475638898b9b51b8fa2f241d24bc241ab8603b";  // ReviewPush client ID
int businessId = 34700;           // Marblelife business identifier
int businessId_2 = 35346;         // Secondary business for testing
long customerId = 28477443;       // Sample customer ID
```

### ICustomerFeedbackService Interface (Core.Review)
```csharp
public interface ICustomerFeedbackService
{
    void GetFeedback(string clientId, int businessId);
    // Additional methods for sending feedback requests, updating status
}
```

### WinJobSessionContext
```csharp
public class WinJobSessionContext : ISessionContext
{
    // Provides execution context for console job
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `ICustomerFeedbackService.GetFeedback(string clientId, int businessId)`
- **Input**: ReviewPush client ID, business ID
- **Output**: void (logs to console/database)
- **Behavior**:
  - Authenticates with ReviewPush API using clientId
  - Retrieves customer feedback for specified business
  - Parses JSON response
  - Updates local database with feedback data
- **Side-effects**: Database writes to CustomerReview table, console logging

### Manual Test Execution
- **Purpose**: Uncomment specific API call in Program.cs to test
- **Example**:
```csharp
var triggerMail = ApplicationManager.DependencyInjection.Resolve<ICustomerFeedbackService>();
triggerMail.GetFeedback(clientId, businessId);  // Uncomment to run
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Review](../../Core/Review/.context/CONTEXT.md)** — ICustomerFeedbackService, review API integration
- **[Core.Application](../../Core/Application/.context/CONTEXT.md)** — Dependency injection, logging
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration

### External
- **ReviewPush API** — Third-party customer feedback platform
- **System.Net.Http** — HTTP client for API calls

### Configuration
Requires ReviewPush API credentials in App.config or Core.Application settings.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Purpose and Limitations
**This is NOT a production utility**. It's a developer tool for:
- Testing ReviewPush API changes
- Debugging customer feedback sync issues
- Validating new business integrations
- Isolating problems outside the job scheduler

**Do NOT**:
- Deploy to production servers
- Run on schedule
- Use for bulk operations

### Test Data Notes
The hardcoded IDs are **real production data**:
- **clientId**: Marblelife's ReviewPush account identifier
- **businessId 34700**: Specific franchisee location
- **businessId_2 35346**: Another franchisee for comparison testing
- **customerId 28477443**: Actual customer record

**Security**: Ensure client ID is kept confidential (API authentication token).

### How to Use for Testing

**Step 1: Update Test Parameters** (if needed):
```csharp
string clientId = "your_client_id_here";
int businessId = 12345;  // Franchisee to test
long customerId = 67890;  // Customer to test
```

**Step 2: Uncomment Desired Test**:
```csharp
var triggerMail = ApplicationManager.DependencyInjection.Resolve<ICustomerFeedbackService>();
triggerMail.GetFeedback(clientId, businessId);  // Remove comment slashes
```

**Step 3: Run in Visual Studio**:
- Set breakpoints in Core.Review implementation
- F5 to debug
- Observe API calls in Fiddler/Postman

**Step 4: Verify Results**:
- Check console output for errors
- Query database for updated CustomerReview records
- Verify API responses match expectations

### Alternative: Use Automated Jobs
For production testing, prefer using the NotificationService (Jobs) with the `IGetCustomerFeedbackService` job instead of this manual utility.

### ReviewPush Integration Details
- **Authentication**: OAuth-like client ID passed in request headers
- **Rate Limiting**: API may throttle requests (check documentation)
- **Endpoint**: Configured in Core.Review service settings
- **Response Format**: JSON with feedback details, ratings, comments

### Common Use Cases

**Case 1: New Franchisee Setup**
```csharp
// Test if new businessId is registered in ReviewPush
int businessId = 99999;  // New franchisee
triggerMail.GetFeedback(clientId, businessId);
```

**Case 2: Missing Feedback**
```csharp
// Debug why customer feedback not appearing in database
long customerId = 12345;
// Set breakpoint in GetFeedback implementation
// Trace API call and response parsing
```

**Case 3: API Credential Validation**
```csharp
// Verify new API credentials after ReviewPush account changes
string newClientId = "new_token_here";
triggerMail.GetFeedback(newClientId, businessId);
// Should return 200 OK, not 401 Unauthorized
```
<!-- END CUSTOM SECTION -->
