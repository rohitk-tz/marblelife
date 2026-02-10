<!-- AUTO-GENERATED: Header -->
# FranchiseeMigration — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
FranchiseeMigration is a one-time data migration tool that seeds the Marblelife database with initial franchisee organization data. It takes hardcoded franchisee configurations (names, locations, contact info, service offerings) and creates complete organization records including users, addresses, phone numbers, and service relationships.

### Design Patterns
- **Data Seeding Pattern**: Hardcoded collection of franchisee configurations for initial database population
- **Builder Pattern**: Fluent helpers (`CreateModel`, `Services`, `Number`) construct complex nested models
- **Batch Processing**: Iterates through collection and processes each franchisee sequentially
- **Dependency Injection**: Uses Core services for state resolution and persistence

### Data Flow
1. Entry via `Program.Main()` → Dependency registration
2. Resolve `FranchiseeFromFileCollection` to load franchisee definitions
3. Call `FranchiseesDetails()` to retrieve hardcoded collection (~50+ franchisees)
4. Resolve `FranchiseeMigrationService` from Core.Organizations
5. Call `ProcessRecords(collection)` to persist each franchisee
6. Service creates:
   - Organization entity (franchisee company)
   - Address records (physical location)
   - Phone numbers (office, cell, fax, toll-free)
   - User accounts (owner, managers)
   - Service relationships (which services this franchisee offers)
   - Fee profile (royalty calculation settings)

### Franchisee Data Structure
Each franchisee includes:
- **Identity**: Name, email, password (for login)
- **Owner**: First/last name
- **Location**: Address, city, state/province, zip, country
- **Contact**: Multiple phone numbers (office, cell, fax, toll-free)
- **Services**: Enabled service types (StoneLife, ColorSeal, etc.) with royalty flags
- **Fee Profile**: Optional custom royalty calculation settings
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### FranchiseeDetailsModel
```csharp
public class FranchiseeDetailsModel
{
    public string Name { get; set; }                // Franchisee business name
    public string Email { get; set; }               // Login email
    public string Password { get; set; }            // Initial password
    public string FirstName { get; set; }           // Owner first name
    public string LastName { get; set; }            // Owner last name
    public string Address1 { get; set; }            // Street address
    public string Address2 { get; set; }            // Suite/unit
    public string City { get; set; }
    public string Zip { get; set; }
    public string State { get; set; }               // State/Province name or abbreviation
    public string Country { get; set; }             // Country name (USA, CANADA, etc.)
    public List<UserEditModel> Users { get; set; } // Additional users
    public FeeProfileEditModel FeeProfile { get; set; }  // Custom royalty settings
    public List<FranchiseeServiceEditModel> Services { get; set; }  // Enabled services
    public List<PhoneEditModel> Phones { get; set; }  // Contact numbers
}
```

### FranchiseeServiceEditModel
```csharp
public class FranchiseeServiceEditModel
{
    public long ServiceTypeId { get; set; }         // Service type enum ID
    public bool CalculateRoyalty { get; set; }      // Include in royalty calculations
}
```

### PhoneEditModel
```csharp
public class PhoneEditModel
{
    public int PhoneType { get; set; }              // Office, Cell, Fax, TollFree
    public string PhoneNumber { get; set; }         // Phone number (varies by country)
}
```

### FeeProfileEditModel
```csharp
public class FeeProfileEditModel
{
    // Custom royalty calculation settings
    public decimal BaseRoyaltyPercentage { get; set; }
    public decimal AdFundPercentage { get; set; }
    // Additional fee structure fields
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `FranchiseeFromFileCollection.FranchiseesDetails()`
- **Input**: None (hardcoded data)
- **Output**: `IList<FranchiseeDetailsModel>` (50+ franchisee configurations)
- **Behavior**:
  - Loads all US states from database for address validation
  - Returns collection of franchisee models with complete setup data
  - Includes domestic (US) and international (Canada, Bahamas, South Africa, UAE) franchisees
- **Side-effects**: Database query for state lookup

### `FranchiseeMigrationService.ProcessRecords()` (Core.Organizations)
- **Input**: `IList<FranchiseeDetailsModel>` collection
- **Output**: void
- **Behavior**:
  - Iterates through each franchisee model
  - Creates Organization entity with type = Franchisee
  - Persists address, phone, user, and service relationships
  - Applies fee profile if specified
  - Handles state/country normalization
- **Side-effects**: Database writes for each franchisee (transactional)

### Helper Methods

#### `CreateModel(name, email, password, ...)`
- **Purpose**: Fluent builder for FranchiseeDetailsModel
- **Parameters**: 14+ parameters for all franchisee attributes
- **Output**: Populated FranchiseeDetailsModel

#### `Services(params Tuple<EnumServiceType, bool>[])`
- **Purpose**: Constructs service offering list with royalty flags
- **Example**: `Services((StoneLife, true), (ColorSeal, false))`

#### `Number(PhoneType type, string number)`
- **Purpose**: Creates PhoneEditModel for various phone types
- **Example**: `Number(PhoneType.Office, "6144420081")`

#### `FeeProfile1()` / `FeeProfile2()`
- **Purpose**: Predefined fee structures for different franchisee tiers
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Organizations](../../Core/Organizations/.context/CONTEXT.md)** — FranchiseeMigrationService, domain entities
- **[Core.Geo](../../Core/Geo/.context/CONTEXT.md)** — State/Country lookups and address validation
- **[Core.Users](../../Core/Users/.context/CONTEXT.md)** — User account creation
- **[Core.Application](../../Core/Application/.context/CONTEXT.md)** — IUnitOfWork, dependency injection
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration

### Database Tables Populated
- `Organization` — Franchisee companies
- `Address` — Physical locations
- `Phone` — Contact numbers
- `User` — Owner and staff accounts
- `FranchiseeService` — Service offering relationships
- `FeeProfile` — Custom royalty settings

### Geolocation Resolution
Uses Core.Geo services to map:
- State names → State IDs (e.g., "CALIFORNIA" → 5)
- Country names → Country IDs (e.g., "CANADA" → 2)
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### One-Time Migration Tool
**Important**: This application is designed to run **once** during initial system setup. Running it multiple times will create duplicate franchisee records. It is not idempotent.

**Safe Re-run Strategy**:
1. Drop and recreate database (use DatabaseDeploy)
2. Run FranchiseeMigration
3. Or add duplicate-check logic before creating organizations

### Geographic Coverage
**Domestic US Franchisees**: ~40 locations across states:
- California (5), Texas (4), Florida (3), Ohio (3)
- Arizona, Connecticut, Delaware, Georgia, Illinois, Maryland, Massachusetts, Michigan, New York, Pennsylvania, Tennessee, Utah, Virginia, Washington

**International Franchisees**: ~10 locations:
- **Canada**: Calgary, Vancouver, Toronto, York (4)
- **Bahamas**: Nassau (1)
- **Cayman Islands**: Grand Cayman (1)
- **South Africa**: Johannesburg-Pretoria (1)
- **United Arab Emirates**: Dubai (commented out, not active)

### Service Type Distribution
Most common service offerings:
- **StoneLife**: Enabled for all franchisees (marble/granite care)
- **ColorSeal**: ~90% of franchisees
- **Tilelok**: ~60% of franchisees
- **Vinylguard**: ~50% of franchisees
- **Enduracrete**: ~20% of franchisees (concrete sealing)
- **Specialty**: Fabricators, TileInstall, Wood, CarpetLife (select locations)

### Password Security Note
**Critical**: Hardcoded passwords in this migration tool should be:
1. Changed immediately after first login
2. Replaced with secure password generation in production
3. Never committed to source control in clear text

Example passwords: "Marblelife1", "Marblelife2", etc.

### State/Country Normalization
The code uses inconsistent naming:
- Some use abbreviations: "CA", "TX", "FL"
- Some use full names: "CALIFORNIA", "ARIZONA"
- Some use country names: "CANADA", "BAHAMAS"

The `FetchAllStataes()` method loads states for lookup, but country resolution may fail if names don't match database values exactly.

### Fee Profile Tiers
**FeeProfile1()**: Standard royalty structure (most franchisees)
**FeeProfile2()**: Premium structure (international locations like Bahamas, Cayman Islands, UAE)

International franchisees typically have different royalty calculations due to currency and tax considerations.

### Phone Number Formatting
Phone numbers are stored as-is without normalization:
- **US**: 10 digits (e.g., "6144420081")
- **Canada**: 10 digits (e.g., "6044567890")
- **International**: Country-specific formats (e.g., "011-971-4-3415658" for UAE)

Consider adding phone normalization logic for consistency.

### Commented-Out Franchisees
The UAE franchisee is commented out:
```csharp
//CreateModel("United Arab Emirates", "uae@marblelife.com", ...)
```
This may indicate inactive or pending locations. Uncomment and adjust as needed.

### Adding New Franchisees
To add franchisees after initial migration:
1. **Don't re-run this tool** (causes duplicates)
2. Use web UI or API to create new franchisees
3. Or create separate migration script with duplicate detection

### State Lookup Performance
`FetchAllStataes()` loads all states into memory:
```csharp
states = _stateRepository.Table.ToArray();
```
This is acceptable for ~50 states but inefficient if scaled. Consider caching in Core.Geo service.
<!-- END CUSTOM SECTION -->
