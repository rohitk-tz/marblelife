# System Architecture

## 📐 Conceptual Diagram

```mermaid
graph TD
    User[Web Browser] -->|HTTPS| IIS[IIS Web Server]
    
    subgraph "DMZ / Web Tier"
        IIS -->|Serve Initial HTML/JS| WebUI[AngularJS Frontend]
        IIS -->|API Calls (JSON)| API[ASP.NET Web API]
    end

    subgraph "Application Tier"
        API -->|Resolve Services| Core[Core Business Logic]
        Core -->|DI| Infra[Infrastructure Layer]
        
        Infra -->|Read/Write| ORM[Entity Framework 6]
        Infra -->|Payment Gateway| AuthNet[Authorize.Net]
        Infra -->|Email| SMTP[SMTP Server]
    end

    subgraph "Data Tier"
        ORM -->|SQL| SQL[(SQL Server)]
    end

    subgraph "Background Processing"
        Jobs[Windows Service Jobs] -->|Read Queue| SQL
        Jobs -->|Poll| HomeAdvisor[HomeAdvisor Email]
        Jobs -->|Send| SMTP
    end
```

## 🔄 Data Flows

### 1. Job Scheduling Flow
1.  **Estimator** creates a `JobEstimate` in the Web UI.
2.  **API** calls `IEstimateService` in `Core.Scheduler`.
3.  **Core** saves data via `IUnitOfWork` / `ORM` to SQL.
4.  **Scheduler** converts Estimate to `Job` and creates `JobScheduler` entry.
5.  **Technician** receives notification (via `Jobs` polling service).

### 2. Billing & Payment Flow
1.  **Job** is marked "Complete".
2.  **System** generates an `Invoice` (`Core.Billing`).
3.  **Background Job** (`IInvoiceLateFeePollingAgent`) checks due dates.
4.  **Customer** pays via Web UI.
5.  **Infrastructure** calls `Authorize.Net`.
6.  **Success**: Records `Payment` and updates `Invoice` status.

## 🛡 Security Design
-   **Authentication**: Custom Token/Hash implementation.
-   **Authorization**: Role-Based (Admin, FranchiseeAdmin, Technician, SalesRep).
-   **Network**: 
    -   API and Web UI hosted on IIS.
    -   Database not exposed publicly.
    -   Background Jobs run within the secure network perimeter.

## 📦 Deployment Strategy
-   **Tool**: `msdeploy.exe` (Web Deploy).
-   **Environments**: QA and Production on AWS EC2 (Windows Server).
-   **Configs**: Environment-specific `Web.config` transformations.
