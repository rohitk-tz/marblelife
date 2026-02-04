# Core/Billing/Contants - AI Context

## Purpose

This folder contains constant values used throughout the Billing module, including configuration values, magic numbers, and other fixed values that shouldn't be hardcoded in business logic.

## Contents

Constant definitions for:
- **Payment Gateway Constants**: Stripe API keys, endpoints
- **Billing Cycle Constants**: Default payment terms, due dates
- **Fee Constants**: Late fee percentages, processing fees
- **Currency Constants**: Supported currencies, decimal places
- **Invoice Constants**: Template IDs, numbering formats
- **Tax Constants**: Default tax rates, categories

## For AI Agents

**Example Constants Class**:
```csharp
public static class BillingConstants
{
    public const int DEFAULT_PAYMENT_TERMS_DAYS = 30;
    public const int LATE_FEE_GRACE_PERIOD_DAYS = 5;
    public const decimal LATE_FEE_PERCENTAGE = 0.015m; // 1.5%
    public const decimal PROCESSING_FEE_PERCENTAGE = 0.029m; // 2.9%
    public const decimal PROCESSING_FEE_FIXED = 0.30m; // $0.30
    
    public static class Currency
    {
        public const string USD = "USD";
        public const string CAD = "CAD";
        public const int DECIMAL_PLACES = 2;
    }
    
    public static class InvoiceStatus
    {
        public const string DRAFT = "Draft";
        public const string SENT = "Sent";
        public const string PAID = "Paid";
    }
}
```

**Using Constants**:
```csharp
// Calculate due date
var dueDate = DateTime.Now.AddDays(BillingConstants.DEFAULT_PAYMENT_TERMS_DAYS);

// Calculate late fee
if (daysPastDue > BillingConstants.LATE_FEE_GRACE_PERIOD_DAYS)
{
    var lateFee = invoiceAmount * BillingConstants.LATE_FEE_PERCENTAGE;
}

// Format currency
var formatted = amount.ToString($"C{BillingConstants.Currency.DECIMAL_PLACES}");
```

## For Human Developers

Best practices for constants:
- Use descriptive names that explain the purpose
- Group related constants in nested classes
- Use const for compile-time constants
- Use static readonly for runtime-initialized values
- Document the meaning and usage of non-obvious constants
- Consider using configuration files for environment-specific values
- Use enums instead of constants for related values
- Keep constants DRY (Don't Repeat Yourself)
