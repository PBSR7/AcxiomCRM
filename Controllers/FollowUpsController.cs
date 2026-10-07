using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly IAuditService _audit;
    public FollowUpsController(ApplicationDbContext db,IAuditService audit){_db=db;_audit=audit;}
    public async Task<IActionResult> Index(string? status){var q=_db.FollowUps.AsQueryable();if(!User.IsInRole("Admin")&&!User.IsInRole("Manager")){var uid=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;q=q.Where(x=>x.AssignedTo==uid);}if(!string.IsNullOrWhiteSpace(status))q=q.Where(x=>x.Status==status);ViewBag.Status=status;return View(await q.OrderBy(x=>x.FollowUpDate).ToListAsync());}
    public IActionResult Create()=>View(new FollowUp{FollowUpDate=DateTime.Now.AddDays(1)});
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Create(FollowUp model){if(model.FollowUpDate.Date<DateTime.UtcNow.Date&&model.Status=="Planned")ModelState.AddModelError(nameof(model.FollowUpDate),"Follow-up date cannot be earlier than today.");if(!ModelState.IsValid)return View(model);model.AssignedTo=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??"";_db.FollowUps.Add(model);await _db.SaveChangesAsync();await _audit.LogAsync("Create","FollowUp",model.FollowUpId.ToString(),null,model.Subject);TempData["Success"]="Follow-up scheduled.";return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Complete(int id){var f=await _db.FollowUps.FindAsync(id);if(f==null)return NotFound();f.Status="Completed";await _db.SaveChangesAsync();await _audit.LogAsync("Complete","FollowUp",id.ToString());return RedirectToAction(nameof(Index));}
}
