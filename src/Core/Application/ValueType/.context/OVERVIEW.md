<!-- AUTO-GENERATED: Header -->
# ValueType
> Immutable value objects for names, secure hashes, and dimensions
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **ValueType** module contains three value object classes that represent domain concepts without identity. Unlike entities (which have unique IDs), value objects are equal if their properties are equal - two `Name` objects with identical first/last names are considered the same.

**Three Value Objects:**
1. **SecureHash**: Pairs hashed text with salt for secure password storage
2. **Name**: Structured person name (first/middle/last) with computed FullName
3. **Dimension**: 2D size representation (width × height) for images and UI elements

**Value Object Benefits:**
- **Type Safety**: `Name` instead of three separate string fields prevents field confusion
- **Encapsulation**: `Name.FullName` hides formatting logic from consumers
- **Immutability**: Objects are created once and not modified (by convention)
- **Semantic Clarity**: `Dimension` makes intent clearer than `(int, int)` tuple
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Store Structured Person Name
```csharp
using Core.Application.ValueType;

// Create name with first and last only
var name = new Name("John", "Doe");
Console.WriteLine(name.FullName);  // "John Doe"

// Create name with middle name
var fullName = new Name("Jane", "Marie", "Smith");
Console.WriteLine(fullName.ToString());  // "Jane Marie Smith"

// Use in entity
public class User : DomainBase
{
    public Name Name { get; set; }
    public string Email { get; set; }
}

var user = new User
{
    Name = new Name("John", "Doe"),
    Email = "john.doe@example.com"
};

// Access computed property
Console.WriteLine($"Welcome, {user.Name.FullName}!");  // "Welcome, John Doe!"
```

### Example 2: Hash Password Securely
```csharp
using Core.Application.ValueType;
using System.Security.Cryptography;
using System.Text;

// Generate random salt
string GenerateSalt()
{
    using (var rng = new RNGCryptoServiceProvider())
    {
        byte[] saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
}

// Hash password with salt
string HashPassword(string password, string salt)
{
    using (var sha256 = SHA256.Create())
    {
        byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
        byte[] hash = sha256.ComputeHash(saltedPassword);
        return Convert.ToBase64String(hash);
    }
}

// Registration: Hash new password
string plainPassword = "MySecureP@ssw0rd";
string salt = GenerateSalt();
string hashedPassword = HashPassword(plainPassword, salt);

var secureHash = new SecureHash(hashedPassword, salt);

// Store in database
user.PasswordHash = secureHash.HashedText;
user.PasswordSalt = secureHash.Salt;

// Login: Verify password
string loginPassword = "MySecureP@ssw0rd";
string hashedAttempt = HashPassword(loginPassword, user.PasswordSalt);
bool isValid = hashedAttempt == user.PasswordHash;
```

### Example 3: Define Image Thumbnail Size
```csharp
using Core.Application.ValueType;

// Define standard thumbnail dimensions
var smallThumb = new Dimension { Width = 150, Height = 150 };
var mediumThumb = new Dimension { Width = 300, Height = 300 };
var largeThumb = new Dimension { Width = 800, Height = 600 };

// Use for image processing
public byte[] GenerateThumbnail(byte[] originalImage, Dimension targetSize)
{
    using (var image = Image.Load(originalImage))
    {
        image.Mutate(x => x.Resize(targetSize.Width, targetSize.Height));
        
        using (var ms = new MemoryStream())
        {
            image.SaveAsJpeg(ms);
            return ms.ToArray();
        }
    }
}

byte[] thumbnail = GenerateThumbnail(uploadedImage, mediumThumb);
```

### Example 4: Calculate Aspect Ratio
```csharp
using Core.Application.ValueType;

var dimension = new Dimension { Width = 1920, Height = 1080 };

decimal aspectRatio = (decimal)dimension.Width / dimension.Height;
Console.WriteLine($"Aspect Ratio: {aspectRatio:F2}");  // "Aspect Ratio: 1.78" (16:9)

// Determine orientation
string orientation = dimension.Width > dimension.Height 
    ? "Landscape" 
    : dimension.Width < dimension.Height 
        ? "Portrait" 
        : "Square";
Console.WriteLine($"Orientation: {orientation}");  // "Orientation: Landscape"
```

### Example 5: Resize Image Proportionally
```csharp
using Core.Application.ValueType;

public Dimension ResizeToFit(Dimension original, int maxWidth, int maxHeight)
{
    decimal aspectRatio = (decimal)original.Width / original.Height;
    
    int newWidth = original.Width;
    int newHeight = original.Height;
    
    // Scale down if too wide
    if (newWidth > maxWidth)
    {
        newWidth = maxWidth;
        newHeight = (int)(maxWidth / aspectRatio);
    }
    
    // Scale down if too tall
    if (newHeight > maxHeight)
    {
        newHeight = maxHeight;
        newWidth = (int)(maxHeight * aspectRatio);
    }
    
    return new Dimension { Width = newWidth, Height = newHeight };
}

var original = new Dimension { Width = 3000, Height = 2000 };
var resized = ResizeToFit(original, 800, 600);
// Result: { Width = 800, Height = 533 } (maintains 1.5 aspect ratio)
```

### Example 6: Store Name in Database (Entity Framework)
```csharp
using Core.Application.ValueType;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
}

public class Employee : DomainBase
{
    // Option 1: Owned type (EF Core 2.1+) - stores in same table
    public Name Name { get; set; }
}

// Configure as owned type in OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Employee>()
        .OwnsOne(e => e.Name, name =>
        {
            name.Property(n => n.FirstName).HasColumnName("FirstName");
            name.Property(n => n.MiddleName).HasColumnName("MiddleName");
            name.Property(n => n.LastName).HasColumnName("LastName");
        });
}

// Query by name components
var employees = dbContext.Employees
    .Where(e => e.Name.LastName == "Smith")
    .ToList();
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### SecureHash
| Property | Type | Purpose |
|----------|------|---------|
| `HashedText` | string | Cryptographic hash of original value |
| `Salt` | string | Random salt added before hashing |

**Constructor**: `SecureHash(string hashedText, string salt)`

### Name
| Property | Type | Purpose |
|----------|------|---------|
| `FirstName` | string | Given name |
| `MiddleName` | string | Middle name (optional, can be empty) |
| `LastName` | string | Surname/family name |
| `FullName` | string | Computed property returning formatted full name |

**Constructors**: 
- `Name()` - Default
- `Name(firstName, lastName)` - Without middle name
- `Name(firstName, middleName, lastName)` - With middle name

**Methods**:
- `ToString()` - Returns "FirstName MiddleName LastName" or "FirstName LastName"

### Dimension
| Property | Type | Purpose |
|----------|------|---------|
| `Width` | int | Width in pixels or units |
| `Height` | int | Height in pixels or units |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: Name.FullName returns " " (single space) for empty names
**Cause**: ToString() concatenates with space even when names are null/empty.
**Solution**: Add null checks or use object initializer with valid data:
```csharp
// ❌ BAD - Creates empty name
var name = new Name(null, null);
Console.WriteLine(name.FullName);  // " " (space)

// ✅ GOOD - Validate before creating
if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
{
    var name = new Name(firstName, lastName);
}
```

### Issue: Password verification always fails with SecureHash
**Cause**: Salt not stored or retrieved correctly from database.
**Solution**: Always store both HashedText and Salt:
```csharp
// ✅ Store both
user.PasswordHash = secureHash.HashedText;
user.PasswordSalt = secureHash.Salt;

// ✅ Verify using stored salt
string hashedAttempt = HashPassword(loginPassword, user.PasswordSalt);
bool isValid = hashedAttempt == user.PasswordHash;
```

### Issue: Dimension has negative or zero values
**Cause**: No validation in property setters.
**Solution**: Add validation logic:
```csharp
public class ValidatedDimension
{
    private int _width;
    private int _height;
    
    public int Width 
    { 
        get => _width; 
        set => _width = value > 0 ? value : throw new ArgumentException("Width must be positive"); 
    }
    
    public int Height 
    { 
        get => _height; 
        set => _height = value > 0 ? value : throw new ArgumentException("Height must be positive"); 
    }
}
```

### Issue: Name comparison doesn't work as expected
**Cause**: Uses reference equality instead of value equality.
**Solution**: Implement IEquatable<Name> or compare properties:
```csharp
// ❌ Reference equality (always false)
var name1 = new Name("John", "Doe");
var name2 = new Name("John", "Doe");
bool equal = (name1 == name2);  // FALSE

// ✅ Compare properties
bool equal = name1.FirstName == name2.FirstName && 
             name1.MiddleName == name2.MiddleName && 
             name1.LastName == name2.LastName;  // TRUE

// ✅ Or compare formatted names
bool equal = name1.FullName == name2.FullName;  // TRUE
```

### Issue: SecureHash doesn't provide password verification method
**Cause**: SecureHash is just a data holder, not a cryptographic service.
**Solution**: Create a separate PasswordHashingService:
```csharp
public interface IPasswordHasher
{
    SecureHash HashPassword(string password);
    bool VerifyPassword(string password, string hash, string salt);
}

public class PasswordHashingService : IPasswordHasher
{
    public SecureHash HashPassword(string password)
    {
        string salt = GenerateSalt();
        string hash = ComputeHash(password, salt);
        return new SecureHash(hash, salt);
    }
    
    public bool VerifyPassword(string password, string hash, string salt)
    {
        string attemptHash = ComputeHash(password, salt);
        return attemptHash == hash;
    }
}
```

### Issue: Image distortion when resizing with Dimension
**Cause**: Not preserving aspect ratio.
**Solution**: Calculate proportional dimensions:
```csharp
// ❌ Distorts image
var resized = new Dimension { Width = 300, Height = 300 };  // Forces square

// ✅ Preserves aspect ratio
public Dimension ResizeProportionally(Dimension original, int maxDimension)
{
    decimal aspectRatio = (decimal)original.Width / original.Height;
    
    if (original.Width > original.Height)
    {
        return new Dimension 
        { 
            Width = maxDimension, 
            Height = (int)(maxDimension / aspectRatio) 
        };
    }
    else
    {
        return new Dimension 
        { 
            Width = (int)(maxDimension * aspectRatio), 
            Height = maxDimension 
        };
    }
}
```
<!-- END CUSTOM SECTION -->
