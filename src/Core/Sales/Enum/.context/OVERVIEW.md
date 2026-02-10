<!-- AUTO-GENERATED: Header -->
# Sales Enumerations
> Strongly-typed constants for customer classifications, upload statuses, audit workflows, and document types
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Enum folder contains 7 enumeration types that replace magic numbers with meaningful names throughout the Sales module. These aren't just simple lists - they're integral to business workflows, status tracking, and data classification.

**Why Enums Matter Here:**
In a franchise system dealing with uploaded files, annual audits, and diverse customer types, having clear, typed constants prevents errors like:
- Setting an upload status to an invalid number
- Forgetting what marketing class "17" means
- Mixing up audit approval codes

**Real-World Example:**
When a franchisee uploads their monthly sales file:
```
1. File uploaded → StatusId = SalesDataUploadStatus.Uploaded (71)
2. Parsing starts → StatusId = SalesDataUploadStatus.ParseInProgress (74)
3. Complete → StatusId = SalesDataUploadStatus.Parsed (72)
```

Without enums, developers would write `StatusId = 72` with no context. With enums, they write `StatusId = (long)SalesDataUploadStatus.Parsed` - self-documenting and type-safe.

**The Marketing Class Story:**
MarbleLife franchisees serve 18 distinct market segments. A "Hotel" customer (class 3) has different sales patterns than a "Residential" homeowner (class 4) or a "National" corporate account (class 17). These classifications drive:
- Revenue reporting: "How much did we make from hotels vs. homes?"
- Marketing: "Send this campaign only to commercial accounts"
- Pricing: "National accounts get volume discounts"

The `MarketingClassType` enum ensures everyone uses the same categories system-wide.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example: Tracking Upload Status

```csharp
using Core.Sales.Domain;
using Core.Sales.Enum;

// Create new upload
var upload = new SalesDataUpload
{
    FranchiseeId = franchiseeId,
    FileId = fileId,
    PeriodStartDate = startDate,
    PeriodEndDate = endDate,
    StatusId = (long)SalesDataUploadStatus.Uploaded, // Queued for parsing
    // ...
};
dbContext.SalesDataUploads.Add(upload);
dbContext.SaveChanges();

// Later, in the polling agent
if (upload.StatusId == (long)SalesDataUploadStatus.Uploaded)
{
    upload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
    dbContext.SaveChanges();
    
    try
    {
        // Parse file logic...
        upload.StatusId = (long)SalesDataUploadStatus.Parsed;
    }
    catch (Exception ex)
    {
        upload.StatusId = (long)SalesDataUploadStatus.Failed;
        LogError(ex);
    }
    dbContext.SaveChanges();
}
```

### Example: Filtering by Marketing Class

```csharp
using Core.Sales.Domain;
using Core.Sales.Enum;

// Get all hotel customers
var hotelCustomers = dbContext.Customers
    .Where(c => c.ClassTypeId == (long)MarketingClassType.Hotel)
    .ToList();

// High-value segments: Hotels, Commercial, National accounts
var highValueSegments = new[]
{
    (long)MarketingClassType.Hotel,
    (long)MarketingClassType.Commercial,
    (long)MarketingClassType.National
};

var vipCustomers = dbContext.Customers
    .Where(c => highValueSegments.Contains(c.ClassTypeId))
    .OrderByDescending(c => c.TotalSales)
    .Take(50)
    .ToList();

// Revenue by marketing class
var revenueByClass = dbContext.Customers
    .GroupBy(c => c.ClassTypeId)
    .Select(g => new
    {
        MarketingClass = (MarketingClassType)g.Key,
        TotalRevenue = g.Sum(c => c.TotalSales),
        CustomerCount = g.Count()
    })
    .OrderByDescending(x => x.TotalRevenue)
    .ToList();

// Output:
// Hotel: $2,450,000 (120 customers)
// Commercial: $1,980,000 (340 customers)
// Residential: $1,250,000 (890 customers)
```

### Example: Annual Audit Workflow

```csharp
using Core.Sales.Domain;
using Core.Sales.Enum;

// Upload parsed, waiting for review
var annualUpload = new AnnualSalesDataUpload
{
    FranchiseeId = franchiseeId,
    FileId = annualFileId,
    PeriodStartDate = new DateTime(2024, 1, 1),
    PeriodEndDate = new DateTime(2024, 12, 31),
    StatusId = (long)SalesDataUploadStatus.Uploaded,
    AuditActionId = (long)AuditActionType.Pending, // Awaiting review
    IsAuditAddressParsing = true
};

// After parsing completes
annualUpload.StatusId = (long)SalesDataUploadStatus.Parsed;
annualUpload.NoOfMismatchedRecords = 12; // Address issues found
dbContext.SaveChanges();

// Staff reviews audit queue
var pendingAudits = dbContext.AnnualSalesDataUploads
    .Where(a => a.AuditActionId == (long)AuditActionType.Pending)
    .ToList();

// Staff approves after fixing mismatches
foreach (var audit in pendingAudits)
{
    if (audit.NoOfMismatchedRecords == 0)
    {
        audit.AuditActionId = (long)AuditActionType.Approved;
        Console.WriteLine($"Approved: {audit.Franchisee.Name} - ${audit.AnnualRoyality:N2} royalty");
    }
    else
    {
        Console.WriteLine($"Pending: {audit.Franchisee.Name} - {audit.NoOfMismatchedRecords} issues");
    }
}
dbContext.SaveChanges();
```

### Example: Job Status Tracking

```csharp
using Core.Sales.Domain;
using Core.Sales.Enum;

// Create estimate
var estimate = new EstimateInvoice
{
    CustomerId = customerId,
    FranchiseeId = franchiseeId,
    PriceOfService = 1250.00f,
    ClassTypeId = (long)MarketingClassType.Residential,
    JobStatus = (long)JobStatusEnum.Created
};
dbContext.EstimateInvoices.Add(estimate);
dbContext.SaveChanges();

// Customer confirms - mark tentative
estimate.JobStatus = (long)JobStatusEnum.Tentative;

// Assign to technician
estimate.JobStatus = (long)JobStatusEnum.Assigned;
estimate.AssignedTechnicianId = technicianId;

// Work begins
estimate.JobStatus = (long)JobStatusEnum.InProgress;
estimate.StartedAt = DateTime.Now;

// Work complete
estimate.JobStatus = (long)JobStatusEnum.Completed;
estimate.CompletedAt = DateTime.Now;
dbContext.SaveChanges();

// Report: Jobs in progress
var activeJobs = dbContext.EstimateInvoices
    .Where(e => e.JobStatus == (long)JobStatusEnum.InProgress)
    .Include(e => e.Customer)
    .Include(e => e.Franchisee)
    .ToList();
```

### Example: Document Compliance Check

```csharp
using Core.Sales.Enum;

// Required documents for franchisee compliance
var requiredDocs = new[]
{
    DocumentEnum.COI,                // Certificate of Insurance
    DocumentEnum.AnnualTaxFiling,    // Tax returns
    DocumentEnum.ResaleCertificate   // Sales tax exemption
};

foreach (var docType in requiredDocs)
{
    var doc = dbContext.FranchiseeDocuments
        .Where(d => d.FranchiseeId == franchiseeId)
        .Where(d => d.DocumentTypeId == (long)docType)
        .Where(d => d.ExpirationDate > DateTime.Now)
        .FirstOrDefault();
    
    if (doc == null)
    {
        Console.WriteLine($"Missing: {docType}");
        SendComplianceReminder(franchiseeId, docType);
    }
}
```

### Example: Enum Display Names (Extension Method)

```csharp
using Core.Sales.Enum;

public static class MarketingClassExtensions
{
    public static string GetDisplayName(this MarketingClassType type)
    {
        return type switch
        {
            MarketingClassType.Commercial => "Commercial",
            MarketingClassType.Education => "Education",
            MarketingClassType.Hotel => "Hotel & Hospitality",
            MarketingClassType.Residential => "Residential",
            MarketingClassType.MedicalLegal => "Medical/Legal",
            MarketingClassType.Goverment => "Government", // Note: enum has typo
            MarketingClassType.National => "National Account",
            MarketingClassType.Unclassified => "Unclassified",
            _ => type.ToString()
        };
    }
}

// Usage in UI
var className = ((MarketingClassType)customer.ClassTypeId).GetDisplayName();
// Output: "Hotel & Hospitality" instead of "Hotel"
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Enumeration Summary

| Enum | Values | Purpose |
|------|--------|---------|
| **SalesDataUploadStatus** | Uploaded(71), ParseInProgress(74), Parsed(72), Failed(73) | Track file upload processing lifecycle |
| **MarketingClassType** | 18 types (Commercial, Hotel, Residential, etc.) | Customer segmentation for marketing and reporting |
| **AuditActionType** | Approved(151), Rejected(152), Pending(153) | Annual data audit approval workflow |
| **AuditReportType** | 40+ types (Type1A, Type5, etc.) | Categorize audit findings and data quality issues |
| **DocumentEnum** | 8 types (COI, TaxFiling, P&L, etc.) | Required franchisee compliance documents |
| **JobStatusEnum** | 6 states (Created, Assigned, InProgress, Completed, Canceled, Tentative) | Estimate/job lifecycle tracking |
| **AnnualGroupType** | 4 types (UnderReporting, OverReporting, ReviewNoAction, Type1H) | Annual audit finding severity categories |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Common Issues

**Problem: InvalidCastException when converting from database value**
- **Cause**: Database contains invalid enum value (e.g., StatusId = 99 but no matching enum)
- **Solution**: 
  ```csharp
  if (Enum.IsDefined(typeof(SalesDataUploadStatus), (int)upload.StatusId))
  {
      var status = (SalesDataUploadStatus)upload.StatusId;
  }
  ```

**Problem: Enum values not matching database Lookup table**
- **Cause**: Database-backed enums (`SalesDataUploadStatus`, `AuditActionType`) must match Lookup IDs
- **Solution**: Verify enum value matches database: `SELECT * FROM Lookup WHERE Id IN (71,72,73,74)`

**Problem: Marketing class "Unclassified" appearing frequently**
- **Cause**: Parser couldn't determine customer type from upload data
- **Solution**: Enhance parser logic or manually reclassify via `ICustomerService.UpdateMarketingClass()`

**Problem: Annual uploads stuck in "Pending" status**
- **Cause**: `AuditActionId = Pending` requires manual review - automated processing halted
- **Solution**: Review audit records, fix data issues, set to `Approved` or `Rejected`

### Best Practices

**Type Safety**:
```csharp
// ✅ Type-safe and self-documenting
upload.StatusId = (long)SalesDataUploadStatus.Parsed;

// ❌ Magic number, error-prone
upload.StatusId = 72;
```

**Reverse Lookup**:
```csharp
// ✅ Convert back to enum for logic
var status = (SalesDataUploadStatus)upload.StatusId;
if (status == SalesDataUploadStatus.Parsed)
{
    // Process parsed data
}

// ❌ Comparing long values directly (works but less clear)
if (upload.StatusId == 72)
{
    // What does 72 mean?
}
```

**Validation Before Cast**:
```csharp
// ✅ Safe casting with validation
if (Enum.IsDefined(typeof(MarketingClassType), customer.ClassTypeId))
{
    var classType = (MarketingClassType)customer.ClassTypeId;
    Console.WriteLine($"Class: {classType.GetDisplayName()}");
}
else
{
    Console.WriteLine($"Invalid marketing class: {customer.ClassTypeId}");
}
```

### Performance Notes

- Enum comparisons are extremely fast (integer comparison)
- Casting has no runtime overhead
- Use enums in LINQ queries freely - they're just integers in SQL
- Don't convert enums to strings unless necessary for UI display
<!-- END CUSTOM SECTION -->
