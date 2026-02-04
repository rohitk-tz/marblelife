# Core/Billing - AI Context

## Purpose

The **Billing** module handles all financial transactions, payment processing, invoicing, credit card management, and integration with payment gateways (primarily Stripe) for the MarbleLife platform.

## Key Entities (Domain/)

### Invoice Management
- **Invoice**: Customer invoices with line items
- **InvoiceItem**: Individual charges and fees
- **InvoicePayment**: Payment records against invoices
- **InvoiceAdjustment**: Credits, discounts, and corrections
- **RecurringInvoice**: Auto-generated recurring invoices

### Payment Processing
- **Payment**: Payment transaction records
- **PaymentMethod**: Stored payment methods (cards, ACH)
- **PaymentTransaction**: Stripe/gateway transaction details
- **Refund**: Refund processing and tracking
- **PaymentSchedule**: Payment plans and installments

### Customer Billing
- **BillingProfile**: Customer billing preferences
- **CreditCard**: Tokenized card information
- **BankAccount**: ACH payment details
- **BillingAddress**: Billing address records
- **PaymentHistory**: Complete payment audit trail

### Financial Management
- **RoyaltyPayment**: Franchisee royalty calculations
- **FranchiseeInvoice**: Corporate invoices to franchisees
- **TaxRate**: Sales tax configuration
- **DiscountCode**: Promotional codes and discounts

## Service Interfaces

### Invoice Services
- **IInvoiceFactory**: Invoice creation from jobs/orders
- **IInvoiceService**: Invoice CRUD and workflow
- **IInvoiceItemFactory**: Line item management
- **IRecurringInvoiceService**: Automated invoice generation

### Payment Services
- **IPaymentFactory**: Payment transaction creation
- **IPaymentService**: Payment processing and reconciliation
- **IPaymentMethodService**: Stored payment method management
- **IRefundService**: Refund processing
- **IPaymentScheduleService**: Installment plans

### Gateway Integration
- **IStripePaymentService**: Stripe API integration
- **IPaymentGatewayService**: Abstract gateway interface
- **ICardTokenizationService**: Secure card storage
- **IPaymentWebhookService**: Webhook processing

### Financial Services
- **IRoyaltyCalculationService**: Royalty fee calculations
- **ITaxCalculationService**: Sales tax computation
- **IDiscountService**: Discount code validation and application
- **IBillingReportService**: Financial reporting

## Implementations (Impl/)

Business logic including:
- Stripe payment processing
- Invoice generation workflows
- Automatic payment retries
- Dunning management (failed payments)
- Tax calculation by jurisdiction
- Royalty fee calculations
- Payment reconciliation

## Enumerations (Enum/)

- **InvoiceStatus**: Draft, Sent, Viewed, PartiallyPaid, Paid, Overdue, Cancelled, Void
- **PaymentStatus**: Pending, Authorized, Captured, Failed, Refunded, Disputed
- **PaymentMethodType**: CreditCard, DebitCard, ACH, Cash, Check, Other
- **CardBrand**: Visa, Mastercard, Amex, Discover
- **TransactionType**: Sale, Authorization, Capture, Refund, Void
- **BillingFrequency**: OneTime, Weekly, Monthly, Quarterly, Annually

## ViewModels (ViewModel/)

- **InvoiceViewModel**: Complete invoice with items
- **PaymentViewModel**: Payment transaction data
- **PaymentMethodViewModel**: Stored payment method info
- **StripePaymentViewModel**: Stripe-specific data
- **RefundViewModel**: Refund request data
- **BillingReportViewModel**: Financial reports

## Business Rules

### Invoicing Rules
1. Invoices auto-generated upon job completion
2. Invoice due date configurable (typically net 30)
3. Automatic late fees after grace period
4. Email invoice to customer automatically
5. Support for partial payments
6. Invoice voiding requires authorization

### Payment Processing
1. All card data tokenized (PCI compliance)
2. Never store raw card numbers
3. Payment authorization before job start for new customers
4. Automatic capture upon job completion
5. Failed payment retry logic (3 attempts over 5 days)
6. Automatic receipt generation and email

### Royalty Calculations
1. Royalty fees calculated on gross revenue
2. Tiered percentage based on revenue slabs
3. Monthly billing cycle for franchisees
4. Adjustments for refunds and chargebacks
5. Detailed breakdown in franchisee portal

### Tax Compliance
1. Sales tax calculated based on service location
2. Tax rates updated automatically
3. Tax-exempt customers supported (certificate required)
4. Detailed tax reporting for compliance

## Dependencies

- **Core/Scheduler**: Jobs trigger invoice generation
- **Core/Sales**: Orders create invoices
- **Core/Organizations**: Franchisee billing and royalties
- **Infrastructure/Billing**: Stripe API client
- **Core/Notification**: Payment confirmations and reminders

## For AI Agents

### Creating Invoice from Job
```csharp
// Auto-generate invoice when job completes
var invoice = _invoiceFactory.CreateFromJob(new InvoiceFromJobViewModel
{
    JobId = jobId,
    CustomerId = customerId,
    DueDate = DateTime.Now.AddDays(30),
    Items = new[]
    {
        new InvoiceItemViewModel
        {
            Description = "Marble Floor Restoration - 500 sq ft",
            Quantity = 500,
            UnitPrice = 8.50m,
            TaxRate = 0.08m
        },
        new InvoiceItemViewModel
        {
            Description = "Sealing and Protection",
            Amount = 250m,
            TaxRate = 0.08m
        }
    }
});

// Calculate totals (subtotal, tax, total)
_invoiceService.CalculateTotals(invoice.Id);

// Send to customer
_invoiceService.SendInvoice(invoice.Id, sendEmail: true);
```

### Processing Payment
```csharp
// Process credit card payment via Stripe
var payment = await _stripePaymentService.ProcessPayment(new PaymentViewModel
{
    InvoiceId = invoiceId,
    Amount = invoiceTotal,
    PaymentMethodId = stripePaymentMethodId,
    CustomerId = customerId,
    Description = $"Payment for Invoice #{invoiceNumber}"
});

if (payment.Status == PaymentStatus.Captured)
{
    // Mark invoice as paid
    _invoiceService.MarkAsPaid(invoiceId, payment.Id);
    
    // Send receipt
    _invoiceService.SendReceipt(invoiceId);
    
    // Update customer balance
    _billingService.UpdateCustomerBalance(customerId);
}
else
{
    // Handle failed payment
    _invoiceService.RecordPaymentFailure(invoiceId, payment.FailureReason);
    _dunningService.ScheduleRetry(invoiceId);
}
```

### Storing Payment Method
```csharp
// Tokenize and store credit card securely
var paymentMethod = await _cardTokenizationService.Tokenize(new CardViewModel
{
    CardNumber = "4242424242424242", // Test card
    ExpiryMonth = 12,
    ExpiryYear = 2025,
    CVV = "123",
    CardholderName = "John Smith"
});

// Save tokenized method
var stored = _paymentMethodService.Store(new PaymentMethodViewModel
{
    CustomerId = customerId,
    Type = PaymentMethodType.CreditCard,
    TokenId = paymentMethod.StripeTokenId,
    Last4 = paymentMethod.Last4,
    Brand = paymentMethod.Brand,
    IsDefault = true
});
```

### Refund Processing
```csharp
// Process full or partial refund
var refund = await _refundService.ProcessRefund(new RefundViewModel
{
    PaymentId = paymentId,
    Amount = refundAmount,  // null for full refund
    Reason = "Customer request - service quality issue",
    RefundToOriginalMethod = true
});

// Adjust invoice
_invoiceService.ApplyCredit(invoiceId, refund.Amount);
```

## For Human Developers

### Common Operations

#### 1. Monthly Billing Cycle
```csharp
// Generate recurring invoices
_recurringInvoiceService.GenerateMonthlyInvoices(billingDate);

// Process scheduled payments
_paymentScheduleService.ProcessDuePayments(DateTime.Today);

// Send overdue reminders
_dunningService.SendOverdueReminders();
```

#### 2. Royalty Processing
```csharp
// Calculate monthly royalties
var royalties = _royaltyCalculationService.CalculateMonthly(
    franchiseeId, 
    startDate, 
    endDate
);

// Generate franchisee invoice
var franchiseeInvoice = _invoiceFactory.CreateFranchiseeInvoice(royalties);

// Process payment
_paymentService.ProcessFranchiseePayment(franchiseeInvoice.Id);
```

#### 3. Payment Reconciliation
```csharp
// Reconcile Stripe payments
var stripePayments = await _stripePaymentService.GetTransactions(startDate, endDate);
var systemPayments = _paymentService.GetPayments(startDate, endDate);

var reconciliation = _billingReportService.Reconcile(stripePayments, systemPayments);
// Returns: matched, unmatched, discrepancies
```

### Best Practices
- Always use transactions for payment operations
- Implement idempotency for payment processing
- Never store raw credit card numbers
- Log all payment attempts and failures
- Implement webhook verification for Stripe events
- Use Stripe's test mode for development
- Validate amounts server-side (never trust client)
- Implement retry logic with exponential backoff
- Store all gateway response data for reconciliation
- Handle timezone differences for payment processing
- Implement PCI compliance measures
- Regular audit of payment records
- Monitor failed payment patterns for fraud detection
