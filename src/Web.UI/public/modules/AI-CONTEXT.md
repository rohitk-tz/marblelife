# Web.UI/public/modules - AI Context

## Purpose

This folder contains all AngularJS modules for the MarbleLife web application, organized by business domain functionality.

## Structure

The modules folder contains 7 AngularJS modules:
- **authentication**: User login, logout, password recovery
- **core**: Shared services, directives, filters, and utilities
- **organizations**: Franchisee and organization management
- **reports**: Business intelligence and reporting
- **sales**: Customer and sales pipeline management
- **scheduler**: Job scheduling and calendar operations
- **users**: User management and profiles

## Module Organization Pattern

Each module follows a consistent structure:
```
module-name/
├── controllers/     # Controller logic for views
├── services/        # Data services and API communication
├── directives/      # Reusable UI components (optional)
├── views/           # HTML templates
└── filters/         # Data transformation filters (optional)
```

## For AI Agents

**AngularJS Module Pattern**:
```javascript
// Module definition
angular.module('marblelife.scheduler', [])
    .config(['$stateProvider', function($stateProvider) {
        $stateProvider
            .state('scheduler', {
                url: '/scheduler',
                templateUrl: 'modules/scheduler/views/calendar.html',
                controller: 'SchedulerController'
            });
    }]);
```

**Controller Pattern**:
```javascript
angular.module('marblelife.scheduler')
    .controller('SchedulerController', [
        '$scope', 
        'SchedulerService',
        function($scope, SchedulerService) {
            $scope.appointments = [];
            
            $scope.loadAppointments = function() {
                SchedulerService.getAppointments()
                    .then(function(response) {
                        $scope.appointments = response.data;
                    });
            };
            
            $scope.loadAppointments();
        }
    ]);
```

## For Human Developers

AngularJS 1.x application with modular architecture:

### Key Concepts:

1. **Modules**: Logical grouping of related functionality
2. **Controllers**: View logic and user interaction handling
3. **Services**: Data operations and business logic
4. **Directives**: Reusable UI components
5. **Views**: HTML templates with Angular binding
6. **Routing**: ui-router for navigation

### Development Workflow:

1. **Add New Feature**:
   - Create controller in appropriate module
   - Create service for API calls
   - Create view template
   - Add route configuration

2. **Shared Functionality**:
   - Put in core module
   - Make reusable across other modules

3. **Module Dependencies**:
   - Core module is dependency for all others
   - Each module can depend on other modules as needed

### Best Practices:
- Use dependency injection for all dependencies
- Keep controllers thin (delegate to services)
- Use services for API communication
- Implement proper error handling
- Follow AngularJS style guide
- Use promises for asynchronous operations
- Minimize $scope usage (prefer controllerAs syntax)
- Test controllers and services with unit tests
