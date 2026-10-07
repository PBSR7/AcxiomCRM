using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Activity
{
    public int ActivityId { get; set; }
    [Required] public string ActivityType { get; set; } = "Call";
    [Required, StringLength(160)] public string Subject { get; set; } = string.Empty;
    [StringLength(1000)] public string Description { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
}
