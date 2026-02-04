# Core/ToDo - AI Context

## Purpose

The **ToDo** module provides task management, reminders, and to-do list functionality for users across the MarbleLife platform.

## Key Entities (Domain/)

### Task Management
- **ToDo**: Task/to-do item entity
- **ToDoCategory**: Task categorization
- **ToDoReminder**: Reminder configuration
- **ToDoAssignment**: Task assignment to users
- **ToDoComment**: Task comments and updates

## Service Interfaces

- **IToDoFactory**: Task creation
- **IToDoService**: Task management (CRUD, completion)
- **IToDoReminderService**: Reminder scheduling
- **IToDoAssignmentService**: Task assignment
- **IToDoNotificationService**: Task notifications

## Implementations (Impl/)

Business logic for:
- Task creation and assignment
- Due date tracking and reminders
- Task prioritization
- Completion tracking
- Overdue task escalation

## Enumerations (Enum/)

- **ToDoStatus**: New, InProgress, Completed, Cancelled, Overdue
- **ToDoPriority**: Low, Normal, High, Urgent
- **ToDoCategory**: FollowUp, Administrative, Technical, Sales, Customer

## ViewModels (ViewModel/)

- **ToDoViewModel**: Complete task data
- **ToDoListViewModel**: Task list with filters
- **ToDoReminderViewModel**: Reminder configuration

## Business Rules

1. **Reminders**: Send reminder at due date/time
2. **Overdue**: Mark tasks overdue after due date
3. **Assignment**: Tasks can be assigned to specific users
4. **Completion**: Track completion date and user
5. **Recurring**: Support for recurring tasks

## Dependencies

- **Core/Users**: Task assignment
- **Core/Notification**: Task reminders
- **Core/Scheduler**: Integration with job tasks

## For AI Agents

### Creating Task
```csharp
// Create follow-up task
var todo = _toDoFactory.Create(new ToDoViewModel
{
    Title = "Follow up with customer about additional services",
    Description = "Customer mentioned interest in patio restoration",
    DueDate = DateTime.Now.AddDays(3),
    Priority = ToDoPriority.Normal,
    Category = ToDoCategory.FollowUp,
    AssignedToUserId = salesRepId,
    RelatedCustomerId = customerId
});

// Set reminder
_toDoReminderService.SetReminder(todo.Id, DateTime.Now.AddDays(2));
```

### Completing Task
```csharp
// Mark task complete
_toDoService.Complete(todoId, new ToDoCompletionViewModel
{
    CompletedBy = userId,
    Notes = "Called customer, scheduled estimate visit"
});
```

## For Human Developers

### Best Practices
- Set realistic due dates
- Use categories for filtering and organization
- Send reminders before due date
- Track task completion rates
- Implement task templates for common workflows
- Support subtasks for complex tasks
- Enable task delegation
