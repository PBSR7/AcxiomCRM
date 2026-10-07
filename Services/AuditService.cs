using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuditService(ApplicationDbContext db, IHttpContextAccessor http, UserManager<ApplicationUser> userManager)
    {
        _db = db; _http = http; _userManager = userManager;
    }

    public async Task LogAsync(string action, string entityName, string? recordId, string? oldValue = null, string? newValue = null)
    {
        var userId = _http.HttpContext?.User?.Identity?.IsAuthenticated == true
            ? _userManager.GetUserId(_http.HttpContext.User) : null;
        var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString();
        _db.AuditLogs.Add(new AuditLog { UserId = userId, Action = action, EntityName = entityName, RecordId = recordId, OldValue = oldValue, NewValue = newValue, IpAddress = ip });
        await _db.SaveChangesAsync();
    }
}
