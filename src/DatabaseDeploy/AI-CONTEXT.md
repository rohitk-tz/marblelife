<!-- AUTO-GENERATED: Header -->
# DatabaseDeploy Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:50:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is a **Custom Database Migration Tool**. It manages the schema and data state of the SQL Server database. It replaces standard Entity Framework Code First Migrations with a script-based approach.

### Workflow
1.  **Check State**: Checks if the database exists and has the Tracking Table.
2.  **Schema Rebuild** (Optional): If `RecreateDB=true`, it wipes the DB and runs scripts from `Schema/`.
3.  **Data Seed**: Runs scripts from `Data/` (Lookup data).
4.  **Incremental Updates**:
    -   Sorts `.sql` files in `Modifications/` alphabetically.
    -   Checks the last executed script in the Tracking DB.
    -   Runs only the new scripts.

### Folder Structure
-   `Schema/`: Base DB creation scripts (Tables, Constraints).
-   `Data/`: Static Reference Data (Enums, Lookups).
-   `Modifications/`: Incremental schema changes (Alter tables, New columns).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### DataFileSyncronizer
The main orchestrator.
-   `StartSync()`: Entry point.
-   `StartSync(folder, lastPoint)`: Iterates through SQL files and executes them using `DataExecutor`.

### DataExecutor
Helper class (assumed internal) that handles the actual ADO.NET connection and SQL execution logic, including splitting scripts by `GO` command if necessary.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### External
-   **System.Data.SqlClient**: Direct SQL interaction.
-   **SQL Server**: The target database.

### Internal
-   Logic here is independent of the rest of the application (Core/Infrastructure) to avoid circular dependencies or locking issues during deployment.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Operational Risk
-   **Destructive Action**: The tool can **WIPE** the database if `RecreateDB` is true in `App.config` or if the user selects 'Y' at the prompt. **Always check configs before running in Production.**
-   **File Ordering**: Scripts are ordered **Alphabetically**. Naming convention `001.Name.sql`, `002.Name.sql` is critical. If you name a file `10.sql` and `2.sql`, `10.sql` runs first (lexicographically)! Use `002`, `010`.

### Troubleshooting
-   **Script Failure**: If a script fails, the process stops. You must fix the SQL file and re-run. The tool resumes from the last *successfully* recorded script.
-   **Tracking Table**: The tool maintains a table (likely `SchemaVersions` or similar) to know where it left off.
<!-- END CUSTOM SECTION -->
