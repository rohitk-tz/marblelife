# Web.UI - AngularJS Single Page Application

## Overview
The Web.UI module is the frontend user interface for the Marble Life scheduling and management system. Built with AngularJS 1.x, it provides a comprehensive Single Page Application (SPA) for managing jobs, schedules, customers, organizations, sales, and reports.

## Technology Stack
- **Framework**: AngularJS 1.x
- **UI Framework**: Bootstrap with Metronic theme
- **Build Tools**: Gulp, Bower, npm
- **State Management**: UI-Router for client-side routing
- **HTTP Client**: Custom HttpWrapper service built on $http
- **Dependencies**: 
  - angular-circular-timepicker
  - angular-star-rating
  - angularjs-bootstrap-datetimepicker
  - Various jQuery plugins

## Architecture

### Module Structure
The application follows a modular architecture with feature-based modules:

```
/public
  /modules
    /authentication - User authentication and login
    /core - Shared services, directives, filters
    /organizations - Franchisee and organization management
    /reports - Business reporting
    /sales - Sales data management
    /scheduler - Job scheduling and calendar management
    /users - User management
  /content
    /fonts - Custom typography (Kievit Book)
    /images - UI assets, flags, layouts, social icons
    /scripts - Third-party scripts and utilities
    /styles - CSS/LESS stylesheets
```

### Application Bootstrap
The application uses a custom module registration system:

```javascript
// ApplicationConfiguration is the central registry
ApplicationConfiguration.registerModule(moduleName);

// Each module configures its own routes
angular.module(moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);
```

### Routing Pattern
All modules use UI-Router with abstract and concrete states:

```javascript
$stateProvider
  .state('core', {
    abstract: true,
    templateUrl: viewsPathPrefix + 'default.client.view.html',
    controller: 'DefaultLayoutController',
    controllerAs: 'vm',
    resolve: { factory: 'checkRouting' }
  })
  .state('core.layout', {
    abstract: true,
    views: {
      header: { templateUrl: '...', controller: 'HeaderController' },
      footer: { templateUrl: '...', controller: 'FooterController' },
      content: { template: '<div ui-view></div>' }
    }
  })
  .state('core.layout.home', {
    url: '/dashboard',
    templateUrl: viewsPathPrefix + 'home.client.view.html',
    controller: "HomeController",
    controllerAs: 'vm'
  });
```

## Core Services

### HttpWrapper Service
A centralized HTTP service that wraps Angular's $http with:
- Automatic token management via cookies
- Standardized error handling
- Success/failure toast notifications
- Loading spinner integration
- Request cancellation on state changes

```javascript
angular.module('core').factory("HttpWrapper", function($http, $q, Toaster) {
  return {
    get: function(opts) {
      // Handles GET requests with standard options
      return httpWrapper.get({
        url: "/api/endpoint",
        showOnSuccess: true,
        showOnFailure: true
      });
    },
    post: function(opts) {
      // Handles POST requests
    },
    put: function(opts) {
      // Handles PUT requests
    },
    delete: function(opts) {
      // Handles DELETE requests
    }
  };
});
```

### Authentication Service
Manages user authentication, session tokens, and identity:

```javascript
angular.module('authentication').service("UserAuthenticationService", 
  function(HttpWrapper, $cookies, APP_CONFIG, $rootScope) {
    
    var login = function(userName, password) {
      return httpWrapper.post({
        url: "/user/login",
        data: { userName, password }
      });
    };
    
    var setTokenAndIdentity = function($rootScope, identity, token) {
      if (identity) {
        $cookies.put(config.clientTokenName, token);
        $rootScope.identity = identity;
        $rootScope.$broadcast('identity');
      } else {
        $cookies.remove(config.clientTokenName);
        $rootScope.identity = null;
      }
    };
    
    return {
      login: login,
      logout: logout,
      getUserIdentity: getUserIdentity,
      forgotPassword: forgotPassword,
      resetPassword: resetPassword
    };
});
```

## Module Patterns

### Controller Pattern
Controllers use the "controller as" syntax with `vm` (view model):

```javascript
angular.module('moduleName').controller('ControllerName', 
  ['$scope', 'ServiceName', function($scope, ServiceName) {
    var vm = this;
    
    vm.data = [];
    vm.save = save;
    vm.delete = deleteItem;
    
    function save() {
      ServiceName.save(vm.data).then(function(result) {
        // Handle success
      });
    }
    
    function init() {
      // Initialization logic
    }
    
    init();
}]);
```

### Service Pattern
Services encapsulate business logic and API calls:

```javascript
angular.module('moduleName').service("ServiceName", 
  ["HttpWrapper", function(httpWrapper) {
    
    var baseUrl = "/api/resource";
    
    function getList(query) {
      return httpWrapper.get({ 
        url: baseUrl + "/list?pageNumber=" + query.pageNumber 
      });
    }
    
    function save(model) {
      return httpWrapper.post({ 
        url: baseUrl + "/save", 
        data: model 
      });
    }
    
    return {
      getList: getList,
      save: save
    };
}]);
```

### Directive Pattern
Custom directives for reusable components:

```javascript
angular.module('moduleName').directive('directiveName', 
  [function() {
    return {
      restrict: 'E',
      templateUrl: '/modules/path/views/directive.html',
      scope: {
        model: '=',
        onSave: '&'
      },
      link: function(scope, element, attrs) {
        // DOM manipulation
      },
      controller: function($scope) {
        // Controller logic
      }
    };
}]);
```

## Key Features

### 1. Authentication Module
- User login with username/password
- Customer login with access code
- Password reset flow (forgot/reset)
- Email confirmation
- Session management with cookies
- Token-based API authentication

### 2. Scheduler Module
- Calendar view for job scheduling
- Job creation and management
- Estimate creation and tracking
- Vacation and meeting scheduling
- Recurring events support
- Media upload (before/after photos)
- E-signature capture
- Email template management
- Calendar import functionality
- Geographic job assignment

### 3. Organizations Module
- Franchisee management
- Service territory configuration
- Fee profiles and pricing
- Service type availability
- Contact information management
- Address management with geocoding

### 4. Sales Module
- Sales data upload and processing
- Customer data management
- Invoice tracking
- Payment processing
- Sales reporting

### 5. Reports Module
- Business intelligence dashboards
- Custom report generation
- Data export functionality
- Filtering and sorting capabilities

### 6. Users Module
- User profile management
- Role-based access control
- Permission management
- User creation and editing
- Password management

## Security Features

### Authentication
- Token-based authentication stored in cookies
- Session validation on route changes
- Route guards with `checkRouting` resolver
- Automatic logout on session expiration

### Encryption
- Client-side encryption using CryptoJS
- AES encryption for sensitive data
- Secure password transmission

### Route Protection
```javascript
resolve: { 
  factory: 'checkRouting' // Validates authentication before route access
}
```

## Build System

### Gulp Tasks
The `gulpfile.js` defines build automation:
- JavaScript concatenation and minification
- CSS preprocessing and minification
- File watching for development
- Vendor library management via Bower
- Asset injection into HTML

### Dependencies Management
- **npm**: Development tools (Gulp, ESLint)
- **Bower**: Frontend libraries (Angular, Bootstrap, jQuery)
- **LibMan**: Additional library management

## Configuration

### Application Config
```javascript
var APP_CONFIG = {
  apiUrl: '/api',
  clientTokenName: 'session-token',
  dateFormat: 'MM/DD/YYYY',
  timeFormat: 'HH:mm'
};
```

### Environment Configuration
- `Web.config` for ASP.NET configuration
- `Web.Debug.config` for debug transformations
- `libman.json` for CDN library configuration
- `bower.json` for frontend dependencies

## Development Workflow

### Local Development
1. Install dependencies: `npm install && bower install`
2. Run Gulp watch: `gulp watch`
3. Serve application via IIS or development server
4. Access at configured URL

### File Structure Convention
```
/modules/{module-name}/
  module.js              # Module definition and routing
  /controllers/          # Feature controllers
  /services/             # Business logic services
  /directives/          # Reusable components
  /filters/             # Data transformation filters
  /views/               # HTML templates
```

## API Integration

### HTTP Communication
All API calls go through the HttpWrapper service:

```javascript
// GET request with query parameters
httpWrapper.get({ 
  url: "/api/jobs/list",
  showOnSuccess: false 
});

// POST request with data
httpWrapper.post({ 
  url: "/api/jobs/save",
  data: jobModel,
  showOnSuccess: true
});

// File upload
httpWrapper.postFile({ 
  url: "/api/media/upload",
  data: formData
});

// File download
httpWrapper.getFileByPost({ 
  url: "/api/reports/download",
  data: query
});
```

### Error Handling
- Validation errors displayed via notification service
- HTTP errors caught and displayed as toast messages
- Model validation errors shown with field highlighting

## UI Components

### Common Directives
- Date/time pickers with custom formatting
- Address input with geocoding
- Phone number formatting
- File upload with preview
- Drag-and-drop interfaces
- Modal dialogs
- Data grids with sorting/filtering

### Notification System
```javascript
// Success notification
toaster.show("Operation completed successfully");

// Error notification
toaster.error("An error occurred");

// Validation errors
Notification.showValidations(modelValidation);
```

## Performance Considerations

### Request Optimization
- Request cancellation on state changes
- Pending request cleanup
- Loading spinners to indicate progress
- Lazy loading of modules (where applicable)

### Caching
- Identity information cached in $rootScope
- Session token cached in cookies
- Dropdown data cached in services

## Testing
The application structure supports:
- Unit testing with Jasmine/Karma
- E2E testing with Protractor
- ESLint for code quality

## Browser Support
- Modern browsers (Chrome, Firefox, Safari, Edge)
- IE11 with polyfills
- Responsive design for mobile/tablet

## Related Documentation
- See `/modules/authentication/AI-CONTEXT.md` for authentication details
- See `/modules/core/AI-CONTEXT.md` for shared services
- See `/modules/scheduler/AI-CONTEXT.md` for scheduling features
- See module-specific documentation for detailed information

## Integration Points
- Backend API: ASP.NET Web API
- Database: Accessed via backend services
- External services: Google Maps (geocoding), payment gateways
- Email: Server-side SMTP integration

## Deployment
The application is deployed as part of an ASP.NET web application:
- Static files served from `/public` directory
- Index.html as entry point
- API proxied through ASP.NET backend
- CDN resources for production performance
