<!-- AUTO-GENERATED: Header -->
# Sales Domain Entities
> Data model layer defining 27 entity classes for customers, sales uploads, invoices, credits, and marketing classifications
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Domain folder contains the complete entity relationship model for the Sales module. These are Entity Framework Code-First entities that define the database schema and relationships. Think of this as the "nouns" of the system - Customer, Invoice, Credit, MarketingClass, etc.

**What Makes This Special:**
Unlike typical CRUD entities, these domain models include:
- **Business Aggregates**: Sales metrics calculated and stored directly on `Customer` entity (TotalSales, AvgSales, NoOfSales) for performance
- **Upload Tracking**: Sophisticated batch upload entities (`SalesDataUpload`, `AnnualSalesDataUpload`) that track parsing status, error counts, and data quality
- **Audit Workflow**: Annual uploads include approval workflow with mismatch tracking for data quality review
- **Marketing Intelligence**: Classification system (18 types) embedded in customer records for segmentation

**Real-World Analogy:**
Imagine a franchisee running a marble restoration business uploads an Excel file with 100 invoices. The system:
1. Creates/updates `Customer` records from the file
2. Tracks the upload in `SalesDataUpload` (status: Uploaded → ParseInProgress → Parsed)
3. Updates `Customer.TotalSales`, `Customer.NoOfSales`, `Customer.AvgSales` as it processes
4. Links invoices to `FranchiseeSalesPayment` for royalty calculations
5. If errors occur, logs them and updates `NumberOfFailedRecords`

At year-end, the `AnnualSalesDataUpload` entity adds an extra layer: it flags address mismatches (`NoOfMismatchedRecords`) and requires staff approval (`AuditActionId`) before the data is accepted into the system.

**Entity Categories:**
- **Core**: Customer, MarketingClass
- **Upload Tracking**: SalesDataUpload, AnnualSalesDataUpload, CustomerDataUpload, InvoiceFileUpload
- **Financial**: AccountCredit, AccountCreditItem, RoyaltyInvoiceItem, NationalInvoiceItem
- **Estimates**: EstimateInvoice, EstimateInvoiceService, EstimateInvoiceCustomer
- **Support**: CustomerEmail, FranchiseeSalesPayment, HoningMeasurement
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup

These entities are used with Entity Framework 6+ and SQL Server. The module must reference:
- Core.Application (for `DomainBase`, `Lookup`, `DataRecorderMetaData`)
- Core.Billing (for `Invoice`, `InvoiceItem`, `Payment`, `CurrencyExchangeRate`)
- Core.Organizations (for `Franchisee`)
- Core.Geo (for `Address`)

```bash
# Migrations (in Package Manager Console)
Add-Migration AddSalesEntities -Project Core -StartupProject Web
Update-Database -Project Core -StartupProject Web
```

### Example: Creating a Customer with Emails

```csharp
using Core.Sales.Domain;
using Core.Geo.Domain;

var customer = new Customer
{
    Name = "Luxury Hotel & Spa",
    ContactPerson = "Maria Rodriguez",
    Phone = "555-0199",
    ClassTypeId = (long)MarketingClassType.Hotel,
    ReceiveNotification = true,
    DateCreated = DateTime.Now,
    Address = new Address
    {
        AddressLine1 = "789 Resort Boulevard",
        City = "Miami",
        StateId = 10, // Florida
        Zip = "33139"
    },
    CustomerEmails = new List<CustomerEmail>
    {
        new CustomerEmail 
        { 
            Email = "maria@luxuryhotel.com", 
            DateCreated = DateTime.Now 
        },
        new CustomerEmail 
        { 
            Email = "maintenance@luxuryhotel.com", 
            DateCreated = DateTime.Now 
        }
    },
    DataRecorderMetaDataId = currentUserMetaDataId
};

// Entity Framework will cascade save Address and CustomerEmails
dbContext.Customers.Add(customer);
dbContext.SaveChanges();

// After first sales parsing, metrics will populate:
// customer.TotalSales = 12500.00m
// customer.NoOfSales = 8
// customer.AvgSales = 1562.50m
```

### Example: Tracking Sales Upload Progress

```csharp
using Core.Sales.Domain;
using Core.Sales.Enum;

// Create upload record
var upload = new SalesDataUpload
{
    FranchiseeId = franchiseeId,
    FileId = uploadedFileId,
    PeriodStartDate = new DateTime(2025, 1, 1),
    PeriodEndDate = new DateTime(2025, 1, 31),
    StatusId = (long)SalesDataUploadStatus.Uploaded,
    CurrencyExchangeRateId = currentRateId,
    IsActive = true,
    IsInvoiceGenerated = false,
    DataRecorderMetaDataId = metaDataId
};
dbContext.SalesDataUploads.Add(upload);
dbContext.SaveChanges();

// Polling agent picks it up, updates during parsing:
upload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
upload.NumberOfCustomers = 45;      // Running count
upload.NumberOfInvoices = 127;      // Running count
upload.NumberOfParsedRecords = 100; // Successful so far
upload.NumberOfFailedRecords = 2;   // Errors encountered
dbContext.SaveChanges();

// Final state after completion:
upload.StatusId = (long)SalesDataUploadStatus.Parsed;
upload.TotalAmount = 45230.50m;     // Sum of all invoice amounts
upload.PaidAmount = 38100.00m;      // Sum of payments
upload.AccruedAmount = 7130.50m;    // Unpaid balance
dbContext.SaveChanges();
```

### Example: Creating an Account Credit

```csharp
using Core.Sales.Domain;

var credit = new AccountCredit
{
    CustomerId = customerId,
    QbInvoiceNumber = "INV-2025-045",
    CreditedOn = DateTime.Now,
    CreditMemoItems = new List<AccountCreditItem>
    {
        new AccountCreditItem
        {
            Description = "Refund - incomplete polish service",
            Amount = 450.00m,
            CurrencyExchangeRateId = currentRateId
        },
        new AccountCreditItem
        {
            Description = "Goodwill credit for service delay",
            Amount = 100.00m,
            CurrencyExchangeRateId = currentRateId
        }
    }
};

dbContext.AccountCredits.Add(credit);
dbContext.SaveChanges();
// Total credit: $550 applied to customer account
```

### Example: Building an Estimate Invoice

```csharp
using Core.Sales.Domain;

var estimate = new EstimateInvoice
{
    CustomerId = customerId,
    FranchiseeId = franchiseeId,
    ClassTypeId = (long)MarketingClassType.Residential,
    EstimateId = jobEstimateId,
    PriceOfService = 1250.00f,
    LessDeposit = 250.00f,
    Notes = "Master bathroom marble restoration",
    Option1 = "Standard polish",
    Option2 = "Premium seal + polish",
    Option3 "Full restoration with repair",
    IsCustomerAvailable = true,
    IsInvoiceParsing = false,
    DataRecorderMetaDataId = metaDataId
};

dbContext.EstimateInvoices.Add(estimate);
dbContext.SaveChanges();

// Add service line items
var service = new EstimateInvoiceService
{
    EstimateInvoiceId = estimate.Id,
    ServiceName = "Marble Floor Polish",
    ServiceType = "Polish",
    Location = "Master Bathroom",
    TypeOfService = "Surface Restoration",
    StoneType = "Carrara Marble",
    StoneColor = "White",
    Price = "850.00",
    Description = "Hone and polish 120 sq ft",
    IsActive = true,
    InvoiceNumber = 1,
    DataRecorderMetaDataId = metaDataId
};

dbContext.EstimateInvoiceServices.Add(service);
dbContext.SaveChanges();
```

### Example: Annual Upload with Audit Workflow

```csharp
using Core.Sales.Domain;

var annualUpload = new AnnualSalesDataUpload
{
    FranchiseeId = franchiseeId,
    FileId = annualReportFileId,
    PeriodStartDate = new DateTime(2024, 1, 1),
    PeriodEndDate = new DateTime(2024, 12, 31),
    StatusId = (long)SalesDataUploadStatus.Uploaded,
    CurrencyExchangeRateId = currentRateId,
    IsAuditAddressParsing = true, // Enable address validation
    AuditActionId = (long)AuditActionType.Pending, // Awaiting review
    DataRecorderMetaDataId = metaDataId
};

dbContext.AnnualSalesDataUploads.Add(annualUpload);
dbContext.SaveChanges();

// After parsing by IAnnualSalesDataParsePollingAgent:
annualUpload.StatusId = (long)SalesDataUploadStatus.Parsed;
annualUpload.NoOfParsedRecords = 1250;
annualUpload.NoOfFailedRecords = 5;
annualUpload.NoOfMismatchedRecords = 12;  // Address mismatches requiring review
annualUpload.WeeklyRoyality = 2500.00m;
annualUpload.AnnualRoyality = 125000.00m;
annualUpload.TotalAmount = 650000.00m;
dbContext.SaveChanges();

// Staff reviews, then approves:
annualUpload.AuditActionId = (long)AuditActionType.Approved;
dbContext.SaveChanges();
// Data now flows into royalty calculations
```

### Example: Querying Customer Sales Metrics

```csharp
// Find high-value hotel customers
var topHotels = dbContext.Customers
    .Include(c => c.Address)
    .Include(c => c.MarketingClass)
    .Where(c => c.ClassTypeId == (long)MarketingClassType.Hotel)
    .Where(c => c.TotalSales > 50000)
    .OrderByDescending(c => c.TotalSales)
    .Take(10)
    .ToList();

foreach (var hotel in topHotels)
{
    Console.WriteLine($"{hotel.Name} - Total: ${hotel.TotalSales:N2}, " +
                      $"Avg: ${hotel.AvgSales:N2}, Jobs: {hotel.NoOfSales}");
}
// Output:
// Grand Plaza Hotel - Total: $125,430.00, Avg: $2,508.60, Jobs: 50
// Seaside Resort - Total: $98,250.00, Avg: $2,206.25, Jobs: 44
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Entity Summary

| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| **Customer** | Master customer record | → MarketingClass, Address, CustomerEmail (collection) |
| **CustomerEmail** | Customer email addresses | → Customer |
| **SalesDataUpload** | Sales batch upload tracking | → Franchisee, File, CurrencyExchangeRate |
| **AnnualSalesDataUpload** | Year-end upload with audit | → SalesDataUpload, Franchisee, File |
| **AccountCredit** | Credit memo header | → Customer, AccountCreditItem (collection) |
| **AccountCreditItem** | Credit line item | → AccountCredit, CurrencyExchangeRate |
| **MarketingClass** | Customer segmentation (18 types) | ← Customer |
| **MasterMarketingClass** | Top-level marketing category | |
| **SubClassMarketingClass** | Marketing sub-category | |
| **EstimateInvoice** | Pre-sale quote | → Customer, EstimateInvoiceCustomer, MarketingClass, Franchisee, JobEstimate |
| **EstimateInvoiceService** | Estimate line item | → EstimateInvoice, EstimateInvoiceService (self-ref for bundling) |
| **EstimateInvoiceCustomer** | Customer snapshot on estimate | |
| **EstimateInvoiceServiceDescription** | Service description template | |
| **RoyaltyInvoiceItem** | Royalty charge line item | → InvoiceItem (1:1) |
| **NationalInvoiceItem** | National account line item | → InvoiceItem (1:1) |
| **AdFundInvoiceItem** | Ad fund contribution line | → InvoiceItem (1:1) |
| **FranchiseeFeeEmailInvoiceItem** | Email service fee line | → InvoiceItem (1:1) |
| **FranchiseeSalesPayment** | Links sales to invoices/payments | → FranchiseeSales, Invoice, Payment, SalesDataUpload |
| **CustomerDataUpload** | Customer-only file upload | |
| **InvoiceFileUpload** | Invoice file upload tracking | |
| **UpdateMarketingClassfileupload** | Marketing class bulk update | |
| **AnnualRoyality** | Annual royalty calculation | |
| **AnnualReportType** | Report type categorization | |
| **HoningMeasurement** | Honing measurement data | |
| **HoningMeasurementDefault** | Default honing values | |
| **MlfsConfigurationSetting** | System configuration KV pairs | |
| **SystemAuditRecord** | System audit trail | |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem: Lazy loading errors (NullReferenceException on navigation properties)**
- **Cause**: Entity Framework lazy loading disabled or navigation property not loaded
- **Solution**: Use `.Include()` to eager load: `dbContext.Customers.Include(c => c.Address).Include(c => c.MarketingClass)`

**Problem: Cascade delete errors when deleting customers**
- **Cause**: Foreign key constraints with invoices, credits, or uploads
- **Solution**: Use soft delete (`customer.IsDeleted = true`) instead of hard delete

**Problem: Customer sales metrics (TotalSales, AvgSales) not updating**
- **Cause**: Metrics calculated during upload parsing, not real-time
- **Solution**: Ensure `SalesDataUpload.StatusId == Parsed` before trusting metrics

**Problem: Annual upload stuck in "Pending" audit status**
- **Cause**: `NoOfMismatchedRecords > 0` requires manual review
- **Solution**: Query `GetAnnualAuditRecord()`, resolve mismatches, set `AuditActionId = Approved`

**Problem: EstimateInvoiceService bundling not working**
- **Cause**: `ParentId` not set correctly for child services
- **Solution**: Create parent service first, then set `service.ParentId = parentService.Id` for children

### Performance Tips

- **Avoid N+1 queries**: Always use `.Include()` for navigation properties in loops
- **Batch saves**: Add multiple entities, then call `SaveChanges()` once
- **Use projections**: Select only needed fields: `.Select(c => new { c.Name, c.TotalSales })`
- **Index foreign keys**: Ensure indexes on `CustomerId`, `FranchiseeId`, `StatusId` columns
- **Denormalized metrics**: `Customer.TotalSales` avoids expensive aggregations - use it!

### Schema Migration Notes

When adding fields:
1. Update entity class
2. `Add-Migration <name>`
3. Review generated migration code
4. `Update-Database`

For existing data, provide defaults or nullable types to avoid migration errors.
<!-- END CUSTOM SECTION -->
