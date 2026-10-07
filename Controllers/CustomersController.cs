using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public CustomersController(
        ApplicationDbContext db,
        IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Customers.AsQueryable();

        if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            q = q.Where(x => x.CreatedBy == uid);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(x =>
                x.CustomerName.Contains(search) ||
                x.Email.Contains(search) ||
                x.Phone.Contains(search) ||
                x.CompanyName.Contains(search));
        }

        ViewBag.Search = search;

        return View(
            await q
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync());
    }

    public IActionResult Create()
    {
        return View(new Customer
        {
            Status = "Active"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        // Generate the customer code before validation
        // because CustomerCode is part of the Customer model.
        if (string.IsNullOrWhiteSpace(model.CustomerCode))
        {
            model.CustomerCode = await GenerateCustomerCodeAsync();
        }

        // Normalize values
        model.Email = model.Email?.Trim() ?? string.Empty;
        model.Phone = model.Phone?.Trim() ?? string.Empty;
        model.CustomerName = model.CustomerName?.Trim() ?? string.Empty;

        // Check duplicate email
        if (!string.IsNullOrWhiteSpace(model.Email) &&
            await _db.Customers.AnyAsync(x => x.Email == model.Email))
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "A customer with this email already exists.");
        }

        // Check duplicate phone
        if (!string.IsNullOrWhiteSpace(model.Phone) &&
            await _db.Customers.AnyAsync(x => x.Phone == model.Phone))
        {
            ModelState.AddModelError(
                nameof(model.Phone),
                "A customer with this phone number already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CreatedBy =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        model.CreatedDate = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(model.Status))
        {
            model.Status = "Active";
        }

        _db.Customers.Add(model);

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Create",
            "Customer",
            model.CustomerId.ToString(),
            null,
            model.CustomerName);

        TempData["Success"] = "Customer created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.CustomerId)
        {
            return BadRequest();
        }

        model.Email = model.Email?.Trim() ?? string.Empty;
        model.Phone = model.Phone?.Trim() ?? string.Empty;
        model.CustomerName = model.CustomerName?.Trim() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(model.Email) &&
            await _db.Customers.AnyAsync(
                x => x.CustomerId != id &&
                     x.Email == model.Email))
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Another customer already uses this email.");
        }

        if (!string.IsNullOrWhiteSpace(model.Phone) &&
            await _db.Customers.AnyAsync(
                x => x.CustomerId != id &&
                     x.Phone == model.Phone))
        {
            ModelState.AddModelError(
                nameof(model.Phone),
                "Another customer already uses this phone number.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _db.Customers.FindAsync(id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.CustomerName = model.CustomerName;
        existing.Email = model.Email;
        existing.Phone = model.Phone;
        existing.CompanyName = model.CompanyName;
        existing.Address = model.Address;
        existing.City = model.City;
        existing.State = model.State;
        existing.Status = model.Status;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Update",
            "Customer",
            id.ToString(),
            null,
            model.CustomerName);

        TempData["Success"] = "Customer updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.Status = "Inactive";

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "Delete/Deactivate",
            "Customer",
            id.ToString());

        TempData["Success"] = "Customer deactivated successfully.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GenerateCustomerCodeAsync()
    {
        string code;

        do
        {
            code = $"CUS-{Random.Shared.Next(1000, 10000)}";
        }
        while (await _db.Customers.AnyAsync(x => x.CustomerCode == code));

        return code;
    }
}