# API/Areas/Scheduler - AI Context

## Purpose

The **Scheduler** area manages job scheduling, technician dispatch, appointment booking, and work order management. It handles the operational workflow from job creation to completion.

## Key Functionality

### Job Management
- Create and schedule jobs
- Assign technicians to jobs
- Track job status (Scheduled, In Progress, Completed, Cancelled)
- Manage job details (services, materials, labor)
- Handle job rescheduling

### Technician Dispatch
- View technician availability
- Assign jobs to technicians
- Optimize routes and schedules
- Track technician location (if GPS enabled)
- Manage technician capacity

### Appointment Booking
- Customer-facing booking interface
- Available time slot calculation
- Confirmation and reminders
- Appointment modifications
- Cancellation handling

### Work Orders
- Generate work orders from jobs
- Before/after photo capture
- Customer signature collection
- Work completion notes
- Quality checkli sts

## Key Controllers

### JobController.cs
Primary job management operations.

**Endpoints**:
- `GET /Scheduler/Job/{id}` - Get job details
- `GET /Scheduler/Job/GetList` - Get job list with filters
- `POST /Scheduler/Job` - Create/update job
- `POST /Scheduler/Job/Assign` - Assign technician to job
- `POST /Scheduler/Job/Reschedule` - Reschedule job
- `POST /Scheduler/Job/Complete` - Mark job as complete
- `GET /Scheduler/Job/GetCalendarView` - Get jobs for calendar display

### TechnicianController.cs
Technician management and scheduling.

**Endpoints**:
- `GET /Scheduler/Technician/GetAvailable` - Get available technicians
- `GET /Scheduler/Technician/GetSchedule` - Get technician's schedule
- `POST /Scheduler/Technician/UpdateAvailability` - Set availability

### AppointmentController.cs
Customer appointment booking and management.

**Endpoints**:
- `POST /Scheduler/Appointment/Book` - Book new appointment
- `GET /Scheduler/Appointment/GetAvailableSlots` - Get available time slots
- `POST /Scheduler/Appointment/Confirm` - Confirm appointment
- `POST /Scheduler/Appointment/Cancel` - Cancel appointment

## Key ViewModels

```csharp
public class JobViewModel
{
    public long Id { get; set; }
    public string JobNumber { get; set; }
    public long CustomerId { get; set; }
    public long FranchiseeId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan ScheduledTime { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public long? TechnicianId { get; set; }
    public JobStatus Status { get; set; }
    public string ServiceAddress { get; set; }
    public List<ServiceItem> Services { get; set; }
    public string SpecialInstructions { get; set; }
}

public class TechnicianScheduleViewModel
{
    public long TechnicianId { get; set; }
    public string TechnicianName { get; set; }
    public List<ScheduledJob> Jobs { get; set; }
    public List<TimeSlot> AvailableSlots { get; set; }
}
```

## Business Rules

- Jobs must be scheduled within franchisee's service territory
- Technicians can only be assigned to jobs they're qualified for
- Cannot double-book technicians
- Minimum 24-hour notice for rescheduling (configurable)
- Jobs must be completed before invoicing
- Customer confirmation required for appointments
- Automatic reminders sent 24 hours before appointment

## Authorization

- **Franchisee Users**: Manage their franchisee's jobs
- **Technicians**: View assigned jobs, update job status
- **Customers**: Book appointments, view their jobs
- **Super Admin**: Full access to all jobs

## Integration Points

- **Sales**: Links to customers and invoices
- **Organizations**: Franchisee territory and service offerings
- **Users**: Technician assignments
- **Notification**: Appointment reminders and confirmations
