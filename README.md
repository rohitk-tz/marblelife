# Marblelife Franchise System

> **Application Status**: Active Legacy System (.NET + AngularJS)

## What is this?
The **Marblelife Franchise System** is the operational backbone for Marblelife franchises. It handles the entire lifecycle of the business: from receiving a lead from HomeAdvisor, to scheduling a technician for a stone restoration job, to collecting payment and paying royalties to HQ.

## 🚀 Getting Started

### Prerequisites
-   **Visual Studio** (2015 or later recommended for .NET Framework 4.x support).
-   **SQL Server** (LocalDB or Developer Edition).
-   **Node.js & npm** (Legacy version may be required for Gulp/Bower).

### Quick Start

#### 1. Database Setup
The system uses a custom migration tool, not standard EF Migrations.
1.  Navigate to `src/DatabaseDeploy`.
2.  Update `App.config` with your local connection string.
3.  Run the application to create the schema and seed initial data.

#### 2. Backend API
1.  Open `MarbleLife.sln` (if available) or `src/API/API.csproj`.
2.  Compile and Run.
3.  API runs on `localhost` (port configurable in IIS Express).

#### 3. Frontend (Web.UI)
The frontend is a legacy AngularJS app.
```bash
cd src/Web.UI
npm install
bower install
gulp test  # For development watch mode
```

## 🏗 Key Components
-   **Backend**: C# ASP.NET Web API 2.
-   **Frontend**: AngularJS.
-   **Database**: SQL Server.
-   **Jobs**: Windows Service for background emails and processing.

For deep documentation on a specific module, verify the `AI-CONTEXT.md` file in the respective folder.
