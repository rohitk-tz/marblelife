<!-- AUTO-GENERATED: Header -->
# Core.Notification Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-04T00:30:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Communication Center**. It handles the delivery of messages (Email, and possibly SMS) to Users, Franchisees, and Customers.

### Key Entities
-   `Notification`: A generic record of a message sent or to be sent.
-   `EmailLog`: (Implicit) Tracking of external email provider responses.

### Logic Flow
1.  **Trigger**: Business event occurs (e.g., Invoice Created, Job Scheduled).
2.  **Queue**: A notification record is created via `INotificationService`.
3.  **Polling**: A background agent (`INotificationPollingAgent`) picks up pending notifications.
4.  **Dispatch**: `IEmailDispatcher` sends the actual email (likely via SMTP or SendGrid/Postmark).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Services
-   `IEmailDispatcher`: Abstract interface for sending emails.
-   `INotificationService`: Application-level interface to "Send a Notification" without worrying about the mechanism.
-   `IPaymentReminderPollingAgent`: Specialized agent for chasing unpaid invoices.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[Core.Users](../Users/AI-CONTEXT.md)**: Recipients are usually Users.
-   **[Core.Billing](../Billing/AI-CONTEXT.md)**: Sends Invoice/Late Fee notifications.
-   **[Core.Scheduler](../Scheduler/AI-CONTEXT.md)**: Sends Job Reminders.

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Async by Design
The presence of `PollingAgent` interfaces suggests that notifications are **Asynchronous**.
The web app writes to a table, and the Windows Service (`Src/Jobs`) actually sends the email. This prevents the Web UI from hanging during slow SMTP handshakes.

### Templates
Look for where the Email HTML templates are stored. They might be in a `Resources` folder, hardcoded strings, or files on disk (common in legacy apps).
<!-- END CUSTOM SECTION -->
