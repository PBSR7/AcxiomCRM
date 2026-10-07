using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    private readonly UserManager<ApplicationUser> _users;

    public LeadsController(
        ApplicationDbContext db,
        IAuditService audit,
        UserManager<ApplicationUser> users)
    {
        _db = db;
        _audit = audit;
        _users = users;
    }

    public async Task<IActionResult> Index(
        string? search,
        string? status)
    {
        var q = _db.Leads.AsQueryable();

        if (!User.IsInRole("Admin") &&
            !User.IsInRole("Manager"))
        {
            var uid = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            q = q.Where(x => x.AssignedTo == uid);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(x =>
                x.LeadName.Contains(search) ||
                x.CompanyName.Contains(search) ||
                x.Email.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            q = q.Where(x => x.Status == status);
        }

        ViewBag.Search = search;
        ViewBag.Status = status;

        return View(
            await q
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync());
    }

    public IActionResult Create()
    {
        return View(new Lead
        {
            Status = "New",
            Priority = "Medium",
            Source = "Website"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lead model)
    {
        // Generate LeadCode before validation
        if (string.IsNullOrWhiteSpace(model.LeadCode))
        {
            model.LeadCode = await GenerateLeadCodeAsync();
        }

        // Normalize user input
        model.LeadName = model.LeadName?.Trim() ?? string.Empty;
        model.Email = model.Email?.Trim() ?? string.Empty;
        model.Phone = model.Phone?.Trim() ?? string.Empty;
        model.CompanyName = model.CompanyName?.Trim() ?? string.Empty;

        // Prevent duplicate email
        if (!string.IsNullOrWhiteSpace(model.Email) &&
            await _db.Leads.AnyAsync(
                x => x.Email == model.Email))
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "A lead with this email already exists.");
        }

        // Prevent duplicate phone
        if (!string.IsNullOrWhiteSpace(model.Phone) &&
            await _db.Leads.AnyAsync(
                x => x.Phone == model.Phone))
        {
            ModelState.AddModelError(
                nameof(model.Phone),
                "A lead with this phone number already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CreatedDate = DateTime.UtcNow;

        model.AssignedTo =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(model.Status))
        {
            model.Status = "New";
        }

        if (string.IsNullOrWhiteSpace(model.Priority))
        {
            model.Priority = "Medium";
        }

        if (string.IsNullOrWhiteSpace(model.Source))
        {
            model.Source = "Website";
        }

        _db.Leads.Add(model);

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Create",
            "Lead",
            model.LeadId.ToString(),
            null,
            model.LeadName);

        TempData["Success"] = "Lead created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var lead = await _db.Leads.FindAsync(id);

        if (lead == null)
        {
            return NotFound();
        }

        return View(lead);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Lead model)
    {
        if (id != model.LeadId)
        {
            return BadRequest();
        }

        model.LeadName = model.LeadName?.Trim() ?? string.Empty;
        model.Email = model.Email?.Trim() ?? string.Empty;
        model.Phone = model.Phone?.Trim() ?? string.Empty;
        model.CompanyName = model.CompanyName?.Trim() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(model.Email) &&
            await _db.Leads.AnyAsync(
                x => x.LeadId != id &&
                     x.Email == model.Email))
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Another lead already uses this email.");
        }

        if (!string.IsNullOrWhiteSpace(model.Phone) &&
            await _db.Leads.AnyAsync(
                x => x.LeadId != id &&
                     x.Phone == model.Phone))
        {
            ModelState.AddModelError(
                nameof(model.Phone),
                "Another lead already uses this phone number.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _db.Leads.FindAsync(id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.LeadName = model.LeadName;
        existing.Email = model.Email;
        existing.Phone = model.Phone;
        existing.CompanyName = model.CompanyName;
        existing.Source = model.Source;
        existing.Status = model.Status;
        existing.Priority = model.Priority;
        existing.ExpectedValue = model.ExpectedValue;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Update",
            "Lead",
            id.ToString(),
            null,
            existing.LeadName);

        TempData["Success"] = "Lead updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Convert(int id)
    {
        var lead = await _db.Leads.FindAsync(id);

        if (lead == null)
        {
            return NotFound();
        }

        var existing = await _db.Customers
            .FirstOrDefaultAsync(
                x => x.Email == lead.Email ||
                     x.Phone == lead.Phone);

        if (existing == null)
        {
            existing = new Customer
            {
                CustomerCode = await GenerateCustomerCodeAsync(),
                CustomerName = lead.LeadName,
                Email = lead.Email,
                Phone = lead.Phone,
                CompanyName = lead.CompanyName,
                CreatedBy = lead.AssignedTo,
                CreatedDate = DateTime.UtcNow,
                Status = "Active"
            };

            _db.Customers.Add(existing);

            await _db.SaveChangesAsync();
        }

        lead.Status = "Converted";

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Convert",
            "Lead",
            id.ToString(),
            "Qualified",
            "Customer:" + existing.CustomerId);

        TempData["Success"] =
            "Lead converted to customer successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var lead = await _db.Leads.FindAsync(id);

        if (lead == null)
        {
            return NotFound();
        }

        return View(lead);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lead = await _db.Leads.FindAsync(id);

        if (lead == null)
        {
            return NotFound();
        }

        lead.Status = "Lost";

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Delete/Close",
            "Lead",
            id.ToString());

        TempData["Success"] = "Lead marked as lost.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GenerateLeadCodeAsync()
    {
        string code;

        do
        {
            code = $"LED-{Random.Shared.Next(1000, 10000)}";
        }
        while (await _db.Leads.AnyAsync(
            x => x.LeadCode == code));

        return code;
    }

    private async Task<string> GenerateCustomerCodeAsync()
    {
        string code;

        do
        {
            code = $"CUS-{Random.Shared.Next(1000, 10000)}";
        }
        while (await _db.Customers.AnyAsync(
            x => x.CustomerCode == code));

        return code;
    }
}