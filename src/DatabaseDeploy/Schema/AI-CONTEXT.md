# DatabaseDeploy/Schema - AI Context

## Purpose

This folder contains the initial database schema creation scripts that define the complete database structure for the MarbleLife application.

## Contents

SQL scripts for creating:
- **Tables**: All entity tables with columns, data types, constraints
- **Primary Keys**: Identity columns and primary key constraints
- **Foreign Keys**: Relationships between tables
- **Indexes**: Performance optimization indexes
- **Stored Procedures**: Database logic (if any)
- **Views**: Predefined queries (if any)
- **Functions**: Reusable SQL functions (if any)

## Organization

Files are numbered sequentially (starting from 0001) and executed in order. The schema represents the baseline database structure.

## For AI Agents

**Table Creation Pattern**:
```sql
-- 0001_CreateCustomersTable.sql
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        Phone NVARCHAR(20) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    )
    
    CREATE UNIQUE INDEX IX_Customers_Email 
    ON Customers(Email) 
    WHERE IsDeleted = 0
END
GO
```

**Foreign Key Pattern**:
```sql
-- 0050_AddForeignKeys.sql
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = 'FK_Jobs_Customers'
)
BEGIN
    ALTER TABLE Jobs
    ADD CONSTRAINT FK_Jobs_Customers
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
END
GO
```

## For Human Developers

Schema scripts define the initial database structure. They execute before Data and Modifications scripts.

### Design Principles:
1. **Normalized Structure**: Follow 3NF where appropriate
2. **Soft Deletes**: Use IsDeleted flag instead of hard deletes
3. **Audit Columns**: Include CreatedDate, ModifiedDate, CreatedBy
4. **Indexed Lookups**: Add indexes on frequently queried columns
5. **Constraints**: Enforce data integrity at database level

### Common Patterns:

#### Base Entity Table:
```sql
CREATE TABLE EntityName (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    -- Entity-specific columns
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    -- Audit columns
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy INT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    -- Soft delete
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedDate DATETIME NULL,
    DeletedBy INT NULL
)
GO
```

#### Relationship Table (Many-to-Many):
```sql
CREATE TABLE CustomerTags (
    CustomerId INT NOT NULL,
    TagId INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_CustomerTags PRIMARY KEY (CustomerId, TagId),
    CONSTRAINT FK_CustomerTags_Customers 
        FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_CustomerTags_Tags 
        FOREIGN KEY (TagId) REFERENCES Tags(Id)
)
GO
```

### Best Practices:
- Use identity columns for primary keys
- Include check for existence before creating objects
- Use appropriate data types (NVARCHAR for Unicode, INT for IDs)
- Add indexes on foreign keys and frequently filtered columns
- Document complex tables with comments
- Use consistent naming conventions
- Group related tables in numbered sequence
- Consider query patterns when designing indexes
