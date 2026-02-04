# Marblelife System Context

## 🔭 Project Overview
**Marblelife** is a comprehensive Franchisee Management System. It allows the HQ to manage franchisees, and franchisees to manage their business operations including:
-   **CRM**: Leads (HomeAdvisor), Customers, and Contacts.
-   **Field Service**: Scheduling Jobs, Technicians, and Work Orders.
-   **Financials**: Invoicing, Payments (Credit Card/check), and Royalty Reporting.
-   **Business Intelligence**: Growth tracking and Product Mix reports.

## 🏗 High-Level Architecture
-   **Frontend**: AngularJS 1.x (managed via `Gulp`, `Bower`, `npm`).
-   **Backend**: ASP.NET Web API 2 (.NET Framework).
-   **Persistence**: SQL Server accessed via Entity Framework 6 (Code First).
-   **Background Processing**: Windows Services (`src/Jobs`) using Quartz.NET.
-   **Deployment**: IIS via Web Deploy (`msdeploy.exe`).

## 🗺 Module Navigation

| Module | Responsibility | Link |
| :--- | :--- | :--- |
| **Core** | **Business Logic** & Domain Entities. Contains sub-modules (Billing, Sales, etc.). | [AI Context](./src/Core/AI-CONTEXT.md) |
| **API** | REST API Endpoints & Auth. | [AI Context](./src/API/AI-CONTEXT.md) |
| **Web.UI** | AngularJS Frontend Application. | [AI Context](./src/Web.UI/AI-CONTEXT.md) |
| **Jobs** | Background Tasks & Email Dispatch. | [AI Context](./src/Jobs/AI-CONTEXT.md) |
| **Infrastructure** | Implementations of Core Interfaces (Data, Logging, PDF). | [AI Context](./src/Infrastructure/AI-CONTEXT.md) |
| **ORM** | Entity Framework Mappings & Context. | [AI Context](./src/ORM/AI-CONTEXT.md) |
| **DatabaseDeploy** | Custom SQL Migration Tool. | [AI Context](./src/DatabaseDeploy/AI-CONTEXT.md) |

### Core Sub-Modules
| Sub-Module | Responsibility | Link |
| :--- | :--- | :--- |
| `Core.Billing` | Invoices, Payments, Gateways. | [AI Context](./src/Core/Billing/AI-CONTEXT.md) |
| `Core.Scheduler` | Calendar, Jobs, Estimates. | [AI Context](./src/Core/Scheduler/AI-CONTEXT.md) |
| `Core.Organizations` | Franchisee Hierarchy & RBAC. | [AI Context](./src/Core/Organizations/AI-CONTEXT.md) |
| `Core.Sales` | Customer CRM & Data Import. | [AI Context](./src/Core/Sales/AI-CONTEXT.md) |
| `Core.Notification` | Email/SMS Dispatch Logic. | [AI Context](./src/Core/Notification/AI-CONTEXT.md) |

## 🛠 Developmental Standards
-   **Dependency Injection**: Unity Container (`src/DependencyInjection`).
-   **Logging**: Custom `ILogService` implementation (likely Log4Net or similar).
-   **Auth**: Custom Token/Hash mechanism (not standard Identity).
-   **Testing**: Basic Unit Tests present, plus manual "Sandbox" consoles (`ReviewSystemAPITest`).

## ⚡ Quick Context Snippets
-   **Database**: Designed for Multi-Tenancy (Franchisee-based).
-   **Legacy**: Uses `DomainBase` for all entities. `IsNew`, `IsDeleted` flags are ubiquitous.
-   **Deploy**: Heavily reliant on `msdeploy.exe` and `deploy.bat` scripts.
-   **Config**: `App.confg` / `Web.config` controlled via `ISettings`.
