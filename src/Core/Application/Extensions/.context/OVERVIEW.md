<!-- AUTO-GENERATED: Header -->
# Extensions
> Extension methods for path manipulation, URL conversion, and currency calculations
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Extensions** module provides utility methods that extend built-in .NET types (`string`, `decimal`) with application-specific functionality. These extensions eliminate boilerplate code and encapsulate business logic for file path handling and multi-currency support.

**Two Extension Classes:**
1. **PathExtensions**: Converts between Windows paths, URLs, relative paths, and absolute paths
2. **CurrencyExtension**: Converts amounts between default currency (USD) and local currencies

**Why extensions instead of utility classes?**
- **Fluent API**: `path.ToUrl()` reads better than `PathUtil.ToUrl(path)`
- **Discoverability**: IntelliSense shows methods directly on `string` and `decimal` types
- **Chainability**: `path.ToRelativePath().ToUrl()` chains naturally
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Store File Path in Database
```csharp
using Core.Application.Extensions;

// User uploads file to C:\inetpub\marblelife\media\invoices\2023\invoice-001.pdf
string uploadedPath = "C:\\inetpub\\marblelife\\media\\invoices\\2023\\invoice-001.pdf";

// Extract relative path for database storage (portable across environments)
string relativePath = uploadedPath.ToRelativePath();
// Result: "\invoices\2023\invoice-001.pdf"

var file = new File
{
    Name = "invoice-001.pdf",
    RelativeLocation = relativePath  // Store relative path
};

fileRepository.Add(file);
unitOfWork.Commit();
```

### Example 2: Resolve Stored Path for File Operations
```csharp
using Core.Application.Extensions;

// Load file from database
var file = fileRepository.GetById(fileId);
string relativePath = file.RelativeLocation;  // "\invoices\2023\invoice-001.pdf"

// Convert to full filesystem path for reading
string fullPath = relativePath.ToFullPath();
// Result: "C:\inetpub\marblelife\media\invoices\2023\invoice-001.pdf"

byte[] fileBytes = File.ReadAllBytes(fullPath);
```

### Example 3: Generate URL for Frontend
```csharp
using Core.Application.Extensions;

var file = fileRepository.GetById(fileId);
string relativePath = file.RelativeLocation;  // "\invoices\2023\invoice-001.pdf"

// Convert to URL format for <img src> or <a href>
string url = relativePath.ToUrl();
// Result: "/invoices/2023/invoice-001.pdf"

return new FileModel
{
    Id = file.Id,
    Name = file.Name,
    Url = url  // Frontend can use directly in <img src={url}>
};
```

### Example 4: Handle HomeAdvisor Integration
```csharp
using Core.Application.Extensions;

// HomeAdvisor lead files stored in dedicated subfolder
string relativeLeadPath = "\\leads\\2023\\lead-12345.pdf";

// Resolve to HomeAdvisor-specific full path
string fullPath = relativeLeadPath.ToFullPathForHomeAdvisor();
// Result: "C:\inetpub\marblelife\media\HomeAdvisor\leads\2023\lead-12345.pdf"

if (File.Exists(fullPath))
{
    ProcessLeadDocument(fullPath);
}
```

### Example 5: Convert Local Currency to USD
```csharp
using Core.Application.Extensions;

// Invoice amount in EUR
decimal amountEUR = 850.50m;

// Exchange rate: 1 EUR = 1.18 USD
decimal eurToUsdRate = 1.18m;

// Convert to USD for financial reporting
decimal amountUSD = amountEUR.ToDefaultCurrency(eurToUsdRate);
// Result: 1003.59 USD (850.50 * 1.18 = 1003.59)
```

### Example 6: Display USD Amount in User's Currency
```csharp
using Core.Application.Extensions;

// Invoice total stored in USD (default currency)
decimal totalUSD = 1200.00m;

// User's preferred currency is GBP
// Exchange rate: 1 GBP = 1.27 USD
decimal gbpToUsdRate = 1.27m;

// Convert to GBP for display
decimal totalGBP = totalUSD.ToLocalCurrency(gbpToUsdRate);
// Result: 944.88 GBP (1200 / 1.27 = 944.88)

return new InvoiceViewModel
{
    TotalUSD = totalUSD,
    TotalLocalCurrency = totalGBP,
    CurrencyCode = "GBP"
};
```

### Example 7: Handle Zero Exchange Rates
```csharp
using Core.Application.Extensions;

decimal amount = 100.00m;
decimal zeroRate = 0m;

// Both methods safely handle zero rates (returns original amount)
decimal result1 = amount.ToDefaultCurrency(zeroRate);  // 100.00 (unchanged)
decimal result2 = amount.ToLocalCurrency(zeroRate);    // 100.00 (unchanged)
```

### Example 8: Convert Windows Path to URL and Back
```csharp
using Core.Application.Extensions;

string windowsPath = "documents\\reports\\2023\\annual-report.pdf";

// Convert to URL format
string url = windowsPath.ToUrl();
// Result: "documents/reports/2023/annual-report.pdf"

// Convert back to Windows path
string pathAgain = url.ToPath();
// Result: "documents\reports\2023\annual-report.pdf"
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

### PathExtensions
| Method | Input | Output | Purpose |
|--------|-------|--------|---------|
| `ToUrl()` | Windows path | URL path | Replace `\` with `/` for web URLs |
| `ToPath()` | URL path | Windows path | Replace `/` with `\` for file I/O |
| `ToRelativePath()` | Full path | Relative path | Strip MediaRootPath prefix for database storage |
| `ToFullPath()` | Relative path | Full path | Prepend MediaRootPath for file operations |
| `ToFullPathForHomeAdvisor()` | Relative path | Full path | Prepend MediaRootPath + "\HomeAdvisor" subfolder |

### CurrencyExtension
| Method | Formula | Purpose |
|--------|---------|---------|
| `ToDefaultCurrency(rate)` | `amount * rate` | Convert local currency → USD (or base currency) |
| `ToLocalCurrency(rate)` | `amount / rate` | Convert USD (or base currency) → local currency |

**Exchange Rate Convention**: `rate` represents local to default currency (e.g., EUR to USD = 1.18)
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: InvalidDataProvidedException when calling ToRelativePath()
**Cause**: Path doesn't contain `ApplicationManager.Settings.MediaRootPath`.
**Solution**: Only use `ToRelativePath()` on paths within the media directory:
```csharp
// ✅ GOOD
string mediaPath = "C:\\media\\uploads\\file.pdf";  // Contains MediaRootPath
string relative = mediaPath.ToRelativePath();

// ❌ BAD
string systemPath = "C:\\Windows\\file.txt";  // Doesn't contain MediaRootPath
string relative = systemPath.ToRelativePath();  // Throws exception
```

### Issue: File not found after calling ToFullPath()
**Cause**: `MediaRootPath` setting is incorrect or relative path has wrong format.
**Solution**: 
- Verify `ApplicationManager.Settings.MediaRootPath` is configured correctly
- Ensure relative path starts with `\` or `/`
- Check that file actually exists at resolved location

### Issue: Currency conversion results are incorrect
**Cause**: Using wrong exchange rate direction.
**Solution**: Remember rate is **local to default**:
```csharp
// ✅ CORRECT: 1 EUR = 1.18 USD
decimal eurToUsd = 1.18m;
decimal usd = (100m).ToDefaultCurrency(eurToUsd);  // 100 EUR → 118 USD

// ❌ WRONG: Using inverse rate
decimal usdToEur = 0.847m;  // Don't use this
decimal usd = (100m).ToDefaultCurrency(usdToEur);  // Wrong result
```

### Issue: Chained currency conversions lose precision
**Cause**: Rounding at each step compounds errors.
**Solution**: Minimize conversions - store in base currency, convert only for display:
```csharp
// ❌ BAD - Multiple conversions
decimal usd = 100m;
decimal eur = usd.ToLocalCurrency(1.18m);
decimal gbp = eur.ToDefaultCurrency(1.18m).ToLocalCurrency(1.27m);  // Precision loss

// ✅ GOOD - Single conversion from base currency
decimal usd = 100m;  // Store in USD
decimal gbp = usd.ToLocalCurrency(1.27m);  // Convert once for display
```

### Issue: Paths work on Windows but break on Linux
**Cause**: `ToPath()` and `ToFullPath()` use Windows-style backslashes.
**Solution**: 
- For cross-platform, use `Path.Combine()` instead of `ToFullPath()`
- Store paths with forward slashes in database
- Use `ToUrl()` to normalize paths

### Issue: URL has double slashes or backslashes
**Cause**: Mixing path formats or calling methods in wrong order.
**Solution**: Follow the correct conversion chain:
```csharp
// ✅ CORRECT order
string fullPath = "C:\\media\\uploads\\file.pdf";
string relative = fullPath.ToRelativePath();  // "\uploads\file.pdf"
string url = relative.ToUrl();                // "/uploads/file.pdf"

// ❌ WRONG - skip ToRelativePath
string url = fullPath.ToUrl();  // "C:/media/uploads/file.pdf" (wrong)
```

### Issue: HomeAdvisor files not found
**Cause**: Using `ToFullPath()` instead of `ToFullPathForHomeAdvisor()`.
**Solution**: Use the specialized method for HomeAdvisor paths:
```csharp
// ❌ WRONG
string path = relPath.ToFullPath();  
// Result: "C:\media\leads\file.pdf" (missing HomeAdvisor subfolder)

// ✅ CORRECT
string path = relPath.ToFullPathForHomeAdvisor();
// Result: "C:\media\HomeAdvisor\leads\file.pdf"
```
<!-- END CUSTOM SECTION -->
