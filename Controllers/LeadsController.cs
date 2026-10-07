using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly IAuditService _audit; private readonly UserManager<ApplicationUser> _users;
    public LeadsController(ApplicationDbContext db, IAuditService audit, UserManager<ApplicationUser> users){_db=db;_audit=audit;_users=users;}
    public async Task<IActionResult> Index(string? search,string? status)
    {
        var q=_db.Leads.AsQueryable(); if(!User.IsInRole("Admin")&&!User.IsInRole("Manager")){var uid=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;q=q.Where(x=>x.AssignedTo==uid);}
        if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.LeadName.Contains(search)||x.CompanyName.Contains(search)||x.Email.Contains(search)); if(!string.IsNullOrWhiteSpace(status))q=q.Where(x=>x.Status==status); ViewBag.Search=search;ViewBag.Status=status; return View(await q.OrderByDescending(x=>x.CreatedDate).ToListAsync());
    }
    public IActionResult Create()=>View(new Lead());
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Create(Lead model){if(!ModelState.IsValid)return View(model);model.LeadCode="LED-"+Random.Shared.Next(1000,9999);model.CreatedDate=DateTime.UtcNow;model.AssignedTo=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??"";_db.Leads.Add(model);await _db.SaveChangesAsync();await _audit.LogAsync("Create","Lead",model.LeadId.ToString(),null,model.LeadName);TempData["Success"]="Lead created.";return RedirectToAction(nameof(Index));}
    public async Task<IActionResult>Edit(int id)=>await _db.Leads.FindAsync(id) is Lead l?View(l):NotFound();
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Edit(int id,Lead model){if(id!=model.LeadId)return BadRequest();if(!ModelState.IsValid)return View(model);var e=await _db.Leads.FindAsync(id);if(e==null)return NotFound();e.LeadName=model.LeadName;e.Email=model.Email;e.Phone=model.Phone;e.CompanyName=model.CompanyName;e.Source=model.Source;e.Status=model.Status;e.Priority=model.Priority;e.ExpectedValue=model.ExpectedValue;await _db.SaveChangesAsync();await _audit.LogAsync("Update","Lead",id.ToString(),null,e.LeadName);TempData["Success"]="Lead updated.";return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Convert(int id)
    {
        var lead=await _db.Leads.FindAsync(id); if(lead==null)return NotFound();
        var existing=await _db.Customers.FirstOrDefaultAsync(x=>x.Email==lead.Email||x.Phone==lead.Phone);
        if(existing==null){existing=new Customer{CustomerCode="CUS-"+Random.Shared.Next(1000,9999),CustomerName=lead.LeadName,Email=lead.Email,Phone=lead.Phone,CompanyName=lead.CompanyName,CreatedBy=lead.AssignedTo,CreatedDate=DateTime.UtcNow};_db.Customers.Add(existing);await _db.SaveChangesAsync();}
        lead.Status="Converted"; await _db.SaveChangesAsync();
        await _audit.LogAsync("Convert","Lead",id.ToString(),"Qualified","Customer:"+existing.CustomerId);
        TempData["Success"]="Lead converted to customer."; return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)=>await _db.Leads.FindAsync(id) is Lead l?View(l):NotFound();
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id){var l=await _db.Leads.FindAsync(id);if(l==null)return NotFound();l.Status="Lost";await _db.SaveChangesAsync();await _audit.LogAsync("Delete/Close","Lead",id.ToString());return RedirectToAction(nameof(Index));}
}
