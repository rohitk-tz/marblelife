# Core/Application/ValueType - AI Context

## Purpose

This folder contains value objects - immutable types that represent concepts through their value rather than identity. Value objects are a key pattern in Domain-Driven Design (DDD).

## Contents

Value object classes:
- **EmailAddress**: Email validation and standardization
- **PhoneNumber**: Phone number formatting and validation
- **Money**: Currency amount with precision handling
- **Address**: Complete address with validation
- **DateRange**: Start and end date pair
- **TimeRange**: Time period with validation
- **Percentage**: Percentage values with constraints
- **Coordinates**: Latitude/longitude pair

## Value Object Characteristics

1. **Immutable**: Once created, cannot be modified
2. **Equality by Value**: Two instances are equal if all properties match
3. **Self-Validating**: Validates on construction
4. **No Identity**: Compared by value, not reference
5. **Behavior**: Contains relevant operations

## For AI Agents

**Creating Value Object**:
```csharp
public class EmailAddress : IEquatable<EmailAddress>
{
    public string Value { get; private set; }
    
    private EmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (!IsValid(email))
            throw new ArgumentException("Invalid email format", nameof(email));
            
        Value = email.ToLowerInvariant().Trim();
    }
    
    public static EmailAddress Create(string email) => new EmailAddress(email);
    
    public static bool TryCreate(string email, out EmailAddress emailAddress)
    {
        try
        {
            emailAddress = new EmailAddress(email);
            return true;
        }
        catch
        {
            emailAddress = null;
            return false;
        }
    }
    
    private static bool IsValid(string email)
    {
        // Email validation logic
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    
    public bool Equals(EmailAddress other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }
    
    public override bool Equals(object obj)
    {
        return Equals(obj as EmailAddress);
    }
    
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    
    public override string ToString()
    {
        return Value;
    }
    
    public static implicit operator string(EmailAddress email)
    {
        return email.Value;
    }
}
```

**Using Value Objects**:
```csharp
// Create value object
var email = EmailAddress.Create("user@example.com");

// Try create (no exception if invalid)
if (EmailAddress.TryCreate(userInput, out var validEmail))
{
    customer.Email = validEmail;
}

// Value equality
var email1 = EmailAddress.Create("user@example.com");
var email2 = EmailAddress.Create("user@example.com");
bool areEqual = email1 == email2; // true

// Use in entities
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public EmailAddress Email { get; set; }
    public PhoneNumber Phone { get; set; }
    public Address BillingAddress { get; set; }
}
```

**Money Value Object Example**:
```csharp
public class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        Amount = Math.Round(amount, 2);
        Currency = currency?.ToUpperInvariant() ?? "USD";
    }
    
    public static Money Create(decimal amount, string currency = "USD")
    {
        return new Money(amount, currency);
    }
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
            
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }
    
    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }
    
    public static Money operator *(Money money, decimal factor)
    {
        return money.Multiply(factor);
    }
    
    // Equality implementation...
}
```

## For Human Developers

Benefits of value objects:
- Encapsulate validation logic in one place
- Make invalid states unrepresentable
- Improve code readability and maintainability
- Reduce primitive obsession
- Enable rich domain models
- Facilitate testing

When to use value objects:
- Complex types with validation rules (email, phone, SSN)
- Amounts with units (money, weight, distance)
- Ranges and intervals (date range, time range)
- Coordinates and geometric types
- Any concept better represented by value than identity

Best practices:
- Make value objects immutable (readonly fields, private setters)
- Validate in constructor
- Provide factory methods (Create, TryCreate)
- Override Equals and GetHashCode for value equality
- Implement IEquatable<T>
- Override ToString for debugging
- Add relevant operations as methods
- Consider implicit/explicit conversion operators
- Use descriptive names (EmailAddress, not Email)
- Document validation rules and constraints
