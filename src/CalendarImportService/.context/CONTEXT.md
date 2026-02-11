<!-- AUTO-GENERATED: Header -->
# CalendarImportService — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
CalendarImportService synchronizes calendar events from iCalendar (.ics) files into the Marblelife system database. It parses recurring events, expands occurrences within a time window, and extracts detailed event metadata for job scheduling or customer appointment tracking.

### Design Patterns
- **Dependency Injection**: Uses ApplicationManager for service resolution
- **Repository Pattern**: Leverages IUnitOfWork for database persistence
- **iCalendar Standard (RFC 5545)**: Parses .ics files via Ical.Net library
- **Time Window Processing**: Fetches occurrences within a relative date range (e.g., last month to now)

### Data Flow
1. Entry via `Program.Main()` → Dependency registration
2. `CalendarImportService.Import()` invoked
3. Reads calendar file path from `ISettings.CalendarFilePath` (App.config)
4. `Ical.Net.Calendar.LoadFromFile()` parses .ics format
5. `GetOccurrences()` expands recurring events into individual instances
6. `CreateJobModel()` processes each occurrence (extracts metadata, prints to console)
7. Intended persistence via `IUnitOfWork` (currently logs only)

### Recurring Event Handling
The service uses Ical.Net to handle complex recurrence patterns:
- **Daily/Weekly/Monthly**: Expands into individual occurrences
- **Exception dates**: Skips canceled instances
- **Time zones**: Converts to system local time
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### CalendarInfoModel.cs
```csharp
public class CalendarInfoModel
{
    public string Summary { get; set; }       // Event title
    public DateTime StartTime { get; set; }   // Occurrence start
    public DateTime EndTime { get; set; }     // Occurrence end
    public string Description { get; set; }   // Event details
    public string Uid { get; set; }           // Unique event identifier
    // Additional metadata fields
}
```

### ICalendarImportService Interface
```csharp
public interface ICalendarImportService
{
    void Import();  // Main entry point for calendar synchronization
}
```

### WinJobSessionContext
```csharp
public class WinJobSessionContext : ISessionContext
{
    // Provides execution context for console job (user, franchise, timing)
}
```

### AppContextStore
```csharp
public class AppContextStore : IAppContextStore
{
    // Stores current session context for service resolution
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `CalendarImportService.Import()`
- **Input**: None (reads file path from ISettings)
- **Output**: void
- **Behavior**: 
  - Loads .ics file from configured path
  - Expands recurring events for date range: `Now - 1 month` to `Now`
  - Extracts event metadata (summary, times, attendees, organizer)
  - Logs each occurrence to console
- **Side-effects**: Console output, potential database writes (not implemented)

### `GetEvents()` (private)
- **Input**: None
- **Output**: void
- **Behavior**:
  - Retrieves calendar file URI from settings
  - Parses with Ical.Net library
  - Filters occurrences by time window
  - Delegates to `CreateJobModel()` for each occurrence

### `CreateJobModel(Occurrence occurrence)` (private)
- **Input**: Ical.Net Occurrence object
- **Output**: void
- **Behavior**:
  - Extracts 20+ metadata fields (Summary, Description, Uid, Attendees, etc.)
  - Converts UTC times to system local time
  - Writes structured output to console
- **Side-effects**: Console logging (no persistence yet)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Application](../../Core/Application/.context/CONTEXT.md)** — Dependency injection, logging, settings
- **[Core.Application.IUnitOfWork](../../Core/Application/.context/CONTEXT.md)** — Database transaction management
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration bootstrapper

### External
- **Ical.Net** (v2.x) — iCalendar RFC 5545 parser for .ics files
- **System.Configuration** — App.config access

### Configuration Dependencies
Requires `CalendarFilePath` in App.config:
```xml
<add key="CalendarFilePath" value="file:///D:/calendar.ics" />
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Current Implementation Limitations
**Note**: The current implementation logs events to the console but **does not persist to the database**. The `CreateJobModel()` method prints metadata but lacks database write operations.

To enable persistence:
1. Add domain model mapping (Occurrence → Job entity)
2. Use `_unitOfWork.Repository<Job>().Save()`
3. Call `_unitOfWork.SaveChanges()`

### Google Calendar Integration
The example configuration uses a Google Calendar export URL:
```
Greg_Mastrangelo_m680oj07q2g6o1fpordcs7chp0@group.calendar.google.com.ics
```

To sync Google Calendar:
1. Export calendar as .ics (Calendar Settings → Export)
2. Place file on accessible path or use direct HTTP URL
3. Update `CalendarFilePath` in App.config

### Time Zone Handling
Events are converted from UTC to system local time:
```csharp
occurrence.Period.StartTime.AsSystemLocal
```
Ensure server time zone matches business location to avoid scheduling errors.

### Recurrence Complexity
The service processes recurring events correctly but:
- **Exception dates** are honored (skips canceled instances)
- **Time limits** prevent infinite recurrence processing
- **Overlapping events** are not deduplicated

### Performance Considerations
- **Large calendars**: Parsing 1000+ events may cause startup delay
- **Network latency**: Remote .ics files require HTTP download time
- **Memory usage**: All occurrences loaded into memory simultaneously
<!-- END CUSTOM SECTION -->
