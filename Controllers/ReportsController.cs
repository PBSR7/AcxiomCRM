using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize(Roles="Admin,Manager")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _db; public ReportsController(ApplicationDbContext db)=>_db=db;
    public async Task<IActionResult> Index(){var opp=await _db.Opportunities.ToListAsync();ViewBag.TotalPipeline=opp.Where(x=>x.Status=="Open").Sum(x=>x.Amount);ViewBag.Weighted=opp.Where(x=>x.Status=="Open").Sum(x=>x.WeightedValue);ViewBag.Won=opp.Count(x=>x.Status=="Won");ViewBag.Conversion=await _db.Leads.CountAsync(x=>x.Status=="Converted");return View(opp);}
}
