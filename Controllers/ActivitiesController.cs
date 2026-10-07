using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class ActivitiesController : Controller
{
    private readonly ApplicationDbContext _db; private readonly IAuditService _audit;
    public ActivitiesController(ApplicationDbContext db, IAuditService audit){_db=db;_audit=audit;}
    public async Task<IActionResult> Index(){var q=_db.Activities.AsQueryable();if(!User.IsInRole("Admin")&&!User.IsInRole("Manager")){var uid=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;q=q.Where(x=>x.AssignedTo==uid);}return View(await q.OrderByDescending(x=>x.ActivityDate).ToListAsync());}
    public IActionResult Create()=>View(new Activity{ActivityDate=DateTime.Now});
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Create(Activity model){if(!ModelState.IsValid)return View(model);model.AssignedTo=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??"";_db.Activities.Add(model);await _db.SaveChangesAsync();await _audit.LogAsync("Create","Activity",model.ActivityId.ToString(),null,model.Subject);TempData["Success"]="Activity recorded.";return RedirectToAction(nameof(Index));}
}
