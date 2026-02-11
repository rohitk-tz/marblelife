# Organizations/ViewModel
> Data Transfer Objects (DTOs) for API/UI communication

## Quick Reference

**58 DTO Files** organized by type:
- **Edit Models** (~20): Form data for create/update operations
- **View Models** (~20): Read-only display data
- **Filter Models** (~6): Search/query parameters
- **List Models** (~6): Paginated response containers
- **Specialized** (~6): Domain-specific DTOs

## Model Categories

### Edit Models (Suffix: EditModel)
Used for forms, validated by FluentValidation:
- FranchiseeEditModel (main form)
- FeeProfileEditModel, RoyaltyFeeSlabsEditModel
- FranchiseeSalesEditModel, FranchiseeServiceFeeEditModel
- DocumentEditModel, etc.

### View Models (Suffix: ViewModel)
Read-only display data with `[NoValidatorRequired]`:
- FranchiseeViewModel, FeeProfileViewModel
- FranchiseeSalesViewModel, etc.

### List Models (Suffix: ListModel)
Paginated response containers:
```csharp
{
    Collection: [ ViewModel, ... ],
    Filter: { applied filters },
    PagingModel: { page, size, totalCount }
}
```

### Filter Models (Suffix: Filter)
Query parameters for search/sort:
- FranchiseeListFilter, DocumentListFilter, etc.

## Usage Pattern

```csharp
// Create/Update
var editModel = new FranchiseeEditModel { ... };
service.Save(editModel);

// List/Search
var filter = new FranchiseeListFilter { Text = "Dallas" };
var result = service.GetFranchiseeCollection(filter, page: 1, size: 25);
// Returns: FranchiseeListModel with Collection, Filter, PagingModel

// Display
FranchiseeViewModel viewModel = result.Collection.First();
```

See [CONTEXT.md](CONTEXT.md) for detailed model structures and validation rules.
