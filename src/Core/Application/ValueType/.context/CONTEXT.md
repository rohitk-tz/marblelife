<!-- AUTO-GENERATED: Header -->
# ValueType — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Defines immutable value objects that represent domain concepts without identity - objects that are equal if their properties are equal. These types encapsulate data with meaningful semantics (Name, SecureHash, Dimension) and ensure data consistency through constructors and computed properties.

### Design Patterns
- **Value Object Pattern**: Objects are compared by value equality, not reference identity
- **Immutability (Partial)**: Types are designed to be set once and read many times (though C# properties allow mutation)
- **Encapsulation**: Computed properties (e.g., `Name.FullName`) hide internal representation details
- **Security**: `SecureHash` pairs hashed text with salt, preventing raw password storage

### Data Flow
1. Value objects are instantiated with validated data via constructors
2. Properties are read but typically not modified after construction
3. Computed properties (like `FullName`) derive values from stored properties
4. Objects are embedded in entities (e.g., `User.Name`) or passed between layers
5. Equality comparisons check property values, not object references
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### SecureHash.cs
```csharp
// Represents a cryptographically hashed value with its salt for password storage
public class SecureHash
{
    public string HashedText { get; set; }  // SHA256/bcrypt hash of the original value
    public string Salt { get; set; }        // Random salt added before hashing
    
    public SecureHash(string hashedText, string salt)
    {
        HashedText = hashedText;
        Salt = salt;
    }
    
    // Usage: Store both HashedText and Salt in database, never store plain password
}
```
**Purpose**: Securely store passwords and sensitive text with salted hashing.

### Name.cs
```csharp
// Represents a person's name with structured components
public class Name
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    
    // Computed property - concatenates name components
    public string FullName 
    { 
        get { return ToString(); } 
    }
    
    // Constructor 1: First and last name only
    public Name(string firstName, string lastName)
        : this(firstName, string.Empty, lastName)
    { }
    
    // Constructor 2: Full name with middle name
    public Name(string firstName, string middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }
    
    // Constructor 3: Default (for ORM hydration)
    public Name() { }
    
    // Formats name as "FirstName MiddleName LastName" or "FirstName LastName" if no middle name
    public override string ToString()
    {
        var name = !string.IsNullOrEmpty(MiddleName)
                   ? string.Format("{0} {1} {2}", FirstName, MiddleName, LastName)
                   : string.Format("{0} {1}", FirstName, LastName);
        return name;
    }
}
```
**Purpose**: Store person names with proper structure, avoiding "name" as single string.

### Dimension.cs
```csharp
// Represents 2D dimensions (width × height) for images, UI elements, etc.
public class Dimension
{
    public int Width { get; set; }   // Width in pixels or units
    public int Height { get; set; }  // Height in pixels or units
    
    // Example: new Dimension { Width = 800, Height = 600 } for 800×600 image
}
```
**Purpose**: Encapsulate size information for images, thumbnails, and layout calculations.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### SecureHash
- **Purpose**: Store salted password hashes securely
- **Properties**:
  - `HashedText`: The hashed value (never store plain text passwords)
  - `Salt`: Random string added before hashing (prevents rainbow table attacks)
- **Constructor**: `SecureHash(string hashedText, string salt)`
- **Usage Pattern**:
  ```csharp
  // Hash password during registration
  string salt = GenerateRandomSalt();
  string hashedPassword = HashWithSalt(plainPassword, salt);
  var secureHash = new SecureHash(hashedPassword, salt);
  user.PasswordHash = secureHash.HashedText;
  user.PasswordSalt = secureHash.Salt;
  
  // Verify password during login
  string hashedAttempt = HashWithSalt(loginPassword, user.PasswordSalt);
  bool isValid = hashedAttempt == user.PasswordHash;
  ```
- **Security Note**: Always use strong hashing algorithms (SHA256, bcrypt, Argon2)

### Name
- **Purpose**: Represent person names with structured data (first/middle/last)
- **Properties**:
  - `FirstName`: Given name
  - `MiddleName`: Middle name (optional, can be empty string)
  - `LastName`: Surname/family name
  - `FullName`: Read-only computed property returning formatted full name
- **Constructors**:
  - `Name()`: Default (for ORM hydration, JSON deserialization)
  - `Name(firstName, lastName)`: For names without middle name
  - `Name(firstName, middleName, lastName)`: Full name with middle name
- **Methods**:
  - `ToString()`: Returns formatted name string
    - With middle name: "John Robert Smith"
    - Without middle name: "John Smith"
- **Usage Pattern**:
  ```csharp
  var name = new Name("John", "Doe");
  Console.WriteLine(name.FullName);  // "John Doe"
  
  var fullName = new Name("Jane", "Marie", "Smith");
  Console.WriteLine(fullName.ToString());  // "Jane Marie Smith"
  ```

### Dimension
- **Purpose**: Represent 2D size (width × height) for images, UI elements
- **Properties**:
  - `Width`: Width in pixels or units (int)
  - `Height`: Height in pixels or units (int)
- **Usage Pattern**:
  ```csharp
  // Define thumbnail size
  var thumbSize = new Dimension { Width = 150, Height = 150 };
  
  // Resize image to dimensions
  ResizeImage(originalImage, thumbSize.Width, thumbSize.Height);
  
  // Calculate aspect ratio
  decimal aspectRatio = (decimal)dimension.Width / dimension.Height;
  ```
- **Common Use Cases**:
  - Image thumbnails (150×150, 300×300)
  - Profile photo dimensions
  - PDF page size
  - UI layout constraints
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
None - pure value object types

### External Dependencies
- **System.String** (BCL) — Used in Name and SecureHash
- **System.Int32** (BCL) — Used in Dimension

### Referenced By
- **User/Employee Entities** — Embed `Name` for structured person names
- **Security/Authentication Services** — Use `SecureHash` for password storage
- **File/Image Services** — Use `Dimension` for thumbnail generation and image metadata
- **UI Services** — Use `Dimension` for layout calculations
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Value Object Best Practices

**1. Immutability (Ideal)**
While these classes use setters for ORM compatibility, treat them as immutable after construction:
```csharp
// ✅ GOOD - Create once, don't modify
var name = new Name("John", "Doe");
user.Name = name;

// ❌ BAD - Modifying after creation
var name = new Name("John", "Doe");
user.Name = name;
name.FirstName = "Jane";  // Breaks value semantics
```

**2. Equality Semantics**
Value objects should be equal if properties are equal:
```csharp
// Consider implementing IEquatable<T> for proper equality:
var name1 = new Name("John", "Doe");
var name2 = new Name("John", "Doe");
// Currently: name1 == name2 is FALSE (reference equality)
// Ideal: name1.Equals(name2) should be TRUE (value equality)
```

**3. Null Handling in Name.ToString()**
Empty middle names are handled gracefully, but null checks could be added:
```csharp
// Current behavior
var name = new Name("John", null, "Doe");
name.ToString();  // Works: "John Doe"

// Edge case: null first/last names
var badName = new Name(null, null);
badName.ToString();  // Returns: " " (space between nulls)
```

### SecureHash Security Considerations

**Never roll your own crypto:**
```csharp
// ❌ INSECURE - Don't implement hashing yourself
string hash = ComputeMD5(password + salt);  // MD5 is broken
var secureHash = new SecureHash(hash, salt);

// ✅ SECURE - Use proven libraries
using (var hasher = new Rfc2898DeriveBytes(password, saltBytes, 10000))
{
    byte[] hashBytes = hasher.GetBytes(32);
    var secureHash = new SecureHash(Convert.ToBase64String(hashBytes), salt);
}

// ✅ BETTER - Use bcrypt/Argon2 via NuGet packages
string hash = BCrypt.Net.BCrypt.HashPassword(password);
var secureHash = new SecureHash(hash, "N/A");  // bcrypt includes salt
```

**Salt generation:**
```csharp
// ✅ Cryptographically secure random salt
using (var rng = new RNGCryptoServiceProvider())
{
    byte[] saltBytes = new byte[32];
    rng.GetBytes(saltBytes);
    string salt = Convert.ToBase64String(saltBytes);
}
```

### Dimension Usage Patterns

**Aspect ratio preservation:**
```csharp
public Dimension ResizeProportionally(Dimension original, int maxWidth, int maxHeight)
{
    decimal aspectRatio = (decimal)original.Width / original.Height;
    
    if (original.Width > maxWidth)
    {
        return new Dimension 
        { 
            Width = maxWidth, 
            Height = (int)(maxWidth / aspectRatio) 
        };
    }
    
    if (original.Height > maxHeight)
    {
        return new Dimension 
        { 
            Width = (int)(maxHeight * aspectRatio), 
            Height = maxHeight 
        };
    }
    
    return original;  // Fits within constraints
}
```

### Common Pitfalls

**1. Mutating Name after assignment:**
```csharp
// ❌ Creates unexpected behavior
user.Name = new Name("John", "Doe");
user.Name.FirstName = "Jane";  // Modifies the same object
// Other code referencing this Name sees the change
```

**2. Forgetting to store Salt with HashedText:**
```csharp
// ❌ Hash is useless without salt
user.PasswordHash = secureHash.HashedText;
// Missing: user.PasswordSalt = secureHash.Salt;

// ✅ Store both
user.PasswordHash = secureHash.HashedText;
user.PasswordSalt = secureHash.Salt;
```

**3. Using Dimension with negative values:**
```csharp
// ❌ No validation - allows invalid dimensions
var dimension = new Dimension { Width = -100, Height = -50 };

// ✅ Add validation in constructor or property setters
if (width <= 0 || height <= 0)
    throw new ArgumentException("Dimensions must be positive");
```
<!-- END CUSTOM SECTION -->
