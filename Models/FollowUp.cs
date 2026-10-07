using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class FollowUp
{
    public int FollowUpId { get; set; }
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    [Required, DataType(DataType.DateTime)] public DateTime FollowUpDate { get; set; }
    [Required] public string FollowUpType { get; set; } = "Call";
    [Required, StringLength(200)] public string Subject { get; set; } = string.Empty;
    [StringLength(500)] public string Remarks { get; set; } = string.Empty;
    [Required] public string Status { get; set; } = "Planned";
    public string AssignedTo { get; set; } = string.Empty;
}
