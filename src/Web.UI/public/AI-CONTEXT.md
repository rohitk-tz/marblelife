# Web.UI Public Assets

## Overview
The `/public` directory contains all client-side assets for the AngularJS Single Page Application, including modules, content assets, and the application entry point.

## Directory Structure

```
/public
├── /content            # Static assets (fonts, images, scripts, styles)
│   ├── /fonts         # Typography files
│   ├── /images        # UI imagery
│   ├── /scripts       # Third-party JavaScript libraries
│   └── /styles        # CSS/LESS stylesheets
├── /modules           # AngularJS application modules
│   ├── app.module.js  # Application bootstrap
│   ├── /authentication
│   ├── /core
│   ├── /organizations
│   ├── /reports
│   ├── /sales
│   ├── /scheduler
│   └── /users
```

## Entry Point

### app.module.js
The main application bootstrap file that:
- Initializes the ApplicationConfiguration object
- Registers all modules in proper dependency order
- Configures global Angular settings
- Sets up interceptors and providers
- Defines application-wide constants

```javascript
'use strict';

// Global configuration object
var ApplicationConfiguration = (function() {
  var applicationModules = [];
  
  return {
    registerModule: function(moduleName, dependencies) {
      angular.module(moduleName, dependencies || []);
      applicationModules.push(moduleName);
    },
    getModules: function() {
      return applicationModules;
    }
  };
})();

// Bootstrap the main application
angular.module('makalu', ApplicationConfiguration.getModules())
  .config(['$locationProvider', '$httpProvider', function($locationProvider, $httpProvider) {
    $locationProvider.html5Mode(false);
    $locationProvider.hashPrefix('!');
    
    // Configure HTTP interceptors
    $httpProvider.interceptors.push('authInterceptor');
  }])
  .constant('APP_CONFIG', {
    apiUrl: '/api',
    clientTokenName: 'session-token',
    dateFormat: 'MM/DD/YYYY'
  });
```

## Modules Organization

Each module follows a consistent structure:
- `module.js` - Module definition and route configuration
- `/controllers` - View controllers
- `/services` - Business logic and API integration
- `/directives` - Reusable UI components
- `/filters` - Data transformation functions (where applicable)
- `/views` - HTML templates

### Module Loading Order
1. **core** - Shared utilities, loaded first
2. **authentication** - User authentication
3. **organizations** - Organization management
4. **users** - User management
5. **scheduler** - Job scheduling
6. **sales** - Sales management
7. **reports** - Reporting features

## Content Assets

### Fonts (`/content/fonts`)
Custom typography for brand consistency:
- **Kievit Book** - Primary font family
  - Regular, Bold, Italic, Bold Italic variants
  - WOFF, TTF, EOT formats for cross-browser support

### Images (`/content/images`)
Organized by category:

**Flags** (`/flags`)
- Country flag icons for internationalization
- Used in country/region selectors
- Standard sizes: 16x16, 32x32

**Layout** (`/layout`)
- Header logos and branding
- Background images and textures
- Loading spinners
- Icon sets for UI elements

**Social** (`/social`)
- Social media integration icons
- Share buttons
- Review system badges

### Scripts (`/content/scripts`)
Third-party JavaScript libraries and utilities:

**Security & Encryption**
- `aes.js` - AES encryption implementation
- `CryptoJSCipher.js` - CryptoJS wrapper for cipher operations
- `angularjs-crypto.js` - Angular integration for cryptography

**UI Components**
- `metronic.js` - Metronic theme core functionality
- `layout.js` - Layout management and responsive behavior
- `bootstrap-hover-dropdown.js` - Enhanced dropdown interactions
- `date-time-picker-beatufier.js` - Date/time picker styling and behavior

**Integration**
All scripts are loaded via index.html or dynamically by modules as needed.

### Styles (`/content/styles`)
CSS and LESS files for styling:

**Structure**
- `app.css` - Main application styles
- `custom.css` - Project-specific customizations
- `theme.css` - Metronic theme styles
- Module-specific CSS files

**Styling Conventions**
- BEM methodology for CSS class naming
- Bootstrap grid system for layout
- Responsive breakpoints: xs (<768px), sm (≥768px), md (≥992px), lg (≥1200px)

## Asset Management

### Build Process
The Gulp build system manages assets:
1. **Concatenation** - Combines multiple JS/CSS files
2. **Minification** - Reduces file sizes for production
3. **Injection** - Automatically injects dependencies into index.html
4. **Versioning** - Cache-busting for production deployments

### CDN Integration
Some libraries loaded from CDN for performance:
- AngularJS core and modules
- jQuery and jQuery UI
- Bootstrap CSS/JS
- Font Awesome icons

### Library Management
- **Bower** (`bower.json`) - Frontend package manager
- **LibMan** (`libman.json`) - Microsoft Library Manager for additional CDN resources
- **npm** (`package.json`) - Development tools and build dependencies

## Performance Optimization

### File Loading Strategy
1. **Critical CSS** - Inlined in `<head>` for above-the-fold content
2. **Deferred JavaScript** - Non-critical scripts loaded asynchronously
3. **Image Optimization** - Compressed images with appropriate formats
4. **Font Loading** - Optimized font loading with font-display: swap

### Caching Strategy
```
Assets are cached with appropriate headers:
- Images: 30 days
- Fonts: 1 year
- Scripts/Styles: Version-based (cache busting)
```

### Module Lazy Loading
Modules can be loaded on-demand to reduce initial bundle size:
```javascript
$ocLazyLoad.load({
  name: 'scheduler',
  files: [
    '/modules/scheduler/module.js',
    '/modules/scheduler/services/*.js',
    '/modules/scheduler/controllers/*.js'
  ]
});
```

## Development Guidelines

### Adding New Modules
1. Create module directory under `/modules/{module-name}`
2. Create `module.js` with module definition and routes
3. Register module in `app.module.js`
4. Add controllers, services, views as needed
5. Update Gulp configuration to include new files

### Adding New Assets
**Images**
- Place in appropriate `/content/images` subdirectory
- Use optimized formats (WebP with fallbacks)
- Name descriptively: `feature-description-size.ext`

**Scripts**
- Add to `/content/scripts`
- Update Gulp configuration for bundling
- Document dependencies and usage

**Styles**
- Add to `/content/styles`
- Follow existing naming conventions
- Import in main stylesheet

### File Naming Conventions
- Controllers: `{feature}.client.controller.js`
- Services: `{feature}.client.service.js`
- Directives: `{feature}.client.directive.js`
- Filters: `{feature}.client.filter.js`
- Views: `{feature}.client.view.html`

## Security Considerations

### Content Security
- No inline scripts (CSP compliance)
- Sanitized user-generated content
- XSS prevention via Angular's built-in escaping
- HTTPS for all external resources

### Asset Integrity
- Subresource Integrity (SRI) for CDN resources
- Version pinning for dependencies
- Regular security updates for libraries

## Testing

### Asset Testing
- Verify all images load correctly
- Test font rendering across browsers
- Validate script loading order
- Check responsive behavior

### Module Testing
Each module should have:
- Unit tests for controllers and services
- Integration tests for directives
- E2E tests for critical user flows

## Browser Compatibility

### Supported Browsers
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+
- IE11 (with polyfills)

### Polyfills Required
For IE11 support:
- ES5-shim
- JSON3
- Promises
- Object.assign

## Deployment

### Production Build
```bash
# Install dependencies
npm install
bower install

# Run production build
gulp build --env production

# Output: /dist directory with optimized assets
```

### Asset Deployment
- Static assets served from web server or CDN
- Versioned filenames for cache invalidation
- Gzip compression enabled
- CDN configuration for global distribution

## Maintenance

### Updating Dependencies
```bash
# Update npm packages
npm update

# Update bower packages
bower update

# Audit for vulnerabilities
npm audit
```

### Adding New Third-Party Libraries
1. Install via bower or npm
2. Add to Gulp configuration
3. Update index.html references (if manual)
4. Document usage in this file

## Related Documentation
- See `/modules/{module-name}/AI-CONTEXT.md` for module-specific details
- See parent `/Web.UI/AI-CONTEXT.md` for application architecture
- See `/content/AI-CONTEXT.md` for detailed asset documentation
