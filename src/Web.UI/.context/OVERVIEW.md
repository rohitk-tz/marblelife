<!-- AUTO-GENERATED: Header -->
# Web.UI
> AngularJS 1.5.8 SPA frontend for MarbleLife franchise management system with calendar-based scheduling, job tracking, and business analytics
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Web.UI module is the user-facing Single Page Application (SPA) for managing MarbleLife's marble and stone restoration franchise operations. Think of it as a "mission control center" where franchise owners, sales staff, and technicians coordinate everything from initial customer estimates to completed job invoicing.

**Why AngularJS 1.5.8?** This is a legacy application built when AngularJS 1.x was the dominant SPA framework. While newer frameworks exist (Angular 2+, React, Vue), the codebase remains on 1.5.8, which means understanding Angular 1.x patterns (controllers, factories, $scope, two-way binding) is essential for maintenance.

**The Core Workflow:**
1. **Calendar-First Design**: The scheduler module's FullCalendar interface is the heart of the system - sales creates estimates on the calendar, operations converts them to jobs, technicians see their daily assignments
2. **Multi-Tenant Architecture**: Each franchisee operates independently with their own users, customers, pricing, and schedules
3. **Offline-First Token Auth**: Once logged in, the session token persists in cookies, allowing the SPA to function even if the user refreshes the page (token is re-validated on bootstrap)
4. **Gulp Build Pipeline**: The `public/` folder contains source code that Gulp transforms (minifies, concatenates, injects) into deployable `client/` (dev), `test/`, or `prod/` builds

**Key Business Features:**
- **Scheduler**: Drag-and-drop calendar for scheduling estimates, jobs, meetings, and technician vacations
- **Job Management**: Track job status, before/after photos, e-signatures, invoice generation
- **Multi-Location Support**: Franchisees can manage multiple service locations with ZIP code routing
- **Reporting**: Revenue tracking, technician performance, job completion rates, sales pipeline analytics
- **User Management**: Role-based access (admin, franchisee owner, sales, technician) with granular UI state permissions

The application follows a **modular monolith** approach - each feature (scheduler, users, organizations, sales, reports) is an independent AngularJS module with its own controllers, services, and views, but they all share the core infrastructure (authentication, HTTP wrapper, notifications).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Setup

#### Prerequisites
- Node.js 6.x or higher (for npm)
- Bower 1.8.x (installed via npm)

#### Installation
```bash
# Install Node.js dependencies (Gulp and build tools)
npm install

# Install Bower dependencies (AngularJS, Bootstrap, jQuery, etc.)
bower install

# Run development build (outputs to client/ directory)
gulp

# Start file watcher for live rebuilds during development
gulp watch
```

#### Production Build
```bash
# Test environment build (minified, concatenated)
gulp test

# Production environment build (minified, concatenated)
gulp production
```

### Example: Adding a New Feature Module

```javascript
// 1. Create module definition in public/modules/myfeature/module.js
'use strict';

var MyFeatureConfiguration = function () {
    return {
        moduleName: "myfeature"
    };
}();

(function () {
    // Register module with main application
    ApplicationConfiguration.registerModule(MyFeatureConfiguration.moduleName);

    // Configure routing
    angular.module(MyFeatureConfiguration.moduleName)
        .config(["$stateProvider", function($stateProvider) {
            $stateProvider
                .state('core.layout.myfeature', {
                    url: '/myfeature',
                    templateUrl: '/modules/myfeature/views/list.client.view.html',
                    controller: 'MyFeatureController',
                    controllerAs: 'vm',
                    resolve: { factory: 'checkRouting' } // Require authentication
                });
        }]);
}());

// 2. Create controller in public/modules/myfeature/controllers/myfeature.client.controller.js
(function () {
    'use strict';

    angular.module(MyFeatureConfiguration.moduleName)
        .controller('MyFeatureController', ['HttpWrapper', 'Toaster', MyFeatureController]);

    function MyFeatureController(httpWrapper, toaster) {
        var vm = this;
        vm.items = [];

        vm.loadItems = function() {
            httpWrapper.get({
                url: '/myfeature/list',
                showOnSuccess: false
            }).then(function(response) {
                vm.items = response.data;
            });
        };

        // Initialize
        vm.loadItems();
    }
}());

// 3. Create service in public/modules/myfeature/services/myfeature.client.service.js
(function () {
    'use strict';

    angular.module(MyFeatureConfiguration.moduleName)
        .service('MyFeatureService', ['HttpWrapper', MyFeatureService]);

    function MyFeatureService(httpWrapper) {
        this.getItems = function() {
            return httpWrapper.get({
                url: '/myfeature/list',
                showOnSuccess: false
            });
        };

        this.createItem = function(item) {
            return httpWrapper.post({
                url: '/myfeature/create',
                data: item
            });
        };
    }
}());

// 4. Create view in public/modules/myfeature/views/list.client.view.html
<div class="page-content">
    <div class="page-bar">
        <h3>My Feature</h3>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div class="portlet light bordered">
                <div class="portlet-body">
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr ng-repeat="item in vm.items track by item.id">
                                <td>{{item.name}}</td>
                                <td>
                                    <button class="btn btn-sm btn-primary" 
                                            ng-click="vm.editItem(item)">
                                        Edit
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>

// 5. Update index.html to load the module
// Add this line in the module loading section (around line 66-71):
<script src="/modules/myfeature/module.js"></script>

// 6. Run gulp to rebuild
gulp
```

### Example: Making an Authenticated API Request

```javascript
// In any controller or service, inject HttpWrapper
angular.module('mymodule')
    .controller('MyController', ['HttpWrapper', 'Notification', function(httpWrapper, notification) {
        var vm = this;

        // GET request with automatic error handling
        vm.loadData = function() {
            httpWrapper.get({
                url: '/scheduler/jobs/123',
                showOnSuccess: false,  // Don't show success toaster
                showOnFailure: true    // Show error toaster on failure (default)
            }).then(function(response) {
                vm.job = response.data;
            });
        };

        // POST request with validation handling
        vm.saveJob = function(job) {
            httpWrapper.post({
                url: '/scheduler/jobs',
                data: job,
                showOnSuccess: true  // Shows success toaster
            }).then(
                function(response) {
                    // Success - response.data contains saved job
                    vm.job = response.data;
                },
                function() {
                    // Error - validation modal already shown by HttpWrapper
                    // Just handle any additional cleanup
                }
            );
        };

        // File upload
        vm.uploadPhoto = function(file) {
            httpWrapper.upload({
                url: '/scheduler/jobs/123/photos',
                data: {
                    file: file,
                    jobId: 123,
                    description: 'Before photo'
                }
            }).then(function(response) {
                vm.photos.push(response.data);
            });
        };

        // DELETE with confirmation
        vm.deleteJob = function(jobId) {
            notification.confirm(
                'Are you sure you want to delete this job?',
                function() {
                    // User confirmed
                    httpWrapper.delete({
                        url: '/scheduler/jobs/' + jobId
                    }).then(function() {
                        // Remove from local list
                        vm.jobs = vm.jobs.filter(function(j) { 
                            return j.id !== jobId; 
                        });
                    });
                },
                function() {
                    // User cancelled - do nothing
                }
            );
        };
    }]);
```

### Example: Using the Authentication Service

```javascript
// Login flow
angular.module('authentication')
    .controller('LoginController', [
        'UserAuthenticationService', 
        '$location', 
        '$rootScope',
        function(authService, $location, $rootScope) {
            var vm = this;

            vm.login = function() {
                authService.login(vm.username, vm.password)
                    .then(function(response) {
                        var userData = response.data;
                        var token = userData.token;

                        // Store token and identity in $rootScope and cookie
                        authService.setTokenAndIdentity($rootScope, userData, token);

                        // Redirect to dashboard
                        $location.path('/dashboard');
                    })
                    .catch(function() {
                        // Error toaster already shown by HttpWrapper
                        vm.loginFailed = true;
                    });
            };

            vm.logout = function() {
                authService.logout().then(function() {
                    $location.path('/login');
                });
            };
        }
    ]);

// Checking authentication in any controller
angular.module('mymodule')
    .controller('MyController', ['$rootScope', '$location', function($rootScope, $location) {
        var vm = this;

        // Check if user is authenticated
        if (!$rootScope.identity) {
            $location.path('/login');
            return;
        }

        // Access user details
        vm.currentUser = $rootScope.identity;
        vm.userName = $rootScope.identity.userName;
        vm.roleId = $rootScope.identity.roleId;
        vm.franchiseeId = $rootScope.identity.franchiseeId;

        // Check permissions (UIStates are loaded after identity)
        vm.canAccessFeature = function(stateName) {
            if (!$rootScope.UIStates) return false;
            return $rootScope.UIStates.data.some(function(state) {
                return state.name === stateName;
            });
        };
    }]);
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Service/Component | Method/Property | Description |
|-------------------|-----------------|-------------|
| **HttpWrapper** | `get(options)` | Execute GET request with automatic error handling and toaster notifications |
| **HttpWrapper** | `post(options)` | Execute POST request with validation modal support |
| **HttpWrapper** | `put(options)` | Execute PUT request for updates |
| **HttpWrapper** | `delete(options)` | Execute DELETE request |
| **HttpWrapper** | `upload(options)` | Multipart file upload with progress tracking |
| **UserAuthenticationService** | `login(userName, password)` | Authenticate user and return session token |
| **UserAuthenticationService** | `logout()` | End user session and clear identity |
| **UserAuthenticationService** | `getUserIdentity(token)` | Validate token and fetch user details |
| **UserAuthenticationService** | `setTokenAndIdentity($rootScope, identity, token)` | Store user session in $rootScope and cookie |
| **Notification** | `showAlert(message, onClose)` | Display modal alert dialog |
| **Notification** | `showValidations(validation)` | Display validation errors in modal |
| **Notification** | `confirm(message, onConfirm, onCancel)` | Display Yes/No confirmation dialog |
| **Toaster** | `show(message)` | Display success toaster notification (auto-dismiss) |
| **Toaster** | `error(message)` | Display error toaster notification (persistent) |
| **ApplicationConfiguration** | `registerModule(moduleName)` | Register new feature module with main application |
| **checkRouting** | (resolve guard) | Validate authentication token on route change, redirect to /login if invalid |
| **$rootScope** | `identity` | Current user object (userId, userName, roleId, franchiseeId, etc.) |
| **$rootScope** | `UIStates` | Array of authorized UI states for current user (for permission checks) |
| **APP_CONFIG** | `apiUrl` | Backend API base URL (default: `/api`) |
| **APP_CONFIG** | `clientTokenName` | Cookie name for session token (environment-specific) |
| **APP_CONFIG** | `defaultPageSize` | Default page size for paginated lists (100) |
| **Gulp** | `gulp` (default) | Clean and build development version to `client/` directory |
| **Gulp** | `gulp watch` | Watch source files and rebuild on changes |
| **Gulp** | `gulp test` | Build optimized test version to `test/` directory |
| **Gulp** | `gulp production` | Build optimized production version to `prod/` directory |

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Token is missing" / Infinite redirect to /login
**Cause**: Session cookie was deleted or expired, but $rootScope.identity still exists.  
**Fix**: The `customInterceptor` should auto-reload the page when this happens. If not, manually clear cookies and reload. Check that cookie domain matches the application domain.

### "Module 'mymodule' is not available"
**Cause**: Module script not loaded in index.html, or loaded in wrong order.  
**Fix**: 
1. Ensure module.js is loaded before controllers/services from that module
2. Run `gulp` to regenerate index.html with proper injection order
3. Check that module is registered with `ApplicationConfiguration.registerModule()`

### Calendar not rendering / Scheduler blank
**Cause**: FullCalendar initialization timing issue or missing API data.  
**Fix**: 
1. Check browser console for JavaScript errors
2. Verify `/scheduler/jobs` API returns valid data array
3. Ensure FullCalendar config in controller has `height: 'auto'` or specific pixel value
4. Check that `ui.calendar` module is loaded (defined in app.module.js dependencies)

### File upload fails with 413 or 400
**Cause**: File size exceeds server limit, or multipart encoding issue.  
**Fix**: 
1. Check backend API `maxRequestLength` in Web.config
2. Ensure file object is passed as `data: {file: file}` to `HttpWrapper.upload()`
3. Use `ng-file-upload` directive: `<input type="file" ngf-select="vm.upload($file)">`

### Gulp watch not detecting changes
**Cause**: Too many files being watched, or OS file descriptor limit.  
**Fix**: 
1. Stop and restart `gulp watch`
2. Exclude large directories like `node_modules` from watch (already excluded in gulpfile)
3. On Linux/Mac, increase file descriptor limit: `ulimit -n 10000`

### "Pending requests cancelled" errors in console
**Cause**: Expected behavior - `customInterceptor` cancels pending HTTP requests on route change to prevent race conditions.  
**Fix**: Not a bug. If you need a request to survive route changes, set `headers: {skipFullPageLoader: true}` in HttpWrapper options.

### Styles not loading after build
**Cause**: Gulp inject didn't find CSS files, or wrong order.  
**Fix**: 
1. Check that CSS files exist in `public/content/styles/`
2. Run `gulp clean` then `gulp` to force complete rebuild
3. Verify `injectStyles` path in gulpfile.js line 175-179 matches your CSS location

### Date/time picker shows wrong timezone
**Cause**: Client timezone offset sent in HTTP headers, but backend may not be handling it correctly.  
**Fix**: 
1. Check that backend reads `Timezoneoffset` and `TimeZoneName` headers from request
2. Verify `customInterceptor` is calculating offset correctly: `-(new Date().getTimezoneOffset())`
3. Use moment.js with `moment.tz.setDefault(vm.timezone)` for consistent timezone handling

### Modal dialogs not closing / stuck backdrop
**Cause**: AngularJS digest cycle issue or modal closed without using `$uibModalInstance.close()`.  
**Fix**: 
1. Always close modals via `$uibModalInstance.close()` or `$uibModalInstance.dismiss()`
2. Don't manipulate DOM directly - use AngularJS bindings
3. Check for JavaScript errors in modal controller that prevent proper cleanup

### Performance issues with large lists (scheduler with 500+ jobs)
**Cause**: AngularJS digest cycle recalculates bindings on every change.  
**Fix**: 
1. Use `track by item.id` in all `ng-repeat` directives
2. Disable two-way binding where not needed: `ng-bind` instead of `{{}}`
3. Use one-time binding syntax: `{{::vm.staticValue}}`
4. Consider pagination or virtual scrolling for lists over 100 items

<!-- END CUSTOM SECTION -->
