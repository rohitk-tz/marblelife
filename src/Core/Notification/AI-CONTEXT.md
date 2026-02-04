# Core/Notification - AI Context

## Purpose

The **Notification** module handles multi-channel communication including email, SMS, push notifications, and in-app messages across the MarbleLife platform.

## Key Entities (Domain/)

### Notification Management
- **Notification**: Core notification entity
- **NotificationTemplate**: Reusable message templates
- **NotificationQueue**: Queued messages for delivery
- **NotificationHistory**: Delivery audit trail
- **NotificationPreference**: User communication preferences

### Communication Channels
- **EmailNotification**: Email-specific data
- **SMSNotification**: SMS message details
- **PushNotification**: Mobile push notifications
- **InAppNotification**: Application notifications

### Delivery Management
- **NotificationBatch**: Bulk notification processing
- **NotificationSchedule**: Scheduled notifications
- **NotificationRetry**: Failed delivery retry logic

## Service Interfaces

### Core Notification Services
- **INotificationService**: Main notification orchestration
- **INotificationTemplateService**: Template management
- **INotificationQueueService**: Queue management
- **INotificationPreferenceService**: User preferences

### Channel Services
- **IEmailService**: Email sending via AWS SES/SMTP
- **ISMSService**: SMS via Twilio/similar
- **IPushNotificationService**: iOS/Android push notifications
- **IInAppNotificationService**: In-app messaging

### Delivery Services
- **INotificationDispatchService**: Multi-channel dispatch
- **INotificationRetryService**: Failed delivery retry
- **INotificationBatchService**: Bulk sending
- **INotificationTrackingService**: Delivery tracking

## Implementations (Impl/)

Business logic for:
- Multi-channel message delivery
- Template rendering with variables
- Delivery queue processing
- Failed delivery retry with backoff
- User preference enforcement
- Delivery analytics

## Enumerations (Enum/)

- **NotificationType**: Email, SMS, Push, InApp
- **NotificationPriority**: Low, Normal, High, Urgent
- **NotificationCategory**: Appointment, Payment, Marketing, System, Alert
- **DeliveryStatus**: Pending, Sent, Delivered, Failed, Bounced, Unsubscribed
- **TemplateType**: Transactional, Marketing, System

## ViewModels (ViewModel/)

- **NotificationViewModel**: Notification data
- **EmailViewModel**: Email-specific properties
- **SMSViewModel**: SMS message data
- **PushNotificationViewModel**: Push notification payload
- **NotificationTemplateViewModel**: Template configuration
- **BulkNotificationViewModel**: Batch sending data

## Business Rules

1. **User Preferences**: Respect opt-out preferences
2. **Rate Limiting**: Prevent notification spam
3. **Priority**: Urgent messages bypass queue
4. **Retry Logic**: 3 attempts with exponential backoff
5. **Tracking**: All notifications logged for audit
6. **Unsubscribe**: One-click unsubscribe in emails
7. **Compliance**: CAN-SPAM and GDPR compliance

## Dependencies

- **Core/Users**: User contact information
- **Infrastructure**: AWS SES, Twilio, FCM integration
- **Core/Scheduler**: Appointment reminders
- **Core/Billing**: Payment notifications
- **Core/Organizations**: Franchisee notifications

## For AI Agents

### Sending Email
```csharp
// Send transactional email
await _emailService.Send(new EmailViewModel
{
    To = "customer@example.com",
    Subject = "Appointment Confirmation",
    TemplateId = "appointment-confirmation",
    Variables = new Dictionary<string, string>
    {
        { "CustomerName", "John Smith" },
        { "AppointmentDate", "March 15, 2024 at 2:00 PM" },
        { "TechnicianName", "Mike Johnson" }
    }
});
```

### Sending SMS
```csharp
// Send SMS reminder
await _smsService.Send(new SMSViewModel
{
    To = "+15551234567",
    Message = "Reminder: Your appointment is tomorrow at 2:00 PM. Reply CONFIRM to confirm.",
    Category = NotificationCategory.Appointment
});
```

### Push Notification
```csharp
// Send push notification to mobile app
await _pushNotificationService.Send(new PushNotificationViewModel
{
    UserId = userId,
    Title = "Job Completed",
    Body = "Your marble restoration is complete!",
    Data = new { jobId = 12345, action = "view" },
    Badge = 1
});
```

### Batch Notifications
```csharp
// Send bulk email campaign
await _notificationBatchService.SendBatch(new BulkNotificationViewModel
{
    Type = NotificationType.Email,
    TemplateId = "monthly-newsletter",
    Recipients = customerEmails,
    ScheduleTime = DateTime.Now.AddHours(2)
});
```

## For Human Developers

### Best Practices
- Always use templates for consistent messaging
- Respect user notification preferences
- Implement retry logic for failed deliveries
- Log all notification attempts
- Use async/await for all notification operations
- Monitor delivery rates and bounce rates
- Implement unsubscribe handling
- Test emails across multiple clients
- Use proper MIME types for attachments
- Sanitize HTML in email templates
- Include tracking pixels for marketing emails
- Use SMS for time-sensitive notifications
- Implement A/B testing for marketing campaigns
