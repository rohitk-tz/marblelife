# Core/Application/Extensions - AI Context

## Purpose

This folder contains C# extension methods that add functionality to existing types, providing reusable utility methods across the MarbleLife application.

## Contents

Extension methods for:
- **String Extensions**: Validation, formatting, parsing
- **DateTime Extensions**: Date calculations, formatting
- **Collection Extensions**: LINQ enhancements, filtering
- **Enum Extensions**: String conversion, display names
- **Object Extensions**: Null checking, cloning, serialization
- **Validation Extensions**: Input validation helpers

## Common Extensions

### String Extensions
```csharp
public static class StringExtensions
{
    public static bool IsValidEmail(this string email);
    public static string ToTitleCase(this string text);
    public static string Truncate(this string text, int maxLength);
    public static string ToSlug(this string text);
    public static bool IsNullOrEmpty(this string text);
}
```

### DateTime Extensions
```csharp
public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime date);
    public static DateTime EndOfDay(this DateTime date);
    public static bool IsWeekend(this DateTime date);
    public static int BusinessDaysUntil(this DateTime start, DateTime end);
    public static string ToFriendlyString(this DateTime date);
}
```

### Enum Extensions
```csharp
public static class EnumExtensions
{
    public static string GetDescription(this Enum value);
    public static T ParseEnum<T>(this string value);
    public static IEnumerable<T> GetValues<T>();
}
```

### Collection Extensions
```csharp
public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection);
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> collection);
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action);
}
```

## For AI Agents

**Using Extensions**:
```csharp
// String extensions
if (email.IsValidEmail())
{
    var slug = customerName.ToSlug();
    var preview = description.Truncate(100);
}

// DateTime extensions
var startOfWeek = DateTime.Now.StartOfWeek();
var businessDays = startDate.BusinessDaysUntil(endDate);
var friendly = appointmentDate.ToFriendlyString(); // "Today at 2:00 PM"

// Enum extensions
var status = "Active".ParseEnum<CustomerStatus>();
var description = status.GetDescription();
var allStatuses = EnumExtensions.GetValues<CustomerStatus>();

// Collection extensions
if (!customerList.IsNullOrEmpty())
{
    var active = customerList.WhereNotNull().Where(c => c.IsActive);
    active.ForEach(c => Console.WriteLine(c.Name));
}
```

**Creating New Extensions**:
```csharp
public static class MyExtensions
{
    // Extension method syntax: this keyword on first parameter
    public static string MyCustomMethod(this string input, string parameter)
    {
        // Implementation
        return input + parameter;
    }
}
```

## For Human Developers

Best practices for extension methods:
- Place in static classes with descriptive names
- Use `this` keyword on first parameter
- Keep methods focused and single-purpose
- Avoid ambiguous method names that could conflict
- Consider performance implications
- Document behavior with XML comments
- Test edge cases thoroughly
- Don't overuse - consider regular methods if only used in one place
- Group related extensions in same class
- Use appropriate parameter validation
- Consider null inputs and handle appropriately
