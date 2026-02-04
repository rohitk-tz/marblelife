# Infrastructure/Billing - AI Context

## Purpose

This folder contains infrastructure implementations for billing and payment services, including payment gateway integrations.

## Contents

Billing infrastructure:
- Payment gateway client implementations (Authorize.Net)
- Payment processing logic
- Invoice generation utilities
- Transaction logging

## Structure

- **Impl/**: Concrete implementations of Core/Billing interfaces

## For AI Agents

**Payment Gateway Integration Pattern**:
```csharp
public class AuthorizeNetService : IPaymentGatewayService
{
    private readonly ISettings _settings;
    
    public async Task<PaymentResult> ProcessPayment(PaymentRequest request)
    {
        // 1. Create payment gateway request
        var gateway = new AuthorizeNetGateway(
            _settings.ApiLoginId, 
            _settings.TransactionKey
        );
        
        // 2. Process transaction
        var response = await gateway.ProcessPayment(
            request.Amount,
            request.CardNumber,
            request.ExpiryDate,
            request.CVV
        );
        
        // 3. Return standardized result
        return new PaymentResult
        {
            Success = response.Approved,
            TransactionId = response.TransactionId,
            Message = response.Message
        };
    }
}
```

## For Human Developers

Infrastructure layer handles actual payment processing:

### Best Practices:
- Never log sensitive payment data (card numbers, CVV)
- Use PCI-compliant practices
- Implement proper error handling
- Store tokenized payment methods only
- Log transaction IDs for reconciliation
- Handle gateway timeouts gracefully
- Test with sandbox/test mode first
