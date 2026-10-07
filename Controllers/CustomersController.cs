using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db; private readonly IAuditService _audit;
    public CustomersController(ApplicationDbContext db, IAuditService audit) { _db = db; _audit = audit; }
    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Customers.AsQueryable();
        if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
        { var uid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; q = q.Where(x => x.CreatedBy == uid); }
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.CustomerName.Contains(search) || x.Email.Contains(search) || x.Phone.Contains(search) || x.CompanyName.Contains(search));
        ViewBag.Search = search; return View(await q.OrderByDescending(x => x.CreatedDate).ToListAsync());
    }
    public IActionResult Create() => View(new Customer());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (await _db.Customers.AnyAsync(x => x.Email == model.Email || x.Phone == model.Phone)) ModelState.AddModelError("", "A customer with this email or phone already exists.");
        if (!ModelState.IsValid) return View(model);
        model.CustomerCode = "CUS-" + Random.Shared.Next(1000, 9999); model.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""; model.CreatedDate = DateTime.UtcNow;
        _db.Customers.Add(model); await _db.SaveChangesAsync(); await _audit.LogAsync("Create", "Customer", model.CustomerId.ToString(), null, model.CustomerName); TempData["Success"] = "Customer created successfully."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id) => await _db.Customers.FindAsync(id) is Customer c ? View(c) : NotFound();
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.CustomerId) return BadRequest();
        if (await _db.Customers.AnyAsync(x => x.CustomerId != id && (x.Email == model.Email || x.Phone == model.Phone))) ModelState.AddModelError("", "Another customer already uses this email or phone."); if (!ModelState.IsValid) return View(model); var existing = await _db.Customers.FindAsync(id); if (existing == null) return NotFound();
        existing.CustomerName=model.CustomerName; existing.Email=model.Email; existing.Phone=model.Phone; existing.CompanyName=model.CompanyName; existing.Address=model.Address; existing.City=model.City; existing.State=model.State; existing.Status=model.Status;
        await _db.SaveChangesAsync(); await _audit.LogAsync("Update", "Customer", id.ToString(), null, model.CustomerName); TempData["Success"]="Customer updated."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(int id) => await _db.Customers.FindAsync(id) is Customer c ? View(c) : NotFound();
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var c=await _db.Customers.FindAsync(id); if(c==null)return NotFound(); c.Status="Inactive"; await _db.SaveChangesAsync(); await _audit.LogAsync("Delete/Deactivate","Customer",id.ToString()); TempData["Success"]="Customer deactivated."; return RedirectToAction(nameof(Index));
    }
}
