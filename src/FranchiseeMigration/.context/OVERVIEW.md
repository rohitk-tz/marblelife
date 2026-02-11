<!-- AUTO-GENERATED: Header -->
# FranchiseeMigration
> One-time data seeding tool for populating initial franchisee organization data
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
FranchiseeMigration is a specialized utility that bootstraps the Marblelife franchisee network by creating complete organization records from hardcoded configurations. It's the first step in setting up a new Marblelife installation, populating the database with 50+ franchisee locations across North America and international markets.

Think of it as an organizational chart importer that doesn't just create names — it sets up complete business entities with addresses, contact information, user accounts, service capabilities, and financial structures. Each franchisee becomes a fully functional node in the system, ready to manage customers and process sales.

**Critical Note**: This is a **one-time migration tool**. Running it multiple times creates duplicate franchisees. Use only during initial database setup, not for ongoing franchisee management.

**What Gets Created**:
- 50+ franchisee organizations (US, Canada, international)
- Physical addresses with geocoding
- Multiple contact numbers (office, cell, fax, toll-free)
- Owner user accounts with login credentials
- Service type relationships (which services each franchisee offers)
- Fee profiles (royalty calculation settings)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Prerequisites
1. .NET Framework 4.5.2+
2. MySQL database with schema created (run DatabaseDeploy first)
3. Core.Organizations services configured

### Initial Setup Sequence
```bash
# 1. Create database schema
DatabaseDeploy.exe

# 2. Seed franchisee data (THIS TOOL)
FranchiseeMigration.exe

# 3. Verify franchisees created
# Check Organization table: SELECT * FROM Organization WHERE OrganizationTypeId = 2
```

### Running the Migration

**First-Time Database Setup**:
```bash
FranchiseeMigration.exe
# Creates ~50 franchisee organizations
# Output: Logs each franchisee creation
```

**Expected Console Output**:
```
Creating franchisee: Columbus
Creating franchisee: Arizona  
Creating franchisee: Los Angeles - South
...
Migration completed: 52 franchisees created
```

### Franchisee Data Structure

Each franchisee includes:

**Identity**:
- Name: "Columbus", "Arizona", "Los Angeles - South"
- Email: columbus@marblelife.com
- Password: Marblelife1 (change after first login!)

**Location**:
- Address: "6441 Commerce Park Dr.", "Suite B"
- City, State, Zip: "Columbus, OH, 43231"
- Country: USA, CANADA, BAHAMAS, CAYMAN ISLANDS, SOUTH AFRICA

**Contact**:
- Office phone: Primary business line
- Cell phone: Mobile contact
- Fax: Document transmission
- Toll-free: Customer service (select locations)

**Services** (examples):
- **StoneLife**: Marble/granite restoration
- **ColorSeal**: Tile grout coloring
- **Tilelok**: Grout sealing
- **Vinylguard**: Vinyl floor protection
- **Enduracrete**: Concrete sealing
- **CleanShield**: Surface protection
- **Wood**: Wood floor care
- **CarpetLife**: Carpet cleaning
- **Fabricators**: Stone fabrication

**Fee Profile**:
- Standard: Default royalty structure
- Premium: International locations (Bahamas, Cayman Islands)

### Service Offering Examples

**Full-Service Franchisee** (Seattle-Tacoma):
```
✓ StoneLife
✓ ColorSeal
✓ Tilelok
✓ Vinylguard
✓ Fabricators
✓ MetalLife
✓ TileInstall
```

**Basic Franchisee** (Connecticut):
```
✓ StoneLife
✓ ColorSeal
✓ Tilelok
```

### Geographic Distribution

**United States**:
- **California**: 5 regions (Fresno, LA North/South, Sacramento, San Diego, Orange County)
- **Texas**: 4 regions (Austin, Dallas North/South, Houston West/Southeast)
- **Florida**: 3 regions (Central, SW, Tampa Bay)
- **Ohio**: 3 regions (Columbus, Cincinnati, Cleveland)

**Canada**:
- Calgary, Vancouver, Toronto, York

**International**:
- Bahamas (Nassau)
- Cayman Islands (Grand Cayman)
- South Africa (Johannesburg-Pretoria)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `FranchiseesDetails()` | Returns hardcoded collection of 50+ franchisee configurations |
| `FetchAllStataes()` | Loads US states from database for address validation |
| `CreateModel()` | Builder method for constructing franchisee models with all attributes |
| `Services()` | Constructs service offering list with royalty calculation flags |
| `Number()` | Creates phone number model with type (Office, Cell, Fax, TollFree) |
| `FeeProfile1/2()` | Returns predefined fee structures (standard vs. premium) |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Duplicate franchisee" Error
**Cause**: Migration ran multiple times on same database.  
**Solution**:
- **Option 1**: Drop and recreate database, re-run full setup sequence
- **Option 2**: Manually delete duplicate records from `Organization` table
- **Prevention**: Add duplicate-check logic to code before creating organizations

### "State not found" Error
**Cause**: State name/abbreviation doesn't match database values.  
**Solution**:
- Verify `State` table populated (from DatabaseDeploy data scripts)
- Check state naming convention in code vs. database
- Update hardcoded state values to match database format

### "Country not found" Error
**Cause**: Country name doesn't match database values.  
**Solution**:
- Verify `Country` table contains: "USA", "CANADA", "BAHAMAS", "CAYMAN ISLANDS", "SOUTH AFRICA"
- Update country names in FranchiseeFromFileCollection to match database
- Or create missing country records manually

### "Service type not found" Error
**Cause**: Service type enum ID doesn't match database.  
**Solution**:
- Verify `ServiceType` table populated with all service types
- Check enum values in `Core.Organizations.Enum.ServiceType`
- Ensure enum IDs match database IDs

### Passwords Not Working
**Cause**: Hardcoded passwords may not meet security requirements.  
**Solution**:
- Update password hashing logic if encryption changed
- Reset passwords via web UI after migration
- Update `CreateModel` calls with stronger passwords

### Missing Phone Numbers
**Cause**: Some franchisees have null/empty phone numbers in code.  
**Solution**:
- Update hardcoded data with correct phone numbers
- Or make phone numbers optional in validation logic

### International Franchisee Issues
**Cause**: Currency, time zone, or address format problems.  
**Solution**:
- Ensure `Country` table has correct currency codes
- Verify `CurrencyExchangeRate` table populated (run ConsoleApplication1)
- Check address validation accepts international formats

### Fee Profile Not Applied
**Cause**: Custom fee profile not saved properly.  
**Solution**:
- Verify `FeeProfile` table and relationships exist
- Check FranchiseeMigrationService persists fee profile
- Manually set fee profile via web UI if needed

### Adding New Franchisees After Migration

**Method 1: Web UI** (Recommended):
1. Login as administrator
2. Navigate to Franchisee Management
3. Click "Add New Franchisee"
4. Fill in all required fields
5. Save

**Method 2: Custom Migration Script**:
```csharp
// Create separate migration for new franchisees with duplicate detection
var existing = _organizationRepository.Get(x => x.Email == "newfran@marblelife.com");
if (existing == null)
{
    // Create new franchisee
}
```

### Modifying Existing Franchisee Data

**Don't re-run migration!** Instead:

1. **Change Address/Phone**:
   - Update via web UI
   - Or write SQL script: `UPDATE Address SET ... WHERE OrganizationId = ?`

2. **Add/Remove Services**:
   - Update via web UI
   - Or manage `FranchiseeService` table directly

3. **Change Fee Profile**:
   - Update via web UI
   - Or modify `FeeProfile` table

### Verifying Migration Success

**SQL Queries**:
```sql
-- Count franchisees created
SELECT COUNT(*) FROM Organization WHERE OrganizationTypeId = 2;

-- List all franchisees with locations
SELECT o.Name, a.City, a.StateId, a.CountryId
FROM Organization o
LEFT JOIN Address a ON o.Id = a.OrganizationId
WHERE o.OrganizationTypeId = 2;

-- Check service offerings
SELECT o.Name, COUNT(fs.ServiceTypeId) AS ServiceCount
FROM Organization o
LEFT JOIN FranchiseeService fs ON o.Id = fs.FranchiseeId
WHERE o.OrganizationTypeId = 2
GROUP BY o.Name;

-- Verify user accounts created
SELECT o.Name, u.Email
FROM Organization o
INNER JOIN User u ON o.Id = u.OrganizationId
WHERE o.OrganizationTypeId = 2;
```

### Best Practices for Initial Setup

1. **Test in Development First**:
   - Run full setup sequence in dev environment
   - Verify all franchisees created correctly
   - Test login with sample franchisee credentials

2. **Backup Before Production**:
   - Create database backup before migration
   - Document any customizations to franchisee data

3. **Update Passwords Immediately**:
   - Change all default passwords after migration
   - Implement password reset workflow

4. **Verify Integrations**:
   - Test QuickBooks integration for each franchisee
   - Verify email notifications work
   - Check reporting and analytics

5. **Document Customizations**:
   - If modifying franchisee data, update FranchiseeFromFileCollection
   - Comment out inactive franchisees (like UAE example)
   - Keep geographic distribution documented
<!-- END CUSTOM SECTION -->
