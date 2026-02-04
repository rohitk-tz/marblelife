# DatabaseDeploy Module - AI Context

## Purpose

The **DatabaseDeploy** module is the database schema deployment and migration management system for the MarbleLife application. It provides a file-based, sequential SQL script execution engine that handles database creation, schema initialization, data seeding, and incremental schema modifications. Built as a standalone console application, it ensures reliable, trackable, and repeatable database deployments across all environments (development, staging, production).

## Architecture Overview

The DatabaseDeploy module implements a **Sequential File-Based Migration** pattern with transactional safety and execution tracking. It processes SQL scripts from three designated folders in a specific order, tracks execution history in the database, and ensures idempotent deployments through checkpointing.

### Key Architectural Patterns

1. **Sequential Migration Pattern**: Numbered SQL files executed in order
2. **Checkpoint Pattern**: Tracks last executed script per folder to enable incremental updates
3. **Transaction Wrapper Pattern**: Each script executes in a transaction with automatic rollback on error
4. **File-Based Versioning**: Migration history maintained through file naming conventions
5. **Folder-Based Organization**: Schema, Data, and Modifications separated for clarity

## Project Structure

```
DatabaseDeploy/
├── Data/                           # Static seed data (executed once)
│   ├── 001a.CountryCityState.sql  # Geographic master data
│   ├── 001b.Zip.sql               # Zip code data
│   ├── 001c.CityZip.sql           # City-Zip mapping
│   ├── 002.LookUpAndOtherStaticData.sql # Lookup tables
│   ├── 003.InitialData.sql        # Business reference data
│   └── 004.EmailTemplate.sql      # Email templates
├── Impl/                           # Implementation classes
│   ├── DataExecutor.cs            # SQL script execution engine
│   ├── DataFileSyncronizer.cs     # Orchestrates deployment workflow
│   └── DatabaseTrack.cs           # Execution tracking model
├── Modifications/                  # Incremental schema changes (558+ files)
│   ├── 001.*.sql
│   ├── 002.*.sql
│   ├── ...
│   └── 558.*.sql
├── Schema/                         # Initial schema creation
│   ├── Schema.mwb                 # MySQL Workbench design file
│   ├── 001.CreateSchema.sql       # Database creation
│   ├── 002.CreateDbUser.sql       # User/permissions setup
│   ├── 003.Tables.sql             # Table definitions
│   └── 004.ProcedureAndRoutines.sql # Stored procedures
├── App.config                      # Configuration (connection strings, settings)
├── Program.cs                      # Console application entry point
├── Log.txt                         # Execution log file
└── Properties/                     # Assembly metadata
```

## Deployment Workflow

### Program.cs - Entry Point

**Purpose**: Initiates database deployment process.

```csharp
static void Main(string[] args)
{
    try
    {
        Console.WriteLine("=== MarbleLife Database Deployment ===");
        Console.WriteLine($"Started at: {DateTime.Now}");
        
        var synchronizer = new DataFileSyncronizer();
        synchronizer.Syncronize();
        
        Console.WriteLine($"Completed at: {DateTime.Now}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FATAL ERROR: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        File.AppendAllText("Log.txt", $"{DateTime.Now}: {ex}\n");
    }
}
```

### DataFileSyncronizer - Orchestration Engine

**Purpose**: Manages the overall deployment process, determines what to execute based on database state.

#### Key Methods

**Syncronize()**

Main orchestration method that determines deployment strategy:

```csharp
public void Syncronize()
{
    // Check if database exists
    bool databaseExists = CheckDatabaseExists();
    bool recreateDb = ConfigurationManager.AppSettings["RecreateDB"] == "true";
    
    if (!databaseExists || recreateDb)
    {
        // Full deployment: Schema + Data + Modifications
        ExecuteFolder("Schema");
        ExecuteFolder("Data");
        ExecuteFolder("Modifications");
    }
    else
    {
        // Incremental deployment: Only new modifications
        ExecuteFolder("Modifications");
    }
}
```

**ExecuteFolder(string folderName)**

Processes all SQL files in specified folder:

```csharp
private void ExecuteFolder(string folderName)
{
    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
    
    if (!Directory.Exists(folderPath))
    {
        Console.WriteLine($"Folder not found: {folderPath}");
        return;
    }
    
    // Get last executed file from tracking table
    string lastExecutedFile = _dataExecutor.GetLastExecutedScriptfortheFolder(folderName);
    
    // Get all SQL files, sorted alphabetically
    var files = Directory.GetFiles(folderPath, "*.sql")
        .OrderBy(f => f)
        .ToList();
    
    // Skip already-executed files
    bool startExecution = string.IsNullOrEmpty(lastExecutedFile);
    
    foreach (var file in files)
    {
        string fileName = Path.GetFileName(file);
        
        // Start executing from checkpoint
        if (!startExecution && fileName == lastExecutedFile)
        {
            startExecution = true;
            continue; // Skip the last executed file
        }
        
        if (startExecution)
        {
            Console.WriteLine($"Executing: {fileName}");
            _dataExecutor.ExecuteScript(file, folderName);
            Console.WriteLine($"  ✓ Success");
        }
    }
}
```

**CheckDatabaseExists()**

Verifies database presence:

```csharp
private bool CheckDatabaseExists()
{
    using (var connection = new MySqlConnection(_defaultConnectionString))
    {
        connection.Open();
        var command = new MySqlCommand("SHOW DATABASES LIKE 'Makalu'", connection);
        var result = command.ExecuteScalar();
        return result != null;
    }
}
```

### DataExecutor - SQL Execution Engine

**Purpose**: Executes individual SQL scripts with transaction safety and tracking.

#### Key Methods

**ExecuteScript(string filePath, string folderName)**

Executes SQL script within transaction:

```csharp
public void ExecuteScript(string filePath, string folderName)
{
    string fileName = Path.GetFileName(filePath);
    string sqlScript = File.ReadAllText(filePath);
    
    using (var connection = new MySqlConnection(_connectionString))
    {
        connection.Open();
        
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                // Execute SQL script
                var command = new MySqlCommand(sqlScript, connection, transaction);
                command.CommandTimeout = 600; // 10 minutes
                command.ExecuteNonQuery();
                
                // Update tracking table
                UpdateDatabaseTrack(folderName, fileName, connection, transaction);
                
                // Commit transaction
                transaction.Commit();
                
                LogSuccess(folderName, fileName);
            }
            catch (Exception ex)
            {
                // Rollback on error
                transaction.Rollback();
                LogError(folderName, fileName, ex);
                throw;
            }
        }
    }
}
```

**GetLastExecutedScriptfortheFolder(string folderName)**

Retrieves checkpoint from tracking table:

```csharp
public string GetLastExecutedScriptfortheFolder(string folderName)
{
    using (var connection = new MySqlConnection(_connectionString))
    {
        connection.Open();
        
        var query = @"SELECT LastExecutedFile 
                      FROM DatabaseTrack 
                      WHERE FolderName = @FolderName";
        
        var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@FolderName", folderName);
        
        var result = command.ExecuteScalar();
        return result?.ToString();
    }
}
```

**UpdateDatabaseTrack(string folderName, string fileName, MySqlConnection connection, MySqlTransaction transaction)**

Updates execution checkpoint:

```csharp
private void UpdateDatabaseTrack(string folderName, string fileName, MySqlConnection connection, MySqlTransaction transaction)
{
    var checkQuery = "SELECT COUNT(*) FROM DatabaseTrack WHERE FolderName = @FolderName";
    var checkCommand = new MySqlCommand(checkQuery, connection, transaction);
    checkCommand.Parameters.AddWithValue("@FolderName", folderName);
    
    bool exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
    
    string query;
    if (exists)
    {
        query = @"UPDATE DatabaseTrack 
                  SET LastExecutedFile = @FileName, 
                      LastExecutedOn = @ExecutedOn 
                  WHERE FolderName = @FolderName";
    }
    else
    {
        query = @"INSERT INTO DatabaseTrack (FolderName, LastExecutedFile, LastExecutedOn) 
                  VALUES (@FolderName, @FileName, @ExecutedOn)";
    }
    
    var command = new MySqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@FolderName", folderName);
    command.Parameters.AddWithValue("@FileName", fileName);
    command.Parameters.AddWithValue("@ExecutedOn", DateTime.UtcNow);
    command.ExecuteNonQuery();
}
```

## Folder Structure & Purpose

### Schema/ - Initial Database Creation

**Purpose**: Creates fresh database schema (only executed when database doesn't exist or RecreateDB=true).

**Files**:

1. **001.CreateSchema.sql** - Database creation
   ```sql
   CREATE DATABASE IF NOT EXISTS Makalu 
   CHARACTER SET utf8 
   COLLATE utf8_general_ci;
   ```

2. **002.CreateDbUser.sql** - User setup
   ```sql
   CREATE USER IF NOT EXISTS 'makalu_user'@'localhost' IDENTIFIED BY 'password';
   GRANT ALL PRIVILEGES ON Makalu.* TO 'makalu_user'@'localhost';
   FLUSH PRIVILEGES;
   ```

3. **003.Tables.sql** - Table definitions (60+ tables)
   ```sql
   CREATE TABLE Franchisee (
       Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
       Name VARCHAR(255) NOT NULL,
       IsActive BIT(1) DEFAULT 1,
       CreatedBy BIGINT,
       DateCreated DATETIME,
       ModifiedBy BIGINT,
       DateModified DATETIME
   );
   -- ... more tables
   ```

4. **004.ProcedureAndRoutines.sql** - Stored procedures
   ```sql
   DELIMITER $$
   CREATE PROCEDURE GetCustomersByFranchisee(IN franchiseeId BIGINT)
   BEGIN
       SELECT * FROM Customer WHERE FranchiseeId = franchiseeId AND IsDeleted = 0;
   END$$
   DELIMITER ;
   ```

**Schema.mwb**: MySQL Workbench visual schema design (source of truth).

### Data/ - Static Seed Data

**Purpose**: Populates lookup tables and reference data (only executed during fresh database creation).

**Files**:

1. **001a.CountryCityState.sql** - Geographic master data
   - Countries: USA, Canada, etc.
   - States/Provinces: All US states, Canadian provinces
   - Cities: Major cities

2. **001b.Zip.sql** - Zip code data
   - ~40,000 US zip codes with latitude/longitude

3. **001c.CityZip.sql** - City-Zip mapping
   - Many-to-many relationship data

4. **002.LookUpAndOtherStaticData.sql** - Lookup tables
   - PhoneType: Mobile, Home, Work, Fax
   - AddressType: Billing, Shipping, Service
   - ServiceType: Stone restoration, Countertop sealing, etc.
   - PaymentMethod: Cash, Check, Credit Card, ACH

5. **003.InitialData.sql** - Business reference data
   - Default franchisee
   - System admin user
   - Default roles and permissions
   - Email templates

6. **004.EmailTemplate.sql** - Email templates
   - Welcome email
   - Invoice notification
   - Payment reminder
   - Review request

### Modifications/ - Incremental Schema Changes

**Purpose**: Sequential schema modifications applied incrementally to existing databases.

**Naming Convention**: `[number].[description].sql`

**Examples**:

- `001.Add_IsActive_to_Customer.sql`
- `025.Create_MarketingLead_Table.sql`
- `100.Alter_Invoice_Add_DueDate.sql`
- `250.Create_Index_Customer_Email.sql`
- `425.Create_Perpetuity.sql`
- `558.Update_LateFee_Calculation.sql`

**Categories**:

1. **Table Creation**
   ```sql
   -- 025.Create_MarketingLead_Table.sql
   CREATE TABLE MarketingLead (
       Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
       FranchiseeId BIGINT NOT NULL,
       Source VARCHAR(100),
       Name VARCHAR(255),
       Email VARCHAR(255),
       Phone VARCHAR(50),
       Status ENUM('New', 'Contacted', 'Converted', 'Lost'),
       CreatedBy BIGINT,
       DateCreated DATETIME,
       FOREIGN KEY (FranchiseeId) REFERENCES Franchisee(Id)
   );
   ```

2. **Column Addition**
   ```sql
   -- 100.Alter_Invoice_Add_DueDate.sql
   ALTER TABLE Invoice 
   ADD COLUMN DueDate DATETIME NULL AFTER InvoiceDate;
   ```

3. **Index Creation**
   ```sql
   -- 250.Create_Index_Customer_Email.sql
   CREATE INDEX idx_customer_email ON Customer(Email);
   ```

4. **Data Migration**
   ```sql
   -- 300.Migrate_OldAddress_to_NewAddress.sql
   INSERT INTO Address (Street, City, State, Zip, CreatedBy, DateCreated)
   SELECT OldStreet, OldCity, OldState, OldZip, 1, NOW()
   FROM Customer
   WHERE OldStreet IS NOT NULL;
   
   UPDATE Customer c
   INNER JOIN Address a ON c.OldStreet = a.Street
   SET c.AddressId = a.Id;
   
   ALTER TABLE Customer 
   DROP COLUMN OldStreet,
   DROP COLUMN OldCity,
   DROP COLUMN OldState,
   DROP COLUMN OldZip;
   ```

5. **Foreign Key Management**
   ```sql
   -- 400.Add_ForeignKey_Job_Customer.sql
   ALTER TABLE Job
   ADD CONSTRAINT fk_job_customer
   FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
   ON DELETE RESTRICT;
   ```

## DatabaseTrack Table

**Purpose**: Tracks execution history to enable incremental deployments.

**Schema**:
```sql
CREATE TABLE DatabaseTrack (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FolderName VARCHAR(100) NOT NULL,
    LastExecutedFile VARCHAR(255) NOT NULL,
    LastExecutedOn DATETIME NOT NULL,
    UNIQUE KEY uk_foldername (FolderName)
);
```

**Sample Data**:
```
| Id | FolderName     | LastExecutedFile                      | LastExecutedOn        |
|----|----------------|---------------------------------------|-----------------------|
| 1  | Schema         | 004.ProcedureAndRoutines.sql          | 2024-01-15 10:30:00  |
| 2  | Data           | 004.EmailTemplate.sql                 | 2024-01-15 10:35:00  |
| 3  | Modifications  | 558.Update_LateFee_Calculation.sql    | 2024-01-20 14:20:00  |
```

**Checkpoint Logic**:
- On deployment, query `LastExecutedFile` for folder
- Skip all files up to and including last executed
- Execute remaining files sequentially
- Update checkpoint after each successful execution

## Configuration

### App.config

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <!-- Recreate database flag (use cautiously!) -->
    <add key="RecreateDB" value="false" />
    
    <!-- Logging level -->
    <add key="LogLevel" value="Info" />
  </appSettings>
  
  <connectionStrings>
    <!-- Bootstrap connection (no database selected) -->
    <add name="DefaultString" 
         connectionString="Server=localhost;Uid=root;Pwd=root123;CharSet=utf8;" 
         providerName="MySql.Data.MySqlClient" />
    
    <!-- Database-specific connection -->
    <add name="ConnectionString" 
         connectionString="Server=localhost;Database=Makalu;Uid=root;Pwd=root123;CharSet=utf8;Allow User Variables=True;" 
         providerName="MySql.Data.MySqlClient" />
  </connectionStrings>
</configuration>
```

**Key Settings**:
- `DefaultString`: Used to check if database exists, create database
- `ConnectionString`: Used to execute schema/data/modification scripts
- `RecreateDB`: When `true`, drops and recreates database (CAUTION: data loss!)

## Deployment Scenarios

### Fresh Database (Development)

```bash
# Set RecreateDB=false (database doesn't exist yet)
DatabaseDeploy.exe

# Output:
# Executing: Schema/001.CreateSchema.sql ✓
# Executing: Schema/002.CreateDbUser.sql ✓
# Executing: Schema/003.Tables.sql ✓
# Executing: Schema/004.ProcedureAndRoutines.sql ✓
# Executing: Data/001a.CountryCityState.sql ✓
# ...
# Executing: Modifications/001.*.sql ✓
# Executing: Modifications/002.*.sql ✓
# ...
# Executing: Modifications/558.*.sql ✓
```

### Incremental Update (Production)

```bash
# Developer adds new modification: 559.Add_CustomerNotes_Table.sql
# Production runs:
DatabaseDeploy.exe

# Output:
# Database exists, skipping Schema and Data folders
# Last executed: Modifications/558.Update_LateFee_Calculation.sql
# Executing: Modifications/559.Add_CustomerNotes_Table.sql ✓
```

### Database Refresh (Staging)

```xml
<!-- Set RecreateDB=true in App.config -->
<add key="RecreateDB" value="true" />
```

```bash
DatabaseDeploy.exe

# Output:
# Dropping existing database...
# Executing: Schema/001.CreateSchema.sql ✓
# ... (full re-creation)
```

## Error Handling & Logging

### Transaction Safety

Every script executes in a transaction:
- **Success**: Transaction commits, tracking table updated
- **Failure**: Transaction rolls back, error logged, execution stops

### Log.txt

All operations logged to `Log.txt`:

```
2024-01-20 14:15:30 - INFO: Starting deployment
2024-01-20 14:15:32 - INFO: Database exists: True
2024-01-20 14:15:32 - INFO: Executing Modifications folder
2024-01-20 14:15:33 - INFO: Last executed: 557.Update_EmailQueue.sql
2024-01-20 14:15:35 - SUCCESS: 558.Update_LateFee_Calculation.sql
2024-01-20 14:15:40 - ERROR: 559.Add_CustomerNotes_Table.sql
2024-01-20 14:15:40 - ERROR DETAILS: Syntax error near 'COLUMN' at line 5
```

### Error Recovery

1. Review `Log.txt` for error details
2. Fix SQL script causing error
3. Re-run `DatabaseDeploy.exe`
4. Script will resume from checkpoint (failed script will retry)

## For AI Agents

### Adding New Migration Script

1. **Determine Next Number**:
   ```bash
   # Check highest number in Modifications/
   ls Modifications/ | sort -n | tail -1
   # Output: 558.Update_LateFee_Calculation.sql
   # Next number: 559
   ```

2. **Create SQL File**:
   ```sql
   -- Modifications/559.Add_CustomerNotes_Table.sql
   
   CREATE TABLE CustomerNotes (
       Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
       CustomerId BIGINT NOT NULL,
       Note TEXT NOT NULL,
       CreatedBy BIGINT NOT NULL,
       DateCreated DATETIME NOT NULL,
       FOREIGN KEY (CustomerId) REFERENCES Customer(Id) ON DELETE CASCADE
   );
   
   CREATE INDEX idx_customernotes_customerid ON CustomerNotes(CustomerId);
   ```

3. **Test Locally**:
   ```bash
   DatabaseDeploy.exe
   # Verify script executes successfully
   ```

4. **Commit to Version Control**:
   ```bash
   git add Modifications/559.Add_CustomerNotes_Table.sql
   git commit -m "Add CustomerNotes table migration"
   ```

### Modifying Existing Tables

**Always create new modification script** (never edit existing scripts):

```sql
-- Modifications/560.Alter_Customer_Add_PreferredContactMethod.sql

ALTER TABLE Customer
ADD COLUMN PreferredContactMethod ENUM('Email', 'Phone', 'SMS') DEFAULT 'Email' AFTER Email;
```

### Data Migration Pattern

```sql
-- Modifications/561.Migrate_Customer_LegacyData.sql

-- Step 1: Add new column
ALTER TABLE Customer ADD COLUMN NewField VARCHAR(100);

-- Step 2: Migrate data
UPDATE Customer
SET NewField = CONCAT(OldField1, ' ', OldField2)
WHERE OldField1 IS NOT NULL;

-- Step 3: Drop old columns (optional, consider backup first)
-- ALTER TABLE Customer DROP COLUMN OldField1, DROP COLUMN OldField2;
```

### Best Practices

- **One logical change per script**: Easier to debug and rollback
- **Test in development first**: Never run untested scripts in production
- **Use descriptive names**: `559.Add_CustomerNotes_Table.sql` not `559.sql`
- **Comment complex logic**: Help future developers understand intent
- **Consider rollback**: Some changes are hard to reverse (DROP TABLE)
- **Backup before major changes**: Especially in production

## For Human Developers

### Running Deployment

#### Development
```bash
# First time setup
cd DatabaseDeploy/bin/Debug
DatabaseDeploy.exe

# Subsequent updates
DatabaseDeploy.exe
```

#### Staging/Production
```bash
# Backup database first
mysqldump -u root -p Makalu > backup_$(date +%Y%m%d).sql

# Deploy changes
DatabaseDeploy.exe

# Verify deployment
mysql -u root -p Makalu -e "SELECT * FROM DatabaseTrack ORDER BY Id DESC LIMIT 5;"
```

### Creating Complex Migrations

#### Adding Indexes for Performance
```sql
-- Modifications/562.Add_Performance_Indexes.sql

-- Improve customer search performance
CREATE INDEX idx_customer_name ON Customer(Name);
CREATE INDEX idx_customer_email ON Customer(Email);

-- Improve invoice queries
CREATE INDEX idx_invoice_franchisee_date ON Invoice(FranchiseeId, InvoiceDate);
CREATE INDEX idx_invoice_customer ON Invoice(CustomerId, InvoiceDate);

-- Improve job scheduling queries
CREATE INDEX idx_job_scheduled_date ON Job(ScheduledDate, FranchiseeId);
```

#### Refactoring Database Structure
```sql
-- Modifications/563.Refactor_Address_Normalization.sql

-- Step 1: Create normalized Address table
CREATE TABLE Address (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Street VARCHAR(255),
    City VARCHAR(100),
    State VARCHAR(50),
    Zip VARCHAR(20),
    Country VARCHAR(100) DEFAULT 'USA',
    CreatedBy BIGINT,
    DateCreated DATETIME
);

-- Step 2: Migrate data from Customer table
INSERT INTO Address (Street, City, State, Zip, CreatedBy, DateCreated)
SELECT DISTINCT Street, City, State, Zip, 1, NOW()
FROM Customer
WHERE Street IS NOT NULL;

-- Step 3: Link customers to addresses
ALTER TABLE Customer ADD COLUMN AddressId BIGINT;

UPDATE Customer c
INNER JOIN Address a ON c.Street = a.Street AND c.City = a.City AND c.State = a.State
SET c.AddressId = a.Id;

-- Step 4: Add foreign key
ALTER TABLE Customer
ADD CONSTRAINT fk_customer_address FOREIGN KEY (AddressId) REFERENCES Address(Id);

-- Step 5: Drop old columns (commented out for safety)
-- ALTER TABLE Customer DROP COLUMN Street, DROP COLUMN City, DROP COLUMN State, DROP COLUMN Zip;
```

### Troubleshooting

#### Script Fails Mid-Execution

**Problem**: Script executes partially then fails.

**Solution**:
1. Transaction rolls back automatically (no partial state)
2. Fix SQL syntax error in script
3. Re-run DatabaseDeploy.exe (will retry failed script)

#### Checkpoint Not Updating

**Problem**: Same scripts execute repeatedly.

**Solution**:
```sql
-- Check tracking table
SELECT * FROM DatabaseTrack;

-- Manually update if needed (use cautiously)
UPDATE DatabaseTrack 
SET LastExecutedFile = '558.Update_LateFee_Calculation.sql', 
    LastExecutedOn = NOW()
WHERE FolderName = 'Modifications';
```

#### Database Connection Issues

**Problem**: Cannot connect to database.

**Solution**:
- Verify MySQL server is running
- Check connection strings in App.config
- Verify user credentials
- Check firewall settings

## Architecture Relationship

```
DatabaseDeploy (Console Application)
    ↓ Reads
Configuration (App.config)
    ↓ Executes
SQL Scripts (Schema, Data, Modifications)
    ↓ Against
MySQL Database (Makalu)
    ↓ Tracks in
DatabaseTrack Table
    ↓ Used by
ORM Module (Entity Framework)
    ↓ Used by
Infrastructure Module (Repositories)
    ↓ Used by
Core/API/Jobs Modules
```

## Future Enhancements

- **Web UI**: Monitor deployment status via dashboard
- **Rollback Support**: Automated rollback scripts for each migration
- **Multi-Environment Config**: Separate config files for dev/staging/prod
- **Parallel Execution**: Run independent scripts in parallel
- **Schema Validation**: Compare EF model vs. actual database schema
- **Backup Integration**: Automatic backup before deployment
- **Notification**: Email/Slack notifications on deployment success/failure
- **Dry Run Mode**: Simulate deployment without executing
