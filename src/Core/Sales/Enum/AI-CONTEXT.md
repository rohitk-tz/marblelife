<!-- AUTO-GENERATED: Header -->
# Sales Enum Module Context
**Version**: 3f7ca98653b76ee0fca84e0a126043097a12de5d
**Generated**: 2026-02-04T06:51:55Z
**Module Path**: src/Core/Sales/Enum
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
The Sales Enum module provides **type-safe constants** for lookup table mappings and status codes used throughout the Sales module. These enums map to `Lookup.Id` primary keys in the database, providing compile-time safety for database references.

**Key Pattern**: **Enum-to-Lookup Mapping**
- All enum values are **hardcoded integers** that match `Lookup.Id` primary keys
- Enums are used in code, but stored as `long` in database `StatusId` fields
- This pattern provides IntelliSense support while maintaining referential integrity

### Design Patterns
- **Type-Safe Database References**: Enums prevent magic numbers in code (e.g., `StatusId = 71` becomes `StatusId = (long)SalesDataUploadStatus.Uploaded`)
- **Lookup Table Pattern**: Database `Lookup` table stores human-readable names, enums provide code-level constants
- **Audit Trail Classification**: `AuditReportType` and `AnnualGroupType` categorize financial discrepancies

### Data Flow
```
Code Layer (Type-Safe)
  ↓
  SalesDataUploadStatus.Parsed (enum value)
  ↓
  Cast to long: (long)SalesDataUploadStatus.Parsed → 72
  ↓
Database Layer
  ↓
  SalesDataUpload.StatusId = 72 (FK to Lookup.Id)
  ↓
  Lookup table: { Id: 72, Name: "Parsed", LookupTypeId: <status_type> }
```

### Cross-Module Consistency
These enums are referenced across:
- **Domain Layer**: Entity properties (e.g., `SalesDataUpload.StatusId`)
- **Implementation Layer**: Service logic (e.g., `upload.StatusId = (long)SalesDataUploadStatus.Parsed`)
- **API Layer**: Status filtering (e.g., `?statusId=72`)
- **UI Layer**: Status display (AngularJS reads Lookup table directly)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Enum Definitions

### SalesDataUploadStatus (File Upload Workflow States)
```csharp
public enum SalesDataUploadStatus
{
    Uploaded = 71,        // File saved, awaiting parsing
    ParseInProgress = 74, // Currently being processed by polling agent
    Parsed = 72,          // Successfully parsed into entities
    Failed = 73           // Parsing error occurred (see ParsedLogFileId)
}
```

**Usage**: Tracks lifecycle of `SalesDataUpload`, `CustomerFileUpload`, `InvoiceFileUpload`, `AnnualSalesDataUpload`

**State Transitions**:
```
Uploaded (71) → ParseInProgress (74) → Parsed (72)
                                     ↘ Failed (73)
```

**Query Example**:
```csharp
// Find all uploads ready for parsing
var pending = _uploadRepo.Table
    .Where(u => u.StatusId == (long)SalesDataUploadStatus.Uploaded)
    .ToList();
```

---

### JobStatusEnum (Job/Task State Machine)
```csharp
public enum JobStatusEnum
{
    Created = 1,      // Job created, not assigned to technician
    Assigned = 2,     // Assigned to technician, not started
    InProgress = 3,   // Technician en route or working
    Completed = 4,    // Job finished, invoice generated
    Canceled = 5,     // Job canceled by customer or HQ
    Tentative = 6     // Estimate/quote, not confirmed
}
```

**Usage**: Tracks job scheduler workflow (links to `Core.Scheduler` module)

**State Transitions**:
```
Created (1) → Assigned (2) → InProgress (3) → Completed (4)
                          ↘ Canceled (5)
Tentative (6) → Assigned (2) [when customer accepts estimate]
```

---

### MarketingClassType (Customer Segmentation)
```csharp
public enum MarketingClassType
{
    Commercial = 1,      // Office buildings, retail stores
    Education = 2,       // Schools, universities
    Hotel = 3,           // Hotels, resorts
    Residential = 4,     // Homeowners (primary segment)
    BuilderTile = 5,     // Tile contractors, builders
    Church = 6,          // Religious institutions
    Club = 7,            // Country clubs, gyms
    Janitorial = 8,      // Janitorial service companies
    MedicalLegal = 9,    // Hospitals, law firms
    Restaurant = 10,     // Food service
    Unclassified = 11,   // Default for unknown
    Condo = 12,          // Condominium associations
    Bank = 13,           // Financial institutions
    Goverment = 14,      // Government buildings (note typo in DB)
    Flooring = 15,       // Flooring companies
    Builder = 16,        // General contractors
    National = 17,       // National account customers
    Mld = 18             // Marblelife Direct (internal)
}
```

**Usage**: Primary customer classification for reporting and pricing

**Business Rules**:
- `National` customers have special fee calculation (see `NationalInvoiceItem`)
- `Residential` is the most common class (60%+ of customers)
- `Unclassified` requires manual review during import
- Typo: `Goverment` (not `Government`) is the actual DB value

**Query Example**:
```csharp
// Get all commercial customers
var commercial = _customerRepo.Table
    .Where(c => c.ClassTypeId == (long)MarketingClassType.Commercial)
    .ToList();
```

---

### AuditReportType (Annual Reconciliation Discrepancy Categories)
```csharp
public enum AuditReportType
{
    Type6 = 1,      // Specific audit types defined by accounting team
    Type1C = 2,     // (Internal documentation not available)
    Type5 = 3,
    Type1B = 4,
    Type1A = 5,
    Type14 = 6,
    Type4 = 7,
    Type1F = 8,
    Type1D = 9,
    Type3 = 10,
    Type2A = 11,
    Type2B = 12,
    Type7 = 13,
    Type8 = 14,
    Type9 = 15,
    Type10B = 16,
    Type11 = 17,
    Type12 = 18,
    Type13 = 19,
    Type13B = 20,
    Type16 = 21,
    Type1 = 22,
    Type17 = 23,
    Type17A = 24,
    Type17B = 25,
    Type17C = 26,
    Type17D = 27,
    Type17E = 28,
    Type18A = 29,
    Type18B = 30,
    Type18C = 31,
    Type18D = 32,
    Type18E = 33,
    Type1H = 34,
    Type5A = 35,
    Type4B = 36,
    Type5B = 37,
    Type4A = 38
}
```

**Usage**: Categorizes discrepancies found during annual tax filing reconciliation (`SystemAuditRecord.AnnualReportTypeId`)

**Context**: When `AnnualSalesDataUpload` detects mismatch between weekly uploads and tax filings, a `SystemAuditRecord` is created with specific audit type. HQ accounting reviews these by type.

**Note**: Type names are intentionally generic (e.g., "Type1A") to avoid exposing proprietary audit logic. Actual meanings are documented in HQ accounting manual.

---

### AuditActionType (Annual Upload Approval Workflow)
```csharp
public enum AuditActionType
{
    Approved = 151,  // HQ reviewed and accepted annual filing
    Rejected = 152,  // HQ found issues, requires franchisee correction
    Pending = 153    // Awaiting HQ review (default state)
}
```

**Usage**: Tracks HQ approval status of `AnnualSalesDataUpload.AuditActionId`

**Workflow**:
```
1. Franchisee uploads annual tax docs
   → AuditActionId = Pending (153)

2. HQ reviews SystemAuditRecords
   ↓
3a. No issues → AuditActionId = Approved (151)
   OR
3b. Issues found → AuditActionId = Rejected (152)
      ↓
   4. Franchisee corrects and re-uploads
      → AuditActionId = Pending (153) [cycle repeats]
```

**Query Example**:
```csharp
// Find all pending annual reviews
var pendingReviews = _annualUploadRepo.Table
    .Where(a => a.AuditActionId == (long)AuditActionType.Pending)
    .Where(a => a.StatusId == (long)SalesDataUploadStatus.Parsed)
    .ToList();
```

---

### AnnualGroupType (Discrepancy Classification)
```csharp
public enum AnnualGroupType
{
    UnderReportomg = 1,  // Note: Typo "Reportomg" (not "Reporting")
    OverReporting = 2,   // Franchisee reported more than tax filing
    ReviewNoAction = 3,  // Discrepancy under threshold, no action needed
    Type1H = 4           // Special case (reference to AuditReportType.Type1H)
}
```

**Usage**: High-level grouping of audit discrepancies for dashboard/reporting

**Business Rules**:
- `UnderReportomg` (typo in DB): Franchisee reported less sales than tax filing shows → Potential missing royalty
- `OverReporting`: Usually data entry error, requires correction
- `ReviewNoAction`: Variance < 5% threshold, marked as acceptable
- `Type1H`: Cross-reference to specific `AuditReportType`

**Note**: Typo `UnderReportomg` cannot be fixed without DB migration due to existing data.

---

### DocumentEnum (Required Franchisee Documents)
```csharp
public enum DocumentEnum
{
    AnnualTaxFiling = 3,      // Year-end tax return
    COI = 5,                  // Certificate of Insurance
    ResaleCertificate = 11,   // Sales tax exemption
    PAndL = 15,               // Profit & Loss statement
    BalanceSheet = 16,        // Balance sheet
    MaterialPurchase = 19,    // Material purchase receipts
    AnnualPayble = 17,        // Accounts payable (note typo "Payble")
    AnnulaReceivable = 18     // Accounts receivable (note typo "Annula")
}
```

**Usage**: References `Organizations.Domain.DocumentType` for required franchisee documents

**Validation Logic**: `SalesDataUploadService.CheckForExpiringDocument()` blocks upload if:
- `COI` expired
- `ResaleCertificate` expired
- `AnnualTaxFiling` not uploaded for previous year

**Note**: Typos in enum names (`Payble`, `AnnulaReceivable`) match DB `DocumentType.Name` exactly.

**Query Example**:
```csharp
// Check if COI is expired
var coi = _franchiseeDocumentRepo.Table
    .Where(d => d.FranchiseeId == franchiseeId)
    .Where(d => d.DocumentTypeId == (long)DocumentEnum.COI)
    .OrderByDescending(d => d.ExpirationDate)
    .FirstOrDefault();

if (coi == null || coi.ExpirationDate < DateTime.Now)
{
    throw new ValidationException("Certificate of Insurance expired");
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal Module Dependencies
- **Core.Application.Domain** - `Lookup` table (stores human-readable enum names)
- **Core.Sales.Domain** - Entity properties reference these enums (e.g., `SalesDataUpload.StatusId`)
- **Core.Organizations.Domain** - `DocumentType` (referenced by `DocumentEnum`)

### External Dependencies
- None (pure enum definitions)

### Cross-Module Usage
```
┌────────────────────────────────────────────────────────────┐
│               Enum Usage Across Layers                      │
└────────────────────────────────────────────────────────────┘

Code Layer (Sales.Impl)
  ├─ SalesDataUploadService.Save()
  │   └─ upload.StatusId = (long)SalesDataUploadStatus.Uploaded
  │
  └─ SalesDataParsePollingAgent.Execute()
      ├─ WHERE StatusId == (long)SalesDataUploadStatus.Uploaded
      └─ SET StatusId = (long)SalesDataUploadStatus.Parsed

Database Layer (ORM)
  ├─ SalesDataUpload.StatusId = 72 (FK to Lookup.Id)
  │
  └─ Lookup table:
      { Id: 72, Name: "Parsed", LookupTypeId: 10 }

API Layer (API/Controllers)
  └─ Query param: ?statusId=72 (direct integer, UI doesn't use enum)

UI Layer (Web.UI AngularJS)
  └─ $scope.statuses = Lookup.getByType('SalesDataUploadStatus')
      // Returns: [{ id: 71, name: "Uploaded" }, { id: 72, name: "Parsed" }, ...]
```

### Related Documentation
- [Sales/Domain AI-CONTEXT](../Domain/AI-CONTEXT.md) - Entity definitions using these enums
- [Sales/Impl AI-CONTEXT](../Impl/AI-CONTEXT.md) - Business logic with enum usage patterns
- [Application/Domain](../../Application/Domain/AI-CONTEXT.md) - `Lookup` table definition
- [Organizations/Enum](../../Organizations/Enum/AI-CONTEXT.md) - Similar enum-to-lookup pattern
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Key Gotchas
1. **Typo Preservation**: `UnderReportomg`, `Payble`, `AnnulaReceivable`, `Goverment` - these typos are **intentional** to match DB data. Do NOT fix in code.
2. **Non-Sequential IDs**: Enum values are NOT sequential (e.g., 71, 72, 73, 74). These match historical `Lookup.Id` assignments.
3. **Always Cast to Long**: Database columns are `long`, enums are `int`. Always cast: `(long)SalesDataUploadStatus.Parsed`.
4. **Lookup Table Sync**: Adding new enum values requires corresponding `Lookup` table insert via SQL migration.
5. **Audit Type Opacity**: `AuditReportType` values (Type1A, Type6, etc.) are intentionally vague. Detailed meanings are in HQ accounting manual (not in code).
6. **Marketing Class Coverage**: `MarketingClassType` enum doesn't cover ALL values in `MarketingClass` table. Custom classes exist (e.g., franchisee-specific segments).

### Historical Context
- **Lookup Table Pattern**: Inherited from legacy system (pre-EF). Modern approach would use discriminator column or enum columns (SQL Server 2016+).
- **Sparse IDs**: Gaps in enum values (e.g., 71, 72, 73, 74 for status, but 151, 152, 153 for audit action) reflect different `LookupType` groupings.
- **Audit Types Proliferation**: Started with 5 types (Type1-Type5), grew to 38+ as accounting refined categorization over years.

### Performance Considerations
- **Index on StatusId**: All upload tables have index on `StatusId`. Queries like `WHERE StatusId = 71` are fast.
- **Enum Caching**: UI loads `Lookup` table once and caches. Changes require app pool restart.

### Testing Notes
- **Unit Tests**: Use enum values directly in assertions: `Assert.AreEqual((long)SalesDataUploadStatus.Parsed, upload.StatusId)`
- **Integration Tests**: Seed `Lookup` table with enum values in test database setup
- **Mocking**: When mocking repositories, return entities with enum-casted `StatusId` fields
<!-- END CUSTOM SECTION -->
