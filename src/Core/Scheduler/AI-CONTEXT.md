# Core/Scheduler - AI Context

## Purpose

The **Scheduler** module manages job scheduling, appointments, technician assignments, and calendar management for the MarbleLife service operations. This is the operational heart of the platform, coordinating service delivery.

## Key Entities (Domain/)

### Job Management
- **Job**: Core job/appointment entity with service details
- **JobItem**: Individual work items within a job
- **JobStatus**: Job lifecycle tracking
- **JobNote**: Internal notes and customer communications
- **JobImage**: Before/after photos and documentation

### Scheduling
- **Schedule**: Master schedule entries
- **ScheduleBlock**: Time blocks and availability
- **Appointment**: Customer-facing appointment slots
- **RecurringSchedule**: Recurring job patterns
- **ScheduleException**: Holiday and special date handling

### Technician Management
- **TechnicianSchedule**: Technician availability and assignments
- **TechnicianRoute**: Optimized routing for multiple jobs
- **TechnicianCapacity**: Workload and capacity planning
- **TimeOffRequest**: PTO and absence management

### Equipment & Resources
- **Equipment**: Tools and machinery tracking
- **EquipmentSchedule**: Equipment availability
- **MaterialRequirement**: Job material needs
- **VehicleSchedule**: Company vehicle assignments

## Service Interfaces

### Job Services
- **IJobFactory**: Job creation with validation
- **IJobService**: Job lifecycle management (create, assign, complete, cancel)
- **IJobItemFactory**: Work item breakdown
- **IJobNoteService**: Communication and notes
- **IJobImageService**: Photo upload and management

### Scheduling Services
- **IScheduleFactory**: Schedule entry creation
- **IScheduleService**: Schedule CRUD and conflict detection
- **IAppointmentService**: Customer appointment booking
- **IRecurringScheduleService**: Recurring job automation
- **IScheduleOptimizationService**: Route and time optimization

### Technician Services
- **ITechnicianScheduleService**: Technician assignment and availability
- **ITechnicianRouteService**: Route planning and optimization
- **ITimeOffService**: PTO request management
- **ICapacityPlanningService**: Workload balancing

### Resource Services
- **IEquipmentService**: Equipment allocation
- **IMaterialService**: Material inventory and requirements
- **IVehicleScheduleService**: Vehicle dispatch

## Implementations (Impl/)

Business logic including:
- Schedule conflict detection and resolution
- Route optimization algorithms
- Capacity planning calculations
- Appointment reminder automation
- Job status workflow transitions
- Real-time schedule updates

## Enumerations (Enum/)

- **JobStatus**: Scheduled, InProgress, Completed, Cancelled, OnHold, Rescheduled
- **AppointmentType**: Initial, Recurring, FollowUp, Emergency
- **Priority**: Low, Normal, High, Urgent
- **TechnicianStatus**: Available, OnJob, Break, OffDuty, Sick
- **ScheduleConflictType**: DoubleBooked, Overlap, OutOfHours, Capacity
- **RecurrencePattern**: Daily, Weekly, BiWeekly, Monthly, Quarterly, Annually

## ViewModels (ViewModel/)

- **JobViewModel**: Complete job data with items
- **ScheduleViewModel**: Schedule entry with customer info
- **AppointmentViewModel**: Booking information
- **TechnicianScheduleViewModel**: Daily technician schedule
- **JobCalendarViewModel**: Calendar view data
- **RouteOptimizationViewModel**: Optimized route with turn-by-turn
- **CapacityViewModel**: Technician and equipment capacity metrics

## Business Rules

### Scheduling Rules
1. No double-booking of technicians
2. Travel time calculated between jobs
3. Job duration estimates based on service type and size
4. Emergency jobs can override scheduled appointments
5. Minimum notice period for bookings (configurable)
6. Maximum jobs per technician per day

### Job Workflow
1. Jobs start as "Scheduled"
2. Check-in required when technician arrives
3. Status updates pushed to customer in real-time
4. Photo documentation required for completion
5. Customer signature for completion
6. Job can be rescheduled with reason tracking

### Technician Assignment
1. Assignment based on: territory, skills, availability, workload
2. Technicians can be assigned multiple jobs per day
3. Route optimization to minimize drive time
4. Skill-based routing (specialized services)
5. Customer preference tracking (favorite technician)

### Recurring Jobs
1. Auto-generation of recurring appointments
2. Customer notification before each occurrence
3. Ability to skip individual occurrences
4. Automatic adjustment for holidays
5. Renewal reminders for contract end

## Dependencies

- **Core/Sales**: Customer and sales order data
- **Core/Organizations**: Franchisee and technician data
- **Core/Users**: Technician user accounts
- **Core/Notification**: Appointment reminders and updates
- **Core/Billing**: Job completion triggers invoicing
- **Core/Geo**: Address geocoding and routing

## For AI Agents

### Creating a Job from Sales Order
```csharp
// Convert sales order to scheduled job
var job = _jobFactory.CreateFromSalesOrder(new JobViewModel
{
    SalesOrderId = orderId,
    CustomerId = customerId,
    FranchiseeId = franchiseeId,
    ServiceType = ServiceType.FloorRestoration,
    EstimatedDuration = TimeSpan.FromHours(4),
    ScheduledDate = requestedDate,
    Items = jobItems
});

// Assign technician
_jobService.AssignTechnician(job.Id, technicianId);

// Optimize route if multiple jobs
var route = _technicianRouteService.OptimizeRoute(technicianId, date);
```

### Scheduling with Conflict Detection
```csharp
// Check availability
var conflicts = _scheduleService.CheckConflicts(
    technicianId, 
    proposedStartTime, 
    duration
);

if (!conflicts.Any())
{
    var schedule = _scheduleFactory.Create(new ScheduleViewModel
    {
        TechnicianId = technicianId,
        JobId = jobId,
        StartTime = proposedStartTime,
        EndTime = proposedStartTime.Add(duration),
        Location = customerLocation
    });
}
else
{
    // Suggest alternative times
    var alternatives = _scheduleService.FindAvailableSlots(
        technicianId, 
        date, 
        duration, 
        maxResults: 5
    );
}
```

### Recurring Job Setup
```csharp
// Create recurring schedule
var recurring = _recurringScheduleService.Create(new RecurringScheduleViewModel
{
    CustomerId = customerId,
    ServiceType = ServiceType.MaintenanceCleaning,
    Pattern = RecurrencePattern.Monthly,
    StartDate = startDate,
    EndDate = endDate.AddYears(1),
    PreferredDayOfWeek = DayOfWeek.Wednesday,
    Duration = TimeSpan.FromHours(2),
    AutoGenerateDaysAhead = 90
});

// Generate next 3 months of appointments
_recurringScheduleService.GenerateInstances(recurring.Id, 90);
```

## For Human Developers

### Common Operations

#### 1. Daily Schedule Management
```csharp
// Get day's schedule for technician
var daySchedule = _technicianScheduleService.GetDailySchedule(technicianId, date);

// Update job status
_jobService.CheckIn(jobId, actualStartTime, gpsLocation);
_jobService.Complete(jobId, completionNotes, beforeAfterPhotos);
```

#### 2. Route Optimization
```csharp
// Get all jobs for technician on date
var jobs = _jobService.GetJobsForTechnician(technicianId, date);

// Optimize route
var optimizedRoute = _technicianRouteService.OptimizeRoute(
    technicianId, 
    jobs, 
    startLocation
);

// Returns: ordered job list with estimated arrival times and driving directions
```

#### 3. Capacity Planning
```csharp
// Check capacity for date range
var capacity = _capacityPlanningService.GetCapacity(
    franchiseeId, 
    startDate, 
    endDate
);

// Returns: available slots, technician utilization, equipment availability
```

### Best Practices
- Always check for conflicts before scheduling
- Include buffer time between jobs for travel
- Update job status in real-time for customer visibility
- Implement optimistic concurrency for schedule updates
- Cache route calculations to reduce API calls
- Send appointment reminders 24 hours in advance
- Log all schedule changes with user and reason
- Handle timezone conversions properly for multi-region franchises
- Implement queue for route optimization (CPU intensive)
