# Sales/Enum - AI Context

## Purpose

This folder contains enumeration types for the **Sales** module. Enums provide type-safe constants for domain-specific values.

## Contents

Enumeration types defining:
- Status values (e.g., Active, Inactive, Pending)
- Type categories (e.g., Residential, Commercial)
- State machines (e.g., workflow stages)
- Configuration options
- Classification types

## For AI Agents

**Enum Pattern**:
```csharp
public enum EntityStatus
{
    New = 0,
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Deleted = 4
}
```

**Using Enums**:
- Reference as `EnumName.Value`
- Store as integers in database
- Convert to strings for display using `.ToString()`
- Use in switch statements for type-safe branching

**Best Practices**:
- Use meaningful names
- Assign explicit values if order matters
- Add [Flags] attribute for bitwise enums
- Use XML comments to document values
- Consider using Description attributes for display text
- Never delete enum values (breaks existing data)
- Add new values at end to maintain compatibility

## For Human Developers

Enumerations provide strongly-typed constants. Guidelines:
- Prefer enums over magic strings/numbers
- Use enums in domain entities and ViewModels
- Add Display attributes for UI-friendly names
- Consider using enum extensions for common operations
- Document non-obvious enum values
- Maintain backward compatibility when modifying
