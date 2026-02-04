# Source Code Directory - AI Context

## Purpose

This is the root source code directory for the MarbleLife (Makalu) platform, containing all application projects, services, and components.

## Structure

The `/src` folder contains 14 distinct projects organized in a multi-tier architecture:

### Core Projects
- **API**: RESTful Web API layer (ASP.NET Web API)
- **Core**: Business logic and domain models
- **Infrastructure**: Data access and external integrations
- **ORM**: Entity Framework database context and mappings
- **DependencyInjection**: IoC container configuration

### User Interface
- **Web.UI**: AngularJS single-page application

### Background Services
- **Jobs**: Windows Service for scheduled tasks
- **NotificationService**: Background notification processing
- **CalendarImportService**: Calendar synchronization service

### Data Management
- **DatabaseDeploy**: Database schema deployment tool
- **CustomerDataUpload**: Bulk data import utility
- **FranchiseeMigration**: Franchisee onboarding tool
- **UpdateInvoiceItemInfo**: Invoice data update utility

### Testing & Utilities
- **ReviewSystemAPITest**: Review system integration tests
- **ConsoleApplication1** (CurrencyExchangeRateService): Currency conversion service

## Solution File

**Makalu.sln** - Visual Studio solution file containing all projects with proper build dependencies and configurations.

## Key Files

- **Makalu.sln**: Main solution file
- **UpgradeLog*.htm**: Visual Studio upgrade history logs
- **package-lock.json**: NPM dependency lock file

## Build Order

The projects have the following dependency hierarchy:
1. **ORM** (Foundation layer - no dependencies)
2. **Core** (depends on ORM)
3. **Infrastructure** (depends on Core, ORM)
4. **DependencyInjection** (depends on Core, Infrastructure)
5. **API** (depends on all above)
6. **Jobs**, **Services** (depend on Core, Infrastructure)
7. **Utilities** (depend on Core, Infrastructure)

## For AI Agents

When working in this directory:
- Respect the dependency hierarchy when making changes
- Changes to Core may require updates in Infrastructure and API
- Always build the entire solution after structural changes
- Use the solution file for build operations, not individual projects
- Check upgrade logs if encountering version-related issues

## For Human Developers

### Initial Setup
```bash
# Open solution in Visual Studio
start Makalu.sln

# Or build from command line
msbuild Makalu.sln /p:Configuration=Release
```

### Project References
- Projects reference each other via project references, not DLL references
- All projects target .NET Framework 4.5.2+
- Shared dependencies managed via NuGet packages

### Configuration Management
- Each project has its own app.config/web.config
- Connection strings must be configured per environment
- API keys and secrets should use secure configuration

## Common Development Tasks

### Adding a New Feature
1. Define domain models in **Core/[Domain]/Domain**
2. Implement business logic in **Core/[Domain]/Impl**
3. Add repository methods in **Infrastructure**
4. Create API controllers in **API/Areas/[Domain]**
5. Update Web.UI for frontend changes

### Database Changes
1. Modify entities in Core
2. Update mappings in ORM if needed
3. Create migration script in DatabaseDeploy
4. Test migration on QA before production

### Adding Background Jobs
1. Create job class in Jobs/Impl
2. Configure job schedule
3. Register in dependency injection
4. Deploy as Windows Service
