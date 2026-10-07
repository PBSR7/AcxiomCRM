# AcxiomCRM — Assignment Coverage

| Requirement | Implementation |
|---|---|
| Authentication | ASP.NET Core Identity login/register/logout |
| Password security | Identity adaptive hashing + password policy |
| Lockout | 5 failed attempts / 10 minute lockout |
| Authorization | Admin / Manager / Sales Executive roles |
| Customer CRUD | Customers controller + validation + duplicate checks |
| Lead workflow | Lead CRUD + status + qualified-to-customer conversion |
| Opportunity | Amount, probability, stage, close-date rules + weighted pipeline |
| Follow-up | Planned/completed workflow + date validation |
| Activities | Call / Meeting / Email / Task logging |
| Dashboard | Role-scoped KPI cards + Chart.js charts |
| Audit | Create/update/close/role/activity events |
| REST API | Customers, Leads and Opportunities with DTO-style responses |
| Reports | Pipeline, weighted pipeline, won opportunities, conversions |
| Client validation | DataAnnotations + unobtrusive jQuery validation |
| Server validation | ModelState + explicit business-rule checks |
| Anti-forgery | MVC POST forms use ValidateAntiForgeryToken |
| SQL injection protection | EF Core parameterized queries |
| UI | Responsive Bootstrap 5, search/filtering, status pills |
| Architecture | Controllers / Models / Data / Services / Views / APIs |

## Demo flow for evaluator

1. Open protected route while logged out → redirected to Identity login.
2. Login as `admin@acxiomcrm.com` → dashboard + Admin menu.
3. Create a customer with invalid email → client validation blocks submission.
4. Try duplicate email/phone → server validation rejects it.
5. Create opportunity with amount `0` → server rejects it.
6. Create opportunity with probability `101` → server rejects it.
7. Set an active opportunity close date in the past → server rejects it.
8. Create a follow-up dated before today → server rejects it.
9. Convert a Qualified Lead → customer is created and lead becomes Converted.
10. Open Audit Log → business actions are visible.
11. Login as Manager → management reports available, security administration restricted.
12. Login as Sales Executive → sales-scoped navigation/data.
13. Call `/api/customers` after authentication → JSON response.
14. Open Dashboard → Chart.js lead and opportunity visuals.
