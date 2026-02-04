# Jobs/Impl - AI Context

## Purpose

This folder contains concrete implementations of background job classes that execute scheduled tasks for the MarbleLife platform.

## Contents

Job implementation classes (47+ jobs):
- **NotificationJobs**: Email/SMS sending jobs
- **DataProcessingJobs**: Data import and parsing
- **BillingJobs**: Invoice generation, payment processing
- **ReportJobs**: Scheduled report generation
- **SyncJobs**: Data synchronization with external systems
- **MaintenanceJobs**: Database cleanup, log rotation

## For AI Agents

**Job Implementation Pattern**:
```csharp
public class SendAppointmentRemindersJob : IJob
{
    private readonly IScheduleService _scheduleService;
    private readonly INotificationService _notificationService;
    private readonly ILogService _logService;
    
    public SendAppointmentRemindersJob(
        IScheduleService scheduleService,
        INotificationService notificationService,
        ILogService logService)
    {
        _scheduleService = scheduleService;
        _notificationService = notificationService;
        _logService = logService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logService.LogInfo("Starting appointment reminder job");
            
            // Get appointments for tomorrow
            var tomorrow = DateTime.Now.AddDays(1);
            var appointments = _scheduleService
                .GetAppointments(tomorrow)
                .Where(a => !a.ReminderSent);
            
            // Send reminders
            foreach (var appointment in appointments)
            {
                await _notificationService.SendAppointmentReminder(
                    appointment.CustomerId,
                    appointment.ScheduledTime
                );
                
                _scheduleService.MarkReminderSent(appointment.Id);
            }
            
            _logService.LogInfo($"Sent {appointments.Count()} reminders");
        }
        catch (Exception ex)
        {
            _logService.LogError("Appointment reminder job failed", ex);
            throw;
        }
    }
}
```

## For Human Developers

Job implementations execute on schedules defined in configuration.

### Job Development Guidelines:

1. **Dependency Injection**: Use constructor injection for all dependencies
2. **Error Handling**: Wrap in try-catch, log errors, rethrow if needed
3. **Logging**: Log start, completion, and any important milestones
4. **Idempotency**: Jobs should be safe to run multiple times
5. **Performance**: Avoid long-running operations, batch where possible
6. **Transactions**: Use transactions for data modifications
7. **Configuration**: Use app settings for job-specific configuration

### Testing Jobs:
```csharp
[TestMethod]
public async Task SendReminders_SendsToScheduledAppointments()
{
    // Arrange
    var mockSchedule = new Mock<IScheduleService>();
    var mockNotification = new Mock<INotificationService>();
    var job = new SendAppointmentRemindersJob(
        mockSchedule.Object,
        mockNotification.Object,
        new LogService()
    );
    
    // Act
    await job.Execute(null);
    
    // Assert
    mockNotification.Verify(x => x.SendAppointmentReminder(
        It.IsAny<int>(), 
        It.IsAny<DateTime>()
    ), Times.AtLeastOnce);
}
```

### Best Practices:
- Keep jobs focused on single responsibility
- Handle failures gracefully
- Implement retry logic for transient failures
- Monitor job execution via logging
- Use distributed locks for jobs that shouldn't run concurrently
- Consider time zones for scheduled jobs
- Test jobs in isolation before deploying
