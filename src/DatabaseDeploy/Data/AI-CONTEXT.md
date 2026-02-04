# DatabaseDeploy/Data - AI Context

## Purpose

This folder contains data seeding scripts that populate initial or reference data in the database after schema creation.

## Contents

SQL scripts for inserting:
- **Master data**: Reference tables, lookup values
- **Initial configuration**: System settings, default values
- **Sample data**: Test data for development (if applicable)
- **User accounts**: Default admin or system accounts

## Organization

Files are typically numbered sequentially (e.g., 001_SeedUserRoles.sql, 002_SeedServiceTypes.sql) to ensure proper execution order.

## For AI Agents

**Data Script Pattern**:
```sql
-- Check if data already exists
IF NOT EXISTS (SELECT 1 FROM ServiceTypes WHERE Name = 'Floor Restoration')
BEGIN
    INSERT INTO ServiceTypes (Name, Description, IsActive)
    VALUES ('Floor Restoration', 'Complete floor restoration service', 1)
END
GO
```

**Best Practices**:
- Use idempotent scripts (check before insert)
- Include GO statements to batch commands
- Use transactions for multiple inserts
- Document the purpose of each data file
- Avoid hardcoding IDs; use lookups where possible

## For Human Developers

Data scripts execute after schema creation. They should be:
- **Idempotent**: Safe to run multiple times
- **Ordered**: Use sequential numbering
- **Documented**: Include comments explaining data purpose
- **Environment-aware**: Consider dev vs. production data needs

### Example:
```sql
-- Seed default franchisee service types
BEGIN TRANSACTION

IF NOT EXISTS (SELECT 1 FROM ServiceTypes WHERE Code = 'RESTORE')
    INSERT INTO ServiceTypes (Code, Name, Category, IsActive)
    VALUES ('RESTORE', 'Restoration', 'Primary', 1)

IF NOT EXISTS (SELECT 1 FROM ServiceTypes WHERE Code = 'CLEAN')
    INSERT INTO ServiceTypes (Code, Name, Category, IsActive)
    VALUES ('CLEAN', 'Cleaning', 'Maintenance', 1)

COMMIT TRANSACTION
GO
```
