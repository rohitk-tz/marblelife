<!-- AUTO-GENERATED: Header -->
# DatabaseDeploy — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
DatabaseDeploy is a schema management and migration utility that synchronizes MySQL database state with SQL scripts stored in the file system. It enforces sequential script execution, tracks migration history, and supports both full database recreation and incremental updates.

### Design Patterns
- **Sequential File Processing**: Scripts are executed in alphabetical order to ensure dependency resolution
- **Tracking Table Pattern**: Uses `DatabaseDeployTrackingInfo` table to track executed scripts and enable incremental migrations
- **Three-Folder Strategy**: Separates concerns into Schema (DDL), Data (seed data), and Modifications (migrations)
- **Transaction-per-Script**: Each SQL file executes within its own transaction for atomic rollback on failure

### Data Flow
1. Application entry via `Program.Main()`
2. `DataFileSyncronizer.StartSync()` determines database state (exists, has tracking, needs recreation)
3. For new databases: Execute Schema → Data → Modifications
4. For existing databases: Execute only new Modifications scripts since last tracked execution
5. `DataExecutor` handles MySQL connection, script execution, and tracking persistence
6. `Logger` writes execution details and errors to log files

### Migration Workflow
**Full Database Creation**:
```
Schema/*.sql → Create DB → Data/*.sql → Modifications/*.sql
```

**Incremental Updates**:
```
Check DatabaseDeployTrackingInfo → Execute new Modifications/*.sql only
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

> Console application with no explicit domain models; relies on file system structure.

**Configuration (App.config)**:
```xml
<connectionStrings>
  <add name="ConnectionString" connectionString="..." />
  <add name="DefaultString" connectionString="..." />
</connectionStrings>
<appSettings>
  <add key="DBName" value="Makalu" />
  <add key="RecreateDB" value="false" />  <!-- Force full recreation -->
</appSettings>
```

**Expected Folder Structure**:
```
DatabaseDeploy/
├── Schema/*.sql        # DDL scripts (CREATE TABLE, CREATE INDEX)
├── Data/*.sql          # Seed data (INSERT statements)
└── Modifications/*.sql # Migration scripts (ALTER TABLE, data fixes)
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `DataFileSyncronizer.StartSync()`
- **Input**: None (reads from App.config and file system)
- **Output**: void (exits on completion or error)
- **Behavior**: 
  - Checks if database exists
  - If `RecreateDB=true` or DB missing → Full recreation
  - If DB exists without tracking table → Prompts user (Y/N) to wipe
  - If DB exists with tracking → Executes only new Modifications scripts
- **Side-effects**: Creates/modifies database, writes logs, updates tracking table

### `DataExecutor.ExecuteScript(string filePath, string folderName)`
- **Input**: SQL file path, folder name (for tracking)
- **Output**: bool (success/failure)
- **Behavior**:
  - Reads SQL file content
  - Executes against MySQL database
  - Inserts tracking record on success
  - Rolls back transaction on failure
- **Side-effects**: Database schema/data changes, tracking table updates

### `Logger.LogInfo(string message)` / `Logger.LogError(string message, Exception ex)`
- **Input**: Log message, optional exception
- **Output**: void
- **Behavior**: Writes timestamped entries to log file
- **Side-effects**: Appends to log file in application directory
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **Core**: None (standalone console application)
- **Infrastructure**: None (direct MySQL.Data usage)

### External
- **MySql.Data** (v6.9.9.0) — MySQL database connectivity
- **System.Configuration** — App.config parsing
- **System.Data** — ADO.NET data access

### Infrastructure References
This console application operates independently of the main Core/Infrastructure modules. It uses raw ADO.NET with MySQL.Data for database operations.
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Script Naming Convention
Scripts must be named to enforce correct execution order:
```
Schema/001_CreateTables.sql
Schema/002_CreateIndexes.sql
Modifications/20240101_AddEmailColumn.sql
Modifications/20240115_MigrateUserData.sql
```

### Safety Mechanisms
- **Idempotency**: Modifications scripts should check for existence before altering
- **Rollback**: Each script runs in a transaction; failure stops the process
- **Tracking**: `DatabaseDeployTrackingInfo` prevents re-execution of completed scripts

### Common Pitfalls
1. **Non-idempotent scripts**: Always use `IF NOT EXISTS` or `IF EXISTS` checks
2. **Large data operations**: Break into smaller batches to avoid timeout
3. **Missing dependencies**: Ensure scripts reference only previously created objects
4. **Character encoding**: Use UTF-8 BOM for special characters

### Manual Intervention Scenarios
- **Corrupted tracking table**: May require manual cleanup or full recreation
- **Failed script mid-execution**: Review database state, fix script, mark as executed in tracking
- **Schema drift**: Production changes not in scripts require reconciliation
<!-- END CUSTOM SECTION -->
