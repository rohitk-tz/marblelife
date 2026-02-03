<!-- AUTO-GENERATED: Header -->
# CalendarImportService Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T23:15:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This service imports calendar events from an **iCalendar (.ics)** file. It uses the `Ical.Net` library to parse the file and iterate through events.

### Status
**Prototype / Incomplete**. 
-   The `CreateJobModel` method currently just prints event details to `Console.WriteLine`.
-   It does **NOT** save anything to the database yet.
-   It seems to be a proof-of-concept for importing external schedules into the MarbleLife system.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Library
Uses `Ical.Net` types:
-   `IICalendarCollection`
-   `Occurrence`
-   `IRecurringComponent`

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### External
-   **Ical.Net**: Third-party library for parsing RFC 5545 iCalendar files.

### Internal
-   **Core**: Uses `ISettings` to get the file path.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Incomplete Implementation
The code finds occurrences between "Last Month" and "Now" (`_clock.UtcNow.AddMonths(-1)`), but strictly dumps them to stdout. There is no mapping to `Job` or `Schedule` entities in the Domain. 

### Configuration
Relies on `ISettings.CalendarFilePath` which must point to a valid `.ics` file locally mapable (e.g., via URI).
<!-- END CUSTOM SECTION -->
