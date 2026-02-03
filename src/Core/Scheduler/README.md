<!-- AUTO-GENERATED: Header -->
# Core.Scheduler
> Field Service & Calendar Management
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module manages the operational side of the business.
**Estimates -> Scheduling -> Jobs -> Execution.**

It handles:
-   **Quoting**: Creating estimates for customers.
-   **Dispatch**: Assigning jobs to technicians.
-   **Calendar**: Managing the schedule visualizer.
-   **Field Data**: Configuring "Before/After" photos and work orders.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Creating a Job
Usually starts from an Estimate.
```csharp
var estimate = estimateService.Get(id);
var job = jobService.CreateFromEstimate(estimate);
```

### Scheduling
```csharp
var appointment = new JobScheduler {
    JobId = job.Id,
    StartTime = DateTime.UtcNow,
    EndTime = DateTime.UtcNow.AddHours(2)
};
schedulerService.Schedule(appointment);
```

<!-- END AUTO-GENERATED -->
