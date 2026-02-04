# CalendarImportService - External Calendar Integration

## Overview
The CalendarImportService is a console application that imports calendar events from external sources (iCal format) and converts them into job/estimate entries in the Marble Life system. This enables franchisees to integrate their existing calendar systems with the scheduling platform.

## Purpose
- Import events from iCal/ICS files
- Parse calendar data into system-compatible format
- Create jobs and estimates from calendar entries
- Support recurring event patterns
- Maintain synchronization with external calendars

## Technology Stack
- **.NET Framework**: C# Console Application
- **Calendar Library**: iCal.NET (Ical.Net)
- **Database**: Entity Framework Core
- **Dependency Injection**: Custom DI container
- **Logging**: Core.Application.ILogService

## Project Structure
```
/CalendarImportService
├── CalendarImportService.csproj       # Project file
├── Program.cs                         # Entry point
├── CalendarImportService.cs           # Main service implementation
├── ICalendarImportService.cs          # Service interface
├── CalendarInfoModel.cs               # Data model
├── AppContextStore.cs                 # Context management
├── WinJobSessionContext.cs            # Session handling
├── App.config                         # Configuration
└── /Properties
    └── AssemblyInfo.cs
```

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <add key="CalendarFilePath" value="file:///C:/Import/calendar.ics" />
    <add key="ImportIntervalMinutes" value="60" />
    <add key="DefaultFranchiseeId" value="1" />
    <add key="CreateJobsFromEvents" value="true" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Main Service Implementation

### CalendarImportService.cs
```csharp
using Core.Application;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Interfaces;
using Ical.Net.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarImportService
{
    public class CalendarImportService : ICalendarImportService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;

        public CalendarImportService()
        {
            _unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _setting = ApplicationManager.DependencyInjection.Resolve<ISettings>();
        }

        public void Import()
        {
            GetEvents();
        }

        private void GetEvents()
        {
            Console.WriteLine("Starting Service!");
            
            var filePath = _setting.CalendarFilePath;
            var path = new Uri(filePath).LocalPath;
            
            // Load calendar from file
            IICalendarCollection calendars = Calendar.LoadFromFile(path);

            // Get occurrences for date range (last month to today)
            var occurrences = calendars.GetOccurrences(
                _clock.UtcNow.AddMonths(-1), 
                _clock.UtcNow
            );
            
            foreach (Occurrence occurrence in occurrences)
            {
                CreateJobModel(occurrence);
            }
        }

        private void CreateJobModel(Occurrence occurrence)
        {
            DateTime occurrenceStartTime = occurrence.Period.StartTime.AsSystemLocal;
            DateTime occurrenceEndTime = occurrence.Period.EndTime.AsSystemLocal;
            
            IRecurringComponent rc = occurrence.Source as IRecurringComponent;
            if (rc != null)
            {
                // Log event details for debugging
                Console.WriteLine("Summary : " + rc.Summary + "," + Environment.NewLine +
                                  "Start : " + occurrenceStartTime + "," + Environment.NewLine +
                                  "End : " + occurrenceEndTime + "," + Environment.NewLine +
                                  "Description : " + rc.Description + "," + Environment.NewLine +
                                  "Location : " + rc.Location + "," + Environment.NewLine +
                                  "Organizer : " + rc.Organizer + "," + Environment.NewLine +
                                  "Attendees : " + rc.Attendees.Count() + "," + Environment.NewLine);
                
                // Parse and create job/estimate from event
                ParseAndCreateJob(rc, occurrenceStartTime, occurrenceEndTime);
            }
        }
        
        private void ParseAndCreateJob(
            IRecurringComponent component, 
            DateTime startTime, 
            DateTime endTime)
        {
            try
            {
                _unitOfWork.StartTransaction();
                
                // Extract customer info from description or custom properties
                var customerInfo = ExtractCustomerInfo(component);
                
                // Create or find customer
                var customer = FindOrCreateCustomer(customerInfo);
                
                // Create job/estimate
                var job = new Job
                {
                    CustomerId = customer.Id,
                    ScheduledDate = startTime,
                    EstimatedDuration = (endTime - startTime).TotalHours,
                    Description = component.Description,
                    Location = component.Location,
                    ExternalId = component.Uid,
                    ImportedDate = _clock.UtcNow,
                    StatusId = (long)JobStatus.Scheduled
                };
                
                _unitOfWork.Repository<Job>().Save(job);
                _unitOfWork.SaveChanges();
                
                _logService.Info($"Created job from calendar event: {component.Summary}");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error($"Error creating job from event: {component.Summary}", ex);
            }
        }
        
        private CustomerInfo ExtractCustomerInfo(IRecurringComponent component)
        {
            // Parse customer information from event description or custom properties
            // Format: "Customer: John Doe, Phone: 555-1234, Email: john@example.com"
            
            var info = new CustomerInfo();
            var description = component.Description ?? "";
            
            // Simple parsing logic (can be enhanced)
            var lines = description.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("Customer:"))
                    info.Name = line.Substring(9).Trim();
                else if (line.StartsWith("Phone:"))
                    info.Phone = line.Substring(6).Trim();
                else if (line.StartsWith("Email:"))
                    info.Email = line.Substring(6).Trim();
            }
            
            // Also check attendees for customer email
            if (string.IsNullOrEmpty(info.Email) && component.Attendees.Any())
            {
                info.Email = component.Attendees.First().Value?.ToString();
            }
            
            return info;
        }
        
        private Customer FindOrCreateCustomer(CustomerInfo info)
        {
            // Try to find existing customer by email or phone
            var customer = _unitOfWork.Repository<Customer>()
                .Get(c => c.Email == info.Email || c.PrimaryPhone == info.Phone);
            
            if (customer == null)
            {
                // Create new customer
                customer = new Customer
                {
                    FirstName = info.Name?.Split(' ').FirstOrDefault() ?? "",
                    LastName = info.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                    Email = info.Email,
                    PrimaryPhone = info.Phone,
                    CreatedDate = _clock.UtcNow,
                    IsActive = true
                };
                
                _unitOfWork.Repository<Customer>().Save(customer);
            }
            
            return customer;
        }
    }
    
    public class CustomerInfo
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
```

## Calendar Data Model

### CalendarInfoModel.cs
```csharp
using System;
using System.Collections.Generic;

namespace CalendarImportService
{
    public class CalendarInfoModel
    {
        public string Uid { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Organizer { get; set; }
        public List<string> Attendees { get; set; }
        public RecurrencePattern Recurrence { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        
        public CalendarInfoModel()
        {
            Attendees = new List<string>();
            CustomProperties = new Dictionary<string, string>();
        }
    }
    
    public class RecurrencePattern
    {
        public RecurrenceFrequency Frequency { get; set; }
        public int Interval { get; set; }
        public DateTime? Until { get; set; }
        public int? Count { get; set; }
        public List<DayOfWeek> ByDay { get; set; }
    }
    
    public enum RecurrenceFrequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
```

## iCal Format Support

### Supported Properties
- **VEVENT**: Event entries
- **SUMMARY**: Event title
- **DESCRIPTION**: Event details
- **DTSTART**: Start date/time
- **DTEND**: End date/time
- **LOCATION**: Event location
- **ORGANIZER**: Event organizer
- **ATTENDEE**: Event attendees
- **RRULE**: Recurrence rules
- **UID**: Unique identifier

### Example iCal File
```
BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Marble Life//Calendar Import//EN
BEGIN:VEVENT
UID:123456@marblelife.com
DTSTART:20240215T090000
DTEND:20240215T110000
SUMMARY:Marble Restoration - John Doe
DESCRIPTION:Customer: John Doe\nPhone: 555-1234\nEmail: john@example.com\nService: Marble Polishing
LOCATION:123 Main St, Anytown, ST 12345
ORGANIZER:mailto:scheduler@marblelife.com
STATUS:CONFIRMED
END:VEVENT
END:VCALENDAR
```

## Recurrence Handling

```csharp
private void ProcessRecurringEvent(IRecurringComponent component)
{
    if (component.RecurrenceRules.Any())
    {
        var rule = component.RecurrenceRules.First();
        
        // Create recurring job pattern
        var recurringJob = new RecurringJob
        {
            BaseJobId = baseJob.Id,
            Frequency = MapFrequency(rule.Frequency),
            Interval = rule.Interval,
            EndDate = rule.Until.HasValue ? rule.Until.Value.AsSystemLocal : (DateTime?)null,
            OccurrenceCount = rule.Count,
            DaysOfWeek = MapDaysOfWeek(rule.ByDay)
        };
        
        _unitOfWork.Repository<RecurringJob>().Save(recurringJob);
    }
}

private JobRecurrenceFrequency MapFrequency(FrequencyType frequency)
{
    switch (frequency)
    {
        case FrequencyType.Daily:
            return JobRecurrenceFrequency.Daily;
        case FrequencyType.Weekly:
            return JobRecurrenceFrequency.Weekly;
        case FrequencyType.Monthly:
            return JobRecurrenceFrequency.Monthly;
        case FrequencyType.Yearly:
            return JobRecurrenceFrequency.Yearly;
        default:
            return JobRecurrenceFrequency.None;
    }
}
```

## Error Handling

```csharp
private void ImportWithErrorHandling()
{
    try
    {
        Import();
    }
    catch (FileNotFoundException ex)
    {
        _logService.Error("Calendar file not found: " + _setting.CalendarFilePath, ex);
        Console.WriteLine("ERROR: Calendar file not found");
    }
    catch (FormatException ex)
    {
        _logService.Error("Invalid calendar format", ex);
        Console.WriteLine("ERROR: Invalid calendar format");
    }
    catch (Exception ex)
    {
        _logService.Error("Unexpected error during import", ex);
        Console.WriteLine("ERROR: Import failed - " + ex.Message);
    }
}
```

## Logging

```csharp
private void LogImportSummary(ImportResult result)
{
    _logService.Info($@"
        Calendar Import Summary:
        - Total Events: {result.TotalEvents}
        - Jobs Created: {result.JobsCreated}
        - Customers Created: {result.CustomersCreated}
        - Errors: {result.Errors}
        - Duration: {result.Duration.TotalSeconds}s
    ");
}
```

## Deployment

### Running as Scheduled Task
```csharp
// Program.cs
static void Main(string[] args)
{
    try
    {
        DependencyRegistrar.RegisterDependencies();
        ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
        ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
        DependencyRegistrar.SetupCurrentContextWinJob();
        
        var service = ApplicationManager.DependencyInjection.Resolve<ICalendarImportService>();
        service.Import();
        
        Console.WriteLine("Import completed successfully");
        Environment.Exit(0);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Import failed: {ex.Message}");
        Environment.Exit(1);
    }
}
```

### Task Scheduler Configuration
```powershell
# Create scheduled task to run every hour
$action = New-ScheduledTaskAction -Execute "C:\Services\CalendarImportService\CalendarImportService.exe"
$trigger = New-ScheduledTaskTrigger -Once -At (Get-Date) -RepetitionInterval (New-TimeSpan -Hours 1) -RepetitionDuration ([TimeSpan]::MaxValue)
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries

Register-ScheduledTask -TaskName "MarbleLife Calendar Import" -Action $action -Trigger $trigger -Principal $principal -Settings $settings
```

## Integration Points

### External Calendar Systems
- **Google Calendar**: Export to iCal format
- **Outlook Calendar**: Save as .ics file
- **Apple Calendar**: Export to iCal
- **Third-party scheduling tools**: iCal export feature

### Internal Systems
- **Job Management**: Creates jobs in scheduler
- **Customer Management**: Creates/updates customer records
- **Notification Service**: Triggers for imported events
- **Reporting**: Import statistics and audit trail

## Best Practices

1. **Validate Input**: Check calendar file format before processing
2. **Deduplication**: Use UID to avoid duplicate imports
3. **Transaction Management**: Use transactions for data integrity
4. **Error Recovery**: Log errors and continue processing
5. **Idempotency**: Safe to run multiple times
6. **Archiving**: Move processed files to archive folder
7. **Monitoring**: Track import success rate and performance

## Related Services
- See `/NotificationService/AI-CONTEXT.md` for notification handling
- See `/CustomerDataUpload/AI-CONTEXT.md` for bulk data import
- See Web.UI Scheduler module for job management
