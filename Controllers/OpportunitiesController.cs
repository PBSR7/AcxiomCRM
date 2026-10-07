using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class OpportunitiesController : Controller
{
    private readonly ApplicationDbContext _db; private readonly IAuditService _audit;
    public OpportunitiesController(ApplicationDbContext db, IAuditService audit){_db=db;_audit=audit;}
    public async Task<IActionResult> Index(string? search,string? stage){var q=_db.Opportunities.Include(x=>x.Customer).AsQueryable();if(!User.IsInRole("Admin")&&!User.IsInRole("Manager")){var uid=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;q=q.Where(x=>x.AssignedTo==uid);}if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.OpportunityName.Contains(search));if(!string.IsNullOrWhiteSpace(stage))q=q.Where(x=>x.Stage==stage);ViewBag.Search=search;ViewBag.Stage=stage;return View(await q.OrderByDescending(x=>x.CreatedDate).ToListAsync());}
    public async Task<IActionResult>Create()=>View(new Opportunity{ExpectedCloseDate=DateTime.UtcNow.Date.AddDays(30),CustomerId=await _db.Customers.Select(x=>x.CustomerId).FirstOrDefaultAsync()});
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Create(Opportunity model){if(model.Amount<=0)ModelState.AddModelError(nameof(model.Amount),"Opportunity Amount must be greater than 0.");if(model.Probability<0||model.Probability>100)ModelState.AddModelError(nameof(model.Probability),"Probability must be between 0 and 100.");if(model.ExpectedCloseDate.Date<DateTime.UtcNow.Date&&model.Status=="Open")ModelState.AddModelError(nameof(model.ExpectedCloseDate),"Expected Close Date cannot be in the past.");if(!ModelState.IsValid)return View(model);model.CreatedDate=DateTime.UtcNow;model.AssignedTo=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??"";_db.Opportunities.Add(model);await _db.SaveChangesAsync();await _audit.LogAsync("Create","Opportunity",model.OpportunityId.ToString(),null,model.OpportunityName);TempData["Success"]="Opportunity created.";return RedirectToAction(nameof(Index));}
    public async Task<IActionResult>Edit(int id)=>await _db.Opportunities.FindAsync(id) is Opportunity o?View(o):NotFound();
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Edit(int id,Opportunity model){if(id!=model.OpportunityId)return BadRequest();if(model.Amount<=0)ModelState.AddModelError(nameof(model.Amount),"Opportunity Amount must be greater than 0.");if(model.Probability<0||model.Probability>100)ModelState.AddModelError(nameof(model.Probability),"Probability must be between 0 and 100.");if(model.ExpectedCloseDate.Date<DateTime.UtcNow.Date&&model.Status=="Open")ModelState.AddModelError(nameof(model.ExpectedCloseDate),"Expected Close Date cannot be in the past.");if(!ModelState.IsValid)return View(model);var e=await _db.Opportunities.FindAsync(id);if(e==null)return NotFound();e.OpportunityName=model.OpportunityName;e.CustomerId=model.CustomerId;e.Amount=model.Amount;e.Stage=model.Stage;e.Probability=model.Probability;e.ExpectedCloseDate=model.ExpectedCloseDate;e.Status=model.Status;e.Notes=model.Notes;await _db.SaveChangesAsync();await _audit.LogAsync("Update","Opportunity",id.ToString(),null,e.OpportunityName);TempData["Success"]="Opportunity updated.";return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult>Delete(int id){var e=await _db.Opportunities.FindAsync(id);if(e==null)return NotFound();e.Status="Lost";e.Stage="Lost";await _db.SaveChangesAsync();await _audit.LogAsync("Delete/Close","Opportunity",id.ToString());return RedirectToAction(nameof(Index));}
}
