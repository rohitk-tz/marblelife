# API/Areas/Payment - AI Context

## Purpose

The **Payment** area handles payment processing, payment method management, transaction recording, and payment reconciliation. It integrates with payment gateways and manages the financial transaction lifecycle.

## Key Functionality

### Payment Processing
- Credit card processing
- ACH/bank transfers
- Check payments
- Cash payments
- Payment gateway integration

### Payment Management
- Record payments against invoices
- Apply credits and adjustments
- Handle partial payments
- Process refunds
- Manage payment plans

### Payment Methods
- Store customer payment methods
- Tokenized card storage (PCI compliance)
- Default payment method selection
- Payment method validation

### Transaction History
- Payment transaction logging
- Receipt generation
- Payment reconciliation
- Failed transaction handling
- Chargeback management

## Key Controllers

### PaymentController.cs
Primary payment operations.

**Endpoints**:
- `POST /Payment/Payment/Process` - Process payment
- `GET /Payment/Payment/GetHistory` - Get payment history
- `POST /Payment/Payment/Refund` - Issue refund
- `GET /Payment/Payment/GetReceipt/{id}` - Get payment receipt

### PaymentMethodController.cs
Payment method management.

**Endpoints**:
- `POST /Payment/PaymentMethod/Add` - Add payment method
- `GET /Payment/PaymentMethod/GetList` - Get customer payment methods
- `POST /Payment/PaymentMethod/SetDefault` - Set default payment method
- `DELETE /Payment/PaymentMethod/{id}` - Remove payment method

## Key ViewModels

```csharp
public class ProcessPaymentRequest
{
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType Method { get; set; }
    public long? PaymentMethodId { get; set; }  // For stored methods
    
    // For new credit card payments
    public string CardNumber { get; set; }
    public string CardholderName { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string CVV { get; set; }
    
    // For ACH
    public string RoutingNumber { get; set; }
    public string AccountNumber { get; set; }
    
    // Optional
    public string Notes { get; set; }
}

public class PaymentResponse
{
    public long PaymentId { get; set; }
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string Message { get; set; }
    public string ReceiptUrl { get; set; }
}

public class PaymentHistoryViewModel
{
    public long Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType Method { get; set; }
    public string TransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public string InvoiceNumber { get; set; }
    public string CustomerName { get; set; }
}

public enum PaymentMethodType
{
    CreditCard = 1,
    DebitCard = 2,
    ACH = 3,
    Check = 4,
    Cash = 5,
    Other = 6
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    Disputed = 5
}
```

## Payment Gateway Integration

### Supported Gateways
- Stripe
- Authorize.Net
- PayPal
- Square

### Integration Pattern
```csharp
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPayment(PaymentRequest request);
    Task<RefundResult> ProcessRefund(string transactionId, decimal amount);
    Task<bool> ValidatePaymentMethod(PaymentMethodInfo method);
}
```

## PCI Compliance

**Security Requirements**:
- Never store full credit card numbers
- Use tokenization for stored payment methods
- Card data transmitted over HTTPS only
- CVV never stored
- Logs do not include sensitive card data
- Regular security audits

**Implementation**:
```csharp
// Tokenize card before storage
var token = await _paymentGateway.TokenizeCard(cardInfo);

var paymentMethod = new PaymentMethod
{
    CustomerId = customerId,
    Token = token,  // Store token, not card number
    Last4Digits = cardInfo.CardNumber.Substring(cardInfo.CardNumber.Length - 4),
    CardType = DetectCardType(cardInfo.CardNumber),
    ExpiryMonth = cardInfo.ExpiryMonth,
    ExpiryYear = cardInfo.ExpiryYear
};
```

## Business Rules

- Payments must reference valid invoice
- Partial payments allowed (updates invoice balance)
- Overpayments create account credit
- Refunds require original transaction ID
- Failed payments retry 3 times before marking failed
- Chargebacks freeze related account
- Payment receipts emailed automatically
- Payment confirmation sent via notification system

## Authorization

- **Customers**: Pay their own invoices
- **Franchisee Users**: Process payments for their customers
- **Super Admin**: Process payments for any customer
- **Technicians**: Accept cash/check payments only

## Error Handling

### Common Errors
- **Insufficient Funds**: Return friendly message, offer payment plan
- **Expired Card**: Prompt for card update
- **Invalid Card Number**: Validate before submission
- **Declined**: Retry with different method
- **Gateway Timeout**: Queue for retry

### Failed Payment Recovery
```csharp
public async Task<PaymentResult> ProcessPaymentWithRetry(PaymentRequest request)
{
    int maxRetries = 3;
    int retryDelay = 5000; // 5 seconds
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            var result = await _paymentGateway.ProcessPayment(request);
            
            if (result.Success)
                return result;
            
            // Don't retry for certain errors
            if (result.ErrorCode == "insufficient_funds" || 
                result.ErrorCode == "card_declined")
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Payment attempt {i + 1} failed", ex);
            
            if (i < maxRetries - 1)
                await Task.Delay(retryDelay);
        }
    }
    
    return new PaymentResult { Success = false, Message = "Payment failed after retries" };
}
```

## Testing

### Test Credit Cards (Stripe)
- **Success**: 4242 4242 4242 4242
- **Declined**: 4000 0000 0000 0002
- **Insufficient Funds**: 4000 0000 0000 9995

### Testing Scenarios
```csharp
[Test]
public void ProcessPayment_ValidCard_Success()
{
    var request = new ProcessPaymentRequest
    {
        InvoiceId = 123,
        Amount = 100.00m,
        Method = PaymentMethodType.CreditCard,
        CardNumber = "4242424242424242",
        ExpiryMonth = 12,
        ExpiryYear = 2025,
        CVV = "123"
    };
    
    var result = _paymentController.Process(request).Result;
    
    Assert.IsTrue(result.Success);
    Assert.IsNotNull(result.TransactionId);
}

[Test]
public void ProcessPayment_DeclinedCard_ReturnsError()
{
    var request = new ProcessPaymentRequest
    {
        InvoiceId = 123,
        Amount = 100.00m,
        CardNumber = "4000000000000002"  // Test declined card
    };
    
    var result = _paymentController.Process(request).Result;
    
    Assert.IsFalse(result.Success);
    Assert.IsTrue(result.Message.Contains("declined"));
}
```

## Reconciliation

### Daily Reconciliation Process
1. Compare recorded payments vs. gateway transactions
2. Identify mismatches
3. Reconcile bank deposits
4. Generate reconciliation report
5. Flag discrepancies for review

### Endpoint
```
GET /Payment/Payment/GetReconciliationReport?date={date}
```

## Integration Points

- **Sales**: Link payments to invoices
- **Organizations**: Franchisee payment settings
- **Notification**: Payment confirmations
- **Reports**: Payment reporting and analytics
- **Scheduler**: Payment for jobs at completion

## Audit Trail

All payment operations logged:
- Payment attempts (success/failure)
- Refund requests
- Payment method changes
- Transaction IDs
- User who processed payment
- Timestamp
- IP address

## Future Enhancements

- Recurring payment support
- Subscription billing
- Payment plans/installments
- International payments
- Cryptocurrency support
- Mobile wallet integration (Apple Pay, Google Pay)
