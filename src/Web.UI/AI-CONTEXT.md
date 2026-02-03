<!-- AUTO-GENERATED: Header -->
# Web.UI Module Context
**Version**: 5b2236257a67dc37cf781165e929f8bb14373046
**Generated**: 2026-02-03T22:35:00+05:30
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## 🧠 Architectural Mental Model

### Core Responsibility
This is the **Single Page Application (SPA)** frontend for the application. It is built using **AngularJS (1.x)**.

### Design Patterns
-   **MVVM / MVC**: AngularJS Controllers act as ViewModels, binding data to HTML Views.
-   **Modular Architecture**: Code is split into feature modules (`modules/sales`, `modules/scheduler`) rather than by technical type.
-   **Dependency Injection**: Uses Angular's built-in DI for Services and Factories.
-   **Task Runner**: Uses **Gulp** for build automation (Watcher, Injection, Minification).
-   **Package Management**: Uses **Bower** for client-side libraries (legacy) and **NPM** for build tools.

### Build Flow
1.  **Dependencies**: `npm install` (Build tools) -> `bower install` (Frontend libs).
2.  **Gulp**:
    -   `gulp-inject`: Scans `public/**/*.js` and `public/**/*.css` and injects script tags into `index.html`.
    -   `gulp-watch`: Watches for changes to re-inject.
    -   `gulp-concat`/`uglify`: (Production) Bundles assets.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## 🧬 Type Definitions / Models

### Angular Module Structure
Located in `public/modules/{FeatureName}`.

-   `{Feature}.module.js`: Module definition (`angular.module('app.sales', [])`).
-   `controllers/`: Page logic (`SalesController.js`).
-   `services/`: API integration (`SalesService.js`).
-   `views/`: HTML Templates (`dashboard.html`).

### Key Modules

| Module | Purpose |
| :--- | :--- |
| **core** | Shared components, directives, and interceptors. |
| **authentication** | Login, Logout, Session management. |
| **scheduler** | The complex job calendar UI. |
| **sales** | Lead management and reporting dashboards. |

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## 🔌 Public Interfaces

### Main Entry Point (`app.module.js`)
-   Configures the **UI Router** (State-based routing).
-   Sets up HTTP Interceptors (for adding Auth Tokens).
-   Bootstraps the application.

### API Integration
-   Services use `$http` to communicate with the backend.
-   **Convention**: Endpoints match the API structure (`/api/Sales/...`). 
-   **Interceptors**: Automatically attach the `token` header to every request from local storage.

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## 🔗 Dependencies & Linking

### Internal
-   **[API](../API/AI-CONTEXT.md)** - The backend REST API consumed by this frontend.

### External
-   **AngularJS** (1.5.x+)
-   **Bootstrap** (Styling)
-   **UI Router** (Navigation)
-   **Gulp** (Build System)

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insight -->
## 💡 Developer Insights

### Legacy Stack Warning
-   **Bower**: This tool is deprecated. Be careful when adding new packages; you might need to use `bower` explicitly or migrate to `npm`.
-   **Gulp v3**: The `package.json` specifies Gulp `^3.9.1`. Gulp 3 syntax is different from Gulp 4 (e.g., array dependency syntax for tasks). Ensure you use a Node version compatible with Gulp 3 (Node 10/12 recommended, might fail on Node 16+).

### Development Workflow
1.  Run `npm install` & `bower install`.
2.  Run `gulp` (Default task).
3.  This starts a `watch` task.
4.  Open `index.html` in a browser (or via a local server like IIS Express or Live Server). **Note**: Accessing via `file://` protocol usually fails due to CORS/Security policies; serve it via HTTP.
<!-- END CUSTOM SECTION -->
