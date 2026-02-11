<!-- AUTO-GENERATED: Header -->
# Sales/Enum — Module Context
**Version**: 12e518684e2978a09496d9ffba2431538d7f10e7
**Generated**: 2025-02-10T15:40:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Enum folder contains 7 enumeration types that provide strongly-typed constants for customer statuses, marketing classifications, sales upload statuses, annual report categorizations, audit actions, and document types. These enumerations ensure data consistency across the application by replacing magic numbers with named constants and enabling compile-time type safety.

### Design Patterns
- **Type-Safe Enumerations**: C# enums providing compile-time validation
- **Database-Backed Enums**: Values match database Lookup table IDs (e.g., `AuditActionType.Approved = 151`)
- **Status State Machines**: Enums like `SalesDataUploadStatus` and `JobStatusEnum` define valid state progressions
- **Hierarchical Classification**: `MarketingClassType` provides 18 distinct customer segments

### Purpose
These enumerations serve as:
1. **Status Tracking**: Upload status (Uploaded → ParseInProgress → Parsed/Failed)
2. **Workflow States**: Audit actions (Pending → Approved/Rejected)
3. **Business Classification**: Marketing classes for customer segmentation
4. **Report Categorization**: Annual audit report types (40+ variations)
5. **Document Tracking**: Required document types for compliance
6. **Job Lifecycle**: Job status tracking (Created → Assigned → InProgress → Completed)

### Usage Context
- **Sales Upload Workflow**: `SalesDataUploadStatus` tracks file processing lifecycle
- **Annual Audits**: `AuditActionType` and `AuditReportType` drive manual review workflow
- **Customer Segmentation**: `MarketingClassType` categorizes customers for reporting and marketing
- **Document Compliance**: `DocumentEnum` identifies required franchisee documents
- **Job Management**: `JobStatusEnum` tracks estimate and service job states
- **Annual Analytics**: `AnnualGroupType` categorizes audit findings
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Enumeration Definitions

### SalesDataUploadStatus.cs
```csharp
public enum SalesDataUploadStatus
{
    Uploaded = 71,        // File uploaded, queued for parsing
    ParseInProgress = 74, // Currently being parsed by polling agent
    Parsed = 72,          // Successfully parsed, data imported
    Failed = 73           // Parsing failed, see error logs
}
```

**Purpose**: Tracks the lifecycle of a sales data upload from submission through parsing completion.

**State Transitions**:
```
Uploaded → ParseInProgress → Parsed (success)
                           → Failed (errors)
```

**Database Mapping**: Values correspond to Lookup table entries (ID 71-74).

**Usage**: Set on `SalesDataUpload.StatusId` and `AnnualSalesDataUpload.StatusId`.

---

### MarketingClassType.cs
```csharp
public enum MarketingClassType
{
    Commercial = 1,     // Office buildings, retail spaces
    Education = 2,      // Schools, universities
    Hotel = 3,          // Hotels, resorts
    Residential = 4,    // Homes, apartments
    BuilderTile = 5,    // New construction tile work
    Church = 6,         // Religious institutions
    Club = 7,           // Country clubs, fitness centers
    Janitorial = 8,     // Cleaning services
    MedicalLegal = 9,   // Medical offices, law firms
    Restaurant = 10,    // Restaurants, cafes
    Unclassified = 11,  // Unable to categorize
    Condo = 12,         // Condominiums
    Bank = 13,          // Financial institutions
    Goverment = 14,     // Government buildings
    Flooring = 15,      // Flooring contractors
    Builder = 16,       // General contractors
    National = 17,      // National account customers
    Mld = 18            // MarbleLife Direct
}
```

**Purpose**: Categorizes customers into 18 business segments for:
- Revenue reporting by segment
- Marketing campaign targeting
- Pricing strategy differentiation
- Royalty calculation variations

**Key Segments**:
- **High-Volume**: Hotel, Commercial, National (larger contracts)
- **Specialized**: BuilderTile, Flooring, Builder (trade contractors)
- **Institutional**: Education, Church, Government, MedicalLegal
- **Consumer**: Residential, Condo
- **Service**: Janitorial, Restaurant, Club

**Usage**: Set on `Customer.ClassTypeId` and `EstimateInvoice.ClassTypeId`.

---

### AuditActionType.cs
```csharp
public enum AuditActionType
{
    Approved = 151,  // Annual data reviewed and accepted
    Rejected = 152,  // Annual data rejected, not imported
    Pending = 153    // Awaiting staff review
}
```

**Purpose**: Tracks approval status for annual sales data uploads requiring manual audit.

**Workflow**:
1. Annual upload parsed → `Pending`
2. Staff reviews flagged records (address mismatches, data anomalies)
3. Staff decision → `Approved` (data flows into system) or `Rejected` (data discarded)

**Database Mapping**: Lookup table IDs 151-153.

**Usage**: Set on `AnnualSalesDataUpload.AuditActionId`.

---

### AuditReportType.cs
```csharp
public enum AuditReportType
{
    Type6 = 1,    // Specific audit finding category
    Type1C = 2,   // (Documentation indicates 40+ report types
    Type5 = 3,    //  each representing different data quality
    Type1B = 4,   //  or compliance findings discovered during
    Type1A = 5,   //  annual data review. Type names like
    Type14 = 6,   //  "1A", "5B" etc. correspond to internal
    Type4 = 7,    //  audit procedure codes.)
    // ... 40+ total types
}
```

**Purpose**: Categorizes findings during annual sales data audits. Each type represents a specific data quality issue, compliance concern, or reporting discrepancy discovered during the manual review process.

**Examples**:
- **Type1A**: Under-reporting detected
- **Type5**: Address mismatch
- **Type17**: Payment frequency inconsistency
- **Type18A-E**: Various revenue recognition issues

**Usage**: Used in annual audit reports and data quality tracking dashboards.

---

### DocumentEnum.cs
```csharp
public enum DocumentEnum
{
    AnnualTaxFiling = 3,      // Annual tax returns
    COI = 5,                  // Certificate of Insurance
    ResaleCertificate = 11,   // Sales tax exemption certificate
    PAndL = 15,               // Profit & Loss statement
    BalanceSheet = 16,        // Balance sheet
    MaterialPurchase = 19,    // Material purchase receipts
    AnnualPayble = 17,        // Accounts payable report
    AnnulaReceivable = 18     // Accounts receivable report (typo in enum)
}
```

**Purpose**: Identifies required document types for franchisee compliance and financial auditing.

**Compliance Requirements**:
- **Insurance**: COI must be current
- **Tax**: AnnualTaxFiling required annually
- **Sales Tax**: ResaleCertificate for tax-exempt transactions
- **Financial**: PAndL, BalanceSheet for financial health assessment
- **Audit Trail**: MaterialPurchase, AnnualPayble, AnnulaReceivable for revenue verification

**Usage**: Referenced in document upload and compliance tracking systems.

---

### JobStatusEnum.cs
```csharp
public enum JobStatusEnum
{
    Created = 1,      // Estimate created, not yet assigned
    Assigned = 2,     // Assigned to technician
    InProgress = 3,   // Work in progress
    Completed = 4,    // Service completed
    Canceled = 5,     // Job canceled
    Tentative = 6     // Pending customer confirmation
}
```

**Purpose**: Tracks the lifecycle of estimate invoices and service jobs.

**State Transitions**:
```
Created → Tentative (awaiting customer confirmation)
        → Assigned → InProgress → Completed
        → Canceled (any state can cancel)
```

**Usage**: Set on estimate invoice and job scheduler entities to track work order status.

---

### AnnualGroupType.cs
```csharp
public enum AnnualGroupType
{
    UnderReportomg = 1,   // Under-reporting detected (typo in enum)
    OverReporting = 2,    // Over-reporting detected
    ReviewNoAction = 3,   // Reviewed, no issues found
    Type1H = 4            // Specific audit category
}
```

**Purpose**: Categorizes annual audit findings by severity/action required.

**Categories**:
- **UnderReporting**: Franchisee reported less sales than found in audit (potential royalty underpayment)
- **OverReporting**: Franchisee reported more sales than verified (possible error)
- **ReviewNoAction**: Audit complete, no discrepancies
- **Type1H**: Cross-reference to `AuditReportType.Type1H` for detailed finding classification

**Usage**: Used in annual audit summary reports and dashboards.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- None (standalone enumerations)

### External Dependencies
- None (standard C# enums)

### Database Relationships
- **Lookup Tables**: Enum values match database Lookup table IDs
  - `SalesDataUploadStatus` → Lookup IDs 71-74
  - `AuditActionType` → Lookup IDs 151-153
  - `DocumentEnum` → Lookup IDs 3, 5, 11, 15-19
  - Other enums stored as integers in entity columns
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Naming Conventions
- **Database-Backed Enums**: Values explicitly set to match Lookup table IDs (e.g., `Uploaded = 71`)
- **Semantic Enums**: Values auto-incremented from 1 (e.g., `MarketingClassType`)
- **Typos Present**: `UnderReportomg` (should be UnderReporting), `AnnulaReceivable` (should be AnnualReceivable)

### Usage Patterns

**Status Checking**:
```csharp
if (upload.StatusId == (long)SalesDataUploadStatus.Parsed)
{
    // Safe to use parsed data
}
```

**Classification Queries**:
```csharp
var hotels = customers.Where(c => c.ClassTypeId == (long)MarketingClassType.Hotel);
var highValue = new[] { 
    MarketingClassType.Hotel, 
    MarketingClassType.Commercial, 
    MarketingClassType.National 
};
var vipCustomers = customers.Where(c => highValue.Contains((MarketingClassType)c.ClassTypeId));
```

**Audit Workflow**:
```csharp
// After parsing annual data
annualUpload.AuditActionId = (long)AuditActionType.Pending;

// After staff review
if (dataIsValid)
    annualUpload.AuditActionId = (long)AuditActionType.Approved;
else
    annualUpload.AuditActionId = (long)AuditActionType.Rejected;
```

### Best Practices
- **Always Cast**: Use `(long)EnumValue` when assigning to entity properties (they're stored as `long` in DB)
- **Reverse Lookup**: Cast back to enum when reading: `(SalesDataUploadStatus)upload.StatusId`
- **Validation**: Check enum values before casting to avoid invalid data exceptions
- **Magic Numbers**: Never use raw numbers (71, 74) - always use enum constants

### Anti-Patterns to Avoid
❌ **Don't hardcode values**: `upload.StatusId = 72` // What does 72 mean?
✅ **Use enums**: `upload.StatusId = (long)SalesDataUploadStatus.Parsed`

❌ **Don't assume order**: Enum values don't always increment sequentially
✅ **Check explicit values**: `AuditActionType.Approved = 151` (not 1)

### Extension Considerations
When adding new enum values:
1. **Database-Backed**: Coordinate with Lookup table insertions (get correct ID from DBA)
2. **Semantic Enums**: Can append new values freely (auto-increments from last)
3. **Migration**: Update existing data if changing enum meanings
4. **Backward Compatibility**: Adding is safe, changing/removing breaks existing data
<!-- END CUSTOM SECTION -->
