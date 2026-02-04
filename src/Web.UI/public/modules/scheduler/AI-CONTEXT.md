# Scheduler Module

## Overview
The Scheduler module is the core scheduling and job management system for Marble Life operations. It provides comprehensive features for managing jobs, estimates, calendar events, technician schedules, vacation planning, and media uploads. This is one of the largest and most complex modules in the application.

## Module Structure

```
/scheduler
├── module.js                                    # Module definition and routing
├── /controllers                                 # 40+ controllers
│   ├── scheduler.calendar.client.controller.js # Main calendar view
│   ├── create.job.client.controller.js         # Job creation
│   ├── manage.job.client.controller.js         # Job management
│   ├── edit.job.info.client.controller.js      # Job editing
│   ├── create.estimate.client.controller.js    # Estimate creation
│   ├── manage.estimate.client.controller.js    # Estimate management
│   ├── manage.vacation.client.controller.js    # Vacation scheduling
│   ├── create.meeting.client.controller.js     # Meeting scheduling
│   ├── job.repeat.client.controller.js         # Recurring jobs
│   ├── estimate.repeat.client.controller.js    # Recurring estimates
│   ├── vacation.repeat.client.controller.js    # Recurring vacation
│   ├── media.upload.client.controller.js       # Photo/video upload
│   ├── before.after.client.controller.js       # Before/after images
│   ├── esignature.client.controller.js         # Electronic signatures
│   ├── import.calendar.client.controller.js    # Calendar import
│   ├── email.template.client.controller.js     # Email templates
│   └── [30+ more controllers]
├── /services
│   ├── job.client.service.js                   # Job operations
│   ├── estimate.client.service.js              # Estimate operations
│   ├── calendar.client.service.js              # Calendar operations
│   ├── before.after.client.service.js          # Image management
│   ├── geoCode.client.service.js               # Geocoding
│   └── [Additional services]
├── /directives
│   └── [Custom scheduler directives]
└── /views
    ├── scheduler.calendar.client.view.html
    ├── create.job.client.view.html
    ├── manage.job.client.view.html
    └── [40+ view templates]
```

## Key Features

### 1. Calendar Management
Interactive calendar for scheduling and viewing jobs, estimates, meetings, and vacation time.

**Calendar Service**
```javascript
angular.module('scheduler').service("CalendarService", 
    ["HttpWrapper", function(httpWrapper) {
        var baseUrl = "/scheduler/calendar";
        
        return {
            saveCalendar: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/upload", 
                    data: model 
                });
            },
            
            getTimeZoneInfo: function() {
                return httpWrapper.post({ 
                    url: "/application/dropdown/GetTimeZoneList" 
                });
            },
            
            getCalendarEvents: function(startDate, endDate, filters) {
                return httpWrapper.get({
                    url: baseUrl + "/events?start=" + startDate + 
                         "&end=" + endDate + "&filters=" + JSON.stringify(filters)
                });
            }
        };
}]);
```

**Calendar Controller Example**
```javascript
angular.module('scheduler').controller('SchedulerCalendarController',
    ['$scope', 'CalendarService', 'JobService', function($scope, calendarService, jobService) {
        var vm = this;
        
        vm.calendarConfig = {
            height: 'auto',
            editable: true,
            eventLimit: true,
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay'
            }
        };
        
        vm.loadEvents = function(start, end, timezone, callback) {
            calendarService.getCalendarEvents(start, end, vm.filters)
                .then(function(response) {
                    var events = response.data.map(function(evt) {
                        return {
                            id: evt.id,
                            title: evt.title,
                            start: evt.startDate,
                            end: evt.endDate,
                            color: evt.color,
                            data: evt
                        };
                    });
                    callback(events);
                });
        };
        
        vm.eventClick = function(event) {
            // Handle event click - open job/estimate details
        };
}]);
```

### 2. Job Management

**Job Service**
```javascript
angular.module('scheduler').service("JobService",
    ["HttpWrapper", function(httpWrapper) {
        var baseUrl = "/scheduler/job";
        
        return {
            saveJob: function(job) {
                return httpWrapper.post({ 
                    url: baseUrl + "/save", 
                    data: job 
                });
            },
            
            getJob: function(jobId) {
                return httpWrapper.get({ 
                    url: baseUrl + "/get/" + jobId 
                });
            },
            
            updateJobStatus: function(jobId, statusId) {
                return httpWrapper.put({
                    url: baseUrl + "/status/" + jobId,
                    data: { statusId: statusId }
                });
            },
            
            assignTechnician: function(jobId, technicianId) {
                return httpWrapper.post({
                    url: baseUrl + "/assign",
                    data: { jobId: jobId, technicianId: technicianId }
                });
            },
            
            deleteJob: function(jobId) {
                return httpWrapper.delete({ 
                    url: baseUrl + "/delete/" + jobId 
                });
            },
            
            getJobHistory: function(jobId) {
                return httpWrapper.get({ 
                    url: baseUrl + "/history/" + jobId 
                });
            }
        };
}]);
```

**Job Creation Flow**
1. Customer selection/creation
2. Service type selection
3. Schedule date/time selection
4. Technician assignment
5. Job details and notes
6. Price estimation
7. Confirmation and calendar placement

### 3. Estimate Management

**Estimate Service**
```javascript
angular.module('scheduler').service("EstimateService",
    ["HttpWrapper", function(httpWrapper) {
        var baseUrl = "/scheduler/estimate";
        
        return {
            saveEstimate: function(estimate) {
                return httpWrapper.post({ 
                    url: baseUrl + "/SaveEstimate", 
                    data: estimate 
                });
            },
            
            getEstimateInfo: function(id) {
                return httpWrapper.get({ 
                    url: baseUrl + "/get/" + id + "/estimate" 
                });
            },
            
            deleteEstimate: function(id) {
                return httpWrapper.delete({ 
                    url: baseUrl + "/delete/" + id + "/estimate" 
                });
            },
            
            convertToJob: function(estimateId) {
                return httpWrapper.post({
                    url: baseUrl + "/convert/" + estimateId
                });
            },
            
            repeatEstimate: function(scheduleInfo) {
                return httpWrapper.post({ 
                    url: baseUrl + "/repeat", 
                    data: scheduleInfo 
                });
            }
        };
}]);
```

**Estimate Features**
- Create detailed service estimates
- Convert estimates to jobs
- Track estimate status (pending, approved, rejected)
- Email estimates to customers
- Recurring estimate scheduling

### 4. Recurring Events

**Repeat Functionality**
```javascript
// Job Repeat
function repeatJob(jobInfo) {
    return httpWrapper.post({
        url: baseUrl + "/repeat",
        data: {
            jobId: jobInfo.jobId,
            frequency: jobInfo.frequency, // Daily, Weekly, Monthly
            interval: jobInfo.interval,
            endDate: jobInfo.endDate,
            occurrences: jobInfo.occurrences
        }
    });
}

// Vacation Repeat
function repeatVacation(vacation) {
    return httpWrapper.post({
        url: baseUrl + "/repeat/vacation",
        data: vacation
    });
}

// Meeting Repeat
function repeatMeeting(meeting) {
    return httpWrapper.post({
        url: baseUrl + "/repeat/meeting",
        data: meeting
    });
}
```

**Repeat Patterns**
- Daily: Every N days
- Weekly: Specific days of week
- Monthly: Specific date or day of month
- Custom: Advanced patterns

### 5. Media Management

**Before/After Service**
```javascript
angular.module('scheduler').service("BeforeAfterService",
    ["HttpWrapper", function(httpWrapper) {
        var baseUrl = "/Scheduler/job";
        
        return {
            getReviewImages: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/getReviewImages", 
                    data: model 
                });
            },
            
            saveImagesBestPair: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/saveImagesBestPair", 
                    data: model 
                });
            },
            
            saveReviewMark: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/saveReviewMarkImage", 
                    data: model 
                });
            },
            
            markImageAsReviwed: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/markImageAsReviwed", 
                    data: model 
                });
            },
            
            markImageAsBestPair: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/markImageAsBestPair", 
                    data: model 
                });
            },
            
            markImageAsAddToLocalGallery: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/markImageAsAddToLocalGallery", 
                    data: model 
                });
            }
        };
}]);
```

**Media Features**
- Upload before/after photos
- Image review and approval workflow
- Best pair selection for marketing
- Local gallery management
- Job documentation
- Invoice line item images

### 6. E-Signature Capture

Electronic signature functionality for job completion and customer approval:
- Touchscreen signature capture
- Customer name and date
- Signature storage and retrieval
- PDF generation with signature
- Email signed documents

### 7. Vacation & Meeting Scheduling

**Vacation Management**
- Schedule technician vacation time
- Recurring vacation patterns
- Block out calendar availability
- Team vacation coordination

**Meeting Management**
- Schedule internal meetings
- Meeting recurrence
- Attendee management
- Meeting reminders

### 8. Geographic Features

**GeoCode Service**
```javascript
angular.module('scheduler').service("GeoCodeService",
    ["HttpWrapper", function(httpWrapper) {
        var baseUrl = "/scheduler/geoCode";
        
        return {
            saveFile: function(model) {
                return httpWrapper.post({ 
                    url: baseUrl + "/file/upload", 
                    data: model 
                });
            },
            
            getGeoList: function(query) {
                return httpWrapper.get({
                    url: baseUrl + "/list" +
                         "?filter.statusId=" + query.statusId +
                         "&pageNumber=" + query.pageNumber +
                         "&pageSize=" + query.pageSize +
                         "&filter.sortingColumn=" + query.sort.propName +
                         "&filter.sortingOrder=" + query.sort.order +
                         "&filter.text=" + query.text
                });
            },
            
            getGeoCode: function(query) {
                return httpWrapper.post({ 
                    url: baseUrl + "/info", 
                    data: query 
                });
            },
            
            downloadAllGeoCode: function(query) {
                return httpWrapper.getFileByPost({ 
                    url: baseUrl + "/DownloadAllGeoCodeFile", 
                    data: query 
                });
            }
        };
}]);
```

**Features**
- Geocode customer addresses
- Territory assignment
- Route optimization
- Distance calculations
- Map integration

### 9. Email Templates

Manage email templates for:
- Estimate notifications
- Job confirmations
- Follow-up communications
- Review requests
- Payment reminders

### 10. Calendar Import

Import events from external calendar formats:
- iCal format support
- Google Calendar integration
- Outlook calendar import
- Bulk event creation

## Controller Patterns

### Modal Controllers
Many controllers manage modal dialogs:
```javascript
angular.module('scheduler').controller('ModalController',
    ['$scope', '$uibModalInstance', 'data', function($scope, $uibModalInstance, data) {
        var vm = this;
        vm.data = angular.copy(data);
        
        vm.save = function() {
            $uibModalInstance.close(vm.data);
        };
        
        vm.cancel = function() {
            $uibModalInstance.dismiss('cancel');
        };
}]);
```

### Main Controllers
Feature controllers with complex business logic:
```javascript
angular.module('scheduler').controller('ManageJobController',
    ['$scope', '$stateParams', 'JobService', 'Toaster',
    function($scope, $stateParams, jobService, toaster) {
        var vm = this;
        vm.jobId = $stateParams.id;
        
        function loadJob() {
            jobService.getJob(vm.jobId).then(function(response) {
                vm.job = response.data;
            });
        }
        
        vm.updateStatus = function(newStatus) {
            jobService.updateJobStatus(vm.jobId, newStatus).then(function() {
                toaster.show('Job status updated');
                loadJob();
            });
        };
        
        vm.assignTechnician = function(techId) {
            jobService.assignTechnician(vm.jobId, techId).then(function() {
                toaster.show('Technician assigned');
                loadJob();
            });
        };
        
        loadJob();
}]);
```

## Job Lifecycle

1. **Estimate Created** - Initial customer inquiry
2. **Estimate Approved** - Customer accepts estimate
3. **Job Scheduled** - Converted to job, date/time set
4. **Job Assigned** - Technician assigned
5. **Job In Progress** - Technician en route or working
6. **Job Completed** - Work finished, signature captured
7. **Invoice Generated** - Billing created
8. **Payment Received** - Transaction complete

## Integration Points

### Customer Module
- Customer selection for jobs/estimates
- Customer contact information
- Service history

### Organizations Module
- Franchisee assignment
- Territory validation
- Service availability

### Sales Module
- Invoice generation
- Payment processing
- Sales reporting

### Users Module
- Technician assignment
- User permissions
- Role-based access

## Performance Considerations

### Calendar Optimization
- Lazy loading of events
- Date range limiting
- Event caching
- Efficient rendering

### Data Loading
- Pagination for large lists
- On-demand detail loading
- Background data refresh
- Optimistic UI updates

## Mobile Considerations

- Touch-friendly calendar interface
- Signature capture on mobile
- Photo upload from camera
- GPS location capture
- Offline capabilities (future)

## Security

- Role-based job access
- Franchisee data isolation
- Signature verification
- Audit trail for changes
- Permission validation

## Best Practices

1. **Validate dates** - Ensure no scheduling conflicts
2. **Check availability** - Verify technician availability
3. **Confirm customers** - Validate customer information
4. **Track changes** - Maintain audit history
5. **Handle media** - Properly process uploads
6. **Optimize queries** - Use efficient data loading
7. **Cache intelligently** - Balance freshness and performance

## Related Documentation
- See `/modules/core/AI-CONTEXT.md` for shared services
- See `/modules/organizations/AI-CONTEXT.md` for franchisee info
- See `/modules/sales/AI-CONTEXT.md` for billing integration
