# Organizations/Impl
> Concrete implementations of services, factories, and validators

## Quick Reference

**24 Implementation Files**:
- **Services** (6): Business logic (FranchiseeInfoService, FranchiseeServiceFeeService, etc.)
- **Factories** (14): Domain ↔ ViewModel transformations
- **Validators** (3): FluentValidation rules for edit models
- **Utilities** (1): Migration service

## Major Components

### Services
- **FranchiseeInfoService** (~2,700 lines): Main franchisee CRUD, reporting, exports
- **FranchiseeServiceFeeService** (~1,900 lines): Fee calculations, billing integration
- **FranchiseeSalesService**: Sales data processing, royalty calculations
- **FranchiseeDocumentService**: Document upload and compliance reporting

### Factories
- Transform between domain entities and view models
- Bidirectional: CreateEditModel(), CreateDomain(), CreateViewModel()
- Orchestrate sub-factories for complex object graphs

### Validators
- FluentValidation-based business rules
- Validate edit models before persistence
- Enforce data integrity (uniqueness, ranges, required fields)

## Usage Pattern

```csharp
// Service usage
var service = container.Resolve<IFranchiseeInfoService>();
var editModel = service.Get(userId);
service.Save(editModel);

// Factory usage
var domain = _franchiseeFactory.CreateDomain(editModel, existingEntity);
var viewModel = _franchiseeFactory.CreateViewModel(domain);
```

See [CONTEXT.md](CONTEXT.md) for detailed implementation documentation.
