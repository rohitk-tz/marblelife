# Organizations Enum Module
> **Type-Safe Constants for Multi-Tenant Franchisee Management**

## Overview

This module contains **13 enumeration types** that define the operational vocabulary of the Marblelife franchisee management system. These enums act as **strongly-typed facades** for database lookup tables, replacing magic numbers with compile-time constants.

**Key Insight**: Every enum value maps to a specific row in the `Lookup` table via its integer ID. When you see `ServiceFeeType.Bookkeeping = 172`, that `172` is the actual primary key in the database.

### Why This Matters
```csharp
// ❌ Before: What does 172 mean?
var fees = db.ServiceFees.Where(f => f.TypeId == 172);

// ✅ After: Crystal clear intent
var fees = db.ServiceFees.Where(f => f.TypeId == (long)ServiceFeeType.Bookkeeping);
```

---

## 🗂️ Enum Catalog

| Enum | Values | Purpose |
|------|--------|---------|
| **ServiceFeeType** | 11 | Recurring and one-time charges beyond royalties (Loan, Bookkeeping, Payroll, etc.) |
| **ServiceType** | 38 | Catalog of restoration, maintenance, and product services offered |
| **ServiceTypeCategory** | 5 | High-level grouping of services (Restoration, Maintenance, ProductChannel) |
| **DocumentType** | 13 | Legal and compliance document categories (W9, COI, License, etc.) |
| **DocumentReportType** | 9 | Financial reporting document subset (P&L, Balance Sheet, Tax Filings) |
| **DocumentCategory** | 2 | Top-level document organization (Franchisee Management vs National Account) |
| **PaymentFrequency** | 5 | Royalty billing cycles (Weekly, Monthly, TwiceAMonth, etc.) |
| **LoanType** | 4 | Loan program categories (ISQFT, SurgicalStrike, Geofence, Other) |
| **LanguageEnum** | 2 | Multilingual support (English, Spanish) |
| **LeadPerformanceEnum** | 2 | Marketing spend tracking (PPC, SEO) |
| **FranchiseeNotesEnum** | 3 | Note categorization (Duration, Owner Notes, Call Center Notes) |
| **FranchiseeCallCategory** | 4 | Call routing preferences (Front Office, Office Person, Next Day Response) |
| **OrganizationNames** | 4 | ⚠️ **Legacy**: Hardcoded franchisee IDs (avoid using) |

---

## 🚀 Quick Start

### Basic Usage: Filtering by Enum

```csharp
using Core.Organizations.Enum;

// Query service fees by type
public IEnumerable<FranchiseeServiceFee> GetBookkeepingFees(long franchiseeId)
{
    return _serviceFeeRepository.Table
        .Where(f => f.FranchiseeId == franchiseeId)
        .Where(f => f.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping) // Cast to long!
        .ToList();
}

// Filter out non-recurring fees
var recurringOnly = serviceFees.Where(f => 
    f.TypeId != (long)ServiceFeeType.Loan &&
    f.TypeId != (long)ServiceFeeType.OneTimeProject &&
    f.TypeId != (long)ServiceFeeType.InterestAmount);
```

### Creating New Entities with Enum References

```csharp
// Create a new service fee using enum constant
var newFee = new FranchiseeServiceFee
{
    FranchiseeId = 42,
    ServiceFeeTypeId = (long)ServiceFeeType.PayrollProcessing, // Enum → FK
    Amount = 250.00m,
    StartDate = DateTime.UtcNow,
    IsActive = true
};
_repository.Add(newFee);

// Create a loan with specific type
var loan = new FranchiseeLoan
{
    FranchiseeId = 42,
    LoanTypeId = (long)LoanType.Geofence,
    Amount = 75000m,
    Duration = 48,
    InterestratePerAnum = 4.5m
};
_loanRepository.Add(loan);
```

### Conditional Logic Based on Enum

```csharp
// Different processing based on service fee type
if (fee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping)
{
    // Bookkeeping can be fixed OR percentage-based
    if (fee.Percentage > 0)
    {
        // Auto-create variable bookkeeping companion entry
        var varFee = new FranchiseeServiceFee {
            ServiceFeeTypeId = (long)ServiceFeeType.VarBookkeeping,
            Percentage = fee.Percentage,
            Amount = 0
        };
        _repository.Add(varFee);
    }
}
else if (fee.ServiceFeeTypeId == (long)ServiceFeeType.Loan)
{
    // Generate amortization schedule
    GenerateLoanSchedule(fee);
}
```

---

## 📚 Detailed Enum Reference

### ServiceFeeType (ID Range: 171-178, 214, 251, 296)
**Categorizes charges billed to franchisees beyond base royalty fees.**

| Member | ID | Description | Special Behavior |
|--------|----|----|------------------|
| `Loan` | 171 | Loan principal repayment | Requires linked `FranchiseeLoan` record |
| `Bookkeeping` | 172 | Fixed bookkeeping fee | Auto-creates `VarBookkeeping` if percentage > 0 |
| `PayrollProcessing` | 173 | Payroll service charges | Invoiced monthly |
| `Recruiting` | 174 | Recruiting/staffing fees | One-time or recurring |
| `OneTimeProject` | 175 | Ad-hoc project fees | **Excluded** from recurring totals |
| `NationalCharge` | 176 | National account charges | Supports percentage-based billing |
| `InterestAmount` | 177 | Loan interest component | Calculated separately from principal |
| `VarBookkeeping` | 178 | Variable bookkeeping (%) | Auto-generated, do not create manually |
| `FRANCHISEETECHMAIL` | 214 | Technician email service | Added for tech communication fees |
| `PHONECALLCHARGES` | 251 | Call center charges | Per-call or subscription-based |
| `SEOCharges` | 296 | SEO marketing fees | Monthly SEO service costs |

**Example: Bookkeeping Fee Lifecycle**
```csharp
// User creates bookkeeping fee with percentage
var bookkeepingFee = new FranchiseeServiceFee {
    ServiceFeeTypeId = (long)ServiceFeeType.Bookkeeping,
    Amount = 100.00m,      // Fixed monthly fee
    Percentage = 2.5m      // + 2.5% of revenue
};

// System auto-creates companion entry
var varBookkeepingFee = new FranchiseeServiceFee {
    ServiceFeeTypeId = (long)ServiceFeeType.VarBookkeeping,
    Amount = 0,
    Percentage = 2.5m      // This entry handles the variable component
};
```

---

### ServiceType (ID Range: 1-32, 36-37)
**Defines the service catalog offered by franchisees.**

**Restoration Services** (1-5, 10-17):
- `StoneLife = 1` - Natural stone restoration
- `Enduracrete = 2` - Concrete floor restoration
- `Tilelok = 3` - Tile and grout services
- `Vinylguard = 4` - Vinyl floor protection
- `Counterlife = 5` - Countertop restoration
- `ColorSeal = 10` - Color sealing services
- `CleanShield = 11` - Protective coating application
- `Wood = 12` - Wood floor restoration
- `MetalLife = 15` - Metal surface restoration
- `CarpetLife = 16` - Carpet cleaning/restoration
- `TileInstall = 17` - New tile installation

**Maintenance Contracts** (6-8):
- `Monthly = 6` - Monthly maintenance plan
- `BiMonthly = 7` - Bi-monthly maintenance
- `Quarterly = 8` - Quarterly maintenance

**Product Sales Channels** (18-30):
- `Product = 18` - General product sales
- `SalesTax = 19` - Sales tax line item
- `WebMld = 20` - MLD website sales
- `WebFranchiseeSales = 21` - Franchisee website sales
- `WebJet = 22` - Jet.com marketplace
- `WebWalmart = 23` - Walmart marketplace
- `WebAmazon = 24` - Amazon marketplace
- `WebAmazonPrime = 25` - Amazon Prime exclusive
- `WebAmazonCanada = 26` - Amazon Canada
- `Hardware = 27` - Hardware store retail
- `Retail = 28` - Direct retail sales
- `MldWarehouse = 30` - MLD warehouse distribution

**Commercial Segments** (31-32):
- `Government = 31` - Government contracts
- `Hotel = 32` - Hospitality sector services

**System** (36-37):
- `Admin = 36` - Administrative/internal use
- `OTHER = 37` - Uncategorized services

**Usage Pattern**:
```csharp
// Find all restoration services
var restorationServices = _serviceRepository.Table
    .Where(s => s.CategoryId == (long)ServiceTypeCategory.Restoration)
    .OrderBy(s => s.OrderBy)
    .ToList();

// Filter product channel sales
var ecommerceSales = invoiceItems
    .Where(i => i.ServiceTypeId >= (long)ServiceType.WebMld && 
                i.ServiceTypeId <= (long)ServiceType.WebAmazonCanada);
```

---

### DocumentType (ID Range: 1-13)
**Categorizes legal and compliance documents.**

| Member | ID | Document Type | Expiry Tracking? |
|--------|----|----|------------------|
| `W9` | 1 | IRS Form W-9 (Tax ID) | No |
| `LoanAgreement` | 2 | Franchisee loan contract | No |
| `AnnualTaxFilling` | 3 | Annual tax filing documents | Yes |
| `FranchiseeContract` | 4 | Franchise agreement | Yes |
| `COI` | 5 | Certificate of Insurance | **Yes** (critical) |
| `EMPLOYEEHANDBOOK` | 6 | Employee handbook | No |
| `HAZOMMANUAL` | 7 | Hazmat/OSHA safety manual | Yes |
| `LICENSE` | 8 | Business license | **Yes** (perpetuity) |
| `UPLOADTAXES` | 9 | Tax document uploads | No |
| `FRANCHISEAGREEMENTSRENEWALS` | 10 | Renewal contracts | Yes |
| `RESALECERTIFICATE` | 11 | Sales tax resale cert | **Yes** (perpetuity) |
| `NCA` | 12 | National Contract Account | Role-based access |
| `NationalAccountAgreement` | 13 | National account agreement | Yes |

**Special Behaviors**:
```csharp
// Documents with perpetuity tracking (never expire, but must be current)
if (documentTypeId == (long)DocumentType.RESALECERTIFICATE || 
    documentTypeId == (long)DocumentType.LICENSE)
{
    document.IsPerpetuity = true; // No expiry date, but must be active
}

// Role-based access control for NCA documents
if (documentTypeId == (long)DocumentType.NCA)
{
    // Only owner who uploaded can view
    var visibleDocs = documents.Where(d => d.UserId == currentUserId);
}

// System-excluded types (not user-selectable in upload form)
var systemTypes = new[] {
    (long)DocumentType.LoanAgreement,
    (long)DocumentType.AnnualTaxFilling,
    (long)DocumentType.FranchiseeContract
};
var userSelectableTypes = allTypes.Where(t => !systemTypes.Contains(t.Id));
```

---

### PaymentFrequency (ID Range: 31-33, 297-298)
**Defines royalty billing cycles.**

| Member | ID | Schedule | Use Case |
|--------|----|----|---------|
| `Weekly` | 31 | Every 7 days | High-frequency billing for large franchisees |
| `Monthly` | 32 | 1st of month | Most common |
| `TwiceAMonth` | 33 | 1st & 15th | Semi-monthly billing |
| `FirstWeek` | 297 | 1st week only | Partial month billing (new franchisees) |
| `SecondWeek` | 298 | 2nd week only | Alternative week billing |

**Example: Invoice Generation**
```csharp
public void GenerateRoyaltyInvoices(DateTime billingPeriod)
{
    var franchisees = _franchiseeRepository.Table
        .Include(f => f.FeeProfile)
        .Where(f => f.IsActive);

    foreach (var franchisee in franchisees)
    {
        switch (franchisee.FeeProfile.PaymentFrequencyId)
        {
            case (long)PaymentFrequency.Weekly:
                // Generate invoice every Monday
                if (billingPeriod.DayOfWeek == DayOfWeek.Monday)
                    CreateInvoice(franchisee, billingPeriod.AddDays(-7), billingPeriod);
                break;

            case (long)PaymentFrequency.Monthly:
                // Generate invoice on 1st of month
                if (billingPeriod.Day == 1)
                    CreateInvoice(franchisee, billingPeriod.AddMonths(-1), billingPeriod);
                break;

            case (long)PaymentFrequency.TwiceAMonth:
                // Generate on 1st and 15th
                if (billingPeriod.Day == 1 || billingPeriod.Day == 15)
                    CreateInvoice(franchisee, billingPeriod.AddDays(-15), billingPeriod);
                break;
        }
    }
}
```

---

### LoanType (ID Range: 244-247)
**Categorizes loan programs by marketing/expansion purpose.**

| Member | ID | Program | Description |
|--------|----|----|-------------|
| `ISQFT` | 244 | iSquareFeet | Marketing platform lead generation loan |
| `SurgicalStrike` | 245 | Market Penetration | Aggressive market entry strategy loan |
| `Geofence` | 246 | Geographic Expansion | Loan for new territory expansion |
| `Other` | 247 | Miscellaneous | Uncategorized loan purposes |

**Business Context**:
- **iSquareFeet (ISQFT)**: Loans for franchisees to participate in the iSquareFeet lead generation platform
- **Surgical Strike**: Targeted high-investment marketing campaigns (e.g., Superbowl ad buys, large billboard campaigns)
- **Geofence**: Loans to fund expansion into new service territories (equipment, initial marketing, staffing)

**Example: Loan Performance Reporting**
```csharp
// Aggregate loan data by program type
var loanSummary = _loanRepository.Table
    .GroupBy(l => l.LoanTypeId)
    .Select(g => new LoanProgramSummary
    {
        LoanTypeId = g.Key,
        LoanTypeName = g.First().LoanType.Name,
        TotalLoans = g.Count(),
        TotalAmount = g.Sum(l => l.Amount),
        AverageAmount = g.Average(l => l.Amount),
        CompletedLoans = g.Count(l => l.IsCompleted == true)
    })
    .ToList();
```

---

### LanguageEnum (ID Range: 249-250)
**Multilingual support for franchisee communications.**

| Member | ID | Language | Notes |
|--------|----|----|-------|
| `English` | 249 | English | Default |
| `Spanish` | 250 | Spanish | Hispanic market support |

**Usage Pattern**:
```csharp
// Select email template by language preference
public string GetEmailTemplate(long franchiseeId, string templateKey)
{
    var franchisee = _franchiseeRepository.GetById(franchiseeId);
    var languageSuffix = franchisee.LanguageId == (long)LanguageEnum.Spanish ? "_es" : "_en";
    
    return _templateService.Get($"{templateKey}{languageSuffix}");
    // Example: "invoice_reminder_en" or "invoice_reminder_es"
}

// Generate localized invoices
if (franchisee.LanguageId == (long)LanguageEnum.Spanish)
{
    invoice.CultureInfo = new CultureInfo("es-MX"); // Mexican Spanish
    invoice.HeaderText = "Factura de Regalías"; // "Royalty Invoice"
}
```

---

### LeadPerformanceEnum (ID Range: 219-220)
**Tracks marketing spend categories for ROI analysis.**

| Member | ID | Category | Description |
|--------|----|----|-------------|
| `PPCSPEND` | 219 | Pay-Per-Click | Google Ads, Bing Ads spend |
| `SEOCOST` | 220 | SEO Services | Monthly SEO optimization fees |

**Example: Marketing ROI Calculation**
```csharp
public MarketingROI CalculateROI(long franchiseeId, DateTime startDate, DateTime endDate)
{
    // Get marketing spend
    var ppcSpend = _leadPerformanceRepo.Table
        .Where(l => l.FranchiseeId == franchiseeId)
        .Where(l => l.LeadTypeId == (long)LeadPerformanceEnum.PPCSPEND)
        .Where(l => l.Date >= startDate && l.Date < endDate)
        .Sum(l => l.Amount);

    var seoSpend = _leadPerformanceRepo.Table
        .Where(l => l.FranchiseeId == franchiseeId)
        .Where(l => l.LeadTypeId == (long)LeadPerformanceEnum.SEOCOST)
        .Where(l => l.Date >= startDate && l.Date < endDate)
        .Sum(l => l.Amount);

    // Calculate revenue from leads
    var leadRevenue = _invoiceRepo.Table
        .Where(i => i.FranchiseeId == franchiseeId)
        .Where(i => i.LeadSource == "PPC" || i.LeadSource == "Organic")
        .Where(i => i.Date >= startDate && i.Date < endDate)
        .Sum(i => i.Total);

    return new MarketingROI
    {
        TotalSpend = ppcSpend + seoSpend,
        TotalRevenue = leadRevenue,
        ROIPercentage = ((leadRevenue - (ppcSpend + seoSpend)) / (ppcSpend + seoSpend)) * 100
    };
}
```

---

## ⚠️ Common Pitfalls

### 1. **Forgetting to Cast Enum to `long`**
```csharp
// ❌ WRONG: Compiler error or incorrect comparison
if (fee.ServiceFeeTypeId == ServiceFeeType.Bookkeeping) { }

// ✅ CORRECT: Explicit cast required
if (fee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping) { }
```

### 2. **Using Enum ToString() for UI Display**
```csharp
// ❌ WRONG: Shows "COI" instead of "Certificate of Insurance"
documentName = ((DocumentType)document.TypeId).ToString();

// ✅ CORRECT: Fetch Lookup.Name from database
documentName = document.DocumentType.Name;
```

### 3. **Modifying Existing Enum Values**
```csharp
// ❌ DANGEROUS: Changing existing enum breaks DB references
public enum ServiceFeeType
{
    Loan = 999, // Was 171 - NOW ALL QUERIES BREAK!
}

// ✅ SAFE: Add new values, never change existing ones
public enum ServiceFeeType
{
    Loan = 171, // Keep original
    NewFeeType = 300 // Add with new ID
}
```

### 4. **Assuming Sequential IDs**
```csharp
// ❌ WRONG: Enum IDs have gaps (33-35 don't exist in ServiceType)
for (int i = 1; i <= 37; i++)
{
    var serviceType = (ServiceType)i; // Throws for undefined values
}

// ✅ CORRECT: Use Enum.GetValues()
foreach (ServiceType type in Enum.GetValues(typeof(ServiceType)))
{
    // Safely iterate only defined values
}
```

### 5. **OrganizationNames Anti-Pattern**
```csharp
// ❌ AVOID: Hardcoded franchisee IDs (brittle, untestable)
if (franchiseeId == (long)OrganizationNames.MIDetroit)
{
    // Special logic for specific franchisee
}

// ✅ BETTER: Use feature flags or configuration
if (_pilotFranchiseeConfig.IsPilotLocation(franchiseeId))
{
    // Feature flag-driven logic
}
```

---

## 🔧 Best Practices

### ✅ DO
- **Always cast enums to `long`** when comparing to database IDs
- **Use enum constants** instead of magic numbers
- **Query Lookup table** for UI-friendly display names
- **Add new enum values** with new IDs (never reuse)
- **Document business logic** tied to specific enum values

### ❌ DON'T
- **Don't** modify existing enum values (immutable after deployment)
- **Don't** use `Enum.ToString()` for user-facing text
- **Don't** assume sequential enum IDs
- **Don't** create enum members without corresponding Lookup rows
- **Don't** use `OrganizationNames` enum (legacy anti-pattern)

---

## 🔗 Related Documentation

- **[Core.Organizations.Domain](../Domain/README.md)**: Domain entities that reference these enums
- **[Core.Organizations.Impl](../Impl/README.md)**: Service layer using enums for business logic
- **[Core.Application.Domain](../../Application/Domain/README.md)**: Lookup table entity definition
- **[API Controllers](../../../API/)**: REST endpoints accepting enum IDs

---

## 📞 Support

For questions about:
- **Enum values**: Check the `Lookup` table in the database
- **Adding new enums**: Coordinate with database migration team
- **Enum usage patterns**: Refer to `Impl/` folder for examples
- **Breaking changes**: Consult with architecture team before modifying existing values
