<!-- AUTO-GENERATED: Header -->
# Sales Enum
> Type-Safe Constants for Status Codes, Customer Segmentation, and Audit Classifications
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Sales Enum** module provides **compile-time type safety** for lookup table references used throughout the Sales CRM system. These enums map to `Lookup.Id` primary keys in the database, preventing magic numbers and providing IntelliSense support.

**Key Pattern**: Enum values are hardcoded integers that match database `Lookup` table primary keys. Code uses enums for readability, but casts to `long` for database queries.

**Enums Included**:
- `SalesDataUploadStatus` - File upload workflow states (Uploaded → Parsed/Failed)
- `JobStatusEnum` - Job scheduler state machine (Created → Completed)
- `MarketingClassType` - Customer segmentation categories (Residential, Commercial, etc.)
- `AuditReportType` - Annual reconciliation discrepancy types (38 categories)
- `AuditActionType` - HQ approval workflow (Approved, Rejected, Pending)
- `AnnualGroupType` - High-level audit groupings (UnderReporting, OverReporting)
- `DocumentEnum` - Required franchisee documents (COI, Tax Filings, etc.)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Quick Start

### Sales Data Upload Status
```csharp
// Set status when creating upload
var upload = new SalesDataUpload
{
    FranchiseeId = 42,
    FileId = fileId,
    StatusId = (long)SalesDataUploadStatus.Uploaded, // ← Cast to long
    // ...
};
_repository.Save(upload);

// Query by status
var pendingUploads = _repository.Table
    .Where(u => u.StatusId == (long)SalesDataUploadStatus.Uploaded)
    .ToList();

// Update status after parsing
upload.StatusId = (long)SalesDataUploadStatus.Parsed;
_repository.Save(upload);
```

### Marketing Class Segmentation
```csharp
// Filter customers by class
var residentialCustomers = _customerRepo.Table
    .Where(c => c.ClassTypeId == (long)MarketingClassType.Residential)
    .ToList();

// Check if customer is national account
if (customer.ClassTypeId == (long)MarketingClassType.National)
{
    // Apply special pricing/fee rules
    var nationalFee = CalculateNationalAccountFee(invoiceAmount);
}

// UI dropdown population (AngularJS)
// Note: UI reads from Lookup table directly, not enum
```

### Job Status Workflow
```csharp
// Create new job
var job = new JobScheduler
{
    StatusId = (long)JobStatusEnum.Created,
    // ...
};

// Assign to technician
job.TechnicianId = techId;
job.StatusId = (long)JobStatusEnum.Assigned;

// Technician starts work
job.StatusId = (long)JobStatusEnum.InProgress;

// Job completed
job.StatusId = (long)JobStatusEnum.Completed;
job.CompletedDate = DateTime.Now;
```

### Annual Audit Actions
```csharp
// HQ uploads franchisee's annual tax filing
var annualUpload = new AnnualSalesDataUpload
{
    FranchiseeId = 42,
    FileId = taxFilingFileId,
    AuditActionId = (long)AuditActionType.Pending, // ← Default state
    // ...
};

// After HQ review, approve or reject
if (noDiscrepanciesFound)
{
    annualUpload.AuditActionId = (long)AuditActionType.Approved;
}
else
{
    annualUpload.AuditActionId = (long)AuditActionType.Rejected;
    // Send notification to franchisee for correction
}

// Query pending reviews
var pendingReviews = _annualUploadRepo.Table
    .Where(a => a.AuditActionId == (long)AuditActionType.Pending)
    .Where(a => a.StatusId == (long)SalesDataUploadStatus.Parsed)
    .ToList();
```

### Document Validation
```csharp
// Check if franchisee can upload sales data
public bool CanUploadSalesData(long franchiseeId)
{
    // Check COI (Certificate of Insurance)
    var coi = _franchiseeDocumentRepo.Table
        .Where(d => d.FranchiseeId == franchiseeId)
        .Where(d => d.DocumentTypeId == (long)DocumentEnum.COI)
        .OrderByDescending(d => d.ExpirationDate)
        .FirstOrDefault();
    
    if (coi == null || coi.ExpirationDate < DateTime.Now)
        return false; // COI expired
    
    // Check Resale Certificate
    var resale = _franchiseeDocumentRepo.Table
        .Where(d => d.FranchiseeId == franchiseeId)
        .Where(d => d.DocumentTypeId == (long)DocumentEnum.ResaleCertificate)
        .OrderByDescending(d => d.ExpirationDate)
        .FirstOrDefault();
    
    if (resale == null || resale.ExpirationDate < DateTime.Now)
        return false; // Resale certificate expired
    
    return true; // All required docs valid
}
```

### Audit Report Categorization
```csharp
// Create audit record for discrepancy
var auditRecord = new SystemAuditRecord
{
    FranchiseeId = franchiseeId,
    AnnualUploadId = annualUpload.Id,
    InvoiceId = discrepancyInvoice.Id,
    QBIdentifier = "INV-2024-001",
    AnnualReportTypeId = (long)AuditReportType.Type1A // ← Specific audit type
};

// Query audits by type for review
var type1Audits = _auditRecordRepo.Table
    .Where(a => a.AnnualReportTypeId == (long)AuditReportType.Type1A)
    .ToList();
```

### Annual Group Classification
```csharp
// Classify discrepancy for dashboard
public AnnualGroupType ClassifyDiscrepancy(decimal weeklyTotal, decimal annualTotal, decimal threshold)
{
    var variance = Math.Abs(weeklyTotal - annualTotal);
    var percentVariance = (variance / annualTotal) * 100;
    
    if (percentVariance < threshold) // e.g., 5%
        return AnnualGroupType.ReviewNoAction;
    
    if (weeklyTotal < annualTotal)
        return AnnualGroupType.UnderReportomg; // Note: Typo preserved from DB
    
    return AnnualGroupType.OverReporting;
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## 📚 Enum Catalog

### SalesDataUploadStatus (Upload Workflow)
| Value | Name | Meaning |
|-------|------|---------|
| 71 | `Uploaded` | File saved to storage, awaiting parsing |
| 74 | `ParseInProgress` | Polling agent is currently processing |
| 72 | `Parsed` | Successfully parsed into domain entities |
| 73 | `Failed` | Parsing error occurred (see ParsedLogFileId) |

**State Flow**: `Uploaded → ParseInProgress → (Parsed | Failed)`

---

### JobStatusEnum (Job Lifecycle)
| Value | Name | Meaning |
|-------|------|---------|
| 1 | `Created` | Job created, not yet assigned |
| 2 | `Assigned` | Assigned to technician, scheduled |
| 3 | `InProgress` | Technician working on job |
| 4 | `Completed` | Job finished, invoice generated |
| 5 | `Canceled` | Job canceled by customer or HQ |
| 6 | `Tentative` | Estimate/quote, not confirmed |

**State Flow**: `Created → Assigned → InProgress → Completed`  
**Alternative**: `Tentative → Assigned` (when estimate accepted)

---

### MarketingClassType (Customer Segmentation)
| Value | Name | Description |
|-------|------|-------------|
| 1 | `Commercial` | Office buildings, retail stores |
| 2 | `Education` | Schools, universities |
| 3 | `Hotel` | Hotels, resorts |
| 4 | `Residential` | Homeowners (primary segment) |
| 5 | `BuilderTile` | Tile contractors, builders |
| 6 | `Church` | Religious institutions |
| 7 | `Club` | Country clubs, gyms |
| 8 | `Janitorial` | Janitorial service companies |
| 9 | `MedicalLegal` | Hospitals, law firms |
| 10 | `Restaurant` | Food service |
| 11 | `Unclassified` | Unknown/pending classification |
| 12 | `Condo` | Condominium associations |
| 13 | `Bank` | Financial institutions |
| 14 | `Goverment` | Government buildings (**Note typo**) |
| 15 | `Flooring` | Flooring companies |
| 16 | `Builder` | General contractors |
| 17 | `National` | National account customers |
| 18 | `Mld` | Marblelife Direct (internal) |

**Business Rules**:
- `Residential` = 60%+ of customer base
- `National` triggers special fee calculation
- `Unclassified` requires manual review
- **Typo**: `Goverment` (not `Government`) matches DB

---

### AuditReportType (38 Audit Categories)
Values range from 1-38 with names like `Type1A`, `Type6`, `Type17C`, etc.

**Purpose**: Categorizes discrepancies found during annual tax filing reconciliation.

**Note**: Type names are intentionally generic. Actual meanings documented in HQ accounting manual (proprietary).

**Common Types** (examples):
- `Type1` (22) - General discrepancy
- `Type6` (1) - High-priority review
- `Type1A` (5) - Sub-category of Type1

---

### AuditActionType (HQ Approval Workflow)
| Value | Name | Meaning |
|-------|------|---------|
| 151 | `Approved` | HQ reviewed and accepted annual filing |
| 152 | `Rejected` | HQ found issues, requires franchisee correction |
| 153 | `Pending` | Awaiting HQ review (default state) |

**Workflow**: `Pending → (Approved | Rejected)` → If rejected → Corrected upload → `Pending` (cycle)

---

### AnnualGroupType (High-Level Groupings)
| Value | Name | Meaning |
|-------|------|---------|
| 1 | `UnderReportomg` | Franchisee reported less than tax filing (**Note typo**) |
| 2 | `OverReporting` | Franchisee reported more than tax filing |
| 3 | `ReviewNoAction` | Discrepancy under threshold, acceptable |
| 4 | `Type1H` | Special case (links to AuditReportType) |

**Threshold**: Typically 5% variance triggers action vs. no action.

---

### DocumentEnum (Required Franchisee Documents)
| Value | Name | Description |
|-------|------|-------------|
| 3 | `AnnualTaxFiling` | Year-end tax return |
| 5 | `COI` | Certificate of Insurance |
| 11 | `ResaleCertificate` | Sales tax exemption |
| 15 | `PAndL` | Profit & Loss statement |
| 16 | `BalanceSheet` | Balance sheet |
| 19 | `MaterialPurchase` | Material purchase receipts |
| 17 | `AnnualPayble` | Accounts payable (**Note typo**) |
| 18 | `AnnulaReceivable` | Accounts receivable (**Note typo**) |

**Validation**: `COI` and `ResaleCertificate` must be current to upload sales data.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Common Patterns -->
## 📋 Common Patterns

### 1. Always Cast to Long
```csharp
// ❌ Wrong - enum is int, DB column is long
upload.StatusId = (int)SalesDataUploadStatus.Uploaded; // Implicit cast may fail

// ✅ Correct - explicit long cast
upload.StatusId = (long)SalesDataUploadStatus.Uploaded;
```

### 2. Query with Enum
```csharp
// Find all uploads ready for parsing
var pending = _uploadRepo.Table
    .Where(u => u.StatusId == (long)SalesDataUploadStatus.Uploaded)
    .OrderBy(u => u.DataRecorderMetaData.DateCreated)
    .ToList();
```

### 3. Status Transition Logic
```csharp
public void TransitionStatus(SalesDataUpload upload, SalesDataUploadStatus newStatus)
{
    // Validate allowed transitions
    var allowedTransitions = new Dictionary<SalesDataUploadStatus, List<SalesDataUploadStatus>>
    {
        { SalesDataUploadStatus.Uploaded, new List<SalesDataUploadStatus> { SalesDataUploadStatus.ParseInProgress } },
        { SalesDataUploadStatus.ParseInProgress, new List<SalesDataUploadStatus> { SalesDataUploadStatus.Parsed, SalesDataUploadStatus.Failed } },
        // Failed and Parsed can be reset to Uploaded via Reparse()
    };
    
    var currentStatus = (SalesDataUploadStatus)upload.StatusId;
    if (!allowedTransitions[currentStatus].Contains(newStatus))
    {
        throw new InvalidOperationException($"Cannot transition from {currentStatus} to {newStatus}");
    }
    
    upload.StatusId = (long)newStatus;
}
```

### 4. Marketing Class Business Logic
```csharp
public decimal CalculateFee(long marketingClassId, decimal amount)
{
    var classType = (MarketingClassType)marketingClassId;
    
    switch (classType)
    {
        case MarketingClassType.National:
            return amount * 0.04m; // 4% national account fee
        
        case MarketingClassType.Residential:
        case MarketingClassType.Commercial:
            return amount * 0.08m; // 8% standard royalty
        
        case MarketingClassType.Unclassified:
            throw new ValidationException("Customer must be classified before billing");
        
        default:
            return amount * 0.08m; // Default royalty rate
    }
}
```

### 5. Document Expiration Check
```csharp
public List<string> GetExpiringDocuments(long franchiseeId, int daysWarning = 30)
{
    var expiringDocs = new List<string>();
    var warningDate = DateTime.Now.AddDays(daysWarning);
    
    // Check COI
    var coi = GetLatestDocument(franchiseeId, DocumentEnum.COI);
    if (coi != null && coi.ExpirationDate < warningDate)
        expiringDocs.Add($"COI expires on {coi.ExpirationDate:MM/dd/yyyy}");
    
    // Check Resale Certificate
    var resale = GetLatestDocument(franchiseeId, DocumentEnum.ResaleCertificate);
    if (resale != null && resale.ExpirationDate < warningDate)
        expiringDocs.Add($"Resale Certificate expires on {resale.ExpirationDate:MM/dd/yyyy}");
    
    return expiringDocs;
}
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Troubleshooting -->
## 🔧 Troubleshooting

### Issue: "Cannot implicitly convert type 'int' to 'long'"
**Cause**: Enum is `int` by default, but database columns are `long`.

**Solution**: Always cast to `long`:
```csharp
// ❌ Compiler error
entity.StatusId = SalesDataUploadStatus.Uploaded;

// ✅ Correct
entity.StatusId = (long)SalesDataUploadStatus.Uploaded;
```

### Issue: Query returns no results with enum filter
**Cause**: Enum value doesn't match database `Lookup.Id`.

**Diagnosis**:
```sql
-- Check Lookup table
SELECT * FROM Lookup WHERE Id IN (71, 72, 73, 74);
-- Verify IDs match enum values
```

**Solution**: If mismatch found, update enum values or add missing Lookup rows.

### Issue: "Marketing class not found" during import
**Cause**: Excel file has class name that doesn't match `MarketingClass.Name`.

**Solution**: Update parser to handle variations:
```csharp
// Normalize class name (remove spaces, uppercase)
var className = row["Class"].ToString().ToUpper().Replace(" ", "");
var marketingClass = _marketingClasses
    .FirstOrDefault(mc => mc.Name.ToUpper().Replace(" ", "") == className);

if (marketingClass == null)
{
    // Fallback to Unclassified
    marketingClass = _marketingClasses
        .First(mc => mc.Id == (long)MarketingClassType.Unclassified);
}
```

### Issue: Typo causing validation errors
**Symptoms**: Code expects "Government" but database has "Goverment".

**Solution**: Use enum, not string literals:
```csharp
// ❌ Wrong - string comparison fails
if (customer.MarketingClass.Name == "Government")

// ✅ Correct - use enum
if (customer.ClassTypeId == (long)MarketingClassType.Goverment)
```

### Issue: New enum value not working in production
**Cause**: Enum added to code, but `Lookup` table not updated in database.

**Solution**: Create SQL migration:
```sql
-- Add new status
INSERT INTO Lookup (Id, Name, LookupTypeId)
VALUES (75, 'NewStatus', 10); -- LookupTypeId 10 = SalesDataUploadStatus
```

Then update enum:
```csharp
public enum SalesDataUploadStatus
{
    Uploaded = 71,
    ParseInProgress = 74,
    Parsed = 72,
    Failed = 73,
    NewStatus = 75 // ← New value
}
```
<!-- END CUSTOM SECTION -->

<!-- CUSTOM SECTION: Related Links -->
## 🔗 Related Documentation
- [Sales/Domain README](../Domain/README.md) - Entity definitions using these enums
- [Sales/Impl README](../Impl/README.md) - Business logic with enum patterns
- [Application/Domain](../../Application/Domain/README.md) - Lookup table structure
- [Organizations/Enum](../../Organizations/Enum/README.md) - Similar enum pattern for organizations
<!-- END CUSTOM SECTION -->
