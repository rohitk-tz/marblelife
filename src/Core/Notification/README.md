<!-- AUTO-GENERATED: Header -->
# Core.Notification
> Email & Alert Dispatcher
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This module is the central hub for outgoing communications. It decouples the "Need to send email" business logic from the "How to send email" technical details.
It primarily relies on a **Polling** architecture (Queue Table -> Background Worker) to ensure resilience.

### Key Features
-   **Transactional Emails**: Job Confirmations, Invoices, Reset Password.
-   **Reminders**: Automated Payment Reminders, Late Fee notices.
-   **Internal Alerts**: Notification to Franchisees about new Leads.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Queuing a Notification
```csharp
var notifyService = IoC.Resolve<INotificationService>();
notifyService.Send(new NotificationModel {
    To = "user@example.com",
    Subject = "New Job",
    Body = "..."
});
```
This (typically) saves to a database table, which is then picked up by the `Jobs` service.

<!-- END AUTO-GENERATED -->
