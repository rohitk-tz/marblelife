<!-- AUTO-GENERATED: Header -->
# Sales Domain
> CRM Domain Model - Customer Management, Sales Data Import, and Royalty Calculation Entities
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Sales Domain** module is the heart of the Marblelife CRM system. It defines the entity model for:

- **Customer Master Data** - Complete customer profiles with addresses, emails, and marketing classifications
- **Sales Data Pipeline** - Excel/CSV import tracking with parsing workflow states
- **Royalty Billing** - Specialized invoice line items for royalty fees, advertising funds, and national account charges
- **Annual Reconciliation** - Year-end validation against tax filings with audit workflow

**Key Insight**: This is a *pure domain layer* - it contains **only** entity definitions with EF6 navigation properties. All business logic lives in the `Impl` folder.

Think of this module as the **database schema expressed as C# classes**. Every entity extends `DomainBase` (provides `Id`, `IsNew`, `IsDeleted` flags) and most include a `DataRecorderMetaDataId` for audit trails (who created/modified, when).
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Quick Start

### Customer Management
```csharp
// Creating a new customer with emails
var customer = new Customer
{
    Name = "Acme Corporation",
    ContactPerson = "John Doe",
    Phone = "555-1234",
    ClassTypeId = (long)MarketingClassType.Commercial, // Lookup reference
    Address = new Address
    {
        Street = "123 Main St",
        CityId = cityId,
        StateId = stateId,
        Zip = "12345"
    },
    CustomerEmails = new List<CustomerEmail>
    {
        new CustomerEmail { Email = "john@acme.com", DateCreated = DateTime.Now }
    },
    DataRecorderMetaData = new DataRecorderMetaData
    {
        DateCreated = DateTime.Now,
        CreatedById = currentUserId
    }
};
```

### Sales Data Upload Tracking
```csharp
// Creating a sales data upload record
var upload = new SalesDataUpload
{
    FranchiseeId = franchiseeId,
    FileId = uploadedFile.Id,
    PeriodStartDate = new DateTime(2024, 1, 1),
    PeriodEndDate = new DateTime(2024, 1, 7),
    StatusId = (long)SalesDataUploadStatus.Uploaded, // 71
    CurrencyExchangeRateId = currentRate.Id,
    IsActive = true,
    IsInvoiceGenerated = false,
    DataRecorderMetaData = new DataRecorderMetaData { DateCreated = DateTime.Now }
};

// Background job will update to ParseInProgress (74) → Parsed (72) or Failed (73)
```

### Royalty Invoice Line Items (Shared PK Pattern)
```csharp
// IMPORTANT: These entities share the same PK as parent InvoiceItem
// Do NOT auto-generate IDs

var invoiceItem = new InvoiceItem
{
    InvoiceId = invoiceId,
    Description = "Weekly Royalty (Jan 1-7, 2024)",
    Amount = 1500.00m,
    // ... other InvoiceItem fields
};

// Save parent first to get ID
invoiceItemRepository.Save(invoiceItem);

// Now create royalty detail with SAME ID
var royaltyDetail = new RoyaltyInvoiceItem
{
    Id = invoiceItem.Id, // ← Explicitly set, don't let EF generate
    Percentage = 8.0m,
    StartDate = new DateTime(2024, 1, 1),
    EndDate = new DateTime(2024, 1, 7),
    Amount = 1500.00m
};

royaltyInvoiceItemRepository.Save(royaltyDetail);
```

### Marketing Classification
```csharp
// Query customers by marketing class
var commercialCustomers = customerRepo.Table
    .Where(c => c.ClassTypeId == (long)MarketingClassType.Commercial)
    .Include(c => c.Address)
    .Include(c => c.CustomerEmails)
    .ToList();
```

### Annual Reconciliation
```csharp
// HQ uploads franchisee's annual tax filing
var annualUpload = new AnnualSalesDataUpload
{
    FranchiseeId = franchiseeId,
    FileId = taxFilingFile.Id,
    PeriodStartDate = new DateTime(2024, 1, 1),
    PeriodEndDate = new DateTime(2024, 12, 31),
    StatusId = (long)SalesDataUploadStatus.Uploaded,
    AuditActionId = (long)AuditActionType.Pending, // 153
    WeeklyRoyality = sumOfWeeklySalesReported,
    AnnualRoyality = royaltyFromTaxFiling,
    CurrencyExchangeRateId = rateId
};

// If mismatch detected during parsing, SystemAuditRecord is created
if (annualUpload.WeeklyRoyality != annualUpload.AnnualRoyality)
{
    var auditRecord = new SystemAuditRecord
    {
        FranchiseeId = franchiseeId,
        AnnualUploadId = annualUpload.Id,
        InvoiceId = discrepancyInvoice.Id,
        QBIdentifier = "INV-2024-001",
        AnnualReportTypeId = (long)AuditReportType.Type1 // Enum defines audit types
    };
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## 📚 Entity Catalog

### Core CRM Entities
| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `Customer` | Customer master record | `Name`, `ContactPerson`, `ClassTypeId`, `AddressId` |
| `CustomerEmail` | Customer email addresses (1:N) | `CustomerId`, `Email`, `DateCreated` |
| `MarketingClass` | Customer segmentation types | `Name`, `ColorCode`, `Alias` |
| `MasterMarketingClass` | Top-level class grouping | `Name`, `Description` |
| `SubClassMarketingClass` | Sub-category of master class | `Name`, `MarketingclassId` |

### Sales Data Import Entities
| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `SalesDataUpload` | Weekly/monthly sales file upload | `FranchiseeId`, `StatusId`, `PeriodStartDate`, `FileId` |
| `CustomerFileUpload` | Customer bulk import tracking | `FileId`, `StatusId` |
| `InvoiceFileUpload` | Invoice bulk import tracking | `FileId`, `StatusId` |
| `EstimateInvoice` | Parsed invoice from upload | `CustomerId`, `PriceOfService`, `FranchiseeId` |
| `EstimateInvoiceCustomer` | Inline customer (non-master) | `CustomerName`, `Email`, `StreetAddress` |

### Royalty/Financial Entities
| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `RoyaltyInvoiceItem` | Royalty fee line item (shared PK) | `Id`, `Percentage`, `Amount`, `StartDate` |
| `AdFundInvoiceItem` | Advertising fund fee (shared PK) | `Id`, `Percentage`, `Amount` |
| `NationalInvoiceItem` | National account fee (shared PK) | `Id`, `Percentage`, `Amount` |
| `FranchiseeFeeEmailInvoiceItem` | Email/tech fee (shared PK) | `Id`, `Percentage`, `Amount` |
| `AccountCredit` | Customer refund/credit memo | `CustomerId`, `QbInvoiceNumber`, `CreditedOn` |
| `AccountCreditItem` | Credit memo line items | `AccountCreditId`, `Description`, `Amount` |

### Annual Reconciliation Entities
| Entity | Purpose | Key Fields |
|--------|---------|------------|
| `AnnualSalesDataUpload` | Year-end tax filing upload | `FranchiseeId`, `AnnualRoyality`, `WeeklyRoyality`, `AuditActionId` |
| `SystemAuditRecord` | Discrepancy tracking | `InvoiceId`, `AnnualUploadId`, `QBIdentifier` |
| `AnnualRoyality` | Historical royalty snapshot | `FranchiseeId`, `Date`, `Royality`, `Sales` |
| `AnnualReportType` | Audit report type lookup | `Name` |

### Helper/Legacy Entities
| Entity | Purpose |
|--------|---------|
| `FranchiseeSalesPayment` | Links sales to payments (legacy) |
| `HoningMeasurement` | Floor honing service measurements |
| `HoningMeasurementDefault` | Default honing rates |
| `MlfsConfigurationSetting` | Module configuration |
| `UpdateMarketingClassfileupload` | Bulk class update tracking |
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Relationships -->
## 🔗 Key Relationships

### Customer Aggregate
```
Customer (1) ────┐
                 ├─→ CustomerEmail (N)
                 ├─→ Address (0..1)
                 ├─→ MarketingClass (1)
                 └─→ DataRecorderMetaData (1)
```

### Sales Data Upload Flow
```
SalesDataUpload (1) ──→ Franchisee (1)
                    ├─→ File (1)
                    ├─→ Lookup [Status] (1)
                    └─→ CurrencyExchangeRate (1)

EstimateInvoice (N) ──→ SalesDataUpload (1) [implicit via parsing]
                    ├─→ Customer (0..1)
                    ├─→ EstimateInvoiceCustomer (0..1)
                    ├─→ JobEstimate (0..1)
                    ├─→ JobScheduler (0..1)
                    └─→ MarketingClass (1)
```

### Royalty Invoice Extensions (Shared PK)
```
InvoiceItem (1) ═══════> RoyaltyInvoiceItem (0..1)  [1:1, shared PK]
            (1) ═══════> AdFundInvoiceItem (0..1)    [1:1, shared PK]
            (1) ═══════> NationalInvoiceItem (0..1)  [1:1, shared PK]
```

### Annual Reconciliation
```
AnnualSalesDataUpload (1) ──→ Franchisee (1)
                          ├─→ SalesDataUpload (0..1)
                          ├─→ File (1)
                          └─→ Lookup [AuditAction] (1)

SystemAuditRecord (N) ──→ AnnualSalesDataUpload (1)
                      ├─→ Invoice (1)
                      ├─→ Franchisee (1)
                      └─→ AnnualReportType (0..1)
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Common Patterns -->
## 📋 Common Patterns

### 1. Status Enum to Lookup Mapping
```csharp
// Enum values map to Lookup.Id primary keys
public enum SalesDataUploadStatus
{
    Uploaded = 71,
    ParseInProgress = 74,
    Parsed = 72,
    Failed = 73
}

// Query usage - must cast to long
var parsed = uploadRepo.Table
    .Where(u => u.StatusId == (long)SalesDataUploadStatus.Parsed)
    .ToList();
```

### 2. Cascade Entity Pattern
```csharp
// [CascadeEntity] signals automatic child collection save
public class Customer : DomainBase
{
    [CascadeEntity(IsCollection = true)]
    public virtual ICollection<CustomerEmail> CustomerEmails { get; set; }
    
    [CascadeEntity]
    public virtual Address Address { get; set; }
}

// Repository layer detects attribute and persists children
customerRepo.Save(customer); // Also saves emails and address
```

### 3. Shared Primary Key (One-to-One Extension)
```csharp
// Parent entity
var invoiceItem = new InvoiceItem { Amount = 1500 };
invoiceItemRepo.Save(invoiceItem); // ID generated (e.g., 42)

// Child entity with SAME ID
var royalty = new RoyaltyInvoiceItem
{
    Id = invoiceItem.Id, // ← Must manually assign
    Percentage = 8.0m
};
royaltyRepo.Save(royalty);

// Querying
var itemWithRoyalty = invoiceItemRepo.Table
    .Include(x => x.RoyaltyInvoiceItem)
    .FirstOrDefault(x => x.Id == 42);
```

### 4. Multi-Tenancy via Franchisee Scoping
```csharp
// No direct FranchiseeId on Customer, but scoped via EstimateInvoice
var franchiseeCustomers = estimateInvoiceRepo.Table
    .Where(ei => ei.FranchiseeId == franchiseeId)
    .Select(ei => ei.Customer)
    .Distinct()
    .ToList();
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Troubleshooting -->
## 🔧 Troubleshooting

### Issue: "The entity type requires a key to be defined" (EF Error)
**Cause**: Shared PK pattern not configured in EF mappings.

**Solution**: Check `ORM/Mappings` for proper one-to-one configuration:
```csharp
// RoyaltyInvoiceItemMap.cs
HasKey(x => x.Id);
HasRequired(x => x.InvoiceItem)
    .WithOptional()
    .Map(m => m.MapKey("Id"));
```

### Issue: Status enum query returns no results
**Cause**: Enum cast to `int` instead of `long`.

**Solution**: Always cast to `long`:
```csharp
// ❌ Wrong
.Where(x => x.StatusId == (int)SalesDataUploadStatus.Parsed)

// ✅ Correct
.Where(x => x.StatusId == (long)SalesDataUploadStatus.Parsed)
```

### Issue: CustomerEmails not loading (N+1 query)
**Cause**: Lazy loading without explicit `.Include()`.

**Solution**: Use eager loading:
```csharp
var customers = customerRepo.Table
    .Include(c => c.CustomerEmails)
    .Include(c => c.Address)
    .ToList();
```

### Issue: Cannot save RoyaltyInvoiceItem (PK collision)
**Cause**: Trying to let EF auto-generate ID for shared PK entity.

**Solution**: Always manually assign ID from parent:
```csharp
royaltyItem.Id = invoiceItem.Id; // Don't rely on EF identity
```

### Issue: Currency conversion mismatch
**Cause**: Using wrong `CurrencyExchangeRateId` for date range.

**Solution**: Query exchange rate by date:
```csharp
var rate = currencyRepo.Table
    .Where(r => r.CountryId == franchisee.CountryId)
    .Where(r => r.DateTime <= salesDataUpload.PeriodEndDate)
    .OrderByDescending(r => r.DateTime)
    .First();
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Related Links -->
## 🔗 Related Documentation
- [Sales/Impl README](../Impl/README.md) - Business logic and services
- [Sales/Enum README](../Enum/README.md) - Status codes and constants
- [Billing Domain](../../Billing/Domain/README.md) - Invoice and payment entities
- [Organizations Domain](../../Organizations/Domain/README.md) - Franchisee management
- [Application Domain](../../Application/Domain/README.md) - Shared base classes
<!-- END CUSTOM SECTION -->
