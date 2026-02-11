<!-- AUTO-GENERATED: Header -->
# CalendarImportService
> iCalendar synchronization utility for importing recurring events into the Marblelife system
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
CalendarImportService bridges external calendar systems (Google Calendar, Outlook, iCal) with the Marblelife appointment or job scheduling system. It reads standard .ics (iCalendar) files, expands recurring events into individual occurrences, and prepares them for database storage.

Think of it as a translator that converts "Meet every Tuesday at 2 PM" into specific dates like "Feb 6 at 2 PM, Feb 13 at 2 PM, Feb 20 at 2 PM" — making them queryable and usable within the application.

**Current Status**: The service successfully parses and logs calendar data but does not yet persist events to the database. It serves as a foundation for future job scheduling automation.

**Use Cases**:
- Syncing franchisee work schedules
- Importing customer appointments from external systems
- Pre-populating job calendars from corporate planning tools
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Prerequisites
1. .NET Framework 4.5.2+
2. MySQL database connection (configured in App.config)
3. iCalendar (.ics) file accessible to the application

### Configuration

**App.config**:
```xml
<connectionStrings>
  <add name="ConnectionString" 
       connectionString="server=localhost; database=Makalu; User ID=Makalu; Password=MakaluUser12!; Port=3308;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
<appSettings>
  <add key="CalendarFilePath" value="file:///D:/projects/calendar.ics" />
</appSettings>
```

### Running the Service

**Basic Execution**:
```bash
CalendarImportService.exe
# Output: Parses calendar, prints event details to console
```

**Expected Console Output**:
```
Starting Service!
Summary : Team Meeting, 
Start : 2/6/2025 2:00:00 PM, 
End : 2/6/2025 3:00:00 PM, 
Description : Weekly sync meeting
Uid : abc123@google.com
Attendees : 3

Summary : Customer Appointment,
Start : 2/7/2025 10:00:00 AM,
End : 2/7/2025 11:00:00 AM,
...
```

### Supported Calendar Formats

**Google Calendar**:
1. Go to Google Calendar → Settings → Export
2. Download .ics file
3. Place on server or use direct URL

**Outlook/Office 365**:
1. Export calendar to .ics format
2. Copy to server path

**Apple Calendar**:
1. File → Export → Calendar Archive
2. Extract .ics files

### Time Window Configuration
By default, imports events from:
- **Start**: 1 month ago (`_clock.UtcNow.AddMonths(-1)`)
- **End**: Current time (`_clock.UtcNow`)

To change window, modify `GetEvents()`:
```csharp
var occurrences = calendars.GetOccurrences(
    _clock.UtcNow.AddMonths(-2),  // Extend to 2 months ago
    _clock.UtcNow.AddMonths(1)     // Include 1 month future
);
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `Import()` | Main entry point; triggers calendar parsing and event extraction |
| `GetEvents()` | Loads .ics file and expands recurring events into occurrences |
| `CreateJobModel(Occurrence)` | Extracts metadata from single event occurrence and logs to console |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "File not found" Error
**Cause**: Invalid path in `CalendarFilePath` setting.  
**Solution**: 
- Use absolute file path: `D:\path\to\calendar.ics`
- Or file URI: `file:///D:/path/to/calendar.ics`
- Verify file exists and application has read permissions

### "Could not find schema information" Error
**Cause**: .ics file has invalid iCalendar syntax.  
**Solution**: 
- Validate .ics file with online tools (icalendar.org validator)
- Re-export from source calendar application
- Check for special characters or encoding issues

### No events displayed
**Cause**: Events outside time window (older than 1 month or in future).  
**Solution**: 
- Adjust date range in `GetEvents()` method
- Verify events exist in .ics file (open in text editor)
- Check time zone conversions

### Incorrect event times
**Cause**: Time zone mismatch between server and calendar.  
**Solution**:
- Set server time zone to match business location
- Or modify code to use explicit time zone conversion
- Verify .ics file includes VTIMEZONE components

### Events not persisting to database
**Cause**: Database write logic not implemented yet.  
**Solution**: 
- Current version only logs to console
- To implement persistence:
  1. Create domain model mapping (Occurrence → Job entity)
  2. Add `_unitOfWork.Repository<Job>().Save()` call in `CreateJobModel()`
  3. Invoke `_unitOfWork.SaveChanges()` after processing

### Performance issues with large calendars
**Cause**: Processing 1000+ events causes startup delay.  
**Solution**:
- Narrow time window (e.g., last 2 weeks instead of last month)
- Implement batch processing with progress logging
- Filter by calendar category or attendee before import

### Recurring events not expanding
**Cause**: .ics file missing RRULE (recurrence rule) definitions.  
**Solution**:
- Re-export calendar ensuring recurring events included
- Verify RRULE syntax in .ics file:
  ```
  RRULE:FREQ=WEEKLY;BYDAY=TU;COUNT=10
  ```
<!-- END CUSTOM SECTION -->
