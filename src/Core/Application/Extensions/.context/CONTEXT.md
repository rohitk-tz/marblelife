<!-- AUTO-GENERATED: Header -->
# Extensions — Module Context
**Version**: d49e7f258f9598da357b5d866d5502423c32f489
**Generated**: 2025-01-10T00:00:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Provides extension methods that add utility functions to built-in types (`string`, `decimal`) for path manipulation, URL conversion, and currency calculations. These methods encapsulate business logic for file path handling and multi-currency support, making them reusable across the application.

### Design Patterns
- **Extension Methods**: Static methods appear as instance methods on extended types for fluent API
- **Business Logic Encapsulation**: Path and currency logic centralized rather than scattered across controllers/services
- **Configuration Dependency**: Uses `ApplicationManager.Settings.MediaRootPath` for environment-specific path resolution

### Data Flow
1. **Path Extensions**: String path → transformation method → converted path (URL/file path/relative/absolute)
2. **Currency Extensions**: Decimal amount + exchange rate → conversion method → converted amount (default/local currency)
3. All path methods validate against `MediaRootPath` configuration setting
4. Currency methods protect against division by zero and round to 2 decimal places
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### PathExtensions.cs
```csharp
// Extends System.String with file path and URL conversion methods
public static class PathExtensions
{
    // Converts Windows path to URL format (backslash to forward slash)
    // Example: "C:\files\image.jpg" → "C:/files/image.jpg"
    public static string ToUrl(this string path)
    {
        return path.Replace("\\", "/");
    }
    
    // Converts URL to Windows path format (forward slash to backslash)
    // Example: "files/images/photo.jpg" → "files\images\photo.jpg"
    public static string ToPath(this string url)
    {
        return url.Replace("/", "\\");
    }
    
    // Extracts relative path by removing MediaRootPath prefix
    // Throws InvalidDataProvidedException if path doesn't contain MediaRootPath
    // Example: "C:\inetpub\marblelife\media\uploads\file.pdf" → "\uploads\file.pdf"
    public static string ToRelativePath(this string path)
    {
        if (!path.ToLower().Contains(ApplicationManager.Settings.MediaRootPath.ToLower()))
            throw new InvalidDataProvidedException();
        
        return path.ToLower().Replace(ApplicationManager.Settings.MediaRootPath.ToLower(), "");
    }
    
    // Converts relative path to full filesystem path by prepending MediaRootPath
    // Example: "\uploads\file.pdf" → "C:\inetpub\marblelife\media\uploads\file.pdf"
    public static string ToFullPath(this string relPath)
    {
        return ApplicationManager.Settings.MediaRootPath + relPath;
    }
    
    // Specialized method for HomeAdvisor integration - adds "\HomeAdvisor" subfolder
    // Example: "\leads\file.pdf" → "C:\inetpub\marblelife\media\HomeAdvisor\leads\file.pdf"
    public static string ToFullPathForHomeAdvisor(this string relPath)
    {
        return ApplicationManager.Settings.MediaRootPath + "\\HomeAdvisor" + relPath;
    }
}
```

### CurrencyExtension.cs
```csharp
// Extends System.Decimal with currency conversion methods
public static class CurrencyExtension
{
    // Converts local currency to default/base currency
    // Formula: amount * rate
    // Example: 100 EUR * 1.18 (EUR to USD rate) → 118.00 USD
    public static decimal ToDefaultCurrency(this decimal amount, decimal rate)
    {
        if (rate != 0 && amount != 0)
            return decimal.Parse((amount * rate).ToString("#.##"));  // Rounds to 2 decimal places
        else 
            return amount;  // Returns unchanged if rate or amount is zero
    }
    
    // Converts default/base currency to local currency
    // Formula: amount / rate
    // Example: 118 USD / 1.18 (EUR to USD rate) → 100.00 EUR
    public static decimal ToLocalCurrency(this decimal amount, decimal rate)
    {
        if (rate != 0 && amount != 0)
            return decimal.Parse((amount / rate).ToString("#.##"));  // Rounds to 2 decimal places
        else 
            return amount;  // Returns unchanged if rate or amount is zero
    }
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### PathExtensions.ToUrl()
- **Signature**: `string ToUrl(this string path)`
- **Purpose**: Convert Windows file path to URL format (replace `\` with `/`)
- **Input**: Windows path string (e.g., `"files\images\photo.jpg"`)
- **Output**: URL-safe path (e.g., `"files/images/photo.jpg"`)
- **Use Case**: Generating URLs for web display of file paths
- **Side Effects**: None - pure transformation

### PathExtensions.ToPath()
- **Signature**: `string ToPath(this string url)`
- **Purpose**: Convert URL format to Windows file path (replace `/` with `\`)
- **Input**: URL path string (e.g., `"files/images/photo.jpg"`)
- **Output**: Windows path (e.g., `"files\images\photo.jpg"`)
- **Use Case**: Converting user-provided URLs to filesystem paths for file operations
- **Side Effects**: None - pure transformation

### PathExtensions.ToRelativePath()
- **Signature**: `string ToRelativePath(this string path)`
- **Purpose**: Strip `MediaRootPath` prefix to get relative path for database storage
- **Input**: Full filesystem path (e.g., `"C:\media\uploads\file.pdf"`)
- **Output**: Relative path (e.g., `"\uploads\file.pdf"`)
- **Validation**: Throws `InvalidDataProvidedException` if input path doesn't contain `MediaRootPath`
- **Configuration Dependency**: Reads `ApplicationManager.Settings.MediaRootPath`
- **Use Case**: Storing portable file references in database (survives path changes)
- **Side Effects**: Case-insensitive comparison

### PathExtensions.ToFullPath()
- **Signature**: `string ToFullPath(this string relPath)`
- **Purpose**: Prepend `MediaRootPath` to relative path for file I/O operations
- **Input**: Relative path (e.g., `"\uploads\file.pdf"`)
- **Output**: Full filesystem path (e.g., `"C:\media\uploads\file.pdf"`)
- **Configuration Dependency**: Reads `ApplicationManager.Settings.MediaRootPath`
- **Use Case**: Resolving database-stored relative paths to actual file locations
- **Side Effects**: None - pure concatenation

### PathExtensions.ToFullPathForHomeAdvisor()
- **Signature**: `string ToFullPathForHomeAdvisor(this string relPath)`
- **Purpose**: Resolve HomeAdvisor-specific file paths with dedicated subfolder
- **Input**: Relative path (e.g., `"\leads\file.pdf"`)
- **Output**: Full path with HomeAdvisor subfolder (e.g., `"C:\media\HomeAdvisor\leads\file.pdf"`)
- **Configuration Dependency**: Reads `ApplicationManager.Settings.MediaRootPath`
- **Use Case**: Integration with HomeAdvisor lead management system
- **Side Effects**: None - pure concatenation

### CurrencyExtension.ToDefaultCurrency()
- **Signature**: `decimal ToDefaultCurrency(this decimal amount, decimal rate)`
- **Purpose**: Convert local currency amount to default/base currency (usually USD)
- **Input**: 
  - `amount`: Value in local currency
  - `rate`: Exchange rate (local to default, e.g., EUR to USD = 1.18)
- **Output**: Amount in default currency, rounded to 2 decimal places
- **Formula**: `amount * rate`
- **Zero Handling**: Returns original amount if either value is zero (avoids errors)
- **Use Case**: Aggregating multi-currency invoices into USD for reporting

### CurrencyExtension.ToLocalCurrency()
- **Signature**: `decimal ToLocalCurrency(this decimal amount, decimal rate)`
- **Purpose**: Convert default/base currency amount to local currency
- **Input**:
  - `amount`: Value in default currency (e.g., USD)
  - `rate`: Exchange rate (local to default, e.g., EUR to USD = 1.18)
- **Output**: Amount in local currency, rounded to 2 decimal places
- **Formula**: `amount / rate`
- **Zero Handling**: Returns original amount if either value is zero (prevents division by zero)
- **Use Case**: Displaying USD prices in user's local currency
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.ApplicationManager** — Provides access to `Settings.MediaRootPath`
- **Core.Application.Exceptions.InvalidDataProvidedException** — Thrown when path validation fails

### External Dependencies
- **System.String** (BCL) — Extended by `PathExtensions`
- **System.Decimal** (BCL) — Extended by `CurrencyExtension`

### Referenced By
- **Core.Application.Impl.FileService** — Uses path extensions for file storage/retrieval
- **Core.Application.Impl.HomeAdvisorFileParser** — Uses `ToFullPathForHomeAdvisor()` for lead imports
- **Controllers** — Use path extensions for URL generation
- **Billing Services** — Use currency extensions for multi-currency invoicing
- **Reporting Services** — Use currency extensions for financial aggregations
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Path Extension Best Practices

**Always use extensions for path manipulation:**
```csharp
// ✅ GOOD - Using extensions
string dbPath = fullPath.ToRelativePath();  // Store in database
string filePath = dbPath.ToFullPath();      // Resolve for file I/O
string url = filePath.ToUrl();              // Display in UI

// ❌ BAD - Manual string manipulation (error-prone)
string dbPath = fullPath.Replace(settings.MediaRootPath, "");
string filePath = settings.MediaRootPath + dbPath;
string url = filePath.Replace("\\", "/");
```

**Path Storage Strategy:**
- **Database**: Always store relative paths (`\uploads\file.pdf`)
- **File I/O**: Convert to full paths using `ToFullPath()`
- **URLs**: Convert to URL format using `ToUrl()` before sending to client

**Why relative paths in database?**
- Survives server migrations (path changes)
- Works across dev/staging/production environments
- Enables cloud storage migration (S3, Azure Blob)

### Currency Extension Best Practices

**Exchange Rate Convention:**
The `rate` parameter represents **local to default currency** conversion:
```csharp
// EUR to USD rate is 1.18 (1 EUR = 1.18 USD)
decimal rate = 1.18m;

// Convert 100 EUR to USD
decimal usd = (100m).ToDefaultCurrency(rate);  // 100 * 1.18 = 118.00 USD

// Convert 118 USD to EUR
decimal eur = (118m).ToLocalCurrency(rate);    // 118 / 1.18 = 100.00 EUR
```

**Rounding Behavior:**
- Both methods round to 2 decimal places using `ToString("#.##")`
- This can cause precision loss in chained conversions
- For precise calculations, use `decimal` throughout and round only for display

**Zero Handling:**
- If `rate == 0`: Returns original amount (prevents division by zero)
- If `amount == 0`: Returns 0 (optimization, skips unnecessary calculation)

### Common Pitfalls

**1. Forgetting to call ToRelativePath before database insert:**
```csharp
// ❌ Stores full path (breaks portability)
file.RelativeLocation = "C:\inetpub\media\uploads\file.pdf";

// ✅ Stores relative path
file.RelativeLocation = fullPath.ToRelativePath();
```

**2. Using ToFullPath on already-full paths:**
```csharp
string fullPath = "C:\media\uploads\file.pdf";
// ❌ Double prefix: "C:\mediaC:\media\uploads\file.pdf"
string broken = fullPath.ToFullPath();

// ✅ Check if already full path
string resolved = fullPath.Contains(settings.MediaRootPath) 
    ? fullPath 
    : fullPath.ToFullPath();
```

**3. Chaining currency conversions loses precision:**
```csharp
// ❌ Precision loss due to rounding at each step
decimal usd = 100m;
decimal eur = usd.ToLocalCurrency(1.18m);      // 84.75 EUR
decimal usdAgain = eur.ToDefaultCurrency(1.18m); // 100.01 USD (rounding error)

// ✅ Store amounts in a single base currency
```

**4. InvalidDataProvidedException on non-media paths:**
```csharp
string systemPath = "C:\Windows\System32\file.txt";
// ❌ Throws exception because path doesn't contain MediaRootPath
string relative = systemPath.ToRelativePath();

// ✅ Only use ToRelativePath for media files
if (systemPath.Contains(settings.MediaRootPath))
{
    string relative = systemPath.ToRelativePath();
}
```
<!-- END CUSTOM SECTION -->
