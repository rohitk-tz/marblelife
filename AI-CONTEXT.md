# MarbleLife (Makalu) - AI Context Documentation

## Project Overview

**MarbleLife** (internally known as "Makalu") is a comprehensive business management platform built with .NET Framework and AngularJS. The system manages franchisee operations, scheduling, billing, customer relationships, and reporting for a marble restoration and maintenance service business.

## Architecture

This is a **multi-tier enterprise application** following these architectural patterns:

### Technology Stack
- **Backend**: ASP.NET Web API (.NET Framework 4.5.2+)
- **Frontend**: AngularJS (Web.UI)
- **Database**: SQL Server (via Entity Framework ORM)
- **Dependency Injection**: Custom DI container
- **Background Processing**: Windows Services (Jobs)
- **Deployment**: IIS with MSDeploy

### Layer Architecture

```
┌─────────────────────────────────────────┐
│         Web.UI (AngularJS SPA)          │
│     Frontend Application Layer          │
└─────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│      API (ASP.NET Web API)              │
│   RESTful API Controllers & Areas       │
└─────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│          Core (Business Logic)          │
│   Domain Models, Services, Validators   │
└─────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│    Infrastructure (Data Access)         │
│   Repository Pattern, External APIs     │
└─────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│        ORM (Entity Framework)           │
│     Database Context & Mappings         │
└─────────────────────────────────────────┘
```

## Main Modules

### 1. **Core** (`/src/Core`)
Central business logic layer containing domain models and service implementations for:
- **Organizations**: Franchisee and company management
- **Sales**: Sales pipeline and customer management
- **Scheduler**: Appointment and job scheduling
- **MarketingLead**: Lead management and tracking
- **Billing**: Payment processing and invoicing
- **Reports**: Business intelligence and analytics
- **Users**: User management and authentication
- **Dashboard**: Aggregated data and metrics
- **Notification**: Multi-channel notification system
- **Review**: Customer review management
- **Geo**: Geographic and location services
- **AWS**: Amazon Web Services integration
- **ToDo**: Task management

### 2. **API** (`/src/API`)
RESTful Web API layer exposing HTTP endpoints organized by business areas:
- Controllers for each domain (Organizations, Sales, Scheduler, etc.)
- Request/Response ViewModels
- Authentication/Authorization attributes
- Dependency injection configuration
- API documentation templates

### 3. **Infrastructure** (`/src/Infrastructure`)
Data access and external service integration:
- Repository implementations
- Database operations
- Third-party API integrations (Stripe, AWS, etc.)
- Application configuration

### 4. **ORM** (`/src/ORM`)
Entity Framework data layer:
- DbContext configuration
- Entity mappings
- Database schema definitions
- Migration support

### 5. **Web.UI** (`/src/Web.UI`)
AngularJS single-page application:
- Modular architecture (authentication, organizations, sales, scheduler, etc.)
- Controllers, services, directives, and views
- Responsive UI components
- Client-side routing

### 6. **Jobs** (`/src/Jobs`)
Background processing services:
- Scheduled tasks (Windows Services)
- Batch operations
- Data synchronization
- Email/notification processing

### 7. **DatabaseDeploy** (`/src/DatabaseDeploy`)
Database deployment and migration tool:
- Schema creation and updates
- Data modifications
- Version control for database changes

### 8. **DependencyInjection** (`/src/DependencyInjection`)
Centralized IoC container configuration for dependency injection across all layers.

## Support Services

- **CalendarImportService**: Integration with external calendar systems
- **CustomerDataUpload**: Bulk customer data import functionality
- **FranchiseeMigration**: Data migration tools for onboarding franchisees
- **NotificationService**: Email/SMS notification background service
- **UpdateInvoiceItemInfo**: Utility for invoice data updates
- **CurrencyExchangeRateService** (ConsoleApplication1): Currency conversion service
- **ReviewSystemAPITest**: Testing suite for review system integration

## Key Design Patterns

1. **Repository Pattern**: Abstraction over data access (Infrastructure layer)
2. **Service Layer Pattern**: Business logic encapsulation (Core layer)
3. **DTO/ViewModel Pattern**: Data transfer between layers
4. **Dependency Injection**: Loose coupling and testability
5. **Area-based Routing**: Logical grouping of API controllers
6. **Domain-Driven Design**: Organized by business domains

## Development Workflow

### Building the Application
1. Open `Makalu.sln` in Visual Studio
2. Restore NuGet packages
3. Build solution (Debug/Release configuration)
4. Configure connection strings in app.config/web.config

### Running Locally
- **API**: Run API project (IIS Express or Self-hosted)
- **Web.UI**: 
  ```bash
  cd Web.UI
  npm install
  bower install
  gulp [test|production]
  ```
- **Background Services**: Run Jobs project as Windows Service or Console

### Deployment
See `/ReadMe.txt` for detailed deployment instructions:
- API: Published via Visual Studio Web Deploy
- Jobs: Deployed as Windows Services (stop before deploy)
- DatabaseDeploy: Execute with appropriate config (backup DB first)
- Web.UI: Build with Gulp, deploy via MSDeploy

## Database Structure

The application uses SQL Server with Entity Framework Code-First approach:
- Entities defined in Core layer (Domain folders)
- Mappings in ORM layer
- Migrations via DatabaseDeploy
- Database per environment (QA: taazaa_qa, Production: production)

## Security Considerations

- Authentication via custom token-based system
- Role-based authorization on API endpoints
- Secure payment processing via Stripe integration
- Connection string encryption
- Input validation at API and domain layers

## Integration Points

1. **Payment Gateway**: Stripe API for billing
2. **AWS Services**: S3 for file storage, SES for emails
3. **SMS Gateway**: Notification delivery
4. **Calendar Systems**: Calendar import/export
5. **Review Platforms**: Customer review aggregation

## Configuration Files

- `app.config` / `web.config`: Application settings per project
- `packages.config`: NuGet dependencies
- `package.json` (Web.UI): NPM dependencies
- `bower.json` (Web.UI): Frontend dependencies
- Connection strings per environment

## Documentation Structure

Each folder contains an `AI-CONTEXT.md` file providing:
- **Purpose**: What the folder contains
- **Key Components**: Important files and their roles
- **Dependencies**: Related modules and external libraries
- **Usage**: How to work with the code
- **AI Instructions**: Specific guidance for AI-assisted development

## Getting Started for AI Agents

When working with this codebase:
1. Understand the multi-tier architecture and layer responsibilities
2. Follow existing patterns (Repository, Service, DTO)
3. Use dependency injection for all dependencies
4. Add appropriate error handling and validation
5. Update relevant ViewModels when changing domain models
6. Test API endpoints after backend changes
7. Update Web.UI services/controllers for frontend changes
8. Document complex business logic
9. Follow C# coding standards (see StyleCop.Cache files)

## Human Developers - Quick Start

1. **Clone & Setup**:
   ```bash
   git clone <repository-url>
   cd marblelife/src
   # Open Makalu.sln in Visual Studio
   ```

2. **Configure Database**:
   - Update connection strings in web.config/app.config
   - Run DatabaseDeploy to initialize schema

3. **Build & Run**:
   - Build solution in Visual Studio
   - Start API project (F5)
   - In separate terminal: `cd Web.UI && npm install && gulp test`
   - Access UI at configured port

4. **Development**:
   - Backend changes: Core → Infrastructure → API layers
   - Frontend changes: Web.UI/public/modules
   - Database changes: DatabaseDeploy project

## Project History

- Originally developed as "Makalu" platform
- Multiple upgrade iterations (see UpgradeLog*.htm files)
- Evolved from ASP.NET MVC to Web API + AngularJS architecture
- Continuous enhancements for franchisee management features
