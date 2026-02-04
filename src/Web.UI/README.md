<!-- AUTO-GENERATED: Header -->
# Web.UI Layer
> AngularJS Single Page Application
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

The Web.UI layer is the client-side application built with **AngularJS**. It communicates with the backend API to provide a responsive user interface for managing Customers, Jobs, Sales, and Franchisees.

**Tech Stack**:
-   **Framework**: AngularJS (1.x)
-   **Build**: Gulp
-   **Package Manager**: Bower (Libs), NPM (Tools)
-   **Styling**: Bootstrap / CSS

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### Prerequisites
-   Node.js (Legacy version recommended for Gulp 3 compatibility, e.g., Node 10/12/14)
-   Bower (`npm install -g bower`)
-   Gulp (`npm install -g gulp-cli`)

### Setup
```bash
# 1. Install Build Tools
npm install

# 2. Install Frontend Libraries
bower install

# 3. Build & Watch
gulp
```

### Running the App
The application is a pure static SPA. It must be hosted on a web server (IIS, Nginx, or a dev server).
-   **IIS Express**: Set `Web.UI` as the Startup Project (if configured as a website in solution).
-   **Standalone**: Use `http-server` or similar to serve the root directory.

### Configuration
-   **API URL**: Check `app.module.js` or `config.js` (if exists) to ensure the API endpoint points to your local backend (usually `localhost:port/api`).

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Troubleshooting -->
## 🔧 Troubleshooting

### "Gulp - ReferenceError: primordials is not defined"
-   **Cause**: Using Node 12+ with Gulp 3.
-   **Fix**: Degrading Node to v10/v11 OR using `npm-force-resolutions` to force `graceful-fs` update. (Recommended: Stick to older Node for this legacy project).

### "Injector Module Error"
-   **Cause**: A new JS file was added but not injected into `index.html`.
-   **Fix**: Run `gulp` again to trigger the `inject` task.

### "CORS Error"
-   **Cause**: Frontend running on port 8080, Backend on port 5000.
-   **Fix**: Ensure `Global.asax.cs` in the API project is setting `Access-Control-Allow-Origin: *`.
<!-- END AUTO-GENERATED -->
