# Organizations/Domain
> Entity classes defining the persistent data model for franchisee organizations, billing profiles, sales tracking, and service management

## Quick Reference

**36 Entity Files** organized into categories:
- Core: Organization, Franchisee, OrganizationRoleUser
- Billing: FeeProfile, RoyaltyFeeSlabs, LateFee, FranchiseeServiceFee, Loan entities
- Sales: FranchiseeSales, FranchiseeAccountCredit
- Services: FranchiseeServices, ServiceType
- Documents: FranchiseDocument, DocumentType
- Lead Performance: LeadPerformanceDetails, ReviewPush integration
- History/Audit: Registration history, notes, duration tracking

## Key Relationships

```
Organization (1:1) → Franchisee (shared primary key)
    Franchisee (1:1) → FeeProfile
        FeeProfile (1:N) → RoyaltyFeeSlabs
    Franchisee (1:N) → FranchiseeSales
    Franchisee (1:N) → FranchiseeServiceFee
    Franchisee (1:N) → FranchiseeServices
```

## Usage

These are EF Core entities mapped to database tables. Always access via repositories, never instantiate directly in UI/API code. Use factories for transformations to/from view models.

See [CONTEXT.md](CONTEXT.md) for detailed entity definitions and relationships.
