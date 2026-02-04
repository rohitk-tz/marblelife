# DatabaseDeploy/Modifications - AI Context

## Purpose

This folder contains database modification scripts for schema changes, data updates, and fixes applied after initial deployment.

## Contents

SQL scripts for:
- **Schema modifications**: ALTER TABLE, ADD COLUMN, CREATE INDEX
- **Data updates**: UPDATE statements, data corrections
- **Constraint changes**: ADD/DROP foreign keys, constraints
- **Performance optimizations**: Index creation, query tuning
- **Bug fixes**: Corrective data modifications

## Organization

Files are numbered sequentially (e.g., 0001_AddEmailIndexToCustomers.sql) in order of application. The numbering continues from the Schema folder's sequence.

## For AI Agents

**Modification Script Pattern**:
```sql
-- 0543_AddPhoneIndexToCustomers.sql
-- Add index on Phone column for faster customer lookups

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_Customers_Phone' 
    AND object_id = OBJECT_ID('Customers')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Customers_Phone
    ON Customers(Phone)
    INCLUDE (FirstName, LastName, Email)
END
GO
```

**Data Modification Pattern**:
```sql
-- 0544_UpdateServiceTypeCategories.sql
-- Standardize service type categories

BEGIN TRANSACTION

UPDATE ServiceTypes
SET Category = 'Primary Services'
WHERE Category IN ('Primary', 'Main', 'Core')

UPDATE ServiceTypes
SET Category = 'Maintenance Services'
WHERE Category IN ('Maintenance', 'Recurring', 'Upkeep')

COMMIT TRANSACTION
GO
```

## For Human Developers

Modifications are applied after Schema and Data scripts. They handle ongoing database evolution.

### Guidelines:
1. **One Change Per File**: Each file should make one logical change
2. **Idempotent**: Scripts should be safe to run multiple times
3. **Backwards Compatible**: Avoid breaking changes when possible
4. **Test in QA First**: Always test modifications before production
5. **Document Purpose**: Include comments explaining the change

### Common Modification Types:

#### Adding Columns:
```sql
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('Customers') 
    AND name = 'PreferredContactMethod'
)
BEGIN
    ALTER TABLE Customers
    ADD PreferredContactMethod NVARCHAR(50) NULL
END
GO
```

#### Adding Indexes:
```sql
CREATE NONCLUSTERED INDEX IX_Jobs_ScheduledDate
ON Jobs(ScheduledDate)
WHERE Status IN ('Scheduled', 'InProgress')
GO
```

#### Data Corrections:
```sql
-- Fix incorrect status values
UPDATE Jobs
SET Status = 'Completed'
WHERE Status = 'Complete'
  AND CompletedDate IS NOT NULL
GO
```

### Best Practices:
- Use descriptive file names indicating the change
- Check for existence before creating/modifying objects
- Use transactions for data modifications
- Add comments explaining why the change is needed
- Keep modifications small and focused
- Test rollback procedures
