# Marblelife Sequence Diagrams

## Overview
This document details the interaction sequences for critical system workflows, highlighting the interplay between the AngularJS frontend, the Web API, the Core Services, and External Providers.

---

## 1. Authentication Flow

The system uses a custom token/hash based authentication mechanism rather than standard ASP.NET Identity or OAuth 2.0.

```mermaid
sequenceDiagram
    autonumber
    participant User
    participant WebUI as AngularJS App
    participant API as AccountController
    participant Svc as UserLoginService
    participant DB as SQL Database

    User->>WebUI: Enters Username/Password
    WebUI->>API: POST /api/Account/Login
    API->>Svc: ValidateCredentials(user, pass)
    Svc->>DB: Fetch UserLogin by Username
    DB-->>Svc: UserLogin Record (Hash + Salt)
    
    loop Hash Validation
        Svc->>Svc: ComputeHash(Pass + Salt)
        Svc->>Svc: Compare with Stored Hash
    end

    alt Invalid Credentials
        Svc-->>API: Authentication Failed
        API-->>WebUI: 401 Unauthorized
    else Valid Credentials
        Svc->>DB: Update LastLoginDate
        Svc-->>API: User Context
        API->>API: Generate Auth Token
        API-->>WebUI: JSON { Token, Role, FranchiseeId }
        WebUI-->>User: Redirect to Dashboard
    end
```

## 2. Global Dashboard Data Load

Fetching the KPIs for the main Franchisee Dashboard.

```mermaid
sequenceDiagram
    autonumber
    participant UI as Dashboard Controller
    participant API as DashboardWebAPI
    participant Facade as DashboardService
    participant Org as FranchiseeService
    participant Sales as SalesService
    participant DB as Database

    UI->>API: GET /api/Dashboard/GetData
    API->>Facade: GetDashboardData(CurrentUserId)
    
    par Parallel Data Fetching
        Facade->>Org: GetFranchiseeDetails()
        Org->>DB: Select * from Franchisee
        
        Facade->>Sales: GetMonthlyRevenue()
        Sales->>DB: Sum(Sales) Group By Month
        
        Facade->>Sales: GetOpenLeadCount()
        Sales->>DB: Count(Leads) where Status='New'
    end
    
    DB-->>Facade: Return Data Sets
    Facade->>Facade: Aggregate into ViewModel
    Facade-->>API: DashboardViewModel
    API-->>UI: JSON Response
```

## 3. Background Lead Ingestion

How emails become leads without user interaction.

```mermaid
sequenceDiagram
    autonumber
    participant Job as Windows Service (Quartz)
    participant Poller as HomeAdvisorPollingAgent
    participant Mail as IMAP Server
    participant Parser as HomeAdvisorParser
    participant DB as Database

    Job->>Poller: Execute() (Every 5 mins)
    Poller->>Mail: Fetch Unseen Emails
    Mail-->>Poller: List<MailMessage>

    loop For Each Email
        Poller->>Parser: Parse(Body)
        Parser->>Parser: Regex Extraction (Name, Phone, Zip)
        
        alt Parse Success
            Parser-->>Poller: MarketingLead Model
            Poller->>DB: Insert into MarketingLeads
            Poller->>DB: Insert into Notifications (New Lead)
        else Parse Fail
            Poller->>DB: Log Error
        end
    end
```

## 4. Payment Transaction Flow

Processing a Credit Card payment for an Invoice.

```mermaid
sequenceDiagram
    autonumber
    participant UI as Payment Page
    participant API as PaymentController
    participant Svc as PaymentService
    participant Gateway as Authorize.Net
    participant DB as Database

    UI->>API: POST /Payment/Process
    API->>Svc: ProcessPayment(InvoiceId, CardToken)
    
    Svc->>DB: Get Invoice Details
    DB-->>Svc: Invoice (Amount, Customer)
    
    Svc->>Gateway: Charge(Amount, Token)
    
    alt Gateway Approved
        Gateway-->>Svc: Transaction ID, Auth Code
        Svc->>DB: Insert Payment Record
        Svc->>DB: Update Invoice Status = Paid
        Svc-->>API: Success
        API-->>UI: Display Receipt
    else Gateway Declined
        Gateway-->>Svc: Error Code, Message
        Svc-->>API: Throw PaymentFailedException
        API-->>UI: Display Error Message
    end
```

## 5. Error Handling & Logging Flow

What happens when code throws an exception.

```mermaid
sequenceDiagram
    autonumber
    participant Code as Service Layer
    participant API as Web API Filter
    participant Logger as LogService
    participant File as Log4Net File
    participant DB as ErrorLog Table

    Code->>Code: Throw Exception!
    Code-->>API: Bubble Up
    
    API->>API: Catch in ExceptionFilterAttribute
    
    par Log to File
        API->>Logger: LogError(ex)
        Logger->>File: Write Stack Trace
    and Log to DB
        API->>DB: Insert ErrorLog Entry
    end
    
    API-->>Code: Return 500 Internal Server Error
    Code-->>User: Generic Error Message
```
