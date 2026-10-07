using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[ApiController, Route("api/leads"), Authorize]
public class ApiLeadsController : ControllerBase
{
    private readonly ApplicationDbContext _db; public ApiLeadsController(ApplicationDbContext db)=>_db=db;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _db.Leads.Select(l=>new{l.LeadId,l.LeadCode,l.LeadName,l.Email,l.Phone,l.Source,l.Status,l.Priority,l.ExpectedValue}).ToListAsync());
    [HttpPost] public async Task<IActionResult> Post(Lead model){if(!ModelState.IsValid)return ValidationProblem(ModelState);_db.Leads.Add(model);await _db.SaveChangesAsync();return Created($"/api/leads/{model.LeadId}",new{model.LeadId,model.LeadName,model.Status});}
}
