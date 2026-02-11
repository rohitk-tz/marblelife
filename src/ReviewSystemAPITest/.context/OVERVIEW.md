<!-- AUTO-GENERATED: Header -->
# ReviewSystemAPITest
> Developer utility for testing ReviewPush customer feedback API integration
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
ReviewSystemAPITest is a manual testing harness for the ReviewPush API integration, used by developers to validate customer feedback synchronization without relying on automated jobs. It provides a quick way to test API connectivity, authentication, and data retrieval for specific franchisees and customers.

**Use Cases**:
- Testing new ReviewPush credentials
- Debugging missing customer feedback
- Validating new franchisee setup in ReviewPush
- Isolating API issues from scheduler problems

**Not for Production**: This is strictly a development/debugging tool with hardcoded test data.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Running Tests
```bash
ReviewSystemAPITest.exe
# Uncomment specific test in Program.cs first
```

### Test Configuration
Edit Program.cs:
```csharp
string clientId = "your_reviewpush_client_id";
int businessId = 34700;  // Franchisee to test
long customerId = 28477443;  // Customer to test

var triggerMail = ApplicationManager.DependencyInjection.Resolve<ICustomerFeedbackService>();
triggerMail.GetFeedback(clientId, businessId);  // Uncomment this line
```

### Expected Output
```
Fetching feedback for business 34700
Retrieved 5 customer reviews
Updated database successfully
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `GetFeedback(clientId, businessId)` | Retrieves customer feedback from ReviewPush API |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "401 Unauthorized" Error
**Cause**: Invalid or expired ReviewPush client ID.  
**Solution**: Update clientId with current API credentials from ReviewPush account settings.

### "No feedback returned"
**Cause**: Business ID not registered in ReviewPush or no feedback exists.  
**Solution**: Verify businessId is correct, check ReviewPush dashboard for expected feedback.

### "Network timeout"
**Cause**: ReviewPush API unreachable or rate limiting.  
**Solution**: Check internet connectivity, verify API endpoint URL, wait and retry.
<!-- END CUSTOM SECTION -->
