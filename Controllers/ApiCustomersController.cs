using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class ApiCustomersController : ControllerBase
{
    private readonly ApplicationDbContext _db; public ApiCustomersController(ApplicationDbContext db)=>_db=db;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _db.Customers.Select(c=>new{c.CustomerId,c.CustomerCode,c.CustomerName,c.Email,c.Phone,c.CompanyName,c.Status,c.CreatedDate}).ToListAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id){var c=await _db.Customers.FindAsync(id);return c==null?NotFound():Ok(new{c.CustomerId,c.CustomerCode,c.CustomerName,c.Email,c.Phone,c.CompanyName,c.Status,c.CreatedDate});}
    [HttpPost] public async Task<IActionResult> Post(Customer model){if(!ModelState.IsValid)return ValidationProblem(ModelState);if(await _db.Customers.AnyAsync(c=>c.Email==model.Email||c.Phone==model.Phone))return Conflict(new{message="Customer email or phone already exists."});model.CustomerCode="CUS-"+Random.Shared.Next(1000,9999);model.CreatedDate=DateTime.UtcNow;model.CreatedBy=User.Identity?.Name??"api";_db.Customers.Add(model);await _db.SaveChangesAsync();return CreatedAtAction(nameof(Get),new{id=model.CustomerId},new{model.CustomerId,model.CustomerCode,model.CustomerName,model.Email,model.Phone,model.Status});}
    [HttpPut("{id:int}")] public async Task<IActionResult> Put(int id,Customer model){var c=await _db.Customers.FindAsync(id);if(c==null)return NotFound();if(!ModelState.IsValid)return ValidationProblem(ModelState);c.CustomerName=model.CustomerName;c.Email=model.Email;c.Phone=model.Phone;c.CompanyName=model.CompanyName;c.Status=model.Status;await _db.SaveChangesAsync();return Ok(new{message="Customer updated."});}
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){var c=await _db.Customers.FindAsync(id);if(c==null)return NotFound();c.Status="Inactive";await _db.SaveChangesAsync();return NoContent();}
}
