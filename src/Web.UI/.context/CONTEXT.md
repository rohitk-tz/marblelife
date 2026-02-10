<!-- AUTO-GENERATED: Header -->
# Web.UI — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2026-02-10T11:21:15+00:00
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
Web.UI is the AngularJS 1.5.8 Single Page Application (SPA) frontend for the MarbleLife franchise management system. It provides a comprehensive scheduling, job management, and reporting interface for managing marble and stone restoration services across multiple franchise locations. The application handles customer scheduling, technician assignment, estimate creation, job tracking, invoicing, and business analytics through an interactive calendar-based interface.

### Design Patterns
- **Module Pattern**: Each feature domain (scheduler, users, organizations, sales, reports, authentication) is isolated as an independent AngularJS module with its own controllers, services, views, and routing configuration
- **Factory/Service Layer**: Business logic and API communication are encapsulated in reusable services following the factory pattern (e.g., `HttpWrapper`, `UserAuthenticationService`, `Notification`)
- **Interceptor Pattern**: HTTP interceptors (`customInterceptor`, `loaderInterceptor`) handle cross-cutting concerns like authentication token injection, timezone headers, and loading state management
- **State-based Routing**: UI-Router manages complex nested view hierarchies with abstract parent states (`core.layout`) and child states for features
- **Resolve Guards**: Route-level authentication checks via `checkRouting` factory prevent unauthorized access
- **Gulp Build Pipeline**: Automated build process concatenates, minifies, and injects assets into index.html for development and optimized production deployments

### Data Flow
1. **Bootstrap**: `index.html` loads `app.module.js` which initializes the main `makalu` AngularJS application with vendor dependencies
2. **Module Registration**: Feature modules (authentication, core, scheduler, users, organizations, sales, reports) self-register via `ApplicationConfiguration.registerModule()` and inject `APP_CONFIG` constants
3. **Authentication**: Login via `UserAuthenticationService` sets session token in cookies, which `customInterceptor` automatically injects into all API request headers as `Token`
4. **Authorization**: `checkRouting` resolve guard validates token on every route, fetches user identity from `/users/identity/{token}`, and redirects to `/login` if invalid
5. **API Communication**: Controllers invoke services → services use `HttpWrapper` → `HttpWrapper` wraps `$http` with standardized error handling and toaster notifications → API responses return `{data, message}` payload structure
6. **State Management**: User identity and UI states stored in `$rootScope.identity` and `$rootScope.UIStates` for global access
7. **Build Process**: Gulp watches source files in `public/` → transforms scripts/styles → injects dependencies into index.html → outputs to `client/` (dev) or `prod/test` (optimized builds)

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

> Core JavaScript/AngularJS configuration objects and constants.

### Application Configuration
```javascript
// Main application module and vendor dependencies
var ApplicationConfiguration = {
    applicationModuleName: 'makalu',
    applicationModuleVendorDependencies: [
        'ngAnimate', 'ngCookies', 'ngSanitize', 'ui.router', 
        'ui.bootstrap', 'ngFileUpload', 'amChartsDirective', 
        'ui.slimscroll', 'fixed.table.header', 'ngRateIt', 
        'angularjs-dropdown-multiselect', 'angularSpinner', 
        'ui.calendar', 'ui.bootstrap.datetimepicker', 
        'colorpicker.module', 'angular.circular.datetimepicker', 'ngImgCrop'
    ],
    registerModule: function(moduleName) { /* registers feature module */ }
};

// Injected into all feature modules
APP_CONFIG = {
    apiUrl: '/api',                           // Backend API base URL
    clientTokenName: window.appTokenName,     // Environment-specific cookie name (dev/stage/test/prod-makalu-auth-token)
    serverTokenName: 'Token',                 // HTTP header name for auth token
    timezoneoffset: 'Timezoneoffset',        // HTTP header for timezone offset
    timeZoneName: 'TimeZoneName',             // HTTP header for timezone name
    defaultPageSize: 100,
    pagingOptions: [1, 5, 10, 15, 20, 25, 50, 100],
    maxSlabsCount: 4,                         // Max marble slabs per job
    maxPhoneCount: 10,
    maxOccurenceCount: 10,
    defaultClassTypeId: 4,
    primaryContact: 'Bruce Jordan',
    secondaryContact: 'Bob Heid',
    contactNumber: '407-330-6245',
    defaultServiceTypeIds: [1, 2, 3, 5, 6, 15],
    defaultClassTypeIds: [1, 2, 3, 4, 12, 13],
    defaultBookkeepingAmount: 250,
    defaultSalesPercentage: 2,
    defaultPayrollAmount: 50,
    defaultRecruitmentAmount: 19,             // per person (tech/sales)
    defaultOneTimeProjectAmount: 100,
    defaultNationalChargePercentage: 10,
    defaultFrequency: 32
};
```

### HTTP Methods Enum
```javascript
var HttpMethod = { 
    Get: 1, 
    Post: 2, 
    Put: 3, 
    Delete: 4 
};
```

### API Response Payload
```javascript
// Standard API response structure
{
    data: Object,              // Response payload (null on error)
    message: {
        message: String,       // Human-readable message
        messageType: Number    // 3 = error, other = success/info
    },
    modelValidation: {         // Present when validation fails
        errors: Array          // Validation error details
    },
    errorCode: Number          // 3 = unauthorized/no token
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### Module Registration API

#### `ApplicationConfiguration.registerModule(moduleName)`
- **Input**: `String` - Module name (e.g., 'scheduler', 'authentication')
- **Output**: None
- **Behavior**: Creates new AngularJS module, pushes to main app dependencies, injects APP_CONFIG constant into module
- **Side-effects**: Modifies global `angular.module(ApplicationConfiguration.applicationModuleName).requires` array

### HTTP Service Layer

#### `HttpWrapper.get(options)`
- **Input**: `{url, showOnSuccess, showOnFailure, data}` object
- **Output**: `Promise` resolving to API response payload
- **Behavior**: Executes GET request via `$http`, automatically handles success/error toaster notifications, cancels pending requests on route change
- **Side-effects**: Shows toaster notification unless `showOnSuccess`/`showOnFailure` disabled, starts/stops `secure-spinner`

#### `HttpWrapper.post(options)`
- **Input**: `{url, data, showOnSuccess, showOnFailure}` object
- **Output**: `Promise` resolving to API response payload
- **Behavior**: Executes POST request, displays validation modal if `modelValidation.errors` present
- **Side-effects**: Same as GET, plus potential validation modal popup

#### `HttpWrapper.put(options)`
- **Input**: Same as POST
- **Output**: `Promise`
- **Behavior**: Executes PUT request for updates
- **Side-effects**: Same as POST

#### `HttpWrapper.delete(options)`
- **Input**: Same as GET
- **Output**: `Promise`
- **Behavior**: Executes DELETE request
- **Side-effects**: Same as GET

#### `HttpWrapper.upload(options)`
- **Input**: `{url, data: {file, ...}}` with `ng-file-upload` file object
- **Output**: `Promise`
- **Behavior**: Multipart file upload using `Upload` service
- **Side-effects**: Progress tracking via `ng-file-upload`

### Authentication Service

#### `UserAuthenticationService.login(userName, password)`
- **Input**: `String` username, `String` password
- **Output**: `Promise<{data: {userId, token, ...}}>`
- **Behavior**: POSTs credentials to `/user/login`, returns user identity and session token
- **Side-effects**: None (caller must set token via `setTokenAndIdentity`)

#### `UserAuthenticationService.logout()`
- **Input**: None
- **Output**: `Promise`
- **Behavior**: Calls `/user/logout`, clears `$rootScope.identity` and `$rootScope.UIStates`
- **Side-effects**: Removes user session from server, reloads page

#### `UserAuthenticationService.getUserIdentity(sessionId)`
- **Input**: `String` - Session token from cookie
- **Output**: `Promise<{data: {userId, userName, roleId, franchiseeId, ...}}>`
- **Behavior**: Fetches user details from `/users/identity/{sessionId}`, used by route guard
- **Side-effects**: None

#### `UserAuthenticationService.setTokenAndIdentity($rootScope, identity, token)`
- **Input**: `$rootScope`, identity object, token string
- **Output**: None
- **Behavior**: Stores identity in `$rootScope.identity`, saves token to cookie via `$cookies.put()`
- **Side-effects**: Sets `{env}-makalu-auth-token` cookie, updates `$rootScope`

### Notification Service

#### `Notification.showAlert(message, onClose)`
- **Input**: `String` message, `Function` onClose callback (optional)
- **Output**: None
- **Behavior**: Opens modal dialog with message and OK button
- **Side-effects**: Blocks UI with modal backdrop

#### `Notification.showValidations(validation)`
- **Input**: `{errors: Array<{field, message}>}` validation object
- **Output**: None
- **Behavior**: Opens modal showing all validation errors in list format
- **Side-effects**: Blocks UI with modal

#### `Notification.confirm(message, onConfirm, onCancel)`
- **Input**: `String` message, confirm/cancel callbacks
- **Output**: None
- **Behavior**: Opens Yes/No confirmation dialog
- **Side-effects**: Blocks UI until user responds

### Toaster Service

#### `Toaster.show(message)`
- **Input**: `String` message
- **Output**: None
- **Behavior**: Displays success toaster notification (top-right, auto-dismiss)
- **Side-effects**: Overlays toaster on screen for 3 seconds

#### `Toaster.error(message)`
- **Input**: `String` message
- **Output**: None
- **Behavior**: Displays error toaster (red, stays until dismissed)
- **Side-effects**: Overlays persistent toaster

### Routing Guard

#### `checkRouting($rootScope, $location, UserAuthenticationService, APP_CONFIG, $cookies)`
- **Input**: AngularJS injected dependencies
- **Output**: `Promise<Boolean>` or `Boolean`
- **Behavior**: Checks if token exists in cookie, fetches user identity, redirects to `/login` if invalid, returns true if authenticated
- **Side-effects**: May redirect to `/login`, sets `$rootScope.identity` on success

### Gulp Build Tasks

#### `gulp` (default)
- **Behavior**: Runs `clean` then `inject` - copies source from `public/` to `client/`, injects scripts/styles into index.html
- **Side-effects**: Deletes `client/` and `tmp/` directories, regenerates deployment build

#### `gulp watch`
- **Behavior**: Watches `public/**/*.js`, `public/**/*.html`, `public/**/*.scss`, `public/lib/**/*` for changes and runs incremental inject
- **Side-effects**: Continuously rebuilds on file changes

#### `gulp test`
- **Behavior**: Sets `optimize=true`, `paths.dep='test'`, `tokenName='test-makalu-auth-token'`, runs production build with minification/concatenation
- **Side-effects**: Generates optimized `test/` output with `vendor.js`, `app.js` bundles

#### `gulp production`
- **Behavior**: Same as `test` but outputs to `prod/` with `prod-makalu-auth-token`
- **Side-effects**: Generates production-ready minified bundles

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Modules
- **[authentication](../authentication/.context/CONTEXT.md)** — User login, logout, session management, identity verification
- **[core](../core/.context/CONTEXT.md)** — Shared services (HttpWrapper, Notification, Toaster), layout controllers, routing configuration
- **[scheduler](../scheduler/.context/CONTEXT.md)** — Calendar-based job/estimate scheduling, technician assignments, vacation management
- **[users](../users/.context/CONTEXT.md)** — User CRUD operations, role management, franchisee user assignments
- **[organizations](../organizations/.context/CONTEXT.md)** — Franchisee organization management, location settings, business rules
- **[sales](../sales/.context/CONTEXT.md)** — Sales pipeline, leads, customer management, estimates/invoices
- **[reports](../reports/.context/CONTEXT.md)** — Business analytics, revenue reports, technician performance, job statistics

### External Dependencies (Bower)
- **angular@1.5.8** — Core AngularJS framework
- **angular-ui-router@0.3.1** — State-based routing with nested views
- **angular-bootstrap@2.0.2** (ui.bootstrap) — Bootstrap 3 components as AngularJS directives
- **bootstrap@3.3.7** — CSS framework for responsive layouts
- **jquery@3.1.0** — DOM manipulation and Bootstrap dependency
- **ng-file-upload@^12.2.8** — Multipart file uploads with progress tracking
- **amcharts@^3.15.2** + **amcharts-angular** — Interactive charts for reports/dashboard
- **toastr@~2.1.3** — Toast notification library
- **angular-ui-calendar@1.0.2** + **fullcalendar@3.6.2** — Scheduler calendar interface
- **angular-ui-bootstrap-datetimepicker@~2.5.1** — Date/time picker for job scheduling
- **angularjs-dropdown-multiselect@~1.11.8** — Multi-select dropdowns for filters
- **angular-spinner@~1.0.1** — Loading spinner overlay
- **font-awesome@4.6.3** — Icon library
- **moment** — Date manipulation (via daterangepicker dependency)
- **angular-bootstrap-colorpicker@^3.0.32** — Color picker for UI customization
- **angular-sanitize@1.5.8** — HTML sanitization for ng-bind-html
- **angular-cookies@1.5.8** — Cookie read/write for session management
- **angular-animate@1.5.8** — Animations for UI transitions

### External Dependencies (NPM - Build Tools)
- **gulp@^3.9.1** — Build automation task runner
- **gulp-inject@^4.1.0** — Injects script/style tags into HTML
- **gulp-concat@^2.6.0** — Concatenates JS files into bundles
- **gulp-uglify@^2.0.0** — Minifies JavaScript for production
- **gulp-clean-css@^2.0.13** — Minifies CSS for production
- **gulp-watch@^4.3.9** — File watcher for incremental builds
- **main-bower-files@^2.13.1** — Extracts main files from Bower dependencies based on overrides
- **del@^2.2.2** — Deletes files/directories for clean task
- **eslint@^3.7.1** + **gulp-eslint@^1.0.0** — Code linting (currently disabled in gulpfile)

### Backend API Integration
- **API Base URL**: Configurable via `config.apiUrl` (defaults to `/api`, typically proxied to backend service)
- **Authentication**: Token-based via HTTP header `Token: {sessionId}`, cookie name varies by environment
- **Timezone Handling**: Every request includes `Timezoneoffset` (minutes from UTC) and `TimeZoneName` headers
- **Expected Response Format**: `{data: Object, message: {message: String, messageType: Number}, modelValidation: {errors: Array}}`
- **Key Endpoints**:
  - `/user/login` (POST) - Authenticate user
  - `/user/logout` (GET) - End session
  - `/users/identity/{token}` (GET) - Validate token and get user details
  - Feature-specific endpoints in respective modules (e.g., `/scheduler/*`, `/organizations/*`)

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Build System Gotchas
- **Gulp 3.x**: Uses older Gulp version - incompatible with Gulp 4.x API changes
- **Bower Overrides**: Critical for controlling which files get injected - see `bower.json` overrides section for amcharts, bootstrap, font-awesome, jquery.inputmask
- **Script Injection Order**: `gulp-order` enforces load sequence (jQuery → Angular → vendor libs → app modules) - breaking this causes initialization errors
- **Environment Tokens**: `window.appTokenName` injected at build time determines cookie name - must match backend expectations

### AngularJS 1.5.8 Constraints
- **Pre-Component Architecture**: Uses controllers, not components (Angular 1.5 components not adopted here)
- **Digest Cycle Performance**: Large scheduler views with hundreds of calendar events can cause slow digests - consider `track by` in ng-repeat
- **Two-Way Binding**: Heavily used throughout - can cause unexpected side effects when objects are mutated

### Routing Architecture
- **Abstract States**: `core` and `core.layout` are abstract parents - cannot navigate directly, only to child states
- **Nested Views**: `core.layout` uses named views (header, footer, content) - all feature modules render in the `content` view
- **Default Routes**: Empty path and `/` both redirect to `/dashboard`

### HTTP Interceptor Behavior
- **Pending Request Cancellation**: All pending HTTP requests are cancelled on `$stateChangeSuccess` - prevents race conditions but can cause aborted request errors
- **Cache Busting**: HTML templates get `?p={version}` query param to prevent browser caching during development
- **Automatic Logout**: If token cookie is missing but `$rootScope.identity` exists, page automatically reloads to clear stale state

### Security Considerations
- **Client-Side Token Storage**: Session tokens stored in browser cookies - vulnerable to XSS if not HttpOnly (set by backend)
- **Route-Level Guards**: All protected routes use `resolve: {factory: 'checkRouting'}` - removes this to expose endpoints
- **No CSRF Protection**: Token-based auth in headers - ensure backend validates token origin
- **Timezone Leakage**: Client timezone sent in every request header - can be used for user fingerprinting

### Common Patterns
- **Modal Dialogs**: All use `$uibModal.open()` with `backdrop: 'static'` to prevent accidental closure
- **Service Naming**: Services end with `.client.service.js`, controllers with `.client.controller.js`, views with `.client.view.html`
- **Error Handling**: Always returns rejected promise (no error object) - check original response in `HttpWrapper` error callback for details
- **Loading Indicators**: `usSpinnerService` with key `secure-spinner` shows full-page overlay - stop it manually if not using HttpWrapper

<!-- END CUSTOM SECTION -->
