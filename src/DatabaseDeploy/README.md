<!-- AUTO-GENERATED: Header -->
# Database Deploy
> Custom SQL Migration Runner
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This console application manages database schema and data updates. It ensures that the database is synchronized with the latest SQL scripts in the repository.

It works in three stages:
1.  **Schema**: Initial table creation (only on fresh install/rebuild).
2.  **Data**: Static data seeding (Lookups, Countries, etc.).
3.  **Modifications**: Incremental updates (The most commonly used folder).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Running Migrations (Local Dev)
1.  Set `DatabaseDeploy` as the Startup Project.
2.  Check `App.config`:
    -   `RecreateDB`: Set to `false` for normal updates, `true` to **WIPE** and rebuild.
    -   `connectionStrings`: Ensure it points to your local SQL Server instance.
3.  Run (F5).
4.  If prompted, confirm if you want to rebuild.

### Creating a New Migration
1.  Create a new `.sql` file in `Modifications/`.
2.  **Naming Convention**: Use sequential numbering `XXX.Description.sql`.
    -   Example: `056.AddColumnToUsers.sql`
3.  Write idempotent SQL if possible (checks `IF NOT EXISTS`), although the tool tracks execution so it won't run twice.

<!-- END AUTO-GENERATED -->
