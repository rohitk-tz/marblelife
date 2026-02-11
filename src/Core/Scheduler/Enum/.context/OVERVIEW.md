<!-- AUTO-GENERATED: Header -->
# Enumerations
> Strongly-typed enums for workflow states, calendar types, and system behaviors
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This folder contains all enumeration types used throughout the scheduler module. Think of enums as the "valid values" dictionary for the system - they define what states a job can be in, what types of calendar entries exist, how customers can respond to confirmations, etc.

**Why Separate Enum Folder?**
- **Prevents Circular Dependencies**: Domain entities, ViewModels, and Services all reference enums - keeping them separate avoids circular references
- **Centralized Validation**: Single source of truth for valid values
- **Database Synchronization**: Enum values match Lookup table IDs for foreign key integrity

**Key Enums:**
- `JobStatusType`: Job lifecycle (Created → Assigned → InProgress → Completed)
- `ScheduleType`: Calendar entry types (Job, Estimate, Vacation, Meeting)
- `ConfirmationEnum`: Customer confirmation responses
- `BeforeAfterImagesType`: Photo categorization
- `TechnicianWorkOrderInvoiceType`: Materials/supplies for invoicing

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Basic Enum Usage
```csharp
using Core.Scheduler.Enum;

// Set job status
job.StatusId = (long)JobStatusType.InProgress;

// Query by status
var completedJobs = jobs.Where(j => j.StatusId == (long)JobStatusType.Completed);

// Switch on enum
switch ((JobStatusType)job.StatusId)
{
    case JobStatusType.Created:
        Console.WriteLine("Job awaiting assignment");
        break;
    case JobStatusType.InProgress:
        Console.WriteLine("Work in progress");
        break;
    // ...
}
```

### Validation
```csharp
public bool IsValidStatusTransition(JobStatusType from, JobStatusType to)
{
    // Business rule: Can't skip from Created directly to Completed
    if (from == JobStatusType.Created && to == JobStatusType.Completed)
        return false;
    
    // Business rule: Canceled is terminal state
    if (from == JobStatusType.Canceled)
        return false;
    
    return true;
}
```

### Schedule Type Filtering
```csharp
// Get only actual jobs (exclude estimates, meetings, vacations)
var jobSchedules = _jobSchedulerRepository.GetAll()
    .Where(js => js.SchedulerStatus == (long)ScheduleType.Job)
    .ToList();

// Get technician vacations
var vacations = _jobSchedulerRepository.GetAll()
    .Where(js => js.SchedulerStatus == (long)ScheduleType.Vacation)
    .Where(js => js.IsVacation == true)
    .ToList();
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## Enum Summary

| Enum | Value Range | Purpose |
|------|-------------|---------|
| `JobStatusType` | 1-6 | Job workflow states |
| `ScheduleType` | 145-149 | Calendar entry classification |
| `ConfirmationEnum` | 2-5, 216-218 | Customer confirmation responses |
| `RepeatFrequency` | 181-184 | Recurring schedule patterns |
| `CalendarStatusType` | 1-2 | Event confirmation status |
| `BeforeAfterImagesType` | 203-207 | Photo categorization |
| `TechnicianWorkOrderInvoiceType` | 269-281 | Materials/supplies for billing |
| `SignatureType` | 289-290 | Signature capture timing |
| `DragDropSchedulerEnum` | 1-3 | Drag-drop operation results |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### Enum Value Mismatch Errors
**Problem**: Foreign key constraint violation when saving Job/JobScheduler.

**Cause**: Enum value doesn't exist in Lookup table.

**Solution**: Verify Lookup table has matching entry:
```sql
SELECT * FROM Lookup WHERE Id = 145 AND Type = 'ScheduleType';
-- Should return: (145, 'ScheduleType', 'Job', ...)
```

### Invalid Cast Exception
**Problem**: `InvalidCastException` when converting `long` to enum.

**Cause**: Database contains value not defined in enum.

**Solution**: Check for orphaned/invalid IDs:
```csharp
var invalidJobs = _jobRepository.GetAll()
    .Where(j => !Enum.IsDefined(typeof(JobStatusType), (int)j.StatusId))
    .ToList();
```

<!-- END CUSTOM SECTION -->
