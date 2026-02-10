<!-- AUTO-GENERATED: Header -->
# Domain
> Core domain entities for file management, lookup tables, metadata tracking, and content types
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The **Domain** module contains the six foundational entities that model the business data for the Marble Life application. These classes map to database tables via Entity Framework and represent the "nouns" of the business domain.

**Why these entities exist:**
- **File & Folder**: Manage uploaded documents, images, and organized storage hierarchies
- **Lookup & LookupType**: Provide referential data for dropdowns (e.g., payment types, invoice statuses)
- **ContentType**: Map file extensions to MIME types for proper HTTP headers
- **DataRecorderMetaData**: Track who created/modified each record and when (audit trail)

**Entity Relationships:**
```
File ──1:1──> DataRecorderMetaData (audit trail)
Lookup ──N:1──> LookupType (category grouping)
ContentType (standalone)
Folder (standalone hierarchy)
```

All entities inherit from `DomainBase` which provides:
- `Id` (primary key)
- `IsNew` (entity state flag)
- `IsDeleted` (soft delete flag)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Example 1: Create and Upload a File
```csharp
using Core.Application.Domain;
using Core.Application.Impl;

var file = new File
{
    Name = "invoice-2023.pdf",
    Caption = "January 2023 Invoice",
    Size = 2048576,  // 2MB in bytes
    RelativeLocation = "/invoices/2023/invoice-2023.pdf",
    MimeType = "application/pdf",
    css = "fa-file-pdf",
    DataRecorderMetaData = new DataRecorderMetaData(userId)
};

// Save via repository
fileRepository.Add(file);
unitOfWork.Commit();
```

### Example 2: Update Modification Metadata
```csharp
using Core.Application.Domain;

// Load entity
var file = fileRepository.GetById(fileId);

// Modify properties
file.Caption = "Updated Caption";

// Track modification
file.DataRecorderMetaData.SetModifiedBy(currentUserId);  // Uses Clock.UtcNow

// Save changes
fileRepository.Update(file);
unitOfWork.Commit();
```

### Example 3: Query Active Lookup Values for a Dropdown
```csharp
using Core.Application.Domain;
using Core.Application.Enum;

// Get all active payment types
var paymentTypes = lookupRepository
    .GetAll()
    .Where(l => l.LookupTypeId == (long)LookupTypes.ChargeCardType && l.IsActive)
    .OrderBy(l => l.RelativeOrder)
    .Select(l => new { l.Id, l.Name })
    .ToList();
```

### Example 4: Create Folder Hierarchy
```csharp
using Core.Application.Domain;

var folder = new Folder
{
    RelativePath = "/invoices/2023/january"
};

folderRepository.Add(folder);
unitOfWork.Commit();
```

### Example 5: Resolve MIME Type for File Extension
```csharp
using Core.Application.Domain;

var contentType = contentTypeRepository
    .GetAll()
    .FirstOrDefault(ct => ct.Name.Equals("PDF", StringComparison.OrdinalIgnoreCase));

string mimeType = contentType?.MimeType ?? "application/octet-stream";
// Returns: "application/pdf"
```

### Example 6: Clone Metadata for Child Entity
```csharp
using Core.Application.Domain;

// Parent entity
var invoice = invoiceRepository.GetById(invoiceId);

// Create child entity with same user context
var lineItem = new InvoiceLineItem
{
    InvoiceId = invoice.Id,
    Description = "Service Fee",
    Amount = 150.00m,
    DataRecorderMetaData = invoice.DataRecorderMetaData.GetClone()
};

lineItemRepository.Add(lineItem);
unitOfWork.Commit();
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Entity | Key Properties | Purpose |
|--------|----------------|---------|
| `File` | Name, Caption, Size, RelativeLocation, MimeType, css, DataRecorderMetaDataId | Uploaded file metadata with audit trail |
| `Folder` | RelativePath | Virtual folder for organizing files |
| `Lookup` | LookupTypeId, Name, Alias, RelativeOrder, IsActive | Dropdown values and reference data |
| `LookupType` | Name | Category grouping for lookups (e.g., "PaymentTypes") |
| `ContentType` | Name, MimeType | File extension to MIME type mapping |
| `DataRecorderMetaData` | DateCreated, DateModified, CreatedBy, ModifiedBy | Audit trail with user tracking and timestamps |

### DataRecorderMetaData Methods
| Method | Signature | Purpose |
|--------|-----------|---------|
| `GetClone()` | `DataRecorderMetaData GetClone()` | Create new metadata inheriting user context |
| `SetModifiedBy` | `void SetModifiedBy(long modifiedBy)` | Update modification timestamp using Clock.UtcNow |
| `SetModifiedBy` | `void SetModifiedBy(long modifiedBy, DateTime dateModified)` | Update modification metadata with explicit timestamp |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Issue: "Clock not registered in DI container" error
**Cause**: `DataRecorderMetaData` constructor tries to resolve `Clock` or `IClock` but DI is not initialized.
**Solution**: 
- Ensure `ApplicationManager.DependencyInjection` is configured before creating entities
- In unit tests, mock `ApplicationManager.DependencyInjection.Resolve<Clock>()` or use explicit constructor: `new DataRecorderMetaData(DateTime.UtcNow, userId)`

### Issue: Lookups not appearing in dropdown
**Cause**: `IsActive = false` or `LookupTypeId` mismatch with enum.
**Solution**: 
- Verify `IsActive = true` in database
- Check that `LookupTypeId` matches the enum value in `Core.Application.Enum.LookupTypes`

### Issue: File RelativeLocation not resolving to correct path
**Cause**: `ApplicationManager.Settings.MediaRootPath` might not be configured.
**Solution**: 
- Check app settings for `MediaRootPath` configuration
- Use `PathExtensions.ToFullPath()` to convert relative to absolute path
- Ensure `RelativeLocation` doesn't include the root path (should be relative)

### Issue: DateModified is null after update
**Cause**: `SetModifiedBy()` was not called before saving.
**Solution**: Always call `entity.DataRecorderMetaData.SetModifiedBy(userId)` before calling repository `Update()` or `Commit()`.

### Issue: ContentType table is empty
**Cause**: Missing database seed data.
**Solution**: Add migration or seed script to populate common MIME types:
```sql
INSERT INTO ContentType (Name, MimeType) VALUES 
  ('PDF', 'application/pdf'),
  ('JPEG', 'image/jpeg'),
  ('PNG', 'image/png'),
  ('XLSX', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
```
<!-- END CUSTOM SECTION -->
