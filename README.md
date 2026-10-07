# AcxiomCRM

A production-oriented Customer Relationship Management (CRM) web application built for the Acxiom CRM assignment.

AcxiomCRM provides role-based access and end-to-end management of customers, leads, opportunities, follow-ups, dashboards, reports, audit logs, and REST APIs.

---

## 📌 Project Overview

AcxiomCRM is designed to support sales teams in managing the complete customer lifecycle.

The application supports three primary roles:

- **Admin** — Full system access
- **Manager** — Sales and CRM management
- **Sales Executive** — Access to assigned CRM records

The project demonstrates secure authentication, role-based authorization, CRUD workflows, client/server validation, CRM business rules, dashboard analytics, audit logging, REST API integration, and responsive UI.

---

## 🚀 Key Features

### 🔐 Authentication & Authorization

- ASP.NET Core Identity authentication
- Login, registration, and logout
- Secure password hashing
- Password policy
- Account lockout
- Role-based authorization
- Protected application routes
- Server-side authorization
- Admin, Manager, and Sales Executive roles

### 👥 Customer Management

- Create customers
- View customer records
- Search customers
- Edit customer information
- Deactivate customers
- Email and phone validation
- Duplicate email prevention
- Duplicate phone prevention
- Automatically generated customer codes
- Customer creation tracking

Example customer code:

```text
CUS-5832
```

### 🎯 Lead Management

- Create and manage leads
- Search leads
- Filter leads by status
- Assign leads to sales executives
- Lead status management
- Lead priority management
- Expected opportunity value
- Duplicate email prevention
- Duplicate phone prevention
- Lead conversion workflow
- Audit logging

Supported lead statuses:

```text
New
Contacted
Qualified
Unqualified
Converted
Lost
```

Lead workflow:

```text
Create
   ↓
Assign
   ↓
Contact
   ↓
Qualify
   ↓
Convert
   ↓
Customer / Opportunity
```

### 💰 Opportunity Management

- Create opportunities
- Assign opportunities
- Opportunity stage tracking
- Opportunity amount
- Probability tracking
- Expected close date
- Open / Won / Lost status
- Pipeline value calculation
- Weighted pipeline calculation
- Business-rule validation

Weighted pipeline:

```text
Weighted Pipeline = Amount × Probability / 100
```

### 📅 Follow-Up Management

- Create follow-ups
- Assign follow-ups
- Track follow-up status
- Planned follow-ups
- Follow-up dates
- Sales activity tracking

### 📊 Dashboard & Analytics

The dashboard provides a real-time overview of CRM performance.

#### KPI Metrics

- Total Customers
- Total Leads
- Open Leads
- Total Opportunities
- Open Opportunities
- Won Opportunities
- Lost Opportunities
- Total Pipeline Value
- Weighted Pipeline
- Pending Follow-Ups

#### Charts

- Lead Status Distribution
- Opportunity Stage Distribution

Charts are implemented using **Chart.js**.

### 👤 User & Role Management

Supported roles:

```text
Admin
Manager
Sales Executive
```

Example access model:

| Role | Access |
| --- | --- |
| Admin | Full system access |
| Manager | Sales and CRM management |
| Sales Executive | Assigned CRM records |

### 📝 Audit Logging

Important CRM operations are recorded in the audit log.

Audit information includes:

- User
- Action
- Entity Name
- Record ID
- Previous Value
- New Value
- Created Date
- IP Address

Example:

```text
Action: Create
Entity: Customer
Record ID: 15
User: Sales Executive
```

This provides traceability for important CRM operations.

### 🌐 REST API

The application provides REST API endpoints for CRM data.

API capabilities include:

- Customer data access
- Lead data access
- Opportunity data access
- Authentication and authorization
- DTO-based responses
- HTTP status codes
- Server-side validation

Example endpoints:

```http
GET  /api/customers
GET  /api/leads
GET  /api/opportunities
POST /api/customers
```

---

## 🏗️ Architecture

The project follows a layered architecture to keep the application modular and maintainable.

```text
Presentation Layer
        ↓
Controllers / Views
        ↓
Application Layer
        ↓
Business Logic / Services
        ↓
Data Access Layer
        ↓
Entity Framework Core
        ↓
SQLite Database
```

Cross-cutting concerns such as Identity/security and audit logging are integrated across the application.

### Project Structure

```text
AcxiomCRM/
│
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/
│
├── Controllers/
│   ├── CustomersController.cs
│   ├── LeadsController.cs
│   ├── OpportunitiesController.cs
│   ├── FollowUpsController.cs
│   ├── DashboardController.cs
│   └── ...
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DbSeeder.cs
│
├── Models/
│   ├── ApplicationUser.cs
│   ├── Customer.cs
│   ├── Lead.cs
│   ├── Opportunity.cs
│   ├── FollowUp.cs
│   └── ...
│
├── Services/
│   ├── AuditService.cs
│   └── ...
│
├── Views/
│   ├── Customers/
│   ├── Leads/
│   ├── Opportunities/
│   ├── FollowUps/
│   ├── Dashboard/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── ...
│
├── Migrations/
│
├── Program.cs
├── appsettings.json
├── .gitignore
├── AcxiomCRM.csproj
└── README.md
```

---

## 🛠️ Technology Stack

| Technology | Purpose |
| --- | --- |
| C# | Application programming language |
| ASP.NET Core MVC (.NET 8) | Web application framework |
| ASP.NET Core Identity | Authentication and authorization |
| Entity Framework Core | ORM and database access |
| SQLite | Relational database |
| Razor Views | Server-side UI |
| Bootstrap 5 | Responsive UI |
| Bootstrap Icons | UI icons |
| Chart.js | Dashboard charts |
| LINQ | Data querying |
| REST API | Programmatic CRM data access |
| Git | Version control |
| GitHub | Source code hosting |

---

## 🔒 Security

Security is an important part of the application.

### Authentication

ASP.NET Core Identity is used for:

- User authentication
- Password hashing
- Password policy
- Login management
- Logout
- Account lockout
- Identity-based authorization

### Authorization

Protected controllers and operations use:

```csharp
[Authorize]
```

Role-specific access is handled using:

```csharp
User.IsInRole("Admin")
User.IsInRole("Manager")
```

### Validation

Both client-side and server-side validation are implemented.

Validation includes:

- Required fields
- Email format
- Phone format
- String length
- Numeric ranges
- Date validation
- Duplicate records
- CRM business rules

### Anti-Forgery Protection

State-changing MVC requests use:

```csharp
[ValidateAntiForgeryToken]
```

This helps protect forms against Cross-Site Request Forgery (CSRF).

### Database Security

Entity Framework Core and LINQ are used for database access, avoiding unsafe raw SQL construction for normal application operations.

---

## 📋 Business Rules

### Customer

- Customer name is required
- Valid email is required
- Valid phone number is required
- Duplicate email is prevented
- Duplicate phone number is prevented

### Lead

- Lead name is required
- Valid email is required
- Valid phone number is required
- Valid lead status is required
- Valid priority is required
- Expected value must be within the allowed range
- Duplicate lead contact information is prevented

### Opportunity

- Amount must be greater than zero for active opportunities
- Probability must be between 0 and 100
- Expected close date cannot be in the past for active opportunities

### Follow-Up

- Follow-up date cannot be earlier than the current date for new/planned follow-ups

---

## 🗄️ Database

The application uses **SQLite** with **Entity Framework Core**.

Main entities include:

```text
ApplicationUser
Customer
Lead
Opportunity
FollowUp
AuditLog
```

Database schema is managed using Entity Framework Core migrations.

The application creates `acxiomcrm.db` automatically on first run when configured to do so.

---

## ⚙️ Installation & Setup

### Prerequisites

Install:

- .NET SDK 8.0 or later
- Git
- Visual Studio Code or Visual Studio

Verify the .NET installation:

```bash
dotnet --version
```

### Clone the Repository

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
cd AcxiomCRM
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Application

```bash
dotnet build
```

A successful build should display:

```text
Build succeeded.
```

### Database Setup

The project uses SQLite and Entity Framework Core.

If migrations need to be applied manually:

```bash
dotnet ef database update
```

If the Entity Framework CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Then run:

```bash
dotnet ef database update
```

### Run the Application

```bash
dotnet run
```

The application will be available at:

```text
http://localhost:5000
```

---

## 🔑 Demo Accounts

The application includes seeded demo accounts for testing.

| Role | Email | Password |
| --- | --- | --- |
| Admin | `admin@acxiomcrm.com` | `Acxiom@123` |
| Manager | `manager@acxiomcrm.com` | `Acxiom@123` |
| Sales Executive | `sales@acxiomcrm.com` | `Acxiom@123` |

> **Security:** These credentials are intended only for local assignment/demo use. Change seeded passwords before any real deployment and never use them for production systems.

---

## 🧪 Testing

### Authentication Checks

- Open a protected page without logging in
- Verify that unauthenticated users are redirected to login
- Login with a valid account
- Verify access to the dashboard
- Verify logout functionality

### Customer Testing

- Create a valid customer
- Test invalid email
- Test invalid phone number
- Test duplicate email
- Test duplicate phone
- Edit a customer
- Deactivate a customer
- Verify audit entries

### Lead Testing

- Create a lead
- Search leads
- Filter leads by status
- Update lead status
- Change priority
- Convert a qualified lead
- Mark a lead as lost
- Verify audit entries

### Opportunity Testing

- Create an opportunity
- Test invalid amount
- Test probability below 0
- Test probability above 100
- Test invalid expected close date
- Verify pipeline calculations
- Verify weighted pipeline

### Follow-Up Testing

- Create a follow-up
- Test invalid follow-up date
- Update follow-up status
- Verify pending follow-ups on dashboard

### Authorization Testing

Test each role independently:

```text
Admin
Manager
Sales Executive
```

Verify that users cannot access functionality outside their permitted scope.

### Audit

Perform operations such as:

```text
Create Customer
Update Customer
Create Lead
Update Lead
Convert Lead
Deactivate Customer
```

Verify that the corresponding audit entries are created.

### REST API

Test:

```http
GET /api/customers
GET /api/leads
GET /api/opportunities
```

Verify:

- Authentication
- Authorization
- HTTP response status
- JSON response
- Returned CRM data

---

## 📈 Dashboard Metrics

The dashboard calculates CRM metrics dynamically from the database.

### Pipeline Value

```text
Pipeline Value =
Sum of Amount for Open Opportunities
```

### Weighted Pipeline

```text
Weighted Pipeline =
Sum of (Amount × Probability / 100)
```

These metrics provide an overview of the potential sales pipeline.

---

## 🎯 CRM Workflow

The overall CRM workflow is:

```text
                    ┌───────────────┐
                    │     Lead      │
                    └───────┬───────┘
                            │
                            ▼
                    ┌───────────────┐
                    │    Assign     │
                    └───────┬───────┘
                            │
                            ▼
                    ┌───────────────┐
                    │   Contact     │
                    └───────┬───────┘
                            │
                            ▼
                    ┌───────────────┐
                    │   Qualify     │
                    └───────┬───────┘
                            │
                            ▼
                  ┌─────────┴─────────┐
                  │                   │
                  ▼                   ▼
           ┌─────────────┐     ┌──────────────┐
           │  Customer   │     │ Opportunity  │
           └─────────────┘     └──────────────┘
                  │                   │
                  └─────────┬─────────┘
                            ▼
                    ┌───────────────┐
                    │   Follow-Up   │
                    └───────────────┘
```

---

## 📊 Application Modules

| Module | Description |
| --- | --- |
| Authentication | Login, logout and Identity management |
| Dashboard | CRM KPIs and analytics |
| Customers | Customer lifecycle management |
| Leads | Lead capture, qualification and conversion |
| Opportunities | Sales pipeline management |
| Follow-Ups | Sales activity tracking |
| Users & Roles | Role-based access management |
| Audit Logs | Activity and change tracking |
| REST API | Programmatic CRM data access |
| Reports | CRM analytics and reporting |

---

## 🔄 Git Workflow

The project uses Git for version control.

Typical workflow:

```bash
git status
git add .
git commit -m "Update CRM functionality"
git push
```

---

## 📁 Important Files

| File | Purpose |
| --- | --- |
| `Program.cs` | Application configuration and dependency injection |
| `ApplicationDbContext.cs` | EF Core database context |
| `DashboardController.cs` | Dashboard metrics and analytics |
| `CustomersController.cs` | Customer management |
| `LeadsController.cs` | Lead management and conversion |
| `OpportunitiesController.cs` | Opportunity management |
| `FollowUpsController.cs` | Follow-up management |
| `AuditService.cs` | Audit logging |
| `ApplicationUser.cs` | Identity user model |
| `appsettings.json` | Application configuration |
| `AcxiomCRM.csproj` | Project dependencies and configuration |

---

## 💡 Design Principles

The project follows these principles:

- Separation of concerns
- Layered architecture
- Reusable services
- Server-side validation
- Client-side validation
- Role-based authorization
- Secure authentication
- Auditability
- Maintainable MVC structure
- Database abstraction through Entity Framework Core

---

## 📌 Project Status

### Status: Completed

Implemented functionality includes:

- Authentication and authorization
- Role-based access
- Customer management
- Lead management
- Lead conversion
- Opportunity management
- Follow-up management
- Dashboard analytics
- Pipeline calculations
- Audit logging
- REST API
- Validation
- SQLite database
- Entity Framework Core
- Responsive UI
- Search and filtering

---

## 👨‍💻 Author

SAI RAMANUJAM

GITAM Deemed to be University

---

## 📜 License

This project was developed as part of a technical assignment and is intended for evaluation and demonstration purposes.
