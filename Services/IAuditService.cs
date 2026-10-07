namespace AcxiomCRM.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string? recordId, string? oldValue = null, string? newValue = null);
}
