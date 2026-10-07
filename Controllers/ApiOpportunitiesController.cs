using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[ApiController, Route("api/opportunities"), Authorize]
public class ApiOpportunitiesController : ControllerBase
{
    private readonly ApplicationDbContext _db; public ApiOpportunitiesController(ApplicationDbContext db)=>_db=db;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _db.Opportunities.Select(o=>new{o.OpportunityId,o.OpportunityName,o.CustomerId,o.Amount,o.Stage,o.Probability,o.ExpectedCloseDate,o.Status,WeightedValue=o.Amount*o.Probability/100m}).ToListAsync());
    [HttpPost] public async Task<IActionResult> Post(Opportunity model){if(model.Amount<=0||model.Probability<0||model.Probability>100)return BadRequest(new{message="Amount must be > 0 and probability must be 0-100."});if(model.ExpectedCloseDate.Date<DateTime.UtcNow.Date&&model.Status=="Open")return BadRequest(new{message="Expected Close Date cannot be in the past."});if(!ModelState.IsValid)return ValidationProblem(ModelState);_db.Opportunities.Add(model);await _db.SaveChangesAsync();return Created($"/api/opportunities/{model.OpportunityId}",new{model.OpportunityId,model.OpportunityName,model.Amount,model.Probability,model.Stage});}
}
