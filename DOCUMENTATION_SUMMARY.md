# MarbleLife Documentation Summary

**Generated**: 2026-02-10  
**Commit**: adcd37bc8b2c00e9992088adbb128d0782f9dba6  
**Status**: ✅ Complete - All 15 modules documented

## Overview

This repository now contains comprehensive in-depth documentation for all modules using the module-context skill pattern. Each module has a `.context/` folder with AI/agent context, human-readable guides, and version tracking.

## Documentation Structure

```
src/
├── [ModuleName]/
│   └── .context/
│       ├── CONTEXT.md       # AI/agent technical reference
│       ├── OVERVIEW.md      # Human-readable developer guide
│       └── metadata.json    # Version tracking for staleness detection
```

## Documented Modules (15)

### Core Application Modules (7)

| Module | Files | Description | Context Link |
|--------|-------|-------------|--------------|
| **ORM** | 2 | Entity Framework 6 DbContext with MySQL, 100+ entity mappings, soft delete, audit tracking | [View Context](src/ORM/.context/CONTEXT.md) |
| **Core** | 1,146 | Domain models & business logic across 16 bounded contexts (Billing, Organizations, Sales, Users, Scheduler, etc.) | [View Context](src/Core/.context/CONTEXT.md) |
| **Infrastructure** | 15 | Service implementations: Repository<T>, UnitOfWork, LogService, PdfGenerator, billing services | [View Context](src/Infrastructure/.context/CONTEXT.md) |
| **DependencyInjection** | - | Unity IoC container with auto-registration via [DefaultImplementation] attribute | [View Context](src/DependencyInjection/.context/CONTEXT.md) |
| **API** | 80 | ASP.NET MVC 5 + Web API 2 with 11 Areas, token authentication, global filters | [View Context](src/API/.context/CONTEXT.md) |
| **Jobs** | - | Windows Service with Quartz.NET scheduler, 50+ automated background jobs | [View Context](src/Jobs/.context/CONTEXT.md) |
| **Web.UI** | 532 | AngularJS 1.5.8 SPA with Gulp build system, 7 feature modules | [View Context](src/Web.UI/.context/CONTEXT.md) |

### Console Utility Modules (8)

| Module | Description | Context Link |
|--------|-------------|--------------|
| **DatabaseDeploy** | MySQL schema migration utility with incremental tracking | [View Context](src/DatabaseDeploy/.context/CONTEXT.md) |
| **CalendarImportService** | iCalendar event synchronization with Google Calendar | [View Context](src/CalendarImportService/.context/CONTEXT.md) |
| **CustomerDataUpload** | Bulk customer data import with intelligent deduplication | [View Context](src/CustomerDataUpload/.context/CONTEXT.md) |
| **FranchiseeMigration** | One-time franchisee data seeding (50+ locations) | [View Context](src/FranchiseeMigration/.context/CONTEXT.md) |
| **NotificationService** | Legacy notification processor (superseded by Jobs module) | [View Context](src/NotificationService/.context/CONTEXT.md) |
| **ReviewSystemAPITest** | ReviewPush API testing and validation harness | [View Context](src/ReviewSystemAPITest/.context/CONTEXT.md) |
| **UpdateInvoiceItemInfo** | Invoice data correction utility with payment matching | [View Context](src/UpdateInvoiceItemInfo/.context/CONTEXT.md) |
| **ConsoleApplication1** | Currency exchange rate import for international franchisees | [View Context](src/ConsoleApplication1/.context/CONTEXT.md) |

## Documentation Features

### CONTEXT.md (AI/Agent Reference)
Each CONTEXT.md file contains:
- **Architectural Mental Model** - Core responsibility and design patterns
- **Data Flow** - Step-by-step request/response flow
- **Type Definitions** - Complete schemas and interfaces
- **Public Interfaces** - API documentation with inputs/outputs/side-effects
- **Dependencies** - Internal module links and external package references
- **Developer Insights** - Implementation details, gotchas, best practices

### OVERVIEW.md (Human-Readable Guide)
Each OVERVIEW.md file contains:
- **Overview** - Purpose explanation with analogies
- **Setup** - Installation and configuration instructions
- **Usage Examples** - Runnable code snippets
- **API Summary** - Quick reference tables
- **Troubleshooting** - Common issues and solutions

### metadata.json (Version Tracking)
Each metadata.json file contains:
- **Commit Hash** - For git-diff based staleness detection
- **File Count** - Number of source files in module
- **Changed Files** - List of modified files in last update
- **Generation Timestamp** - When documentation was created/updated

## Technology Stack

### Backend
- ASP.NET MVC 5.2.3 + Web API 2
- .NET Framework 4.5.2
- Entity Framework 6.1.3
- MySQL 6.9.9
- Unity IoC 4.0.1
- FluentValidation 6.2.1
- Quartz.NET (job scheduling)
- NLog 4.3.7 (logging)
- Authorize.Net 1.9.0 (payments)
- wkhtmltopdf (PDF generation)

### Frontend
- AngularJS 1.5.8
- UI-Router
- Gulp 3.9.1
- Bower
- Bootstrap, jQuery, FullCalendar, AmCharts

## Architecture Patterns Documented

- **Domain-Driven Design** - Bounded contexts per business domain
- **Clean Architecture** - Layered approach (Domain → Infrastructure → Application)
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Factory Pattern** - Object creation and ViewModel ↔ Domain mapping
- **Service Pattern** - Business logic encapsulation
- **Dependency Injection** - Unity container with auto-registration
- **Soft Delete Pattern** - Logical deletes with audit trails
- **FluentValidation** - Composable validation rules
- **MVC Areas** - Feature-based organization

## Key Statistics

- **Modules Documented**: 15 (100%)
- **Documentation Files**: 45 total
  - CONTEXT.md files: 15
  - OVERVIEW.md files: 15
  - metadata.json files: 15
- **Source Files Analyzed**: ~2,000+
- **Lines of Documentation**: ~15,000+
- **Git Commits**: 8
- **Branch**: copilot/generate-in-depth-documentation

## Use Cases

This documentation is designed for:

### 🤖 AI Agents & Coding Assistants
- Complete system context for accurate code generation
- Architecture patterns for following best practices
- API references for seamless integration
- Dependency understanding for impact analysis

### 👥 Developer Onboarding
- System understanding from day one
- Practical examples and workflows
- Troubleshooting common issues
- Technology stack comprehension

### 🔧 Maintenance & Refactoring
- Architecture reviews and assessments
- Technical debt identification
- Module dependency mapping
- Code quality improvements

### 📈 Future Updates
- Staleness detection via git-diff comparison
- AUTO-GENERATED section updates
- CUSTOM SECTION preservation for manual notes
- Incremental documentation maintenance

## Update Process

To update documentation when code changes:

1. **Staleness Check**: Compare metadata.json commit hash with current HEAD
2. **Detect Changes**: Run `git diff <hash> HEAD -- <module-path>`
3. **Update Sections**: Modify AUTO-GENERATED sections only
4. **Preserve Custom**: Keep CUSTOM SECTION markers intact
5. **Update Metadata**: Save new commit hash to metadata.json

Example:
```bash
# Check staleness for ORM module
cd src/ORM
OLD_HASH=$(jq -r .last_commit .context/metadata.json)
git diff --name-only $OLD_HASH HEAD -- .
# If changes detected, regenerate documentation
```

## Contributing

When adding new modules or making significant changes:
1. Follow the module-context skill template structure
2. Create `.context/` folder with all three files (CONTEXT.md, OVERVIEW.md, metadata.json)
3. Use AUTO-GENERATED and CUSTOM SECTION markers
4. Include commit hash in metadata.json
5. Link to other modules using relative paths

## Documentation Maintenance

The documentation follows these principles:
- **Never duplicate code** - Reference, don't replicate
- **Explain patterns, not implementation** - Focus on "why" over "what"
- **Use concrete examples** - Runnable code snippets
- **Keep it current** - Use metadata.json for staleness tracking
- **Preserve manual additions** - CUSTOM SECTION markers protect user content

---

**Generated by**: module-context skill  
**Template Version**: 1.1  
**Last Updated**: 2026-02-10
