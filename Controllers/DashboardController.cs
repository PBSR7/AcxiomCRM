using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public DashboardController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _users.GetUserAsync(User);

        var isAdmin = User.IsInRole("Admin");
        var isManager = User.IsInRole("Manager");
        var uid = user?.Id ?? "";

        var customers = _db.Customers.AsQueryable();
        var leads = _db.Leads.AsQueryable();
        var opportunities = _db.Opportunities.AsQueryable();
        var followups = _db.FollowUps.AsQueryable();

        // Sales Executives can only see their own records
        if (!isAdmin && !isManager)
        {
            customers = customers.Where(x => x.CreatedBy == uid);
            leads = leads.Where(x => x.AssignedTo == uid);
            opportunities = opportunities.Where(x => x.AssignedTo == uid);
            followups = followups.Where(x => x.AssignedTo == uid);
        }

        // Get open opportunities first.
        // SQLite does not support Sum() directly on decimal values.
        var openOpportunityValues = await opportunities
            .Where(x => x.Status == "Open")
            .Select(x => new
            {
                x.Amount,
                x.Probability
            })
            .ToListAsync();

        // Calculate pipeline values in C#
        var pipelineValue = openOpportunityValues
            .Sum(x => x.Amount);

        var weightedPipeline = openOpportunityValues
            .Sum(x => x.Amount * x.Probability / 100m);

        var vm = new DashboardViewModel
        {
            TotalCustomers = await customers.CountAsync(),

            TotalLeads = await leads.CountAsync(),

            OpenLeads = await leads.CountAsync(
                x => x.Status != "Lost" &&
                     x.Status != "Converted"),

            TotalOpportunities = await opportunities.CountAsync(),

            OpenOpportunities = await opportunities.CountAsync(
                x => x.Status == "Open"),

            WonOpportunities = await opportunities.CountAsync(
                x => x.Status == "Won"),

            LostOpportunities = await opportunities.CountAsync(
                x => x.Status == "Lost"),

            PipelineValue = pipelineValue,

            WeightedPipeline = weightedPipeline,

            PendingFollowUps = await followups.CountAsync(
                x => x.Status == "Planned"),

            LeadStatus = await leads
                .GroupBy(x => x.Status)
                .Select(g => new ChartPoint(
                    g.Key,
                    g.Count()))
                .ToListAsync(),

            OpportunityStages = await opportunities
                .GroupBy(x => x.Stage)
                .Select(g => new ChartPoint(
                    g.Key,
                    g.Count()))
                .ToListAsync()
        };

        return View(vm);
    }
}

public record ChartPoint(string Label, int Value);

public class DashboardViewModel
{
    public int TotalCustomers { get; set; }

    public int TotalLeads { get; set; }

    public int OpenLeads { get; set; }

    public int TotalOpportunities { get; set; }

    public int OpenOpportunities { get; set; }

    public int WonOpportunities { get; set; }

    public int LostOpportunities { get; set; }

    public decimal PipelineValue { get; set; }

    public decimal WeightedPipeline { get; set; }

    public int PendingFollowUps { get; set; }

    public List<ChartPoint> LeadStatus { get; set; } = [];

    public List<ChartPoint> OpportunityStages { get; set; } = [];
}