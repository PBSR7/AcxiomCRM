using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<ApplicationDbContext>();
        foreach (var role in new[] { "Admin", "Manager", "Sales Executive" })
            if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

        var accounts = new[]
        {
            (Email: "admin@acxiomcrm.com", Name: "System Admin", Role: "Admin"),
            (Email: "manager@acxiomcrm.com", Name: "Sales Manager", Role: "Manager"),
            (Email: "sales@acxiomcrm.com", Name: "Sales Executive", Role: "Sales Executive")
        };
        foreach (var a in accounts)
        {
            var user = await userManager.FindByEmailAsync(a.Email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = a.Email, Email = a.Email, FullName = a.Name, EmailConfirmed = true };
                await userManager.CreateAsync(user, "Acxiom@123");
                await userManager.AddToRoleAsync(user, a.Role);
            }
        }
        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer { CustomerCode = "CUS-1001", CustomerName = "Nova Retail", Email = "contact@novaretail.com", Phone = "9876543210", CompanyName = "Nova Retail Pvt Ltd", City = "Hyderabad", State = "Telangana", CreatedBy = "seed" },
                new Customer { CustomerCode = "CUS-1002", CustomerName = "BluePeak Systems", Email = "hello@bluepeak.com", Phone = "9123456780", CompanyName = "BluePeak Systems", City = "Bengaluru", State = "Karnataka", CreatedBy = "seed" });
            await db.SaveChangesAsync();
        }
        if (!await db.Leads.AnyAsync())
        {
            var sales = await userManager.FindByEmailAsync("sales@acxiomcrm.com");
            db.Leads.AddRange(
                new Lead { LeadCode = "LED-1001", LeadName = "Arjun Mehta", Email = "arjun@example.com", Phone = "9988776655", CompanyName = "Vertex Labs", Source = "Website", Status = "Qualified", Priority = "High", ExpectedValue = 750000, AssignedTo = sales?.Id ?? "" },
                new Lead { LeadCode = "LED-1002", LeadName = "Sneha Rao", Email = "sneha@example.com", Phone = "8877665544", CompanyName = "GrowthWorks", Source = "Referral", Status = "Contacted", Priority = "Medium", ExpectedValue = 350000, AssignedTo = sales?.Id ?? "" });
            await db.SaveChangesAsync();
        }
        if (!await db.Opportunities.AnyAsync())
        {
            var customer = await db.Customers.FirstAsync();
            var sales = await userManager.FindByEmailAsync("sales@acxiomcrm.com");
            db.Opportunities.AddRange(
                new Opportunity { OpportunityName = "Nova CRM Expansion", CustomerId = customer.CustomerId, Amount = 1250000, Stage = "Proposal", Probability = 70, ExpectedCloseDate = DateTime.UtcNow.Date.AddDays(25), AssignedTo = sales?.Id ?? "" },
                new Opportunity { OpportunityName = "Analytics Upgrade", CustomerId = customer.CustomerId, Amount = 600000, Stage = "Negotiation", Probability = 60, ExpectedCloseDate = DateTime.UtcNow.Date.AddDays(45), AssignedTo = sales?.Id ?? "" });
            await db.SaveChangesAsync();
        }
    }
}
