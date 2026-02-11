<!-- AUTO-GENERATED: Header -->
# DatabaseDeploy
> MySQL schema migration utility with tracking and incremental update support
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
DatabaseDeploy is the database schema synchronization tool for the Marblelife system. Think of it as a version control system for your database — it keeps SQL scripts organized in folders, executes them in order, and remembers which ones have already run. This ensures development, staging, and production databases stay in sync without manual intervention or duplicate executions.

Unlike full ORM migrations (Entity Framework, Flyway), DatabaseDeploy uses a simple folder-based approach where SQL scripts are executed sequentially. The `Schema/` folder creates the initial structure, `Data/` loads seed information, and `Modifications/` handles incremental changes over time.

**Key Benefits**:
- **Idempotent deployments**: Safe to run multiple times; only new scripts execute
- **Auditability**: Every change tracked in `DatabaseDeployTrackingInfo` table
- **Simplicity**: Plain SQL files, no framework-specific syntax
- **Rollback safety**: Each script runs in a transaction
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Prerequisites
1. MySQL server running (version 5.7+)
2. .NET Framework 4.5.2+
3. Database connection credentials

### Configuration
Edit `App.config` before running:

```xml
<connectionStrings>
  <add name="ConnectionString" 
       connectionString="server=localhost; database=Makalu; User ID=root; Password=Pass@123; Port=3308;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
<appSettings>
  <add key="DBName" value="Makalu" />
  <add key="RecreateDB" value="false" />  <!-- Set to true to drop/recreate -->
</appSettings>
```

### Running the Tool

#### First-time Database Creation
```bash
# Set RecreateDB=false in App.config
DatabaseDeploy.exe
# Executes: Schema → Data → Modifications
```

#### Incremental Updates (Existing Database)
```bash
# Add new script: Modifications/20250210_AddCustomerIndex.sql
DatabaseDeploy.exe
# Only executes new scripts since last run
```

#### Force Full Rebuild
```bash
# Set RecreateDB=true in App.config
DatabaseDeploy.exe
# WARNING: Drops entire database and recreates from scratch
```

### Adding Migration Scripts

**Schema Changes (Modifications folder)**:
```sql
-- Modifications/20250210_AddEmailColumn.sql
ALTER TABLE Customer 
ADD COLUMN IF NOT EXISTS Email VARCHAR(255) NULL;

-- Always check existence to prevent re-run errors
```

**Data Migrations**:
```sql
-- Modifications/20250211_MigratePhoneFormat.sql
UPDATE Customer 
SET PhoneNumber = CONCAT('+1', PhoneNumber)
WHERE CountryCode = 'US' AND PhoneNumber NOT LIKE '+%';
```

### Console Output
```
Starting Service!
Database Found!
Starting Folder Modifications
Executing 20250210_AddEmailColumn.sql
Executing 20250211_MigratePhoneFormat.sql
Hit <Enter> to exit.....
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Class/Method | Description |
|--------------|-------------|
| `Program.Main()` | Entry point; invokes DataFileSyncronizer |
| `DataFileSyncronizer.StartSync()` | Orchestrates migration logic (create vs. update) |
| `DataExecutor.ExecuteScript()` | Executes single SQL file within transaction |
| `DataExecutor.DatabaseExists()` | Checks if target database exists |
| `DataExecutor.TrackingTableExists()` | Verifies `DatabaseDeployTrackingInfo` presence |
| `Logger.LogInfo()` | Writes informational messages to log |
| `Logger.LogError()` | Writes errors with stack traces to log |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Database is missing Tracking Information"
**Cause**: Database exists but doesn't have `DatabaseDeployTrackingInfo` table.  
**Solution**: Choose (Y) to wipe and recreate, or manually add tracking table if data must be preserved.

### "EXCEPTION: Access denied for user"
**Cause**: Incorrect credentials in App.config.  
**Solution**: Verify `ConnectionString` matches MySQL server configuration.

### "Script already executed"
**Cause**: Attempting to re-run a script that's in the tracking table.  
**Solution**: 
- Rename script with new timestamp
- Or delete from `DatabaseDeployTrackingInfo` (dangerous)
- Or set `RecreateDB=true` for full rebuild

### "Timeout expired"
**Cause**: Large data migration script exceeds default timeout (30 seconds).  
**Solution**: 
- Break into smaller batches
- Increase `CommandTimeout` in `DataExecutor.cs`

### Script fails mid-execution
**Cause**: Syntax error, constraint violation, or dependency issue.  
**Solution**:
1. Check log file for error details
2. Fix the SQL script
3. Manually verify database state
4. Re-run (failed scripts don't get tracked)

### Production deployment checklist
- [ ] Test all scripts in development environment
- [ ] Backup production database
- [ ] Schedule maintenance window (for large data migrations)
- [ ] Set `RecreateDB=false` (never drop production!)
- [ ] Run during low-traffic period
- [ ] Monitor logs for errors
- [ ] Verify application connectivity post-deployment
<!-- END CUSTOM SECTION -->
