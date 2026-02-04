# DatabaseDeploy/Impl - AI Context

## Purpose

This folder contains implementation classes for the DatabaseDeploy tool, including the deployment engine, script execution logic, and checkpoint tracking.

## Contents

Implementation classes:
- **DeploymentEngine**: Main orchestration logic
- **ScriptExecutor**: SQL script execution
- **CheckpointManager**: Tracks applied migrations
- **FileReader**: Reads SQL files
- **Logger**: Deployment logging
- **ConnectionManager**: Database connection handling

## For AI Agents

**Deployment Flow**:
```csharp
public class DeploymentEngine
{
    public void Deploy()
    {
        // 1. Connect to database
        // 2. Read checkpoint (last applied script)
        // 3. Get pending scripts (Schema, Data, Modifications)
        // 4. Execute scripts in order
        // 5. Update checkpoint
        // 6. Log results
    }
}
```

**Script Execution Pattern**:
```csharp
public class ScriptExecutor
{
    public void ExecuteScript(string scriptPath)
    {
        var sql = File.ReadAllText(scriptPath);
        var batches = sql.Split(new[] { "GO" }, 
            StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var batch in batches)
        {
            _connection.Execute(batch);
        }
        
        _checkpointManager.UpdateCheckpoint(scriptPath);
    }
}
```

## For Human Developers

The DatabaseDeploy tool implements a file-based migration system:

### Key Components:
1. **DeploymentEngine**: Orchestrates the entire deployment
2. **ScriptExecutor**: Handles SQL execution with GO statement parsing
3. **CheckpointManager**: Maintains migration history in database
4. **Error Handling**: Rolls back on failure, logs errors

### Configuration:
```xml
<appSettings>
    <add key="ConnectionString" value="..." />
    <add key="RecreateDB" value="false" />
    <add key="LogPath" value="logs/" />
</appSettings>
```

### Usage:
```bash
DatabaseDeploy.exe
# Reads scripts from Schema/, Data/, Modifications/
# Applies pending scripts
# Updates checkpoint
```

**Best Practices**:
- Always backup database before deployment
- Test migrations in QA environment first
- Monitor checkpoint table for migration history
- Review logs after deployment
- Use transactions where appropriate
