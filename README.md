# AcxiomCRM

A production-oriented CRM demonstration built for the Acxiom CRM assignment.

## Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core + SQLite
- ASP.NET Core Identity + role-based authorization
- Bootstrap 5 + Bootstrap Icons
- Chart.js

## Implemented
- Identity login/register/logout, password policy and account lockout
- Roles: Admin, Manager, Sales Executive
- Dashboard KPIs + Chart.js lead/pipeline charts
- Customer, Lead, Opportunity and Follow-up workflows
- Client/server validation and CRM business rules
- Audit logging for important operations
- REST APIs for Customers, Leads and Opportunities
- Management reports
- Responsive UI and search/filtering

## Run locally
```bash
dotnet restore
dotnet run
```
The app creates `acxiomcrm.db` automatically on first run.

### Demo accounts
- Admin: `admin@acxiomcrm.com` / `Acxiom@123`
- Manager: `manager@acxiomcrm.com` / `Acxiom@123`
- Sales Executive: `sales@acxiomcrm.com` / `Acxiom@123`

Change seeded passwords before any real deployment.

## API examples
After login, try:
- `GET /api/customers`
- `GET /api/leads`
- `GET /api/opportunities`
- `POST /api/customers`

## Architecture
Controllers -> Services -> EF Core DbContext -> SQLite, with Identity/security and audit as cross-cutting services.
