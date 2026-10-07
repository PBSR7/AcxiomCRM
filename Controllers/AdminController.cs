using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize(Roles="Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _users; private readonly IAuditService _audit; private readonly ApplicationDbContext _db;
    public AdminController(UserManager<ApplicationUser> users,IAuditService audit,ApplicationDbContext db){_users=users;_audit=audit;_db=db;}
    public async Task<IActionResult> Users(){var users=await _users.Users.ToListAsync();var rows=new List<UserRow>();foreach(var u in users)rows.Add(new UserRow(u, string.Join(", ", await _users.GetRolesAsync(u))));return View(rows);}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(string id){var u=await _users.FindByIdAsync(id);if(u==null)return NotFound();u.LockoutEnabled=true;u.LockoutEnd=u.LockoutEnd==null?DateTimeOffset.UtcNow.AddYears(10):null;await _users.UpdateAsync(u);await _audit.LogAsync("User Status Change","User",id,null,u.LockoutEnd==null?"Active":"Inactive");return RedirectToAction(nameof(Users));}
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRole(string id, string role)
    {
        if (!new[] { "Admin", "Manager", "Sales Executive" }.Contains(role)) return BadRequest();
        var u = await _users.FindByIdAsync(id); if (u == null) return NotFound();
        var current = await _users.GetRolesAsync(u); if (current.Count > 0) await _users.RemoveFromRolesAsync(u, current);
        await _users.AddToRoleAsync(u, role); await _audit.LogAsync("Role Change", "User", id, string.Join(",", current), role);
        return RedirectToAction(nameof(Users));
    }
    public async Task<IActionResult> Audit(){return View(await _db.AuditLogs.OrderByDescending(x=>x.CreatedDate).Take(250).ToListAsync());}
}
public record UserRow(ApplicationUser User,string Roles);
