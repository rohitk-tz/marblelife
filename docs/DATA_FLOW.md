# Marblelife Data Flow Documentation

## Overview
Data flows in the Marblelife system primarily revolve around the lifecycle of a **Franchise Operation**: from Lead Acquisition to Job Execution to Payment Collection. The system acts as a central data hub, synchronizing state between the Database, External APIs, and the User Interface.

---

## High-Level Data Flow

```mermaid
flowchart TB
    subgraph "Input Sources"
        USER[Franchisee/Admin User]
        HOME[HomeAdvisor Lead]
        CUST[Customer Payment]
    end

    subgraph "Processing Layer"
        API[Web API Controller]
        SVC[Core Services]
        VAL[Validation Logic]
        JOBS[Background Jobs]
    end

    subgraph "Storage Layer"
        DB[(SQL Server)]
    end

    subgraph "Output Layer"
        DASH[Dashboard UI]
        EMAIL[Email Notifications]
        RPT[PDF Reports]
    end
    
    USER -->|JSON| API
    CUST -->|Card Details| API
    HOME -->|Email| JOBS
    
    API --> VAL
    VAL --> SVC
    SVC --> DB
    
    JOBS -->|Parse| DB
    
    DB --> DASH
    DB -->|Poll| JOBS
    JOBS --> EMAIL
```

---

## Core Data Flow Scenarios

### 1. New Marketing Lead Flow

Data enters automatically from external sources (HomeAdvisor) or manual entry, and travels to the Franchisee's dashboard.

```mermaid
flowchart LR
    SOURCE[HomeAdvisor Email] -->|IMAP Poll| JOBS[Lead Polling Agent]
    JOBS -->|Parse & Map| LEAD_SVC[Marketing Lead Service]
    LEAD_SVC -->|Zip Code Lookup| GEO[Geo Service]
    GEO -->|Assign Franchisee| DB[(MarketingLeads Table)]
    
    DB -->|Read| API[Web API]
    API -->|Display| UI[Franchisee Dashboard]
    
    subgraph "Notification"
        DB -->|Trigger| NOTIFY[Notification Service]
        NOTIFY -->|Queue| EMAIL[Email Queue]
        EMAIL -->|Send| FRANCH[Franchisee Email]
    end
```

### 2. Job Estimation & Scheduling Flow

How a lead becomes a scheduled job.

```mermaid
flowchart TB
    UI[Dashboard] -->|Create Estimate| EST_SVC[Estimate Service]
    EST_SVC -->|Save| DB[(JobEstimate Table)]
    
    UI -->|Approve & Schedule| SCHED_SVC[Scheduler Service]
    SCHED_SVC -->|Read Estimate| DB
    SCHED_SVC -->|Create Job| JOB_SVC[Job Service]
    JOB_SVC -->|Insert| DB_JOB[(Job Table)]
    SCHED_SVC -->|Create Appointment| DB_SCHED[(JobScheduler Table)]
    
    DB_JOB -->|Sync| TECH[Technician View]
```

### 3. Payment Processing Flow

How money is collected and recorded.

```mermaid
flowchart TB
    CUSTOMER[User/Customer] -->|Submit Payment| API[Payment Controller]
    API -->|Validate| BILL_SVC[Billing Service]
    BILL_SVC -->|Process| GATEWAY[Authorize.Net Provider]
    
    GATEWAY -->|Response| BILL_SVC
    
    decision{Success?}
    BILL_SVC -- Yes --> REC_PAY[Record Payment]
    BILL_SVC -- No --> RET_ERR[Return Error]
    
    REC_PAY -->|Update Invoice| INV_SVC[Invoice Service]
    INV_SVC -->|Set Paid Status| DB[(Invoice Table)]
    REC_PAY -->|Insert Audit| DB_AUDIT[(AuditPayment Table)]
```

---

## Data Transformation Pipeline

1.  **Lead Parsing**: Raw HTML emails from HomeAdvisor are parsed using Regex/HtmlAgilityPack to extract Client Name, Phone, and Request Details. These are mapped to the `MarketingLead` domain entity.
2.  **Excel Uploads**: The `CustomerDataUpload` module reads Excel rows, validates them against existing Customer records (fuzzy matching on Name/Address), and transforms them into `FranchiseeSales` records.

---

## Integration Data Flows

### Authorize.Net (Payments)
-   **Direction**: Outbound
-   **Data**: Credit Card Token, Amount, Invoice ID
-   **Protocol**: HTTPS (XML/JSON API)

### ReviewPush (Reputation)
-   **Direction**: Outbound + Inbound
-   **Data**: Customer Email, Location ID -> Request Review
-   **Protocol**: REST API
